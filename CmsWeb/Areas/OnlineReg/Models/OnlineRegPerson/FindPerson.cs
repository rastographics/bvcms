using CmsData;
using System;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {
        private Person _person;

        public Person person
        {
            get
            {
                if (_person == null)
                {
                    if (PeopleId.HasValue)
                    {
                        _person = CurrentDatabase.LoadPersonById(PeopleId.Value);
                        count = 1;
                    }
                    else
                    {
                        var list = CurrentDatabase.FindPerson(FirstName, LastName, SqlDate(birthday), EmailAddress, Phone.GetDigits()).ToList();
                        count = list.Count;
                        if (count == 1)
                        {
                            _person = CurrentDatabase.LoadPersonById(list[0].PeopleId ?? 0);
                        }

                        if (_person != null)
                        {
                            PeopleId = _person.PeopleId;
                        }
                        else
                        {
                            Log("PersonNotFound");
                        }
                    }
                }

                return _person;
            }
        }

        private DateTime? SqlDate(DateTime? birthday)
        {
            if (birthday.HasValue)
            {
                var d = birthday.Value;
                if (d.Year < 1900)
                {
                    birthday = new DateTime(1900 + (d.Year % 100), d.Month, d.Day);
                }
            }
            return birthday;
        }
    }
}
