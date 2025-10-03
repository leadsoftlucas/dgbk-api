using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Extensions;
using LucasRT.DGBK.RestApi.Domain;
using System.Security.Cryptography;
using System.Text;

namespace LucasRT.DGBK.RestApi.Infrastructure
{
    /// <summary>
    /// Provides functionality to generate HMAC-SHA256 signatures.
    /// </summary>
    /// <remarks>This class is designed to create secure signatures for data using a secret key and a timestamp. It is typically used in scenarios where data integrity and authenticity need to be verified.</remarks>
    public static class HmacSigner
    {
        /// <summary>
        /// Generates a hexadecimal signature for the given input using HMAC-SHA256.
        /// </summary>
        /// <returns>A lowercase hexadecimal string representing the HMAC-SHA256 signature of the combined timestamp and body.</returns>
        public static string SignHmac(this string body, out long timestamp)
        {
            using HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(EnvUtil.Get(EnvConstant.HMAC)));
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{timestamp}.{body}"));

            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        /// <summary>
        /// Verifies the HMAC signature of the provided message body using the specified signature header and timestamp.
        /// </summary>
        /// <remarks>This method uses the HMACSHA256 algorithm to compute the hash of the concatenated
        /// timestamp and body. The computed hash is compared to the signature provided in the signature header using a
        /// constant-time comparison to prevent timing attacks.</remarks>
        /// <param name="body">The message body to verify.</param>
        /// <param name="signatureHeader">The signature header containing the HMAC signature to verify.  Must start with "sha256=" followed by the
        /// hexadecimal signature.</param>
        /// <param name="timestamp">The timestamp used in the HMAC calculation.</param>
        /// <returns><see langword="true"/> if the HMAC signature is valid; otherwise, <see langword="false"/>.</returns>
        public static bool VerifyHmac(this string body, string signatureHeader, string timestamp)
        {
            // assinatura recebida no header: sha256=hex
            if (signatureHeader.IsNothing() || !signatureHeader.StartsWith("sha256="))
                return false;

            string signatureReceived = signatureHeader["sha256=".Length..];

            string data = $"{timestamp}.{body}";
            byte[] key = Encoding.UTF8.GetBytes(EnvUtil.Get(EnvConstant.HMAC));
            byte[] bytes = Encoding.UTF8.GetBytes(data);

            using HMACSHA256 hmac = new(key);
            byte[] hash = hmac.ComputeHash(bytes);
            string signatureCalculated = Convert.ToHexString(hash).ToLowerInvariant();

            return signatureReceived.ConstantTimeEquals(signatureCalculated);
        }

        /// <summary>
        /// Compares two strings for equality in a manner that is resistant to timing attacks.
        /// </summary>
        /// <remarks>This method performs a constant-time comparison, meaning it takes the same amount of
        /// time to compare two strings regardless of their content, which helps prevent timing attacks. Both strings
        /// must be of the same length to be considered equal.</remarks>
        /// <param name="a">The first string to compare. Must not be null.</param>
        /// <param name="b">The second string to compare. Must not be null.</param>
        /// <returns><see langword="true"/> if the strings are equal; otherwise, <see langword="false"/>.</returns>
        private static bool ConstantTimeEquals(this string a, string b)
        {
            if (a.Length != b.Length)
                return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
                result |= a[i] ^ b[i];

            return result == 0;
        }
    }
}
