using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class FailedMailModel : PagedTableModel<EmailQueueToFail, FailedMailInfo>
    {
        public int PeopleId
        {
            get { return peopleid; }
            set
            {
                peopleid = value;
                Person = DbUtil.Db.LoadPersonById(peopleid);
                var i = (from p in DbUtil.Db.People
                         where p.PeopleId == peopleid
                         select new { p.EmailAddress, p.EmailAddress2 }).Single();
                email = i.EmailAddress;
                email2 = i.EmailAddress2;
            }
        }
        private int peopleid;
        public Person Person;

        public string email;
        public string email2;

        public FailedMailModel()
            : base("Time", "desc", true)
        {}

        public override IQueryable<EmailQueueToFail> DefineModelList()
        {
            return from e in DbUtil.Db.EmailQueueToFails
                   where PeopleId == e.PeopleId
                   select e;
        }

        public override IEnumerable<FailedMailInfo> DefineViewList(IQueryable<EmailQueueToFail> q)
        {
            var isadmin = HttpContextFactory.Current.User.IsInRole("Admin");
            var isdevel = HttpContextFactory.Current.User.IsInRole("Developer");
            return from e in q
                   let et = DbUtil.Db.EmailQueueTos.SingleOrDefault(ef => ef.Id == e.Id && ef.PeopleId == e.PeopleId)
                   let eq = DbUtil.Db.EmailQueues.SingleOrDefault(ew => ew.Id == et.Id)
                   orderby e.Time descending
                   select new FailedMailInfo
                          {
                              time = e.Time,
                              eventx = e.EventX,
                              type = e.EventX == e.Bouncetype ? "" : e.Bouncetype,
                              reason = e.Reason,
                              emailid = e.Id,
                              name = et != null ? et.Person.Name : "unknown",
                              subject = et != null ? et.EmailQueue.Subject : "unknown",
                              peopleid = e.PeopleId,
                              email = e.Email,
                              devel = isdevel,
                              admin = isadmin
                          };
        }

        public override IQueryable<EmailQueueToFail> DefineModelSort(IQueryable<EmailQueueToFail> q)
        {
            switch (SortExpression)
            {
                case "Time desc":
                default:
                    return q.OrderByDescending(m => m.Time);
            }
        }
    }
}
