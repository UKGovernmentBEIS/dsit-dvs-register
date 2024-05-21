using DVSRegister.CommonUtility.Models;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;

namespace DVSRegister.BusinessLogic.Services.CAB
{
	public class BucketService : IBucketService
	{
        private readonly AmazonS3Client _s3Client;
        private readonly string _bucketName;

        public BucketService(string bucketName, string accessKey, string password)
        {
            var awsCredentials = new BasicAWSCredentials(accessKey, password);
            _s3Client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.EUWest2);
            _bucketName = bucketName;
        }

        public async Task<GenericResponse> WriteToS3Bucket(Stream fileStream, string fileName)
        {
            try
            {
                    // Ensure the file stream is at the beginning
                    fileStream.Position = 0;

                    // Create a PutObjectRequest
                    var putRequest = new PutObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = fileName,
                        InputStream = fileStream
                    };

                    // Upload the file
                    PutObjectResponse response = await _s3Client.PutObjectAsync(putRequest);

                    // Generate a pre-signed URL if needed
                    string url = _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
                    {
                        BucketName = _bucketName,
                        Key = fileName,
                        Expires = DateTime.UtcNow.AddHours(1) // URL expiry time
                    });

                    return new GenericResponse
                    {
                        Success = true,
                        Data = url
                        
                    };
            }
            catch (Exception ex)
            {
                    return new GenericResponse
                    {
                        Success = false,
                        Data = $"Error uploading file: {ex.Message}"
                    };
                }
            }
    }
}

