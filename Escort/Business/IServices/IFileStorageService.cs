using Microsoft.AspNetCore.Http;
using Shared.Model.Response;

namespace Business.IServices
{
    public interface IFileStorageService
    {
        Task<string> UploadFile(IFormFile file, string directory, string containerName);
        Task<List<UploadedFileResponseModel>> UploadFile(IEnumerable<IFormFile> files, string directory, string containerName);
        Task<string?> GetDocumentUrl(string pathToImage, string mediaType = "");
        Task DeleteFile(string pathToImage);
        Task<string> UploadFileByBase64(string base64, string directory, string containerName);

        //For save video


    }
}
