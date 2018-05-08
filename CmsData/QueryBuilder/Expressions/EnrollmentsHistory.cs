/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData
{
    public partial class Condition
    {
        internal Expression WasMemberAsOf()
        {
            var tf = CodeIds == "1";
            var now = Util.Now;
            EndDate = EndDate?.AddDays(1) ?? StartDate?.AddDays(1);
            Expression<Func<Person, bool>> pred = p => (
                from et in p.EnrollmentTransactions
                where et.TransactionTypeId <= 3 // things that start a change
                where et.TransactionStatus == false
                where et.TransactionDate <= EndDate // transaction starts <= looked for end
                where et.MemberTypeId != MemberTypeCode.Prospect
                where (et.Pending ?? false) == false
                where (et.NextTranChangeDate ?? now) >= StartDate // transaction ends >= looked for start
                where (OrgTypeInt ?? 0) == 0 || et.Organization.OrganizationTypeId == OrgTypeInt
                where OrganizationInt == 0 || et.OrganizationId == OrganizationInt
                where DivisionInt == 0 || et.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                where ProgramInt == 0 || et.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                select et
                ).Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression WasRecentMemberOf()
        {
            var tf = CodeIds == "1";
            var now = Util.Now;
            var dt = now.AddDays(-Days).Date;
            Expression<Func<Person, bool>> pred = p => (
                from et in p.EnrollmentTransactions
                where et.TransactionTypeId <= 3 // things that start a change
                where et.TransactionStatus == false
                where et.MemberTypeId != MemberTypeCode.Prospect
                where (et.Pending ?? false) == false
                where (et.NextTranChangeDate ?? now) >= dt // transaction ends >= looked for start
                where (OrgTypeInt ?? 0) == 0 || et.Organization.OrganizationTypeId == OrgTypeInt
                where OrganizationInt == 0 || et.OrganizationId == OrganizationInt
                where DivisionInt == 0 || et.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                where ProgramInt == 0 || et.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                select et
                ).Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression IsPreviousMemberOf()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p => (
                from et in p.EnrollmentTransactions
                where et.TransactionTypeId == 1
                where !p.OrganizationMembers.Any(mm => mm.OrganizationId == et.OrganizationId && mm.PeopleId == et.PeopleId)
                where OrganizationInt == 0 || et.OrganizationId == OrganizationInt
                where DivisionInt == 0 || et.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                where ProgramInt == 0 || et.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                select et
                ).Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression MemberTypeAsOf()
        {
            var end = EndDate?.AddDays(1) ?? StartDate?.AddDays(1);

            Expression<Func<Person, bool>> pred = p => (
                from et in p.EnrollmentTransactions
                where et.TransactionTypeId <= 3 // things that start a change
                where et.TransactionStatus == false
                where StartDate <= (et.NextTranChangeDate ?? Util.Now) // where it ends
                where et.TransactionDate <= end // where it begins
                where (et.Pending ?? false) == false
                where CodeIntIds.Contains(et.MemberTypeId)  // what it's type was during that time
                where (OrgTypeInt ?? 0) == 0 || et.Organization.OrganizationTypeId == OrgTypeInt
                where OrganizationInt == 0 || et.OrganizationId == OrganizationInt
                where DivisionInt == 0 || et.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                where ProgramInt == 0 || et.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                select et
                ).Any();
            Expression expr = Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression OrgJoinDate()
        {
            Expression<Func<Person, bool>> pred;
            switch (op)
            {
                case CompareType.Equal:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.EnrollmentDate.Value.Date == DateValue);
                    break;
                case CompareType.Greater:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.EnrollmentDate.Value.Date > DateValue);
                    break;
                case CompareType.GreaterEqual:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.EnrollmentDate.Value.Date >= DateValue);
                    break;
                case CompareType.Less:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.EnrollmentDate.Value.Date < DateValue);
                    break;
                case CompareType.LessEqual:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.EnrollmentDate.Value.Date <= DateValue);
                    break;
                case CompareType.NotEqual:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.EnrollmentDate.Value.Date != DateValue);
                    break;
                case CompareType.IsNull:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.EnrollmentDate == null);
                    break;
                case CompareType.IsNotNull:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.EnrollmentDate != null);
                    break;
                default:
                    return AlwaysFalse();
            }
            return Expression.Invoke(pred, parm);
        }
        internal Expression OrgMemberCreatedDate()
        {
            Expression<Func<Person, bool>> pred;
            switch (op)
            {
                case CompareType.Equal:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.CreatedDate.Value.Date == DateValue);
                    break;
                case CompareType.Greater:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.CreatedDate.Value.Date > DateValue);
                    break;
                case CompareType.GreaterEqual:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.CreatedDate.Value.Date >= DateValue);
                    break;
                case CompareType.Less:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.CreatedDate.Value.Date < DateValue);
                    break;
                case CompareType.LessEqual:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.CreatedDate.Value.Date <= DateValue);
                    break;
                case CompareType.NotEqual:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.CreatedDate.Value.Date != DateValue);
                    break;
                case CompareType.IsNull:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.CreatedDate == null);
                    break;
                case CompareType.IsNotNull:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && m.MemberTypeId != MemberTypeCode.Prospect
                        && ((m.Pending ?? false) == false)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.CreatedDate != null);
                    break;
                default:
                    return AlwaysFalse();
            }
            return Expression.Invoke(pred, parm);
        }
        internal Expression FirstOrgJoinDate()
        {
            IQueryable<int> q = null;
            switch (op)
            {
                case CompareType.Equal:
                    if (DateValue == null)
                        q = from p in db.People
                            where !(from et in p.EnrollmentTransactions
                                    where et.TransactionTypeId == 1
                                    && (OrganizationInt == 0 || et.OrganizationId == OrganizationInt)
                                    && (DivisionInt == 0 || et.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                                    && (ProgramInt == 0 || et.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                                    select et).Any()
                            select p.PeopleId;
                    else
                        q = from p in db.People
                            where (from et in p.EnrollmentTransactions
                                   where et.TransactionTypeId == 1
                                    && (OrganizationInt == 0 || et.OrganizationId == OrganizationInt)
                                    && (DivisionInt == 0 || et.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                                    && (ProgramInt == 0 || et.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                                   select et.TransactionDate.Date).Min() == DateValue
                            select p.PeopleId;
                    break;
                case CompareType.Greater:
                    q = from p in db.People
                        where (from et in p.EnrollmentTransactions
                               where et.TransactionTypeId == 1
                                    && (OrganizationInt == 0 || et.OrganizationId == OrganizationInt)
                                    && (DivisionInt == 0 || et.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                                    && (ProgramInt == 0 || et.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                               select et.TransactionDate.Date).Min() > DateValue
                        select p.PeopleId;
                    break;
                case CompareType.GreaterEqual:
                    q = from p in db.People
                        where (from et in p.EnrollmentTransactions
                               where et.TransactionTypeId == 1
                                    && (OrganizationInt == 0 || et.OrganizationId == OrganizationInt)
                                    && (DivisionInt == 0 || et.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                                    && (ProgramInt == 0 || et.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                               select et.TransactionDate.Date).Min() >= DateValue
                        select p.PeopleId;
                    break;
                case CompareType.Less:
                    q = from p in db.People
                        where (from et in p.EnrollmentTransactions
                               where et.TransactionTypeId == 1
                                    && (OrganizationInt == 0 || et.OrganizationId == OrganizationInt)
                                    && (DivisionInt == 0 || et.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                                    && (ProgramInt == 0 || et.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                               select et.TransactionDate.Date).Min() < DateValue
                        select p.PeopleId;
                    break;
                case CompareType.LessEqual:
                    q = from p in db.People
                        where (from et in p.EnrollmentTransactions
                               where et.TransactionTypeId == 1
                                    && (OrganizationInt == 0 || et.OrganizationId == OrganizationInt)
                                    && (DivisionInt == 0 || et.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                                    && (ProgramInt == 0 || et.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                               select et.TransactionDate.Date).Min() <= DateValue
                        select p.PeopleId;
                    break;
                case CompareType.NotEqual:
                    if (DateValue == null)
                        q = from p in db.People
                            where (from et in p.EnrollmentTransactions
                                   where et.TransactionTypeId == 1
                                    && (OrganizationInt == 0 || et.OrganizationId == OrganizationInt)
                                    && (DivisionInt == 0 || et.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                                    && (ProgramInt == 0 || et.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                                   select et).Any()
                            select p.PeopleId;
                    else
                        q = from p in db.People
                            where (from et in p.EnrollmentTransactions
                                   where et.TransactionTypeId == 1
                                    && (OrganizationInt == 0 || et.OrganizationId == OrganizationInt)
                                    && (DivisionInt == 0 || et.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                                    && (ProgramInt == 0 || et.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                                   select et.TransactionDate.Date).Min() != DateValue
                            select p.PeopleId;
                    break;
            }

            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression OrgJoinDateDaysAgo()
        {
            var days = TextValue.ToInt();
            Expression<Func<Person, int?>> pred = p =>
                SqlMethods.DateDiffDay(p.OrganizationMembers.Where(m =>
                    (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                    && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                    && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                    ).Min(m => m.EnrollmentDate), Util.Now);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Constant(days, typeof(int?));
            return Compare(left, right);
        }
        internal Expression OrgJoinDateCompare()
        {
            var prop2 = CodeIdValue;
            Expression<Func<Person, DateTime?>> pred = p =>
                p.OrganizationMembers.Where(m =>
                    (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                    && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                    && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                    ).Min(m => m.EnrollmentDate);
            Expression left = Expression.Invoke(pred, parm);
            var right = Expression.Property(parm, prop2);
            return Compare(left, right);
        }
        internal Expression OrgInactiveDate()
        {
            Expression<Func<Person, bool>> pred;
            switch (op)
            {
                case CompareType.Equal:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.InactiveDate.Value.Date == DateValue);
                    break;
                case CompareType.Greater:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.InactiveDate.Value.Date > DateValue);
                    break;
                case CompareType.GreaterEqual:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.InactiveDate.Value.Date >= DateValue);
                    break;
                case CompareType.Less:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.InactiveDate.Value.Date < DateValue);
                    break;
                case CompareType.LessEqual:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.InactiveDate.Value.Date <= DateValue);
                    break;
                case CompareType.NotEqual:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.InactiveDate.Value.Date != DateValue);
                    break;
                case CompareType.IsNull:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.InactiveDate == null);
                    break;
                case CompareType.IsNotNull:
                    pred = p => p.OrganizationMembers.Any(m =>
                        (OrganizationInt == 0 || m.OrganizationId == OrganizationInt)
                        && (DivisionInt == 0 || m.Organization.DivOrgs.Any(t => t.DivId == DivisionInt))
                        && (ProgramInt == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == ProgramInt)))
                        && m.InactiveDate != null);
                    break;
                default:
                    return AlwaysFalse();
            }
            return Expression.Invoke(pred, parm);
        }
        internal Expression OrgMemberJoinedAsOf()
        {
            var tf = CodeIds == "1";
            EndDate = EndDate?.AddDays(1) ?? StartDate?.AddDays(1);
            Expression<Func<Person, bool>> pred = p => (
                from om in p.OrganizationMembers
                where om.EnrollmentDate >= StartDate
                where om.EnrollmentDate < EndDate
                where (OrgTypeInt ?? 0) == 0 || om.Organization.OrganizationTypeId == OrgTypeInt
                where OrganizationInt == 0 || om.OrganizationId == OrganizationInt
                where DivisionInt == 0 || om.Organization.DivOrgs.Any(dg => dg.DivId == DivisionInt)
                where ProgramInt == 0 || om.Organization.DivOrgs.Any(dg => dg.Division.ProgDivs.Any(pg => pg.ProgId == ProgramInt))
                select om
                ).Any();
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
