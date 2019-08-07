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

namespace CMSWebTests.Areas.Reports.Models.Reports.Export
{
    [Collection("Database collection")]
    public class ExportInvolvementsTests
    {
        private int OrgId { get; set; }

        [Fact]
        public void InvolvementList_Should_Have_FamilyId()
        {
            var db = CMSDataContext.Create(Util.Host);
            var m = OrganizationModel.Create(db, FakeRequestManager.FakeRequest().CurrentUser);
            var FakeOrg = FakeOrganizationUtils.MakeFakeOrganization();
            
            m.OrgId = FakeOrg.org.OrganizationId;

            var TestInvolvementList = ExportInvolvements.InvolvementList(m.QueryId);
            TestInvolvementList.ShouldNotBeNull();
        }

        [Fact]
        public void ShouldDeleteReg()
        {
            FakeOrganizationUtils.DeleteOrg(OrgId);
            var db = CMSDataContext.Create(Util.Host);
            var CurrentOrg = db.LoadOrganizationById(OrgId);
            CurrentOrg.ShouldBe(null);
        }
    }
}
