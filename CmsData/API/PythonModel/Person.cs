using System.Collections.Generic;
using System.Linq;
using CmsData.API;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public void CreateTask(int forPeopleId, Person p, string description)
        {
            var t = p.AddTaskAbout(db, forPeopleId, description);
            db.SubmitChanges();
            db.Email(db.Setting("AdminMail", "support@touchpointsoftware.com"), db.LoadPersonById(forPeopleId),
                "TASK: " + description,
                Task.TaskLink(db, description, t.Id) + "<br/>" + p.Name);
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
            var id = db.FetchOrCreateCampusId(campus);
            var q = db.PeopleQuery2(savedQuery);
            foreach (var p in q)
            {
                p.UpdateValue("CampusId", id);
                p.LogChanges(db);
                db.SubmitChanges();
            }
        }

        public void UpdateField(Person p, string field, object value)
        {
            p.UpdateValue(field, value);
        }

        public void UpdateMemberStatus(object savedQuery, string status)
        {
            var id = Person.FetchOrCreateMemberStatus(db, status);
            var q = db.PeopleQuery2(savedQuery);
            foreach (var p in q)
            {
                p.UpdateValue("MemberStatusId", id);
                p.LogChanges(db);
                db.SubmitChanges();
            }
        }

        public void UpdateNamedField(object savedQuery, string field, object value)
        {
            var q = db.PeopleQuery2(savedQuery);
            foreach (var p in q)
            {
                p.UpdateValue(field, value);
                p.LogChanges(db);
                db.SubmitChanges();
            }
        }

        public void UpdateNewMemberClassDate(object savedQuery, object dt)
        {
            var q = db.PeopleQuery2(savedQuery);
            foreach (var p in q)
            {
                p.UpdateValue("NewMemberClassDate", dt);
                p.LogChanges(db);
                db.SubmitChanges();
            }
        }

        public void UpdateNewMemberClassDateIfNullForLastAttended(object savedQuery, object orgId)
        {
            var q = db.PeopleQuery2(savedQuery);
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
                    p.LogChanges(db);
                    db.SubmitChanges();
                }
            }
        }

        public void UpdateNewMemberClassStatus(object savedQuery, string status)
        {
            var id = Person.FetchOrCreateNewMemberClassStatus(db, status);
            var q = db.PeopleQuery2(savedQuery);
            foreach (var p in q)
            {
                p.UpdateValue("NewMemberClassStatusId", id);
                p.LogChanges(db);
                db.SubmitChanges();
            }
        }
    }
}