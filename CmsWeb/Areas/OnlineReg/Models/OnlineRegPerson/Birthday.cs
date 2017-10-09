using System;
using System.Linq;
using System.Text.RegularExpressions;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {
        internal int? bmon, byear, bday;

        private DateTime _Birthday;
        public DateTime? birthday
        {
            get
            {
                if (_Birthday == DateTime.MinValue)
                    Util.BirthDateValid(bmon, bday, byear, out _Birthday);
                return _Birthday == DateTime.MinValue ? (DateTime?)null : _Birthday;
            }
        }
        public int GetAge(DateTime bd)
        {
            int y = bd.Year;
            if (y == Util.SignalNoYear)
                return 0;
            if (y < 1000)
                if (y < 50)
                    y = y + 2000;
                else y = y + 1900;
            var dt = DateTime.Today;
            int age = dt.Year - y;
            if (dt.Month < bd.Month || (dt.Month == bd.Month && dt.Day < bd.Day))
                age--;
            return age;
        }
        public string DateOfBirth
        {
            get
            {
                return Person.FormatBirthday(byear, bmon, bday, PeopleId);
            }
            set
            {
                bday = null;
                bmon = null;
                byear = null;
                DateTime dt;
                if (DateTime.TryParse(value, out dt))
                {
                    bday = dt.Day;
                    bmon = dt.Month;
                    if (Regex.IsMatch(value, @"\d+/\d+/\d+"))
                        byear = dt.Year;
                }
                else
                {
                    int n;
                    if (int.TryParse(value, out n))
                        if (n >= 1 && n <= 12)
                            bmon = n;
                        else
                            byear = n;
                }
            }
        }

        public bool DateValid()
        {
            return bmon.HasValue && byear.HasValue && bday.HasValue;
        }
    }
}
