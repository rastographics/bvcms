using CmsData;
using CmsWeb.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public partial class QueryModel : QueryResults
    {
        private static readonly List<CodeValueItem> BitCodes = new List<CodeValueItem>
            {
                new CodeValueItem {Id = 1, Value = "True", Code = "T"},
                new CodeValueItem {Id = 0, Value = "False", Code = "F"},
            };
        private static readonly List<CodeValueItem> EqualBitCodes = new List<CodeValueItem>
            {
                new CodeValueItem {Id = 1, Value = "True", Code = "T"},
            };

        private string conditionName;
        private FieldClass fieldMap;

        public Guid? SelectedId { get; set; }
        public string CodeIdValue { get; set; }

        public QueryModel()
        {
            ConditionName = "Group";
        }

        public QueryModel(CMSDataContext db)
            : this()
        {
            Db = db;
        }

        public QueryModel(Guid? id, CMSDataContext db)
            : this(db)
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

        public string OrgStatus { get; set; }
        private int OrgStatusInt => OrgStatus.GetCsvToken().ToInt();

        public string Ministry { get; set; }
        private int MinistryInt => Ministry.GetCsvToken().ToInt();

        public string OnlineReg { get; set; }
        private int OnlineRegInt => OnlineReg.GetCsvToken().ToInt();

        public string OrgType2 { get; set; }
        public string SavedQuery { get; set; }
        public string Comparison { get; set; }
        public string OrgName { get; set; }

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
                fieldMap = FieldClass.Fields[value];
            }
        }

        public string ConditionText { get { return fieldMap.Name; } }

        public IEnumerable<CategoryClass> FieldCategories()
        {
            var q = from c in CategoryClass.Categories
                    where c.Title != "Grouping"
                    where c.Name != "OrgFilter"
                    select c;
            return q;
        }

        [Obsolete]
        public Tag TagAllIds()
        {
            return TagAll();
        }

        /// <summary>
        /// Add the specified tag to the result of this query
        /// </summary>
        /// <param name="tagname">The name of the tag to assign to the result of this QueryModel. Uses the user's session id as a default value if nothing is supplied.</param>
        public Tag TagAll(string tagname = "")
        {
            if (string.IsNullOrEmpty(tagname))
            {
                tagname = Util.SessionId; // not specifying an explicit name, so use the session id as a default
            }
            var tag = Db.FetchOrCreateTag(tagname, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
            return TagAll(tag);
        }

        /// <summary>
        /// Add the specified tag to the result of this query
        /// </summary>
        /// <param name="tag">An object instance of the tag to append</param>
        public Tag TagAll(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            Db.CurrentTagName = tag.Name;
            Db.CurrentTagOwnerId = tag.PersonOwner.PeopleId;

            var q = DefineModelList();

            var newTags = new List<TagPerson>();

            foreach (var p in q.Where(p => !p.Tags.Any(t => t.Id == tag.Id)).ToList())
            {
                newTags.Add(new TagPerson { PeopleId = p.PeopleId, Id = tag.Id, DateCreated = DateTime.Now });
            }

            Db.TagPeople.InsertAllOnSubmit(newTags);
            Db.SubmitChanges();

            return tag;
        }

        /// <summary>
        /// Removes the specified tag from the results of this query
        /// </summary>
        /// <param name="tagname">The name of the tag to remove from the result of this QueryModel. Uses the current tag name as a default value if nothing is supplied</param>
        public Tag UntagAll(string tagname = "")
        {
            if (string.IsNullOrEmpty(tagname))
            {
                tagname = Util2.CurrentTag; // not specifying an explicit name, so use the current tag name as default
            }
            var tag = Db.FetchOrCreateTag(tagname, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
            return UntagAll(tag);
        }

        /// <summary>
        /// Removes the specified tag from the results of this query
        /// </summary>
        /// <param name="tag">An object instance of the tag to remove</param>
        public Tag UntagAll(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            Db.CurrentTagName = tag.Name;
            Db.CurrentTagOwnerId = tag.PersonOwner.PeopleId;

            var q = DefineModelList();
            var removedTags = new List<TagPerson>();

            foreach (var p in q.Where(p => p.Tags.Any(t => t.Id == tag.Id)).ToList())
            {
                removedTags.Add(new TagPerson { PeopleId = p.PeopleId, Id = tag.Id });
            }

            Db.TagPeople.AttachAll(removedTags);
            Db.TagPeople.DeleteAllOnSubmit(removedTags);
            Db.SubmitChanges();

            return tag;
        }

        public bool Validate(ModelStateDictionary m)
        {
            DateTime dt = DateTime.MinValue;
            int i = 0;
            if (DaysVisible && !int.TryParse(Days, out i))
            {
                if (new[] { "IsFamilyGiver", "IsFamilyPledger" }.Contains(ConditionName) == false)
                {
                    m.AddModelError("Days", "must be integer");
                }
            }
            if (i > 10000)
            {
                m.AddModelError("Days", "days > 10000");
            }

            if (TagsVisible && string.Join(",", Tags).Length > 500)
            {
                m.AddModelError("tagvalues", "too many tags selected");
            }

            if (Comparison == "Contains")
            {
                if (!TextValue.HasValue())
                {
                    m.AddModelError("TextValue", "cannot be empty");
                }
            }

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
        public bool UseEmployerNotTeacher => Db?.Setting("UseEmployerNotTeacher") ?? false;
        public bool ShowAltNameOnSearchResults => Db?.Setting("ShowAltNameOnSearchResults") ?? false;
    }
}
