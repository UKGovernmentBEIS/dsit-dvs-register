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

        [Fact]
        public async Task GetProviderDetails_WithServiceQualityLevelMapping_IncludesQualityMapping()
        {
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);
            ProviderProfile provider = await SaveProviderAsync(dbContext, true);
            Service service = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 8001, richMappings: true);

            ProviderProfile result = await repository.GetProviderDetails(provider.Id);

            Assert.NotEmpty(result.Services!);
            Service resultService = result.Services!.First();
            Assert.NotNull(resultService.ServiceQualityLevelMapping);
            Assert.NotEmpty(resultService.ServiceQualityLevelMapping);
            // Verify RemovedTime is not set for pending removal services
            Assert.Null(resultService.RemovedTime);
        }

        [Fact]
        public async Task GetProviderDetails_WithServiceSupSchemeMapping_IncludesSchemeMappings()
        {
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);
            ProviderProfile provider = await SaveProviderAsync(dbContext, true);
            Service service = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 8002, richMappings: true);

            ProviderProfile result = await repository.GetProviderDetails(provider.Id);

            Assert.NotEmpty(result.Services!);
            Service resultService = result.Services!.First();
            Assert.NotNull(resultService.ServiceSupSchemeMapping);
            Assert.NotEmpty(resultService.ServiceSupSchemeMapping);
            // Verify RemovedTime is not set for pending removal services
            Assert.Null(resultService.RemovedTime);
        }

        [Fact]
        public async Task GetProviderDetails_WithServiceIdentityProfileMapping_IncludesIdentityMapping()
        {
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);
            ProviderProfile provider = await SaveProviderAsync(dbContext, true);
            Service service = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 8003, richMappings: true);

            ProviderProfile result = await repository.GetProviderDetails(provider.Id);

            Assert.NotEmpty(result.Services!);
            Service resultService = result.Services!.First();
            Assert.NotNull(resultService.ServiceIdentityProfileMapping);
            Assert.NotEmpty(resultService.ServiceIdentityProfileMapping);
            // Verify RemovedTime is not set for pending removal services
            Assert.Null(resultService.RemovedTime);
        }

        [Fact]
        public async Task GetProviderDetails_WithNoOptionalMappings_ReturnsServiceWithoutOptionalIncludes()
        {
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);
            ProviderProfile provider = await SaveProviderAsync(dbContext, true);
            Service service = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 8004, richMappings: false);

            ProviderProfile result = await repository.GetProviderDetails(provider.Id);

            Assert.NotEmpty(result.Services!);
            Service resultService = result.Services!.First();
            Assert.Empty(resultService.ServiceQualityLevelMapping ?? []);
            Assert.Empty(resultService.ServiceSupSchemeMapping ?? []);
            Assert.Empty(resultService.ServiceIdentityProfileMapping ?? []);
            // Verify RemovedTime is not set for pending removal services
            Assert.Null(resultService.RemovedTime);
            // NOTE: Provider.RemovedTime is NULL only because this provider was NEVER removed.
            // A provider CAN have RemovedTime even with pending service removal requests.
            // See: ApproveServiceRemoval_PreviouslyRemovedProvider_UpdatesProviderRemovedTimeWhenLastServiceRemoved
            Assert.Null(result.RemovedTime);
        }

        [Fact]
        public async Task GetProviderDetails_ProviderRemovedTimeAlreadySet_IsNotClearedByReadQuery()
        {
            // NOTE: this state (a removed provider carrying a pending service removal request) is
            // constructed directly against the database rather than via the app's publish/removal
            // flows, since PublicInterestCheckRepository.UpdateServiceStatus resets
            // Provider.RemovedTime to null whenever a service is (re)published. This test is a
            // defensive regression guard confirming the read-only GetProviderDetails query never
            // clears or recomputes Provider.RemovedTime - it is not asserting a state reachable
            // through the UI.
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);

            // Step 1: Create provider and service, remove provider
            ProviderProfile provider = await SaveProviderAsync(dbContext, true);
            Service service1 = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 8005);

            var providerRemovalRequest = new ProviderRemovalRequest
            {
                ProviderProfileId = provider.Id,
                IsRequestPending = true,
                PreviousProviderStatus = ProviderStatusEnum.NA,
                ProviderRemovalRequestServiceMapping =
                [
                    new ProviderRemovalRequestServiceMapping
                    {
                        ServiceId = service1.Id,
                        PreviousServiceStatus = ServiceStatusEnum.Published
                    }
                ]
            };
            dbContext.ProviderRemovalRequest.Add(providerRemovalRequest);
            await dbContext.SaveChangesAsync();

            // Approve provider removal - Provider now has RemovedTime
            await repository.ApproveProviderRemoval(provider.Id, providerRemovalRequest.Id, "user@test.com");

            ProviderProfile removedProvider = await dbContext.ProviderProfile.SingleAsync(p => p.Id == provider.Id);
            DateTime? providerRemovedTime = removedProvider.RemovedTime;
            Assert.NotNull(providerRemovedTime);

            // Step 2: Directly insert a second service with a pending removal request against the
            // already-removed provider. This bypasses the normal publish flow on purpose so we can
            // isolate GetProviderDetails' read behaviour from unrelated write paths.
            Service service2 = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 8006);

            var service2RemovalRequest = new ServiceRemovalRequest
            {
                ServiceId = service2.Id,
                IsRequestPending = true,  // ← PENDING, NOT YET REMOVED
                PreviousServiceStatus = ServiceStatusEnum.Published
            };
            dbContext.ServiceRemovalRequest.Add(service2RemovalRequest);
            await dbContext.SaveChangesAsync();

            // Step 3: Query provider details - should show S2 with pending removal
            ProviderProfile result = await repository.GetProviderDetails(provider.Id);

            // Service has pending removal but Provider ALREADY has RemovedTime - the query should
            // return both unchanged.
            Assert.NotEmpty(result.Services!);
            Service resultService = result.Services!.First(s => s.Id == service2.Id);
            Assert.Null(resultService.RemovedTime); // Service NOT yet removed
            Assert.Equal(ServiceStatusEnum.AwaitingRemovalConfirmation, resultService.ServiceStatus);

            // Provider RemovedTime from the earlier removal must be untouched by this read query.
            Assert.NotNull(result.RemovedTime);
            Assert.Equal(providerRemovedTime, result.RemovedTime);
            Assert.False(result.IsInRegister);
        }

        [Fact]
        public async Task ApproveProviderRemoval_WithRemovedUnderReassignService_UpdatesCabTransferStatus()
        {
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);
            ProviderProfile provider = await SaveProviderAsync(dbContext, true);
            Service transferService = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 9001);
            int userId = await dbContext.User.Select(user => user.Id).FirstAsync();

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
                        ServiceId = transferService.Id,
                        PreviousServiceStatus = ServiceStatusEnum.RemovedUnderReassign
                    }
                ]
            };
            dbContext.ProviderRemovalRequest.Add(request);
            await dbContext.SaveChangesAsync();

            GenericResponse response = await repository.ApproveProviderRemoval(provider.Id, request.Id,
                "test.user@dsit.gov.com");

            Assert.True(response.Success);
            RequestManagement updatedRequest = await dbContext.RequestManagement.SingleAsync(r => r.Id == requestManagement.Id);
            Assert.Equal(RequestStatusEnum.Removed, updatedRequest.RequestStatus);

            // Verify RemovedTime is set
            Service updatedService = await dbContext.Service.SingleAsync(s => s.Id == transferService.Id);
            Assert.NotNull(updatedService.RemovedTime);
            Assert.Equal(ServiceStatusEnum.Removed, updatedService.ServiceStatus);

            ProviderProfile updatedProvider = await dbContext.ProviderProfile.SingleAsync(p => p.Id == provider.Id);
            Assert.NotNull(updatedProvider.RemovedTime);
            Assert.False(updatedProvider.IsInRegister);

            ProviderRemovalRequest updatedProviderRequest = await dbContext.ProviderRemovalRequest.SingleAsync(pr => pr.Id == request.Id);
            Assert.NotNull(updatedProviderRequest.RemovedTime);
        }

        [Fact]
        public async Task ApproveProviderRemoval_WithCabAwaitingRemovalConfirmationService_HandlesStatusCorrectly()
        {
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);
            ProviderProfile provider = await SaveProviderAsync(dbContext, true);
            Service service = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 9002);

            var request = new ProviderRemovalRequest
            {
                ProviderProfileId = provider.Id,
                IsRequestPending = true,
                PreviousProviderStatus = ProviderStatusEnum.NA,
                ProviderRemovalRequestServiceMapping =
                [
                    new ProviderRemovalRequestServiceMapping
                    {
                        ServiceId = service.Id,
                        PreviousServiceStatus = ServiceStatusEnum.CabAwaitingRemovalConfirmation
                    }
                ]
            };
            dbContext.ProviderRemovalRequest.Add(request);
            await dbContext.SaveChangesAsync();

            GenericResponse response = await repository.ApproveProviderRemoval(provider.Id, request.Id,
                "test.user@dsit.gov.com");

            Assert.True(response.Success);
            Service updatedService = await dbContext.Service.SingleAsync(s => s.Id == service.Id);
            Assert.Equal(ServiceStatusEnum.Removed, updatedService.ServiceStatus);
            Assert.NotNull(updatedService.RemovedTime);
        }

        [Fact]
        public async Task ApproveServiceRemoval_AllServicesRemoved_CreatesAutomaticProviderRemoval()
        {
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);
            ProviderProfile provider = await SaveProviderAsync(dbContext, true);
            Service lastService = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 10001);

            var removalRequest = new ServiceRemovalRequest
            {
                ServiceId = lastService.Id,
                IsRequestPending = true,
                PreviousServiceStatus = ServiceStatusEnum.Published
            };
            dbContext.ServiceRemovalRequest.Add(removalRequest);
            await dbContext.SaveChangesAsync();

            GenericResponse response = await repository.ApproveServiceRemoval(lastService.Id, removalRequest.Id,
                "test.user@dsit.gov.com");

            Assert.True(response.Success);
            ProviderProfile updatedProvider = await dbContext.ProviderProfile.SingleAsync(p => p.Id == provider.Id);
            Assert.False(updatedProvider.IsInRegister);
            Assert.NotNull(updatedProvider.RemovedTime);
            Assert.Equal(ProviderStatusEnum.NA, updatedProvider.ProviderStatus);

            ProviderRemovalRequest autoCreatedRequest = await dbContext.ProviderRemovalRequest
                .SingleAsync(r => r.ProviderProfileId == provider.Id);
            Assert.NotNull(autoCreatedRequest.RemovedTime);
            Assert.False(autoCreatedRequest.IsRequestPending);
        }

        [Fact]
        public async Task ApproveServiceRemoval_SaveFails_ReturnsFailureAndRollsBack()
        {
            int serviceId;
            int requestId;
            await using (var setupContext = CreateDbContext())
            {
                serviceId = await SaveServiceAsync(setupContext, ServiceStatusEnum.AwaitingRemovalConfirmation, true);
                Service service = await setupContext.Service.SingleAsync(s => s.Id == serviceId);
                var request = new ServiceRemovalRequest
                {
                    ServiceId = serviceId,
                    IsRequestPending = true,
                    PreviousServiceStatus = ServiceStatusEnum.Published
                };
                setupContext.ServiceRemovalRequest.Add(request);
                await setupContext.SaveChangesAsync();
                requestId = request.Id;
            }

            await using var failingContext = CreateThrowingDbContext();
            var repository = new RemoveProvider2iRepository(failingContext, logger);

            GenericResponse response = await repository.ApproveServiceRemoval(serviceId, requestId, "user");

            Assert.False(response.Success);
        }

        [Fact]
        public async Task ApproveProviderRemoval_SaveFails_ReturnsFailureAndRollsBack()
        {
            int providerId;
            int requestId;
            await using (var setupContext = CreateDbContext())
            {
                ProviderProfile provider = await SaveProviderAsync(setupContext, true);
                Service service = await SaveServiceEntityAsync(setupContext, provider.Id,
                    ServiceStatusEnum.AwaitingRemovalConfirmation, true, 11001);

                var request = new ProviderRemovalRequest
                {
                    ProviderProfileId = provider.Id,
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
                setupContext.ProviderRemovalRequest.Add(request);
                await setupContext.SaveChangesAsync();
                providerId = provider.Id;
                requestId = request.Id;
            }

            await using var failingContext = CreateThrowingDbContext();
            var repository = new RemoveProvider2iRepository(failingContext, logger);

            GenericResponse response = await repository.ApproveProviderRemoval(providerId, requestId, "user");

            Assert.False(response.Success);
        }

        [Fact]
        public async Task CancelRemoveProviderRequest_SaveFails_ReturnsFailureAndRollsBack()
        {
            int providerId;
            int requestId;
            await using (var setupContext = CreateDbContext())
            {
                ProviderProfile provider = await SaveProviderAsync(setupContext, true);
                Service service = await SaveServiceEntityAsync(setupContext, provider.Id,
                    ServiceStatusEnum.AwaitingRemovalConfirmation, true, 11002);

                var request = new ProviderRemovalRequest
                {
                    ProviderProfileId = provider.Id,
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
                setupContext.ProviderRemovalRequest.Add(request);
                await setupContext.SaveChangesAsync();
                providerId = provider.Id;
                requestId = request.Id;
            }

            await using var failingContext = CreateThrowingDbContext();
            var repository = new RemoveProvider2iRepository(failingContext, logger);

            GenericResponse response = await repository.CancelRemoveProviderRequest(providerId, requestId, "user");

            Assert.False(response.Success);
        }

        [Fact]
        public async Task CancelRemoveServiceRequest_WithBulkRemovalRequest_RestoresBulkRequestPending()
        {
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);
            ProviderProfile provider = await SaveProviderAsync(dbContext, true);
            Service service = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 11003);
            int userId = await dbContext.User.Select(user => user.Id).FirstAsync();

            var bulkRequest = new ServiceBulkRemovalRequest
            {
                Comment = "Bulk removal",
                IsRequestPending = false,
                RequestedTime = DateTime.UtcNow,
                RequestedBy = userId
            };
            var serviceRemovalRequest = new ServiceRemovalRequest
            {
                ServiceId = service.Id,
                IsRequestPending = true,
                PreviousServiceStatus = ServiceStatusEnum.Published,
                ServiceBulkRemovalRequest = bulkRequest
            };
            dbContext.ServiceRemovalRequest.Add(serviceRemovalRequest);
            await dbContext.SaveChangesAsync();

            GenericResponse response = await repository.CancelRemoveServiceRequest(service.Id, serviceRemovalRequest.Id,
                "test.user@dsit.gov.com");

            Assert.True(response.Success);
            ServiceRemovalRequest updatedRequest = await dbContext.ServiceRemovalRequest.SingleAsync();
            Assert.False(updatedRequest.IsRequestPending);
            ServiceBulkRemovalRequest updatedBulk = await dbContext.ServiceBulkRemovalRequest.SingleAsync();
            Assert.False(updatedBulk.IsRequestPending);
        }

        [Fact]
        public async Task ApproveServiceRemoval_PreviouslyRemovedProvider_UpdatesProviderRemovedTimeWhenLastServiceRemoved()
        {
            await using var dbContext = CreateDbContext();
            var repository = new RemoveProvider2iRepository(dbContext, logger);

            // Step 1: Create provider and service, remove provider
            ProviderProfile provider = await SaveProviderAsync(dbContext, true);
            Service service1 = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 12001);

            var providerRemovalRequest = new ProviderRemovalRequest
            {
                ProviderProfileId = provider.Id,
                IsRequestPending = true,
                PreviousProviderStatus = ProviderStatusEnum.NA,
                ProviderRemovalRequestServiceMapping =
                [
                    new ProviderRemovalRequestServiceMapping
                    {
                        ServiceId = service1.Id,
                        PreviousServiceStatus = ServiceStatusEnum.Published
                    }
                ]
            };
            dbContext.ProviderRemovalRequest.Add(providerRemovalRequest);
            await dbContext.SaveChangesAsync();

            // Approve provider removal
            var approveProviderResponse = await repository.ApproveProviderRemoval(provider.Id, providerRemovalRequest.Id,
                "test.user@dsit.gov.com");

            Assert.True(approveProviderResponse.Success);
            ProviderProfile removedProvider = await dbContext.ProviderProfile.SingleAsync(p => p.Id == provider.Id);
            DateTime? firstRemovedTime = removedProvider.RemovedTime;
            Assert.NotNull(firstRemovedTime);
            Assert.False(removedProvider.IsInRegister);

            // Step 2: Add new service under previously removed provider
            Service service2 = await SaveServiceEntityAsync(dbContext, provider.Id,
                ServiceStatusEnum.AwaitingRemovalConfirmation, true, 12002);

            var service2RemovalRequest = new ServiceRemovalRequest
            {
                ServiceId = service2.Id,
                IsRequestPending = true,
                PreviousServiceStatus = ServiceStatusEnum.Published
            };
            dbContext.ServiceRemovalRequest.Add(service2RemovalRequest);
            await dbContext.SaveChangesAsync();

            // Step 3: Approve S2 removal - should trigger provider auto-removal with NEW RemovedTime
            var approveServiceResponse = await repository.ApproveServiceRemoval(service2.Id, service2RemovalRequest.Id,
                "test.user@dsit.gov.com");

            Assert.True(approveServiceResponse.Success);

            // Verify S2 is removed
            Service removedService2 = await dbContext.Service.SingleAsync(s => s.Id == service2.Id);
            Assert.NotNull(removedService2.RemovedTime);
            Assert.False(removedService2.IsInRegister);
            Assert.Equal(ServiceStatusEnum.Removed, removedService2.ServiceStatus);

            // Verify provider auto-removal was triggered with NEW RemovedTime
            ProviderProfile updatedProvider = await dbContext.ProviderProfile.SingleAsync(p => p.Id == provider.Id);
            Assert.NotNull(updatedProvider.RemovedTime);
            // RemovedTime should be updated (S2 removal is more recent than first removal)
            Assert.True(updatedProvider.RemovedTime >= firstRemovedTime,
                $"Provider RemovedTime should be updated when new service is removed. First: {firstRemovedTime}, Current: {updatedProvider.RemovedTime}");

            // Verify auto-created provider removal request
            var autoCreatedProviderRequest = await dbContext.ProviderRemovalRequest
                .Where(pr => pr.ProviderProfileId == provider.Id && pr.IsRequestPending == false && pr.RemovedTime != null)
                .OrderByDescending(pr => pr.RemovedTime)
                .FirstAsync();
            Assert.NotNull(autoCreatedProviderRequest);
            Assert.NotNull(autoCreatedProviderRequest.RemovedTime);
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
