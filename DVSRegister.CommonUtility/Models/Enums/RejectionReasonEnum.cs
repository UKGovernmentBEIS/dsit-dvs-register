using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum RejectionReasonEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("Failed due diligence check")]
        DiligenceCheck,
        [Description("Submitted incorrect information ")]
        IncorrectInfo,
    }
}
