using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UtilityExtensions;
using System.Text;

namespace CmsData.Registration
{
    public class Settings
	{
		public List<Ask> AskItems { get; set; }
		public bool AskVisible(string name)
		{
			return AskItems.Find(aa => aa.Type == name) != null;
		}
		public decimal? Deposit { get; set; }
		public string Title { get; set; }
		public string Shell { get; set; }
		public string ShellBs { get; set; }
		public decimal? Fee { get; set; }
		public decimal? ExtraFee { get; set; }
		public decimal? MaximumFee { get; set; }
		public bool ApplyMaxToOtherFees { get; set; }
		public bool AllowOnlyOne { get; set; }
		public bool TargetExtraValues { get; set; }
		public bool AllowReRegister { get; set; }
		public bool AllowSaveProgress { get; set; }
		public bool OtherFeesAddedToOrgFee { get; set; }
		public bool IncludeOtherFeesWithDeposit { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public string ReminderSubject { get; set; }
		public string ReminderBody { get; set; }
		public string SupportSubject { get; set; }
		public string SupportBody { get; set; }
		public string SenderSubject { get; set; }
		public string SenderBody { get; set; }
		public string Terms { get; set; }
		public string ConfirmationTrackingCode { get; set; }

		public bool MemberOnly { get; set; }
		public bool AskDonation { get; set; }
		public string SpecialScript { get; set; }
		public string DonationLabel { get; set; }
		public string ExtraValueFeeName { get; set; }
		public bool NoReqBirthYear { get; set; }
		public bool NotReqDOB { get; set; }
		public bool NotReqAddr { get; set; }
		public bool NotReqPhone { get; set; }
		public bool NotReqGender { get; set; }
		public bool NotReqMarital { get; set; }
		public bool NotReqZip { get; set; }
		public int? DonationFundId { get; set; }
        public string AccountingCode { get; set; }
		public int? TimeSlotLockDays { get; set; }
		public string GroupToJoin { get; set; }
	    public bool AddAsProspect { get; set; }

        public bool UseBootstrap
        {
            get { return org != null && (org.UseBootstrap ?? true); }
        }

        public string DonationFund()
		{
			return (from f in Db.ContributionFunds
					where f.FundId == DonationFundId
					select f.FundName).SingleOrDefault();
		}

		public string InstructionSelect { get; set; }
		public string InstructionFind { get; set; }
		public string InstructionOptions { get; set; }
		public string InstructionSpecial { get; set; }
		public string InstructionLogin { get; set; }
		public string InstructionSubmit { get; set; }
		public string InstructionSorry { get; set; }
		public string InstructionAll { get; set; }
		public string ThankYouMessage { get; set; }

		public List<OrgFee> OrgFees { get; set; }
		public List<AgeGroup> AgeGroups { get; set; }
		public TimeSlots TimeSlots { get; set; }
		public List<int> LinkGroupsFromOrgs { get; set; }
		public List<int> ValidateOrgIds { get; set; }

		public string ValidateOrgs
		{
			get { return string.Join(",", ValidateOrgIds); }
			set
			{
				if (value.HasValue())
					ValidateOrgIds = (from i in value.Split(',')
									  where i.ToInt() > 0 || i.ToInt() < 0
									  select i.ToInt()).ToList();
				else
					ValidateOrgIds = new List<int>();
			}
		}

		public class HtmlText2
		{
			public string Text { get; set; }
			public string Html { get; set; }
			public override string ToString()
			{
				if (Html.HasValue())
					return Html;
				return Text;
			}
		}
		public class AgeGroup
		{
			public int Id { get; set; }
			public string SmallGroup { get; set; }
			public int StartAge { get; set; }
			public int EndAge { get; set; }
			public decimal? Fee { get; set; }
		}

        public class OrgFee
        {
            public string Name;
            [DisplayName("Org Id")]
            public int? OrgId { get; set; }
            public decimal? Fee { get; set; }
        }

		public Organization org { get; set; }
		public int OrgId { get; set; }
		public CMSDataContext Db { get; set; }

	    public Settings()
		{
	        OrgFees = new List<OrgFee>();
			TimeSlots = new TimeSlots();
			AgeGroups = new List<AgeGroup>();
			LinkGroupsFromOrgs = new List<int>();
			ValidateOrgs = "";
			AskItems = new List<Ask>();
		}

		public Settings(string s, CMSDataContext Db, int OrgId)
			: this()
		{
			this.Db = Db;
			this.OrgId = OrgId;
			org = Db.LoadOrganizationById(OrgId);
			var parser = new Parser(s);

			while (parser.NextSection())
				ParseSection(parser);
			SetUniqueIds("AskDropdown");
			SetUniqueIds("AskExtraQuestions");
			SetUniqueIds("AskCheckboxes");
			SetUniqueIds("AskText");
			SetUniqueIds("AskMenu");
		    var sglist = new List<string>();
            AskItems.ForEach(a => sglist.AddRange(a.SmallGroups()));
            var q = sglist.GroupBy(mi => mi).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (q.Any())
                throw parser.GetException("Duplicate SmallGroup: " + string.Join(",", q));

			parser.data = null;
		}

		private void SetUniqueIds(string t)
		{
			var n = 0;
			foreach (var dd in AskItems.Where(aa => aa.Type == t))
				dd.UniqueId = n++;
		}

		private AskSize asksize; // To support old Registration Documents

		void ParseSection(Parser parser)
		{
			switch (parser.curr.kw)
			{
				case Parser.RegKeywords.AskParents:
				case Parser.RegKeywords.AnswersNotRequired:
				case Parser.RegKeywords.AskSMS:
				case Parser.RegKeywords.AskDoctor:
				case Parser.RegKeywords.AskInsurance:
				case Parser.RegKeywords.AskEmContact:
				case Parser.RegKeywords.AskAllergies:
				case Parser.RegKeywords.AskChurch:
				case Parser.RegKeywords.AskTylenolEtc:
				case Parser.RegKeywords.AskCoaching:
					AskItems.Add(Ask.ParseAsk(parser));
					break;
				case Parser.RegKeywords.SpecialScript:
					SpecialScript = parser.GetString();
					break;
				case Parser.RegKeywords.AskSuggestedFee:
					AskItems.Add(AskSuggestedFee.Parse(parser));
					break;
				case Parser.RegKeywords.AskTickets:
					AskItems.Add(AskTickets.Parse(parser));
					break;
				case Parser.RegKeywords.AskRequest:
					AskItems.Add(AskRequest.Parse(parser));
					break;
				case Parser.RegKeywords.AskHeader:
					AskItems.Add(AskHeader.Parse(parser));
					break;
				case Parser.RegKeywords.AskInstruction:
					AskItems.Add(AskInstruction.Parse(parser));
					break;
				case Parser.RegKeywords.Dropdown:
				case Parser.RegKeywords.AskOptions:
				case Parser.RegKeywords.ExtraOptions:
				case Parser.RegKeywords.Dropdown1:
				case Parser.RegKeywords.Dropdown2:
				case Parser.RegKeywords.Dropdown3:
					AskItems.Add(AskDropdown.Parse(parser));
					break;
				case Parser.RegKeywords.MenuItems:
					AskItems.Add(AskMenu.Parse(parser));
					break;
				case Parser.RegKeywords.ExtraQuestions:
					AskItems.Add(AskExtraQuestions.Parse(parser));
					break;
				case Parser.RegKeywords.Text:
					AskItems.Add(AskText.Parse(parser));
					break;
				case Parser.RegKeywords.Checkboxes:
				case Parser.RegKeywords.Checkboxes2:
					AskItems.Add(AskCheckboxes.Parse(parser));
					break;
				case Parser.RegKeywords.YesNoQuestions:
					AskItems.Add(AskYesNoQuestions.Parse(parser));
					break;
				case Parser.RegKeywords.AskGradeOptions:
				case Parser.RegKeywords.GradeOptions:
					AskItems.Add(AskGradeOptions.Parse(parser));
					break;
				case Parser.RegKeywords.AskSize:
					AskItems.Add(AskSize.Parse(parser));
					break;

				case Parser.RegKeywords.MemberOnly:
					MemberOnly = parser.GetBool();
					break;
				case Parser.RegKeywords.AskMedical:
					parser.GetBool();
					break;
				case Parser.RegKeywords.AskDonation:
					AskDonation = parser.GetBool();
					break;
				case Parser.RegKeywords.NoReqBirthYear:
				    NoReqBirthYear = parser.GetBool();
					break;
				case Parser.RegKeywords.NotReqDOB:
					NotReqDOB = parser.GetBool();
					break;
				case Parser.RegKeywords.NotReqAddr:
					NotReqAddr = parser.GetBool();
					break;
				case Parser.RegKeywords.NotReqPhone:
					NotReqPhone = parser.GetBool();
					break;
				case Parser.RegKeywords.NotReqGender:
					NotReqGender = parser.GetBool();
					break;
				case Parser.RegKeywords.NotReqMarital:
					NotReqMarital = parser.GetBool();
					break;
				case Parser.RegKeywords.NotReqZip:
					NotReqZip = parser.GetBool();
					break;
				case Parser.RegKeywords.DonationFundId:
					DonationFundId = parser.GetNullInt();
					break;
				case Parser.RegKeywords.AccountingCode:
				    AccountingCode = parser.GetString();
					break;
				case Parser.RegKeywords.DonationLabel:
					DonationLabel = parser.GetString();
					break;
				case Parser.RegKeywords.ExtraValueFeeName:
					ExtraValueFeeName = parser.GetString();
					break;
				case Parser.RegKeywords.GroupToJoin:
					GroupToJoin = parser.GetString();
					break;
				case Parser.RegKeywords.AddAsProspect:
					AddAsProspect = parser.GetBool();
					break;
				case Parser.RegKeywords.LinkGroupsFromOrgs:
					LinkGroupsFromOrgs = (from i in parser.curr.value.Split(',')
										  where i.ToInt() > 0
										  select i.ToInt()).ToList();
					parser.lineno++;
					break;
				case Parser.RegKeywords.ValidateOrgs:
					ValidateOrgs = parser.curr.value;
					parser.lineno++;
					break;
				case Parser.RegKeywords.Terms:
					ParseTerms(parser);
					break;
				case Parser.RegKeywords.Instructions:
					ParseInstructions(parser);
					break;
				case Parser.RegKeywords.Confirmation:
					ParseConfirmation(parser);
					break;
				case Parser.RegKeywords.ConfirmationTrackingCode:
                    ParseTrackingCode(parser);
					break;
				case Parser.RegKeywords.Reminder:
					ParseReminder(parser);
					break;
				case Parser.RegKeywords.SupportEmail:
					ParseSupport(parser);
					break;
				case Parser.RegKeywords.SenderEmail:
					ParseSender(parser);
					break;
				case Parser.RegKeywords.AgeGroups:
					ParseAgeGroups(parser);
					break;
				case Parser.RegKeywords.OrgMemberFees:
				case Parser.RegKeywords.OrgFees:
					ParseOrgFees(parser);
					break;
				case Parser.RegKeywords.VoteTags:
					ParseVoteTags(parser);
					break;
				case Parser.RegKeywords.Title:
					Title = parser.GetString();
					break;
				case Parser.RegKeywords.Shell:
					Shell = parser.GetString();
					break;
				case Parser.RegKeywords.ShellBs:
					ShellBs = parser.GetString();
					break;
				case Parser.RegKeywords.Fee:
					Fee = parser.GetDecimal();
					break;


// BEGIN support for old Registration Documents
				case Parser.RegKeywords.GiveOrgMembAccess:
					parser.GetBool();
					break;
				case Parser.RegKeywords.UseBootstrap:
					parser.GetBool();
					break;
				case Parser.RegKeywords.AskGrade:
					parser.GetBool();
					parser.GetLabel("Grade");
					break;

				case Parser.RegKeywords.AskShirtSize: 
					parser.GetBool();
					asksize = new AskSize();
					AskItems.Add(asksize);
					break;
				case Parser.RegKeywords.ShirtSizes:
					asksize.list = AskSize.ParseShirtSizes(parser);
					break;
				case Parser.RegKeywords.AllowLastYearShirt:
					asksize.AllowLastYear = parser.GetBool();
					break;
				case Parser.RegKeywords.ShirtFee:
                    if (asksize == null)
                    {
                        asksize = new AskSize();
                        AskItems.Add(asksize);
                    }
					asksize.Fee = parser.GetDecimal();
					break;
// END support for old Registration Documents

				case Parser.RegKeywords.Deposit:
					Deposit = parser.GetDecimal();
					break;
				case Parser.RegKeywords.ExtraFee:
					ExtraFee = parser.GetDecimal();
					break;
				case Parser.RegKeywords.MaximumFee:
					MaximumFee = parser.GetDecimal();
					break;
				case Parser.RegKeywords.AllowOnlyOne:
					AllowOnlyOne = parser.GetBool();
					break;
                case Parser.RegKeywords.OtherFeesAdded:
				case Parser.RegKeywords.OtherFeesAddedToOrgFee:
					OtherFeesAddedToOrgFee = parser.GetBool();
					break;
				case Parser.RegKeywords.IncludeOtherFeesWithDeposit:
					IncludeOtherFeesWithDeposit = parser.GetBool();
					break;
				case Parser.RegKeywords.ApplyMaxToOtherFees:
					ApplyMaxToOtherFees = parser.GetBool();
					break;
				case Parser.RegKeywords.AllowReRegister:
					AllowReRegister = parser.GetBool();
					break;
				case Parser.RegKeywords.AllowSaveProgress:
					AllowSaveProgress = parser.GetBool();
					break;
				case Parser.RegKeywords.TargetExtraValues:
					TargetExtraValues = parser.GetBool();
					break;
				case Parser.RegKeywords.TimeSlotLockDays:
					TimeSlotLockDays = parser.GetNullInt();
					break;
				case Parser.RegKeywords.TimeSlots:
					TimeSlots = TimeSlots.Parse(parser);
					if (TimeSlotLockDays.HasValue)
						TimeSlots.TimeSlotLockDays = TimeSlotLockDays;
					break;
			}
		}

		private void ParseTerms(Parser parser)
		{
			Terms = parser.GetString();
			if (Terms.HasValue() || parser.curr.indent == 0)
				return;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				switch (parser.curr.kw)
				{
					case Parser.RegKeywords.Html:
						parser.lineno++;
						break;
					case Parser.RegKeywords.Body:
						Terms = parser.GetString();
						break;
					default:
						throw parser.GetException("unexpected line");
				}
			}
		}
		private void ParseTrackingCode(Parser parser)
		{
			ConfirmationTrackingCode = parser.GetString();
			if (ConfirmationTrackingCode.HasValue() || parser.curr.indent == 0)
				return;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				switch (parser.curr.kw)
				{
					case Parser.RegKeywords.Html:
						parser.lineno++;
						break;
					case Parser.RegKeywords.Body:
						ConfirmationTrackingCode = parser.GetString();
						break;
					default:
						throw parser.GetException("unexpected line");
				}
			}
		}
		private void ParseConfirmation(Parser parser)
		{
			parser.lineno++;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				switch (parser.curr.kw)
				{
					case Parser.RegKeywords.Html:
						parser.lineno++;
						break;
					case Parser.RegKeywords.Subject:
						Subject = parser.GetString();
						break;
					case Parser.RegKeywords.Body:
						Body = parser.GetString();
						break;
					default:
						throw parser.GetException("unexpected line in Confirmation");
				}
			}
		}
		private void ParseReminder(Parser parser)
		{
			parser.lineno++;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				switch (parser.curr.kw)
				{
					case Parser.RegKeywords.Html:
						parser.lineno++;
						break;
					case Parser.RegKeywords.ReminderSubject:
						ReminderSubject = parser.GetString();
						break;
					case Parser.RegKeywords.ReminderBody:
						ReminderBody = parser.GetString();
						break;
					default:
						throw parser.GetException("unexpected line in Reminder");
				}
			}
		}
		private void ParseSupport(Parser parser)
		{
			parser.lineno++;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				switch (parser.curr.kw)
				{
					case Parser.RegKeywords.Html:
						parser.lineno++;
						break;
					case Parser.RegKeywords.SupportSubject:
						SupportSubject = parser.GetString();
						break;
					case Parser.RegKeywords.SupportBody:
						SupportBody = parser.GetString();
						break;
					default:
						throw parser.GetException("unexpected line in SupportEmail");
				}
			}
		}
		private void ParseSender(Parser parser)
		{
			parser.lineno++;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				switch (parser.curr.kw)
				{
					case Parser.RegKeywords.Html:
						parser.lineno++;
						break;
					case Parser.RegKeywords.SenderSubject:
						SenderSubject = parser.GetString();
						break;
					case Parser.RegKeywords.SenderBody:
						SenderBody = parser.GetString();
						break;
					default:
						throw parser.GetException("unexpected line in SenderConfirmation");
				}
			}
		}
		private void ParseInstructions(Parser parser)
		{
			parser.lineno++;
			if (parser.curr.indent == 0)
				return;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				switch (parser.curr.kw)
				{
					case Parser.RegKeywords.Html:
						parser.lineno++;
						break;
					case Parser.RegKeywords.Select:
						InstructionSelect = parser.GetString();
						break;
					case Parser.RegKeywords.Find:
						InstructionFind = parser.GetString();
						break;
					case Parser.RegKeywords.Options:
						InstructionOptions = parser.GetString();
						break;
					case Parser.RegKeywords.Special:
						InstructionSpecial = parser.GetString();
						break;
					case Parser.RegKeywords.Login:
						InstructionLogin = parser.GetString();
						break;
					case Parser.RegKeywords.Submit:
						InstructionSubmit = parser.GetString();
						break;
					case Parser.RegKeywords.Sorry:
						InstructionSorry = parser.GetString();
						break;
					case Parser.RegKeywords.Thanks:
						ThankYouMessage = parser.GetString();
						break;
					case Parser.RegKeywords.Body:
						InstructionAll = parser.GetString();
						break;
					default:
						throw parser.GetException("unexpected line");
				}
			}
		}
		private void ParseVoteTags(Parser parser)
		{
			parser.lineno++;
			if (parser.curr.indent == 0)
				return;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				if (parser.curr.kw != Parser.RegKeywords.Display)
					throw parser.GetException("unexpected line");
				parser.GetString();
				if (parser.curr.indent <= startindent)
					throw parser.GetException("expected indented Message,Confirm, or SmallGroup");
				var ind = parser.curr.indent;
				while (parser.curr.indent == ind)
				{
					switch (parser.curr.kw)
					{
						case Parser.RegKeywords.Message:
							parser.GetString();
							break;
						case Parser.RegKeywords.Confirm:
							parser.GetBool();
							break;
						case Parser.RegKeywords.SmallGroup:
							parser.GetString();
							break;
						default:
							throw parser.GetException("unexpected line");
					}
				}
			}
		}
		private void ParseAgeGroups(Parser parser)
		{
			parser.lineno++;
			if (parser.curr.indent == 0)
				return;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				var agegroup = new AgeGroup();
				if (parser.curr.kw != Parser.RegKeywords.None)
					throw parser.GetException("unexpected line");
				agegroup.SmallGroup = parser.GetLine();
				var m = parser.agerange.Match(agegroup.SmallGroup);
				if (!m.Success)
				{
					parser.lineno--;
					throw parser.GetException("expected age range like 0-12");
				}
				agegroup.StartAge = m.Groups["start"].Value.ToInt();
				agegroup.EndAge = m.Groups["end"].Value.ToInt();
				if (parser.curr.indent <= startindent)
					throw parser.GetException("expected either indented SmallGroup or Fee");
				var ind = parser.curr.indent;
				while (parser.curr.indent == ind)
				{
					switch (parser.curr.kw)
					{
						case Parser.RegKeywords.SmallGroup:
							agegroup.SmallGroup = parser.GetString(agegroup.SmallGroup);
							break;
						case Parser.RegKeywords.Fee:
							agegroup.Fee = parser.GetDecimal();
							break;
						default:
							throw parser.GetException("unexpected line");
					}
				}
				AgeGroups.Add(agegroup);
			}
		}
		private void ParseOrgFees(Parser parser)
		{
			parser.lineno++;
			if (parser.curr.indent == 0)
				return;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				if (parser.curr.kw != Parser.RegKeywords.None)
					throw parser.GetException("unexpected line");
				var oid = parser.GetInt();
				if (oid == 0)
				{
					parser.lineno--;
					throw parser.GetException("invalid orgid");
				}
				var f = new OrgFee { OrgId = oid };
				if (parser.curr.indent > startindent)
				{
					if (parser.curr.kw != Parser.RegKeywords.Fee)
						throw parser.GetException("expected fee");
					f.Fee = parser.GetDecimal();
				}
				OrgFees.Add(f);
			}
		}
		public override string ToString()
		{
			var sb = new StringBuilder();

			AddConfirmation(sb);
			AddReminder(sb);
			AddSupport(sb);
			AddSender(sb);
			AddFees(sb);
			AddValueCk(0, sb, "IncludeOtherFeesWithDeposit", IncludeOtherFeesWithDeposit);
			AddDonation(sb);
			AddAgeGroups(sb);
			AddOrgFees(sb);
			AddValueCk(0, sb, "OtherFeesAddedToOrgFee", OtherFeesAddedToOrgFee);
			AddInstructions(sb);
			AddTerms(sb);
			AddTrackCode(sb);

			AddValueCk(0, sb, "Title", Title);
			AddValueCk(0, sb, "ValidateOrgs", ValidateOrgs);
			AddValueCk(0, sb, "Shell", Shell);
			AddValueCk(0, sb, "ShellBs", ShellBs);
			AddValueCk(0, sb, "SpecialScript", SpecialScript);
			AddValueCk(0, sb, "AllowOnlyOne", AllowOnlyOne);
			AddValueCk(0, sb, "TargetExtraValues", TargetExtraValues);
			AddValueCk(0, sb, "AllowReRegister", AllowReRegister);
			AddValueCk(0, sb, "AllowSaveProgress", AllowSaveProgress);
			AddValueCk(0, sb, "MemberOnly", MemberOnly);
			AddValueCk(0, sb, "GroupToJoin", GroupToJoin);
			AddValueCk(0, sb, "AddAsProspect", AddAsProspect);
			AddValueCk(0, sb, "NoReqBirthYear", NoReqBirthYear);
			AddValueCk(0, sb, "NotReqDOB", NotReqDOB);
			AddValueCk(0, sb, "NotReqAddr", NotReqAddr);
			AddValueCk(0, sb, "NotReqZip", NotReqZip);
			AddValueCk(0, sb, "NotReqPhone", NotReqPhone);
			AddValueCk(0, sb, "NotReqGender", NotReqGender);
			AddValueCk(0, sb, "NotReqMarital", NotReqMarital);

			TimeSlots.Output(sb);
			foreach(var a in AskItems)
				a.Output(sb);

			return sb.ToString();
		}

        private void AddFees(StringBuilder sb)
		{
			AddValueCk(0, sb, "Fee", Fee);
			AddValueCk(0, sb, "Deposit", Deposit);
			AddValueCk(0, sb, "ExtraFee", ExtraFee);
			AddValueCk(0, sb, "MaximumFee", MaximumFee);
			AddValueCk(0, sb, "ApplyMaxToOtherFees", ApplyMaxToOtherFees);
			AddValueCk(0, sb, "ExtraValueFeeName", ExtraValueFeeName);
			AddValueCk(0, sb, "AccountingCode", AccountingCode);
			sb.AppendLine();
		}
		private void AddDonation(StringBuilder sb)
		{
			AddValueCk(0, sb, "AskDonation", AskDonation);
			AddSingleOrMultiLine(0, sb, "DonationLabel", DonationLabel);
			AddValueCk(0, sb, "DonationFundId", DonationFundId);
			sb.AppendLine();
		}
		private void AddAgeGroups(StringBuilder sb)
		{
			if (AgeGroups.Count == 0)
				return;
			AddValueNoCk(0, sb, "AgeGroups", "");
			foreach (var i in AgeGroups)
			{
				AddValueCk(1, sb, "{0}-{1}".Fmt(i.StartAge, i.EndAge));
				AddValueCk(2, sb, "SmallGroup", i.SmallGroup);
				AddValueCk(2, sb, "Fee", i.Fee);
			}
			sb.AppendLine();
		}
		private void AddOrgFees(StringBuilder sb)
		{
			if (OrgFees.Count == 0)
				return;
			AddValueNoCk(0, sb, "OrgFees", "");
			foreach (var i in OrgFees)
			{
				AddValueCk(1, sb, "{0}".Fmt(i.OrgId));
				AddValueCk(2, sb, "Fee", i.Fee);
			}
			sb.AppendLine();
		}
		private void AddConfirmation(StringBuilder sb)
		{
			AddValueNoCk(0, sb, "Confirmation", "");
			AddValueNoCk(1, sb, "Subject", Subject);
			AddSingleOrMultiLine(1, sb, "Body", Body);
		}
		private void AddReminder(StringBuilder sb)
		{
			AddValueNoCk(0, sb, "Reminder", "");
			AddValueNoCk(1, sb, "ReminderSubject", ReminderSubject);
			AddSingleOrMultiLine(1, sb, "ReminderBody", ReminderBody);
		}
		private void AddSupport(StringBuilder sb)
		{
			AddValueNoCk(0, sb, "SupportEmail", "");
			AddValueNoCk(1, sb, "SupportSubject", SupportSubject);
			AddSingleOrMultiLine(1, sb, "SupportBody", SupportBody);
		}
		private void AddSender(StringBuilder sb)
		{
			AddValueNoCk(0, sb, "SenderEmail", "");
			AddValueNoCk(1, sb, "SenderSubject", SenderSubject);
			AddSingleOrMultiLine(1, sb, "SenderBody", SenderBody);
		}
		private void AddSingleOrMultiLine(int n, StringBuilder sb, string Section, string ht)
		{
			if (ht == null)
				return;
			if (ht.Contains("\n"))
			{
				AddValueCk(n, sb, Section, "<<");
				sb.AppendLine("----------");
				sb.AppendLine(ht.Trim());
				sb.AppendLine("----------");
			}
			else
				AddValueNoCk(n, sb, Section, ht);
		}
		private void AddInstructions(StringBuilder sb)
		{
			AddValueNoCk(0, sb, "Instructions", "");
			AddSingleOrMultiLine(1, sb, "Login", InstructionLogin);
			AddSingleOrMultiLine(1, sb, "Select", InstructionSelect);
			AddSingleOrMultiLine(1, sb, "Find", InstructionFind);
			AddSingleOrMultiLine(1, sb, "Options", InstructionOptions);
			AddSingleOrMultiLine(1, sb, "Special", InstructionSpecial);
			AddSingleOrMultiLine(1, sb, "Submit", InstructionSubmit);
			AddSingleOrMultiLine(1, sb, "Sorry", InstructionSorry);
			AddSingleOrMultiLine(1, sb, "Thanks", ThankYouMessage);
		}
		private void AddTerms(StringBuilder sb)
		{
			AddSingleOrMultiLine(0, sb, "Terms", Terms);
		}
		private void AddTrackCode(StringBuilder sb)
		{
			AddSingleOrMultiLine(0, sb, "ConfirmationTrackingCode", ConfirmationTrackingCode);
		}
		public static void AddValueCk(int n, StringBuilder sb, string label, int? value)
		{
			if (value.HasValue)
				sb.AppendFormat("{0}{1}: {2}\n", new string('\t', n), label, value);
		}
		public static void AddValueCk(int n, StringBuilder sb, string label, decimal? value)
		{
			if (value.HasValue)
				sb.AppendFormat("{0}{1}: {2}\n", new string('\t', n), label, value.ToString2("n2"));
		}
		public static void AddValueCk(int n, StringBuilder sb, string label, bool? value)
		{
			if (value == true)
				sb.AppendFormat("{0}{1}: {2}\n", new string('\t', n), label, value.ToString());
		}
		public static void AddValueCk(int n, StringBuilder sb, string label, string value)
		{
			if (value.HasValue())
				sb.AppendFormat("{0}{1}: {2}\n", new string('\t', n), label, value.Trim());
		}
		public static void AddValueNoCk(int n, StringBuilder sb, string label, string value)
		{
			sb.AppendFormat("{0}{1}: {2}\n", new string('\t', n), label, value);
		}
		public static void AddValueCk(int n, StringBuilder sb, string value)
		{
			if (value.HasValue())
				sb.AppendFormat("{0}{1}\n", new string('\t', n), value.Trim());
		}
		public class MasterOrgInfo
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}
		public MasterOrgInfo MasterOrg()
		{
			var q = from o in Db.ViewMasterOrgs
					where o.PickListOrgId == OrgId
					select new MasterOrgInfo
					{
						Id = o.OrganizationId,
						Name = o.OrganizationName
					};
			var i = q.FirstOrDefault();
			if (i == null)
				return new MasterOrgInfo();
			return i;
		}
    }
}