using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsData.View;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class DuplicatesModel
    {
        public Person person { get; set; }
        public List<PotentialDup> MatchThis { get; set; }
        public DuplicatesModel() { }
        public DuplicatesModel(int id)
        {
            person = DbUtil.Db.LoadPersonById(id);
            MatchThis = new List<PotentialDup>
            {
                new PotentialDup()
                {
                    PeopleId = person.PeopleId,
                    First = person.FirstName,
                    Last = person.LastName,
                    Nick = person.NickName,
                    Middle = person.MiddleName,
                    BMon = person.BirthMonth,
                    BDay = person.BirthDay,
                    BYear = person.BirthYear,
                    Email = person.EmailAddress,
                    FamAddr = person.Family.AddressLineOne,
                    PerAddr = person.AddressLineOne,
                    Member = person.MemberStatus.Description
                }
            };
        }
    }
}
