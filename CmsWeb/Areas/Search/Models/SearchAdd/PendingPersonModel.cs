using CmsData;
using CmsWeb.Areas.People.Models;
using CmsWeb.Code;
using CmsWeb.Constants;
using CmsWeb.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class PendingPersonModel : IDbBinder
    {
        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public PendingPersonModel() { }
        public PendingPersonModel(CMSDataContext db)
        {
            CurrentDatabase = db;            
        }

        public CMSDataContext CurrentDatabase { get; set; }
        public int index { get; set; }
        public string context { get; set; }
        private string _FirstName;
        [StringLength(25), Required(ErrorMessage = "required, or put 'na' if not known"), RemoveNA]
        public string FirstName {
            get
            {
                return (this.IsBusiness) ? "na" : _FirstName;
            }
            set {
                _FirstName = value;
            }
        }        

        [StringLength(25)]
        public string NickName { get; set; }

        [StringLength(25)]
        public string MiddleName { get; set; }
        private string _LastName;
        [StringLength(100), Required(ErrorMessage = "required")]
        public string LastName
        {
            get { return _LastName ; }
            set { _LastName = value; }
        }
        public string LastNameLabel
        {
            get
            {
                return (IsBusiness) ? "Business/Entity name" : "Last Name";
            }
        }

        [StringLength(10)]
        public string TitleCode { get; set; }

        [StringLength(10)]
        public string SuffixCode { get; set; }

        [DisplayName("Birthday")]
        [DateEmptyOrValid]
        [BirthdateValid]
        public string DOB { get; set; }

        [StringLength(20), RemoveNA]
        public string CellPhone { get; set; }

        [StringLength(150), EmailAddress, RemoveNA]
        public string EmailAddress { get; set; }

        private CodeInfo _Gender = new CodeInfo(99, "Gender");
        [UnallowedCode("99", ErrorMessage = "specify gender (or unknown)")]
        public CodeInfo Gender
        {
            get
            {
                return IsBusiness ? new CodeInfo(0, "Gender") : _Gender;
            }
            set { _Gender = value; }
        }

        private CodeInfo _MaritalStatus = new CodeInfo(99, "MaritalStatus");

        [UnallowedCode("99", ErrorMessage = "specify marital status (or unknown)")]
        public CodeInfo MaritalStatus
        {
            get
            {
                return IsBusiness ? new CodeInfo(0, "MaritalStatus") : _MaritalStatus;
            }
            set { _MaritalStatus = value; }
        }

        public CodeInfo Campus { get; set; }

        public CodeInfo EntryPoint { get; set; }

        [StringLength(20), RemoveNA]
        public string HomePhone { get; set; }

        private DateTime? birthday;

        public DateTime? Birthday
        {
            get
            {
                DateTime dt;
                if (!birthday.HasValue && DOB.NotEqual("na"))
                {
                    if (Util.DateValid(DOB, out dt))
                    {
                        birthday = dt;
                    }
                }

                return birthday;
            }
        }

        public int? Age
        {
            get
            {
                if (Birthday.HasValue)
                {
                    return Birthday.Value.AgeAsOf(Util.Now);
                }

                return null;
            }
        }

        public bool IsNew => !PeopleId.HasValue;

        [NoUpdate]
        public int FamilyId { get; set; }
        private Family _family;
        public Family Family
        {
            get
            {
                if (_family == null && FamilyId > 0)
                {
                    _family = CurrentDatabase.Families.Single(f => f.FamilyId == FamilyId);
                }

                return _family;
            }
        }

        [NoUpdate]
        public int? PeopleId { get; set; }
        private Person person;
        public Person Person
        {
            get
            {
                if (person == null && PeopleId.HasValue)
                {
                    person = CurrentDatabase.LoadPersonById(PeopleId.Value);
                }

                return person;
            }
        }

        public AddressInfo AddressInfo { get; set; }

        public string PotentialDuplicate { get; set; }

        internal void CheckDuplicate()
        {
            var pids = CurrentDatabase.FindPerson(FirstName, LastName, Birthday, null, CellPhone.GetDigits()).Select(pp => pp.PeopleId).ToList();
            var q = from p in CurrentDatabase.People
                    where pids.Contains(p.PeopleId)
                    select new { p.PeopleId, p.Name, p.PrimaryAddress, p.Age, };
            var sb = new StringBuilder();
            foreach (var p in q)
            {
                if (sb.Length == 0)
                {
                    sb.AppendLine("<ul>\n");
                }

                sb.AppendFormat("<li><a href=\"/Person2/{1}\" target=\"_blank\">{0}</a> ({1}), {2}, age:{3}</li>\n", p.Name, p.PeopleId, p.PrimaryAddress, p.Age);
            }
            if (sb.Length > 0)
            {
                PotentialDuplicate = sb + "</ul>\n";
            }
        }

        public bool IsNewFamily { get; set; }
        private bool _IsBusiness;
        [DisplayName("Add a business/entity")]
        public bool IsBusiness {
            get { return _IsBusiness; }
            set { _IsBusiness = value; }
        }

        internal void AddPerson(int originid, int? entrypointid, int? campusid, bool? isbusiness = false)
        {
            Family f;
            if (FamilyId > 0)
            {
                f = Family;
            }
            else
            {
                f = new Family();
                AddressInfo.CopyPropertiesTo(f);
                f.ResCodeId = AddressInfo.ResCode.Value.ToInt2();
                f.HomePhone = HomePhone.GetDigits();
            }
            NickName = NickName?.Trim();

            var position = CurrentDatabase.ComputePositionInFamily(Age ?? -1, MaritalStatus.Value == "20", FamilyId) ?? 10;

            FirstName = FirstName.Trim();
            if (FirstName == "na")
            {
                FirstName = "";
            }

            person = Person.Add(CurrentDatabase, f, position,
                                 null, FirstName.Trim(), NickName, LastName.Trim(), DOB, false, Gender.Value.ToInt(),
                                 originid, entrypointid, isbusiness: (bool)isbusiness);

            this.CopyPropertiesTo(Person);
            Person.CellPhone = CellPhone.GetDigits();

            if (campusid == 0)
            {
                campusid = null;
            }

            Person.CampusId = Util.PickFirst(campusid.ToString(), CurrentDatabase.Setting("DefaultCampusId", "")).ToInt2();
            if (Person.CampusId == 0)
            {
                Person.CampusId = null;
            }

            CurrentDatabase.SubmitChanges();
            DbUtil.LogActivity($"{((bool)person.IsBusiness ? "AddBusiness" : "AddPerson")} {person.PeopleId}");
            CurrentDatabase.Refresh(RefreshMode.OverwriteCurrentValues, Person);
            PeopleId = Person.PeopleId;
        }

        public void LoadAddress()
        {
            if (FamilyId <= 0)
            {
                AddressInfo = new AddressInfo();
                HomePhone = "";
#if DEBUG
                AddressInfo.AddressLineOne = "235 revere";
                AddressInfo.ZipCode = "38018";
#endif
                return;
            }
            var f = Family;
            AddressInfo = new AddressInfo(f.AddressLineOne, f.AddressLineTwo, f.CityName, f.StateCode, f.ZipCode, f.CountryName);
            HomePhone = f.HomePhone.GetDigits();
        }

        public string CityStateZip => $"{AddressInfo.CityName}, {AddressInfo.StateCode.Value} {AddressInfo.ZipCode}";
    }
}
