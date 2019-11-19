using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.RecReg")]
    public partial class RecReg : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int? _PeopleId;

        private int? _ImgId;

        private bool? _IsDocument;

        private bool? _ActiveInAnotherChurch;

        private string _ShirtSize;

        private bool? _MedAllergy;

        private string _Email;

        private string _MedicalDescription;

        private string _Fname;

        private string _Mname;

        private bool? _Coaching;

        private bool? _Member;

        private bool? _NoMember;

        private string _Emcontact;

        private string _Emphone;

        private string _Doctor;

        private string _Docphone;

        private string _Insurance;

        private string _Policy;

        private string _PassportNumber;

        private string _PassportExpires;

        private string _Comments;

        private bool? _Tylenol;

        private bool? _Advil;

        private bool? _Maalox;

        private bool? _Robitussin;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnPeopleIdChanging(int? value);
        partial void OnPeopleIdChanged();

        partial void OnImgIdChanging(int? value);
        partial void OnImgIdChanged();

        partial void OnIsDocumentChanging(bool? value);
        partial void OnIsDocumentChanged();

        partial void OnActiveInAnotherChurchChanging(bool? value);
        partial void OnActiveInAnotherChurchChanged();

        partial void OnShirtSizeChanging(string value);
        partial void OnShirtSizeChanged();

        partial void OnMedAllergyChanging(bool? value);
        partial void OnMedAllergyChanged();

        partial void OnEmailChanging(string value);
        partial void OnEmailChanged();

        partial void OnMedicalDescriptionChanging(string value);
        partial void OnMedicalDescriptionChanged();

        partial void OnFnameChanging(string value);
        partial void OnFnameChanged();

        partial void OnMnameChanging(string value);
        partial void OnMnameChanged();

        partial void OnCoachingChanging(bool? value);
        partial void OnCoachingChanged();

        partial void OnMemberChanging(bool? value);
        partial void OnMemberChanged();

        partial void OnNoMemberChanging(bool? value);
        partial void OnNoMemberChanged();

        partial void OnEmcontactChanging(string value);
        partial void OnEmcontactChanged();

        partial void OnEmphoneChanging(string value);
        partial void OnEmphoneChanged();

        partial void OnDoctorChanging(string value);
        partial void OnDoctorChanged();

        partial void OnDocphoneChanging(string value);
        partial void OnDocphoneChanged();

        partial void OnInsuranceChanging(string value);
        partial void OnInsuranceChanged();

        partial void OnPolicyChanging(string value);
        partial void OnPolicyChanged();

        partial void OnPassportNumberChanging(string value);
        partial void OnPassportNumberChanged();

        partial void OnPassportExpiresChanging(string value);
        partial void OnPassportExpiresChanged();

        partial void OnCommentsChanging(string value);
        partial void OnCommentsChanged();

        partial void OnTylenolChanging(bool? value);
        partial void OnTylenolChanged();

        partial void OnAdvilChanging(bool? value);
        partial void OnAdvilChanged();

        partial void OnMaaloxChanging(bool? value);
        partial void OnMaaloxChanged();

        partial void OnRobitussinChanging(bool? value);
        partial void OnRobitussinChanged();

        #endregion

        public RecReg()
        {
            _Person = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    SendPropertyChanging();
                    _Id = value;
                    SendPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int")]
        [IsForeignKey]
        public int? PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    if (_Person.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnPeopleIdChanging(value);
                    SendPropertyChanging();
                    _PeopleId = value;
                    SendPropertyChanged("PeopleId");
                    OnPeopleIdChanged();
                }
            }
        }

        [Column(Name = "ImgId", UpdateCheck = UpdateCheck.Never, Storage = "_ImgId", DbType = "int")]
        public int? ImgId
        {
            get => _ImgId;

            set
            {
                if (_ImgId != value)
                {
                    OnImgIdChanging(value);
                    SendPropertyChanging();
                    _ImgId = value;
                    SendPropertyChanged("ImgId");
                    OnImgIdChanged();
                }
            }
        }

        [Column(Name = "IsDocument", UpdateCheck = UpdateCheck.Never, Storage = "_IsDocument", DbType = "bit")]
        public bool? IsDocument
        {
            get => _IsDocument;

            set
            {
                if (_IsDocument != value)
                {
                    OnIsDocumentChanging(value);
                    SendPropertyChanging();
                    _IsDocument = value;
                    SendPropertyChanged("IsDocument");
                    OnIsDocumentChanged();
                }
            }
        }

        [Column(Name = "ActiveInAnotherChurch", UpdateCheck = UpdateCheck.Never, Storage = "_ActiveInAnotherChurch", DbType = "bit")]
        public bool? ActiveInAnotherChurch
        {
            get => _ActiveInAnotherChurch;

            set
            {
                if (_ActiveInAnotherChurch != value)
                {
                    OnActiveInAnotherChurchChanging(value);
                    SendPropertyChanging();
                    _ActiveInAnotherChurch = value;
                    SendPropertyChanged("ActiveInAnotherChurch");
                    OnActiveInAnotherChurchChanged();
                }
            }
        }

        [Column(Name = "ShirtSize", UpdateCheck = UpdateCheck.Never, Storage = "_ShirtSize", DbType = "nvarchar(50)")]
        public string ShirtSize
        {
            get => _ShirtSize;

            set
            {
                if (_ShirtSize != value)
                {
                    OnShirtSizeChanging(value);
                    SendPropertyChanging();
                    _ShirtSize = value;
                    SendPropertyChanged("ShirtSize");
                    OnShirtSizeChanged();
                }
            }
        }

        [Column(Name = "MedAllergy", UpdateCheck = UpdateCheck.Never, Storage = "_MedAllergy", DbType = "bit")]
        public bool? MedAllergy
        {
            get => _MedAllergy;

            set
            {
                if (_MedAllergy != value)
                {
                    OnMedAllergyChanging(value);
                    SendPropertyChanging();
                    _MedAllergy = value;
                    SendPropertyChanged("MedAllergy");
                    OnMedAllergyChanged();
                }
            }
        }

        [Column(Name = "email", UpdateCheck = UpdateCheck.Never, Storage = "_Email", DbType = "nvarchar(80)")]
        public string Email
        {
            get => _Email;

            set
            {
                if (_Email != value)
                {
                    OnEmailChanging(value);
                    SendPropertyChanging();
                    _Email = value;
                    SendPropertyChanged("Email");
                    OnEmailChanged();
                }
            }
        }

        [Column(Name = "MedicalDescription", UpdateCheck = UpdateCheck.Never, Storage = "_MedicalDescription", DbType = "nvarchar(1000)")]
        public string MedicalDescription
        {
            get => _MedicalDescription;

            set
            {
                if (_MedicalDescription != value)
                {
                    OnMedicalDescriptionChanging(value);
                    SendPropertyChanging();
                    _MedicalDescription = value;
                    SendPropertyChanged("MedicalDescription");
                    OnMedicalDescriptionChanged();
                }
            }
        }

        [Column(Name = "fname", UpdateCheck = UpdateCheck.Never, Storage = "_Fname", DbType = "nvarchar(80)")]
        public string Fname
        {
            get => _Fname;

            set
            {
                if (_Fname != value)
                {
                    OnFnameChanging(value);
                    SendPropertyChanging();
                    _Fname = value;
                    SendPropertyChanged("Fname");
                    OnFnameChanged();
                }
            }
        }

        [Column(Name = "mname", UpdateCheck = UpdateCheck.Never, Storage = "_Mname", DbType = "nvarchar(80)")]
        public string Mname
        {
            get => _Mname;

            set
            {
                if (_Mname != value)
                {
                    OnMnameChanging(value);
                    SendPropertyChanging();
                    _Mname = value;
                    SendPropertyChanged("Mname");
                    OnMnameChanged();
                }
            }
        }

        [Column(Name = "coaching", UpdateCheck = UpdateCheck.Never, Storage = "_Coaching", DbType = "bit")]
        public bool? Coaching
        {
            get => _Coaching;

            set
            {
                if (_Coaching != value)
                {
                    OnCoachingChanging(value);
                    SendPropertyChanging();
                    _Coaching = value;
                    SendPropertyChanged("Coaching");
                    OnCoachingChanged();
                }
            }
        }

        [Column(Name = "member", UpdateCheck = UpdateCheck.Never, Storage = "_Member", DbType = "bit")]
        public bool? Member
        {
            get => _Member;

            set
            {
                if (_Member != value)
                {
                    OnMemberChanging(value);
                    SendPropertyChanging();
                    _Member = value;
                    SendPropertyChanged("Member");
                    OnMemberChanged();
                }
            }
        }

        [Column(Name = "nomember", UpdateCheck = UpdateCheck.Never, Storage = "_NoMember", DbType = "bit")]
        public bool? NoMember
        {
            get => _NoMember;

            set
            {
                if (_NoMember != value)
                {
                    OnNoMemberChanging(value);
                    SendPropertyChanging();
                    _NoMember = value;
                    SendPropertyChanged("NoMember");
                    OnNoMemberChanged();
                }
            }
        }

        [Column(Name = "emcontact", UpdateCheck = UpdateCheck.Never, Storage = "_Emcontact", DbType = "nvarchar(100)")]
        public string Emcontact
        {
            get => _Emcontact;

            set
            {
                if (_Emcontact != value)
                {
                    OnEmcontactChanging(value);
                    SendPropertyChanging();
                    _Emcontact = value;
                    SendPropertyChanged("Emcontact");
                    OnEmcontactChanged();
                }
            }
        }

        [Column(Name = "emphone", UpdateCheck = UpdateCheck.Never, Storage = "_Emphone", DbType = "nvarchar(50)")]
        public string Emphone
        {
            get => _Emphone;

            set
            {
                if (_Emphone != value)
                {
                    OnEmphoneChanging(value);
                    SendPropertyChanging();
                    _Emphone = value;
                    SendPropertyChanged("Emphone");
                    OnEmphoneChanged();
                }
            }
        }

        [Column(Name = "doctor", UpdateCheck = UpdateCheck.Never, Storage = "_Doctor", DbType = "nvarchar(100)")]
        public string Doctor
        {
            get => _Doctor;

            set
            {
                if (_Doctor != value)
                {
                    OnDoctorChanging(value);
                    SendPropertyChanging();
                    _Doctor = value;
                    SendPropertyChanged("Doctor");
                    OnDoctorChanged();
                }
            }
        }

        [Column(Name = "docphone", UpdateCheck = UpdateCheck.Never, Storage = "_Docphone", DbType = "nvarchar(50)")]
        public string Docphone
        {
            get => _Docphone;

            set
            {
                if (_Docphone != value)
                {
                    OnDocphoneChanging(value);
                    SendPropertyChanging();
                    _Docphone = value;
                    SendPropertyChanged("Docphone");
                    OnDocphoneChanged();
                }
            }
        }

        [Column(Name = "insurance", UpdateCheck = UpdateCheck.Never, Storage = "_Insurance", DbType = "nvarchar(100)")]
        public string Insurance
        {
            get => _Insurance;

            set
            {
                if (_Insurance != value)
                {
                    OnInsuranceChanging(value);
                    SendPropertyChanging();
                    _Insurance = value;
                    SendPropertyChanged("Insurance");
                    OnInsuranceChanged();
                }
            }
        }

        [Column(Name = "policy", UpdateCheck = UpdateCheck.Never, Storage = "_Policy", DbType = "nvarchar(100)")]
        public string Policy
        {
            get => _Policy;

            set
            {
                if (_Policy != value)
                {
                    OnPolicyChanging(value);
                    SendPropertyChanging();
                    _Policy = value;
                    SendPropertyChanged("Policy");
                    OnPolicyChanged();
                }
            }
        }

        [Column(Name = "passportnumber", UpdateCheck = UpdateCheck.Never, Storage = "_PassportNumber", DbType = "nvarchar(100)")]
        public string PassportNumber
        {
            get => _PassportNumber;

            set
            {
                if (_PassportNumber != value)
                {
                    OnPassportNumberChanging(value);
                    SendPropertyChanging();
                    _PassportNumber = value;
                    SendPropertyChanged("PassportNumber");
                    OnPassportNumberChanged();
                }
            }
        }

        [Column(Name = "passportexpires", UpdateCheck = UpdateCheck.Never, Storage = "_PassportExpires", DbType = "nvarchar(100)")]
        public string PassportExpires
        {
            get => _PassportExpires;

            set
            {
                if (_PassportExpires != value)
                {
                    OnPassportExpiresChanging(value);
                    SendPropertyChanging();
                    _PassportExpires = value;
                    SendPropertyChanged("PassportExpires");
                    OnPassportExpiresChanged();
                }
            }
        }

        [Column(Name = "Comments", UpdateCheck = UpdateCheck.Never, Storage = "_Comments", DbType = "nvarchar")]
        public string Comments
        {
            get => _Comments;

            set
            {
                if (_Comments != value)
                {
                    OnCommentsChanging(value);
                    SendPropertyChanging();
                    _Comments = value;
                    SendPropertyChanged("Comments");
                    OnCommentsChanged();
                }
            }
        }

        [Column(Name = "Tylenol", UpdateCheck = UpdateCheck.Never, Storage = "_Tylenol", DbType = "bit")]
        public bool? Tylenol
        {
            get => _Tylenol;

            set
            {
                if (_Tylenol != value)
                {
                    OnTylenolChanging(value);
                    SendPropertyChanging();
                    _Tylenol = value;
                    SendPropertyChanged("Tylenol");
                    OnTylenolChanged();
                }
            }
        }

        [Column(Name = "Advil", UpdateCheck = UpdateCheck.Never, Storage = "_Advil", DbType = "bit")]
        public bool? Advil
        {
            get => _Advil;

            set
            {
                if (_Advil != value)
                {
                    OnAdvilChanging(value);
                    SendPropertyChanging();
                    _Advil = value;
                    SendPropertyChanged("Advil");
                    OnAdvilChanged();
                }
            }
        }

        [Column(Name = "Maalox", UpdateCheck = UpdateCheck.Never, Storage = "_Maalox", DbType = "bit")]
        public bool? Maalox
        {
            get => _Maalox;

            set
            {
                if (_Maalox != value)
                {
                    OnMaaloxChanging(value);
                    SendPropertyChanging();
                    _Maalox = value;
                    SendPropertyChanged("Maalox");
                    OnMaaloxChanged();
                }
            }
        }

        [Column(Name = "Robitussin", UpdateCheck = UpdateCheck.Never, Storage = "_Robitussin", DbType = "bit")]
        public bool? Robitussin
        {
            get => _Robitussin;

            set
            {
                if (_Robitussin != value)
                {
                    OnRobitussinChanging(value);
                    SendPropertyChanging();
                    _Robitussin = value;
                    SendPropertyChanged("Robitussin");
                    OnRobitussinChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_RecReg_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
        public Person Person
        {
            get => _Person.Entity;

            set
            {
                Person previousValue = _Person.Entity;
                if (((previousValue != value)
                            || (_Person.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Person.Entity = null;
                        previousValue.RecRegs.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.RecRegs.Add(this);

                        _PeopleId = value.PeopleId;

                    }

                    else
                    {
                        _PeopleId = default(int?);

                    }

                    SendPropertyChanged("Person");
                }
            }
        }

        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            if ((PropertyChanging != null))
            {
                PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
