using System;
using System.Collections.Generic;
using System.Linq;
using ImageData;
using IronPython.Modules;
using UtilityExtensions;
using System.Text;

namespace CmsData.View
{
    public partial class OrgPerson
    {
        public string CityStateZip
        {
            get { return Util.FormatCSZ4(City,St,Zip); }
        }

        public string BirthDate
        {
            get { return Util.FormatBirthday( BirthYear, BirthMonth, BirthDay); }
        }
    }
}
