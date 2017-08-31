using System;
using System.Linq;

namespace CmsData.Classes.HealthChecker
{
    public static class HealthChecker
    {
        public static string GetHealthLabel(int orgId)
        {
            var organization = DbUtil.Db.Organizations.SingleOrDefault(x => x.OrganizationId == orgId);
            if (organization == null || organization.contactsHad.Count == 0) return string.Empty;

            var healthLabel = string.Empty;
            var healthValue = 1;
            foreach (var contact in organization.contactsHad)
            {
                if (contact.ContactExtras == null) return string.Empty;

                foreach (var contactExtra in contact.ContactExtras)
                {
                    if (contactExtra.Metadata == null) continue;

                    var groupHealth = contactExtra.Metadata.Split(',');
                    if (groupHealth.Length < 2) continue;

                    var contactLabel = groupHealth[0];
                    var contactValue = int.Parse(groupHealth[1]);
                    if (contactValue >= healthValue)
                    {
                        healthLabel = contactLabel;
                        healthValue = contactValue;
                    }
                }
            }

            return healthLabel;
        }

        public static DateTime? GetLastVisit(int orgId)
        {
            var organization = DbUtil.Db.Organizations.SingleOrDefault(x => x.OrganizationId == orgId);
            if (organization?.contactsHad.Count == 0) return null;

            return organization?.contactsHad.Max(x => x.ContactDate);
        }
    }
}
