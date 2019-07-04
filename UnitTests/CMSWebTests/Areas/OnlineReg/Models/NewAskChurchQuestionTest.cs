using Xunit;
using System.Collections;
using System.Net.Http;
using CmsWeb.Areas.OnlineReg.Models;
using System.Collections.Generic;

namespace CMSWebTests.Areas.OnlineReg.Models.AskChurch
{
    public class NewAskChurchQuestionTest
    {
        public static bool BuildDb = true;
        public static bool DropDb = false;
        public static IDictionary Items;
        private const string Url = "https://localhost.tpsdb.com";
        private static readonly HttpClient client = new HttpClient();

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

            var model = GetFakeOnlineRegModel();

            model.List[0].memberus = memberus;
            model.List[0].otherchurch = otherchurch;
            model.List[0].nochurch = nochurch;
            model.List[0].paydeposit = true;

            var resultSubmitQuestions = controller.SubmitQuestions(0, model);
            var resultCompleteRegistration = controller.CompleteRegistration(model);

            Assert.NotNull(resultSubmitQuestions);
            Assert.NotNull(resultCompleteRegistration);        }

        private static OnlineRegModel GetFakeOnlineRegModel()
        {
            var m = new OnlineRegModel(ContexTestUtils.FakeHttpContext().Request, ContexTestUtils.CurrentDatabase(), 106, null, null, null, null);
            m.UserPeopleId = 2;
            return m;
        }
    }
}
