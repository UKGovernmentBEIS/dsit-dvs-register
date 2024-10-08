﻿using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models
{
    public enum ServiceStatusEnum
    { //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("Submitted")]
        Submitted = 1,
        [Description("Received")] // Status when provider consents to proceed application, opening loop
        Received = 2,      
        [Description("Ready to publish")]//Status when provider consents to publish application, closing the loop
        ReadyToPublish = 3,
        [Description("Published")]
        Published = 4,
        [Description("Removed")]
        Removed = 5
    }
}
