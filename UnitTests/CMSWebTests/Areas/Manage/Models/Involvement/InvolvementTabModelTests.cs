using System.Collections.Generic;
using CmsWeb.Areas.Manage.Models.Involvement;
using Shouldly;
using Xunit;

namespace CMSWebTests.Areas.Manage.Models.Involvement
{
    public class InvolvementTabModelTests
    {
        [Fact]
        public void ShouldBeAbleToUpdateCustomizeInvolvementTabXmlViaPost()
        {
            // requires the existence of a parameter-less constructor for ajax post and model binding.
            var involvementTabModel = new InvolvementTabModel();
            involvementTabModel.ShouldNotBeNull();
        }

        [Fact]
        public void ShouldBeAbleToReadXml()
        {
            var involvementTypes = InvolvementTypes();
            var model = new CustomizeInvolvementModel(involvementTypes);
            model.Current.ReadXml(InvolvementTableCurrentXml());

            var currentTab = model.Current;
            currentTab.Name.ShouldBe("InvolvementTableCurrent");
            currentTab.Xml.ShouldBe(InvolvementTableCurrentXml());
            currentTab.Types.Count.ShouldBe(5);

            var leaderCoachGroup = currentTab.Types[1];
            leaderCoachGroup.Columns.Count.ShouldBe(6);
            leaderCoachGroup.Name.ShouldBe("Leader Coach Group");
            leaderCoachGroup.Columns[0].Field.ShouldBe("Organization");
            leaderCoachGroup.Columns[0].Label.ShouldBe(null);
            leaderCoachGroup.Columns[0].Role.ShouldBe(null);
            leaderCoachGroup.Columns[0].Sortable.ShouldBe(true);
            leaderCoachGroup.Columns[1].Field.ShouldBe("Leader");
            leaderCoachGroup.Columns[1].Label.ShouldBe(null);
            leaderCoachGroup.Columns[1].Role.ShouldBe(null);
            leaderCoachGroup.Columns[1].Sortable.ShouldBe(false);
            leaderCoachGroup.Columns[2].Field.ShouldBe("Enroll Date");
            leaderCoachGroup.Columns[2].Label.ShouldBe(null);
            leaderCoachGroup.Columns[2].Role.ShouldBe(null);
            leaderCoachGroup.Columns[2].Sortable.ShouldBe(true);
            leaderCoachGroup.Columns[3].Field.ShouldBe("MemberType");
            leaderCoachGroup.Columns[3].Label.ShouldBe(null);
            leaderCoachGroup.Columns[3].Role.ShouldBe(null);
            leaderCoachGroup.Columns[3].Sortable.ShouldBe(false);
            leaderCoachGroup.Columns[4].Field.ShouldBe("Location");
            leaderCoachGroup.Columns[4].Label.ShouldBe(null);
            leaderCoachGroup.Columns[4].Role.ShouldBe("CG-LC");
            leaderCoachGroup.Columns[4].Sortable.ShouldBe(false);
            leaderCoachGroup.Columns[5].Field.ShouldBe("Health");
            leaderCoachGroup.Columns[5].Label.ShouldBe(null);
            leaderCoachGroup.Columns[5].Role.ShouldBe("CG-LC");
            leaderCoachGroup.Columns[5].Sortable.ShouldBe(false);
        }

        [Fact]
        public void ShouldBeAbleToBuildXml()
        {
            var involvementTypes = InvolvementTypes();
            var model = new CustomizeInvolvementModel(involvementTypes);
            model.Current.ReadXml(InvolvementTableCurrentXml());

            var xml = model.Current.BuildXml();
            xml.ShouldBe(InvolvementTableCurrentXml());
        }

        private List<InvolvementTabModel.OrgType> InvolvementTypes()
        {
            return new List<InvolvementTabModel.OrgType>
            {
                new InvolvementTabModel.OrgType
                {
                    Id = 2,
                    Description = "Registration"
                },
                new InvolvementTabModel.OrgType
                {
                    Id = 3,
                    Description = "Mailing List"
                },
                new InvolvementTabModel.OrgType
                {
                    Id = 4,
                    Description = "Community Group"
                },
                new InvolvementTabModel.OrgType
                {
                    Id = 5,
                    Description = "Faith & Work Group"
                },
                new InvolvementTabModel.OrgType
                {
                    Id = 7,
                    Description = "Children and Youth Group"
                },
                new InvolvementTabModel.OrgType
                {
                    Id = 8,
                    Description = "Volunteer Group"
                },
                new InvolvementTabModel.OrgType
                {
                    Id = 9,
                    Description = "Staff Use Only"
                },
                new InvolvementTabModel.OrgType
                {
                    Id = 10,
                    Description = "Leader Coach Group"
                },
                new InvolvementTabModel.OrgType
                {
                    Id = 11,
                    Description = "Pledge Fund"
                },
                new InvolvementTabModel.OrgType
                {
                    Id = 12,
                    Description = "Giving Fund"
                },
                new InvolvementTabModel.OrgType
                {
                    Id = 13,
                    Description = "Mission Trip"
                },
            };
        }

        private string InvolvementTableCurrentXml()
        {
            return "<InvolvementTable>\r\n  <Columns orgtype=\"Mailing List\">\r\n    <Column field=\"Organization\" sortable=\"true\" />\r\n    <Column field=\"Enroll Date\" sortable=\"true\" />\r\n    <Column field=\"Leave\" label=\"Unsubscribe\" />\r\n  </Columns>\r\n  <Columns orgtype=\"Leader Coach Group\">\r\n    <Column field=\"Organization\" sortable=\"true\" />\r\n    <Column field=\"Leader\" />\r\n    <Column field=\"Enroll Date\" sortable=\"true\" />\r\n    <Column field=\"MemberType\" />\r\n    <Column field=\"Location\" role=\"CG-LC\" />\r\n    <Column field=\"Health\" role=\"CG-LC\" />\r\n  </Columns>\r\n  <Columns orgtype=\"Community Groups\">\r\n    <Column field=\"Organization\" sortable=\"true\" />\r\n    <Column field=\"Leader\" />\r\n    <Column field=\"Location\" sortable=\"true\" />\r\n    <Column field=\"MemberType\" />\r\n    <Column field=\"Schedule\" />\r\n    <Column field=\"Health\" />\r\n  </Columns>\r\n  <Columns orgtype=\"Registration\">\r\n    <Column field=\"Organization\" sortable=\"true\" />\r\n    <Column field=\"Enroll Date\" sortable=\"true\" />\r\n    <Column field=\"Leader\" />\r\n  </Columns>\r\n  <Columns orgtype=\"Volunteer\">\r\n    <Column field=\"Organization\" sortable=\"true\" />\r\n    <Column field=\"Leader\" />\r\n    <Column field=\"Enroll Date\" sortable=\"true\" />\r\n    <Column field=\"MemberType\" />\r\n    <Column field=\"ViewCalendar\" label=\"View Calendar\" />\r\n  </Columns>\r\n</InvolvementTable>";
        }
    }
}
