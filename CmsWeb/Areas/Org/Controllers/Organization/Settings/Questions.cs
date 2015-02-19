using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using UtilityExtensions;
using Settings = CmsData.Registration.Settings;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrganizationController
    {
        [HttpPost]
        public ActionResult Questions(int id)
        {
            return View("Settings/Questions", getRegSettings(id));
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult QuestionsEdit(int id)
        {
            return PartialView("Settings/QuestionsEdit", getRegSettings(id));
        }
        [HttpPost]
        public ActionResult QuestionsUpdate(int id)
        {
            var m = getRegSettings(id);
            DbUtil.LogActivity("Update SettingsQuestions {0}".Fmt(m.org.OrganizationName));
            m.AskItems.Clear();
            m.TimeSlots.list.Clear();
            try
            {
                if (!TryUpdateModel(m))
                {
                    var q = from e in ModelState.Values
                            where e.Errors.Count > 0
                            select e.Errors[0].ErrorMessage;
                    throw new Exception(q.First());
                }
                string s = m.ToString();
                m = new Settings(s, DbUtil.Db, id);
                m.org.RegSetting = m.ToString();
                DbUtil.Db.SubmitChanges();
                if (!m.org.NotifyIds.HasValue())
                    ModelState.AddModelError("Form", needNotify);
                return PartialView("Settings/Questions", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form", ex.Message);
                return Content("error:" + ex.Message);
                //return View("OnlineRegQuestionsEdit", m);
            }
        }
        [HttpPost]
        public ActionResult NewMenuItem(string id)
        {
            return PartialView("EditorTemplates/MenuItem", new AskMenu.MenuItem { Name = id });
        }
        [HttpPost]
        public ActionResult NewDropdownItem(string id)
        {
            return PartialView("EditorTemplates/DropdownItem", new AskDropdown.DropdownItem { Name = id });
        }
        [HttpPost]
        public ActionResult NewCheckbox(string id)
        {
            return PartialView("EditorTemplates/CheckboxItem", new AskCheckboxes.CheckboxItem { Name = id });
        }
        [HttpPost]
        public ActionResult NewGradeOption(string id)
        {
            return PartialView("EditorTemplates/GradeOption", new AskGradeOptions.GradeOption { Name = id });
        }
        [HttpPost]
        public ActionResult NewYesNoQuestion(string id)
        {
            return PartialView("EditorTemplates/YesNoQuestion", new AskYesNoQuestions.YesNoQuestion { Name = id });
        }
        [HttpPost]
        public ActionResult NewSize(string id)
        {
            return PartialView("EditorTemplates/Size", new AskSize.Size { Name = id });
        }
        [HttpPost]
        public ActionResult NewExtraQuestion(string id)
        {
            return PartialView("EditorTemplates/ExtraQuestion", new AskExtraQuestions.ExtraQuestion { Name = id });
        }
        [HttpPost]
        public ActionResult NewText(string id)
        {
            return PartialView("EditorTemplates/Text", new AskExtraQuestions.ExtraQuestion { Name = id });
        }
        [HttpPost]
        public ActionResult NewTimeSlot(string id)
        {
            return PartialView("EditorTemplates/TimeSlot", new TimeSlots.TimeSlot { Name = id });
        }

        [HttpPost]
        public ActionResult NewAsk(string id, string type)
        {
            var template = "EditorTemplates/" + type;
            switch (type)
            {
                case "AnswersNotRequired":
                case "AskSMS":
                case "AskEmContact":
                case "AskInsurance":
                case "AskDoctor":
                case "AskAllergies":
                case "AskTylenolEtc":
                case "AskParents":
                case "AskCoaching":
                case "AskChurch":
                    return PartialView(template, new Ask(type) { Name = id });
                case "AskCheckboxes":
                    return PartialView(template, new AskCheckboxes() { Name = id });
                case "AskDropdown":
                    return PartialView(template, new AskDropdown() { Name = id });
                case "AskMenu":
                    return PartialView(template, new AskMenu() { Name = id });
                case "AskSuggestedFee":
                    return PartialView(template, new AskSuggestedFee() { Name = id });
                case "AskSize":
                    return PartialView(template, new AskSize() { Name = id });
                case "AskRequest":
                    return PartialView(template, new AskRequest() { Name = id });
                case "AskHeader":
                    return PartialView(template, new AskHeader() { Name = id });
                case "AskInstruction":
                    return PartialView(template, new AskInstruction() { Name = id });
                case "AskTickets":
                    return PartialView(template, new AskTickets() { Name = id });
                case "AskYesNoQuestions":
                    return PartialView(template, new AskYesNoQuestions() { Name = id });
                case "AskExtraQuestions":
                    return PartialView(template, new AskExtraQuestions() { Name = id });
                case "AskText":
                    return PartialView(template, new AskText() { Name = id });
                case "AskGradeOptions":
                    return PartialView(template, new AskGradeOptions() { Name = id });
            }
            return Content("unexpected type " + type);
        }
    }
}