using System;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Text;
using UtilityExtensions;

namespace CmsWeb.Models
{
public static class TableUtil
    {
        public static void AddRow(this PdfPTable t, string s, Font bfont)
        {
            t.AddCell("");
            t.CompleteRow();
            var c = new PdfPCell(t.DefaultCell);
            c.Border = PdfPCell.TOP_BORDER;
            c.BorderColorTop = BaseColor.BLACK;
            c.BorderWidthTop = 2.0f;
            c.Colspan = t.NumberOfColumns;
            c.AddElement(new Paragraph(s, bfont));
            c.GrayFill = .8f;
            t.AddCell(c);
            c.MinimumHeight = t.DefaultCell.MinimumHeight;
        }
        public static void AddPlainRow(this PdfPTable t, string s, Font bfont)
        {
            var c = new PdfPCell(t.DefaultCell);
            c.Border = PdfPCell.NO_BORDER;
            c.Colspan = t.NumberOfColumns;
            c.AddElement(new Phrase(s, bfont));
            t.AddCell(c);
        }
        public static void AddHeader(this PdfPTable t, string s, Font font)
        {
            var c = new PdfPCell(new Paragraph(s, font));
            c.Border = PdfPCell.BOTTOM_BORDER;
            c.BackgroundColor = t.DefaultCell.BackgroundColor;
            c.MinimumHeight = t.DefaultCell.MinimumHeight;
            c.SetLeading(t.DefaultCell.Leading, 1f);
            t.AddCell(c);
        }
        public static void Add(this PdfPTable t, string s, Font font)
        {
            var c = new PdfPCell(new Paragraph(s, font));
            c.Border = t.DefaultCell.Border;
            c.MinimumHeight = t.DefaultCell.MinimumHeight;
            c.BackgroundColor = t.DefaultCell.BackgroundColor;
            c.SetLeading(t.DefaultCell.Leading, 1f);
            t.AddCell(c);
        }
        public static void AddRight(this PdfPTable t, string s, Font font)
        {
            var c = new PdfPCell(new Paragraph(s, font));
            c.Border = t.DefaultCell.Border;
            c.BackgroundColor = t.DefaultCell.BackgroundColor;
            c.SetLeading(t.DefaultCell.Leading, 1f);
            c.HorizontalAlignment = Element.ALIGN_RIGHT;
            t.AddCell(c);
        }
        public static void AddLeft(this PdfPTable t, string s, int colspan, Font font)
        {
            var c = new PdfPCell(new Paragraph(s, font));
            c.Border = t.DefaultCell.Border;
            c.BackgroundColor = t.DefaultCell.BackgroundColor;
            c.SetLeading(t.DefaultCell.Leading, 1f);
            c.Colspan = colspan;
            c.HorizontalAlignment = Element.ALIGN_LEFT;
            t.AddCell(c);
        }
        public static void AddRight(this PdfPTable t, string s, int colspan, Font font)
        {
            var c = new PdfPCell(new Paragraph(s, font));
            c.Border = t.DefaultCell.Border;
            c.BackgroundColor = t.DefaultCell.BackgroundColor;
            c.SetLeading(t.DefaultCell.Leading, 1f);
            c.Colspan = colspan;
            c.HorizontalAlignment = Element.ALIGN_RIGHT;
            t.AddCell(c);
        }
        public static void AddCentered(this PdfPTable t, string s, int colspan, Font font)
        {
            var c = new PdfPCell(new Paragraph(s, font));
            c.Border = t.DefaultCell.Border;
            c.BackgroundColor = t.DefaultCell.BackgroundColor;
            c.SetLeading(t.DefaultCell.Leading, 1f);
            c.Colspan = colspan;
            c.HorizontalAlignment = Element.ALIGN_CENTER;
            t.AddCell(c);
        }
        public static void Add(this PdfPTable t, string s, int colspan, Font font)
        {
            var c = new PdfPCell(new Paragraph(s, font));
            c.Border = t.DefaultCell.Border;
            c.BackgroundColor = t.DefaultCell.BackgroundColor;
            c.SetLeading(t.DefaultCell.Leading, 1f);
            c.Colspan = colspan;
            c.AddElement(new Paragraph(s, font));
            t.AddCell(c);
        }
        public static void SetNoBorder(this PdfPTable t)
        {
            t.DefaultCell.SetLeading(2.0f, 1f);
            t.DefaultCell.Border = PdfPCell.NO_BORDER;
            t.WidthPercentage = 100f;
        }
        public static void SetNoPadding(this PdfPTable t)
        {
            t.DefaultCell.SetLeading(2.0f, 1f);
            t.DefaultCell.Border = PdfPCell.NO_BORDER;
            t.WidthPercentage = 100f;
            t.DefaultCell.Padding = 0;
        }
        public static void AddLine(this Paragraph p, string s, Font font)
        {
            if (s.HasValue())
            {
                if (p.Content.Length > 0)
                    p.Add("\n");
                p.Add(new Chunk(s, font));
            }
        }
    }
}