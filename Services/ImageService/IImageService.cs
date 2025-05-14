namespace BackEnd.Services.ImageService
{
    public interface IImageService
    {
        Task<string> SaveWebpAsync(IFormFile file, string folderPath, string defaultPath);
    }
}
