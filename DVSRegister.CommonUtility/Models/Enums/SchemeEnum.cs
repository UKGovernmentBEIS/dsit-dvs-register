using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum SchemeEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("Right to Work")]
        RightToWork = 1,
        [Description("Right to Rent")]
        RightToRent = 2,
        [Description("Disclosure and Barring Service")]
        DisclosureAndBarring = 3
    }
}
