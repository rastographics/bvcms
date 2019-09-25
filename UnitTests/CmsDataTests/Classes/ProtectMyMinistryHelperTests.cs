using CmsData;
using CmsData.Classes.ProtectMyMinistry;
using CmsDataTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Linq;
using UtilityExtensions;
using Xunit;

namespace CmsDataTests
{
    [Collection(Collections.Database)]
    public class ProtectMyMinistryHelperTests
    {
        [Fact]
        public void Should_Create_BackgroundChecks()
        {
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var bgChLabel = CreateLabel(db);               

                var bgCheck = ProtectMyMinistryHelper.Create(db, 1, 1, "Combo", 1, bgChLabel.Id);
                var result = db.BackgroundChecks.FirstOrDefault(b => b.Id == bgCheck.Id);

                result.PeopleID.ShouldBe(1);
                result.UserID.ShouldBe(1);
                result.ServiceCode.ShouldBe("Combo");
                result.ReportTypeID.ShouldBe(1);
                result.ReportLabelID.ShouldBe(bgChLabel.Id);

                db.BackgroundChecks.DeleteOnSubmit(result);
                db.BackgroundCheckLabels.DeleteOnSubmit(bgChLabel);
                db.SubmitChanges();
            }
        }

        private BackgroundCheckLabel CreateLabel(CMSDataContext db)
        {
            var bgChLabel = new BackgroundCheckLabel
            {
                Id = 10,
                Code = "ABC",
                Description = "Description",
                Hardwired = false
            };
            db.BackgroundCheckLabels.InsertOnSubmit(bgChLabel);
            db.SubmitChanges();
            return bgChLabel;
        }
    }
}
