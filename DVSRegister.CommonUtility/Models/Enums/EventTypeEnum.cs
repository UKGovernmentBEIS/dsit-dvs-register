namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum EventTypeEnum
    {
        NA = 0,
        Adduser = 1,
        AddProvider = 2,
        AddService = 3,
        CompanyInfoUpdate = 4,
        PrimaryContactUpdate = 5,
        SecondaryContactUpdate = 6,
        PublicContactUpdate = 7,
        CertificateReview = 8,
        RestoreCertificateReview = 9,
        AddOpeningLoopToken = 10,
        OpeningLoop = 11,
        RemoveOpeningLoopToken = 12,
        PICheck = 13,
        PICheckLog = 14,
        TrustmarkNumberGeneration = 15,
        AddClosingLoopToken = 16,
        ClosingTheLoop = 17,
        RemoveClosingLoopToken = 18,
        RegisterManagement = 19,
        RemoveProvider = 20,
        RemoveService = 21,
        RemoveProvider2i = 22,
        RemoveServices2i = 23,
        RemoveProviderRemovalToken = 24,
        RemovedByCronJob = 25,
        RemoveServiceRequestedByCab = 25,
        UpdateUser = 26,
        SaveAsDraftService = 27
    }
}
