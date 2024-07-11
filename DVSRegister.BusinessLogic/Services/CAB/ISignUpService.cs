using System;
using Amazon.CognitoIdentityProvider.Model;
using DVSRegister.CommonUtility.Models;


namespace DVSAdmin.BusinessLogic.Services
{
    public interface ISignUpService
    {
        public Task<string> ForgotPassword(string email);

        public Task<GenericResponse> ConfirmPassword(string email, string password, string oneTimePassword);

        public Task<string> MFAConfirmation(string email, string password, string mfaCode);

        public Task<string> SignInAndWaitForMfa(string email, string password);

        public Task<AuthenticationResultType> ConfirmMFAToken(string session, string email, string token);
        public void SignOut(string accesssToken);
        public Task<GenericResponse> ResetPassword(string email, string password, string oneTimePassword);
    }
}
