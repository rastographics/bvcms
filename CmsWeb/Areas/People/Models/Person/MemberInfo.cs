using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CmsData;
using UtilityExtensions;
using System.Data.Linq;
using System.Text;
using CmsWeb.Code;

namespace CmsWeb.Areas.People.Models.Person
{
    public class MemberInfo
    {
        public CmsData.Person person;
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

        [UIHint("Date"), DisplayName("Date")]
        public string DecisionDate { get; set; }

        // Baptism --------------------------------------------------

        [DisplayName("Status")]
        public CodeInfo BaptismStatus { get; set; }

        [DisplayName("Type")]
        public CodeInfo BaptismType { get; set; }

        [UIHint("Date"), DisplayName("Date")]
        public string BaptismDate { get; set; }

        [UIHint("Date"), DisplayName("Scheduled")]
        public string BaptismSchedDate { get; set; }

        // Drop --------------------------------------------------

        [DisplayName("Type"), FieldInfo(IdField = "DropCodeId")]
        public CodeInfo DropType { get; set; }

        [UIHint("Date"), DisplayName("Date")]
        public string DropDate { get; set; }

        [UIHint("Text")]
        public string NewChurch { get; set; }

        // New Member Class --------------------------------------------------

        [DisplayName("Status")]
        public CodeInfo NewMemberClassStatus { get; set; }

        [UIHint("Date"), DisplayName("Date")]
        public string NewMemberClassDate { get; set; }

        // Membership --------------------------------------------------

        [TrackChanges]
        public CodeInfo MemberStatus { get; set; }

        [DisplayName("How Joined"), FieldInfo(IdField = "JoinCodeId")]
        public CodeInfo JoinType { get; set; }

        [UIHint("Date")]
        public string JoinDate { get; set; }

        [UIHint("Text")]
        public string PrevChurch { get; set; }

        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                if (_id == 0)
                    return;
                person = Db.LoadPersonById(value);
            }
        }
        public MemberInfo()
        {
            Db = DbUtil.Db;
        }
        public MemberInfo(int id)
            : this()
        {
            Id = id;
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
            i.p.LogChanges(DbUtil.Db, changes, Util.UserPeopleId.Value);

            changes = this.CopyPropertiesTo(i.Family, onlyfields: "HomePhone");
            i.Family.LogChanges(DbUtil.Db, changes, i.p.PeopleId, Util.UserPeopleId.Value);

            var ret = i.p.MemberProfileAutomation(DbUtil.Db);
            if (ret == "ok")
            {
                DbUtil.Db.SubmitChanges();
                DbUtil.LogActivity("Updated Person: {0}".Fmt(i.p.Name));
            }
            DbUtil.Db.Refresh(RefreshMode.OverwriteCurrentValues, i.p);
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
