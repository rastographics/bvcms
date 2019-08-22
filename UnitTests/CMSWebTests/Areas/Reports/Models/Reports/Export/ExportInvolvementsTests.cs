using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmsWeb.Areas.People.Models;
using Shouldly;
using Xunit;
using CmsWeb.Models;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;
using CmsData;
using CmsData.Codes;
using OfficeOpenXml;
using System.Reflection;
using CmsWeb;

namespace CMSWebTests.Areas.Reports.Models.Reports.Export
{
    [Collection("Database collection")]
    public class ExportInvolvementsTests
    {
        [Fact]
        public void InvolvementList_Should_Have_FamilyId()
        {
            var db = CMSDataContext.Create(Util.Host);
            var controller = new CmsWeb.Areas.OnlineReg.Controllers.OnlineRegController(FakeRequestManager.FakeRequest());
            var routeDataValues = new Dictionary<string, string> { { "controller", "OnlineReg" } };
            controller.ControllerContext = ControllerTestUtils.FakeContextController(controller, routeDataValues);

            var m = OrganizationModel.Create(db, FakeRequestManager.FakeRequest().CurrentUser);
            var FakeOrg = FakeOrganizationUtils.MakeFakeOrganization();
            var model = FakeOrganizationUtils.GetFakeOnlineRegModel(FakeOrg.org.OrganizationId);

            m.OrgId = FakeOrg.org.OrganizationId;          

            var resultSubmitQuestions = controller.SubmitQuestions(0, model);
            var resultCompleteRegistration = controller.CompleteRegistration(model);

            var TestInvolvementList = ExportInvolvements.InvolvementList(m.QueryId);
            var pkg = typeof(EpplusResult).GetField("pkg", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(TestInvolvementList);

            using (ExcelPackage package = (ExcelPackage)pkg)
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                object[,] cellValues = (object[,])worksheet.Cells.Value;
                List<string> ReportColumns = cellValues.Cast<object>().ToList().ConvertAll(x => Convert.ToString(x));
                var FamilyId = worksheet.Cells[1, 2].Value.ToString().Trim();

                ReportColumns.ShouldContain("FamilyId");
                FamilyId.ShouldBe("FamilyId");
            }
            FakeOrganizationUtils.DeleteOrg(FakeOrg.org.OrganizationId);
        }
    }
}
