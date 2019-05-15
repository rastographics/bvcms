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
        public string _merchantHandle;

        public PushpayPayment(PushpayConnection Pushpay, CMSDataContext db, PaymentProcessTypes processType)
        {
            _pushpay = Pushpay;
            _db = db;
            _merchantHandle = MultipleGatewayUtils.Setting("PushpayMerchant", "", (int)processType);
        }

        public async Task<Payment> GetPayment(string paymentToken)
        {
            IEnumerable<Merchant> merchants = await _pushpay.SearchMerchants(_merchantHandle);
            return await _pushpay.GetPayment(merchants.FirstOrDefault().Key, paymentToken);
        }

        public async Task<RecurringPayment> GetRecurringPayment(string paymentToken)
        {
            IEnumerable<Merchant> merchants = await _pushpay.SearchMerchants(_merchantHandle);
            return await _pushpay.GetRecurringPayment(merchants.FirstOrDefault().Key, paymentToken);
        }

        public async Task<IEnumerable<RecurringPayment>> GetRecurringPaymentsForAPayer(string payerKey)
        {
            if (payerKey == null)
            {
                return null;
            }
            return await _pushpay.GetRecurringPaymentsForAPayer(payerKey);
        }

        public async Task<Fund> GetFundForRegistration(string OrganizationName)
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
