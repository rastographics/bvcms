using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using CmsData;
using Dapper;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class ContactorsModel
    {
        public Contact Contact;
        public ContactorsModel(int id)
        {
            Contact = CurrentDatabase.Contacts.SingleOrDefault(cc => cc.ContactId == id);
        }
        public bool CanViewComments { get; set; }
        private IQueryable<Contactor> _contactors;
        private IQueryable<Contactor> FetchContactors()
        {
            if (_contactors == null)
                _contactors = from c in CurrentDatabase.Contactors
                    where c.ContactId == Contact.ContactId
                    orderby c.person.Name2
                    select c;
            return _contactors;
        }
        int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchContactors().Count();
            return _count.Value;
        }
        public IEnumerable<ContactorInfo> Contactors()
        {
            var q = FetchContactors();
            var q2 = from c in q
                select new ContactorInfo()
                {
                    ContactId = c.ContactId,
                    PeopleId = c.PeopleId,
                    Name = c.person.Name
                };
            return q2;
        }

        public void RemoveContactor(int PeopleId)
        {
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            cn.Execute("delete Contactors where ContactId = @cid and PeopleId = @pid",
                new {cid = Contact.ContactId, pid = PeopleId});
        }
        public Guid ConvertToQuery()
        {
            var c = CurrentDatabase.ScratchPadCondition();
            c.Reset();
            c.AddNewClause(QueryType.ContactMaker, CompareType.Equal, Contact.ContactId);
            c.Save(CurrentDatabase);
            return c.Id;
        }

        public class ContactorInfo 
        {
            public int ContactId { get; set; }
            public int PeopleId { get; set; }
            public string Name { get; set; }
        }
    }
}
