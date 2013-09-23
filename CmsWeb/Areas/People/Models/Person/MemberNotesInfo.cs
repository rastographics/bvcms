using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using System.Web.Mvc;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class MemberNotesInfo
    {
        private static CodeValueModel cv = new CodeValueModel();

        public int PeopleId { get; set; }

        public int? LetterStatusId { get; set; }
        public string LetterStatus { get { return cv.LetterStatusCodes().ItemValue(LetterStatusId ?? 0); } }
        public DateTime? LetterRequested { get; set; }
        public DateTime? LetterReceived { get; set; }
        public string LetterNotes { get; set; }

        public static MemberNotesInfo GetMemberNotesInfo(int? id)
        {
            var q = from p in DbUtil.Db.People
                    where p.PeopleId == id
                    select new MemberNotesInfo
                    {
                        PeopleId = p.PeopleId,
                        LetterStatusId = p.LetterStatusId ?? 0,
                        LetterReceived = p.LetterDateReceived,
                        LetterRequested = p.LetterDateRequested,
                        LetterNotes = p.LetterStatusNotes,
                    };
            return q.Single();
        }
        public void UpdateMemberNotes()
        {
            var p = DbUtil.Db.LoadPersonById(PeopleId);

            if (LetterStatusId == 0)
                LetterStatusId = null;

            p.LetterStatusId = LetterStatusId;
            p.LetterDateReceived = LetterReceived;
            p.LetterDateRequested = LetterRequested;
            p.LetterStatusNotes = LetterNotes;

            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("Updated Growth: {0}".Fmt(p.Name));
        }
        public static IEnumerable<SelectListItem> LetterStatuses()
        {
            return CodeValueModel.ConvertToSelect(cv.LetterStatusCodes(), "Id");
        }
    }
}
