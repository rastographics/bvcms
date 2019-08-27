using ImageData;
using ImageDataTests.Resources;
using ImageDataTests.Support;
using SharedTestFixtures;
using Shouldly;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UtilityExtensions;
using Xunit;

namespace ImageDataTests
{
    [Collection(Collections.Database)]
    public class CMSImageDataContextTests : ImageDataTestBase
    {
        [Theory]
        [InlineData("text/plain", @"<div>this is an html fragment</div>")]
        [InlineData("text/plain", @"<?xml version=""1.0"" encoding=""utf-8""?><doc>this is an xml fragment</doc>")]
        [InlineData("image/svg", @"<svg><g><circle fill=""#FF0000"" stroke=""#000000"" stroke-width=""5"" cx=""83"" cy=""83"" r=""83""/></g></svg>")]
        public void ContentTest(string contentType, string content)
        {
            var db = CMSImageDataContext.Create(Util.Host);
            var image = new Image
            {
                Bits = Encoding.UTF8.GetBytes(content),
                Mimetype = contentType,
                Length = content.Length,
            };
            db.Images.InsertOnSubmit(image);
            db.SubmitChanges();

            var expected = contentType.Equals("text/plain") ? content : null;

            db.Content(image.Id).ShouldBe(expected);
        }

        [Fact]
        public void DeleteOnSubmitTest()
        {
            var db = CMSImageDataContext.Create(Util.Host);
            var image = CreateJpegImage();
            db.Images.InsertOnSubmit(image);
            db.SubmitChanges();

            db.DeleteOnSubmit(image.Id);
            db.SubmitChanges();
            db.Images.Where(i => i.Id == image.Id).Any().ShouldBeFalse();
        }

        [Fact]
        public void UpdateImageFromBitsTest()
        {
            int id;
            using (var db = CMSImageDataContext.Create(Util.Host))
            {
                var image = CreateJpegImage();
                db.Images.InsertOnSubmit(image);
                db.SubmitChanges();
                id = image.Id;
                image = null;
            }

            Image newImage;

            var bytes = GetImageBytes("Untitled");

            using (var db = CMSImageDataContext.Create(Util.Host))
            {
                newImage = db.UpdateImageFromBits(id, bytes);
            }
            var expected = GetImageBytes("UpdateImageFromBitsTest");
            newImage.Bits.ShouldBe(expected);
            newImage.Mimetype.ShouldBe("image/jpeg");
        }
    }
}
