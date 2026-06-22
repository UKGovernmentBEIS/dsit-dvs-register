using Amazon.CognitoIdentityProvider.Model;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.AWS;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Repositories;
using NSubstitute;

namespace DVSRegister.UnitTests.Services;

public class SignUpServiceTests
{
    private readonly ICognitoClient _cognitoClient;
    private readonly ILoginEmailSender _emailSender;
    private readonly IUserRepository _userRepository;
    private readonly SignUpService _service;

    public SignUpServiceTests()
    {
        _cognitoClient = Substitute.For<ICognitoClient>();
        _emailSender = Substitute.For<ILoginEmailSender>();
        _userRepository = Substitute.For<IUserRepository>();

        _service = new SignUpService(_cognitoClient, _emailSender, _userRepository);
    }

    #region ConfirmMFAToken

    [Fact]
    public async Task ConfirmMFAToken_ValidInput_ReturnsAuthenticationResult()
    {
        var session = "test-session";
        var email = "Test@Example.com";
        var token = "123456";
        var expectedResult = new AuthenticationResultType
        {
            AccessToken = "access-token",
            IdToken = "id-token"
        };

        _cognitoClient.ConfirmMFAToken(session, email.ToLower(), token)
            .Returns(Task.FromResult(expectedResult));

        var result = await _service.ConfirmMFAToken(session, email, token);

        Assert.NotNull(result);
        Assert.Equal(expectedResult.AccessToken, result.AccessToken);
        Assert.Equal(expectedResult.IdToken, result.IdToken);

        await _cognitoClient.Received(1).ConfirmMFAToken(session, email.ToLower(), token);
    }

    [Fact]
    public async Task ConfirmMFAToken_WithNullResult_ReturnsNull()
    {
        var session = "test-session";
        var email = "Test@Example.com";
        var token = "123456";

        _cognitoClient.ConfirmMFAToken(session, email.ToLower(), token)
            .Returns(Task.FromResult<AuthenticationResultType?>(null)!);

        var result = await _service.ConfirmMFAToken(session, email, token);

        Assert.Null(result);
    }

    #endregion

    #region ResetPassword

    [Fact]
    public async Task ResetPassword_ValidInput_ReturnsCognitoResponse()
    {
        var email = "Test@Example.com";
        var password = "NewPassword123!";
        var otp = "654321";
        var expected = new GenericResponse { Success = true };

        _cognitoClient.ConfirmPasswordReset(email.ToLower(), password, otp)
            .Returns(Task.FromResult(expected));

        var result = await _service.ResetPassword(email, password, otp);

        Assert.True(result.Success);

        await _cognitoClient.Received(1).ConfirmPasswordReset(email.ToLower(), password, otp);
    }

    [Fact]
    public async Task ResetPassword_FailureFromCognito_PropagatesFailure()
    {
        var email = "Test@Example.com";
        var password = "NewPassword123!";
        var otp = "654321";
        var expected = new GenericResponse
            { Success = false, Data = "Error while resetting password, please try again later" };

        _cognitoClient.ConfirmPasswordReset(email.ToLower(), password, otp)
            .Returns(Task.FromResult(expected));

        var result = await _service.ResetPassword(email, password, otp);

        Assert.False(result.Success);
        Assert.Equal(expected.Data, result.Data);
    }

    #endregion

    #region ConfirmPassword

    [Fact]
    public async Task ConfirmPassword_ValidInput_ReturnsCognitoResponse()
    {
        var email = "Test@Example.com";
        var password = "NewPassword123!";
        var otp = "123456";
        var expected = new GenericResponse { Success = true, Data = "secret-code" };

        _cognitoClient.ConfirmPasswordAndGenerateMFAToken(email.ToLower(), password, otp)
            .Returns(Task.FromResult(expected));

        var result = await _service.ConfirmPassword(email, password, otp);

        Assert.True(result.Success);
        Assert.Equal(expected.Data, result.Data);

        await _cognitoClient.Received(1).ConfirmPasswordAndGenerateMFAToken(email.ToLower(), password, otp);
    }

    [Fact]
    public async Task ConfirmPassword_InvalidCode_ReturnsFailure()
    {
        var email = "Test@Example.com";
        var password = "NewPassword123!";
        var otp = "000000";
        var expected = new GenericResponse { Success = false, ErrorMessage = "Invalid verification code provided" };

        _cognitoClient.ConfirmPasswordAndGenerateMFAToken(email.ToLower(), password, otp)
            .Returns(Task.FromResult(expected));

        var result = await _service.ConfirmPassword(email, password, otp);

        Assert.False(result.Success);
        Assert.Equal(expected.ErrorMessage, result.ErrorMessage);
    }

