using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using CmsData;
using System.Web.Mvc;
using System.Text;
using System.Configuration;
using UtilityExtensions;
using System.Data.Linq.SqlClient;

namespace CmsWeb.Areas.Setup.Models
{
    public class PromotionModel
    {
        public IEnumerable<Promotion> Promotions()
        {
            return DbUtil.Db.Promotions.OrderBy(p => p.Sort).ThenBy(p => p.Description);
        }
        public IEnumerable<SelectListItem> Programs()
        {
            var q = from c in DbUtil.Db.Programs
                    orderby c.Name
                    select new 
                    SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name,
                    };
            return q;
        }
        public bool CanPromote(int id)
        {
            var p = DbUtil.Db.Promotions.Single(pr => pr.Id == id);
            var fromdiv = p.FromDivId;
            var todiv = p.ToDivId;
            var q = from om in DbUtil.Db.OrganizationMembers
                    where om.Organization.DivOrgs.Any(d => d.DivId == fromdiv)
                    where (om.Pending ?? false) == false
                    let pc = DbUtil.Db.OrganizationMembers.FirstOrDefault(op =>
                       op.Pending == true
                       && op.PeopleId == om.PeopleId
                       && op.Organization.DivOrgs.Any(dd => dd.DivId == todiv))
                    where pc != null
                    select pc;
            return q.Count() > 0;
        }
        public void Promote(int id)
        {
            var p = DbUtil.Db.Promotions.Single(pr => pr.Id == id);
            var fromdiv = p.FromDivId;
            var todiv = p.ToDivId;
            var q = from om in DbUtil.Db.OrganizationMembers
                    where om.Organization.DivOrgs.Any(d => d.DivId == fromdiv)
                    where (om.Pending ?? false) == false
                    let pc = DbUtil.Db.OrganizationMembers.FirstOrDefault(op =>
                       op.Pending == true
                       && op.PeopleId == om.PeopleId
                       && op.Organization.DivOrgs.Any(dd => dd.DivId == todiv))
                    where pc != null
                    select new { om, pc };
            var list = new Dictionary<int, CmsData.Organization>();
            var qlist = q.ToList();
            foreach (var i in qlist)
            {
                i.om.Drop(DbUtil.Db);
                DbUtil.Db.SubmitChanges();
                i.pc.Pending = false;
                DbUtil.Db.SubmitChanges();
                list[i.pc.OrganizationId] = i.pc.Organization;
            }
            foreach (var o in list.Values)
                if (o.PendingLoc.HasValue())
                {
                    o.Location = o.PendingLoc;
                    o.PendingLoc = null;
                }
            DbUtil.Db.SubmitChanges();
        }
    }
}
