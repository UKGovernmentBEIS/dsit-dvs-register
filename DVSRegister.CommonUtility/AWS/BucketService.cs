using DVSRegister.CommonUtility.Models;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Amazon.S3.Transfer;

namespace DVSRegister.CommonUtility
{
    public class BucketService : IBucketService
    {
        private readonly S3Configuration config;
        private readonly ILogger logger;
        private readonly S3FileKeyGenerator keyGenerator;
        private readonly AmazonS3Client s3Client;


        public BucketService(IOptions<S3Configuration> options, ILogger<BucketService> logger, S3FileKeyGenerator keyGenerator, AmazonS3Client s3Client)
        {
            config = options.Value;
            this.logger = logger;
            this.keyGenerator = keyGenerator;
            this.s3Client = s3Client;
        }

        /// <summary>
        /// upload file to s3
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>

        public async Task<GenericResponse> WriteToS3Bucket(Stream fileStream, string fileName)
        {
            var keyName = keyGenerator.GetS3Key(fileName);
            try
            {
                var putRequest = new PutObjectRequest
                {
                    InputStream = fileStream,
                    BucketName = config.BucketName,
                    Key = keyName
                };

                var response = await s3Client.PutObjectAsync(putRequest);
                return new GenericResponse { Success = true, Data = keyName };
            }
            catch (AmazonS3Exception e)
            {
                logger.LogError("AWS S3 error when writing  file to bucket: '{0}', key: '{1}'. Message:'{2}'", config.BucketName, keyName, e.Message);                
                return new GenericResponse { Success = true };
                //return new GenericResponse { Success = false }; //ToDo: uncomment once bucket access ready
            }
            catch (Exception e)
            {
                logger.LogError("Error when writing file to bucket: '{0}', key: '{1}'. Message:'{2}'", config.BucketName, keyName, e.Message);
                //return new GenericResponse { Success = false };//ToDo: uncomment once bucket access ready
                return new GenericResponse { Success = true };
            }
        }

        /// <summary>
        /// Download file from s3
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public async Task<byte[]?> DownloadFileAsync(string keyName)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = config.BucketName,
                    Key = keyName
                };

                using (var response = await s3Client.GetObjectAsync(request))
                using (var responseStream = response.ResponseStream)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await responseStream.CopyToAsync(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (AmazonS3Exception e)
            {
                logger.LogError("AWS S3 error when writing CSV file to bucket: '{0}', key: '{1}'. Message:'{2}'", config.BucketName, keyName, e.Message);
                return null;
            }
            catch (Exception e)
            {
                logger.LogError("Error when writing file to bucket: '{0}', key: '{1}'. Message:'{2}'", config.BucketName, keyName, e.Message);
                return null;

            }
        }
    }
}

