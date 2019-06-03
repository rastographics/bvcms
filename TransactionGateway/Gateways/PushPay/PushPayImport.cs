using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CmsData;
using CmsData.Codes;
using TransactionGateway;
using TransactionGateway.ApiModels;
using UtilityExtensions;
using Organization = TransactionGateway.ApiModels.Organization;

namespace PushPay
{
    class PushPayImport
    {
        private const string PushPayKey = "PushPayKey";
        private const string PushPayTransactionNum = "PushPay Transaction #";
        private PushpayConnection pushpay;
        private CMSDataContext db;

        private DateTime StartDate;
        private bool ImportUnreconciled;

        public PushPayImport(string dbname, string connstr)
        {
            var cb = new SqlConnectionStringBuilder(connstr) { InitialCatalog = dbname };
            var host = dbname.Split(new char[] { '_' }, 2)[1];
            db = CMSDataContext.Create(cb.ConnectionString, host);
        }

        public async Task<int> Run()
        {
            if (db.Setting("PushPayEnableImport"))
            {
#if DEBUG
                Console.WriteLine("Running PushPay Import for " + db.Host);
#endif
                string access_token, refresh_token;
                access_token = db.GetSetting("PushpayAccessToken", "");
                refresh_token = db.GetSetting("PushpayRefreshToken", "");
                pushpay = new PushpayConnection(access_token, refresh_token, db);
                ImportUnreconciled = db.Setting("PushPayImportUnreconciled");
            }
            else
            {
                return 0;
            }
#if DEBUG
            try
            {
                return await RunInternal();
            }
            catch (Exception ex)
            {
                Console.WriteLine("PushPay error");
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
                Environment.Exit(1);
                return 0;
            }
#else
            return await RunInternal();
#endif
        }

