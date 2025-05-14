
using SixLabors.ImageSharp.Formats.Webp;

namespace BackEnd.Services.ImageService
{
    public class ImageService : IImageService
    {
        public async Task<string> SaveWebpAsync(IFormFile file, string folderPath, string defaultPath)
        {
            if (file is { Length: > 0 })
            {
                var fileName = $"{Guid.NewGuid()}.webp";
                var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderPath);
                Directory.CreateDirectory(absolutePath);

                var filePath = Path.Combine(absolutePath, fileName);

                using var image = await Image.LoadAsync(file.OpenReadStream());

                var encoder = new WebpEncoder { Quality = 75 };
                await image.SaveAsync(filePath, encoder);

                return Path.Combine("/", folderPath, fileName).Replace("\\", "/");
            }

            return Path.Combine("/", folderPath, defaultPath).Replace("\\", "/");
        }
    }
}
