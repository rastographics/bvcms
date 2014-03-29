using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Text;
using CmsData.Registration;
using UtilityExtensions;
using System.Web.Mvc;
using System.Xml.Linq;
using CmsData.Codes;
using System.Configuration;
using CmsWeb.Code;

namespace CmsWeb.Models
{
	[Serializable]
	public class ManageGivingModel
	{
		public int pid { get; set; }
		public int orgid { get; set; }
		public DateTime? StartWhen { get; set; }
		public DateTime? StopWhen { get; set; }
		public string SemiEvery { get; set; }
		public int? Day1 { get; set; }
		public int? Day2 { get; set; }
		public int? EveryN { get; set; }
		public string Period { get; set; }
		public string Type { get; set; }
		public string Cardnumber { get; set; }
		public DateTime? NextDate { get; set; }
		public string Expires { get; set; }
		public string Cardcode { get; set; }
		public string Routing { get; set; }
		public string Account { get; set; }
		public bool testing { get; set; }
		public decimal total { get; set; }
		public string HeadingLabel { get; set; }
	    public string firstname { get; set; }
	    public string middleinitial { get; set; }
	    public string lastname { get; set; }
	    public string suffix { get; set; }
	    public string address { get; set; }
	    public string city { get; set; }
	    public string state { get; set; }
	    public string zip { get; set; }
	    public string phone { get; set; }

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
			NoEChecksAllowed = OnlineRegModel.GetTransactionGateway() != "sage";
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
	        firstname = pi.FirstName ?? person.FirstName;
	        middleinitial = (pi.MiddleInitial ?? person.MiddleName).Truncate(1);
	        lastname = pi.LastName ?? person.LastName;
	        suffix = pi.Suffix ?? person.SuffixCode;
	        address = pi.Address ?? person.PrimaryAddress;
	        city = pi.City ?? person.PrimaryCity;
	        state = pi.State ?? person.PrimaryState;
	        zip = pi.Zip ?? person.PrimaryZip;
	        phone = pi.Phone ?? person.HomePhone ?? person.CellPhone;

		    total = FundItem.Sum(ff => ff.Value) ?? 0;
		}

		public void ValidateModel(ModelStateDictionary ModelState)
		{
			if (Type == "C")
				Payments.ValidateCreditCardInfo(ModelState, Cardnumber, Expires, Cardcode);
			else if (Type == "B")
				Payments.ValidateBankAccountInfo(ModelState, Routing, Account);
			else
				ModelState.AddModelError("Type", "Must select Bank Account or Credit Card");
			if (SemiEvery == "S")
			{
				if (!Day1.HasValue || !Day2.HasValue)
					ModelState.AddModelError("Semi", "Both Days must have values");
				else if (Day2 > 31)
					ModelState.AddModelError("Semi", "Day2 must be 31 or less");
				else if (Day1 >= Day2)
					ModelState.AddModelError("Semi", "Day1 must be less than Day2");
			}
			else if (SemiEvery == "E")
			{
				if (!EveryN.HasValue || EveryN < 1)
					ModelState.AddModelError("Every", "Days must be > 0");
			}
			else
				ModelState.AddModelError("Every", "Must Choose Payment Frequency");
			if (!StartWhen.HasValue)
				ModelState.AddModelError("StartWhen", "StartDate must have a value");
			else if (StartWhen <= DateTime.Today)
				ModelState.AddModelError("StartWhen", "StartDate must occur after today");
			else if (StopWhen.HasValue && StopWhen <= StartWhen)
				ModelState.AddModelError("StopWhen", "StopDate must occur after StartDate");

            if(!firstname.HasValue())
				ModelState.AddModelError("firstname", "needs name");
            if(!lastname.HasValue())
				ModelState.AddModelError("firstname", "needs name");
            if(!address.HasValue())
				ModelState.AddModelError("address", "Needs address");
            if(!city.HasValue())
				ModelState.AddModelError("city", "Needs city");
            if(!state.HasValue())
				ModelState.AddModelError("state", "Needs state");
            if(!zip.HasValue())
				ModelState.AddModelError("zip", "Needs zip");
            if(!phone.HasValue())
				ModelState.AddModelError("phone", "Needs phone");
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
	}
}
