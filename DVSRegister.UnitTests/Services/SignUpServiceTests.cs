using System.Globalization;
using Amazon.CognitoIdentityProvider.Model;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Repositories;
using NSubstitute;

namespace DVSRegister.UnitTests.Services
{
    public class SignUpServiceTests
    {
        private const string Email = "CAB.User@Example.COM";
        private const string LowercaseEmail = "cab.user@example.com";
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

        [Fact]
        public async Task ConfirmMFAToken_ValidRequest_LowercasesEmailAndReturnsAuthenticationResult()
        {
            var expected = new AuthenticationResultType { AccessToken = "access-token" };
            _cognitoClient.ConfirmMFAToken("session", LowercaseEmail, "123456").Returns(expected);

            var result = await _service.ConfirmMFAToken("session", Email, "123456");

            Assert.Same(expected, result);
            await _cognitoClient.Received(1).ConfirmMFAToken("session", LowercaseEmail, "123456");
        }

        [Fact]
        public async Task ConfirmMFAToken_CognitoFailure_PropagatesException()
        {
            var expected = new InvalidOperationException("Cognito unavailable");
            _cognitoClient.ConfirmMFAToken("session", LowercaseEmail, "123456")
                .Returns(Task.FromException<AuthenticationResultType>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ConfirmMFAToken("session", Email, "123456"));

            Assert.Same(expected, exception);
        }

        [Fact]
        public async Task ResetPassword_ValidRequest_LowercasesEmailAndReturnsResponseUnchanged()
        {
            var expected = new GenericResponse { Success = true, Data = "reset" };
            _cognitoClient.ConfirmPasswordReset(LowercaseEmail, "password", "123456").Returns(expected);

            var result = await _service.ResetPassword(Email, "password", "123456");

            Assert.Same(expected, result);
            await _cognitoClient.Received(1)
                .ConfirmPasswordReset(LowercaseEmail, "password", "123456");
        }

        [Fact]
        public async Task ResetPassword_DuplicateEmailResponse_ReturnsResponseUnchanged()
        {
            var expected = new GenericResponse
            {
                Success = false,
                ErrorMessage = "Email already exists"
            };
            _cognitoClient.ConfirmPasswordReset(LowercaseEmail, "password", "123456").Returns(expected);

            var result = await _service.ResetPassword(Email, "password", "123456");

            Assert.Same(expected, result);
        }

        [Fact]
        public async Task ResetPassword_CognitoFailure_PropagatesException()
        {
            var expected = new InvalidOperationException("Reset failed");
            _cognitoClient.ConfirmPasswordReset(LowercaseEmail, "password", "123456")
                .Returns(Task.FromException<GenericResponse>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ResetPassword(Email, "password", "123456"));

            Assert.Same(expected, exception);
        }

        [Fact]
        public async Task ConfirmPassword_ValidRequest_LowercasesEmailAndReturnsResponseUnchanged()
        {
            var expected = new GenericResponse { Success = true, Data = "mfa-secret" };
            _cognitoClient.ConfirmPasswordAndGenerateMFAToken(LowercaseEmail, "password", "123456")
                .Returns(expected);

            var result = await _service.ConfirmPassword(Email, "password", "123456");

            Assert.Same(expected, result);
            await _cognitoClient.Received(1)
                .ConfirmPasswordAndGenerateMFAToken(LowercaseEmail, "password", "123456");
        }

        [Fact]
        public async Task ConfirmPassword_DuplicateUserResponse_ReturnsResponseUnchanged()
        {
            var expected = new GenericResponse
            {
                Success = false,
                ErrorMessage = "User account already exists"
            };
            _cognitoClient.ConfirmPasswordAndGenerateMFAToken(LowercaseEmail, "password", "123456")
                .Returns(expected);

            var result = await _service.ConfirmPassword(Email, "password", "123456");

            Assert.Same(expected, result);
            Assert.Empty(_userRepository.ReceivedCalls());
        }

        [Fact]
        public async Task ConfirmPassword_CognitoFailure_PropagatesException()
        {
            var expected = new InvalidOperationException("Confirmation failed");
            _cognitoClient.ConfirmPasswordAndGenerateMFAToken(LowercaseEmail, "password", "123456")
                .Returns(Task.FromException<GenericResponse>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ConfirmPassword(Email, "password", "123456"));

            Assert.Same(expected, exception);
        }

        [Fact]
        public async Task ForgotPassword_ValidEmail_LowercasesEmailAndReturnsResponseUnchanged()
        {
            _cognitoClient.ForgotPassword(LowercaseEmail).Returns("OK");

            var result = await _service.ForgotPassword(Email);

            Assert.Equal("OK", result);
            await _cognitoClient.Received(1).ForgotPassword(LowercaseEmail);
        }

