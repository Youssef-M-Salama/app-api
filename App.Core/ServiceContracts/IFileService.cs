using App.Core.DTOs.ResultPattern;
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
        /// Formats an image path to return a URL starting with /images.
        /// Returns null if relativePath is null or empty.
        /// </summary>
        string? GetImageUrl(string? relativePath);

        /// <summary>
        /// Validates and saves a PDF to wwwroot/pdfs/{folder}/.
        /// Returns the relative path e.g. /pdfs/verifications/abc123.pdf
        /// </summary>
        Task<ServiceResult<string>> SavePdfAsync(IFormFile file, PdfFolder folder);

        /// <summary>
        /// Deletes a PDF file by its relative path.
        /// Silently ignores if the file does not exist.
        /// </summary>
        Task DeletePdfAsync(string? relativePath);

        /// <summary>
        /// Validates the PDF without saving it.
        /// Checks size and extension.
        /// </summary>
        ServiceResult<object> ValidatePdf(IFormFile file);

        /// <summary>
        /// Formats a PDF path to return a URL starting with /pdfs.
        /// Returns null if relativePath is null or empty.
        /// </summary>
        string? GetPdfUrl(string? relativePath);
    }
}