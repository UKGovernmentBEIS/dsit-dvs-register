using AutoMapper;
using DVSAdmin.CommonUtility.Models.Enums;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.Remove2i;
using DVSRegister.BusinessLogic.Remove2i;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Entities;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace DVSRegister.UnitTests.Services
{
    public class RemoveProvider2IServiceTests
    {
        private readonly IRemoveProvider2iRepository _repository;
        private readonly ICabRepository _cabRepository;
        private readonly IMapper _mapper;
        private readonly IRemoval2iCheckEmailSender _emailSender;
        private readonly RemoveProvider2iService _service;

        public RemoveProvider2IServiceTests()
        {
            _repository = Substitute.For<IRemoveProvider2iRepository>();
            _cabRepository = Substitute.For<ICabRepository>();
            _mapper = Substitute.For<IMapper>();
            _emailSender = Substitute.For<IRemoval2iCheckEmailSender>();
            _service = new RemoveProvider2iService(
                _repository,
                _cabRepository,
                _mapper,
                _emailSender);
        }

        [Fact]
        public async Task GetProviderRemovalDetailsByRemovalToken_ValidToken_LoadsProviderAndMapsRequest()
        {
            var request = new ProviderRemovalRequest { Id = 11, ProviderProfileId = 21 };
            var provider = new ProviderProfile { Id = 21 };
            var expected = new ProviderRemovalRequestDto { Id = 11 };
            _repository.GetRemoveProviderToken("token", "token-id").Returns(request);
            _repository.GetProviderDetails(21).Returns(provider);
            _mapper.Map<ProviderRemovalRequestDto>(request).Returns(expected);

            var result = await _service.GetProviderRemovalDetailsByRemovalToken("token", "token-id");

            Assert.Same(expected, result);
            Assert.Same(provider, request.Provider);
            await _repository.Received(1).GetRemoveProviderToken("token", "token-id");
            await _repository.Received(1).GetProviderDetails(21);
            _mapper.Received(1).Map<ProviderRemovalRequestDto>(request);
        }

        [Fact]
        public async Task GetProviderRemovalDetailsByRemovalToken_MissingRequest_ThrowsNullReferenceException()
        {
            _repository.GetRemoveProviderToken("missing", "token-id")
                .Returns((ProviderRemovalRequest)null!);

            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _service.GetProviderRemovalDetailsByRemovalToken("missing", "token-id"));

            await _repository.DidNotReceive().GetProviderDetails(Arg.Any<int>());
        }

        [Fact]
        public async Task GetProviderRemovalDetailsByRemovalToken_RepositoryThrows_PropagatesFailure()
        {
            _repository.GetRemoveProviderToken("token", "token-id")
                .ThrowsAsync(new InvalidOperationException("token failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetProviderRemovalDetailsByRemovalToken("token", "token-id"));

            Assert.Equal("token failure", exception.Message);
        }

        [Fact]
        public async Task GetProviderDetailsWithRemovedServices_ValidIds_ReturnsMappedProvider()
        {
            var serviceIds = new List<int> { 3, 4 };
            var provider = new ProviderProfile { Id = 2 };
            var expected = new ProviderProfileDto { Id = 2 };
            _repository.GetProviderDetailsWithRemovedServices(2, serviceIds).Returns(provider);
            _mapper.Map<ProviderProfileDto>(provider).Returns(expected);

            var result = await _service.GetProviderDetailsWithRemovedServices(2, serviceIds);

            Assert.Same(expected, result);
            await _repository.Received(1).GetProviderDetailsWithRemovedServices(2, serviceIds);
            _mapper.Received(1).Map<ProviderProfileDto>(provider);
        }

        [Fact]
        public async Task GetServiceDetailsWithProvider_ValidService_ReturnsMappedService()
        {
            var service = new Service { Id = 7 };
            var expected = new ServiceDto { Id = 7 };
            _repository.GetServiceDetailsWithProvider(7).Returns(service);
            _mapper.Map<ServiceDto>(service).Returns(expected);

            var result = await _service.GetServiceDetailsWithProvider(7);

            Assert.Same(expected, result);
            await _repository.Received(1).GetServiceDetailsWithProvider(7);
            _mapper.Received(1).Map<ServiceDto>(service);
        }

        [Fact]
        public async Task GetServiceRemovalDetailsByRemovalToken_ValidToken_LoadsServiceAndMapsRequest()
        {
            var partialService = new Service { Id = 31 };
            var fullService = new Service { Id = 31, ServiceName = "Full service" };
            var request = new ServiceRemovalRequest { Id = 41, Service = partialService };
            var expected = new ServiceRemovalRequestDto { Id = 41 };
            _repository.GetRemoveServiceToken("token", "token-id").Returns(request);
            _repository.GetServiceDetails(31).Returns(fullService);
            _mapper.Map<ServiceRemovalRequestDto>(request).Returns(expected);

            var result = await _service.GetServiceRemovalDetailsByRemovalToken("token", "token-id");

            Assert.Same(expected, result);
            Assert.Same(fullService, request.Service);
            await _repository.Received(1).GetServiceDetails(31);
            _mapper.Received(1).Map<ServiceRemovalRequestDto>(request);
        }

        [Fact]
        public async Task GetServiceRemovalDetailsByRemovalToken_MissingRequest_ThrowsNullReferenceException()
        {
            _repository.GetRemoveServiceToken("missing", "token-id")
                .Returns((ServiceRemovalRequest)null!);

            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _service.GetServiceRemovalDetailsByRemovalToken("missing", "token-id"));

            await _repository.DidNotReceive().GetServiceDetails(Arg.Any<int>());
        }

        [Fact]
        public async Task ApproveProviderRemoval_SuccessfulUpdate_SendsExpectedNotifications()
        {
            var response = new GenericResponse { Success = true, InstanceId = 51 };
            var request = CreateProviderRemovalRequest();
            _repository.ApproveProviderRemoval(7, 17, "admin@example.com").Returns(response);
            _cabRepository.GetCabEmailListForServices(Arg.Any<List<int>>())
                .Returns(["cab-one@example.com", "cab-two@example.com"]);
            var reason = request.RemovalReason.GetDescription();
            const string removedServiceNames = "Service one\rService three";

            var result = await _service.ApproveProviderRemoval(request, "admin@example.com");

            Assert.Same(response, result);
            await _cabRepository.Received(1).GetCabEmailListForServices(
                Arg.Is<List<int>>(ids => ids.SequenceEqual(new[] { 1, 2, 3 })));
            await _emailSender.Received(1)
                .SendRemovalRequestConfirmedToDIP("Primary", "primary@example.com");
            await _emailSender.Received(1)
                .SendRemovalRequestConfirmedToDIP("Secondary", "secondary@example.com");
            await _emailSender.Received(1)
                .SendProviderRemovalConfirmationToDSIT("Provider", removedServiceNames);
            await _emailSender.Received(1)
                .SendRecordRemovedToDSIT("Provider", removedServiceNames, reason);
            await _emailSender.Received(1).RecordRemovedConfirmedToCabOrProvider(
                "cab-one@example.com", "cab-one@example.com", "Provider", removedServiceNames, reason);
            await _emailSender.Received(1).RecordRemovedConfirmedToCabOrProvider(
                "cab-two@example.com", "cab-two@example.com", "Provider", removedServiceNames, reason);
            await _emailSender.Received(1).RecordRemovedConfirmedToCabOrProvider(
                "Primary", "primary@example.com", "Provider", removedServiceNames, reason);
            await _emailSender.Received(1).RecordRemovedConfirmedToCabOrProvider(
                "Secondary", "secondary@example.com", "Provider", removedServiceNames, reason);
        }

        [Fact]
        public async Task ApproveProviderRemoval_AlreadyProcessedResponse_ReturnsFailureWithoutNotifications()
        {
            var response = new GenericResponse
            {
                Success = false,
                ErrorMessage = "Already removed"
            };
            _repository.ApproveProviderRemoval(7, 17, "admin@example.com").Returns(response);

            var result = await _service.ApproveProviderRemoval(
                CreateProviderRemovalRequest(),
                "admin@example.com");

            Assert.Same(response, result);
            await _cabRepository.DidNotReceive()
                .GetCabEmailListForServices(Arg.Any<List<int>>());
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task ApproveProviderRemoval_RepositoryUpdateThrows_PropagatesWithoutNotifications()
        {
            _repository.ApproveProviderRemoval(7, 17, "admin@example.com")
                .ThrowsAsync(new InvalidOperationException("update failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ApproveProviderRemoval(
                    CreateProviderRemovalRequest(),
                    "admin@example.com"));

            Assert.Equal("update failure", exception.Message);
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task ApproveProviderRemoval_CabLookupThrows_PropagatesWithoutNotifications()
        {
            _repository.ApproveProviderRemoval(7, 17, "admin@example.com")
                .Returns(new GenericResponse { Success = true });
            _cabRepository.GetCabEmailListForServices(Arg.Any<List<int>>())
                .ThrowsAsync(new InvalidOperationException("cab failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ApproveProviderRemoval(
                    CreateProviderRemovalRequest(),
                    "admin@example.com"));

            Assert.Equal("cab failure", exception.Message);
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task ApproveProviderRemoval_NotificationThrows_PropagatesAfterRepositoryUpdate()
        {
            _repository.ApproveProviderRemoval(7, 17, "admin@example.com")
                .Returns(new GenericResponse { Success = true });
            _cabRepository.GetCabEmailListForServices(Arg.Any<List<int>>()).Returns([]);
            _emailSender.SendRemovalRequestConfirmedToDIP("Primary", "primary@example.com")
                .ThrowsAsync(new InvalidOperationException("notification failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ApproveProviderRemoval(
                    CreateProviderRemovalRequest(),
                    "admin@example.com"));

            Assert.Equal("notification failure", exception.Message);
            await _repository.Received(1)
                .ApproveProviderRemoval(7, 17, "admin@example.com");
            await _emailSender.DidNotReceive()
                .SendProviderRemovalConfirmationToDSIT(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task CancelProviderRemoval_SuccessfulUpdate_SendsDeclineNotificationsForAllServices()
        {
            var response = new GenericResponse { Success = true };
            var request = CreateProviderRemovalRequest();
            _repository.CancelRemoveProviderRequest(7, 17, "admin@example.com").Returns(response);

            var result = await _service.CancelProviderRemoval(request, "admin@example.com");

            Assert.Same(response, result);
            await _emailSender.Received(1)
                .RemovalRequestDeclinedToProvider("Primary", "primary@example.com");
            await _emailSender.Received(1)
                .RemovalRequestDeclinedToProvider("Secondary", "secondary@example.com");
            await _emailSender.Received(1)
                .RemovalRequestDeclinedToDSIT("Provider", "Service one\rService two\rService three");
        }

        [Fact]
        public async Task CancelProviderRemoval_AlreadyProcessedResponse_ReturnsFailureWithoutNotifications()
        {
            var response = new GenericResponse { Success = false, ErrorMessage = "Already cancelled" };
            _repository.CancelRemoveProviderRequest(7, 17, "admin@example.com").Returns(response);

            var result = await _service.CancelProviderRemoval(
                CreateProviderRemovalRequest(),
                "admin@example.com");

            Assert.Same(response, result);
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task CancelProviderRemoval_NotificationThrows_PropagatesAfterRepositoryUpdate()
        {
            _repository.CancelRemoveProviderRequest(7, 17, "admin@example.com")
                .Returns(new GenericResponse { Success = true });
            _emailSender.RemovalRequestDeclinedToProvider("Primary", "primary@example.com")
                .ThrowsAsync(new InvalidOperationException("notification failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CancelProviderRemoval(
                    CreateProviderRemovalRequest(),
                    "admin@example.com"));

            Assert.Equal("notification failure", exception.Message);
            await _repository.Received(1)
                .CancelRemoveProviderRequest(7, 17, "admin@example.com");
        }

        [Fact]
        public async Task ApproveServiceRemoval_PartialRemoval_SendsProviderAndDsitNotifications()
        {
            var response = new GenericResponse { Success = true, InstanceId = 71 };
            var request = CreateServiceRemovalRequest();
            _repository.ApproveServiceRemoval(9, 19, "admin@example.com").Returns(response);
            var reason = request.ServiceRemovalReason!.Value.GetDescription();

            var result = await _service.ApproveServiceRemoval(request, "admin@example.com");

            Assert.Same(response, result);
            await _emailSender.Received(1).RemoveServiceConfirmationToProvider(
                "Primary", "primary@example.com", "Removed service", reason);
            await _emailSender.Received(1).RemoveServiceConfirmationToProvider(
                "Secondary", "secondary@example.com", "Removed service", reason);
            await _emailSender.Received(1).ServiceRemovalConfirmationToDSIT(
                "Provider", "Removed service", reason);
        }

        [Fact]
        public async Task ApproveServiceRemoval_AlreadyRemovedResponse_ReturnsFailureWithoutNotifications()
        {
            var response = new GenericResponse { Success = false, ErrorMessage = "Already removed" };
            _repository.ApproveServiceRemoval(9, 19, "admin@example.com").Returns(response);

            var result = await _service.ApproveServiceRemoval(
                CreateServiceRemovalRequest(),
                "admin@example.com");

            Assert.Same(response, result);
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task ApproveServiceRemoval_RepositoryUpdateThrows_PropagatesWithoutNotifications()
        {
            _repository.ApproveServiceRemoval(9, 19, "admin@example.com")
                .ThrowsAsync(new InvalidOperationException("update failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ApproveServiceRemoval(
                    CreateServiceRemovalRequest(),
                    "admin@example.com"));

            Assert.Equal("update failure", exception.Message);
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task ApproveServiceRemoval_NotificationThrows_PropagatesAfterRepositoryUpdate()
        {
            _repository.ApproveServiceRemoval(9, 19, "admin@example.com")
                .Returns(new GenericResponse { Success = true });
            _emailSender.RemoveServiceConfirmationToProvider(
                    "Primary", "primary@example.com", Arg.Any<string>(), Arg.Any<string>())
                .ThrowsAsync(new InvalidOperationException("notification failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ApproveServiceRemoval(
                    CreateServiceRemovalRequest(),
                    "admin@example.com"));

            Assert.Equal("notification failure", exception.Message);
            await _repository.Received(1)
                .ApproveServiceRemoval(9, 19, "admin@example.com");
        }

        [Fact]
        public async Task ApproveServiceRemoval_MissingRemovalReason_ThrowsAfterRepositoryUpdate()
        {
            var request = CreateServiceRemovalRequest();
            request.ServiceRemovalReason = null;
            _repository.ApproveServiceRemoval(9, 19, "admin@example.com")
                .Returns(new GenericResponse { Success = true });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ApproveServiceRemoval(request, "admin@example.com"));

            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task CancelServiceRemoval_SuccessfulUpdate_SendsDeclineNotifications()
        {
            var response = new GenericResponse { Success = true };
            var request = CreateServiceRemovalRequest();
            _repository.CancelRemoveServiceRequest(9, 19, "admin@example.com").Returns(response);

            var result = await _service.CancelServiceRemoval(request, "admin@example.com");

            Assert.Same(response, result);
            await _emailSender.Received(1)
                .RemovalRequestDeclinedToProvider("Primary", "primary@example.com");
            await _emailSender.Received(1)
                .RemovalRequestDeclinedToProvider("Secondary", "secondary@example.com");
            await _emailSender.Received(1)
                .RemovalRequestDeclinedToDSIT("Provider", "Removed service");
        }

        [Fact]
        public async Task CancelServiceRemoval_AlreadyProcessedResponse_ReturnsFailureWithoutNotifications()
        {
            var response = new GenericResponse { Success = false, ErrorMessage = "Already cancelled" };
            _repository.CancelRemoveServiceRequest(9, 19, "admin@example.com").Returns(response);

            var result = await _service.CancelServiceRemoval(
                CreateServiceRemovalRequest(),
                "admin@example.com");

            Assert.Same(response, result);
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task CancelServiceRemoval_NotificationThrows_PropagatesAfterRepositoryUpdate()
        {
            _repository.CancelRemoveServiceRequest(9, 19, "admin@example.com")
                .Returns(new GenericResponse { Success = true });
            _emailSender.RemovalRequestDeclinedToProvider("Primary", "primary@example.com")
                .ThrowsAsync(new InvalidOperationException("notification failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CancelServiceRemoval(
                    CreateServiceRemovalRequest(),
                    "admin@example.com"));

            Assert.Equal("notification failure", exception.Message);
            await _repository.Received(1)
                .CancelRemoveServiceRequest(9, 19, "admin@example.com");
        }

        private static ProviderRemovalRequestDto CreateProviderRemovalRequest()
        {
            return new ProviderRemovalRequestDto
            {
                Id = 17,
                ProviderProfileId = 7,
                RemovalReason = RemovalReasonsEnum.ProviderRequestedRemoval,
                Provider = new ProviderProfileDto
                {
                    Id = 7,
                    RegisteredName = "Provider",
                    PrimaryContactFullName = "Primary",
                    PrimaryContactEmail = "primary@example.com",
                    SecondaryContactFullName = "Secondary",
                    SecondaryContactEmail = "secondary@example.com",
                    Services =
                    [
                        new ServiceDto
                        {
                            Id = 1,
                            ServiceName = "Service one",
                            ServiceStatus = ServiceStatusEnum.AwaitingRemovalConfirmation
                        },
                        new ServiceDto
                        {
                            Id = 2,
                            ServiceName = "Service two",
                            ServiceStatus = ServiceStatusEnum.Published
                        },
                        new ServiceDto
                        {
                            Id = 3,
                            ServiceName = "Service three",
                            ServiceStatus = ServiceStatusEnum.AwaitingRemovalConfirmation
                        }
                    ]
                }
            };
        }

        private static ServiceRemovalRequestDto CreateServiceRemovalRequest()
        {
            return new ServiceRemovalRequestDto
            {
                Id = 19,
                ServiceId = 9,
                ServiceRemovalReason = ServiceRemovalReasonEnum.ProviderRequestedRemoval,
                Service = new ServiceDto
                {
                    Id = 9,
                    ServiceName = "Removed service",
                    Provider = new ProviderProfileDto
                    {
                        Id = 7,
                        RegisteredName = "Provider",
                        PrimaryContactFullName = "Primary",
                        PrimaryContactEmail = "primary@example.com",
                        SecondaryContactFullName = "Secondary",
                        SecondaryContactEmail = "secondary@example.com"
                    }
                }
            };
        }
    }
}
