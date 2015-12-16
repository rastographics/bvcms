using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class Condition
    {
        public Dictionary<Guid, Condition> AllConditions { get; set; }

        #region Fields

        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public int Order { get; set; }
        public string ConditionName { get; set; }
        public string Comparison { get; set; }
        public string TextValue { get; set; }
        public DateTime? DateValue { get; set; }
        public string CodeIdValue { get; set; }

        private DateTime? startDate;
        public DateTime? StartDate
        {
            get
            {
                if (db != null)
                    return db.QbStartDateOverride ?? startDate;
                return startDate;
            }
            set { startDate = value; }
        }

        private DateTime? endDate;
        public DateTime? EndDate
        {
            get
            {
                if (db != null)
                    return db.QbEndDateOverride ?? endDate;
                return endDate;
            }
            set { endDate = value; }
        }

        public string Ministry { get; set; }
        public int? MinistryInt => Ministry.ToInt2() ?? ProgramInt; // Ministry used to be stored in Program

        public string Program { get; set; }
        public int? ProgramInt => Division.HasValue() ? 0 : Program.GetCsvToken().ToInt();

        private string division;
        public string Division
        {
            get
            {
                if (db != null)
                    return Util.PickFirst(db.QbDivisionOverride.ToString(), division);
                return division;
            }
            set { division = value; }
        }
        public int? DivisionInt => Organization.HasValue() ? 0 : Division.GetCsvToken().ToInt();

        public string Organization { get; set; }
        public int OrganizationInt => Organization.GetCsvToken().ToInt();

        public int Days { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
        public string PreviousName { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Quarters { get; set; }
        public string SavedQuery { get; set; }
        public string SavedQueryIdDesc => SavedQuery;
        public string Tags { get; set; }
        public string PmmLabels => Tags;

        public string Schedule { get; set; }
        public int ScheduleInt => Schedule.GetCsvToken().ToInt();

        public int? Age { get; set; }

        public string Campus { get; set; }
        public int? CampusInt => Campus.GetCsvToken().ToInt();

        public string OrgType { get; set; }
        public int? OrgTypeInt => OrgType.GetCsvToken().ToInt2();

        public string OrgStatus { get; set; }
        public int? OrgStatusInt => OrgStatus.GetCsvToken().ToInt2();

        public string OnlineReg { get; set; }
        public int? OnlineRegInt => OnlineReg.GetCsvToken().ToInt2();

        public int? OrgType2 { get; set; }

        public string OrgName { get; set; }
        public Guid? NewMatchAnyId;
        internal Query JustLoadedQuery;

        #endregion

        public IEnumerable<Condition> Conditions
        {
            get
            {
                var q = from c in AllConditions.Values
                        where c.ParentId == Id
                        orderby c.Order
                        select c;
                return q;
            }
        }
        public Condition Parent
        {
            get
            {
                if (ParentId.HasValue)
                    return AllConditions[ParentId.Value];
                return null;
            }
        }
        private FieldClass _FieldInfo;
        public FieldClass FieldInfo
        {
            get
            {
                try
                {
                    if ((_FieldInfo == null || _FieldInfo.Name != ConditionName))
                        _FieldInfo = FieldClass.Fields[ConditionName];
                    return _FieldInfo;
                }
                catch (Exception)
                {
                    //throw new Exception("QB Field not found: " + ConditionName);
                    return null;
                }
            }
        }
        public void SetComparisonType(CompareType value)
        {
            Comparison = value.ToString();
            compare = null;
            compare = Compare2;
        }
        public void SetQueryType(QueryType value)
        {
            ConditionName = value.ToString();
        }
        public CompareType ComparisonType => CompareClass.Convert(Comparison, this);

        private CompareClass compare;
        public CompareClass Compare2
        {
            get
            {
                if (compare == null)
                {
                    switch (ComparisonType)
                    {
                        case CompareType.IsNull:
                            Comparison = "Equal";
                            TextValue = string.Empty;
                            break;
                        case CompareType.IsNotNull:
                            Comparison = "NotEqual";
                            TextValue = string.Empty;
                            break;
                    }
                    compare = CompareClass.Comparisons.SingleOrDefault(cm =>
                        cm.FieldType == FieldInfo.Type && cm.CompType == ComparisonType);
                }
                return compare;
            }
        }
        public override string ToString()
        {
            string ret = "null";
            if (ConditionName == "MatchAnything")
                ret = "Match Anything";
            switch (ComparisonType)
            {
                case CompareType.AllTrue:
                    ret = "Match ALL of the conditions below";
                    break;
                case CompareType.AnyTrue:
                    ret = "Match ANY of the conditions below";
                    break;
                case CompareType.AllFalse:
                    ret = "Match NONE of the conditions below";
                    break;
                default:
                    if (Compare2 != null)
                        ret = Compare2.ToString(this);
                    else if (ComparisonType == CompareType.Equal && FieldInfo.Type == FieldType.NumberLG)
                    {
                        SetComparisonType(CompareType.LessEqual);
                        if (Compare2 != null)
                            ret = Compare2.ToString(this);
                    }
                    break;
            }
            return ret;
        }

        internal void SetIncludeDeceased()
        {
            var c = this;
            while (c.Parent != null)
                c = c.Parent;
            c.includeDeceased = true;
        }
        internal void SetParentsOf(CompareType op, bool tf)
        {
            var c = this;
            while (c.Parent != null)
                c = c.Parent;
            c.ParentsOf = ((tf && op == CompareType.Equal) || (!tf && op == CompareType.NotEqual));
        }
        internal void SetPlusParentsOf(CompareType op, bool tf)
        {
            var c = this;
            while (c.Parent != null)
                c = c.Parent;
            c.PlusParentsOf = ((tf && op == CompareType.Equal) || (!tf && op == CompareType.NotEqual));
        }
        private bool includeDeceased;
        public bool ParentsOf { get; set; }
        public bool PlusParentsOf { get; set; }
        public Expression<Func<Person, bool>> Predicate(CMSDataContext db)
        {
            db.CopySession();
            var parameter = Expression.Parameter(typeof(Person), "p");
            Expression tree;
            try
            {
                tree = ExpressionTree(parameter, db);
            }
            catch (Exception ex)
            {
                var x = ToXml();
                throw new Exception("problem in query:\n" + x, ex);
            }
            if (tree == null)
                tree = CompareConstant(parameter, "PeopleId", CompareType.NotEqual, 0);
            if (includeDeceased == false)
                tree = Expression.And(tree, CompareConstant(parameter, "IsDeceased", CompareType.NotEqual, true));
            if (Util2.OrgLeadersOnly)
                tree = Expression.And(OrgLeadersOnly(db, parameter), tree);
            return Expression.Lambda<Func<Person, bool>>(tree, parameter);
        }
        private Expression OrgLeadersOnly(CMSDataContext db, ParameterExpression parm)
        {
            var tag = db.OrgLeadersOnlyTag2();
            Expression<Func<Person, bool>> pred = p =>
                p.Tags.Any(t => t.Id == tag.Id);
            return Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
        }
        private bool InAllAnyFalse => Parent.IsGroup && Parent.ComparisonType == CompareType.AllFalse;

        private bool AnyFalseTrue => ComparisonType == CompareType.AnyTrue;

        private Expression ExpressionTree(ParameterExpression parm, CMSDataContext Db)
        {
            Expression expr = null;
            if (IsGroup)
            {
                foreach (var clause in Conditions)
                {
                    if (clause.FieldInfo == null)
                        continue;
                    if (clause.FieldInfo.QueryType == QueryType.IncludeDeceased)
                    {
                        SetIncludeDeceased();
                        continue;
                    }
                    if (clause.FieldInfo.QueryType == QueryType.ParentsOf)
                    {
                        SetParentsOf(clause.ComparisonType, clause.CodeIds == "1");
                        continue;
                    }
                    if (clause.FieldInfo.QueryType == QueryType.PlusParentsOf)
                    {
                        SetPlusParentsOf(clause.ComparisonType, clause.CodeIds == "1");
                        continue;
                    }
                    if (clause.FieldInfo.QueryType == QueryType.DeceasedDate)
                        SetIncludeDeceased();
                    if (expr == null)
                        expr = clause.ExpressionTree(parm, Db);
                    else
                    {
                        var right = clause.ExpressionTree(parm, Db);
                        if (right != null)
                            expr = AnyFalseTrue
                                ? Expression.Or(expr, right)
                                : Expression.And(expr, right);
                    }
                }
                return expr;
            }
            expr = Compare2 == null
                ? AlwaysFalse(parm)
                : GetExpression(parm, Db);
            if (InAllAnyFalse)
                expr = Expression.Not(expr);
            return expr;
        }

        public bool HasMultipleCodes
        {
            get
            {
                if (ConditionName == "MatchAnything")
                    return false;
                var e = Compare2;
                if (e == null)
                    return false;
                return e.CompType == CompareType.OneOf
                    || e.CompType == CompareType.NotOneOf;
            }
        }

        private static readonly FieldType[] CodeTypes =
        {
            FieldType.Bit,
            FieldType.NullBit,
            FieldType.Code,
            FieldType.NullCode,
            FieldType.CodeStr,
            FieldType.DateField,
        };
        private bool IsCode => CodeTypes.Contains(Compare2.FieldType);

        private enum Part { Id = 0, Code = 1 }
        private string GetCodeIdValuePart(string value, Part part)
        {
            if (value != null && value.Contains(","))
                return value.SplitStr(",", 2)[(int)part];
            return value;
        }
        internal string CodeIdText
        {
            get
            {
                if (!IsCode)
                    return "";
                if (Compare2.FieldType == FieldType.CodeStr)
                {
                    if (HasMultipleCodes)
                        return string.Join(", ", CodeIdValue.SplitStr(";").Select(s => $"'{s.Replace("'", "''")}'"));
                    return $"'{CodeIdValue?.Replace("'", "''") ?? CodeIdValue}'";
                }
                if (HasMultipleCodes)
                    return string.Join(", ", (from s in CodeIdValue.SplitStr(";")
                                              where s != "multiselect-all"
                                              let aa = s.Split(',')
                                              select aa.Length > 1 ? $"{aa[0]}[{aa[1]}]" : aa[0]
                        ).ToArray());
                var a = CodeIdValue.SplitStr(",", 2);
                return a.Length > 1 ? $"{a[0]}[{a[1]}]" : CodeIdValue;
            }
        }
        internal string CodeValues
        {
            get
            {
                if (!IsCode)
                    return "";
                if (HasMultipleCodes)
                    return string.Join(", ", (from s in CodeIdValue.SplitStr(";")
                                              let aa = s.Split(':')
                                              select aa[0]
                        ).ToArray());
                var a = CodeIdValue.Split(':');
                return a[0];
            }
        }
        internal string CodeIds
        {
            get
            {
                if (!IsCode)
                    return "";
                if (HasMultipleCodes)
                {
                    var q = from s in CodeIdValue.SplitStr(";")
                            select GetCodeIdValuePart(s, Part.Id);
                    return string.Join(",", q.ToArray());
                }

                // handle deprecated values
                if (CodeIdValue.Equal("True"))
                    CodeIdValue = "1,True";
                else if (CodeIdValue.Equal("False"))
                    CodeIdValue = "0,False";

                var cid = GetCodeIdValuePart(CodeIdValue, Part.Id);

                // handle reasonable default for missing true/false condition
                if (cid == null && FieldInfo.Type == FieldType.Bit)
                    cid = "1"; 

               return cid;
            }
        }
        internal int[] CodeIntIds
        {
            get
            {
                if (!IsCode)
                    return null;
                if (HasMultipleCodes)
                {
                    var q = from s in CodeIdValue.SplitStr(";")
                            where s != "multiselect-all"
                            select GetCodeIdValuePart(s, Part.Id).ToInt();
                    return q.ToArray();
                }
                return new[] { GetCodeIdValuePart(CodeIdValue, Part.Id).ToInt() };
            }
        }
        internal string[] CodeStrIds
        {
            get
            {
                if (!IsCode)
                    return null;
                if (!HasMultipleCodes)
                    return new[] { GetCodeIdValuePart(CodeIdValue, Part.Id) };
                var q = from s in CodeIdValue.SplitStr(";")
                        select GetCodeIdValuePart(s, Part.Id);
                return q.ToArray();
            }
        }
        public bool IsFirst => IsGroup && !ParentId.HasValue;

        public bool IsGroup => FieldInfo.Type == FieldType.Group;

        public bool IsLastNode => !ParentId.HasValue || Parent.Conditions.Count() == 1;

        public Condition Clone(Condition parent = null, Guid? useGuid = null)
        {
            var newclause = new Condition();
            if (parent == null)
                newclause.AllConditions = new Dictionary<Guid, Condition>();
            else
                newclause.AllConditions = parent.AllConditions;
            newclause.CopyFrom(this);
            if (useGuid.HasValue)
                newclause.Id = useGuid.Value;
            newclause.AllConditions.Add(newclause.Id, newclause);
            foreach (var c in Conditions)
            {
                var nc = c.Clone(newclause);
                nc.ParentId = newclause.Id;
            }
            return newclause;
        }
        private void CopyFrom(Condition from)
        {
            Id = Guid.NewGuid();
            Age = from.Age;
            Order = from.Order;
            CodeIdValue = from.CodeIdValue;
            Comparison = from.Comparison;
            DateValue = from.DateValue;
            Days = from.Days;
            Description = from.Description;
            PreviousName = from.PreviousName;
            Division = from.Division;
            EndDate = from.EndDate;
            ConditionName = from.ConditionName;
            Organization = from.Organization;
            Program = from.Program;
            Quarters = from.Quarters;
            SavedQuery = from.SavedQuery;
            Schedule = from.Schedule;
            StartDate = from.StartDate;
            Tags = from.Tags;
            TextValue = from.TextValue;
        }
        public void DeleteClause()
        {
            var allClauses = AllConditions;
            foreach (var c in Conditions)
                c.DeleteClause();
            allClauses.Remove(Id);
        }
        public void Reset()
        {
            foreach (var c in Conditions)
                c.DeleteClause();
            SetQueryType(QueryType.Group);
            SetComparisonType(CompareType.AllTrue);
        }
        public static Condition CreateNewGroupClause(string name = null)
        {
            var c = new Condition
            {
                Description = name,
                Id = Guid.NewGuid(),
                AllConditions = new Dictionary<Guid, Condition>(),
                ConditionName = QueryType.Group.ToString(),
                Comparison = CompareType.AllTrue.ToString()
            };
            c.AllConditions.Add(c.Id, c);
            return c;
        }

        public static Condition CreateAllGroup(string name = null)
        {
            return CreateNewGroupClause(name);
        }
        public static Condition CreateAllFalseGroup(string name = null)
        {
            var c = CreateNewGroupClause(name);
            c.Comparison = CompareType.AllFalse.ToString();
            return c;
        }
        public void IncrementLastRun()
        {
            if (JustLoadedQuery == null)
                return;
            JustLoadedQuery.RunCount++;
            JustLoadedQuery.LastRun = DateTime.Now;
        }

        public Condition AddNewGroupClause()
        {
            var c = new Condition
            {
                ParentId = Id,
                Id = Guid.NewGuid(),
                AllConditions = AllConditions,
                ConditionName = QueryType.Group.ToString(),
                Comparison = CompareType.AnyTrue.ToString(),
                Order = MaxClauseOrder() + 2
            };
            AllConditions.Add(c.Id, c);
            return c;
        }
        public int MaxClauseOrder()
        {
            if (!Conditions.Any())
                return 0;
            return Conditions.Max(qc => qc.Order);
        }
        public void ReorderClauses()
        {
            var q = from c in Conditions
                    orderby c.Order
                    select c;
            int n = 1;
            foreach (var c in q)
            {
                c.Order = n;
                n += 2;
            }
        }
        public Condition AddNewClause()
        {
            var c = new Condition
            {
                ParentId = Id,
                Id = Guid.NewGuid(),
                AllConditions = AllConditions,
                ConditionName = QueryType.MatchAnything.ToString(),
                Comparison = CompareType.Equal.ToString(),
                Order = MaxClauseOrder() + 2
            };
            NewMatchAnyId = c.Id;
            AllConditions.Add(c.Id, c);
            return c;
        }
        public Condition AddNewClause(QueryType type, CompareType op, object value = null)
        {
            var c = AddNewClause();
            c.SetQueryType(type);
            c.SetComparisonType(op);
            if (type == QueryType.MatchAnything || type == QueryType.MatchNothing)
            {
                c.CodeIdValue = "1,true";
                return c;
            }
            if (type == QueryType.HasMyTag)
            {
                c.Tags = value.ToString();
                c.CodeIdValue = "1,true";
                return c;
            }
            switch (c.FieldInfo.Type)
            {
                case FieldType.NullBit:
                case FieldType.Bit:
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                    c.CodeIdValue = value.ToString();
                    break;
                case FieldType.Date:
                case FieldType.DateSimple:
                    c.DateValue = (DateTime?)value;
                    break;
                case FieldType.Number:
                case FieldType.NullNumber:
                case FieldType.NullInteger:
                case FieldType.String:
                case FieldType.StringEqual:
                case FieldType.Integer:
                case FieldType.IntegerSimple:
                case FieldType.IntegerEqual:
                    c.TextValue = value.ToString();
                    break;
                default:
                    throw new ArgumentException("type not allowed");
            }
            return c;
        }
        public bool CanCut
        {
            get { return !IsFirst && (!IsLastNode || Parent.ParentId.HasValue); }
        }
        public bool CanRemove
        {
            get { return !IsFirst && !IsLastNode; }
        }
        public bool HasGroupBelow
        {
            get { return ParentId.HasValue && Parent.Conditions.Any(gg => gg.IsGroup); }
        }
        public class FlagItem
        {
            public string Text { get; set; }
            public int Value { get; set; }
            public string Tag { get; set; }
        }
        public static IEnumerable<FlagItem> StatusFlags(CMSDataContext db)
        {
            var q = (from c in db.Queries
                     where SqlMethods.Like(c.Name, "F[0-9][0-9]:%")
                     let t = db.Tags.SingleOrDefault(tt => tt.Name == c.Name.Substring(0, 3))
                     where t != null
                     orderby c.Name
                     select new FlagItem
                     {
                         Tag = t.Name,
                         Text = c.Name,
                         Value = t.Id
                     });
            return q;
        }
    }
}
