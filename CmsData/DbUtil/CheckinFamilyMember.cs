using System;
using System.Linq;
using UtilityExtensions;
using System.Text;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData.View
{
    public partial class CheckinFamilyMember
    {
        public string BirthDay
        {
            get { return Util.FormatBirthday(BYear, BMon, BDay); }
        }
        public string DisplayName
        {
            get
            {
                if (Age <= 18)
                    return "{0} ({1})".Fmt(Name, Age);
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
                var dt = DateTime.MinValue;
                DateTime? bd = null;
                if (DateTime.TryParse(BirthDay, out dt))
                    bd = dt;
                return bd.FormatDate();
            }
        }
    }
}
