using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.CabTransfer;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace DVSRegister.UnitTests.Repository
{
    // Existing workflows must keep their behaviour and keep the one stable ServiceGuid per logical service
    [Collection("Postgres Collection")]
    public class ServiceGuidPreservationTests(PostgresTestFixture fixture) : IAsyncLifetime
    {
        public Task InitializeAsync() => fixture.ResetAsync();

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task ApproveServiceRemoval_KeepsServiceGuidAndUpdatesStatusFlags()
        {
            await using var dbContext = fixture.CreateDbContext();
            var (provider, published, _) = await RepositoryTestHelper.SeedPublishedWithNewerDraftAsync(dbContext);
            Guid serviceGuid = published.ServiceGuid;
            Guid versionGuid = published.Guid;
            published.ServiceStatus = ServiceStatusEnum.AwaitingRemovalConfirmation;
            var removalRequest = new ServiceRemovalRequest
            {
                ServiceId = published.Id,
                IsRequestPending = true,
                RemovalRequestTime = DateTime.UtcNow.AddDays(-1),
                PreviousServiceStatus = ServiceStatusEnum.Published
            };
            dbContext.ServiceRemovalRequest.Add(removalRequest);
            await dbContext.SaveChangesAsync();
            var repository = new RemoveProvider2iRepository(dbContext, Substitute.For<ILogger<RemoveProvider2iRepository>>());

            var response = await repository.ApproveServiceRemoval(published.Id, removalRequest.Id, "test.user@dsit.gov.com");

            Service removed = await fixture.CreateDbContext().Service.AsNoTracking().SingleAsync(s => s.Id == published.Id);
            Assert.True(response.Success);
            Assert.Equal(ServiceStatusEnum.Removed, removed.ServiceStatus);
            Assert.False(removed.IsInRegister);
            Assert.Equal(serviceGuid, removed.ServiceGuid);
            Assert.Equal(versionGuid, removed.Guid);
            Assert.Equal(provider.Id, removed.ProviderProfileId);
        }

        [Fact]
        public async Task ApproveCabTransfer_KeepsServiceGuid_DeletesInProgressVersion_AndAllowsReuseOfVersionNumber()
        {
            await using var dbContext = fixture.CreateDbContext();
            var (provider, published, draft) = await RepositoryTestHelper.SeedPublishedWithNewerDraftAsync(dbContext);
            Guid serviceGuid = published.ServiceGuid;
            int draftVersion = draft.ServiceVersion;
            int serviceKey = published.ServiceKey;

            published.ServiceStatus = ServiceStatusEnum.PublishedUnderReassign;
            CabUser toCabUser = await dbContext.CabUser.SingleAsync(u => u.CabEmail == "test.user@dsit.gov.com");
            toCabUser.AccountStatus = AccountStatusEnum.Active;
            User initiator = await dbContext.User.FirstAsync();
            var transfer = new CabTransferRequest
            {
                ServiceId = published.Id,
                FromCabUserId = published.CabUserId,
                ToCabId = toCabUser.CabId,
                PreviousServiceStatus = ServiceStatusEnum.Published,
                RequestManagement = new RequestManagement
                {
                    Id = Guid.NewGuid().ToString(),
                    InitiatedUserId = initiator.Id,
                    CabId = toCabUser.CabId,
                    RequestType = RequestTypeEnum.CabTransfer,
                    RequestStatus = RequestStatusEnum.Pending,
                    ModifiedTime = DateTime.UtcNow
                }
            };
            dbContext.CabTransferRequest.Add(transfer);
            await dbContext.SaveChangesAsync();
            var transferRepository = new CabTransferRepository(dbContext, Substitute.For<ILogger<CabTransferRepository>>());

            var response = await transferRepository.ApproveOrCancelTransferRequest(true, transfer.Id, provider.Id, "test.user@dsit.gov.com");

            await using var verifyContext = fixture.CreateDbContext();
            var rows = await verifyContext.Service.AsNoTracking().Where(s => s.ServiceKey == serviceKey).ToListAsync();
            Assert.True(response.Success);
            Service transferred = Assert.Single(rows);
            Assert.Equal(published.Id, transferred.Id);
            Assert.Equal(serviceGuid, transferred.ServiceGuid);
            Assert.Equal(toCabUser.Id, transferred.CabUserId);
            Assert.True(transferred.IsCurrent);

            // The deleted in-progress version number is reused by the next reapplication and keeps the same ServiceGuid
            var cabRepository = new CabRepository(verifyContext, Substitute.For<ILogger<CabRepository>>());
            Service reapplication = RepositoryTestHelper.CreateService(toCabUser.Id, "reapplication after transfer", provider.Id,
                ServiceStatusEnum.SavedAsDraft, null, null, null, serviceKey);
            reapplication.ServiceRoleMapping = [new() { RoleId = 1 }];
            var reapplicationResponse = await cabRepository.SaveServiceReApplication(reapplication, "test.user@dsit.gov.com", false, null);

            Service reused = await verifyContext.Service.AsNoTracking().SingleAsync(s => s.Id == reapplicationResponse.InstanceId);
            Assert.True(reapplicationResponse.Success);
            Assert.Equal(draftVersion, reused.ServiceVersion);
            Assert.Equal(serviceGuid, reused.ServiceGuid);
        }

        [Fact]
        public async Task SaveServiceAmendments_KeepsServiceGuid()
        {
            await using var dbContext = fixture.CreateDbContext();
            var cabRepository = new CabRepository(dbContext, Substitute.For<ILogger<CabRepository>>());
            ProviderProfile providerProfile = RepositoryTestHelper.CreateProviderProfile(1, "amendments provider");
            int providerId = (await cabRepository.SaveProviderProfile(providerProfile, "test@test.com")).InstanceId;
            var saved = await cabRepository.SaveService(
                RepositoryTestHelper.CreateService(1, "amend me", providerId, ServiceStatusEnum.AmendmentsRequired, true, true, true, 0), "test.user123@test.com");
            Service original = await dbContext.Service.AsNoTracking().SingleAsync(s => s.Id == saved.InstanceId);
            dbContext.CertificateReview.Add(RepositoryTestHelper.CreateFailedCertificateReview(1, saved.InstanceId, providerId,
                CertificateReviewEnum.AmendmentsRequired, "comments", "incorrect", "", "amendments needed"));
            await dbContext.SaveChangesAsync();

            Service amended = RepositoryTestHelper.CreateService(1, "amended", providerId, ServiceStatusEnum.AmendmentsRequired, null, null, null, original.ServiceKey);
            amended.Id = saved.InstanceId;
            var response = await cabRepository.SaveServiceAmendments(amended, "test.user123@test.com");

            Service after = await dbContext.Service.AsNoTracking().SingleAsync(s => s.Id == saved.InstanceId);
            Assert.True(response.Success);
            Assert.Equal("amended", after.ServiceName);
            Assert.Equal(original.ServiceGuid, after.ServiceGuid);
        }

        [Fact]
        public async Task SaveServiceReApplication_MultipleVersions_AllShareOneServiceGuid()
        {
            await using var dbContext = fixture.CreateDbContext();
            var cabRepository = new CabRepository(dbContext, Substitute.For<ILogger<CabRepository>>());
            ProviderProfile providerProfile = RepositoryTestHelper.CreateProviderProfile(1, "versions provider");
            int providerId = (await cabRepository.SaveProviderProfile(providerProfile, "test@test.com")).InstanceId;
            var first = await cabRepository.SaveService(
                RepositoryTestHelper.CreateService(1, "v1", providerId, ServiceStatusEnum.Submitted, true, true, true, 0), "test.user123@test.com");
            Service version1 = await dbContext.Service.AsNoTracking().SingleAsync(s => s.Id == first.InstanceId);

            foreach (string name in new[] { "v2", "v3" })
            {
                Service next = RepositoryTestHelper.CreateService(1, name, providerId, ServiceStatusEnum.Submitted, null, null, null, version1.ServiceKey);
                next.ServiceRoleMapping = [new() { RoleId = 1 }];
                Assert.True((await cabRepository.SaveServiceReApplication(next, "test.user123@test.com", false, null)).Success);
            }

            var versions = await dbContext.Service.AsNoTracking().Where(s => s.ServiceKey == version1.ServiceKey).ToListAsync();
            Assert.Equal(3, versions.Count);
            Assert.Single(versions.Select(v => v.ServiceGuid).Distinct());
            Assert.Equal(3, versions.Select(v => v.Guid).Distinct().Count());
        }
    }
}
