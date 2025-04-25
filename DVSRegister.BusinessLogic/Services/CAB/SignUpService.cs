using Amazon.CognitoIdentityProvider.Model;
using DVSAdmin.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services
{
    public class SignUpService : ISignUpService
	{
        private CognitoClient _cognitoClient;
        private readonly LoginEmailSender _emailSender;

        public SignUpService(CognitoClient cognitoClient, LoginEmailSender emailSender)
		{
            _cognitoClient = cognitoClient;
            _emailSender = emailSender;
		}

        public async Task<AuthenticationResultType> ConfirmMFAToken(string session, string email, string token)
        {
            return await _cognitoClient.ConfirmMFAToken(session, email, token);
        }

        public async Task<GenericResponse> ResetPassword(string email, string password, string oneTimePassword)
        {
            return await _cognitoClient.ConfirmPasswordReset(email, password, oneTimePassword);
        }

        public async Task<GenericResponse> ConfirmPassword(string email, string password, string oneTimePassword)
        {
            return await _cognitoClient.ConfirmPasswordAndGenerateMFAToken(email, password, oneTimePassword);
        }

        public async Task<string> ForgotPassword(string email)
        {
            return await _cognitoClient.ForgotPassword(email);
        }

        public async Task<string> MFAConfirmation(string email, string password, string mfaCode)
        {
            string response = await _cognitoClient.MFARegistrationConfirmation(email, password, mfaCode);
            if(response == "OK")
            {
                await _emailSender.SendEmailCabAccountCreated(email, email);
            }
            return response;  
        }

        public async Task<string> SignInAndWaitForMfa(string email, string password)
        {
            string response = await _cognitoClient.SignInAndWaitForMfa(email, password);
            if(response == Constants.IncorrectPassword) 
            {
                await _emailSender.SendEmailCabFailedLoginAttempt(email, Helper.GetLocalDateTime(DateTime.UtcNow, "dd MMM yyyy h:mm tt"));
            }
            return response;
        }

        public async void SignOut(string accesssToken)
        {
            await _cognitoClient.SignOutUserAsync(accesssToken);
        }
    }
}

