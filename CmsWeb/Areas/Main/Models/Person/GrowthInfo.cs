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
                        ChristAsSavior = p.PeopleExtras.SingleOrDefault(ee => ee.Field == "ChristAsSavior").BitValue ?? false,
                        InterestedInJoining = p.PeopleExtras.SingleOrDefault(ee => ee.Field == "InterestedInJoining").BitValue ?? false,
                        SendInfo = p.PeopleExtras.SingleOrDefault(ee => ee.Field == "InfoBecomeAChristian").BitValue ?? false,
                        PleaseVisit = p.PeopleExtras.SingleOrDefault(ee => ee.Field == "PleaseVisit").BitValue ?? false,
                        MemberAnyChurch = p.PeopleExtras.SingleOrDefault(ee => ee.Field == "MemberAnyChurch").BitValue ?? false,
                    };
            return q.Single();
        }
        public void UpdateGrowth()
        {
            var p = DbUtil.Db.LoadPersonById(PeopleId);

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

            if(p.ChristAsSavior != ChristAsSavior)
                if(p.ChristAsSavior)
                    p.RemoveExtraValue(DbUtil.Db, "ChristAsSavior");
                else
                    p.AddEditExtraBool("ChristAsSavior", true);

            if(p.InterestedInJoining != InterestedInJoining)
                if(p.InterestedInJoining)
                    p.RemoveExtraValue(DbUtil.Db, "InterestedInJoining");
                else
                    p.AddEditExtraBool("InterestedInJoining", true);

            if(p.MemberAnyChurch != MemberAnyChurch)
                if(p.MemberAnyChurch == true)
                    p.RemoveExtraValue(DbUtil.Db, "MemberAnyChurch");
                else
                    p.AddEditExtraBool("MemberAnyChurch", true);

            if(p.PleaseVisit != PleaseVisit)
                if(p.PleaseVisit)
                    p.RemoveExtraValue(DbUtil.Db, "PleaseVisit");
                else
                    p.AddEditExtraBool("PleaseVisit", true);
            
            if(p.InfoBecomeAChristian != SendInfo)
                if(p.InfoBecomeAChristian)
                    p.RemoveExtraValue(DbUtil.Db, "InfoBecomeAChristian");
                else
                    p.AddEditExtraBool("InfoBecomeAChristian", true);

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
