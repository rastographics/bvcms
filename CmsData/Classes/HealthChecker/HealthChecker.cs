using System;
using System.Linq;

namespace CmsData.Classes.HealthChecker
{
    public static class HealthChecker
    {
        public static MetricName GetHealthMetric(int orgId)
        {
            var organization = DbUtil.Db.Organizations.SingleOrDefault(x => x.OrganizationId == orgId);
            if (organization == null || organization.contactsHad.Count == 0) return MetricName.UNKNOWN;

            var groupHealth = MetricName.GREEN;
            foreach (var contact in organization.contactsHad)
            {
                if (contact.ContactExtras == null) return MetricName.UNKNOWN;

                foreach (var contactExtra in contact.ContactExtras)
                {
                    if (contactExtra.Metadata == null) continue;

                    Enum.TryParse(contactExtra.Metadata.ToUpper(), out MetricName contactHealth);
                    if (contactHealth > groupHealth) groupHealth = contactHealth;
                }
            }

            return groupHealth;
        }

        public static DateTime? GetLastVisit(int orgId)
        {
            var organization = DbUtil.Db.Organizations.SingleOrDefault(x => x.OrganizationId == orgId);
            if (organization?.contactsHad.Count == 0) return null;

            return organization?.contactsHad.Max(x => x.ContactDate);
        }
    }

    public enum MetricName
    {
        UNKNOWN = 0,
        RED = 3,
        YELLOW = 2,
        GREEN = 1
    }
}
