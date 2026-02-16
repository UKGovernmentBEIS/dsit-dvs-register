using Amazon.S3;
using Amazon.S3.Model;
using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace DVSRegister.CommonUtility
{
    public class BucketService : IBucketService
    {
   
        private readonly ILogger logger;
        private readonly S3FileKeyGenerator keyGenerator;
        private readonly AmazonS3Client s3Client;


        public BucketService(ILogger<BucketService> logger, S3FileKeyGenerator keyGenerator, AmazonS3Client s3Client)
        {
           
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

        public async Task<GenericResponse> WriteToS3Bucket(Stream fileStream, string fileName, string bucketName)
        {
            var keyName = keyGenerator.GetS3Key(fileName);
            try
            {
                var putRequest = new PutObjectRequest
                {
                    InputStream = fileStream,
                    BucketName = bucketName,
                    Key = keyName
                };

                var response = await s3Client.PutObjectAsync(putRequest);
                return new GenericResponse { Success = true, Data = keyName };
            }
            catch (AmazonS3Exception e)
            {
                logger.LogError("AWS S3 error when writing  file to bucket: '{BucketName}', key: '{keyName}'. Message:'{Message}'", bucketName, keyName, e.Message);
                return new GenericResponse { Success = false };
            }
            catch (Exception e)
            {
                logger.LogError("Error when writing file to bucket: '{BucketName}', key: '{keyName}'. Message:'{Message}'", bucketName, keyName, e.Message);
                return new GenericResponse { Success = false };               
            }
        }


        /// <summary>
        /// Delete file from S3
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>

        public async Task<GenericResponse> DeleteFromS3Bucket(string keyName, string bucketName)
        {           
            try
            {               
                DeleteObjectRequest request = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                var response = await s3Client.DeleteObjectAsync(request);
                return new GenericResponse { Success = true};
                
            }
            catch (AmazonS3Exception e)
            {
                logger.LogError("AWS S3 error when deleting  file from bucket: '{BucketName}', key: '{keyName}'. Message:'{Message}'", bucketName, keyName, e.Message);
                return new GenericResponse { Success = false };
            }
            catch (Exception e)
            {
                logger.LogError("Error when deleting file from bucket: '{BucketName}', key: '{keyName}'. Message:'{Message}'", bucketName, keyName, e.Message);
                return new GenericResponse { Success = false };
            }
        }

        /// <summary>
        /// Download file from s3
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public async Task<byte[]?> DownloadFileAsync(string keyName, string bucketName)
        {
            try
            {

                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
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
                logger.LogError("AWS S3 error when reading  file from bucket: '{0}', Message:'{1}'", bucketName,  e.Message);
                return null;
            }
            catch (Exception e)
            {
                logger.LogError("Error when reading file from bucket: '{0}',  Message:'{1}'", bucketName,  e.Message);
                return null;
            }
        }

        /// <summary>
        /// Download as zip from a prefix path
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="prefix"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MemoryStream> GetPrefixZipAsync(string bucket, string prefix, CancellationToken cancellationToken = default)
        {


            try
            {
                var ms = new MemoryStream();
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
                {

                    var list = await s3Client.ListObjectsV2Async(new ListObjectsV2Request
                    {
                        BucketName = bucket,
                        Prefix = prefix
                    }, cancellationToken);

                    foreach (var item in list.S3Objects)
                    {

                        if (item.Key.EndsWith("/") && item.Size == 0) continue;

                        var entryName = item.Key.StartsWith(prefix, StringComparison.Ordinal)
                            ? item.Key.Substring(prefix.Length)
                            : item.Key;

                        if (string.IsNullOrEmpty(entryName)) continue;

                        var entry = zip.CreateEntry(entryName, CompressionLevel.Optimal);
                        await using var entryStream = entry.Open();
                        using var obj = await s3Client.GetObjectAsync(bucket, item.Key, cancellationToken);
                        await obj.ResponseStream.CopyToAsync(entryStream, cancellationToken);
                    }
                }

                ms.Position = 0;
                return ms;
            }
            catch (Exception ex)
            {
                logger.LogError("AWS S3 error GetPrefixZipAsync : '{Message}'", ex.Message);
                return null!;
            }
        }
        /// <summary>
        /// Download file as stream
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="keyName"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<MemoryStream?> DownloadFileStreamAsync(string keyName, string bucketName, CancellationToken ct = default)
        {
            try
            {
                var request = new GetObjectRequest { BucketName = bucketName, Key = keyName };
                var response = await s3Client.GetObjectAsync(request, ct);


                var ms = new MemoryStream();
                await response.ResponseStream.CopyToAsync(ms, ct);
                ms.Position = 0;


                response.Dispose();

                return ms; // caller disposes this stream
            }
            catch (AmazonS3Exception e)
            {
                logger.LogError(e, "S3 read error. Bucket: {Bucket}, Key: {Key}", bucketName, keyName);
                return null;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Read error. Bucket: {Bucket}, Key: {Key}", bucketName, keyName);
                return null;
            }
        }
    }
}

