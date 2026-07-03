using System.Linq;

namespace App.Core.Helpers
{
    public static class PhoneNumberHelper
    {
        /// <summary>
        /// Normalizes an Egyptian phone number to the international format +20...
        /// Assumes the input is already validated (10 or 11 digits).
        /// </summary>
        public static string? NormalizeEgyptianPhoneNumber(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return phone;

            // Remove any non-digits
            var digits = new string(phone.Where(char.IsDigit).ToArray());

            if (digits.StartsWith("01") && digits.Length == 11)
            {
                return "+20" + digits.Substring(1);
            }

            if (digits.StartsWith("1") && digits.Length == 10)
            {
                return "+20" + digits;
            }

            return phone; // Return as is if it doesn't match expected patterns
        }
    }
}
