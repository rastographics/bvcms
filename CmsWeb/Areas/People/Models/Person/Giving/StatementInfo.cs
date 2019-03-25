using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CmsWeb.Areas.People.Models
{
    public class StatementInfo
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public int Count { get; set; }
    }

    public class StatementInfoWithFund : StatementInfo
    {
        public int FundId { get; set; }
        public string FundName { get; set; }
        public string FundGroupName { get; set; }
    }

    public class CustomFundSetDisplayHelper
    {
        private readonly CMSDataContext _dbContext;

        public CustomFundSetDisplayHelper(CMSDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ProcessList(IEnumerable<StatementInfoWithFund> statementInfos)
        {
            var standardFundName = _dbContext.Setting("StandardFundSetName", "Standard Statements");
            var document = XDocument.Parse(_dbContext.Content("CustomStatements", "<CustomStatement/>"));

            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("empty", "http://demo.com/2011/demo-schema");

            foreach (var node in document.XPathSelectElements("/CustomStatements/Statement", namespaceManager))
            {
                var description = node.Attribute("description").Value;
                var funds = node.Element("Funds").Value.Replace("\n", "").Trim();

                var fundsArray = funds.Split('-');

                int fundStart = 0;
                int fundEnd = 0;

                if (fundsArray.Length > 1)
                {
                    fundStart = int.Parse(fundsArray[0]);
                    fundEnd = int.Parse(fundsArray[1]);
                }
                else
                {
                    fundStart = int.Parse(fundsArray[0]);
                    fundEnd = fundStart + 1;
                }

                foreach (var item in statementInfos.Where(x => x.FundId >= fundStart && x.FundId < fundEnd))
                {
                    item.FundGroupName = description;
                }
            }

            foreach (var item in statementInfos.Where(x => x.FundGroupName == string.Empty))
            {
                item.FundGroupName = standardFundName;
            }
        }
    }
}
