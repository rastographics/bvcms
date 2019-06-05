using System.Linq;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegModel
    {
        public class TransactionInfo
        {
            public string First { get; set; }
            public string Last { get; set; }
            public string Email { get; set; }
            public string Middle { get; set; }
            public string Suffix { get; set; }
            public string Address { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string Zip { get; set; }
            public string Phone { get; set; }
            public PaymentInfo payinfo { get; set; }
        }
        private OnlineRegPersonModel FirstRegistrant { get { return List[0]; } }
        private Person FirstPerson { get { return FirstRegistrant.person; } }

        private TransactionInfo InitializeTransactionInfo()
        {
            var r = new TransactionInfo();
            var accountId = MultipleGatewayUtils.GetAccount(CurrentDatabase, ProcessType)?.GatewayAccountId ?? 0;

            if (user != null && FirstPerson != null)
            {
                r.payinfo = FirstPerson.PaymentInfo(accountId);
            }
            if (r.payinfo == null)
            {
                r.payinfo = new PaymentInfo { MaskedAccount = "", MaskedCard = "", GatewayAccountId = accountId };
            }
            return r;
        }
        public TransactionInfo GetTransactionInfo()
        {
            if (List.Count == 0)
                return null;
            var r = InitializeTransactionInfo();

			SetTransactionInfo(r);
            SetTransactionInfoAddress(r);
            SetTransactionInfoForParents(r);
            return r;

        }

        private void SetTransactionInfoAddress(TransactionInfo r)
        {

            if (!UserPeopleId.HasValue && !FirstRegistrant.IsNew) 
                return;

            if (FirstPerson == null)
            {
                r.Address = FirstRegistrant.AddressLineOne.Truncate(50);
                r.Address2 = FirstRegistrant.AddressLineTwo.Truncate(50);
                r.City = FirstRegistrant.City;
                r.State = FirstRegistrant.State;
                r.Country = FirstRegistrant.Country;
                r.Zip = FirstRegistrant.ZipCode;
                r.Phone = FirstRegistrant.Phone.FmtFone();
            }
            else
            {
                r.Address = r.payinfo.Address ?? FirstPerson.PrimaryAddress.Truncate(50);
                r.Address2 = r.payinfo.Address2 ?? FirstPerson.PrimaryAddress2.Truncate(50);
                r.City = r.payinfo.City ?? FirstPerson.PrimaryCity;
                r.State = r.payinfo.State ?? FirstPerson.PrimaryState;
                r.Country = r.payinfo.Country ?? FirstPerson.PrimaryCountry;
                r.Zip = r.payinfo.Zip ?? FirstPerson.PrimaryZip;
                r.Phone = Util.PickFirst(r.payinfo.Phone, FirstPerson.HomePhone, FirstPerson.CellPhone).FmtFone();
            }
        }

        private void SetTransactionInfoForParents(TransactionInfo r)
        {
            if (FirstRegistrant.org == null || !FirstRegistrant.setting.AskVisible("AskParents")) 
                return;

            var a = (FirstRegistrant.fname ?? FirstRegistrant.mname ?? "").Trim().Split(' ');
            if (a.Length > 1)
            {
                r.First = a[0];
                r.Last = a[1];
            }
            else
                r.Last = a[0];
        }

        private void SetTransactionInfo(TransactionInfo r)
        {
			if (user != null)
			{
			    r.First = user.FirstName;
			    r.Last = user.LastName;
			    r.Middle = user.MiddleName.Truncate(1);
			    r.Email = user.EmailAddress;
			    r.Suffix = user.SuffixCode;
			    r.Phone = (user.HomePhone ?? user.CellPhone).FmtFone();
			}
            else
            {
			    r.Email = FirstRegistrant.EmailAddress;
                r.First = FirstRegistrant.FirstName;
                r.Last = FirstRegistrant.LastName;
                r.Phone = FirstRegistrant.Phone;
            } 
        }
    }
}
