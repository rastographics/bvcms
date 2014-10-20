using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AuthorizeNet.APICore;
using Community.CsharpSqlite;
using UtilityExtensions;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using AuthorizeNet;

namespace CmsData
{
    public class AuthorizeNet2 : IGateway
    {
        readonly XNamespace ns = "AnetApi/xml/v1/schema/AnetApiSchema.xsd";
        const string produrl = "https://api.authorize.net/xml/v1/request.api";
        const string testurl = "https://apitest.authorize.net/xml/v1/request.api";
        readonly string url;
        readonly string login;
        readonly string key;
        readonly bool testing;
        readonly CMSDataContext db;

        private bool IsLive = false;
        private ServiceMode ServiceMode { get { return IsLive ? ServiceMode.Live : ServiceMode.Test; } }

        public string GatewayType { get { return "AuthorizeNet"; } }

        public AuthorizeNet2(CMSDataContext Db, bool testing)
        {
            this.testing = testing || Db.Setting("GatewayTesting", "false").ToLower() == "true";

            this.db = Db;
            if (this.testing)
            {
                login = "9t8Pqzs4CW3S";
                key = "9j33v58nuZB865WR";
                IsLive = false;
            }
            else
            {
                login = Db.Setting("x_login", "");
                key = Db.Setting("x_tran_key", "");
                IsLive = true;
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

        public void StoreInVault(int peopleId, string type, string cardNumber, string expires, string cardCode,
            string routing, string account, bool giving)
        {
            var expiredt = DbUtil.NormalizeExpires(expires).Value;

            var p = db.LoadPersonById(peopleId);
            var billToAddress = new AuthorizeNet.Address
            {
                City = p.PrimaryCity,
                First = p.FirstName,
                Last = p.LastName,
                State = p.PrimaryState,
                Zip = p.PrimaryZip,
                Phone = p.HomePhone ?? p.CellPhone,
                Street = p.PrimaryAddress
            };

            Customer customer = null;

            //var customerIds = CustomerGateway.GetCustomerIDs();
            var pi = p.PaymentInfo();
            if (pi == null)
            {
                pi = new PaymentInfo();
                p.PaymentInfos.Add(pi);
            }

            if (pi.AuNetCustId == null) // create a new profilein Authorize.NET CIM
            {
                // NOTE: this can throw an error if the email address already exists...
                // TODO: Authorize.net needs to release a new Nuget package, because they don't have a clean way to pass in customer ID (aka PeopleId) yet... the latest code has a parameter for this, though
                //       - we could call UpdateCustomer after the fact to do this if we wanted to
                customer = CustomerGateway.CreateCustomer(p.EmailAddress, p.Name);
                customer.ID = peopleId.ToString();

                // we only have to do this because we set the ID property and we want that saved...
                CustomerGateway.UpdateCustomer(customer);
            }

            if (type == "B")
                SaveECheckToProfile(routing, account, pi, customer, billToAddress);
            else if (type == "C")
                SaveCreditCardToProfile(cardNumber, cardCode, expiredt, pi, customer, billToAddress);
            else
                throw new ArgumentException("Type {0} not supported".Fmt(type), "type");

            db.SubmitChanges();
        }

        private void SaveECheckToProfile(string routing, string account, PaymentInfo pi, Customer customer, AuthorizeNet.Address billToAddress)
        {
            var foundPaymentProfile = customer.PaymentProfiles.SingleOrDefault(p => p.ProfileID == pi.AuNetCustPayBankId.ToString());

            var bankAccount = new BankAccount
            {
                accountNumber = account,
                routingNumber = routing,
                accountType = BankAccountType.Checking,
                nameOnAccount = customer.Description
            };

            if (foundPaymentProfile == null)
            {
                var paymentProfileId = CustomerGateway.AddECheckBankAccount(customer.ProfileID, bankAccount, billToAddress);

                pi.AuNetCustPayBankId = paymentProfileId.ToInt();
            }
            else
            {
                foundPaymentProfile.eCheckBankAccount = bankAccount;
                foundPaymentProfile.BillingAddress = billToAddress;

                var isSaved = CustomerGateway.UpdatePaymentProfile(customer.ProfileID, foundPaymentProfile);
                if (!isSaved)
                    throw new Exception("UpdatePaymentProfile failed to save credit card for {0}".Fmt(pi.PeopleId));
            }
        }

        // NOTE: this can throw an error if the credit card number already exists...
        private void SaveCreditCardToProfile(string cardNumber, string cardCode, DateTime expires, PaymentInfo pi, Customer customer, AuthorizeNet.Address billToAddress)
        {
            var foundPaymentProfile = customer.PaymentProfiles.SingleOrDefault(p => p.ProfileID == pi.AuNetCustPayId.ToString());

            if (foundPaymentProfile == null)
            {
                var paymentProfileId = CustomerGateway.AddCreditCard(customer.ProfileID, cardNumber,
                    expires.Month, expires.Year, cardCode, billToAddress);

                pi.AuNetCustPayId = paymentProfileId.ToInt();
            }
            else
            {
                if (!cardNumber.StartsWith("X"))
                {
                    foundPaymentProfile.CardNumber = cardNumber;
                    foundPaymentProfile.CardCode = cardCode;
                }
                foundPaymentProfile.CardExpiration = expires.ToString("MMyy");
                foundPaymentProfile.BillingAddress = billToAddress;

                var isSaved = CustomerGateway.UpdatePaymentProfile(customer.ProfileID, foundPaymentProfile);
                if (!isSaved)
                    throw new Exception("UpdatePaymentProfile failed to save echeck for {0}".Fmt(pi.PeopleId));
            }
        }


        public void RemoveFromVault(int peopleId)
        {
            var p = db.LoadPersonById(peopleId);
            var pi = p.PaymentInfo();
            if (pi == null)
                return;

            if (CustomerGateway.DeleteCustomer(pi.AuNetCustId.ToString()))
            {
                pi.SageCardGuid = null;
                pi.SageBankGuid = null;
                pi.MaskedCard = null;
                pi.MaskedAccount = null;
                pi.Ccv = null;
                pi.AuNetCustId = null;
                pi.AuNetCustPayId = null;
                pi.AuNetCustPayBankId = null;
                db.SubmitChanges();
            }
            else
            {
                throw new Exception("Failed to delete customer {0}".Fmt(peopleId));
            }
        }

        private void AddUpdateCustomerProfile(int PeopleId, string type, string cardnumber, string expires, string cardcode, string routing, string account, bool giving)
        {
            var p = db.LoadPersonById(PeopleId);
            var pi = p.PaymentInfo();
            if (pi == null)
            {
                pi = new PaymentInfo();
                p.PaymentInfos.Add(pi);
            }
            if (pi.AuNetCustId == null) // create a new profilein Authorize.NET CIM
            {
                var target = new CustomerGateway(login, key);
                var cust = target.CreateCustomer(p.EmailAddress, p.Name, pi.AuNetCustId.ToString());
                pi.AuNetCustId = cust.ProfileID.ToInt();
                var address = new AuthorizeNet.Address()
                {
                    City = p.PrimaryCity,
                    First = p.FirstName,
                    Last = p.LastName,
                    ID = p.PeopleId.ToString(),
                    Phone = p.HomePhone ?? p.CellPhone,
                    State = p.PrimaryState,
                    Street = p.PrimaryAddress,
                    Zip = p.PrimaryZip,
                };
                if (type == "B") // new vault bank account
                {
                    var bankaccount = new AuthorizeNet.BankAccount()
                    {
                        accountNumber = account,
                        routingNumber = routing,
                        accountType = BankAccountType.Checking,
                        nameOnAccount = p.Name
                    };
                    var resp = target.AddECheckBankAccount(cust.ProfileID, bankaccount, address);
                }
                else // new vault credit card
                {
                    var exyear = expires.Substring(2, 2).ToInt();
                    var exmonth = expires.Substring(0, 2).ToInt();
                    var resp = target.AddCreditCard(cust.ProfileID, cardnumber, exmonth, exyear, cardcode, address);
                }
            }
            else // update existing
            {
                //                var target = new CustomerGateway(login, key);
                //                var c1 = target.GetCustomer(pi.AuNetCustId.ToString());
                //                var p1 = c1.PaymentProfiles[0];
                //                target.UpdatePaymentProfile(pi.AuNetCustPayId.ToString(), p1);
                //
                //                target.UpdatePaymentProfile(pi.AuNetCustId.ToString(), );
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
                if (type == "B") // update bank account
                {
                    var bankaccountElement = Element("bankAccount", Element("nameOnAccount", p.Name));
                    if (!routing.StartsWith("X"))
                        bankaccountElement.Add(Element("routingNumber", routing));
                    if (!account.StartsWith("X"))
                        bankaccountElement.Add(Element("accountNumber", account));

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
                                Element("payment", bankaccountElement),
                                Element("customerPaymentProfileId", pi.AuNetCustPayId)
                                )
                            )
                        );
                    x = getResponse(request.ToString());
                }
                else // update credit card
                {
                    var creditcardElement = Element("creditCard", Element("expirationDate", expires));
                    if (!cardcode.StartsWith("X"))
                        creditcardElement.Add(Element("cardCode", cardnumber));
                    if (!cardnumber.StartsWith("X"))
                        creditcardElement.Add(Element("cardNumber", cardnumber));

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
                                Element("payment", creditcardElement),
                                Element("customerPaymentProfileId", pi.AuNetCustPayId)
                                )
                            )
                        );
                    x = getResponse(request.ToString());
                }
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
        private XDocument getCustomerProfileIds()
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
            return x;
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
        private string getCustomerProfile(int custid)//int PeopleId)
        {
            var request = new XDocument(new XDeclaration("1.0", "utf-8", null),
                Element("getCustomerProfileRequest",
                    Element("merchantAuthentication",
                        Element("name", login),
                        Element("transactionKey", key)
                        ),
                    Element("customerProfileId", custid)
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
            if (resp == null)
                throw new Exception("no response");
            var tr = new TransactionResponse
            {
                Approved = ElementValue(resp, "responseCode") == "1",
                AuthCode = ElementValue(resp, "authCode"),
                Message = ElementValue(FirstElement(resp, "message"), "description"),
                TransactionId = ElementValue(resp, "transId")
            };
            return tr;
        }
        private XElement FirstElement(XContainer e, string v)
        {
            return e.Descendants(ns + v).First();
        }
        private string ElementValue(XContainer e, string v)
        {
            var r = e.Element(ns + v);
            return r != null ? r.Value : "";
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
                p["x_login"] = login;
                p["x_tran_key"] = key;
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
        private AuthorizeNet.IGateway _gateway;
        private AuthorizeNet.IGateway Gateway
        {
            get
            {
                if (_gateway == null)
                    _gateway = new Gateway(login, key, !IsLive);
                return _gateway;
            }
        }

        private ICustomerGateway _customerGateway;
        /// <summary>
        /// For "vault"-like functionality
        /// </summary>
        private ICustomerGateway CustomerGateway
        {
            get
            {
                if (_customerGateway == null)
                    _customerGateway = new CustomerGateway(login, key, ServiceMode);
                return _customerGateway;
            }
        }

        private IReportingGateway _reportingGateway;
        /// <summary>
        /// For batches, settlement, etc.
        /// </summary>
        private IReportingGateway ReportingGateway
        {
            get
            {
                if (_reportingGateway == null)
                    _reportingGateway = new ReportingGateway(login, key, ServiceMode);
                return _reportingGateway;
            }
        }
    }
}