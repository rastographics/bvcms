using System.IO;
using Xunit;
using ImageData;
using System;
using System.Web;

namespace ImageDataTests
{
    [Collection("Image Collection")]
    public class ImageTests
    {
        [Fact]
        public void NewImagePDFTest()
        {
            byte[] bits;
            string mimetype;
            string workingDirectory = Environment.CurrentDirectory;
            using (FileStream stream = File.Open(@"TestDocuments/test.pdf", FileMode.Open))
            {
                bits = new byte[stream.Length];
                stream.Read(bits, 0, bits.Length);
                mimetype = MimeMapping.GetMimeMapping(stream.Name);
            }
            int MediumId = Image.NewImageFromBits(bits, mimetype).Id;
            Image.DeleteOnSubmit(MediumId);
        }

        [Fact]
        public void NewImageJPGTest()
        {
            byte[] bits;
            string workingDirectory = Environment.CurrentDirectory;
            using (FileStream stream = File.Open(@"TestDocuments/test.jpg", FileMode.Open))
            {
                bits = new byte[stream.Length];
                stream.Read(bits, 0, bits.Length);
            }
            int LargeId = Image.NewImageFromBits(bits).Id;
            Image.DeleteOnSubmit(LargeId);
        }
    }
}
