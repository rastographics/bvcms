using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using CmsData;
using CmsData.Registration;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public class SpecialRegModel
    {
        public static Dictionary<string, string> ParseResults(NameValueCollection elements)
        {
            var q = (from e in elements.AllKeys
                     where e.StartsWith("EV_")
                     select e).ToDictionary(ee => ee.Substring(3), ee => elements[ee]);
            return q;
        }
        public static void SaveResults(int id, int peopleId, Dictionary<string, string> items)
        {
            var org = (from e in DbUtil.Db.Organizations
                       where e.OrganizationId == id
                       select e).FirstOrDefault();

            var person = (from e in DbUtil.Db.People
                          where e.PeopleId == peopleId
                          select e).FirstOrDefault();

            if (person == null) return;
            if (org == null) return;

            var summary = new StringBuilder();

            foreach (var item in items)
            {
                if (item.Value.AllDigits())
                    person.AddEditExtraInt(item.Key, item.Value.ToInt());
                else
                    person.AddEditExtraValue(item.Key, item.Value);
                summary.AppendFormat("{0}: {1}<br>", item.Key, item.Value);
            }

            DbUtil.Db.SubmitChanges();

            List<Person> staffList = DbUtil.Db.StaffPeopleForOrg(id);
            var staff = staffList[0];

            var regSettings = new Settings(org.RegSetting, DbUtil.Db, id);

            var subject = Util.PickFirst(regSettings.Subject, "No subject");
            var body = Util.PickFirst(regSettings.Body, "confirmation email body not found");

            subject = subject.Replace("{org}", org.OrganizationName);

            body = body.Replace("{church}", DbUtil.Db.Setting("NameOfChurch", "church"), ignoreCase: true)
                .Replace("{name}", person.Name, ignoreCase: true)
                .Replace("{date}", DateTime.Now.ToString("d"), ignoreCase: true)
                .Replace("{email}", person.EmailAddress, ignoreCase: true)
                .Replace("{phone}", person.HomePhone.FmtFone(), ignoreCase: true)
                .Replace("{contact}", staff.Name, ignoreCase: true)
                .Replace("{contactemail}", staff.EmailAddress, ignoreCase: true)
                .Replace("{contactphone}", org.PhoneNumber.FmtFone(), ignoreCase: true)
                .Replace("{details}", summary.ToString(), ignoreCase: true);

            DbUtil.Db.Email(staff.FromEmail, person, subject, body);
            DbUtil.Db.Email(person.FromEmail, staff, "Registration completed for {0}".Fmt(org.OrganizationName), "{0} completed {1}<br/><br/>{2}".Fmt(person.Name, org.OrganizationName, summary));
        }
    }
}