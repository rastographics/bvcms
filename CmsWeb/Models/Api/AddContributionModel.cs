using CmsData;
using CmsData.Codes;
using System;
using System.Linq;
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

        public AddContributionModel() { }
        public Result Add(CMSDataContext db)
        {
            if (!Util.ValidEmail(Email))
            {
                throw new Exception("Need a valid email address");
            }

            if (Amount <= 0)
            {
                throw new Exception("Amount must be > 0");
            }

            Person person = null;
            var list = db.FindPerson(First, Last, null, Email, Phone.GetDigits()).ToList();
            var count = list.Count;
            if (count > 0)
            {
                person = db.LoadPersonById(list[0].PeopleId ?? 0);
            }

            var result = new Result();
            if (count > 1)
            {
                result.MultipleMatches = true;
            }

            if (person == null)
            {
                result.NewPerson = true;
                var f = new Family
                {
                    AddressLineOne = Address,
                    ZipCode = Zip,
                    HomePhone = Phone.GetDigits().Truncate(20),
                };
                var amsresult = AddressVerify.LookupAddress(Address, null, null, null, Zip);
                if (amsresult.found != false && !amsresult.error.HasValue() && amsresult.Line1 != "error")
                {
                    f.CityName = amsresult.City;
                    f.StateCode = amsresult.State;
                    f.ZipCode = amsresult.Zip.GetDigits().Truncate(10);
                }
                db.SubmitChanges();

                var position = 10;
                person = Person.Add(db, true, f, position, null, First.Trim(), null, Last.Trim(), "", 0, 0, OriginCode.Contribution, null);
                person.EmailAddress = Email.Trim();
                person.SendEmailAddress1 = true;

                if (count == 0)
                {
                    person.Comments = "Added during api postcontribution because record was not found";
                }
            }

            var c = person.PostUnattendedContribution(db, Amount, Fundid, Notes);
            c.ContributionDate = Date ?? DateTime.Now;
            c.MetaInfo = Source;
            db.SubmitChanges();
            result.PeopleId = person.PeopleId;
            result.ContributionId = c.ContributionId;
            result.Source = Source;
            DbUtil.LogActivity($"ApiPostContribution {result}", person.PeopleId);
            return result;
        }

        public class Result
        {
            public int PeopleId { get; set; }
            public int ContributionId { get; set; }
            public bool NewPerson { get; set; }
            public bool MultipleMatches { get; set; }
            public string Source { get; set; }
            public override string ToString()
            {
                return $"Id:{ContributionId},Source:{Source},NewPerson:{NewPerson},MultipleMatches:{MultipleMatches}";
            }
        }
    }
}
