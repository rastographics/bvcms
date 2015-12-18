using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CmsData.API;
using CmsData.Registration;
using UtilityExtensions;

namespace RegistrationSettingsParser
{
    public partial class Parser
    {
        public int lineno;
        string[] lines;
        public readonly Regex skipline = new Regex(@"^(?:\s*)$|^(?:\s*\#.*)$");
        public readonly Regex linedata = new Regex(@"^(?<sp>\s*)(?:(?<keyword>[a-zA-Z]\w*):)?\s*(?<val>.*)$");
        public readonly Regex agerange = new Regex(@"(?<start>\d+)-(?<end>\d+)");
        public class LineData
        {
            public int indent { get; set; }
            public string line { get; set; }
            public int lineno { get; set; }
            public RegKeywords kw { get; set; }
            public string value { get; set; }
            public bool html { get; set; }
            public override string ToString()
            {
                return line;
            }
        }
        public List<LineData> data = new List<LineData>();

        private Settings set;

        public Parser(Settings settings)
        {
            set = settings;
        }

        public Parser(string s, Settings settings)
        {
            set = settings;
            lines = SplitLines(s);
            for (lineno = 0; lineno < lines.Length; )
            {
                var d = GetLineData();
                if (d == null)
                    break;
                data.Add(d);
            }
            lines = null;
            lineno = 0;

        }
        public bool NextSection()
        {
            if (lineno >= data.Count)
                return false;
            if (curr.kw == RegKeywords.None)
                throw GetException("unknown keyword " + curr.line);
            if (curr.indent > 0)
                throw GetException("expected no indent");
            // positioned ready for ParseSection();
            return true;
        }

        public Exception GetException(string msg)
        {
            var stackTrace = new StackTrace();
            var name = stackTrace.GetFrame(1).GetMethod().Name;
            return new Exception($"{name}: {msg}");
        }

        public string curline
        {
            get
            {
                if (lineno < lines.Length)
                    return lines[lineno];
                return null;
            }
        }
        public LineData curr
        {
            get
            {
                if (lineno < data.Count)
                    return data[lineno];
                return new LineData();
            }
        }
        public LineData prev { get { return data[lineno - 1]; } }
        public static string[] SplitLines(string s)
        {
            var splitlines = new Regex(@"\r?\n");
            if (!s.HasValue())
                return new string[] { };
            var lines = splitlines.Split(s);
            return lines;
        }
        LineData GetLineData()
        {
            while (lineno < lines.Length && skipline.IsMatch(curline))
            {
                lineno++;
                continue;
            }
            if (lineno >= lines.Length)
                return null;
            var m = linedata.Match(curline);
            if (m.Success)
            {
                var d = new LineData();
                d.line = curline.Trim();
                lineno++;
                d.lineno = lineno;
                d.indent = m.Groups["sp"].Value.Length;
                d.value = m.Groups["val"].Value.TrimEnd();
                var keyword = m.Groups["keyword"].Value;

                RegKeywords kw;
                if (!Enum.TryParse<RegKeywords>(keyword, true, out kw))
                    kw = RegKeywords.None;
                d.kw = kw;
                if (d.value == "<<")
                {
                    var lookfor = curline.Trim();
                    lineno++;
                    d.html = true;
                    var sb = new StringBuilder();
                    while (lineno < lines.Length && curline.Trim() != lookfor)
                    {
                        sb.Append(curline.Trim());
                        lineno++;
                    }
                    d.value = sb.ToString();
                    lineno++;
                }
                return d;
            }
            return null;
        }

