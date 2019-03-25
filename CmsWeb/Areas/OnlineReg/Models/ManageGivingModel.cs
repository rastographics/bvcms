using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Classes;
using CmsData.Finance;
using CmsData.Registration;
using CmsWeb.Areas.OnlineReg.Controllers;
using CmsWeb.Code;
using Dapper;
using Microsoft.Scripting.Utils;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    [Serializable]
    public class ManageGivingModel
    {
        public int pid { get; set; }
        public int orgid { get; set; }

        public IList<string> DefaultFundIds = new List<string>();

        public IList<string> FallbackDefaultFundIds = new List<string>();

        public string FirstDefaultFundName { get; set; }

        public string RepeatPattern { get; set; }

        [DisplayName("Start On or After")]
        public DateTime? StartWhen { get; set; }

        public bool StartWhenIsNew { get; set; } = true;

        public DateTime? StopWhen { get; set; }
        public string SemiEvery { get; set; }

        [DisplayName("Day 1 of Month")]
        public int? Day1 { get; set; }

        [DisplayName("Day 2 of Month")]
        public int? Day2 { get; set; }

        [DisplayName("Repeat every")]
        public int? EveryN { get; set; }

        public string Period { get; set; }
        public string Type { get; set; }
        public string CreditCard { get; set; }
        public DateTime? NextDate { get; set; }

        [DisplayName("Expires (MMYY)")]
        public string Expires { get; set; }

        public string CVV { get; set; }
        public string Routing { get; set; }
        public string Account { get; set; }
        public bool testing { get; set; }
        public decimal total { get; set; }
        public string HeadingLabel { get; set; }
        public string FirstName { get; set; }
        public string Middle { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public IEnumerable<SelectListItem> Countries
        {
            get
            {
                var list = CodeValueModel.ConvertToSelect(CodeValueModel.GetCountryList().Where(c => c.Code != "NA"), null);
                list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "" });
                return list;
            }
        }

        public string Zip { get; set; }
        public string Phone { get; set; }

        private Dictionary<int, decimal?> _fundItem = new Dictionary<int, decimal?>();
        public Dictionary<int, decimal?> FundItem
        {
            get { return _fundItem; }
            set { _fundItem = value; }
        }

        public decimal? FundItemValue(int n)
        {
            if (FundItem.ContainsKey(n))
                return FundItem[n];
            return null;
        }

        [NonSerialized]
        private Person _person;
        public Person person
        {
            get
            {
                if (_person == null)
                    _person = DbUtil.Db.LoadPersonById(pid);
                return _person;
            }
        }

        [NonSerialized]
        private Organization _organization;
        public Organization Organization
        {
            get
            {
                if (_organization == null)
                    _organization = DbUtil.Db.Organizations.Single(d => d.OrganizationId == orgid);
                return _organization;
            }
        }

        [NonSerialized]
        private Settings _setting;
        public Settings Setting => _setting ?? (_setting = DbUtil.Db.CreateRegistrationSettings(orgid));

        public bool NoCreditCardsAllowed { get; set; }
        public bool NoEChecksAllowed { get; set; }

        public string SpecialGivingFundsHeader => DbUtil.Db.Setting("SpecialGivingFundsHeader", "Special Giving Funds");

        public bool HasManagedGiving => person?.ManagedGiving() != null;

        public ManageGivingModel()
        {
            HeadingLabel = DbUtil.Db.Setting("ManageGivingHeaderLabel", "Giving Opportunities");
            //testing = ConfigurationManager.AppSettings["testing"].ToBool();
#if DEBUG2
            testing = true;
#endif
            NoCreditCardsAllowed = DbUtil.Db.Setting("NoCreditCardGiving", "false").ToBool();
            NoEChecksAllowed = DbUtil.Db.Setting("NoEChecksAllowed", "false").ToBool();
        }

        public ManageGivingModel(int pid, int orgid = 0, string defaultFundIds = "")
            : this()
        {
            this.pid = pid;
            this.orgid = orgid;

            if (person == null)
                return;

            PopulateDefaultFundIds(defaultFundIds, person);

            var rg = person.ManagedGiving();
            if (rg != null)
                PopulateSetup(rg);
            else if (Util.HasValue(Setting.ExtraValueFeeName))
                PopulateExtraValueDefaults();
            else
                PopulateReasonableDefaults();
            
            var pi = PopulatePaymentInfo();
            PopulateBillingName(pi);
            PopulateBillingAddress(pi);

        }

        private void PopulateDefaultFundIds(string defaultFundIds, Person person)
        {
            if (string.IsNullOrWhiteSpace(defaultFundIds) && person.CampusId.HasValue)
            {
                // look up campus default fund mapping if present.
                var setting = $"DefaultCampusFunds-{person.CampusId}";
                var db = DbUtil.DbReadOnly;
                defaultFundIds = db.Setting(setting, string.Empty);
            }
            
            if (!string.IsNullOrWhiteSpace(defaultFundIds))
            {
                var list = defaultFundIds.Split(',');
                DefaultFundIds.AddRange(list);
            }

            if (DefaultFundIds.Any())
            {
                FirstDefaultFundName = OnlineRegPersonModel.GetFundName(DefaultFundIds.First().ToInt());
            }

            var fundList = OnlineRegPersonModel.FundList();
            FallbackDefaultFundIds.AddRange(fundList.Where(f => !DefaultFundIds.Contains(f.Value)).Select(f => f.Value));
        }

        private void PopulateBillingName(PaymentInfo pi)
        {
            FirstName = pi.FirstName ?? person.FirstName;
            Middle = (pi.MiddleInitial ?? person.MiddleName).Truncate(1);
            LastName = pi.LastName ?? person.LastName;
            Suffix = pi.Suffix ?? person.SuffixCode;
        }

        private void PopulateBillingAddress(PaymentInfo pi)
        {
            Address = pi.Address ?? person.PrimaryAddress;
            Address2 = pi.Address2 ?? person.PrimaryAddress2;
            City = pi.City ?? person.PrimaryCity;
            State = pi.State ?? person.PrimaryState;
            Country = pi.Country ?? person.PrimaryCountry;
            Zip = pi.Zip ?? person.PrimaryZip;
            Phone = pi.Phone ?? person.HomePhone ?? person.CellPhone;
        }

        private PaymentInfo PopulatePaymentInfo()
        {
            var pi = person.PaymentInfo();
            if (pi == null)
                return new PaymentInfo();

            CreditCard = pi.MaskedCard;
            Account = pi.MaskedAccount;
            Expires = pi.Expires;
            Routing = Util.Mask(new StringBuilder(pi.Routing), 2);
            NoCreditCardsAllowed = DbUtil.Db.Setting("NoCreditCardGiving", "false").ToBool();
            Type = pi.PreferredGivingType;
            if (NoCreditCardsAllowed)
                Type = PaymentType.Ach; // bank account only
            else if (NoEChecksAllowed)
                Type = PaymentType.CreditCard; // credit card only
            Type = NoEChecksAllowed ? PaymentType.CreditCard : Type;
            ClearMaskedNumbers(pi);
            total = FundItem.Sum(ff => ff.Value) ?? 0;

            return pi;
        }

        private void PopulateExtraValueDefaults()
        {
            var f = OnlineRegPersonModel.FullFundList().SingleOrDefault(ff => ff.Text == Setting.ExtraValueFeeName);
            PopulateReasonableDefaults();

            var evamt = person.GetExtra(Setting.ExtraValueFeeName).ToDecimal();
            if (f != null && evamt > 0)
                FundItem.Add(f.Value.ToInt(), evamt);
        }

        private void PopulateReasonableDefaults()
        {
            // reasonable defaults
            RepeatPattern = "M";
            Period = "M";
            SemiEvery = "E";
            EveryN = 1;
            StartWhen = DateTime.Today.AddDays(1);
        }

        private void PopulateSetup(ManagedGiving rg)
        {
            RepeatPattern = rg.SemiEvery != "S" ? rg.Period : rg.SemiEvery;
            SemiEvery = rg.SemiEvery;
            StartWhen = rg.StartWhen;
            StartWhenIsNew = false;
            StopWhen = null; //rg.StopWhen;
            Day1 = rg.Day1;
            Day2 = rg.Day2;
            EveryN = rg.EveryN;
            Period = rg.Period;
            foreach (var ra in person.RecurringAmounts.AsEnumerable())
                FundItem.Add(ra.FundId, ra.Amt);

            NextDate = rg.NextDate;
        }

        private void ClearMaskedNumbers(PaymentInfo pi)
        {
            var gateway = DbUtil.Db.Setting("TransactionGateway", "");

            var clearBankDetails = false;
            var clearCreditCardDetails = false;

            switch (gateway.ToLower())
            {
                case "sage":
                    clearBankDetails = !pi.SageBankGuid.HasValue;
                    clearCreditCardDetails = !pi.SageCardGuid.HasValue;
                    break;
                case "transnational":
                    clearBankDetails = !pi.TbnBankVaultId.HasValue;
                    clearCreditCardDetails = !pi.TbnCardVaultId.HasValue;
                    break;
                case "authorizenet":
                    clearBankDetails = !pi.AuNetCustPayBankId.HasValue;
                    clearCreditCardDetails = !pi.AuNetCustPayId.HasValue;
                    break;
                case "bluepay":
                    clearCreditCardDetails = !String.IsNullOrEmpty(pi.BluePayCardVaultId);
                    //TODO: Handle bank account too.
                    break;
            }

            if (clearBankDetails)
            {
                Account = string.Empty;
                Routing = string.Empty;
            }

            if (clearCreditCardDetails)
            {
                CreditCard = string.Empty;
                CVV = string.Empty;
                Expires = string.Empty;
            }
        }

        public void Confirm(Controller controller)
        {
            var details = ViewExtensions2.RenderPartialViewToString(controller, "ManageGiving/EmailConfirmation", this);

            var staff = DbUtil.Db.StaffPeopleForOrg(orgid);
            var from = staff[0];

            if (!string.IsNullOrEmpty(Setting.Body))
            {
                var text = Setting.Body.Replace("{church}", DbUtil.Db.Setting("NameOfChurch", "church"),
                    ignoreCase: true);
                //            text = text.Replace("{name}", person.Name, ignoreCase: true);
                text = text.Replace("{date}", DateTime.Now.ToString("d"), ignoreCase: true);
                text = text.Replace("{email}", person.EmailAddress, ignoreCase: true);
                text = text.Replace("{phone}", person.HomePhone.FmtFone(), ignoreCase: true);
                text = text.Replace("{contact}", from.Name, ignoreCase: true);
                text = text.Replace("{contactemail}", from.EmailAddress, ignoreCase: true);
                text = text.Replace("{contactphone}", Organization.PhoneNumber.FmtFone(), ignoreCase: true);
                text = text.Replace("{details}", details, ignoreCase: true);

                DbUtil.Db.EmailFinanceInformation(from.FromEmail, person, Setting.Subject, text);
            }

            DbUtil.Db.EmailFinanceInformation(from.FromEmail, staff, "Managed giving", $"Managed giving for {person.Name} ({pid}) {Util.Host}");

            var msg = GetThankYouMessage(@"<p>Thank you {first}, for managing your recurring giving</p>
<p>You should receive a confirmation email shortly.</p>");
            msg = msg.Replace("{first}", person.PreferredName, ignoreCase: true);
            ThankYouMessage = msg;
        }

        public string Title => "Online Recurring Giving";

        public string ThankYouMessage { get; set; }

        public void ValidateModel(ModelStateDictionary modelState)
        {
            var dorouting = false;
            var doaccount = Util.HasValue(Account) && !Account.StartsWith("X");

            if (Util.HasValue(Routing) && !Routing.StartsWith("X"))
                dorouting = true;

            if (doaccount || dorouting)
            {
                if (doaccount)
                    Account = Account.GetDigits();
                if (dorouting)
                    Routing = Routing.GetDigits();
            }

            if (Type == PaymentType.CreditCard)
                PaymentValidator.ValidateCreditCardInfo(modelState,
                    new PaymentForm
                    {
                        CreditCard = CreditCard,
                        Expires = Expires,
                        CVV = CVV,
                        SavePayInfo = true
                    });
            else if (Type == PaymentType.Ach)
                PaymentValidator.ValidateBankAccountInfo(modelState, Routing, Account);
            else
                modelState.AddModelError("Type", "Must select Bank Account or Credit Card");

            if (SemiEvery == "S")
            {
                if (!Day1.HasValue || !Day2.HasValue)
                    modelState.AddModelError("Day2", "Both Days must have values");
                else if (Day2 > 31)
                    modelState.AddModelError("Day2", "Day2 must be 31 or less");
                else if (Day1 >= Day2)
                    modelState.AddModelError("Day1", "Day1 must be less than Day2");
            }
            else if (SemiEvery == "E")
            {
                if (!EveryN.HasValue || EveryN < 1)
                    modelState.AddModelError("EveryN", "Days must be > 0");
            }
            else
                modelState.AddModelError("SemiEvery", "Must Choose Payment Frequency");

            if (!StartWhen.HasValue)
                modelState.AddModelError("StartWhen", "StartDate must have a value");
            else if (StartWhenIsNew && StartWhen <= DateTime.Today)
                modelState.AddModelError("StartWhen", "StartDate must occur after today");

            if (!Util.HasValue(FirstName))
                modelState.AddModelError("FirstName", "Needs first name");
            if (!Util.HasValue(LastName))
                modelState.AddModelError("LastName", "Needs last name");
            if (!Util.HasValue(Address))
                modelState.AddModelError("Address", "Needs address");
            if (!Util.HasValue(City))
                modelState.AddModelError("City", "Needs city");
            if (!Util.HasValue(State))
                modelState.AddModelError("State", "Needs state");
            if (!Util.HasValue(Country))
                modelState.AddModelError("Country", "Needs country");
            if (!Util.HasValue(Zip))
                modelState.AddModelError("Zip", "Needs zip");
            if (!Util.HasValue(Phone))
                modelState.AddModelError("Phone", "Needs phone");
        }

        public void Update()
        {
            var db = DbUtil.Db;
            // first check for total amount greater than zero.
            // if so we skip everything except updating the amounts.
            var chosenFunds = FundItemsChosen().ToList();
            if (chosenFunds.Sum(f => f.amt) > 0)
            {
                var pi = person.PaymentInfo();
                if (pi == null)
                {
                    pi = new PaymentInfo();
                    person.PaymentInfos.Add(pi);
                }
                pi.SetBillingAddress(FirstName, Middle, LastName, Suffix, Address, Address2, City, State, Country, Zip, Phone);

                // first need to do a $1 auth if it's a credit card and throw any errors we get back
                // from the gateway.
                var vaultSaved = false;
                var gateway = db.Gateway(testing);
                if (Type == PaymentType.CreditCard)
                {
                    // perform $1 auth.
                    // need to randomize the $1 amount because some gateways will reject same auth amount
                    // subsequent times per user.
                    var random = new Random();
                    var dollarAmt = (decimal)random.Next(100, 199) / 100;

                    TransactionResponse transactionResponse;
                    if (CreditCard.StartsWith("X"))
                    {
//                        // store payment method in the gateway vault first before doing the auth.
//                          If it starts with X, we should not be storing it in the vault.
//                        gateway.StoreInVault(pid, Type, CreditCard, Expires, CVV, Routing, Account, giving: true);
                        vaultSaved = true; // prevent it from saving later
                        transactionResponse = gateway.AuthCreditCardVault(pid, dollarAmt, "Recurring Giving Auth", 0);
                    }
                    else
                    {
                        var pf = new PaymentForm()
                        {
                            CreditCard = CreditCard,
                            First = FirstName,
                            Last = LastName
                        };
                        if(IsCardTester(pf, "ManagedGiving"))
                            throw new Exception("Found Card Tester");
                        transactionResponse = gateway.AuthCreditCard(pid, dollarAmt, CreditCard, Expires,
                            "Recurring Giving Auth", 0, CVV, string.Empty,
                            FirstName, LastName, Address, Address2, City, State, Country, Zip, Phone);
                    }

                    if (!transactionResponse.Approved)
                        throw new Exception(transactionResponse.Message);

                    // if we got this far that means the auth worked so now let's do a void for that auth.
                    var voidResponse = gateway.VoidCreditCardTransaction(transactionResponse.TransactionId);
                }

                // store payment method in the gateway vault if not already saved.
                if (!vaultSaved)
                    gateway.StoreInVault(pid, Type, CreditCard, Expires, CVV, Routing, Account, giving: true);

                // save all the managed giving data.
                var mg = person.ManagedGiving();
                if (mg == null)
                {
                    mg = new ManagedGiving();
                    person.ManagedGivings.Add(mg);
                }
                mg.SemiEvery = SemiEvery;
                mg.Day1 = Day1;
                mg.Day2 = Day2;
                mg.EveryN = EveryN;
                mg.Period = Period;
                mg.StartWhen = StartWhen;
                mg.StopWhen = StopWhen;
                mg.NextDate = mg.FindNextDate(DateTime.Today);
            }

            db.RecurringAmounts.DeleteAllOnSubmit(person.RecurringAmounts);
            db.SubmitChanges();

            person.RecurringAmounts.AddRange(chosenFunds.Select(c => new RecurringAmount { FundId = c.fundid, Amt = c.amt }));
            db.SubmitChanges();
        }

        private bool IsCardTester(PaymentForm pf, string from)
        {
            if (!Util.IsHosted || !pf.CreditCard.HasValue())
                return false;
            var hash = Pbkdf2Hasher.HashString(pf.CreditCard);
            var db = DbUtil.Db;
            db.InsertIpLog(HttpContextFactory.Current.Request.UserHostAddress, hash);

            if (pf.IsProblemUser())
                return OnlineRegController.LogRogueUser("Problem User", from);
            var iscardtester = ConfigurationManager.AppSettings["IsCardTester"];
            if (!iscardtester.HasValue())
            {
                return false;
            }
            var result = db.Connection.ExecuteScalar<string>(iscardtester, new {ip = HttpContextFactory.Current.Request.UserHostAddress});
            if(result.Equal("OK"))
                return false;
            return OnlineRegController.LogRogueUser(result, from);
        }

        public class FundItemChosen
        {
            public string desc { get; set; }
            public int fundid { get; set; }
            public decimal amt { get; set; }
        }

        public IEnumerable<FundItemChosen> FundItemsChosen()
        {
            if (FundItem == null)
                return new List<FundItemChosen>();
            var items = OnlineRegPersonModel.FullFundList();
            var q = from i in FundItem
                    join m in items on i.Key equals m.Value.ToInt()
                    where i.Value.HasValue
                    select new FundItemChosen { fundid = m.Value.ToInt(), desc = m.Text, amt = i.Value.Value };
            return q;
        }

        public decimal Total()
        {
            return FundItemsChosen().Sum(f => f.amt);
        }

        public object Autocomplete
        {
            get
            {
#if DEBUG
                return new { AUTOCOMPLETE = "on" };
#else
                return new { AUTOCOMPLETE = "off" };
#endif
            }
        }

        public string Instructions
        {
            get
            {
                var ins =
                    $@"
<div class=""instructions login"">{Setting.InstructionLogin}</div>
<div class=""instructions select"">{Setting.InstructionSelect}</div>
<div class=""instructions find"">{Setting.InstructionFind}</div>
<div class=""instructions options"">{Setting.InstructionOptions}</div>
<div class=""instructions submit"">{Setting.InstructionSubmit}</div>
<div class=""instructions special"">{Setting.InstructionSpecial}</div>
<div class=""instructions sorry"">{Setting.InstructionSorry}</div>
";
                ins = OnlineRegModel.DoReplaceForExtraValueCode(ins, person);
                return ins;
            }
        }

        public SelectList PeriodOptions()
        {
            return new SelectList(new Dictionary<string, string>
            {
                {"M", "Month"},
                {"W", "Week"},
            }, "Key", "Value");
        }

        public SelectList RepeatPatternOptions()
        {
            return new SelectList(new Dictionary<string, string>
            {
                {"M", "Monthly"},
                {"S", "Twice a Month"},
                {"W", "Weekly"}
            }, "Key", "Value");
        }

        public SelectList EveryNOptions()
        {
            return new SelectList(new Dictionary<string, string>
            {
                {"1", "1"},
                {"2", "2"},
                {"3", "3"},
                {"4", "4"},
                {"5", "5"},
                {"6", "6"},
                {"7", "7"},
                {"8", "8"},
                {"9", "9"},
                {"10", "10"},
                {"11", "11"},
                {"12", "12"},
                {"13", "13"},
                {"14", "14"},
                {"15", "15"},
                {"16", "16"},
                {"17", "17"},
                {"18", "18"},
                {"19", "19"},
                {"20", "20"},
                {"21", "21"},
                {"22", "22"},
                {"23", "23"},
                {"24", "24"},
                {"25", "25"},
                {"26", "26"},
                {"27", "27"},
                {"28", "28"},
                {"29", "29"},
                {"30", "30"},
            }, "Key", "Value");
        }

        private string GetThankYouMessage(string def)
        {
            var msg = Util.PickFirst(_setting.ThankYouMessage, def);
            return msg;
        }

        public string AutocompleteOnOff => Util.IsDebug() ? "on" : "off";

        public bool ManagedGivingStopped { get; private set; }

        public void CancelManagedGiving(int peopleId)
        {
            var db = DbUtil.Db;
            var p = db.LoadPersonById(peopleId);
            db.RecurringAmounts.DeleteAllOnSubmit(p.RecurringAmounts);

            var mg = p.ManagedGiving();
            if (mg != null)
                db.ManagedGivings.DeleteOnSubmit(mg);

            db.SubmitChanges();

            ManagedGivingStopped = true;
        }
        public void Log(string action)
        {
            DbUtil.LogActivity("OnlineReg ManageGiving " + action, orgid, pid);
        }

        public class RecurringForPerson
        {
            public int FundId { get; set; }
            public decimal Amt { get; set; }
            public string FundName { get; set; }
            public int FundStatusId { get; set; }
            public int? OnlineSort { get; set; }
        }

        public IEnumerable<RecurringForPerson> RecurringAmounts()
        {
        string sql = $@"
SELECT ra.FundId, ra.Amt, f.FundName, f.FundStatusId , f.OnlineSort 
FROM dbo.RecurringAmounts ra
JOIN dbo.ContributionFund f ON f.FundId = ra.FundId
WHERE ra.PeopleId = @pid
        ";
            return DbUtil.Db.Connection.Query<RecurringForPerson>(sql, new { pid});
        }
    }
}
