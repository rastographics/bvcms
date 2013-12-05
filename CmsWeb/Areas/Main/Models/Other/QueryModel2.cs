using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CmsData;
using CmsWeb.Code;
using Newtonsoft.Json;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public partial class QueryModel2
    {
        private CMSDataContext Db;
        public QueryModel2()
        {
            Db = DbUtil.Db;
        }
        private Condition topclause;
        [JsonIgnore]
        public Condition TopClause
        {
            get
            {
                var tc =  topclause ?? (topclause = QueryId == null
                    ? Db.FetchLastQuery()
                    : Db.LoadCopyOfExistingQuery(QueryId.Value));
                QueryId = tc.Id;
                ConditionName = "Group";
                return tc;
            }
        }
        public string Description { get; set; }
        public Guid? QueryId { get; set; }
        public void LoadScratchPad()
        {
        }

        private string conditionName;
        public string ConditionName
        {
            get { return conditionName; }
            set
            {
                conditionName = value;
                fieldMap = FieldClass.Fields[value];
            }
        }
        public Guid? SelectedId { get; set; }
        public bool CodesVisible { get; set; }
        public List<SelectListItem> TagData { get; set; }
        public List<SelectListItem> PmmLabelData { get; set; }
        public IEnumerable<SelectListItem> CodeData { get; set; }
        public List<SelectListItem> CompareData { get; set; }
        public List<SelectListItem> ProgramData { get; set; }
        public List<SelectListItem> DivisionData { get; set; }
        public List<SelectListItem> OrganizationData { get; set; }
        public List<SelectListItem> ViewData { get; set; }
        public int? Program { get; set; }
        public int? Division { get; set; }
        public int? Organization { get; set; }
        public int? Schedule { get; set; }
        public int? Campus { get; set; }
        public int? OrgType { get; set; }
        public string Days { get; set; }
        public string Age { get; set; }
        public string Quarters { get; set; }
        public string QuartersLabel { get; set; }
        public string View { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Comparison { get; set; }
        public string[] Tags { get; set; }
        public string[] PmmLabels { get; set; }
        public int? Ministry { get; set; }
        public string SavedQueryDesc { get; set; }
        public bool IsPublic { get; set; }
        public string CodeValue { get; set; }
        public string[] CodeValues { get; set; }
        public string TextValue { get; set; }
        public string DateValue { get; set; }
        public string NumberValue { get; set; }
        public string IntegerValue { get; set; }
        public bool UpdateEnabled { get; set; }
        public bool AddToGroupEnabled { get; set; }
        public bool AddEnabled { get; set; }
        public bool RemoveEnabled { get; set; }
        public bool SelectMultiple { get; set; }
        public string ConditionText { get { return fieldMap.Title; } }
        private static List<CodeValueItem> BitCodes =
            new List<CodeValueItem> 
            { 
                new CodeValueItem { Id = 1, Value = "True", Code = "T" }, 
                new CodeValueItem { Id = 0, Value = "False", Code = "F" }, 
            };
        internal FieldClass fieldMap;

        DateTime? DateParse(string s)
        {
            DateTime dt;
            if (DateTime.TryParse(s, out dt))
                return dt;
            return null;
        }

        public string DateString(DateTime? dt)
        {
            if (dt.HasValue)
                return dt.Value.ToShortDateString();
            return "";
        }

//        public void UpdateCondition(QueryBuilderClause c)
//        {
//        }

        [JsonIgnore]
        public Condition Selected
        {
            get
            {
                var gid = SelectedId ?? Guid.Empty;
                return TopClause.AllConditions[gid];
            }
        }
        public void EditCondition()
        {
            var c = Selected;
            if (c == null)
                return;
            CompareData = Comparisons().ToList();
            ConditionName = c.FieldInfo.Name;
            Comparison = c.Comparison;

            if (QuartersVisible)
                QuartersLabel = fieldMap.QuartersTitle;
            if (TagsVisible)
            {
                var cv = new CodeValueModel();
                TagData = CodeValueModel.ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Code");
            }
            if (PmmLabelsVisible)
            {
                PmmLabelData = CodeValueModel.ConvertToSelect(CodeValueModel.PmmLabels(), "Code");
            }

            var cvctl = new CodeValueModel();
            switch (fieldMap.Type)
            {
                case FieldType.Bit:
                case FieldType.NullBit:
                    CodeData = CodeValueModel.ConvertToSelect(BitCodes, fieldMap.DataValueField);
                    break;
                case FieldType.NullCode:
                case FieldType.Code:
                case FieldType.CodeStr:
                    if (fieldMap.DataSource == "ExtraValues")
                        CodeData = StandardExtraValues.ExtraValueCodes();
                    else if (fieldMap.DataSource == "Campuses")
                        CodeData = Campuses();
						  else if( fieldMap.DataSource == "Activities" )
						  {
							  var ret = ( from e in DbUtil.Db.CheckInActivities select e.Activity ).Distinct();
							  CodeData = from e in ret
								  select new SelectListItem() {Text = e, Value = e};
						  }
						  else
							  CodeData = CodeValueModel.ConvertToSelect( Util.CallMethod( cvctl, fieldMap.DataSource ), fieldMap.DataValueField );
		            break;
                case FieldType.DateField:
                    CodeData = CodeValueModel.ConvertToSelect(Util.CallMethod(cvctl, fieldMap.DataSource), fieldMap.DataValueField);
                    break;
            }

            if (fieldMap.Type == FieldType.Group)
            {
                CompareData = Comparisons().ToList();
                UpdateEnabled = c.IsGroup;
            }
            else
            {
                UpdateEnabled = !c.IsGroup && !c.IsFirst;
                AddToGroupEnabled = c.IsGroup;
                AddEnabled = !c.IsFirst;
                RemoveEnabled = !c.IsFirst;
            }
            switch (c.FieldInfo.Type)
            {
                case FieldType.String:
                case FieldType.StringEqual:
                case FieldType.StringEqualOrStartsWith:
                    TextValue = c.TextValue;
                    break;
                case FieldType.Integer:
                case FieldType.IntegerSimple:
                case FieldType.IntegerEqual:
                case FieldType.NullInteger:
                    IntegerValue = c.TextValue;
                    break;
                case FieldType.Number:
                case FieldType.NumberLG:
                case FieldType.NullNumber:
                    NumberValue = c.TextValue;
                    break;
                case FieldType.Date:
                case FieldType.DateSimple:
                    DateValue = DateString(c.DateValue);
                    break;
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                case FieldType.DateField:
                case FieldType.Bit:
                case FieldType.NullBit:
                    CodeValue = c.CodeIdValue;
                    if (c.HasMultipleCodes && CodeValue.HasValue())
                    {
                        CodeValues = c.CodeIdValue.Split(';');
                        foreach (var i in CodeData)
                            i.Selected = CodeValues.Contains(i.Value);
                    }
                    break;
            }
            Program = c.Program;
            DivisionData = Divisions(Program).ToList();
            Division = c.Division;
            OrganizationData = Organizations(Division).ToList();
            Organization = c.Organization;
            Schedule = c.Schedule;
            Campus = c.Campus;
            OrgType = c.OrgType;
            StartDate = DateString(c.StartDate);
            EndDate = DateString(c.EndDate);
            SelectMultiple = c.HasMultipleCodes;
            AddToGroupEnabled = c.IsGroup;
            AddEnabled = !c.IsFirst;
            RemoveEnabled = !c.IsFirst;
            Days = c.Days.ToString();
            Age = c.Age.ToString();
            Quarters = c.Quarters;
            if (TagsVisible)
            {
                if (c.Tags != null)
                    Tags = c.Tags.Split(';');
                var cv = new CodeValueModel();
                TagData = CodeValueModel.ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Code");
                foreach (var i in TagData)
                    i.Selected = Tags.Contains(i.Value);
            }
            if (PmmLabelsVisible)
            {
                if (c.Tags != null)
                    PmmLabels = c.Tags.Split(',').Select(vv => vv).ToArray();
                var cv = new CodeValueModel();
                PmmLabelData = CodeValueModel.ConvertToSelect(CodeValueModel.PmmLabels(), "Id");
                if(PmmLabels != null)
                    foreach (var i in PmmLabelData)
                        i.Selected = PmmLabels.Contains(i.Value);
            }
            if (MinistryVisible)
                Ministry = c.Program;
            //SavedQueryDesc = c.SavedQueryIdDesc;
        }

        public void SetCodes()
        {
            SelectMultiple = Comparison.EndsWith("OneOf");
        }

//        public void NewCondition(QueryBuilderClause gc, int order)
//        {
//        }

        public string SaveQuery()
        {
            return null;
        }

        public void AddConditionToGroup()
        {
        }

        public void AddConditionAfterCurrent()
        {
        }

        public void DeleteCondition()
        {
        }

        public void UpdateCondition()
        {
            var c = Selected;
            if (c == null)
                return;
            UpdateCondition(c);
        }
        private void UpdateCondition(Condition c)
        {
            c.ConditionName = conditionName;
            c.Comparison = Comparison;
            switch (c.FieldInfo.Type)
            {
                case FieldType.String:
                case FieldType.StringEqual:
                case FieldType.StringEqualOrStartsWith:
                    c.TextValue = TextValue;
                    break;
                case FieldType.Integer:
                case FieldType.IntegerSimple:
                case FieldType.IntegerEqual:
                case FieldType.NullInteger:
                    c.TextValue = IntegerValue;
                    break;
                case FieldType.Number:
                case FieldType.NumberLG:
                case FieldType.NullNumber:
                    c.TextValue = NumberValue;
                    break;
                case FieldType.Date:
                case FieldType.DateSimple:
                    c.DateValue = DateParse(DateValue);
                    break;
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                case FieldType.DateField:
                case FieldType.Bit:
                case FieldType.NullBit:
                    if (c.HasMultipleCodes && CodeValues != null)
                        c.CodeIdValue = string.Join(";", CodeValues);
                    else
                        c.CodeIdValue = CodeValue;
                    break;
            }
            c.Program = Program ?? 0;
            c.Division = Division ?? 0;
            c.Organization = Organization ?? 0;
            if (MinistryVisible)
                c.Program = Ministry ?? 0;
            c.Schedule = Schedule ?? 0;
            c.Campus = Campus ?? 0;
            c.OrgType = OrgType ?? 0;
            c.StartDate = DateParse(StartDate);
            c.EndDate = DateParse(EndDate);
            c.Days = Days.ToInt();
            c.Age = Age.ToInt();
            c.Quarters = Quarters;
            if (Tags != null)
                c.Tags = string.Join(";", Tags);
            else if (PmmLabels != null)
                c.Tags = string.Join(",", PmmLabels);
            else
                c.Tags = null;
            c.Description = SavedQueryDesc;
            Db.SubmitChanges();
        }

        public void CopyAsNew()
        {
        }

        public void InsertGroupAbove()
        {
        }

        public IEnumerable<SelectListItem> Comparisons()
        {
            return from c in CompareClass.Comparisons
                   where c.FieldType == fieldMap.Type
                   select new SelectListItem { Text = c.CompType.ToString(), Value = c.CompType.ToString() };
        }
        public IEnumerable<SelectListItem> Schedules()
        {
            var q = from o in DbUtil.Db.Organizations
                    let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                    where sc != null
                    group o by new { ScheduleId = sc.ScheduleId ?? 10800, sc.MeetingTime } into g
                    orderby g.Key.ScheduleId
                    select new SelectListItem
                    {
                        Value = g.Key.ScheduleId.ToString(),
                        Text = DbUtil.Db.GetScheduleDesc(g.Key.MeetingTime)
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(None)", Value = "-1" });
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public IEnumerable<SelectListItem> Campuses()
        {
            var q = from o in DbUtil.Db.Organizations
                    where o.CampusId != null
                    group o by o.CampusId into g
                    orderby g.Key
                    select new SelectListItem
                    {
                        Value = g.Key.ToString(),
                        Text = g.First().Campu.Description
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(None)", Value = "-1" });
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public IEnumerable<SelectListItem> OrgTypes()
        {
            var q = from t in Db.OrganizationTypes
                    orderby t.Code
                    select new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Description
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public IEnumerable<SelectListItem> Programs()
        {
            var q = from t in Db.Programs
                    orderby t.Name
                    select new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Name
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public IEnumerable<SelectListItem> Divisions(int? progid)
        {
            var q = from div in Db.Divisions
                    where div.ProgDivs.Any(d => d.ProgId == progid)
                    orderby div.Name
                    select new SelectListItem
                    {
                        Value = div.Id.ToString(),
                        Text = div.Name
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public IEnumerable<SelectListItem> Organizations(int? divid)
        {
            var roles = Db.CurrentRoles();
            var q = from ot in Db.DivOrgs
                where ot.Organization.LimitToRole == null || roles.Contains(ot.Organization.LimitToRole)
                    where ot.DivId == (divid ?? 0)
                    && (SqlMethods.DateDiffMonth(ot.Organization.OrganizationClosedDate, Util.Now) < 14
                        || ot.Organization.OrganizationStatusId == 30)
                    where (Util2.OrgMembersOnly == false && Util2.OrgLeadersOnly == false) || (ot.Organization.SecurityTypeId != 3)
                    orderby ot.Organization.OrganizationStatusId, ot.Organization.OrganizationName
                    select new SelectListItem
                    {
                        Value = ot.OrgId.ToString(),
                        Text = CmsData.Organization.FormatOrgName(ot.Organization.OrganizationName,
                           ot.Organization.LeaderName, ot.Organization.Location)
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }

        private int level;
        public IEnumerable<QueryClauseDisplay2> ConditionList()
        {
            level = 0;
            return ClauseAndSubs(new List<QueryClauseDisplay2>(), TopClause);
        }
        private List<QueryClauseDisplay2> ClauseAndSubs(List<QueryClauseDisplay2> list, Condition qc)
        {
            list.Add(new QueryClauseDisplay2 { Level = (level * 25), Clause = qc });
            level++;
            foreach (var c in qc.Conditions)
                list = ClauseAndSubs(list, c);
            level--;
            return list;
        }

        public IEnumerable<CategoryClass> FieldCategories()
        {
            yield break;
        }

        public IEnumerable<SelectListItem> SavedQueries()
        {
            yield break;
        }

        public IEnumerable<SelectListItem> Ministries()
        {
            yield break;
        }

//        public int FetchCount()
//        {
//            return 0;
//        }

        public List<CmsWeb.Models.PeopleInfo> Results { get; set; }
        public void PopulateResults()
        {
        }

//        public IEnumerable<CmsWeb.Models.PeopleInfo> FetchPeopleList()
//        {
//            yield break;
//        }

        public Tag TagAllIds()
        {
            return null;
        }

//        public IQueryable<Person> PersonQuery()
//        {
//            return null;
//        }

        public void TagAll(Tag tag = null)
        {
        }

        public void UnTagAll()
        {
        }

//        public IEnumerable<CmsWeb.Models.PeopleInfo> FetchPeopleList(IQueryable<Person> query)
//        {
//            yield break;
//        }

//        public IQueryable<Person> ApplySort(IQueryable<Person> q)
//        {
//            return null;
//        }

        public Dictionary<string, string> Errors { get; set; }
        public bool ShowResults { get; set; }
        public string Sort { get; set; }
        public string Direction { get; set; }
        public int? Page { get; set; }
        public int StartRow { get; set; }
        public int? PageSize { get; set; }
        public int Count { get; private set; }
        public PagerModel pagerModel()
        {
            return null;
        }

//        public PagerModel pagerModel()
//        {
//            return null;
//        }
    }
    public class QueryClauseDisplay2
    {
        public Unit Level { get; set; }
        public Condition Clause;
    }
}