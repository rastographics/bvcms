using CmsData;
using CmsData.View;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Code
{
    public class PledgesHelper
    {
        public static List<PledgesSummary> PledgesSummaryByFundList(List<PledgesSummary> pledgesSummary, List<int> fundIdList)
        {   
            return pledgesSummary.Where(p => fundIdList.Contains(p.FundId)).ToList();
        }

        public static List<int> GetFundIdListFromString(string fundsString)
        {
            try
            {
                return fundsString.Split(',').Select(id => int.Parse(id)).ToList();
            }
            catch (Exception e)
            {
                return new List<int>();
            }            
        }

        public static List<PledgesSummary> GetFilteredPledgesSummary(CMSDataContext db, int peopleId)
        {
            var fundString = db.GetSetting("PostContributionPledgeFunds", "");
            return PledgesSummaryByFundList(db.PledgesSummary(peopleId).ToList(), GetFundIdListFromString(fundString));
        }
    }
}
