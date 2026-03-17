using Microsoft.AspNetCore.Http;

namespace HMS.Services.Abstraction
{
    public interface IAttachementService
    {
        Task<string?> UploadFileAsync(IFormFile file, string folderName);

        bool DeleteFile(string fileName, string folderName);
    }
}
