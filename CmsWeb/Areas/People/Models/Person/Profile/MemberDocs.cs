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
        public int? ThumbId { get; set; }
        public int? LargeId { get; set; }
        public int? Docid { get; set; }
        public string Uploader { get; set; }
        public bool? IsDocument { get; set; }
        public string Name { get; set; }
        public string FormName { get; set; }

        public string DocUrl
        {
            get
            {
                if (IsDocument == true)
                {
                    return "/Image/" + Docid;
                }

                return "/Image/" + LargeId;
            }
        }

        public string ImgUrl
        {
            get
            {
                if (IsDocument == true)
                {
                    return "/Content/images/adobe.png";
                }

                return "/Image/" + ThumbId;
            }
        }
        public MemberDocModel() { }
        public static IEnumerable<MemberDocModel> DocForms(int id)
        {
            return from f in DbUtil.Db.MemberDocForms
                   where f.PeopleId == id
                   let uploader = DbUtil.Db.People.SingleOrDefault(uu => uu.PeopleId == f.UploaderId)
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
                       Uploader = uploader.Name
                   };
        }
        public static void DeleteDocument(CMSDataContext db, CMSImageDataContext idb, int id, int docid)
        {
            var m = db.MemberDocForms.SingleOrDefault(mm => mm.Id == docid && mm.PeopleId == id);
            idb.DeleteOnSubmit(m.SmallId);
            idb.DeleteOnSubmit(m.MediumId);
            idb.DeleteOnSubmit(m.LargeId);
            db.MemberDocForms.DeleteOnSubmit(m);
            db.SubmitChanges();
            idb.SubmitChanges();
        }

        internal static void UpdateName(CMSDataContext db, int pk, string value)
        {
            var m = db.MemberDocForms.Single(mm => mm.Id == pk);
            m.Name = value;
            db.SubmitChanges();
        }
    }
}
