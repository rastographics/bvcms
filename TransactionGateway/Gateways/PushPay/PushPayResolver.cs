using CmsData;
using CmsData.Codes;
using CmsData.Finance;
using System;
using System.Linq;
using System.Threading.Tasks;
using TransactionGateway.ApiModels;
using UtilityExtensions;

namespace TransactionGateway
{
    public class PushpayResolver
    {
        private CMSDataContext _db;
        private PushpayConnection _pushpay;
        private const string PushPayKey = "PushPayKey";
        private const string PushPayTransactionNum = "PushPay Transaction #";

        public PushpayResolver(PushpayConnection pushpay, CMSDataContext db)
        {
            _db = db;
            _pushpay = pushpay;
        }

        public ContributionFund ResolveFund(Fund fund)
        {
            // take a pushpay fund and find or create a touchpoint fund
            IQueryable<ContributionFund> funds = _db.ContributionFunds.AsQueryable();

            var result = from f in funds
                         where f.FundName == fund.Name
                         where f.FundStatusId > 0
                         orderby f.FundStatusId
                         orderby f.FundId descending
                         select f;
            if (result.Any())
            {
                int id = result.Select(f => f.FundId).First();
                return _db.ContributionFunds.SingleOrDefault(f => f.FundId == id);
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
                _db.ContributionFunds.InsertOnSubmit(f);
                _db.SubmitChanges();
                return f;
            }
        }

        public string ResolvePayerKey(int PeopleId)
        {
            PeopleExtra pe = _db.PeopleExtras.Where(c => c.PeopleId == PeopleId && c.Field == "PushPayKey").FirstOrDefault();
            if (pe == null) return null;                     
            return  pe.StrValue;
        }

        public int? ResolvePersonId(Payer payer)
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

            IQueryable<Person> people = _db.People.AsQueryable();

            // first look for an already established person link
            int? PersonId = null;

            if (hasKey)
            {
                PersonId = _db.PeopleExtras.Where(p => p.Field == PushPayKey && p.StrValue == payer.Key).Select(p => p.PeopleId).SingleOrDefault();
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
                Person person = Person.Add(_db, null, payer.firstName, null, payer.lastName, null);
                person.EmailAddress = payer.emailAddress;
                person.CellPhone = payer.mobileNumber;
                person.Comments = "Added in context of PushPayImport because record was not found";
                _db.SubmitChanges();
                PersonId = person.PeopleId;
            }
            // add extra value
            if (payer.Key.HasValue())
            {
                _db.AddExtraValueData(PersonId, PushPayKey, payer.Key, null, null, null, null);
            }
            return PersonId;
        }

        public async Task<BundleHeader> ResolveSettlement(Settlement settlement)
        {
            // take a pushpay settlement and find or create a touchpoint bundle
            IQueryable<BundleHeader> bundles = _db.BundleHeaders.AsQueryable();

            var result = from b in bundles
                         where b.ReferenceId == settlement.Key
                         where b.ReferenceIdType == BundleReferenceIdTypeCode.PushPaySettlement
                         select b;
            if (result.Any())
            {
                int id = result.Select(b => b.BundleHeaderId).SingleOrDefault();
                return _db.BundleHeaders.SingleOrDefault(b => b.BundleHeaderId == id);
            }
            else
            {
                if (settlement.TotalAmount == null || settlement.TotalAmount.Amount == 0)
                {
                    settlement = await _pushpay.GetSettlement(settlement.Key);
                }
                return CreateBundle(settlement.EstimatedDepositDate.ToLocalTime(), settlement.TotalAmount?.Amount, null, null, settlement.Key, BundleReferenceIdTypeCode.PushPaySettlement);
            }
        }

        public BundleHeader CreateBundle(DateTime CreatedOn, decimal? BundleTotal, decimal? TotalCash, decimal? TotalChecks, string RefId, int? RefIdType)
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
            _db.BundleHeaders.InsertOnSubmit(bh);
            _db.SubmitChanges();
            return bh;
        }

        public Transaction ResolveTransaction(Payment payment, int personId, int orgId, string des)
        {
            Person person = _db.LoadPersonById(personId);
            var ti = new Transaction
            {
                TransactionId = payment.TransactionId,
                Name = person.Name,
                First = person.FirstName,
                MiddleInitial = person.MiddleName.IsNotNull() ? person.MiddleName[0].ToString() : String.Empty,
                Last = person.LastName,
                Suffix = person.SuffixCode,
                Donate = null,
                Amtdue = 0,
                Amt = payment.Amount.Amount,
                Emails = person.EmailAddress,
                Testing = false,
                Description = $"Pushpay {des}",
                OrgId = orgId,
                Url = null,
                Address = person.AddressLineOne,
                TransactionGateway = "pushpay",
                City = person.CityName,
                State = person.StateCode,
                Zip = person.ZipCode,
                DatumId = null,
                Phone = person.HomePhone,
                OriginalId = null,
                Financeonly = null,
                TransactionDate = Util.Now,
                PaymentType = payment.PaymentMethodType == "CreditCard" ? PaymentType.CreditCard : PaymentType.Ach,
                LastFourCC =
                    payment.PaymentMethodType == "CreditCard" ? payment.Card.Reference.Substring(payment.Card.Reference.Length - 4) : null,
                LastFourACH = null,
                Approved = true
            };

            ti.TransactionPeople.Add(new TransactionPerson()
            {
                PeopleId = personId,
                OrgId = orgId,
                Amt = payment.Amount.Amount
            });

            _db.Transactions.InsertOnSubmit(ti);
            _db.SubmitChanges();

            return ti;
        }

        private bool TransactionAlreadyImported(string transactionId)
        {
            // check if a transaction has already been imported
            IQueryable<PushPayLog> logs = _db.PushPayLogs.AsQueryable();

            var result = (from l in logs
                          where l.TransactionId == transactionId
                          select l).Any();
            return result;
        }

        public Contribution ResolvePayment(Payment payment, ContributionFund fund, int? PersonId, BundleHeader bundle)
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
                _db.Contributions.InsertOnSubmit(contribution);
                _db.SubmitChanges();

                // assign contribution to bundle
                BundleDetail bd = new BundleDetail
                {
                    BundleHeaderId = bundle.BundleHeaderId,
                    ContributionId = contribution.ContributionId,
                    CreatedBy = 0,
                    CreatedDate = DateTime.Now
                };
                _db.BundleDetails.InsertOnSubmit(bd);
                _db.SubmitChanges();
            }
            return contribution;
        }

        private IQueryable<Contribution> ContributionWithPayment(Payment payment)
        {
            IQueryable<Contribution> contributions = _db.Contributions.AsQueryable();

            return
            from c in contributions
            where c.ContributionDate == payment.CreatedOn.ToLocalTime()
            where c.ContributionAmount == payment.Amount.Amount
            where c.MetaInfo == PushPayTransactionNum + payment.TransactionId
            select c;
        }

        public bool TransactionAlreadyImported(Payment payment)
        {
            return ContributionWithPayment(payment).Any();
        }
    }
}
