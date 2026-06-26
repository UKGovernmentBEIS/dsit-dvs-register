using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.CabRemovalRequest;
using NSubstitute;

namespace DVSRegister.UnitTests.Services.CabRemovalRequest;

public class CabRemovalRequestServiceTests
{
    private readonly ICabRemovalRequestRepository _repository;
    private readonly ICabService _cabService;
    private readonly ICabRemovalRequestEmailSender _emailSender;
    private readonly CabRemovalRequestService _service;

    public CabRemovalRequestServiceTests()
    {
        _repository = Substitute.For<ICabRemovalRequestRepository>();
        _cabService = Substitute.For<ICabService>();
        _emailSender = Substitute.For<ICabRemovalRequestEmailSender>();

        _service = new CabRemovalRequestService(
            _repository,
            _cabService,
            _emailSender
        );
    }

    #region AddServiceRemovalrequest

    [Fact]
    public async Task AddServiceRemovalrequest_ProviderRemoval_Success_ReturnsSuccessResponse()
    {
        var cabId = 1;
        var serviceId = 2;
        var email = "cab@example.com";
        var reason = "No longer needed";
        var isProviderRemoval = true;

        _repository.AddServiceRemovalRequest(cabId, serviceId, email, reason)
            .Returns(new GenericResponse { Success = true });

        var serviceDto = CreateServiceDto("Acme Corp", "Acme Service");
        _cabService.GetServiceDetailsWithProvider(serviceId, cabId)
            .Returns(serviceDto);

        var result = await _service.AddServiceRemovalrequest(cabId, serviceId, email, reason, isProviderRemoval);

        Assert.True(result.Success);
        await _repository.Received(1).AddServiceRemovalRequest(cabId, serviceId, email, reason);
        await _cabService.Received(1).GetServiceDetailsWithProvider(serviceId, cabId);
    }

    [Fact]
    public async Task AddServiceRemovalrequest_ProviderRemoval_ServiceFound_CallsProviderRemovalEmails()
    {
        var cabId = 1;
        var serviceId = 2;
        var email = "cab@example.com";
        var reason = "No longer needed";
        var isProviderRemoval = true;

        _repository.AddServiceRemovalRequest(cabId, serviceId, email, reason)
            .Returns(new GenericResponse { Success = true });

        var serviceDto = CreateServiceDto("Acme Corp", "Acme Service");
        _cabService.GetServiceDetailsWithProvider(serviceId, cabId)
            .Returns(serviceDto);

        var result = await _service.AddServiceRemovalrequest(cabId, serviceId, email, reason, isProviderRemoval);

        Assert.True(result.Success);
        await _emailSender.Received(1)
            .RecordRemovalRequestByCabToDSIT("Acme Corp", "Acme Service", reason);
        await _emailSender.Received(1)
            .RecordRemovalRequestConfirmationToCab(email, email, "Acme Corp", "Acme Service", reason);
    }

    [Fact]
    public async Task AddServiceRemovalrequest_ServiceRemoval_Success_ReturnsSuccessResponse()
    {
        var cabId = 1;
        var serviceId = 3;
        var email = "cab@example.com";
        var reason = "Retiring service";
        var isProviderRemoval = false;

        _repository.AddServiceRemovalRequest(cabId, serviceId, email, reason)
            .Returns(new GenericResponse { Success = true });

        var serviceDto = CreateServiceDto("Beta Ltd", "Beta Service");
        _cabService.GetServiceDetailsWithProvider(serviceId, cabId)
            .Returns(serviceDto);

        var result = await _service.AddServiceRemovalrequest(cabId, serviceId, email, reason, isProviderRemoval);

        Assert.True(result.Success);
        await _repository.Received(1).AddServiceRemovalRequest(cabId, serviceId, email, reason);
        await _cabService.Received(1).GetServiceDetailsWithProvider(serviceId, cabId);
    }

