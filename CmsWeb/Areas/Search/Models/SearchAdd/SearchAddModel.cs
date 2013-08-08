/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using CmsWeb.Areas.People.Models;
using CmsWeb.Code;
using CmsWeb.Models;
using UtilityExtensions;
using CmsData;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data.Linq;

namespace CmsWeb.Areas.Search.Models
{
    public class SearchAddModel
    {
        public SearchAddModel()
        {
            Pager = new PagerModel2(Count);
            Pager.ShowPageSize = false;
        }
        public PagerModel2 Pager { get; set; }
        public string type { get; set; }

        public string Title
        {
            get
            {
                switch (type.ToLower())
                {
                    case "addpeople":
                    case "menu":
                        return "Add to Database";
                    case "addtotag":
                        return "Add to Tag";
                    case "family":
                        return "Add to Family";
                    case "relatedfamily":
                        return "Add as Related Family";
                    case "org":
                        return "Add as Member of Organization";
                    case "pending":
                        return "Add as Pending Member of Organization";
                    case "visitor":
                        return "Add as Visitor to Meeting";
                    case "registered":
                        return "Add as Registered for Future Meeting";
                    case "contactee":
                        return "Add as Contactee for Ministry Contact";
                    case "contactor":
                        return "Add as Contactor for Ministry Contact";
                    case "contributor":
                        return "Add for a Gift";
                }
                return "";
            }
        }

        private string[] noaddtypes = { "relatedfamily", "mergeto", "contactor", "taskdelegate", "taskowner", "taskdelegate2" };
        private string[] usersonlytypes = { "taskdelegate", "taskowner", "taskdelegate2" };
        private string[] onlyonetypes = { "taskdelegate", "taskowner", "taskdelegate2", "mergeto", "relatedfamily" };
        public bool CanAdd { get { return !noaddtypes.Contains(type.ToLower()); } }
        public string typeid { get; set; }
        public bool UsersOnly { get { return usersonlytypes.Contains(type.ToLower()); } }
        public bool OnlyOne { get { return onlyonetypes.Contains(type.ToLower()); } }
        public string submit { get; set; }

        public int NewFamilyId { get; set; }
        public int? EntryPointId { get; set; }
        public int? CampusId { get; set; }
        public int Index { get; set; }

        public int NextNewFamilyId()
        {
            NewFamilyId--;
            return NewFamilyId;
        }

        public AddressInfo AddressInfo { get; set; }

        private IList<SearchPersonModel> list = new List<SearchPersonModel>();
        public IList<SearchPersonModel> List
        {
            get { return list; }
            set { list = value; }
        }

        [UIHint("Text")]
        public string Name { get; set; }

        [UIHint("Text")]
        [DisplayName("Communication")]
        public string Phone { get; set; }

        [UIHint("Text")]
        public string Address { get; set; }

        [DisplayName("Date of Birth")]
        [UIHint("Text")]
        public string dob { get; set; }

        public string HelpLink(string page)
        {
            return Util.HelpLink("_SearchAdd_{0}".Fmt(page));
        }
        private IEnumerable<PeopleInfo> PeopleList(IQueryable<CmsData.Person> query)
        {
            var q = from p in query
                    orderby p.Name2
                    select new PeopleInfo
                    {
                        PeopleId = p.PeopleId,
                        FamilyId = p.FamilyId,
                        Name = p.Name,
                        Middle = p.MiddleName,
                        GoesBy = p.NickName,
                        First = p.FirstName,
                        Maiden = p.MaidenName,
                        Address = p.PrimaryAddress,
                        CityStateZip = p.PrimaryCity + ", " + p.PrimaryState + " " + p.PrimaryZip.Substring(0, 5),
                        Age = p.Age,
                        JoinDate = p.JoinDate,
                        BirthDate = p.BirthMonth + "/" + p.BirthDay + "/" + p.BirthYear,
                        HomePhone = p.HomePhone,
                        CellPhone = p.CellPhone,
                        WorkPhone = p.WorkPhone,
                        MemberStatus = p.MemberStatus.Description,
                        Email = p.EmailAddress
                    };
            return q;
        }

