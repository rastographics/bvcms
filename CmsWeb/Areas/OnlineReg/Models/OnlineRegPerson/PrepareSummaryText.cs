using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CmsData.Registration;
using UtilityExtensions;
using CmsData;
using CmsData.View;
using CmsWeb.Areas.OnlineReg.Controllers;
using Elmah;
using HandlebarsDotNet;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public class OnlineRegPersonModel0
    {
        public Organization org { get; set; }
        public int? MissionTripGoerId { get; set; }
        public Settings setting { get; set; }
        public int PeopleId { get; set; }
        public int orgid { get; set; }
        public int? tranid { get; set; }
        public string gradeoption { get; set; }
        public Person person { get; set; }
        public string AgeGroup()
        {
            foreach (var i in setting.AgeGroups)
                if (person.Age >= i.StartAge && person.Age <= i.EndAge)
                    return i.SmallGroup;
            return string.Empty;
        }
        public decimal? MissionTripSupportGeneral { get; set; }
        public List<FamilyAttendInfo> FamilyAttend { get; set; }
        public Dictionary<int, decimal?> FundItem { get; set; }
        public List<Dictionary<string, string>> ExtraQuestion { get; set; }
        public List<Dictionary<string, string>> Text { get; set; }
        public Dictionary<string, bool?> YesNoQuestion { get; set; }
        public List<string> option { get; set; }
        public List<string> Checkbox { get; set; }
        public List<Dictionary<string, int?>> MenuItem { get; set; }
        public Dictionary<string, string> GradeOptions(Ask ask)
        {
            var d = ((AskGradeOptions)ask).list.ToDictionary(k => k.Code.ToString(), v => v.Description);
            d.Add("00", "(please select)");
            return d;
        }

        public bool? advil { get; set; }
        public bool? tylenol { get; set; }
        public bool? maalox { get; set; }
        public bool? robitussin { get; set; }

    }
    public class SummaryInfo : Sty
    {
        private readonly CMSDataContext db;

        public SummaryInfo(CMSDataContext db)
        {
            this.db = db;
            Handlebars.RegisterHelper("YesNo", (writer, options, context, args) =>
            {
                var tf = (bool?)(args[0]);
                writer.Write(tf == true ? "Yes" : tf == false ? "No" : "");
            });
            Handlebars.RegisterHelper("SubGroupChoice", (writer, options, context, args) =>
            {
                var ask = (AskDropdown)(args[0]);
                writer.Write(ask.SmallGroupChoice(p.option).Description);// HERE
            });
            Handlebars.RegisterHelper("YesNoAnswer", (writer, options, context, args) =>
            {
                var SubGroup = args[0].ToString();
                writer.Write(p.YesNoQuestion[SubGroup] == true ? "Yes" : "No"); // HERE
            });
            Handlebars.RegisterHelper("SetMenu", (writer, options, context, args) =>
            {
                var menu = args[0] as AskMenu;
                if (menu == null)
                    return;
                mlist = menu.MenuItemsChosen(p.MenuItem[menu.UniqueId]).ToList(); // HERE
                if (mlist.Any())
                    lastidesc = mlist.Last().desc;
            });
            Handlebars.RegisterHelper("SetCheckbox", (writer, options, context, args) =>
            {
                var cb = args[0] as AskCheckboxes;
                if (cb == null)
                    return;
                cblist = cb.CheckboxItemsChosen(p.Checkbox).ToList(); // HERE
                ilabel = cb.Label;
                if (cblist.Any())
                    lastidesc = cblist.Last().Description;
            });
            Handlebars.RegisterHelper("SetYesNoQuestions", (writer, options, context, args) =>
            {
                var yn = args[0] as AskYesNoQuestions;
                if (yn == null)
                    return;
                ynlist = yn.list.Where(a => p.YesNoQuestion.ContainsKey(a.SmallGroup)).ToList(); // HERE
                if (ynlist.Any())
                    lastidesc = ynlist.Last().Question;
            });
            Handlebars.RegisterHelper("SetExtraQuestions", (writer, options, context, args) =>
            {
                var eq = args[0] as AskExtraQuestions;
                if (eq == null)
                    return;
                eqlist = p.ExtraQuestion[eq.UniqueId].Where(a => a.Value.HasValue()).ToList(); // HERE
                if (eqlist.Any())
                    lastidesc = eqlist.Last().Key;
            });
            Handlebars.RegisterHelper("SetTextQuestions", (writer, options, context, args) =>
            {
                var tx = args[0] as AskText;
                if (tx == null)
                    return;
                txlist = p.Text[tx.UniqueId].Where(a => a.Value.HasValue()).ToList(); // HERE
                if (txlist.Any())
                    lastidesc = txlist.Last().Key;
            });
            Handlebars.RegisterHelper("GradeOption", (writer, options, context, args) =>
            {
                var go = (AskGradeOptions)(args[0]);
                if (go == null)
                    return;
                writer.Write(p.GradeOptions(go)[p.gradeoption ?? "00"]); // HERE
            });
            Handlebars.RegisterHelper("SetLabel", (writer, options, context, args) =>
                ilabel = args[0] as string);
            Handlebars.RegisterHelper("SetRowStyle", (writer, options, context, args) =>
                rs = lastidesc == args[0].ToString() ? args[1].ToString() : args[2].ToString());

            //var template = Handlebars.Compile(CmsData.Properties.Resources.Details2);
        }

        public string PrepareSummaryInfo(OnlineRegPersonModel0 p)
        {
            this.p = p;
            om = DbUtil.Db.OrganizationMembers.Single(vv => vv.PeopleId == p.PeopleId && vv.OrganizationId == p.orgid);
            ti = DbUtil.Db.Transactions.SingleOrDefault(tt => tt.Id == p.tranid);
            if (ShowTransaction)
                ts = om.TransactionSummary(this.db);
            rr = p.person.GetRecReg();
            OrganizationName = p.org.OrganizationName;
            Name = p.person.Name;
            return "";
        }

        // ReSharper disable InconsistentNaming
        // ReSharper disable NotAccessedField.Local
        public string Name { get; set; }
        public string OrganizationName { get; set; }
        private RecReg rr;
        public string ilabel { get; set; }
        public string lastidesc { get; set; }
        public Transaction ti { get; set; }
        public bool ShowTransaction => (ti.Amt ?? 0) > 0 && om != null;
        public OrganizationMember om { get; set; }
        public OnlineRegPersonModel0 p { get; set; }
        public TransactionSummary ts { get; set; }
        public string IndAmt => ts.IndAmt.ToString2("c");
        public string TotPaid => om.TotalPaid(db).ToString("c");
        public string AmountDue => om.AmountDue(db).ToString("c");
        public Person goer => db.LoadPersonById(p.MissionTripGoerId ?? 0);
        public bool HasAgeGroups => p.setting.AgeGroups.Count > 0;
        public string AgeGroup => p.AgeGroup();
        public bool HasGoer => p.MissionTripGoerId > 0;
        public bool HasGeneralSupport => p.MissionTripSupportGeneral > 0;
        public string ShowNotMember => rr.Member == true ? "" : "Not";
        public string ShowNotActive => rr.ActiveInAnotherChurch == true ? "" : "Not";
        public List<AskMenu.MenuItemChosen> mlist { get; set; }
        public bool HasMenuItems => mlist.Any();
        public string rs { get; private set; }
        public List<AskCheckboxes.CheckboxItem> cblist { get; set; }
        public List<AskYesNoQuestions.YesNoQuestion> ynlist { get; set; }
        public List<KeyValuePair<string, string>> eqlist { get; set; }
        public List<KeyValuePair<string, string>> txlist { get; set; }
        public bool HasCheckedboxes => cblist.Any();
        public bool ilabelHasValue => ilabel.HasValue();
        public bool SupportMissionTrip { get; set; }
    }
    public partial class OnlineRegPersonModel
    {
        public string PrepareSummaryText(Transaction ti)
        {
            if (RecordFamilyAttendance())
                return SummarizeFamilyAttendance();

            if (Util2.UseNewDetails)
            {
                var ctl = HttpContext.Current.Items["controller"] as OnlineRegController;
                try
                {
                    return ViewExtensions2.RenderPartialViewToString(ctl, "Other/Details", this);
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    return $"{ex.Message}\n\n{ex.StackTrace}";
                }
            }
            var om = GetOrgMember();
            var sb = StartSummary();
            SummarizePayment(ti, om, sb);
            if (Parent.SupportMissionTrip)
                SummarizeSupportMissionTrip(sb);
            else
                SummarizeAnswers(sb, om);
            return FinishSummary(sb);
        }

        private static void SummarizePayment(Transaction ti, OrganizationMember om, StringBuilder sb)
        {
            if ((ti.Amt ?? 0) == 0 || om == null)
                return;

            var ts = om.TransactionSummary(DbUtil.Db);
            if (ts != null)
                sb.AppendFormat(@"
<tr><td colspan='2'>
<table cellpadding=4>
    <tr>
        <td>Registrant Fee</td>
        <td>Amount Paid</td>
        <td>Amount Due</td>
    </tr>
    <tr>
        <td align='right'>{0}</td>
        <td align='right'>{1}</td>
        <td align='right'>{2}</td>
    </tr>
</table>
</td></tr>
    ", ts.IndAmt.ToString2("c"),
                    om.TotalPaid(DbUtil.Db).ToString("c"),
                    om.AmountDue(DbUtil.Db).ToString("c"));
        }

        private StringBuilder StartSummary()
        {
            var sb = new StringBuilder();
            sb.Append("<table>");
            sb.AppendFormat("<tr><td width='50%'>Org:</td><td width='50%'>{0}</td></tr>\n", org.OrganizationName);
            sb.AppendFormat("<tr><td>First:</td><td>{0}</td></tr>\n", person.PreferredName);
            sb.AppendFormat("<tr><td>Last:</td><td>{0}</td></tr>\n", person.LastName);
            return sb;
        }
        private static string FinishSummary(StringBuilder sb)
        {
            sb.AppendLine("</table>");
            return sb.ToString();
        }


        private void SummarizeSupportMissionTrip(StringBuilder sb)
        {
            if (!Parent.SupportMissionTrip)
                return;
            var goer = DbUtil.Db.LoadPersonById(MissionTripGoerId ?? 0);
            if (goer != null)
                sb.Append($"<tr><td>Support Mission Trip for:</td><td>{goer.Name}</td></tr>\n");
            if (MissionTripSupportGeneral > 0)
                sb.Append("<tr><td>Support Mission Trip:</td><td>Any other participiants</td></tr>\n");
        }

        private string SummarizeFamilyAttendance()
        {
            var sb = StartSummary();
            foreach (var m in FamilyAttend.Where(m => m.Attend))
                if (m.PeopleId != null)
                    sb.Append($"<tr><td colspan=\"2\">{m.Name}{(m.Age.HasValue ? $" ({m.Age})" : "")}</td></tr>\n");
                else
                {
                    sb.Append($"<tr><td colspan=\"2\">{m.Name}{(m.Age.HasValue ? $" ({m.Age})" : "")}");
                    if (m.Email.HasValue())
                        sb.Append($", {m.Email}");
                    if (m.Birthday.HasValue())
                        sb.Append($", {m.Birthday}");
                    if (m.MaritalId.HasValue)
                        sb.Append($", {m.Marital}");
                    if (m.GenderId.HasValue)
                        sb.Append($", {m.Gender}");
                    sb.Append("</td></tr>\n");
                }
            return FinishSummary(sb);
        }

        private void SummarizeAnswers(StringBuilder sb, OrganizationMember om)
        {
            if (Parent.SupportMissionTrip)
                return;

            var rr = person.RecRegs.Single();

            foreach (var ask in setting.AskItems)
            {
                switch (ask.Type)
                {
                    case "AskTickets":
                        sb.AppendFormat("<tr><td>Tickets:</td><td>{0}</td></tr>\n", om.Tickets);
                        break;
                    case "AskSize":
                        sb.AppendFormat("<tr><td>Shirt:</td><td>{0}</td></tr>\n", om.ShirtSize);
                        break;
                    case "AskEmContact":
                        sb.AppendFormat("<tr><td>Emerg Contact:</td><td>{0}</td></tr>\n", rr.Emcontact);
                        sb.AppendFormat("<tr><td>Emerg Phone:</td><td>{0}</td></tr>\n", rr.Emphone);
                        break;
                    case "AskDoctor":
                        sb.AppendFormat("<tr><td>Physician Name:</td><td>{0}</td></tr>\n", rr.Doctor);
                        sb.AppendFormat("<tr><td>Physician Phone:</td><td>{0}</td></tr>\n", rr.Docphone);
                        break;
                    case "AskInsurance":
                        sb.AppendFormat("<tr><td>Insurance Carrier:</td><td>{0}</td></tr>\n", rr.Insurance);
                        sb.AppendFormat("<tr><td>Insurance Policy:</td><td>{0}</td></tr>\n", rr.Policy);
                        break;
                    case "AskRequest":
                        sb.AppendFormat("<tr><td>{1}:</td><td>{0}</td></tr>\n", om.Request, ((AskRequest)ask).Label);
                        break;
                    case "AskHeader":
                        sb.AppendFormat("<tr><td colspan='2'><h4>{0}</h4></td></tr>\n", ((AskHeader)ask).Label);
                        break;
                    case "AskInstruction":
                        break;
                    case "AskAllergies":
                        sb.AppendFormat("<tr><td>Medical:</td><td>{0}</td></tr>\n", rr.MedicalDescription);
                        break;
                    case "AskTylenolEtc":
                        sb.AppendFormat("<tr><td>Tylenol?: {0},", tylenol == true ? "Yes" : tylenol == false ? "No" : "");
                        sb.AppendFormat(" Advil?: {0},", advil == true ? "Yes" : advil == false ? "No" : "");
                        sb.AppendFormat(" Robitussin?: {0},", robitussin == true ? "Yes" : robitussin == false ? "No" : "");
                        sb.AppendFormat(" Maalox?: {0}</td></tr>\n", maalox == true ? "Yes" : maalox == false ? "No" : "");
                        break;
                    case "AskChurch":
                        sb.AppendFormat("<tr><td>Member:</td><td>{0}</td></tr>\n", rr.Member);
                        sb.AppendFormat("<tr><td>OtherChurch:</td><td>{0}</td></tr>\n", rr.ActiveInAnotherChurch);
                        break;
                    case "AskParents":
                        sb.AppendFormat("<tr><td>Mother's name:</td><td>{0}</td></tr>\n", rr.Mname);
                        sb.AppendFormat("<tr><td>Father's name:</td><td>{0}</td></tr>\n", rr.Fname);
                        break;
                    case "AskCoaching":
                        sb.AppendFormat("<tr><td>Coaching:</td><td>{0}</td></tr>\n", rr.Coaching);
                        break;
                    case "AskSMS":
                        sb.AppendFormat("<tr><td>Receive Texts:</td><td>{0}</td></tr>\n", person.ReceiveSMS);
                        break;
                    case "AskDropdown":
                        SummarizeDropdownChoice(sb, ask);
                        break;
                    case "AskMenu":
                        SummarizeMenuChoices(sb, ask);
                        break;
                    case "AskCheckboxes":
                        SummarizeCheckboxChoices(sb, ask);
                        break;
                    case "AskYesNoQuestions":
                        SummarizeYesNoChoices(sb, ask);
                        break;
                    case "AskExtraQuestions":
                        SummarizeExtraAnswers(sb, ask);
                        break;
                    case "AskText":
                        SummarieTextAnswers(sb, ask);
                        break;
                    case "AskGradeOptions":
                        SummarizeGradeOption(sb, ask);
                        break;
                }
            }
            if (setting.AgeGroups.Count > 0)
                sb.AppendFormat("<tr><td>AgeGroup:</td><td>{0}</td></tr>\n", AgeGroup());
        }

        private void SummarizeDropdownChoice(StringBuilder sb, Ask ask)
        {
            sb.AppendFormat("<tr><td>{1}:</td><td>{0}</td></tr>\n",
                ((AskDropdown)ask).SmallGroupChoice(option).Description,
                Util.PickFirst(((AskDropdown)ask).Label, "Options"));
        }

        private void SummarizeGradeOption(StringBuilder sb, Ask ask)
        {
            sb.AppendFormat("<tr><td>GradeOption:</td><td>{0}</td></tr>\n",
                GradeOptions(ask).SingleOrDefault(s => s.Value == (gradeoption ?? "00")).Text);
        }

        private void SummarieTextAnswers(StringBuilder sb, Ask ask)
        {
            foreach (var a in Text[ask.UniqueId])
                if (a.Value.HasValue())
                    sb.Append($"<tr><td>{a.Key}:</td><td>{a.Value}</td></tr>\n");
        }

        private void SummarizeExtraAnswers(StringBuilder sb, Ask ask)
        {
            foreach (var a in ExtraQuestion[ask.UniqueId])
                if (a.Value.HasValue())
                    sb.Append($"<tr><td>{a.Key}:</td><td>{a.Value}</td></tr>\n");
        }

        private void SummarizeYesNoChoices(StringBuilder sb, Ask ask)
        {
            foreach (var a in ((AskYesNoQuestions)ask).list)
                if (YesNoQuestion.ContainsKey(a.SmallGroup))
                    sb.Append($"<tr><td>{a.Question}:</td><td>{(YesNoQuestion[a.SmallGroup] == true ? "Yes" : "No")}</td></tr>\n");
        }

        private void SummarizeCheckboxChoices(StringBuilder sb, Ask ask)
        {
            var askcb = (AskCheckboxes)ask;
            var menulabel = askcb.Label;
            foreach (var i in askcb.CheckboxItemsChosen(Checkbox))
            {
                string row;
                if (menulabel.HasValue())
                    sb.Append($"<tr><td colspan='2'><br>{menulabel}</td></tr>\n");
                if (i.Fee > 0)
                    row = $"<tr><td></td><td>{i.Description} (${i.Fee:N2})<br>({i.SmallGroup})</td></tr>\n";
                else
                    row = $"<tr><td></td><td>{i.Description}<br>({i.SmallGroup})</td></tr>\n";
                sb.Append(row);
                menulabel = string.Empty;
            }
        }

        private void SummarizeMenuChoices(StringBuilder sb, Ask ask)
        {
            var menulabel = ((AskMenu)ask).Label;
            foreach (var i in ((AskMenu)ask).MenuItemsChosen(MenuItem[ask.UniqueId]))
            {
                string row;
                if (i.amt > 0)
                    row = $"<tr><td>{menulabel}</td><td>{i.number} {i.desc} (at {i.amt:N2})</td></tr>\n";
                else
                    row = $"<tr><td>{menulabel}</td><td>{i.number} {i.desc}</td></tr>\n";
                sb.Append(row);
                menulabel = string.Empty;
            }
        }
    }
}
