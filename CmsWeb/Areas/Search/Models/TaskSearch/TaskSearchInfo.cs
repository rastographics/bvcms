using System;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Code;
using Newtonsoft.Json;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    [Serializable]
    public class TaskSearchInfo
    {
        public string Delegate { get; set; }
        public string Originator { get; set; }
        public string Owner { get; set; }
        public string About { get; set; }
        public bool Archived { get; set; }
        public CodeInfo TaskStatus { get; set; }
        public DateTime? EndDt { get; set; }
        public int? Lookback { get; set; }
        public bool IsPrivate { get; set; }
        public bool ShowNotes { get; set; }
        public bool ExcludeNewPerson { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }

        public TaskSearchInfo()
        {
            if (TaskStatus == null)
                TaskStatus = new CodeInfo("TaskStatus");
        }

        private static string NewTaskSearchString => JsonConvert.SerializeObject(new TaskSearchInfo());

        private const string StrTaskSearch = "TaskSearch";

        internal void GetPreference()
        {
            var os = JsonConvert.DeserializeObject<TaskSearchInfo>(
                DbUtil.Db.UserPreference(StrTaskSearch, NewTaskSearchString));
            if (os != null)
                this.CopyPropertiesFrom(os);
        }

        internal void SavePreference()
        {
            DbUtil.Db.SetUserPreference(StrTaskSearch,
                JsonConvert.SerializeObject(this));
        }

        internal void ClearPreference()
        {
            DbUtil.Db.SetUserPreference(StrTaskSearch, NewTaskSearchString);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        public class OptionInfo
        {
            public int Status { get; set; }
            public bool Archived { get; set; }
            public bool ExcludeNewPerson { get; set; }
            public int? Lookback { get; set; }
            public DateTime? EndDt { get; set; }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        public OptionInfo GetOptions()
        {
            return new OptionInfo
            {
                Status = TaskStatus.Value.ToInt(),
                Archived = Archived,
                ExcludeNewPerson = ExcludeNewPerson,
                Lookback = Lookback,
                EndDt = EndDt,
            };
        }

        public static OptionInfo GetOptions(string s) => JsonConvert.DeserializeObject<OptionInfo>(s);

        public string OnePersonFilter()
        {
            return (Originator.HasValue() ? "Ori" : "")
                   + (About.HasValue() ? "Abo" : "")
                   + (Owner.HasValue() ? "Own" : "")
                   + (Delegate.HasValue() ? "Del" : "");
        }

        public HtmlString Icon(string which)
        {
            var i = new TagBuilder("i");
            i.AddCssClass("fa");
            i.AddCssClass("fa-lg");
            if (OnePersonFilter() == which)
            {
                i.AddCssClass("fa-close");
                i.AddCssClass("red");
            }
            else
                i.AddCssClass("fa-filter");
            return new HtmlString(i.ToString());
        }
    }
}