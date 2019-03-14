using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionGateway
{
    public class PushpayGateway : IRGateway
    {
        private readonly CMSDataContext db;

        private PushpayConnection _pushpay;
        private PushpayPayment _pushpayPayment;
        private PushpayResolver _resolver;
        private string _givingLink;
        private string _merchantHandle;

        public string GatewayType => "Pushpay";

        public PushpayGateway(CMSDataContext CurrentDatabase, bool testing)
        {
            _pushpay = new PushpayConnection(
                CurrentDatabase.GetSetting("PushPayAccessToken", ""),
                CurrentDatabase.GetSetting("PushPayRefreshToken", ""),
                CurrentDatabase,
                Configuration.Current.PushpayAPIBaseUrl,
                Configuration.Current.PushpayClientID,
                Configuration.Current.PushpayClientSecret,
                Configuration.Current.OAuth2TokenEndpoint,
                Configuration.Current.TouchpointAuthServer,
                Configuration.Current.OAuth2AuthorizeEndpoint);
            _pushpayPayment = new PushpayPayment(_pushpay, CurrentDatabase);
            _resolver = new PushpayResolver(_pushpay, CurrentDatabase);

            _merchantHandle = CurrentDatabase.Setting("PushpayMerchant", null);
            _givingLink = $"{Configuration.Current.PushpayGivingLinkBase}/{_merchantHandle}";
        }

        public Transaction CreateTransaction(string paymentToken)
        {
            return null;
        }
    }
}
