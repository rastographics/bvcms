using Xunit;
using CmsWeb.Areas.Finance.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedTestFixtures;
using Shouldly;
using Resources = CMSWebTests.Properties.CMSWebTestsResources;

namespace CmsWeb.Areas.Finance.Models.ReportTests
{
    [Collection(Collections.Database)]
    public class ContributionStatementsExtractTests : DatabaseTestBase
    {
        [Theory]
        [MemberData(nameof(StatementConfig_DefaultValues))]
        public void GetStatementSpecification_Default_Test(
            string customStatements,
            string description,
            int[] funds,
            string template,
            string templateBody,
            string header,
            string notice,
            string templateFooter)
        {
            using (var xdb = db.Copy())
            {
                var contents = new[] { "StatementTemplate", "StatementTemplateBody", "StatementHeader", "StatementNotice", "StatementTemplateFooter", "CustomStatements" };
                xdb.Contents.DeleteAllOnSubmit(xdb.Contents.Where(c => contents.Contains(c.Name)));
                xdb.SubmitChanges();
            }
            db.WriteContentText("CustomStatements", customStatements);

            var actual = ContributionStatementsExtract.GetStatementSpecification(db, description);
            actual.ShouldNotBeNull();
            actual.Description.ShouldBe(description);
            actual.Funds.ShouldBe(funds);
            actual.Template.ShouldBe(template);
            actual.TemplateBody.ShouldBe(templateBody);
            actual.Header.ShouldBe(header);
            actual.Notice.ShouldBe(notice);
            actual.Footer.ShouldBe(templateFooter);
        }

        public static IEnumerable<object[]> StatementConfig_DefaultValues()
        {
            yield return new object[] { null, null, null, null, null, Resources.DefaultHeader, Resources.DefaultNotice, null };
            yield return new object[] { null, null, null, null, null, Resources.DefaultHeader, Resources.DefaultNotice, null };
            yield return new object[] { Resources.StatementSpecification1, "CustomStatement1", new int[] {}, "<template />", "<templatebody />", "<header />", "<notice />", "<footer />" };
            yield return new object[] { Resources.StatementSpecification1, "CustomStatement2", new[] { 1, 2 }, "<template2 />", "<templatebody2 />", "<header2 />", "<notice2 />", "<footer2 />" };
        }

        [Theory]
        [MemberData(nameof(StatementConfig_CustomValues))]
        public void GetHTMLStatementSpecification_Custom_Test(
            string customStatements,
            string description,
            int[] funds,
            string template,
            string templateBody,
            string header,
            string notice,
            string templateFooter)
        {
            using (var xdb = db.Copy())
            {
                var contents = new[] { "CustomTemplate", "CustomTemplateBody", "CustomHeader", "CustomNotice", "CustomTemplateFooter", "CustomStatements" };
                xdb.Contents.DeleteAllOnSubmit(xdb.Contents.Where(c => contents.Contains(c.Name)));
                xdb.SubmitChanges();
            }
            db.WriteContentText("CustomTemplate", template);
            db.WriteContentText("CustomTemplateBody", templateBody);
            db.WriteContentHtml("CustomHeader", header);
            db.WriteContentHtml("CustomNotice", notice);
            db.WriteContentText("CustomStatements", customStatements);
            db.WriteContentText("CustomTemplateFooter", templateFooter);
            db.FetchOrCreateFund(3, "Fund 3");

            var actual = ContributionStatementsExtract.GetStatementSpecification(db, description);
            actual.ShouldNotBeNull();
            actual.Description.ShouldBe(description);
            actual.Funds.ShouldBe(funds);
            actual.Template.ShouldBe(template);
            actual.TemplateBody.ShouldBe(templateBody);
            actual.Header.ShouldBe(header);
            actual.Notice.ShouldBe(notice);
            actual.Footer.ShouldBe(templateFooter);
        }

        public static IEnumerable<object[]> StatementConfig_CustomValues()
        {
            yield return new object[] { Resources.StatementSpecification2, "CustomStatement3", new int[] {}, "<template3 />", "<templatebody3 />", "<header3 />", "<notice3 />", "<footer3 />" };
            yield return new object[] { Resources.StatementSpecification2, "CustomStatement4", new[] { 1, 2, 3 }, "<template4 />", "<templatebody4 />", "<header4 />", "<notice4 />", "<footer4 />" };
        }
    }
}
