using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml.Serialization;
using CmsData;
using CmsData.Codes;
using CmsWeb.Models;
using ImageData;
using UtilityExtensions;
using DbUtil = CmsData.DbUtil;

namespace CmsWeb.Areas.Public.Controllers
{
    public class APICheckin2Controller : CmsController
    {
        public const int ADD_ERROR_NONE = 0;
        public const int ADD_ERROR_EXISTS = 1;
        public const int ADD_ERROR_OTHER = 2;

        private static bool Authenticate(string role = "Checkin")
        {
            return AccountModel.AuthenticateMobile("Checkin").IsValid;
        }

        public ActionResult Match(string id, int campus, int thisday, int? page, string kiosk, bool? kioskmode)
        {
            if (!Authenticate())
                return Content("not authorized");
            Response.NoCache();
            DbUtil.Db.SetNoLock();
            DbUtil.LogActivity("checkin " + id);

            var matches = DbUtil.Db.CheckinMatch(id).ToList();

            if (!matches.Any())
                return new FamilyResult(0, campus, thisday, 0, false); // not found
            if (matches.Count() == 1)
                return new FamilyResult(matches.Single().Familyid.Value, campus, thisday, 0, matches[0].Locked ?? false);
            return new MultipleResult(matches, page);
        }

        public ActionResult Find(string id, string building, int? page, string querybit)
        {
            if (!Authenticate())
                return Content("not authorized");
            Response.NoCache();
            DbUtil.Db.SetNoLock();
            DbUtil.LogActivity("CheckinFind " + building + " " + id);

            var m = new CheckInModel();
            var matches = m.Find(id);

            if (!matches.Any())
                return new FindResult(0, building, querybit);
            if (matches.Count() == 1)
                return new FindResult(matches.Single().Familyid.Value, building, querybit);

            return new MultipleResult(matches, page);
        }

        public ActionResult Family(int id, int campus, int thisday, string kiosk, bool? kioskmode)
        {
            if (!Authenticate())
                return Content("not authorized");
            Response.NoCache();
            DbUtil.Db.SetNoLock();
            DbUtil.LogActivity("checkin fam " + id);
            return new FamilyResult(id, campus, thisday, 0, false);
        }

        public ActionResult SingleFamily(int id, string building, string querybit)
        {
            if (!Authenticate())
                return Content("not authorized");
            Response.NoCache();
            DbUtil.Db.SetNoLock();
            DbUtil.LogActivity("checkin fam " + building + " " + id);
            return new FindResult(id, building, querybit);
        }

        public ActionResult Class(int id, int thisday)
        {
            if (!Authenticate())
                return Content("not authorized");
            Response.NoCache();
            DbUtil.Db.SetNoLock();
            DbUtil.LogActivity("checkin class " + id);
            return new ClassResult(id, thisday);
        }

        public ActionResult Classes(int id, int campus, int thisday, bool? noagecheck, bool? kioskmode)
        {
            if (!Authenticate())
                return Content("not authorized");
            Response.NoCache();
            DbUtil.Db.SetNoLock();
            DbUtil.LogActivity("checkin classes " + id);
            return new ClassesResult(id, thisday, campus, noagecheck ?? false, kioskmode ?? false);
        }

        public ActionResult NameSearch(string id, int? page)
        {
            if (!Authenticate())
                return Content("not authorized");
            Response.NoCache();
            DbUtil.Db.SetNoLock();
            DbUtil.LogActivity("checkin namesearch " + id);
            return new NameSearchResult2(id, page ?? 1);
        }

        [HttpPost]
        public ActionResult AddPerson(int id, PersonInfo m)
        {
            if (!Authenticate())
                return Content("not authorized");
            DbUtil.LogActivity($"checkin AddPerson {m.first} {m.last} ({m.dob})");

            var f = id > 0
                ? DbUtil.Db.Families.Single(fam => fam.FamilyId == id)
                : new Family();

            var position = DbUtil.Db.ComputePositionInFamily(m.dob.Age0(), m.marital == 20, id) ?? 10;
            var p = Person.Add(f, position,
                null, m.first, m.goesby, m.last, m.dob, false, m.gender,
                OriginCode.Visit, null);

            UpdatePerson(p, m, true);
            return Content(f.FamilyId + "." + p.PeopleId);
        }

