using Elmah;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using Twilio;
using Twilio.Base;
using Twilio.Rest.Api.V2010;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using UtilityExtensions;

namespace CmsData.Classes.Twilio
{
    public class TwilioHelper
    {
        public static void QueueSms(object query, int iSendGroupID, string sTitle, string sMessage)
        {
            var q = DbUtil.Db.PeopleQuery2(query);
            QueueSms(q, iSendGroupID, sTitle, sMessage);
        }

        public static void QueueSms(Guid iQBID, int iSendGroupID, string sTitle, string sMessage)
        {
            var q = DbUtil.Db.PeopleQuery(iQBID);
            QueueSms(q, iSendGroupID, sTitle, sMessage);
        }

        public static void QueueSms(IQueryable<Person> q, int iSendGroupID, string sTitle, string sMessage)
        {
            // Create new SMS send list
            var list = new SMSList();

            list.Created = DateTime.Now;
            list.SendAt = DateTime.Now;
            list.SenderID = Util.UserPeopleId ?? 1;
            list.SendGroupID = iSendGroupID;
            list.Title = sTitle;
            list.Message = sMessage;

            DbUtil.Db.SMSLists.InsertOnSubmit(list);
            DbUtil.Db.SubmitChanges();

            // Load all people but tell why they can or can't be sent to

            foreach (var i in q)
            {
                var item = new SMSItem();

                item.ListID = list.Id;
                item.PeopleID = i.PeopleId;

                if (!string.IsNullOrEmpty(i.CellPhone))
                {
                    item.Number = i.CellPhone;
                }
                else
                {
                    item.Number = "";
                    item.NoNumber = true;
                }

                if (!i.ReceiveSMS)
                {
                    item.NoOptIn = true;
                }

                DbUtil.Db.SMSItems.InsertOnSubmit(item);
            }

            DbUtil.Db.SubmitChanges();

            // Check for how many people have cell numbers and want to receive texts
            var qSMS = from p in q
                       where p.CellPhone != null
                       where p.ReceiveSMS
                       select p;

            var countSMS = qSMS.Count();

            // Add counts for SMS, e-Mail and none
            list.SentSMS = countSMS;
            list.SentNone = q.Count() - countSMS;

            DbUtil.Db.SubmitChanges();

            ExecuteCmsTwilio(list.Id);
        }

        private static void ExecuteCmsTwilio(int listID)
        {
            string cmstwilio = System.Web.HttpContext.Current.Server.MapPath("~/bin/cmstwilio.exe");
            Process.Start(new ProcessStartInfo
            {
                FileName = cmstwilio,
                Arguments = $"{listID} --host {Util.Host}",
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(cmstwilio)
            });
        }

        public static void ProcessQueue(int iListID, string sHost)
        {
            var db = DbUtil.Create(sHost);
            var sSID = GetSid(db);
            var sToken = GetToken(db);

            if (sSID.Length == 0 || sToken.Length == 0) return;

            var cb = new SqlConnectionStringBuilder(db.ConnectionString) { InitialCatalog = "ELMAH" };
            var ErrorLog = new SqlErrorLog(cb.ConnectionString) { ApplicationName = "BVCMS" };
            
            var smsList = (from e in db.SMSLists
                           where e.Id == iListID
                           select e).Single();

            var smsItems = from e in db.SMSItems
                           where e.ListID == iListID
                           select e;

            var smsGroup = (from e in db.SMSNumbers
                            where e.GroupID == smsList.SendGroupID
                            select e).ToList();

            var iCount = 0;

            var hostUrl = db.Setting("DefaultHost", "");

            foreach (var item in smsItems)
            {
                try
                {
                    if (item.NoNumber || item.NoOptIn) continue;

                    var callbackUrl = hostUrl.HasValue() ? $"{hostUrl}/WebHook/Twilio/{item.Id}" : null;
                    var response = SendSms(sSID, sToken, smsGroup[iCount].Number, item.Number, smsList.Message, callbackUrl);

                    if (IsSmsFailed(response))
                    {
                        item.ErrorMessage = $"({response.ErrorCode}) {response.ErrorMessage}";
                    }
                    item.Sent = true;
                    item.ResultStatus = $"{response.Status}";

                    db.SubmitChanges();

                    iCount++;
                    if (iCount >= smsGroup.Count()) iCount = 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    ErrorLog.Log(new Error(ex));

                    item.ResultStatus = $"error";
                    item.ErrorMessage = $"{ex.Message}";
                    db.SubmitChanges();
                }
            }
        }

