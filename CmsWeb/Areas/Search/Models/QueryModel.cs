/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml.Linq;
using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;
using UtilityExtensions;
using System.Data.Linq.SqlClient;
using System.Web.UI.WebControls;

namespace CmsWeb.Areas.Search.Models
{
    public class QueryModel
    {
        private CMSDataContext Db;
        public QueryBuilderClause TopClause;
        private int TagTypeId { get; set; }
        private string TagName { get; set; }
        private int? TagOwner { get; set; }

        public CmsWeb.Models.PagerModel2 Pager { get; set; }

        public QueryModel()
        {
            Db = DbUtil.Db;
            Db.SetUserPreference("NewCategories", "true");
            ConditionName = "Group";
            TagTypeId = DbUtil.TagTypeId_Personal;
            TagName = Util2.CurrentTagName;
            TagOwner = Util2.CurrentTagOwnerId;
            Pager = new PagerModel2(Count) {Direction = "asc"};
        }
        public int Count()
        {
            return FetchCount();
        }
        public string Description { get; set; }
        public int? QueryId { get; set; }

        public void LoadScratchPad()
        {
            TopClause = Db.QueryBuilderScratchPad();
            if (QueryId.HasValue && QueryId.Value != TopClause.QueryId)
            {
                var existing = Db.LoadQueryById(QueryId.Value);
                if (existing != null)
                {
                    TopClause.CopyFromAll(existing, DbUtil.Db);
                    Description = TopClause.Description;
                    SavedQueryDesc = TopClause.Description;
                    TopClause.Description = Util.ScratchPad;
                    Db.SubmitChanges();
                }
            }
            QueryId = TopClause.QueryId;
        }

        public int? SelectedId { get; set; }

        public bool RightPanelVisible { get; set; }
        public bool ComparePanelVisible { get; set; }
        public bool TextVisible { get; set; }
        public bool NumberVisible { get; set; }
        public bool IntegerVisible { get; set; }
        public bool CodeVisible { get; set; }
        public bool DateVisible { get; set; }
        public bool ProgramVisible { get; set; }
        public bool DivisionVisible { get; set; }
        public bool EndDateVisible { get; set; }
        public bool StartDateVisible { get; set; }
        public bool OrganizationVisible { get; set; }
        public bool ScheduleVisible { get; set; }
        public bool CampusVisible { get; set; }
        public bool OrgTypeVisible { get; set; }
        public bool DaysVisible { get; set; }
        public bool AgeVisible { get; set; }
        public bool SavedQueryVisible { get; set; }
        public bool MinistryVisible { get; set; }
        public bool QuartersVisible { get; set; }
        public bool TagsVisible { get; set; }

        public List<SelectListItem> TagData { get; set; }

        private static List<CodeValueItem> BitCodes =
            new List<CodeValueItem> 
            { 
                new CodeValueItem { Id = 1, Value = "True", Code = "T" }, 
                new CodeValueItem { Id = 0, Value = "False", Code = "F" }, 
            };
        public IEnumerable<SelectListItem> GetCodeData()
        {
            var cvctl = new CodeValueModel();
            switch (fieldMap.Type)
            {
                case FieldType.Bit:
                case FieldType.NullBit:
                    return ConvertToSelect(BitCodes, fieldMap.DataValueField);
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                    if (fieldMap.DataSource == "ExtraValues")
                        return StandardExtraValues.ExtraValueCodes();
                    if (fieldMap.DataSource == "Campuses")
                        return Campuses();
                    return ConvertToSelect(Util.CallMethod(cvctl, fieldMap.DataSource), fieldMap.DataValueField);
                case FieldType.DateField:
                    return ConvertToSelect(Util.CallMethod(cvctl, fieldMap.DataSource), fieldMap.DataValueField);
            }
            return null;
        }

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

