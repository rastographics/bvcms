using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Code
{
    public class Payments
    {
        public static void ValidateCreditCardInfo(ModelStateDictionary ModelState, PaymentForm pf)
        {
            if (pf.SavePayInfo && pf.CreditCard.HasValue() && pf.CreditCard.StartsWith("X"))
                return;
            if (!ValidateCard(pf.CreditCard, pf.SavePayInfo))
            {
                ModelState.AddModelError("CreditCard", "invalid card number");
                ModelState.AddModelError("Cardnumber", "invalid card number");
            }
            if (!pf.Expires.HasValue())
            {
                ModelState.AddModelError("Expires", "need expiration date");
                return;
            }

            var exp = DbUtil.NormalizeExpires(pf.Expires);
            if (exp == null)
                ModelState.AddModelError("Expires", "invalid expiration date (MMYY)");
            if (!pf.CCV.HasValue())
            {
                ModelState.AddModelError("CCV", "need Cardcode");
                return;
            }
            var ccvlen = pf.CCV.GetDigits().Length;
            if (ccvlen < 3 || ccvlen > 4)
                ModelState.AddModelError("CCV", "invalid Cardcode");
        }
        public static void ValidateBankAccountInfo(ModelStateDictionary ModelState, string Routing, string Account)
        {
            if (!checkABA(Routing))
                ModelState.AddModelError("Routing", "invalid routing number");
            if (!Account.HasValue())
                ModelState.AddModelError("Account", "need account number");
            else if (!Account.StartsWith("X") && Account.Length <= 4)
                ModelState.AddModelError("Account", "please enter entire account number with any leading zeros");
        }


        private static bool checkABA(string s)
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
            int len = 0;
            for (int i = 0; i < s.Length; i++)
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
            int sum = 0;
            for (int i = len - 1; i >= 0; i--)
                if (i % 2 == len % 2)
                {
                    int n = number[i] * 2;
                    sum += (n / 10) + (n % 10);
                }
                else
                    sum += number[i];
            return sum % 10 == 0;
        }
    }
}