using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace DVSRegister.UnitTests.Repository
{
    [Collection("Postgres Collection")]
    public class RemoveProvider2iRepositoryTests : IAsyncLifetime
    {
        private readonly PostgresTestFixture fixture;
        private readonly ILogger<RemoveProvider2iRepository> logger;

        public RemoveProvider2iRepositoryTests(PostgresTestFixture fixture)
        {
            this.fixture = fixture;
            logger = Substitute.For<ILogger<RemoveProvider2iRepository>>();
        }

        public Task InitializeAsync() => fixture.ResetAsync();

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task ApproveServiceRemoval_ValidRequest_UsesServiceRemovedTimeForRegisterHistory()
        {
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);
            int serviceId = await SaveServiceAsync(dbContext, ServiceStatusEnum.AwaitingRemovalConfirmation, true);
            Service serviceWithAssets = await dbContext.Service.SingleAsync(entry => entry.Id == serviceId);
            AddTrustmarkAssets(dbContext, serviceWithAssets, 1001);
            var removalRequest = new ServiceRemovalRequest
            {
                ServiceId = serviceId,
                IsRequestPending = true,
                RemovalRequestTime = DateTime.UtcNow.AddDays(-1),
                PreviousServiceStatus = ServiceStatusEnum.Published
            };
            dbContext.ServiceRemovalRequest.Add(removalRequest);
            await dbContext.SaveChangesAsync();

            var response = await repository.ApproveServiceRemoval(serviceId, removalRequest.Id, "test.user@dsit.gov.com");

            Service service = await dbContext.Service.SingleAsync(entry => entry.Id == serviceId);
            ServiceRemovalRequest savedRequest = await dbContext.ServiceRemovalRequest
                .SingleAsync(entry => entry.Id == removalRequest.Id);
            PublishedRegisterEntryRevision revision = await dbContext.PublishedRegisterEntryRevisions
                .SingleAsync(entry => entry.ServiceId == serviceId);
            Assert.True(response.Success);
            Assert.NotNull(service.RemovedTime);
            Assert.Equal(service.RemovedTime, savedRequest.RemovedTime);
            Assert.Equal(service.RemovedTime.Value.Ticks / 10, revision.RemovedOn!.Value.Ticks / 10);
            Assert.False((await dbContext.TrustmarkNumber.SingleAsync(entry => entry.ServiceId == serviceId)).IsActive);
            Assert.Empty(dbContext.DownloadLogoToken);
        }

        [Fact]
        public async Task DetailAndTokenQueries_ReturnExpectedGraphsAndFallbacks()
        {
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);
            ProviderProfile provider = await SaveProviderAsync(dbContext, true);
            Service service = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 2001, true);
            var providerRequest = new ProviderRemovalRequest
            {
                ProviderProfileId = provider.Id,
                Token = "provider-token",
                TokenId = "provider-token-id",
                IsRequestPending = true,
                PreviousProviderStatus = ProviderStatusEnum.NA,
                ProviderRemovalRequestServiceMapping =
                [
                    new ProviderRemovalRequestServiceMapping
                    {
                        ServiceId = service.Id,
                        PreviousServiceStatus = ServiceStatusEnum.Published
                    }
                ]
            };
            var serviceRequest = new ServiceRemovalRequest
            {
                ServiceId = service.Id,
                Token = "service-token",
                TokenId = "service-token-id",
                IsRequestPending = true,
                PreviousServiceStatus = ServiceStatusEnum.Published
            };
            dbContext.AddRange(providerRequest, serviceRequest);
            await dbContext.SaveChangesAsync();

            Assert.Equal(provider.Id, (await repository.GetRemoveProviderToken("provider-token", "provider-token-id")).ProviderProfileId);
            Assert.Equal(service.Id, (await repository.GetRemoveServiceToken("service-token", "service-token-id")).ServiceId);
            Assert.Equal(service.Id, (await repository.GetProviderDetailsWithService("service-token", "service-token-id")).ServiceId);
            Assert.Equal(provider.Id, (await repository.GetProviderDetailsWithRemovedServices(provider.Id, [service.Id])).Id);
            Assert.Single((await repository.GetProviderDetails(provider.Id)).Services!);
            Assert.Equal(service.Id, (await repository.GetServiceDetailsWithProvider(service.Id)).Id);
            Assert.Equal(service.Id, (await repository.GetServiceDetails(service.Id)).Id);
            Assert.Null(await repository.GetRemoveProviderToken("missing", "missing"));
            Assert.Null(await repository.GetRemoveServiceToken("missing", "missing"));
            Assert.Null(await repository.GetProviderDetailsWithService("missing", "missing"));
            Assert.Equal(0, (await repository.GetProviderDetails(int.MaxValue)).Id);
            Assert.Equal(0, (await repository.GetServiceDetailsWithProvider(int.MaxValue)).Id);
            Assert.Equal(0, (await repository.GetServiceDetails(int.MaxValue)).Id);
        }

        [Fact]
        public async Task ApproveProviderRemoval_ValidRequest_UpdatesProviderServicesAndPendingWork()
        {
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);
            ProviderProfile provider = await SaveProviderAsync(dbContext, true, ProviderStatusEnum.UpdatesRequested);
            Service updatesService = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 3001);
            Service displayService = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 3002);
            await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 3003);
            Service transferService = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 3004);
            int userId = await dbContext.User.Select(user => user.Id).FirstAsync();
            AddTrustmarkAssets(dbContext, updatesService, 3001);
            dbContext.ProviderProfileDraft.Add(new ProviderProfileDraft
            {
                ProviderProfileId = provider.Id,
                RequestedUserId = userId,
                PreviousProviderStatus = ProviderStatusEnum.NA,
                ModifiedTime = DateTime.UtcNow
            });
            dbContext.ServiceDraft.Add(new ServiceDraft
            {
                ServiceId = updatesService.Id,
                ProviderProfileId = provider.Id,
                RequestedUserId = userId,
                PreviousServiceStatus = ServiceStatusEnum.Published,
                ModifiedTime = DateTime.UtcNow
            });
            dbContext.ServiceCustomDisplayChangeRequest.Add(new ServiceCustomDisplayChangeRequest
            {
                ServiceId = displayService.Id,
                RequestedUserId = userId,
                RequestedTime = DateTime.UtcNow,
                IsRequestPending = true,
                OldValue = System.Text.Json.JsonDocument.Parse("{}"),
                NewValue = System.Text.Json.JsonDocument.Parse("{}"),
                HiddenValue = System.Text.Json.JsonDocument.Parse("{}")
            });
            var requestManagement = new RequestManagement
            {
                Id = Guid.NewGuid().ToString(),
                InitiatedUserId = userId,
                CabId = 1,
                RequestType = RequestTypeEnum.CabTransfer,
                RequestStatus = RequestStatusEnum.AwaitingRemoval,
                ModifiedTime = DateTime.UtcNow
            };
            dbContext.CabTransferRequest.Add(new CabTransferRequest
            {
                ServiceId = transferService.Id,
                FromCabUserId = 1,
                ToCabId = 1,
                PreviousServiceStatus = ServiceStatusEnum.Published,
                RequestManagement = requestManagement,
                DecisionTime = DateTime.UtcNow
            });
            var request = new ProviderRemovalRequest
            {
                ProviderProfileId = provider.Id,
                IsRequestPending = true,
                Token = "token",
                TokenId = "token-id",
                PreviousProviderStatus = ProviderStatusEnum.UpdatesRequested,
                ProviderRemovalRequestServiceMapping =
                [
                    new ProviderRemovalRequestServiceMapping
                    {
                        ServiceId = updatesService.Id,
                        PreviousServiceStatus = ServiceStatusEnum.UpdatesRequested
                    },
                    new ProviderRemovalRequestServiceMapping
                    {
                        ServiceId = displayService.Id,
                        PreviousServiceStatus = ServiceStatusEnum.DisplayChangeRequested
                    },
                    new ProviderRemovalRequestServiceMapping
                    {
                        ServiceId = transferService.Id,
                        PreviousServiceStatus = ServiceStatusEnum.PublishedUnderReassign
                    }
                ]
            };
            dbContext.ProviderRemovalRequest.Add(request);
            await dbContext.SaveChangesAsync();

            GenericResponse response = await repository.ApproveProviderRemoval(provider.Id, request.Id,
                "test.user@dsit.gov.com");

            Assert.True(response.Success);
            Assert.Empty(dbContext.ProviderProfileDraft);
            Assert.Empty(dbContext.ServiceDraft);
            Assert.False((await dbContext.ServiceCustomDisplayChangeRequest.SingleAsync()).IsRequestPending);
            Assert.False((await dbContext.TrustmarkNumber.SingleAsync()).IsActive);
            Assert.Empty(dbContext.DownloadLogoToken);
            Assert.Equal(RequestStatusEnum.Removed,
                (await dbContext.RequestManagement.SingleAsync(entry => entry.Id == requestManagement.Id)).RequestStatus);
            Assert.Equal(4, await dbContext.PublishedRegisterEntryRevisions.CountAsync());
            Assert.All(await dbContext.Service.Where(entry => entry.ProviderProfileId == provider.Id).ToListAsync(),
                entry => Assert.NotNull(entry.RemovedTime));
        }

        [Fact]
        public async Task CancelRemoveProviderRequest_ValidRequest_RestoresServiceRequestsAndTransfer()
        {
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);
            ProviderProfile provider = await SaveProviderAsync(dbContext, true);
            Service pendingService = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 4001);
            Service transferService = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 4002);
            int userId = await dbContext.User.Select(user => user.Id).FirstAsync();
            var bulkRequest = new ServiceBulkRemovalRequest
            {
                Comment = "Bulk",
                IsRequestPending = false,
                RequestedTime = DateTime.UtcNow,
                RequestedBy = userId
            };
            dbContext.ServiceRemovalRequest.Add(new ServiceRemovalRequest
            {
                ServiceId = pendingService.Id,
                IsRequestPending = false,
                PreviousServiceStatus = ServiceStatusEnum.Published,
                ServiceBulkRemovalRequest = bulkRequest
            });
            var requestManagement = new RequestManagement
            {
                Id = Guid.NewGuid().ToString(),
                InitiatedUserId = userId,
                CabId = 1,
                RequestType = RequestTypeEnum.CabTransfer,
                RequestStatus = RequestStatusEnum.AwaitingRemoval,
                ModifiedTime = DateTime.UtcNow
            };
            dbContext.CabTransferRequest.Add(new CabTransferRequest
            {
                ServiceId = transferService.Id,
                FromCabUserId = 1,
                ToCabId = 1,
                PreviousServiceStatus = ServiceStatusEnum.Published,
                RequestManagement = requestManagement,
                DecisionTime = DateTime.UtcNow
            });
            var request = new ProviderRemovalRequest
            {
                ProviderProfileId = provider.Id,
                IsRequestPending = true,
                PreviousProviderStatus = ProviderStatusEnum.NA,
                ProviderRemovalRequestServiceMapping =
                [
                    new ProviderRemovalRequestServiceMapping
                    {
                        ServiceId = pendingService.Id,
                        PreviousServiceStatus = ServiceStatusEnum.CabAwaitingRemovalConfirmation
                    },
                    new ProviderRemovalRequestServiceMapping
                    {
                        ServiceId = transferService.Id,
                        PreviousServiceStatus = ServiceStatusEnum.PublishedUnderReassign,
                        PreviousCabTransferStatus = RequestStatusEnum.Approved
                    }
                ]
            };
            dbContext.ProviderRemovalRequest.Add(request);
            await dbContext.SaveChangesAsync();

            GenericResponse response = await repository.CancelRemoveProviderRequest(provider.Id, request.Id,
                "test.user@dsit.gov.com");

            Assert.True(response.Success);
            Assert.True((await dbContext.ServiceRemovalRequest.SingleAsync()).IsRequestPending);
            Assert.True((await dbContext.ServiceBulkRemovalRequest.SingleAsync()).IsRequestPending);
            Assert.Equal(RequestStatusEnum.Approved,
                (await dbContext.RequestManagement.SingleAsync(entry => entry.Id == requestManagement.Id)).RequestStatus);
            Assert.Empty(dbContext.Set<ProviderRemovalRequestServiceMapping>());
        }

        [Fact]
        public async Task RemovalCommands_InvalidIdentifiers_ReturnFailure()
        {
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);
            ProviderProfile provider = await SaveProviderAsync(dbContext, true);
            Service service = await SaveServiceEntityAsync(dbContext, provider.Id, ServiceStatusEnum.Published,
                true, 5001);
            var serviceRequest = new ServiceRemovalRequest
            {
                ServiceId = service.Id,
                IsRequestPending = true,
                PreviousServiceStatus = ServiceStatusEnum.Published
            };
            dbContext.ServiceRemovalRequest.Add(serviceRequest);
            await dbContext.SaveChangesAsync();

            Assert.False((await repository.ApproveProviderRemoval(provider.Id, int.MaxValue, "user")).Success);
            Assert.False((await repository.CancelRemoveProviderRequest(provider.Id, int.MaxValue, "user")).Success);
            Assert.False((await repository.ApproveServiceRemoval(service.Id, serviceRequest.Id, "user")).Success);
            Assert.False((await repository.CancelRemoveServiceRequest(service.Id, serviceRequest.Id, "user")).Success);
            Assert.False((await repository.ApproveServiceRemoval(int.MaxValue, int.MaxValue, "user")).Success);
            Assert.False((await repository.CancelRemoveServiceRequest(int.MaxValue, int.MaxValue, "user")).Success);
        }

        [Fact]
        public async Task CancelRemoveServiceRequest_ValidRequest_RestoresPublishedStatus()
        {
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);
            ProviderProfile provider = await SaveProviderAsync(dbContext, true);
            Service service = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 6001);
            var request = new ServiceRemovalRequest
            {
                ServiceId = service.Id,
                IsRequestPending = true,
                Token = "token",
                TokenId = "token-id",
                PreviousServiceStatus = ServiceStatusEnum.Published
            };
            dbContext.ServiceRemovalRequest.Add(request);
            await dbContext.SaveChangesAsync();

            GenericResponse response = await repository.CancelRemoveServiceRequest(service.Id, request.Id,
                "test.user@dsit.gov.com");

            Assert.True(response.Success);
            Assert.Equal(ServiceStatusEnum.Published, service.ServiceStatus);
            Assert.False(request.IsRequestPending);
            Assert.Null(request.Token);
            Assert.Null(request.TokenId);
        }

        [Fact]
        public async Task CancelRemoveServiceRequest_SaveFails_ReturnsFailureAndLogsError()
        {
            int serviceId;
            int requestId;
            await using (var setupContext = CreateDbContext())
            {
                ProviderProfile provider = await SaveProviderAsync(setupContext, true);
                Service service = await SaveServiceEntityAsync(setupContext, provider.Id,
                    ServiceStatusEnum.AwaitingRemovalConfirmation, true, 7001);
                var request = new ServiceRemovalRequest
                {
                    ServiceId = service.Id,
                    IsRequestPending = true,
                    PreviousServiceStatus = ServiceStatusEnum.Published
                };
                setupContext.ServiceRemovalRequest.Add(request);
                await setupContext.SaveChangesAsync();
                serviceId = service.Id;
                requestId = request.Id;
            }

            await using var failingContext = CreateThrowingDbContext();
            var repository = new RemoveProvider2iRepository(failingContext, logger);

            GenericResponse response = await repository.CancelRemoveServiceRequest(serviceId, requestId, "user");

            Assert.False(response.Success);
            Assert.Single(logger.ReceivedCalls().Where(call => call.GetMethodInfo().Name == nameof(ILogger.Log)));
        }

        private DVSRegisterDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<DVSRegisterDbContext>()
                .UseNpgsql(fixture.GetConnectionString())
                .Options;
            return new DVSRegisterDbContext(options);
        }

        private ThrowingDVSRegisterDbContext CreateThrowingDbContext()
        {
            var options = new DbContextOptionsBuilder<DVSRegisterDbContext>()
                .UseNpgsql(fixture.GetConnectionString())
                .Options;
            return new ThrowingDVSRegisterDbContext(options);
        }

        private static async Task<ProviderProfile> SaveProviderAsync(DVSRegisterDbContext dbContext,
            bool isInRegister, ProviderStatusEnum status = ProviderStatusEnum.AwaitingRemovalConfirmation)
        {
            ProviderProfile provider = RepositoryTestHelper.CreateProviderProfile(1, "Removal test provider");
            provider.IsInRegister = isInRegister;
            provider.ProviderStatus = status;
            dbContext.ProviderProfile.Add(provider);
            await dbContext.SaveChangesAsync();
            return provider;
        }

        private static async Task<Service> SaveServiceEntityAsync(DVSRegisterDbContext dbContext,
            int providerId, ServiceStatusEnum status, bool isInRegister, int serviceKey, bool richMappings = false)
        {
            Service service = RepositoryTestHelper.CreateService(1, $"Removal test service {serviceKey}", providerId,
                status, richMappings, richMappings, richMappings, serviceKey);
            service.IsInRegister = isInRegister;
            dbContext.Service.Add(service);
            await dbContext.SaveChangesAsync();
            return service;
        }

        private static void AddTrustmarkAssets(DVSRegisterDbContext dbContext, Service service, int serviceKey)
        {
            service.TrustmarkNumber = new TrustmarkNumber
            {
                ProviderProfileId = service.ProviderProfileId,
                ServiceId = service.Id,
                CompanyId = 2000 + service.ProviderProfileId,
                ServiceNumber = serviceKey % 99 + 1,
                ServiceKey = serviceKey,
                TrustMarkNumber = $"TM-{serviceKey}",
                TrustMarkID = $"ID-{serviceKey}",
                PngLogoLink = "logo.png",
                JpegLogoLink = "logo.jpg",
                SvgLogoLink = "logo.svg",
                TrustMarkNumberVerified = true,
                LogoVerified = true,
                IsActive = true,
                TimeStamp = DateTime.UtcNow
            };
            service.DownloadLogoToken = new DownloadLogoToken
            {
                Id = Guid.NewGuid().ToString(),
                TokenId = $"token-id-{serviceKey}",
                Token = $"token-{serviceKey}",
                ServiceId = service.Id,
                CreatedTime = DateTime.UtcNow
            };
            dbContext.Add(service.TrustmarkNumber);
            dbContext.Add(service.DownloadLogoToken);
        }

        private sealed class ThrowingDVSRegisterDbContext(DbContextOptions<DVSRegisterDbContext> options)
            : DVSRegisterDbContext(options)
        {
            public override Task<int> SaveChangesAsync(TeamEnum team = TeamEnum.NA,
                EventTypeEnum eventType = EventTypeEnum.NA, string actorId = null!) =>
                throw new InvalidOperationException("Simulated database failure");
        }

        private static async Task<int> SaveServiceAsync(DVSRegisterDbContext dbContext,
            ServiceStatusEnum status, bool isInRegister)
        {
            ProviderProfile provider = RepositoryTestHelper.CreateProviderProfile(1, "Removal test provider");
            provider.IsInRegister = isInRegister;
            provider.ProviderStatus = ProviderStatusEnum.AwaitingRemovalConfirmation;
            dbContext.ProviderProfile.Add(provider);
            await dbContext.SaveChangesAsync();

            Service service = RepositoryTestHelper.CreateService(1, "Removal test service", provider.Id,
                status, false, false, false, 1001);
            service.IsInRegister = isInRegister;
            dbContext.Service.Add(service);
            await dbContext.SaveChangesAsync();
            return service.Id;
        }
    }
}
