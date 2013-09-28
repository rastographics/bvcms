using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using MoreLinq;
using NPOI.POIFS.Properties;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public partial class QueryModel : QueryResults
    {
        public Guid? SelectedId { get; set; }

        [UIHint("DropDown")] public int? Program { get; set; }
        [UIHint("DropDown")] public int? Division { get; set; }
        [UIHint("DropDown")] public int? Organization { get; set; }
        [UIHint("DropDown")] public string Schedule { get; set; }
        [UIHint("DropDown")] public string Campus { get; set; }
        [UIHint("DropDown")] public string OrgType { get; set; }
        [UIHint("DropDown")] public string Ministry { get; set; }
        [UIHint("DropDown")] public string SavedQuery { get; set; }
        [UIHint("DropDown")] public string Comparison { get; set; }

        public bool IsPublic { get; set; }
        public string Days { get; set; }
        public int? Age { get; set; }
        public string Quarters { get; set; }
        public string QuartersLabel { get; set; }
        public string View { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string[] Tags { get; set; }
        public string[] PmmLabels { get; set; }

        public string CodeValue { get; set; }
        public string[] CodeValues { get; set; }

        public string TextValue { get; set; }
        public DateTime? DateValue { get; set; }
        public decimal? NumberValue { get; set; }
        public int? IntegerValue { get; set; }

        public bool HasMultipleCodes { get; set; }

        public List<SelectListItem> TagData { get; set; }
        public List<SelectListItem> PmmLabelData { get; set; }

        public QueryModel()
        {
            Db.SetUserPreference("NewCategories", "true");
            ConditionName = "Group";
        }

        private static List<CodeValueItem> BitCodes =
            new List<CodeValueItem> 
            { 
                new CodeValueItem { Id = 1, Value = "True", Code = "T" }, 
                new CodeValueItem { Id = 0, Value = "False", Code = "F" }, 
            };

        private FieldClass fieldMap;
        private string _ConditionName;
        public string ConditionName
        {
            get { return _ConditionName; }
            set
            {
                _ConditionName = value;
                fieldMap = FieldClass.Fields[value];
            }
        }
        public string ConditionText { get { return fieldMap.Title; } }

        public void SetCodes()
        {
            SetVisibility();
            HasMultipleCodes = Comparison.EndsWith("OneOf");
        }
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
            if (TopClause == null)
                LoadQuery();
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
            if (TopClause == null)
                LoadQuery();
            Db.SetNoLock();
            var q = Db.People.Where(TopClause.Predicate(Db));
            if (TopClause.ParentsOf)
                q = Db.PersonQueryParents(q);
            Db.UnTagAll(q);
        }
        public bool Validate(ModelStateDictionary m)
        {
            SetVisibility();
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
    }
}