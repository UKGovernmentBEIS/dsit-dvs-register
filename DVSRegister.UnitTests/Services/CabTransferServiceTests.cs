using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services.CabTransfer;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.CabTransfer;
using DVSRegister.Data.Entities;
using NSubstitute;

namespace DVSRegister.UnitTests.Services
{
    public class CabTransferServiceTests
    {
        private const int RequestId = 11;
        private const int ProviderProfileId = 22;
        private const string LoggedInUserEmail = "decision-maker@example.com";
        private readonly ICabTransferRepository _repository;
        private readonly ICabTransferEmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly CabTransferService _service;

        public CabTransferServiceTests()
        {
            _repository = Substitute.For<ICabTransferRepository>();
            _emailSender = Substitute.For<ICabTransferEmailSender>();
            _mapper = Substitute.For<IMapper>();
            _service = new CabTransferService(_repository, _emailSender, _mapper);
        }

        [Fact]
        public async Task GetServiceDetailsWithCabTransferDetails_ValidIds_ReturnsMappedService()
        {
            var service = new Service { Id = 7 };
            var expected = new ServiceDto { Id = 7 };
            _repository.GetServiceDetailsWithCabTransferDetails(7, 8).Returns(service);
            _mapper.Map<ServiceDto>(service).Returns(expected);

            var result = await _service.GetServiceDetailsWithCabTransferDetails(7, 8);

            Assert.Same(expected, result);
            await _repository.Received(1).GetServiceDetailsWithCabTransferDetails(7, 8);
            _mapper.Received(1).Map<ServiceDto>(service);
        }

        [Fact]
        public async Task GetServiceDetailsWithCabTransferDetails_InvalidIds_MapsRepositoryResultUnchanged()
        {
            _repository.GetServiceDetailsWithCabTransferDetails(-1, 0).Returns((Service)null!);
            var expected = new ServiceDto();
            _mapper.Map<ServiceDto>(null!).Returns(expected);

            var result = await _service.GetServiceDetailsWithCabTransferDetails(-1, 0);

            Assert.Same(expected, result);
            _mapper.Received(1).Map<ServiceDto>(null!);
        }

        [Fact]
        public async Task GetServiceDetailsWithCabTransferDetails_RepositoryThrows_PropagatesExceptionWithoutMapping()
        {
            var expected = new InvalidOperationException("Service lookup failed");
            _repository.GetServiceDetailsWithCabTransferDetails(7, 8)
                .Returns(Task.FromException<Service>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetServiceDetailsWithCabTransferDetails(7, 8));

            Assert.Same(expected, exception);
            Assert.Empty(_mapper.ReceivedCalls());
        }

        [Fact]
        public async Task GetServiceDetailsWithCabTransferDetails_MapperThrows_PropagatesException()
        {
            var service = new Service();
            var expected = new AutoMapperMappingException("Mapping failed");
            _repository.GetServiceDetailsWithCabTransferDetails(7, 8).Returns(service);
            _mapper.Map<ServiceDto>(service).Returns(_ => throw expected);

            var exception = await Assert.ThrowsAsync<AutoMapperMappingException>(() =>
                _service.GetServiceDetailsWithCabTransferDetails(7, 8));

            Assert.Same(expected, exception);
        }

        [Fact]
        public async Task GetCabTransferRequestDetails_ValidRequestId_ReturnsMappedRequest()
        {
            var request = new CabTransferRequest { Id = RequestId };
            var expected = new CabTransferRequestDto { Id = RequestId };
            _repository.GetCabTransferRequestDetails(RequestId).Returns(request);
            _mapper.Map<CabTransferRequestDto>(request).Returns(expected);

            var result = await _service.GetCabTransferRequestDetails(RequestId);

            Assert.Same(expected, result);
            await _repository.Received(1).GetCabTransferRequestDetails(RequestId);
            _mapper.Received(1).Map<CabTransferRequestDto>(request);
        }

