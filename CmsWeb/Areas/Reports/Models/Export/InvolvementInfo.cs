using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                sb.AppendFormat("<strong>{0}</strong><br/>", _Name);
                if (_Addr.HasValue())
                    sb.AppendFormat("{0}<br/>", _Addr);
                if (_Addr2.HasValue())
                    sb.AppendFormat("{0}<br/>", _Addr2);
                if (_City.HasValue())
                    sb.AppendFormat("{0}, ", _City);
                if (_State.HasValue())
                    sb.AppendFormat("{0} ", _State);
                if (Zip.HasValue())
                    sb.AppendFormat("{0}<br/>", Zip);
                if (_HomePhone.HasValue())
                    sb.AppendFormat("H {0}<br/>", _HomePhone.FmtFone());
                if (_WorkPhone.HasValue())
                    sb.AppendFormat("W {0}<br/>", _WorkPhone.FmtFone());
                if (_CellPhone.HasValue())
                    sb.AppendFormat("C {0}<br/>", _CellPhone.FmtFone());
                if (_Email.HasValue())
                    sb.AppendFormat("{0}<br/>", _Email);
                sb.Append("ID# " + _PeopleId);
                return sb.ToString();
            }
        }
        private string _Name;
        public string Name
        {
            set { _Name = value; }
        }
        private string _Addr;
        public string Addr
        {
            set { _Addr = value; }
        }
        private string _Addr2;
        public string Addr2
        {
            set
            {
                if (_Addr2 == value)
                    return;
                _Addr2 = value;
            }
        }
        private string _City;
        public string City
        {
            set { _City = value; }
        }
        private string _State;
        public string State
        {
            set { _State = value; }
        }
        public string Zip { get; set; }
        private string _HomePhone;
        public string HomePhone
        {
            set { _HomePhone = value; }
        }
        private string _WorkPhone;
        public string WorkPhone
        {
            set { _WorkPhone = value; }
        }
        private string _CellPhone;
        public string CellPhone
        {
            set { _CellPhone = value; }
        }
        private string _Email;
        public string Email
        {
            set { _Email = value; }
        }
        private int _PeopleId;
        public int PeopleId
        {
            set { _PeopleId = value; }
        }

        public string BfcClasses
        {
            get
            {
                if (!_OrgName.HasValue())
                    return "";
                var s = "{0}, {1}, {2}, {3}"
                    .Fmt(_DivName, _OrgName, _Teacher, _MemberType);
                if (_AttendPct.HasValue)
                    s += ", {0:n1}%".Fmt(_AttendPct.Value);
                return s;
            }
        }
        private string _DivName;
        public string DivName
        {
            set { _DivName = value; }
        }
        private string _OrgName;
        public string OrgName
        {
            set { _OrgName = value; }
        }
        private string _Teacher;
        public string Teacher
        {
            set { _Teacher = value; }
        }
        private string _MemberType;
        public string MemberType
        {
            set { _MemberType = value; }
        }
        private decimal? _AttendPct;
        public decimal? AttendPct
        {
            set { _AttendPct = value; }
        }

        
        public string Spouse { get; set; }
        public int Age { get; set; }
        public string JoinInfo { get; set; }
        private IEnumerable<ExportInvolvements.ActivityInfo> _Activities;
        public IEnumerable<ExportInvolvements.ActivityInfo> activities
        {
            set { _Activities = value; }
        }
        
        public string Activities
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var o in _Activities)
                {

                    sb.AppendFormat("{0} - {1}", o.Name, o.Leader);
                    if (o.Pct.HasValue)
                        sb.AppendFormat(", {0:n1}%", o.Pct.Value);
					sb.Append("<br/>");
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
