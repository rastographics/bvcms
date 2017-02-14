using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CmsData.Registration;
using Elmah;
using HandlebarsDotNet;
using UtilityExtensions;

namespace CmsData.OnlineRegSummaryText
{
    public class SummaryInfo
    {
        private CMSDataContext db;

        public SummaryInfo(CMSDataContext db, int pid, int oid)
        {
            this.db = db;
            OrgMember = OrganizationMember.Load(db, pid, oid);
            Person = db.LoadPersonById(pid);
            Organization = db.LoadOrganizationById(oid);
            if (!OrgMember.OnlineRegData.HasValue())
                return;
            // ReSharper disable once UseObjectOrCollectionInitializer
            RegPerson = new OnlineRegPersonModel0(OrgMember.OnlineRegData, db);
            if (RegPerson.PeopleId == 0)
            {
                RegPerson.PeopleId = pid;
                RegPerson.person = Person;
            }
            RegPerson.setting = db.CreateRegistrationSettings(Organization.RegSettingXml, oid);

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

            Handlebars.RegisterHelper("TopLabel", (writer, context, args) => { writer.Write(TopLabel); });
            Handlebars.RegisterHelper("BottomStyle", (writer, context, args) => { writer.Write(RowStyle); });

            PythonModel.RegisterHelpers(db);
        }

