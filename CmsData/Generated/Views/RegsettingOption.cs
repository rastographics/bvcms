using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RegsettingOptions")]
    public partial class RegsettingOption
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private string _OrganizationName;

        private bool? _AskAllergies;

        private bool? _AnswersNotRequired;

        private bool? _AskChurch;

        private bool? _AskCoaching;

        private bool? _AskDoctor;

        private bool? _AskEmContact;

        private bool? _AskInsurance;

        private bool? _AskPassport;

        private bool? _AskParents;

        private bool? _AskSMS;

        private bool? _AskTylenolEtc;

        private bool? _AskSuggestedFee;

        private string _AskRequest;

        private string _AskTickets;

        private bool? _NoReqBirthYear;

        private bool? _NotReqDOB;

        private bool? _NotReqAddr;

        private bool? _NotReqZip;

        private bool? _NotReqPhone;

        private bool? _NotReqGender;

        private bool? _NotReqMarital;

        private bool? _ShowDOBOnFind;

        private bool? _ShowPhoneOnFind;

        private string _ConfirmationTrackingCode;

        private string _ValidateOrgs;

        private string _ShellBs;

        private string _FinishRegistrationButton;

        private string _SpecialScript;

        private string _GroupToJoin;

        private int? _TimeOut;

        private bool? _AllowOnlyOne;

        private bool? _TargetExtraValues;

        private bool? _AllowReRegister;

        private bool? _AllowSaveProgress;

        private bool? _MemberOnly;

        private bool? _AddAsProspect;

        private bool? _DisallowAnonymous;

        private decimal? _Fee;

        private decimal? _MaxFee;

        private decimal? _ExtraFee;

        private decimal? _Deposit;

        private string _AccountingCode;

        private string _ExtraValueFeeName;

        private bool? _ApplyMaxToOtheFees;

        private bool? _IncludeOtherFeesWithDeposit;

        private bool? _OtherFeesAddedToOrgFee;

        private bool? _AskDonation;

        private string _DonationLabel;

        private int? _DonationFundId;

        public RegsettingOption()
        {
        }

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsDbGenerated = true)]
        public int OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    _OrganizationId = value;
                }
            }
        }

        [Column(Name = "OrganizationName", Storage = "_OrganizationName", DbType = "nvarchar(100) NOT NULL")]
        public string OrganizationName
        {
            get => _OrganizationName;

            set
            {
                if (_OrganizationName != value)
                {
                    _OrganizationName = value;
                }
            }
        }

        [Column(Name = "AskAllergies", Storage = "_AskAllergies", DbType = "bit")]
        public bool? AskAllergies
        {
            get => _AskAllergies;

            set
            {
                if (_AskAllergies != value)
                {
                    _AskAllergies = value;
                }
            }
        }

        [Column(Name = "AnswersNotRequired", Storage = "_AnswersNotRequired", DbType = "bit")]
        public bool? AnswersNotRequired
        {
            get => _AnswersNotRequired;

            set
            {
                if (_AnswersNotRequired != value)
                {
                    _AnswersNotRequired = value;
                }
            }
        }

        [Column(Name = "AskChurch", Storage = "_AskChurch", DbType = "bit")]
        public bool? AskChurch
        {
            get => _AskChurch;

            set
            {
                if (_AskChurch != value)
                {
                    _AskChurch = value;
                }
            }
        }

        [Column(Name = "AskCoaching", Storage = "_AskCoaching", DbType = "bit")]
        public bool? AskCoaching
        {
            get => _AskCoaching;

            set
            {
                if (_AskCoaching != value)
                {
                    _AskCoaching = value;
                }
            }
        }

        [Column(Name = "AskDoctor", Storage = "_AskDoctor", DbType = "bit")]
        public bool? AskDoctor
        {
            get => _AskDoctor;

            set
            {
                if (_AskDoctor != value)
                {
                    _AskDoctor = value;
                }
            }
        }

        [Column(Name = "AskEmContact", Storage = "_AskEmContact", DbType = "bit")]
        public bool? AskEmContact
        {
            get => _AskEmContact;

            set
            {
                if (_AskEmContact != value)
                {
                    _AskEmContact = value;
                }
            }
        }

        [Column(Name = "AskInsurance", Storage = "_AskInsurance", DbType = "bit")]
        public bool? AskInsurance
        {
            get => _AskInsurance;

            set
            {
                if (_AskInsurance != value)
                {
                    _AskInsurance = value;
                }
            }
        }

        [Column(Name = "AskPassport", Storage = "_AskPassport", DbType = "bit")]
        public bool? AskPassport
        {
            get => _AskPassport;

            set
            {
                if (_AskPassport != value)
                {
                    _AskPassport = value;
                }
            }
        }

        [Column(Name = "AskParents", Storage = "_AskParents", DbType = "bit")]
        public bool? AskParents
        {
            get => _AskParents;

            set
            {
                if (_AskParents != value)
                {
                    _AskParents = value;
                }
            }
        }

        [Column(Name = "AskSMS", Storage = "_AskSMS", DbType = "bit")]
        public bool? AskSMS
        {
            get => _AskSMS;

            set
            {
                if (_AskSMS != value)
                {
                    _AskSMS = value;
                }
            }
        }

        [Column(Name = "AskTylenolEtc", Storage = "_AskTylenolEtc", DbType = "bit")]
        public bool? AskTylenolEtc
        {
            get => _AskTylenolEtc;

            set
            {
                if (_AskTylenolEtc != value)
                {
                    _AskTylenolEtc = value;
                }
            }
        }

        [Column(Name = "AskSuggestedFee", Storage = "_AskSuggestedFee", DbType = "bit")]
        public bool? AskSuggestedFee
        {
            get => _AskSuggestedFee;

            set
            {
                if (_AskSuggestedFee != value)
                {
                    _AskSuggestedFee = value;
                }
            }
        }

        [Column(Name = "AskRequest", Storage = "_AskRequest", DbType = "varchar(80)")]
        public string AskRequest
        {
            get => _AskRequest;

            set
            {
                if (_AskRequest != value)
                {
                    _AskRequest = value;
                }
            }
        }

        [Column(Name = "AskTickets", Storage = "_AskTickets", DbType = "varchar(100)")]
        public string AskTickets
        {
            get => _AskTickets;

            set
            {
                if (_AskTickets != value)
                {
                    _AskTickets = value;
                }
            }
        }

        [Column(Name = "NoReqBirthYear", Storage = "_NoReqBirthYear", DbType = "bit")]
        public bool? NoReqBirthYear
        {
            get => _NoReqBirthYear;

            set
            {
                if (_NoReqBirthYear != value)
                {
                    _NoReqBirthYear = value;
                }
            }
        }

        [Column(Name = "NotReqDOB", Storage = "_NotReqDOB", DbType = "bit")]
        public bool? NotReqDOB
        {
            get => _NotReqDOB;

            set
            {
                if (_NotReqDOB != value)
                {
                    _NotReqDOB = value;
                }
            }
        }

        [Column(Name = "NotReqAddr", Storage = "_NotReqAddr", DbType = "bit")]
        public bool? NotReqAddr
        {
            get => _NotReqAddr;

            set
            {
                if (_NotReqAddr != value)
                {
                    _NotReqAddr = value;
                }
            }
        }

        [Column(Name = "NotReqZip", Storage = "_NotReqZip", DbType = "bit")]
        public bool? NotReqZip
        {
            get => _NotReqZip;

            set
            {
                if (_NotReqZip != value)
                {
                    _NotReqZip = value;
                }
            }
        }

        [Column(Name = "NotReqPhone", Storage = "_NotReqPhone", DbType = "bit")]
        public bool? NotReqPhone
        {
            get => _NotReqPhone;

            set
            {
                if (_NotReqPhone != value)
                {
                    _NotReqPhone = value;
                }
            }
        }

        [Column(Name = "NotReqGender", Storage = "_NotReqGender", DbType = "bit")]
        public bool? NotReqGender
        {
            get => _NotReqGender;

            set
            {
                if (_NotReqGender != value)
                {
                    _NotReqGender = value;
                }
            }
        }

        [Column(Name = "NotReqMarital", Storage = "_NotReqMarital", DbType = "bit")]
        public bool? NotReqMarital
        {
            get => _NotReqMarital;

            set
            {
                if (_NotReqMarital != value)
                {
                    _NotReqMarital = value;
                }
            }
        }

        [Column(Name = "ShowDOBOnFind", Storage = "_ShowDOBOnFind", DbType = "bit")]
        public bool? ShowDOBOnFind
        {
            get => _ShowDOBOnFind;

            set
            {
                if (_ShowDOBOnFind != value)
                {
                    _ShowDOBOnFind = value;
                }
            }
        }

        [Column(Name = "ShowPhoneOnFind", Storage = "_ShowPhoneOnFind", DbType = "bit")]
        public bool? ShowPhoneOnFind
        {
            get => _ShowPhoneOnFind;

            set
            {
                if (_ShowPhoneOnFind != value)
                {
                    _ShowPhoneOnFind = value;
                }
            }
        }

        [Column(Name = "ConfirmationTrackingCode", Storage = "_ConfirmationTrackingCode", DbType = "varchar")]
        public string ConfirmationTrackingCode
        {
            get => _ConfirmationTrackingCode;

            set
            {
                if (_ConfirmationTrackingCode != value)
                {
                    _ConfirmationTrackingCode = value;
                }
            }
        }

        [Column(Name = "ValidateOrgs", Storage = "_ValidateOrgs", DbType = "varchar(200)")]
        public string ValidateOrgs
        {
            get => _ValidateOrgs;

            set
            {
                if (_ValidateOrgs != value)
                {
                    _ValidateOrgs = value;
                }
            }
        }

        [Column(Name = "ShellBs", Storage = "_ShellBs", DbType = "varchar(50)")]
        public string ShellBs
        {
            get => _ShellBs;

            set
            {
                if (_ShellBs != value)
                {
                    _ShellBs = value;
                }
            }
        }

        [Column(Name = "FinishRegistrationButton", Storage = "_FinishRegistrationButton", DbType = "varchar(80)")]
        public string FinishRegistrationButton
        {
            get => _FinishRegistrationButton;

            set
            {
                if (_FinishRegistrationButton != value)
                {
                    _FinishRegistrationButton = value;
                }
            }
        }

        [Column(Name = "SpecialScript", Storage = "_SpecialScript", DbType = "varchar(50)")]
        public string SpecialScript
        {
            get => _SpecialScript;

            set
            {
                if (_SpecialScript != value)
                {
                    _SpecialScript = value;
                }
            }
        }

        [Column(Name = "GroupToJoin", Storage = "_GroupToJoin", DbType = "varchar(50)")]
        public string GroupToJoin
        {
            get => _GroupToJoin;

            set
            {
                if (_GroupToJoin != value)
                {
                    _GroupToJoin = value;
                }
            }
        }

        [Column(Name = "TimeOut", Storage = "_TimeOut", DbType = "int")]
        public int? TimeOut
        {
            get => _TimeOut;

            set
            {
                if (_TimeOut != value)
                {
                    _TimeOut = value;
                }
            }
        }

        [Column(Name = "AllowOnlyOne", Storage = "_AllowOnlyOne", DbType = "bit")]
        public bool? AllowOnlyOne
        {
            get => _AllowOnlyOne;

            set
            {
                if (_AllowOnlyOne != value)
                {
                    _AllowOnlyOne = value;
                }
            }
        }

        [Column(Name = "TargetExtraValues", Storage = "_TargetExtraValues", DbType = "bit")]
        public bool? TargetExtraValues
        {
            get => _TargetExtraValues;

            set
            {
                if (_TargetExtraValues != value)
                {
                    _TargetExtraValues = value;
                }
            }
        }

        [Column(Name = "AllowReRegister", Storage = "_AllowReRegister", DbType = "bit")]
        public bool? AllowReRegister
        {
            get => _AllowReRegister;

            set
            {
                if (_AllowReRegister != value)
                {
                    _AllowReRegister = value;
                }
            }
        }

        [Column(Name = "AllowSaveProgress", Storage = "_AllowSaveProgress", DbType = "bit")]
        public bool? AllowSaveProgress
        {
            get => _AllowSaveProgress;

            set
            {
                if (_AllowSaveProgress != value)
                {
                    _AllowSaveProgress = value;
                }
            }
        }

        [Column(Name = "MemberOnly", Storage = "_MemberOnly", DbType = "bit")]
        public bool? MemberOnly
        {
            get => _MemberOnly;

            set
            {
                if (_MemberOnly != value)
                {
                    _MemberOnly = value;
                }
            }
        }

        [Column(Name = "AddAsProspect", Storage = "_AddAsProspect", DbType = "bit")]
        public bool? AddAsProspect
        {
            get => _AddAsProspect;

            set
            {
                if (_AddAsProspect != value)
                {
                    _AddAsProspect = value;
                }
            }
        }

        [Column(Name = "DisallowAnonymous", Storage = "_DisallowAnonymous", DbType = "bit")]
        public bool? DisallowAnonymous
        {
            get => _DisallowAnonymous;

            set
            {
                if (_DisallowAnonymous != value)
                {
                    _DisallowAnonymous = value;
                }
            }
        }

        [Column(Name = "Fee", Storage = "_Fee", DbType = "money")]
        public decimal? Fee
        {
            get => _Fee;

            set
            {
                if (_Fee != value)
                {
                    _Fee = value;
                }
            }
        }

        [Column(Name = "MaxFee", Storage = "_MaxFee", DbType = "money")]
        public decimal? MaxFee
        {
            get => _MaxFee;

            set
            {
                if (_MaxFee != value)
                {
                    _MaxFee = value;
                }
            }
        }

        [Column(Name = "ExtraFee", Storage = "_ExtraFee", DbType = "money")]
        public decimal? ExtraFee
        {
            get => _ExtraFee;

            set
            {
                if (_ExtraFee != value)
                {
                    _ExtraFee = value;
                }
            }
        }

        [Column(Name = "Deposit", Storage = "_Deposit", DbType = "money")]
        public decimal? Deposit
        {
            get => _Deposit;

            set
            {
                if (_Deposit != value)
                {
                    _Deposit = value;
                }
            }
        }

        [Column(Name = "AccountingCode", Storage = "_AccountingCode", DbType = "varchar(50)")]
        public string AccountingCode
        {
            get => _AccountingCode;

            set
            {
                if (_AccountingCode != value)
                {
                    _AccountingCode = value;
                }
            }
        }

        [Column(Name = "ExtraValueFeeName", Storage = "_ExtraValueFeeName", DbType = "varchar(50)")]
        public string ExtraValueFeeName
        {
            get => _ExtraValueFeeName;

            set
            {
                if (_ExtraValueFeeName != value)
                {
                    _ExtraValueFeeName = value;
                }
            }
        }

        [Column(Name = "ApplyMaxToOtheFees", Storage = "_ApplyMaxToOtheFees", DbType = "bit")]
        public bool? ApplyMaxToOtheFees
        {
            get => _ApplyMaxToOtheFees;

            set
            {
                if (_ApplyMaxToOtheFees != value)
                {
                    _ApplyMaxToOtheFees = value;
                }
            }
        }

        [Column(Name = "IncludeOtherFeesWithDeposit", Storage = "_IncludeOtherFeesWithDeposit", DbType = "bit")]
        public bool? IncludeOtherFeesWithDeposit
        {
            get => _IncludeOtherFeesWithDeposit;

            set
            {
                if (_IncludeOtherFeesWithDeposit != value)
                {
                    _IncludeOtherFeesWithDeposit = value;
                }
            }
        }

        [Column(Name = "OtherFeesAddedToOrgFee", Storage = "_OtherFeesAddedToOrgFee", DbType = "bit")]
        public bool? OtherFeesAddedToOrgFee
        {
            get => _OtherFeesAddedToOrgFee;

            set
            {
                if (_OtherFeesAddedToOrgFee != value)
                {
                    _OtherFeesAddedToOrgFee = value;
                }
            }
        }

        [Column(Name = "AskDonation", Storage = "_AskDonation", DbType = "bit")]
        public bool? AskDonation
        {
            get => _AskDonation;

            set
            {
                if (_AskDonation != value)
                {
                    _AskDonation = value;
                }
            }
        }

        [Column(Name = "DonationLabel", Storage = "_DonationLabel", DbType = "varchar(50)")]
        public string DonationLabel
        {
            get => _DonationLabel;

            set
            {
                if (_DonationLabel != value)
                {
                    _DonationLabel = value;
                }
            }
        }

        [Column(Name = "DonationFundId", Storage = "_DonationFundId", DbType = "int")]
        public int? DonationFundId
        {
            get => _DonationFundId;

            set
            {
                if (_DonationFundId != value)
                {
                    _DonationFundId = value;
                }
            }
        }
    }
}
