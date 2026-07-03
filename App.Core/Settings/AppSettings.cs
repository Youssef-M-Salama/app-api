namespace App.Core.Settings
{
    public class AppSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public int EmailVerificationTokenExpirationHours { get; set; }
        public int MaxImageSizeInMb { get; set; }
        public string[] AllowedImageExtensions { get; set; } = [];
        public int MaxPdfSizeInMb { get; set; } = 5;
        public string[] AllowedPdfExtensions { get; set; } = [".pdf"];
    }
}