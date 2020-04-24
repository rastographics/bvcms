using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionGateway.ApiModels;

namespace TransactionGateway
{
    public class PushpayPayment
    {
        private PushpayConnection _pushpay;
        private CMSDataContext _db;
        public string _defaultMerchantHandle;

        public PushpayPayment(PushpayConnection Pushpay, CMSDataContext db, PaymentProcessTypes processType)
        {
            _pushpay = Pushpay;
            _db = db;
            _defaultMerchantHandle = MultipleGatewayUtils.Setting(db, "PushpayMerchant", "", (int)processType);
        }

        public async Task<Payment> GetPayment(string paymentToken, string merchantHandle)
        {
            IEnumerable<Merchant> merchants = await _pushpay.SearchMerchants(merchantHandle);
            var payment = await _pushpay.GetPayment(merchants.FirstOrDefault().Key, paymentToken);
            if (payment == null)
                throw new Exception("Payment not found");

            return payment;
        }

        public async Task<RecurringPayment> GetRecurringPayment(string paymentToken, string merchantHandle)
        {
            IEnumerable<Merchant> merchants = await _pushpay.SearchMerchants(merchantHandle);
            var recurringPayment = await _pushpay.GetRecurringPayment(merchants.FirstOrDefault().Key, paymentToken);
            if (recurringPayment?.Schedule == null)
                throw new Exception("Recurring payment not found");

            return recurringPayment;
        }

        public async Task<IEnumerable<RecurringPayment>> GetRecurringPaymentsForAPayer(string payerKey)
        {
            if (payerKey == null)
            {
                return null;
            }
            return await _pushpay.GetRecurringPaymentsForAPayer(payerKey);
        }

        public async Task<Fund> GetFundForOrganizationName(string OrganizationName)
        {
            var pushpayOrganizations = await _pushpay.GetOrganizations();
            Fund fund = new Fund();
            foreach (var item in pushpayOrganizations)
            {
                var key = item.Key;
                var funds = await _pushpay.GetFundsForOrganization(key);
                fund = funds.Items.Where(f => f.Name == OrganizationName).FirstOrDefault();
                if (fund == null)
                {
                    //create fund                    
                }
            }
            return fund;
        }
    }
}
