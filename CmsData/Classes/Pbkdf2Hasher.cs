using System;
using System.Configuration;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using UtilityExtensions;

namespace CmsData.Classes
{
    public static class Pbkdf2Hasher
    {
        public static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);
            var saltString = Convert.ToBase64String(salt);

            var iterations = ConfigurationManager.AppSettings["Pbkdf2Iterations"].ToInt2() ?? 1000;
            var hashBytes = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, iterations, numBytesRequested:64);
            var hash = Convert.ToBase64String(hashBytes);

            var storageString = saltString + "|" + hash;
            return storageString;
        }
        public static string HashString(string s)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);
            var saltString = Convert.ToBase64String(salt);
            var setting = ConfigurationManager.AppSettings["Pbkdf2Salt"];
            if (setting.HasValue()) saltString = setting;
            salt = Convert.FromBase64String(saltString);
            var iterations = ConfigurationManager.AppSettings["Pbkdf2Iterations"].ToInt2() ?? 1000;
            var hashBytes = KeyDerivation.Pbkdf2(s, salt, KeyDerivationPrf.HMACSHA512, iterations, numBytesRequested:64);
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var a = hashedPassword.Split('|');
            var storedSalt = a[0];
            var storedHash = a[1];
            var saltBytes = Convert.FromBase64String(storedSalt);

            var iterations = ConfigurationManager.AppSettings["Pbkdf2Iterations"].ToInt2() ?? 1000;
            var calcHashBytes = KeyDerivation.Pbkdf2(providedPassword, saltBytes, KeyDerivationPrf.HMACSHA512, iterations, numBytesRequested:64);
            var calcHash = Convert.ToBase64String(calcHashBytes);

            return calcHash == storedHash;
        }
    }
}
