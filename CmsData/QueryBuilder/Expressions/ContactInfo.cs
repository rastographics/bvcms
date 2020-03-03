using System;
using System.Linq;
using System.Linq.Expressions;

namespace CmsData
{
    public partial class Condition
    {
        internal Expression HasInvalidEmailAddress()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                (db.IsValidEmail(p.EmailAddress ?? "") == false
                || db.IsValidEmail(p.EmailAddress2 ?? "") == false);

            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
        internal Expression SpouseHasEmail()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(pp => pp.PeopleId == p.SpouseId && pp.EmailAddress.Contains("@"));
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }

        internal Expression SpouseOrHeadWithEmail()
        {
            var q = db.ViewSpouseOrHeadWithEmails.Select(p => p.PeopleId.Value);
            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression HeadOrSpouseWithEmail()
        {
            var q = db.ViewHeadOrSpouseWithEmails.Select(p => p.PeopleId.Value);
            var tag = db.PopulateTemporaryTag(q);
            Expression<Func<Person, bool>> pred = p => p.Tags.Any(t => t.Id == tag.Id);
            Expression expr = Expression.Invoke(pred, parm);
            return expr;
        }
        internal Expression HasZipPlus4()
        {
            var tf = CodeIds == "1";
            Expression<Func<Person, bool>> pred = p =>
                p.Family.People.Any(pp => pp.PrimaryZip.Length >= 9);
            Expression expr = Expression.Convert(Expression.Invoke(pred, parm), typeof(bool));
            if (op == CompareType.Equal ^ tf)
                expr = Expression.Not(expr);
            return expr;
        }
    }
}
