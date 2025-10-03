using System.Security.Cryptography;
using System.Text;

namespace LucasRT.DGBK.RestApi.Infrastructure
{
    /// <summary>
    /// Provides functionality to generate HMAC-SHA256 signatures.
    /// </summary>
    /// <remarks>This class is designed to create secure signatures for data using a secret key and a
    /// timestamp. It is typically used in scenarios where data integrity and authenticity need to be
    /// verified.</remarks>
    public static class HmacSigner
    {
        /// <summary>
        /// Generates a hexadecimal signature for the given input using HMAC-SHA256.
        /// </summary>
        /// <param name="secret">The secret key used for generating the HMAC signature. Cannot be null or empty.</param>
        /// <param name="timestamp">The timestamp to include in the signature. Represents the time at which the signature is generated.</param>
        /// <param name="body">The body content to include in the signature. Represents the data to be signed.</param>
        /// <returns>A lowercase hexadecimal string representing the HMAC-SHA256 signature of the combined timestamp and body.</returns>
        public static string Sign(string secret, string timestamp, string body)
        {
            using HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(secret));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{timestamp}.{body}"));

            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
