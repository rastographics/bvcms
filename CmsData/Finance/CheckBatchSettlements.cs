using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CmsData;
using CmsData.Finance;
using CmsData.View;
using MoreLinq;
using UtilityExtensions;

namespace CmsData.Finance
{
    public class CheckBatchedTransactions
    {
        public static void CheckBatchSettlements(CMSDataContext db, IGateway gateway, DateTime start, DateTime end)
        {
            var response = gateway.GetBatchDetails(start, end);

            // get distinct batches
            var allBatchReferences = (from batchTran in response.BatchTransactions
                                      select batchTran.BatchReference).Distinct();

            // first filter out batches that we have already been updated or inserted.
            // now find unmatched batch references
            var unmatchedBatchReferences = allBatchReferences.Where(br => !db.CheckedBatches.Any(tt => tt.BatchRef == br)).ToList();

            // given unmatched batch references, get the matched batch transactions again
            var unMatchedBatchTransactions =
                response.BatchTransactions.Where(x => unmatchedBatchReferences.Contains(x.BatchReference)).ToList();


            var batchTypes = unMatchedBatchTransactions.Select(x => x.BatchType).Distinct();

            foreach (var batchType in batchTypes)
            {
                // key it by transaction reference and payment type.
                var unMatchedKeyedByReference = unMatchedBatchTransactions.Where(x => x.BatchType == batchType).ToDictionary(x => x.Reference, x => x);

                // next let's get all the approved matching transactions from our transaction table by transaction id (reference).
                var approvedMatchingTransactions = from transaction in db.Transactions
                                                   where unMatchedKeyedByReference.Keys.Contains(transaction.TransactionId)
                                                   where (transaction.PaymentType == null || transaction.PaymentType == (batchType == BatchType.Ach ? PaymentType.Ach : PaymentType.CreditCard))
                                                   where transaction.Approved == true
                                                   select transaction;

                // next key the matching approved transactions that came from our transaction table by the transaction id (reference).
                var distinctTransactionIds = approvedMatchingTransactions.Select(x => x.TransactionId).Distinct();

                // finally let's get a list of all transactions that need to be inserted, which we don't already have.
                var transactionsToInsert = from transaction in unMatchedKeyedByReference
                                           where !distinctTransactionIds.Contains(transaction.Key)
                                           select transaction.Value;

                var notbefore = DateTime.Parse("6/1/12"); // the date when Sage payments began in BVCMS (?)

                // spin through each transaction and insert them to the transaction table.
                foreach (var transactionToInsert in transactionsToInsert)
                {
                    // get the original transaction.
                    var originalTransaction = db.Transactions.SingleOrDefault(t => t.TransactionId == transactionToInsert.Reference && transactionToInsert.TransactionDate >= notbefore && t.PaymentType == (batchType == BatchType.Ach ? PaymentType.Ach : PaymentType.CreditCard));

                    if (originalTransaction == null)
                    {
                        DbUtil.LogActivity(
                            string.Format("OriginalTransactionNotFoundWithReference {0} and Batch {1}",
                            transactionToInsert.Reference, transactionToInsert.BatchReference));
                    }

                    // get the first and last name.
                    string first, last;
                    Util.NameSplit(transactionToInsert.Name, out first, out last);

                    // get the settlement date, however we are not exactly sure why we add four hours to the settlement date.
                    // we think it is to handle all timezones and push to the next day??
                    var settlementDate = AdjustSettlementDateForAllTimeZones(transactionToInsert.SettledDate);

                    // insert the transaction record.
                    db.Transactions.InsertOnSubmit(new Transaction
                    {
                        Name = transactionToInsert.Name,
                        First = first,
                        Last = last,
                        TransactionId = transactionToInsert.Reference,
                        Amt = transactionToInsert.TransactionType == TransactionType.Credit ||
                              transactionToInsert.TransactionType == TransactionType.Refund
                            ? -transactionToInsert.Amount
                            : transactionToInsert.Amount,
                        Approved = transactionToInsert.Approved,
                        Message = transactionToInsert.Message,
                        TransactionDate = transactionToInsert.TransactionDate,
                        TransactionGateway = gateway.GatewayType,
                        Settled = settlementDate,
                        Batch = settlementDate, // this date now will be the same as the settlement date.
                        Batchref = transactionToInsert.BatchReference,
                        Batchtyp = transactionToInsert.BatchType == BatchType.Ach ? "eft" : "bankcard",
                        OriginalId = originalTransaction != null ? (originalTransaction.OriginalId ?? originalTransaction.Id) : (int?) null,
                        Fromsage = true,
                        Description = originalTransaction != null ? originalTransaction.Description : $"no description from {gateway.GatewayType}, id={transactionToInsert.TransactionId}",
                        PaymentType = transactionToInsert.BatchType == BatchType.Ach ? PaymentType.Ach : PaymentType.CreditCard,
                        LastFourCC = transactionToInsert.BatchType == BatchType.CreditCard ? transactionToInsert.LastDigits : null,
                        LastFourACH = transactionToInsert.BatchType == BatchType.Ach ? transactionToInsert.LastDigits : null
                    });
                }

                // next update Existing transactions with new batch data if there are any.
                foreach (var existingTransaction in approvedMatchingTransactions)
                {
                    if (!unMatchedKeyedByReference.ContainsKey(existingTransaction.TransactionId))
                        continue;

                    // first get the matching batch transaction.
                    var batchTransaction = unMatchedKeyedByReference[existingTransaction.TransactionId];

                    // get the adjusted settlement date
                    var settlementDate = AdjustSettlementDateForAllTimeZones(batchTransaction.SettledDate);

                    existingTransaction.Batch = settlementDate; // this date now will be the same as the settlement date.
                    existingTransaction.Batchref = batchTransaction.BatchReference;
                    existingTransaction.Batchtyp = batchTransaction.BatchType == BatchType.Ach ? "eft" : "bankcard";
                    existingTransaction.Settled = settlementDate;
                    existingTransaction.PaymentType = batchTransaction.BatchType == BatchType.Ach ? PaymentType.Ach : PaymentType.CreditCard;
                    existingTransaction.LastFourCC = batchTransaction.BatchType == BatchType.CreditCard ? batchTransaction.LastDigits : null;
                    existingTransaction.LastFourACH = batchTransaction.BatchType == BatchType.Ach ? batchTransaction.LastDigits : null;
                }
            }


            // finally we need to mark these batches as completed if there are any.
            foreach (var batch in unMatchedBatchTransactions.DistinctBy(x => x.BatchReference))
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

            db.SubmitChanges();
        }
        /// <summary>
        ///     we are not exactly sure why we add four hours to the settlement date
        ///     we think it is to handle all timezones and push to the next day??
        /// </summary>
        /// <param name="settlementDate"></param>
        /// <returns></returns>
        private static DateTime AdjustSettlementDateForAllTimeZones(DateTime settlementDate)
        {
            return settlementDate.AddHours(4);
        }

    }
}
