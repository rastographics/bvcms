using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;
using System.Text;
using CmsData;
using CmsData.Registration;

namespace RegistrationSettingsParser
{
    public partial class Parser
    {
        public static Settings ParseSettings(string s, CMSDataContext Db, int OrgId)
        {
            var settings = ParseSettings(s);
            settings.Db = Db;
            settings.OrgId = OrgId;
            settings.org = Db.LoadOrganizationById(OrgId);
            return settings;
        }
        public static Settings ParseSettings(string s)
        {
            var settings = new Settings();
            var parser = new Parser(s, settings);

            while (parser.NextSection())
                parser.ParseSection();
            settings.SetUniqueIds("AskDropdown");
            settings.SetUniqueIds("AskExtraQuestions");
            settings.SetUniqueIds("AskCheckboxes");
            settings.SetUniqueIds("AskText");
            settings.SetUniqueIds("AskMenu");
            var sglist = new List<string>();
            settings.AskItems.ForEach(a => sglist.AddRange(a.SmallGroups()));
            var q = sglist.GroupBy(mi => mi).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (q.Any())
                throw parser.GetException("Duplicate SmallGroup: " + string.Join(",", q));

            parser.data = null;
            return settings;
        }


        private AskSize asksize; // To support old Registration Documents
        void ParseSection()
        {
            switch (curr.kw)
            {
                case RegKeywords.AskParents:
                case RegKeywords.AnswersNotRequired:
                case RegKeywords.AskSMS:
                case RegKeywords.AskDoctor:
                case RegKeywords.AskInsurance:
                case RegKeywords.AskEmContact:
                case RegKeywords.AskAllergies:
                case RegKeywords.AskChurch:
                case RegKeywords.AskTylenolEtc:
                case RegKeywords.AskCoaching:
                    set.AskItems.Add(ParseAsk());
                    break;
                case RegKeywords.SpecialScript:
                    set.SpecialScript = GetString();
                    break;
                case RegKeywords.AskSuggestedFee:
                    set.AskItems.Add(ParseAskSuggestedFee());
                    break;
                case RegKeywords.AskTickets:
                    set.AskItems.Add(ParseAskTickets());
                    break;
                case RegKeywords.AskRequest:
                    set.AskItems.Add(ParseAskRequest());
                    break;
                case RegKeywords.AskHeader:
                    set.AskItems.Add(ParseAskHeader());
                    break;
                case RegKeywords.AskInstruction:
                    set.AskItems.Add(ParseAskInstruction());
                    break;
                case RegKeywords.Dropdown:
                case RegKeywords.AskOptions:
                case RegKeywords.ExtraOptions:
                case RegKeywords.Dropdown1:
                case RegKeywords.Dropdown2:
                case RegKeywords.Dropdown3:
                    set.AskItems.Add(ParseAskDropdown());
                    break;
                case RegKeywords.MenuItems:
                    var m = ParseAskMenu();
                    if (m.list.Count > 0)
                        set.AskItems.Add(m);
                    break;
                case RegKeywords.ExtraQuestions:
                    set.AskItems.Add(ParseAskExtraQuestions());
                    break;
                case RegKeywords.Text:
                    set.AskItems.Add(ParseAskText());
                    break;
                case RegKeywords.Checkboxes:
                case RegKeywords.Checkboxes2:
                    set.AskItems.Add(ParseAskCheckboxes());
                    break;
                case RegKeywords.YesNoQuestions:
                    set.AskItems.Add(ParseAskYesNoQuestions());
                    break;
                case RegKeywords.AskGradeOptions:
                case RegKeywords.GradeOptions:
                    set.AskItems.Add(ParseAskGradeOptions());
                    break;
                case RegKeywords.AskSize:
                    asksize = ParseAskSize();
                    if (asksize.list.Count > 0)
                        set.AskItems.Add(asksize);
                    break;

                case RegKeywords.MemberOnly:
                    set.MemberOnly = GetBool();
                    break;
                case RegKeywords.AskMedical:
                    GetBool();
                    break;
                case RegKeywords.AskDonation:
                    set.AskDonation = GetBool();
                    break;
                case RegKeywords.NoReqBirthYear:
                    set.NoReqBirthYear = GetBool();
                    break;
                case RegKeywords.NotReqDOB:
                    set.NotReqDOB = GetBool();
                    break;
                case RegKeywords.NotReqAddr:
                    set.NotReqAddr = GetBool();
                    break;
                case RegKeywords.NotReqPhone:
                    set.NotReqPhone = GetBool();
                    break;
                case RegKeywords.NotReqGender:
                    set.NotReqGender = GetBool();
                    break;
                case RegKeywords.NotReqMarital:
                    set.NotReqMarital = GetBool();
                    break;
                case RegKeywords.NotReqCampus:
                    set.NotReqCampus = GetBool();
                    break;
                case RegKeywords.NotReqZip:
                    set.NotReqZip = GetBool();
                    break;
                case RegKeywords.DonationFundId:
                    set.DonationFundId = GetNullInt();
                    break;
                case RegKeywords.AccountingCode:
                    set.AccountingCode = GetString();
                    break;
                case RegKeywords.DonationLabel:
                    set.DonationLabel = GetString();
                    break;
                case RegKeywords.ExtraValueFeeName:
                    set.ExtraValueFeeName = GetString();
                    break;
                case RegKeywords.GroupToJoin:
                    set.GroupToJoin = GetString();
                    break;
                case RegKeywords.AddAsProspect:
                    set.AddAsProspect = GetBool();
                    break;
                case RegKeywords.LinkGroupsFromOrgs:
                    set.LinkGroupsFromOrgs = (from i in curr.value.Split(',')
                                              where i.ToInt() > 0
                                              select i.ToInt()).ToList();
                    lineno++;
                    break;
                case RegKeywords.ValidateOrgs:
                    set.ValidateOrgs = curr.value;
                    lineno++;
                    break;
                case RegKeywords.Terms:
                    ParseTerms();
                    break;
                case RegKeywords.Instructions:
                    ParseInstructions();
                    break;
                case RegKeywords.Confirmation:
                    ParseConfirmation();
                    break;
                case RegKeywords.ConfirmationTrackingCode:
                    ParseTrackingCode();
                    break;
                case RegKeywords.Reminder:
                    ParseReminder();
                    break;
                case RegKeywords.SupportEmail:
                    ParseSupport();
                    break;
                case RegKeywords.SenderEmail:
                    ParseSender();
                    break;
                case RegKeywords.AgeGroups:
                    ParseAgeGroups();
                    break;
                case RegKeywords.OrgMemberFees:
                case RegKeywords.OrgFees:
                    ParseOrgFees();
                    break;
                case RegKeywords.VoteTags:
                    ParseVoteTags();
                    break;
                case RegKeywords.Shell:
                    set.Shell = GetString();
                    break;
                case RegKeywords.ShellBs:
                    set.ShellBs = GetString();
                    break;
                case RegKeywords.Fee:
                    set.Fee = GetDecimal();
                    break;
                case RegKeywords.TimeOut:
                    set.TimeOut = GetNullInt();
                    break;
                case RegKeywords.DisallowAnonymous:
                    set.DisallowAnonymous = GetBool();
                    break;
                case RegKeywords.FinishRegistrationButton:
                    set.FinishRegistrationButton = GetString();
                    break;


                // BEGIN support for old Registration Documents
                case RegKeywords.Title:
                    GetString();
                    break;
                case RegKeywords.GiveOrgMembAccess:
                    GetBool();
                    break;
                case RegKeywords.UseBootstrap:
                    GetBool();
                    break;
                case RegKeywords.AskGrade:
                    GetBool();
                    GetLabel("Grade");
                    break;

                case RegKeywords.AskShirtSize:
                    GetBool();
                    asksize = new AskSize { Label = "Size" };
                    set.AskItems.Add(asksize);
                    break;
                case RegKeywords.ShirtSizes:
                    asksize.list = ParseShirtSizes();
                    break;
                case RegKeywords.AllowLastYearShirt:
                    asksize.AllowLastYear = GetBool();
                    break;
                case RegKeywords.ShirtFee:
                    GetDecimal();
                    break;
                // END support for old Registration Documents

                case RegKeywords.Deposit:
                    set.Deposit = GetDecimal();
                    break;
                case RegKeywords.ExtraFee:
                    set.ExtraFee = GetDecimal();
                    break;
                case RegKeywords.MaximumFee:
                    set.MaximumFee = GetDecimal();
                    break;
                case RegKeywords.AllowOnlyOne:
                    set.AllowOnlyOne = GetBool();
                    break;
                case RegKeywords.OtherFeesAdded:
                case RegKeywords.OtherFeesAddedToOrgFee:
                    set.OtherFeesAddedToOrgFee = GetBool();
                    break;
                case RegKeywords.IncludeOtherFeesWithDeposit:
                    set.IncludeOtherFeesWithDeposit = GetBool();
                    break;
                case RegKeywords.ApplyMaxToOtherFees:
                    set.ApplyMaxToOtherFees = GetBool();
                    break;
                case RegKeywords.AllowReRegister:
                    set.AllowReRegister = GetBool();
                    break;
                case RegKeywords.AllowSaveProgress:
                    set.AllowSaveProgress = GetBool();
                    break;
                case RegKeywords.TargetExtraValues:
                    set.TargetExtraValues = GetBool();
                    break;
                case RegKeywords.TimeSlotLockDays:
                    set.TimeSlotLockDays = GetNullInt();
                    break;
                case RegKeywords.TimeSlots:
                    set.TimeSlots = ParseTimeSlots();
                    if (set.TimeSlotLockDays.HasValue)
                        set.TimeSlots.TimeSlotLockDays = set.TimeSlotLockDays;
                    break;
            }
        }

