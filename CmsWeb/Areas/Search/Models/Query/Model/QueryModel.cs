using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Public.Controllers;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public partial class QueryModel : QueryResults
    {
        private static readonly List<CodeValueItem> BitCodes =
            new List<CodeValueItem>
            {
                new CodeValueItem {Id = 1, Value = "True", Code = "T"},
                new CodeValueItem {Id = 0, Value = "False", Code = "F"},
            };
        private static readonly List<CodeValueItem> EqualBitCodes =
            new List<CodeValueItem>
            {
                new CodeValueItem {Id = 1, Value = "True", Code = "T"},
            };

        private string conditionName;
        private FieldClass2 fieldMap;

        public Guid? SelectedId { get; set; }
        public string CodeIdValue { get; set; }

        public QueryModel()
        {
            Db.SetUserPreference("NewCategories", "true");
            ConditionName = "Group";
        }

        public QueryModel(Guid? id)
            : this()
        {
            QueryId = id;
            DbUtil.LogActivity($"Running Query ({id})");
        }

        public string Program { get; set; }
        private int ProgramInt => Program.GetCsvToken().ToInt();

        public string Division { get; set; }
        private int DivisionInt => Division.GetCsvToken().ToInt();

        public string Organization { get; set; }
        private int OrganizationInt => Organization.GetCsvToken().ToInt();

        public string Schedule { get; set; }
        private int ScheduleInt => Schedule.GetCsvToken().ToInt();

        public string Campus { get; set; }
        private int CampusInt => Campus.GetCsvToken().ToInt();
        
        public string OrgType { get; set; }
        private int OrgTypeInt => OrgType.GetCsvToken().ToInt();

        public string OrgType2 { get; set; }
        public string Ministry { get; set; }
        public string SavedQuery { get; set; }
        public string Comparison { get; set; }
        public string OrgName { get; set; }
        public string OrgStatus { get; set; }
        public string OnlineReg { get; set; }

        public bool IsPublic { get; set; }
        public string Days { get; set; }
        public int? Age { get; set; }
        public string Quarters { get; set; }

        public string QuartersLabel
        {
            get { return QuartersVisible ? fieldMap.QuartersTitle : ""; }
        }

        public string View { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string Tags { get; set; }

        [SkipFieldOnCopyProperties]
        public List<string> TagValues
        {
            get { return (Tags ?? "").Split(';').ToList(); }
            set { Tags = string.Join(";", value); }
        }
        [SkipFieldOnCopyProperties]
        public List<string> PmmLabels
        {
            get { return (Tags ?? "").Split(';').ToList(); }
            set { Tags = string.Join(";", value); }
        }

        public List<string> CodeValues
        {
            get { return (CodeIdValue ?? "").Split(';').ToList(); }
            set { CodeIdValue = string.Join(";", value.Where(cc => cc != "multiselect-all")); }
        }

        public string TextValue { get; set; }

        [SkipFieldOnCopyProperties]
        public decimal? NumberValue
        {
            get { return TextValue.ToDecimal(); }
            set { TextValue = value.ToString(); }
        }

        [SkipFieldOnCopyProperties]
        public int? IntegerValue
        {
            get { return TextValue.ToInt2(); }
            set { TextValue = value.ToString(); }
        }

        public DateTime? DateValue { get; set; }

        public string ConditionName
        {
            get { return conditionName; }
            set
            {
                conditionName = value;
                fieldMap = FieldClass2.Fields[value];
            }
        }

        public string ConditionText { get { return fieldMap.Title; } }

        public IEnumerable<CategoryClass2> FieldCategories()
        {
            var q = from c in CategoryClass2.Categories
                    where c.Title != "Grouping"
                    select c;
            return q;
        }

        public Tag TagAllIds()
        {
            var q = DefineModelList();
            var tag = Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_Query);
            Db.TagAll(q, tag);
            return tag;
        }

        public void TagAll(Tag tag = null)
        {
            Db.SetNoLock();
            var q = Db.People.Where(TopClause.Predicate(Db));
            if (TopClause.ParentsOf)
                q = Db.PersonQueryParents(q);
            if (tag != null)
                Db.TagAll(q, tag);
            else
                Db.TagAll(q);
        }

        public void UnTagAll()
        {
            Db.SetNoLock();
            var q = Db.People.Where(TopClause.Predicate(Db));

            if (TopClause.PlusParentsOf)
                q = Db.PersonQueryPlusParents(q);
            else if (TopClause.ParentsOf)
                q = Db.PersonQueryParents(q);

            Db.UnTagAll(q);
        }

        public bool Validate(ModelStateDictionary m)
        {
            DateTime dt = DateTime.MinValue;
            int i = 0;
            if (DaysVisible && !int.TryParse(Days, out i))
                m.AddModelError("Days", "must be integer");
            if (i > 10000)
                m.AddModelError("Days", "days > 10000");
            if (TagsVisible && string.Join(",", Tags).Length > 500)
                m.AddModelError("tagvalues", "too many tags selected");
            if (Comparison == "Contains")
                if (!TextValue.HasValue())
                    m.AddModelError("TextValue", "cannot be empty");
            return m.IsValid;
        }
        public void UpdateCondition()
        {
            this.CopyPropertiesTo(Selected);
            TopClause.Save(Db, increment: true);
        }
        public void EditCondition()
        {
            this.CopyPropertiesFrom(Selected);
        }
    }
}
