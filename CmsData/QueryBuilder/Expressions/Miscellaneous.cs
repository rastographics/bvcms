/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using CmsData.API;
using Dapper;
using UtilityExtensions;

namespace CmsData
{
    public partial class Condition
    {
        internal Expression StatusFlag()
        {
            var codes0 = CodeValues.Split(',').Select(ff => ff.Trim()).ToList();
            var q = db.ViewStatusFlagLists.ToList();
            if (!db.FromBatch)
                q = (from f in q
                     where f.RoleName == null || db.CurrentRoles().Contains(f.RoleName)
                     select f).ToList();
            var codes = (from f in q
                         join c in codes0 on f.Flag equals c into j
                         from c in j
                         select c).ToList();
            Expression<Func<Person, bool>> pred =
                p => p.Tags.Any(tt => codes.Contains(tt.Tag.Name) && tt.Tag.TypeId == DbUtil.TagTypeId_StatusFlags);
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression QueryTag()
        {
            var names = CodeText.Split(',');
            var q = (from t in db.Tags
                     where t.TypeId == DbUtil.TagTypeId_QueryTags
                     where names.Contains(t.Name)
                     select t.Name).ToList();

            Expression<Func<Person, bool>> pred =
                p => p.Tags.Any(tt => q.Contains(tt.Tag.Name) && tt.Tag.TypeId == DbUtil.TagTypeId_QueryTags);
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }

        internal Expression HasCurrentTag()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.Tags.Any(t => t.Tag.Name == db.CurrentTagName && t.Tag.PeopleId == db.CurrentTagOwnerId);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }

        internal Expression HasMyTag()
        {
            var tf = CodeIds == "1";
            var a = (Tags ?? "").Split(';').Select(s => s.Split(',')[0].ToInt()).ToArray();
            Expression<Func<Person, bool>> pred = p =>
                    p.Tags.Any(t => a.Contains(t.Id));
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }

        internal Expression HasMemberDocs()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                    p.MemberDocForms.Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }

        internal Expression SavedQuery2()
        {
            var tf = CodeIds == "1";
            var a = SavedQuery.Split(":".ToCharArray(), 2);
            Guid queryId;
            Guid.TryParse(a[0], out queryId);
            var q2 = db.PeopleQuery(queryId);
            if (q2 == null)
                return AlwaysFalse();
            var tag = db.PopulateTemporaryTag(q2.Select(pp => pp.PeopleId));

            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }

