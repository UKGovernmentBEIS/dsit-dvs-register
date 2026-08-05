namespace DVSRegister.CommonUtility.Email
{
    public interface ICabTransferEmailSender
    {
        Task<bool> SendCabTransferConfirmationToCabB(string email, string acceptingCabName, string providerName, string serviceName);
        Task<bool> SendCabTransferConfirmationToCabA(string email, string currentCabName, string acceptingCabName, string providerName, string serviceName);
        Task<bool> SendCabTransferCancellationToCabB(string email, string acceptingCabName, string providerName, string serviceName);
        Task<bool> SendCabTransferConfirmationToDSIT(string existingCabName, string acceptingCabName, string providerName, string serviceName);
        Task<bool> SendCabTransferCancellationToDSIT(string existingCabName, string decliningCabName, string providerName, string serviceName);
    }
}
