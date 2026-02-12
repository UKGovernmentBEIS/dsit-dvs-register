using DVSRegister.CommonUtility.Models;

namespace DVSRegister.CommonUtility
{
    public interface IBucketService
    {
        public Task<GenericResponse> WriteToS3Bucket(Stream fileStream, string fileName, string bucketName);
        public Task<byte[]?> DownloadFileAsync(string keyName, string bucketName);
        public Task<GenericResponse> DeleteFromS3Bucket(string keyName, string bucketName);

    }
}

