using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmsData;
using CmsData.Classes.OnlineReg;
using CmsData.Registration;
using CmsData.View;
using HandlebarsDotNet;
using UtilityExtensions;

namespace CmsData.API
{
    public class SummaryInfo : Sty
    {
        private readonly CMSDataContext db;

        public SummaryInfo(CMSDataContext db)
        {
            this.db = db;
            Handlebars.RegisterHelper("YesNo", (writer, options, context, args) =>
            {
                var tf = (bool?) (args[0]);
                writer.Write(tf == true ? "Yes" : tf == false ? "No" : "");
            });
            Handlebars.RegisterHelper("SubGroupChoice", (writer, options, context, args) =>
            {
                var ask = (AskDropdown) (args[0]);
                writer.Write(ask.SmallGroupChoice(p.option).Description);
            });
            Handlebars.RegisterHelper("YesNoAnswer", (writer, options, context, args) =>
            {
                var SubGroup = args[0].ToString();
                writer.Write(p.YesNoQuestion[SubGroup] == true ? "Yes" : "No");
            });
            Handlebars.RegisterHelper("SetMenu", (writer, options, context, args) =>
            {
                var menu = args[0] as AskMenu;
                if (menu == null)
                    return;
                mlist = menu.MenuItemsChosen(p.MenuItem[menu.UniqueId]).ToList();
                if (mlist.Any())
                    lastidesc = mlist.Last().desc;
            });
            Handlebars.RegisterHelper("SetCheckbox", (writer, options, context, args) =>
            {
                var cb = args[0] as AskCheckboxes;
                if (cb == null)
                    return;
                cblist = cb.CheckboxItemsChosen(p.Checkbox).ToList();
                ilabel = cb.Label;
                if (cblist.Any())
                    lastidesc = cblist.Last().Description;
            });
            Handlebars.RegisterHelper("SetYesNoQuestions", (writer, options, context, args) =>
            {
                var yn = args[0] as AskYesNoQuestions;
                if (yn == null)
                    return;
                ynlist = yn.list.Where(a => p.YesNoQuestion.ContainsKey(a.SmallGroup)).ToList();
                if (ynlist.Any())
                    lastidesc = ynlist.Last().Question;
            });
            Handlebars.RegisterHelper("SetExtraQuestions", (writer, options, context, args) =>
            {
                var eq = args[0] as AskExtraQuestions;
                if (eq == null)
                    return;
                eqlist = p.ExtraQuestion[eq.UniqueId].Where(a => a.Value.HasValue()).ToList();
                if (eqlist.Any())
                    lastidesc = eqlist.Last().Key;
            });
            Handlebars.RegisterHelper("SetTextQuestions", (writer, options, context, args) =>
            {
                var tx = args[0] as AskText;
                if (tx == null)
                    return;
                txlist = p.Text[tx.UniqueId].Where(a => a.Value.HasValue()).ToList();
                if (txlist.Any())
                    lastidesc = txlist.Last().Key;
            });
            Handlebars.RegisterHelper("GradeOption", (writer, options, context, args) =>
            {
                var go = (AskGradeOptions) (args[0]);
                if (go == null)
                    return;
                writer.Write(p.GradeOptions(go).SingleOrDefault(s => s.Value == (p.gradeoption ?? "00"))?.Text);
            });
            Handlebars.RegisterHelper("SetLabel", (writer, options, context, args) =>
                ilabel = args[0] as string);
            Handlebars.RegisterHelper("SetRowStyle", (writer, options, context, args) =>
                rs = lastidesc == args[0].ToString() ? args[1].ToString() : args[2].ToString());

            var template = Handlebars.Compile(CmsData.Properties.Resources.Details2);
        }

        public string PrepareSummaryInfo(OnlineRegPersonModel p)
        {
            this.p = p;
            om = p.GetOrgMember();
            ti = p.Parent.Transaction;
            if (ShowTransaction)
                ts = om.TransactionSummary(this.db);
            rr = p.person.GetRecReg();
        }

        // ReSharper disable InconsistentNaming
        // ReSharper disable NotAccessedField.Local
        private RecReg rr;
        public string ilabel { get; set; }
        public string lastidesc { get; set; }
        public Transaction ti { get; set; }
        public bool ShowTransaction => (ti.Amt ?? 0) > 0 && om != null;
        public OrganizationMember om { get; set; }
        public OnlineRegPersonModel p { get; set; }
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
    }
}