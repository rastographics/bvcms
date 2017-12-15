using CmsWeb.Areas.People.Models;
using Shouldly;
using Xunit;

namespace UnitTests.Areas.People.Models.Contact
{
    public class ContactExtraLocationConfigTests
    {
        [Theory]
        [InlineData(null, "redeemerwide", "cggroupvisit", "cghealthassessment", "redeemerwide-cggroupvisit-cghealthassessment")]
        [InlineData(123, "redeemerwide", "cggroupvisit", "cghealthassessment", "redeemerwide-cggroupvisit-cghealthassessment")]
        [InlineData(123, "what", "what", "what", "OrganizationStandard")]
        [InlineData(null, "what", "what", "what", "PersonStandard")]
        [InlineData(123, "wscommunitygroups", "what", "what", "wscommunitygroups--")]
        [InlineData(123, "Not Match, but shows off partial matches", "This Is A Contact Type", "Test Reason", "-thisisacontacttype-testreason")]
        public void Should_be_able_to_get_location(int? orgId, string ministry, string contactReason, string contactType, string expectedResult)
        {
            var config = new ContactExtraLocationConfig(ValidXmlContent());
            var location = config.GetLocationFor(orgId, ministry, contactReason, contactType);

            location.ShouldBe(expectedResult);
        }

        private static string ValidXmlContent()
        {
            return @"
<ContactExtraLocations>
    <Location>
        <Ministry name=""redeemerwide"" />
        <ContactType name=""cggroupvisit"" />
        <ContactReason name=""cghealthassessment"" />
    </Location>
    <Location>
        <Ministry name=""dtcommunitygroups"" />
        <ContactType name=""cgvisit"" />
        <ContactReason name=""cg"" />
    </Location>
    <Location>
        <Ministry name=""wsgeneral"" />
        <ContactType name=""cgvisit"" />
        <ContactReason name=""cg"" />
    </Location>
    <Location>
        <Ministry name=""dtgeneral"" />
        <ContactType name=""phonecallmade"" />
        <ContactReason name=""membership"" />
    </Location>
    <Location>
        <Ministry name=""redeemerwide"" />
        <ContactType name=""lettermailed"" />
        <ContactReason name=""birthday"" />
    </Location>
    <Location>
        <Ministry name=""notspecified"" />
        <ContactType name=""emailsent"" />
        <ContactReason name=""notspecified"" />
    </Location>
    <Location>
        <Ministry name=""bvcmssupport"" />
        <ContactType name=""cgvisit"" />
        <ContactReason name=""cg"" />
    </Location>
    <Location>
        <Ministry name=""cfw"" />
        <ContactType name=""lettermailed"" />
        <ContactReason name=""personal"" />
    </Location>
    <Location>
        <Ministry name=""dtsst"" />
        <ContactType name=""emailsent"" />
        <ContactReason name=""baby"" />
    </Location>
    <Location>
        <Ministry name=""cfw"" />
        <ContactType name=""cgvisit"" />
        <ContactReason name=""baby"" />
    </Location>
    <Location>
        <Ministry name=""cfw"" />
        <ContactType name=""lettermailed"" />
        <ContactReason name=""bereavement"" />
    </Location>
    <Location>
        <Ministry name=""escommunitygroups"" />
        <ContactType name=""cggroupvisit"" />
        <ContactReason name=""cghealthassessment"" />
    </Location>
    <Location>
        <Ministry name=""wscommunitygroups"" />
        <ContactType name=""cggroupvisit"" />
        <ContactReason name=""cghealthassessment"" />
    </Location>
    <Location>
        <Ministry name=""wscommunitygroups"" />
    </Location>
    <Location>
        <ContactType name=""This Is A Contact Type"" />
        <ContactReason name=""Test Reason"" />
    </Location>
</ContactExtraLocations>
";
        }
    }
}
