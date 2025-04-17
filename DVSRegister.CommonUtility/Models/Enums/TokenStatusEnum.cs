namespace DVSRegister.CommonUtility.Models
{

    //Donot change order of the enum as the ids are used to save in database
    //New entries should be added at the last
    public enum TokenStatusEnum
    {
        NA = 0,//Token not generated yet
        Requested = 1,
        Expired = 2,
        AdminCancelled = 3,
        UserCancelled = 4,
        RequestCompleted = 5,
        RequestResent = 6

    }
}