        internal Expression InSqlList()
        {
            var tf = CodeIds == "1";
            var scriptname = Quarters;
            var cn = db.ReadonlyConnection();

            var sql = db.ContentOfTypeSql(scriptname);
            if (!sql.HasValue())
                return AlwaysFalse();


            var list = cn.Query<int>(sql).ToList();

            var tag = db.PopulateTempTag(list);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RecActiveOtherChurch()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> hasapp = p => p.RecRegs.Count() > 0;
            Expression<Func<Person, bool>> pred = p =>
                    p.RecRegs.Any(v => v.ActiveInAnotherChurch == true)
                    && p.RecRegs.Any();
            Expression expr1 = Expression.Convert(Expression.Invoke(hasapp, parm), typeof(bool));
            Expression expr2 = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr2 = Expression.Not(expr2);
            return Expression.And(expr1, expr2);
        }
        internal Expression RecInterestedCoaching()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> hasapp = p => p.RecRegs.Count() > 0;
            Expression<Func<Person, bool>> pred = p =>
                    p.RecRegs.Any(v => v.Coaching == true)
                    && p.RecRegs.Any();
            Expression expr1 = Expression.Convert(Expression.Invoke(hasapp, parm), typeof(bool));
            Expression expr2 = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr2 = Expression.Not(expr2);
            return Expression.And(expr1, expr2);
        }
        internal Expression InOneOfMyOrgs()
        {
            var tf = CodeIds == "1";
            var uid = Util.UserPeopleId;
            Expression<Func<Person, bool>> pred = p =>
                p.OrganizationMembers.Any(m =>
                    db.OrganizationMembers.Any(um =>
                        um.OrganizationId == m.OrganizationId && um.PeopleId == uid)
                );
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression CheckInVisits()
        {
            var visits = TextValue.ToInt();
            Expression<Func<Person, bool>> pred = null;
            switch (op)
            {
                case CompareType.Greater:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() > visits
                                 select g).Any();
                    break;
                case CompareType.Less:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() < visits
                                 select g).Any();
                    break;
                case CompareType.GreaterEqual:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() >= visits
                                 select g).Any();
                    break;
                case CompareType.LessEqual:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() <= visits
                                 select g).Any();
                    break;
                case CompareType.Equal:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() == visits
                                 select g).Any();
                    break;
                case CompareType.NotEqual:
                    pred = p => (from e in p.CheckInTimes
                                 where e.AccessTypeID == 2
                                 group e by e.PeopleId into g
                                 where g.Count() != visits
                                 select g).Any();
                    break;
            }

            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression MedicalLength()
        {
            var len = TextValue.ToInt();
            if (len == 0 && op == CompareType.Equal)
            {
                Expression<Func<Person, bool>> pp = p =>
                    !p.RecRegs.Any() || p.RecRegs.First().MedicalDescription == null ||
                    p.RecRegs.First().MedicalDescription.Length == 0;
                return Expression.Invoke(pp, parm);
            }
            Expression<Func<Person, int>> pred = p => p.RecRegs.Sum(rr => rr.MedicalDescription.Length);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Convert(Expression.Constant(len), left.Type);
            return Compare(left, right);
        }
        internal Expression HasFailedEmails()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                db.EmailQueueToFails.Any(f => f.PeopleId == p.PeopleId && (f.Time >= StartDate || StartDate == null));
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression RecentFlagAdded()
        {
            var codes0 = CodeValues.Split(',').Select(ff => ff.Trim()).ToList();
            var q = db.ViewStatusFlagLists.ToList();
            if (!db.FromBatch)
                q = (from f in q
                     where f.RoleName == null || db.CurrentRoles().Contains(f.RoleName)
                     select f).ToList();
            var codes = (from f in q
                         join c in codes0 on f.Flag equals c into j
                         from c in j
                         select c).ToList();
            var now = Util.Now;
            var dt = now.AddDays(-Days).Date;

            Expression<Func<Person, bool>> pred = p => (
                    from t in p.Tags
                    where codes.Contains(t.Tag.Name)
                    where t.Tag.TypeId == DbUtil.TagTypeId_StatusFlags
                    where t.DateCreated >= dt
                    select t
                    ).Any();
            Expression expr = Expression.Invoke(pred, parm); // substitute parm for p
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression DaysSinceDate()
        {
            var days = Util.Now.Date.Subtract(StartDate ?? SqlDateTime.MinValue.Value).Days;
            var left = Expression.Constant(days, typeof(int?));
            var right = Expression.Constant(TextValue.ToInt(), typeof(int?));
            return Compare(left, right);
        }
        internal Expression DaysSinceDateField()
        {
            var prop2 = CodeIdValue;
            Expression<Func<Person, int?>> pred = null;
            switch (prop2)
            {
                case "JoinDate":
                    pred = p => SqlMethods.DateDiffDay(p.JoinDate, Util.Now.Date);
                    break;
                case "NewMemberClassDate":
                    pred = p => SqlMethods.DateDiffDay(p.NewMemberClassDate, Util.Now.Date);
                    break;
                case "WeddingDate":
                    pred = p => SqlMethods.DateDiffDay(p.WeddingDate, Util.Now.Date);
                    break;
                case "BaptismDate":
                    pred = p => SqlMethods.DateDiffDay(p.BaptismDate, Util.Now.Date);
                    break;
                case "DecisionDate":
                    pred = p => SqlMethods.DateDiffDay(p.DecisionDate, Util.Now.Date);
                    break;
                case "DropDate":
                    pred = p => SqlMethods.DateDiffDay(p.DropDate, Util.Now.Date);
                    break;
                default:
                    throw new Exception("unknown property " + prop2);
            }
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(Days, typeof(int?));
            return Compare(left, right);
        }
    }
}
