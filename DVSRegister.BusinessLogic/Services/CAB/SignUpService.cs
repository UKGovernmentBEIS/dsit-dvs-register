using Amazon.CognitoIdentityProvider.Model;
using DVSAdmin.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.AWS;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Repositories;

namespace DVSRegister.BusinessLogic.Services.CAB;

public class SignUpService(ICognitoClient cognitoClient, ILoginEmailSender emailSender, IUserRepository userRepository)
    : ISignUpService
{
    public async Task<AuthenticationResultType> ConfirmMFAToken(string session, string email, string token)
    {
        return await cognitoClient.ConfirmMFAToken(session, email.ToLower(), token);
    }

    public async Task<GenericResponse> ResetPassword(string email, string password, string oneTimePassword)
    {
        return await cognitoClient.ConfirmPasswordReset(email.ToLower(), password, oneTimePassword);
    }

    public async Task<GenericResponse> ConfirmPassword(string email, string password, string oneTimePassword)
    {
        return await cognitoClient.ConfirmPasswordAndGenerateMFAToken(email.ToLower(), password, oneTimePassword);
    }

    public async Task<string> ForgotPassword(string email)
    {
        return await cognitoClient.ForgotPassword(email.ToLower());
    }

    public async Task<string> MFAConfirmation(string email, string password, string mfaCode)
    {
        string response = await cognitoClient.MFARegistrationConfirmation(email.ToLower(), password, mfaCode);
        if (response == "OK")
        {
            GenericResponse genericResponse =
                await userRepository.UpdateAccountStatus(email, AccountStatusEnum.Active, email);
            if (genericResponse.Success)
            {
                await emailSender.SendEmailCabAccountCreated(email, email);
                var ofDiaManagers = await userRepository.GetAllOfDIAManagerUsers();
                var cabUser = await userRepository.GetUser(email);
                foreach (var manager in ofDiaManagers)
                {
                    await emailSender.SendEmailCabAccountCreatedToDSIT(manager.FullName!, manager.Email, email,
                        cabUser.Cab?.RegisteredName);
                }
            }
        }

        return response;
    }

    public async Task<string> SignInAndWaitForMfa(string email, string password)
    {
        string response = await cognitoClient.SignInAndWaitForMfa(email.ToLower(), password);
        if (response == Constants.IncorrectLoginDetails)
        {
            await emailSender.SendEmailCabFailedLoginAttempt(email,
                Helper.GetLocalDateTime(DateTime.UtcNow, "dd MMM yyyy h:mm tt"));
        }

        return response;
    }

    public async void SignOut(string accesssToken)
    {
        await cognitoClient.SignOutUserAsync(accesssToken);
    }
}