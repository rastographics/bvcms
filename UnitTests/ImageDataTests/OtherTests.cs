using ImageData;
using ImageDataTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Linq;
using UtilityExtensions;
using Xunit;

namespace ImageDataTests
{
    [Collection(Collections.Database)]
    public class OtherTests : ImageDataTestBase
    {
        [Fact]
        public void InsertTest()
        {
            using (var db = CMSImageDataContext.Create(Util.Host))
            {
                var other = NewOther();
                db.Others.InsertOnSubmit(other);
                db.SubmitChanges();

                db.Others.Where(o => o.Id == other.Id).Any().ShouldBeTrue();
            }
        }

        [Fact]
        public void DeleteTest()
        {
            Other other;
            using (var db = CMSImageDataContext.Create(Util.Host))
            {
                other = NewOther();
                db.Others.InsertOnSubmit(other);
                db.SubmitChanges();
            }

            using (var db = CMSImageDataContext.Create(Util.Host))
            {
                db.Others.DeleteOnSubmit(db.Others.Where(o => o.Id == other.Id).First());
                db.SubmitChanges();

                db.Others.Where(o => o.Id == other.Id).Any().ShouldBeFalse();
            }
        }

        private Other NewOther()
        {
            return new Other
            {
                Created = DateTime.Now,
                UserID = 1,
                First = GetImageBytes("Check1"),
                Second = GetImageBytes("Check2")
            };
        }
    }
}
