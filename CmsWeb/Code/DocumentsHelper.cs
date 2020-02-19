using CmsData;
using CmsWeb.Areas.OnlineReg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Code
{
    public class DocumentsHelper
    {
        public static void SaveTemporaryDocuments(CMSDataContext db, List<OnlineRegPersonModel> NewPeople)
        {
            foreach (var person in NewPeople)
            {
                var tempDocuments = db.OrgMemberDocuments.Where(p => p.DocumentName.Contains(person.EmailAddress));
                var newDocuments = ChangeDocName(db, tempDocuments.ToList(), person.EmailAddress);
                db.OrgMemberDocuments.DeleteAllOnSubmit(tempDocuments);
                db.SubmitChanges();

                db.OrgMemberDocuments.InsertAllOnSubmit(newDocuments);
                db.SubmitChanges();
            }
        }

        public static List<OrgMemberDocument> ChangeDocName(CMSDataContext db, List<OrgMemberDocument> tempDocuments, string personEmail)
        {
            var person = db.People.SingleOrDefault(p => p.EmailAddress == personEmail);
            List<OrgMemberDocument> newDocuments = new List<OrgMemberDocument>();

            foreach (var item in tempDocuments)
            {
                var tempDocName = item.DocumentName.Split('_');
                newDocuments.Add(new OrgMemberDocument()
                {
                    DocumentName = $"{tempDocName[0]}_{person.LastName}_{person.FirstName}_{person.PeopleId}",
                    ImageId = item.ImageId,
                    PeopleId = person.PeopleId,
                    OrganizationId = item.OrganizationId
                });
            }
            return newDocuments;
        }
    }
}
