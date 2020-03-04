using CmsData;
using CmsWeb.Areas.OnlineReg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImageData;

namespace CmsWeb.Code
{
    public class DocumentsHelper
    {
        public static void SaveTemporaryDocuments(CMSDataContext db, List<OnlineRegPersonModel> NewPeople, int orgId)
        {
            foreach (var person in NewPeople)
            {
                var tempDocuments = db.OrgTemporaryDocuments.Where(p => p.OrganizationId == orgId & p.LastName == person.LastName & p.FirstName == person.FirstName & p.EmailAddress == person.EmailAddress);
                var newDocuments = ChangeToMemberDocuments(db, tempDocuments.ToList(), person.PeopleId.Value);
                db.OrgTemporaryDocuments.DeleteAllOnSubmit(tempDocuments);
                db.SubmitChanges();

                db.OrgMemberDocuments.InsertAllOnSubmit(newDocuments);
                db.SubmitChanges();
            }
        }

        public static void CreateTemporaryDocument(CMSDataContext db, CMSImageDataContext idb, string docname, int orgId, string emailAddress, string lastName, string firstName, HttpPostedFileBase file)
        {
            var document = db.OrgTemporaryDocuments.SingleOrDefault(o => o.DocumentName == docname & o.LastName == lastName & o.FirstName == firstName & o.EmailAddress == emailAddress);
            if (document != null)
            {
                db.OrgTemporaryDocuments.DeleteOnSubmit(document);
                Image.Delete(idb, document.ImageId);
                db.SubmitChanges();
            }

            int imageId = DocumentsData.StoreImageFromDocument(idb, file);
            db.OrgTemporaryDocuments.InsertOnSubmit(new OrgTemporaryDocuments()
            {
                DocumentName = docname,
                ImageId = imageId,
                LastName = lastName,
                FirstName = firstName,
                EmailAddress = emailAddress,
                OrganizationId = orgId,
                CreatedDate = DateTime.Now
            });
            db.SubmitChanges();
        }

        public static void CreateMemberDocument(CMSDataContext db, CMSImageDataContext idb, string docname, int orgId, Person person, HttpPostedFileBase file)
        {
            string docName = $"{docname}_{person.LastName}_{person.FirstName}_{person.PeopleId}";

            var document = db.OrgMemberDocuments.SingleOrDefault(o => o.DocumentName == docName & o.PeopleId == person.PeopleId & o.OrganizationId == orgId);
            if (document != null)
            {
                db.OrgMemberDocuments.DeleteOnSubmit(document);
                Image.Delete(idb, document.ImageId);
                db.SubmitChanges();
            }

            int imageId = DocumentsData.StoreImageFromDocument(idb, file);
            db.OrgMemberDocuments.InsertOnSubmit(new OrgMemberDocument()
            {
                DocumentName = docName,
                ImageId = imageId,
                PeopleId = person.PeopleId,
                OrganizationId = orgId,
                CreatedDate = DateTime.Now
            });
            db.SubmitChanges();
        }

        public static List<OrgMemberDocument> ChangeToMemberDocuments(CMSDataContext db, List<OrgTemporaryDocuments> tempDocuments, int peopleId)
        {
            var person = db.People.SingleOrDefault(p => p.PeopleId == peopleId);
            List<OrgMemberDocument> newDocuments = new List<OrgMemberDocument>();

            foreach (var item in tempDocuments)
            {
                var tempDocName = item.DocumentName.Split('_');
                newDocuments.Add(new OrgMemberDocument()
                {
                    DocumentName = $"{item.DocumentName}_{person.LastName}_{person.FirstName}_{person.PeopleId}",
                    ImageId = item.ImageId,
                    PeopleId = person.PeopleId,
                    OrganizationId = item.OrganizationId,
                    CreatedDate = DateTime.Now
                });
            }
            return newDocuments;
        }
    }
}