        [Fact]
        public async Task ForgotPassword_InvalidEmailResponse_ReturnsValidationMessageUnchanged()
        {
            _cognitoClient.ForgotPassword(LowercaseEmail).Returns(Constants.EmailErrorMessage);

            var result = await _service.ForgotPassword(Email);

            Assert.Equal(Constants.EmailErrorMessage, result);
        }

        [Fact]
        public async Task ForgotPassword_WhitespaceEmail_ForwardsWhitespaceUnchanged()
        {
            _cognitoClient.ForgotPassword("   ").Returns("response");

            var result = await _service.ForgotPassword("   ");

            Assert.Equal("response", result);
            await _cognitoClient.Received(1).ForgotPassword("   ");
        }

        [Fact]
        public async Task ForgotPassword_NullEmail_ThrowsNullReferenceExceptionBeforeCallingCognito()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => _service.ForgotPassword(null!));

            Assert.Empty(_cognitoClient.ReceivedCalls());
        }

        [Fact]
        public async Task ForgotPassword_CognitoFailure_PropagatesException()
        {
            var expected = new InvalidOperationException("Forgot password failed");
            _cognitoClient.ForgotPassword(LowercaseEmail).Returns(Task.FromException<string>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ForgotPassword(Email));

            Assert.Same(expected, exception);
        }

        [Theory]
        [InlineData("KO")]
        [InlineData("ok")]
        [InlineData("")]
        public async Task MFAConfirmation_ResponseIsNotExactOk_ReturnsResponseWithoutActivatingUser(string response)
        {
            _cognitoClient.MFARegistrationConfirmation(LowercaseEmail, "password", "123456")
                .Returns(response);

            var result = await _service.MFAConfirmation(Email, "password", "123456");

            Assert.Equal(response, result);
            Assert.Empty(_userRepository.ReceivedCalls());
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task MFAConfirmation_CognitoFailure_PropagatesExceptionWithoutActivatingUser()
        {
            var expected = new InvalidOperationException("MFA failed");
            _cognitoClient.MFARegistrationConfirmation(LowercaseEmail, "password", "123456")
                .Returns(Task.FromException<string>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.MFAConfirmation(Email, "password", "123456"));

            Assert.Same(expected, exception);
            Assert.Empty(_userRepository.ReceivedCalls());
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task MFAConfirmation_AccountUpdateFails_ReturnsOkWithoutSendingNotifications()
        {
            ConfigureSuccessfulMfa();
            _userRepository.UpdateAccountStatus(Email, AccountStatusEnum.Active, Email)
                .Returns(new GenericResponse { Success = false });

            var result = await _service.MFAConfirmation(Email, "password", "123456");

            Assert.Equal("OK", result);
            await _userRepository.Received(1)
                .UpdateAccountStatus(Email, AccountStatusEnum.Active, Email);
            await _userRepository.DidNotReceiveWithAnyArgs().GetAllOfDIAManagerUsers();
            await _userRepository.DidNotReceiveWithAnyArgs().GetUser(default!);
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task MFAConfirmation_AccountUpdateThrows_PropagatesExceptionWithoutSendingNotifications()
        {
            ConfigureSuccessfulMfa();
            var expected = new InvalidOperationException("Repository unavailable");
            _userRepository.UpdateAccountStatus(Email, AccountStatusEnum.Active, Email)
                .Returns(Task.FromException<GenericResponse>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.MFAConfirmation(Email, "password", "123456"));

            Assert.Same(expected, exception);
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task MFAConfirmation_AccountCreatedEmailThrows_PropagatesExceptionBeforeUserLookups()
        {
            ConfigureSuccessfulAccountUpdate();
            var expected = new InvalidOperationException("Notify unavailable");
            _emailSender.SendEmailCabAccountCreated(Email, Email)
                .Returns(Task.FromException<bool>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.MFAConfirmation(Email, "password", "123456"));

            Assert.Same(expected, exception);
            await _userRepository.DidNotReceiveWithAnyArgs().GetAllOfDIAManagerUsers();
            await _userRepository.DidNotReceiveWithAnyArgs().GetUser(default!);
        }

        [Fact]
        public async Task MFAConfirmation_NoManagers_SendsCabEmailAndReadsExistingCabUserWithoutCreatingOne()
        {
            ConfigureSuccessfulAccountUpdate();
            var cabUser = CreateCabUser("Registered CAB");
            _userRepository.GetAllOfDIAManagerUsers().Returns(new List<User>());
            _userRepository.GetUser(Email).Returns(cabUser);

            var result = await _service.MFAConfirmation(Email, "password", "123456");

            Assert.Equal("OK", result);
            await _emailSender.Received(1).SendEmailCabAccountCreated(Email, Email);
            await _userRepository.Received(1).GetAllOfDIAManagerUsers();
            await _userRepository.Received(1).GetUser(Email);
            await _userRepository.DidNotReceiveWithAnyArgs().UpdateCabUser(default!);
            await _emailSender.DidNotReceiveWithAnyArgs()
                .SendEmailCabAccountCreatedToDSIT(default!, default!, default!, default!);
        }

        [Fact]
        public async Task MFAConfirmation_ManagersExist_SendsOneNotificationPerManagerUsingCabLink()
        {
            ConfigureSuccessfulAccountUpdate();
            var managers = new List<User>
            {
                new() { FullName = "First Manager", Email = "first@example.com" },
                new() { FullName = "Second Manager", Email = "second@example.com" }
            };
            _userRepository.GetAllOfDIAManagerUsers().Returns(managers);
            _userRepository.GetUser(Email).Returns(CreateCabUser("Registered CAB"));

            var result = await _service.MFAConfirmation(Email, "password", "123456");

            Assert.Equal("OK", result);
            await _emailSender.Received(1)
                .SendEmailCabAccountCreatedToDSIT("First Manager", "first@example.com", Email, "Registered CAB");
            await _emailSender.Received(1)
                .SendEmailCabAccountCreatedToDSIT("Second Manager", "second@example.com", Email, "Registered CAB");
            await _emailSender.Received(2)
                .SendEmailCabAccountCreatedToDSIT(Arg.Any<string>(), Arg.Any<string>(), Email, "Registered CAB");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(999)]
        public async Task MFAConfirmation_ManagerHasMissingOrInvalidRole_DoesNotValidateRoleBeforeNotification(int role)
        {
            ConfigureSuccessfulAccountUpdate();
            var manager = new User
            {
                FullName = "Manager",
                Email = "manager@example.com",
                UserRole = (UserRoleEnum)role
            };
            _userRepository.GetAllOfDIAManagerUsers().Returns(new List<User> { manager });
            _userRepository.GetUser(Email).Returns(CreateCabUser("Registered CAB"));

            var result = await _service.MFAConfirmation(Email, "password", "123456");

            Assert.Equal("OK", result);
            await _emailSender.Received(1)
                .SendEmailCabAccountCreatedToDSIT("Manager", "manager@example.com", Email, "Registered CAB");
        }

        [Fact]
        public async Task MFAConfirmation_CabUserHasNoCabLink_SendsManagerNotificationWithNullCabName()
        {
            ConfigureSuccessfulAccountUpdate();
            _userRepository.GetAllOfDIAManagerUsers().Returns(new List<User>
            {
                new() { FullName = "Manager", Email = "manager@example.com" }
            });
            _userRepository.GetUser(Email).Returns(new CabUser { CabEmail = Email, Cab = null });
            string? receivedCabName = "not-null";
            _emailSender.SendEmailCabAccountCreatedToDSIT(
                    "Manager",
                    "manager@example.com",
                    Email,
                    Arg.Do<string>(value => receivedCabName = value))
                .Returns(true);

            var result = await _service.MFAConfirmation(Email, "password", "123456");

            Assert.Equal("OK", result);
            Assert.Null(receivedCabName);
        }

        [Fact]
        public async Task MFAConfirmation_ManagerLookupFails_PropagatesExceptionAfterCabNotification()
        {
            ConfigureSuccessfulAccountUpdate();
            var expected = new InvalidOperationException("Manager lookup failed");
            _userRepository.GetAllOfDIAManagerUsers()
                .Returns(Task.FromException<List<User>>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.MFAConfirmation(Email, "password", "123456"));

            Assert.Same(expected, exception);
            await _emailSender.Received(1).SendEmailCabAccountCreated(Email, Email);
            await _userRepository.DidNotReceiveWithAnyArgs().GetUser(default!);
        }

        [Fact]
        public async Task MFAConfirmation_CabUserLookupFails_PropagatesExceptionBeforeManagerNotifications()
        {
            ConfigureSuccessfulAccountUpdate();
            var expected = new InvalidOperationException("CAB user lookup failed");
            _userRepository.GetAllOfDIAManagerUsers().Returns(new List<User>
            {
                new() { FullName = "Manager", Email = "manager@example.com" }
            });
            _userRepository.GetUser(Email).Returns(Task.FromException<CabUser>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.MFAConfirmation(Email, "password", "123456"));

            Assert.Same(expected, exception);
            await _emailSender.DidNotReceiveWithAnyArgs()
                .SendEmailCabAccountCreatedToDSIT(default!, default!, default!, default!);
        }

        [Fact]
        public async Task MFAConfirmation_ManagerNotificationFails_PropagatesExceptionAfterSuccessfulActivation()
        {
            ConfigureSuccessfulAccountUpdate();
            var expected = new InvalidOperationException("Manager notification failed");
            _userRepository.GetAllOfDIAManagerUsers().Returns(new List<User>
            {
                new() { FullName = "Manager", Email = "manager@example.com" }
            });
            _userRepository.GetUser(Email).Returns(CreateCabUser("Registered CAB"));
            _emailSender.SendEmailCabAccountCreatedToDSIT(
                    "Manager", "manager@example.com", Email, "Registered CAB")
                .Returns(Task.FromException<bool>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.MFAConfirmation(Email, "password", "123456"));

            Assert.Same(expected, exception);
            await _userRepository.Received(1)
                .UpdateAccountStatus(Email, AccountStatusEnum.Active, Email);
        }

        [Fact]
        public async Task SignInAndWaitForMfa_ValidCredentials_LowercasesEmailAndReturnsSessionWithoutAlert()
        {
            _cognitoClient.SignInAndWaitForMfa(LowercaseEmail, "password").Returns("session");

            var result = await _service.SignInAndWaitForMfa(Email, "password");

            Assert.Equal("session", result);
            await _cognitoClient.Received(1).SignInAndWaitForMfa(LowercaseEmail, "password");
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task SignInAndWaitForMfa_IncorrectCredentials_SendsFailedLoginAlertUsingOriginalEmail()
        {
            string? receivedTimestamp = null;
            _cognitoClient.SignInAndWaitForMfa(LowercaseEmail, "password")
                .Returns(Constants.IncorrectLoginDetails);
            _emailSender.SendEmailCabFailedLoginAttempt(
                    Email,
                    Arg.Do<string>(value => receivedTimestamp = value))
                .Returns(true);

            var result = await _service.SignInAndWaitForMfa(Email, "password");

            Assert.Equal(Constants.IncorrectLoginDetails, result);
            Assert.True(DateTime.TryParseExact(
                receivedTimestamp,
                "dd MMM yyyy h:mm tt",
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out _));
            await _emailSender.Received(1)
                .SendEmailCabFailedLoginAttempt(Email, Arg.Any<string>());
        }

        [Theory]
        [InlineData("Enter a valid email address")]
        [InlineData("")]
        [InlineData("Different failure")]
        public async Task SignInAndWaitForMfa_ResponseIsNotIncorrectLoginConstant_DoesNotSendAlert(string response)
        {
            _cognitoClient.SignInAndWaitForMfa(LowercaseEmail, "password").Returns(response);

            var result = await _service.SignInAndWaitForMfa(Email, "password");

            Assert.Equal(response, result);
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task SignInAndWaitForMfa_CognitoFailure_PropagatesExceptionWithoutAlert()
        {
            var expected = new InvalidOperationException("Sign in failed");
            _cognitoClient.SignInAndWaitForMfa(LowercaseEmail, "password")
                .Returns(Task.FromException<string>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.SignInAndWaitForMfa(Email, "password"));

            Assert.Same(expected, exception);
            Assert.Empty(_emailSender.ReceivedCalls());
        }

        [Fact]
        public async Task SignInAndWaitForMfa_FailedLoginAlertThrows_PropagatesException()
        {
            var expected = new InvalidOperationException("Notify unavailable");
            _cognitoClient.SignInAndWaitForMfa(LowercaseEmail, "password")
                .Returns(Constants.IncorrectLoginDetails);
            _emailSender.SendEmailCabFailedLoginAttempt(Email, Arg.Any<string>())
                .Returns(Task.FromException<bool>(expected));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.SignInAndWaitForMfa(Email, "password"));

            Assert.Same(expected, exception);
        }

        [Fact]
        public async Task SignOut_AccessTokenProvided_ForwardsTokenToCognito()
        {
            var signOutCalled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _cognitoClient.SignOutUserAsync("access-token").Returns(_ =>
            {
                signOutCalled.SetResult();
                return Task.CompletedTask;
            });

            _service.SignOut("access-token");
            await signOutCalled.Task.WaitAsync(TimeSpan.FromSeconds(2));

            await _cognitoClient.Received(1).SignOutUserAsync("access-token");
        }

        private void ConfigureSuccessfulMfa()
        {
            _cognitoClient.MFARegistrationConfirmation(LowercaseEmail, "password", "123456")
                .Returns("OK");
        }

        private void ConfigureSuccessfulAccountUpdate()
        {
            ConfigureSuccessfulMfa();
            _userRepository.UpdateAccountStatus(Email, AccountStatusEnum.Active, Email)
                .Returns(new GenericResponse { Success = true });
        }

        private static CabUser CreateCabUser(string registeredName)
        {
            return new CabUser
            {
                CabEmail = Email,
                Cab = new Cab
                {
                    CabName = "CAB",
                    RegisteredName = registeredName
                }
            };
        }
    }
}
