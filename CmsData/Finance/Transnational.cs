using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web;
using CmsData.Classes.Transnational;
using UtilityExtensions;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace CmsData
{
    public class Transnational
    {
        readonly string login;
        readonly string key;
        CMSDataContext db;
        readonly bool testing;

        public Transnational(CMSDataContext db, bool testing)
        {
            this.db = db;
            this.testing = testing;
        }
        public void storeVault(int PeopleId,
            string type, string cardnumber, string expires, string cardcode,
            string routing, string account, bool giving)
        {
            var p = db.LoadPersonById(PeopleId);
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
                        CCNumber = cardnumber,
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
                    if (!cardnumber.StartsWith("X"))
                    {
                        // Update Card and expiration date

                        var t = new TNBVaultUpdateCC()
                        {
                            testing = testing,
                            VaultID = pi.TbnCardVaultId.ToString(),
                            CCNumber = cardnumber,
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
        public void deleteVaultData(int PeopleId)
        {
            var p = db.LoadPersonById(PeopleId);
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

        public TransactionResponse voidTransactionRequest(string reference)
        {
            var t = new TNBTransactionVoid()
            {
                testing = testing,
                TransactionID = reference
            };
            var resp = TNBHelper.Transaction(t).getTransactionResponse();
            return resp;
        }
        public TransactionResponse voidCheckRequest(string reference)
        {
            return voidTransactionRequest(reference);
        }

        public TransactionResponse creditTransactionRequest(string reference, Decimal amt)
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
        public TransactionResponse creditCheckTransactionRequest(string reference, Decimal amt)
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

        public TransactionResponse createTransactionRequest(int PeopleId, decimal amt,
            string cardnumber, string expires, string description, int tranid, string cardcode,
            string email, string first, string last,
            string addr, string city, string state, string zip, string phone)
        {
            var tnb = new TNBTransactionSaleCC()
            {
                testing = testing,
                CCNumber = cardnumber,
                CCCVV = cardcode,
                CCExp = expires,
                Amount = amt.ToString(),
                PONumber = PeopleId.ToString(),
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
        public TransactionResponse createCheckTransactionRequest(int PeopleId, decimal amt,
            string routing, string acct, string description, int tranid,
            string email, string first, string middle, string last, string suffix,
            string addr, string city, string state, string zip, string phone)
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
                PONumber = PeopleId.ToString(),
                OrderID = tranid.ToString(),
                OrderDescription = description
            };

            //where to put tranid, PeopleId, description

            return TNBHelper.Transaction(tnb).getTransactionResponse();
        }
        public TransactionResponse createVaultTransactionRequest(int PeopleId, decimal amt, string description, int tranid, string type)
        {
            var p = db.LoadPersonById(PeopleId);
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
        public DataSet SettledBatchSummary(DateTime start, DateTime end, bool IncludeCreditCard, bool IncludeVirtualCheck)
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
    }
}