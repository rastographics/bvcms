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
    }
}
