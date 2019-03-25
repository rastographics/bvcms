using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class CheckInRecModel
    {
        public CheckInRecModel() { }
        public CheckInRecModel(int orgId, int? pid)
        {
            var q = from o in DbUtil.Db.Organizations
                    where o.OrganizationId == orgId
                    select o.OrganizationName;
            OrgName = q.SingleOrDefault();
            var q2 = from p in DbUtil.Db.People
                     where p.PeopleId == pid
                     select new PersonInfo
                     {
                         PeopleId = p.PeopleId,
                         Name = p.Name,
                         Birthday = p.DOB,
                         ImageId = p.Picture.MediumId ?? 0,
                         Addr1 = p.PrimaryAddress,
                         Addr2 = p.PrimaryAddress2,
                         CityStateZip = p.CityStateZip,
                         Phone = p.HomePhone.FmtFone("H "),
                         Cell = p.CellPhone.FmtFone("C "),
                         Email = p.EmailAddress,
                         School = p.SchoolOther,
                         Year = p.Grade.ToString(),
                         CheckInNotes = p.CheckInNotes,
                         FamilyId = p.FamilyId
                     };
            person = q2.SingleOrDefault();
            if (person == null)
            {
                person = new PersonInfo
                {
                    PeopleId = 0,
                    Name = "not found"
                };
            }
            else
            {
                var guid = (Guid?)(HttpContextFactory.Current.Session["checkinguid"]);
                if (!guid.HasValue)
                {
                    //var tt = new TemporaryToken
                    //{
                    //    Id = Guid.NewGuid(),
                    //    CreatedBy = Util.UserId1,
                    //    CreatedOn = Util.Now,
                    //};
                    //DbUtil.Db.TemporaryTokens.InsertOnSubmit(tt);
                    //DbUtil.Db.SubmitChanges();
                    //guid = tt.Id;
                    HttpContextFactory.Current.Session["checkinguid"] = guid;
                }
                this.guid = guid.ToString();
            }
            OrgId = orgId;
        }

        public int OrgId { get; set; }
        public string OrgName { get; set; }
        public PersonInfo person { get; set; }
        public string guid { get; set; }
        public string host => DbUtil.Db.CmsHost;

        public string WithBreak(string s)
        {
            if (s.HasValue())
            {
                return s + "<br />";
            }

            return string.Empty;
        }

        public IEnumerable<FamilyMemberInfo> GetFamilyMembers()
        {
            var q = from p in DbUtil.Db.People
                    where p.FamilyId == person.FamilyId
                    where p.PeopleId != person.PeopleId
                    select new FamilyMemberInfo
                    {
                        Name = p.Name,
                        PeopleId = p.PeopleId
                    };
            return q;
        }

        public class FamilyMemberInfo
        {
            public int PeopleId { get; set; }
            public string Name { get; set; }
        }

        public class PersonInfo
        {
            private string _School;
            public int? PeopleId { get; set; }
            public string Name { get; set; }
            public string Birthday { get; set; }
            public int ImageId { get; set; }
            public string Addr1 { get; set; }
            public string Addr2 { get; set; }
            public string CityStateZip { get; set; }
            public string Phone { get; set; }
            public string Cell { get; set; }
            public string Email { get; set; }
            public int FamilyId { get; set; }

            public string School
            {
                get
                {
                    if (!_School.HasValue())
                    {
                        return "click to add";
                    }

                    return _School;
                }
                set { _School = value; }
            }

            public string Year { get; set; }
            public string Grade { get; set; }
            public string CheckInNotes { get; set; }
            public string ImageUrl => $"/Portrait/{ImageId}";
        }
    }
}
