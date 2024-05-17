using System;
using DVSAdmin.BusinessLogic.Services;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services
{
	public class SignUpService : ISignUpService
	{
        private CognitoClient _cognitoClient;

        public SignUpService(CognitoClient cognitoClient)
		{
            _cognitoClient = cognitoClient;
		}

        public async Task<string> ConfirmMFAToken(string session, string email, string token)
        {
            return await _cognitoClient.ConfirmMFAToken(session, email, token);
        }

        public Task<GenericResponse> ConfirmPassword(string email, string password, string oneTimePassword)
        {
            throw new NotImplementedException();
        }

        public Task<string> ForgotPassword(string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> MFAConfirmation(string email, string password, string mfaCode)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SignInAndWaitForMfa(string email, string password)
        {
            string response = await _cognitoClient.SignInAndWaitForMfa(email, password);
            return response;
        }
    }
}