        private async Task<int> RunInternal()
        {
            int Count = 0;
            var organizations = await pushpay.GetOrganizations();
            foreach (Organization org in organizations)
            {
                try
                {
                    int OnPaymentPage = 0;
                    bool HasPaymentsToProcess = true;

                    Init(org.Key);

                    while (HasPaymentsToProcess)
                    {
                        PaymentList payments = await pushpay.GetPaymentsForOrganizationSince(org.Key, StartDate, OnPaymentPage);
                        int PaymentPages = (payments.TotalPages.HasValue ? (int)payments.TotalPages : 1);

                        foreach (Payment payment in payments.Items)
                        {
                            var merchantKey = payment.Recipient.Key;
                            if (!TransactionAlreadyImported(payment))
                            {
                                // determine the batch to put the payment in
                                BundleHeader bundle;
                                if (payment.Batch?.Key.HasValue() == true)
                                {
                                    bundle = await ResolveBatch(payment.Batch, merchantKey);
                                }
                                else if (payment.Settlement?.Key.HasValue() == true)
                                {
                                    bundle = await ResolveSettlement(payment.Settlement);
                                }
                                else
                                {
                                    if (ImportUnreconciled)
                                    {
                                        // create a new bundle for each payment not part of a PushPay batch or settlement
                                        bundle = CreateBundle(payment.CreatedOn.ToLocalTime(), payment.Amount.Amount, null, null, payment.TransactionId, BundleReferenceIdTypeCode.PushPayStandaloneTransaction);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                Console.WriteLine("Importing payment " + payment.TransactionId);

                                // resolve the payer, fund, and payment
                                int? PersonId = ResolvePersonId(payment.Payer);
                                ContributionFund fund = ResolveFund(payment.Fund);
                                Contribution contribution = ResolvePayment(payment, fund, PersonId, bundle);

                                // mark this payment as imported
                                RecordImportProgress(org, bundle, contribution, payment);
                                Count++;
                            }
                        }
                        // done with this page of payments, see if there's more
                        if (PaymentPages > OnPaymentPage + 1)
                        {
                            OnPaymentPage++;
                        }
                        else
                        {
                            HasPaymentsToProcess = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    string timestamp = DateTime.UtcNow.ToString("yyyy-dd-MM-HH-mm-ss-fff");
                    string exception = $"{org.Key}\n{e.ToString()}";
                    File.WriteAllText($"PushPay{timestamp}-{db.Host}.txt", exception);
                }
            }
            return Count;
        }

        private Contribution ResolvePayment(Payment payment, ContributionFund fund, int? PersonId, BundleHeader bundle)
        {
            // find/create a touchpoint contribution from a pushpay payment
            Contribution contribution;
            var result = ContributionWithPayment(payment);
            if (result.Any())
            {
                contribution = result.Single();
            }
            else
            {
                contribution = new Contribution
                {
                    PeopleId = PersonId,
                    ContributionDate = payment.CreatedOn.ToLocalTime(),
                    ContributionAmount = payment.Amount.Amount,
                    ContributionTypeId = (fund.NonTaxDeductible == true) ? ContributionTypeCode.NonTaxDed : ContributionTypeCode.Online,
                    ContributionStatusId = (payment.Amount.Amount >= 0) ? ContributionStatusCode.Recorded : ContributionStatusCode.Reversed,
                    Origin = ContributionOriginCode.PushPay,
                    CreatedDate = DateTime.Now,
                    FundId = fund.FundId,

                    MetaInfo = PushPayTransactionNum + payment.TransactionId
                };
                db.Contributions.InsertOnSubmit(contribution);
                db.SubmitChanges();

                // assign contribution to bundle
                BundleDetail bd = new BundleDetail
                {
                    BundleHeaderId = bundle.BundleHeaderId,
                    ContributionId = contribution.ContributionId,
                    CreatedBy = 0,
                    CreatedDate = DateTime.Now
                };
                db.BundleDetails.InsertOnSubmit(bd);
                db.SubmitChanges();
            }
            return contribution;
        }

        private IQueryable<Contribution> ContributionWithPayment(Payment payment)
        {
            IQueryable<Contribution> contributions = db.Contributions.AsQueryable();

            return
            from c in contributions
            where c.ContributionDate == payment.CreatedOn.ToLocalTime()
            where c.ContributionAmount == payment.Amount.Amount
            where c.MetaInfo == PushPayTransactionNum + payment.TransactionId
            select c;
        }

        private async Task<BundleHeader> ResolveBatch(Batch batch, string merchantKey)
        {
            // take a pushpay batch and find or create a touchpoint bundle
            IQueryable<BundleHeader> bundles = db.BundleHeaders.AsQueryable();

            var result = from b in bundles
                         where b.ReferenceId == batch.Key
                         where b.ReferenceIdType == BundleReferenceIdTypeCode.PushPayBatch
                         select b;
            if (result.Any())
            {
                int id = result.Select(b => b.BundleHeaderId).SingleOrDefault();
                return db.BundleHeaders.SingleOrDefault(b => b.BundleHeaderId == id);
            }
            else
            {
                if (batch.TotalAmount == null || batch.TotalAmount.Amount == 0)
                {
                    batch = await pushpay.GetBatch(merchantKey, batch.Key);
                }
                return CreateBundle(batch.CreatedOn.ToLocalTime(), batch.TotalAmount?.Amount, batch.TotalCashAmount?.Amount, batch.TotalCheckAmount?.Amount, batch.Key, BundleReferenceIdTypeCode.PushPayBatch);
            }
        }

        private async Task<BundleHeader> ResolveSettlement(Settlement settlement)
        {
            // take a pushpay settlement and find or create a touchpoint bundle
            IQueryable<BundleHeader> bundles = db.BundleHeaders.AsQueryable();

            var result = from b in bundles
                         where b.ReferenceId == settlement.Key
                         where b.ReferenceIdType == BundleReferenceIdTypeCode.PushPaySettlement
                         select b;
            if (result.Any())
            {
                int id = result.Select(b => b.BundleHeaderId).SingleOrDefault();
                return db.BundleHeaders.SingleOrDefault(b => b.BundleHeaderId == id);
            }
            else
            {
                if (settlement.TotalAmount == null || settlement.TotalAmount.Amount == 0)
                {
                    settlement = await pushpay.GetSettlement(settlement.Key);
                }
                return CreateBundle(settlement.EstimatedDepositDate.ToLocalTime(), settlement.TotalAmount?.Amount, null, null, settlement.Key, BundleReferenceIdTypeCode.PushPaySettlement);
            }
        }

        private BundleHeader CreateBundle(DateTime CreatedOn, decimal? BundleTotal, decimal? TotalCash, decimal? TotalChecks, string RefId, int? RefIdType)
        {
            // create a touchpoint bundle
            BundleHeader bh = new BundleHeader
            {
                ChurchId = 1,
                CreatedBy = 1,
                CreatedDate = CreatedOn,
                RecordStatus = false,
                BundleStatusId = BundleStatusCode.OpenForDataEntry,
                ContributionDate = CreatedOn,
                BundleHeaderTypeId = BundleTypeCode.Online,
                DepositDate = null,
                BundleTotal = BundleTotal,
                TotalCash = TotalCash,
                TotalChecks = TotalChecks,
                ReferenceId = RefId,
                ReferenceIdType = RefIdType
            };
            db.BundleHeaders.InsertOnSubmit(bh);
            db.SubmitChanges();
            return bh;
        }

        private int? ResolvePersonId(Payer payer)
        {
            // take a pushpay payer and find or create a touchpoint person
            bool hasKey = payer.Key.HasValue();
            bool hasEmail = payer.emailAddress.HasValue();
            bool hasMobileNumber = payer.mobileNumber.HasValue();
            bool hasFirstAndLastName = payer.firstName.HasValue() && payer.lastName.HasValue();

            if (!hasKey &&
                !hasEmail &&
                !hasMobileNumber &&
                !hasFirstAndLastName)
            {
                // can't resolve - typically due to an anonymous donation
                return null;
            }

            IQueryable<Person> people = db.People.AsQueryable();

            // first look for an already established person link
            int? PersonId = null;

            if (hasKey)
            {
                PersonId = db.PeopleExtras.Where(p => p.Field == PushPayKey && p.StrValue == payer.Key).Select(p => p.PeopleId).SingleOrDefault();
                if (PersonId.HasValue && PersonId != 0)
                {
                    return PersonId;
                }
            }

            IQueryable<Person> result = null;
            Func<bool> hasResults = delegate () { return result != null && result.Any(); };

            if (hasEmail && hasFirstAndLastName)
            {
                result = people.Where(p => p.EmailAddress == payer.emailAddress
                    && p.FirstName == payer.firstName && p.LastName == payer.lastName);
            }

            if (hasEmail && !hasResults())
            {
                result = people.Where(p => p.EmailAddress == payer.emailAddress);
            }

            var cellPhone = payer.mobileNumber.GetDigits();
            if (hasMobileNumber && hasFirstAndLastName && !hasResults())
            {
                result = people.Where(p => p.CellPhone == cellPhone
                    && p.FirstName == payer.firstName && p.LastName == payer.lastName);
            }

            if (hasMobileNumber && !hasResults())
            {
                result = people.Where(p => p.CellPhone == cellPhone);
            }

            if (hasFirstAndLastName && !hasResults())
            {
                result = people.Where(p => p.FirstName == payer.firstName && p.LastName == payer.lastName);
            }

            if (hasResults())
            {
                PersonId = result.OrderBy(p => p.CreatedDate).Select(p => p.PeopleId).First();
            }
            else
            {
                Person person = Person.Add(db, null, payer.firstName, null, payer.lastName, null);
                person.EmailAddress = payer.emailAddress;
                person.CellPhone = payer.mobileNumber;
                person.Comments = "Added in context of PushPayImport because record was not found";
                db.SubmitChanges();
                PersonId = person.PeopleId;
            }
            // add extra value
            if (payer.Key.HasValue())
            {
                db.AddExtraValueData(PersonId, PushPayKey, payer.Key, null, null, null, null);
            }
            return PersonId;
        }

        private ContributionFund ResolveFund(Fund fund)
        {
            // take a pushpay fund and find or create a touchpoint fund
            IQueryable<ContributionFund> funds = db.ContributionFunds.AsQueryable();

            var result = from f in funds
                         where f.FundName == fund.Name
                         where f.FundStatusId > 0
                         orderby f.FundStatusId
                         orderby f.FundId descending
                         select f;
            if (result.Any())
            {
                int id = result.Select(f => f.FundId).First();
                return db.ContributionFunds.SingleOrDefault(f => f.FundId == id);
            }
            else
            {
                var max_id = from fn in funds
                             orderby fn.FundId descending
                             select fn.FundId + 1;
                int fund_id = max_id.FirstOrDefault();

                ContributionFund f = new ContributionFund
                {
                    FundId = fund_id,
                    FundName = fund.Name,
                    FundStatusId = 1,
                    CreatedDate = DateTime.Now.Date,
                    CreatedBy = 1,
                    FundDescription = fund.Name,
                    NonTaxDeductible = !fund.taxDeductible
                };
                db.ContributionFunds.InsertOnSubmit(f);
                db.SubmitChanges();
                return f;
            }
        }

        private bool TransactionAlreadyImported(Payment payment)
        {
            return ContributionWithPayment(payment).Any();
        }

        private bool TransactionAlreadyImported(string transactionId)
        {
            // check if a transaction has already been imported
            IQueryable<PushPayLog> logs = db.PushPayLogs.AsQueryable();

            var result = (from l in logs
                          where l.TransactionId == transactionId
                          select l).Any();
            return result;
        }

        private void Init(string orgkey)
        {
            // load initial import status so we can start where we left off
            var startDateSetting = db.Setting("PushPayImportStartDate", "");
            if (startDateSetting.HasValue() && DateTime.TryParse(startDateSetting, out StartDate))
            {
                db.SetSetting("PushPayImportStartDate", null);
                db.SubmitChanges();
                return;
            }

            IQueryable<PushPayLog> logs = db.PushPayLogs.AsQueryable();

            var result = (from l in logs
                          where l.OrganizationKey == orgkey
                          orderby l.TransactionDate descending
                          select l);

            if (result.Any())
            {
                StartDate = ((DateTime)result.Select(l => l.TransactionDate).First());
            }
            else
            {
                StartDate = new DateTime(1970, 1, 1);
            }
        }

        private void RecordImportProgress(Organization org, BundleHeader bundle, Contribution contribution, Payment payment)
        {
            // record our import status so that we can recover if need be; also useful for debugging
            PushPayLog log = new PushPayLog
            {
                // TouchPoint values
                BundleHeaderId = bundle.BundleHeaderId,
                ContributionId = contribution.ContributionId,

                // PushPay values
                OrganizationKey = org.Key,
                SettlementKey = payment.Settlement?.Key,
                BatchKey = payment.Batch?.Key,
                TransactionDate = payment.UpdatedOn,
                TransactionId = payment.TransactionId,

                ImportDate = DateTime.Now,
            };
            db.PushPayLogs.InsertOnSubmit(log);
            db.SubmitChanges();
        }
    }
}