        public void SetVisibility()
        {
            ComparePanelVisible = fieldMap.Name != "MatchAnything";
            RightPanelVisible = ComparePanelVisible;
            TextVisible = false;
            NumberVisible = false;
            CodeVisible = false;
            DateVisible = false;
            ConditionName = ConditionName;
            CompareData = Comparisons().ToList();
            DivisionVisible = fieldMap.HasParam("Division");
            ProgramVisible = fieldMap.HasParam("Program");
            OrganizationVisible = fieldMap.HasParam("Organization");
            ScheduleVisible = fieldMap.HasParam("Schedule");
            CampusVisible = fieldMap.HasParam("Campus");
            OrgTypeVisible = fieldMap.HasParam("OrgType");
            DaysVisible = fieldMap.HasParam("Days");
            AgeVisible = fieldMap.HasParam("Age");
            SavedQueryVisible = fieldMap.HasParam("SavedQueryIdDesc");
            MinistryVisible = fieldMap.HasParam("Ministry");
            QuartersVisible = fieldMap.HasParam("Quarters");
            if (QuartersVisible)
                QuartersLabel = fieldMap.QuartersTitle;
            TagsVisible = fieldMap.HasParam("Tags");
            if (TagsVisible)
            {
                var cv = new CodeValueModel();
                TagData = ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Code");
            }
            StartDateVisible = fieldMap.HasParam("StartDate");
            EndDateVisible = fieldMap.HasParam("EndDate");

            switch (fieldMap.Type)
            {
                case FieldType.Bit:
                case FieldType.NullBit:
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                case FieldType.DateField:
                    CodeVisible = true;
                    break;
                case FieldType.String:
                case FieldType.StringEqual:
                case FieldType.StringEqualOrStartsWith:
                    TextVisible = true;
                    break;
                case FieldType.NullNumber:
                case FieldType.Number:
                    NumberVisible = true;
                    break;
                case FieldType.NullInteger:
                case FieldType.Integer:
                case FieldType.IntegerSimple:
                case FieldType.IntegerEqual:
                    IntegerVisible = true;
                    break;
                case FieldType.Date:
                case FieldType.DateSimple:
                    DateVisible = true;
                    break;
            }
            var cc = Db.LoadQueryById(SelectedId);
            if (cc == null)
                return;

            UpdateEnabled = !cc.IsGroup && !cc.IsFirst;
            AddToGroupEnabled = cc.IsGroup;
            AddEnabled = !cc.IsFirst;
            RemoveEnabled = cc.CanRemove;

            if (fieldMap.Type == FieldType.Group)
            {
                CompareData = Comparisons().ToList();
                RightPanelVisible = false;
                UpdateEnabled = cc.IsGroup;
                return;
            }
        }
        public List<SelectListItem> ConvertToSelect(object items, string valuefield)
        {
            var list = items as IEnumerable<CodeValueItem>;
            List<SelectListItem> list2;
            List<string> values;
            if (CodeValues != null) 
                values = CodeValues.ToList();
            else if(CodeValue != null)
                values = new List<string> {CodeValue};
            else
                values = new List<string>();
            switch (valuefield)
            {
                case "IdCode":
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.IdCode, Selected = values.Contains(c.IdCode) }).ToList();
                    break;
                case "Id":
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.Id.ToString(), Selected = values.Contains(c.Id.ToString()) }).ToList();
                    break;
                case "Code":
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.Code, Selected = values.Contains(c.Code) }).ToList();
                    break;
                default:
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.Value, Selected = values.Contains(c.Value) }).ToList();
                    break;
            }
            return list2;
        }
        DateTime? DateParse(string s)
        {
            DateTime dt;
            if (DateTime.TryParse(s, out dt))
                return dt;
            return null;
        }
        int? IntParse(string s)
        {
            int i;
            if (int.TryParse(s, out i))
                return i;
            return null;
        }
        string DateString(DateTime? dt)
        {
            if (dt.HasValue)
                return dt.Value.ToShortDateString();
            return "";
        }
        private void UpdateCondition(QueryBuilderClause c)
        {
            c.Field = ConditionName;
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
            c.SavedQueryIdDesc = SavedQueryDesc;
            //Db.SubmitChanges();
            SelectedId = null;
        }
        public void EditCondition()
        {
            var c = Db.LoadQueryById(SelectedId);
            if (c == null)
                return;
            ConditionName = c.FieldInfo.Name;
            SetVisibility();
            Comparison = c.Comparison;
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
                        foreach (var i in GetCodeData())
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
            Days = c.Days.ToString();
            Age = c.Age.ToString();
            Quarters = c.Quarters;
            if (TagsVisible)
            {
                if (c.Tags != null)
                    Tags = c.Tags.Split(';');
                var cv = new CodeValueModel();
                TagData = ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Code");
                foreach (var i in TagData)
                    i.Selected = Tags.Contains(i.Value);
            }
            if (MinistryVisible)
                Ministry = c.Program;
            SavedQueryDesc = c.SavedQueryIdDesc;
        }
        public void SetCodes()
        {
            SetVisibility();
            SelectMultiple = Comparison.EndsWith("OneOf");
        }
        public void SaveQuery()
        {
            var saveto = Db.QueryBuilderClauses.FirstOrDefault(c =>
                (c.SavedBy == Util.UserName || c.SavedBy == "public") && c.Description == SavedQueryDesc);
            if (saveto == null)
            {
                saveto = new QueryBuilderClause();
                Db.QueryBuilderClauses.InsertOnSubmit(saveto);
            }
            saveto.CopyFromAll(TopClause, DbUtil.Db); // save Qb on top of existing
            if (saveto.SavedBy != "public")
                saveto.SavedBy = Util.UserName;
            saveto.Description = SavedQueryDesc;
            saveto.IsPublic = IsPublic;
            Db.SubmitChanges();
            Description = SavedQueryDesc;
        }
        public int AddConditionToGroup()
        {
            var c = Db.LoadQueryById(SelectedId);
            var nc = c.AddNewClause(QueryType.MatchAnything, CompareType.Equal, null);
            Db.SubmitChanges();
            return nc.QueryId;
        }
        public int AddGroupToGroup()
        {
            var c = Db.LoadQueryById(SelectedId);
            var g = new QueryBuilderClause();
            g.SetQueryType(QueryType.Group);
            g.SetComparisonType(CompareType.AllTrue);
            var currParent = c.Parent;
            g.Parent = c;
            var nc = g.AddNewClause(QueryType.MatchAnything, CompareType.Equal, null);
            Db.SubmitChanges();
            return nc.QueryId;
        }
        public void AddNewConditionAfterCurrent(int id)
        {
            var c = Db.LoadQueryById(id);
            var nc = c.Parent.AddNewClause(QueryType.MatchAnything, CompareType.Equal, null);
            Db.SubmitChanges();
            SelectedId = nc.QueryId;
            EditCondition();
        }
        public int CopyCurrentCondition(int id)
        {
            var c = Db.LoadQueryById(id);
            SelectedId = id;
            EditCondition();
            var nc = NewCondition(c.Parent, c.ClauseOrder + 1);
            Db.SubmitChanges();
            SelectedId = nc.QueryId;
            if (nc.IsGroup)
                AddMatchAnyThingToGroup(nc);
            EditCondition();
            return nc.QueryId;
        }

        private void AddMatchAnyThingToGroup(QueryBuilderClause nc)
        {
            nc = nc.AddNewClause(QueryType.MatchAnything, CompareType.Equal, null);
            Db.SubmitChanges();
            SelectedId = nc.QueryId;
            EditCondition();
        }

        public void AddConditionAfterCurrent()
        {
            var c = Db.LoadQueryById(SelectedId);
            var nc = NewCondition(c.Parent, c.ClauseOrder + 1);
            Db.SubmitChanges();
            if (nc.IsGroup)
            {
                nc = nc.AddNewClause(QueryType.MatchAnything, CompareType.Equal, null);
                Db.SubmitChanges();
                SelectedId = nc.QueryId;
            }
        }
        private QueryBuilderClause NewCondition(QueryBuilderClause gc, int order)
        {
            var c = new QueryBuilderClause();
            c.ClauseOrder = order;
            gc.Clauses.Add(c);
            gc.ReorderClauses();
            UpdateCondition(c);
            return c;
        }

        public void DeleteCondition()
        {
            var c = Db.LoadQueryById(SelectedId);
            if (c == null)
                return;
            SelectedId = c.Parent.QueryId;
            Db.DeleteQueryBuilderClauseOnSubmit(c);
            Db.SubmitChanges();
            EditCondition();
        }
        public void UpdateCondition()
        {
            var c = Db.LoadQueryById(SelectedId);
            if (c == null)
                return;
            UpdateCondition(c);
        }
        public void ChangeGroup(string comp)
        {
            var c = Db.LoadQueryById(SelectedId);
            c.Comparison = comp;
            Db.SubmitChanges();
        }
