using CmsData;
using CmsData.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.Report
{
    public class ContributionModel
    {
        public Person person { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public ContributionModel() { }
        public ContributionModel(int pid)
        {
            person = DbUtil.Db.LoadPersonById(pid);
            var intQtr = (Util.Now.Date.Month) / 3 + 1;

            if (intQtr == 1)
            {
                FromDate = new DateTime(Util.Now.Date.Year - 1, 1, 1);
                ToDate = new DateTime(Util.Now.Date.Year - 1, 12, 31);
            }
            else
            {
                FromDate = new DateTime(Util.Now.Date.Year, 1, 1);
                ToDate = (new DateTime(Util.Now.Date.Year, ((intQtr - 1) * 3), 1)).AddMonths(1).AddDays(-1);
            }
        }

        public IEnumerable<YearInfo> FetchYears()
        {
            var q = from c in DbUtil.Db.Contributions
                    where person.PeopleId == c.PeopleId || person == null
                    where c.ContributionTypeId != ContributionTypeCode.Pledge
                    where c.ContributionStatusId == ContributionStatusCode.Recorded
                    where !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                    group c by c.ContributionDate.Value.Year
                    into g
                    orderby g.Key descending
                    select new YearInfo
                    {
                        Year = g.Key,
                        Count = g.Count(),
                        Amount = g.Sum(c => c.ContributionAmount),
                        PeopleId = person == null ? 0 : person.PeopleId,
                    };
            return q;
        }


        public class YearInfo
        {
            public int PeopleId { get; set; }
            public int? Year { get; set; }
            public int? Count { get; set; }
            public decimal? Amount { get; set; }
        }
    }
}


