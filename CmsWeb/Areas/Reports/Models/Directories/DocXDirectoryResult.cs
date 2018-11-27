/* Author: David Carroll
 * Copyright (c) 2017 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using UtilityExtensions;
using Xceed.Words.NET;


namespace CmsWeb.Areas.Reports.Models
{
    public class DocXDirectoryResult : ActionResult
    {
        private readonly Guid id;
        private EmailReplacements replacements;
        private Dictionary<string, string> emptyReplacementsDictionary;
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
            var wc = new WebClient();
            var bytes = wc.DownloadData(template);

            var ms = new MemoryStream(bytes);
            docx = DocX.Load(ms);

            replacements = new EmailReplacements(DbUtil.Db, docx);
            emptyReplacementsDictionary = replacements.DocXReplacementsDictionary(null);

            if (emptyReplacementsDictionary.ContainsKey("{lastnamestartswith}"))
            {
                BuildDocumentIndexStyle(list);
            }
            else
            {
                BuildDocument(list);
            }

            response.Clear();
            response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            response.AddHeader("content-disposition", $"attachment;filename={filename}-{DateTime.Now.ToSortableDateTime()}.docx");

            ms = new MemoryStream();
            docx.SaveAs(ms);
            ms.WriteTo(response.OutputStream);
            response.End();

        }

        private void BuildDocument(List<DirectoryInfo> list)
        {
            var tbl = docx.Tables[0];
            var c = 0;

            var templaterow = tbl.Rows[0];
            var ncols = templaterow.ColumnCount;
            Row row = null;

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
                {
                    c = 0; // start a new row
                }
            }
            while (c > 0 && c < ncols && row != null)
            {
                DoEmptyCell(row.Cells[c++]);
            }

            tbl.RemoveRow(0); // the first row was used as a template
        }
        private void BuildDocumentIndexStyle(List<DirectoryInfo> list)
        {
            var templatetbl = docx.Tables[0];
            var c = 0;

            var templaterow = templatetbl.Rows[0];
            var ncols = templaterow.ColumnCount;
            Row row = null;
            Table tbl = null;
            Paragraph alphaParagraph = null;

            foreach (var pg in docx.Paragraphs)
            {
                if (pg.Text.Contains("{lastnamestartswith}"))
                {
                    alphaParagraph = pg;
                    break;
                }
            }

            var alphaindex = "@";
            foreach (var m in list)
            {
                if (m.Person.LastName[0] != alphaindex[0])
                {
                    tbl?.RemoveRow(0);
                    var pg = docx.InsertParagraph(alphaParagraph);
                    pg.KeepWithNextParagraph();
                    alphaindex = m.Person.LastName.Substring(0, 1);
                    pg.ReplaceText("{lastnamestartswith}", alphaindex);
                    tbl = docx.InsertTable(templatetbl);
                    c = 0;
                }
                Debug.Assert(tbl != null, "tbl != null");
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
                {
                    c = 0; // start a new row
                }
            }
            while (c > 0 && c < ncols && row != null)
            {
                DoEmptyCell(row.Cells[c++]);
            }

            tbl?.RemoveRow(0); // remove first row from last table worked on
            docx.RemoveParagraph(alphaParagraph); // remove the alphaParagraph template
            docx.Tables[0].Remove(); // remove the template table
            docx.Paragraphs[0].Remove(trackChanges: false); // remove the empty paragraph after the template table
        }
        private void DoCellReplacements(Container cell, DirectoryInfo di)
        {
            var dict = replacements.DocXReplacementsDictionary(di.Person);
            foreach (var pg in cell.Paragraphs.Where(vv => vv.Text.HasValue()))
            {
                if (dict.Keys.Any(vv => pg.Text.Contains(vv)))
                {
                    foreach (var d in dict)
                    {
                        if (pg.Text.Contains(d.Key))
                        {
                            if (!ReplaceCode(pg, d.Key, di))
                            {
                                pg.ReplaceText(d.Key, d.Value);
                            }
                        }
                    }
                }
            }
        }
        private void DoEmptyCell(Container cell)
        {
            foreach (var pg in cell.Paragraphs.Where(vv => vv.Text.HasValue()))
            {
                if (emptyReplacementsDictionary.Keys.Any(vv => pg.Text.Contains(vv)))
                {
                    foreach (var d in emptyReplacementsDictionary)
                    {
                        if (pg.Text.Contains(d.Key))
                        {
                            pg.ReplaceText(d.Key, "");
                        }
                    }
                }
            }
        }

        private bool ReplaceCode(Paragraph pg, string code, DirectoryInfo di)
        {
            if (code.Equal("{altname}"))
            {
                pg.ReplaceText(code, di.Person.AltName);
            }
            else if (code.Equal("{name}"))
            {
                pg.ReplaceText(code, di.Person.Name);
            }
            else if (code.Equal("{name2}"))
            {
                pg.ReplaceText(code, di.Person.Name2);
            }
            else if (code.Equal("{familyname}"))
            {
                pg.ReplaceText(code, di.FamilyName);
            }
            else if (code.Equal("{familytitle}"))
            {
                pg.ReplaceText(code, di.FamilyTitle);
            }
            else if (code.Equal("{lastname}"))
            {
                pg.ReplaceText(code, di.Person.LastName);
            }
            else if (code.Equal("{firstnames}"))
            {
                pg.ReplaceText(code, di.FirstNames);
            }
            else if (code.Equal("{addr}"))
            {
                pg.ReplaceText(code, di.Address);
            }
            else if (ReplaceBday(pg, code, di))
            {
                return true;
            }
            else if (ReplaceAnniversary(pg, code, di))
            {
                return true;
            }
            else if (ReplaceEmail(pg, code, di))
            {
                return true;
            }
            else if (ReplaceSpEmail(pg, code, di))
            {
                return true;
            }
            else if (ReplacePhone(pg, code, di))
            {
                return true;
            }
            else if (ReplaceCell(pg, code, di))
            {
                return true;
            }
            else if (ReplaceSpCell(pg, code, di))
            {
                return true;
            }
            else if (ReplaceSpouse(pg, code, di))
            {
                return true;
            }
            else if (ReplaceKids(pg, code, di))
            {
                return true;
            }
            else if (ReplacePic(pg, code, di))
            {
                return true;
            }
            else
            {
                return false;
            }

            return true;
        }


        private readonly Regex kidsre = new Regex(@"{kids(:(?<text>[^}]*)){0,1}}");
        private bool ReplaceKids(Paragraph pg, string code, DirectoryInfo di)
        {
            if (!kidsre.IsMatch(code))
            {
                return false;
            }

            if (di.Children.HasValue())
            {
                var m = kidsre.Match(code);
                var txt = m.Groups["text"].Value;
                pg.ReplaceText(code, txt.HasValue()
                    ? txt.Replace("_names_", di.Children)
                    : di.Children);
            }
            else
            {
                pg.ReplaceText(code, "");
            }

            return true;
        }

        private readonly Regex spousere = new Regex(@"{spouse(:(?<text>[^}]*)){0,1}}");
        private bool ReplaceSpouse(Paragraph pg, string code, DirectoryInfo di)
        {
            if (!spousere.IsMatch(code))
            {
                return false;
            }

            if (di.SpouseName.HasValue())
            {
                var m = spousere.Match(code);
                var txt = m.Groups["text"].Value;
                pg.ReplaceText(code, txt.HasValue()
                    ? txt.Replace("_name_", di.SpouseName)
                    : di.SpouseName);
            }
            else
            {
                pg.ReplaceText(code, "");
            }

            return true;
        }

        private readonly Regex spcellre = new Regex(@"{spcell(:(?<text>[^}]*)){0,1}}");
        private bool ReplaceSpCell(Paragraph pg, string code, DirectoryInfo di)
        {
            if (!spcellre.IsMatch(code))
            {
                return false;
            }

            if (di.SpouseCell.HasValue())
            {
                var m = spcellre.Match(code);
                var txt = m.Groups["text"].Value;
                if (!txt.HasValue())
                {
                    pg.ReplaceText(code, di.SpouseCell);
                }
                else
                {
                    txt = txt.Replace("_number_", di.SpouseCell)
                        .Replace("_first_", di.SpouseFirst);
                    pg.ReplaceText(code, txt);
                }
            }
            else
            {
                pg.ReplaceText(code, "");
            }

            return true;
        }

        private readonly Regex cellre = new Regex(@"{cell(:(?<text>[^}]*)){0,1}}");
        private bool ReplaceCell(Paragraph pg, string code, DirectoryInfo di)
        {
            if (!cellre.IsMatch(code))
            {
                return false;
            }

            if (di.CellPhone.HasValue())
            {
                var m = cellre.Match(code);
                var txt = m.Groups["text"].Value;
                if (!txt.HasValue())
                {
                    pg.ReplaceText(code, di.CellPhone);
                }
                else
                {
                    txt = txt.Replace("_number_", di.CellPhone)
                        .Replace("_first_", di.Person.PreferredName);
                    pg.ReplaceText(code, txt);
                }
            }
            else
            {
                pg.ReplaceText(code, "");
            }

            return true;
        }

        private readonly Regex phonere = new Regex(@"{phone(:(?<text>[^}]*)){0,1}}");
        private bool ReplacePhone(Paragraph pg, string code, DirectoryInfo di)
        {
            if (!phonere.IsMatch(code))
            {
                return false;
            }

            if (!di.HomePhone.HasValue() || di.SpouseDoNotPublishPhone == true)
            {
                pg.ReplaceText(code, "");
            }
            else
            {
                var m = phonere.Match(code);
                var txt = m.Groups["text"].Value;
                pg.ReplaceText(code, txt.HasValue()
                    ? txt.Replace("_number_", di.HomePhone)
                    : di.HomePhone);
            }
            return true;
        }

        private readonly Regex spemailre = new Regex(@"{spemail(:(?<text>[^}]*)){0,1}}");
        private bool ReplaceSpEmail(Paragraph pg, string code, DirectoryInfo di)
        {
            if (!spemailre.IsMatch(code))
            {
                return false;
            }

            if (di.SpouseEmail.HasValue())
            {
                var m = spemailre.Match(code);
                var txt = m.Groups["text"].Value;
                if (!txt.HasValue())
                {
                    pg.ReplaceText(code, di.SpouseEmail);
                }
                else
                {
                    txt = txt.Replace("_addr_", di.SpouseEmail)
                        .Replace("_first_", di.SpouseFirst);
                    pg.ReplaceText(code, txt);
                }
            }
            else
            {
                pg.ReplaceText(code, "");
            }

            return true;
        }

        private readonly Regex emailre = new Regex(@"{email(:(?<text>[^}]*)){0,1}}");
        private bool ReplaceEmail(Paragraph pg, string code, DirectoryInfo di)
        {
            if (!emailre.IsMatch(code))
            {
                return false;
            }

            if (di.Person.EmailAddress.HasValue())
            {
                var m = emailre.Match(code);
                var txt = m.Groups["text"].Value;
                if (!txt.HasValue())
                {
                    pg.ReplaceText(code, di.Person.EmailAddress);
                }
                else
                {
                    txt = txt.Replace("_addr_", di.Person.EmailAddress)
                        .Replace("_first_", di.Person.PreferredName);
                    pg.ReplaceText(code, txt);
                }
            }
            else
            {
                pg.ReplaceText(code, "");
            }

            return true;
        }

        private readonly Regex dtfmtre = new Regex("_(?<fmt>[^_]*)_");
        private readonly Regex bdayre = new Regex(@"{bday(:(?<text>[^}]*)){0,1}}");
        private bool ReplaceBday(Paragraph pg, string code, DirectoryInfo di)
        {
            if (!bdayre.IsMatch(code))
            {
                return false;
            }

            if (di.Person.BirthDate.HasValue)
            {
                var m = bdayre.Match(code);
                var txt = m.Groups["text"].Value;

                if (!txt.HasValue())
                {
                    pg.ReplaceText(code, di.Person.BirthDate.ToString2("MMM d"));
                }
                else
                {
                    txt = txt.Replace("_first_", di.Person.PreferredName);
                    m = dtfmtre.Match(txt);
                    var repl = m.Value;
                    var mfmt = m.Groups["fmt"].Value;
                    txt = txt.Replace(repl, di.Person.BirthDate.ToString2(Util.PickFirst(mfmt, "MMM d")));
                    pg.ReplaceText(code, txt);
                }
            }
            else
            {
                pg.ReplaceText(code, "");
            }

            return true;
        }

        private readonly Regex anniversaryre = new Regex(@"{anniversary(:(?<text>[^}]*)){0,1}}");
        private bool ReplaceAnniversary(Paragraph pg, string code, DirectoryInfo di)
        {
            if (!anniversaryre.IsMatch(code))
            {
                return false;
            }

            if (di.Person.WeddingDate.HasValue)
            {
                var m = anniversaryre.Match(code);
                var txt = m.Groups["text"].Value;
                m = dtfmtre.Match(txt);
                var repl = m.Value;
                var mfmt = m.Groups["fmt"].Value;
                pg.ReplaceText(code, txt.HasValue()
                    ? txt.Replace(repl, di.Person.WeddingDate.ToString2(Util.PickFirst(mfmt, "MMM d")))
                    : di.Person.WeddingDate.ToString2("MMM d"));
            }
            else
            {
                pg.ReplaceText(code, "");
            }

            return true;
        }

        private readonly Regex picre = new Regex(@"{(?<pic>pic|fampic) +(?<width>[\.\d]*) +(?<height>[\.\d]*)}");
        private bool ReplacePic(Paragraph pg, string code, DirectoryInfo di)
        {
            if (!picre.IsMatch(code))
            {
                return false;
            }

            var m = picre.Match(code);
            var pic = m.Groups["pic"].Value;
            var height = m.Groups["height"].Value.ToDouble();
            var width = m.Groups["width"].Value.ToDouble();
            pg.ReplaceText(code, "");
            AddPicture(pg, width, height, pic == "pic" ? di.ImageId : di.FamImageId);
            return true;
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
            {
                using (var os = img.ResizeToStream(largewidth, largeheight, "pad"))
                {
                    var pic = docx.AddImage(os).CreatePicture(heightpixels, widthpixels);
                    pg.AppendPicture(pic);
                }
            }
        }
    }
}
