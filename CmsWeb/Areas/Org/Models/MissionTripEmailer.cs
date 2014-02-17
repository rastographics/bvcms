using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using CmsData;
using Newtonsoft.Json;
using UtilityExtensions;
using Settings = CmsData.Registration.Settings;

namespace CmsWeb.Areas.Org.Models
{
    public class MissionTripEmailer
    {

        public int PeopleId { get; set; }

        private int orgId;
        public int OrgId
        {
            get { return orgId; }
            set
            {
                orgId = value;
                var org = DbUtil.Db.LoadOrganizationById(value);
                var m = new Settings(org.RegSetting, DbUtil.Db, value);
                Subject = m.SupportSubject;
                Body = m.SupportBody;
            }
        }
        public List<int> Recipient { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }

        public class SearchInfo
        {
            public string url { get; set; }
            public string line1 { get; set; }
            public string line2 { get; set; }
        }

        public static IEnumerable<SearchInfo> Search(int pid, string text)
        {
            text = text.Trim();

            if (!Util.ValidEmail(text) && text.Trim().GetDigits().Length != 10)
                return new List<SearchInfo>();

            var qp = from p in DbUtil.Db.People.AsQueryable()
                     where p.DeceasedDate == null
                     select p;

            var phone = text.GetDigits();
            if (phone.Length == 10)
                qp = from p in qp
                     where (p.EmailAddress.HasValue() && p.SendEmailAddress1 == true)
                            || (p.EmailAddress2.HasValue() && p.SendEmailAddress2 == true)
                     where
                         p.CellPhone.Contains(phone)
                         || p.Family.HomePhone.Contains(phone)
                         || p.WorkPhone.Contains(phone)
                     select p;
            else if (Util.ValidEmail(text))
                qp = from p in qp
                     where (p.EmailAddress == text && p.SendEmailAddress1 == true)
                           || (p.EmailAddress2 == text && p.SendEmailAddress2 == true)
                     select p;
            const string addsupport = "/MissionTripEmail/AddSupporter/{0}/{1}";
            var rp = from p in qp
                     let age = p.Age.HasValue ? " (" + p.Age + ")" : ""
                     orderby p.Name2
                     select new SearchInfo()
                     {
                         url = addsupport.Fmt(pid, p.PeopleId),
                         line1 = p.Name2 + age,
                         line2 = p.PrimaryAddress ?? "",
                     };
            var list = rp.Take(8).ToList();
            if (list.Count == 0 && Util.ValidEmail(text))
                list.Add(new SearchInfo()
                {
                    url = addsupport.Fmt(pid, text),
                    line1 = text,
                });
            return list;
        }

        public class RecipientItem
        {
            public int Id { get; set; }
            public string NameOrEmail { get; set; }
            public bool Active { get; set; }
        }

        public static IEnumerable<RecipientItem> GetRecipients(int id)
        {
            var q = from g in DbUtil.Db.GoerSupporters
                    where g.GoerId == id
                    where (g.Unsubscribe ?? false) == false
                    select new RecipientItem()
                    {
                        Id = g.Id,
                        NameOrEmail = g.SupporterId != null
                            ? g.Supporter.Name
                            : g.NoDbEmail,
                        Active = g.Active ?? false,
                    };
            return q;
        }

        public static int ToggleActive(int id)
        {
            var q = from e in DbUtil.Db.GoerSupporters
                    where e.Id == id
                    select e;
            var r = q.Single();
            r.Active = !r.Active;
            DbUtil.Db.SubmitChanges();
            return r.GoerId;
        }
        public static int RemoveSupporter(int id)
        {
            var q = from e in DbUtil.Db.GoerSupporters
                    where e.Id == id
                    select e;
            var r = q.Single();
            var goerid = r.GoerId;
            DbUtil.Db.GoerSupporters.DeleteOnSubmit(r);
            DbUtil.Db.SubmitChanges();
            return goerid;
        }

