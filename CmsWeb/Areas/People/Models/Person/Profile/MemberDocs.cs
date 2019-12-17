using CmsData;
using ImageData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Areas.People.Models
{
    public class MemberDocModel
    {
        public int Id { get; set; }
        public DateTime? DocDate { get; set; }
        public int? Docid { get; set; }
        public bool Finance { get; set; }
        public string FormName { get; set; }
        public bool? IsDocument { get; set; }
        public int? LargeId { get; set; }
        public string Name { get; set; }
        public int? ThumbId { get; set; }
        public string Uploader { get; set; }

        private string DocsRoot => (Finance ? "/FinanceDocs/" : "/MemberDocs/");
        public string DocUrl => DocsRoot + (IsDocument == true ? Docid : LargeId);

        public string ImgUrl => IsDocument == true 
            ? "/Content/images/adobe.png"
            : DocsRoot + ThumbId;
        
        public MemberDocModel() { }

        public static IEnumerable<MemberDocModel> DocForms(CMSDataContext db, int peopleId, bool finance)
        {
            return from f in db.MemberDocForms
                   where f.PeopleId == peopleId
                   where f.Finance == finance
                   let uploader = db.People.SingleOrDefault(uu => uu.PeopleId == f.UploaderId)
                   orderby f.DocDate
                   select new MemberDocModel
                   {
                       DocDate = f.DocDate,
                       Id = f.Id,
                       ThumbId = f.SmallId,
                       LargeId = f.LargeId,
                       Docid = f.MediumId,
                       IsDocument = f.IsDocument,
                       Name = f.Person.Name,
                       FormName = f.Name,
                       Finance = f.Finance,
                       Uploader = uploader.Name
                   };
        }

        public static MemberDocForm DeleteDocument(CMSDataContext db, CMSImageDataContext idb, int id, int docid)
        {
            var m = db.MemberDocForms.SingleOrDefault(mm => mm.Id == docid && mm.PeopleId == id);
            idb.DeleteOnSubmit(m.SmallId);
            idb.DeleteOnSubmit(m.MediumId);
            idb.DeleteOnSubmit(m.LargeId);
            db.MemberDocForms.DeleteOnSubmit(m);
            db.SubmitChanges();
            idb.SubmitChanges();
            return m;
        }

        internal static void UpdateName(CMSDataContext db, int pk, string value)
        {
            var m = db.MemberDocForms.Single(mm => mm.Id == pk);
            m.Name = value;
            db.SubmitChanges();
        }
    }
}
