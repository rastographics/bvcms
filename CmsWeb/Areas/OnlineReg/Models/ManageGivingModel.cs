using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CmsData;
using System.Text;
using CmsData.Registration;
using UtilityExtensions;
using System.Web.Mvc;
using CmsWeb.Code;

namespace CmsWeb.Models
{
    [Serializable]
    public class ManageGivingModel
    {
        public int pid { get; set; }
        public int orgid { get; set; }
        [DisplayName("Start On or After")]
        public DateTime? StartWhen { get; set; }
        public DateTime? StopWhen { get; set; }
        public string SemiEvery { get; set; }
        [DisplayName("Day1 of Month")]
        public int? Day1 { get; set; }
        [DisplayName("Day2 of Month")]
        public int? Day2 { get; set; }
        [DisplayName("Recurring every")]
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

        private Dictionary<int, decimal?> _FundItem = new Dictionary<int, decimal?>();

        public Dictionary<int, decimal?> FundItem
        {
            get { return _FundItem; }
            set { _FundItem = value; }
        }

        public decimal? FundItemValue(int n)
        {
            if (FundItem.ContainsKey(n))
                return FundItem[n];
            return null;
        }

        [NonSerialized]
        private Person _Person;

        public Person person
        {
            get
            {
                if (_Person == null)
                    _Person = DbUtil.Db.LoadPersonById(pid);
                return _Person;
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
        private Settings setting;
        public Settings Setting
        {
            get { return setting ?? (setting = new Settings(Organization.RegSetting, DbUtil.Db, orgid)); }
        }
        private bool? usebootstrap;
        public bool UseBootstrap
        {
            get
            {
                if (!usebootstrap.HasValue)
                    usebootstrap = Setting.UseBootstrap;
                return usebootstrap.Value;
            }
        }

        public bool NoCreditCardsAllowed { get; set; }
        public bool NoEChecksAllowed { get; set; }
        public ManageGivingModel()
        {
            HeadingLabel = DbUtil.Db.Setting("ManageGivingHeaderLabel", "Giving Opportunities");
            //testing = ConfigurationManager.AppSettings["testing"].ToBool();
#if DEBUG2
            testing = true;
#endif
            NoCreditCardsAllowed = DbUtil.Db.Setting("NoCreditCardGiving", "false").ToBool();
            NoEChecksAllowed = DbUtil.Db.Setting("NoEChecksAllowed ", "false").ToBool();
        }

        public ManageGivingModel(int pid, int orgid = 0)
            : this()
        {
            this.pid = pid;
            this.orgid = orgid;
            var rg = person.ManagedGiving();
            var pi = person.PaymentInfo();
            if (rg != null && pi != null)
            {
                SemiEvery = rg.SemiEvery;
                StartWhen = rg.StartWhen;
                StopWhen = null; //rg.StopWhen;
                Day1 = rg.Day1;
                Day2 = rg.Day2;
                EveryN = rg.EveryN;
                Period = rg.Period;
                foreach (var ra in person.RecurringAmounts.AsEnumerable())
                    FundItem.Add(ra.FundId, ra.Amt);
                Cardnumber = pi.MaskedCard;
                Account = pi.MaskedAccount;
                Expires = pi.Expires;
                Cardcode = Util.Mask(new StringBuilder(pi.Ccv), 0);
                Routing = Util.Mask(new StringBuilder(pi.Routing), 2);
                NextDate = rg.NextDate;
                NoCreditCardsAllowed = DbUtil.Db.Setting("NoCreditCardGiving", "false").ToBool();
                Type = pi.PreferredGivingType;
                if (NoCreditCardsAllowed)
                    Type = "B"; // bank account only
                else if (NoEChecksAllowed)
                    Type = "C"; // credit card only
                Type = NoEChecksAllowed ? "C" : Type;
            }
            else if (Setting.ExtraValueFeeName.HasValue())
            {
                var f = CmsWeb.Models.OnlineRegPersonModel.Funds().SingleOrDefault(ff => ff.Text == Setting.ExtraValueFeeName);
                // reasonable defaults
                Period = "M";
                SemiEvery = "E";
                EveryN = 1;
                var evamt = person.GetExtra(Setting.ExtraValueFeeName).ToDecimal();
                if (f != null && evamt > 0)
                    FundItem.Add(f.Value.ToInt(), evamt);
            }
            if (pi == null)
                pi = new PaymentInfo();
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
            text = text.Replace("{name}", person.Name, ignoreCase: true);
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
            Util.SendMsg(Util.SysFromEmail, Util.Host, Util.TryGetMailAddress(DbUtil.Db.StaffEmailForOrg(orgid)),
                Setting.Subject, text,
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

        public void ValidateModel(ModelStateDictionary ModelState)
        {
            if (UseBootstrap)
            {
                var r = AddressVerify.LookupAddress(Address, "", "", "", Zip);
                if (r.Line1 != "error")
                {
                    if (r.found == false)
                    {
                        ModelState.AddModelError("Zip",
                            r.address + ", to skip address check, Change the country to USA, Not Validated");
                    }
                    if (r.Line1 != Address)
                        Address = r.Line1;
                    if (r.City != (City ?? ""))
                        City = r.City;
                    if (r.State != (State ?? ""))
                        State = r.State;
                    if (r.Zip != (Zip ?? ""))
                        Zip = r.Zip;
                }
            }
            bool dorouting = false;
            bool doaccount = Account.HasValue() && !Account.StartsWith("X");

            if (Routing.HasValue() && !Routing.StartsWith("X"))
                dorouting = true;

            if (doaccount || dorouting)
            {
                if (doaccount)
                    Account = Account.GetDigits();
                if (dorouting)
                    Routing = Routing.GetDigits();
            }

            if (Type == "C")
                Payments.ValidateCreditCardInfo(ModelState,
                    new PaymentForm
                    {
                        CreditCard = Cardnumber,
                        Expires = Expires,
                        CCV =
                            Cardcode,
                        SavePayInfo = true
                    });
            else if (Type == "B")
                Payments.ValidateBankAccountInfo(ModelState, Routing, Account);
            else
                ModelState.AddModelError("Type", "Must select Bank Account or Credit Card");
            if (SemiEvery == "S")
            {
                if (!Day1.HasValue || !Day2.HasValue)
                    ModelState.AddModelError("Day2", "Both Days must have values");
                else if (Day2 > 31)
                    ModelState.AddModelError("Day2", "Day2 must be 31 or less");
                else if (Day1 >= Day2)
                    ModelState.AddModelError("Day1", "Day1 must be less than Day2");
            }
            else if (SemiEvery == "E")
            {
                if (!EveryN.HasValue || EveryN < 1)
                    ModelState.AddModelError("EveryN", "Days must be > 0");
            }
            else
                ModelState.AddModelError("SemiEvery", "Must Choose Payment Frequency");
            if (!StartWhen.HasValue)
                ModelState.AddModelError("StartWhen", "StartDate must have a value");
            else if (StartWhen <= DateTime.Today)
                ModelState.AddModelError("StartWhen", "StartDate must occur after today");
//            else if (StopWhen.HasValue && StopWhen <= StartWhen)
//                ModelState.AddModelError("StopWhen", "StopDate must occur after StartDate");

            if (!FirstName.HasValue())
                ModelState.AddModelError("FirstName", "needs name");
            if (!LastName.HasValue())
                ModelState.AddModelError("LastName", "needs name");
            if (!Address.HasValue())
                ModelState.AddModelError("Address", "Needs address");
            if (!UseBootstrap)
            {
                if (!City.HasValue())
                    ModelState.AddModelError("City", "Needs city");
                if (!State.HasValue())
                    ModelState.AddModelError("State", "Needs state");
            }
            if (!Zip.HasValue())
                ModelState.AddModelError("Zip", "Needs zip");
            if (!Phone.HasValue())
                ModelState.AddModelError("Phone", "Needs phone");
        }
        public void Update()
        {
            var pi = person.PaymentInfo();
            if (Cardcode.HasValue() && Cardcode.Contains("X"))
                Cardcode = pi.Ccv;
            var gateway = OnlineRegModel.GetTransactionGateway();
            if (gateway == "authorizenet")
            {
                var au = new AuthorizeNet(DbUtil.Db, testing);
                au.AddUpdateCustomerProfile(pid,
                    Type,
                    Cardnumber,
                    Expires,
                    Cardcode,
                    Routing,
                    Account);
            }
            else if (gateway == "sage")
            {
                var sg = new SagePayments(DbUtil.Db, testing);
                sg.storeVault(pid,
                    Type,
                    Cardnumber,
                    Expires,
                    Cardcode,
                    Routing,
                    Account,
                    giving: true);
            }
            else
                throw new Exception("ServiceU not supported");

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

            pi.FirstName = FirstName.Truncate(50);
            pi.MiddleInitial = Middle.Truncate(10);
            pi.LastName = LastName.Truncate(50);
            pi.Suffix = Suffix.Truncate(10);
            pi.Address = Address.Truncate(50);
            pi.City = City.Truncate(50);
            pi.State = State.Truncate(10);
            pi.Zip = Zip.Truncate(15);
            pi.Phone = Phone.Truncate(25);

            DbUtil.Db.ExecuteCommand("DELETE dbo.RecurringAmounts WHERE PeopleId = {0}", pid);
            foreach (var c in FundItemsChosen())
            {
                var ra = new RecurringAmount
                {
                    PeopleId = pid,
                    FundId = c.fundid,
                    Amt = c.amt
                };
                DbUtil.Db.RecurringAmounts.InsertOnSubmit(ra);
            }
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
            var items = OnlineRegPersonModel.Funds();
            var q = from i in FundItem
                    join m in items on i.Key equals m.Value.ToInt()
                    where i.Value.HasValue
                    select new FundItemChosen { fundid = m.Value.ToInt(), desc = m.Text, amt = i.Value.Value };
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
        private string GetThankYouMessage(string def)
        {
            var msg = Util.PickFirst(setting.ThankYouMessage, def);
            return msg;
        }
        public string AutocompleteOnOff
        {
            get
            {
                return Util.IsDebug() ? "on" : "off";
            }
        }
    }
}
