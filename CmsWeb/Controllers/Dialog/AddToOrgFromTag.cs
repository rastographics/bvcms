using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public partial class DialogController
    {
        public class TagInfo : LongRunningOp
        {
            public TagInfo()
            {
                
            }
            public TagInfo(int id, string group)
            {
                Id = id;
                Group = group;
                Tag = new CodeInfo("0", "Tag");
            }
            public TagInfo(LongRunningOp lop, string group)
            {
                lop.CopyPropertiesTo(this);
                Group = group;
            }
            [DisplayName("Choose A Tag")]
            public CodeInfo Tag { get; set; }
            public string Group { get; set; }
        }
        [Route("~/Dialog/AddToOrgFromTag/{id:int}/{group}")]
        public ActionResult AddToOrgFromTag(int id, string group, TagInfo model)
        {
            const string addtoorgfromtag = "addtoorgfromtag";
            if (Request.HttpMethod == "GET") // initial display
            {
    			var r = DbUtil.Db.LongRunningOps.SingleOrDefault(m => m.Id == id && m.Operation == addtoorgfromtag );
                if (r != null) 
                    DbUtil.Db.LongRunningOps.DeleteOnSubmit(r);
                DbUtil.Db.SubmitChanges();
                return View(new TagInfo(id, group));
            }
            // If we are here, this was a POST
            // Validate first
            if (model.Tag != null && model.Tag.Value == "0")
            {
                // They did not choose a tag, show errror message
                ModelState.AddModelError("Tag", "Must choose a tag");
                return View(new TagInfo(id, group));
            }
            // let them confirm by seeing the count and the tagname
            if (model.Count == null && model.Tag != null)
            {
                var tag = DbUtil.Db.TagById(model.Tag.Value.ToInt());
                var q = tag.People(DbUtil.Db);
                model.Count = q.Count();
                return View(model);
            }
			var rr = DbUtil.Db.LongRunningOps.SingleOrDefault(m => m.Id == id && m.Operation == addtoorgfromtag );
            if (rr == null && model.Tag != null) 
            {
                // Do the deed here
    			DbUtil.LogActivity("Add to org from tag for {0}".Fmt(Session["ActiveOrganization"]));
    		    var qid = DbUtil.Db.FetchLastQuery().Id;
				var q = model.Tag.Value == "-1"
                    ? DbUtil.Db.PeopleQuery(qid).Select(pp => pp.PeopleId)
                    : from t in DbUtil.Db.TagPeople
					  where t.Id == model.Tag.Value.ToInt()
					  select t.PeopleId;
				var pids = q.ToList();
    			var runningtotals = new LongRunningOp()
    			{
    				Started = DateTime.Now,
    				Count = pids.Count,
    				Processed = 0,
    				Id = id,
                    Operation = addtoorgfromtag,
    			};
    			DbUtil.Db.LongRunningOps.InsertOnSubmit(runningtotals);
    			DbUtil.Db.SubmitChanges();
    			var host = Util.Host;
    			System.Threading.Tasks.Task.Factory.StartNew(() =>
    			{
    				Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
    				var db = new CMSDataContext(Util.GetConnectionString(host));
    			    var cul = db.Setting("Culture", "en-US");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

    			    LongRunningOp r = null;
    			    foreach (var pid in pids)
    				{
    					db.Dispose();
    					db = new CMSDataContext(Util.GetConnectionString(host));
    				    switch (group)
    				    {
    				        case GroupSelectCode.Member:
            				    OrganizationMember.InsertOrgMembers(db, id, pid, MemberTypeCode.Member, DateTime.Now, null, pending: false);
    				            break;
    				        case GroupSelectCode.Pending:
            				    OrganizationMember.InsertOrgMembers(db, id, pid, MemberTypeCode.Member, DateTime.Now, null, pending: true);
    				            break;
    				        case GroupSelectCode.Prospect:
            				    OrganizationMember.InsertOrgMembers(db, id, pid, MemberTypeCode.Prospect, DateTime.Now, null, pending: false);
    				            break;
    				        case GroupSelectCode.Inactive:
            				    OrganizationMember.InsertOrgMembers(db, id, pid, MemberTypeCode.InActive, DateTime.Now, DateTime.Now, pending: false);
    				            break;
    				        case GroupSelectCode.Previous:
            				    Organization.AddAsPreviousMember(db, id, pid, MemberTypeCode.InActive, DateTime.Now, DateTime.Now);
    				            break;
    				    }
    				    r = db.LongRunningOps.SingleOrDefault(m => m.Id == id && m.Operation == addtoorgfromtag);
    				    Debug.Assert(r != null, "r != null");
    				    r.Processed++;
    			        db.SubmitChanges();
                        Thread.Sleep(1000);
    				}
			        r = db.LongRunningOps.Single(m => m.Id == id && m.Operation == addtoorgfromtag);
    				r.Completed = DateTime.Now;
    	            db.SubmitChanges();
    			});
            }
            // Display Progress here
            rr = DbUtil.Db.LongRunningOps.SingleOrDefault(m => m.Id == id && m.Operation == addtoorgfromtag);
			return View(new TagInfo(rr, group));
		}
    }
}
