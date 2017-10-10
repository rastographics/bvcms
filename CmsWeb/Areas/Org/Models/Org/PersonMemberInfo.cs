using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using UtilityExtensions;
using System.Web.Mvc;

namespace CmsWeb.Areas.Org.Models
{
    public class PersonMemberInfo
    {
        public int PeopleId { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string BirthDate => Person.FormatBirthday(BirthYear, BirthMon, BirthDay, PeopleId);
        public int? BirthYear { get; set; }
        public int? BirthMon { get; set; }
        public int? BirthDay { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string CityStateZip { get; set; }
        public string EmailAddress { get; set; }
        private int _PhonePref;
        public int PhonePref { set { _PhonePref = value; } }
        private string _HomePhone;
        public string HomePhone
        {
            set
            {
                if (value.HasValue())
                {
                    _HomePhone = PhoneFmt(string.Empty, PhoneType.Home, value);
                    _Phones.Add(PhoneFmt("H", PhoneType.Home, value));
                }
            }
            get { return _HomePhone; }
        }
        private string _CellPhone;
        public string CellPhone
        {
            set
            {
                if (value.HasValue())
                {
                    _CellPhone = PhoneFmt(string.Empty, PhoneType.Cell, value);
                    _Phones.Add(PhoneFmt("C", PhoneType.Cell, value));
                }
            }
            get { return _CellPhone; }
        }

        private string _WorkPhone;
        public string WorkPhone
        {
            set
            {
                if (value.HasValue())
                {
                    _WorkPhone = PhoneFmt(string.Empty, PhoneType.Work, value);
                    _Phones.Add(PhoneFmt("W", PhoneType.Work, value));
                }
            }
            get { return _WorkPhone; }
        }
        public string MemberStatus { get; set; }
        public string Email { get; set; }
        public string BFTeacher { get; set; }
        public int? BFTeacherId { get; set; }
        public string Age { get; set; }
        public string MemberTypeCode { get; set; }
        public string MemberType { get; set; }
        public int MemberTypeId { get; set; }
        public DateTime? InactiveDate { get; set; }
        public decimal? AttendPct { get; set; }
        public DateTime? LastAttended { get; set; }
        public bool Hidden { get; set; }
        public bool HasTag { get; set; }
        public int FromTab { get; set; }
        public DateTime? Joined { get; set; }
        public DateTime? Dropped { get; set; }
        private enum PhoneType
        {
            Home, Cell, Work
        }
        private string PhoneFmt(string prefix, PhoneType type, string number)
        {
            var s = number.FmtFone(type + " ");
            if ((type == PhoneType.Home && _PhonePref == 10)
                || (type == PhoneType.Cell && _PhonePref == 20)
                || (type == PhoneType.Work && _PhonePref == 30))
                return number.FmtFone("*" + prefix + " ");
            return number.FmtFone(prefix + " ");
        }
        private List<string> _Phones = new List<string>();
        public List<string> Phones
        {
            get { return _Phones; }
        }
    }
}
