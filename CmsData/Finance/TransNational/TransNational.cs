using System;
using System.Data;
using CmsData.Classes.Transnational;

namespace CmsData.Finance.TransNational
{
    internal class TransNational : IGateway
    {
        readonly string login;
        readonly string key;
        CMSDataContext db;
        readonly bool testing;

        public string GatewayType { get { return "Transnational"; } }

        public TransNational(CMSDataContext db, bool testing)
        {
            this.db = db;
            this.testing = testing || db.Setting("GatewayTesting", "false").ToLower() == "true";
        }
        public void StoreInVault(int peopleId, string type, string cardNumber, string expires, string cardCode, string routing, string account, bool giving)
        {
            var p = db.LoadPersonById(peopleId);
            var pi = p.PaymentInfo();
            if (pi == null)
            {
                pi = new PaymentInfo();
                p.PaymentInfos.Add(pi);
            }

            if (type == "C")
            {
                if (pi.TbnCardVaultId == null) // start fresh
                {
                    var t = new TNBVaultAddCC()
                    {
                        testing = testing,
                        CCNumber = cardNumber,
                        CCExp = expires,

                        Address = p.PrimaryAddress,
                        City = p.PrimaryCity,
                        Country = p.PrimaryCountry ?? "USA",
                        EMail = p.EmailAddress,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Phone = p.HomePhone ?? p.CellPhone
                    };
                    var resp = TNBHelper.VaultTransaction(t);
                    pi.TbnCardVaultId = resp.customer_vault_id;
                }
                else // update
                {
                    if (!cardNumber.StartsWith("X"))
                    {
                        // Update Card and expiration date

                        var t = new TNBVaultUpdateCC()
                        {
                            testing = testing,
                            VaultID = pi.TbnCardVaultId.ToString(),
                            CCNumber = cardNumber,
                            CCExp = expires,

                            Address = p.PrimaryAddress,
                            City = p.PrimaryCity,
                            Country = p.PrimaryCountry ?? "USA",
                            EMail = p.EmailAddress,
                            FirstName = p.FirstName,
                            LastName = p.LastName,
                            Phone = p.HomePhone ?? p.CellPhone
                        };
                        var resp = TNBHelper.VaultTransaction(t);
                    }
                    else
                    {
                        // Update Expiration date only, does this work?

                        var t = new TNBVaultUpdateCC()
                        {
                            testing = testing,
                            VaultID = pi.TbnCardVaultId.ToString(),
                            CCExp = expires,
                        };
                        var resp = TNBHelper.VaultTransaction(t);
                    }
                }
            }
            else // bank account
            {
                if (pi.TbnBankVaultId == null) // start fresh
                {
                    var t = new TNBVaultAddACH()
                    {
                        testing = testing,
                        ACHAccount = account,
                        ACHRouting = routing,

                        Address = p.PrimaryAddress,
                        City = p.PrimaryCity,
                        Country = p.PrimaryCountry ?? "USA",
                        EMail = p.EmailAddress,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Phone = p.HomePhone ?? p.CellPhone
                    };
                    var resp = TNBHelper.VaultTransaction(t);
                    pi.TbnBankVaultId = resp.customer_vault_id;
                }
                else
                {
                    if (!account.StartsWith("X"))
                    {
                        var t = new TNBVaultUpdateACH()
                        {
                            testing = testing,
                            VaultID = pi.TbnBankVaultId.ToString(),
                            ACHAccount = account,
                            ACHRouting = routing,

                            Address = p.PrimaryAddress,
                            City = p.PrimaryCity,
                            Country = p.PrimaryCountry ?? "USA",
                            EMail = p.EmailAddress,
                            FirstName = p.FirstName,
                            LastName = p.LastName,
                            Phone = p.HomePhone ?? p.CellPhone
                        };
                        var resp = TNBHelper.VaultTransaction(t);
                    }
                }
            }
        }
        public void RemoveFromVault(int peopleId)
        {
            var p = db.LoadPersonById(peopleId);
            var pi = p.PaymentInfo();
            if (pi == null)
                return;

            if (pi.TbnCardVaultId.HasValue)
            {
                var t = new TNBVaultDelete()
                {
                    testing = testing,
                    VaultID = pi.TbnCardVaultId.ToString()
                };
                TNBHelper.VaultTransaction(t);
            }
            if (pi.TbnBankVaultId.HasValue)
            {
                var t = new TNBVaultDelete()
                {
                    testing = testing,
                    VaultID = pi.TbnBankVaultId.ToString()
                };
                TNBHelper.VaultTransaction(t);
            }

            pi.TbnCardVaultId = null;
            pi.TbnBankVaultId = null;
            pi.MaskedCard = null;
            pi.MaskedAccount = null;
            pi.Ccv = null;
            db.SubmitChanges();
        }

