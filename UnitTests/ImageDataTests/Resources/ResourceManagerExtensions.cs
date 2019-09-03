using System.Resources;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace ImageDataTests.Resources
{
    static class ResourceManagerExtensions
    {
        public static byte[] GetImageBytes(this ResourceManager rm, string name)
        {
            byte[] bytes;
            var bitmap = rm.GetObject(name) as Bitmap;
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                bytes = stream.ToArray();
            }

            return bytes;
        }
    }
}
