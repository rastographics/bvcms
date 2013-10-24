using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using CmsData;
using CmsData.Registration;
using UtilityExtensions;

namespace CmsWeb.Models
{
	public class SpecialRegModel
	{
		public static void ParseResults(int id, int peopleId, NameValueCollection elements )
		{
			var org = (from e in DbUtil.Db.Organizations
						  where e.OrganizationId == id
						  select e).FirstOrDefault();

			var person = (from e in DbUtil.Db.People
							  where e.PeopleId == peopleId
							  select e).FirstOrDefault();

			if (person == null) return;
			if (org == null) return;

			var summary = "";

			foreach (var item in elements.AllKeys)
			{
				if (item.StartsWith("EV_"))
				{
					var evName = item.Substring(3);
					int iValue;

					if (Int32.TryParse(elements[item], out iValue))
					{
						person.AddEditExtraInt(evName, iValue);
					}
					else
					{
						person.AddEditExtraValue(evName, elements[item]);
					}

					summary += evName + ": " + elements[item] + "<br>";
				}
			}

			DbUtil.Db.SubmitChanges();

			List<Person> staffList = DbUtil.Db.StaffPeopleForOrg(id);
			var staff = staffList[0];

			var regSettings = new Settings(org.RegSetting, DbUtil.Db, id);

			var subject = Util.PickFirst(regSettings.Subject, "No subject");
			var body = Util.PickFirst(regSettings.Body, "confirmation email body not found");

			subject = subject.Replace("{org}", org.OrganizationName);

			body = body.Replace("{church}", DbUtil.Db.Setting("NameOfChurch", "church"), ignoreCase: true);
			body = body.Replace("{name}", person.Name, ignoreCase: true);
			body = body.Replace("{date}", DateTime.Now.ToString("d"), ignoreCase: true);
			body = body.Replace("{email}", person.EmailAddress, ignoreCase: true);
			body = body.Replace("{phone}", person.HomePhone.FmtFone(), ignoreCase: true);
			body = body.Replace("{contact}", staff.Name, ignoreCase: true);
			body = body.Replace("{contactemail}", staff.EmailAddress, ignoreCase: true);
			body = body.Replace("{contactphone}", org.PhoneNumber.FmtFone(), ignoreCase: true);
			body = body.Replace("{details}", summary, ignoreCase: true);

			DbUtil.Db.Email(staff.FromEmail, person, subject, body);
			DbUtil.Db.Email(person.FromEmail, staff, "Registration completed for {0}".Fmt(org.OrganizationName), "{0} completed {1}<br/><br/>{2}".Fmt(person.Name, org.OrganizationName, summary));
		}
	}
}