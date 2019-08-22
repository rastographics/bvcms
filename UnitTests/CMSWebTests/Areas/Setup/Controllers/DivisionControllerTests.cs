using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityExtensions;
using Xunit;
using Shouldly;

namespace CMSWebTests.Areas.Setup.Controllers
{
    [Collection("Database collection")]
    public class DivisionControllerTests
    {
        [Theory]
        [InlineData ("1")]
        [InlineData ("yes")]
        public void Should_Change_No_Zero_field(string value)
        {
            DbUtil.Db = CMSDataContext.Create(Util.Host);

            var controller = new CmsWeb.Areas.Setup.Controllers.DivisionController(FakeRequestManager.FakeRequest());
            var routeDataValues = new Dictionary<string, string> { { "controller", "Division" } };
            controller.ControllerContext = ControllerTestUtils.FakeContextController(controller, routeDataValues);
            
            Program prog = new Program()
            {
                Name = "MockProgram",
                RptGroup = null,
                StartHoursOffset = null,
                EndHoursOffset = null
            };
            DbUtil.Db.Programs.InsertOnSubmit(prog);
            DbUtil.Db.SubmitChanges();

            Division div = new Division()
            {
                Name = "MockDivision",
                ProgId = prog.Id,
                SortOrder = null,
                EmailMessage = null,
                EmailSubject = null,
                Instructions = null,
                Terms = null,
                ReportLine = null,
                NoDisplayZero = false
            };
            DbUtil.Db.Divisions.InsertOnSubmit(div);
            DbUtil.Db.SubmitChanges();

            controller.Edit("z" + div.Id, value);

            bool? result = DbUtil.Db.Divisions.Where(x => x.Id == div.Id).Select(y => y.NoDisplayZero).First();
            result.ShouldBe(true);

            DbUtil.Db.ExecuteCommand("DELETE FROM [ProgDiv] WHERE [ProgId] = {0} AND [DivId] = {1}", prog.Id, div.Id);
            DbUtil.Db.ExecuteCommand("DELETE FROM [Division] WHERE [Id] = {0}", div.Id);
            DbUtil.Db.ExecuteCommand("DELETE FROM [Program] WHERE [Id] = {0}", prog.Id);
        }
    }
}
