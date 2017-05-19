/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using Novacode;
using UtilityExtensions;

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
            this.filename = filename ?? "picturedir";
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

            var firstrow = docx.Tables[0].Rows[0];
            var ncols = firstrow.ColumnCount;
            Row row = null;

            replacements = new EmailReplacements(DbUtil.Db, docx);

            foreach (var m in list)
            {
                if (c == 0)
                {
                    row = tbl.InsertRow(firstrow);
                    tbl.Rows.Add(row);
                }
                var cell = row?.Cells[c++];
                AddCellWithReplacements(cell, m);
                if (c == ncols)
                    c = 0;
            }
            tbl.RemoveRow(0); // the first row was used as a template

            response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            response.AddHeader("content-disposition", $"attachment;filename={filename}-{DateTime.Now.ToSortableDateTime()}.docx");
            response.AddHeader("content-length", ms.Length.ToString());
            docx.SaveAs(response.OutputStream);
        }

        private void AddCellWithReplacements(Container cell, DirectoryInfo di)
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
                            else if (d.Key.StartsWith("{bdmd}"))
                                pg.ReplaceText(d.Key, di.Person.BirthDate.ToString2("MMM/d"));
                            else if (d.Key.Equal("{spouse}"))
                                pg.ReplaceText(d.Key, di.SpouseName);
                            else if (d.Key.Equal("{kids}"))
                                pg.ReplaceText(d.Key, di.Children);
                            else if (d.Key.StartsWith("{pic:"))
                            {
                                pg.ReplaceText(d.Key, "");
// {pic:.67,.645}
// .67 = aspect ratio width/height
// 0.645 = inches wide
// 96 = pixels in inch
                                AddPicture(pg, 200, 300, di);
                            }
                            else
                                pg.ReplaceText(d.Key, d.Value);
        }


        private void AddPicture(Paragraph pg, int width, int height, DirectoryInfo di)
        {
            var img = ImageData.Image.ImageFromId(di.ImageId);
            if (img != null)
                using (var os = img.ResizeToStream(width, height, "pad"))
                {
                    var w62 = Convert.ToDouble(width) / 200 * 62;
                    var rat = Convert.ToDouble(height) / width;
                    var pic = docx.AddImage(os).CreatePicture((w62 * rat).ToInt(), w62.ToInt());
                    pg.InsertPicture(pic);
                }
        }

        private byte[] DirectoryTemplate()
        {
            var loc = Util.PickFirst(template, DbUtil.Db.Setting("DirectoryTemplate", ""));
            if (loc.HasValue())
            {
                var wc = new WebClient();
                return wc.DownloadData(loc);
            }
            return Resource1.DocXDirectory;
        }
    }
}