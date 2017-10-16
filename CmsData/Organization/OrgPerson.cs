using System.Collections.Generic;
using UtilityExtensions;
using System.Text;
using System.Web;
using CmsData.Codes;

namespace CmsData.View
{
    public partial class OrgFilterPerson
    {
        public string CityStateZip => Util.FormatCSZ4(City,St,Zip);

        public HtmlString AddressBlock
        {
            get
            {
                var sb = new StringBuilder();
                if (Address.HasValue())
                    sb.Append(Address);
                if (Address2.HasValue())
                {
                    if (sb.Length > 0)
                        sb.Append("<br>");
                    sb.Append(Address2);
                }
                var csz = CityStateZip;
                if (csz.HasValue())
                {
                    if (sb.Length > 0)
                        sb.Append("<br>");
                    sb.Append(csz);
                }
                if (sb.Length > 0)
                {
                    sb.Insert(0, "<div>");
                    sb.Append("</div>");
                }
                return new HtmlString(sb.ToString());
            }
        }

        public string Group => GroupCode == GroupSelectCode.Member ? "Member"
            : GroupCode == GroupSelectCode.Inactive ? "Inactive"
                : "NonMember";

        public string BirthDate => Person.FormatBirthday(BirthYear, BirthMonth, BirthDay, PeopleId);

        public IEnumerable<string> Phones
        {
            get
            {
                var phones = new List<string>();
                if(CellPhone.HasValue())
                    phones.Add(CellPhone.FmtFone("C"));
                if(HomePhone.HasValue())
                    phones.Add(HomePhone.FmtFone("H"));
                if(WorkPhone.HasValue())
                    phones.Add(WorkPhone.FmtFone("W"));
                return phones;
            }
        }

        public long? LastAttendedTicks => LastAttended?.Ticks;
    }
}
