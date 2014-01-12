using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Web.Mvc;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Models.PersonPage
{
    public class GrowthInfo
    {
        private static CodeValueModel cv = new CodeValueModel();

        public int PeopleId { get; set; }

        public int? InterestPointId { get; set; }
        public string InterestPoint { get { return cv.InterestPoints().ItemValue(InterestPointId ?? 0); } }
        public int? OriginId { get; set; }
        public string Origin { get { return cv.Origins().ItemValue(OriginId ?? 0); } }
        public int? EntryPointId { get; set; }
        public string EntryPoint { get { return cv.EntryPoints().ItemValue(EntryPointId ?? 0); } }
        public bool? MemberAnyChurch { get; set; }
        public bool ChristAsSavior { get; set; }
        public bool PleaseVisit { get; set; }
        public bool InterestedInJoining { get; set; }
        public bool SendInfo { get; set; }
        public string Comments { get; set; }

        public static GrowthInfo GetGrowthInfo(int? id)
        {
            var q = from p in DbUtil.Db.People
                    where p.PeopleId == id
                    select new GrowthInfo
                    {
                        PeopleId = p.PeopleId,

                        InterestPointId = p.InterestPointId ?? 0,
                        OriginId = p.OriginId ?? 0,
                        EntryPointId = p.EntryPointId ?? 0,
                        Comments = p.Comments,
                        ChristAsSavior = p.PeopleExtras.SingleOrDefault(ee => ee.Field == "IC:ChristAsSavior").BitValue ?? false,
                        InterestedInJoining = p.PeopleExtras.SingleOrDefault(ee => ee.Field == "IC:InterestedInJoining").BitValue ?? false,
                        SendInfo = p.PeopleExtras.SingleOrDefault(ee => ee.Field == "IC:InfoBecomeAChristian").BitValue ?? false,
                        PleaseVisit = p.PeopleExtras.SingleOrDefault(ee => ee.Field == "IC:PleaseVisit").BitValue ?? false,
                        MemberAnyChurch = p.PeopleExtras.SingleOrDefault(ee => ee.Field == "IC:MemberAnyChurch").BitValue ?? false,
                    };
            return q.Single();
        }
        public void UpdateGrowth()
        {
            var p = DbUtil.Db.LoadPersonById(PeopleId);
            var q = from pp in DbUtil.Db.People
                    where pp.PeopleId == PeopleId
                    select new GrowthInfo
                    {
                        ChristAsSavior = pp.PeopleExtras.SingleOrDefault(ee => ee.Field == "IC:ChristAsSavior").BitValue ?? false,
                        InterestedInJoining = pp.PeopleExtras.SingleOrDefault(ee => ee.Field == "IC:InterestedInJoining").BitValue ?? false,
                        SendInfo = pp.PeopleExtras.SingleOrDefault(ee => ee.Field == "IC:InfoBecomeAChristian").BitValue ?? false,
                        PleaseVisit = pp.PeopleExtras.SingleOrDefault(ee => ee.Field == "IC:PleaseVisit").BitValue ?? false,
                        MemberAnyChurch = pp.PeopleExtras.SingleOrDefault(ee => ee.Field == "IC:MemberAnyChurch").BitValue ?? false,
                    };
            var gi = q.Single();

            if (InterestPointId == 0)
                InterestPointId = null;
            if (OriginId == 0)
                OriginId = null;
            if (EntryPointId == 0)
                EntryPointId = null;

            p.InterestPointId = InterestPointId;
            p.OriginId = OriginId;
            p.EntryPointId = EntryPointId;
            p.Comments = Comments;

            if(gi.ChristAsSavior != ChristAsSavior)
                if(gi.ChristAsSavior)
                    p.RemoveExtraValue(DbUtil.Db, "IC:ChristAsSavior");
                else
                    p.AddEditExtraBool("IC:ChristAsSavior", true);

            if(gi.InterestedInJoining != InterestedInJoining)
                if(gi.InterestedInJoining)
                    p.RemoveExtraValue(DbUtil.Db, "IC:InterestedInJoining");
                else
                    p.AddEditExtraBool("IC:InterestedInJoining", true);

            if(gi.MemberAnyChurch != MemberAnyChurch)
                if(gi.MemberAnyChurch == true)
                    p.RemoveExtraValue(DbUtil.Db, "IC:MemberAnyChurch");
                else
                    p.AddEditExtraBool("IC:MemberAnyChurch", true);

            if(gi.PleaseVisit != PleaseVisit)
                if(gi.PleaseVisit)
                    p.RemoveExtraValue(DbUtil.Db, "IC:PleaseVisit");
                else
                    p.AddEditExtraBool("IC:PleaseVisit", true);
            
            if(gi.SendInfo != SendInfo)
                if(gi.SendInfo)
                    p.RemoveExtraValue(DbUtil.Db, "IC:InfoBecomeAChristian");
                else
                    p.AddEditExtraBool("IC:InfoBecomeAChristian", true);

            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("Updated Growth: {0}".Fmt(p.Name));
        }
        public static IEnumerable<SelectListItem> InterestPoints()
        {
            return CodeValueModel.ConvertToSelect(cv.InterestPoints(), "Id");
        }
        public static IEnumerable<SelectListItem> Origins()
        {
            return CodeValueModel.ConvertToSelect(cv.Origins(), "Id");
        }
        public static IEnumerable<SelectListItem> EntryPoints()
        {
            return CodeValueModel.ConvertToSelect(cv.EntryPoints(), "Id");
        }
    }
}
