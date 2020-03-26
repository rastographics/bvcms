using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using Elmah;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
// Used for HTML Image Capture
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using CmsWeb.Areas.Main.Models;
using Newtonsoft.Json;
using UtilityExtensions;
using Content = CmsData.Content;
using Encoder = System.Drawing.Imaging.Encoder;
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
            var ContentKeywordFilter = Util.ContentKeywordFilter;
            if (ContentKeywordFilter.HasValue())
            {
                content.SetKeyWords(CurrentDatabase, new[] { ContentKeywordFilter });
            }
            var tid = EmailTemplatesModel.FetchTemplateByName("Empty Template", CurrentDatabase).Id;
            if (ContentTypeCode.EmailTemplates.Contains(content.TypeID))
                content.Id = tid;

            CurrentDatabase.Contents.InsertOnSubmit(content);
            CurrentDatabase.SubmitChanges();
            ViewBag.ContentKeywords = ContentKeywordFilter ?? "";

            if (useUnlayer == true)
            {
                ViewBag.TemplateId = 0; // force using a blank empty template with only "click here to edit content"
                return View("UnLayerCompose", content);
            }

            return RedirectEdit(content);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveUnlayerTemplate(int? saveid, string name, int roleid, string title, string body, string unlayerDesign)
        {
            Content content = null;

            if (saveid.HasValue && saveid > 0)
            {
                content = CurrentDatabase.ContentFromID(saveid.Value);
            }

            if (content == null)
            {
                content = new Content
                {
                    Name = name.HasValue()
                        ? name
                        : "new template",
                    TypeID = ContentTypeCode.TypeUnlayerTemplate,
                    RoleID = roleid,
                    OwnerID = CurrentDatabase.UserId
                };
            }

            content.Title = title;
            content.RoleID = roleid;
            content.Name = name.HasValue() ? name : content.Name;

            var bodytemplate = new { design = unlayerDesign, rawHtml = GetBody(body) };
            content.Body = JsonConvert.SerializeObject(bodytemplate);
            
            content.DateCreated = DateTime.Now;

            if (!saveid.HasValue || saveid == 0)
            {
                CurrentDatabase.Contents.InsertOnSubmit(content);
            }

            CurrentDatabase.SubmitChanges();

            ViewBag.templateID = content.Id;

            var templatedraft = content.TypeID == ContentTypeCode.TypeUnlayerTemplate ? "emailTemplates" : "savedDrafts";
            return Redirect($"/Display/#tab_{templatedraft}");
        }
        private string GetBody(string body)
        {
            if (body == null)
            {
                body = "";
            }

            body = body.RemoveGrammarly();
            var doc = new HtmlDocument();
            doc.LoadHtml(body);
            var ele = doc.DocumentNode.SelectSingleNode("/div[@bvedit='discardthis']");
            if (ele != null)
            {
                body = ele.InnerHtml;
            }

            return body;
        }

        [HttpPost]
        public ActionResult ContentUpdate(int id, string name, string title, string body, bool? snippet, int? roleid, string contentKeyWords, string stayaftersave = null)
        {
            var content = CurrentDatabase.Contents.SingleOrDefault(c => c.Id == id);
            content.Name = name;
            content.Title = string.IsNullOrWhiteSpace(title) ? name : title;
            content.Body = body?.Trim();
            content.RemoveGrammarly();
            content.RoleID = roleid ?? 0;
            content.Snippet = snippet;
            content.SetKeyWords(CurrentDatabase, contentKeyWords.SplitStr(",").Select(vv => vv.Trim()).ToArray());

            if (content.TypeID == ContentTypeCode.TypeEmailTemplate)
            {
                try
                {
                    var captureWebPageBytes = CaptureWebPageBytes(body, 100, 150);
                    var ii = CurrentImageDatabase.UpdateImageFromBits(content.ThumbID, captureWebPageBytes);
                    if (ii == null)
                    {
                        content.ThumbID = ImageData.Image.NewImageFromBits(captureWebPageBytes, CurrentImageDatabase).Id;
                    }

                    content.DateCreated = DateTime.Now;
                    content.CreatedBy = Util.UserName;
                }
                catch (Exception ex)
                {
                    var errorLog = ErrorLog.GetDefault(null);
                    errorLog.Log(new Error(ex));
                }
            }

            CurrentDatabase.SubmitChanges();

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
                        return Content(Util.EndShowMessage(ex.Message, "javascript: history.go(-1)", "There is invalid XML in this document. Go Back to Repair"));
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
                        return Content(Util.EndShowMessage(ex.InnerException?.Message, "javascript: history.go(-1)", "There is invalid XML in this document. Go Back to Repair"));
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
                        return Content(Util.EndShowMessage(ex.InnerException.Message, "javascript: history.go(-1)", "There is invalid XML in this document. Go Back to Repair"));
                    }
                }
            }

            if (stayaftersave == "true")
            {
                return RedirectToAction("ContentEdit", new { id, snippet });
            }

            var url = GetIndexTabUrl(content);
            return Redirect(url);
        }

        public ActionResult ContentDelete(int id)
        {
            var content = CurrentDatabase.ContentFromID(id);
            CurrentDatabase.ExecuteCommand("DELETE FROM dbo.ContentKeywords WHERE Id = {0}", id);
            CurrentDatabase.ExecuteCommand("DELETE FROM dbo.Content WHERE Id = {0}", id);
            var url = GetIndexTabUrl(content);
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

        //        [HttpPost]
        //        public ActionResult RunSqlScript(string body, string parameter)
        //        {
        //            var cn = new SqlConnection(Util.ConnectionStringReadOnly);
        //            cn.Open();
        //            var script = $"DECLARE @p1 VARCHAR(100) = '{parameter}'\n{body}\n";
        //            var cmd = new SqlCommand(script);
        //            var rd = cmd.ExecuteReader();
        //            return new GridResult(rd);
        //        }
        [HttpPost]
        public ActionResult SavePythonScript(string name, string body, string contentKeyWords)
        {
            var content = CurrentDatabase.Content(name, "", ContentTypeCode.TypePythonScript);
            content.Body = body;
            CurrentDatabase.SubmitChanges();
            return new EmptyResult();
        }

        public static byte[] CaptureWebPageBytes(string body, int width, int height)
        {
            bool bDone = false;
            byte[] data = null;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;

            //sta thread to allow intiate WebBrowser
            var staThread = new Thread(delegate ()
            {
                data = CaptureWebPageBytesP(body, width, height);
                bDone = true;
            });

            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();

            while (!bDone)
            {
                endDate = DateTime.Now;
                TimeSpan tsp = endDate.Subtract(startDate);

                System.Windows.Forms.Application.DoEvents();
                if (tsp.Seconds > 50)
                {
                    break;
                }
            }
            staThread.Abort();
            return data;
        }

        private static byte[] CaptureWebPageBytesP(string body, int width, int height)
        {
            byte[] data;

            using (WebBrowser web = new WebBrowser())
            {
                web.ScrollBarsEnabled = false; // no scrollbars
                web.ScriptErrorsSuppressed = true; // no errors

                web.DocumentText = body;
                while (web.ReadyState != System.Windows.Forms.WebBrowserReadyState.Complete)
                {
                    System.Windows.Forms.Application.DoEvents();
                }

                web.Width = web.Document.Body.ScrollRectangle.Width;
                web.Height = web.Document.Body.ScrollRectangle.Height;

                // a bitmap that we will draw to
                using (Bitmap bmp = new Bitmap(web.Width, web.Height))
                {
                    // draw the web browser to the bitmap
                    web.DrawToBitmap(bmp, new Rectangle(web.Location.X, web.Location.Y, web.Width, web.Height));
                    // draw the web browser to the bitmap

                    GraphicsUnit units = GraphicsUnit.Pixel;
                    RectangleF destRect = new RectangleF(0F, 0F, width, height);
                    RectangleF srcRect = new RectangleF(0, 0, web.Width, web.Width * 1.5F);

                    Bitmap b = new Bitmap(width, height);
                    using (Graphics g = Graphics.FromImage(b))
                    {
                        g.Clear(Color.White);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(bmp, destRect, srcRect, units);
                    }

                    using (MemoryStream stream = new MemoryStream())
                    {
                        EncoderParameter qualityParam = null;
                        EncoderParameters encoderParams = null;
                        ImageCodecInfo imageCodec = null;
                        imageCodec = GetEncoderInfo("image/jpeg");

                        qualityParam = new EncoderParameter(Encoder.Quality, 100L);

                        encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = qualityParam;
                        b.Save(stream, imageCodec, encoderParams);
                        
                        b.Save(stream, ImageFormat.Jpeg);
                        stream.Position = 0;
                        data = new byte[stream.Length];
                        stream.Read(data, 0, (int)stream.Length);
                    }
                }
            }
            return data;
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                {
                    return encoders[j];
                }
            }
            return null;
        }

        private string GetIndexTabUrl(Content content)
        {
            var url = Url.Action("Index");
            switch (content.TypeID)
            {
                case ContentTypeCode.TypeHtml:
                    url += "#tab_htmlContent";
                    break;
                case ContentTypeCode.TypeText:
                    url += "#tab_textContent";
                    break;
                case ContentTypeCode.TypeSqlScript:
                    url += "#tab_sqlScripts";
                    break;
                case ContentTypeCode.TypePythonScript:
                    url += "#tab_pythonScripts";
                    break;
                case ContentTypeCode.TypeEmailTemplate:
                    url += "#tab_emailTemplates";
                    break;
                case ContentTypeCode.TypeSavedDraft:
                    url += "#tab_savedDrafts";
                    break;
            }

            return url;
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
    }
    public class GridResult : ActionResult
    {
        private readonly IDataReader rd;
        public GridResult(IDataReader rd)
        {
            this.rd = rd;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var t = PythonModel.HtmlTable(rd);
            t.RenderControl(new HtmlTextWriter(context.HttpContext.Response.Output));
        }
        public static string Table(IDataReader rd, string title = null, int? maxrows = null, string excellink = null)
        {
            var t = PythonModel.HtmlTable(rd, title, maxrows);
            var sb = new StringBuilder();

            if (excellink.HasValue())
            {
                var tc = new TableCell()
                {
                    ColumnSpan = rd.FieldCount,
                    Text = excellink,
                };
                var tr = new TableFooterRow();
                tr.Cells.Add(tc);
                t.Rows.Add(tr);
            }
            t.RenderControl(new HtmlTextWriter(new StringWriter(sb)));
            return sb.ToString();
        }
    }
}
