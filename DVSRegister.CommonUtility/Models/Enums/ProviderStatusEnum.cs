using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum ProviderStatusEnum
    {//Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("Unpublished")]
        Unpublished = 1,
        [Description("Ready to publish")] // Action required
        ReadyToPublish = 2,
        [Description("Published")]
        Published = 3,
        [Description("Ready to publish")] //Published - action required
        ReadyToPublishNext = 4,
        [Description("Removed from register")]
        RemovedFromRegister = 5,
        [Description("Awaiting removal confirmation")]
        AwaitingRemovalConfirmation = 6,
        [Description("Awaiting removal confirmation")]
        CabAwaitingRemovalConfirmation = 7,
        [Description("Updates requsted")]
        UpdatesRequested = 8


    }
}
