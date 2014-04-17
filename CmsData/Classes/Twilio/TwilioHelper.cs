using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Twilio;
using UtilityExtensions;
using CmsData;

namespace CmsData.Classes.Twilio
{
    public class TwilioHelper
    {
        public const Boolean bTestMode = false;

        public static void QueueSMS(Guid iQBID, int iSendGroupID, string sTitle, string sMessage)
        {
            var q = DbUtil.Db.PeopleQuery(iQBID);

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

                if (i.CellPhone != null && i.CellPhone.Length > 0)
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
                where p.ReceiveSMS == true
                select p;

            var countSMS = qSMS.Count();

            /*
            if (countSMS > 0)
            {
                foreach (var i in qSMS)
                {
                    var item = new SMSItem();

                    item.ListID = list.Id;
                    item.PeopleID = i.PeopleId;
                    item.Number = i.CellPhone;

                    DbUtil.Db.SMSItems.InsertOnSubmit(item);
                }
            }
            */

            // Add counts for SMS, e-Mail and none
            list.SentSMS = countSMS;
            list.SentNone = q.Count() - countSMS;

            DbUtil.Db.SubmitChanges();

            ProcessQueue(list.Id);
        }

        public static void ProcessQueue( int iNewListID )
        {
            string sHost = Util.Host;
            string cmshost = Util.ServerLink();
            string sSID = getSID();
            string sToken = getToken();
            int iListID = iNewListID;

            if (sSID.Length == 0 || sToken.Length == 0) return;

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

                string stSID = sSID;
                string stToken = sToken;
                int itListID = iListID;
                bool btSent = false;

                try
                {
                    var Db = new CMSDataContext(Util.GetConnectionString(sHost));
                    Db.Host = sHost;
                    Db.CmsHost = cmshost;

                    var smsList = (from e in Db.SMSLists
                                   where e.Id == itListID
                                   select e).Single();

                    var smsItems = from e in Db.SMSItems
                                   where e.ListID == itListID
                                   select e;

                    var smsGroup = (from e in Db.SMSNumbers
                                    where e.GroupID == smsList.SendGroupID
                                    select e).ToList();

                    int iCount = 0;

                    foreach (var item in smsItems)
                    {
                        if (item.NoNumber || item.NoOptIn) continue;

                        btSent = sendSMS( stSID, stToken, smsGroup[iCount].Number, item.Number, smsList.Message );

                        if (btSent)
                        {
                            item.Sent = true;
                            Db.SubmitChanges();
                        }

                        iCount++;
                        if( iCount >= smsGroup.Count() ) iCount = 0;
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            });
        }


        public static bool sendSMS( String sSID, String sToken, String sFrom, String sTo, String sBody )
        {
            // Needs API keys. Removed to keep private

            if (bTestMode)
            {
                Debug.WriteLine("Message sending to " + sTo + " from " + sFrom + ": " + sBody + " --- via " + sSID + " / " + sToken);
                return true;
            }
            else
            {
                var twilio = new TwilioRestClient(sSID, sToken);
                var msg = twilio.SendMessage(sFrom, sTo, sBody);
                if (msg.Status != "failed") return true;
                else return false;
            }
        }

        public static List<IncomingPhoneNumber> getNumberList()
        {
            var twilio = new TwilioRestClient( getSID(), getToken() );
            var numbers = twilio.ListIncomingPhoneNumbers();

            return numbers.IncomingPhoneNumbers;
        }

        public static List<TwilioNumber> getUnusedNumberList()
        {
            List<TwilioNumber> available = new List<TwilioNumber>();

            var twilio = new TwilioRestClient(getSID(), getToken());
            var numbers = twilio.ListIncomingPhoneNumbers();

            var used = (from e in DbUtil.Db.SMSNumbers
                        select e).ToList();

            for (var iX = numbers.IncomingPhoneNumbers.Count() - 1; iX > -1; iX--)
            {
                if (used.Where(n => n.Number == numbers.IncomingPhoneNumbers[iX].PhoneNumber).Count() > 0)
                    numbers.IncomingPhoneNumbers.RemoveAt(iX);
            }

            foreach (var item in numbers.IncomingPhoneNumbers)
            {
                var newNum = new TwilioNumber();
                newNum.Name = item.FriendlyName;
                newNum.Number = item.PhoneNumber;

                available.Add(newNum);
            }

            return available;
        }

        public static List<UserRole> getUnassignedPeople(int id)
        {
            var role = (from e in DbUtil.Db.Roles
                        where e.RoleName == "SendSMS"
                        select e).SingleOrDefault();

            // If no results on the role, send back empty list
            if( role == null ) return new List<UserRole>();


            var assigned = (from e in DbUtil.Db.SMSGroupMembers
                            where e.GroupID == id
                            select e).ToList();

            var people = (from e in DbUtil.Db.UserRoles
                          where e.RoleId == role.RoleId
                          select e).ToList();

            for (var iX = people.Count() - 1; iX > -1; iX--)
            {
                if (assigned.Where(n => n.UserID == people[iX].UserId).Count() > 0)
                    people.RemoveAt(iX);
            }

            return people;
        }

        public static List<SMSGroup> getAvailableLists(int iUserID)
        {
            var groups = (from e in DbUtil.Db.SMSGroups
                          where e.SMSGroupMembers.Any( f => f.UserID == iUserID )
                          select e).ToList();

            return groups;
        }

        public static int getSendCount(Guid iQBID)
        {
            var q = DbUtil.Db.PeopleQuery(iQBID);

            return (from p in q
                    where p.CellPhone != null
                    where p.ReceiveSMS == true
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

            if (!groups.Any()) return false;

            return true;
        }

        public static string getSID()
        {
            return DbUtil.Db.Setting("TwilioSID", "");
        }

        public static string getToken()
        {
            return DbUtil.Db.Setting("TwilioToken", "");
        }

        public class TwilioNumber
        {
            public string Number { get; set; }
            public string Name { get; set; }

            public string Description
            {
                get
                {
                    return Name + " (" + Number + ")";
                }
            }
        }
    }
}
