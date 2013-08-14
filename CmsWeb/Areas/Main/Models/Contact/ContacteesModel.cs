using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using CmsData;
using Dapper;
using UtilityExtensions;

namespace CmsWeb.Models.ContactPage
{
    public class ContacteesModel
    {
        public CmsData.Contact Contact;
        public PagerModel2 Pager { get; set; }
        public ContacteesModel(int id)
        {
            Contact = DbUtil.Db.Contacts.Single(cc => cc.ContactId == id);
            Pager = new PagerModel2(Count);
        }
        private IQueryable<Contactee> _contactees;
        private IQueryable<Contactee> FetchContactees()
        {
            if (_contactees == null)
                _contactees = from c in DbUtil.Db.Contactees
                              where c.ContactId == Contact.ContactId
                              orderby c.person.Name2
                              select c;
            return _contactees;
        }
        int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchContactees().Count();
            return _count.Value;
        }
        public IEnumerable<ContactInfo> Contactees()
        {
            var q = FetchContactees();
            var q2 = from c in q
                     let task = DbUtil.Db.Tasks.FirstOrDefault(t =>
                          t.WhoId == c.PeopleId && t.SourceContactId == Contact.ContactId)
                     select new ContactInfo
                     {
                         ContactId = Contact.ContactId,
                         TaskId = task.Id,
                         PeopleId = c.PeopleId,
                         PrayedForPerson = c.PrayedForPerson ?? false,
                         ProfessionOfFaith = c.ProfessionOfFaith ?? false,
                         Name = c.person.Name
                     };
            return q2.Skip(Pager.StartRow).Take(Pager.PageSize);
        }

        public void RemoveContactee(int PeopleId)
        {
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            cn.Execute("delete Contactees where ContactId = @cid and PeopleId = @pid",
                new {cid = Contact.ContactId, pid = PeopleId});
        }

        public class ContactInfo
        {
            public int ContactId { get; set; }
            public int? TaskId { get; set; }
            public int PeopleId { get; set; }
            public bool PrayedForPerson { get; set; }
            public bool ProfessionOfFaith { get; set; }
            public string Name { get; set; }
        }
    }
}
