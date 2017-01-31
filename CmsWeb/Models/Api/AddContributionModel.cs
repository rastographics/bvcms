using System;
using System.Linq;
using CmsData;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsWeb.Models.Api
{
    public class AddContributionModel
    {
        public string First;
        public string Last;
        public string Email;
        public string Phone;
        public string Address;
        public string Zip;
        public DateTime? Date;
        public decimal Amount;
        public string Notes;
        public string Source;
        public int? Fundid;

        public Result Add(CMSDataContext db)
        {
            if (!Util.ValidEmail(Email))
                throw new Exception("Need a valid email address");
            if (Amount <= 0)
                throw new Exception("Amount must be > 0");

            Person person = null;
            var list = db.FindPerson(First, Last, null, Email, Phone.GetDigits()).ToList();
            var count = list.Count;
            if (count > 0)
                person = db.LoadPersonById(list[0].PeopleId ?? 0);

            var result = new Result();
            if (count > 1)
                result.MultipleMatches = true;

            if (person == null)
            {
                result.NewPerson = true;
                var f = new Family
                {
                    AddressLineOne = Address,
                    ZipCode = Zip,
                    HomePhone = Phone.GetDigits().Truncate(20),
                };
                DbUtil.Db.SubmitChanges();

                var position = 10;
                person = Person.Add(db, true, f, position, null, First.Trim(), null, Last.Trim(), "", 0, 0,
                    OriginCode.Contribution, null);
                person.EmailAddress = Email.Trim();
                person.SendEmailAddress1 = true;

                if (count == 0)
                    person.Comments = "Added during api postcontribution because record was not found";
                else if (count > 1)
                    person.Comments = "Added during api postcontribution because there was more than 1 match";
            }

            var c = person.PostUnattendedContribution(DbUtil.Db, Amount, Fundid, Notes);
            c.ContributionDate = Date ?? DateTime.Now;
            db.SubmitChanges();
            result.PeopleId = person.PeopleId;
            result.ContributionId = c.ContributionId;
            return result;
        }

        public class Result
        {
            public int PeopleId { get; set; }
            public int ContributionId { get; set; }
            public bool NewPerson { get; set; }
            public bool MultipleMatches { get; set; }
        }
    }
}