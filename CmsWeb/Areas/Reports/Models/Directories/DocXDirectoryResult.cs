/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using Novacode;
using UtilityExtensions;
using Paragraph = Novacode.Paragraph;

namespace CmsWeb.Areas.Reports.Models
{
    public class DocXDirectoryResult : ActionResult
    {
        private readonly Guid id;
        private EmailReplacements replacements;
        private DocX docx;
        private readonly string template;
        private readonly string filename;

        public DocXDirectoryResult(Guid id, string filename, string template)
        {
            this.template = template;
            this.filename = filename;
            this.id = id;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;

            var list = ExcelExportModel.DirectoryList(id);

            if (list.Count == 0)
            {
                response.Write("no data found");
                return;
            }
            var bytes = DirectoryTemplate();
            var ms = new MemoryStream(bytes);
            docx = DocX.Load(ms);

            var tbl = docx.Tables[0];
            var c = 0;

            var templaterow = tbl.Rows[0];
            var ncols = templaterow.ColumnCount;
            Row row = null;

            // remove extra rows
            for (var i = tbl.RowCount - 1; i > 0; i--)
                tbl.RemoveRow(i);

            replacements = new EmailReplacements(DbUtil.Db, docx);

            foreach (var m in list)
            {
                if (c == 0) // start a new row
                {
                    row = tbl.InsertRow(templaterow);
                    row.BreakAcrossPages = false;
                    tbl.Rows.Add(row);
                }

                Debug.Assert(row != null, "row != null");
                var cell = row.Cells[c];
                DoCellReplacements(cell, m);
                if (++c == ncols)
                    c = 0; // start a new row
            }
            while(c > 0 && c < ncols && row != null)
                DoEmptyCell(row.Cells[c++]);

            tbl.RemoveRow(0); // the first row was used as a template

            response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            response.AddHeader("content-disposition", $"attachment;filename={filename}-{DateTime.Now.ToSortableDateTime()}.docx");
            response.AddHeader("content-length", ms.Length.ToString());
            docx.SaveAs(response.OutputStream);
        }

        private readonly Regex picre = new Regex(@"{(?<pic>pic|fampic) +(?<width>[\.\d]*) +(?<height>[\.\d]*)}");
        private readonly Regex spousere = new Regex(@"{spouse(:(?<text>[^}]*)){0,1}}");
        private readonly Regex emailre = new Regex(@"{email(:(?<text>[^}]*)){0,1}}");
        private readonly Regex phonere = new Regex(@"{phone(:(?<text>[^}]*)){0,1}}");
        private readonly Regex cellre = new Regex(@"{cell(:(?<text>[^}]*)){0,1}}");
        private readonly Regex kidsre = new Regex(@"{kids(:(?<text>[^}]*)){0,1}}");
        private readonly Regex bdayre = new Regex(@"{bday(:(?<text>[^}]*)){0,1}}");
        private readonly Regex anniversaryre = new Regex(@"{anniversary(:(?<text>[^}]*)){0,1}}");
        private readonly Regex dtfmtre = new Regex("_(?<fmt>[^_]*)_");

