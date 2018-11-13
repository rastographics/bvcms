using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using System;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public class ManagePledgesModel
    {
        public int pid { get; set; }
        public int orgid { get; set; }
        private Person _Person;
        public string ThankYouMessage { get; set; }
        public Person person
        {
            get
            {
                if (_Person == null)
                {
                    _Person = DbUtil.Db.LoadPersonById(pid);
                }

                return _Person;
            }
        }
        private Organization _organization;
        public Organization Organization
        {
            get
            {
                if (_organization == null)
                {
                    _organization = DbUtil.Db.Organizations.Single(d => d.OrganizationId == orgid);
                }

                return _organization;
            }
        }
        public decimal? pledge { get; set; }

        public ManagePledgesModel()
        {

        }
        public class PledgeInfo
        {
            public decimal Pledged { get; set; }
            public decimal Given { get; set; }
        }

        private Settings setting;
        public Settings Setting => setting ?? (setting = DbUtil.Db.CreateRegistrationSettings(orgid));

        public PledgeInfo GetPledgeInfo()
        {
            var RRTypes = new int[] { 6, 7 };
            var q = from c in DbUtil.Db.Contributions
                    where c.FundId == Setting.DonationFundId
                    where c.PeopleId == pid
                    where !RRTypes.Contains(c.ContributionTypeId)
                    group c by pid into g
                    select new PledgeInfo
                    {
                        Pledged = g.Where(c => c.ContributionTypeId == ContributionTypeCode.Pledge).Sum(c => c.ContributionAmount) ?? 0,
                        Given = g.Where(c => c.ContributionTypeId != ContributionTypeCode.Pledge).Sum(c => c.ContributionAmount) ?? 0,
                    };
            return q.SingleOrDefault() ?? new PledgeInfo { Given = 0m, Pledged = 0m };
        }
        public ManagePledgesModel(int pid, int orgid)
        {
            this.pid = pid;
            this.orgid = orgid;
        }
        public void Confirm()
        {
            var staff = DbUtil.Db.StaffPeopleForOrg(orgid);

            var desc = $"{person.Name}; {person.PrimaryAddress}; {person.PrimaryCity}, {person.PrimaryState} {person.PrimaryZip}";

            person.PostUnattendedContribution(DbUtil.Db,
                pledge ?? 0,
                Setting.DonationFundId,
                desc, pledge: true);

            var pi = GetPledgeInfo();
            var body = Setting.Body ?? "no confirmation body found";
            body = body.Replace("{amt}", pi.Pledged.ToString("N2"), ignoreCase: true)
                .Replace("{org}", Organization.OrganizationName, ignoreCase: true)
                .Replace("{first}", person.PreferredName, ignoreCase: true);
            DbUtil.Db.EmailRedacted(staff[0].FromEmail, person, Setting.Subject, body);

            DbUtil.Db.Email(person.FromEmail, staff, "Online Pledge",
                $@"{person.Name} made a pledge to {Organization.OrganizationName}");

            ThankYouMessage = GetThankYouMessage(@"
  <h2>Confirmation</h2>
  <p>
    Thank you {first}, for making your pledge to {org}<br/>
    You should receive a confirmation email shortly.
  </p>
");
        }
        public void Log(string action)
        {
            DbUtil.LogActivity("OnlineReg ManagePledge " + action, orgid, pid);
        }
        private string GetThankYouMessage(string def)
        {
            var msg = Util.PickFirst(setting.ThankYouMessage, def)
                .Replace("{first}", person.PreferredName, ignoreCase: true)
                .Replace("{org}", Organization.OrganizationName, ignoreCase: true);
            return msg;
        }
        public bool NotAvailable()
        {
            var dt = DateTime.Now;
            var dt1 = DateTime.Parse("1/1/1900");
            var dt2 = DateTime.Parse("1/1/2200");
            return Organization.RegistrationClosed == true
                   || Organization.OrganizationStatusId == OrgStatusCode.Inactive
                   || dt < (Organization.RegStart ?? dt1)
                   || (dt > (Organization.RegEnd ?? dt2));
        }
    }
}
