/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CmsData;
using Novacode;
using UtilityExtensions;
using Formatting = Novacode.Formatting;

namespace CmsWeb.Areas.Reports.Models
{
    public class FamilyPictureDir : ActionResult
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
            public double CellWidth { get; set; }
            public double MaxPicHeight { get; set; }

            public double PicHeightPixels => Pixels(MaxPicHeight);

            public double FontSize { get; set; }
            public double FontSizeName { get; set; }
            public double FontSizeEmail { get; set; }
            public double MarginLeft { get; set; }
            public double MarginRight { get; set; }
            public double MarginTop { get; set; }
            public double MarginBottom { get; set; }

            public Formatting namebold => new Formatting { Size = FontSizeName, Bold = true };
            public Formatting emailsmall => new Formatting { Size = FontSizeEmail };
            public Formatting font => new Formatting { Size = FontSize };
        }
        public FamilyPictureDir(Guid id)
        {
            this.id = id;
            dd = DocX.Create("ttt.docx");

            pa = new Parameters()
            {
                PageHeight = 8.5,
                PageWidth = 5.5,
                MarginLeft = .5,
                MarginRight = .3,
                MarginTop = .5,
                MarginBottom = .3,
                FontSizeName = 18.0,
                FontSize = 14,
            };

            pa.CellWidth = pa.PageWidth - pa.MarginLeft - pa.MarginRight;
            pa.RowHeight = pa.PageHeight - pa.MarginTop - pa.MarginBottom;
            pa.MaxPicHeight = pa.RowHeight * .4;

            dd.PageHeight = Pixels(pa.PageHeight);
            dd.PageWidth = Pixels(pa.PageWidth);
            dd.MarginLeft = Pixels(pa.MarginLeft);
            dd.MarginRight = Pixels(pa.MarginRight);
            dd.MarginTop = Pixels(pa.MarginTop);
            dd.MarginBottom = Pixels(pa.MarginBottom);
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;

            var q = familyList(id);

            if (!q.Any())
            {
                Response.Write("no data found");
                return;
            }
            using (var ms = new MemoryStream())
            {
                var row = 0;
                var tt = dd.InsertTable(q.Count, 1);
                //var border = new Border(BorderStyle.Tcbs_single, BorderSize.one, 4, System.Drawing.Color.Black);
                for (var r = 0; r < tt.RowCount; r++)
                {
                    var rr = tt.Rows[r];
                    rr.Height = Pixels(pa.RowHeight);
                    var c = rr.Cells[0];
                    c.Width = Pixels(pa.CellWidth);
//                    c.SetBorder(TableCellBorderType.Left, border);
//                    c.SetBorder(TableCellBorderType.Top, border);
//                    c.SetBorder(TableCellBorderType.Right, border);
//                    c.SetBorder(TableCellBorderType.Bottom, border);
                }
                foreach (var p in q)
                {
                    AddFamily(tt.Rows[row], p);
                    row++;
                }
                dd.SaveAs(ms);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                Response.AddHeader("content-disposition", "filename=picturedir.docx");
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
        }


        private void AddFamily(Row rr, FamilyInfo f)
        {
            var c = rr.Cells[0];
            var img = ImageData.Image.ImageFromId(f.ImageId);
            if (img != null)
            {
                var ratio = img.Ratio();
                using (var os = img.ResizeToStream("width=1000&height=1000&mode=max"))
                {
                    var h = pa.PicHeightPixels;
                    var w = h * ratio;

                    var str = dd.AddImage(os);
                    var pic = str.CreatePicture(h.ToInt(), w.ToInt());

                    c.Paragraphs[0].InsertPicture(pic);
                    c.Paragraphs[0].Alignment = Alignment.center;
                }
            }

            c.InsertParagraph();
            var p = c.InsertParagraph(f.Title, false, pa.namebold);
            p.Alignment = Alignment.center;

            var cc = f.Children().ToList();

            c.InsertParagraph("", false, pa.font);
            var t2 = c.InsertTable(cc.Count + 2 + (f.Head2.HasValue() ? 1 : 0), 2);
            t2.Alignment = Alignment.center;
            t2.AutoFit = AutoFit.Contents;
            t2.Rows[0].Cells[1].Width = 0;

            var row = 0;
            t2.Rows[row].Cells[0].Paragraphs[0].InsertText(f.Head1, false, pa.font);
            t2.Rows[row].Cells[1].Paragraphs[0].InsertText(f.BirthDay, false, pa.font);
            if (f.Head2 != null)
            {
                row++;
                t2.Rows[0].Cells[1].Width = 0;
                t2.Rows[row].Cells[0].Paragraphs[0].InsertText(f.Head2, false, pa.font);
                t2.Rows[row].Cells[1].Paragraphs[0].InsertText(f.BirthDay2, false, pa.font);
            }
            foreach (var ch in cc)
            {
                row++;
                t2.Rows[0].Cells[1].Width = 0;
                t2.Rows[row].Cells[0].Paragraphs[0].InsertText(ch.Display, false, pa.font);
                t2.Rows[row].Cells[1].Paragraphs[0].InsertText(ch.BirthDay, false, pa.font);
            }
            row++;
            t2.Rows[row].MergeCells(0, 1);

            var cell = t2.Rows[row].Cells[0];
            var pg = cell.Paragraphs[0];

            var addr = f.FullAddress;
            if (addr.HasValue())
                pg.InsertText("\n" + addr, false, pa.font);

            var phones = f.Phones;
            if(phones.HasValue())
                pg.InsertText("\n" + phones, false, pa.font);

            var emails = f.Emails;
            if(emails.HasValue())
                pg.InsertText("\n" + emails, false, pa.font);
        }

        private static float Pixels(double inches)
        {
            return Convert.ToSingle(inches * 1440 / 15);
        }

        private static List<FamilyInfo> familyList(Guid queryid)
        {
            var Db = DbUtil.Db;
            var query = Db.PeopleQuery(queryid);
            var q = from p in query
                    where p.PeopleId == p.Family.HeadOfHouseholdId
                    let familyname = p.Family.People
                        .Where(pp => pp.PeopleId == pp.Family.HeadOfHouseholdId)
                        .Select(pp => pp.LastName).SingleOrDefault()
                    orderby familyname, p.FamilyId
                    let title =
                        p.Family.FamilyExtras.Where(mm => mm.Field == "FamilyName" && mm.Data != null)
                            .Select(mm => mm.Data)
                            .SingleOrDefault()
                    let sp = p.Family.People.SingleOrDefault(ff => p.SpouseId == ff.PeopleId)
                    let name1 =
                        p.PeopleExtras.Where(mm => mm.Field == "DisplayName" && mm.Data != null)
                            .Select(mm => mm.Data)
                            .SingleOrDefault()
                    let name2 =
                        sp.PeopleExtras.Where(mm => mm.Field == "DisplayName" && mm.Data != null)
                            .Select(mm => mm.Data)
                            .SingleOrDefault()
                    select new FamilyInfo
                    {
                        Head1 = name1 ?? p.PreferredName,
                        Head2 = name2 ?? sp.PreferredName,
                        First1 = p.PreferredName,
                        First2 = sp.PreferredName,
                        BDay1 = p.BirthDay,
                        BMon1 = p.BirthMonth,
                        BDay2 = sp.BirthDay,
                        BMon2 = sp.BirthMonth,
                        Title = title ?? ("The " + p.Name + " Family"),
                        Address = p.PrimaryAddress,
                        Address2 = p.PrimaryAddress2,
                        City = p.PrimaryCity,
                        State = p.PrimaryState,
                        Zip = p.PrimaryZip.FmtZip(),
                        Email = p.EmailAddress,
                        Email2 = sp.EmailAddress,
                        Cellphone = p.CellPhone.FmtFone(),
                        Cellphone2 = sp.CellPhone.FmtFone(),
                        Homephone = p.HomePhone.FmtFone(),
                        children = p.PositionInFamilyId != 10
                            ? ""
                            : string.Join("~", p.Family.People
                                .Where(cc => cc.PositionInFamilyId > 10)
                                .Select(cc =>
                                    $"{(cc.LastName == familyname ? cc.PreferredName : cc.Name)}|{cc.Age}|{cc.BirthMonth}|{cc.BirthDay}"
                                )),
                        ImageId = p.Family.Picture.LargeId,
                    };
            var list = q.Take(10000).ToList();
            return list;
        }

        public class FamilyInfo
        {
            public string Head1 { get; set; }
            public string Head2 { get; set; }
            public string First1 { get; set; }
            public string First2 { get; set; }
            public int? BDay1 { get; set; }
            public int? BMon1 { get; set; }
            public string BirthDay
            {
                get { return Util.FormatBirthday(null, BMon1, BDay1); }
            }
            public int? BDay2 { get; set; }
            public int? BMon2 { get; set; }
            public string BirthDay2 => Util.FormatBirthday(null, BMon2, BDay2);
            public string Title { get; set; }
            public string Address { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }

            public string Emails
            {
                get
                {
                    var sb = new StringBuilder();
                    if (Email.HasValue())
                        sb.AppendLine($"{First1}: {Email}");
                    if (Email2.HasValue())
                        sb.AppendLine($"{First2}: {Email2}");
                    return sb.ToString().TrimEnd();
                }
            }
            public string Phones
            {
                get
                {
                    var sb = new StringBuilder();
                    if (Homephone.HasValue())
                        sb.AppendLine(Homephone.FmtFone("H "));
                    if (Cellphone.HasValue())
                        sb.AppendLine(Cellphone.FmtFone("C " + First1));
                    if (Cellphone2.HasValue())
                        sb.AppendLine(Cellphone2.FmtFone("C " + First2));
                    return sb.ToString();
                }
            }
            public string FullAddress
            {
                get
                {
                    var sb = new StringBuilder();
                    if (Address.HasValue())
                        sb.AppendLine(Address);
                    if (Address2.HasValue())
                        sb.AppendLine(Address2);
                    if (CSZ.HasValue())
                        sb.AppendLine(CSZ);
                    return sb.ToString();
                }
            }

            public string CSZ => Util.FormatCSZ5(City, State, Zip);

            public string Email { get; set; }
            public string Email2 { get; set; }
            public string Cellphone { get; set; }
            public string Cellphone2 { get; set; }
            public string Homephone { get; set; }
            public string children { get; set; }
            public int? ImageId { get; set; }

            public IEnumerable<ChildInfo> Children()
            {
                if (children.Length == 0)
                    return new List<ChildInfo>();
                var cc = children.Split('~');
                return cc.Select(s =>
                {
                    var ss = s.Split('|');
                    return new ChildInfo()
                    {
                        Name = ss[0],
                        Age = ss[1].ToInt2(),
                        BMon = ss[2].ToInt2(),
                        BDay = ss[3].ToInt2()
                    };
                });
            }
        }

        public class ChildInfo
        {
            public string Name { get; set; }
            public int? Age { get; set; }

            public string Display => Age.HasValue ? $"{Name} ({Age})" : Name;

            public int? BDay { get; set; }
            public int? BMon { get; set; }

            public string BirthDay => Util.FormatBirthday(null, BMon, BDay);
        }
    }
}
