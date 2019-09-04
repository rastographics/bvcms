using CmsData;
using CMSWebTests;
using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Areas.Reports.Views.Reports
{
    [Collection(Collections.Webapp)]
    public class ApplicationTests : AccountTestBase
    {
        private int OrgId { get; set; }

        [Fact]
        public void Application_Report_Should_Have_Awnsers()
        {
            var requestManager = FakeRequestManager.Create();

            var Orgconfig = new Organization()
            {
                OrganizationName = "MockName",
                RegistrationTitle = "MockTitle",
                Location = "MockLocation",
                RegistrationTypeId = 1,
                RegSettingXml = XMLSettings()
            };

            var FakeMasterOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager, Orgconfig);
            OrgId = FakeMasterOrg.org.OrganizationId;

            var NewSpecialContent = SpecialContentUtils.CreateSpecialContent(0, "MembershipApp2017", null);
            SpecialContentUtils.UpdateSpecialContent(NewSpecialContent.Id, "MembershipApp2017", "MembershipApp2017", GetValidHtmlConten(), false, null, "", null);

            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin", "Membership" });
            Login();

            Open($"{rootUrl}OnlineReg/{OrgId}");

            var InputField = Find(id: "List0.Text0_0");
            InputField.Clear();
            InputField.SendKeys("ThisTextMustAppearInTests");

            Find(id: "otheredit").Click();

            Open($"{rootUrl}OnlineReg/{OrgId}");
            Open($"{rootUrl}Reports/Application/{OrgId}/{user.PeopleId}/MembershipApp2017");

            Wait(6);

            PageSource.ShouldContain("ThisTextMustAppearInTests");
            SpecialContentUtils.DeleteSpecialContent(NewSpecialContent.Id);
        }

        private string XMLSettings()
        {
            string Settings = @"
                <Settings id=""1"">
                    <Confirmation>
                       <Subject>Confirmation for Redeemer Membership Application</Subject>
                       <Body>
                            &lt;h4&gt;GENERAL INFORMATION&lt;/h4&gt;
                            &lt;div class=""question""&gt;Vow 1 reads: &amp;quot;Do you acknowledge yourself to be a sinner in the sight of God, justly deserving his displeasure and without hope except through his sovereign mercy?&amp;quot;&lt;/div&gt;
                            &lt;div class=""answer""&gt;{regtext:Vow 1 reads: &amp;quot;Do you acknowledge yourself to be a sinner in the sight of God, justly deserving his displeasure and without hope except through his sovereign mercy?&amp;quot;}&lt;/div&gt;
                       </Body>
                    </Confirmation>
                    <Options>
                        <ShellBs>ShellMembershipApp</ShellBs>
                        <AllowOnlyOne>True</AllowOnlyOne>
                        <AllowSaveProgress>True</AllowSaveProgress>
                        <DisallowAnonymous>True</DisallowAnonymous>
                    </Options>
                    <AskItems>
                        <AskText>
                        <Question>Vow 1 reads: ""Do you acknowledge yourself to be a sinner in the sight of God, justly deserving his displeasure and without hope except through his sovereign mercy?""</Question>
                        </AskText>
                    </AskItems>
                </Settings>";

            return Settings;
        }

        private string GetValidHtmlConten()
        {
            string res = "<html>\r\n<head>\r\n\t<title></title>\r\n</head>\r\n<body>\r\n<p><a href=\"javascript:window.print()\"><u>Print this page</u></a></p>\r\n\r\n<address>{name}<br />\r\n{address} {csz}<br />\r\n{email}<br />\r\nH {homephone}<br />\r\nC {cellphone}<br />\r\nDate of Birth: {dob}<br />\r\n<br />\r\nApplication complete date: {extradate:App:Completed}</address>\r\n\r\n<h4>GENERAL INFORMATION</h4>\r\n\r\n<p>Redeemer Congregation</p>\r\n\r\n<p>{smallgroup:[Downtown]}{smallgroup:[East Side]}{smallgroup:[West Side]}{smallgroup:[Lincoln Square]}</p>\r\n\r\n<p>Tell us how you got to Redeemer.</p>\r\n\r\n<p>{regtext:Tell us how you got to Redeemer.}</p>\r\n\r\n<p>&nbsp;</p>\r\n\r\n<p>Are you in a Redeemer Community Group? If yes, who is your Community Group leader?</p>\r\n\r\n<p>{regtext:Redeemer Community Group Leader}</p>\r\n\r\n<p>&nbsp;</p>\r\n\r\n<p>In what other ways have you been involved with Redeemer and/or ministries? (Sunday Services Teams, HFNY, CFW, etc)?</p>\r\n\r\n<p>{regtext:Involvement at Redeemer}</p>\r\n\r\n<p>&nbsp;</p>\r\n\r\n<p>Did you become a Christian through Redeemer or through a Redeemer ministry?</p>\r\n\r\n<p>{smallgroup:[Christian Redeemer]}</p>\r\n\r\n<p>&nbsp;</p>\r\n\r\n<p>If you answered yes to the above question, are you willing to read (or have someone else read) your testimony during a worship service?</p>\r\n\r\n<p>{smallgroup:[Share Testimony]}</p>\r\n\r\n<p>&nbsp;</p>\r\n\r\n<p>Have you ever been baptized?</p>\r\n\r\n<p>{regyesno:Have you ever been baptized?}</p>\r\n\r\n<p>&nbsp;</p>\r\n\r\n<p>What is your relationship status?</p>\r\n\r\n<p>{smallgroup:[Single]}{smallgroup:[Engaged]}{smallgroup:[Married]}{smallgroup:[Separated]}{smallgroup:[Divorced]}{smallgroup:[Widowed]}</p>\r\n\r\n<p>&nbsp;</p>\r\n\r\n<p>If you answered engaged or married above, please tell us to whom and if they are pursuing Redeemer membership. Engaged and married couples will be interviewed together.</p>\r\n\r\n<p>{regtext:If you answered engaged or married above, please tell us to whom and if they are pursuing Redeemer membership. Engaged and married couples will be interviewed together.}</p>\r\n\r\n<h4>GOSPEL UNDERSTANDING</h4>\r\n\r\n<p>If God were to ask you, &quot;Why should I let you into my kingdom?&quot; what reasons would you give?</p>\r\n\r\n<p>{regtext:If God were to ask you, &quot;Why should I let you into my kingdom?&quot; what reasons would you give?}</p>\r\n\r\n<p>&nbsp;</p>\r\n\r\n<p>Please tell us your story of how you came to faith in Jesus Christ.</p>\r\n\r\n<p>{regtext:Please tell us your story of how you came to faith in Jesus Christ.}</p>\r\n\r\n<p>&nbsp;</p>\r\n\r\n<p>Is there anything that we should know about your life that would help us to pastor you better?</p>\r\n\r\n<p>{regtext:Is there anything that we should know about your life that would help us to pastor you better?}</p>\r\n\r\n<p>&nbsp;</p>\r\n\r\n<p>Describe your spiritual disciplines. (Bible study, Sunday worship, etc.)</p>\r\n\r\n<p>{regtext:Describe your spiritual disciplines. (Bible study, Sunday worship, etc.) }</p>\r\n\r\n<h4>FORMER CHURCH</h4>\r\n\r\n<p>Former church name:&nbsp;{regquestion:Former church name:}</p>\r\n\r\n<p>Former church denomination:&nbsp;{regquestion:Former church denomination:}</p>\r\n\r\n<p>Former church address:&nbsp;{regquestion:Former church address:}</p>\r\n\r\n<p>&nbsp;</p>\r\n\r\n<h4>MEMBERSHIP</h4>\r\n\r\n<p>Vow 1 reads: &quot;Do you acknowledge yourself to be a sinner in the sight of God, justly deserving his displeasure and without hope except through his sovereign mercy?&quot;</p>\r\n\r\n<p>{regtext:Vow 1 reads: &quot;Do you acknowledge yourself to be a sinner in the sight of God, justly deserving his displeasure and without hope except through his sovereign mercy?&quot;}</p>\r\n\r\n<p>&nbsp;</p>\r\n\r\n<p>Vow 2 reads: &quot;Do you believe in the Lord Jesus Christ as the Son of God, and Savior of sinners, and do you receive and rest upon him alone for salvation as he is offered in the gospel?&quot;</p>\r\n\r\n<p>{regtext:Vow 2 reads: &quot;Do you believe in the Lord Jesus Christ as the Son of God, and Savior of sinners, and do you receive and rest upon him alone for salvation as he is offered in the gospel?&quot;}</p>\r\n\r\n<p>&nbsp;</p>\r\n\r\n<p>Vow 3 reads: &quot;Do you now resolve and promise, in humble reliance upon the grace of the Holy Spirit, that you will endeavor to live as becomes a follower of Christ?&quot;</p>\r\n\r\n<p>{regtext:Vow 3 reads: &quot;Do you now resolve and promise, in humble reliance upon the grace of the Holy Spirit, that you will endeavor to live as becomes a follower of Christ?&quot;}</p>\r\n\r\n<p>&nbsp;</p>\r\n\r\n<p>Vow 4 reads: &quot;Do you promise to support the Church in its worship and work to the best of your ability?&quot;</p>\r\n\r\n<p>{regtext:Vow 4 reads: &quot;Do you promise to support the Church in its worship and work to the best of your ability?&quot;}</p>\r\n\r\n<p>&nbsp;</p>\r\n\r\n<p>Vow 5 reads: &quot;Do you submit yourself to the government and discipline of the Church, and promise to study its purity and peace?&quot;</p>\r\n\r\n<p>{regtext:Vow 5 reads: &quot;Do you submit yourself to the government and discipline of the Church, and promise to study its purity and peace?&quot;}</p>\r\n\r\n<h4>ADDITIONAL FEEDBACK/QUESTIONS</h4>\r\n\r\n<p>Are there any specific items or concerns you would like to discuss during your membership interview?</p>\r\n\r\n<p>{regtext:Are there any specific items or concerns you would like to discuss during your membership interview?}</p>\r\n</body>\r\n</html>\r\n";
            return res;
        }
    }
}
