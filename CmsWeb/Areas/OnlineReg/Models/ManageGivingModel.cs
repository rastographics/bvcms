using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CmsData;
using CmsData.Finance;
using CmsData.Registration;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Models
{
    [Serializable]
    public class ManageGivingModel
    {
        public int pid { get; set; }
        public int orgid { get; set; }
        public string RepeatPattern { get; set; }

        [DisplayName("Start On or After")]
        public DateTime? StartWhen { get; set; }

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
        public string Cardnumber { get; set; }
        public DateTime? NextDate { get; set; }

        [DisplayName("Expires (MMYY)")]
        public string Expires { get; set; }

        public string Cardcode { get; set; }
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
        public string City { get; set; }
        public string State { get; set; }
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
        public Settings Setting
        {
            get { return _setting ?? (_setting = new Settings(Organization.RegSetting, DbUtil.Db, orgid)); }
        }

        private bool? _usebootstrap;
        public bool UseBootstrap
        {
            get
            {
                if (!_usebootstrap.HasValue)
                    _usebootstrap = Setting.UseBootstrap;
                return _usebootstrap.Value;
            }
        }

        public bool NoCreditCardsAllowed { get; set; }
        public bool NoEChecksAllowed { get; set; }

        public string SpecialGivingFundsHeader
        {
            get { return DbUtil.Db.Setting("SpecialGivingFundsHeader", "Special Giving Funds"); }
        }

        public bool HasManagedGiving
        {
            get { return person.ManagedGiving() != null; }
        }

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

        public ManageGivingModel(int pid, int orgid = 0)
            : this()
        {
            this.pid = pid;
            this.orgid = orgid;
            var rg = person.ManagedGiving();
            if (rg != null)
            {
                RepeatPattern = rg.SemiEvery != "S" ? rg.Period : rg.SemiEvery;
                SemiEvery = rg.SemiEvery;
                StartWhen = rg.StartWhen;
                StopWhen = null; //rg.StopWhen;
                Day1 = rg.Day1;
                Day2 = rg.Day2;
                EveryN = rg.EveryN;
                Period = rg.Period;
                foreach (var ra in person.RecurringAmounts.AsEnumerable())
                    FundItem.Add(ra.FundId, ra.Amt);

                NextDate = rg.NextDate;
            }
            else if (Setting.ExtraValueFeeName.HasValue())
            {
                var f = OnlineRegPersonModel.FullFundList().SingleOrDefault(ff => ff.Text == Setting.ExtraValueFeeName);
                // reasonable defaults
                RepeatPattern = "M";
                Period = "M";
                SemiEvery = "E";
                EveryN = 1;
                var evamt = person.GetExtra(Setting.ExtraValueFeeName).ToDecimal();
                if (f != null && evamt > 0)
                    FundItem.Add(f.Value.ToInt(), evamt);
            }

            var pi = person.PaymentInfo();
            if (pi == null)
                pi = new PaymentInfo();
            else
            {
                Cardnumber = pi.MaskedCard;
                Account = pi.MaskedAccount;
                Expires = pi.Expires;
                Cardcode = Util.Mask(new StringBuilder(pi.Ccv), 0);
                Routing = Util.Mask(new StringBuilder(pi.Routing), 2);
                NoCreditCardsAllowed = DbUtil.Db.Setting("NoCreditCardGiving", "false").ToBool();
                Type = pi.PreferredGivingType;
                if (NoCreditCardsAllowed)
                    Type = PaymentType.Ach; // bank account only
                else if (NoEChecksAllowed)
                    Type = PaymentType.CreditCard; // credit card only
                Type = NoEChecksAllowed ? PaymentType.CreditCard : Type;
            }

            FirstName = pi.FirstName ?? person.FirstName;
            Middle = (pi.MiddleInitial ?? person.MiddleName).Truncate(1);
            LastName = pi.LastName ?? person.LastName;
            Suffix = pi.Suffix ?? person.SuffixCode;
            Address = pi.Address ?? person.PrimaryAddress;
            City = pi.City ?? person.PrimaryCity;
            State = pi.State ?? person.PrimaryState;
            Zip = pi.Zip ?? person.PrimaryZip;
            Phone = pi.Phone ?? person.HomePhone ?? person.CellPhone;

            total = FundItem.Sum(ff => ff.Value) ?? 0;
        }

        public void Confirm(Controller controller)
        {
            var details = ViewExtensions2.RenderPartialViewToString(controller, "ManageGiving/EmailConfirmation", this);

            var staff = DbUtil.Db.StaffPeopleForOrg(orgid)[0];
            var text = Setting.Body.Replace("{church}", DbUtil.Db.Setting("NameOfChurch", "church"), ignoreCase: true);
//            text = text.Replace("{name}", person.Name, ignoreCase: true);
            text = text.Replace("{date}", DateTime.Now.ToString("d"), ignoreCase: true);
            text = text.Replace("{email}", person.EmailAddress, ignoreCase: true);
            text = text.Replace("{phone}", person.HomePhone.FmtFone(), ignoreCase: true);
            text = text.Replace("{contact}", staff.Name, ignoreCase: true);
            text = text.Replace("{contactemail}", staff.EmailAddress, ignoreCase: true);
            text = text.Replace("{contactphone}", Organization.PhoneNumber.FmtFone(), ignoreCase: true);
            text = text.Replace("{details}", details, ignoreCase: true);

            var contributionemail = (from ex in DbUtil.Db.PeopleExtras
                                     where ex.Field == "ContributionEmail"
                                     where ex.PeopleId == person.PeopleId
                                     select ex.Data).SingleOrDefault();
            if (!Util.ValidEmail(contributionemail))
                contributionemail = person.FromEmail;

            var from = Util.TryGetMailAddress(DbUtil.Db.StaffEmailForOrg(orgid));
            var mm = new EmailReplacements(DbUtil.Db, text, from);
            text = mm.DoReplacements(person);

            Util.SendMsg(Util.SysFromEmail, Util.Host, from, Setting.Subject, text,
                Util.EmailAddressListFromString(contributionemail), 0, pid);

            Util.SendMsg(Util.SysFromEmail, Util.Host, Util.TryGetMailAddress(contributionemail),
                "Managed Giving",
                "Managed giving for {0} ({1})".Fmt(person.Name, pid),
                Util.EmailAddressListFromString(DbUtil.Db.StaffEmailForOrg(orgid)),
                0, pid);

            var msg = GetThankYouMessage(@"<p>Thank you {first}, for managing your recurring giving</p>
<p>You should receive a confirmation email shortly.</p>");
            msg = msg.Replace("{first}", person.PreferredName, ignoreCase: true);
            ThankYouMessage = msg;
        }

        public string Title
        {
            get { return "Online Recurring Giving"; }
        }

        public string ThankYouMessage { get; set; }

        public void ValidateModel(ModelStateDictionary modelState)
        {
            var dorouting = false;
            var doaccount = Account.HasValue() && !Account.StartsWith("X");

            if (Routing.HasValue() && !Routing.StartsWith("X"))
                dorouting = true;

            if (doaccount || dorouting)
            {
                if (doaccount)
                    Account = Account.GetDigits();
                if (dorouting)
                    Routing = Routing.GetDigits();
            }

            if (Type == PaymentType.CreditCard)
                Payments.ValidateCreditCardInfo(modelState,
                    new PaymentForm
                    {
                        CreditCard = Cardnumber,
                        Expires = Expires,
                        CCV =
                            Cardcode,
                        SavePayInfo = true
                    });
            else if (Type == PaymentType.Ach)
                Payments.ValidateBankAccountInfo(modelState, Routing, Account);
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
            else if (StartWhen <= DateTime.Today)
                modelState.AddModelError("StartWhen", "StartDate must occur after today");
            //            else if (StopWhen.HasValue && StopWhen <= StartWhen)
            //                modelState.AddModelError("StopWhen", "StopDate must occur after StartDate");

            if (!FirstName.HasValue())
                modelState.AddModelError("FirstName", "Needs first name");
            if (!LastName.HasValue())
                modelState.AddModelError("LastName", "Needs last name");
            if (!Address.HasValue())
                modelState.AddModelError("Address", "Needs address");
            if (!City.HasValue())
                modelState.AddModelError("City", "Needs city");
            if (!State.HasValue())
                modelState.AddModelError("State", "Needs state");
            if (!Zip.HasValue())
                modelState.AddModelError("Zip", "Needs zip");
            if (!Phone.HasValue())
                modelState.AddModelError("Phone", "Needs phone");
        }

        public void Update()
        {
            // first check for total amount greater than zero.
            // if so we skip everything except updating the amounts.
            var chosenFunds = FundItemsChosen().ToList();
            if (chosenFunds.Sum(f => f.amt) > 0)
            {
                if (Cardcode.HasValue() && Cardcode.Contains("X"))
                {
                    var payinfo = person.PaymentInfo();
                    if (payinfo == null)
                        throw new Exception("X not allowed in CVV");
                    Cardcode = payinfo.Ccv;
                }

                var pi = person.PaymentInfo();
                if (pi == null)
                {
                    pi = new PaymentInfo();
                    person.PaymentInfos.Add(pi);
                }
                pi.SetBillingAddress(FirstName, Middle, LastName, Suffix, Address, City, State, Zip, Phone);

                // first need to do a $1 auth if it's a credit card and throw any errors we get back
                // from the gateway.
                var vaultSaved = false;
                var gateway = DbUtil.Db.Gateway(testing);
                if (Type == PaymentType.CreditCard)
                {
                    // perform $1 auth.
                    // need to randomize the $1 amount because some gateways will reject same auth amount
                    // subsequent times per user.
                    var random = new Random();
                    var dollarAmt = (decimal)random.Next(100, 199) / 100;

                    TransactionResponse transactionResponse;
                    if (Cardnumber.StartsWith("X"))
                    {
                        // store payment method in the gateway vault first before doing the auth.
                        gateway.StoreInVault(pid, Type, Cardnumber, Expires, Cardcode, Routing, Account, giving: true);
                        vaultSaved = true;
                        transactionResponse = gateway.AuthCreditCardVault(pid, dollarAmt, "Recurring Giving Auth", 0);
                    }
                    else
                        transactionResponse = gateway.AuthCreditCard(pid, dollarAmt, Cardnumber, Expires,
                                                                     "Recurring Giving Auth", 0, Cardcode, string.Empty,
                                                                     FirstName, LastName, Address, City, State, Zip, Phone);

                    if (!transactionResponse.Approved)
                        throw new Exception(transactionResponse.Message);
                }

                // store payment method in the gateway vault if not already saved.
                if (!vaultSaved)
                    gateway.StoreInVault(pid, Type, Cardnumber, Expires, Cardcode, Routing, Account, giving: true);

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

            DbUtil.Db.RecurringAmounts.DeleteAllOnSubmit(person.RecurringAmounts);
            DbUtil.Db.SubmitChanges();

            person.RecurringAmounts.AddRange(chosenFunds.Select(c => new RecurringAmount { FundId = c.fundid, Amt = c.amt }));
            DbUtil.Db.SubmitChanges();
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
                    select new FundItemChosen {fundid = m.Value.ToInt(), desc = m.Text, amt = i.Value.Value};
            return q;
        }

        public Decimal Total()
        {
            return FundItemsChosen().Sum(f => f.amt);
        }

        public object Autocomplete
        {
            get
            {
#if DEBUG
                return new {AUTOCOMPLETE = "on"};
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
                    @"
<div class=""instructions login"">{0}</div>
<div class=""instructions select"">{1}</div>
<div class=""instructions find"">{2}</div>
<div class=""instructions options"">{3}</div>
<div class=""instructions submit"">{4}</div>
<div class=""instructions special"">{5}</div>
<div class=""instructions sorry"">{6}</div>
"
                        .Fmt(Setting.InstructionLogin,
                            Setting.InstructionSelect,
                            Setting.InstructionFind,
                            Setting.InstructionOptions,
                            Setting.InstructionSubmit,
                            Setting.InstructionSpecial,
                            Setting.InstructionSorry
                        );
                ins = OnlineRegModel.DoReplaceForExtraValueCode(ins, person);
                return ins;
            }
        }

        public SelectList PeriodOptions()
        {
            if (UseBootstrap)
                return new SelectList(new Dictionary<string, string>
                {
                    {"M", "Month"},
                    {"W", "Week"},
                }, "Key", "Value");
            return new SelectList(new Dictionary<string, string>
            {
                {"M", "Month(s)"},
                {"W", "Week(s)"},
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

        public string AutocompleteOnOff
        {
            get { return Util.IsDebug() ? "on" : "off"; }
        }

        public bool ManagedGivingStopped { get; private set; }

        public void CancelManagedGiving(int peopleId)
        {
		    var p = DbUtil.Db.LoadPersonById(peopleId);
			DbUtil.Db.RecurringAmounts.DeleteAllOnSubmit(p.RecurringAmounts);

			var mg = p.ManagedGiving();
			if (mg != null)
				DbUtil.Db.ManagedGivings.DeleteOnSubmit(mg);

			DbUtil.Db.SubmitChanges();

            ManagedGivingStopped = true;
        }
    }
}
