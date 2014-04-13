/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using UtilityExtensions;
using CmsWeb.Models;
using System.Data.SqlClient;
// Used for pulling image from service
using System.Net;
// Used for HTML Image Capture
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using CmsWeb.Code;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin,Design")]
    [ValidateInput(false)]
    [RouteArea("Manage", AreaPrefix= "Display"), Route("{action}/{id?}")]
    public class DisplayController : CmsStaffController
    {
        [Route("~/Display")]
        public ActionResult Index()
        {
            return View(new ContentModel());
        }

        public ActionResult ContentView(int id)
        {
            var content = DbUtil.ContentFromID(id);
            return View(content);
        }

        public ActionResult ContentEdit(int id)
        {
            var content = DbUtil.ContentFromID(id);
            return RedirectEdit(content);
        }

        [HttpPost]
        public ActionResult ContentCreate(int newType, string newName, int? newRole)
        {
            var content = new Content();
            content.Name = newName;
            content.TypeID = newType;
            content.RoleID = newRole ?? 0;
            content.Title = content.Body = "";
            content.DateCreated = DateTime.Now;

            DbUtil.Db.Contents.InsertOnSubmit(content);
            DbUtil.Db.SubmitChanges();

            return RedirectEdit(content);
        }

        [HttpPost]
        public ActionResult ContentUpdate(int id, string name, string title, string body, int? roleid)
        {
            var content = DbUtil.ContentFromID(id);
            content.Name = name;
            content.Title = title;
            content.Body = body;
            content.RoleID = roleid ?? 0;

            string sRenderType = DbUtil.Db.Setting("RenderEmailTemplate", "none");

            if (content.TypeID == ContentTypeCode.TypeEmailTemplate)
            {
//                switch (sRenderType)
//                {
//                    case "Local":// Uses local server resources 
//                    case "true":
                        var captureWebPageBytes = CaptureWebPageBytes(body, 100, 150);
                        var ii = ImageData.Image.UpdateImageFromBits(content.ThumbID, captureWebPageBytes);
                        if(ii == null)
                            content.ThumbID = ImageData.Image.NewImageFromBits(captureWebPageBytes).Id;
//                        break;
//
//                    case "Service": // Used to send HTML to another server for offloaded processing
//                        var coll = new NameValueCollection();
//                        coll.Add("sHTML", body.Replace("\r", "").Replace("\n", "").Replace("\t", ""));
//
//                        var wc = new WebClient();
//                        var resp = wc.UploadValues(ConfigurationManager.AppSettings["CreateThumbnailService"], "POST",
//                            coll);
//
//                        ii = ImageData.DbUtil.Db.Images.FirstOrDefault(i => i.Id == content.ThumbID);
//                        if (ii != null) 
//                            ImageData.Image.UpdateImageFromBits(content.ThumbID, resp);
//                        else 
//                            content.ThumbID = ImageData.Image.NewImageFromBits(resp).Id;
//                        break;
//                }
                content.DateCreated = DateTime.Now;
            }
            DbUtil.Db.SubmitChanges();

            if (string.Compare(content.Name, "StandardExtraValues.xml", ignoreCase: true) == 0)
            {
                try
                {
                    StandardExtraValues.GetExtraValues();
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.InnerException is System.Xml.XmlException)
                        return Content(Util.EndShowMessage(ex.InnerException.Message, "javascript: history.go(-1)", "Go Back to Repair"));
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult ContentDelete(int id)
        {
            var content = DbUtil.ContentFromID(id);
            DbUtil.Db.Contents.DeleteOnSubmit(content);
            DbUtil.Db.SubmitChanges();
            return RedirectToAction("Index", "Display");
        }

        public ActionResult ContentDeleteDrafts(string[] draftID)
        {
            string deleteList = String.Join(",", draftID);
            DbUtil.Db.ExecuteCommand("DELETE FROM dbo.Content WHERE Id IN(" + deleteList + ")", "");
            return RedirectToAction("Index", "Display");
        }

        public ActionResult RedirectEdit(Content cContent)
        {
            switch (cContent.TypeID) // 0 = HTML, 1 = Text, 2 = eMail Template
            {
                case ContentTypeCode.TypeHtml:
                    return View("EditHTML", cContent);

                case ContentTypeCode.TypeText:
                    return View("EditText", cContent);

                case ContentTypeCode.TypeEmailTemplate:
                case ContentTypeCode.TypeSavedDraft:
                    return View("EditTemplate", cContent);
            }

            return View("Index");
        }

        public ActionResult OrgContent(int id, string what, bool? div)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            if (div == true && org.Division == null)
                return Content("no main division");

            switch (what)
            {
                case "message":
                    if (div == true)
                    {
                        ViewData["html"] = org.Division.EmailMessage;
                        ViewData["title"] = org.Division.EmailSubject;
                    }
                    break;
                case "instructions":
                    if (div == true)
                        ViewData["html"] = org.Division.Instructions;
                    ViewData["title"] = "Instructions";
                    break;
                case "terms":
                    if (div == true)
                        ViewData["html"] = org.Division.Terms;
                    ViewData["title"] = "Terms";
                    break;
            }
            ViewData["id"] = id;
            return View();
        }

        [HttpPost]
        public ActionResult UpdateOrgContent(int id, bool? div, string what, string title, string html)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);

            switch (what)
            {
                case "message":
                    if (div == true)
                    {
                        org.Division.EmailMessage = html;
                        org.Division.EmailSubject = title;
                    }
                    break;
                case "instructions":
                    if (div == true)
                        org.Division.Instructions = html;
                    break;
                case "terms":
                    if (div == true)
                        org.Division.Terms = html;
                    break;
            }
            DbUtil.Db.SubmitChanges();
            return Redirect("/Organization/" + id);
        }

        public static byte[] CaptureWebPageBytes(string body, int width, int height)
        {
            bool bDone = false;
            byte[] data = null;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;

            //sta thread to allow intiate WebBrowser
            var staThread = new Thread(delegate()
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

        static byte[] CaptureWebPageBytesP(string body, int width, int height)
        {
            byte[] data;

            using (WebBrowser web = new WebBrowser())
            {
                web.ScrollBarsEnabled = false; // no scrollbars
                web.ScriptErrorsSuppressed = true; // no errors

                web.DocumentText = body;
                while (web.ReadyState != System.Windows.Forms.WebBrowserReadyState.Complete)
                    System.Windows.Forms.Application.DoEvents();

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
                            if (encoderParams != null) encoderParams.Dispose();
                            if (qualityParam != null) qualityParam.Dispose();
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
                    return encoders[j];
            }
            return null;
        }
    }
}