using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Code
{
    public class PaymentValidator
    {
        public static void ValidateCreditCardInfo(ModelStateDictionary modelState, PaymentForm pf)
        {
            if (pf.SavePayInfo && pf.CreditCard.HasValue() && pf.CreditCard.StartsWith("X"))
                return;

            if (!ValidateCard(pf.CreditCard, pf.SavePayInfo))
            {
                modelState.AddModelError("CreditCard", "The card number is invalid.");
            }
            if (!pf.Expires.HasValue())
            {
                modelState.AddModelError("Expires", "The expiration date is required.");
                return;
            }

            var exp = DbUtil.NormalizeExpires(pf.Expires);
            if (exp == null)
                modelState.AddModelError("Expires", "The expiration date format is invalid (MMYY).");

            if (!pf.CVV.HasValue())
            {
                modelState.AddModelError("CVV", "The CVV is required.");
                return;
            }

            var cvvlen = pf.CVV.GetDigits().Length;
            if (cvvlen < 3 || cvvlen > 4)
                modelState.AddModelError("CVV", "The CVV must be a 3 or 4 digit number.");
        }

        public static void ValidateBankAccountInfo(ModelStateDictionary modelState, string routing, string account)
        {
            if (!CheckABA(routing))
                modelState.AddModelError("Routing", "The routing number is invalid.");

            if (!account.HasValue())
                modelState.AddModelError("Account", "The account number is invalid.");
            else if (!account.StartsWith("X") && account.Length <= 4)
                modelState.AddModelError("Account", "Please enter entire account number with any leading zeros.");
        }

        private static bool CheckABA(string s)
        {
            if (!s.HasValue())
                return false;
            if (s.StartsWith("X"))
                return true;
            var t = s.GetDigits();
            if (t.Length != 9)
                return false;
            var n = 0;
            for (var i = 0; i < t.Length; i += 3)
            {
                n += int.Parse(t.Substring(i, 1)) * 3
                    + int.Parse(t.Substring(i + 1, 1)) * 7
                    + int.Parse(t.Substring(i + 2, 1));
            }
            if (n != 0 && n % 10 == 0)
                return true;
            return false;
        }

        public static bool ValidateCard(string s, bool savedCard)
        {
            if (!s.HasValue())
                return false;
            if (s.StartsWith("X") && savedCard)
                return true;
            var number = new int[16];
            var len = 0;
            for (var i = 0; i < s.Length; i++)
            {
                if (char.IsDigit(s, i))
                {
                    if (len == 16)
                        return false;
                    number[len++] = s[i] - '0';
                }
            }

            switch (s[0])
            {
                case '5':
                    if (len != 16)
                        return false;
                    if (number[1] == 0 || number[1] > 5)
                        return false;
                    break;

                case '4':
                    if (len != 16 && len != 13)
                        return false;
                    break;

                case '3':
                    if (len != 15)
                        return false;
                    if ((number[1] != 4 && number[1] != 7))
                        return false;
                    break;

                case '6':
                    if (len != 16)
                        return false;
                    if (number[1] != 0 || number[2] != 1 || number[3] != 1)
                        return false;
                    break;
            }
            var sum = 0;
            for (var i = len - 1; i >= 0; i--)
                if (i % 2 == len % 2)
                {
                    var n = number[i] * 2;
                    sum += (n / 10) + (n % 10);
                }
                else
                    sum += number[i];
            return sum % 10 == 0;
        }
    }
}