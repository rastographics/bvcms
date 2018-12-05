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
        public SpecialRegModel() { }
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
                else if (item.Value == "true" || item.Value == "false")
                    person.AddEditExtraBool(item.Key, item.Value.ToBool());
                else
                    person.AddEditExtraCode(item.Key, item.Value);
                summary.AppendFormat("{0}: {1}<br>", item.Key, item.Value);
            }

            DbUtil.Db.SubmitChanges();

            var staffList = DbUtil.Db.StaffPeopleForOrg(id);
            var staff = staffList[0];

            var regSettings = DbUtil.Db.CreateRegistrationSettings(id);

            var subject = Util.PickFirst(regSettings.Subject, "No subject");
            var body = Util.PickFirst(regSettings.Body, "confirmation email body not found");

            subject = subject.Replace("{org}", org.OrganizationName);

            body = body.Replace("{church}", DbUtil.Db.Setting("NameOfChurch", "church"), true)
                .Replace("{name}", person.Name, true)
                .Replace("{date}", Util.Now.ToString("d"), true)
                .Replace("{email}", person.EmailAddress, true)
                .Replace("{phone}", person.HomePhone.FmtFone(), true)
                .Replace("{contact}", staff.Name, true)
                .Replace("{contactemail}", staff.EmailAddress, true)
                .Replace("{contactphone}", org.PhoneNumber.FmtFone(), true)
                .Replace("{details}", summary.ToString(), true);

            DbUtil.Db.Email(staff.FromEmail, person, subject, body);
            DbUtil.Db.Email(person.FromEmail, staff, $"Registration completed for {org.OrganizationName}", $"{person.Name} completed {org.OrganizationName}<br/><br/>{summary}");
        }
    }
}