        public static string GetResults(CMSDataContext db, int pid, int oid)
        {
            var host = db.Host;
            try
            {
                var si = new SummaryInfo(db, pid, oid);
                var template = Handlebars.Compile(Properties.Resources.Details2);
                var ret = template(si);
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception($@"
disposed: {db.IsDisposed}
message: {ex.Message}
host: {host}
oid: {oid}
pid: {pid}
", ex);
            }
        }

        private string RowStyle { get; set; }
        private string TopLabel { get; set; }

        private OrganizationMember OrgMember { get; }
        private Organization Organization { get; }
        private Person Person { get; set; }
        private OnlineRegPersonModel0 RegPerson { get; set; }
        private Ask currentAsk;

        private bool SupportMissionTrip => RegPerson.MissionTripSupportGeneral > 0 || RegPerson.MissionTripGoerId > 0;

        private void Registrant(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            options.Template(writer, new
            {
                Person.Name,
                Organization.OrganizationName,
            });
        }

        private void AskItems(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (SupportMissionTrip)
                return;
            var list = RegPerson.setting.AskItems;
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
            var ti = db.Transactions.SingleOrDefault(tt => tt.Id == OrgMember.TranId);
            if (ti == null || (ti.Amt ?? 0) == 0)
                return;
            var ts = OrgMember.TransactionSummary(db);
            var amtFee = (ts.IndPaid + ts.IndDue);
            var amtDonation = ts.IsDonor ? ts.Donation : 0;
            var info = new
            {
                AmtFee = amtFee.ToString2("c"),
                AmtDonation = amtDonation.ToString("c"),
                AmtPaid = OrgMember.AmountPaidTransactions(db).ToString2("c"),
                AmtDue = OrgMember.AmountDueTransactions(db).ToString2("c"),
                HasDonation = ts.IsDonor,
            };
            options.Template(writer, info);
        }
        private void IfSupportMissionTrip(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (!SupportMissionTrip)
                return;
            var goer = db.LoadPersonById(RegPerson.MissionTripGoerId ?? 0);
            options.Template(writer, new
            {
                Goer = goer,
                HasGoer = RegPerson.MissionTripGoerId > 0,
                HasGeneralSupport = RegPerson.MissionTripSupportGeneral > 0,
            });
        }

        public void IfAskAllergies(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskAllergies)
                options.Template(writer, OrgMember.Person.GetRecReg());
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
            var recReg = OrgMember.Person.GetRecReg();
            options.Template(writer, new
            {
                ShowNotMember = recReg.Member == true ? "" : "Not",
                ShowNotActive = recReg.ActiveInAnotherChurch == true ? "" : "Not",
            });
        }
        private void IfAskCoaching(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskCoaching)
                options.Template(writer, OrgMember.Person.GetRecReg());
        }
        private void IfAskDoctor(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskDoctor)
                options.Template(writer, OrgMember.Person.GetRecReg());
        }
        private void IfAskDropdown(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskDropdown && RegPerson.option != null)
                options.Template(writer, new
                {
                    OptionsLabel = Util.PickFirst(((AskDropdown)currentAsk).Label, "Options"),
                    SubGroupChoice = ((AskDropdown)currentAsk).SmallGroupChoice(RegPerson.option).Description,
                });
        }
        private void IfAskEmContact(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskEmContact)
                options.Template(writer, OrgMember.Person.GetRecReg());
        }
        private void IfAskExtraQuestions(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (!currentAsk.IsAskExtraQuestions)
                return;
            var list = RegPerson.ExtraQuestion[currentAsk.UniqueId].Where(a => a.Value.HasValue()).ToList();
            if (!list.Any())
                return;
            var lastKey = list.Last().Key;
            foreach (var item in list)
            {
                RowStyle = lastKey == item.Key ? CssStyle.BottomBorder : CssStyle.PadBottom;
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
                options.Template(writer, new { GradeOption = RegPerson.GradeOptions(currentAsk)[RegPerson.gradeoption ?? "00"] });
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
                options.Template(writer, OrgMember.Person.GetRecReg());
        }
        private void IfAskMenu(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskMenu)
                options.Template(writer, (object)context);
        }
        private void IfAskParents(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskParents)
                options.Template(writer, OrgMember.Person.GetRecReg());
        }
        private void IfAskRequest(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (!currentAsk.IsAskRequest)
                return;
            options.Template(writer, new
            {
                ((AskRequest)currentAsk).Label,
                OrgMember.Request,
            });
        }
        private void IfAskTextQuestions(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (!currentAsk.IsAskText)
                return;
            var list = RegPerson.Text[((AskText)currentAsk).UniqueId].Where(a => a.Value.HasValue()).ToList();
            if (!list.Any())
                return;
            var lastKey = list.Last().Key;
            foreach (var item in list)
            {
                RowStyle = lastKey == item.Key ? CssStyle.BottomBorder : CssStyle.PadBottom;
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
                options.Template(writer, OrgMember);
        }
        private void IfAskTylenolEtc(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskTylenolEtc)
                options.Template(writer, new
                {
                    Tylenol = RegPerson.tylenol == true ? "Yes" : "No",
                    Advil = RegPerson.advil == true ? "Yes" : "No",
                    Maalox = RegPerson.maalox == true ? "Yes" : "No",
                    Robitussin = RegPerson.robitussin == true ? "Yes" : "No",
                });
        }
        private void IfAskSize(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (currentAsk.IsAskSize)
                options.Template(writer, OrgMember);
        }
        private void IfAskSms(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (!currentAsk.IsAskSms)
                return;
            if (RegPerson.person == null)
                RegPerson.person = db.LoadPersonById(RegPerson.PeopleId);
            options.Template(writer, new { RegPerson.person.ReceiveSMS });
        }
        private void IfAskYesNoQuestions(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (!currentAsk.IsAskYesNoQuestions)
                return;
            var list = ((AskYesNoQuestions)currentAsk).list
                .Where(a => RegPerson.YesNoQuestion.ContainsKey(a.SmallGroup)).ToList();
            if (!list.Any())
                return;
            var lastKey = list.Last().Question;
            foreach (var item in list)
            {
                RowStyle = lastKey == item.Question ? CssStyle.BottomBorder : CssStyle.PadBottom;
                options.Template(writer, new
                {
                    item.Question,
                    Answer = RegPerson.YesNoQuestion[item.SmallGroup] == true ? "Yes" : "No",
                });
            }
        }

        private void IfHasAgeGroups(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (RegPerson.setting.AgeGroups.Count == 0)
                return;
            options.Template(writer, new { AgeGroup = RegPerson.AgeGroup() });
        }
        private void Checkboxes(TextWriter writer, HelperOptions options, dynamic context, params object[] args)
        {
            if (!currentAsk.IsAskCheckboxes)
                return;
            var list = ((AskCheckboxes)currentAsk).CheckboxItemsChosen(RegPerson.Checkbox).ToList();
            if (!list.Any())
                return;
            var lastDesc = list.Last().Description;
            foreach (var item in list)
            {
                RowStyle = lastDesc == item.Description ? "" : CssStyle.PadBottom;
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
            if (!currentAsk.IsAskMenu || RegPerson.MenuItem == null)
                return;
            var obj = (AskMenu)currentAsk;
            if (RegPerson.MenuItem.Count <= obj.UniqueId)
                return;
            var list = obj.MenuItemsChosen(RegPerson.MenuItem[obj.UniqueId]).ToList();
            if (!list.Any())
                return;
            if (RegPerson.MenuItem == null)
                return;
            var lastDesc = list.Last().desc;
            var TopLabel = "Choices";
            foreach (var item in list)
            {
                RowStyle = lastDesc == item.desc ? CssStyle.BottomBorder : CssStyle.PadBottom;
                options.Template(writer, new
                {
                    TopLabel,
                    Number = item.number,
                    Desc = item.desc,
                    HasAmt = item.amt > 0,
                    DispAmt = item.amt.ToString("N2"),
                });
                Debug.Write(this.TopLabel);
                this.TopLabel = "";
            }
        }
    }
}