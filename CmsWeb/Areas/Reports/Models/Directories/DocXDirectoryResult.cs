/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Areas.Public.Models;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Models;
using Newtonsoft.Json;
using Novacode;
using OpenXmlPowerTools;
using UtilityExtensions;
using Formatting = Novacode.Formatting;
using Paragraph = Novacode.Paragraph;
using Table = Novacode.Table;
using Util = UtilityExtensions.Util;
using static CmsWeb.Areas.Reports.Models.CompactPictureDir;

namespace CmsWeb.Areas.Reports.Models
{
    public class DocXDirectoryResult : ActionResult
    {
        private readonly Guid id;
        private EmailReplacements replacements;
        private DocX docx;
        private DocX curr;

        public DocXDirectoryResult(Guid id, string parameters)
        {
            this.id = id;
            pa = JsonConvert.DeserializeObject<Parameters>(parameters);
        }

        private class DirectoryInfo
        {
            public Person Person { get; set; }
            public string Children { get; set; }
            public int? ImageId { get; set; }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            curr = docx.Copy();

            var query = DbUtil.Db.PeopleQuery(id);
            var q = from p in query
                    let familyname = p.Family.People
                        .Where(pp => pp.PeopleId == pp.Family.HeadOfHouseholdId)
                        .Select(pp => pp.LastName).SingleOrDefault()
                    orderby familyname, p.FamilyId, p.Name2
                    select new DirectoryInfo()
                    {
                        Person = p,
                        Children = p.PositionInFamilyId != 10 ? ""
                            : string.Join(", ", (from cc in p.Family.People
                                                 where cc.PositionInFamilyId == 30
                                                 where cc.Age <= 18
                                                 select cc.LastName == familyname
                                                         ? cc.PreferredName
                                                         : cc.Name).ToList()),
                        ImageId = p.Picture.LargeId
                    };

            if (!q.Any())
            {
                response.Write("no data found");
                return;
            }
            var bytes = DirectoryTemplate() ?? Resource1.DocXDirectory;
            var ms = new MemoryStream(bytes);
            docx = DocX.Load(ms);
            replacements = new EmailReplacements(DbUtil.Db, docx);

            var tbl = curr.Tables[0];
            var emptyrow = tbl.InsertRow();
            tbl.Rows.Add(emptyrow);
            tbl.RemoveRow(0);
            var r = 0;
            var c = 0;

            var tplrow = docx.Tables[0].Rows[0];
            var ncols = tplrow.ColumnCount;
            var tplcell = tplrow.Cells[0];
            Novacode.Row row = null;

            foreach (var m in q)
            {
                if (c == 0)
                {
                    row = tbl.InsertRow(tplrow);
                    tbl.Rows.Add(row);
                }
                var cell = row?.Cells[c++];
                cell = tbl.Insert
                AddCellWithReplacements(cell, m);
                if (c == ncols)
                    c = 0;
            }
        }

        private void AddCellWithReplacements(Novacode.Cell cell, DirectoryInfo di)
        {
            var dict = replacements.DocXReplacementsDictionary(di.Person);
            foreach (var pg in cell.Paragraphs.Where(vv => vv.Text.HasValue()))
                if (dict.Keys.Any(vv => pg.Text.Contains(vv)))
                    foreach (var d in dict)
                        if (pg.Text.Contains(d.Key))
                            if (d.Key.Equal("{altname}"))
                                pg.ReplaceText(d.Key, di.Person.AltName);
                            else if (d.Key.Equal("{name}"))
                                pg.ReplaceText(d.Key, di.Person.Name2);
                            else
                                pg.ReplaceText(d.Key, d.Value);
        }


        private int FillCell(Novacode.Row rr, int col, ExcelPic p)
        {
            var c = rr.Cells[col];
            var img = ImageData.Image.ImageFromId(p.ImageId);
            if (img != null)
                using (var os = img.ResizeToStream(200, 300, "max"))
                {
                    var pic = docx.AddImage(os).CreatePicture((pa.PicWidthPixels * 1.5).ToInt(), pa.PicWidthPixels);
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

        private static byte[] DirectoryTemplate()
        {
            var loc = DbUtil.Db.Setting("DirectoryTemplate", "");
            if (loc.HasValue())
            {
                var wc = new WebClient();
                return wc.DownloadData(loc);
            }
            return null;
        }

        private readonly Parameters pa;
    }
}