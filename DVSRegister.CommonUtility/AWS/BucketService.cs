using Amazon.S3;
using Amazon.S3.Model;
using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace DVSRegister.CommonUtility
{
    public class BucketService : IBucketService
    {
   
        private readonly ILogger logger;
        private readonly S3FileKeyGenerator keyGenerator;
        private readonly AmazonS3Client s3Client;
        private readonly IConfiguration configuration;


        public BucketService(ILogger<BucketService> logger, S3FileKeyGenerator keyGenerator, AmazonS3Client s3Client, IConfiguration configuration)
        {
           
            this.logger = logger;
            this.keyGenerator = keyGenerator;
            this.s3Client = s3Client;
            this.configuration = configuration;
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
                logger.LogError(e, "AWS S3 error when writing  file to bucket: '{BucketName}', key: '{keyName}'", Helper.SanitizeForLog(bucketName), Helper.SanitizeForLog(keyName));
                return new GenericResponse { Success = false };
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error when writing file to bucket: '{BucketName}', key: '{keyName}'", Helper.SanitizeForLog(bucketName), Helper.SanitizeForLog(keyName));
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
                logger.LogError(e, "AWS S3 error when deleting  file from bucket: '{BucketName}', key: '{keyName}'.", Helper.SanitizeForLog(bucketName), Helper.SanitizeForLog(keyName));
                return new GenericResponse { Success = false };
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error when deleting file from bucket: '{BucketName}', key: '{keyName}'.", Helper.SanitizeForLog(bucketName), Helper.SanitizeForLog(keyName));
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
                logger.LogError(e, "AWS S3 error in DownloadFileAsync: '{BucketName}', key: '{keyName}'.", Helper.SanitizeForLog(bucketName), Helper.SanitizeForLog(keyName));
                return null;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in DownloadFileAsync: '{BucketName}', key: '{keyName}'.", Helper.SanitizeForLog(bucketName), Helper.SanitizeForLog(keyName));
                return null;
            }
        }

        /// <summary>
        /// Download using cloudfront domain 
        /// for local debug will download from localstack
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="prefix"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MemoryStream> GetPrefixZipAsync(string bucket, string prefix, string trustmarkNumber, string pngMainKey, string svgMainKey, string jpegMainKey, CancellationToken cancellationToken = default)
        {
            try
            {
                bool useCloudFront = Convert.ToBoolean(configuration["UseCloudFront"]);
                if(useCloudFront)
                {

                    string cloudFrontDomain = configuration["CloudfrontDomain"]!;
                    string[] pngArray = {
                           $"https://{cloudFrontDomain}/{pngMainKey}",
                           $"https://{cloudFrontDomain}/{pngMainKey.Replace("main","mono-black")}",
                           $"https://{cloudFrontDomain}/{pngMainKey.Replace("main","mono-white")}",
                           $"https://{cloudFrontDomain}/{pngMainKey.Replace("main","black-grey")}"

                        };


                    string[] svgArray = {
                           $"https://{cloudFrontDomain}/{svgMainKey}",
                           $"https://{cloudFrontDomain}/{svgMainKey.Replace("main","mono-black")}",
                           $"https://{cloudFrontDomain}/{svgMainKey.Replace("main","mono-white")}",
                           $"https://{cloudFrontDomain}/{svgMainKey.Replace("main","black-grey")}"

                        };

                    string[] jpegArray = {
                           $"https://{cloudFrontDomain}/{jpegMainKey}",
                           $"https://{cloudFrontDomain}/{jpegMainKey.Replace("main","mono-black")}",
                           $"https://{cloudFrontDomain}/{jpegMainKey.Replace("main","mono-white")}",
                           $"https://{cloudFrontDomain}/{jpegMainKey.Replace("main","black-grey")}"

                        };

                    using var workingMs = new MemoryStream();
                    using var zip = new ZipArchive(workingMs, ZipArchiveMode.Create, true);

                    if (prefix.Contains("png"))
                    {
                        await AddGroupAsync(zip, pngArray, $"{trustmarkNumber}-png", cancellationToken);
                    }
                    else if (prefix.Contains("jpeg"))
                    {
                        await AddGroupAsync(zip, jpegArray, $"{trustmarkNumber}-jpeg", cancellationToken);
                    }
                    else if (prefix.Contains("svg"))
                    {
                        await AddGroupAsync(zip, svgArray, $"{trustmarkNumber}-svg", cancellationToken);
                    }
                    else
                    {
                        // zip all into different folders inside tm/
                        await AddGroupAsync(zip, pngArray, $"{trustmarkNumber}/png", cancellationToken);
                        await AddGroupAsync(zip, jpegArray, $"{trustmarkNumber}/jpeg", cancellationToken);
                        await AddGroupAsync(zip, svgArray, $"{trustmarkNumber}/svg", cancellationToken);
                    }


                    zip.Dispose();                    
                    // Copy into a *new* MemoryStream that will be returned
                    var output = new MemoryStream(workingMs.ToArray());
                    output.Position = 0;
                    return output;


                }

                else
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
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "AWS S3 error GetPrefixZipAsync");
                return null!;
            }
        }





        /// <summary>
        /// Download file as stream useCloudFront = true - from cloud front
        /// useCloudFront = true - from local stack
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="keyName"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<MemoryStream?> DownloadFileStreamAsync(string keyName, string bucketName,  CancellationToken ct = default)
        {
            try
            {
                bool useCloudFront = Convert.ToBoolean(configuration["UseCloudFront"]);
                if (useCloudFront)
                {
                    string cloudFrontBaseUrl = configuration["CloudfrontDomain"]!;
                    var fileUrl = $"https://{cloudFrontBaseUrl}/{keyName}";

                    using var http = new HttpClient();

                    var response = await http.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead, ct);
                    response.EnsureSuccessStatusCode();

                    var ms = new MemoryStream();
                    await response.Content.CopyToAsync(ms, ct);
                    ms.Position = 0;
                    return ms; 

                }
                else
                {
                    var request = new GetObjectRequest { BucketName = bucketName, Key = keyName };
                    var response = await s3Client.GetObjectAsync(request, ct);


                    var ms = new MemoryStream();
                    await response.ResponseStream.CopyToAsync(ms, ct);
                    ms.Position = 0;


                    response.Dispose();

                    return ms; // caller disposes this stream
                }              
            }
            catch (AmazonS3Exception e)
            {
                logger.LogError(e, "S3 read error. Bucket: {Bucket}, Key: {Key}", Helper.SanitizeForLog(bucketName), Helper.SanitizeForLog(keyName));
                return null;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Read error. Bucket: {Bucket}, Key: {Key}", Helper.SanitizeForLog(bucketName), Helper.SanitizeForLog(keyName));
                return null;
            }
        }



        private async Task<byte[]> DownloadFromCloudFrontAsync(string url, CancellationToken cancellationToken)
        {
            using var http = new HttpClient();
            var response = await http.GetAsync(url, cancellationToken);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }


        private static async Task AddToZipAsync(ZipArchive zip, string entryName, byte[] data, CancellationToken cancellationToken)
        {
            var entry = zip.CreateEntry(entryName, CompressionLevel.Optimal);
            await using var stream = entry.Open();
            await stream.WriteAsync(data, cancellationToken);
        }


        private async Task AddGroupAsync(ZipArchive zip, string[] urls, string folderName, CancellationToken cancellationToken)
        {
            foreach (var url in urls)
            {
                var bytes = await DownloadFromCloudFrontAsync(url, cancellationToken);

                var fileName = Path.GetFileName(url);
                var entryName = $"{folderName}/{fileName}";

                await AddToZipAsync(zip, entryName, bytes, cancellationToken);
            }
        }


    }
}

