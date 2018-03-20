using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            foreach (var i in q) {
                var item = new SMSItem();

                item.ListID = list.Id;
                item.PeopleID = i.PeopleId;

                if (!string.IsNullOrEmpty(i.CellPhone)) {
                    item.Number = i.CellPhone;
                } else {
                    item.Number = "";
                    item.NoNumber = true;
                }

                if (!i.ReceiveSMS) {
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

            ProcessQueue(list.Id);
        }

        public static void ProcessQueue(int iNewListID)
        {
            var sHost = Util.Host;
            var sSID = GetSid();
            var sToken = GetToken();
            var iListID = iNewListID;

            if (sSID.Length == 0 || sToken.Length == 0) return;

            var ElmahContext = Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current);

            HostingEnvironment.QueueBackgroundWorkItem(ct =>
           {
               var stSID = sSID;
               var stToken = sToken;
               var itListID = iListID;

               try
               {
                   var db = DbUtil.Create(sHost);

                   var smsList = (from e in db.SMSLists
                                  where e.Id == itListID
                                  select e).Single();

                   var smsItems = from e in db.SMSItems
                                  where e.ListID == itListID
                                  select e;

                   var smsGroup = (from e in db.SMSNumbers
                                   where e.GroupID == smsList.SendGroupID
                                   select e).ToList();

                   var iCount = 0;

                   foreach (var item in smsItems)
                   {
                       if (item.NoNumber || item.NoOptIn) continue;

                       var btSent = SendSms(stSID, stToken, smsGroup[iCount].Number, item.Number, smsList.Message);

                       if (btSent)
                       {
                           item.Sent = true;
                           db.SubmitChanges();
                       }

                       iCount++;
                       if (iCount >= smsGroup.Count()) iCount = 0;
                   }
               }
               catch (Exception ex)
               {
                   Debug.WriteLine(ex);
                   ElmahContext.Log(new Elmah.Error(ex));
               }
           });
        }

        private static bool SendSms(string sSID, string sToken, string sFrom, string sTo, string sBody)
        {
            // Needs API keys. Removed to keep private

            TwilioClient.Init(sSID, sToken);

            int retries = 0;

            MessageResource response = MessageResource.Create(new PhoneNumber(sTo), from: new PhoneNumber(sFrom), body: sBody);

            while (retries < 3)
            {
                if (response.Status == MessageResource.StatusEnum.Sent ||
                    response.Status == MessageResource.StatusEnum.Delivered)
                {
                    return true;
                }
                else if (response.Status == MessageResource.StatusEnum.Undelivered ||
                    response.Status == MessageResource.StatusEnum.Failed)
                {
                    Thread.Sleep(500); // wait for Twilio throttling
                    return false;
                }
                else // Accepted || Queued || Sending
                {
                    Thread.Sleep(100); // wait for Twilio throttling
                    retries++;
                    response = MessageResource.Fetch(new FetchMessageOptions(response.Sid), TwilioClient.GetRestClient());
                }
            }

            return false;

            //            var twilio = new TwilioRestClient(sSID, sToken);
            //            var msg = twilio.SendMessage(sFrom, sTo, sBody);
            //            if (msg.Status != "failed") return true;
            //            return false;
        }

		//		public static List<IncomingPhoneNumber> GetNumberList()
		//		{
		//			TwilioClient.Init( GetSid(), GetToken() );
		//			
		//			ResourceSet<IncomingPhoneNumberResource> number = IncomingPhoneNumberResource.Read();
		//			string text = number.First().PhoneNumber.ToString();
		//			
		//			var twilio = new TwilioRestClient( GetSid(), GetToken() );
		//			var numbers = twilio.ListIncomingPhoneNumbers();
		//
		//			return numbers.IncomingPhoneNumbers;
		//		}

		public static List<TwilioNumber> GetUnusedNumberList()
		{
			List<TwilioNumber> available = new List<TwilioNumber>();

			TwilioClient.Init( GetSid(), GetToken() );
			var numbers = IncomingPhoneNumberResource.Read().ToList();

			var used = (from e in DbUtil.Db.SMSNumbers
							select e).ToList();

			for( var iX = numbers.Count - 1; iX > -1; iX-- ) {
				if( used.Any( n => n.Number == numbers[iX].PhoneNumber.ToString() ) )
					numbers.RemoveAt( iX );
			}

			foreach( var item in numbers ) {
				var newNum = new TwilioNumber();
				newNum.Name = item.FriendlyName;
				newNum.Number = item.PhoneNumber.ToString();

				available.Add( newNum );
			}

			return available;
		}

		public static List<UserRole> GetUnassignedPeople( int id )
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

			for( var iX = people.Count() - 1; iX > -1; iX-- ) {
				if( assigned.Any( n => n.UserID == people[iX].UserId ) )
					people.RemoveAt( iX );
			}

			return people;
		}

		public static List<SMSGroup> GetAvailableLists( int iUserID )
		{
			var groups = (from e in DbUtil.Db.SMSGroups
								where e.SMSGroupMembers.Any( f => f.UserID == iUserID )
								select e).ToList();

			return groups;
		}

		public static int GetSendCount( Guid iQBID )
		{
			var q = DbUtil.Db.PeopleQuery( iQBID );

			return (from p in q
						where p.CellPhone != null
						where p.ReceiveSMS
						select p).Count();
		}

		public static bool userSendSMS( int iUserID )
		{
			var role = (from e in DbUtil.Db.Roles
							where e.RoleName == "SendSMS"
							select e).SingleOrDefault();

			if( role == null ) return false;

			var person = from e in DbUtil.Db.UserRoles
							where e.RoleId == role.RoleId
							where e.UserId == iUserID
							select e;

			if( !person.Any() ) return false;

			var groups = from e in DbUtil.Db.SMSGroupMembers
							where e.UserID == iUserID
							select e;

			return groups.Any();
		}

		private static string GetSid()
		{
			return DbUtil.Db.Setting( "TwilioSID", "" );
		}

		private static string GetToken()
		{
			return DbUtil.Db.Setting( "TwilioToken", "" );
		}

		public class TwilioNumber
		{
			public string Number { get; set; }
			public string Name { get; set; }

			public string Description => $"{Name} ({Number})";
		}
	}
}
