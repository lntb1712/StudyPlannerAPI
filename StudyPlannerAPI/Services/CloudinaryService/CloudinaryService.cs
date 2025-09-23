using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using StudyPlannerAPI.DTOs;

namespace StudyPlannerAPI.Services.CloudinaryService
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> options)
        {
            var cfg = options.Value;
            var acc = new Account(cfg.CloudName, cfg.ApiKey, cfg.ApiSecret);
            _cloudinary = new Cloudinary(acc);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string?> UploadFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0) return null;

            await using var stream = file.OpenReadStream();

            // Nếu là ảnh muốn crop/transform thì dùng ImageUploadParams, 
            // nhưng RawUploadParams hỗ trợ file nhiều loại (pdf, docx, zip,...)
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            if (result == null || result.StatusCode != System.Net.HttpStatusCode.OK) return null;

            return result.SecureUrl?.ToString();
        }
    }
}
