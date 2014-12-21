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

        public IEnumerable<string> Phones
        {
            get
            {
                var phones = new List<string>();
                if(CellPhone.HasValue())
                    phones.Add(CellPhone.FmtFone("C"));
                if(HomePhone.HasValue())
                    phones.Add(WorkPhone.FmtFone("H"));
                if(WorkPhone.HasValue())
                    phones.Add(WorkPhone.FmtFone("W"));
                return phones;
            }
        }
    }
}
