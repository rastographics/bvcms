using CmsData;
using CmsData.Codes;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class ContacteesModel
    {
        public Contact Contact;
        public ContacteesModel() { }
        public ContacteesModel(int id)
        {
            Contact = DbUtil.Db.Contacts.Single(cc => cc.ContactId == id);
        }

        public bool CanViewComments { get; set; }
        private IQueryable<Contactee> _contactees;
        private IQueryable<Contactee> FetchContactees()
        {
            if (_contactees == null)
            {
                _contactees = from c in DbUtil.Db.Contactees
                              where c.ContactId == Contact.ContactId
                              orderby c.person.Name2
                              select c;
            }

            return _contactees;
        }

        private int? _count;
        public int Count()
        {
            if (!_count.HasValue)
            {
                _count = FetchContactees().Count();
            }

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

            if (DbUtil.Db.Setting("UseContactVisitedOrgs") &&
                DbUtil.Db.Setting("UX-ShowVisitedOrgInContactees"))
            {
                if (Contact.OrganizationId.HasValue)
                {
                    var orgContact = new ContactInfo
                    {
                        ContactId = Contact.ContactId,
                        Name = Contact.organization.OrganizationName,
                        IsOrg = true,
                        OrgId = Contact.OrganizationId.Value
                    };

                    var q3 = q2.ToList();
                    q3.Add(orgContact);

                    return q3;
                }
            }

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

        public void RemoveContactee(int peopleId)
        {
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            cn.Execute("delete Contactees where ContactId = @cid and PeopleId = @pid",
                new { cid = Contact.ContactId, pid = peopleId });
        }

        public int AddTask(int peopleId)
        {
            if (!Util.UserPeopleId.HasValue)
            {
                throw new Exception("missing user on AddFollowup Task");
            }

            var uid = Util.UserPeopleId.Value;
            var task = new CmsData.Task
            {
                OwnerId = uid,
                WhoId = peopleId,
                ListId = CmsData.Task.GetRequiredTaskList(DbUtil.Db, "InBox", uid).Id,
                SourceContactId = Contact.ContactId,
                Description = "Follow up",
                Notes = Contact.Comments,
                StatusId = TaskStatusCode.Active,
                Project = Contact.MinistryId == null ? null : Contact.Ministry.MinistryName,
                ForceCompleteWContact = true
            };
            DbUtil.Db.Tasks.InsertOnSubmit(task);
            DbUtil.Db.SubmitChanges();
            return task.Id;
        }

        public class ContactInfo
        {
            public int ContactId { get; set; }
            public int? TaskId { get; set; }
            public int PeopleId { get; set; }
            public bool IsOrg { get; set; }
            public int OrgId { get; set; }
            public bool PrayedForPerson { get; set; }
            public bool ProfessionOfFaith { get; set; }
            public string Name { get; set; }
        }
    }
}
