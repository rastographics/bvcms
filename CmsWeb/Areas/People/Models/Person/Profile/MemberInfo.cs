using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CmsData;
using System.Data.Linq;
using CmsWeb.Code;

namespace CmsWeb.Areas.People.Models
{
    public class MemberInfo
    {
        public Person person;
        private readonly CMSDataContext Db;

        [NoUpdate]
        public int PeopleId { get; set; }

        // Contributions --------------------------------------------------

        [DisplayName("Contribution Statement")]
        public CodeInfo ContributionOptions { get; set; }

        [DisplayName("Envelope Option")]
        public CodeInfo EnvelopeOptions { get; set; }

        // Decision --------------------------------------------------

        [DisplayName("Type")]
        public CodeInfo DecisionType { get; set; }

        [DisplayName("Date")]
        public DateTime? DecisionDate { get; set; }

        // Baptism --------------------------------------------------

        [DisplayName("Status")]
        public CodeInfo BaptismStatus { get; set; }

        [DisplayName("Type")]
        public CodeInfo BaptismType { get; set; }

        [DisplayName("Date")]
        public DateTime? BaptismDate { get; set; }

        [DisplayName("Scheduled")]
        public DateTime? BaptismSchedDate { get; set; }

        // Drop --------------------------------------------------

        [DisplayName("Type"), FieldInfo(IdField = "DropCodeId")]
        public CodeInfo DropType { get; set; }

        [DisplayName("Date")]
        public DateTime? DropDate { get; set; }

        [DisplayName("New Church"), StringLength(60)]
        public string OtherNewChurch { get; set; }

        // New Member Class --------------------------------------------------

        [DisplayName("Status")]
        public CodeInfo NewMemberClassStatus { get; set; }

        [DisplayName("Date")]
        public DateTime? NewMemberClassDate { get; set; }

        // Membership --------------------------------------------------

        [TrackChanges]
        public CodeInfo MemberStatus { get; set; }

        [DisplayName("How Joined"), FieldInfo(IdField = "JoinCodeId")]
        public CodeInfo JoinType { get; set; }

        public DateTime? JoinDate { get; set; }

        [DisplayName("Prev Church"), StringLength(60)]
        public string OtherPreviousChurch { get; set; }

        public MemberInfo()
        {
            Db = DbUtil.Db;
        }
        public MemberInfo(int id)
            : this()
        {
            person = Db.LoadPersonById(id);
            if (person == null)
                return;
            this.CopyPropertiesFrom(person);
        }

        public string UpdateMember()
        {
            var i = (from p in DbUtil.Db.People
                     where p.PeopleId == PeopleId
                     select new
                     {
                         p,
                         p.Family
                     }).Single();

            var changes = this.CopyPropertiesTo(i.p, excludefields: "HomePhone");
            i.p.LogChanges(DbUtil.Db, changes);


            var ret = i.p.MemberProfileAutomation(DbUtil.Db);
            if (ret == "ok")
            {
                DbUtil.Db.SubmitChanges();
                changes = this.CopyPropertiesTo(i.Family, onlyfields: "HomePhone");
                i.Family.LogChanges(DbUtil.Db, changes, i.p.PeopleId);
                person = i.p;
                DbUtil.Db.Refresh(RefreshMode.OverwriteCurrentValues, person);
            }
            return ret;
        }

        public List<string[]> StatusFlags()
        {
            var q1 = (from f in DbUtil.Db.StatusFlags()
                      select f).ToList();
            var q2 = (from t in DbUtil.Db.TagPeople
                      where t.PeopleId == PeopleId
                      where t.Tag.TypeId == 100
                      select t.Tag.Name).ToList();
            var q = from t in q2
                    join f in q1 on t equals f[0]
                    select f;
            var list = q.ToList();
            return list;
        }
    }
}
