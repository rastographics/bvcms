using System;
using System.Collections.Generic;
using System.Linq;
using CmsData.API;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public int? AgeInMonths(DateTime? birthdate, DateTime asof)
        {
            if (!birthdate.HasValue)
                return null;
            if (birthdate > asof)
                return null;
            var dt = birthdate.Value;
            var mos = 0;
            while (true)
                if (dt.AddMonths(++mos) > asof)
                    return mos - 1;
        }

        public void CreateTask(int ministerId, int aboutId, string description, string notes = null)
        {
            using (var db2 = NewDataContext())
            {
                var about = db2.LoadPersonById(aboutId);
                var minister = db2.LoadPersonById(ministerId);
                var t = new Task
                {
                    OwnerId = ministerId,
                    Description = description,
                    Notes = notes,
                    ForceCompleteWContact = true,
                    ListId = Task.GetRequiredTaskList(db2, "InBox", ministerId).Id,
                    StatusId = TaskStatusCode.Active,
                    WhoId = aboutId,
                };
                db2.Tasks.InsertOnSubmit(t);
                db2.SubmitChanges();
                var taskLink = Task.TaskLink(db2, description, t.Id);
                db2.Email(
                    db2.Setting("AdminMail", "support@touchpointsoftware.com"), // from email
                    minister, // to person
                    "TASK: " + description, // subject
                    $@"{taskLink}<br/>\n{about.Name}\n<p>{notes}</p>"); // body
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

        public List<int> PeopleIds(object query)
        {
            var list = db.PeopleQuery2(query).Select(ii => ii.PeopleId).ToList();
            return list;
        }

        public void UpdateCampus(object query, object campus)
        {
            var str = campus as string;
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(query);
                var id = campus is int || str.AllDigits() 
                    ? campus.ToInt() 
                    : db2.FetchOrCreateCampusId(str);
                if (id == 0)
                    return;
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

        public void UpdateMemberStatus(object query, object status)
        {
            var str = status as string;
            using (var db2 = NewDataContext())
            {
                var id = status is int || str.AllDigits() 
                    ? status.ToInt() 
                    : Person.FetchOrCreateMemberStatus(db2, str);
                if (id == 0)
                    return;
                var q = db2.PeopleQuery2(query);
                foreach (var p in q)
                {
                    p.UpdateValue("MemberStatusId", id);
                    p.LogChanges(db2);
                    db2.SubmitChanges();
                }
            }
        }

        public void UpdateNamedField(object query, string field, object value)
        {
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(query);
                foreach (var p in q)
                {
                    p.UpdateValue(field, value);
                    p.LogChanges(db2);
                    db2.SubmitChanges();
                }
            }
        }

        public void UpdateNewMemberClassDate(object query, object dt)
        {
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(query);
                foreach (var p in q)
                {
                    p.UpdateValue("NewMemberClassDate", dt);
                    p.LogChanges(db2);
                    db2.SubmitChanges();
                }
            }
        }

        public void UpdateNewMemberClassDateIfNullForLastAttended(object query, object orgId)
        {
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(query);
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

        public void UpdateNewMemberClassStatus(object query, object status)
        {
            var str = status as string;
            using (var db2 = NewDataContext())
            {
                var id = status is int || str.AllDigits()
                    ? status.ToInt()
                    : Person.FetchOrCreateNewMemberClassStatus(db2, str);
                if (id == 0)
                    return;
                var q = db2.PeopleQuery2(query);
                foreach (var p in q)
                {
                    p.UpdateValue("NewMemberClassStatusId", id);
                    p.LogChanges(db2);
                    db2.SubmitChanges();
                }
            }
        }

        public void UpdateContributionOption(object query, int option)
        {
            var list = db.PeopleQuery2(query).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                var p = db2.LoadPersonById(pid);
                p.UpdateContributionOption(db2, option);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }

        public void UpdateEnvelopeOption(object query, int option)
        {
            var list = db.PeopleQuery2(query).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                var p = db2.LoadPersonById(pid);
                p.UpdateEnvelopeOption(db2, option);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }

        public void UpdateElectronicStatement(object query, bool tf)
        {
            var list = db.PeopleQuery2(query).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                var p = db2.LoadPersonById(pid);
                p.UpdateElectronicStatement(db2, tf);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }

        public void AddRole(object query, string role)
        {
            var disallow = new[]
            {
                "admin",
                "applicationreview",
                "backgroundcheck",
                "creditcheck",
                "delete",
                "developer",
                "finance",
                "financeadmin",
                "manager",
                "manager2",
                "membership",
                "managetransactions",
                "memberdocs",
            };
            if (disallow.Any(rr => rr.Equal(role)))
            {
                db.LogActivity($"PythonModel.AddRole(query, {role}) denied");
                return;
            }
            db.LogActivity($"PythonModel.AddRole(query, {role})");
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(query);
                foreach (var p in q)
                {
                    var user = p.Users.FirstOrDefault();
                    if (user != null)
                    {
                        user.AddRole(db2, role);
                        db2.SubmitChanges();
                    }
                    else
                    {
                        var uname = MembershipService.FetchUsername(db2, p.PreferredName, p.LastName);
                        var pword = Guid.NewGuid().ToString();
                        user = new User() { PeopleId = p.PeopleId, Password = pword, Username = uname, MustChangePassword = false, IsApproved = true, Name = p.Name };
                        db2.SubmitChanges();
                        db2.Users.InsertOnSubmit(user);
                        user.AddRole(db2, role);
                        db2.SubmitChanges();
                    }
                }
            }
        }
        public void RemoveRole(object query, string role)
        {
            db.LogActivity($"PythonModel.RemoveRole(query, {role})");
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(query);
                foreach (var p in q)
                {
                    var user = p.Users.FirstOrDefault();
                    if (user != null)
                    {
                        var oldroles = user.Roles;
                        var newroles = oldroles.Where(rr => !rr.Equal(role)).ToArray();
                        if(newroles.Length == oldroles.Length)
                            continue;
                        user.SetRoles(db2, newroles);
                        db2.SubmitChanges();
                    }
                }
            }
        }
    }
}