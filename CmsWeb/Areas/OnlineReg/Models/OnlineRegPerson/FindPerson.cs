using CmsData;
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
                        _person = DbUtil.Db.LoadPersonById(PeopleId.Value);
                        count = 1;
                    }
                    else
                    {
                        var list = DbUtil.Db.FindPerson(FirstName, LastName, birthday, EmailAddress, Phone.GetDigits()).ToList();
                        count = list.Count;
                        if (count == 1)
                        {
                            _person = DbUtil.Db.LoadPersonById(list[0].PeopleId ?? 0);
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
    }
}
