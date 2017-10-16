using System;
using System.Text;
using System.Web;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class SearchResultInfo
    {
        public int PeopleId { get; set; }
        public int FamilyId { get; set; }
        public string Name { get; set; }
        public string Middle { get; set; }
        public string Maiden { get; set; }
        public string GoesBy { get; set; }
        public string First { get; set; }
        public string Address { get; set; }
        public string CityStateZip { get; set; }
        public int? Age { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        public string WorkPhone { get; set; }
        public string MemberStatus { get; set; }
        public DateTime? JoinDate { get; set; }
        public int? BirthDay { get; set; }
        public int? BirthMon { get; set; }
        public int? BirthYear { get; set; }
        public string BirthDate => Person.FormatBirthday(BirthYear, BirthMon, BirthDay, PeopleId);
        public string Email { get; set; }

        public HtmlString ToolTip
        {
            get
            {
                var ret = new StringBuilder();
                ret.AppendFormat("PeopleId: {0}<br>\n", PeopleId);
                if (CellPhone.HasValue())
                    ret.AppendFormat("{0}&nbsp;&nbsp;", CellPhone.FmtFone("C "));
                if (HomePhone.HasValue())
                    ret.AppendFormat("{0}&nbsp;&nbsp;", HomePhone.FmtFone("H "));
                if (WorkPhone.HasValue())
                    ret.AppendFormat("{0}&nbsp;&nbsp;", WorkPhone.FmtFone("W "));
                if (Email.HasValue())
                    ret.AppendFormat("<a>{0}</a>&nbsp;&nbsp;", Email);
                if (ret.Length > 0)
                    ret.Append("<br>\n");

                var names = new StringBuilder();
                if (GoesBy.HasValue() && First != GoesBy)
                    names.AppendFormat("{0}first: {1}", names.Length > 0 ? ", " : "", First);
                if (Middle.HasValue())
                    names.AppendFormat("{0}middle: {1}", names.Length > 0 ? ", " : "", Middle);
                if (Maiden.HasValue())
                    names.AppendFormat("{0}maiden: {1}", names.Length > 0 ? ", " : "", Maiden);
                if (names.Length > 0)
                    ret.AppendFormat("{0}<br>\n", names);

                if (BirthDate.HasValue())
                    ret.AppendFormat("Birthday: {0}&nbsp;&nbsp;", BirthDate);
                ret.AppendFormat("[<i>{0}</i>]&nbsp;&nbsp;", MemberStatus);
                if (JoinDate.HasValue)
                    ret.Append("Joined: " + JoinDate.ToDate().FormatDate());

                if (CityStateZip.HasValue())
                    ret.AppendFormat("<br>\n{0}", CityStateZip);

                return new HtmlString(ret.ToString());
            }
        }
    }
}
