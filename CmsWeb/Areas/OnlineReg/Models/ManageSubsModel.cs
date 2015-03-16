using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Text;
using CmsData.Registration;
using UtilityExtensions;
using System.Web.Mvc;
using System.Xml.Linq;
using CmsData.Codes;

namespace CmsWeb.Models
{
    public class ManageSubsModel
    {
        public int pid { get; set; }
        public int? masterorgid { get; set; }
        private Person _Person;
        public Person person
        {
            get
            {
                if (_Person == null)
                    _Person = DbUtil.Db.LoadPersonById(pid);
                return _Person;
            }
        }
        public string Description()
        {
            return masterorg.OrganizationName;
        }
        private Organization _masterorg;
        public Organization masterorg
        {
            get
            {
                if (_masterorg != null)
                    return _masterorg;
                if (masterorgid.HasValue)
                    _masterorg = DbUtil.Db.LoadOrganizationById(masterorgid.Value);
                return _masterorg;
            }
        }
        public ManageSubsModel()
        {

        }
        public ManageSubsModel(int pid, int id)
        {
            this.pid = pid;
            var org = DbUtil.Db.LoadOrganizationById(id);
            if (org.RegistrationTypeId != RegistrationTypeCode.ManageSubscriptions2)
                throw new Exception("must be a ManageSubscriptions RegistrationType");
            masterorgid = id;
            _masterorg = org;
        }
        public int[] Subscribe { get; set; }
        public class OrgSub
        {
            public int OrgId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public bool Checked { get; set; }
            public string CHECKED
            {
                get
                {
                    return Checked ? "checked=\"checked\"" : "";
                }
            }
        }
        public IEnumerable<OrgSub> FetchSubs()
        {
            return from o in OnlineRegModel.UserSelectClasses(masterorg)
                   select new OrgSub
                   {
                       OrgId = o.OrganizationId,
                       Name = o.OrganizationName,
                       Description = o.Description,
                       Checked = o.OrganizationMembers.Any(om => om.PeopleId == pid)
                   };
        }
        public IEnumerable<OrgSub> OrderSubs(IEnumerable<OrgSub> q)
        {
            if (!masterorgid.HasValue)
                return q;
            var cklist = masterorg.OrgPickList.Split(',').Select(oo => oo.ToInt()).ToList();
            var list = q.ToList();
            var d = new Dictionary<int, int>();
            var n = 0;
            foreach (var i in cklist)
                d.Add(n++, i);
            var qq = from o in list
                     join i in d on o.OrgId equals i.Value into j
                     from i in j
                     orderby i.Key
                     select o;
            return qq;
        }
        private string _summary;
        public string Summary
        {
            get
            {
                if (!_summary.HasValue())
                {
                    var q = from i in FetchSubs()
                            where i.Checked == true
                            select i;

                    var sb = new StringBuilder();
                    foreach (var s in OrderSubs(q))
                        sb.AppendFormat("<p><b>{0}</b><br/>{1}</p>\n",
                            s.Name, s.Description);
                    _summary = Util.PickFirst(sb.ToString(), "<p>no subscriptions</p>");
                }
                return _summary;
            }
        }
        public void UpdateSubscriptions()
        {
            var q = from o in OnlineRegModel.UserSelectClasses(masterorg)
                    let om = o.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == pid)
                    where om != null
                    select om;
            var current = q.ToList();

            if (Subscribe == null)
                Subscribe = new int[] { };

            var drops = from om in current
                        join id in Subscribe on om.OrganizationId equals id into j
                        from id in j.DefaultIfEmpty()
                        where id == 0
                        select om;

            var joins = from id in Subscribe
                        join om in current on id equals om.OrganizationId into j
                        from om in j.DefaultIfEmpty()
                        where om == null
                        select id;

            foreach (var om in drops)
            {
                om.Drop(DbUtil.Db);
                DbUtil.Db.SubmitChanges();
            }
            foreach (var id in joins)
            {
                OrganizationMember.InsertOrgMembers(DbUtil.Db,
                    id, pid, MemberTypeCode.Member, DateTime.Now, null, false);
                DbUtil.Db.SubmitChanges();
                //DbUtil.Db.UpdateMainFellowship(id);
            }
        }
        private Settings setting;
        public Settings Setting
        {
            get
            {
                return setting ?? (setting = new Settings(masterorg.RegSetting, DbUtil.Db, masterorg.OrganizationId));
            }
        }
        public string Instructions
        {
            get
            {
                return @"
<div class=""instructions login"">{0}</div>
<div class=""instructions select"">{1}</div>
<div class=""instructions find"">{2}</div>
<div class=""instructions options"">{3}</div>
<div class=""instructions special"">{4}</div>
<div class=""instructions submit"">{5}</div>
<div class=""instructions sorry"">{6}</div>
".Fmt(Setting.InstructionLogin,
                     Setting.InstructionSelect,
                     Setting.InstructionFind,
                     Setting.InstructionOptions,
                     Setting.InstructionSpecial,
                     Setting.InstructionSubmit,
                     Setting.InstructionSorry
                     );
            }
        }
    }
}
