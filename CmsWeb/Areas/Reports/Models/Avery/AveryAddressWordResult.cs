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
using System.Web.Mvc;
using CmsWeb.Models;
using Novacode;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class AveryAddressWordResult : ActionResult
    {
        public Guid id;
        public string format;
        public bool? titles;
        public bool? useMailFlags;
        public bool usephone { get; set; }
        public int skip = 0;

        public bool? sortzip;
        public string sort
        {
            get { return (sortzip ?? false) ? "Zip" : "Name"; }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var ctl = new MailingController { UseTitles = titles ?? false, UseMailFlags = useMailFlags ?? false };
            var Response = context.HttpContext.Response;

            IEnumerable<MailingController.MailingInfo> q = null;
            switch (format)
            {
                case "Individual":
                    q = ctl.FetchIndividualList(sort, id);
                    break;
                case "GroupAddress":
                    q = ctl.GroupByAddress(sort, id);
                    break;
                case "Family":
                case "FamilyMembers":
                    q = ctl.FetchFamilyList(sort, id);
                    break;
                case "ParentsOf":
                    q = ctl.FetchParentsOfList(sort, id);
                    break;
                case "CouplesEither":
                    q = ctl.FetchCouplesEitherList(sort, id);
                    break;
                case "CouplesBoth":
                    q = ctl.FetchCouplesBothList(sort, id);
                    break;
                default:
                    Response.Write("unknown format");
                    return;
            }
            if (!q.Any())
            {
                Response.Write("no data found");
                return;
            }
            using (var ms = new MemoryStream())
            {
                var dd = DocX.Create("ttt.docx");
                dd.MarginLeft = 30;
                dd.MarginRight = 24;
                dd.MarginTop = 48;
                dd.MarginBottom = 30;
                dd.PageHeight = 1056;
                dd.PageWidth = 816;
                var col = 0;
                var row = 0;
                Table tt = null;
                foreach (var p in q)
                {
                    if (tt == null || col == 0 && row == 0)
                    {
                        tt = dd.InsertTable(10, 5);
                        foreach (var rr in tt.Rows)
                            for (var i = 0; i < 5; i++)
                            {
                                rr.Cells[i].VerticalAlignment = VerticalAlignment.Center;
                                rr.Height = 96.0;
                                rr.Cells[i].Width = i % 2 == 0
                                    ? 252.4667
                                    : 11.4;
                                if (i % 2 == 0)
                                    rr.Cells[i].MarginLeft = 30;
                            }
                    }
                    if (skip > 0)
                    {
                        row = skip / 3;
                        col = skip % 3;
                        if (col > 0)
                            col++;
                        if (col > 2)
                            col++;
                    }
                    var c = tt.Rows[row].Cells[col];

                    if (format == "GroupAddress")
                        c.Paragraphs[0].InsertText(p.LabelName + " " + p.LastName);
                    else if ((format == "CouplesEither" || format == "CouplesBoth") && p.CoupleName.HasValue())
                        c.Paragraphs[0].InsertText(p.CoupleName);
                    else
                        c.Paragraphs[0].InsertText(p.LabelName);


                    if (p.MailingAddress.HasValue())
                        c.InsertParagraph(p.MailingAddress.Trim());
                    else
                    {
                        c.InsertParagraph(p.Address);
                        if (p.Address2.HasValue())
                            c.InsertParagraph(p.Address2);
                        c.InsertParagraph(p.CSZ);
                    }

                    col += 2;
                    if (col == 6)
                    {
                        row++;
                        col = 0;
                        if (row == 10)
                            row = 0;
                    }
                }
                dd.SaveAs(ms);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                Response.AddHeader("content-disposition", "filename=avery-labels.docx");
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
        }
    }
}