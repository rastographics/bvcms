using System;
using System.Linq;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegModel
    {
        public static string GetTransactionGateway()
        {
            return DbUtil.Db.Setting("TransactionGateway", "notSupported").ToLower();
        }

        private decimal? payAmt;
        public decimal PayAmount()
        {
            if (payAmt.HasValue)
                return payAmt.Value;
            decimal max = 0;
            decimal amt = List.Sum(p => p.AmountToPay());
            if (List.Count > 0) 
                max = List.Max(p => p.org != null ? p.setting.MaximumFee ?? 0 : 0);
            if (max == 0)
                return CachePayAmount(amt);
            var totalother = List.Sum(p => p.TotalOther());
            if (List.Any(p => p.setting.ApplyMaxToOtherFees) && amt > max)
                amt = max;
            else if ((amt - totalother) > max)
                amt = max + totalother;

            return CachePayAmount(amt);
        }

        private decimal CachePayAmount(decimal amt)
        {
            payAmt = amt;
            if (org != null) // Check Family Deposit
            {
                var famdeposit = org.GetExtraValue("FamilyDeposit");
                if (famdeposit != null && famdeposit.IntValue.HasValue)
                {
                    var total = TotalAmount();
                    var famdep = Convert.ToDecimal(famdeposit.IntValue);
                    payAmt = Math.Min(total, famdep);
                }
            }
            return payAmt.Value;
        }


        private decimal? totAmt;
        public decimal TotalAmount()
        {
            if (totAmt.HasValue)
                return totAmt.Value;
            var amt = List.Sum(p => p.TotalAmount());
            var max = List.Max(p => p.org != null ? p.setting.MaximumFee ?? 0 : 0);
            if (max == 0)
                return CacheTotalAmount(amt);
            var totalother = List.Sum(p => p.TotalOther());
            if (List.Any(p => p.setting.ApplyMaxToOtherFees) && amt > max)
                amt = max;
            else if ((amt - totalother) > max)
                amt = max + totalother;
            return CacheTotalAmount(amt);
        }
        private decimal CacheTotalAmount(decimal amt)
        {
            totAmt = amt;
            return amt;
        }

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
            if (!UserPeopleId.HasValue && !List[0].IsNew) 
                return;

            var p = List[0].person;
            if (user != null)
                p = user;
            r.Address = r.payinfo.Address ?? p.PrimaryAddress.Truncate(50);
            r.Address2 = r.payinfo.Address2 ?? p.PrimaryAddress2.Truncate(50);
            r.City = r.payinfo.City ?? p.PrimaryCity;
            r.State = r.payinfo.State ?? p.PrimaryState;
            r.Country = r.payinfo.Country ?? p.PrimaryCountry;
            r.Zip = r.payinfo.Zip ?? p.PrimaryZip;
            r.Phone = Util.PickFirst(r.payinfo.Phone, p.HomePhone, p.CellPhone).FmtFone();
        }

        private void SetTransactionInfoForParents(TransactionInfo r)
        {
            var p = List[0];
            if (p.org == null || !p.setting.AskVisible("AskParents")) 
                return;

            p.Suffix = "";
            var a = (p.fname ?? p.mname ?? "").Trim().Split(' ');
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
                return;
            }
            var p = List[0];
            r.Email = p.EmailAddress;
            r.Suffix = p.Suffix;
            r.First = p.FirstName;
            r.Middle = p.MiddleName;
            r.Last = p.LastName;
            r.Suffix = p.Suffix;
            r.Phone = p.HomePhone ?? p.Phone;
        }

        private TransactionInfo InitializeTransactionInfo()
        {
            var pp = List[0].person;
            var r = new TransactionInfo();
            if (user != null && pp != null)
                r.payinfo = pp.PaymentInfos.FirstOrDefault();
            if (r.payinfo == null)
                r.payinfo = new PaymentInfo {MaskedAccount = "", MaskedCard = ""};
            return r;
        }
    }
}
