using App.Core.DTO.ResultPattern;
using App.Core.Enums;
using App.Core.ServiceContracts;
using App.Core.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace App.Infrastructure.Services
{
    /// <summary>
    /// Handles image upload, deletion, and URL resolution.
    /// Stores files in wwwroot/images/{folder}/.
    /// </summary>
    public class FileService : IFileService
    {
        private readonly AppSettings _appSettings;
        private readonly string _wwwRootPath;

        private const string ImagesFolder = "images";

        public FileService(IOptions<AppSettings> appSettings, IWebHostEnvironment env)
        {
            _appSettings = appSettings.Value;
            _wwwRootPath = env.WebRootPath
                ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }   

        // =========================================================
        // PUBLIC METHODS
        // =========================================================

        /// <inheritdoc/>
        public async Task<ServiceResult<string>> SaveImageAsync(IFormFile file, ImageFolder folder)
        {
            var validation = ValidateImage(file);
            if (!validation.Response.Success)
                return ServiceResult<string>.BadRequest(
                    validation.Response.Message,
                    validation.Response.Error?.Details);

            try
            {
                var folderPath = GetFolderPath(folder);
                EnsureDirectoryExists(folderPath);

                var fileName = GenerateFileName(file.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                var relativePath = BuildRelativePath(folder, fileName);
                return ServiceResult<string>.Success("Image uploaded successfully", relativePath);
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Internal(
                    "Failed to save image",
                    new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task DeleteImageAsync(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            try
            {
                var fullPath = Path.Combine(
                    _wwwRootPath,
                    relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(fullPath))
                    await Task.Run(() => File.Delete(fullPath));
            }
            catch
            {
                // Silently ignore — file may already be deleted or path invalid
            }
        }

        /// <inheritdoc/>
        public ServiceResult<object> ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return ServiceResult<object>.BadRequest("No image file provided");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_appSettings.AllowedImageExtensions.Contains(extension))
                return ServiceResult<object>.BadRequest(
                    $"Invalid image format. Allowed: {string.Join(", ", _appSettings.AllowedImageExtensions)}");

            var maxBytes = _appSettings.MaxImageSizeInMb * 1024 * 1024;
            if (file.Length > maxBytes)
                return ServiceResult<object>.BadRequest(
                    $"Image size exceeds the maximum allowed size of {_appSettings.MaxImageSizeInMb}MB");

            return ServiceResult<object>.Success("Image is valid");
        }

        /// <inheritdoc/>
        public string? BuildFullUrl(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return null;
            return $"{_appSettings.BaseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
        }

        // =========================================================
        // PRIVATE HELPERS
        // =========================================================

        private string GetFolderPath(ImageFolder folder)
        {
            var subFolder = folder.ToString().ToLower();
            return Path.Combine(_wwwRootPath, ImagesFolder, subFolder);
        }

        private static string BuildRelativePath(ImageFolder folder, string fileName)
        {
            return $"/{ImagesFolder}/{folder.ToString().ToLower()}/{fileName}";
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private static string GenerateFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
            return $"{Guid.NewGuid()}{extension}";
        }
    }
}