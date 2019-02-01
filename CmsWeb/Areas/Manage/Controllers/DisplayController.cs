/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */
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
using UtilityExtensions;
using Content = CmsData.Content;
using Encoder = System.Drawing.Imaging.Encoder;
using Image = System.Drawing.Image;

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
            return View(new ContentModel());
        }

        [HttpPost]
        public ActionResult SetContentKeywordFilter(string keywordfilter, string tab)
        {
            if (keywordfilter.HasValue())
            {
                Session["ContentKeywordFilter"] = keywordfilter.Trim();
            }
            else
            {
                Session.Remove("ContentKeywordFilter");
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
            return RedirectEdit(i.content);
        }
        public ActionResult Content(int? id)
        {
            if (!id.HasValue)
            {
                throw new HttpException(404, "No ID found.");
            }

            var content = DbUtil.ContentFromID(id.Value);
            if (content == null)
            {
                throw new HttpException(404, "No ID found.");
            }

            return Content(content.Body);
        }

        [HttpPost]
        public ActionResult ContentCreate(int newType, string newName, int? newRole)
        {
            var content = new Content();
            content.Name = newName;
            content.TypeID = newType;
            content.RoleID = newRole ?? 0;
            content.Title = newName;
            content.Body = "";
            content.DateCreated = DateTime.Now;
            var ContentKeywordFilter = Session["ContentKeywordFilter"] as string;
            if (ContentKeywordFilter.HasValue())
            {
                content.SetKeyWords(CurrentDatabase, new[] { ContentKeywordFilter });
            }

            CurrentDatabase.Contents.InsertOnSubmit(content);
            CurrentDatabase.SubmitChanges();
            ViewBag.ContentKeywords = ContentKeywordFilter ?? "";

            return RedirectEdit(content);
        }

        [HttpPost]
        public ActionResult ContentUpdate(int id, string name, string title, string body, bool? snippet, int? roleid, string contentKeyWords, string stayaftersave = null)
        {
            var content = DbUtil.ContentFromID(id);
            content.Name = name;
            content.Title = string.IsNullOrWhiteSpace(title) ? name : title;
            content.Body = body;
            content.RemoveGrammarly();
            content.RoleID = roleid ?? 0;
            content.Snippet = snippet;
            content.SetKeyWords(CurrentDatabase, contentKeyWords.SplitStr(",").Select(vv => vv.Trim()).ToArray());

            if (content.TypeID == ContentTypeCode.TypeEmailTemplate)
            {
                try
                {
                    var captureWebPageBytes = CaptureWebPageBytes(body, 100, 150);
                    var ii = ImageData.Image.UpdateImageFromBits(content.ThumbID, captureWebPageBytes);
                    if (ii == null)
                    {
                        content.ThumbID = ImageData.Image.NewImageFromBits(captureWebPageBytes).Id;
                    }

                    content.DateCreated = DateTime.Now;
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
            var content = DbUtil.ContentFromID(id);
            CurrentDatabase.ExecuteCommand("DELETE FROM dbo.ContentKeywords WHERE Id = {0}", id);
            CurrentDatabase.ExecuteCommand("DELETE FROM dbo.Content WHERE Id = {0}", id);
            var url = GetIndexTabUrl(content);
            return Redirect(url);
        }

        public ActionResult ContentDeleteDrafts(string[] draftID)
        {
            string deleteList = String.Join(",", draftID);
            CurrentDatabase.ExecuteCommand("DELETE FROM dbo.Content WHERE Id IN(" + deleteList + ")", "");
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

        public ActionResult EmailBody(int id)
        {
            var content = DbUtil.ContentFromID(id);
            content.RemoveGrammarly();
            return View(content);
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
                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(web.Width, web.Height))
                {
                    // draw the web browser to the bitmap
                    web.DrawToBitmap(bmp, new Rectangle(web.Location.X, web.Location.Y, web.Width, web.Height));
                    // draw the web browser to the bitmap

                    GraphicsUnit units = GraphicsUnit.Pixel;
                    RectangleF destRect = new RectangleF(0F, 0F, width, height);
                    RectangleF srcRect = new RectangleF(0, 0, web.Width, web.Width * 1.5F);

                    Bitmap b = new Bitmap(width, height);
                    Graphics g = Graphics.FromImage((Image)b);
                    g.Clear(Color.White);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    g.DrawImage(bmp, destRect, srcRect, units);
                    g.Dispose();

                    using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                    {
                        EncoderParameter qualityParam = null;
                        EncoderParameters encoderParams = null;
                        try
                        {
                            ImageCodecInfo imageCodec = null;
                            imageCodec = GetEncoderInfo("image/jpeg");

                            qualityParam = new EncoderParameter(Encoder.Quality, 100L);

                            encoderParams = new EncoderParameters(1);
                            encoderParams.Param[0] = qualityParam;
                            b.Save(stream, imageCodec, encoderParams);
                        }
                        catch (Exception)
                        {
                            throw new Exception();
                        }
                        finally
                        {
                            if (encoderParams != null)
                            {
                                encoderParams.Dispose();
                            }

                            if (qualityParam != null)
                            {
                                qualityParam.Dispose();
                            }
                        }
                        b.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
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
