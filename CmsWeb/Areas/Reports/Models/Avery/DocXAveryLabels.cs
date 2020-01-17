using CmsData;
using CmsWeb.Lifecycle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Mvc;
using UtilityExtensions;
using Xceed.Words.NET;

namespace CmsWeb.Models
{
    public class DocXAveryLabels : ActionResult
    {
        public IRequestManager RequestManager { get; set; }
        private CMSDataContext CurrentDatabase => RequestManager.CurrentDatabase;

        public int Skip = 0;
        public string Format;
        public bool? Titles;
        public bool? UseMailFlags;
        public bool? SortZip;
        public string Sort => (SortZip ?? false) ? "Zip" : "Name";
        public bool UsePhone { get; set; }

        private readonly Guid QueryId;
        public DocXAveryLabels(Guid queryId, IRequestManager requestManager)
        {
            RequestManager = requestManager;
            QueryId = queryId;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var controller = new MailingController(RequestManager) { UseTitles = Titles ?? false, UseMailFlags = UseMailFlags ?? false };
            var response = context.HttpContext.Response;
            IEnumerable<MailingController.MailingInfo> q = null;
            switch (Format)
            {
                case "Individual":
                    q = controller.FetchIndividualList(Sort, QueryId);
                    break;
                case "GroupAddress":
                    q = controller.GroupByAddress(Sort, QueryId);
                    break;
                case "Family":
                case "FamilyMembers":
                    q = controller.FetchFamilyList(Sort, QueryId);
                    break;
                case "ParentsOf":
                    q = controller.FetchParentsOfList(Sort, QueryId);
                    break;
                case "CouplesEither":
                    q = controller.FetchCouplesEitherList(Sort, QueryId);
                    break;
                case "CouplesBoth":
                    q = controller.FetchCouplesBothList(Sort, QueryId);
                    break;
                default:
                    response.Write("unknown format");
                    return;
            }

            DocX template;
            using (var templateStream = new MemoryStream(AveryLabelTemplate() ?? Resource1.DocxAveryLabel))
            {
                template = DocX.Load(templateStream);
            }
            var row = 0;
            var col = 0;
            DocX finaldoc = null;
            DocX currpage = null;
            foreach (var mailing in q)
            {
                if (currpage == null || (row == 0 && col == 0))
                {
                    currpage = template.Copy();
                }

                if (Skip > 0)
                {
                    row = Skip / 3;
                    col = Skip % 3;
                    if (col > 0)
                    {
                        col *= 2;
                    }

                    Skip = 0;
                }
                var cell = currpage.Tables[0].Rows[row].Cells[col];
                var paragraph = cell.Paragraphs[0];

                if (Format == "GroupAddress")
                {
                    paragraph.InsertText(mailing.LabelName + " " + mailing.LastName);
                }
                else if ((Format == "CouplesEither" || Format == "CouplesBoth") && mailing.CoupleName.HasValue())
                {
                    paragraph.InsertText(mailing.CoupleName);
                }
                else
                {
                    paragraph.InsertText(mailing.LabelName);
                }

                if (mailing.MailingAddress.HasValue())
                {
                    paragraph.InsertText($"\n{mailing.MailingAddress.Trim()}");
                }
                else
                {
                    paragraph.InsertText($"\n{mailing.Address}");
                    if (mailing.Address2.HasValue())
                    {
                        paragraph.InsertText($"\n{mailing.Address2}");
                    }

                    paragraph.InsertText($"\n{mailing.CSZ}");
                }
                if (UsePhone)
                {
                    var phone = Util.PickFirst(mailing.CellPhone.FmtFone("C "), mailing.HomePhone.FmtFone("H "));
                    paragraph.InsertText($"\n{phone}");
                }

                col += 2;
                if (col != 6)
                {
                    continue;
                }

                col = 0;
                row++;
                if (row != 10)
                {
                    continue;
                }

                row = 0;
                if (finaldoc == null)
                {
                    finaldoc = currpage;
                }
                else
                {
                    finaldoc.InsertDocument(currpage);
                }

                currpage = null;
            }
            if (finaldoc == null && currpage == null)
            {
                response.Write("no data found");
                return;
            }
            if (finaldoc == null)
            {
                finaldoc = currpage;
            }
            else if (currpage != null)
            {
                finaldoc.InsertDocument(currpage);
            }

            response.Clear();
            response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            response.AddHeader("content-disposition", $"attachment;filename=AveryLabels-{DateTime.Now.ToSortableDateTime()}.docx");

            using (var ms = new MemoryStream())
            {
                finaldoc.SaveAs(ms);
                ms.WriteTo(response.OutputStream);
            }
            response.End();
        }

        private byte[] AveryLabelTemplate()
        {
            var loc = CurrentDatabase.Setting("AveryLabelTemplate", "");
            if (loc.HasValue())
            {
                var wc = new WebClient();
                return wc.DownloadData(loc);
            }
            return null;
        }
    }
}
