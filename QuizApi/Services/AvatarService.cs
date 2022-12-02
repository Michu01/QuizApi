using Microsoft.Extensions.Hosting;

using QuizApi.Models;

namespace QuizApi.Services
{
    public class AvatarService : IAvatarService
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        private readonly ISet<string> allowedExtensions = new HashSet<string>()
        {
            ".jpg", ".png", ".gif", ".jpeg"
        };

        private string AvatarsPath => Path.Combine(webHostEnvironment.WebRootPath, "images", "avatars");

        public AvatarService(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        private string[] GetMatches(int userId) => Directory.GetFiles(AvatarsPath, $"{userId}.*");

        public async Task<string> Change(IFormFile file, int userId)
        {
            string? extension = Path.GetExtension(file.FileName);

            if (extension is null || !allowedExtensions.Contains(extension))
            {
                throw new Exception($"Invalid extension. Accepted extensions are: {string.Join(',', allowedExtensions)}");
            }

            foreach (string match in GetMatches(userId))
            {
                File.Delete(match);
            }

            string path = Path.Combine(AvatarsPath, $"{userId}{extension}");

            using FileStream fileStream = new(path, FileMode.Create);

            await file.CopyToAsync(fileStream);

            return Path.GetRelativePath(webHostEnvironment.WebRootPath, path);
        }

        public string? GetPath(int id)
        {
            string[] matches = GetMatches(id);

            if (matches.Length == 0)
            {
                return null;
            }

            return Path.GetRelativePath(webHostEnvironment.WebRootPath, matches[0]);
        }
    }
}
