using Xunit;
using CmsData;
using System.Collections.Generic;
using UtilityExtensions;
using System.Linq;

namespace CMSWebTests.Areas.OnlineReg.Models.AskChurch
{
    [Collection("Database collection")]
    public class NewAskChurchQuestionTest
    {
        private int OrgId { get; set; }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(false, false, false)]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        public void ShouldPassAskChurchQuestions(bool memberus, bool otherchurch, bool nochurch)
        {
            var controller = new CmsWeb.Areas.OnlineReg.Controllers.OnlineRegController(FakeRequestManager.FakeRequest());
            var routeDataValues = new Dictionary<string, string> { { "controller", "OnlineReg" } };
            controller.ControllerContext = ControllerTestUtils.FakeContextController(controller, routeDataValues);

            var FakeOrg = FakeOrganizationUtils.MakeFakeOrganization();
            OrgId = FakeOrg.org.OrganizationId;

            DbUtil.CreateDatabase(Util.Host);
            var IsOrgNull = DbUtil.Db.Organizations.Where(x=> x.OrganizationId == OrgId);
            if (DbUtil.Db.Organizations.Where(x => x.OrganizationId == OrgId).IsNull())
            {
                FakeOrganizationUtils.FakeNewOrganizationModel = null;
                FakeOrg = FakeOrganizationUtils.MakeFakeOrganization();
                OrgId = FakeOrg.org.OrganizationId;
            }

            var model = FakeOrganizationUtils.GetFakeOnlineRegModel(OrgId);

            model.List[0].memberus = memberus;
            model.List[0].otherchurch = otherchurch;
            model.List[0].nochurch = nochurch;
            model.List[0].paydeposit = true;

            var resultSubmitQuestions = controller.SubmitQuestions(0, model);
            var resultCompleteRegistration = controller.CompleteRegistration(model);

            Assert.NotNull(resultSubmitQuestions);
            Assert.NotNull(resultCompleteRegistration);
        }

        ~NewAskChurchQuestionTest()
        {
            Dispose();
        }

        [Fact]
        public void Dispose()
        {
            FakeOrganizationUtils.DeleteOrg(OrgId);
        }
    }
}
