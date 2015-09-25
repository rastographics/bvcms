using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CmsData.Registration;
using CmsData.View;
using HandlebarsDotNet;
using UtilityExtensions;

namespace CmsData.OnlineRegSummaryText
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class SummaryInfo : Sty
    {
        private readonly CMSDataContext db;

        public SummaryInfo(CMSDataContext db, int pid, int oid)
        {
            this.db = db;
            RegisterHelpers();
            om = OrganizationMember.Load(DbUtil.Db, pid, oid);
            ti = DbUtil.Db.Transactions.SingleOrDefault(tt => tt.Id == om.TranId);
            if (ShowTransaction)
                ts = om.TransactionSummary(this.db);
            RecReg = om.Person.GetRecReg();
            p = new OnlineRegPersonModel0();
            p.ReadXml(om.OnlineRegData);
            p.setting = DbUtil.Db.CreateRegistrationSettings(om.Organization.GetRegSetting(), oid);
            Goer = db.LoadPersonById(p.MissionTripGoerId ?? 0);

        }

        public override string ToString()
        {
            try
            {
                var template = Handlebars.Compile(Properties.Resources.Details2);
                return template(this);
            }
            catch (Exception ex)
            {
                return ex.Message + "\n" + ex.StackTrace;
            }
        }

        private readonly Transaction ti;
        private readonly TransactionSummary ts;

        public OrganizationMember om { get; }
        public Organization org => om.Organization;
        public Person person => om.Person;

        public RecReg RecReg { get; set; }
        public string ilabel { get; set; }
        public bool ilabelHasValue => ilabel.HasValue();
        public string lastidesc { get; set; }
        public bool ShowTransaction => ti != null && (ti.Amt ?? 0) > 0 && om != null;
        public OnlineRegPersonModel0 p { get; set; }
        public string IndAmt => ts.IndAmt.ToString2("c");
        public string TotPaid => om.TotalPaid(db).ToString("c");
        public string AmountDue => om.AmountDue(db).ToString("c");
        public Person Goer { get; set; }
        public List<Ask> AskItems => p.setting.AskItems;
        public string AgeGroup => p.AgeGroup();

        public bool HasAgeGroups => p.setting.AgeGroups.Count > 0;
        public bool HasCheckedboxes => cblist.Any();
        public bool HasGoer => p.MissionTripGoerId > 0;
        public bool HasGeneralSupport => p.MissionTripSupportGeneral > 0;
        public bool HasMenuItems => mlist != null && mlist.Any();
        public bool SupportMissionTrip => p.MissionTripSupportGeneral > 0 || p.MissionTripGoerId > 0;

        public string ShowNotMember => RecReg.Member == true ? "" : "Not";
        public string ShowNotActive => RecReg.ActiveInAnotherChurch == true ? "" : "Not";
        public string rs { get; private set; }

        public List<AskMenu.MenuItemChosen> mlist { get; set; }
        public List<AskCheckboxes.CheckboxItem> cblist { get; set; }
        public List<AskYesNoQuestions.YesNoQuestion> ynlist { get; set; }
        public List<KeyValuePair<string, string>> eqlist { get; set; }
        public List<KeyValuePair<string, string>> txlist { get; set; }

        private void RegisterHelpers()
        {
            Handlebars.RegisterHelper("YesNo", (writer, context, args) =>
            {
                var tf = (bool?)(args[0]);
                writer.Write(tf == true ? "Yes" : tf == false ? "No" : "");
            });
            Handlebars.RegisterHelper("SubGroupChoice", (writer, context, args) =>
            {
                var ask = args[0] as AskDropdown;
                if (ask == null)
                    return;
                writer.Write(ask.SmallGroupChoice(p.option).Description);
            });
            Handlebars.RegisterHelper("YesNoAnswer", (writer, context, args) =>
            {
                var subGroup = args[0].ToString();
                writer.Write(p.YesNoQuestion[subGroup] == true ? "Yes" : "No");
            });
            Handlebars.RegisterHelper("SetMenu", (writer, context, args) =>
            {
                var menu = args[0] as AskMenu;
                if (menu == null)
                    return;
                if (p.MenuItem == null)
                    return;
                mlist = menu.MenuItemsChosen(p.MenuItem[menu.UniqueId]).ToList(); // HERE
                if (mlist.Any())
                    lastidesc = mlist.Last().desc;
            });
            Handlebars.RegisterHelper("SetCheckbox", (writer, context, args) =>
            {
                var cb = args[0] as AskCheckboxes;
                if (cb == null)
                    return;
                cblist = cb.CheckboxItemsChosen(p.Checkbox).ToList(); // HERE
                ilabel = cb.Label;
                if (cblist.Any())
                    lastidesc = cblist.Last().Description;
            });
            Handlebars.RegisterHelper("SetYesNoQuestions", (writer, context, args) =>
            {
                var yn = args[0] as AskYesNoQuestions;
                if (yn == null)
                    return;
                ynlist = yn.list.Where(a => p.YesNoQuestion.ContainsKey(a.SmallGroup)).ToList(); // HERE
                if (ynlist.Any())
                    lastidesc = ynlist.Last().Question;
            });
            Handlebars.RegisterHelper("SetExtraQuestions", (writer, context, args) =>
            {
                var eq = args[0] as AskExtraQuestions;
                if (eq == null)
                    return;
                eqlist = p.ExtraQuestion[eq.UniqueId].Where(a => a.Value.HasValue()).ToList(); // HERE
                if (eqlist.Any())
                    lastidesc = eqlist.Last().Key;
            });
            Handlebars.RegisterHelper("SetTextQuestions", (writer, context, args) =>
            {
                var tx = args[0] as AskText;
                if (tx == null)
                    return;
                txlist = p.Text[tx.UniqueId].Where(a => a.Value.HasValue()).ToList(); // HERE
                if (txlist.Any())
                    lastidesc = txlist.Last().Key;
            });
            Handlebars.RegisterHelper("GradeOption", (writer, context, args) =>
            {
                var go = (AskGradeOptions)(args[0]);
                if (go == null)
                    return;
                writer.Write(p.GradeOptions(go)[p.gradeoption ?? "00"]); // HERE
            });
            Handlebars.RegisterHelper("SetLabel", (writer, context, args) => { ilabel = args[0] as string; });
            Handlebars.RegisterHelper("SetRowStyle",
                (writer, context, args) => { rs = lastidesc == args[0].ToString() ? args[1].ToString() : args[2].ToString(); });
        }
    }
}