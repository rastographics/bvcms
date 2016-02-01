using System.Collections.Generic;
using System.Linq;
using CmsData.API;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public void CreateTask(int ministerId, int aboutId, string description)
        {
            using (var db2 = NewDataContext())
            {
                var about = db2.LoadPersonById(aboutId);
                var minister = db2.LoadPersonById(ministerId);
                var t = new Task
                {
                    OwnerId = ministerId,
                    Description = description,
                    ForceCompleteWContact = true,
                    ListId = Task.GetRequiredTaskList(db2, "InBox", ministerId).Id,
                    StatusId = TaskStatusCode.Active,
                    WhoId = aboutId,
                };
                db2.Tasks.InsertOnSubmit(t);
                db2.SubmitChanges();
                db2.Email(
                    db2.Setting("AdminMail", "support@touchpointsoftware.com"), // from email
                    minister, // to person
                    "TASK: " + description, // subject
                    Task.TaskLink(db2, description, t.Id) + "<br/>" + about.Name // body
                );
                db2.SubmitChanges();
            }
        }

        public APIPerson.Person GetPerson(object pid)
        {
            var api = new APIPerson(db);
            var p = api.GetPersonData(pid.ToInt());
            return p;
        }

        public APIPerson.Person GetSpouse(object pid)
        {
            var p1 = db.LoadPersonById(pid.ToInt());
            if (p1 == null)
                return null;
            var api = new APIPerson(db);
            var p = api.GetPersonData(p1.SpouseId ?? 0);
            return p;
        }

        public List<int> PeopleIds(object savedQuery)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            return list;
        }

        public void UpdateCampus(object savedQuery, string campus)
        {
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(savedQuery);
                var id = db2.FetchOrCreateCampusId(campus);
                foreach (var p in q)
                {
                    p.UpdateValue("CampusId", id);
                    p.LogChanges(db2);
                    db2.SubmitChanges();
                }
            }
        }

        public void UpdateField(Person p, string field, object value)
        {
            using (var db2 = NewDataContext())
            {
                var pp = db2.LoadPersonById(p.PeopleId);
                pp.UpdateValue(field, value);
                db2.SubmitChanges();
            }
        }

        public void UpdateMemberStatus(object savedQuery, string status)
        {
            using (var db2 = NewDataContext())
            {
                var id = Person.FetchOrCreateMemberStatus(db2, status);
                var q = db2.PeopleQuery2(savedQuery);
                foreach (var p in q)
                {
                    p.UpdateValue("MemberStatusId", id);
                    p.LogChanges(db2);
                    db2.SubmitChanges();
                }
            }
        }

        public void UpdateNamedField(object savedQuery, string field, object value)
        {
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(savedQuery);
                foreach (var p in q)
                {
                    p.UpdateValue(field, value);
                    p.LogChanges(db2);
                    db2.SubmitChanges();
                }
            }
        }

        public void UpdateNewMemberClassDate(object savedQuery, object dt)
        {
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(savedQuery);
                foreach (var p in q)
                {
                    p.UpdateValue("NewMemberClassDate", dt);
                    p.LogChanges(db2);
                    db2.SubmitChanges();
                }
            }
        }

        public void UpdateNewMemberClassDateIfNullForLastAttended(object savedQuery, object orgId)
        {
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(savedQuery);
                foreach (var p in q)
                {
                    // skip any who already have their new member class date set
                    if (p.NewMemberClassDate.HasValue)
                        continue;

                    // get the most recent attend date
                    var lastAttend = p.Attends
                                      .Where(x => x.OrganizationId == orgId.ToInt() && x.AttendanceFlag)
                                      .OrderByDescending(x => x.MeetingDate)
                                      .FirstOrDefault();

                    if (lastAttend != null)
                    {
                        p.UpdateValue("NewMemberClassDate", lastAttend.MeetingDate);
                        p.LogChanges(db2);
                        db2.SubmitChanges();
                    }
                }
            }
        }

        public void UpdateNewMemberClassStatus(object savedQuery, string status)
        {
            using (var db2 = NewDataContext())
            {
                var id = Person.FetchOrCreateNewMemberClassStatus(db2, status);
                var q = db2.PeopleQuery2(savedQuery);
                foreach (var p in q)
                {
                    p.UpdateValue("NewMemberClassStatusId", id);
                    p.LogChanges(db2);
                    db2.SubmitChanges();
                }
            }
        }
        public void UpdateContributionOption(object savedQuery, int option)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                var p = db2.LoadPersonById(pid);
                p.UpdateContributionOption(db2, option);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }
        public void UpdateEnvelopeOption(object savedQuery, int option)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                var p = db2.LoadPersonById(pid);
                p.UpdateEnvelopeOption(db2, option);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }
        public void UpdateElectronicStatement(object savedQuery, bool tf)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                var p = db2.LoadPersonById(pid);
                p.UpdateElectronicStatement(db2, tf);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }
    }
}