using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class QueryStatsResult : ActionResult
    {
        StringBuilder sb = new StringBuilder();
        public override void ExecuteResult(ControllerContext context)
        {
            var dt = DateTime.Parse("1/1/1900");
            var firstrunid = DateTime.Now.Date.Subtract(dt).Days - 200;
            var q = from s in DbUtil.Db.QueryStats
                where s.RunId > firstrunid
                group s by s.RunId into g
                orderby g.Key descending
                select new
                {
                    g.Key,
                    list = from s in g.OrderBy(ss => ss.StatId)
                        select new { Count = s.Count as int?, s.StatId }
                };
            var rows = q.Count();

            var d = new List<Dictionary<string, string>>();

            var q3 = from s in DbUtil.Db.QueryStats
                where s.RunId > firstrunid
                group s by s.StatId into g
                orderby g.Key
                select new { g.Key, g.OrderByDescending(ss => ss.RunId).First().Description };

            var head = q3.ToDictionary(ss => ss.Key, ss => ss.Description);

            var Response = context.HttpContext.Response;
            foreach (var r in q)
            {
                var row = new Dictionary<string, string>();
                row["S00"] = dt.AddDays(r.Key).ToString("d");
                foreach (var s in r.list)
                    row[s.StatId] = s.Count.ToString2("N0");
                d.Add(row);
            }
            Response.Write("<table cellpadding=4>\n<tr><td>Date</td>");
            foreach (var c in head)
                Response.Write("<td align='right'>{0}</td>".Fmt(c.Value));
            Response.Write("</tr>\n");
            foreach (var r in d)
            {
                Response.Write("<tr><td>{0}</td>".Fmt(r["S00"]));
                foreach (var c in head)
                {
                    if (r.ContainsKey(c.Key))
                        Response.Write("<td align='right'>{0}</td>".Fmt(r[c.Key]));
                    else
                        Response.Write("<td></td>");
                }
                Response.Write("</tr>\n");
            }
            Response.Write("</table>");
        }
    }
}