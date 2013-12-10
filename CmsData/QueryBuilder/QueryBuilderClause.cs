/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using Community.CsharpSqlite;
using IronPython.Modules;
using UtilityExtensions;
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace CmsData
{
    public partial class QueryBuilderClause
    {
        private FieldClass _FieldInfo;
        public FieldClass FieldInfo
        {
            get
            {
                try
                {
                    if ((_FieldInfo == null || _FieldInfo.Name != Field))
                        _FieldInfo = FieldClass.Fields[Field];
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
            get { return CompareClass.Convert(Comparison); }
        }
        private CompareClass _Compare;
        public CompareClass Compare
        {
            get
            {
                if (_Compare == null)
                    _Compare = CompareClass.Comparisons.SingleOrDefault(cm =>
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

        public List<int> ReturnFamilyMemberTypeCodes { get; set; }
        public void AddFamilyMemberTypeCode(int code)
        {
            if(ReturnFamilyMemberTypeCodes == null)
                ReturnFamilyMemberTypeCodes = new List<int>();
            if (!ReturnFamilyMemberTypeCodes.Contains(code))
                ReturnFamilyMemberTypeCodes.Add(code);
        }
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
            //db.TaggedPeople(tag.Id).Select(t => t.PeopleId).Contains(p.PeopleId);
            return Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
        }
        private Expression OrgLeadersOnly(CMSDataContext db, ParameterExpression parm)
        {
            var tag = db.OrgLeadersOnlyTag2();
            Expression<Func<Person, bool>> pred = p =>
                p.Tags.Any(t => t.Id == tag.Id);
            //db.TaggedPeople(tag.Id).Select(t => t.PeopleId).Contains(p.PeopleId);
            return Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
        }
        private bool InAllAnyFalse
        {
            get
            {
                return Parent.IsGroup && Parent.ComparisonType == CompareType.AllFalse;
            }
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
                foreach (var clause in Clauses.OrderBy(c => c.ClauseOrder))
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
        public int MaxClauseOrder()
        {
            int max = 0;
            if (Clauses.Count() > 0)
                max = Clauses.Max(qc => qc.ClauseOrder);
            return max;
        }
        public void ReorderClauses()
        {
            var q = from c in Clauses
                    orderby c.ClauseOrder
                    select c;
            int n = 1;
            foreach (var c in q)
            {
                c.ClauseOrder = n;
                n += 2;
            }
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
            get { return Parent == null || Parent.Clauses.Count == 1; }
        }
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            switch (action)
            {
                case System.Data.Linq.ChangeAction.Insert:
                case System.Data.Linq.ChangeAction.Update:
                    CreatedOn = Util.Now;
                    break;
            }
        }
        public QueryBuilderClause Clone(CMSDataContext Db)
        {
            var q = new QueryBuilderClause();
            q.CopyFrom(this);
            Db.QueryBuilderClauses.InsertOnSubmit(q);
            foreach (var c in Clauses)
                q.Clauses.Add(c.Clone(Db));
            return q;
        }
        private void CopyFrom(QueryBuilderClause from)
        {
            Age = from.Age;
            ClauseOrder = from.ClauseOrder;
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
            OrgType = from.OrgType;
            SavedQueryIdDesc = from.SavedQueryIdDesc;
            Schedule = from.Schedule;
            StartDate = from.StartDate;
            Tags = from.Tags;
            TextValue = from.TextValue;
        }
        public void CopyFromAll(QueryBuilderClause from, CMSDataContext Db)
        {
            foreach (var c in Clauses)
                DeleteClause(c, Db);
            CopyFrom(from);
            foreach (var c in from.Clauses)
                Clauses.Add(c.Clone(Db));
        }
        private void DeleteClause(QueryBuilderClause qb, CMSDataContext Db)
        {
            foreach (var c in qb.Clauses)
                DeleteClause(c, Db);
            Db.QueryBuilderClauses.DeleteOnSubmit(qb);
        }
        public void CleanSlate(CMSDataContext Db)
        {
            foreach (var c in Clauses)
                DeleteClause(c, Db);
            SetQueryType(QueryType.Group);
            SetComparisonType(CompareType.AllTrue);
            Db.SubmitChanges();
        }
        public int CleanSlate2(CMSDataContext Db)
        {
            foreach (var c in Clauses)
                DeleteClause(c, Db);
            SetQueryType(QueryType.Group);
            SetComparisonType(CompareType.AllTrue);
            var nc = AddNewClause(QueryType.MatchAnything, CompareType.Equal, null);
            Db.SubmitChanges();
            return nc.QueryId;
        }
        public static QueryBuilderClause NewGroupClause()
        {
            var qb = new QueryBuilderClause();
            qb.SetQueryType(QueryType.Group);
            qb.SetComparisonType(CompareType.AllTrue);
            return qb;
        }
        public QueryBuilderClause AddNewGroupClause(CompareType op)
        {
            var qb = new QueryBuilderClause();
            qb.ClauseOrder = qb.MaxClauseOrder() + 1;
            qb.SetQueryType(QueryType.Group);
            this.Clauses.Add(qb);
            qb.SetComparisonType(op);
            return qb;
        }
        public QueryBuilderClause AddNewClause(QueryType type, CompareType op, object value)
        {
            var qb = new QueryBuilderClause();
            qb.ClauseOrder = qb.MaxClauseOrder() + 1;
            qb.SetQueryType(type);
            this.Clauses.Add(qb);
            qb.SetComparisonType(op);
            if (type == QueryType.MatchAnything)
            {
                qb.CodeIdValue = "1,true";
                return qb;
            }
            if (type == QueryType.HasMyTag)
            {
                qb.Tags = value.ToString();
                qb.CodeIdValue = "1,true";
                return qb;
            }
            switch (qb.FieldInfo.Type)
            {
                case FieldType.NullBit:
                case FieldType.Bit:
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                    qb.CodeIdValue = value.ToString();
                    break;
                case FieldType.Date:
                case FieldType.DateSimple:
                    qb.DateValue = (DateTime?)value;
                    break;
                case FieldType.Number:
                case FieldType.NullNumber:
                case FieldType.NullInteger:
                case FieldType.String:
                case FieldType.StringEqual:
                case FieldType.Integer:
                case FieldType.IntegerSimple:
                case FieldType.IntegerEqual:
                    qb.TextValue = value.ToString();
                    break;
                default:
                    throw new ArgumentException("type not allowed");
            }
            return qb;
        }
        public QueryBuilderClause SaveTo(CMSDataContext db, string name, string user, bool ispublic)
        {
            var saveto = new QueryBuilderClause();
            db.QueryBuilderClauses.InsertOnSubmit(saveto);
            saveto.CopyFromAll(this, db);
            saveto.SavedBy = user;
            saveto.Description = name;
            saveto.IsPublic = ispublic;
            db.SubmitChanges();
            return saveto;
        }

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
            var q = (from c in db.QueryBuilderClauses
                     where c.GroupId == null && c.Field == "Group"
                     where SqlMethods.Like(c.Description, "F[0-9][0-9]:%")
                     let t = db.Tags.SingleOrDefault(tt => tt.Name == c.Description.Substring(0, 3))
                     where t != null
                     orderby c.Description
                     select new FlagItem()
                         {
                             Tag = t.Name,
                             Text = c.Description,
                             Value = t.Id
                         });
            return q;
        }
    }
}