        public IEnumerable<PeopleInfo> PeopleList()
        {
            var q = FetchPeople().Skip(Pager.StartRow).Take(Pager.PageSize);
            return PeopleList(q);
        }

        private IQueryable<CmsData.Person> FetchPeople()
        {
            if (query == null)
                query = ApplySearch();
            return query;
        }
        private int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchPeople().Count();
            return _count.Value;
        }

        private IQueryable<CmsData.Person> query = null;
        private IQueryable<CmsData.Person> ApplySearch()
        {
            if (query.IsNotNull())
                return query;

            var db = DbUtil.Db;
            if (Util2.OrgMembersOnly)
                query = db.OrgMembersOnlyTag2().People(db);
            else if (Util2.OrgLeadersOnly)
                query = db.OrgLeadersOnlyTag2().People(db);
            else
                query = db.People.AsQueryable();

            if (UsersOnly)
                query = query.Where(p => p.Users.Any(uu => uu.UserRoles.Any(ur => ur.Role.RoleName == "Access")));

            if (Name.HasValue())
            {
                string First, Last;
                Util.NameSplit(Name, out First, out Last);
                if (First.HasValue())
                    query = from p in query
                            where (p.LastName.StartsWith(Last) || p.MaidenName.StartsWith(Last))
                            && (p.FirstName.StartsWith(First) || p.NickName.StartsWith(First) || p.MiddleName.StartsWith(First))
                            select p;
                else
                    if (Last.AllDigits())
                        query = from p in query
                                where p.PeopleId == Last.ToInt()
                                select p;
                    else
                        query = from p in query
                                where p.LastName.StartsWith(Last) || p.MaidenName.StartsWith(Last)
                                select p;
            }
            if (Address.IsNotNull())
            {
                Address = Address.Trim();
                if (Address.HasValue())
                    query = from p in query
                            where p.Family.AddressLineOne.Contains(Address)
                               || p.Family.AddressLineTwo.Contains(Address)
                               || p.Family.CityName.Contains(Address)
                               || p.Family.ZipCode.Contains(Address)
                            select p;
            }
            if (Phone.IsNotNull())
            {
                Phone = Phone.Trim();
                if (Phone.HasValue())
                    query = from p in query
                            where p.CellPhone.Contains(Phone) || p.EmailAddress.Contains(Phone)
                            || p.Family.HomePhone.Contains(Phone)
                            || p.WorkPhone.Contains(Phone)
                            select p;
            }
            if (dob.HasValue())
            {
                DateTime dt;
                if (DateTime.TryParse(dob, out dt))
                    if (Regex.IsMatch(dob, @"\d+/\d+/\d+"))
                        query = query.Where(p => p.BirthDay == dt.Day && p.BirthMonth == dt.Month && p.BirthYear == dt.Year);
                    else
                        query = query.Where(p => p.BirthDay == dt.Day && p.BirthMonth == dt.Month);
                else
                {
                    int n;
                    if (int.TryParse(dob, out n))
                        if (n >= 1 && n <= 12)
                            query = query.Where(p => p.BirthMonth == n);
                        else
                            query = query.Where(p => p.BirthYear == n);
                }
            }
            return query;
        }
        public class PeopleInfo
        {
            public int PeopleId { get; set; }
            public int FamilyId { get; set; }
            public string Name { get; set; }
            public string Middle { get; set; }
            public string Maiden { get; set; }
            public string GoesBy { get; set; }
            public string First { get; set; }
            public string Address { get; set; }
            public string CityStateZip { get; set; }
            public int? Age { get; set; }
            public string HomePhone { get; set; }
            public string CellPhone { get; set; }
            public string WorkPhone { get; set; }
            public string MemberStatus { get; set; }
            public DateTime? JoinDate { get; set; }
            public string BirthDate { get; set; }
            public string Email { get; set; }

