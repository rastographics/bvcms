using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using CmsData.Registration;
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
            om = OrganizationMember.Load(DbUtil.Db, pid, oid);
            p = new OnlineRegPersonModel0();
            p.ReadXml(om.OnlineRegData);
            p.setting = DbUtil.Db.CreateRegistrationSettings(om.Organization.GetRegSetting(), oid);

            Handlebars.RegisterHelper("Registrant", Registrant);
            Handlebars.RegisterHelper("IfShowTransaction", IfShowTransaction);
            Handlebars.RegisterHelper("IfSupportMissionTrip", IfSupportMissionTrip);
            Handlebars.RegisterHelper("AskItems", AskItems);

            Handlebars.RegisterHelper("IfAskAllergies", IfAskAllergies);
            Handlebars.RegisterHelper("IfAskCheckboxes", IfAskCheckboxes);
            Handlebars.RegisterHelper("IfAskCoaching", IfAskCoaching);
            Handlebars.RegisterHelper("IfAskChurch", IfAskChurch);
            Handlebars.RegisterHelper("IfAskDoctor", IfAskDoctor);
            Handlebars.RegisterHelper("IfAskDropdown", IfAskDropdown);
            Handlebars.RegisterHelper("IfAskEmContact", IfAskEmContact);
            Handlebars.RegisterHelper("IfAskExtraQuestions", IfAskExtraQuestions);
            Handlebars.RegisterHelper("IfAskGradeOptions", IfAskGradeOptions);
            Handlebars.RegisterHelper("IfAskHeader", IfAskHeader);
            Handlebars.RegisterHelper("IfAskInstruction", IfAskInstruction);
            Handlebars.RegisterHelper("IfAskInsurance", IfAskInsurance);
            Handlebars.RegisterHelper("IfAskMenu", IfAskMenu);
            Handlebars.RegisterHelper("IfAskParents", IfAskParents);
            Handlebars.RegisterHelper("IfAskRequest", IfAskRequest);
            Handlebars.RegisterHelper("IfAskSize", IfAskSize);
            Handlebars.RegisterHelper("IfAskSms", IfAskSms);
            Handlebars.RegisterHelper("IfAskTextQuestions", IfAskTextQuestions);
            Handlebars.RegisterHelper("IfAskTickets", IfAskTickets);
            Handlebars.RegisterHelper("IfAskTylenolEtc", IfAskTylenolEtc);
            Handlebars.RegisterHelper("IfAskYesNoQuestions", IfAskYesNoQuestions);

            Handlebars.RegisterHelper("IfHasAgeGroups", IfHasAgeGroups);
            Handlebars.RegisterHelper("MenuItems", MenuItems);
            Handlebars.RegisterHelper("Checkboxes", Checkboxes);
            RegisterOtherHelpers();
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

        private string rowstyle { get; set; }
        private string toplabel { get; set; }

        public OrganizationMember om { get; }
        public Organization org => om.Organization;
        public Person person => om.Person;
        public OnlineRegPersonModel0 p { get; set; }
        private Ask currentAsk;

        private bool supportMissionTrip => p.MissionTripSupportGeneral > 0 || p.MissionTripGoerId > 0;

        private void Registrant(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            options.Template(writer, new
            {
                om.Person.Name,
                om.Organization.OrganizationName,
            });
        }

        private void AskItems(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (supportMissionTrip)
                return;
            var list = p.setting.AskItems;
            if (!list.Any())
                return;
            foreach (var item in list)
            {
                currentAsk = item;
                options.Template(writer, item);
            }
        }
        private void IfShowTransaction(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            var ti = db.Transactions.SingleOrDefault(tt => tt.Id == om.TranId);
            if (ti == null || (ti.Amt ?? 0) == 0)
                return;
            var ts = om.TransactionSummary(db);
            options.Template(writer, new
            {
                IndAmt = ts.IndAmt.ToString2("c"),
                TotPaid = om.TotalPaid(db).ToString("c"),
                AmountDue = om.AmountDue(db).ToString("c"),
            });
        }
        private void IfSupportMissionTrip(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (!supportMissionTrip)
                return;
            var Goer = db.LoadPersonById(p.MissionTripGoerId ?? 0);
            options.Template(writer, new
            {
                Goer,
                HasGoer = p.MissionTripGoerId > 0,
                HasGeneralSupport = p.MissionTripSupportGeneral > 0,
            });
        }

        public void IfAskAllergies(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskAllergies)
                options.Template(writer, om.Person.GetRecReg());
        }
        private void IfAskCheckboxes(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskCheckboxes)
                options.Template(writer, (object)context);
        }
        private void IfAskChurch(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (!currentAsk.IsAskChurch)
                return;
            var RecReg = om.Person.GetRecReg();
            options.Template(writer, new
            {
                ShowNotMember = RecReg.Member == true ? "" : "Not",
                ShowNotActive = RecReg.ActiveInAnotherChurch == true ? "" : "Not",
            });
        }
        private void IfAskCoaching(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskCoaching)
                options.Template(writer, om.Person.GetRecReg());
        }
        private void IfAskDoctor(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskDoctor)
                options.Template(writer, om.Person.GetRecReg());
        }
        private void IfAskDropdown(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskDropdown)
                options.Template(writer, new
                {
                    OptionsLabel = Util.PickFirst(((AskDropdown)currentAsk).Label, "Options"),
                    SubGroupChoice = ((AskDropdown)currentAsk).SmallGroupChoice(p.option).Description,
                });
        }
        private void IfAskEmContact(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskEmContact)
                options.Template(writer, om.Person.GetRecReg());
        }
        private void IfAskExtraQuestions(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (!currentAsk.IsAskExtraQuestions)
                return;
            var list = p.ExtraQuestion[currentAsk.UniqueId].Where(a => a.Value.HasValue()).ToList();
            if (!list.Any())
                return;
            var lastKey = list.Last().Key;
            foreach (var item in list)
            {
                rowstyle = lastKey == item.Key ? BottomBorder : PadBottom;
                options.Template(writer, new
                {
                    Question = item.Key,
                    Answer = item.Value,
                });
            }
        }
        private void IfAskGradeOptions(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskGradeOptions)
                options.Template(writer, new { GradeOption = p.GradeOptions(currentAsk)[p.gradeoption ?? "00"] });
        }
        private void IfAskHeader(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskHeader)
                options.Template(writer, new { ((AskHeader)currentAsk).Label });
        }
        private void IfAskInstruction(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskInstruction)
                options.Template(writer, new { ((AskInstruction)currentAsk).Label });
        }
        private void IfAskInsurance(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskInsurance)
                options.Template(writer, om.Person.GetRecReg());
        }
        private void IfAskMenu(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskMenu)
                options.Template(writer, (object)context);
        }
        private void IfAskParents(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskParents)
                options.Template(writer, om.Person.GetRecReg());
        }
        private void IfAskRequest(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (!currentAsk.IsAskRequest)
                return;
            options.Template(writer, new
            {
                ((AskRequest)currentAsk).Label,
                om.Request,
            });
        }
        private void IfAskTextQuestions(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (!currentAsk.IsAskText)
                return;
            var list = p.Text[((AskText)currentAsk).UniqueId].Where(a => a.Value.HasValue()).ToList();
            if (!list.Any())
                return;
            var lastKey = list.Last().Key;
            foreach (var item in list)
            {
                rowstyle = lastKey == item.Key ? BottomBorder : PadBottom;
                options.Template(writer, new
                {
                    Question = item.Key,
                    Answer = item.Value,
                });
            }
        }
        private void IfAskTickets(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskTickets)
                options.Template(writer, om);
        }
        private void IfAskTylenolEtc(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskTylenolEtc)
                options.Template(writer, new
                {
                    Tylenol = p.tylenol == true ? "Yes" : "No",
                    Advil = p.advil == true ? "Yes" : "No",
                    Maalox = p.maalox == true ? "Yes" : "No",
                    Robitussin = p.robitussin == true ? "Yes" : "No",
                });
        }
        private void IfAskSize(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskSize)
                options.Template(writer, om);
        }
        private void IfAskSms(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskSms)
                options.Template(writer, new { p.person.ReceiveSMS });
        }
        private void IfAskYesNoQuestions(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (!currentAsk.IsAskYesNoQuestions)
                return;
            var list = ((AskYesNoQuestions)currentAsk).list
                .Where(a => p.YesNoQuestion.ContainsKey(a.SmallGroup)).ToList();
            if (!list.Any())
                return;
            var lastKey = list.Last().Question;
            foreach (var item in list)
            {
                rowstyle = lastKey == item.Question ? BottomBorder : PadBottom;
                options.Template(writer, new
                {
                    item.Question,
                    Answer = p.YesNoQuestion[item.SmallGroup] == true ? "Yes" : "No",
                });
            }
        }

        private void IfHasAgeGroups(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (p.setting.AgeGroups.Count == 0)
                return;
            options.Template(writer, new { AgeGroup = p.AgeGroup() });
        }
        private void RegisterOtherHelpers()
        {
            Handlebars.RegisterHelper("TopLabel", (writer, context, args) => { writer.Write(toplabel); });

            Handlebars.RegisterHelper("BottomStyle", (writer, context, args) => { writer.Write(rowstyle); });
            Handlebars.RegisterHelper("BottomBorder", (writer, context, args) => { writer.Write(BottomBorder); });
            Handlebars.RegisterHelper("AlignTop", (writer, context, args) => { writer.Write(AlignTop); });
            Handlebars.RegisterHelper("AlignRight", (writer, context, args) => { writer.Write(AlignRight); });
            Handlebars.RegisterHelper("DataLabelStyle", (writer, context, args) => { writer.Write(DataLabelStyle); });
            Handlebars.RegisterHelper("LabelStyle", (writer, context, args) => { writer.Write(LabelStyle); });
            Handlebars.RegisterHelper("DataStyle", (writer, context, args) => { writer.Write(DataStyle); });
        }

        private void Checkboxes(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (!currentAsk.IsAskCheckboxes)
                return;
            var list = ((AskCheckboxes)currentAsk).CheckboxItemsChosen(p.Checkbox).ToList();
            if (!list.Any())
                return;
            var lastDesc = list.Last().Description;
            foreach (var item in list)
            {
                rowstyle = lastDesc == item.Description ? "" : PadBottom;
                options.Template(writer, new
                {
                    DisplayDescription = item.Fee > 0
                        ? $"{item.Description} (${item.Fee.ToString2("N2")}<br/>({item.SmallGroup})"
                        : $"{item.Description}<br/>({item.SmallGroup})",
                });
            }
        }
        private void MenuItems(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (!currentAsk.IsAskMenu || p.MenuItem == null)
                return;
            var obj = (AskMenu)currentAsk;
            var list = obj.MenuItemsChosen(p.MenuItem[obj.UniqueId]).ToList();
            if (!list.Any())
                return;
            if (p.MenuItem == null)
                return;
            var lastDesc = list.Last().desc;
            var TopLabel = "Choices";
            foreach (var item in list)
            {
                rowstyle = lastDesc == item.desc ? BottomBorder : PadBottom;
                options.Template(writer, new
                {
                    TopLabel,
                    Number = item.number,
                    Desc = item.desc,
                    HasAmt = item.amt > 0,
                    DispAmt = item.amt.ToString("N2"),
                });
                Debug.Write(toplabel);
                toplabel = "";
            }
        }
    }
}