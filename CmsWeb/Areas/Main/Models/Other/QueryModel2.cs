using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
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
            Db.SetUserPreference("NewCategories", "false");
            ConditionName = "Group";
            Direction = "asc";
            TagTypeId = DbUtil.TagTypeId_Personal;
            TagName = Util2.CurrentTagName;
            TagOwner = Util2.CurrentTagOwnerId;
        }

        private Condition topclause;
        [JsonIgnore]
        public Condition TopClause
        {
            get
            {
                if (topclause != null)
                    return topclause;
                topclause = topclause ?? (topclause = QueryId == null
                    ? Db.FetchLastQuery()
                    : Db.LoadCopyOfExistingQuery(QueryId.Value));
                return topclause;
            }
            set
            {
                topclause = value;
                QueryId = value.Id;
            }
        }
        public string Description { get { return topclause.Description; } }
        public string SaveToDescription { get { return topclause.PreviousName ?? topclause.Description; } }
        public Guid? QueryId { get; set; }

        private string conditionName;
        public string ConditionName
        {
            get { return conditionName; }
            set
            {
                conditionName = value;
                fieldMap = FieldClass2.Fields[value];
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
        public bool SelectMultiple { get { return Comparison != null && Comparison.EndsWith("OneOf"); } }
        public string ConditionText { get { return fieldMap.Title; } }
        private static List<CodeValueItem> BitCodes =
            new List<CodeValueItem> 
            { 
                new CodeValueItem { Id = 1, Value = "True", Code = "T" }, 
                new CodeValueItem { Id = 0, Value = "False", Code = "F" }, 
            };
        internal FieldClass2 fieldMap;

        public static string IdCode(object items, int id)
        {
            var list = items as IEnumerable<CodeValueItem>;
            var ret = (from v in list
                       where v.Id == id
                       select v.IdCode).Single();
            return ret;
        }
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
                if (dt.Value.TimeOfDay.Ticks > 0)
                    return dt.FormatDateTm();
                else
                    return dt.Value.ToShortDateString();
            return "";
        }
        [JsonIgnore]
        public Condition Selected
        {
            get
            {
                var gid = SelectedId ?? Guid.Empty;
                if(TopClause.AllConditions.ContainsKey(gid))
                    return TopClause.AllConditions[gid];
                return null;
            }
        }
        public void SetVisibility()
        {
            CodeData = null;
            ConditionName = ConditionName;
            CompareData = Comparisons().ToList();
            if (QuartersVisible)
                QuartersLabel = fieldMap.QuartersTitle;
            if (TagsVisible)
            {
                var cv = new CodeValueModel();
                TagData = CodeValueModel.ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Code");
            }
            if (PmmLabelsVisible)
            {
                var cv = new CodeValueModel();
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
            var cc = Selected;
            if (cc == null)
                return;

            if (fieldMap.Type == FieldType.Group)
            {
                CompareData = Comparisons().ToList();
                UpdateEnabled = cc.IsGroup;
                return;
            }
            UpdateEnabled = !cc.IsGroup && !cc.IsFirst;
            AddToGroupEnabled = cc.IsGroup;
            AddEnabled = !cc.IsFirst;
            RemoveEnabled = !cc.IsFirst;
        }
        public void EditCondition()
        {
            var c = Selected;
            if (c == null)
                return;
            SetVisibility();

            ConditionName = c.FieldInfo.Name;
            CompareData = Comparisons().ToList();
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
                    {
                        CodeData = StandardExtraValues.ExtraValueCodes();
                    }
                    else if (fieldMap.DataSource == "FamilyExtraValues")
                        CodeData = StandardExtraValues.FamilyExtraValueCodes();
                    else if (fieldMap.DataSource == "Campuses")
                        CodeData = Campuses();
                    else if (fieldMap.DataSource == "Activities")
                    {
                        var ret = (from e in DbUtil.Db.CheckInActivities select e.Activity).Distinct();
                        CodeData = from e in ret
                                   select new SelectListItem() { Text = e, Value = e };
                    }
                    else
                        CodeData = CodeValueModel.ConvertToSelect(Util.CallMethod(cvctl, fieldMap.DataSource), fieldMap.DataValueField);
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
                if (PmmLabels != null)
                    foreach (var i in PmmLabelData)
                        i.Selected = PmmLabels.Contains(i.Value);
            }
            if (MinistryVisible)
                Ministry = c.Program;
            //SavedQueryDesc = c.SavedQueryIdDesc;
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
            TopClause.Save(Db, increment: true);
        }

        public string SaveQuery()
        {
            var isStatusFlag = Regex.IsMatch(SavedQueryDesc, @"\AF\d\d:", RegexOptions.IgnoreCase);
            if (isStatusFlag)
            {
                var prefix = SavedQueryDesc.Substring(0, 4);
                // see if there is a status flag query with this flag number already existing
                var exis = Db.Queries.FirstOrDefault(c => c.Name.StartsWith(prefix));
                if (exis != null)
                {
                    if (exis.Owner != Util.UserName)
                        return "error: StatusFlag {0} already exists by owner '{1}'".Fmt(prefix, exis.Owner);
                    if (exis.Name != SavedQueryDesc)
                        return "error: StatusFlag {0} already exists with a different name '{1}'".Fmt(prefix, exis.Name);
                    exis.Text = TopClause.ToXml();
                    exis.Ispublic = IsPublic;
                    return "";
                }
            }
            TopClause.IsPublic = IsPublic;
            TopClause.Description = SavedQueryDesc;
            TopClause.Save(Db, increment: true);
            return "";
        }
        private Condition NewCondition(Condition gc)
        {
            if (gc == null)
                return null;
            var c = gc.AddNewClause();
            UpdateCondition(c);
            return c;
        }
        public void AddConditionToGroup()
        {
            if(NewCondition(Selected) != null)
                TopClause.Save(Db);
        }
        public void AddConditionAfterCurrent()
        {
            var c = Selected;
            if (c == null)
                return;
            if(NewCondition(Selected.Parent) != null)
                TopClause.Save(Db);
        }
        public void DeleteCondition()
        {
            var c = Selected;
            if (c == null)
                return;
            SelectedId = c.Parent.Id;
            c.DeleteClause();
            SetVisibility();
            TopClause.Save(Db, increment: true);
        }
        public void InsertGroupAbove()
        {
            var g = new Condition
            {
                Id = Guid.NewGuid(),
                ConditionName = QueryType.Group.ToString(),
                Comparison = CompareType.AnyTrue.ToString(),
                AllConditions = Selected.AllConditions
            };
            if (Selected.IsFirst)
            {
                Selected.ParentId = g.Id;
                g.ParentId = null;
            }
            else
            {
                var list = Selected.Parent.Conditions.Where(cc => cc.Order >= Selected.Order).ToList();
                g.ParentId = Selected.ParentId;
                foreach (var c in list)
                    c.ParentId = g.Id;
                g.Order = Selected.MaxClauseOrder();
            }
            Selected.AllConditions.Add(g.Id, g);
            if (g.IsFirst)
            {
                // g will now becojme the new TopClause
                g.Description = TopClause.Description;

                // swap TopClauseId with the new GroupId so the saved query will have the same id
                var tcid = TopClause.Id;
                var gid = g.Id;
                var conditions = TopClause.Conditions.ToList();
                TopClause.Id = gid;
                foreach (var c in conditions)
                    c.ParentId = gid;
                g.Id = tcid;
                TopClause.ParentId = g.Id;
                TopClause = g;
                TopClause.Save(Db);
            }
            TopClause.Save(Db, increment: true);
            //            var cc = Db.LoadQueryById(SelectedId);
            //            var g = new QueryBuilderClause();
            //            g.SetQueryType(QueryType.Group);
            //            g.SetComparisonType(CompareType.AllTrue);
            //            g.ClauseOrder = cc.ClauseOrder;
            //            if (cc.IsFirst)
            //                cc.Parent = g;
            //            else
            //            {
            //                var currParent = cc.Parent;
            //                // find all clauses from cc down at same level
            //                var q = from c in cc.Parent.Clauses
            //                        orderby c.ClauseOrder
            //                        where c.ClauseOrder >= cc.ClauseOrder
            //                        select c;
            //                foreach (var c in q)
            //                    c.Parent = g;   // change to new parent
            //                g.Parent = currParent;
            //            }
            //            if (cc.SavedBy.HasValue())
            //            {
            //                g.SavedBy = Util.UserName;
            //                g.Description = cc.Description;
            //                g.CreatedOn = cc.CreatedOn;
            //                cc.IsPublic = false;
            //                cc.Description = null;
            //                cc.SavedBy = null;
            //            }
            //            Db.SubmitChanges();
            //            if (g.IsFirst)
            //            {
            //                Qb = g;
            //                QueryId = g.QueryId;
            //            }
        }

        public IEnumerable<SelectListItem> Comparisons()
        {
            if (!ConditionName.HasValue())
                ConditionName = "Group";
            return from c in CompareClass2.Comparisons
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

        public IEnumerable<CategoryClass2> FieldCategories()
        {
            var q = from c in CategoryClass2.Categories
                    where c.Title != "Grouping"
                    select c;
            return q;
        }
        public List<SelectListItem> SavedQueries()
        {
            var cv = new CodeValueModel();
            return CodeValueModel.ConvertToSelect(cv.UserQueries(), "Code");
        }
        public List<SelectListItem> Ministries()
        {
            var q = from t in Db.Ministries
                    orderby t.MinistryDescription
                    select new SelectListItem
                    {
                        Value = t.MinistryId.ToString(),
                        Text = t.MinistryName
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }

        private int count;
        private int TagTypeId { get; set; }
        private string TagName { get; set; }
        private int? TagOwner { get; set; }

        public List<PeopleInfo> Results { get; set; }
        public void PopulateResults()
        {
            var query = PersonQuery();
            count = query.Count();
            query = ApplySort(query);
            query = query.Skip(StartRow).Take(PageSize.Value);
            Results = FetchPeopleList(query).ToList();
        }
        private IQueryable<Person> PersonQuery()
        {
            Db.SetNoLock();
            var q = Db.People.Where(TopClause.Predicate(Db));
            if (TopClause.ParentsOf)
                return Db.PersonQueryParents(q);
            return q;
        }
        private IQueryable<Person> ApplySort(IQueryable<Person> q)
        {
            if (Sort == null)
                Sort = "Name";
            if (Direction != "desc")
                switch (Sort)
                {
                    case "Name":
                        q = from p in q
                            orderby p.LastName,
                            p.FirstName,
                            p.PeopleId
                            select p;
                        break;
                    case "Status":
                        q = from p in q
                            orderby p.MemberStatus.Code,
                            p.LastName,
                            p.FirstName,
                            p.PeopleId
                            select p;
                        break;
                    case "Address":
                        q = from p in q
                            orderby p.PrimaryState,
                            p.PrimaryCity,
                            p.PrimaryAddress,
                            p.PeopleId
                            select p;
                        break;
                    case "Fellowship Leader":
                        q = from p in q
                            orderby p.BFClass.LeaderName,
                            p.LastName,
                            p.FirstName,
                            p.PeopleId
                            select p;
                        break;
                    case "Employer":
                        q = from p in q
                            orderby p.EmployerOther,
                            p.LastName,
                            p.FirstName,
                            p.PeopleId
                            select p;
                        break;
                    case "Communication":
                        q = from p in q
                            orderby p.EmailAddress,
                            p.LastName,
                            p.FirstName,
                            p.PeopleId
                            select p;
                        break;
                    case "DOB":
                        q = from p in q
                            orderby p.BirthMonth, p.BirthDay,
                            p.LastName, p.FirstName
                            select p;
                        break;
                }
            else
                switch (Sort)
                {
                    case "Status":
                        q = from p in q
                            orderby p.MemberStatus.Code descending,
                            p.LastName descending,
                            p.FirstName descending,
                            p.PeopleId descending
                            select p;
                        break;
                    case "Address":
                        q = from p in q
                            orderby p.PrimaryState descending,
                            p.PrimaryCity descending,
                            p.PrimaryAddress descending,
                            p.PeopleId descending
                            select p;
                        break;
                    case "Name":
                        q = from p in q
                            orderby p.LastName descending,
                            p.LastName descending,
                            p.PeopleId descending
                            select p;
                        break;
                    case "Fellowship Leader":
                        q = from p in q
                            orderby p.BFClass.LeaderName descending,
                            p.LastName descending,
                            p.FirstName descending,
                            p.PeopleId descending
                            select p;
                        break;
                    case "Employer":
                        q = from p in q
                            orderby p.EmployerOther descending,
                            p.LastName descending,
                            p.FirstName descending,
                            p.PeopleId descending
                            select p;
                        break;
                    case "Communication":
                        q = from p in q
                            orderby p.EmailAddress descending,
                            p.LastName descending,
                            p.FirstName descending,
                            p.PeopleId descending
                            select p;
                        break;
                    case "DOB":
                        q = from p in q
                            orderby p.BirthMonth descending, p.BirthDay descending,
                            p.LastName descending, p.FirstName descending
                            select p;
                        break;
                }
            return q;
        }
        private IEnumerable<PeopleInfo> FetchPeopleList(IQueryable<Person> query)
        {
            if (query == null)
            {
                Db.SetNoLock();
                query = PersonQuery();
                count = query.Count();
            }
            var q = from p in query
                    select new PeopleInfo
                    {
                        PeopleId = p.PeopleId,
                        Name = p.Name,
                        BirthDate = Util.FormatBirthday(p.BirthYear, p.BirthMonth, p.BirthDay),
                        Address = p.PrimaryAddress,
                        Address2 = p.PrimaryAddress2,
                        CityStateZip = Util.FormatCSZ(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                        HomePhone = p.HomePhone,
                        CellPhone = p.CellPhone,
                        WorkPhone = p.WorkPhone,
                        PhonePref = p.PhonePrefId,
                        MemberStatus = p.MemberStatus.Description,
                        Email = p.EmailAddress,
                        BFTeacher = p.BFClass.LeaderName,
                        BFTeacherId = p.BFClass.LeaderId,
                        Employer = p.EmployerOther,
                        Age = p.Age.ToString(),
                        HasTag = p.Tags.Any(t => t.Tag.Name == TagName && t.Tag.PeopleId == TagOwner && t.Tag.TypeId == TagTypeId),
                    };
            return q;
        }

        public Tag TagAllIds()
        {
            var q = PersonQuery();
            var tag = Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_Query);
            Db.TagAll(q, tag);
            return tag;
        }

        public void TagAll(Tag tag = null)
        {
            var q = PersonQuery();
            if (tag != null)
                Db.TagAll(q, tag);
            else
                Db.TagAll(q);
        }

        public void UnTagAll()
        {
            var q = PersonQuery();
            Db.UnTagAll(q);
        }

        public Dictionary<string, string> Errors { get; set; }
        public bool Validate()
        {
            SetVisibility();
            Errors = new Dictionary<string, string>();
            DateTime dt = DateTime.MinValue;
            if (StartDateVisible)
                if (!DateTime.TryParse(StartDate, out dt) || dt.Year <= 1900 || dt.Year >= 2200)
                    Errors.Add("StartDate", "invalid");
            if (EndDateVisible && EndDate.HasValue())
                if (!DateTime.TryParse(EndDate, out dt) || dt.Year <= 1900 || dt.Year >= 2200)
                    Errors.Add("EndDate", "invalid");
            int i = 0;
            if (DaysVisible && !int.TryParse(Days, out i))
                Errors.Add("Days", "must be integer");
            if (i > 10000)
                Errors.Add("Days", "days > 1000");
            if (AgeVisible && !int.TryParse(Age, out i))
                Errors.Add("Age", "must be integer");


            if (TagsVisible && Tags != null && string.Join(",", Tags).Length > 500)
                Errors.Add("tagvalues", "too many tags selected");

            if (CodesVisible && CodeValues.Length == 0)
                Errors.Add("CodeValues", "must select item(s)");

            if (NumberVisible && !NumberValue.HasValue())
                Errors.Add("NumberValue", "must have a number value");
            else
            {
                float f;
                if (NumberVisible && NumberValue.HasValue())
                    if (!float.TryParse(NumberValue, out f))
                        Errors.Add("NumberValue", "must have a valid number value (no decoration)");
            }

            if (DateVisible && !Comparison.EndsWith("Equal"))
                if (!DateTime.TryParse(DateValue, out dt) || dt.Year <= 1900 || dt.Year >= 2200)
                    Errors.Add("DateValue", "need valid date");

            if (Comparison == "Contains")
                if (!TextValue.HasValue())
                    Errors.Add("TextValue", "cannot be empty");

            return Errors.Count == 0;
        }
        public bool ShowResults { get; set; }
        public string Sort { get; set; }
        public string Direction { get; set; }
        private int? _Page;
        public int? Page
        {
            get { return _Page ?? 1; }
            set { _Page = value; }
        }
        private int StartRow
        {
            get { return ((Page ?? 1) - 1) * PageSize ?? 10; }
        }
        public int? PageSize
        {
            get { return DbUtil.Db.UserPreference("PageSize", "10").ToInt(); }
            set
            {
                if (value.HasValue)
                    DbUtil.Db.SetUserPreference("PageSize", value);
            }
        }

        public int Count { get { return count; } }

        public PagerModel pagerModel()
        {
            return new PagerModel
            {
                Page = Page ?? 1,
                PageSize = PageSize ?? 10,
                Action = "List",
                Controller = "Task",
                Count = Count,
                ToggleTarget = true
            };
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