/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace UtilityExtensions
{
    public static partial class Util
    {
        public static string EmailHref(this string addr, string name)
        {
            if (!addr.HasValue())
            {
                return "";
            }

            return $"mailto:{addr}";
        }
        public static string FullEmail(string email, string name)
        {
            if (email.HasValue())
            {
                if (!name.HasValue() || name.Contains("?"))
                {
                    return email;
                }
                else
                {
                    var na = Regex.Replace(name, @";|,|\""", "");
                    var a = email.SplitStr(",;");
                    var q = from ad in a
                            select na + " <" + ad.Trim() + ">";
                    return string.Join(";", q);
                }
            }

            return string.Empty;
        }
        public static List<MailAddress> DistinctEmails(this List<MailAddress> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                {
                    for (var j = i + 1; j < list.Count; j++)
                    {
                        if (list[j] != null)
                        {
                            if (string.Compare(list[i].Address, list[j].Address, ignoreCase: true) == 0)
                            {
                                list[j] = null;
                            }
                        }
                    }
                }
            }

            return list.Where(ll => ll != null).ToList();
        }
        private class MailAddressComparer : IEqualityComparer<MailAddress>
        {
            public bool Equals(MailAddress x, MailAddress y)
            {
                if (x == null)
                {
                    return y == null;
                }

                if (y == null)
                {
                    return false;
                }

                var eq = string.Compare(x.Address, y.Address, ignoreCase: true) == 0;
                return eq;
            }
            public int GetHashCode(MailAddress obj)
            {
                return obj.Address.ToLower().GetHashCode();
            }
        }
        public static List<MailAddress> ToMailAddressList(string addresses)
        {
            var list = new List<MailAddress>();
            foreach (var ad in addresses.SplitStr(",;"))
            {
                AddGoodAddress(list, ad);
            }

            return list;
        }
        public static List<MailAddress> ToMailAddressList(string address, string name)
        {
            return ToMailAddressList(Util.TryGetMailAddress(address, name));
        }
        public static List<MailAddress> ToMailAddressList(MailAddress ma)
        {
            return new List<MailAddress> { ma };
        }

        public static void AddGoodAddress(List<MailAddress> list, string emailAddress)
        {
            MailAddress mailAddress;
            if (Util.TryGetMailAddress(emailAddress, out mailAddress))
            {
                if (!list.Any(mm => mm.Address == emailAddress))
                {
                    list.Add(mailAddress);
                }
            }
        }

        public static string EmailAddressListToString(this List<MailAddress> list)
        {
            var addrs = string.Join(", ", list.Select(tt => tt.ToString()));
            return addrs;
        }
        public static List<MailAddress> SendErrorsTo()
        {
            var a = ConfigurationManager.AppSettings["senderrorsto"];
            return EmailAddressListFromString(a.HasValue() ? a : AdminMail);
        }
        public static List<MailAddress> EmailAddressListFromString(string addresses)
        {
            var a = addresses.SplitStr(",;");
            var list = new List<MailAddress>();
            foreach (var ad in a)
            {
                AddGoodAddress(list, ad);
            }

            return list;
        }

        public static bool TryGetMailAddress(string address, out MailAddress mailAddress)
        {
            var valid = true;
            mailAddress = null;
            try
            {
                mailAddress = TryGetMailAddress(address);
                valid = mailAddress != null;
            }
            catch
            {
                valid = false;
            }
            return valid;
        }

        public static MailAddress TryGetMailAddress(string address)
        {
            if (address.HasValue())
            {
                address = address.Trim();
            }

            try
            {
                if (!address.HasValue())
                {
                    return null;
                }

                var ma = new MailAddress(address);
                if (ValidEmail(ma.Address))
                {
                    return ma;
                }
            }
            catch (Exception)
            {
                throw new Exception($"bad email address: {address}");
            }
            return null;
        }
        public static MailAddress TryGetMailAddress(string address, string name)
        {
            if (address.HasValue())
            {
                address = address.Trim();
            }

            if (name.HasValue())
            {
                name = name.Replace("\"", "");
            }

            if (ValidEmail(address))
            {
                return Util.FirstAddress(address, name);
            }

            return null;
        }
        public static bool ValidEmail(string email)
        {
            if (!email.HasValue())
            {
                return false;
            }

            if (email.Contains(" "))
            {
                return false;
            }

            var re1 = new Regex(@"^(.*\b(?=\w))\b[A-Z0-9._%+-]+(?<=[^.])@[A-Z0-9.-]+(?<!\.)\.[A-Z]{2,}\b\b(?!\w)$", RegexOptions.IgnoreCase);
            var re2 = new Regex(@"^[A-Z0-9._%+-]+(?<=[^.])@[A-Z0-9.-]+(?<!\.)\.[A-Z]{2,}$", RegexOptions.IgnoreCase);
            var a = email.SplitStr(",;");
            foreach (var m in a)
            {
                var b = re1.IsMatch(m) || re2.IsMatch(m);
                if (b)
                {
                    return true; // at least one good email address
                }
            }
            return false;
        }
        public static MailAddress FirstAddress(string addrs)
        {
            return FirstAddress(addrs, null);
        }
        public static MailAddress FirstAddress(string addrs, string name)
        {
            if (!addrs.HasValue())
            {
                addrs = AdminMail;
            }

            var a = addrs.SplitStr(",;");
            try
            {
                var ma = new MailAddress(a[0]);
                if (name.HasValue())
                {
                    return new MailAddress(ma.Address, name);
                }

                return ma;
            }
            catch (Exception)
            {
                if (!ValidEmail(AdminMail))
                {
                    throw new Exception($"bad AdminMail address <{AdminMail}>");
                }

                if (name.HasValue())
                {
                    return new MailAddress(AdminMail, name);
                }

                return new MailAddress(AdminMail);
            }
        }

        public static MailAddress FirstAddress2(string addrs, string name)
        {
            if (!addrs.HasValue())
            {
                addrs = AdminMail;
            }

            var a = addrs.SplitStr(",;");
            try
            {
                return new MailAddress(a[0], name);
            }
            catch (Exception)
            {
                if (!ValidEmail(AdminMail))
                {
                    throw new Exception($"bad AdminMail address <{AdminMail}>");
                }

                if (name.HasValue())
                {
                    return new MailAddress(AdminMail, name);
                }

                return new MailAddress(AdminMail);
            }
        }
        public static string ObscureEmail(string email)
        {
            if (!ValidEmail(email))
            {
                return email;
            }

            var a = email.Split('@');
            var rest = new string('.', 6);
            return a[0].Substring(0, 1) + rest + "@" + a[1];
        }
        public static string ObscureAccount(string acct)
        {
            var rest = new string('x', acct.Length - 4);
            return rest + acct.Substring(acct.Length - 4);
        }
        private const string STR_AdminMail = "AdminMail";
        public static string AdminMail
        {
            get
            {
                var tag = ConfigurationManager.AppSettings["senderrorsto"];
                if (HttpContextFactory.Current != null)
                {
                    if ((HttpContextFactory.Current.Items[STR_AdminMail] as string).HasValue())
                    {
                        tag = HttpContextFactory.Current.Items[STR_AdminMail].ToString();
                    }
                }

                return tag;
            }
            set
            {
                if (HttpContextFactory.Current != null)
                {
                    HttpContextFactory.Current.Items[STR_AdminMail] = value;
                }
            }
        }
        public static void AddAddr(this MailMessage msg, MailAddress a)
        {
            if (IsInRoleEmailTest)
            {
                a = new MailAddress(UserEmail, a.DisplayName + " (test)");
            }

            msg.To.Add(a);
        }
        private const string STR_UserEmail = "UserEmail";

        public static string UserEmail
        {
            get
            {
                string email = null;
                if (HttpContextFactory.Current != null)
                {
                    if (HttpContextFactory.Current.Session != null)
                    {
                        if (HttpContextFactory.Current.Session[STR_UserEmail] != null)
                        {
                            email = HttpContextFactory.Current.Session[STR_UserEmail] as String;
                        }
                    }
                }
                else
                {
                    email = (string)Thread.GetData(Thread.GetNamedDataSlot("UserEmail"));
                }

                return email;
            }
            set
            {
                if (HttpContextFactory.Current != null)
                {
                    if (HttpContextFactory.Current.Session != null)
                    {
                        HttpContextFactory.Current.Session[STR_UserEmail] = value;
                    }
                }
                else
                {
                    Thread.SetData(Thread.GetNamedDataSlot(STR_UserEmail), value);
                }
            }
        }
        public static bool IsInRoleEmailTest
        {
            get
            {
                if (HttpContextFactory.Current != null)
                {
                    return HttpContextFactory.Current.User.IsInRole("EmailTest") || ((bool?)HttpContextFactory.Current.Session?["IsInRoleEmailTest"] ?? false);
                }

                return (bool?)Thread.GetData(Thread.GetNamedDataSlot("IsInRoleEmailTest")) ?? false;
            }
            set
            {
                if (HttpContextFactory.Current != null)
                {
                    if (HttpContextFactory.Current.Session != null)
                    {
                        HttpContextFactory.Current.Session["IsInRoleEmailTest"] = value;
                    }
                }
                else
                {
                    Thread.SetData(Thread.GetNamedDataSlot("IsInRoleEmailTest"), value);
                }
            }
        }
    }
}

