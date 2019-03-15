using CmsData;
using CmsData.Codes;
using CmsData.Finance;
using System.Linq;
using System.Threading.Tasks;
using TransactionGateway.ApiModels;
using TransactionGateway.Common;
using UtilityExtensions;

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
            db = CurrentDatabase;
            _pushpay = new PushpayConnection(
                db.GetSetting("PushPayAccessToken", ""),
                db.GetSetting("PushPayRefreshToken", ""),
                CurrentDatabase,
                Configuration.Current.PushpayAPIBaseUrl,
                Configuration.Current.PushpayClientID,
                Configuration.Current.PushpayClientSecret,
                Configuration.Current.OAuth2TokenEndpoint,
                Configuration.Current.TouchpointAuthServer,
                Configuration.Current.OAuth2AuthorizeEndpoint);
            _pushpayPayment = new PushpayPayment(_pushpay, db);
            _resolver = new PushpayResolver(_pushpay, db);

            _merchantHandle = db.Setting("PushpayMerchant", null);
            _givingLink = $"{Configuration.Current.PushpayGivingLinkBase}/{_merchantHandle}";
        }

        public Transaction ConfirmTransaction(Transaction transaction, string paymentToken)
        {
            Payment payment = _pushpayPayment.GetPayment(paymentToken).Result;
            int? PersonId = _resolver.ResolvePersonId(payment.Payer);
            Person person = new Person();
            if (PersonId.HasValue)
            {
                person = db.LoadPersonById(PersonId.Value);
            }
            if (payment != null && !PersonId.HasValue)
            {
                transaction.TransactionId = payment.TransactionId;
                transaction.Name = person.Name;
                transaction.First = person.FirstName;
                transaction.MiddleInitial = person.MiddleName[0].ToString();
                transaction.Last = person.LastName;
                transaction.Suffix = person.SuffixCode;
                transaction.Donate = transaction.Donate;
                transaction.Amtdue = transaction.Amtdue;
                transaction.Amt = payment.Amount.Amount;
                transaction.Emails = person.EmailAddress;
                //transaction.Testing = false;
                transaction.Description = transaction.Description;
                transaction.OrgId = transaction.OrgId;
                transaction.Url = null;
                transaction.Address = person.AddressLineOne;
                transaction.TransactionGateway = "Pushpay";
                transaction.City = person.CityName;
                transaction.State = person.StateCode;
                transaction.Zip = person.ZipCode;
                transaction.DatumId = null;
                transaction.Phone = person.HomePhone;
                transaction.OriginalId = null;
                transaction.Financeonly = null;
                transaction.TransactionDate = Util.Now;
                transaction.PaymentType = payment.PaymentMethodType == "CreditCard" ? PaymentType.CreditCard : PaymentType.Ach;
                transaction.LastFourCC =
                    payment.PaymentMethodType == "CreditCard" ? payment.Card.Reference.Substring(payment.Card.Reference.Length - 4) : null;
                transaction.LastFourACH = null;
                transaction.Approved = true;
            }
            return transaction;
        }
    }
}
