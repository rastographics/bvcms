using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using UtilityExtensions;
using System.Linq.Expressions;
namespace CmsData
{
    public partial class QueryBuilderClause2
    {
        public Dictionary<Guid, QueryBuilderClause2> AllClauses { get; set; }

        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public int Order { get; set; }
        public string Field { get; set; }
        public string Comparison { get; set; }
        public string TextValue { get; set; }
        public DateTime? DateValue { get; set; }
        public string CodeIdValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Program { get; set; }
        public int Division { get; set; }
        public int Organization { get; set; }
        public int Days { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Quarters { get; set; }
        public string SavedQueryIdDesc { get; set; }
        public string Tags { get; set; }
        public int Schedule { get; set; }
        public int? Age { get; set; }
        public int? Campus { get; set; }
        public int? OrgType { get; set; }
        public string From { get; set; }
        public IEnumerable<QueryBuilderClause2> Clauses
        {
            get
            {
                var q = from c in AllClauses.Values
                        where c.Id == ParentId
                        orderby c.Order
                        select c;
                return q;
            }
        }
        public QueryBuilderClause2 Parent
        {
            get
            {
                if (ParentId.HasValue)
                    return AllClauses[ParentId.Value];
                return null;
            }
        }
        private FieldClass2 _FieldInfo;
        public FieldClass2 FieldInfo
        {
            get
            {
                try
                {
                    if ((_FieldInfo == null || _FieldInfo.Name != Field))
                        _FieldInfo = FieldClass2.Fields[Field];
                    return _FieldInfo;
                }
                catch (Exception)
                {
                    throw new Exception("QB Field not found: " + Field);
                }
            }
        }
        public void SetComparisonType(CompareType value)
        {
            Comparison = value.ToString();
        }
        public void SetQueryType(QueryType value)
        {
            Field = value.ToString();
        }
        public CompareType ComparisonType
        {
            get { return CompareClass2.Convert(Comparison); }
        }
        private CompareClass2 _Compare;
        public CompareClass2 Compare
        {
            get
            {
                if (_Compare == null)
                    _Compare = CompareClass2.Comparisons.SingleOrDefault(cm =>
                        cm.FieldType == FieldInfo.Type && cm.CompType == ComparisonType);
                return _Compare;
            }
        }
        public override string ToString()
        {
            if (Field == "MatchAnything")
                return "Match Anything";
            if (!IsGroup)
                if (Compare != null)
                    return Compare.ToString(this);
            switch (ComparisonType)
            {
                case CompareType.AllTrue:
                    return "Match ALL of the conditions below";
                case CompareType.AnyTrue:
                    return "Match ANY of the conditions below";
                case CompareType.AllFalse:
                    return "Match NONE of the conditions below";
            }
            return "null";
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
        private bool includeDeceased = false;
        public bool ParentsOf { get; set; }
        public Expression<Func<Person, bool>> Predicate(CMSDataContext db)
        {
            db.CopySession();
            var parm = Expression.Parameter(typeof(Person), "p");
            var tree = ExpressionTree(parm, db);
            if (tree == null)
                tree = Expressions.CompareConstant(parm, "PeopleId", CompareType.NotEqual, 0);
            if (includeDeceased == false)
                tree = Expression.And(tree, Expressions.CompareConstant(parm,
                                        "IsDeceased", CompareType.NotEqual, true));
            if (Util2.OrgMembersOnly)
                tree = Expression.And(OrgMembersOnly(db, parm), tree);
            else if (Util2.OrgLeadersOnly)
                tree = Expression.And(OrgLeadersOnly(db, parm), tree);
            return Expression.Lambda<Func<Person, bool>>(tree, parm);
        }
        private Expression OrgMembersOnly(CMSDataContext db, ParameterExpression parm)
        {
            var tag = db.OrgMembersOnlyTag2();
            Expression<Func<Person, bool>> pred = p =>
                p.Tags.Any(t => t.Id == tag.Id);
            return Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
        }
        private Expression OrgLeadersOnly(CMSDataContext db, ParameterExpression parm)
        {
            var tag = db.OrgLeadersOnlyTag2();
            Expression<Func<Person, bool>> pred = p =>
                p.Tags.Any(t => t.Id == tag.Id);
            return Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
        }
        private bool InAllAnyFalse
        {
            get { return Parent.IsGroup && Parent.ComparisonType == CompareType.AllFalse; }
        }
        private bool AnyFalseTrue
        {
            get { return ComparisonType == CompareType.AnyTrue; }
        }
        private Expression ExpressionTree(ParameterExpression parm, CMSDataContext Db)
        {
            Expression expr = null;
            if (IsGroup)
            {
                foreach (var clause in Clauses)
                    if (expr == null)
                        expr = clause.ExpressionTree(parm, Db);
                    else
                    {
                        var right = clause.ExpressionTree(parm, Db);
                        if (right != null && expr != null)
                            if (AnyFalseTrue)
                                expr = Expression.Or(expr, right);
                            else
                                expr = Expression.And(expr, right);
                    }
                return expr;
            }
            expr = Compare.Expression(this, parm, Db);
            if (InAllAnyFalse)
                expr = Expression.Not(expr);
            return expr;
        }
        public bool HasMultipleCodes
        {
            get
            {
                if (Field == "MatchAnything")
                    return false;
                var e = Compare;
                if (e == null)
                    return false;
                return e.CompType == CompareType.OneOf
                    || e.CompType == CompareType.NotOneOf;
            }
        }
        private bool IsCode
        {
            get
            {
                var e = Compare;
                return e.FieldType == FieldType.Bit
                    || e.FieldType == FieldType.NullBit
                    || e.FieldType == FieldType.Code
                    || e.FieldType == FieldType.NullCode
                    || e.FieldType == FieldType.CodeStr
                    || e.FieldType == FieldType.DateField;
            }
        }
        private enum Part { Id = 0, Code = 1 }
        private string GetCodeIdValuePart(string value, Part part)
        {
            if (value != null && value.Contains(","))
                return value.SplitStr(",", 2)[(int)part];
            return value;
        }
        internal string CodeValues
        {
            get
            {
                if (IsCode)
                    if (HasMultipleCodes)
                        return string.Join(", ", (from s in CodeIdValue.SplitStr(";")
                                                  select GetCodeIdValuePart(s, Part.Code)).ToArray());
                    else
                        return GetCodeIdValuePart(CodeIdValue, Part.Code);
                return "";
            }
        }
        internal string CodeIds
        {
            get
            {
                if (IsCode)
                    if (HasMultipleCodes)
                    {
                        var q = from s in CodeIdValue.SplitStr(";")
                                select GetCodeIdValuePart(s, Part.Id);
                        return string.Join(",", q.ToArray());
                    }
                    else
                        return GetCodeIdValuePart(CodeIdValue, Part.Id);
                return "";
            }
        }
        internal int[] CodeIntIds
        {
            get
            {
                if (IsCode)
                    if (HasMultipleCodes)
                    {
                        var q = from s in CodeIdValue.SplitStr(";")
                                select GetCodeIdValuePart(s, Part.Id).ToInt();
                        return q.ToArray();
                    }
                    else
                        return new int[] { GetCodeIdValuePart(CodeIdValue, Part.Id).ToInt() };
                return null;
            }
        }
        internal string[] CodeStrIds
        {
            get
            {
                if (IsCode)
                    if (HasMultipleCodes)
                    {
                        var q = from s in CodeIdValue.SplitStr(";")
                                select GetCodeIdValuePart(s, Part.Id);
                        return q.ToArray();
                    }
                    else
                        return new string[] { GetCodeIdValuePart(CodeIdValue, Part.Id) };
                return null;
            }
        }
        public bool IsFirst
        {
            get { return IsGroup && Parent == null; }
        }
        public bool IsGroup
        {
            get { return FieldInfo.Type == FieldType.Group; }
        }
        public bool IsLastNode
        {
            get { return Parent == null || Parent.Clauses.Count() == 1; }
        }
        public QueryBuilderClause2 Clone(QueryBuilderClause2 parent = null, Guid? useGuid = null)
        {
            var newclause = new QueryBuilderClause2();
            if (useGuid.HasValue)
                newclause.Id = useGuid.Value;
            else
                newclause.Id = Guid.NewGuid();
            newclause.CopyFrom(this);
            foreach (var c in Clauses)
            {
                var nc = c.Clone(newclause);
                nc.ParentId = newclause.Id;
                newclause.AllClauses.Add(nc.Id, nc);
            }
            return newclause;
        }
        private void CopyFrom(QueryBuilderClause2 from)
        {
            Id = Guid.NewGuid();
            Age = from.Age;
            Order = from.Order;
            CodeIdValue = from.CodeIdValue;
            Comparison = from.Comparison;
            DateValue = from.DateValue;
            Days = from.Days;
            Description = from.Description;
            Division = from.Division;
            EndDate = from.EndDate;
            Field = from.Field;
            Organization = from.Organization;
            Program = from.Program;
            Quarters = from.Quarters;
            SavedQueryIdDesc = from.SavedQueryIdDesc;
            Schedule = from.Schedule;
            StartDate = from.StartDate;
            Tags = from.Tags;
            TextValue = from.TextValue;
        }
//        public void CopyFromAll(QueryBuilderClause2 from)
//                c.DeleteClause();
        public void DeleteClause()
        {
            var allClauses = AllClauses;
            foreach (var c in Clauses)
                c.DeleteClause();
            allClauses.Remove(Id);
        }
        //        public void CleanSlate(CMSDataContext Db)
        //        {
        //            foreach (var c in Clauses)
        //                DeleteClause(c, Db);
        //            SetQueryType(QueryType.Group);
        //            SetComparisonType(CompareType.AllTrue);
        //            Db.SubmitChanges();
        //        }
        //        public int CleanSlate2(CMSDataContext Db)
        //        {
        //            foreach (var c in Clauses)
        //                DeleteClause(c, Db);
        //            SetQueryType(QueryType.Group);
        //            SetComparisonType(CompareType.AllTrue);
        //            var nc = AddNewClause(QueryType.MatchAnything, CompareType.Equal, null);
        //            Db.SubmitChanges();
        //            return nc.QueryId;
        //        }

        public static QueryBuilderClause2 CreateNewClause(QueryType type, CompareType compare, QueryBuilderClause2 parent = null)
        {
            var c = new QueryBuilderClause2 { Id = Guid.NewGuid(), };
            c.SetQueryType(type);
            c.SetComparisonType(compare);
            if (parent != null)
            {
                c.ParentId = parent.Id;
                c.Order = parent.MaxClauseOrder() + 2;
            }
            return c;
        }
        public QueryBuilderClause2 CreateNewGroupClause()
        {
            var c = new QueryBuilderClause2 { Id = Guid.NewGuid() };
            c.SetQueryType(QueryType.Group);
            c.SetComparisonType(CompareType.AllTrue);
            return c;
        }
        public int MaxClauseOrder()
        {
            return Clauses.Max(qc => qc.Order);
        }
        public void ReorderClauses()
        {
            var q = from c in Clauses
                    orderby c.Order
                    select c;
            int n = 1;
            foreach (var c in q)
            {
                c.Order = n;
                n += 2;
            }
        }
        public QueryBuilderClause2 AddNewClause(QueryType type, CompareType op, object value)
        {
            var c = new QueryBuilderClause2();
            c.SetQueryType(type);
            AllClauses.Add(c.Id, c);
            c.ParentId = Id;
            c.SetComparisonType(op);
            if (type == QueryType.MatchAnything)
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
        //        public QueryBuilderClause2 SaveTo(CMSDataContext db, string name, string user, bool ispublic)
        //        {
        //            var saveto = new QueryBuilderClause2();
        //            db.QueryBuilderClauses.InsertOnSubmit(saveto);
        //            saveto.CopyFromAll(this, db);
        //            saveto.SavedBy = user;
        //            saveto.Description = name;
        //            saveto.IsPublic = ispublic;
        //            db.SubmitChanges();
        //            return saveto;
        //        }
        public bool CanCut
        {
            get { return !IsFirst && (!IsLastNode || Parent.Parent != null); }
        }
        public bool CanRemove
        {
            get { return !IsFirst && !IsLastNode; }
        }
        public bool HasGroupBelow
        {
            get { return Parent != null && Parent.Clauses.Any(gg => gg.IsGroup); }
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
                     select new FlagItem()
                         {
                             Tag = t.Name,
                             Text = c.Name,
                             Value = t.Id
                         });
            return q;
        }
    }
}
