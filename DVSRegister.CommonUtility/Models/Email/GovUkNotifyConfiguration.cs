﻿namespace DVSRegister.CommonUtility.Models
{
    public class GovUkNotifyConfiguration
    {
        public const string ConfigSection = "GovUkNotify";
        public string ApiKey { get; set; }
        public string OfDiaEmailId { get; set; }
        public string LoginLink { get; set; }
        public string CabSignUpLink { get; set; }
        public string CabLoginLink { get; set; }

        public EmailConfirmationTemplate EmailConfirmationTemplate { get; set; }
        public ApplicationReceivedTemplate ApplicationReceivedTemplate { get; set; }
        public CabAccountCreatedTemplate CabAccountCreatedTemplate { get; set; }
        public CabFailedLoginAttemptTemplate CabFailedLoginAttemptTemplate { get; set; }

        public CabInformationSubmittedTemplate CabInformationSubmittedTemplate { get; set; }
        public CabSignUpActivationTemplate CabSignUpActivationTemplate { get; set; }  
        public CabSubmittedDSITEmailTemplate CabSubmittedDSITEmailTemplate { get; set; }

        public AgreementToPublishTemplate AgreementToPublishTemplate { get; set; }
        public AgreementToPublishToDSITTemplate AgreementToPublishToDSITTemplate { get; set; }
        public AgreementToProceedApplicationToDSIT AgreementToProceedApplicationToDSIT { get; set; }
    }
}
