namespace EventManagmentTask.Repositories
{
    public interface ICloudinaryRepository
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