        [HttpPost]
        public ActionResult EditPerson(int id, PersonInfo m)
        {
            if (!Authenticate())
                return Content("not authorized");
            DbUtil.LogActivity($"checkin EditPerson {m.first} {m.last} ({m.dob})");
            var p = DbUtil.Db.LoadPersonById(id);
            UpdatePerson(p, m, false);
            return Content(p.FamilyId.ToString());
        }

        private string Trim(string s)
        {
            if (s.HasValue())
                return s.Trim();
            return s;
        }

        private void UpdatePerson(Person p, PersonInfo m, bool isNew)
        {
            var psb = new List<ChangeDetail>();
            var fsb = new List<ChangeDetail>();
            var keys = Request.Form.AllKeys.ToList();

            if (!m.home.HasValue() && m.cell.HasValue())
                m.home = m.cell;

            if (keys.Contains("zip") || keys.Contains("addr"))
            {
                var result = AddressVerify.LookupAddress(m.addr, p.PrimaryAddress2, null, null, m.zip.Zip5());
                if (result.found != false && !result.error.HasValue() && result.Line1 != "error")
                {
                    UpdateField(fsb, p.Family, "AddressLineOne", result.Line1);
                    UpdateField(fsb, p.Family, "AddressLineTwo", result.Line2);
                    UpdateField(fsb, p.Family, "CityName", result.City);
                    UpdateField(fsb, p.Family, "StateCode", result.State);
                    UpdateField(fsb, p.Family, "ZipCode", result.Zip.GetDigits().Truncate(10));
                    var rc = DbUtil.Db.FindResCode(result.Zip, null);
                    UpdateField(fsb, p.Family, "ResCodeId", rc.ToString());
                }
                else
                {
                    if (keys.Contains("addr"))
                        UpdateField(fsb, p.Family, "AddressLineOne", m.addr);
                    UpdateField(fsb, p.Family, "ZipCode", m.zip.Zip5());
                    UpdateField(fsb, p.Family, "CityName", null);
                    UpdateField(fsb, p.Family, "StateCode", null);
                }
            }

            if (keys.Contains("home"))
                UpdateField(fsb, p.Family, "HomePhone", m.home.GetDigits());
            if (keys.Contains("goesby"))
                UpdateField(psb, p, "NickName", Trim(m.goesby));
            if (keys.Contains("first"))
                UpdateField(psb, p, "FirstName", Trim(m.first));
            if (keys.Contains("last"))
                UpdateField(psb, p, "LastName", Trim(m.last));
            if (keys.Contains("dob"))
            {
                DateTime dt;
                DateTime.TryParse(m.dob, out dt);
                if (p.BirthDate != dt)
                    UpdateField(psb, p, "DOB", m.dob);
            }
            if (keys.Contains("email"))
                UpdateField(psb, p, "EmailAddress", Trim(m.email));
            if (keys.Contains("cell"))
                UpdateField(psb, p, "CellPhone", m.cell.GetDigits());
            if (keys.Contains("marital"))
                UpdateField(psb, p, "MaritalStatusId", m.marital);
            if (keys.Contains("gender"))
                UpdateField(psb, p, "GenderId", m.gender);

            var rr = p.GetRecReg();
            if (keys.Contains("allergies"))
                if (m.allergies != rr.MedicalDescription)
                    p.SetRecReg().MedicalDescription = m.allergies;
            if (keys.Contains("grade"))
                if (m.AskGrade)
                    if (m.grade.ToInt2() != p.Grade)
                        p.Grade = m.grade.ToInt2();
            if (m.AskEmFriend)
            {
                if (keys.Contains("parent"))
                    if (m.parent != rr.Mname)
                        p.SetRecReg().Mname = m.parent;
                if (keys.Contains("emfriend"))
                    if (m.emfriend != rr.Emcontact)
                        p.SetRecReg().Emcontact = m.emfriend;
                if (keys.Contains("emphone"))
                    if (m.emphone != rr.Emphone)
                        p.SetRecReg().Emphone = m.emphone.Truncate(50);
            }
            if (isNew)
            {
                if (keys.Contains("campusid"))
                    if (m.campusid > 0)
                        UpdateField(psb, p, "CampusId", m.campusid);
            }
            if (m.AskChurch)
                if (keys.Contains("activeother"))
                    if (m.activeother.ToBool() != rr.ActiveInAnotherChurch)
                        p.SetRecReg().ActiveInAnotherChurch = m.activeother.ToBool();
            if (m.AskChurchName)
                if (keys.Contains("churchname"))
                    UpdateField(psb, p, "OtherPreviousChurch", Trim(m.churchname));

            p.LogChanges(DbUtil.Db, psb);
            p.Family.LogChanges(DbUtil.Db, fsb, p.PeopleId, Util.UserPeopleId ?? 0);
            DbUtil.Db.SubmitChanges();
            if (DbUtil.Db.Setting("NotifyCheckinChanges", "true").ToBool() && (psb.Count > 0 || fsb.Count > 0))
            {
                var sb = new StringBuilder();
                foreach (var c in psb)
                    sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", c.Field, c.Before, c.After);
                foreach (var c in fsb)
                    sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", c.Field, c.Before, c.After);
                var np = DbUtil.Db.GetNewPeopleManagers();
                if (np != null)
                    DbUtil.Db.EmailRedacted(p.FromEmail, np,
                        "Basic Person Info Changed during checkin on " + Util.Host, $@"
<p><a href=""{DbUtil.Db.ServerLink("/Person2/" + p.PeopleId)}"">{Util.UserName} ({p.PeopleId})</a> changed the following information for {p.PreferredName} ({p.LastName}):</p>
<table>{sb}</table>");
            }
        }

        private void UpdateField(List<ChangeDetail> fsb, Family f, string prop, string value)
        {
            f.UpdateValue(fsb, prop, value);
        }

        private void UpdateField(List<ChangeDetail> psb, Person p, string prop, string value)
        {
            p.UpdateValue(psb, prop, value);
        }

        private void UpdateField(List<ChangeDetail> psb, Person p, string prop, object value)
        {
            p.UpdateValue(psb, prop, value);
        }

        public ActionResult Campuses()
        {
#if DEBUG
#else
            if (!Authenticate())
            {
                DbUtil.LogActivity($"checkin {AccountModel.UserName2} not authenticated");
                return Content("not authorized");
            }
#endif
            DbUtil.LogActivity($"checkin {AccountModel.UserName2} authenticated");

            var list = (from c in DbUtil.Db.Campus
                        where c.Organizations.Any(o => o.CanSelfCheckin == true)
                        orderby c.Id
                        select new CampusItem
                        {
                            Campus = c,
                            password = DbUtil.Db.Setting("kioskpassword" + c.Id, "kio.")
                        }).ToList();

            if (list.Count > 0)
            {
                list.Insert(0, new CampusItem
                {
                    Campus = new Campu { Id = 0, Description = "All Campuses" },
                    password = DbUtil.Db.Setting("kioskpassword" + 0, "kio.")
                });
            }

            return View(list);
        }

        [HttpPost]
        public ContentResult RecordAttend(int PeopleId, int OrgId, bool Present, int thisday, string kiosk)
        {
            if (!Authenticate())
                return Content("not authorized");

            DbUtil.LogActivity($"checkin {PeopleId}, {OrgId}, {(Present ? "attend0" : "unattend0")}");
            var m = new CheckInModel();
            m.RecordAttend(PeopleId, OrgId, Present, thisday);
            var r = new ContentResult();
            r.Content = "success";
            return r;
        }

        [HttpPost]
        public ContentResult RecordAttend2(int PeopleId, int OrgId, bool Present, DateTime hour, string kiosk)
        {
            if (!Authenticate())
                return Content("not authorized");
            DbUtil.LogActivity($"checkin {PeopleId}, {OrgId}, {(Present ? "attend" : "unattend")}");
            Attend.RecordAttend(DbUtil.Db, PeopleId, OrgId, Present, hour);
            var r = new ContentResult();
            r.Content = "success";
            return r;
        }

        [HttpPost]
        public ContentResult Membership(int PeopleId, int OrgId, bool Member)
        {
            if (!Authenticate())
                return Content("not authorized");
            DbUtil.LogActivity($"checkin {PeopleId}, {OrgId}, {(Member ? "join" : "unjoin")}");
            var m = new CheckInModel();
            m.JoinUnJoinOrg(PeopleId, OrgId, Member);
            var r = new ContentResult();
            r.Content = "success";
            return r;
        }

        [Authorize(Roles = "Access")]
        public ActionResult CheckIn(int? id, int? pid)
        {
            Session.Timeout = 1000;
            Session["CheckInOrgId"] = id ?? 0;
            var m = new CheckInRecModel(id ?? 0, pid);
            return View(m);
        }

        [HttpPost]
        public JsonResult PostCheckIn(int id, string KeyCode)
        {
            Session["CheckInOrgId"] = id;
            var q = from kc in DbUtil.Db.CardIdentifiers
                    where KeyCode == kc.Id
                    select kc.PeopleId;
            var pid = q.SingleOrDefault();
            if (pid > 0)
            {
                var dt = Util.Now;
                var ck = new CheckInTime
                {
                    CheckInTimeX = dt,
                    PeopleId = pid
                    //KeyCode = KeyCode
                };
                DbUtil.Db.CheckInTimes.InsertOnSubmit(ck);
                DbUtil.Db.SubmitChanges();
            }
            return Json(new { pid });
        }

        [HttpPost]
        public ContentResult NewKeyCard(int pid, string KeyCode)
        {
            if (!Authenticate())
                return Content("not authorized");
            var p = DbUtil.Db.LoadPersonById(pid);
            if (p == null)
                return Content("No person to associate card with");
            var q = from kc in DbUtil.Db.CardIdentifiers
                    where KeyCode == kc.Id
                    select kc;
            var card = q.SingleOrDefault();
            if (card == null)
            {
                card = new CardIdentifier { Id = KeyCode };
                DbUtil.Db.CardIdentifiers.InsertOnSubmit(card);
            }
            card.PeopleId = pid;
            DbUtil.Db.SubmitChanges();
            return Content("Card Associated");
        }

        [HttpPost]
        public ContentResult Edit(string id, string value)
        {
            if (!Authenticate())
                return Content("not authorized");
            var a = id.Split('.');
            var c = new ContentResult();
            c.Content = value;
            var pid = a[1].ToInt();
            var p = DbUtil.Db.People.Single(pp => pp.PeopleId == pid);
            switch (a[0][0])
            {
                case 's':
                p.SchoolOther = value;
                break;
                case 'y':
                p.Grade = value.ToInt();
                break;
                case 'n':
                p.CheckInNotes = value;
                break;
            }
            DbUtil.Db.SubmitChanges();
            return c;
        }

        [HttpPost]
        public ContentResult UploadImage(int id)
        {
            if (!AccountModel.AuthenticateMobile().IsValid)
                return Content("not authorized");
            //		    if (!User.IsInRole("Edit") && !User.IsInRole("Checkin"))
            //				return Content("not authorized");

            DbUtil.LogActivity("checkin uploadpic " + id);
            var person = DbUtil.Db.People.Single(pp => pp.PeopleId == id);
            if (person.Picture == null)
                person.Picture = new Picture();
            var bits = new byte[Request.InputStream.Length];
            Request.InputStream.Read(bits, 0, bits.Length);

            var p = person.Picture;
            p.CreatedDate = Util.Now;
            p.CreatedBy = Util.UserName;
            p.ThumbId = Image.NewImageFromBits(bits, 50, 50).Id;
            p.SmallId = Image.NewImageFromBits(bits, 120, 120).Id;
            p.MediumId = Image.NewImageFromBits(bits, 320, 400).Id;
            p.LargeId = Image.NewImageFromBits(bits).Id;
            person.LogPictureUpload(DbUtil.Db, Util.UserPeopleId ?? 1);
            DbUtil.Db.SubmitChanges();
            return Content("done");
        }

        public ActionResult FetchImage(int id)
        {
            if (!Authenticate())
                return Content("not authorized");
            var person = DbUtil.Db.People.Single(pp => pp.PeopleId == id);
            if (person.PictureId != null)
            {
                DbUtil.LogActivity("checkin picture " + id);
                return new ImageResult(person.Picture.MediumId ?? 0);
            }
            return new ImageResult(0);
        }

        public ActionResult CheckInList()
        {
            var m = from t in DbUtil.Db.CheckInTimes
                    orderby t.CheckInTimeX descending
                    select t;
            return View(m.Take(200));
        }

        [HttpPost]
        public ActionResult UnLockFamily(int fid)
        {
            if (!Authenticate())
                return Content("not authorized");
            var lockf = DbUtil.Db.FamilyCheckinLocks.SingleOrDefault(f => f.FamilyId == fid);
            if (lockf != null)
            {
                lockf.Locked = false;
                DbUtil.Db.SubmitChanges();
            }
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult ReportPrinterProblem(string kiosk, int campusid)
        {
            if (!Authenticate())
                return Content("not authorized");
            var setting = "KioskNotify";
            if (campusid > 0)
                setting += campusid;
            var address = DbUtil.Db.Setting(setting, null);
            if (address.HasValue())
            {
                try
                {

                    var msg = kiosk + " at " + DateTime.Now.ToShortTimeString();
                    DbUtil.Db.SendEmail(Util.TryGetMailAddress(DbUtil.AdminMail),
                        "Printer Problem", msg, Util.ToMailAddressList(address), 0, null);
                }
                catch (Exception)
                {
                }
            }
            return new EmptyResult();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ContentResult UploadPrintJob(string id)
        {
            if (!Authenticate())
                return Content("not authorized");

            var reader = new StreamReader(Request.InputStream);
            var job = reader.ReadToEnd();

            var m = new CheckInModel();
            m.SavePrintJob(id, job);
            return Content("done");
        }

        public ActionResult FetchPrintJobs(string id)
        {
            if (!Authenticate())
                return Content("not authorized");
            var m = new CheckInModel();
            var b = m.GetNextPrintJobs(id);
            return Content(b, "text/xml");
        }

        public ActionResult FetchBuildingActivities(string id)
        {
            if (!Authenticate())
                return Content("not authorized");
            var m = new CheckInModel();
            return Content(DbUtil.Db.Content($"BuildingCheckin-{id}.xml",
                "<BuildingActivity/>"), "text/xml");
        }

        public ContentResult FetchGuestCount(string id)
        {
            Response.NoCache();
            var dt = DateTime.Now;
            var dtStart = dt.Date;
            var dtEnd = dt.Date.AddHours(24);

            var count = (from e in DbUtil.Db.CheckInTimes
                         where e.CheckInTimeX >= dtStart
                         where e.CheckInTimeX < dtEnd
                         where e.GuestOfPersonID == id.ToInt()
                         select e).Count();

            return Content(count.ToString());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ContentResult AddIDCard(string cardid, int personid, bool overwrite = false)
        {
            var error = ADD_ERROR_NONE;

            var card = (from e in DbUtil.Db.CardIdentifiers
                        where e.Id == cardid
                        select e).FirstOrDefault();

            if (card == null)
            {
                var ci = new CardIdentifier();
                ci.Id = cardid;
                ci.PeopleId = personid;

                DbUtil.Db.CardIdentifiers.InsertOnSubmit(ci);
                DbUtil.Db.SubmitChanges();
            }
            else if (overwrite)
            {
                card.PeopleId = personid;
                DbUtil.Db.SubmitChanges();
            }
            else
            {
                error = ADD_ERROR_EXISTS;
            }

            return Content(error.ToString());
            // Error return: 0 = None, 1 = Exists, 2 = Other
        }

        [HttpPost]
        [ValidateInput(false)]
        public ContentResult FetchLabelFormat(string sName, int iSize)
        {
            if (!Authenticate()) return Content("");

            // Size -30 and +30 is because some labels are a few tenths of inches different in the driver
            var label = (from e in DbUtil.Db.LabelFormats
                         where e.Name == sName
                         where e.Size > (iSize - 30) && e.Size < (iSize + 30)
                         select e).FirstOrDefault();

            if (label == null)
            {
                return Content("");
            }
            return Content(label.Format);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ContentResult SaveLabelFormat(string sName, int iSize, string sFormat)
        {
            if (!Authenticate()) return Content("2");

            var label = (from e in DbUtil.Db.LabelFormats
                         where e.Name == sName
                         where e.Size == iSize
                         select e).FirstOrDefault();

            if (label == null)
            {
                var lfNew = new LabelFormat();
                lfNew.Name = sName;
                lfNew.Size = iSize;
                lfNew.Format = sFormat;
                DbUtil.Db.LabelFormats.InsertOnSubmit(lfNew);
                DbUtil.Db.SubmitChanges();
            }
            else
            {
                label.Format = sFormat;
                DbUtil.Db.SubmitChanges();
            }

            return Content("0");
        }

        public ContentResult FetchLabelList()
        {
            if (!Authenticate()) return Content("Not Authorized");
            Response.NoCache();

            var list = from e in DbUtil.Db.LabelFormats
                       orderby e.Size, e.Name
                       select string.Concat(e.Name, "~", e.Size);

            if (list == null)
            {
                return Content("No Records");
            }
            var types = list.ToArray();
            return Content(string.Join(",", types));
        }

        [HttpPost]
        [ValidateInput(false)]
        public ContentResult BuildingCheckin(int id, string location, int accesstype, int? guestof)
        {
            if (!Authenticate())
                return Content("not authorized");

            CheckInTime g = null;

            if (guestof != null)
            {
                g = (from e in DbUtil.Db.CheckInTimes
                     where e.Id == guestof
                     select e).FirstOrDefault();
            }

            var reader = new StreamReader(Request.InputStream);
            var s = reader.ReadToEnd();

            if (!s.HasValue())
                s = "<Activities />";

            var xs = new XmlSerializer(typeof(List<Activity>), new XmlRootAttribute("Activities"));
            var activities = xs.Deserialize(new StringReader(s)) as List<Activity>;

            var last = from e in DbUtil.Db.CheckInTimes
                       where e.PeopleId == id
                       where e.CheckInTimeX <= DateTime.Now
                       where e.CheckInTimeX >= DateTime.Now.AddHours(-1.5)
                       select e;

            if (guestof == null)
            {
                last = from f in last
                       where f.GuestOfId == null
                       select f;
            }
            else
            {
                last = from f in last
                       where f.GuestOfId == guestof
                       select f;
            }

            CheckInTime ac = null;

            if (last.Any())
            {
                ac = last.Single();

                foreach (var e in ac.CheckInActivities)
                    DbUtil.Db.CheckInActivities.DeleteOnSubmit(e);

                DbUtil.Db.SubmitChanges();

                foreach (var a in activities)
                    ac.CheckInActivities.Add(new CheckInActivity { Activity = a.Name });

                ac.AccessTypeID = accesstype;
            }
            else
            {
                ac = new CheckInTime
                {
                    PeopleId = id,
                    Location = location,
                    CheckInTimeX = DateTime.Now,
                    GuestOfId = guestof,
                    GuestOfPersonID = (g != null ? g.PeopleId ?? 0 : 0),
                    AccessTypeID = accesstype
                };

                foreach (var a in activities)
                    ac.CheckInActivities.Add(new CheckInActivity { Activity = a.Name });

                DbUtil.Db.CheckInTimes.InsertOnSubmit(ac);
            }


            DbUtil.Db.SubmitChanges();

            foreach (var a in activities)
            {
                if (a.org > 0)
                    Attend.RecordAttend(DbUtil.Db, id, a.org, true, DateTime.Today);
            }

            return Content(ac.Id.ToString());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ContentResult BuildingUnCheckin(int id)
        {
            if (!Authenticate())
                return Content("not authorized");

            var ct = DbUtil.Db.CheckInTimes.SingleOrDefault(cc => cc.Id == id);
            DbUtil.Db.CheckInActivities.DeleteAllOnSubmit(ct.CheckInActivities);
            DbUtil.Db.CheckInTimes.DeleteOnSubmit(ct);
            DbUtil.Db.SubmitChanges();

            return Content("done");
        }

        public class CampusItem
        {
            public Campu Campus { get; set; }
            public string password { get; set; }
        }

        [Serializable]
        public class Activity
        {
            [XmlAttribute]
            public string name { get; set; }

            [XmlAttribute]
            public int org { get; set; }

            [XmlText]
            public string display { get; set; }

            public string Name => name ?? display;

            public override string ToString()
            {
                return display;
            }
        }
    }
}
