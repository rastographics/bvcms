using System.Text;
using ImageData;
using ImageDataTests.Resources;

namespace ImageDataTests.Support
{
    public class ImageDataTestBase
    {
        protected const string mimeTypeJpeg = "image/jpeg";
        protected const string mimeTypePng = "image/png";
        protected const string mimeTypeText = "text/plain";

        protected Image CreateJpegImage()
        {
            return new Image
            {
                Bits = new byte[] { 0x12, 0x12, 0x12, 0x12, 0x12, 0x12, 0x12, 0x12, 0x12, 0x12, 0x12, 0x12, 0x12, 0x12, 0x12, 0x12, 0x12, 0x12, 0x12, 0x12 },
                Mimetype = mimeTypeJpeg,
                Length = 20,
            };
        }

        protected byte[] GetImageBytes(string name)
        {
            return ImageDataTestsResources.ResourceManager.GetImageBytes(name);
        }

        protected Image ImageFromResource(string name)
        {
            var bytes = GetImageBytes(name);
            return new Image
            {
                Bits = bytes,
                Length = bytes.Length,
                Mimetype = mimeTypeJpeg,
            };
        }

        protected Image ImageFromText(string text)
        {
            var bytes = Encoding.ASCII.GetBytes(text);
            return new Image
            {
                Bits = bytes,
                Length = bytes.Length,
                Mimetype = mimeTypeText,
            };
        }
    }
}