        public static int AddRecipient(int id, int? supporterid, string email)
        {
            var q = from e in DbUtil.Db.GoerSupporters
                    where e.GoerId == id
                    where e.SupporterId == supporterid || e.NoDbEmail == email
                    select e;
            var r = q.SingleOrDefault();
            if (r == null)
            {
                var n = new GoerSupporter()
                {
                    GoerId = id,
                    SupporterId = supporterid,
                    NoDbEmail = email,
                    Active = true,
                    Created = DateTime.Now,
                };
                DbUtil.Db.GoerSupporters.InsertOnSubmit(n);
            }
            else
                r.Active = true;
            DbUtil.Db.SubmitChanges();
            return r.Id;
        }
        public string TestSend()
        {
            var p = DbUtil.Db.LoadPersonById(Util.UserPeopleId.Value);
            try
            {
                DbUtil.Db.Email(p.EmailAddress, p, null, Subject, Body, false);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { error = true, message = ex.Message });
            }
            return JsonConvert.SerializeObject(new { message = "Test Email Sent" });
        }
        public string Send()
        {
            var p = DbUtil.Db.LoadPersonById(PeopleId);
            if (!Subject.HasValue() || !Body.HasValue())
                return "Subject needs some text";

            DbUtil.LogActivity("MissionTripEmail Send {0}".Fmt(PeopleId));

            var glist = (from g in DbUtil.Db.GoerSupporters
                         where g.GoerId == PeopleId
                         where Recipient.Contains(g.Id)
                         select g).ToList();
            var elist = (from g in glist
                         where g.SupporterId == null
                         select g).ToList();
            var plist = (from g in glist
                         where g.SupporterId != null
                         where (g.Unsubscribe ?? false) == false
                         select g.SupporterId ?? 0).ToList();

            if (plist.Any())
            {
                var tag = DbUtil.Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId ?? Util.UserId1, DbUtil.TagTypeId_Emailer);
                DbUtil.Db.ExecuteCommand("delete TagPerson where Id = {0}", tag.Id);
                DbUtil.Db.TagAll(plist, tag);
                DbUtil.Db.CopySession();
                var from = new MailAddress(p.EmailAddress ?? p.EmailAddress2, p.Name);
                DbUtil.Db.CurrentOrgId = OrgId;
                var emailQueue = DbUtil.Db.CreateQueue(p.PeopleId, from, Subject, Body, null, tag.Id, false);
                DbUtil.Db.SendPeopleEmail(emailQueue.Id);
            }
            foreach (var e in elist)
            {
                SendNoDbEmail(p, e);
            }

            try
            {
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { error = true, message = ex.Message });
            }
            return JsonConvert.SerializeObject(new { message = "Test Email Sent" });
        }
        private void SendNoDbEmail(Person goer, GoerSupporter gs)
        {
            var sysFromEmail = Util.SysFromEmail;
            var from = new MailAddress(goer.EmailAddress ?? goer.EmailAddress2, goer.Name);

            try
            {
                var text = Body;
                //var aa = DbUtil.Db.DoReplacements(ref text, goer, null);

                var qs = "OptOut/UnSubscribe/?gid=" + Util.EncryptForUrl("{0}".Fmt(gs.Id));
                var url = Util.URLCombine(DbUtil.Db.CmsHost, qs);
                var link = @"<a href=""{0}"">Unsubscribe</a>".Fmt(url);
                text = text.Replace("{unsubscribe}", link, ignoreCase: true);
                text = text.Replace("%7Bfromemail%7D", from.Address, ignoreCase: true);
                Util.SendMsg(sysFromEmail, DbUtil.Db.CmsHost, from, Subject, text, Util.ToMailAddressList(gs.NoDbEmail), gs.Id, null);
            }
            catch (Exception ex)
            {
                Util.SendMsg(sysFromEmail, DbUtil.Db.CmsHost, from, "sent emails - error", ex.ToString(),
                    Util.ToMailAddressList(from), gs.Id, null);
                throw;
            }
        }
    }
}

/*
 * {name}
 * {first}
 * {last}
 * {occupation}
 * {emailhref}
 * {smallgroup
 * {addsmallgroup
 * {nextmeetingtime}
 * {today}
 * {peopleid}
 * {createaccount}
 * http://votelink 
 * http://sendlink 
 * http://registerlink 
 * http://rsvplink 
 * http://volsublink 
 * http://volreqlink
 * {toemail}
 * {fromemail}
 * {barcode}
 * {cellphone}
 * {campus}
 * {track}
 * {paylink}
 * {amtdue}
 */