        private void ParseTerms()
        {
            set.Terms = GetString();
            if (set.Terms.HasValue() || curr.indent == 0)
                return;
            var startindent = curr.indent;
            while (curr.indent == startindent)
            {
                switch (curr.kw)
                {
                    case RegKeywords.Html:
                        lineno++;
                        break;
                    case RegKeywords.Body:
                        set.Terms = GetString();
                        break;
                    default:
                        throw GetException("unexpected line");
                }
            }
        }
        private void ParseTrackingCode()
        {
            set.ConfirmationTrackingCode = GetString();
            if (set.ConfirmationTrackingCode.HasValue() || curr.indent == 0)
                return;
            var startindent = curr.indent;
            while (curr.indent == startindent)
            {
                switch (curr.kw)
                {
                    case RegKeywords.Html:
                        lineno++;
                        break;
                    case RegKeywords.Body:
                        set.ConfirmationTrackingCode = GetString();
                        break;
                    default:
                        throw GetException("unexpected line");
                }
            }
        }
        private void ParseConfirmation()
        {
            lineno++;
            var startindent = curr.indent;
            while (curr.indent == startindent)
            {
                switch (curr.kw)
                {
                    case RegKeywords.Html:
                        lineno++;
                        break;
                    case RegKeywords.Subject:
                        set.Subject = GetString();
                        break;
                    case RegKeywords.Body:
                        set.Body = GetString();
                        break;
                    default:
                        throw GetException("unexpected line in Confirmation");
                }
            }
        }
        private void ParseReminder()
        {
            lineno++;
            var startindent = curr.indent;
            while (curr.indent == startindent)
            {
                switch (curr.kw)
                {
                    case RegKeywords.Html:
                        lineno++;
                        break;
                    case RegKeywords.ReminderSubject:
                        set.ReminderSubject = GetString();
                        break;
                    case RegKeywords.ReminderBody:
                        set.ReminderBody = GetString();
                        break;
                    default:
                        throw GetException("unexpected line in Reminder");
                }
            }
        }
        private void ParseSupport()
        {
            lineno++;
            var startindent = curr.indent;
            while (curr.indent == startindent)
            {
                switch (curr.kw)
                {
                    case RegKeywords.Html:
                        lineno++;
                        break;
                    case RegKeywords.SupportSubject:
                        set.SupportSubject = GetString();
                        break;
                    case RegKeywords.SupportBody:
                        set.SupportBody = GetString();
                        break;
                    default:
                        throw GetException("unexpected line in SupportEmail");
                }
            }
        }
        private void ParseSender()
        {
            lineno++;
            var startindent = curr.indent;
            while (curr.indent == startindent)
            {
                switch (curr.kw)
                {
                    case RegKeywords.Html:
                        lineno++;
                        break;
                    case RegKeywords.SenderSubject:
                        set.SenderSubject = GetString();
                        break;
                    case RegKeywords.SenderBody:
                        set.SenderBody = GetString();
                        break;
                    default:
                        throw GetException("unexpected line in SenderConfirmation");
                }
            }
        }
        private void ParseInstructions()
        {
            lineno++;
            if (curr.indent == 0)
                return;
            var startindent = curr.indent;
            while (curr.indent == startindent)
            {
                switch (curr.kw)
                {
                    case RegKeywords.Html:
                        lineno++;
                        break;
                    case RegKeywords.Select:
                        set.InstructionSelect = GetString();
                        break;
                    case RegKeywords.Find:
                        set.InstructionFind = GetString();
                        break;
                    case RegKeywords.Options:
                        set.InstructionOptions = GetString();
                        break;
                    case RegKeywords.Special:
                        set.InstructionSpecial = GetString();
                        break;
                    case RegKeywords.Login:
                        set.InstructionLogin = GetString();
                        break;
                    case RegKeywords.Submit:
                        set.InstructionSubmit = GetString();
                        break;
                    case RegKeywords.Sorry:
                        set.InstructionSorry = GetString();
                        break;
                    case RegKeywords.Thanks:
                        set.ThankYouMessage = GetString();
                        break;
                    case RegKeywords.FinishRegistrationButton:
                        set.FinishRegistrationButton = GetString();
                        break;
                    case RegKeywords.Body:
                        GetString();
                        break;
                    default:
                        throw GetException("unexpected line");
                }
            }
        }
        private void ParseVoteTags()
        {
            lineno++;
            if (curr.indent == 0)
                return;
            var startindent = curr.indent;
            while (curr.indent == startindent)
            {
                if (curr.kw != RegKeywords.Display)
                    throw GetException("unexpected line");
                GetString();
                if (curr.indent <= startindent)
                    throw GetException("expected indented Message,Confirm, or SmallGroup");
                var ind = curr.indent;
                while (curr.indent == ind)
                {
                    switch (curr.kw)
                    {
                        case RegKeywords.Message:
                            GetString();
                            break;
                        case RegKeywords.Confirm:
                            GetBool();
                            break;
                        case RegKeywords.SmallGroup:
                            GetString();
                            break;
                        default:
                            throw GetException("unexpected line");
                    }
                }
            }
        }
        private void ParseAgeGroups()
        {
            lineno++;
            if (curr.indent == 0)
                return;
            var startindent = curr.indent;
            while (curr.indent == startindent)
            {
                var agegroup = new Settings.AgeGroup();
                if (curr.kw != RegKeywords.None)
                    throw GetException("unexpected line");
                agegroup.SmallGroup = GetLine();
                var m = agerange.Match(agegroup.SmallGroup);
                if (!m.Success)
                {
                    lineno--;
                    throw GetException("expected age range like 0-12");
                }
                agegroup.StartAge = m.Groups["start"].Value.ToInt();
                agegroup.EndAge = m.Groups["end"].Value.ToInt();
                if (curr.indent <= startindent)
                    throw GetException("expected either indented SmallGroup or Fee");
                var ind = curr.indent;
                while (curr.indent == ind)
                {
                    switch (curr.kw)
                    {
                        case RegKeywords.SmallGroup:
                            agegroup.SmallGroup = GetString(agegroup.SmallGroup);
                            break;
                        case RegKeywords.Fee:
                            agegroup.Fee = GetDecimal();
                            break;
                        default:
                            throw GetException("unexpected line");
                    }
                }
                set.AgeGroups.Add(agegroup);
            }
        }
        private void ParseOrgFees()
        {
            lineno++;
            if (curr.indent == 0)
                return;
            var startindent = curr.indent;
            while (curr.indent == startindent)
            {
                if (curr.kw != RegKeywords.None)
                    throw GetException("unexpected line");
                var oid = GetInt();
                if (oid == 0)
                {
                    lineno--;
                    throw GetException("invalid orgid");
                }
                var f = new Settings.OrgFee { OrgId = oid };
                if (curr.indent > startindent)
                {
                    if (curr.kw != RegKeywords.Fee)
                        throw GetException("expected fee");
                    f.Fee = GetDecimal();
                }
                set.OrgFees.Add(f);
            }
        }

