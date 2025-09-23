namespace StudyPlannerAPI.Services.CloudinaryService
{
    public interface ICloudinaryService
    {
        Task<string?> UploadFileAsync(IFormFile file, string folder);
    }
}
