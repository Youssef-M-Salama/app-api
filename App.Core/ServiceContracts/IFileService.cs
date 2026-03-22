using App.Core.DTO.ResultPattern;
using App.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace App.Core.ServiceContracts
{
    /// <summary>
    /// Service for handling file upload, deletion, and URL resolution.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Validates and saves an image to wwwroot/images/{folder}/.
        /// Returns the relative path e.g. /images/users/abc123.jpg
        /// </summary>
        Task<ServiceResult<string>> SaveImageAsync(IFormFile file, ImageFolder folder);

        /// <summary>
        /// Deletes an image file by its relative path.
        /// Silently ignores if the file does not exist.
        /// </summary>
        Task DeleteImageAsync(string? relativePath);

        /// <summary>
        /// Validates the image without saving it.
        /// Checks size and extension.
        /// </summary>
        ServiceResult<object> ValidateImage(IFormFile file);

        /// <summary>
        /// Builds a full URL from a relative path using BaseUrl from settings.
        /// Returns null if relativePath is null or empty.
        /// </summary>
        string? BuildFullUrl(string? relativePath);
    }
}