        [Fact]
        public async Task GetCabTransferRequestDetails_InvalidRequestId_MapsRepositoryResultUnchanged()
        {
            _repository.GetCabTransferRequestDetails(-1).Returns((CabTransferRequest)null!);
            var expected = new CabTransferRequestDto();
            _mapper.Map<CabTransferRequestDto>(null!).Returns(expected);

            var result = await _service.GetCabTransferRequestDetails(-1);

            Assert.Same(expected, result);
            _mapper.Received(1).Map<CabTransferRequestDto>(null!);
        }

        [Fact]
        public async Task GetCabTransferRequestDetails_RepositoryThrows_PropagatesExceptionWithoutMapping()
        {
            var expected = new InvalidOperationException("Request lookup failed");
            _repository.GetCabTransferRequestDetails(RequestId)
                .Returns(Task.FromException<CabTransferRequest>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetCabTransferRequestDetails(RequestId));

            Assert.Same(expected, exception);
            Assert.Empty(_mapper.ReceivedCalls());
        }

        [Fact]
        public async Task GetCabTransferRequestDetails_MapperThrows_PropagatesException()
        {
            var request = new CabTransferRequest();
            var expected = new AutoMapperMappingException("Mapping failed");
            _repository.GetCabTransferRequestDetails(RequestId).Returns(request);
            _mapper.Map<CabTransferRequestDto>(request).Returns(_ => throw expected);

            var exception = await Assert.ThrowsAsync<AutoMapperMappingException>(() =>
                _service.GetCabTransferRequestDetails(RequestId));

            Assert.Same(expected, exception);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ApproveOrCancelTransferRequest_InvalidRequestId_ReturnsFailureWithoutNotifications(bool approve)
        {
            var expected = new GenericResponse { Success = false, ErrorMessage = "Invalid request" };
            ConfigureDecisionResponse(approve, expected, requestId: -1);

            var result = await _service.ApproveOrCancelTransferRequest(
                approve, -1, ProviderProfileId, LoggedInUserEmail);

            Assert.Same(expected, result);
            await _repository.Received(1).ApproveOrCancelTransferRequest(
                approve, -1, ProviderProfileId, LoggedInUserEmail);
            await _repository.DidNotReceiveWithAnyArgs().GetCabTransferRequestDetails(default);
            await _repository.DidNotReceiveWithAnyArgs().GetActiveCabUsers(default);
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ApproveOrCancelTransferRequest_AlreadyProcessedRequest_ReturnsFailureWithoutNotifications(bool approve)
        {
            var expected = new GenericResponse { Success = false, ErrorMessage = "Already processed" };
            ConfigureDecisionResponse(approve, expected);

            var result = await _service.ApproveOrCancelTransferRequest(
                approve, RequestId, ProviderProfileId, LoggedInUserEmail);

            Assert.Same(expected, result);
            await _repository.DidNotReceiveWithAnyArgs().GetCabTransferRequestDetails(default);
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task ApproveOrCancelTransferRequest_RepositoryThrows_PropagatesExceptionWithoutNotifications()
        {
            var expected = new InvalidOperationException("Decision failed");
            _repository.ApproveOrCancelTransferRequest(
                    true, RequestId, ProviderProfileId, LoggedInUserEmail)
                .Returns(Task.FromException<GenericResponse>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ApproveOrCancelTransferRequest(
                    true, RequestId, ProviderProfileId, LoggedInUserEmail));

            Assert.Same(expected, exception);
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task ApproveOrCancelTransferRequest_Approval_NotifiesAllCabUsersAndDsit()
        {
            var expected = ConfigureSuccessfulDecision(true);
            var request = CreateRequest();
            var acceptingUsers = new List<CabUser>
            {
                CreateCabUser(20, "Accepting CAB", "accepting-one@example.com"),
                CreateCabUser(20, "Accepting CAB", "accepting-two@example.com")
            };
            var currentUsers = new List<CabUser>
            {
                CreateCabUser(10, "Current CAB", "current-one@example.com"),
                CreateCabUser(10, "Current CAB", "current-two@example.com")
            };
            ConfigureRequestAndUsers(request, acceptingUsers, currentUsers);

            var result = await _service.ApproveOrCancelTransferRequest(
                true, RequestId, ProviderProfileId, LoggedInUserEmail);

            Assert.Same(expected, result);
            await _emailSender.Received(1).SendCabTransferConfirmationToCabB(
                "accepting-one@example.com", "Accepting CAB", "Provider", "Service");
            await _emailSender.Received(1).SendCabTransferConfirmationToCabB(
                "accepting-two@example.com", "Accepting CAB", "Provider", "Service");
            await _emailSender.Received(1).SendCabTransferConfirmationToDSIT(
                "Current CAB", "Accepting CAB", "Provider", "Service");
            await _emailSender.Received(1).SendCabTransferConfirmationToCabA(
                "current-one@example.com", "Current CAB", "Accepting CAB", "Provider", "Service");
            await _emailSender.Received(1).SendCabTransferConfirmationToCabA(
                "current-two@example.com", "Current CAB", "Accepting CAB", "Provider", "Service");
            await _repository.Received(1).GetActiveCabUsers(20);
            await _repository.Received(1).GetActiveCabUsers(10);
        }

        [Fact]
        public async Task ApproveOrCancelTransferRequest_ApprovalWithNoActiveUsers_UsesEmptyCabNamesAndStillNotifiesDsit()
        {
            var expected = ConfigureSuccessfulDecision(true);
            var request = CreateRequest();
            ConfigureRequestAndUsers(request, new List<CabUser>(), new List<CabUser>());

            var result = await _service.ApproveOrCancelTransferRequest(
                true, RequestId, ProviderProfileId, LoggedInUserEmail);

            Assert.Same(expected, result);
            await _emailSender.Received(1).SendCabTransferConfirmationToDSIT(
                "Current CAB", string.Empty, "Provider", "Service");
            await _emailSender.DidNotReceiveWithAnyArgs()
                .SendCabTransferConfirmationToCabB(default!, default!, default!, default!);
            await _emailSender.DidNotReceiveWithAnyArgs()
                .SendCabTransferConfirmationToCabA(default!, default!, default!, default!, default!);
        }

        [Fact]
        public async Task ApproveOrCancelTransferRequest_ApprovalWithNoCurrentCabUsers_SkipsCurrentCabEmails()
        {
            ConfigureSuccessfulDecision(true);
            var request = CreateRequest();
            ConfigureRequestAndUsers(
                request,
                new List<CabUser> { CreateCabUser(20, "Accepting CAB", "accepting@example.com") },
                new List<CabUser>());

            var result = await _service.ApproveOrCancelTransferRequest(
                true, RequestId, ProviderProfileId, LoggedInUserEmail);

            Assert.True(result.Success);
            await _emailSender.Received(1).SendCabTransferConfirmationToCabB(
                "accepting@example.com", "Accepting CAB", "Provider", "Service");
            await _emailSender.DidNotReceiveWithAnyArgs()
                .SendCabTransferConfirmationToCabA(default!, default!, default!, default!, default!);
        }

        [Fact]
        public async Task ApproveOrCancelTransferRequest_Cancellation_NotifiesAcceptingCabUsersAndDsitOnly()
        {
            var expected = ConfigureSuccessfulDecision(false);
            var request = CreateRequest();
            var acceptingUsers = new List<CabUser>
            {
                CreateCabUser(20, "Declining CAB", "first@example.com"),
                CreateCabUser(20, "Declining CAB", "second@example.com")
            };
            _repository.GetCabTransferRequestDetails(RequestId).Returns(request);
            _repository.GetActiveCabUsers(20).Returns(acceptingUsers);

            var result = await _service.ApproveOrCancelTransferRequest(
                false, RequestId, ProviderProfileId, LoggedInUserEmail);

            Assert.Same(expected, result);
            await _emailSender.Received(1).SendCabTransferCancellationToCabB(
                "first@example.com", "Declining CAB", "Provider", "Service");
            await _emailSender.Received(1).SendCabTransferCancellationToCabB(
                "second@example.com", "Declining CAB", "Provider", "Service");
            await _emailSender.Received(1).SendCabTransferCancellationToDSIT(
                "Current CAB", "Declining CAB", "Provider", "Service");
            await _repository.DidNotReceive().GetActiveCabUsers(10);
            await _emailSender.DidNotReceiveWithAnyArgs()
                .SendCabTransferConfirmationToCabA(default!, default!, default!, default!, default!);
        }

        [Fact]
        public async Task ApproveOrCancelTransferRequest_CancellationWithNoActiveUsers_UsesEmptyCabNameAndStillNotifiesDsit()
        {
            ConfigureSuccessfulDecision(false);
            var request = CreateRequest();
            _repository.GetCabTransferRequestDetails(RequestId).Returns(request);
            _repository.GetActiveCabUsers(20).Returns(new List<CabUser>());

            var result = await _service.ApproveOrCancelTransferRequest(
                false, RequestId, ProviderProfileId, LoggedInUserEmail);

            Assert.True(result.Success);
            await _emailSender.Received(1).SendCabTransferCancellationToDSIT(
                "Current CAB", string.Empty, "Provider", "Service");
            await _emailSender.DidNotReceiveWithAnyArgs()
                .SendCabTransferCancellationToCabB(default!, default!, default!, default!);
        }

        [Fact]
        public async Task ApproveOrCancelTransferRequest_EmailReturnsFalse_ReturnsSuccessAndContinuesNotifications()
        {
            var expected = ConfigureSuccessfulDecision(true);
            var request = CreateRequest();
            ConfigureRequestAndUsers(
                request,
                new List<CabUser> { CreateCabUser(20, "Accepting CAB", "accepting@example.com") },
                new List<CabUser> { CreateCabUser(10, "Current CAB", "current@example.com") });
            _emailSender.SendCabTransferConfirmationToCabB(
                    "accepting@example.com", "Accepting CAB", "Provider", "Service")
                .Returns(false);

            var result = await _service.ApproveOrCancelTransferRequest(
                true, RequestId, ProviderProfileId, LoggedInUserEmail);

            Assert.Same(expected, result);
            await _emailSender.Received(1).SendCabTransferConfirmationToDSIT(
                "Current CAB", "Accepting CAB", "Provider", "Service");
            await _emailSender.Received(1).SendCabTransferConfirmationToCabA(
                "current@example.com", "Current CAB", "Accepting CAB", "Provider", "Service");
        }

        [Fact]
        public async Task ApproveOrCancelTransferRequest_RequestDetailsLookupThrows_PropagatesExceptionAfterSuccessfulUpdate()
        {
            ConfigureSuccessfulDecision(true);
            var expected = new InvalidOperationException("Request details unavailable");
            _repository.GetCabTransferRequestDetails(RequestId)
                .Returns(Task.FromException<CabTransferRequest>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ApproveOrCancelTransferRequest(
                    true, RequestId, ProviderProfileId, LoggedInUserEmail));

            Assert.Same(expected, exception);
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task ApproveOrCancelTransferRequest_AcceptingCabLookupThrows_PropagatesExceptionWithoutNotifications()
        {
            ConfigureSuccessfulDecision(true);
            _repository.GetCabTransferRequestDetails(RequestId).Returns(CreateRequest());
            var expected = new InvalidOperationException("CAB users unavailable");
            _repository.GetActiveCabUsers(20)
                .Returns(Task.FromException<List<CabUser>>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ApproveOrCancelTransferRequest(
                    true, RequestId, ProviderProfileId, LoggedInUserEmail));

            Assert.Same(expected, exception);
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task ApproveOrCancelTransferRequest_AcceptingCabEmailThrows_PropagatesExceptionBeforeDsitNotification()
        {
            ConfigureSuccessfulDecision(true);
            var request = CreateRequest();
            _repository.GetCabTransferRequestDetails(RequestId).Returns(request);
            _repository.GetActiveCabUsers(20).Returns(new List<CabUser>
            {
                CreateCabUser(20, "Accepting CAB", "accepting@example.com")
            });
            var expected = new InvalidOperationException("Notify unavailable");
            _emailSender.SendCabTransferConfirmationToCabB(
                    "accepting@example.com", "Accepting CAB", "Provider", "Service")
                .Returns(Task.FromException<bool>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ApproveOrCancelTransferRequest(
                    true, RequestId, ProviderProfileId, LoggedInUserEmail));

            Assert.Same(expected, exception);
            await _emailSender.DidNotReceiveWithAnyArgs()
                .SendCabTransferConfirmationToDSIT(default!, default!, default!, default!);
            await _repository.DidNotReceive().GetActiveCabUsers(10);
        }

        [Fact]
        public async Task ApproveOrCancelTransferRequest_CurrentCabLookupThrows_PropagatesExceptionAfterAcceptingAndDsitNotifications()
        {
            ConfigureSuccessfulDecision(true);
            var request = CreateRequest();
            _repository.GetCabTransferRequestDetails(RequestId).Returns(request);
            _repository.GetActiveCabUsers(20).Returns(new List<CabUser>
            {
                CreateCabUser(20, "Accepting CAB", "accepting@example.com")
            });
            var expected = new InvalidOperationException("Current CAB users unavailable");
            _repository.GetActiveCabUsers(10).Returns(Task.FromException<List<CabUser>>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ApproveOrCancelTransferRequest(
                    true, RequestId, ProviderProfileId, LoggedInUserEmail));

            Assert.Same(expected, exception);
            await _emailSender.Received(1).SendCabTransferConfirmationToCabB(
                "accepting@example.com", "Accepting CAB", "Provider", "Service");
            await _emailSender.Received(1).SendCabTransferConfirmationToDSIT(
                "Current CAB", "Accepting CAB", "Provider", "Service");
        }

        [Fact]
        public async Task ApproveOrCancelTransferRequest_CancellationEmailThrows_PropagatesExceptionBeforeDsitNotification()
        {
            ConfigureSuccessfulDecision(false);
            var request = CreateRequest();
            _repository.GetCabTransferRequestDetails(RequestId).Returns(request);
            _repository.GetActiveCabUsers(20).Returns(new List<CabUser>
            {
                CreateCabUser(20, "Declining CAB", "declining@example.com")
            });
            var expected = new InvalidOperationException("Notify unavailable");
            _emailSender.SendCabTransferCancellationToCabB(
                    "declining@example.com", "Declining CAB", "Provider", "Service")
                .Returns(Task.FromException<bool>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ApproveOrCancelTransferRequest(
                    false, RequestId, ProviderProfileId, LoggedInUserEmail));

            Assert.Same(expected, exception);
            await _emailSender.DidNotReceiveWithAnyArgs()
                .SendCabTransferCancellationToDSIT(default!, default!, default!, default!);
        }

        private void ConfigureDecisionResponse(
            bool approve,
            GenericResponse response,
            int requestId = RequestId)
        {
            _repository.ApproveOrCancelTransferRequest(
                    approve, requestId, ProviderProfileId, LoggedInUserEmail)
                .Returns(response);
        }

        private GenericResponse ConfigureSuccessfulDecision(bool approve)
        {
            var response = new GenericResponse { Success = true };
            ConfigureDecisionResponse(approve, response);
            return response;
        }

        private void ConfigureRequestAndUsers(
            CabTransferRequest request,
            List<CabUser> acceptingUsers,
            List<CabUser> currentUsers)
        {
            _repository.GetCabTransferRequestDetails(RequestId).Returns(request);
            _repository.GetActiveCabUsers(20).Returns(acceptingUsers);
            _repository.GetActiveCabUsers(10).Returns(currentUsers);
        }

        private static CabTransferRequest CreateRequest()
        {
            return new CabTransferRequest
            {
                Id = RequestId,
                ToCabId = 20,
                Service = new Service
                {
                    ServiceName = "Service",
                    Provider = new ProviderProfile { RegisteredName = "Provider" }
                },
                FromCabUser = CreateCabUser(10, "Current CAB", "original@example.com")
            };
        }

        private static CabUser CreateCabUser(int cabId, string cabName, string email)
        {
            return new CabUser
            {
                CabId = cabId,
                CabEmail = email,
                Cab = new Cab { Id = cabId, CabName = cabName }
            };
        }
    }
}