        public TransactionResponse VoidCreditCardTransaction(string reference)
        {
            var t = new TNBTransactionVoid()
            {
                testing = testing,
                TransactionID = reference
            };
            var resp = TNBHelper.Transaction(t).getTransactionResponse();
            return resp;
        }
        public TransactionResponse VoidCheckTransaction(string reference)
        {
            return VoidCreditCardTransaction(reference);
        }

        public TransactionResponse RefundCreditCard(string reference, Decimal amt)
        {
            var t = new TNBTransactionRefund()
            {
                testing = testing,
                TransactionID = reference,
                Amount = amt.ToString(),
            };
            var resp = TNBHelper.Transaction(t).getTransactionResponse();
            return resp;
        }
        public TransactionResponse RefundCheck(string reference, Decimal amt)
        {
            var t = new TNBTransactionRefund()
            {
                testing = testing,
                TransactionID = reference,
                Amount = amt.ToString(),
            };
            var resp = TNBHelper.Transaction(t).getTransactionResponse();
            return resp;
        }

        public TransactionResponse PayWithCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string city, string state, string zip, string phone)
        {
            var tnb = new TNBTransactionSaleCC()
            {
                testing = testing,
                CCNumber = cardnumber,
                CCCVV = cardcode,
                CCExp = expires,
                Amount = amt.ToString(),
                PONumber = peopleId.ToString(),
                OrderDescription = description,
                OrderID = tranid.ToString(),
                EMail = email,
                FirstName = first,
                LastName = last,
                Address = addr,
                City = city,
                State = state,
                Zip = zip,
                Phone = phone
            };
            return TNBHelper.Transaction(tnb).getTransactionResponse();
        }
        public TransactionResponse PayWithCheck(int peopleId, decimal amt, string routing, string acct, string description, int tranid, string email, string first, string middle, string last, string suffix, string addr, string city, string state, string zip, string phone)
        {
            var tnb = new TNBTransactionSaleACH()
            {
                testing = testing,
                ACHRouting = routing,
                ACHAccount = acct,
                Amount = amt.ToString(),
                EMail = email,
                FirstName = first,
                LastName = last,
                Address = addr,
                City = city,
                State = state,
                Zip = zip,
                Phone = phone,
                PONumber = peopleId.ToString(),
                OrderID = tranid.ToString(),
                OrderDescription = description
            };

            //where to put tranid, PeopleId, description

            return TNBHelper.Transaction(tnb).getTransactionResponse();
        }
        public TransactionResponse PayWithVault(int peopleId, decimal amt, string description, int tranid, string type)
        {
            var p = db.LoadPersonById(peopleId);
            var pi = p.PaymentInfo();
            if (pi == null)
                return new TransactionResponse
                {
                    Approved = false,
                    Message = "missing payment info",
                };
            var t = new TNBVaultTransaction()
            {
                testing = true,
                VaultID = ((type ?? "B") == "B" ? pi.TbnBankVaultId : pi.TbnCardVaultId).ToString(),
                Amount = amt.ToString(),
            };
            var resp = TNBHelper.VaultTransaction(t).getTransactionResponse();
            return resp;
        }
        public DataSet SettledBatchSummary(DateTime start, DateTime end, bool includeCreditCard, bool includeVirtualCheck)
        {
            return null;
        }
        public DataSet SettledBatchListing(string batchref, string type)
        {
            return null;
        }
        public DataSet VirtualCheckRejects(DateTime startdt, DateTime enddt)
        {
            return null;
        }
        public DataSet VirtualCheckRejects(DateTime rejectdate)
        {
            return null;
        }


        public bool CanVoidRefund
        {
            get { return true; }
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