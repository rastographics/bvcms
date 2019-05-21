using CmsData.Finance.Acceptiva.Core;
using CmsData.Finance.Acceptiva.Get;
using CmsData.Finance.Acceptiva.Store;
using CmsData.Finance.Acceptiva.Transaction.Charge;
using CmsData.Finance.Acceptiva.Transaction.Get;
using CmsData.Finance.Acceptiva.Transaction.Refund;
using CmsData.Finance.Acceptiva.Transaction.Settlement;
using CmsData.Finance.Acceptiva.Transaction.Void;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsData.Finance
{
    public class AcceptivaGateway : IGateway
    {
        private readonly string _apiKey;
        private readonly string _merch_ach_id;
        private readonly string _merch_cc_id;
        private readonly bool _isTesting = false;
        private readonly bool _automaticSettle = false;
        private readonly CMSDataContext db;
        private readonly PaymentProcessTypes ProcessType;

        public string GatewayType => "Acceptiva";
        public string GatewayName { get; set; }

        public string Identifier => $"{GatewayType}-{_apiKey}-{_merch_ach_id}-{_merch_cc_id}";

        public AcceptivaGateway(CMSDataContext db, bool testing, PaymentProcessTypes ProcessType)
        {
            this.db = db;
            this.ProcessType = ProcessType;

            if (testing || MultipleGatewayUtils.GatewayTesting(db, ProcessType))
            {
                _apiKey = "CZDWp7dXCo4W3xTA7LtWAijidvPdj2wa";
                _merch_ach_id = "dKdDFtqC";
                _merch_cc_id = "R6MLUevR";
                _isTesting = true;
                //If this setting exists we settle transactions manually, so we can refund.
                //For live environment settlements are automatic 1 day later
                _automaticSettle = db.Setting("AutomaticSettle");
            }
            else
            {
                _apiKey = MultipleGatewayUtils.Setting(db, "AcceptivaApiKey", "", (int)ProcessType);
                _merch_ach_id = MultipleGatewayUtils.Setting(db, "AcceptivaAchId", "", (int)ProcessType);
                _merch_cc_id = MultipleGatewayUtils.Setting(db, "AcceptivaCCId", "", (int)ProcessType);

                if (string.IsNullOrWhiteSpace(_apiKey))
                    throw new Exception("AcceptivaApiKey setting not found, which is required for Acceptiva.");
                if (string.IsNullOrWhiteSpace(_merch_ach_id))
                    throw new Exception("AcceptivaAcctId setting not found, which is required for Acceptiva.");
                if (string.IsNullOrWhiteSpace(_merch_cc_id))
                    throw new Exception("AcceptivaCCId setting not found, which is required for Acceptiva.");
            }
        }

        public bool CanVoidRefund => true;
        public bool CanGetSettlementDates => true;
        public bool UseIdsForSettlementDates => false;
        public bool CanGetBounces => true;

        public void StoreInVault(int peopleId, string type, string cardNumber, string expires, string cardCode, string routing, string account, bool giving)
        {
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo == null)
            {
                paymentInfo = new PaymentInfo();
                person.PaymentInfos.Add(paymentInfo);
            }
            //Set values to be ignored in API
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.StartsWith("X"))
            {
                cardNumber = string.Empty;
                expires = string.Empty;
            }
            else
            {
                paymentInfo.MaskedCard = Util.MaskCC(cardNumber);
                paymentInfo.Expires = expires;
            }
            if (string.IsNullOrEmpty(account) || account.StartsWith("X"))
            {
                account = string.Empty;
                routing = string.Empty;
            }
            else
            {
                paymentInfo.MaskedAccount = Util.MaskAccount(account);
                paymentInfo.Routing = Util.Mask(new StringBuilder(routing), 2);
            }
            //Check if Accpetiva has this payer data
            string acceptivaPayerid = GetAcceptivaPayerId(peopleId);
            if (string.IsNullOrEmpty(acceptivaPayerid))
            {
                paymentInfo.AcceptivaPayerId = StoreNewPayerData(person, paymentInfo, cardNumber, expires, routing, account);
            }
            else
            {
                paymentInfo.AcceptivaPayerId = acceptivaPayerid;
                StorePayerData(person, paymentInfo, cardNumber, expires, account, routing);
            }
            if (giving)
                paymentInfo.PreferredGivingType = type;
            else
                paymentInfo.PreferredPaymentType = type;
            db.SubmitChanges();
        }

        public void RemoveFromVault(int peopleId)
        {
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo == null)
                return;

            // clear out local record and save changes.
            paymentInfo.AcceptivaPayerId = null;
            paymentInfo.MaskedCard = null;
            paymentInfo.MaskedAccount = null;
            paymentInfo.Expires = null;
            db.SubmitChanges();
        }

        public TransactionResponse VoidCreditCardTransaction(string reference)
        {
            return VoidTransaction(reference);
        }

        public TransactionResponse VoidCheckTransaction(string reference)
        {
            return VoidTransaction(reference);
        }

        public TransactionResponse RefundCreditCard(string reference, decimal amt, string lastDigits = "")
        {
            return RefundTransaction(reference, amt);
        }

        public TransactionResponse RefundCheck(string reference, decimal amt, string lastDigits = "")
        {
            return RefundTransaction(reference, amt);
        }

        public TransactionResponse AuthCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            return CreditCardCharge(peopleId, amt, cardnumber, expires, description, tranid, cardcode, email, first, last, addr, addr2, city, state, country, zip, phone);
        }

        public TransactionResponse PayWithCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            var response = CreditCardCharge(peopleId, amt, cardnumber, expires, description, tranid, cardcode, email, first, last, addr, addr2, city, state, country, zip, phone);

            if (_automaticSettle && response.Approved)
                SettleTransaction(response.TransactionId);

            return response;
        }

        public TransactionResponse PayWithCheck(int peopleId, decimal amt, string routing, string acct, string description, int tranid, string email, string first, string middle, string last, string suffix, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            var achCharge = new AchCharge(
                _isTesting,
                _apiKey,
                _merch_ach_id,
                new Ach
                {
                    AchAccNum = acct,
                    AchRoutingNum = routing
                },
                new Payer
                {
                    LastName = last,
                    FirstName = first,
                    Address = addr,
                    Address2 = addr2,
                    City = city,
                    State = state,
                    Country = country,
                    Zip = zip,
                    Email = email,
                    Phone = phone
                },
                amt,
                tranid.ToString(CultureInfo.InvariantCulture),
                description,
                peopleId.ToString(CultureInfo.InvariantCulture));

            var response = achCharge.Execute();

            var transactionResponse = new TransactionResponse
            {
                Approved = response.Response.Status == "success" ? true : false,
                AuthCode = response.Response.TransStatus,
                Message = $"{response.Response.TransStatusMsg}#{response.Response.Items.First().IdString}",
                TransactionId = response.Response.TransIdStr
            };

            if (_automaticSettle && transactionResponse.Approved)
                SettleTransaction(response.Response.TransIdStr);

            return transactionResponse;
        }

        public TransactionResponse AuthCreditCardVault(int peopleId, decimal amt, string description, int tranid)
        {
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo?.AcceptivaPayerId == null)
                return new TransactionResponse
                {
                    Approved = false,
                    Message = "missing payment info",
                };

            return StoredPayerCharge(_merch_cc_id, paymentInfo.AcceptivaPayerId, amt, tranid.ToString(), description, 1, person.LastName, person.FirstName);
        }

        public TransactionResponse PayWithVault(int peopleId, decimal amt, string description, int tranid, string type)
        {
            TransactionResponse transactionResponse;
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo == null)
                return new TransactionResponse
                {
                    Approved = false,
                    Message = "missing payment info",
                };

            if (type == PaymentType.CreditCard) // credit card
                transactionResponse = StoredPayerCharge(_merch_cc_id, paymentInfo.AcceptivaPayerId, amt, tranid.ToString(), description, 1, person.LastName, person.FirstName);
            else // bank account
                transactionResponse = StoredPayerCharge(_merch_ach_id, paymentInfo.AcceptivaPayerId, amt, tranid.ToString(), description, 2, person.LastName, person.FirstName);

            if (_automaticSettle && transactionResponse.Approved)
                SettleTransaction(transactionResponse.TransactionId);

            return transactionResponse;
        }

        public BatchResponse GetBatchDetails(DateTime start, DateTime end)
        {
            var GetTransDetails = new GetSettledTransDetails(_isTesting, _apiKey, start, end);
            var transactionsList = GetTransDetails.Execute();

            var batchTransactions = new List<BatchTransaction>();

            foreach (var item in transactionsList)
            {
                batchTransactions.Add(new BatchTransaction
                {
                    TransactionId = int.Parse(item.Response.Items[0].Id),
                    Reference = item.Response.TransIdStr,
                    BatchReference = GetBatchReference(start, end),
                    TransactionType = GetTransactionType(item.Response.AmtProcessed),
                    BatchType = GetBatchType(item.Response.PaymentType),
                    Name = $"{item.Response.PayerFname} {item.Response.PayerLname}",
                    Amount = item.Response.AmtProcessed,
                    Approved = true,
                    Message = item.Response.TransStatusMsg,
                    TransactionDate = item.Response.TransDatetime,
                    SettledDate = item.Response.TransDatetime.AddDays(1),
                    LastDigits = item.Response.AcctLastFour
                });
            }

            return new BatchResponse(batchTransactions);
        }

        public ReturnedChecksResponse GetReturnedChecks(DateTime start, DateTime end)
        {
            var returnedChecks = new List<ReturnedCheck>();
            var getECheckReturned = new GetReturnedEChecks(_isTesting, _apiKey, start, end);
            var response = getECheckReturned.Execute();

            foreach (var returnedCheck in response)
            {
                returnedChecks.Add(new ReturnedCheck
                {
                    TransactionId = returnedCheck.Response.AcctName.ToInt(),
                    Name = returnedCheck.Response.AcctName,
                    RejectCode = returnedCheck.Response.TransStatus.ToString(),
                    RejectAmount = returnedCheck.Response.AmtProcessed,
                    RejectDate = returnedCheck.Response.TransDatetime.AddDays(1)
                });
            }

            return new ReturnedChecksResponse(returnedChecks);
        }

        public void CheckBatchSettlements(List<string> transactionids)
        {
            throw new NotImplementedException();
        }

        public void CheckBatchSettlements(DateTime start, DateTime end)
        {
            string testString = string.Empty;
            if (_isTesting)
                testString = "(testing)";

            var response = GetBatchDetails(start, end);

            var batchTransactions = response.BatchTransactions.ToList();
            var batchTypes = batchTransactions.Select(x => x.BatchType).Distinct();

            foreach (var batchType in batchTypes)
            {
                // key it by transaction reference and payment type.
                var unMatchedKeyedByReference = batchTransactions.Where(x => x.BatchType == batchType).ToDictionary(x => x.Reference + testString, x => x);

                // next let's get all the approved matching transactions from our transaction table by transaction id (reference).
                var approvedMatchingTransactions = from transaction in db.Transactions
                                                   where unMatchedKeyedByReference.Keys.Contains(transaction.TransactionId)
                                                   where transaction.Approved == true
                                                   select transaction;

                // next key the matching approved transactions that came from our transaction table by the transaction id (reference).
                var distinctTransactionIds = approvedMatchingTransactions.Select(x => x.TransactionId).Distinct();

                // finally let's get a list of all transactions that need to be inserted, which we don't already have.
                var transactionsToInsert = from transaction in unMatchedKeyedByReference
                                           where !distinctTransactionIds.Contains(transaction.Key)
                                           select transaction.Value;                

                // spin through each transaction and insert them to the transaction table.
                InsertMissingTransactions(transactionsToInsert, batchType);

                // next update Existing transactions with new batch data if there are any.
                UpdateExistingTransactions(approvedMatchingTransactions, unMatchedKeyedByReference);                
            }

            // finally we need to mark these batches as completed if there are any.
            MarkBatchesAsComplete(batchTransactions);            

            db.SubmitChanges();
        }        

        public string VaultId(int peopleId)
        {
            return db.PaymentInfos.Single(pp => pp.PeopleId == peopleId).AcceptivaPayerId;
        }

        //private methods
        private TransactionResponse CreditCardCharge(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string addr2, string city, string state, string country, string zip, string phone)
        {
            var cardCharge = new CreditCardCharge(
                _isTesting,
                _apiKey,
                _merch_cc_id,
                new CreditCard
                {
                    CardNum = cardnumber,
                    CardExpiration = expires,
                    CardCvv = cardcode
                },
                new Payer
                {
                    LastName = last,
                    FirstName = first,
                    Address = addr,
                    Address2 = addr2,
                    City = city,
                    State = UPSStateCodes.FromStateCountry(state, country, db) ?? state,
                    Country = ISO3166.Alpha3FromName(country) ?? country,
                    Zip = zip,
                    Email = email,
                    Phone = phone
                },
                amt,
                tranid.ToString(CultureInfo.InvariantCulture),
                description);

            var response = cardCharge.Execute();

            return new TransactionResponse
            {
                Approved = response.Response.Status == "success" ? true : false,
                AuthCode = response.Response.ProcessorResponseCode,
                Message = $"{response.Response.TransStatusMsg}#{response.Response.Items.First().IdString}",
                TransactionId = response.Response.TransIdStr
            };
        }

        private TransactionResponse StoredPayerCharge(string merchId, string acceptivaPayerId, decimal amt, string tranId, string description, int paymentType, string lname, string fname)
        {
            var storedPayerCharge = new StoredPayerCharge(_isTesting, _apiKey, merchId, acceptivaPayerId, amt, tranId, description, paymentType, lname, fname);
            var response = storedPayerCharge.Execute();

            return new TransactionResponse
            {
                Approved = response.Response.Status == "success" ? true : false,
                AuthCode = response.Response.TransStatus,
                Message = $"{response.Response.TransStatusMsg}#{response.Response.Items.First().IdString}",
                TransactionId = response.Response.TransIdStr
            };
        }

        private void SettleTransaction(string transactionId)
        {
            var settleTransaction = new SettleTransaction(_isTesting, _apiKey, transactionId);
            settleTransaction.Execute();
        }

        private TransactionResponse VoidTransaction(string reference)
        {
            var voidTrans = new VoidTrans(_isTesting, _apiKey, reference);
            var response = voidTrans.Execute();

            return new TransactionResponse
            {
                Approved = response.Response.Status == "success" ? true : false,
                AuthCode = response.Response.ProcessorResponseCode,
                Message = response.Response.Errors.FirstOrDefault()?.ErrorMsg,
                TransactionId = response.Response.TransIdStr
            };
        }

        private TransactionResponse RefundTransaction(string reference, decimal amt)
        {
            string testString = string.Empty;
            if (_isTesting)
                testString = "(testing)";

            string[] message = db.Transactions.SingleOrDefault(p => p.TransactionId == reference + testString).Message.Split('#');
            string idString = message[1];
            var refundTrans = new RefundTransPartial(_isTesting, _apiKey, reference, idString, amt);
            var response = refundTrans.Execute();

            return new TransactionResponse
            {
                Approved = response.Response.Status == "success" ? true : false,
                AuthCode = response.Response.ProcessorResponseCode,
                Message = $"{response.Response.TransStatusMsg}#{response.Response.Items.First().IdString}",
                TransactionId = response.Response.TransIdStr
            };
        }

        private void MarkBatchesAsComplete(List<BatchTransaction> batchTransactions)
        {
            foreach (var batch in batchTransactions.DistinctBy(x => x.BatchReference))
            {
                var checkedBatch = db.CheckedBatches.SingleOrDefault(bb => bb.BatchRef == batch.BatchReference);
                if (checkedBatch == null)
                {
                    db.CheckedBatches.InsertOnSubmit(
                        new CheckedBatch
                        {
                            BatchRef = batch.BatchReference,
                            CheckedX = DateTime.Now
                        });
                }
                else
                    checkedBatch.CheckedX = DateTime.Now;
            }
        }

        private void UpdateExistingTransactions(IQueryable<Transaction> approvedMatchingTransactions, Dictionary<string, BatchTransaction> unMatchedKeyedByReference)
        {
            foreach (var existingTransaction in approvedMatchingTransactions)
            {
                if (!unMatchedKeyedByReference.ContainsKey(existingTransaction.TransactionId))
                    continue;

                // first get the matching batch transaction.
                var batchTransaction = unMatchedKeyedByReference[existingTransaction.TransactionId];

                // get the adjusted settlement date
                var settlementDate = batchTransaction.SettledDate.AddHours(4);

                existingTransaction.Batch = settlementDate; // this date now will be the same as the settlement date.
                existingTransaction.Batchref = batchTransaction.BatchReference;
                existingTransaction.Batchtyp = batchTransaction.BatchType == BatchType.Ach ? "eft" : "bankcard";
                existingTransaction.Settled = settlementDate;
                existingTransaction.PaymentType = batchTransaction.BatchType == BatchType.Ach ? PaymentType.Ach : PaymentType.CreditCard;
                existingTransaction.LastFourCC = batchTransaction.BatchType == BatchType.CreditCard ? batchTransaction.LastDigits : null;
                existingTransaction.LastFourACH = batchTransaction.BatchType == BatchType.Ach ? batchTransaction.LastDigits : null;
            }
        }

        private void InsertMissingTransactions(IEnumerable<BatchTransaction> transactionsToInsert, BatchType batchType)
        {
            string testString = string.Empty;
            if (_isTesting)
                testString = "(testing)";

            foreach (var transactionToInsert in transactionsToInsert)
            {
                var notbefore = DateTime.Parse("6/1/12"); // the date when Sage payments began in BVCMS (?)
                // get the original transaction.
                var originalTransaction = db.Transactions.SingleOrDefault(t => t.TransactionId == transactionToInsert.Reference && transactionToInsert.TransactionDate >= notbefore && t.PaymentType == (batchType == BatchType.Ach ? PaymentType.Ach : PaymentType.CreditCard));

                // get the first and last name.
                string first, last;
                Util.NameSplit(transactionToInsert.Name, out first, out last);

                // get the settlement date, however we are not exactly sure why we add four hours to the settlement date.
                // we think it is to handle all timezones and push to the next day??
                var settlementDate = transactionToInsert.SettledDate.AddHours(4);

                // insert the transaction record.
                db.Transactions.InsertOnSubmit(new Transaction
                {
                    Name = transactionToInsert.Name,
                    First = first,
                    Last = last,
                    TransactionId = transactionToInsert.Reference + testString,
                    Amt = transactionToInsert.Amount,
                    Approved = transactionToInsert.Approved,
                    Message = transactionToInsert.Message,
                    TransactionDate = transactionToInsert.TransactionDate,
                    TransactionGateway = GatewayName,
                    Settled = settlementDate,
                    Batch = settlementDate, // this date now will be the same as the settlement date.
                    Batchref = transactionToInsert.BatchReference,
                    Batchtyp = transactionToInsert.BatchType == BatchType.Ach ? "eft" : "bankcard",
                    OriginalId = originalTransaction != null ? (originalTransaction.OriginalId ?? originalTransaction.Id) : (int?)null,
                    Fromsage = true,
                    Description = originalTransaction != null ? originalTransaction.Description : $"no description from {GatewayType}, id={transactionToInsert.TransactionId}",
                    PaymentType = transactionToInsert.BatchType == BatchType.Ach ? PaymentType.Ach : PaymentType.CreditCard,
                    LastFourCC = transactionToInsert.BatchType == BatchType.CreditCard ? transactionToInsert.LastDigits : null,
                    LastFourACH = transactionToInsert.BatchType == BatchType.Ach ? transactionToInsert.LastDigits : null
                });
            }
        }

        private BatchType GetBatchType(int paymentType)
        {
            switch (paymentType)
            {
                case 1:
                    return BatchType.CreditCard;
                case 2:
                    return BatchType.Ach;
                default:
                    return BatchType.Unknown;
            }
        }

        private TransactionType GetTransactionType(decimal amtProcessed)
        {
            if (amtProcessed > 0)
            {
                return TransactionType.Charge;
            }
            else
            {
                return TransactionType.Refund;
            }
        }

        private string GetBatchReference(DateTime start, DateTime end)
        {
            return $"{start.ToString("yyMMdd")}{end.ToString("yyMMdd")}{DateTime.Now.ToString("MMddhhmm")}";
        }

        private string GetAcceptivaPayerId(int peopleId)
        {
            var getPayerData = new GetPayerData(_isTesting, _apiKey, peopleId);
            var response = getPayerData.Execute();
            if (response.Response.Status != "success")
            {
                return null;
            }
            return response.Response.PayerIdStr;
        }

        private void StorePayerData(Person person, PaymentInfo paymentInfo, string cardNumber, string expires, string account, string routing)
        {
            //var creditCard = new CreditCard();
            //creditCard.CardNum = cardNumber;
            //creditCard.CardExpiration = expires;
            //var storePayer = new StorePayer();

            var storePayer = new StorePayer(
                _isTesting,
                _apiKey,
                new Payer
                {
                    LastName = paymentInfo.LastName ?? person.LastName,
                    FirstName = paymentInfo.FirstName ?? person.LastName,
                    Address = paymentInfo.Address ?? person.AddressLineOne,
                    Address2 = paymentInfo.Address2 ?? person.AddressLineTwo,
                    City = paymentInfo.City ?? person.CityName,
                    State = UPSStateCodes.FromStateCountry(paymentInfo.State ?? null, paymentInfo.Country, db) ?? paymentInfo.State,
                    Country = ISO3166.Alpha3FromName(paymentInfo.Country ?? person.CountryName) ?? null,
                    Zip = paymentInfo.Zip ?? person.PrimaryZip,
                    Email = person.EmailAddress,
                    Phone = paymentInfo.Phone ?? person.HomePhone ?? person.CellPhone
                },
                paymentInfo.AcceptivaPayerId,
                new CreditCard
                {
                    CardNum = cardNumber,
                    CardExpiration = expires
                },
                new Ach
                {
                    AchAccNum = account,
                    AchRoutingNum = routing
                });

            var response = storePayer.Execute();
            if (response.Response.Status != "success")
                throw new Exception(
                    $"Acceptiva failed to update the credit card for people id: {person.PeopleId}, responseCode: {response.Response.Errors.FirstOrDefault()?.ErrorNo}, responseText: {response.Response.Errors.FirstOrDefault()?.ErrorMsg}");
        }

        private string StoreNewPayerData(Person person, PaymentInfo paymentInfo, string cardNumber, string expires, string account, string routing)
        {
            var storePayer = new StoreNewPayer(
                _isTesting,
                _apiKey,
                new Payer
                {
                    LastName = paymentInfo.LastName ?? person.LastName,
                    FirstName = paymentInfo.FirstName ?? person.LastName,
                    Address = paymentInfo.Address ?? person.AddressLineOne,
                    Address2 = paymentInfo.Address2 ?? person.AddressLineTwo,
                    City = paymentInfo.City ?? person.CityName,
                    State = UPSStateCodes.FromStateCountry(paymentInfo.State ?? null, paymentInfo.Country, db) ?? paymentInfo.State,
                    Country = ISO3166.Alpha3FromName(paymentInfo.Country ?? person.CountryName) ?? null,
                    Zip = paymentInfo.Zip ?? person.PrimaryZip,
                    Email = person.EmailAddress,
                    Phone = paymentInfo.Phone ?? person.HomePhone ?? person.CellPhone
                },
                person.PeopleId.ToString(),
                new CreditCard
                {
                    CardNum = cardNumber,
                    CardExpiration = expires
                },
                new Ach
                {
                    AchAccNum = account,
                    AchRoutingNum = routing
                });

            var response = storePayer.Execute();
            if (response.Response.Status != "success")
                throw new Exception(
                    $"Acceptiva failed to update the credit card for people id: {person.PeopleId}, responseCode: {response.Response.Errors.FirstOrDefault()?.ErrorNo}, responseText: {response.Response.Errors.FirstOrDefault()?.ErrorMsg}");

            return response.Response.PayerIdStr;
        }

        private string AchType(int? pid)
        {
            var type = "checking";
            if (pid.HasValue)
            {
                var usesaving = MultipleGatewayUtils.Setting(db, "UseSavingAccounts", (int)ProcessType);
                if (usesaving)
                {
                    if (Person.GetExtraValue(db, pid.Value, "AchSaving")?.BitValue == true)
                        type = "savings";
                }
            }
            return type;
        }
    }
}
