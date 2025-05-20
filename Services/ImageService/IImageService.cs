namespace BackEnd.Services.ImageService
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
