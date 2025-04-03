using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Business.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Shared.Common;
using Shared.Model.Response;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Business.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly AwsKeys _awsConfig;
        private string _documentContainerName = "";

        public FileStorageService(IOptions<AwsKeys> awsConfig)
        {
            _awsConfig = awsConfig.Value;
        }

        public async Task<string> UploadFile(IFormFile file, string directory, string containerName)
        {
            string newFileName = Guid.NewGuid().ToString() + ".png";
            try
            {

                using (var ms = new MemoryStream())
                {

                    await file.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    string s = Convert.ToBase64String(fileBytes);
                    var originalImageStream = new MemoryStream(ms.ToArray());
                    var thumbnailImageStream = new MemoryStream();
                    // act on the Base64 data
                    bool uploadimg = await SendImagetoS3(newFileName, ms, directory);

                    CreateThumbnail(originalImageStream, thumbnailImageStream);
                    directory = "user/thumbnail_detail/";
                    // Upload thumbnail
                    bool uploadThumbnail = await SendImagetoS3(newFileName, thumbnailImageStream, directory);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }

            return newFileName;
        }

        public async Task<List<UploadedFileResponseModel>> UploadFile(IEnumerable<IFormFile> files, string directory, string containerName)
        {
            List<UploadedFileResponseModel> modelList = [];
            try
            {
                if (files != null && files.Any())
                {
                    foreach (var file in files)
                    {
                        string newFileName = UploadFile(file, directory, containerName).Result;

                        var model = new UploadedFileResponseModel
                        {
                            FileName = newFileName,
                            Title = file.FileName,
                            MediaType = file.ContentType
                        };
                        modelList.Add(model);
                    }
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return await Task.FromResult(modelList);
        }

        private async Task<bool> SendImagetoS3(string fileName, Stream stream, string subdirectory)
        {
            try
            {
                TransferUtility fileTransferUtility = new TransferUtility(new AmazonS3Client(_awsConfig.AwsAccessKeyId, _awsConfig.AwsSecretAccessKey, Amazon.RegionEndpoint.APSoutheast2));

                string bucketName = _awsConfig.ImageBucketName;

                // Upload original image
                TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    Key = "Uploads/" + subdirectory + fileName,
                    BucketName = bucketName
                };
                await fileTransferUtility.UploadAsync(uploadRequest);

                // Create and upload thumbnail
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        private void CreateThumbnail(Stream originalStream, Stream thumbnailStream)
        {
            // Reset the position of the original stream to the beginning
            originalStream.Position = 0;

            using (var image = SixLabors.ImageSharp.Image.Load(originalStream))
            {
                int thumbnailWidth = 200; // Define the width of the thumbnail
                image.Mutate(x => x.Resize(thumbnailWidth, 0)); // Resize, maintaining aspect ratio

                image.Save(thumbnailStream, new PngEncoder()); // Save the thumbnail to the stream
                thumbnailStream.Position = 0; // Reset the stream position to the beginning
            }
        }

        public async Task<string> UploadFileByBase64(string base64, string directory, string containerName)
        {
            _documentContainerName = containerName;
            string newFileName = Guid.NewGuid().ToString() + ".png";
            var fileBytes = Convert.FromBase64String(base64.Split(',')[1]);

            using (var ms = new MemoryStream(fileBytes))
            {
                await SendImagetoS3(newFileName, ms, directory);

                var originalImageStream = new MemoryStream(ms.ToArray());
                var thumbnailImageStream = new MemoryStream();
                // act on the Base64 data


                CreateThumbnail(originalImageStream, thumbnailImageStream);
                if(directory== "user/detail/")
                {
                    directory = "user/thumbnail_detail/";
                }
                else
                {
                    directory = "user/thumbnail_profile/";
                }
               
                // Upload thumbnail
                await SendImagetoS3(newFileName, thumbnailImageStream, directory);
            }


            if (!string.IsNullOrEmpty(newFileName))
                return newFileName;
            else
                return string.Empty;
        }


        public async Task DeleteFile(string pathToImage)
        {
            await DeleteFile(pathToImage, _documentContainerName);
        }

        private async Task DeleteFile(string fileName, string subdirectory)
        {
            var fileTransferUtility = new AmazonS3Client(_awsConfig.AwsAccessKeyId, _awsConfig.AwsSecretAccessKey, Amazon.RegionEndpoint.APSoutheast2);

            string bucket_name = _awsConfig.ImageBucketName;
            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = bucket_name,
                    Key = $"Uploads/user/detail/{fileName}"
                };
                await fileTransferUtility.DeleteObjectAsync(deleteRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }



        public async Task<string?> GetDocumentUrl(string pathToImage, string mediaType = "")
        {
            return await GetSasUrl(pathToImage, string.IsNullOrEmpty(mediaType) ? "images" : mediaType);
        }
        private async Task<bool> DoesS3ObjectExistAsync(string bucketName, string key)
        {
            IAmazonS3 s3Client = new AmazonS3Client(_awsConfig.AwsAccessKeyId, _awsConfig.AwsSecretAccessKey, Amazon.RegionEndpoint.APSoutheast2);
            try
            {
                var response = await s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = key
                });

                return true;
            }
            catch (AmazonS3Exception e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }
                throw;
            }
        }




        private async Task<string?> GetSasUrl(string pathToFile, string documentContainerName)
        {
            bool fileExists;
            if (documentContainerName == "videos")
            {
                fileExists = await DoesS3ObjectExistAsync(_awsConfig.VideoBucketName, pathToFile);
            }
            else
            {
                fileExists = await DoesS3ObjectExistAsync(_awsConfig.ImageBucketName, "Uploads/" + pathToFile);
            }



            if (!fileExists)
            {
                return null;
            }

            // Generate a pre-signed URL
            IAmazonS3 clientOnlyGet = (new AmazonS3Client(_awsConfig.AwsAccessKeyId, _awsConfig.AwsSecretAccessKey, Amazon.RegionEndpoint.APSoutheast2));
            GetPreSignedUrlRequest requestFileOnlyGet = new GetPreSignedUrlRequest();
            requestFileOnlyGet.BucketName = "" + _awsConfig.ImageBucketName;
            requestFileOnlyGet.Key = "Uploads/" + pathToFile;
            requestFileOnlyGet.Expires = DateTime.Now.AddMinutes(30);
            requestFileOnlyGet.Protocol = Protocol.HTTPS;

            if (documentContainerName == "videos")
            {
                requestFileOnlyGet.BucketName = _awsConfig.VideoBucketName;
                requestFileOnlyGet.Key = pathToFile;
            }
            return clientOnlyGet.GetPreSignedURL(requestFileOnlyGet);

        }

    }
}
