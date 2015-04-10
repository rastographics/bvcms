using System;
using System.Linq;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public partial class OnlineRegModel
    {
        public static string GetTransactionGateway()
        {
            return DbUtil.Db.Setting("TransactionGateway", "serviceu").ToLower();
        }

        public decimal PayAmount()
        {
            decimal max = 0;
            decimal amt = List.Sum(p => p.AmountToPay());
            if (List.Count > 0) 
                max = List.Max(p => p.org != null ? p.setting.MaximumFee ?? 0 : 0);
            if (max == 0)
                return amt;
            var totalother = List.Sum(p => p.TotalOther());
            if (List.Any(p => p.setting.ApplyMaxToOtherFees) && amt > max)
                amt = max;
            else if ((amt - totalother) > max)
                amt = max + totalother;
            var famdeposit = org.GetExtraValue("FamilyDeposit");
            if (famdeposit != null)
                return Convert.ToDecimal(famdeposit.IntValue);
            return amt;
        }

        public decimal TotalAmount()
        {
            var amt = List.Sum(p => p.TotalAmount());
            var max = List.Max(p => p.org != null ? p.setting.MaximumFee ?? 0 : 0);
            if (max == 0)
                return amt;
            var totalother = List.Sum(p => p.TotalOther());
            if (List.Any(p => p.setting.ApplyMaxToOtherFees) && amt > max)
                amt = max;
            else if ((amt - totalother) > max)
                amt = max + totalother;
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
			var p = List[0];
			var pp = p.person;
            var r = new TransactionInfo();

			if (user != null && pp != null)
				r.payinfo = pp.PaymentInfos.FirstOrDefault();
			if (r.payinfo == null)
				r.payinfo = new PaymentInfo { MaskedAccount = "", MaskedCard = "" };

			if (user != null)
			{
				pp = user;
			    r.First = pp.FirstName;
			    r.Last = pp.LastName;
			    r.Middle = pp.MiddleName.Truncate(1);
			    r.Email = pp.EmailAddress;
			    r.Suffix = pp.SuffixCode;
			    r.Phone = (pp.HomePhone ?? pp.CellPhone).FmtFone();
			}
            else
            {
			    r.Email = p.EmailAddress;
			    r.Suffix = p.Suffix;
                r.First = p.FirstName;
                r.Middle = p.MiddleName;
                r.Last = p.LastName;
                r.Suffix = p.Suffix;
                r.Phone = p.HomePhone ?? p.Phone;
            } 

            if (p.org != null && p.setting.AskVisible("AskParents"))
            {
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
            if (UserPeopleId.HasValue || p.IsNew)
            {
                if (pp == null)
                {
                    r.Address = p.AddressLineOne.Truncate(50);
                    r.Address2 = p.AddressLineTwo.Truncate(50);
                    r.City = p.City;
                    r.State = p.State;
                    r.Country = p.Country;
                    r.Zip = p.ZipCode;
                    r.Phone = p.Phone.FmtFone();
                }
                else
                {
                    r.Address = r.payinfo.Address ?? pp.PrimaryAddress.Truncate(50);
                    r.Address2 = r.payinfo.Address2 ?? pp.PrimaryAddress2.Truncate(50);
                    r.City = r.payinfo.City ?? pp.PrimaryCity;
                    r.State = r.payinfo.State ?? pp.PrimaryState;
                    r.Country = r.payinfo.Country ?? pp.PrimaryCountry;
                    r.Zip = r.payinfo.Zip ?? pp.PrimaryZip;
                    r.Phone = Util.PickFirst(r.payinfo.Phone, pp.HomePhone, pp.CellPhone).FmtFone();
                }
            }
            return r;
        }
    }
}
