using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using Newtonsoft.Json;

namespace CmsWeb.Areas.Manage.Models.SMSMessages
{
    public class SmsReplyWordsModel : IDbBinder
    {
        public CMSDataContext CurrentDatabase { get; set; }
        public SmsReplyWordsModel()
        {
        }
        public int GroupId { get; set; }
        public List<SmsActionModel> Actions { get; set; }

        public SmsReplyWordsModel(CMSDataContext db)
        {
            CurrentDatabase = db;
            Actions = new List<SmsActionModel>();
        }
        public IEnumerable<SelectListItem> SmsGroups()
        {
            var q = from c in CurrentDatabase.SMSGroups
                    select new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(select group)" });
            return list;
        }

        public void PopulateActions()
        {
            Actions = new List<SmsActionModel>();
            if (GroupId == -1)
                return;
            string json = CurrentDatabase.SMSGroups.FirstOrDefault(
                v => v.Id == GroupId)?.ReplyWords;
            if(json == null)
                return;
            Actions = JsonConvert.DeserializeObject<List<SmsActionModel>>(json);
            PopulateMetaData();
        }

        public void Save()
        {
            var group = CurrentDatabase.SMSGroups.FirstOrDefault(
                v => v.Id == GroupId);
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
