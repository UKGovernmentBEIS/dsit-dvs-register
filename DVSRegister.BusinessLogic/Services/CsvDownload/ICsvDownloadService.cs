using DVSRegister.BusinessLogic.Models;

namespace DVSRegister.BusinessLogic.Services
{
    public interface ICsvDownloadService
    {
        public Task<CsvDownload> DownloadAsync();
    }
}