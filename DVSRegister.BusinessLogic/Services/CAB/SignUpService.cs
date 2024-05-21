﻿using DVSAdmin.BusinessLogic.Services;
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
            return response;  
        }

        public async Task<string> SignInAndWaitForMfa(string email, string password)
        {
            string response = await _cognitoClient.SignInAndWaitForMfa(email, password);
            return response;
        }
    }
}

