using System;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.OnlineReg.Models;
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

        public static Areas.Giving.Models.Message ValidateCreditCardInfo(string cardNumber = "", string cvv = "", string expiresMonth = "", string expiresYear = "")
        {
            if (!cardNumber.HasValue())
            {
                return Areas.Giving.Models.Message.successMessage("The credit card number is required.", Areas.Giving.Models.Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (!ValidateCard(cardNumber))
            {
                return Areas.Giving.Models.Message.successMessage("Credit card number invalid.", Areas.Giving.Models.Message.API_ERROR_PAYMENT_METHOD_CREDIT_CARD_NUM_INVALID);
            }
            if (!cvv.HasValue())
            {
                return Areas.Giving.Models.Message.successMessage("The CVV is required.", Areas.Giving.Models.Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (!expiresMonth.HasValue())
            {
                return Areas.Giving.Models.Message.successMessage("The expiration month is required.", Areas.Giving.Models.Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (!expiresYear.HasValue())
            {
                return Areas.Giving.Models.Message.successMessage("The expiration year is required.", Areas.Giving.Models.Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }

            var today = DateTime.Now;
            if(Convert.ToInt32(expiresYear) < today.Year)
            {
                return Areas.Giving.Models.Message.successMessage("Credit card has expired.", Areas.Giving.Models.Message.API_ERROR_PAYMENT_METHOD_CREDIT_CARD_EXPIRED);
            }
            if (Convert.ToInt32(expiresYear) == today.Year)
            {
                if (Convert.ToInt32(expiresMonth) < today.Month)
                {
                    return Areas.Giving.Models.Message.successMessage("Credit card has expired.", Areas.Giving.Models.Message.API_ERROR_PAYMENT_METHOD_CREDIT_CARD_EXPIRED);
                }
            }

            var cvvLength = cvv.GetDigits().Length;
            if (cvvLength < 3 || cvvLength > 4)
            {
                return Areas.Giving.Models.Message.successMessage("The CVV must be a 3 or 4 digit number.", Areas.Giving.Models.Message.API_ERROR_PAYMENT_METHOD_CREDIT_CARD_NUM_INVALID);
            }
            return Areas.Giving.Models.Message.successMessage("Credit card is valid.", Areas.Giving.Models.Message.API_ERROR_NONE);
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

        public static Areas.Giving.Models.Message ValidateBankAccountInfo(string bankAccount = "", string bankRouting = "")
        {
            if (!bankAccount.HasValue())
            {
                return Areas.Giving.Models.Message.successMessage("The bank account number is required.", Areas.Giving.Models.Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if (!bankRouting.HasValue())
            {
                return Areas.Giving.Models.Message.successMessage("The bank routing number is required.", Areas.Giving.Models.Message.API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY);
            }
            if(double.TryParse(bankAccount, out _) == false)
            {
                return Areas.Giving.Models.Message.successMessage("Bank account number must be numbers only.", Areas.Giving.Models.Message.API_ERROR_PAYMENT_METHOD_BANK_ACCOUNT_NUM_INVALID);
            }
            if (double.TryParse(bankRouting, out _) == false)
            {
                return Areas.Giving.Models.Message.successMessage("Bank routing number must be numbers only.", Areas.Giving.Models.Message.API_ERROR_PAYMENT_METHOD_BANK_ROUTING_NUM_INVALID);
            }
            if (!CheckBank(bankRouting))
            {
                return Areas.Giving.Models.Message.successMessage("Bank routing number is invald.", Areas.Giving.Models.Message.API_ERROR_PAYMENT_METHOD_BANK_ROUTING_NUM_INVALID);
            }
            return Areas.Giving.Models.Message.successMessage("Bank is valid.", Areas.Giving.Models.Message.API_ERROR_NONE);
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

        private static bool CheckBank(string routing)
        {
            if (!routing.HasValue())
                return false;
            var t = routing.GetDigits();
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

        public static bool ValidateCard(string cardNum)
        {
            if (!cardNum.HasValue())
            {
                return false;
            }
            var number = new int[16];
            var len = 0;
            for (var i = 0; i < cardNum.Length; i++)
            {
                if (char.IsDigit(cardNum, i))
                {
                    if (len == 16)
                        return false;
                    number[len++] = cardNum[i] - '0';
                }
            }

            switch (cardNum[0])
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
            {
                if (i % 2 == len % 2)
                {
                    var n = number[i] * 2;
                    sum += (n / 10) + (n % 10);
                }
                else
                {
                    sum += number[i];
                }
            }
            return sum % 10 == 0;
        }
    }
}
