using CmsWeb.Areas.Public.Models;
using SharedTestFixtures;
using System;
using SharedTestFixtures;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace CMSWebTests.Areas.Public.Models
{
    [Collection(Collections.Database)]
    public class SmallGroupFinderModelTests : IDisposable
    {
        private int SpecialContentId { get; set; }
        [Fact]
        public void Should_Exclude_In_Small_Group_Filter()
        {
            var SGFxml = SpecialContentUtils.CreateSpecialContent(1, "SGF-MockedXML.xml", null);
            SpecialContentId = SGFxml.Id;
            SpecialContentUtils.UpdateSpecialContent(SpecialContentId, SGFxml.Name, SGFxml.Name, "<?xml version=\"1.0\" encoding=\"utf-8\"?>  <SGF divisionid=\"30\" layout=\"SGF-Layout\" gutter=\"SGF-Gutter\">    <SGFSettings>     <SGFSetting name=\"SubmitText\" value=\"Find Groups\" />     <SGFSetting name=\"ShowHeaders\" value=\"true\" />     <SGFSetting name=\"TextSize\" value=\"18\" />     <SGFSetting name=\"FontFamily\" value=\"Verdana,Arial,Helvetica,sans-serif\" />     <SGFSetting name=\"BGColor\" value=\"#FFFFFF\" />     <SGFSetting name=\"FGColor\" value=\"#000000\" />  </SGFSettings>    <SGFFilters>     <SGFFilter name=\"SGF:Gender\" title=\"Gender\" locked=\"false\" lockedvalue=\"\" />     <SGFFilter name=\"SGF:Childcare\" title=\"Childcare\" locked=\"false\" lockedvalue=\"\" />     <SGFFilter name=\"SGF:Location\" title=\"Location\" locked=\"false\" lockedvalue=\"\" />     <SGFFilter name=\"Campus\" title=\"Church\" locked=\"false\" lockedvalue=\"\" exclude=\"Do Not Attend\" />  </SGFFilters>    </SGF>", null, null, null);

            SettingUtils.UpdateSetting("SGF-OrgTypes", "MockedOrgType, Do Not Attend");

            SmallGroupFinderModel m = new SmallGroupFinderModel();

            m.load("MockedXML");

            var filter = m.getFilterItems(3);
            filter[0].value.ShouldBe("-- All --");
        }

        public void Dispose()
        {
            SpecialContentUtils.DeleteSpecialContent(SpecialContentId);
        }
    }
}
