using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CmsData;
using CmsData.Codes;
using Elmah;
using ImageData;
using Newtonsoft.Json;
using UtilityExtensions;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace CmsWeb.Areas.Manage.Models
{
    public class DisplayModel
    {
        public CMSDataContext CurrentDatabase { get; set; }
        public CMSImageDataContext CurrentImageDatabase { get; set; }
        public DisplayModel(CMSDataContext db, CMSImageDataContext idb)
        {
            CurrentDatabase = db;
            CurrentImageDatabase = idb;
        }
        public Content Clone(Content existing)
        {
            var content = new Content()
            {
                Name = existing.Name + " Copy",
                Title = existing.Title,
                Body = existing.Body,
                TypeID = existing.TypeID,
                RoleID = existing.RoleID,
            };
            return content;
        }
        public Content SaveUnlayerTemplateCommon(int? saveid, string name, int roleid, string title, string body,
            string unlayerDesign)
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
            content.DateCreated = DateTime.Now;
            content.CreatedBy = Util.UserName;

            var bodytemplate = new {design = unlayerDesign, rawHtml = GetBody(body)};
            content.Body = JsonConvert.SerializeObject(bodytemplate);

            SaveThumbnail(body, content);

            if (!saveid.HasValue || saveid == 0)
            {
                CurrentDatabase.Contents.InsertOnSubmit(content);
            }

            CurrentDatabase.SubmitChanges();
            return content;
        }

        public void SaveThumbnail(string body, Content content)
        {
#if DEBUG
            if (!DbUtil.DatabaseExists("CMSi_" + CurrentDatabase.Host))
            {
                return;
            }
#endif
            if (ContentTypeCode.IsTemplate(content.TypeID))
            {
                try
                {
                    var captureWebPageBytes = CaptureWebPageBytes(body, 100, 150);
                    var ii = CurrentImageDatabase.UpdateImageFromBits(content.ThumbID, captureWebPageBytes);
                    if (ii == null)
                    {
                        content.ThumbID = ImageData.Image.NewImageFromBits(captureWebPageBytes, CurrentImageDatabase).Id;
                    }
                }
                catch (Exception ex)
                {
                    var errorLog = ErrorLog.GetDefault(null);
                    errorLog.Log(new Error(ex));
                }
            }
        }
        public string GetBody(string body)
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

        public Content ContentUpdateCommon(int id, string name, string title, string body, bool? snippet, int? roleid,
            string contentKeyWords)
        {
            var content = CurrentDatabase.Contents.SingleOrDefault(c => c.Id == id);
            content.Name = name;
            content.Title = string.IsNullOrWhiteSpace(title) ? name : title;
            content.Body = body?.Trim();
            content.RoleID = roleid ?? 0;
            content.Snippet = snippet;
            content.DateCreated = DateTime.Now;
            content.CreatedBy = Util.UserName;
            content.SetKeyWords(CurrentDatabase, contentKeyWords.SplitStr(",").Select(vv => vv.Trim()).ToArray());

            content.RemoveGrammarly();
            SaveThumbnail(body, content);
            CurrentDatabase.SubmitChanges();
            return content;
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

        public string GetIndexTabUrl(Content content)
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
}