        public string GetString()
        {
            return RemoveTroublesomeCharacters(GetString(null));
        }
        public static string RemoveTroublesomeCharacters(string inString)
        {
            if (inString == null) return null;

            StringBuilder newString = new StringBuilder();
            char ch;

            for (int i = 0; i < inString.Length; i++)
            {

                ch = inString[i];
                if (XmlConvert.IsXmlChar(ch))
                    newString.Append(ch);
            }
            return newString.ToString();

        }
        public string GetString(string def)
        {
            var s = curr.value;
            //if (def.HasValue() && !s.HasValue())
            if (!s.HasValue())
                s = def;
            lineno++;
            return s;
        }
        public string GetLine()
        {
            var s = curr.line;
            lineno++;
            return s;
        }
        public decimal? GetDecimal()
        {
            decimal d;
            if (!decimal.TryParse(curr.value, out d))
                throw GetException("expected numeric value");
            lineno++;
            return d;
        }
        public int GetInt()
        {
            if (!curr.value.HasValue())
                throw GetException("expected integer value");
            int i;
            if (!int.TryParse(curr.value, out i))
                throw GetException("expected integer value");
            lineno++;
            return i;
        }
        public DateTime GetTime()
        {
            if (!curr.value.HasValue())
                throw GetException("expected time value");
            DateTime i;
            if (!DateTime.TryParse(curr.value, out i))
                throw GetException("expected time value");
            lineno++;
            return i;
        }
        public DateTime GetDateTime()
        {
            if (!curr.value.HasValue())
                throw GetException("expected datetime value");
            DateTime i;
            if (!DateTime.TryParse(curr.value, out i))
                throw GetException("expected datetime value");
            lineno++;
            return i;
        }
        public int? GetNullInt()
        {
            if (!curr.value.HasValue())
            {
                lineno++;
                return null;
            }
            int i;
            if (!int.TryParse(curr.value, out i))
                throw GetException("expected integer value");
            lineno++;
            return i;
        }
        public bool GetBool()
        {
            if (!curr.value.HasValue())
            {
                lineno++;
                return false;
            }
            bool b;
            if (!bool.TryParse(curr.value, out b))
                throw GetException("expected true/false");
            lineno++;
            return b;
        }
        public string GetLabel(string def)
        {
            if (curr.indent > 0)
            {
                if (curr.kw != RegKeywords.Label)
                    throw GetException("expected Label");
                var label = curr.value;
                lineno++;
                return label;
            }
            return def;
        }
        public int? GetInt(RegKeywords kw)
        {
            if (curr.indent > 0)
            {
                if (curr.kw != kw)
                    return null;
                return GetNullInt();
            }
            return null;
        }
        public enum RegKeywords
        {
            Shell,
            ShellBs,
            Fee,
            AskSuggestedFee,
            Deposit,
            ShirtFee,
            ExtraFee,
            MaximumFee,
            AllowOnlyOne,
            TargetExtraValues,
            AllowReRegister,
            AllowSaveProgress,
            OrgMemberFees,
            AskTickets,
            AskRequest,
            AskGradeOptions,
            GradeOptions,
            AgeGroups,
            OrgFees,
            AskShirtSize,
            AllowLastYearShirt,
            ShirtSizes,
            YesNoQuestions,
            ExtraQuestions,
            Text,
            MenuItems,
            ValidateOrgs,
            MemberOnly,
            LinkGroupsFromOrgs,
            AnswersNotRequired,
            AskSMS,
            AskParents,
            AskDoctor,
            AskInsurance,
            AskEmContact,
            AskAllergies,
            AskChurch,
            AskTylenolEtc,
            AskCoaching,
            AskGrade,
            AskDonation,
            AskHeader,
            AskInstruction,
            DonationLabel,
            ExtraValueFeeName,
            DonationFundId,
            AccountingCode,
            GroupToJoin,
            GiveOrgMembAccess,
            AddAsProspect,
            UseBootstrap,
            NoReqBirthYear,
            NotReqDOB,
            NotReqAddr,
            NotReqZip,
            NotReqPhone,
            NotReqGender,
            NotReqMarital,
            NotReqCampus,
            ExtraOptions,
            Dropdown,
            Dropdown1,
            Dropdown2,
            Dropdown3,
            Limit,
            Confirmation,
            Reminder,
            SupportEmail,
            SenderEmail,
            Terms,
            Label,
            Time,
            DayOfWeek,
            TimeSlots,
            TimeSlotLockDays,
            Options,
            Special,
            Code,
            SmallGroup,
            Checkboxes,
            Checkboxes2,
            Minimum,
            Maximum,
            Columns,
            Items,
            Subject,
            Body,
            ReminderSubject,
            ReminderBody,
            SupportSubject,
            SupportBody,
            SenderSubject,
            SenderBody,
            VoteTags,
            Confirm,
            Message,
            Display,
            Instructions,
            Select,
            SpecialScript,
            Find,
            Login,
            Submit,
            Sorry,
            Html,
            Title,
            None,
            AskSize,
            Sizes,
            AllowLastYear,
            OtherFeesAddedToOrgFee,
            ApplyMaxToOtherFees,
            IncludeOtherFeesWithDeposit,
            ConfirmationTrackingCode,
            TimeOut,
            DisallowAnonymous,


            OtherFeesAdded,
            AskOptions,
            AskMedical,
            Thanks,
            FinishRegistrationButton,
        }
    }
}