    [Fact]
    public async Task AddServiceRemovalrequest_ServiceRemoval_ServiceFound_CallsServiceRemovalEmails()
    {
        var cabId = 1;
        var serviceId = 4;
        var email = "cab@example.com";
        var reason = "Retiring service";
        var isProviderRemoval = false;

        _repository.AddServiceRemovalRequest(cabId, serviceId, email, reason)
            .Returns(new GenericResponse { Success = true });

        var serviceDto = CreateServiceDto("Beta Ltd", "Beta Service");
        _cabService.GetServiceDetailsWithProvider(serviceId, cabId)
            .Returns(serviceDto);

        var result = await _service.AddServiceRemovalrequest(cabId, serviceId, email, reason, isProviderRemoval);

        Assert.True(result.Success);
        await _emailSender.Received(1)
            .CabServiceRemovalRequested(email, email, "Beta Ltd", "Beta Service", reason);
        await _emailSender.Received(1)
            .CabServiceRemovalRequestedToDSIT("Beta Ltd", "Beta Service", reason);
    }

    [Fact]
    public async Task AddServiceRemovalrequest_ProviderRemoval_ServiceNotFound_NoEmailsSent()
    {
        var cabId = 1;
        var serviceId = 5;
        var email = "cab@example.com";
        var reason = "No longer needed";
        var isProviderRemoval = true;

        _repository.AddServiceRemovalRequest(cabId, serviceId, email, reason)
            .Returns(new GenericResponse { Success = true });

        _cabService.GetServiceDetailsWithProvider(serviceId, cabId)
            .Returns(default(ServiceDto?));

        var result = await _service.AddServiceRemovalrequest(cabId, serviceId, email, reason, isProviderRemoval);

        Assert.True(result.Success);
        await _emailSender.DidNotReceive()
            .RecordRemovalRequestByCabToDSIT(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task AddServiceRemovalrequest_ServiceRemoval_ServiceNotFound_NoEmailsSent()
    {
        var cabId = 1;
        var serviceId = 5;
        var email = "cab@example.com";
        var reason = "Retiring";
        var isProviderRemoval = false;

        _repository.AddServiceRemovalRequest(cabId, serviceId, email, reason)
            .Returns(new GenericResponse { Success = true });

        _cabService.GetServiceDetailsWithProvider(serviceId, cabId)
            .Returns(default(ServiceDto?));

        var result = await _service.AddServiceRemovalrequest(cabId, serviceId, email, reason, isProviderRemoval);

        Assert.True(result.Success);
        await _emailSender.DidNotReceive().CabServiceRemovalRequested(Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task AddServiceRemovalrequest_RepositoryFailure_ReturnsFailureResponse()
    {
        var cabId = 1;
        var serviceId = 6;
        var email = "cab@example.com";
        var reason = "Failure case";
        var isProviderRemoval = true;

        _repository.AddServiceRemovalRequest(cabId, serviceId, email, reason)
            .Returns(new GenericResponse { Success = false });

        var result = await _service.AddServiceRemovalrequest(cabId, serviceId, email, reason, isProviderRemoval);

        Assert.False(result.Success);
        await _cabService.DidNotReceive().GetServiceDetailsWithProvider(Arg.Any<int>(), Arg.Any<int>());
    }

    [Fact]
    public async Task AddServiceRemovalrequest_ProviderRemoval_WithEmptyReason_SendsEmailsWithEmptyReason()
    {
        var cabId = 1;
        var serviceId = 7;
        var email = "cab@example.com";
        var reason = "";
        var isProviderRemoval = true;

        _repository.AddServiceRemovalRequest(cabId, serviceId, email, reason)
            .Returns(new GenericResponse { Success = true });

        var serviceDto = CreateServiceDto("Gamma Inc", "Gamma Service");
        _cabService.GetServiceDetailsWithProvider(serviceId, cabId)
            .Returns(serviceDto);

        var result = await _service.AddServiceRemovalrequest(cabId, serviceId, email, reason, isProviderRemoval);

        Assert.True(result.Success);
        await _emailSender.Received(1)
            .RecordRemovalRequestByCabToDSIT("Gamma Inc", "Gamma Service", "");
    }

    [Theory]
    [InlineData(0, 1, true)]
    [InlineData(-1, 1, true)]
    [InlineData(1, 0, true)]
    [InlineData(0, 1, false)]
    [InlineData(-1, 1, false)]
    [InlineData(1, 0, false)]
    public async Task AddServiceRemovalrequest_EdgeCaseIds_FlowsThroughRepository(
        int cabId, int serviceId, bool isProviderRemoval)
    {
        var email = "cab@example.com";
        var reason = "Test reason";

        _repository.AddServiceRemovalRequest(cabId, serviceId, email, reason)
            .Returns(new GenericResponse { Success = true });

        _cabService.GetServiceDetailsWithProvider(serviceId, cabId)
            .Returns(CreateServiceDto("Edge Corp", "Edge Service"));

        var result = await _service.AddServiceRemovalrequest(cabId, serviceId, email, reason, isProviderRemoval);

        Assert.True(result.Success);
    }

    #endregion

    #region IsLastService

    [Fact]
    public async Task IsLastService_RepositoryReturnsTrue_ReturnsTrue()
    {
        var serviceId = 1;
        var providerProfileId = 2;

        _repository.IsLastService(serviceId, providerProfileId)
            .Returns(true);

        var result = await _service.IsLastService(serviceId, providerProfileId);

        Assert.True(result);
        await _repository.Received(1).IsLastService(serviceId, providerProfileId);
    }

    [Fact]
    public async Task IsLastService_RepositoryReturnsFalse_ReturnsFalse()
    {
        var serviceId = 1;
        var providerProfileId = 2;

        _repository.IsLastService(serviceId, providerProfileId)
            .Returns(false);

        var result = await _service.IsLastService(serviceId, providerProfileId);

        Assert.False(result);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(-1, 5)]
    public async Task IsLastService_EdgeCaseIds_DelegatesToRepository(int serviceId, int providerProfileId)
    {
        _repository.IsLastService(serviceId, providerProfileId)
            .Returns(true);

        var result = await _service.IsLastService(serviceId, providerProfileId);

        Assert.True(result);
        await _repository.Received(1).IsLastService(serviceId, providerProfileId);
    }

    #endregion

    #region CancelServiceRemovalRequest

    [Fact]
    public async Task CancelServiceRemovalRequest_Success_ReturnsSuccessResponse()
    {
        var cabId = 1;
        var serviceId = 2;
        var email = "cab@example.com";

        _repository.CancelServiceRemovalRequest(serviceId, email)
            .Returns(new GenericResponse { Success = true });

        var serviceDto = CreateServiceDtoWithCabUser("Delta Ltd", "Delta Service", "Delta CAB");
        _cabService.GetServiceDetailsWithProvider(serviceId, cabId)
            .Returns(serviceDto);

        var result = await _service.CancelServiceRemovalRequest(cabId, serviceId, email);

        Assert.True(result.Success);

        await _repository.Received(1).CancelServiceRemovalRequest(serviceId, email);
        await _cabService.Received(1).GetServiceDetailsWithProvider(serviceId, cabId);
    }

    [Fact]
    public async Task CancelServiceRemovalRequest_Success_CallsCancelEmails()
    {
        var cabId = 1;
        var serviceId = 2;
        var email = "cab@example.com";

        _repository.CancelServiceRemovalRequest(serviceId, email)
            .Returns(new GenericResponse { Success = true });

        var serviceDto = CreateServiceDtoWithCabUser("Delta Ltd", "Delta Service", "Delta CAB");
        _cabService.GetServiceDetailsWithProvider(serviceId, cabId)
            .Returns(serviceDto);

        await _service.CancelServiceRemovalRequest(cabId, serviceId, email);

        await _emailSender.Received(1)
            .RemovalRequestCancelledToCab(email, "Delta CAB", "Delta Ltd", "Delta Service");
        await _emailSender.Received(1)
            .RemovalRequestCancelledToDSIT("Delta CAB", "Delta Ltd", "Delta Service");
    }

    [Fact]
    public async Task CancelServiceRemovalRequest_RepositoryFailure_ReturnsFailureAndNoEmails()
    {
        var cabId = 1;
        var serviceId = 2;
        var email = "cab@example.com";

        _repository.CancelServiceRemovalRequest(serviceId, email)
            .Returns(new GenericResponse { Success = false });

        var result = await _service.CancelServiceRemovalRequest(cabId, serviceId, email);

        Assert.False(result.Success);

        await _cabService.DidNotReceive()
            .GetServiceDetailsWithProvider(Arg.Any<int>(), Arg.Any<int>());
    }

    [Fact]
    public async Task CancelServiceRemovalRequest_ServiceNotFound_NoEmailsSent()
    {
        var cabId = 1;
        var serviceId = 2;
        var email = "cab@example.com";

        _repository.CancelServiceRemovalRequest(serviceId, email)
            .Returns(new GenericResponse { Success = true });

        _cabService.GetServiceDetailsWithProvider(serviceId, cabId)
            .Returns(default(ServiceDto?));

        var result = await _service.CancelServiceRemovalRequest(cabId, serviceId, email);

        Assert.True(result.Success);
        await _emailSender.DidNotReceive().RemovalRequestCancelledToCab(Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task CancelServiceRemovalRequest_CabUserNull_NoEmailsSent()
    {
        var cabId = 1;
        var serviceId = 2;
        var email = "cab@example.com";

        _repository.CancelServiceRemovalRequest(serviceId, email)
            .Returns(new GenericResponse { Success = true });

        var serviceDto = new ServiceDto
        {
            Id = 1,
            ServiceName = "Test Service",
            Provider = new ProviderProfileDto { RegisteredName = "Test Provider" },
            CabUser = default!
        };
        _cabService.GetServiceDetailsWithProvider(serviceId, cabId)
            .Returns(serviceDto);

        var result = await _service.CancelServiceRemovalRequest(cabId, serviceId, email);

        Assert.True(result.Success);
        await _emailSender.DidNotReceive().RemovalRequestCancelledToCab(Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<string>());
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(-1, 1)]
    public async Task CancelServiceRemovalRequest_EdgeCaseIds_FlowsThroughRepository(int cabId, int serviceId)
    {
        var email = "cab@example.com";

        _repository.CancelServiceRemovalRequest(serviceId, email)
            .Returns(new GenericResponse { Success = true });

        _cabService.GetServiceDetailsWithProvider(serviceId, cabId)
            .Returns(CreateServiceDtoWithCabUser("Epsilon Ltd", "Epsilon Service", "Epsilon CAB"));

        var result = await _service.CancelServiceRemovalRequest(cabId, serviceId, email);

        Assert.True(result.Success);
    }

    #endregion

    #region Helpers

    private static ServiceDto CreateServiceDto(string providerName, string serviceName)
    {
        return new ServiceDto
        {
            Id = 1,
            ServiceName = serviceName,
            Provider = new ProviderProfileDto
            {
                RegisteredName = providerName
            }
        };
    }

    private static ServiceDto CreateServiceDtoWithCabUser(
        string providerName, string serviceName, string cabName)
    {
        return new ServiceDto
        {
            Id = 1,
            ServiceName = serviceName,
            Provider = new ProviderProfileDto
            {
                RegisteredName = providerName
            },
            CabUser = new CabUserDto
            {
                Cab = new CabDto
                {
                    CabName = cabName
                }
            }
        };
    }

    #endregion
}