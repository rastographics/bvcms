using CmsData;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsWeb.Models;
using Newtonsoft.Json;

namespace CmsWeb.Areas.Setup.Models
{
    public class SmsReplyWordsModel : IDbBinder
    {
        public CMSDataContext CurrentDatabase { get; set; }

        public string Number { get; set; }
        public List<SmsActionModel> Actions { get; set; }

        public SmsReplyWordsModel(CMSDataContext db)
        {
            CurrentDatabase = db;
            Actions = new List<SmsActionModel>();
        }
        public IEnumerable<SelectListItem> SmsNumbers()
        {
            var q = from c in CurrentDatabase.SMSNumbers
                    join g in CurrentDatabase.SMSGroups on c.GroupID equals g.Id
                    select new
                    SelectListItem
                    {
                        Value = c.Number,
                        Text = $"{g.Name} - {c.Number}"
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(select number)" });
            return list;
        }

        public void PopulateNumber()
        {
            Actions = new List<SmsActionModel>();
            if (string.IsNullOrEmpty(Number) || Number == "(select number)")
                return;
            string json = CurrentDatabase.SMSNumbers.FirstOrDefault(
                v => v.Number == Number)?.ReplyWords;
            Actions = JsonConvert.DeserializeObject<List<SmsActionModel>>(json);
            PopulateMetaData();
        }

        public void Save()
        {
            var number = CurrentDatabase.SMSNumbers.FirstOrDefault(
                v => v.Number == Number);
            if (number != null)
                number.ReplyWords = ToString();
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
