namespace DVSRegister.CommonUtility.Models
{
    public class S3FileKeyGenerator
    {
        public string GetS3Key(string fileName)
        {
            var random = new Random();
            long uniqueId = DateTime.UtcNow.Ticks + random.Next();
            int lastIndex = fileName.LastIndexOf('.');
            string fileNameWithoutExtension = fileName.Substring(0, lastIndex);       
            return $"{fileNameWithoutExtension}_{uniqueId}.pdf";
        }
    }
}
