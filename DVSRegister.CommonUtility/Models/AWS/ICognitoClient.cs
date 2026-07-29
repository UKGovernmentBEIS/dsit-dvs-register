using Amazon.CognitoIdentityProvider.Model;

namespace DVSRegister.CommonUtility.Models
{
    public interface ICognitoClient
    {
        Task<string> ForgotPassword(string email);
        Task<GenericResponse> ConfirmPasswordReset(string email, string password, string oneTimePassCode);
        Task<GenericResponse> ConfirmPasswordAndGenerateMFAToken(string email, string password, string oneTimePassCode);
        Task<string> MFARegistrationConfirmation(string email, string password, string mfaCode);
        Task<string> SignInAndWaitForMfa(string email, string password);
        Task<AuthenticationResultType> ConfirmMFAToken(string session, string email, string token);
        Task SignOutUserAsync(string accessToken);
    }
}
