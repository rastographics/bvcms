using System.IO;
using Xunit;
using ImageData;
using System;
using System.Web;
using Shouldly;

namespace ImageDataTests
{
    [Collection("Image Collection")]
    public class ImageTests
    {
        [Theory]
        [InlineData("test.csv")]
        [InlineData("test.doc")]
        [InlineData("test.docx")]
        [InlineData("test.jpg")]
        [InlineData("test.pdf")]
        [InlineData("test.png")]
        [InlineData("test.xls")]
        [InlineData("test.xlsx")]
        public void CreateImageFromType_ShouldWork(string fileName)
        {
            byte[] bits;
            string mimetype;
            string workingDirectory = Environment.CurrentDirectory;
            using (FileStream stream = File.Open($@"TestDocuments/{fileName}", FileMode.Open))
            {
                bits = new byte[stream.Length];
                stream.Read(bits, 0, bits.Length);
                mimetype = MimeMapping.GetMimeMapping(stream.Name);
            }
            Image i = Image.CreateImageFromType(bits, mimetype);
            i.Mimetype.ShouldBe(mimetype);
            i.Bits.ShouldBe(bits);
            i.Length.ShouldBe(bits.Length);
        }        
    }
}
