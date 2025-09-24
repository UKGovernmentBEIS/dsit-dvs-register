using DVSRegister.CommonUtility.Models.Email;

namespace DVSRegister.CommonUtility.Models
{
    public class GovUkNotifyConfiguration
    {
        public const string ConfigSection = "GovUkNotify";
        public string ApiKey { get; set; }
        public string OfDiaEmailId { get; set; }
        public string LoginLink { get; set; }
        public string CabSignUpLink { get; set; }
        public string CabLoginLink { get; set; }
       
        public CabAccountCreatedTemplate CabAccountCreatedTemplate { get; set; }
        public CabFailedLoginAttemptTemplate CabFailedLoginAttemptTemplate { get; set; }

        public CabInformationSubmittedTemplate CabInformationSubmittedTemplate { get; set; }
        public CabSignUpActivationTemplate CabSignUpActivationTemplate { get; set; }  
        public CabSubmittedDSITEmailTemplate CabSubmittedDSITEmailTemplate { get; set; }

        public AgreementToPublishTemplate AgreementToPublishTemplate { get; set; }
        public AgreementToPublishToDSITTemplate AgreementToPublishToDSITTemplate { get; set; }
        public AgreementToProceedApplicationToDSIT AgreementToProceedApplicationToDSIT { get; set; }
        public ConfirmationToProceedApplicationToDIP ConfirmationToProceedApplicationToDIP { get; set; }
        public DeclinedToProceedApplicationToDSIT DeclinedToProceedApplicationToDSIT { get; set; }
        public DeclinedToProceedConfirmationToDIP DeclinedToProceedConfirmationToDIP { get; set; }
        public ProviderRemovalRequestConfirmed ProviderRemovalRequestConfirmed { get; set; }

        public ProviderRemovalConfirmationToDSIT ProviderRemovalConfirmationToDSIT { get; set; }

        public RecordRemovedToDSIT RecordRemovedToDSIT { get; set; }

        public RemovalRequestDeclinedToProvider RemovalRequestDeclinedToProvider { get; set; }
        public RemovalRequestDeclinedToDSIT RemovalRequestDeclinedToDSIT { get; set; }

        public _2iCheckDeclinedNotificationToDSIT _2iCheckDeclinedNotificationToDSIT { get; set; }

        public _2iCheckApprovedNotificationToDSIT _2iCheckApprovedNotificationToDSIT { get;set; }

        public RecordRemovedConfirmedToCabOrProvider RecordRemovedConfirmedToCabOrProvider { get; set; }

        public RemoveServiceConfirmationToProvider RemoveServiceConfirmationToProvider { get; set; }
        public ServiceRemovalConfirmationToDSIT ServiceRemovalConfirmationToDSIT { get; set; }

        public ServiceRemovedConfirmedToCabOrProvider ServiceRemovedConfirmedToCabOrProvider { get;set;}
        public ServiceRemovedToDSIT ServiceRemovedToDSIT { get; set; }  
        public CabServiceRemovalRequested CabServiceRemovalRequested { get; set; }
        public CabServiceRemovalRequestedToDSIT CabServiceRemovalRequestedToDSIT { get; set; }

        public RecordRemovalRequestByCabToDSIT RecordRemovalRequestByCabToDSIT { get; set; }

        public RecordRemovalRequestConfirmationToCab RecordRemovalRequestConfirmationToCab { get;set; }

        public Service2iCheckDeclinedToDSIT Service2iCheckDeclinedToDSIT { get; set; }
        public Service2iCheckApprovedToDSIT Service2iCheckApprovedToDSIT { get; set; }

        public EditProviderAccepted EditProviderAccepted { get; set; }

        public EditProviderDeclined EditProviderDeclined { get; set; }  

        public EditServiceAccepted EditServiceAccepted { get; set; }
        public EditServiceDeclined EditServiceDeclined { get; set; }
        
        public CabTransferConfirmationToCabA CabTransferConfirmationToCabA { get; set; }
        public CabTransferConfirmationToCabB CabTransferConfirmationToCabB { get; set; }
        public CabTransferCancellationToCabB CabTransferCancellationToCabB { get; set; }
        public CabTransferCancellationToDSIT CabTransferCancellationToDSIT { get; set; }
        public CabTransferConfirmationToDSIT CabTransferConfirmationToDSIT { get; set; }
    }
}
