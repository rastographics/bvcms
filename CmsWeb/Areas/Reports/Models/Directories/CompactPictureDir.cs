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
using UtilityExtensions;
using Newtonsoft.Json;
using Formatting = Novacode.Formatting;

namespace CmsWeb.Areas.Reports.Models
{
    public class CompactPictureDir : ActionResult
    {
        private readonly Guid id;
        private readonly DocX dd;
        private readonly Parameters pa;

        public class Parameters
        {
            public string Comment { get; set; }
            public double PageHeight { get; set; }
            public double PageWidth { get; set; }
            public double RowHeight { get; set; }
            public double SpacerWidth { get; set; }
            public double LabelWidth { get; set; }
            public double PicWidth { get; set; }
            public double FontSize { get; set; }
            public double FontSizeName { get; set; }
            public double FontSizeEmail { get; set; }
            public double MarginLeft { get; set; }
            public double MarginRight { get; set; }
            public double MarginTop { get; set; }
            public double MarginBottom { get; set; }
            public int PicWidthPixels { get; set; }

            public Formatting namebold => new Formatting {Size = FontSizeName, Bold = true};
            public Formatting emailsmall => new Formatting {Size = FontSizeEmail};
            public Formatting font => new Formatting {Size = FontSize};
        }
        public CompactPictureDir(Guid id, string parameters)
        {
            this.id = id;
            dd = DocX.Create("ttt.docx");

            pa = JsonConvert.DeserializeObject<Parameters>(parameters);

//            dd.PageHeight = Pixels(pa.PageHeight);
//            dd.PageWidth = Pixels(pa.PageWidth);
            dd.MarginLeft = Pixels(pa.MarginLeft);
            dd.MarginRight = Pixels(pa.MarginRight);
            dd.MarginTop = Pixels(pa.MarginTop);
            dd.MarginBottom = Pixels(pa.MarginBottom);
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
                var col = 0;
                var row = 0;
                var tt = dd.InsertTable(q.Count / 3 + (q.Count % 3 > 0 ? 1 : 0), 6);
                tt.AutoFit = AutoFit.ColumnWidth;
                for (var r = 0; r < tt.RowCount; r++)
                {
                    var rr = tt.Rows[r];
                    rr.Height = Pixels(pa.RowHeight);
                    for (var i = 0; i < 6; i++)
                    {
                        var c = rr.Cells[i];
                        c.MarginRight = 0;
                        if (i % 2 == 0)
                        {
                            c.Width = Pixels(pa.PicWidth);
                            c.MarginLeft = 0;
                        }
                        else
                        {
                            c.Width = Pixels(pa.LabelWidth - pa.PicWidth);
                            c.MarginLeft = Pixels(pa.SpacerWidth);
                        }
                    }
                }
                foreach (var p in q)
                {
                    col = FillCell(tt.Rows[row], col, p);
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


        private int FillCell(Novacode.Row rr, int col, ExcelPic p)
        {
            var c = rr.Cells[col];
            var img = ImageData.Image.ImageFromId(p.ImageId);
            if (img != null)
                using (var os = img.ResizeToStream(200, 300, "max"))
                {
                    var pic = dd.AddImage(os).CreatePicture((pa.PicWidthPixels * 1.5).ToInt(), pa.PicWidthPixels);
                    c.Paragraphs[0].InsertPicture(pic);
                }
            col++;
            c = rr.Cells[col];
            //c.RemoveParagraphAt(0);

            c.Paragraphs[0].InsertText($"{p.LastName}, {p.FirstName}", false, pa.namebold);

            c.InsertParagraph(p.Email, false, pa.emailsmall);

            if (p.BirthDate.HasValue())
                c.InsertParagraph($"BD {p.BirthDay}", false, pa.font);
            if (p.Spouse.HasValue())
                c.InsertParagraph($"Spouse: {p.Spouse}", false, pa.font);
            if (p.Children.HasValue())
                c.InsertParagraph($"Kids: {p.Children}", false, pa.font);
            col++;
            return col;
        }

        private static float Pixels(double inches)
        {
            return Convert.ToSingle(inches * 1440 / 15);
        }
    }
}
