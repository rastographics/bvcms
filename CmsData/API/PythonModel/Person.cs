using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData.API;
using CmsData.Codes;
using UtilityExtensions;
using Novacode;
using System.Xml.Linq;

namespace CmsData
{
    public partial class PythonModel
    {
        public int AddPerson(string firstname, string nickname, string lastname, string email, int marital = 0, int gender = 0, int? familymemberid = null)
        {
            using (var db2 = NewDataContext())
            {
                var p = Person.Add(db2, null, firstname, nickname, lastname, null);
                p.GenderId = gender;
                p.MaritalStatusId = marital;
                p.EmailAddress = email;
                db2.SubmitChanges();
                return p.PeopleId;
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
                        user = new User() {PeopleId = p.PeopleId, Password = pword, Username = uname, MustChangePassword = false, IsApproved = true, Name = p.Name};
                        db2.SubmitChanges();
                        db2.Users.InsertOnSubmit(user);
                        user.AddRole(db2, role);
                        db2.SubmitChanges();
                    }
                }
            }
        }

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

        public void DeletePeople(object query)
        {
            if (!HttpContext.Current.User.IsInRole("developer"))
                db.LogActivity("Python DeletePerson {query} denied");

            var list = PeopleIds(query);
            foreach (var pid in list)
            {
                db.PurgePerson(pid);
                db.LogActivity($"Python DeletePerson {pid}");
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
                        if (newroles.Length == oldroles.Length)
                            continue;
                        user.SetRoles(db2, newroles);
                        db2.SubmitChanges();
                    }
                }
            }
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

        /// <summary>
        /// Add the specified tag to the given Person
        /// </summary>
        public void AddTag(Person p, string tagName, int? ownerId, int tagTypeId)
        {
            var db2 = NewDataContext();
            Person.Tag(db2, p.PeopleId, tagName, ownerId, tagTypeId);
            db2.SubmitChanges();
            db2.Dispose();
        }

        /// <summary>
        /// Perform a "mailmerge" of sorts, replacing merge fields with properties from Person
        /// and returning the merged document in the http response
        /// </summary>
        /// <param name="pid">The person ID to get the properties from</param>
        /// <param name="templateName">The full path to the Word document template</param>
        /// <param name="filePrefix">The prefix of the filename to use in the http response</param>
        public void MergeDocument(object pid, string templateName, string filePrefix)
        {
            Person person = db.LoadPersonById(pid.ToInt());

            DocX document = DocX.Load(templateName);
            XElement root = document.Xml;
            Merge(root, person);

            String filename = filePrefix + person.FirstName + person.LastName + ".docx";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("content-disposition", "inline;filename=\"" + filename + "\"");
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            document.SaveAs(HttpContext.Current.Response.OutputStream);
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// Replace merge fields with properties from Person, on the specified element and all of it's children
        /// </summary>
        private void Merge(XElement element, Person person)
        {
            if (element.HasElements)
            {
                foreach (var child in element.Elements())
                {
                    // a little recursion is good for the soul
                    Merge(child, person);
                }
            }
            else
            {
                string value = element.Value;
                if (value != null && value.Length > 0)
                {
                    if (value.First() == (char)171 && value.Last() == (char)187)
                    {
                        // most typical merge field, such as <<lastname>>
                        string property = value.Substring(1, value.Length - 2);
                        ReplaceValue(element, property, person);
                    }
                    else if (value.First() == (char)171 && value.Length == 1)
                    {
                        // merge field is spit into multiple elements due to formatting
                        // blank out opening chevron
                        element.Value = "";
                        // find merge element
                        XElement mergeElement = FindMergeElement(element);
                        // replace value
                        ReplaceValue(mergeElement, mergeElement.Value, person);
                    }
                    else if (value.First() == (char)187 && value.Length == 1)
                    {
                        // blank out closing chevron
                        element.Value = "";
                    }
                }
            }
        }

        /// <summary>
        /// Find the element that has the merge field.
        /// Typically the opening chevron is in an element by itself, with a merge field in the next element,
        /// and then the closing chevron in another element.  Sometimes there is also a bookmark element
        /// between the opening chevron element and the merge field element.
        /// </summary>
        /// <param name="element">The opening chevron element</param>
        /// <returns>The element that contains the merge field</returns>
        private XElement FindMergeElement(XElement element)
        {
            XElement mergeElement;
            IEnumerable<XElement> siblings = element.Parent.ElementsAfterSelf();
            if (siblings.ElementAt(0).Name.LocalName.Equals("bookmarkStart"))
                mergeElement = siblings.ElementAt(1).Elements().ElementAt(1);
            else
                mergeElement = siblings.ElementAt(0).Elements().ElementAt(1);
            return mergeElement;
        }

        /// <summary>
        /// Replace the element value with the specified property from Person
        /// </summary>
        private void ReplaceValue(XElement element, string property, Person person)
        {
            object replacement = person.GetType().GetProperty(property).GetValue(person, null);
            element.Value = replacement == null ? "" : replacement.ToString();
        }
    }
}