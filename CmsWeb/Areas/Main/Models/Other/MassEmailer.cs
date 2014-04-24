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
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime? Schedule { get; set; }
        public bool PublicViewable { get; set; }

        public string Host { get; set; }
        public Session2 session2 { get; set; }

        public MassEmailer()
        {
        }
        public MassEmailer(Guid id, bool? parents)
        {
            wantParents = parents ?? false;
            var q = DbUtil.Db.PeopleQuery(id);
            var c = DbUtil.Db.LoadQueryById2((Guid)id);
            var cc = c.ToClause();
            if (!cc.ParentsOf && wantParents)
                q = DbUtil.Db.PersonQueryParents(q);

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
            var emailqueue = DbUtil.Db.CreateQueue(From, Subject, Body, Schedule, TagId, PublicViewable);
            if (emailqueue == null)
                return 0;
            emailqueue.Transactional = transactional;
            return emailqueue.Id;
        }

        public IEnumerable<SelectListItem> EmailFroms()
        {
            return new SelectList(new CodeValueModel().PeopleToEmailFor(), "Code", "Value");
        }
    }
}