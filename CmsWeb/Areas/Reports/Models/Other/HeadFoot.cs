using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text.pdf;
using iTextSharp.text;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
        public class HeadFoot : PdfPageEventHelper
        {
            private PdfTemplate tpl;
            private PdfContentByte dc;
            private BaseFont font;
            public string HeaderText { get; set; }
            public string FooterText { get; set; }

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                base.OnOpenDocument(writer, document);
                font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                dc = writer.DirectContent;
                tpl = dc.CreateTemplate(50, 50);
            }
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);
                float fLen;
                string sText = null;

                //---Header left
                sText = HeaderText;
                const float HeadFontSize = 11f;
                fLen = font.GetWidthPoint(sText, HeadFontSize);
                dc.BeginText();
                dc.SetFontAndSize(font, HeadFontSize);
                dc.SetTextMatrix(30, document.PageSize.Height - 30);
                dc.ShowText(sText);
                dc.EndText();

                // Footer
                //---Column 1: Title
                if (FooterText.HasValue())
                {
                    sText = FooterText;
                    fLen = font.GetWidthPoint(sText, 8);
                    dc.BeginText();
                    dc.SetFontAndSize(font, 8);
                    dc.SetTextMatrix(30, 30);
                    dc.ShowText(sText);
                    dc.EndText();
                }

                //---Column 2: Date/Time
                sText = DateTime.Now.ToShortDateString();
                fLen = font.GetWidthPoint(sText, 8);
                dc.BeginText();
                dc.SetFontAndSize(font, 8);
                dc.SetTextMatrix(document.PageSize.Width / 2 - fLen / 2, 30);
                dc.ShowText(sText);
                dc.EndText();

                //---Column 3: Page Number
                sText = $"Page {writer.PageNumber} of ";
                fLen = font.GetWidthPoint(sText, 8);
                dc.BeginText();
                dc.SetFontAndSize(font, 8);
                dc.SetTextMatrix(document.PageSize.Width - 90, 30);
                dc.ShowText(sText);
                dc.EndText();
                dc.AddTemplate(tpl, document.PageSize.Width - 90 + fLen, 30);
            }
            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                tpl.BeginText();
                tpl.SetFontAndSize(font, 8);
                tpl.ShowText(writer.PageNumber.ToString());
                tpl.EndText();
                base.OnCloseDocument(writer, document);
            }
        }
}
