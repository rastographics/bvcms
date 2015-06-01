using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsData.View;
using CmsWeb.Models;

namespace CmsWeb.Areas.Search.Models
{
    public class IncompleteRegistrations : PagedTableModel<RecentIncompleteRegistrations2, RecentIncompleteRegistrations2>
    {
        public int? days { get; set; }
        public string oids { get; set; }

        public IncompleteRegistrations()
        {
            
        }
        public IncompleteRegistrations(OrgSearchModel orgsearch, int? days) : base("Date", "desc", true)
        {
            var q = orgsearch.FetchOrgs();
            oids = string.Join(",", q.OrderBy(mm => mm.OrganizationName).Select(mm => mm.OrganizationId));
            this.days = days;
        }

        public override IQueryable<RecentIncompleteRegistrations2> DefineModelList()
        {
            return DbUtil.Db.RecentIncompleteRegistrations2(oids, days);
        }

        public override IQueryable<RecentIncompleteRegistrations2> DefineModelSort(IQueryable<RecentIncompleteRegistrations2> q)
        {
            if(Direction == "asc")
                switch (SortExpression)
                {
                    case "Date":
                        return from r in q
                               orderby r.Stamp
                               select r;
                    case "Person":
                        return from r in q
                               orderby r.Name
                               select r;
                    case "Organization":
                        return from r in q
                               orderby r.OrgName, r.Name
                               select r;
                }
            else
                switch (SortExpression)
                {
                    case "Date":
                        return from r in q
                               orderby r.Stamp descending 
                               select r;
                    case "Person":
                        return from r in q
                               orderby r.Name descending 
                               select r;
                    case "Organization":
                        return from r in q
                               orderby r.OrgName descending, r.Name
                               select r;
                }
            return q;
        }

        public override IEnumerable<RecentIncompleteRegistrations2> DefineViewList(IQueryable<RecentIncompleteRegistrations2> q)
        {
            return q;
        }
    }
}