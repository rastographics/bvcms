using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Constants;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Models.SmsMessages
{
    public class SmsReceivedMessagesModel : PagedTableModel<SmsReceived, SmsReceived>
    {
        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public SmsReceivedMessagesModel()
        {
        }
        public SmsReceivedMessagesModel(CMSDataContext db) : base(db, useAjax: true)
        {
        }
        [Display(Name = "Start Date")]
        public DateTime? RecdFilterStart { get; set; }
        [Display(Name = "End Date")]
        public DateTime? RecdFilterEnd { get; set; }
        [Display(Name = "Message")]
        public string RecdFilterMessage { get; set; }
        [Display(Name = "Recipient Group")]
        public string RecdFilterGroupId { get; set; }
        [Display(Name = "Sender")]
        public string RecdFilterSender { get; set; }

        private IQueryable<SmsReceived> FetchReceivedMessages()
        {
            var q = from message in CurrentDatabase.SmsReceiveds select message;
            if (RecdFilterStart != null)
                q = q.Where(e => e.DateReceived >= RecdFilterStart);
            if (RecdFilterEnd != null)
                q = q.Where(e => e.DateReceived < RecdFilterEnd.Value.AddHours(24));
            if (RecdFilterGroupId.ToInt() > 0)
                q = q.Where(e => e.ToGroupId == RecdFilterGroupId.ToInt());
            if (RecdFilterSender.HasValue())
            {
                q = RecdFilterSender.AllDigits()
                    ? q.Where(e => e.FromPeopleId == RecdFilterSender.ToInt())
                    : q.Where(e => e.Person.Name.Contains(RecdFilterSender));
            }
            if (RecdFilterMessage.HasValue())
                q = q.Where(e => e.Body.Contains(RecdFilterMessage));
            return q;
        }
        public override IQueryable<SmsReceived> DefineModelList()
        {
            var q = FetchReceivedMessages();
            if (!count.HasValue)
                count = q.Count();
            return q;
        }

        public override IEnumerable<SmsReceived> DefineViewList(IQueryable<SmsReceived> q)
        {
            return q;
        }

        public IEnumerable<SelectListItem> Groups()
        {
            var q = from c in CurrentDatabase.SMSGroups
                where !c.IsDeleted
                select new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                };
            var groups = q.ToList();
            groups.Insert(0, new SelectListItem {Text = "(select group)", Value = "0"});
            return groups;
        }

        public override IQueryable<SmsReceived> DefineModelSort(IQueryable<SmsReceived> q)
        {
            if (Direction == "asc")
            {
                switch (Sort)
                {
                    case "Recipient":
                        q = from e in q
                            orderby e.SMSGroup.Name, e.ToNumber
                            select e;
                        break;
                    case "Sender":
                        q = q.OrderBy(e => e.Person.Name);
                        break;
                    case "Message":
                        q = q.OrderBy(e => e.Body);
                        break;
                    case "Received":
                    default:
                        q = q.OrderByDescending(e => e.DateReceived);
                        break;
                }
            }
            else
            {
                switch (Sort)
                {
                    case "Recipient":
                        q = from e in q
                            orderby e.SMSGroup.Name descending, e.ToNumber descending
                            select e;
                        break;
                    case "Sender":
                        q = q.OrderByDescending(e => e.Person.Name2);
                        break;
                    case "Message":
                        q = q.OrderByDescending(e => e.Body);
                        break;
                    case "Received":
                    default:
                        q = q.OrderByDescending(e => e.DateReceived);
                        break;
                }
            }

            return q;
        }
        public void TagAll(string tagname, bool? cleartagfirst)
        {
            var workingTag = CurrentDatabase.FetchOrCreateTag(Util2.GetValidTagName(tagname), CurrentDatabase.UserPeopleId, DbUtil.TagTypeId_Personal);
            var shouldEmptyTag = cleartagfirst ?? false;

            if (shouldEmptyTag)
            {
                CurrentDatabase.ClearTag(workingTag);
            }
            if (workingTag == null)
            {
                throw new ArgumentNullException(nameof(workingTag));
            }

            CurrentDatabase.CurrentTagName = workingTag.Name;
            CurrentDatabase.CurrentTagOwnerId = workingTag.PersonOwner.PeopleId;

            var q = DefineModelList();

            var listpeople = q.Select(vv => vv.Person.PeopleId).Distinct();
            CurrentDatabase.TagAll(listpeople, workingTag);
            Util2.CurrentTag = workingTag.Name;
        }

        public static ReceivedDetailViewModel Detail(CMSDataContext db, int id)
        {
            return (from r in db.SmsReceiveds
                     where r.Id == id
                     select new ReceivedDetailViewModel()
                     {
                         R = r,
                         PersonName = r.Person.Name,
                         GroupName = r.SMSGroup.Name
                     }).Single();
        }

        public static object FetchReplyToData(CMSDataContext db, int receivedId)
        {
            var m = (from r in db.SmsReceiveds
                where r.Id == receivedId
                select new
                {
                    FromGroup = r.SMSGroup.Name,
                    ToPerson = r.Person.Name,
                    ToMessage = r.Body,
                    Response = r.ActionResponse,
                    ReceivedId = r.Id
                }).Single();
            return m;
        }
    }
}
