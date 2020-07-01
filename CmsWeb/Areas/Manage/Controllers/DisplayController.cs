using CmsData.Codes;
using CmsWeb.Code;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System;
using System.Linq;
// Used for HTML Image Capture
using System.Web;
using System.Web.Mvc;
using CmsWeb.Areas.Main.Models;
using CmsWeb.Areas.Manage.Models;
using Newtonsoft.Json;
using UtilityExtensions;
using Content = CmsData.Content;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin,Design")]
    [ValidateInput(false)]
    [RouteArea("Manage", AreaPrefix = "Display"), Route("{action}/{id?}")]
    public class DisplayController : CmsStaffController
    {
        public DisplayController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/Display")]
        [Route("~/Manage/Display")]
        public ActionResult Index()
        {
            return View(new ContentModel(CurrentDatabase));
        }

        [HttpPost]
        public ActionResult SetContentKeywordFilter(string keywordfilter, string tab)
        {
            if (keywordfilter.HasValue())
            {
                Util.ContentKeywordFilter = keywordfilter.Trim();
            }
            else
            {
                Util.ContentKeywordFilter = null;
            }

            return Redirect($"/Display#tab_{tab}");
        }

        public ActionResult ContentEdit(int? id, bool? snippet)
        {
            if (!id.HasValue)
            {
                throw new HttpException(404, "No ID found.");
            }

            var q = from c in CurrentDatabase.Contents
                    where c.Id == id.Value
                    select new { content = c, keywords = string.Join(",", c.ContentKeyWords.Select(vv => vv.Word)) };
            var i = q.SingleOrDefault();
            if (i == null)
            {
                throw new HttpException(404, "No ID found.");
            }

            if (snippet == true)
            {
                i.content.Snippet = true;
                CurrentDatabase.SubmitChanges();
            }
            ViewBag.ContentKeywords = i.keywords;
            if (ContentTypeCode.IsUnlayer(i.content.TypeID))
            {
                ViewBag.TemplateId = id;
                return View("UnLayerCompose", i.content);
            }
            return RedirectEdit(i.content);
        }
        public ActionResult Content(int? id)
        {
            if (!id.HasValue)
            {
                throw new HttpException(404, "No ID found.");
            }

            var content = CurrentDatabase.ContentFromID(id.Value);
            if (content == null)
            {
                throw new HttpException(404, "No ID found.");
            }

            return Content(content.Body);
        }

        [HttpPost]
        public ActionResult ContentCreate(int newType, string newName, int? newRole, bool? useUnlayer)
        {
            var content = new Content();
            content.Name = newName;
            if (!content.Name.HasValue())
                content.Name = "new template " + DateTime.Now.FormatDateTm();
            content.TypeID = newType == ContentTypeCode.TypeEmailTemplate && useUnlayer == true
                ? ContentTypeCode.TypeUnlayerTemplate
                : newType;
            content.RoleID = newRole ?? 0;
            content.Body = "";
            content.Title = "";
            content.DateCreated = DateTime.Now;
            content.CreatedBy = Util.UserName;
            var contentKeywordFilter = Util.ContentKeywordFilter;
            if (contentKeywordFilter.HasValue())
            {
                content.SetKeyWords(CurrentDatabase, new[] { contentKeywordFilter });
            }
            var tid = EmailTemplatesModel.FetchTemplateByName("Empty Template", CurrentDatabase).Id;
            if (ContentTypeCode.EmailTemplates.Contains(content.TypeID))
                content.Id = tid;

            CurrentDatabase.Contents.InsertOnSubmit(content);
            CurrentDatabase.SubmitChanges();
            ViewBag.ContentKeywords = contentKeywordFilter ?? "";

            if (useUnlayer == true)
            {
                ViewBag.TemplateId = 0; // force using a blank empty template with only "click here to edit content"
                return View("UnLayerCompose", content);
            }

            return RedirectEdit(content);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveUnlayerTemplateCopy(int saveid, string name, int roleid, string title, string body, string unlayerDesign)
        {
            var m = GetDisplayModel();
            var existing = m.SaveUnlayerTemplateCommon(saveid, name, roleid, title, body, unlayerDesign);
            var content = m.Clone(existing);
            m.SaveThumbnail(body, content);
            CurrentDatabase.Contents.InsertOnSubmit(content);
            CurrentDatabase.SubmitChanges();
            return Redirect($"/Display/ContentEdit/{content.Id}");
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveUnlayerTemplate(int? saveid, string name, int roleid, string title, string body, string unlayerDesign)
        {
            var m = GetDisplayModel();
            var content = m.SaveUnlayerTemplateCommon(saveid, name, roleid, title, body, unlayerDesign);

            ViewBag.templateID = content.Id;
            var templatedraft = content.TypeID == ContentTypeCode.TypeUnlayerTemplate ? "emailTemplates" : "savedDrafts";
            return Redirect($"/Display/#tab_{templatedraft}");
        }

        // This method only applies to TypeEmailTemplate
        [HttpPost]
        public ActionResult ContentUpdateCopy(int id, string name, string title, string body, bool? snippet, int? roleid, string contentKeyWords, string stayaftersave = null)
        {
            var m = GetDisplayModel();
            var existing = m.ContentUpdateCommon(id, name, title, body, snippet, roleid, contentKeyWords);
            var content = m.Clone(existing);
            m.SaveThumbnail(body, content);
            CurrentDatabase.Contents.InsertOnSubmit(content);
            CurrentDatabase.SubmitChanges();
            return Redirect($"/Display/ContentEdit/{content.Id}");
        }
        [HttpPost]
        public ActionResult ContentUpdate(int id, string name, string title, string body, bool? snippet, int? roleid, string contentKeyWords, string stayaftersave = null)
        {
            var m = GetDisplayModel();
            var content = m.ContentUpdateCommon(id, name, title, body, snippet, roleid, contentKeyWords);

            if (string.Compare(content.Name, "CustomReportsMenu", StringComparison.OrdinalIgnoreCase) == 0)
            {
                try
                {
                    var list = ReportsMenuModel.CustomItems1;
                }
                catch (Exception ex)
                {
                    if (ex is System.Xml.XmlException)
                    {
                        return Content(Util.EndShowMessage(ex.Message, "javascript: history.go(-1)",
                            "There is invalid XML in this document. Go Back to Repair"));
                    }
                }
            }
            else if (string.Compare(content.Name, "CustomReports", StringComparison.OrdinalIgnoreCase) == 0)
            {
                try
                {
                    var list = CurrentDatabase.ViewCustomScriptRoles.ToList();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("XML parsing", ignoreCase: true))
                    {
                        return Content(Util.EndShowMessage(ex.InnerException?.Message, "javascript: history.go(-1)",
                            "There is invalid XML in this document. Go Back to Repair"));
                    }
                }
            }
            else if (string.Compare(content.Name, "StandardExtraValues2", StringComparison.OrdinalIgnoreCase) == 0)
            {
                try
                {
                    CmsData.ExtraValue.Views.GetStandardExtraValues(CurrentDatabase, "People");
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.InnerException is System.Xml.XmlException)
                    {
                        return Content(Util.EndShowMessage(ex.InnerException.Message, "javascript: history.go(-1)",
                            "There is invalid XML in this document. Go Back to Repair"));
                    }
                }
            }

            if (stayaftersave == "true")
            {
                return RedirectToAction("ContentEdit", new {id = id, snippet = snippet });
            }

            var url = m.GetIndexTabUrl(content);
            return Redirect(url);
        }
        public ActionResult ContentDelete(int id)
        {
            var m = GetDisplayModel();
            var content = CurrentDatabase.ContentFromID(id);
            CurrentDatabase.ExecuteCommand("DELETE FROM dbo.ContentKeywords WHERE Id = {0}", id);
            CurrentDatabase.ExecuteCommand("DELETE FROM dbo.Content WHERE Id = {0}", id);
            var url = m.GetIndexTabUrl(content);
            return Redirect(url);
        }

        public ActionResult ContentDeleteDrafts(string[] draftID)
        {
            string deleteList = string.Join(",", draftID.Select(d => d.ToInt()));//Prevent potential SQL injection
            CurrentDatabase.ExecuteCommand($"DELETE FROM dbo.ContentKeywords WHERE Id IN({deleteList})");
            CurrentDatabase.ExecuteCommand($"DELETE FROM dbo.Content WHERE Id IN({deleteList})", "");
            return Redirect("/Display#tab_savedDrafts");
        }

        public ActionResult RedirectEdit(Content cContent)
        {
            switch (cContent.TypeID) // 0 = HTML, 1 = Text, 2 = eMail Template
            {
                case ContentTypeCode.TypeHtml:
                    cContent.RemoveGrammarly();
                    return View("EditHTML", cContent);

                case ContentTypeCode.TypeText:
                    return View("EditText", cContent);

                case ContentTypeCode.TypeSqlScript:
                    return View("EditSqlScript", cContent);

                case ContentTypeCode.TypePythonScript:
                    ViewBag.SimpleTextarea = CurrentDatabase.UserPreference("SimpleTextarea", "false");
                    return View("EditPythonScript", cContent);

                case ContentTypeCode.TypeEmailTemplate:
                    cContent.RemoveGrammarly();
                    return View("EditTemplate", cContent);

                case ContentTypeCode.TypeSavedDraft:
                    cContent.RemoveGrammarly();
                    return View("EditDraft", cContent);
            }
            return View("Index");
        }

        [HttpPost]
        public ActionResult SavePythonScript(string name, string body, string contentKeyWords)
        {
            var content = CurrentDatabase.Content(name, "", ContentTypeCode.TypePythonScript);
            content.Body = body;
            CurrentDatabase.SubmitChanges();
            return new EmptyResult();
        }

        [Route("~/Manage/Display/EmailBody")]
        public ActionResult EmailBody(string id)
        {
            if (id == "0") // used for unlayer, force using a blank empty template with only "click here to edit content"
            {
                ViewBag.body = "<div class='bvedit'>Click here to edit content</div>";
                ViewBag.design = "";
                ViewBag.useUnlayer = true;
                return View();
            }
            var i = id.ToInt();
            var c = ViewExtensions2.GetContent(i);
            if (c == null)
            {
                return new EmptyResult();
            }

            var design = string.Empty;
            var body = string.Empty;

            if (ContentTypeCode.IsUnlayer(c.TypeID))
            {
                if (!c.Body.HasValue())
                {
                    c.Body = "<div class='bvedit'>Click here to edit content</div>";
                }
                else
                {
                    dynamic payload = JsonConvert.DeserializeObject(c.Body);
                    design = payload.design;
                    body = payload.rawHtml;
                }
            }
            else
                body = c.Body;
            
            var doc = new HtmlDocument();
            doc.LoadHtml(body);
            var bvedits = doc.DocumentNode.SelectNodes("//div[contains(@class,'bvedit') or @bvedit]");
            if (bvedits == null || !bvedits.Any())
            {
                body = $"<div bvedit='discardthis'>{body}</div>";
            }

            ViewBag.body = body;
            ViewBag.design = design;
            ViewBag.useUnlayer = true;
            return View();
        }

        private DisplayModel GetDisplayModel() => new DisplayModel(CurrentDatabase, CurrentImageDatabase);
    }
}
