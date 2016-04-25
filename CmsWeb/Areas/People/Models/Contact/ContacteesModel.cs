using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;
using Dapper;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Areas.People.Models
{
    public class ContacteesModel
    {
        public Contact Contact;
        public ContacteesModel(int id)
        {
            Contact = DbUtil.Db.Contacts.Single(cc => cc.ContactId == id);
        }

        public bool CanViewComments { get; set; }
        private IQueryable<Contactee> _contactees;
        private IQueryable<Contactee> FetchContactees()
        {
            if (_contactees == null)
                _contactees = from c in DbUtil.Db.Contactees
                              where c.ContactId == Contact.ContactId
                              orderby (c.person != null ? c.person.Name2 : c.organization.OrganizationName)
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
                         ContacteeId = c.ContacteeId,
                         ContactId = Contact.ContactId,
                         TaskId = task.Id,
                         PeopleId = c.PeopleId,
                         OrganizationId = c.OrganizationId,
                         PrayedForPerson = c.PrayedForPerson ?? false,
                         ProfessionOfFaith = c.ProfessionOfFaith ?? false,
                         Name = (c.person != null ? c.person.Name : c.organization.OrganizationName)
                     };
            return q2; 
        }
        public Guid ConvertToQuery()
        {
            var c = DbUtil.Db.ScratchPadCondition();
            c.Reset();
            c.AddNewClause(QueryType.ContactRecipient, CompareType.Equal, Contact.ContactId);
            c.Save(DbUtil.Db);
            return c.Id;
        }

        public void RemoveContactee(int ContacteeId)
        {
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            cn.Execute("delete Contactees where ContacteeId = @cid",
                new {cid = ContacteeId});
        }

        public int AddTask(int PeopleId)
        {
            var uid = Util.UserPeopleId.Value;
            var task = new Task
            {
                OwnerId = uid,
                WhoId = PeopleId,
                SourceContactId = Contact.ContactId,
                Description = "Follow up",
                Notes = Contact.Comments,
                ListId = TaskModel.InBoxId(uid),
                StatusId = TaskStatusCode.Active,
                Project = Contact.MinistryId == null ? null : Contact.Ministry.MinistryName,
                ForceCompleteWContact = true
            };
            DbUtil.Db.Tasks.InsertOnSubmit(task);
            DbUtil.Db.SubmitChanges();
            return task.Id;
        }

        public int AddOrgContactee(int orgId)
        {
            // Prevent inserting duplicates
            if (DbUtil.Db.Contactees.Any(x => x.ContactId == Contact.ContactId && x.OrganizationId == orgId))
                return -1;

            var contactee = new Contactee
            {
                ContactId = Contact.ContactId,
                OrganizationId = orgId
            };

            DbUtil.Db.Contactees.InsertOnSubmit(contactee);
            DbUtil.Db.SubmitChanges();
            return contactee.ContacteeId;
        }

        public class ContactInfo
        {
            public int ContacteeId { get; set; }
            public int ContactId { get; set; }
            public int? TaskId { get; set; }
            public int? PeopleId { get; set; }
            public int? OrganizationId { get; set; }
            public bool PrayedForPerson { get; set; }
            public bool ProfessionOfFaith { get; set; }
            public string Name { get; set; }
        }
    }
}
