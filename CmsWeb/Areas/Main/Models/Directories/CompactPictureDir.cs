/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CmsWeb.Models;
using Novacode;
using NPOI.SS.Formula.Functions;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class CompactPictureDir : ActionResult
    {
        public Guid id;

        public CompactPictureDir(Guid id)
        {
            this.id = id;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;

            var q = ExcelExportModel.List(id);

            if (!q.Any())
            {
                Response.Write("no data found");
                return;
            }
            using (var ms = new MemoryStream())
            {
                var dd = DocX.Create("ttt.docx");
                dd.PageHeight = Pixels(11f);
                dd.PageWidth = Pixels(8.5f);
                dd.MarginLeft = Pixels(.5f);
                dd.MarginRight = Pixels(.5f);
                dd.MarginTop = Pixels(.5f);
                dd.MarginBottom = Pixels(.5f);
                var col = 0;
                var row = 0;
                var tt = dd.InsertTable(q.Count / 3 + (q.Count % 3 > 0 ? 1 : 0), 6);
                tt.AutoFit = AutoFit.ColumnWidth;
                for (var r = 0; r < tt.RowCount; r++)
                {
                    var rr = tt.Rows[r];
                    rr.Height = 120;
                    for (var i = 0; i < 6; i++)
                    {
                        var c = rr.Cells[i];
                        c.MarginRight = 0;
                        if (i%2 == 0)
                        {
                            c.Width = Pixels(.67f);
                            c.MarginLeft = 0;
                        }
                        else
                        {
                            c.Width = Pixels(1f);
                            c.Width = Pixels(1.83f);
                            c.MarginLeft = Pixels(.04f);
                        }
                    }
                }
                foreach (var p in q)
                {
                    col = FillCell(tt.Rows[row], col, p, dd);
                    if (col < 6) 
                        continue;
                    row++;
                    col = 0;
                }
                dd.SaveAs(ms);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                Response.AddHeader("content-disposition", "filename=picturedir.docx");
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
        }


        private static int FillCell(Novacode.Row rr, int col, ExcelPic p, DocX dd)
        {
            var c = rr.Cells[col];
            var img = ImageData.Image.ImageFromId(p.ImageId);
            if (img != null)
                using (var os = img.ResizeToStream(200, 300, "crop"))
                {
                    var pic = dd.AddImage(os).CreatePicture(93, 62);
                    c.Paragraphs[0].InsertPicture(pic);
                }
            col++;
            c = rr.Cells[col];
            c.Paragraphs.RemoveAt(0);

            c.Paragraphs[0].InsertText("{0}, {1}".Fmt(p.LastName, p.FirstName));
            c.Paragraphs[0].FontSize(12).Bold();
            c.InsertParagraph(p.Email).FontSize(9);
            if (p.BirthDate.HasValue())
                c.InsertParagraph("BD {0}".Fmt(p.BirthDay)).FontSize(10);
            if (p.Spouse.HasValue())
                c.InsertParagraph("Spouse: {0}".Fmt(p.Spouse)).FontSize(10);
            if (p.Children.HasValue())
                c.InsertParagraph("Kids: {0}".Fmt(p.Children)).FontSize(10);
            col++;
            return col;
        }

        private static float Pixels(float inches)
        {
            return inches * 1440 / 15;
        }
    }
}