//        public void CopyAsNew()
//        {
//            var Qb = Db.LoadQueryById(SelectedId).Clone(DbUtil.Db);
//            if (!Qb.IsGroup)
//            {
//                var g = new QueryBuilderClause();
//                g.SetQueryType(QueryType.Group);
//                g.SetComparisonType(CompareType.AllTrue);
//                Qb.Parent = g;
//                Qb = g;
//            }
//            Db.SubmitChanges();
//            QueryId = Qb.QueryId;
//        }
        public void MoveToPreviousGroup()
        {
            var cc = Db.LoadQueryById(SelectedId);
            var fp = cc.Parent;
            var g = cc.Parent.Parent;
            if (g != null)
            {
                cc.Parent = g;
                if (fp.Clauses.Count == 0)
                    Db.DeleteQueryBuilderClauseOnSubmit(fp);
            }
            else if (cc.IsGroup && fp.Clauses.Count == 1)
            {
                TopClause = cc;
                cc.Parent = null;
                Db.DeleteQueryBuilderClauseOnSubmit(fp);
                QueryId = cc.QueryId;
    			Util.QueryBuilderScratchPadId = TopClause.QueryId;
                TopClause.Description = Util.ScratchPad;
                TopClause.SavedBy = Util.UserName;
            }
            Db.SubmitChanges();
        }
        public void Paste(int id)
        {
            var clip = HttpContext.Current.Session["QueryClipboard"] as string;
            if (clip == null)
                return;
            var a = clip.Split(',');
            var clipop = a[0];
            var clipid = a[1].ToInt();
            var clipclause = Db.LoadQueryById(clipid);
            var targetclause = Db.LoadQueryById(id);
            var originalParent = clipclause.Parent;
            if (clipop == "Copy")
            {
                clipclause = clipclause.Clone(Db);
                originalParent = null;
            }
            if (targetclause.IsGroup)
            {
                targetclause.Clauses.Add(clipclause);
                clipclause.ClauseOrder = 0;
            }
            else
            {
                targetclause.Parent.Clauses.Add(clipclause);
                clipclause.ClauseOrder = targetclause.ClauseOrder + 1;
                targetclause.Parent.ReorderClauses();
            }
            if(originalParent != null && originalParent.Clauses.Count == 0)
                DbUtil.Db.QueryBuilderClauses.DeleteOnSubmit(originalParent);
            HttpContext.Current.Session["QueryClipboard"] = "Copy," + clipid;
            DbUtil.Db.SubmitChanges();
        }
        public void MoveToGroupBelow()
        {
            var cc = Db.LoadQueryById(SelectedId);
            var g = cc.Parent.Clauses.First(gg => gg.IsGroup);
            cc.Parent = g;
            Db.SubmitChanges();
        }
        public void InsertGroupAbove()
        {
            var cc = Db.LoadQueryById(SelectedId);
            var g = new QueryBuilderClause();
            g.SetQueryType(QueryType.Group);
            g.SetComparisonType(CompareType.AllTrue);
            g.ClauseOrder = cc.ClauseOrder;
            if (cc.IsFirst)
            {
                cc.Parent = g;
            }
            else
            {
                var currParent = cc.Parent;
                // find all clauses from cc down at same level
                var q = from c in cc.Parent.Clauses
                    orderby c.ClauseOrder
                    where c.ClauseOrder >= cc.ClauseOrder
                    select c;
                foreach (var c in q)
                    c.Parent = g; // change to new parent
                g.Parent = currParent;
            }
            if (cc.SavedBy.HasValue())
            {
                g.SavedBy = Util.UserName;
                g.Description = cc.Description;
                g.CreatedOn = cc.CreatedOn;
                cc.IsPublic = false;
                cc.Description = null;
                cc.SavedBy = null;
            }
            Db.SubmitChanges();
            if (g.IsFirst)
            {
                TopClause = g;
                QueryId = g.QueryId;
    			Util.QueryBuilderScratchPadId = TopClause.QueryId;
            }
        }
        public IEnumerable<SelectListItem> GroupComparisons()
        {
            return from c in CompareClass.Comparisons
                   where c.FieldType == FieldType.Group
                   select new SelectListItem 
                   { 
                       Text = c.CompType == CompareType.AllTrue ? "All" 
                            : c.CompType == CompareType.AnyTrue ? "Any" 
                            : c.CompType == CompareType.AllFalse ? "None" 
                            : "unknown", 
                       Value = c.CompType.ToString() 
                   };
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
        public static IEnumerable<SelectListItem> Divisions(int? progid)
        {
            var q = from div in DbUtil.Db.Divisions
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
        public static IEnumerable<SelectListItem> Organizations(int? divid)
        {
            var roles = DbUtil.Db.CurrentRoles();
            var q = from ot in DbUtil.Db.DivOrgs
                    where ot.Organization.LimitToRole == null || roles.Contains(ot.Organization.LimitToRole)
                    where ot.DivId == divid
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
        public List<QueryClauseDisplay> ConditionList()
        {
            if (TopClause == null)
                LoadScratchPad();
            level = 0;
            return ClauseAndSubs(new List<QueryClauseDisplay>(), TopClause);
        }
        private List<QueryClauseDisplay> ClauseAndSubs(List<QueryClauseDisplay> list, QueryBuilderClause qc)
        {
            list.Add(new QueryClauseDisplay { Level = level, Clause = qc });
            level++;
            var q = qc.Clauses.OrderBy(c => c.ClauseOrder);
            foreach (var c in q)
                list = ClauseAndSubs(list, c);
            level--;
            return list;
        }
        public IEnumerable<CategoryClass> FieldCategories()
        {
            var q = from c in CategoryClass.Categories
                    where c.Title != "Grouping"
                    select c;
            return q;
        }
        public List<SelectListItem> SavedQueries()
        {
            var cv = new CodeValueModel();
            return ConvertToSelect(cv.UserQueries(), "Code");
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

        private IQueryable<Person> query;
        private int? count;
        public int FetchCount()
        {
            Db.SetNoLock();
            query = PersonQuery();
            count = query.Count();
            return count ?? 0;
        }
        public List<PeopleInfo> Results;
        public void PopulateResults()
        {
            query = PersonQuery();
            count = query.Count();
            query = ApplySort(query);
            query = query.Skip(Pager.StartRow).Take(Pager.PageSize);
            Results = FetchPeopleList(query).ToList();
        }
        public IEnumerable<PeopleInfo> FetchPeopleList()
        {
            query = ApplySort(query);
            query = query.Skip(Pager.StartRow).Take(Pager.PageSize);
            return FetchPeopleList(query);
        }
        public class MyClass
        {
            public int Id { get; set; }
            public int PeopleId { get; set; }
        }
        public Tag TagAllIds()
        {
            query = PersonQuery();
            var tag = Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_Query);
            Db.TagAll(query, tag);
            return tag;
        }
        private IQueryable<Person> PersonQuery()
        {
            if (TopClause == null)
                LoadScratchPad();
            Db.SetNoLock();
            var q = Db.People.Where(TopClause.Predicate(Db));
            if (TopClause.ParentsOf)
                return Db.PersonQueryParents(q);
            return q;
        }
        public void TagAll(Tag tag = null)
        {
            if (TopClause == null)
                LoadScratchPad();
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
                LoadScratchPad();
            Db.SetNoLock();
            var q = Db.People.Where(TopClause.Predicate(Db));
            if (TopClause.ParentsOf)
                q = Db.PersonQueryParents(q);
            Db.UnTagAll(q);
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
        private IQueryable<Person> ApplySort(IQueryable<Person> q)
        {
//            if (Pager.Sort == null)
//                Pager.Sort = "Name";
            if (Pager.Direction != "desc")
                switch (Pager.Sort)
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
                switch (Pager.Sort)
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

        public bool Validate(ModelStateDictionary m)
        {
            SetVisibility();
            DateTime dt = DateTime.MinValue;
            if (StartDateVisible)
                if (!DateTime.TryParse(StartDate, out dt) || dt.Year <= 1900 || dt.Year >= 2200)
                    m.AddModelError("StartDate", "invalid date");
            if (EndDateVisible && EndDate.HasValue())
                if (!DateTime.TryParse(EndDate, out dt) || dt.Year <= 1900 || dt.Year >= 2200)
                    m.AddModelError("EndDate", "invalid date");
            int i = 0;
            if (DaysVisible && !int.TryParse(Days, out i))
                m.AddModelError("Days", "must be integer");
            if (i > 10000)
                m.AddModelError("Days", "days > 10000");
            if (AgeVisible && !int.TryParse(Age, out i))
                m.AddModelError("Age", "must be integer");


            if (IntegerVisible && !Comparison.EndsWith("Null") && !int.TryParse(IntegerValue, out i))
                m.AddModelError("IntegerValue", "need integer");

            if (TagsVisible && string.Join(",", Tags).Length > 500)
                m.AddModelError("tagvalues", "too many tags selected");

            decimal d;
            if (NumberVisible && !Comparison.EndsWith("Null") && !decimal.TryParse(NumberValue, out d))
                m.AddModelError("NumberValue", "need number");

            if (DateVisible && !Comparison.EndsWith("Null"))
                if (!DateTime.TryParse(DateValue, out dt) || dt.Year <= 1900 || dt.Year >= 2200)
                    m.AddModelError("DateValue", "need valid date");

            if (Comparison == "Contains")
                if (!TextValue.HasValue())
                    m.AddModelError("TextValue", "cannot be empty");

            return m.IsValid;
        }

        public bool ShowResults { get; set; }

        public bool CanSave { get; set; }
    }
    public class QueryClauseDisplay
    {
        public int Level { get; set; }
        public QueryBuilderClause Clause;
    }
}
