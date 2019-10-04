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

namespace CMSWebTests.Areas.Reports.Models.Reports.Export
{
    [Collection(Collections.Database)]
    public class ExportInvolvementsTests : DatabaseTestBase
    {
        [Fact]
        public void InvolvementList_Should_Have_FamilyId()
        {
            var requestManager = FakeRequestManager.Create();
            var db = requestManager.CurrentDatabase;
            var controller = new CmsWeb.Areas.OnlineReg.Controllers.OnlineRegController(requestManager);
            var routeDataValues = new Dictionary<string, string> { { "controller", "OnlineReg" } };
            controller.ControllerContext = ControllerTestUtils.FakeControllerContext(controller, routeDataValues);

            var m = OrganizationModel.Create(db, requestManager.CurrentUser);
            var FakeOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager);
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

        [Fact]
        public void ExcelListShouldNotHaveDeceased()
        {
            var requestManager = FakeRequestManager.Create();
            var db = requestManager.CurrentDatabase;

            var family = new Family();
            db.Families.InsertOnSubmit(family);
            db.SubmitChanges();

            var hoh = new Person
            {
                Family = family,
                FirstName = RandomString(),
                LastName = RandomString(),
                EmailAddress = RandomString() + "@example.com",
                MemberStatusId = MemberStatusCode.Member,
                PositionInFamilyId = PositionInFamily.PrimaryAdult
            };
            var child = new Person
            {
                Family = family,
                FirstName = RandomString(),
                LastName = RandomString(),
                EmailAddress = RandomString() + "@example.com",
                DeceasedDate = DateTime.Now,
                MemberStatusId = MemberStatusCode.Member,
                PositionInFamilyId = PositionInFamily.Child
            };

            db.People.InsertOnSubmit(hoh);
            db.People.InsertOnSubmit(child);
            db.SubmitChanges();

            string code = "FamilyId = " + family.FamilyId;
            var query = QueryModel.QueryCode(db, code);
            query.Count().ShouldBeGreaterThan(0);

            var ExcelPics = ExcelExportModel.List(query.QueryId.Value);
            ExcelPics.Where(p => p.Children.HasValue()).Count().ShouldBe(0);

            db.PurgePerson(hoh.PeopleId);
            db.PurgePerson(child.PeopleId);
            db.Families.DeleteOnSubmit(family);
            db.SubmitChanges();
        }
    }
}
