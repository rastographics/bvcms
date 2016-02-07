using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CmsData.API;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public void EmailContent(object savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string contentName)
        {
            EmailContent2(savedQuery, queuedBy, fromAddr, fromName, subject, contentName);
        }

        public void Email2(Guid qid, int queuedBy, string fromAddr, string fromName, string subject, string body)
        {
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery(qid);
                Email2(db2, q, queuedBy, fromAddr, fromName, subject, body);
            }
        }

        public void EmailContent2(Guid qid, int queuedBy, string fromAddr, string fromName, string contentName)
        {
            var c = db.ContentOfTypeHtml(contentName);
            if (c == null)
                return;
            Email2(qid, queuedBy, fromAddr, fromName, c.Title, c.Body);
        }

        public void EmailContent2(Guid qid, int queuedBy, string fromAddr, string fromName, string subject,
            string contentName)
        {
            var c = db.ContentOfTypeHtml(contentName);
            if (c == null)
                return;
            Email2(qid, queuedBy, fromAddr, fromAddr, subject, c.Body);
        }
        public Guid OrgMembersQuery(int progid, int divid, int orgid, string memberTypes)
        {
            var c = db.ScratchPadCondition();
            c.Reset();
            var mtlist = memberTypes.Split(',');
            var mts = string.Join(";", from mt in db.MemberTypes
                                       where mtlist.Contains(mt.Description)
                                       select $"{mt.Id},{mt.Code}");
            var clause = c.AddNewClause(QueryType.MemberTypeCodes, CompareType.OneOf, mts);
            clause.Program = progid.ToString();
            clause.Division = divid.ToString();
            clause.Organization = orgid.ToString();
            c.Save(db);
            return c.Id;
        }

    }
}