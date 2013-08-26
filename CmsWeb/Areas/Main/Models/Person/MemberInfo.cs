using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Web.Mvc;
using CmsWeb.Code;
using UtilityExtensions;
using System.Data.Linq.SqlClient;
using System.Data.Linq;
using System.Text;

namespace CmsWeb.Models.PersonPage
{
	public class MemberInfo
	{
		private static CodeValueModel cv = new CodeValueModel();
		public int PeopleId { get; set; }

		public int? StatementOptionId { get; set; }

		public string StatementOption
		{
			get { return cv.EnvelopeOptionList().ItemValue(StatementOptionId ?? 0); }
		}

		public int? EnvelopeOptionId { get; set; }

		public string EnvelopeOption
		{
			get { return cv.EnvelopeOptionList().ItemValue(EnvelopeOptionId ?? 0); }
		}

		public int? DecisionTypeId { get; set; }

		public string DecisionType
		{
			get { return cv.DecisionTypeList().ItemValue(DecisionTypeId ?? 0); }
		}

		public DateTime? DecisionDate { get; set; }
		public int JoinTypeId { get; set; }

		public string JoinType
		{
			get { return cv.JoinTypeList().ItemValue(JoinTypeId); }
		}

		public DateTime? JoinDate { get; set; }
		public int? BaptismTypeId { get; set; }

		public string BaptismType
		{
			get { return cv.BaptismTypeList().ItemValue(BaptismTypeId ?? 0); }
		}

		public int? BaptismStatusId { get; set; }

		public string BaptismStatus
		{
			get { return cv.BaptismStatusList().ItemValue(BaptismStatusId ?? 0); }
		}

		public DateTime? BaptismDate { get; set; }
		public DateTime? BaptismSchedDate { get; set; }
		public int DropTypeId { get; set; }

		public string DropType
		{
			get { return cv.DropTypeList().ItemValue(DropTypeId); }
		}

		public DateTime? DropDate { get; set; }
		public string NewChurch { get; set; }
		public string PrevChurch { get; set; }
		public int? NewMemberClassStatusId { get; set; }

		public string NewMemberClassStatus
		{
			get { return cv.NewMemberClassStatusList().ItemValue(NewMemberClassStatusId ?? 0); }
		}

		public DateTime? NewMemberClassDate { get; set; }
		public int MemberStatusId { get; set; }

		public string MemberStatus
		{
			get { return cv.MemberStatusCodes().ItemValue(MemberStatusId); }
		}

		public static MemberInfo GetMemberInfo(int? id)
		{
			var q = from p in DbUtil.Db.People
			        where p.PeopleId == id
			        select new MemberInfo
			               {
			               	PeopleId = p.PeopleId,
			               	BaptismSchedDate = p.BaptismSchedDate,
			               	BaptismDate = p.BaptismDate,
			               	DecisionDate = p.DecisionDate,
			               	DropDate = p.DropDate,
			               	DropTypeId = p.DropCodeId,
			               	JoinTypeId = p.JoinCodeId,
			               	NewChurch = p.OtherNewChurch,
			               	PrevChurch = p.OtherPreviousChurch,
			               	NewMemberClassDate = p.NewMemberClassDate,
			               	MemberStatusId = p.MemberStatusId,
			               	JoinDate = p.JoinDate,
			               	BaptismTypeId = p.BaptismTypeId ?? 0,
			               	BaptismStatusId = p.BaptismStatusId ?? 0,
			               	DecisionTypeId = p.DecisionTypeId ?? 0,
			               	EnvelopeOptionId = p.EnvelopeOptionsId ?? 0,
			               	StatementOptionId = p.ContributionOptionsId ?? 0,
			               	NewMemberClassStatusId = p.NewMemberClassStatusId ?? 0,
			               };
			return q.Single();
		}

		public string UpdateMember()
		{
			if (NewMemberClassStatusId == 0)
				NewMemberClassStatusId = null;
			if (StatementOptionId == 0)
				StatementOptionId = null;
			if (DecisionTypeId == 0)
				DecisionTypeId = null;
			if (BaptismStatusId == 0)
				BaptismStatusId = null;
			if (EnvelopeOptionId == 0)
				EnvelopeOptionId = null;
			if (BaptismTypeId == 0)
				BaptismTypeId = null;
			var p = DbUtil.Db.LoadPersonById(PeopleId);
			var psb = new StringBuilder();
			p.UpdateValue(psb, "MemberStatusId", MemberStatusId);
			p.BaptismSchedDate = BaptismSchedDate;
			p.BaptismTypeId = BaptismTypeId;
			p.BaptismStatusId = BaptismStatusId;
			p.BaptismDate = BaptismDate;
			p.DecisionDate = DecisionDate;
			p.DecisionTypeId = DecisionTypeId;
			p.DropDate = DropDate;
			p.DropCodeId = DropTypeId;
			p.EnvelopeOptionsId = EnvelopeOptionId;
			p.ContributionOptionsId = StatementOptionId;
			p.JoinCodeId = JoinTypeId;
			p.JoinDate = JoinDate;
			p.OtherNewChurch = NewChurch;
			p.OtherPreviousChurch = PrevChurch;
			p.NewMemberClassDate = NewMemberClassDate;
			p.NewMemberClassStatusId = NewMemberClassStatusId;
			p.LogChanges(DbUtil.Db, psb, Util.UserPeopleId.Value);
			var ret = p.MemberProfileAutomation(DbUtil.Db);
			if (ret == "ok")
			{
				DbUtil.Db.SubmitChanges();
				DbUtil.LogActivity("Updated Person: {0}".Fmt(p.Name));
			}
			//else
			//   Elmah.ErrorSignal.FromCurrentContext().Raise(
			//        new Exception(ret + " for PeopleId:" + p.PeopleId));
			DbUtil.Db.Refresh(RefreshMode.OverwriteCurrentValues, p);
			return ret;
		}

		private static int? CviOrNull(CodeValueItem cvi)
		{
			if (cvi == null)
				return null;
			return cvi.Id;
		}

		public static IEnumerable<SelectListItem> MemberStatuses()
		{
			return CodeValueModel.ConvertToSelect(cv.MemberStatusCodes(), "Id");
		}

		public static IEnumerable<SelectListItem> BaptismStatuses()
		{
			return CodeValueModel.ConvertToSelect(cv.BaptismStatusList(), "Id");
		}

		public static IEnumerable<SelectListItem> DecisionCodes()
		{
			return CodeValueModel.ConvertToSelect(cv.DecisionTypeList(), "Id");
		}

		public static IEnumerable<SelectListItem> EnvelopeOptions()
		{
			return CodeValueModel.ConvertToSelect(cv.EnvelopeOptionList(), "Id");
		}

		public static IEnumerable<SelectListItem> JoinTypes()
		{
			return CodeValueModel.ConvertToSelect(cv.JoinTypeList(), "Id");
		}

		public static IEnumerable<SelectListItem> BaptismTypes()
		{
			return CodeValueModel.ConvertToSelect(cv.BaptismTypeList(), "Id");
		}

		public static IEnumerable<SelectListItem> DropTypes()
		{
			return CodeValueModel.ConvertToSelect(cv.DropTypeList(), "Id");
		}

		public static IEnumerable<SelectListItem> NewMemberClassStatuses()
		{
			return CodeValueModel.ConvertToSelect(cv.NewMemberClassStatusList(), "Id");
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
