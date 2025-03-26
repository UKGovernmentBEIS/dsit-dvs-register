using System.ComponentModel;

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
        [Description("Removed from register")]
        Removed = 5,
        [Description("Awaiting removal confirmation")]
        AwaitingRemovalConfirmation = 6,
        [Description("Removal request sent to DSIT")]
        CabAwaitingRemovalConfirmation = 7,
        [Description("Saved as draft")]
        SavedAsDraft = 8,
        [Description("Updates requested")]
        UpdatesRequested = 9,
        [Description("Amendments needed")]
        AmendmentsRequired = 10,
        [Description("Submitted")]
        Resubmitted = 11

    }
}