        public static string Output(Settings set)
        {
            var parser = new Parser(set);
            return parser.Output();
        }
        public string Output()
        {
            var sb = new StringBuilder();

            AddConfirmation(sb);
            AddReminder(sb);
            AddSupport(sb);
            AddSender(sb);
            AddFees(sb);
            AddValueCk(0, sb, "IncludeOtherFeesWithDeposit", set.IncludeOtherFeesWithDeposit);
            AddDonation(sb);
            AddAgeGroups(sb);
            AddOrgFees(sb);
            AddValueCk(0, sb, "OtherFeesAddedToOrgFee", set.OtherFeesAddedToOrgFee);
            AddInstructions(sb);
            AddTerms(sb);
            AddTrackCode(sb);

            AddValueCk(0, sb, "ValidateOrgs", set.ValidateOrgs);
            AddValueCk(0, sb, "Shell", set.Shell);
            AddValueCk(0, sb, "ShellBs", set.ShellBs);
            AddValueCk(0, sb, "FinishRegistrationButton", set.FinishRegistrationButton);
            AddValueCk(0, sb, "SpecialScript", set.SpecialScript);
            AddValueCk(0, sb, "AllowOnlyOne", set.AllowOnlyOne);
            AddValueCk(0, sb, "TargetExtraValues", set.TargetExtraValues);
            AddValueCk(0, sb, "AllowReRegister", set.AllowReRegister);
            AddValueCk(0, sb, "AllowSaveProgress", set.AllowSaveProgress);
            AddValueCk(0, sb, "MemberOnly", set.MemberOnly);
            AddValueCk(0, sb, "GroupToJoin", set.GroupToJoin);
            AddValueCk(0, sb, "AddAsProspect", set.AddAsProspect);
            AddValueCk(0, sb, "NoReqBirthYear", set.NoReqBirthYear);
            AddValueCk(0, sb, "NotReqDOB", set.NotReqDOB);
            AddValueCk(0, sb, "NotReqAddr", set.NotReqAddr);
            AddValueCk(0, sb, "NotReqZip", set.NotReqZip);
            AddValueCk(0, sb, "NotReqPhone", set.NotReqPhone);
            AddValueCk(0, sb, "NotReqGender", set.NotReqGender);
            AddValueCk(0, sb, "NotReqMarital", set.NotReqMarital);
            AddValueCk(0, sb, "NotReqCampus", set.NotReqCampus);
            AddValueCk(0, sb, "TimeOut", set.TimeOut);
            AddValueCk(0, sb, "DisallowAnonymous", set.DisallowAnonymous);

            Output(sb, set.TimeSlots);
            foreach (var a in set.AskItems)
                OutputAsk(sb, a);

            return sb.ToString();
        }

