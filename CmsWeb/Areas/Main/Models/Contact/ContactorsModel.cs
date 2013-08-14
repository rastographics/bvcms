using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CmsData;
using Dapper;
using UtilityExtensions;

namespace CmsWeb.Models.ContactPage
{
    public class ContactorsModel
    {
        public CmsData.Contact Contact;
        public ContactorsModel(int id)
        {
            Contact = DbUtil.Db.Contacts.Single(cc => cc.ContactId == id);
        }
        private IQueryable<Contactor> _contactors;
        private IQueryable<Contactor> FetchContactors()
        {
            if (_contactors == null)
                _contactors = from c in DbUtil.Db.Contactors
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

        public class ContactorInfo 
        {
            public int ContactId { get; set; }
            public int PeopleId { get; set; }
            public string Name { get; set; }
        }
    }
}
