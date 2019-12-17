using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTestFixtures
{
    public class MockGateways
    {
        public static GatewayAccount CreateSaveGatewayAccount(CMSDataContext db, GatewayTypes gatewayType)
        {
            var gatewayAccount = new GatewayAccount();
            var gateway = db.Gateways.FirstOrDefault(p => p.GatewayId == (int)gatewayType);
            if (gateway != null)
            {
                gatewayAccount.GatewayAccountName = DatabaseTestBase.RandomString();
                gatewayAccount.GatewayId = gateway.GatewayId;
                db.GatewayAccount.InsertOnSubmit(gatewayAccount);
                db.SubmitChanges();
                var template = db.GatewayConfigurationTemplate.Where(p => p.GatewayId == gateway.GatewayId);
                CreateSaveGatewayDetails(db, gatewayAccount, template);                             
                db.SubmitChanges();                
            }
            return gatewayAccount;
        }

        public static void CreateSaveGatewayDetails(CMSDataContext db, GatewayAccount gatewayAccount, IQueryable<GatewayConfigurationTemplate> template)
        {
            foreach (var item in template)
            {
                var detail = new GatewayDetails
                {
                    GatewayAccountId = gatewayAccount.GatewayAccountId,
                    GatewayDetailName = item.GatewayDetailName,
                    GatewayDetailValue = item.IsBoolean ? "true" : DatabaseTestBase.RandomString(),
                    IsBoolean = item.IsBoolean
                };
                db.GatewayDetails.InsertOnSubmit(detail);
                db.SubmitChanges();
            }            
        }
    }
}
