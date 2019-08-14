using Xunit;
using SharedTestFixtures;
using ImageData;
using UtilityExtensions;
using ImageDataTests.Support;
using System.Linq;
using Shouldly;
using System.IO;
using System.Text;

namespace ImageDataTests
{
    [Collection(Collections.Database)]
    public class ImageTests : ImageDataTestBase
    {
        [Fact]
        public void DeleteTest()
        {
            var image = CreateJpegImage();
            using (var db = CMSImageDataContext.Create(Util.Host))
            {
                db.Images.InsertOnSubmit(image);
                db.SubmitChanges();
                db.Images.Where(i => i.Id == image.Id).Any().ShouldBeTrue();
            }

            using (var db = CMSImageDataContext.Create(Util.Host))
            {
                Image.Delete(db, image.Id);
                db.Images.Where(i => i.Id == image.Id).Any().ShouldBeFalse();
            }
        }

        [Fact]
        public void ImageFromIdTest()
        {
            var image = CreateJpegImage();
            using (var db = CMSImageDataContext.Create(Util.Host))
            {
                db.Images.InsertOnSubmit(image);
                db.SubmitChanges();
                Image.ImageFromId(db, image.Id).ShouldNotBeNull();
            }

        }

        [Fact]
        public void NewImageFromBitsTest()
        {
            using (var db = CMSImageDataContext.Create(Util.Host))
            {
                var image = Image.NewImageFromBits(GetImageBytes("Untitled"), db);
                image.Mimetype.ShouldBe(mimeTypeJpeg);
                db.Images.Where(i => i.Id == image.Id).Any().ShouldBeTrue();

                image = Image.NewImageFromBits(GetImageBytes("RatioTest"), mimeTypePng, db);
                image.Mimetype.ShouldBe(mimeTypePng);
                db.Images.Where(i => i.Id == image.Id).Any().ShouldBeTrue();

                image = Image.NewImageFromBits(GetImageBytes("RatioTest"), 50, 50, db);
                image.Mimetype.ShouldBe(mimeTypeJpeg);
                db.Images.Where(i => i.Id == image.Id).Any().ShouldBeTrue();
                using (var ms = new MemoryStream(image.Bits))
                {
                    using (var bitmap = System.Drawing.Image.FromStream(ms))
                    {
                        bitmap.Width.ShouldBe(12);
                    }
                }
            }
        }

        [Fact]
        public void RatioTest()
        {
            var image = ImageFromResource("RatioTest");
            image.Ratio().ShouldBe(0.25);
        }

        [Fact]
        public void CreateNewTinyImageTest()
        {
            Image resized;
            using (var db = CMSImageDataContext.Create(Util.Host))
            {
                var image = ImageFromResource("CreateNewTinyImageTest");
                resized = image.CreateNewTinyImage(db);
            }

            resized.Bits.Length.ShouldBe(1249);
            using (var ms = new MemoryStream(resized.Bits))
            {
                var i = System.Drawing.Image.FromStream(ms);
                i.Width.ShouldBe(50);
                i.Height.ShouldBe(50);
            }
        }

        [Fact]
        public void ToStringTest()
        {
            var image = ImageFromResource("Untitled");
            image.ToString().ShouldBeNull();

            var text = "The quick brown fox jumps over the lazy dog";
            image = ImageFromText(text);
            image.ToString().ShouldBe(text);
        }

        [Fact]
        public void GetBitmapTest()
        {
            var image = ImageFromResource("CreateNewTinyImageTest");
            var bitmap = image.GetBitmap();
            bitmap.Width.ShouldBe(100);
            bitmap.Height.ShouldBe(100);

            bitmap = image.GetBitmap(30, 40);
            bitmap.Width.ShouldBe(30);
            bitmap.Height.ShouldBe(30);

            bitmap = image.GetBitmap(40, 30);
            bitmap.Width.ShouldBe(30);
            bitmap.Height.ShouldBe(30);
        }

        [Theory]
        [InlineData(25, 25, null, 25, 25)]
        [InlineData(60, 50, "max", 50, 50)]
        [InlineData(40, 30, "none", 40, 30)]
        [InlineData(40, 30, "crop", 40, 30)]
        [InlineData(40, 30, "pad", 40, 30)]
        public void ResizeFromBitsTest(int w, int h, string mode, int width, int height)
        {
            var bytes = GetImageBytes("CreateNewTinyImageTest");
            var resized = Image.ResizeFromBits(bytes, w, h, mode);
            using (var ms = new MemoryStream(resized))
            {
                var i = System.Drawing.Image.FromStream(ms);
                i.Width.ShouldBe(width);
                i.Height.ShouldBe(height);
            }
        }

        [Fact]
        public void ResizeToStreamTest()
        {
            var image = ImageFromResource("CreateNewTinyImageTest");
            using (var stream = image.ResizeToStream(75, 75))
            {
                stream.Length.ShouldBe(1408);
                var bitmap = System.Drawing.Image.FromStream(stream);
                bitmap.Width.ShouldBe(75);
                bitmap.Height.ShouldBe(75);
            }
        }

        [Fact]
        public void NewTextFromStringTest()
        {
            const string text = "this is a test";
            Image image;
            using (var db = CMSImageDataContext.Create(Util.Host))
            {
                image = Image.NewTextFromString(text, db);
            }

            image.Bits.ShouldBe(Encoding.ASCII.GetBytes(text));
            image.Mimetype.ShouldBe(mimeTypeText);
            image.Id.ShouldNotBe(0);
        }

        [Fact]
        public void SetTextTest()
        {
            const string text = "this is a test";
            Image image = new Image();
            image.SetText(text);

            image.Bits.ShouldBe(Encoding.ASCII.GetBytes(text));
            image.Mimetype.ShouldBe(mimeTypeText);
            image.Id.ShouldBe(0);
        }
    }
}
