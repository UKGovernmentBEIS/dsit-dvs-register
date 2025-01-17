using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum ServiceRemovalReasonEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("The service certificate has expired")]
        RemovedByCronJob = 1,
        [Description("The service provider has requested to remove one or more services, and there are remaining services published on the register for this provider")]
        ProviderRequestedRemoval,
        [Description("The service provider no longer provides the listed service")]
        ProviderNotExists,
    }
}