        private void DoCellReplacements(Container cell, DirectoryInfo di)
        {
            var dict = replacements.DocXReplacementsDictionary(di.Person);
            foreach (var pg in cell.Paragraphs.Where(vv => vv.Text.HasValue()))
                if (dict.Keys.Any(vv => pg.Text.Contains(vv)))
                    foreach (var d in dict)
                        if (pg.Text.Contains(d.Key))
                            if (d.Key.Equal("{altname}"))
                                pg.ReplaceText(d.Key, di.Person.AltName);
                            else if (d.Key.Equal("{name}"))
                                pg.ReplaceText(d.Key, di.Person.Name);
                            else if (d.Key.Equal("{familyname}"))
                                pg.ReplaceText(d.Key, di.FamilyName);
                            else if (d.Key.Equal("{addr}"))
                                pg.ReplaceText(d.Key, di.Address);
                            else if (bdayre.IsMatch(d.Key))
                            {
                                if (!di.Person.BirthDate.HasValue)
                                {
                                    pg.ReplaceText(d.Key, "");
                                    continue;
                                }
                                var m = bdayre.Match(d.Key);
                                var txt = m.Groups["text"].Value;
                                m = dtfmtre.Match(txt);
                                var repl = m.Value;
                                var mfmt = m.Groups["fmt"].Value;
                                pg.ReplaceText(d.Key, txt.HasValue()
                                    ? txt.Replace(repl, di.Person.BirthDate.ToString2(Util.PickFirst(mfmt, "MMM d")))
                                    : di.Person.BirthDate.ToString2("MMM d"));
                            }
                            else if (anniversaryre.IsMatch(d.Key))
                            {
                                if (!di.Person.WeddingDate.HasValue)
                                {
                                    pg.ReplaceText(d.Key, "");
                                    continue;
                                }
                                var m = anniversaryre.Match(d.Key);
                                var txt = m.Groups["text"].Value;
                                m = dtfmtre.Match(txt);
                                var repl = m.Value;
                                var mfmt = m.Groups["fmt"].Value;
                                pg.ReplaceText(d.Key, txt.HasValue()
                                    ? txt.Replace(repl, di.Person.WeddingDate.ToString2(Util.PickFirst(mfmt, "MMM d")))
                                    : di.Person.WeddingDate.ToString2("MMM d"));
                            }
                            else if (emailre.IsMatch(d.Key))
                            {
                                if (!di.Person.EmailAddress.HasValue())
                                {
                                    pg.ReplaceText(d.Key, "");
                                    continue;
                                }
                                var m = emailre.Match(d.Key);
                                var txt = m.Groups["text"].Value;
                                pg.ReplaceText(d.Key, txt.HasValue()
                                        ? txt.Replace("_addr_", di.Person.EmailAddress)
                                        : di.Person.EmailAddress);
                            }
                            else if (phonere.IsMatch(d.Key))
                            {
                                if (!di.HomePhone.HasValue())
                                {
                                    pg.ReplaceText(d.Key, "");
                                    continue;
                                }
                                var m = phonere.Match(d.Key);
                                var txt = m.Groups["text"].Value;
                                pg.ReplaceText(d.Key, txt.HasValue()
                                        ? txt.Replace("_number_", di.HomePhone)
                                        : di.HomePhone);
                            }
                            else if (cellre.IsMatch(d.Key))
                            {
                                if (!di.CellPhone.HasValue())
                                {
                                    pg.ReplaceText(d.Key, "");
                                    continue;
                                }
                                var m = cellre.Match(d.Key);
                                var txt = m.Groups["text"].Value;
                                pg.ReplaceText(d.Key, txt.HasValue()
                                        ? txt.Replace("_number_", di.CellPhone)
                                        : di.CellPhone);
                            }
                            else if (spousere.IsMatch(d.Key))
                            {
                                if (!di.SpouseName.HasValue())
                                {
                                    pg.ReplaceText(d.Key, "");
                                    continue;
                                }
                                var m = spousere.Match(d.Key);
                                var txt = m.Groups["text"].Value;
                                pg.ReplaceText(d.Key, txt.HasValue()
                                        ? txt.Replace("_name_", di.SpouseName)
                                        : di.SpouseName);
                            }
                            else if (kidsre.IsMatch(d.Key))
                            {
                                if (!di.Children.HasValue())
                                {
                                    pg.ReplaceText(d.Key, "");
                                    continue;
                                }
                                var m = kidsre.Match(d.Key);
                                var txt = m.Groups["text"].Value;
                                pg.ReplaceText(d.Key, txt.HasValue()
                                        ? txt.Replace("_names_", di.Children)
                                        : di.Children);
                            }
                            else if (picre.IsMatch(d.Key))
                            {
                                var m = picre.Match(d.Key);
                                var pic = m.Groups["pic"].Value;
                                var height = m.Groups["height"].Value.ToDouble();
                                var width = m.Groups["width"].Value.ToDouble();
                                pg.ReplaceText(d.Key, "");
                                AddPicture(pg, width, height, pic == "pic" ? di.ImageId : di.FamImageId);
                            }
                            else
                                pg.ReplaceText(d.Key, d.Value);
        }
        private void DoEmptyCell(Container cell)
        {
            var dict = replacements.DocXReplacementsDictionary(null);
            foreach (var pg in cell.Paragraphs.Where(vv => vv.Text.HasValue()))
                if (dict.Keys.Any(vv => pg.Text.Contains(vv)))
                    foreach (var d in dict)
                        if (pg.Text.Contains(d.Key))
                            pg.ReplaceText(d.Key, "");
        }


        private void AddPicture(Paragraph pg, double width, double height, int? imageid)
        {
            const int oversizedpixelsinch = 310;
            const int pixelsinch = 96;
            var largewidth = (oversizedpixelsinch * width).ToInt();
            var largeheight = (oversizedpixelsinch * height).ToInt();
            var widthpixels = (width * pixelsinch).ToInt();
            var heightpixels = (height * pixelsinch).ToInt();

            var img = ImageData.Image.ImageFromId(imageid);
            if (img != null)
                using (var os = img.ResizeToStream(largewidth, largeheight, "pad"))
                {
                    var pic = docx.AddImage(os).CreatePicture(heightpixels, widthpixels);
                    pg.AppendPicture(pic);
                }
        }

        private byte[] DirectoryTemplate()
        {
            var loc = template;
            var wc = new WebClient();
            return wc.DownloadData(loc);
        }
    }
}