            public HtmlString ToolTip
            {
                get
                {
                    var ret = new StringBuilder();
                    if (CellPhone.HasValue())
                        ret.AppendFormat("{0}&nbsp;&nbsp;", CellPhone.FmtFone("C "));
                    if (HomePhone.HasValue())
                        ret.AppendFormat("{0}&nbsp;&nbsp;", HomePhone.FmtFone("H "));
                    if (WorkPhone.HasValue())
                        ret.AppendFormat("{0}&nbsp;&nbsp;", WorkPhone.FmtFone("W "));
                    if (ret.Length > 0)
                        ret.Append("<br>\n");

                    var names = new StringBuilder();
                    if (GoesBy.HasValue() && First != GoesBy)
                        names.AppendFormat("{0}first: {1}", names.Length > 0 ? ", " : "", First);
                    if (Middle.HasValue())
                        names.AppendFormat("{0}middle: {1}", names.Length > 0 ? ", " : "", Middle);
                    if (Maiden.HasValue())
                        names.AppendFormat("{0}maiden: {1}", names.Length > 0 ? ", " : "", Maiden);
                    if (names.Length > 0)
                        ret.AppendFormat("{0}<br>\n", names);

                    if (BirthDate.HasValue())
                        ret.AppendFormat("Birthday: {0}&nbsp;&nbsp;", BirthDate);
                    ret.AppendFormat("[<i>{0}</i>]&nbsp;&nbsp;", MemberStatus);
                    if (JoinDate.HasValue)
                        ret.Append("Joined: " + JoinDate.ToDate().FormatDate());

                    if (CityStateZip.HasValue())
                        ret.AppendFormat("<br>\n{0}", CityStateZip);

                    return new HtmlString(ret.ToString());
                }
            }
        }
        public static string AddRelatedFamily(int peopleid, int relatedPersonId)
        {
            var p = DbUtil.Db.LoadPersonById(peopleid);
            var p2 = DbUtil.Db.LoadPersonById(relatedPersonId);
            var rf = DbUtil.Db.RelatedFamilies.SingleOrDefault(r =>
                (r.FamilyId == p.FamilyId && r.RelatedFamilyId == p2.FamilyId)
                || (r.FamilyId == p2.FamilyId && r.RelatedFamilyId == p.FamilyId)
                );
            if (rf == null)
            {
                rf = new RelatedFamily
                {
                    FamilyId = p.FamilyId,
                    RelatedFamilyId = p2.FamilyId,
                    FamilyRelationshipDesc = "",
                    CreatedBy = Util.UserId1,
                    CreatedDate = Util.Now,
                };
                DbUtil.Db.RelatedFamilies.InsertOnSubmit(rf);
                DbUtil.Db.SubmitChanges();
            }
            return "#rf-{0}-{1}".Fmt(rf.FamilyId, rf.RelatedFamilyId);
        }

        public SearchPersonModel NewPerson(int familyid)
        {
            var p = new SearchPersonModel
            {
                FamilyId = familyid,
                index = List.Count,
                Gender = new CodeInfo(99, "Gender"),
                Marital = new CodeInfo(99, "Marital"),
                Campus = new CodeInfo(CampusId, "Campus"),
                EntryPoint = new CodeInfo(EntryPointId, "EntryPoint"),
                context = type,
                Title = new CodeInfo("", "Title"),
            };
            if (familyid == 0)
            {
                p.FamilyId = NextNewFamilyId();
                p.isNewFamily = true;
            }
#if DEBUG
            p.First = "David";
            p.Last = "Carr." + DateTime.Now.Millisecond;
            p.Gender = new CodeInfo(0, "Gender");
            p.Marital = new CodeInfo(0, "Marital");
            p.dob = "na";
            p.Email = "na";
            p.Phone = "na";
#endif
            List.Add(p);
            return p;
        }
    }
}
