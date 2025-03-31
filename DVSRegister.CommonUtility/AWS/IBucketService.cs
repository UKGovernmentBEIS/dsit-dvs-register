using DVSRegister.CommonUtility.Models;

namespace DVSRegister.CommonUtility
{
    public interface IBucketService
    {
        public Task<GenericResponse> WriteToS3Bucket(Stream fileStream, string fileName);
        public Task<byte[]?> DownloadFileAsync(string keyName);
        public Task<GenericResponse> DeleteFromS3Bucket(string keyName);

    }
}

