namespace DVSRegister.BusinessLogic.Models
{
    public class CsvDownload
    {
        public byte[] FileContent { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}