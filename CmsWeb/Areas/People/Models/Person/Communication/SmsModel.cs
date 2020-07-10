using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Constants;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class SmsModel : PagedTableModel<SmsViewModel, SmsViewModel>
    {
        public int PeopleId { get; set; }
        public string GroupId { get; set; }
        private int GroupIdInt => GroupId.ToInt();

        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public SmsModel()
        {
            Init();
        }

        public SmsModel(CMSDataContext db) : base(db)
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();
            base.AjaxPager = true;
        }

        public IEnumerable<SelectListItem> PersonSmsGroups()
        {
            var q = from g in CurrentDatabase.SMSGroups
                where !g.IsDeleted
                where g.SMSLists.Any(vv => vv.SMSItems.Any(v => v.PeopleID == PeopleId))
                    where !g.IsDeleted
                    select new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = g.Name
                    };
            var groups = q.ToList();
            groups.Insert(0, new SelectListItem { Text = "(all sms groups)", Value = "0" });
            return groups;
        }

        public IQueryable<SmsViewModel> GetTextMessages()
        {
            var qo = from o in CurrentDatabase.SMSLists
                     join i in CurrentDatabase.SMSItems on o.Id equals i.ListID
                     where o.SendGroupID == GroupIdInt || GroupIdInt == 0
                     where i.PeopleID == PeopleId
                     where i.Sent
                     select new SmsViewModel()
                     {
                         Id = o.Id,
                         IsIncoming = false,
                         ReplyToIncomingId = o.ReplyToId,
                         Date = o.SendAt,
                         Number = o.SMSGroup.Name,
                         Message = o.Message,
                         Sender = o.Person.Name
                     };
            var qi = from r in CurrentDatabase.SmsReceiveds
                     where r.FromPeopleId == PeopleId
                     where r.ToGroupId == GroupIdInt || GroupIdInt == 0
                     select new SmsViewModel()
                     {
                         Id = r.Id,
                         IsIncoming = true,
                         ReplyToIncomingId = null,
                         Date = r.DateReceived,
                         Number = r.FromNumber,
                         Message = r.Body,
                         Sender = r.Person.Name
                     };
            return qo.Union(qi);
        }

        public override IQueryable<SmsViewModel> DefineModelList()
        {
            if (list != null)
                return list;
            return list = GetTextMessages();
        }

        public override IQueryable<SmsViewModel> DefineModelSort(IQueryable<SmsViewModel> q)
        {
            return q.OrderByDescending(vv => vv.Date);
        }

        public override IEnumerable<SmsViewModel> DefineViewList(IQueryable<SmsViewModel> q)
        {
            return q;
        }
    }
}
