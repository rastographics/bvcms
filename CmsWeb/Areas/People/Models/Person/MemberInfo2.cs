using System.Collections.Generic;
using System.Linq;
using CmsData;
using UtilityExtensions;
using System.Data.Linq;
using System.Text;
using CmsWeb.Code;

namespace CmsWeb.Areas.People.Models.Person
{
	public class MemberInfo2
	{
        internal CmsData.Person person;
        private readonly CMSDataContext Db;

		public int PeopleId { get; set; }
		public string DecisionDate { get; set; }
		public string JoinDate { get; set; }
		public string BaptismDate { get; set; }
		public string BaptismSchedDate { get; set; }
		public string DropDate { get; set; }
		public string NewChurch { get; set; }
		public string PrevChurch { get; set; }
		public string NewMemberClassDate { get; set; }

        [ZeroToNull]
		public int StatementOptionId { get; set; }
        [ZeroToNull]
		public int EnvelopeOptionId { get; set; }
        [ZeroToNull]
		public int DecisionTypeId { get; set; }
        [ZeroToNull]
		public int JoinTypeId { get; set; }
        [ZeroToNull]
		public int BaptismTypeId { get; set; }
        [ZeroToNull]
		public int BaptismStatusId { get; set; }
        [ZeroToNull]
		public int DropTypeId { get; set; }
        [ZeroToNull]
		public int? NewMemberClassStatusId { get; set; }

        [TrackChanges]
		public int MemberStatusId { get; set; }

        [CodeValue]
	    public string StatementOption { get; set; }
        [CodeValue]
		public string EnvelopeOption { get; set; }
        [CodeValue]
		public string DecisionType { get; set; }
        [CodeValue]
	    public string DropType { get; set; }
        [CodeValue]
		public string BaptismStatus { get; set; }
        [CodeValue]
		public string BaptismType { get; set; }
        [CodeValue]
	    public string NewMemberClassStatus { get; set; }
        [CodeValue]
	    public string MemberStatus { get; set; }
        [CodeValue]
	    public string JoinType { get; set; }

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
	    public MemberInfo2()
	    {
            Db = DbUtil.Db;
	    }
	    public MemberInfo2(int id)
            : this()
	    {
	        Id = id;
            if (person == null)
                return;
            this.CopyPropertiesFrom(person);
	    }

		public string UpdateMember()
		{
			var p = DbUtil.Db.LoadPersonById(PeopleId);
            p.CopyPropertiesFrom(this);

			var psb = new StringBuilder();
			p.UpdateValue(psb, "MemberStatusId", MemberStatusId);
			p.LogChanges(DbUtil.Db, psb, Util.UserPeopleId.Value);

			var ret = p.MemberProfileAutomation(DbUtil.Db);
			if (ret == "ok")
			{
				DbUtil.Db.SubmitChanges();
				DbUtil.LogActivity("Updated Person: {0}".Fmt(p.Name));
			}
			DbUtil.Db.Refresh(RefreshMode.OverwriteCurrentValues, p);
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