    #endregion

    #region ForgotPassword

    [Fact]
    public async Task ForgotPassword_ValidEmail_ReturnsCognitoResponse()
    {
        var email = "Test@Example.com";
        var expected = "OK";

        _cognitoClient.ForgotPassword(email.ToLower())
            .Returns(Task.FromResult(expected));

        var result = await _service.ForgotPassword(email);

        Assert.Equal(expected, result);

        await _cognitoClient.Received(1).ForgotPassword(email.ToLower());
    }

    [Fact]
    public async Task ForgotPassword_UserNotFound_ReturnsUserNotFoundMessage()
    {
        var email = "nobody@example.com";
        var expected = "Enter a valid email address";

        _cognitoClient.ForgotPassword(email.ToLower())
            .Returns(Task.FromResult(expected));

        var result = await _service.ForgotPassword(email);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task ForgotPassword_LimitExceeded_ReturnsLimitMessage()
    {
        var email = "Test@Example.com";
        var expected = "Attempt limit exceeded, please try after some time.";

        _cognitoClient.ForgotPassword(email.ToLower())
            .Returns(Task.FromResult(expected));

        var result = await _service.ForgotPassword(email);

        Assert.Equal(expected, result);
    }

    #endregion

    #region MFAConfirmation

    [Fact]
    public async Task MFAConfirmation_OK_UpdatesStatusAndSendsEmails()
    {
        var email = "cab@provider.com";
        var password = "Password123!";
        var mfaCode = "123456";

        _cognitoClient.MFARegistrationConfirmation(email, password, mfaCode)
            .Returns(Task.FromResult("OK"));

        _userRepository.UpdateAccountStatus(email, AccountStatusEnum.Active, email)
            .Returns(Task.FromResult(new GenericResponse { Success = true }));

        _userRepository.GetAllOfDIAManagerUsers()
            .Returns(Task.FromResult(new List<User>
            {
                new User { FullName = "Manager One", Email = "m1@gov.uk" }
            }));

        _userRepository.GetUser(email)
            .Returns(Task.FromResult(new CabUser
            {
                CabEmail = email,
                Cab = new Cab { RegisteredName = "Test CAB" }
            }));

        _emailSender.SendEmailCabAccountCreated(email, email)
            .Returns(Task.FromResult(true));

        _emailSender.SendEmailCabAccountCreatedToDSIT(
                "Manager One", "m1@gov.uk", email, "Test CAB")
            .Returns(Task.FromResult(true));

        var result = await _service.MFAConfirmation(email, password, mfaCode);

        Assert.Equal("OK", result);

        await _userRepository.Received(1)
            .UpdateAccountStatus(email, AccountStatusEnum.Active, email);

        await _emailSender.Received(1)
            .SendEmailCabAccountCreated(email, email);

        await _emailSender.Received(1)
            .SendEmailCabAccountCreatedToDSIT("Manager One", "m1@gov.uk", email, "Test CAB");
    }

    [Fact]
    public async Task MFAConfirmation_OK_MultipleManagers_SendsEmailToEach()
    {
        var email = "cab@provider.com";
        var password = "Password123!";
        var mfaCode = "123456";

        _cognitoClient.MFARegistrationConfirmation(email, password, mfaCode)
            .Returns(Task.FromResult("OK"));

        _userRepository.UpdateAccountStatus(email, AccountStatusEnum.Active, email)
            .Returns(Task.FromResult(new GenericResponse { Success = true }));

        _userRepository.GetAllOfDIAManagerUsers()
            .Returns(Task.FromResult(new List<User>
            {
                new User { FullName = "Manager A", Email = "a@gov.uk" },
                new User { FullName = "Manager B", Email = "b@gov.uk" }
            }));

        _userRepository.GetUser(email)
            .Returns(Task.FromResult(new CabUser
            {
                CabEmail = email,
                Cab = new Cab { RegisteredName = "Test CAB" }
            }));

        _emailSender.SendEmailCabAccountCreated(email, email)
            .Returns(Task.FromResult(true));

        _emailSender.SendEmailCabAccountCreatedToDSIT(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<string>())
            .Returns(Task.FromResult(true));

        var result = await _service.MFAConfirmation(email, password, mfaCode);

        Assert.Equal("OK", result);

        await _emailSender.Received(2)
            .SendEmailCabAccountCreatedToDSIT(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<string>());
    }

    [Fact]
    public async Task MFAConfirmation_OK_UpdateStatusFails_NoEmailSent()
    {
        var email = "cab@provider.com";
        var password = "Password123!";
        var mfaCode = "123456";

        _cognitoClient.MFARegistrationConfirmation(email, password, mfaCode)
            .Returns(Task.FromResult("OK"));

        _userRepository.UpdateAccountStatus(email, AccountStatusEnum.Active, email)
            .Returns(Task.FromResult(new GenericResponse { Success = false }));

        var result = await _service.MFAConfirmation(email, password, mfaCode);

        Assert.Equal("OK", result);

        await _userRepository.Received(1)
            .UpdateAccountStatus(email, AccountStatusEnum.Active, email);

        await _emailSender.DidNotReceive()
            .SendEmailCabAccountCreated(Arg.Any<string>(), Arg.Any<string>());

        await _emailSender.DidNotReceive()
            .SendEmailCabAccountCreatedToDSIT(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<string>());
    }

    [Fact]
    public async Task MFAConfirmation_NotOK_ReturnsKOWithoutSideEffects()
    {
        var email = "cab@provider.com";
        var password = "Password123!";
        var mfaCode = "000000";

        _cognitoClient.MFARegistrationConfirmation(email, password, mfaCode)
            .Returns(Task.FromResult("KO"));

        var result = await _service.MFAConfirmation(email, password, mfaCode);

        Assert.Equal("KO", result);

        await _userRepository.DidNotReceive()
            .UpdateAccountStatus(Arg.Any<string>(), Arg.Any<AccountStatusEnum>(), Arg.Any<string>());

        await _emailSender.DidNotReceive()
            .SendEmailCabAccountCreated(Arg.Any<string>(), Arg.Any<string>());

        await _emailSender.DidNotReceive()
            .SendEmailCabAccountCreatedToDSIT(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<string>());
    }

    #endregion

    #region SignInAndWaitForMfa

    [Fact]
    public async Task SignInAndWaitForMfa_ValidCredentials_ReturnsSession()
    {
        var email = "Test@Example.com";
        var password = "Password123!";
        var expectedSession = "valid-session-id";

        _cognitoClient.SignInAndWaitForMfa(email.ToLower(), password)
            .Returns(Task.FromResult(expectedSession));

        var result = await _service.SignInAndWaitForMfa(email, password);

        Assert.Equal(expectedSession, result);

        await _emailSender.DidNotReceive()
            .SendEmailCabFailedLoginAttempt(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task SignInAndWaitForMfa_IncorrectDetails_ReturnsMessageAndSendsEmail()
    {
        var email = "wrong@Example.com";
        var password = "WrongPassword";

        _cognitoClient.SignInAndWaitForMfa(email.ToLower(), password)
            .Returns(Task.FromResult(Constants.IncorrectLoginDetails));

        _emailSender.SendEmailCabFailedLoginAttempt(
                email,
                Arg.Any<string>())
            .Returns(Task.FromResult(true));

        var result = await _service.SignInAndWaitForMfa(email, password);

        Assert.Equal(Constants.IncorrectLoginDetails, result);

        await _emailSender.Received(1)
            .SendEmailCabFailedLoginAttempt(email, Arg.Any<string>());
    }

    [Fact]
    public async Task SignInAndWaitForMfa_UserDisabled_ReturnsMessageWithoutEmail()
    {
        var email = "locked@Example.com";
        var password = "Password123!";

        _cognitoClient.SignInAndWaitForMfa(email.ToLower(), password)
            .Returns(Task.FromResult(Constants.UserDisabled));

        var result = await _service.SignInAndWaitForMfa(email, password);

        Assert.Equal(Constants.UserDisabled, result);

        await _emailSender.DidNotReceive()
            .SendEmailCabFailedLoginAttempt(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task SignInAndWaitForMfa_EmptyResponse_ReturnsEmptyWithoutEmail()
    {
        var email = "error@Example.com";
        var password = "Password123!";

        _cognitoClient.SignInAndWaitForMfa(email.ToLower(), password)
            .Returns(Task.FromResult(""));

        var result = await _service.SignInAndWaitForMfa(email, password);

        Assert.Empty(result);

        await _emailSender.DidNotReceive()
            .SendEmailCabFailedLoginAttempt(Arg.Any<string>(), Arg.Any<string>());
    }

    #endregion

    #region SignOut

    [Fact]
    public async Task SignOut_WithAccessToken_CallsSignOutOnCognito()
    {
        var accessToken = "test-access-token";

        _cognitoClient.SignOutUserAsync(accessToken)
            .Returns(Task.CompletedTask);

        _service.SignOut(accessToken);

        await _cognitoClient.Received(1)
            .SignOutUserAsync(accessToken);
    }

    #endregion
}