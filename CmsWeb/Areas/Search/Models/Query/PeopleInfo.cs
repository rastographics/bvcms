/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System.Collections.Generic;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class PeopleInfo
    {
        private enum PhoneType
        {
            Home, Cell, Work
        }
        public int PeopleId { get; set; }
        public string MemberStatus { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string AltName { get; set; }
        public string BirthDate => Person.FormatBirthday(BirthYear, BirthMon, BirthDay, PeopleId);
        public int? BirthYear { get; set; }
        public int? BirthMon { get; set; }
        public int? BirthDay { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string CityStateZip { get; set; }
        public string Email { get; set; }
        public string Age { get; set; }
        private int _PhonePref;
        public int PhonePref { set { _PhonePref = value; } }
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
        public string BFTeacher { get; set; }
        public string Employer { get; set; }
        public int? BFTeacherId { get; set; }
        public bool Deceased { get; set; }
        public bool HasTag { get; set; }
        
    }
}
