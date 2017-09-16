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

            // Only want to look at the most recent Contact
            var latestContact = organization.contactsHad
                .OrderByDescending(x => x.ContactDate)
                .ThenByDescending(x => x.CreatedDate)
                .First();
            if (latestContact.ContactExtras == null) return string.Empty;

            // Calculate and return the lowest health grade from the contact as Org health
            var healthLabel = string.Empty;
            var healthValue = 1;
            foreach (var contactExtra in latestContact.ContactExtras)
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
