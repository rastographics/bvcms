using System.Collections.Generic;
using System.Text;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class InvolvementInfo
    {
        public string Info
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendFormat("{0}\n", Name);
                if (Addr.HasValue())
                    sb.AppendFormat("{0}\n", Addr);
                if (Addr2.HasValue())
                    sb.AppendFormat("{0}\n", Addr2);
                if (City.HasValue())
                    sb.AppendFormat("{0}, ", City);
                if (State.HasValue())
                    sb.AppendFormat("{0} ", State);
                if (Zip.HasValue())
                    sb.AppendFormat("{0}\n", Zip);
                if (HomePhone.HasValue())
                    sb.AppendFormat("H {0}\n", HomePhone.FmtFone());
                if (WorkPhone.HasValue())
                    sb.AppendFormat("W {0}\n", WorkPhone.FmtFone());
                if (CellPhone.HasValue())
                    sb.AppendFormat("C {0}\n", CellPhone.FmtFone());
                if (Email.HasValue())
                    sb.AppendFormat("{0}\n", Email);
                sb.Append("ID# " + PeopleId);
                return sb.ToString();
            }
        }

        public string Name { get; set; }
        public string Addr { get; set; }
        public string Addr2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string CellPhone { get; set; }
        public string Email { get; set; }
        public int PeopleId { get; set; }

        public string MainFellowship
        {
            get
            {
                if (!OrgName.HasValue())
                    return "";
                var s = $"{DivName}, {OrgName}, {Teacher}, {MemberType}";
                if (AttendPct.HasValue)
                    s += $", {AttendPct.Value:n1}%";
                return s;
            }
        }

        public string DivName { get; set; }
        public string OrgName { get; set; }
        public string Teacher { get; set; }
        public string MemberType { get; set; }
        public decimal? AttendPct { get; set; }


        public string Spouse { get; set; }
        internal int? AgeDb;
        public int Age => Person.AgeDisplay(AgeDb, PeopleId) ?? 0;
        public string JoinInfo { get; set; }

        // ReSharper disable once InconsistentNaming
        internal IEnumerable<ExportInvolvements.ActivityInfo> activities;

        public string ActivityInfo
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var o in activities)
                {

                    sb.AppendFormat("{0} - {1}", o.Name, o.Leader);
                    if (o.Pct.HasValue)
                        sb.AppendFormat(", {0:n1}%", o.Pct.Value);
                    sb.Append("\n");
                }
                return sb.ToString();
            }
        }

        public string Notes { get; set; }
        public string OfficeUseOnly { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Campus { get; set; }
        public string CampusDate { get; set; }

    }
}
