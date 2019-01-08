using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Models
{
    public class PromotionModel
    {
        public PromotionModel() { }
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
            var q = from om in DbUtil.Db.OrganizationMembers
                    where om.Organization.DivOrgs.Any(d => d.DivId == fromdiv)
                    where (om.Pending ?? false) == false
                    let pcid = om.OrgMemberExtras.Where(vv => vv.Field == "PromotingTo").Select(vv => vv.IntValue).SingleOrDefault()
                    let pc = DbUtil.Db.OrganizationMembers.FirstOrDefault(op =>
                       op.Pending == true
                       && op.PeopleId == om.PeopleId
                       && op.OrganizationId == pcid)
                    where pc != null
                    select pc;
            return q.Any();
        }
        public void Promote(int id)
        {
            var p = DbUtil.Db.Promotions.Single(pr => pr.Id == id);
            var fromdiv = p.FromDivId;
            var q = from om in DbUtil.Db.OrganizationMembers
                    where om.Organization.DivOrgs.Any(d => d.DivId == fromdiv)
                    where (om.Pending ?? false) == false
                    let pcid = om.OrgMemberExtras.Where(vv => vv.Field == "PromotingTo").Select(vv => vv.IntValue).SingleOrDefault()
                    let pc = DbUtil.Db.OrganizationMembers.FirstOrDefault(op =>
                       op.Pending == true
                       && op.PeopleId == om.PeopleId
                       && op.OrganizationId == pcid)
                    where pc != null
                    where om.OrganizationId != pc.OrganizationId // should not promote to same class
                    select new { om, pc };
            var list = new Dictionary<int, Organization>();
            var qlist = q.ToList();
            foreach (var i in qlist)
            {
                DbUtil.Db.SubmitChanges();
                i.om.Drop(DbUtil.Db);
                DbUtil.Db.SubmitChanges();
                i.pc.Pending = false;
                i.pc.EnrollmentDate = DateTime.Now;
                DbUtil.Db.SubmitChanges();
                list[i.pc.OrganizationId] = i.pc.Organization;
            }
            foreach (var o in list.Values)
            {
                if (o.PendingLoc.HasValue())
                {
                    o.Location = o.PendingLoc;
                    o.PendingLoc = null;
                }
            }

            DbUtil.Db.SubmitChanges();
        }
    }
}
