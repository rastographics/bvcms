using CmsData;
using System.Linq;

namespace SharedTestFixtures
{
    public class MockPaymentProcess
    {
        public static string PaymentProcessNullCheck(CMSDataContext db)
        {
            var paymentProcess = (from p in db.PaymentProcess where p.ProcessName == "Recurring Giving" select p).FirstOrDefault();
            var actionTaken = "";
            if (paymentProcess.GatewayAccountId == null)
            {
                paymentProcess.GatewayAccountId = 4;
                db.SubmitChanges();
                actionTaken = "changed";
            }
            else
            {
                actionTaken = "good";
            }
            return actionTaken;
        }

        public static void ChangePaymentProcessToNull(CMSDataContext db)
        {
            var paymentProcess = (from p in db.PaymentProcess where p.ProcessName == "Recurring Giving" select p).FirstOrDefault();
            paymentProcess.GatewayAccountId = null;
            db.SubmitChanges();
        }
    }
}
