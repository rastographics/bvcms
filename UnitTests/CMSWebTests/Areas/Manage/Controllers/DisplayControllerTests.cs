using System.Text.RegularExpressions;
using System.Web.Mvc;
using Newtonsoft.Json;
using SharedTestFixtures;
using Shouldly;
using UtilityExtensions;
using Xunit;

namespace CMSWebTests.Areas.Manage.Controllers
{
    [Collection(Collections.Database)]
    public class DisplayControllerTests
    {
        [Fact]
        public void SaveUnlayerTemplateCopy()
        {
            var requestManager = FakeRequestManager.Create();
            var db = requestManager.CurrentDatabase;
            var controller = new CmsWeb.Areas.Manage.Controllers.DisplayController(requestManager);
            var content = db.ContentFromID(94);
            dynamic payload = JsonConvert.DeserializeObject(content.Body);
            string design = payload.design;
            string body = payload.rawHtml;
            var result = controller.SaveUnlayerTemplateCopy(content.Id, content.Name, content.RoleID, content.Title,
                body, design) as RedirectResult;
            result.ShouldNotBe(null);
            var id = Regex.Match(result.Url, @"/Display/ContentEdit/(\d*)").Groups[1].Value.ToInt();
            var copy = db.ContentFromID(id);
            copy.Name.ShouldBe(content.Name + " Copy");
            db.ExecuteCommand("delete dbo.Content where Name like '% Copy'");
        }
    }
}
