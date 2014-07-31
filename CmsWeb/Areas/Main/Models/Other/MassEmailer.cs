using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;
using System.IO;
using System.Web.Mvc;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CmsWeb.Models;

namespace CmsWeb.Areas.Main.Models
{
    [Serializable]
    public class MassEmailer
    {
        public int Count { get; set; }

        public int TagId { get; set; }
        public bool wantParents { get; set; }
        public bool CcParents { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime? Schedule { get; set; }
        public bool PublicViewable { get; set; }

        public string Host { get; set; }

        public MassEmailer()
        {
        }
        public MassEmailer(Guid id, bool? parents = null, bool? ccparents = null)
        {
            wantParents = parents ?? false;
            CcParents = ccparents ?? false;
            var q = DbUtil.Db.PeopleQuery(id);
            var c = DbUtil.Db.LoadQueryById2(id);
            var cc = c.ToClause();
            if (!cc.ParentsOf && wantParents)
                q = DbUtil.Db.PersonQueryParents(q);

            if(CcParents)
                q = from p in q
                    where (p.EmailAddress ?? "") != "" 
                        || (p.Family.HeadOfHousehold.EmailAddress ?? "") != "" 
                        || (p.Family.HeadOfHouseholdSpouse.EmailAddress ?? "") != ""
                    where (p.SendEmailAddress1 ?? true) 
                        || (p.SendEmailAddress2 ?? false)
                        || (p.Family.HeadOfHousehold.SendEmailAddress1 ?? false)
                        || (p.Family.HeadOfHousehold.SendEmailAddress2 ?? false)
                        || (p.Family.HeadOfHouseholdSpouse.SendEmailAddress1 ?? false)
                        || (p.Family.HeadOfHouseholdSpouse.SendEmailAddress2 ?? false)
                    select p;
            else
                q = from p in q
                    where p.EmailAddress != null
                    where p.EmailAddress != ""
                    where (p.SendEmailAddress1 ?? true) || (p.SendEmailAddress2 ?? false)
                    select p;
            Count = q.Count();
            var tag = DbUtil.Db.PopulateSpecialTag(q, DbUtil.TagTypeId_Emailer);
            TagId = tag.Id;
        }

        public int CreateQueue(bool transactional = false)
        {
            var From = new MailAddress(FromAddress, FromName);
            DbUtil.Db.CopySession();
            var emailqueue = DbUtil.Db.CreateQueue(From, Subject, Body, Schedule, TagId, PublicViewable, CcParents);
            if (emailqueue == null)
                return 0;
            emailqueue.Transactional = transactional;
            DbUtil.Db.SubmitChanges();
            return emailqueue.Id;
        }

        public IEnumerable<SelectListItem> EmailFroms()
        {
            return new SelectList(new CodeValueModel().PeopleToEmailFor(), "Code", "Value");
        }
    }
}