        private static MessageResource SendSms(string sSID, string sToken, string sFrom, string sTo, string sBody, string callbackUrl)
        {
            // Needs API keys. Removed to keep private

            TwilioClient.Init(sSID, sToken);
            Uri callbackUri = callbackUrl.HasValue() ? new Uri(callbackUrl) : null;
            MessageResource response = MessageResource.Create(new PhoneNumber(sTo), from: new PhoneNumber(sFrom), body: sBody, statusCallback: callbackUri);

            if (IsSmsSent(response))
            {
                Console.WriteLine($"Message to {sTo} succeeded with status {response.Status}");
            }
            else if (IsSmsFailed(response))
            {
                Console.WriteLine($"Message to {sTo} failed with status {response.Status} Err:({response.ErrorCode}) {response.ErrorMessage}");
            }
            else // Accepted || Queued || Sending
            {
                Console.WriteLine($"Message to {sTo} is queued with status {response.Status}");
            }

            return response;
        }

        public static bool IsSmsFailed(MessageResource response)
        {
            return response.Status == MessageResource.StatusEnum.Undelivered || response.Status == MessageResource.StatusEnum.Failed;
        }

        public static bool IsSmsSent(MessageResource response)
        {
            return response.Status == MessageResource.StatusEnum.Sent || response.Status == MessageResource.StatusEnum.Delivered;
        }

        public static List<TwilioNumber> GetUnusedNumberList()
        {
            List<TwilioNumber> available = new List<TwilioNumber>();

            TwilioClient.Init(GetSid(DbUtil.Db), GetToken(DbUtil.Db));
            var numbers = IncomingPhoneNumberResource.Read().ToList();

            var used = (from e in DbUtil.Db.SMSNumbers
                        select e).ToList();

            for (var iX = numbers.Count - 1; iX > -1; iX--)
            {
                if (used.Any(n => n.Number == numbers[iX].PhoneNumber.ToString()))
                    numbers.RemoveAt(iX);
            }

            foreach (var item in numbers)
            {
                var newNum = new TwilioNumber();
                newNum.Name = item.FriendlyName;
                newNum.Number = item.PhoneNumber.ToString();

                available.Add(newNum);
            }

            return available;
        }

        public static List<UserRole> GetUnassignedPeople(int id)
        {
            var role = (from e in DbUtil.Db.Roles
                        where e.RoleName == "SendSMS"
                        select e).SingleOrDefault();

            // If no results on the role, send back empty list
            if (role == null) return new List<UserRole>();

            var assigned = (from e in DbUtil.Db.SMSGroupMembers
                            where e.GroupID == id
                            select e).ToList();

            var people = (from e in DbUtil.Db.UserRoles
                          where e.RoleId == role.RoleId
                          select e).ToList();

            for (var iX = people.Count() - 1; iX > -1; iX--)
            {
                if (assigned.Any(n => n.UserID == people[iX].UserId))
                    people.RemoveAt(iX);
            }

            return people;
        }

        public static List<SMSGroup> GetAvailableLists(int iUserID)
        {
            var groups = (from e in DbUtil.Db.SMSGroups
                          where e.SMSGroupMembers.Any(f => f.UserID == iUserID)
                          select e).ToList();

            return groups;
        }

        public static int GetSendCount(Guid iQBID)
        {
            var q = DbUtil.Db.PeopleQuery(iQBID);

            return (from p in q
                    where p.CellPhone != null
                    where p.ReceiveSMS
                    select p).Count();
        }

        public static bool userSendSMS(int iUserID)
        {
            var role = (from e in DbUtil.Db.Roles
                        where e.RoleName == "SendSMS"
                        select e).SingleOrDefault();

            if (role == null) return false;

            var person = from e in DbUtil.Db.UserRoles
                         where e.RoleId == role.RoleId
                         where e.UserId == iUserID
                         select e;

            if (!person.Any()) return false;

            var groups = from e in DbUtil.Db.SMSGroupMembers
                         where e.UserID == iUserID
                         select e;

            return groups.Any();
        }

        private static string GetSid(CMSDataContext db)
        {
            return db.Setting("TwilioSID", "");
        }

        private static string GetToken(CMSDataContext db)
        {
            return db.Setting("TwilioToken", "");
        }

        public class TwilioNumber
        {
            public string Number { get; set; }
            public string Name { get; set; }

            public string Description => $"{Name} ({Number})";
        }
    }
}
