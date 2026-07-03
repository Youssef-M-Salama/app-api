using App.Core.DTOs.ResultPattern;
using App.Core.Enums;
using App.Core.ServiceContracts;
using App.Core.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<FileService> _logger;

        private const string ImagesFolder = "images";
        private const string PdfsFolder = "pdfs";

        public FileService(IOptions<AppSettings> appSettings, IWebHostEnvironment env, ILogger<FileService> logger)
        {
            _appSettings = appSettings.Value;
            _wwwRootPath = env.WebRootPath
                ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            _logger = logger;
        }

        // =========================================================
        // PUBLIC METHODS
        // =========================================================

        /// <inheritdoc/>
        public async Task<ServiceResult<string>> SaveImageAsync(IFormFile file, ImageFolder folder)
        {
            try
            {
                var validation = ValidateImage(file);
                if (!validation.Response.Success)
                    return ServiceResult<string>.BadRequest(
                        validation.Response.Message,
                        validation.Response.Error?.Details);

                var folderPath = GetFolderPath(folder);
                EnsureDirectoryExists(folderPath);

                var fileName = GenerateFileName(file.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                var relativePath = BuildRelativePath(folder, fileName);
                return ServiceResult<string>.Success("تم رفع الصورة بنجاح", relativePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save image to folder {Folder}", folder);
                return ServiceResult<string>.Internal(
                    "فشل حفظ الصورة",
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
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete image at {RelativePath}", relativePath);
                // Silently ignore — file may already be deleted or path invalid
            }
        }

        /// <inheritdoc/>
        public ServiceResult<object> ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return ServiceResult<object>.BadRequest("لم يتم توفير ملف صورة");

            if (_appSettings == null)
                return ServiceResult<object>.Internal("إعدادات التطبيق غير مهيأة");

            var fileName = file.FileName ?? "unnamed.jpg";
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (_appSettings.AllowedImageExtensions == null || _appSettings.AllowedImageExtensions.Length == 0)
                return ServiceResult<object>.Internal("امتدادات الصور المسموح بها غير مهيأة");

            if (!_appSettings.AllowedImageExtensions.Contains(extension))
                return ServiceResult<object>.BadRequest(
                    $"تنسيق الصورة غير صالح. المسموح به: {string.Join(", ", _appSettings.AllowedImageExtensions)}");

            var maxBytes = (_appSettings.MaxImageSizeInMb > 0 ? _appSettings.MaxImageSizeInMb : 2) * 1024 * 1024;
            if (file.Length > maxBytes)
                return ServiceResult<object>.BadRequest(
                    $"حجم الصورة يتجاوز الحد الأقصى المسموح به وهو {_appSettings.MaxImageSizeInMb} ميجابايت");

            return ServiceResult<object>.Success("الصورة صالحة");
        }

        /// <inheritdoc/>
        public string? GetImageUrl(string? relativePath)
        {
            var baseUrl = _appSettings?.BaseUrl?.TrimEnd('/') ?? "";

            if (string.IsNullOrWhiteSpace(relativePath))
                return $"{baseUrl}/images/dummy.jpg";

            // Ensure path starts with /
            var path = relativePath.StartsWith("/") ? relativePath : "/" + relativePath;

            return $"{baseUrl}{path}";
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<string>> SavePdfAsync(IFormFile file, PdfFolder folder)
        {
            try
            {
                var validation = ValidatePdf(file);
                if (!validation.Response.Success)
                    return ServiceResult<string>.BadRequest(
                        validation.Response.Message,
                        validation.Response.Error?.Details);

                var folderPath = GetPdfFolderPath(folder);
                EnsureDirectoryExists(folderPath);

                var fileName = GenerateFileName(file.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                var relativePath = BuildPdfRelativePath(folder, fileName);
                return ServiceResult<string>.Success("تم رفع ملف PDF بنجاح", relativePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save PDF to folder {Folder}", folder);
                return ServiceResult<string>.Internal(
                    "فشل حفظ ملف PDF",
                    new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task DeletePdfAsync(string? relativePath)
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
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete PDF at {RelativePath}", relativePath);
                // Silently ignore — file may already be deleted or path invalid
            }
        }

        /// <inheritdoc/>
        public ServiceResult<object> ValidatePdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return ServiceResult<object>.BadRequest("لم يتم توفير ملف PDF");

            if (_appSettings == null)
                return ServiceResult<object>.Internal("إعدادات التطبيق غير مهيأة");

            var fileName = file.FileName ?? "unnamed.pdf";
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            var allowedExtensions = _appSettings.AllowedPdfExtensions ?? new[] { ".pdf" };

            if (!allowedExtensions.Contains(extension))
                return ServiceResult<object>.BadRequest(
                    $"تنسيق الملف غير صالح. المسموح به: {string.Join(", ", allowedExtensions)}");

            var maxMb = _appSettings.MaxPdfSizeInMb > 0 ? _appSettings.MaxPdfSizeInMb : 5;
            var maxBytes = maxMb * 1024 * 1024;
            
            if (file.Length > maxBytes)
                return ServiceResult<object>.BadRequest(
                    $"حجم الملف يتجاوز الحد الأقصى المسموح به وهو {maxMb} ميجابايت");

            return ServiceResult<object>.Success("الملف صالح");
        }

        /// <inheritdoc/>
        public string? GetPdfUrl(string? relativePath)
        {
            var baseUrl = _appSettings?.BaseUrl?.TrimEnd('/') ?? "";

            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            // Ensure path starts with /
            var path = relativePath.StartsWith("/") ? relativePath : "/" + relativePath;

            return $"{baseUrl}{path}";
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
            {
                var directory = Directory.CreateDirectory(path);
                // Ensure directory is created and accessible
                if (!directory.Exists)
                    throw new IOException($"Failed to create directory at {path}");
            }
        }

        private static string GenerateFileName(string? originalFileName)
        {
            var extension = !string.IsNullOrEmpty(originalFileName)
                ? Path.GetExtension(originalFileName).ToLowerInvariant()
                : ".jpg";
            return $"{Guid.NewGuid()}{extension}";
        }

        private string GetPdfFolderPath(PdfFolder folder)
        {
            var subFolder = folder.ToString().ToLower();
            return Path.Combine(_wwwRootPath, PdfsFolder, subFolder);
        }

        private static string BuildPdfRelativePath(PdfFolder folder, string fileName)
        {
            return $"/{PdfsFolder}/{folder.ToString().ToLower()}/{fileName}";
        }
    }
}