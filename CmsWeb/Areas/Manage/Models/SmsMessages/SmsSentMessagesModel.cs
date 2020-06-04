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
    public class SmsSentMessagesModel : PagedTableModel<SMSList, SMSList>
    {
        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public SmsSentMessagesModel()
        {
        }
        public SmsSentMessagesModel(CMSDataContext db) : base(db, useAjax: true)
        {
        }
        [Display(Name = "Start Date")]
        public DateTime? SentFilterStart { get; set; }
        [Display(Name = "End Date")]
        public DateTime? SentFilterEnd { get; set; }
        [Display(Name = "Message Title")]
        public string SentFilterTitle { get; set; }
        [Display(Name = "Group")]
        public string SentFilterGroupId { get; set; }
        [Display(Name = "Group Member")]
        public string SentFilterPeopleId { get; set; }

        private IQueryable<SMSList> FetchSentMessages()
        {
            var ingroups = UserInGroups();
            var q = from message in CurrentDatabase.SMSLists
                where ingroups.Contains(message.SendGroupID)
                select message;

            if (SentFilterStart != null)
                q = q.Where(e => e.SendAt >= SentFilterStart);
            if (SentFilterEnd != null)
                q = q.Where(e => e.SendAt < SentFilterEnd.Value.AddHours(24));
            if (SentFilterGroupId.ToInt() > 0)
                q = q.Where(e => e.SendGroupID == SentFilterGroupId.ToInt());
            if (SentFilterPeopleId.ToInt() > 0)
                q = q.Where(e => e.SenderID == SentFilterPeopleId.ToInt());
            if (SentFilterTitle.HasValue())
            {
                var titleid = SentFilterTitle.Substring(1);
                q = titleid.AllDigits()
                    ? q.Where(e => e.ReplyToId == titleid.ToInt())
                    : q.Where(e => e.Title.Contains(SentFilterTitle));
            }
            return q;
        }

        private List<int> UserInGroups()
        {
            var manageSmsUser = CurrentDatabase.CurrentUser.InRole("ManageSms");
            return (from gm in CurrentDatabase.SMSGroupMembers
                where manageSmsUser || gm.User.PeopleId == CurrentDatabase.UserPeopleId
                select gm.GroupID).ToList();
        }

        public override IQueryable<SMSList> DefineModelList()
        {
            var q = FetchSentMessages();
            if (!count.HasValue)
                count = q.Count();
            return q;
        }

        public override IEnumerable<SMSList> DefineViewList(IQueryable<SMSList> q)
        {
            return q;
        }

        public IEnumerable<SelectListItem> Groups()
        {
            var ingroups = UserInGroups();
            var q = from c in CurrentDatabase.SMSGroups
                where ingroups.Contains(c.Id)
                    where !c.IsDeleted
                    select new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    };
            var groups = q.ToList();
            groups.Insert(0, new SelectListItem { Text = "(select group)", Value = "0" });
            return groups;
        }

        public IEnumerable<SelectListItem> GroupMembers()
        {
            var q = from c in CurrentDatabase.SMSGroupMembers
                    where SentFilterGroupId.ToInt() == c.GroupID
                    select new SelectListItem
                    {
                        Value = c.User.PeopleId.ToString(),
                        Text = c.User.Name2,
                    };
            var groupMembers = q.ToList();
            groupMembers.Insert(0, new SelectListItem { Text = "(select group member)", Value = "0" });
            return groupMembers;
        }

        public override IQueryable<SMSList> DefineModelSort(IQueryable<SMSList> q)
        {
            if (Direction == "asc")
            {
                switch (Sort)
                {
                    case "Sent/Scheduled":
                        q = q.OrderBy(e => e.Created);
                        break;
                    case "From":
                        q = q.OrderBy(e => e.Person.Name);
                        break;
                    case "Title":
                        q = q.OrderBy(e => e.Title);
                        break;
                    case "Include":
                        q = q.OrderBy(e => e.SentSMS);
                        break;
                    case "Exclude":
                        q = q.OrderBy(e => e.SentNone);
                        break;
                }
            }
            else
            {
                switch (Sort)
                {
                    case "Sent/Scheduled":
                        q = q.OrderByDescending(e => e.Created);
                        break;
                    case "From":
                        q = q.OrderByDescending(e => e.Person.Name);
                        break;
                    case "Title":
                        q = q.OrderByDescending(e => e.Title);
                        break;
                    case "Include":
                        q = q.OrderByDescending(e => e.SentSMS);
                        break;
                    case "Exclude":
                        q = q.OrderByDescending(e => e.SentNone);
                        break;
                    default:
                        q = q.OrderByDescending(e => e.Created);
                        break;
                }
            }
            return q;
        }

        public IQueryable<int> Recipients()
        {
            return (from msg in DefineModelList()
                    join r in CurrentDatabase.SMSItems
                        on msg.Id equals r.ListID
                    where r.Sent == true
                    where r.PeopleID != null
                    select r.PeopleID.Value).Distinct();
        }
        public Tag TagAll(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            CurrentDatabase.CurrentTagName = tag.Name;
            CurrentDatabase.CurrentTagOwnerId = tag.PersonOwner.PeopleId;

            var q = Recipients();
            var listpeople = q.Select(vv => vv).Distinct();
            CurrentDatabase.TagAll(listpeople, tag);
            return tag;
        }
    }
}
