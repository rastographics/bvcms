using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using CmsData;
using CmsData.Registration;
using Newtonsoft.Json;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models
{
    public class MissionTripEmailer
    {
        private int orgId;
        public int PeopleId { get; set; }

        public int OrgId
        {
            get { return orgId; }
            set
            {
                orgId = value;
                var m = DbUtil.Db.CreateRegistrationSettings(value);
                Subject = m.SupportSubject;
                Body = m.SupportBody;
            }
        }

        public List<RecipientItem> Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public static IEnumerable<SearchInfo> Search(int pid, string text)
        {
            text = text.Trim();

            if (!Util.ValidEmail(text) && text.Trim().GetDigits().Length != 10)
                return new List<SearchInfo>();

            var qp = from p in DbUtil.Db.People
                     where p.DeceasedDate == null
                     select p;

            var phone = text.GetDigits();
            if (phone.Length == 10)
                qp = from p in qp
                     where p.CellPhone.Contains(phone)
                           || p.Family.HomePhone.Contains(phone)
                           || p.WorkPhone.Contains(phone)
                     select p;
            else if (Util.ValidEmail(text))
                qp = from p in qp
                     where p.EmailAddress == text || p.EmailAddress2 == text
                     select p;
            const string addsupport = "/MissionTripEmail2/AddSupporter/{0}/{1}";
            var rp = from p in qp
                     where (p.EmailAddress.Length > 0 && (p.SendEmailAddress1 ?? true))
                           || (p.EmailAddress2.Length > 0 && p.SendEmailAddress2 == true)
                     orderby p.Name2
                     select new SearchInfo
                     {
                         url = string.Format(addsupport, pid, p.PeopleId),
                         age = p.Age,
                         name = p.Name2,
                         peopleid = p.PeopleId,
                         line2 = p.PrimaryAddress ?? ""
                     };
            var list = rp.Take(8).ToList();
            if (list.Count == 0 && Util.ValidEmail(text))
                list.Add(new SearchInfo
                {
                    url = string.Format(addsupport, pid, text),
                    name = text,
                });
            return list;
        }

        public List<RecipientItem> GetRecipients()
        {
            var q = from g in DbUtil.Db.GoerSupporters
                    where g.GoerId == PeopleId
                    where (g.Unsubscribe ?? false) == false
                    let sender = (from s in DbUtil.Db.GoerSenderAmounts
                                  where s.GoerId == g.GoerId
                                  where s.OrgId == OrgId
                                  where (s.InActive ?? false) == false
                                  where s.SupporterId == g.SupporterId
                                  where (s.NoNoticeToGoer ?? false) == false
                                  select s.Amount).Sum()
                    select new RecipientItem
                    {
                        Id = g.Id,
                        NameOrEmail = g.SupporterId != null
                            ? g.Supporter.Name
                            : g.NoDbEmail,
                        Active = g.Active ?? false,
                        SenderAmt = sender ?? 0,
                        Salutation = (g.Salutation.Length > 0)
                            ? g.Salutation
                            : g.SupporterId == null
                                ? "Dear Friend"
                                : (g.Goer.Age < 30 && (g.Supporter.Age - g.Goer.Age) > 10)
                                    ? "Dear " + g.Supporter.TitleCode + " " + g.Supporter.LastName
                                    : "Hi " + g.Supporter.PreferredName
                    };
            return q.ToList();
        }

        public void UpdateRecipients()
        {
            var q = (from g in DbUtil.Db.GoerSupporters
                     where g.GoerId == PeopleId
                     where (g.Unsubscribe ?? false) == false
                     select new {g, goer = g.Goer, supporter = g.Supporter}).ToList();
            var list = (from gs in q
                        join r in Recipients on gs.g.Id equals r.Id
                        select new {gs, r}).ToList();
            foreach (var i in list)
            {
                var goersupporter = i.gs.g;
                var goer = i.gs.goer;
                var supporter = i.gs.supporter;
                var recipient = i.r;
                goersupporter.Active = i.r.Active;
                goersupporter.Salutation = recipient.Salutation;
                if (recipient.Salutation.HasValue())
                    continue;
                goersupporter.Salutation = supporter == null
                    ? "Dear Friend"
                    : (goer.Age < 30 && (supporter.Age - goer.Age) > 10)
                        ? "Dear " + supporter.TitleCode + " " + supporter.LastName
                        : "Hi " + supporter.PreferredName;
            }
            DbUtil.Db.SubmitChanges();
            Recipients = GetRecipients();
        }

        public void RemoveSupporter(int id)
        {
            var q = from e in DbUtil.Db.GoerSupporters
                    where e.Id == id
                    select e;
            var r = q.Single();
            DbUtil.Db.GoerSupporters.DeleteOnSubmit(r);
            DbUtil.Db.SubmitChanges();
            Recipients = GetRecipients();
        }

        public int AddRecipient(int? supporterid, string email)
        {
            var q = from e in DbUtil.Db.GoerSupporters
                    where e.GoerId == PeopleId
                    where e.SupporterId == supporterid || e.NoDbEmail == email
                    select e;
            var r = q.SingleOrDefault();
            if (r == null)
            {
                r = new GoerSupporter
                {
                    GoerId = PeopleId,
                    SupporterId = supporterid,
                    NoDbEmail = email,
                    Active = true,
                    Created = DateTime.Now
                };
                var supporter = DbUtil.Db.People.SingleOrDefault(pp => pp.PeopleId == supporterid);
                var goer = DbUtil.Db.People.SingleOrDefault(pp => pp.PeopleId == PeopleId);
                if (supporter == null)
                    r.Salutation = "Dear Friend";
                else if (goer != null && goer.Age < 30 && (supporter.Age - goer.Age) > 10)
                    r.Salutation = "Dear " + supporter.TitleCode + " " + supporter.LastName;
                else
                    r.Salutation = "Hi " + supporter.PreferredName;

                DbUtil.Db.GoerSupporters.InsertOnSubmit(r);
            }
            else
                r.Active = true;
            DbUtil.Db.SubmitChanges();
            Recipients = GetRecipients();
            return r.Id;
        }

        public string TestSend()
        {
            var p = DbUtil.Db.LoadPersonById(Util.UserPeopleId.Value);
            try
            {
                DbUtil.Db.CopySession();
                var from = new MailAddress(p.EmailAddress ?? p.EmailAddress2, p.Name);
                DbUtil.Db.SetCurrentOrgId(OrgId);
                var gs = new GoerSupporter
                {
                    Created = DateTime.Now,
                    Goer = p,
                    Supporter = p,
                    SupporterId = p.PeopleId,
                    GoerId = p.PeopleId
                };
                var plist = new List<GoerSupporter> {gs};
                DbUtil.Db.SubmitChanges();
                var emailQueue = DbUtil.Db.CreateQueueForSupporters(p.PeopleId, from, Subject, Body, null, plist, false);
                DbUtil.Db.SendPeopleEmail(emailQueue.Id);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new {error = true, message = ex.Message});
            }
            return JsonConvert.SerializeObject(new {message = "Test Email Sent"});
        }

        public string Send()
        {
            var p = DbUtil.Db.LoadPersonById(PeopleId);
            if (!Subject.HasValue() || !Body.HasValue())
                return "Subject needs some text";

            DbUtil.LogActivity($"MissionTripEmail Send {PeopleId}");

            var glist = from g in DbUtil.Db.GoerSupporters
                        where (g.Unsubscribe ?? false) == false
                        where (g.Active == true)
                        where g.GoerId == PeopleId
                        select g;
            var elist = (from g in glist
                         where g.SupporterId == null
                         select g).ToList();
            var plist = (from g in glist
                         where g.SupporterId != null
                         select g).ToList();

            if (plist.Any())
            {
                DbUtil.Db.CopySession();
                var from = new MailAddress(p.EmailAddress ?? p.EmailAddress2, p.Name);
                DbUtil.Db.SetCurrentOrgId(OrgId);
                var emailQueue = DbUtil.Db.CreateQueueForSupporters(p.PeopleId, from, Subject, Body, null, plist, false);
                DbUtil.Db.SendPeopleEmail(emailQueue.Id);
            }
            foreach (var e in elist)
            {
                SendNoDbEmail(p, e);
            }
            return null;
        }

        private void SendNoDbEmail(Person goer, GoerSupporter gs)
        {
            var from = new MailAddress(goer.EmailAddress ?? goer.EmailAddress2, goer.Name);

            try
            {
                var text = Body;
                //var aa = DbUtil.Db.DoReplacements(ref text, goer, null);

                text = text.Replace("{salutation}", gs.Salutation, true);
                text = text.Replace("{track}", "", true);
                var qs = "OptOut/UnSubscribe/?gid=" + Util.EncryptForUrl($"{gs.Id}");
                var url = DbUtil.Db.ServerLink(qs);
                var link = $@"<a href=""{url}"">Unsubscribe</a>";
                text = text.Replace("{unsubscribe}", link, true);
                text = text.Replace("%7Bfromemail%7D", from.Address, true);
                var supportlink = DbUtil.Db.ServerLink($"/OnlineReg/{OrgId}?gsid={gs.Id}");
                text = text.Replace("http://supportlink", supportlink, true);
                text = text.Replace("https://supportlink", supportlink, true);
                DbUtil.Db.SendEmail(from, Subject, text, Util.ToMailAddressList(gs.NoDbEmail), gs.Id);
            }
            catch (Exception ex)
            {
                DbUtil.Db.SendEmail(from, "sent emails - error", ex.ToString(),
                    Util.ToMailAddressList(from), gs.Id);
                throw;
            }
        }

        public class SearchInfo
        {
            public string url { get; set; }
            public string line1 => name + (age != null ? $"({Person.AgeDisplay(age, peopleid)})" : "");
            public string line2 { get; set; }
            internal int peopleid;
            internal int? age;
            internal string name;
        }

        public class RecipientItem
        {
            public int Id { get; set; }
            public string NameOrEmail { get; set; }
            public bool Active { get; set; }
            public string Salutation { get; set; }
            public decimal SenderAmt { get; set; }
        }
    }
}
