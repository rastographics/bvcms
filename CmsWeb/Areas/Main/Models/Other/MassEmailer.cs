using CmsData;
using CmsWeb.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Models
{
    [Serializable]
    public class MassEmailer
    {
        public int Count { get; set; }

        public int TagId { get; set; }
        public int? OrgId { get; set; }
        public bool wantParents { get; set; }
        public bool noDuplicates { get; set; }
        public bool CcParents { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string UnlayerDesign { get; set; }
        public bool? UseUnlayer { get; set; }
        public DateTime? Schedule { get; set; }
        public bool PublicViewable { get; set; }
        public List<string> Recipients { get; set; }
        public List<int> RecipientIds { get; set; }
        public IEnumerable<int> AdditionalRecipients { get; set; }
        public bool OnlyProspects { get; set; }

        public List<MailAddress> CcAddresses = new List<MailAddress>();
        public bool recovery;
        public Guid guid;

        public string Cc
        {
            get
            {
                if (CcAddresses == null) { return null; }
                return String.Join(",", CcAddresses);
            }
            set
            {
                if (value == null) { CcAddresses = null; }
                else
                {
                    try
                    {
                        CcAddresses = value.Split(',').Select(a => new MailAddress(a)).ToList();
                    }
                    catch
                    {
                        List<String> CcAddressesString = value.Split(',').ToList();
                        var re1 = new Regex(@"^(.*\b(?=\w))\b[A-Z0-9._%+-]+(?<=[^.])@[A-Z0-9._-]+\.[A-Z]{2,4}\b\b(?!\w)$", RegexOptions.IgnoreCase);
                        var re2 = new Regex(@"^[A-Z0-9._%+-]+(?<=[^.])@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase);

                        foreach (string address in CcAddressesString)
                        {
                            string s = address.Trim();
                            if (re1.IsMatch(s) || re2.IsMatch(s))
                            {
                                CcAddresses.Add(new MailAddress(s));
                            }
                        }
                    };

                }
            }
        }


        public string Host { get; set; }

        public MassEmailer()
        {
        }

        public MassEmailer(int tagid, string host, string subject, bool? parents = false)
        {
            wantParents = parents ?? false;
            var tag = DbUtil.Db.TagById(tagid);
            TagId = tag.Id;
            var people = tag.People(DbUtil.Db);
            Recipients = people.Select(p => p.ToString()).ToList();
            Count = people.Count();

        }
        public MassEmailer(Guid id, bool? parents = null, bool? ccparents = null, bool? nodups = null, int? orgid = null)
        {
            wantParents = parents ?? false;
            noDuplicates = nodups ?? false;
            CcParents = ccparents ?? false;
            OrgId = orgid;
            IQueryable<Person> q = null;
            if (OrgId.HasValue && id != Guid.Empty)
            {
                q = from p in DbUtil.Db.PeopleQuery(id)
                    where ((p.EmailAddress ?? "") != "" && (p.SendEmailAddress1 ?? true))
                        || ((p.EmailAddress2 ?? "") != "" && (p.SendEmailAddress2 ?? false))
                    select p;
            }
            else
            {
                if (id == Guid.Empty)
                {
                    q = DbUtil.Db.PeopleQuery2(Util.UserPeopleId.ToString());
                }
                else
                {
                    q = DbUtil.Db.PeopleQuery(id);
                }

                var c = DbUtil.Db.LoadQueryById2(id);
                var cc = c.ToClause();

                if (!cc.PlusParentsOf && !cc.ParentsOf && wantParents)
                {
                    q = DbUtil.Db.PersonQueryParents(q);
                }

                if (CcParents)
                {
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
                }
                else
                {
                    q = from p in q
                        where ((p.EmailAddress ?? "") != "" && (p.SendEmailAddress1 ?? true))
                            || ((p.EmailAddress2 ?? "") != "" && (p.SendEmailAddress2 ?? false))
                        select p;
                }
            }
            var tag = DbUtil.Db.PopulateSpecialTag(q, DbUtil.TagTypeId_Emailer);
            TagId = tag.Id;
            if (noDuplicates)
            {
                DbUtil.Db.NoEmailDupsInTag(TagId);
            }

            var people = tag.People(DbUtil.Db);
            Recipients = people.Select(p => p.ToString()).ToList();
            Count = people.Count();
        }

        public EmailQueue CreateQueueForOrg()
        {
            var From = new MailAddress(FromAddress, FromName);
            DbUtil.Db.CopySession();

            if (!Schedule.HasValue)
            {
                var tag = DbUtil.Db.TagById(TagId);
                var q = tag.People(DbUtil.Db);
                Count = q.Count();
                if (Count >= 300)
                {
                    Schedule = Util.Now.AddSeconds(10); // some time for emailqueue to be ready to go
                }
            }

            if (!OrgId.HasValue)
            {
                throw new Exception("no org to email from");
            }

            var emailqueue = DbUtil.Db.CreateQueueForOrg(From, Subject, Body, Schedule, OrgId.Value, PublicViewable, Cc);
            if (emailqueue == null)
            {
                return null;
            }

            emailqueue.NoReplacements = noDuplicates;
            DbUtil.Db.SubmitChanges();
            return emailqueue;
        }
        public EmailQueue CreateQueue(bool transactional = false)
        {
            if (OrgId.HasValue)
            {
                return CreateQueueForOrg();
            }

            var From = new MailAddress(FromAddress, FromName);
            DbUtil.Db.CopySession();

            if (!Schedule.HasValue)
            {
                var tag = DbUtil.Db.TagById(TagId);
                if (tag == null)
                {
                    throw new Exception("email tag is missing");
                }

                var q = tag.People(DbUtil.Db);
                Count = q.Count();
                if (Count >= 150)
                {
                    Schedule = Util.Now.AddSeconds(10); // some time for emailqueue to be ready to go
                }
            }

            var emailqueue = DbUtil.Db.CreateQueue(From, Subject, Body, Schedule, TagId, PublicViewable, CcParents, Cc);
            if (emailqueue == null)
            {
                return null;
            }

            emailqueue.NoReplacements = noDuplicates;
            emailqueue.Transactional = transactional;
            DbUtil.Db.SubmitChanges();
            return emailqueue;
        }

        public IEnumerable<SelectListItem> EmailFroms()
        {
            return new SelectList(new CodeValueModel().PeopleToEmailFor(), "Code", "Value");
        }
    }
}
