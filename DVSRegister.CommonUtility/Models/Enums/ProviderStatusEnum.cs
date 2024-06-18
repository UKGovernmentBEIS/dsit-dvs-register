using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum ProviderStatusEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("Received")]
        Received = 1
    }
}