        private void AddFees(StringBuilder sb)
        {
            AddValueCk(0, sb, "Fee", set.Fee);
            AddValueCk(0, sb, "Deposit", set.Deposit);
            AddValueCk(0, sb, "ExtraFee", set.ExtraFee);
            AddValueCk(0, sb, "MaximumFee", set.MaximumFee);
            AddValueCk(0, sb, "ApplyMaxToOtherFees", set.ApplyMaxToOtherFees);
            AddValueCk(0, sb, "ExtraValueFeeName", set.ExtraValueFeeName);
            AddValueCk(0, sb, "AccountingCode", set.AccountingCode);
            sb.AppendLine();
        }
        private void AddDonation(StringBuilder sb)
        {
            AddValueCk(0, sb, "AskDonation", set.AskDonation);
            AddSingleOrMultiLine(0, sb, "DonationLabel", set.DonationLabel);
            AddValueCk(0, sb, "DonationFundId", set.DonationFundId);
            sb.AppendLine();
        }
        private void AddAgeGroups(StringBuilder sb)
        {
            if (set.AgeGroups.Count == 0)
                return;
            AddValueNoCk(0, sb, "AgeGroups", "");
            foreach (var i in set.AgeGroups)
            {
                AddValueCk(1, sb, $"{i.StartAge}-{i.EndAge}");
                AddValueCk(2, sb, "SmallGroup", i.SmallGroup);
                AddValueCk(2, sb, "Fee", i.Fee);
            }
            sb.AppendLine();
        }
        private void AddOrgFees(StringBuilder sb)
        {
            if (set.OrgFees.Count == 0)
                return;
            AddValueNoCk(0, sb, "OrgFees", "");
            foreach (var i in set.OrgFees)
            {
                AddValueCk(1, sb, $"{i.OrgId}");
                AddValueCk(2, sb, "Fee", i.Fee);
            }
            sb.AppendLine();
        }
        private void AddConfirmation(StringBuilder sb)
        {
            AddValueNoCk(0, sb, "Confirmation", "");
            AddValueNoCk(1, sb, "Subject", set.Subject);
            AddSingleOrMultiLine(1, sb, "Body", set.Body);
        }
        private void AddReminder(StringBuilder sb)
        {
            AddValueNoCk(0, sb, "Reminder", "");
            AddValueNoCk(1, sb, "ReminderSubject", set.ReminderSubject);
            AddSingleOrMultiLine(1, sb, "ReminderBody", set.ReminderBody);
        }
        private void AddSupport(StringBuilder sb)
        {
            AddValueNoCk(0, sb, "SupportEmail", "");
            AddValueNoCk(1, sb, "SupportSubject", set.SupportSubject);
            AddSingleOrMultiLine(1, sb, "SupportBody", set.SupportBody);
        }
        private void AddSender(StringBuilder sb)
        {
            AddValueNoCk(0, sb, "SenderEmail", "");
            AddValueNoCk(1, sb, "SenderSubject", set.SenderSubject);
            AddSingleOrMultiLine(1, sb, "SenderBody", set.SenderBody);
        }
        private static void AddSingleOrMultiLine(int n, StringBuilder sb, string section, string ht)
        {
            if (ht == null)
                return;
            if (ht.Contains("\n"))
            {
                AddValueCk(n, sb, section, "<<");
                sb.AppendLine("----------");
                sb.AppendLine(ht.Trim());
                sb.AppendLine("----------");
            }
            else
                AddValueNoCk(n, sb, section, ht);
        }
        private void AddInstructions(StringBuilder sb)
        {
            AddValueNoCk(0, sb, "Instructions", "");
            AddSingleOrMultiLine(1, sb, "Login", set.InstructionLogin);
            AddSingleOrMultiLine(1, sb, "Select", set.InstructionSelect);
            AddSingleOrMultiLine(1, sb, "Find", set.InstructionFind);
            AddSingleOrMultiLine(1, sb, "Options", set.InstructionOptions);
            AddSingleOrMultiLine(1, sb, "Special", set.InstructionSpecial);
            AddSingleOrMultiLine(1, sb, "Submit", set.InstructionSubmit);
            AddSingleOrMultiLine(1, sb, "Sorry", set.InstructionSorry);
            AddSingleOrMultiLine(1, sb, "Thanks", set.ThankYouMessage);
        }
        private void AddTerms(StringBuilder sb)
        {
            AddSingleOrMultiLine(0, sb, "Terms", set.Terms);
        }
        private void AddTrackCode(StringBuilder sb)
        {
            AddSingleOrMultiLine(0, sb, "ConfirmationTrackingCode", set.ConfirmationTrackingCode);
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
    }
}