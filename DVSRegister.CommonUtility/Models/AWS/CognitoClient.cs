﻿using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;

public class CognitoClient
{
    public readonly string _userPoolId;
    public readonly string _clientId;
    public readonly string _region;
    public readonly AmazonCognitoIdentityProviderClient _provider;

    public CognitoClient(string userPoolId, string clientId, string region)
    {
        _userPoolId = userPoolId;
        _clientId = clientId;
        _region = region;

        // Initialize the Amazon Cognito Identity Provider client
        _provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), RegionEndpoint.EUWest2);
    }

    private async Task<InitiateAuthResponse> GetAccessToken(string email, string password)
    {
        var authRequest = new InitiateAuthRequest
        {
            AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
            ClientId = _clientId,
            AuthParameters = new Dictionary<string, string>
                {
                    {"USERNAME", email},
                    {"PASSWORD", password}
                }
        };

        var authResponse = await _provider.InitiateAuthAsync(authRequest);
        return authResponse;
      
    }

    public async Task<string> ForgotPassword(string email)
    {
        try
        {
            var forgotPasswordRequest = new ForgotPasswordRequest
            {
                ClientId = _clientId,
                Username = email
            };
            var forgotPasswordResponse = await _provider.ForgotPasswordAsync(forgotPasswordRequest);
            return forgotPasswordResponse.HttpStatusCode.ToString();
        }
        catch (UserNotFoundException)
        {
            return ("Enter a valid email address");
        }
        catch (LimitExceededException)
        {
            // Handle limit exceeded error
            return ("Attempt limit exceeded, please try after some time.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return ("An error occurred");
        }
    }

    public async Task<GenericResponse> ConfirmPasswordReset(string email, string password, string oneTimePassCode)
    {
       GenericResponse genericResponse = new GenericResponse();
        var confirmForgotPasswordRequest = new ConfirmForgotPasswordRequest
        {
            ClientId = _clientId,
            Username = email,
            Password = password,
            ConfirmationCode = oneTimePassCode
        };
        try
        {
            ConfirmForgotPasswordResponse confirmForgotPasswordResponse=  await _provider.ConfirmForgotPasswordAsync(confirmForgotPasswordRequest);
            if(confirmForgotPasswordResponse != null && confirmForgotPasswordResponse.HttpStatusCode == System.Net.HttpStatusCode.OK) 
            { 
                genericResponse .Success = true;
            }
            else
            {
                genericResponse.Success = false;
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resetting password : {ex.Message}");
            genericResponse.Success = false;
            genericResponse.Data = "Error while resetting password, please try again later";          
        }
        return genericResponse;


    }
    public async Task<GenericResponse> ConfirmPasswordAndGenerateMFAToken(string email, string password, string oneTimePassCode)
    {
        GenericResponse genericResponse = new GenericResponse();
        var confirmForgotPasswordRequest = new ConfirmForgotPasswordRequest
        {
            ClientId = _clientId,
            Username = email,
            Password = password,
            ConfirmationCode = oneTimePassCode
        };


        // Confirm Password Request
        try
        {
            await _provider.ConfirmForgotPasswordAsync(confirmForgotPasswordRequest);
        }
        catch (CodeMismatchException ex)
        {
            Console.WriteLine($"Error confirming password : {ex.Message}");
            genericResponse.Success = false;
            genericResponse.ErrorMessage = "Invalid verification code provided";
            return genericResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error confirming password : {ex.Message}");
            genericResponse.Success = false;
            genericResponse.ErrorMessage = "Error while confirming password, please try again later";
            return genericResponse;
        }



        // Generate MFA Token Request
        try
        {
            InitiateAuthResponse initiateAuthResponse = await GetAccessToken(email, password);
            if(initiateAuthResponse.ChallengeParameters != null && initiateAuthResponse.ChallengeName!=null)
            {
                genericResponse.ErrorMessage = "User account already exists";
                return genericResponse;
            }
            var associateSoftwareTokenRequest = new AssociateSoftwareTokenRequest
            {
                AccessToken = initiateAuthResponse.AuthenticationResult.AccessToken
            };
            var associateSoftwareTokenResponse = await _provider.AssociateSoftwareTokenAsync(associateSoftwareTokenRequest);

            genericResponse.Success = true;
            genericResponse.Data = associateSoftwareTokenResponse.SecretCode;
            return genericResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding MFA: {ex.Message}");
            genericResponse.Success = false;
            genericResponse.ErrorMessage = "Error while generating MFA Token, please try again later";
            return genericResponse;
        }
    }

    public async Task<string> MFARegistrationConfirmation(string email, string password, string mfaCode)
    {
        try
        {
            InitiateAuthResponse initiateAuthResponse = await GetAccessToken(email, password);
            var verifySoftwareTokenRequest = new VerifySoftwareTokenRequest
            {
                AccessToken = initiateAuthResponse.AuthenticationResult.AccessToken,
                UserCode = mfaCode,
                FriendlyDeviceName = "MFAEnabled-" + email
            };

            var verifySoftwareTokenResponse = await _provider.VerifySoftwareTokenAsync(verifySoftwareTokenRequest);
            if (verifySoftwareTokenResponse.HttpStatusCode.ToString() == "OK")
            {               
                var mfaOptions = new SetUserMFAPreferenceRequest()
                {
                    AccessToken = initiateAuthResponse.AuthenticationResult.AccessToken,
                    SoftwareTokenMfaSettings = new SoftwareTokenMfaSettingsType
                    {
                        Enabled = true,
                        PreferredMfa = true
                    }
                };

                var enableSoftwareTokenResponse = await _provider.SetUserMFAPreferenceAsync(mfaOptions);
                return "OK";
            }
            else
            {
                return "KO";
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error registering MFA Token: {ex.Message}");
            return "KO";
        }
    }

    public async Task<string> SignInAndWaitForMfa(string email, string password)
    {
        var authRequest = new InitiateAuthRequest
        {
            AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
            ClientId = _clientId,
            AuthParameters = new Dictionary<string, string>
                {
                    {"USERNAME", email},
                    {"PASSWORD", password}
                }
        };

        try
        {
            var authResponse = await _provider.InitiateAuthAsync(authRequest);
            return authResponse.Session.ToString();
        }
        catch (Amazon.CognitoIdentityProvider.Model.NotAuthorizedException ex)
        {
            return Constants.IncorrectPassword;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in logging in {ex.Message} ");
            return "";
        }
    }

    public async Task<AuthenticationResultType> ConfirmMFAToken(string session, string email, string token)
    {
        var challengeResponse = new RespondToAuthChallengeRequest
        {
            ChallengeName = ChallengeNameType.SOFTWARE_TOKEN_MFA,
            ClientId = _clientId,
            Session = session,
            ChallengeResponses = new Dictionary<string, string>
                {
                    {"USERNAME", email},
                    {"SOFTWARE_TOKEN_MFA_CODE", token}
                }
        };

        try
        {
            var response = await _provider.RespondToAuthChallengeAsync(challengeResponse);

            return response.AuthenticationResult;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in logging in {ex.Message} ");
            return null;
        }
    }

    public async Task SignOutUserAsync(string accessToken)
    {
        try
        {
            var signOutRequest = new GlobalSignOutRequest
            {
                AccessToken = accessToken
            };
            var response = await _provider.GlobalSignOutAsync(signOutRequest);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while signing out: " + ex.Message);
        }
    }

    public void Dispose()
    {
        _provider.Dispose();
    }
}