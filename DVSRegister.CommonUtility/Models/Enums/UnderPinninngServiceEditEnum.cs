namespace DVSRegister.CommonUtility.Models
{
    //Donot change order of the enum as the ids are used to save in database
    //New entries should be added at the last
    public enum UnderPinninngServiceEditEnum
    {
        PublishedToPublished= 1,
        PublishedToSelectOrChangeManual = 2,        
        PublishedToEnterManual = 3,
        ManualToPublished = 4,
        ManualToSelectOrChangeManual = 5,
        ManualToEnterManual = 6
     
    }
}
