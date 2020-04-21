using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTestFixtures
{
    public class MockOrganizations
    {
        public static Organization CreateOrganization(CMSDataContext db, string orgName = null, int? campusId = null, int? fromId = null, int? type = null)
        {
            Organization org = null;
            var newOrg = new Organization();
            if (fromId != null)
            {
                org = db.LoadOrganizationById(fromId);
            }
            if (org == null)
            {
                org = db.Organizations.First();
            }

            newOrg.CreatedDate = DateTime.Now;
            newOrg.CreatedBy = 0;
            newOrg.OrganizationName = orgName ?? DatabaseTestBase.RandomString();
            newOrg.EntryPointId = org.EntryPointId;
            newOrg.OrganizationTypeId = type ?? org.OrganizationTypeId;
            newOrg.OrganizationStatusId = 30;
            newOrg.DivisionId = org.DivisionId;
            newOrg.CampusId = campusId;

            db.Organizations.InsertOnSubmit(newOrg);
            db.SubmitChanges();

            return newOrg;
        }
        public static void DeleteOrganization(CMSDataContext db, Organization organization)
        {
            db.Organizations.DeleteOnSubmit(organization);
            db.SubmitChanges();
        }
    }
}
