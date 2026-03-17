using HMS.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HMS.Services.Helpers
{
    public class AttachementService : IAttachementService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024; //5MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".svg" };
        private readonly ILogger<AttachementService> _logger;

        public AttachementService(ILogger<AttachementService> logger)
        {
            _logger = logger;
        }
        public async Task<string?> UploadFileAsync(IFormFile file, string folderName)
        {
            try
            {
                if (file is null || file.Length == 0)
                    return null;

                if (file.Length > _maxFileSize)
                    return null;

                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!_allowedExtensions.Contains(extension))
                    return null;

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folderName);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + extension;

                var fullPath = Path.Combine(folderPath, fileName);

                using var fileStream = new FileStream(fullPath, FileMode.Create);

                await file.CopyToAsync(fileStream);

                return fileName;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Un expected error happend while uploading image on stream");

                return null;
            }

        }

        public bool DeleteFile(string fileName, string folderName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName))
                    return false;

                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folderName, fileName);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Un expected error happend while deleting image from server");
                return false;
            }
        }


    }
}
