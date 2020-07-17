using CmsData;
using System.Linq;

namespace SharedTestFixtures
{
    public class MockPaymentProcess
    {
        public static void PaymentProcessNullCheck(CMSDataContext db)
        {
            var paymentProcessList = (from p in db.PaymentProcess select p).ToList();
            foreach(var item in paymentProcessList)
            {
                if (item.GatewayAccountId == null)
                {
                    item.GatewayAccountId = 4;
                    db.SubmitChanges();
                }
            }
        }

        public static void ChangePaymentProcessToNull(CMSDataContext db)
        {
            var paymentProcess = (from p in db.PaymentProcess where p.ProcessName == "Recurring Giving" select p).FirstOrDefault();
            paymentProcess.GatewayAccountId = null;
            db.SubmitChanges();
        }
    }
}
