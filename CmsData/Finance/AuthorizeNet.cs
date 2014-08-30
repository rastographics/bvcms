using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UtilityExtensions;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace CmsData
{
    internal class AuthorizeNet : IGateway
    {
        readonly XNamespace ns = "AnetApi/xml/v1/schema/AnetApiSchema.xsd";
        const string produrl = "https://api.authorize.net/xml/v1/request.api";
        const string testurl = "https://apitest.authorize.net/xml/v1/request.api";
        readonly string url;
        readonly string login;
        readonly string key;
        readonly bool testing;
        readonly CMSDataContext db;

        public string GatewayType { get { return "AuthorizeNet"; } }

        public AuthorizeNet(CMSDataContext Db, bool testing)
        {
            this.testing = testing;

            this.db = Db;
            if (testing)
            {
                login = "9t8Pqzs4CW3S";
                key = "9j33v58nuZB865WR";
                url = testurl;
            }
            else
            {
                login = Db.Setting("x_login", "");
                key = Db.Setting("x_tran_key", "");
                url = produrl;
            }
        }
        private XDocument getResponse(string request)
        {
            var wc = new WebClient();
            wc.Headers.Add("Content-Type", "text/xml");
            var bits = Encoding.UTF8.GetBytes(request);
            var ret = wc.UploadData(url, "POST", bits);
            using (var xmlStream = new MemoryStream(ret))
            using (var xmlReader = new XmlTextReader(xmlStream))
            {
                var x = XDocument.Load(xmlReader);
                var result = x.Descendants(ns + "resultCode").First().Value;
                if (result == "Error")
                {
                    var message = x.Descendants(ns + "text").First().Value;
                    throw new Exception(message);
                }
                return x;
            }
        }

        private void AddUpdateCustomerProfile(int PeopleId, string type, string cardnumber, string expires, string cardcode, string routing, string account, bool giving)
        {
            var exp = expires;
            if (exp.HasValue())
                exp = "20" + expires.Substring(2, 2) + "-" + expires.Substring(0, 2);
            var p = db.LoadPersonById(PeopleId);
            var pi = p.PaymentInfo();
            if (pi == null)
            {
                pi = new PaymentInfo();
                p.PaymentInfos.Add(pi);
            }
            if (pi.AuNetCustId == null) // create a new profilein Authorize.NET CIM
            {
                XDocument request = null;
                if (type == "B")
                {
                    request = new XDocument(new XDeclaration("1.0", "utf-8", null),
                        Element("createCustomerProfileRequest",
                            Element("merchantAuthentication",
                                Element("name", login),
                                Element("transactionKey", key)
                                ),
                            Element("profile",
                                Element("merchantCustomerId", PeopleId),
                                Element("email", p.EmailAddress),
                                Element("paymentProfiles",
                                    Element("billTo",
                                        Element("firstName", p.FirstName),
                                        Element("lastName", p.LastName),
                                        Element("address", p.PrimaryAddress),
                                        Element("city", p.PrimaryCity),
                                        Element("state", p.PrimaryState),
                                        Element("zip", p.PrimaryZip),
                                        Element("phoneNumber", p.HomePhone)
                                        ),
                                    Element("payment",
                                        Element("bankAccount",
                                            Element("routingNumber", routing),
                                            Element("accountNumber", account),
                                            Element("nameOnAccount", p.Name)
                                            )
                                        )
                                    )
                                )
                            )
                        );
                }
                else
                {
                    request = new XDocument(new XDeclaration("1.0", "utf-8", null),
                    Element("createCustomerProfileRequest",
                        Element("merchantAuthentication",
                            Element("name", login),
                            Element("transactionKey", key)
                            ),
                        Element("profile",
                            Element("merchantCustomerId", PeopleId),
                            Element("email", p.EmailAddress),
                            Element("paymentProfiles",
                                Element("billTo",
                                    Element("firstName", p.FirstName),
                                    Element("lastName", p.LastName),
                                    Element("address", p.PrimaryAddress),
                                    Element("city", p.PrimaryCity),
                                    Element("state", p.PrimaryState),
                                    Element("zip", p.PrimaryZip),
                                    Element("phoneNumber", p.HomePhone)
                                    ),
                                Element("payment",
                                    Element("creditCard",
                                        Element("cardNumber", cardnumber),
                                        Element("expirationDate", exp),
                                        Element("cardCode", cardcode)
                                        )
                                    )
                                )
                            )
                        )
                    );
                }
                var s = request.ToString();
                var x = getResponse(s);
                var id = x.Descendants(ns + "customerProfileId").First().Value.ToInt();
                var pid = x.Descendants(ns + "customerPaymentProfileIdList")
                            .Descendants(ns + "numericString").First().Value.ToInt();
                pi.AuNetCustId = id;
                pi.AuNetCustPayId = pid;
            }
            else
            {
                if (account.HasValue() && account.StartsWith("X"))
                {
                    var xe = getCustomerPaymentProfile(PeopleId);
                    var xba = xe.Descendants(ns + "bankAccount").Single();
                    routing = xba.Element(ns + "routingNumber").Value;
                    account = xba.Element(ns + "accountNumber").Value;
                }

                var request = new XDocument(new XDeclaration("1.0", "utf-8", null),
                    Element("updateCustomerProfileRequest",
                        Element("merchantAuthentication",
                            Element("name", login),
                            Element("transactionKey", key)
                            ),
                        Element("profile",
                            Element("merchantCustomerId", PeopleId),
                            Element("email", p.EmailAddress),
                            Element("customerProfileId", pi.AuNetCustId)
                            )
                    )
                );
                var x = getResponse(request.ToString());
                if (type == "B")
                    request = new XDocument(new XDeclaration("1.0", "utf-8", null),
                        Element("updateCustomerPaymentProfileRequest",
                            Element("merchantAuthentication",
                                Element("name", login),
                                Element("transactionKey", key)
                                ),
                            Element("customerProfileId", pi.AuNetCustId),
                            Element("paymentProfile",
                                Element("billTo",
                                    Element("firstName", p.FirstName),
                                    Element("lastName", p.LastName),
                                    Element("address", p.PrimaryAddress),
                                    Element("city", p.PrimaryCity),
                                    Element("state", p.PrimaryState),
                                    Element("zip", p.PrimaryZip),
                                    Element("phoneNumber", p.HomePhone)
                                    ),
                                Element("payment",
                                    Element("bankAccount",
                                        Element("routingNumber", routing),
                                        Element("accountNumber", account),
                                        Element("nameOnAccount", p.Name)
                                        )
                                    ),
                                Element("customerPaymentProfileId", pi.AuNetCustPayId)
                            )
                        )
                    );
                else
                    request = new XDocument(new XDeclaration("1.0", "utf-8", null),
                        Element("updateCustomerPaymentProfileRequest",
                            Element("merchantAuthentication",
                                Element("name", login),
                                Element("transactionKey", key)
                                ),
                            Element("customerProfileId", pi.AuNetCustId),
                            Element("paymentProfile",
                                Element("billTo",
                                    Element("firstName", p.FirstName),
                                    Element("lastName", p.LastName),
                                    Element("address", p.PrimaryAddress),
                                    Element("city", p.PrimaryCity),
                                    Element("state", p.PrimaryState),
                                    Element("zip", p.PrimaryZip),
                                    Element("phoneNumber", p.HomePhone)
                                    ),
                                Element("payment",
                                    Element("creditCard",
                                        Element("cardNumber", cardnumber),
                                        Element("expirationDate", exp),
                                        Element("cardCode", cardcode)
                                        )
                                    ),
                                Element("customerPaymentProfileId", pi.AuNetCustPayId)
                            )
                        )
                    );
                x = getResponse(request.ToString());
            }
			if (giving)
				pi.PreferredGivingType = type;
			else
				pi.PreferredPaymentType = type;
            pi.MaskedAccount = Util.MaskAccount(account);
            pi.MaskedCard = Util.MaskCC(cardnumber);
            pi.Ccv = cardcode;
            pi.Expires = expires;
            db.SubmitChanges();
        }
        private string getCustomerProfileIds()
        {
            var request = new XDocument(new XDeclaration("1.0", "utf-8", null),
                Element("getCustomerProfileIdsRequest",
                    Element("merchantAuthentication",
                        Element("name", login),
                        Element("transactionKey", key)
                        )
                    )
                );
            var x = getResponse(request.ToString());
            return x.ToString();
        }
        private XDocument getCustomerPaymentProfile(int PeopleId)
        {
            var rg = db.PaymentInfos.Single(pp => pp.PeopleId == PeopleId);
            var request = new XDocument(new XDeclaration("1.0", "utf-8", null),
                Element("getCustomerPaymentProfileRequest",
                    Element("merchantAuthentication",
                        Element("name", login),
                        Element("transactionKey", key)
                        ),
                    Element("customerProfileId", rg.AuNetCustId),
                    Element("customerPaymentProfileId", rg.AuNetCustPayId)
                )
            );
            var x = getResponse(request.ToString());
            return x;
        }
        private string getCustomerProfile(int PeopleId)
        {
            var au = db.PaymentInfos.Single(pp => pp.PeopleId == PeopleId);
            var request = new XDocument(new XDeclaration("1.0", "utf-8", null),
                Element("getCustomerProfileRequest",
                    Element("merchantAuthentication",
                        Element("name", login),
                        Element("transactionKey", key)
                        ),
                    Element("customerProfileId", au.AuNetCustId)
                )
            );
            var x = getResponse(request.ToString());
            return x.ToString();
        }
        private XElement Element(string name, params object[] content)
        {
            return new XElement(ns + name, content);
        }
        private TransactionResponse createTransactionRequest(int PeopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode)
        {
            var p = db.LoadPersonById(PeopleId);
            var request = new XDocument(new XDeclaration("1.0", "utf-8", null),
            Element("createTransactionRequest",
                Element("merchantAuthentication",
                    Element("name", login),
                    Element("transactionKey", key)
                    ),
                Element("transactionRequest",
                    Element("transactionType", "authCaptureTransaction"), // or refundTransaction or voidTransaction
                    Element("amount", amt),
                    Element("payment",
                        Element("creditCard",
                            Element("cardNumber", cardnumber),
                            Element("expirationDate", expires),
                            Element("cardCode", cardcode)
                            )
                        ),
                    Element("order",
                        Element("invoiceNumber", tranid),
                        Element("description", description)
                        ),
                    Element("customer",
                        Element("id", PeopleId),
                        Element("email", p.EmailAddress)
                        ),
                    Element("billTo",
                        Element("firstName", p.FirstName),
                        Element("lastName", p.LastName),
                        Element("address", p.PrimaryAddress),
                        Element("city", p.PrimaryCity),
                        Element("state", p.PrimaryState),
                        Element("zip", p.PrimaryZip),
                        Element("phoneNumber", p.HomePhone)
                        ),
                    Element("customerIP", Util.GetIPAddress())
                    )
                )
            );

            var x = getResponse(request.ToString());
            var resp = x.Descendants(ns + "transactionResponse").First();
            var tr = new TransactionResponse
            {
                Approved = resp.Element(ns + "responseCode").Value == "1",
                AuthCode = resp.Element(ns + "authCode").Value,
                Message = resp.Descendants(ns + "message").First().Element(ns + "description").Value,
                TransactionId = resp.Element(ns + "transId").Value
            };
            return tr;
        }

        public void StoreInVault(int peopleId, string type, string cardnumber, string expires, string cardcode, string routing, string account, bool giving)
        {
            AddUpdateCustomerProfile(peopleId, type, cardnumber, DbUtil.NormalizeExpires(expires).ToString2("MMyy"), cardcode, routing, account, giving);
        }

        public void RemoveFromVault(int peopleId)
        {
            var request = new XDocument(new XDeclaration("1.0", "utf-8", null),
                Element("deleteCustomerProfileRequest",
                    Element("merchantAuthentication",
                        Element("name", login),
                        Element("transactionKey", key)
                        ),
                    Element("customerProfileId", peopleId)
                )
            );
            var x = getResponse(request.ToString());
        }

        public TransactionResponse VoidCreditCardTransaction(string reference)
        {
            throw new NotImplementedException();
        }

        public TransactionResponse VoidCheckTransaction(string reference)
        {
            throw new NotImplementedException();
        }

        public TransactionResponse RefundCreditCard(string reference, decimal amt)
        {
            throw new NotImplementedException();
        }

        public TransactionResponse RefundCheck(string reference, decimal amt)
        {
            throw new NotImplementedException();
        }

        public TransactionResponse PayWithCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string city, string state, string zip, string phone)
        {
            string url0 = "https://secure.authorize.net/gateway/transact.dll";
            if (testing)
                url0 = "https://test.authorize.net/gateway/transact.dll";


            var p = new Dictionary<string, string>();
            p["x_delim_data"] = "TRUE";
            p["x_delim_char"] = "|";
            p["x_relay_response"] = "FALSE";
            p["x_type"] = "AUTH_CAPTURE";
            p["x_method"] = "CC";

            if (testing)
            {
                p["x_login"] = "9t8Pqzs4CW3S";
                p["x_tran_key"] = "9j33v58nuZB865WR";
            }
            else
            {
                p["x_login"] = DbUtil.Db.Setting("x_login", "");
                p["x_tran_key"] = DbUtil.Db.Setting("x_tran_key", "");
            }

            p["x_card_num"] = cardnumber;
            p["x_card_code"] = cardcode;
            p["x_exp_date"] = expires;
            p["x_amount"] = amt.ToString();
            p["x_description"] = description;
            p["x_invoice_num"] = tranid.ToString();
            p["x_cust_id"] = peopleId.ToString();
            p["x_first_name"] = first;
            p["x_last_name"] = last;
            p["x_address"] = addr;
            p["x_city"] = city;
            p["x_state"] = state;
            p["x_zip"] = zip;
            p["x_email"] = email;

            var sb = new StringBuilder();
            foreach (var kv in p)
                sb.AppendFormat("{0}={1}&", kv.Key, HttpUtility.UrlEncode(kv.Value));
            sb.Length = sb.Length - 1;

            var wc = new WebClient();
            var req = WebRequest.Create(url0);
            req.Method = "POST";
            req.ContentLength = sb.Length;
            req.ContentType = "application/x-www-form-urlencoded";

            var sw = new StreamWriter(req.GetRequestStream());
            sw.Write(sb.ToString());
            sw.Close();

            var r = req.GetResponse();
            using (var rs = new StreamReader(r.GetResponseStream()))
            {
                var resp = rs.ReadToEnd();
                rs.Close();
                var a = resp.Split('|');
                return new TransactionResponse
                {
                    Approved = a[0] == "1",
                    Message = a[3],
                    AuthCode = a[4],
                    TransactionId = a[6]
                };
            }
        }

        public TransactionResponse PayWithCheck(int peopleId, decimal amt, string routing, string acct, string description, int tranid, string email, string first, string middle, string last, string suffix, string addr, string city, string state, string zip, string phone)
        {
            string url0 = "https://secure.authorize.net/gateway/transact.dll";
            if (testing)
                url0 = "https://test.authorize.net/gateway/transact.dll";

            var p = new Dictionary<string, string>();
            p["x_delim_data"] = "TRUE";
            p["x_delim_char"] = "|";
            p["x_relay_response"] = "FALSE";
            p["x_method"] = "ECHECK";

            if (testing)
            {
                p["x_login"] = "9t8Pqzs4CW3S";
                p["x_tran_key"] = "9j33v58nuZB865WR";
            }
            else
            {
                p["x_login"] = DbUtil.Db.Setting("x_login", "");
                p["x_tran_key"] = DbUtil.Db.Setting("x_tran_key", "");
            }

            p["x_bank_aba_code"] = routing;
            p["x_bank_acct_num"] = acct;
            p["x_bank_acct_type"] = "CHECKING";
            p["x_bank_acct_name"] = first + " " + last;
            p["x_echeck_type"] = "WEB";
            p["x_recurring_billing"] = "FALSE";
            p["x_amount"] = amt.ToString();

            p["x_description"] = description;
            p["x_invoice_num"] = tranid.ToString();
            p["x_cust_id"] = peopleId.ToString();
            p["x_last_name"] = last;
            p["x_address"] = addr;
            p["x_city"] = city;
            p["x_state"] = state;
            p["x_zip"] = zip;

            var sb = new StringBuilder();
            foreach (var kv in p)
                sb.AppendFormat("{0}={1}&", kv.Key, HttpUtility.UrlEncode(kv.Value));
            sb.Length = sb.Length - 1;

            var wc = new WebClient();
            var req = WebRequest.Create(url0);
            req.Method = "POST";
            req.ContentLength = sb.Length;
            req.ContentType = "application/x-www-form-urlencoded";

            var sw = new StreamWriter(req.GetRequestStream());
            sw.Write(sb.ToString());
            sw.Close();

            var r = req.GetResponse();
            using (var rs = new StreamReader(r.GetResponseStream()))
            {
                var resp = rs.ReadToEnd();
                rs.Close();
                var a = resp.Split('|');
                return new TransactionResponse
                {
                    Approved = a[0] == "1",
                    Message = a[3],
                    AuthCode = a[4],
                    TransactionId = a[6]
                };
            }
        }

        public TransactionResponse PayWithVault(int peopleId, decimal amt, string description, int tranid, string type)
        {
            var pi = db.PaymentInfos.Single(pp => pp.PeopleId == peopleId);
            if (pi == null)
                return new TransactionResponse
                {
                    Approved = false,
                    Message = "missing payment info",
                };
            var request = new XDocument(new XDeclaration("1.0", "utf-8", null),
            Element("createCustomerProfileTransactionRequest",
                Element("merchantAuthentication",
                    Element("name", login),
                    Element("transactionKey", key)
                    ),
                Element("refId", peopleId),
                Element("transaction",
                    Element("profileTransAuthCapture",
                        Element("amount", amt),
                        Element("customerProfileId", pi.AuNetCustId),
                        Element("customerPaymentProfileId", pi.AuNetCustPayId),
                        Element("order",
                            Element("invoiceNumber", tranid),
                            Element("description", description)
                            ),
                        Element("cardCode", pi.Ccv)
                        )
                    )
                )
            );
            var x = getResponse(request.ToString());

            var resp = x.Descendants(ns + "directResponse").First().Value;
            var a = resp.Split(',');
            var tr = new TransactionResponse
            {
                Approved = a[0] == "1",
                Message = a[3],
                AuthCode = a[4],
                TransactionId = a[6]
            };
            return tr;
        }

        public System.Data.DataSet SettledBatchSummary(DateTime start, DateTime end, bool includeCreditCard, bool includeVirtualCheck)
        {
            throw new NotImplementedException();
        }

        public System.Data.DataSet SettledBatchListing(string batchref, string type)
        {
            throw new NotImplementedException();
        }

        public System.Data.DataSet VirtualCheckRejects(DateTime startdt, DateTime enddt)
        {
            throw new NotImplementedException();
        }

        public System.Data.DataSet VirtualCheckRejects(DateTime rejectdate)
        {
            throw new NotImplementedException();
        }

        public bool CanVoidRefund
        {
            get { return false; }
        }

        public bool CanGetSettlementDates
        {
            get { return false; }
        }

        public bool CanGetBounces
        {
            get { return false; }
        }
    }
}