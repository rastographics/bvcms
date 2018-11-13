using System.Linq;
using CmsData;
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
                    if (PeopleId.HasValue)
                    {
                        _person = CurrentDatabase.LoadPersonById(PeopleId.Value);
                        count = 1;
                    }
                    else
                    {
                        var list = CurrentDatabase.FindPerson(FirstName, LastName, birthday, EmailAddress, Phone.GetDigits()).ToList();
                        count = list.Count;
                        if (count == 1)
                            _person = CurrentDatabase.LoadPersonById(list[0].PeopleId ?? 0);
                        if (_person != null)
                            PeopleId = _person.PeopleId;
                        else
                            Log("PersonNotFound");
                    }
                return _person;
            }
        }
    }
}