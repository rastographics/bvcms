using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Main.Models;
using NSubstitute;
using NSubstitute.Core.Arguments;
using SharedTestFixtures;
using Shouldly;
using UtilityExtensions;
using Xunit;

namespace CMSWebTests.Areas.Main.Controllers
{
    [Collection(Collections.Database)]
    public class EmailControllerTests
    {
        [Fact]
        public void Should_Use_Email_Draft_Versioning()
        {
            var requestManager = FakeRequestManager.Create();
            var db = requestManager.CurrentDatabase;
            var controller = new CmsWeb.Areas.Main.Controllers.EmailController(requestManager);

            db.SetSetting("UseEmailDraftVersioning", "true");
            var initialDraft = CreateInitialEmailDraft(db);
            var email = CreateTestEmail();
            
            controller.SaveDraft(email, initialDraft.Id, "Test", 1);

            var versionedDraft = db.Contents.FirstOrDefault(c => c.ArchivedFromId == initialDraft.Id);
            versionedDraft.ShouldNotBeNull();
            versionedDraft.Name.ShouldBe($"{initialDraft.Name}.v1");
        }

        [Fact]
        public void Should_Not_Use_Email_Draft_Versioning()
        {
            var requestManager = FakeRequestManager.Create();
            var db = requestManager.CurrentDatabase;
            var controller = new CmsWeb.Areas.Main.Controllers.EmailController(requestManager);

            db.SetSetting("UseEmailDraftVersioning", "false");
            var initialDraft = CreateInitialEmailDraft(db);
            var email = CreateTestEmail();

            controller.SaveDraft(email, initialDraft.Id, "Test", 1);

            var versionedDraft = db.Contents.FirstOrDefault(c => c.ArchivedFromId == initialDraft.Id);
            versionedDraft.ShouldBeNull();
        }

        private Content CreateInitialEmailDraft(CMSDataContext db)
        {
            var content = new Content
            {
                Name = "Test Email",
                TypeID = ContentTypeCode.TypeSavedDraft,
                RoleID = 1,
                OwnerID = 1
            };
            db.Contents.InsertOnSubmit(content);
            db.SubmitChanges();

            return content;
        }

        private MassEmailer CreateTestEmail()
        {
            return new MassEmailer
            {
                Subject = "Test Email",
                Body = "This is just a test.",
                UnlayerDesign = string.Empty,
                UseUnlayer = false
            };
        }
    }
}
