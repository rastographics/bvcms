using CmsWeb.Areas.Org.Controllers;
using CmsWeb.Areas.Org.Models;
using SharedTestFixtures;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace CMSWebTests.Areas.Org.Controllers
{
    [Collection(Collections.Database)]
    public class TestOrgGroupsModel
    {
        [Fact]
        public void TestOrgGroupsMethods()
        {
            var controller = new OrgGroupsController(FakeRequestManager.Create());
            var routeDataValues = new Dictionary<string, string>();
            controller.ControllerContext = ControllerTestUtils.FakeControllerContext(controller, routeDataValues);
            var db = DatabaseFixture.NewDbContext();
            var groupNames = new []{"Test Group", "Another Name", "Yet Another"};

            var memtags = db.OrgMemMemTags.Where(nn => groupNames.Contains(nn.MemberTag.Name));
            db.OrgMemMemTags.DeleteAllOnSubmit(memtags);

            var groups = db.MemberTags.Where(nn => groupNames.Contains(nn.Name));
            db.MemberTags.DeleteAllOnSubmit(groups);

            var org = db.LoadOrganizationByName("App Testing Org");
            var m = new OrgGroupsModel(db, org.OrganizationId);

            // MakeNewGroup
            m.GroupName = groupNames[0];
            var r = controller.MakeNewGroup(m);
            r.ShouldBeOfType<RedirectResult>();

            var g = db.MemberTags.SingleOrDefault(gg => gg.Name == groupNames[0]);
            g.ShouldNotBeNull();

            // MakeLeaderOfTargetGroup
            const int pid = 2; // David Carroll
            m.List.Add(pid);
            r = controller.MakeLeaderOfTargetGroup(m);
            r.ShouldBeOfType<ViewResult>();
            var mt = db.OrgMemMemTags.SingleOrDefault(gg => gg.OrgId == org.OrganizationId && gg.PeopleId == pid);
            mt.ShouldNotBeNull();

            // RemoveAsLeaderOfTargetGroup
            db = DatabaseFixture.NewDbContext();
            r = controller.RemoveAsLeaderOfTargetGroup(m);
            r.ShouldBeOfType<ViewResult>();
            mt = db.OrgMemMemTags.SingleOrDefault(gg => gg.OrgId == org.OrganizationId && gg.PeopleId == pid);
            Assert.NotNull(mt);
            mt.IsLeader.ShouldBe(false);

            // RemoveSelectedFromTargetGroup
            r = controller.RemoveSelectedFromTargetGroup(m);
            r.ShouldBeOfType<ViewResult>();
            mt = db.OrgMemMemTags.SingleOrDefault(gg => gg.OrgId == org.OrganizationId && gg.PeopleId == pid);
            mt.ShouldBeNull();

            // RenameGroup
            m.GroupName = groupNames[1];
            r = controller.RenameGroup(m);
            r.ShouldBeOfType<RedirectResult>();
            g = db.MemberTags.SingleOrDefault(gg => gg.OrgId == org.OrganizationId && gg.Name == groupNames[1]);
            Assert.NotNull(g);
            g.Name.ShouldBe(groupNames[1]);

            // EditGroup
            m.GroupName = groupNames[2];
            m.CheckInCapacityDefault = 10;
            r = controller.EditGroup(m);
            r.ShouldBeOfType<RedirectResult>();
            db = DatabaseFixture.NewDbContext();
            g = db.MemberTags.SingleOrDefault(gg => gg.OrgId == org.OrganizationId && gg.Name == groupNames[2]);
            Assert.NotNull(g);
            g.Name.ShouldBe(groupNames[2]);
            g.CheckInCapacityDefault.ShouldBe(10);

            // DeleteGroup
            m.GroupName = groupNames[2];
            r = controller.DeleteGroup(m);
            r.ShouldBeOfType<RedirectResult>();
            g = db.MemberTags.SingleOrDefault(gg => gg.OrgId == org.OrganizationId && gg.Name == groupNames[2]);
            g.ShouldBeNull();

            // todo: to complete the remaining methods
            // DeleteGroups
            // UpdateScore
            // UploadScores
            // SwapPlayers
            // CreateTeams
            // AssignSelectedToTargetGroup
            // ToggleCheckin
        }
    }
}
