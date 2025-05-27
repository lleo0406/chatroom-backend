
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using SixLabors.ImageSharp.Formats.Webp;

namespace BackEnd.Services.ImageService
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public ImageService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            var webpStream = await ConvertToWebpStreamAsync(file);
            if (webpStream == null) return null;

            webpStream.Position = 0;

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, webpStream),
                Folder = "avatars",
                Format = "webp"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString();
        }

        private async Task<MemoryStream> ConvertToWebpStreamAsync(IFormFile file)
        {
            if (file is { Length: > 0 })
            {
                using var image = await Image.LoadAsync(file.OpenReadStream());

                var outputStream = new MemoryStream();
                var encoder = new WebpEncoder { Quality = 75 };
                await image.SaveAsync(outputStream, encoder);

                outputStream.Position = 0;
                return outputStream;
            }

            return null;
        }
    }
}
