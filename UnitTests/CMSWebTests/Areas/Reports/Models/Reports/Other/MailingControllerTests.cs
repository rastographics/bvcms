using System;
using System.Collections.Generic;
using System.Linq;
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
using SharedTestFixtures;
using CmsWeb.Areas.Search.Models;

namespace CMSWebTests.Areas.Reports.Models.Reports
{
    [Collection(Collections.Database)]
    public class MailingControllerTests : DatabaseTestBase
    {
        [Fact]
        public void FetchExcelCouplesBoth_Should_Show_Correct_Head_Of_House_Id()
        {
            var requestManager = FakeRequestManager.Create();
            var db = requestManager.CurrentDatabase;
            var controller = new CmsWeb.Areas.OnlineReg.Controllers.OnlineRegController(requestManager);
            var routeDataValues = new Dictionary<string, string> { { "controller", "OnlineReg" } };
            controller.ControllerContext = ControllerTestUtils.FakeControllerContext(controller, routeDataValues);

            var m = OrganizationModel.Create(db, requestManager.CurrentUser);
            var FakeOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager);

            var wife = CreateUser(RandomString(), RandomString());
            wife.Person.GenderId = 2;
            wife.Person.MaritalStatusId = MaritalStatusCode.Married;
            db.SubmitChanges();
            var model = FakeOrganizationUtils.GetFakeOnlineRegModel(FakeOrg.org.OrganizationId, wife.UserId);
            m.OrgId = FakeOrg.org.OrganizationId;
            var resultSubmitQuestions = controller.SubmitQuestions(0, model);
            var resultCompleteRegistration = controller.CompleteRegistration(model);

            var husband = CreateUser(RandomString(), RandomString());
            husband.Person.GenderId = 1;
            husband.Person.MaritalStatusId = MaritalStatusCode.Married;
            db.SubmitChanges();
            var model2 = FakeOrganizationUtils.GetFakeOnlineRegModel(FakeOrg.org.OrganizationId, husband.UserId);
            m.OrgId = FakeOrg.org.OrganizationId;
            var resultSubmitQuestions2 = controller.SubmitQuestions(0, model2);
            var resultCompleteRegistration2 = controller.CompleteRegistration(model2);

            var mailingModel = new MailingController();
            var ExcelCouplesBoth = mailingModel.FetchExcelCouplesBoth(m.QueryId, 500);
            var pkg = typeof(EpplusResult).GetField("pkg", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ExcelCouplesBoth);

            //using (ExcelPackage package = (ExcelPackage)pkg)
            //{
            //    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

            //    object[,] cellValues = (object[,])worksheet.Cells.Value;
            //    List<string> ReportColumns = cellValues.Cast<object>().ToList().ConvertAll(x => Convert.ToString(x));
            //    var FamilyId = worksheet.Cells[1, 2].Value.ToString().Trim();

            //    ReportColumns.ShouldContain("FamilyId");
            //    FamilyId.ShouldBe("FamilyId");
            //}
            //FakeOrganizationUtils.DeleteOrg(FakeOrg.org.OrganizationId);
        }
    }
}
