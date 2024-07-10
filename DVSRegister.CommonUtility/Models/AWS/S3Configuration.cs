namespace DVSRegister.CommonUtility.Models
{
    public class S3Configuration
    {
        public const string ConfigSection = "S3Config";

        public string BucketName { get; set; }

        public string Region { get; set; }
    }
}
