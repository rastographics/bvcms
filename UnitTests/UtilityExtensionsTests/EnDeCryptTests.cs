using System;
using System.Linq;
using UtilityExtensions;
using Xunit;

namespace UtilityExtensionsTests
{
    [Collection("EnDeCrypt Tests")]
    public class EnDeCryptTests
    {
        [Fact]
        public void EncryptTest()
        {
            string randomString = RandomString(20);
            string sEncrypted = Util.Encrypt(randomString);
            string sDecrypted = Util.Decrypt(sEncrypted);

            Assert.Equal(randomString, sDecrypted);
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
