using System;
using System.Globalization;
using UtilityExtensions;

namespace CmsData.View
{
    public partial class CheckinFamilyMember
    {
        public string BirthDay => Person.FormatBirthday(BYear, BMon, BDay, null, null);

        public string DisplayName
        {
            get
            {
                if (Age <= 18)
                    return $"{Name} ({Age})";
                return Name;
            }
        }
        public string DisplayClass
        {
            get
            {
                string s = "";
                if (Location.HasValue())
                    if (!ClassX.StartsWith(Location))
                        s = Location + ", ";
                s += ClassX;
                if (Leader.HasValue())
                    s += ", " + Leader;
                return s;
            }
        }
        public string OrgName
        {
            get
            {
                string s = ClassX;
                if (Leader.HasValue())
                    s += ", " + Leader;
                return s;
            }
        }

        public string dob
        {
            get
            {
                DateTime dt;
                DateTime? bd = null;
                var parsed = Util.IsCultureUS()
                    ? BirthDay.DateTryParse(out dt)
                    : BirthDay.DateTryParseUS(out dt);
                if(parsed)
                    bd = dt;
                return Util.IsCultureUS()
                    ? bd.FormatDate()
                    : bd.FormatDateUS();
            }
        }
    }
}
