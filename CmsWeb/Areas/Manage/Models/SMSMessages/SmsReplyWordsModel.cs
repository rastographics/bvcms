using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using CmsData;
using CmsWeb.Models;
using Newtonsoft.Json;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Models.SmsMessages
{
    public class SmsReplyWordsModel : IDbBinder
    {
        public CMSDataContext CurrentDatabase { get; set; }

        public SmsReplyWordsModel()
        {
        }

        [Display(Name = "Group")]
        public string GroupId { get; set; }

        public int GroupIdInt
        {
            get => GroupId.ToInt();
            set => GroupId = value.ToString();
        }
        public List<SmsReplyWordsActionModel> Actions { get; set; }

        public SmsReplyWordsModel(CMSDataContext db)
        {
            CurrentDatabase = db;
            Actions = new List<SmsReplyWordsActionModel>();
        }

        private List<int> UserInGroups()
        {
            var manageSmsUser = CurrentDatabase.CurrentUser.InRole("ManageSms");
            return (from gm in CurrentDatabase.SMSGroupMembers
                    where manageSmsUser || gm.User.PeopleId == CurrentDatabase.UserPeopleId
                    select gm.GroupID).ToList();
        }

        public IEnumerable<SelectListItem> SmsGroups()
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
            var list = q.ToList();
            if (list.Count == 1)
            {
                GroupId = list[0].Value;
                PopulateActions();
            }
            else
                list.Insert(0, new SelectListItem {Text = "(select group)"});
            return list;
        }

        public void PopulateActions()
        {
            Actions = new List<SmsReplyWordsActionModel>();
            if (GroupIdInt == -1)
                return;
            string json = CurrentDatabase.SMSGroups.FirstOrDefault(
                v => v.Id == GroupIdInt)?.ReplyWords;
            if (json == null)
                return;
            Actions = JsonConvert.DeserializeObject<List<SmsReplyWordsActionModel>>(json) ?? new List<SmsReplyWordsActionModel>();
            PopulateMetaData();
        }

        public void Save()
        {
            var group = CurrentDatabase.SMSGroups.FirstOrDefault(
                v => v.Id == GroupIdInt);
            if (group != null)
                group.ReplyWords = ToString();
            CurrentDatabase.SubmitChanges();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(Actions,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        public void PopulateMetaData()
        {
            foreach (var a in Actions)
            {
                a.PopulateMetaData();
            }
        }
    }
}
