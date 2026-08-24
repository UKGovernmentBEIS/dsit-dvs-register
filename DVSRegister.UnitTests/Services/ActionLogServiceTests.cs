using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace DVSRegister.UnitTests.Services
{
    public class ActionLogServiceTests
    {
        private readonly IActionLogRepository _actionLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ActionLogService> _logger;
        private readonly ActionLogService _service;

        public ActionLogServiceTests()
        {
            _actionLogRepository = Substitute.For<IActionLogRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _logger = Substitute.For<ILogger<ActionLogService>>();

            _actionLogRepository.GetActionCategory(Arg.Any<ActionCategoryEnum>())
                .Returns(new ActionCategory { Id = 4 });
            _actionLogRepository.GetActionDetails(Arg.Any<ActionDetailsEnum>())
                .Returns(new ActionDetails { Id = 15 });
            _userRepository.GetUser(Arg.Any<string>())
                .Returns(new CabUser { Id = 99 });

            _service = new ActionLogService(
                _actionLogRepository,
                _userRepository,
                _logger);
        }

        [Fact]
        public async Task AddEditActionLogs_BusinessNamesChangedForPublishedProvider_SavesVisibleSerializedLog()
        {
            var previous = new Dictionary<string, List<string>>
            {
                [Constants.RegisteredName] = ["Old registered"],
                [Constants.TradingName] = ["Old trading"]
            };
            var current = new Dictionary<string, List<string>>
            {
                [Constants.RegisteredName] = ["New registered"],
                [Constants.TradingName] = ["New trading"]
            };
            var provider = CreateProvider([
                new ServiceDto { IsInRegister = true }
            ]);

            await _service.AddEditActionLogs(
                ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum.BusinessDetailsUpdate,
                "user@example.com",
                new ChangeSet(current, previous),
                provider);

            await _actionLogRepository.Received(1).SaveActionLogs(
                Arg.Is<ActionLogs>(log =>
                    log.ActionCategoryId == 4 &&
                    log.ActionDetailsId == 15 &&
                    log.CabUserId == 99 &&
                    log.ProviderProfileId == provider.Id &&
                    log.ShowInRegisterUpdates &&
                    log.DisplayMessage ==
                        $"Old registered to New registered ({Constants.RegisteredName}){Environment.NewLine}" +
                        $"Old trading to New trading ({Constants.TradingName})" &&
                    log.OldValues!.RootElement
                        .GetProperty(Constants.RegisteredName)[0].GetString() == "Old registered" &&
                    log.NewValues!.RootElement
                        .GetProperty(Constants.TradingName)[0].GetString() == "New trading"));
        }

        [Fact]
        public async Task AddEditActionLogs_TradingNameChangedForRemovedProvider_SavesVisibleTradingNameLog()
        {
            var previous = new Dictionary<string, List<string>>
            {
                [Constants.TradingName] = ["Old trading"]
            };
            var current = new Dictionary<string, List<string>>
            {
                [Constants.TradingName] = ["New trading"]
            };
            var provider = CreateProvider([
                new ServiceDto { ServiceStatus = ServiceStatusEnum.Removed }
            ]);

            await _service.AddEditActionLogs(
                ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum.BusinessDetailsUpdate,
                "user@example.com",
                new ChangeSet(current, previous),
                provider);

            await _actionLogRepository.Received(1).SaveActionLogs(
                Arg.Is<ActionLogs>(log =>
                    log.ShowInRegisterUpdates &&
                    log.DisplayMessage ==
                        $"Old trading to New trading ({Constants.TradingName})"));
        }

        [Fact]
        public async Task AddEditActionLogs_NonNameBusinessDetailsChanged_SavesHiddenLogWithEmptyMessage()
        {
            var previous = new Dictionary<string, List<string>>
            {
                [Constants.CompanyRegistrationNumber] = ["Old number"]
            };
            var current = new Dictionary<string, List<string>>
            {
                [Constants.CompanyRegistrationNumber] = ["New number"]
            };

            await _service.AddEditActionLogs(
                ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum.BusinessDetailsUpdate,
                string.Empty,
                new ChangeSet(current, previous),
                CreateProvider());

            await _actionLogRepository.Received(1).SaveActionLogs(
                Arg.Is<ActionLogs>(log =>
                    !log.ShowInRegisterUpdates &&
                    log.DisplayMessage == string.Empty &&
                    log.CabUserId == null));
            await _userRepository.DidNotReceive().GetUser(Arg.Any<string>());
        }

        [Fact]
        public async Task AddEditActionLogs_BusinessNameChangedForUnpublishedProvider_SavesHiddenNamedLog()
        {
            var previous = CreateData(Constants.RegisteredName, "Old name");
            var current = CreateData(Constants.RegisteredName, "New name");

            await _service.AddEditActionLogs(
                ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum.BusinessDetailsUpdate,
                "user@example.com",
                new ChangeSet(current, previous),
                CreateProvider());

            await _actionLogRepository.Received(1).SaveActionLogs(
                Arg.Is<ActionLogs>(log =>
                    !log.ShowInRegisterUpdates &&
                    log.DisplayMessage ==
                        $"Old name to New name ({Constants.RegisteredName})"));
        }

        [Fact]
        public async Task AddEditActionLogs_PrivateContactChanged_SavesHiddenProviderLog()
        {
            var previous = new Dictionary<string, List<string>>
            {
                [Constants.PrimaryContactName] = ["Old contact"]
            };
            var current = new Dictionary<string, List<string>>
            {
                [Constants.PrimaryContactName] = ["New contact"]
            };

            await _service.AddEditActionLogs(
                ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum.ProviderContactUpdate,
                "user@example.com",
                new ChangeSet(current, previous),
                CreateProvider());

            await _actionLogRepository.Received(1).SaveActionLogs(
                Arg.Is<ActionLogs>(log =>
                    !log.ShowInRegisterUpdates &&
                    log.DisplayMessage == "Provider name"));
        }

        [Theory]
        [InlineData(Constants.PublicContactEmail)]
        [InlineData(Constants.ProviderWebsiteAddress)]
        [InlineData(Constants.ProviderTelephoneNumber)]
        public async Task AddEditActionLogs_PublicContactChangedForPublishedProvider_SavesVisibleProviderLog(
            string publicContactKey)
        {
            var previous = new Dictionary<string, List<string>>
            {
                [publicContactKey] = ["old value"]
            };
            var current = new Dictionary<string, List<string>>
            {
                [publicContactKey] = ["new value"]
            };
            var provider = CreateProvider([
                new ServiceDto { IsInRegister = true }
            ]);

            await _service.AddEditActionLogs(
                ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum.ProviderContactUpdate,
                "user@example.com",
                new ChangeSet(current, previous),
                provider);

            await _actionLogRepository.Received(1).SaveActionLogs(
                Arg.Is<ActionLogs>(log =>
                    log.ShowInRegisterUpdates &&
                    log.DisplayMessage == provider.RegisteredName &&
                    log.OldValues!.RootElement.GetProperty(publicContactKey)[0].GetString() == "old value" &&
                    log.NewValues!.RootElement.GetProperty(publicContactKey)[0].GetString() == "new value"));
        }

        [Fact]
        public async Task AddEditActionLogs_PublicContactChangedForUnpublishedProvider_SavesHiddenProviderLog()
        {
            var previous = CreateData(Constants.PublicContactEmail, "old@example.com");
            var current = CreateData(Constants.PublicContactEmail, "new@example.com");

            await _service.AddEditActionLogs(
                ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum.ProviderContactUpdate,
                "user@example.com",
                new ChangeSet(current, previous),
                CreateProvider());

            await _actionLogRepository.Received(1).SaveActionLogs(
                Arg.Is<ActionLogs>(log =>
                    !log.ShowInRegisterUpdates &&
                    log.DisplayMessage == "Provider name"));
        }

        [Fact]
        public async Task AddEditActionLogs_ProviderUpdateWithUnsupportedDetails_SavesSerializedLogWithEmptyMessage()
        {
            var previous = new Dictionary<string, List<string>>
            {
                ["Field"] = ["Old"]
            };
            var current = new Dictionary<string, List<string>>
            {
                ["Field"] = ["New"]
            };

            await _service.AddEditActionLogs(
                ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum.ServiceUpdates,
                "user@example.com",
                new ChangeSet(current, previous),
                CreateProvider());

            await _actionLogRepository.Received(1).SaveActionLogs(
                Arg.Is<ActionLogs>(log =>
                    log.DisplayMessage == string.Empty &&
                    log.OldValues != null &&
                    log.NewValues != null));
        }

        [Fact]
        public async Task AddEditActionLogs_NonProviderCategory_DoesNotSaveLog()
        {
            await _service.AddEditActionLogs(
                ActionCategoryEnum.ServiceUpdates,
                ActionDetailsEnum.ServiceUpdates,
                "user@example.com",
                CreateChangeSet(),
                CreateProvider());

            await _actionLogRepository.DidNotReceive()
                .SaveActionLogs(Arg.Any<ActionLogs>());
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task AddEditActionLogs_EmptyCurrentOrPreviousData_LogsFailureWithoutSaving(
            bool emptyCurrent,
            bool emptyPrevious)
        {
            var current = emptyCurrent
                ? new Dictionary<string, List<string>>()
                : CreateData(Constants.RegisteredName, "New");
            var previous = emptyPrevious
                ? new Dictionary<string, List<string>>()
                : CreateData(Constants.RegisteredName, "Old");

            await _service.AddEditActionLogs(
                ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum.BusinessDetailsUpdate,
                "user@example.com",
                new ChangeSet(current, previous),
                CreateProvider());

            await _actionLogRepository.DidNotReceive()
                .SaveActionLogs(Arg.Any<ActionLogs>());
            AssertLoggedError("Previous data or updated data null");
        }

        [Fact]
        public async Task AddEditActionLogs_NullCurrentData_LogsFailureWithoutSaving()
        {
            await _service.AddEditActionLogs(
                ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum.BusinessDetailsUpdate,
                "user@example.com",
                new ChangeSet(null!, CreateData(Constants.RegisteredName, "Old")),
                CreateProvider());

            await _actionLogRepository.DidNotReceive()
                .SaveActionLogs(Arg.Any<ActionLogs>());
            AssertLoggedError("Previous data or updated data null");
        }

        [Fact]
        public async Task AddEditActionLogs_RepositoryThrows_LogsFailureWithoutPropagating()
        {
            _actionLogRepository.SaveActionLogs(Arg.Any<ActionLogs>())
                .ThrowsAsync(new InvalidOperationException("database failure"));

            await _service.AddEditActionLogs(
                ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum.ProviderContactUpdate,
                "user@example.com",
                CreateChangeSet(),
                CreateProvider());

            AssertLoggedError("database failure");
        }

        [Fact]
        public async Task AddActionLog_ServiceWithAllRelatedRecords_SavesFullyMappedLog()
        {
            var before = DateTime.UtcNow;
            var service = CreateService(10);
            service.ServiceStatus = ServiceStatusEnum.Published;
            service.PublicInterestCheck =
            [
                new PublicInterestCheckDto { Id = 101 },
                new PublicInterestCheckDto { Id = 102, IsLatestReviewVersion = true }
            ];
            service.CertificateReview =
            [
                new CertificateReviewDto { Id = 201 },
                new CertificateReviewDto { Id = 202, IsLatestReviewVersion = true }
            ];
            service.CabTransferRequestId = 301;
            service.ServiceRemovalRequestId = 401;
            service.ProviderRemovalRequestServiceMapping = new ProviderRemovalRequestServiceMappingDto
            {
                ProviderRemovalRequestId = 501
            };

            await _service.AddActionLog(
                service,
                ActionCategoryEnum.ActionRequests,
                ActionDetailsEnum.DisplayChangeRequestSent,
                "user@example.com",
                "Admin message");

            var after = DateTime.UtcNow;
            await _actionLogRepository.Received(1).SaveActionLogs(
                Arg.Is<ActionLogs>(log =>
                    log.ActionCategoryId == 4 &&
                    log.ActionDetailsId == 15 &&
                    log.ServiceId == 10 &&
                    log.ProviderProfileId == 1010 &&
                    log.CabUserId == 99 &&
                    log.PublicInterestCheckId == 102 &&
                    log.CertificateReviewId == 202 &&
                    log.CabTransferRequestId == 301 &&
                    log.ServiceRemovalRequestId == 401 &&
                    log.ProviderRemovalRequestId == 501 &&
                    log.DisplayMessageAdmin == "Admin message" &&
                    log.DisplayMessage == string.Empty &&
                    log.ServiceStatus == ServiceStatusEnum.Published &&
                    log.OldValues == null &&
                    log.NewValues == null &&
                    log.LogDate == DateTime.UtcNow.Date &&
                    log.LoggedTime >= before &&
                    log.LoggedTime <= after));
            await _actionLogRepository.Received(1)
                .GetActionCategory(ActionCategoryEnum.ActionRequests);
            await _actionLogRepository.Received(1)
                .GetActionDetails(ActionDetailsEnum.DisplayChangeRequestSent);
        }

        [Fact]
        public async Task AddActionLog_MissingOptionalRecordsAndUser_SavesNullRelatedIds()
        {
            _userRepository.GetUser("missing@example.com")
                .Returns((CabUser)null!);
            var service = CreateService(11);
            service.Provider.RegisteredName = null;
            service.PublicInterestCheck = null!;
            service.CertificateReview = null!;
            service.CabTransferRequestId = 0;
            service.ServiceRemovalRequestId = 0;
            service.ProviderRemovalRequestServiceMapping = null;

            await _service.AddActionLog(
                service,
                ActionCategoryEnum.CR,
                ActionDetailsEnum.CR_APR,
                "missing@example.com");

            await _actionLogRepository.Received(1).SaveActionLogs(
                Arg.Is<ActionLogs>(log =>
                    log.CabUserId == null &&
                    log.PublicInterestCheckId == null &&
                    log.CertificateReviewId == null &&
                    log.CabTransferRequestId == null &&
                    log.ServiceRemovalRequestId == null &&
                    log.ProviderRemovalRequestId == null &&
                    log.UpdateRequestedUserId == null &&
                    log.UpdateRequestedTime == null &&
                    log.DisplayMessageAdmin == null));
        }

        [Fact]
        public async Task AddActionLog_UserWithZeroId_SavesNullUserId()
        {
            _userRepository.GetUser("user@example.com")
                .Returns(new CabUser { Id = 0 });

            await _service.AddActionLog(
                CreateService(),
                ActionCategoryEnum.PI,
                ActionDetailsEnum.PI_Primary_Pass,
                "user@example.com");

            await _actionLogRepository.Received(1).SaveActionLogs(
                Arg.Is<ActionLogs>(log => log.CabUserId == null));
        }

        [Fact]
        public async Task AddActionLog_EmptyUserEmail_SkipsUserLookupAndSavesNullUserId()
        {
            await _service.AddActionLog(
                CreateService(),
                ActionCategoryEnum.ServiceUpdates,
                ActionDetailsEnum.ServiceNameUpdate,
                string.Empty);

            await _userRepository.DidNotReceive().GetUser(Arg.Any<string>());
            await _actionLogRepository.Received(1).SaveActionLogs(
                Arg.Is<ActionLogs>(log => log.CabUserId == null));
        }

        [Fact]
        public async Task AddActionLog_NullUserEmail_SkipsUserLookupAndSavesNullUserId()
        {
            await _service.AddActionLog(
                CreateService(),
                ActionCategoryEnum.ServiceUpdates,
                ActionDetailsEnum.ServiceNameUpdate,
                null!);

            await _userRepository.DidNotReceive().GetUser(Arg.Any<string>());
            await _actionLogRepository.Received(1).SaveActionLogs(
                Arg.Is<ActionLogs>(log => log.CabUserId == null));
        }

        [Fact]
        public async Task AddActionLog_NullService_ThrowsArgumentNullException()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.AddActionLog(
                    null!,
                    ActionCategoryEnum.CR,
                    ActionDetailsEnum.CR_APR,
                    "user@example.com"));

            Assert.Equal("serviceDto", exception.ParamName);
            await _actionLogRepository.DidNotReceive()
                .SaveActionLogs(Arg.Any<ActionLogs>());
        }

        [Fact]
        public async Task AddActionLog_NullProvider_ThrowsArgumentNullException()
        {
            var service = CreateService();
            service.Provider = null!;

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.AddActionLog(
                    service,
                    ActionCategoryEnum.CR,
                    ActionDetailsEnum.CR_APR,
                    "user@example.com"));

            Assert.Equal("serviceDto.Provider", exception.ParamName);
            await _actionLogRepository.DidNotReceive()
                .SaveActionLogs(Arg.Any<ActionLogs>());
        }

        [Fact]
        public async Task AddActionLog_CategoryLookupThrows_PropagatesFailureWithoutSaving()
        {
            _actionLogRepository.GetActionCategory(ActionCategoryEnum.CR)
                .ThrowsAsync(new InvalidOperationException("category failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AddActionLog(
                    CreateService(),
                    ActionCategoryEnum.CR,
                    ActionDetailsEnum.CR_APR,
                    "user@example.com"));

            Assert.Equal("category failure", exception.Message);
            await _actionLogRepository.DidNotReceive()
                .SaveActionLogs(Arg.Any<ActionLogs>());
        }

        [Fact]
        public async Task AddActionLog_SaveThrows_PropagatesFailure()
        {
            _actionLogRepository.SaveActionLogs(Arg.Any<ActionLogs>())
                .ThrowsAsync(new InvalidOperationException("save failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AddActionLog(
                    CreateService(),
                    ActionCategoryEnum.CR,
                    ActionDetailsEnum.CR_APR,
                    "user@example.com"));

            Assert.Equal("save failure", exception.Message);
        }

        [Fact]
        public async Task AddMultipleActionLogs_HighVolumeServices_SavesAllLogsInSingleBatch()
        {
            var services = Enumerable.Range(1, 100)
                .Select(CreateService)
                .ToList();

            await _service.AddMultipleActionLogs(
                services,
                ActionCategoryEnum.ActionRequests,
                ActionDetailsEnum.DisplayChangeRequestSent,
                "user@example.com",
                "Bulk update");

            await _actionLogRepository.Received(1).SaveMultipleActionLogs(
                Arg.Is<List<ActionLogs>>(logs =>
                    logs.Count == 100 &&
                    logs.Select(log => log.ServiceId).SequenceEqual(
                        Enumerable.Range(1, 100).Select(id => (int?)id)) &&
                    logs.All(log =>
                        log.ActionCategoryId == 4 &&
                        log.ActionDetailsId == 15 &&
                        log.DisplayMessageAdmin == "Bulk update")));
        }

        [Fact]
        public async Task AddMultipleActionLogs_MixedServiceData_MapsEachServiceIndependently()
        {
            var first = CreateService();
            first.PublicInterestCheck =
            [
                new PublicInterestCheckDto { Id = 21, IsLatestReviewVersion = true }
            ];
            var second = CreateService(2);
            second.CertificateReview =
            [
                new CertificateReviewDto { Id = 32, IsLatestReviewVersion = true }
            ];
            second.ProviderRemovalRequestServiceMapping = new ProviderRemovalRequestServiceMappingDto
            {
                ProviderRemovalRequestId = 42
            };

            await _service.AddMultipleActionLogs(
                [first, second],
                ActionCategoryEnum.PI,
                ActionDetailsEnum.PI_ServicePublish,
                "user@example.com");

            await _actionLogRepository.Received(1).SaveMultipleActionLogs(
                Arg.Is<List<ActionLogs>>(logs =>
                    logs.Count == 2 &&
                    logs[0].ServiceId == 1 &&
                    logs[0].ProviderProfileId == 1001 &&
                    logs[0].PublicInterestCheckId == 21 &&
                    logs[0].CertificateReviewId == null &&
                    logs[1].ServiceId == 2 &&
                    logs[1].ProviderProfileId == 1002 &&
                    logs[1].PublicInterestCheckId == null &&
                    logs[1].CertificateReviewId == 32 &&
                    logs[1].ProviderRemovalRequestId == 42));
        }

        [Fact]
        public async Task AddMultipleActionLogs_EmptyServices_SavesEmptyBatch()
        {
            await _service.AddMultipleActionLogs(
                [],
                ActionCategoryEnum.CR,
                ActionDetailsEnum.CR_APR,
                "user@example.com");

            await _actionLogRepository.Received(1)
                .SaveMultipleActionLogs(Arg.Is<List<ActionLogs>>(logs => logs.Count == 0));
            await _actionLogRepository.DidNotReceive()
                .GetActionCategory(Arg.Any<ActionCategoryEnum>());
        }

        [Fact]
        public async Task AddMultipleActionLogs_NullServices_ThrowsArgumentNullException()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.AddMultipleActionLogs(
                    null!,
                    ActionCategoryEnum.CR,
                    ActionDetailsEnum.CR_APR,
                    "user@example.com"));

            Assert.Equal("serviceDtos", exception.ParamName);
            await _actionLogRepository.DidNotReceive()
                .SaveMultipleActionLogs(Arg.Any<List<ActionLogs>>());
        }

        [Fact]
        public async Task AddMultipleActionLogs_ServiceWithNullProvider_ThrowsNullReferenceException()
        {
            var service = CreateService();
            service.Provider = null!;

            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _service.AddMultipleActionLogs(
                    [service],
                    ActionCategoryEnum.CR,
                    ActionDetailsEnum.CR_APR,
                    "user@example.com"));

            await _actionLogRepository.DidNotReceive()
                .SaveMultipleActionLogs(Arg.Any<List<ActionLogs>>());
        }

        [Fact]
        public async Task AddMultipleActionLogs_BulkSaveThrows_PropagatesFailure()
        {
            _actionLogRepository.SaveMultipleActionLogs(Arg.Any<List<ActionLogs>>())
                .ThrowsAsync(new InvalidOperationException("bulk failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AddMultipleActionLogs(
                    [CreateService()],
                    ActionCategoryEnum.CR,
                    ActionDetailsEnum.CR_APR,
                    "user@example.com"));

            Assert.Equal("bulk failure", exception.Message);
        }

        private void AssertLoggedError(string expectedMessage)
        {
            Assert.Contains(
                _logger.ReceivedCalls(),
                call => call.GetArguments().Any(argument =>
                    argument?.ToString()?.Contains(expectedMessage) == true));
        }

        private static ProviderProfileDto CreateProvider(
            ICollection<ServiceDto>? services = null)
        {
            return new ProviderProfileDto
            {
                Id = 7,
                RegisteredName = "Provider name",
                Services = services
            };
        }

        private static ServiceDto CreateService(int id = 1)
        {
            return new ServiceDto
            {
                Id = id,
                ServiceName = $"Service {id}",
                ServiceStatus = ServiceStatusEnum.AwaitingRemovalConfirmation,
                Provider = new ProviderProfileDto
                {
                    Id = 1000 + id,
                    RegisteredName = $"Provider {id}"
                },
                PublicInterestCheck = [],
                CertificateReview = []
            };
        }

        private static ChangeSet CreateChangeSet()
        {
            return new ChangeSet(
                CreateData(Constants.PrimaryContactName, "New contact"),
                CreateData(Constants.PrimaryContactName, "Old contact"));
        }

        private static Dictionary<string, List<string>> CreateData(
            string key,
            string value)
        {
            return new Dictionary<string, List<string>>
            {
                [key] = [value]
            };
        }
    }
}