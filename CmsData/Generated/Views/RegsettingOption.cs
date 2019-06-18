using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData.View
{
	[Table(Name="RegsettingOptions")]
	public partial class RegsettingOption
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _OrganizationId;
		
		private string _OrganizationName;
		
		private bool? _AskAllergies;
		
		private bool? _AnswersNotRequired;
		
		private bool? _AskChurch;
		
		private bool? _AskCoaching;
		
		private bool? _AskDoctor;
		
		private bool? _AskEmContact;
		
		private bool? _AskInsurance;
		
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

		
		
		[Column(Name="OrganizationId", Storage="_OrganizationId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsDbGenerated=true)]
		public int OrganizationId
		{
			get
			{
				return this._OrganizationId;
			}

			set
			{
				if (this._OrganizationId != value)
					this._OrganizationId = value;
			}

		}

		
		[Column(Name="OrganizationName", Storage="_OrganizationName", DbType="nvarchar(100) NOT NULL")]
		public string OrganizationName
		{
			get
			{
				return this._OrganizationName;
			}

			set
			{
				if (this._OrganizationName != value)
					this._OrganizationName = value;
			}

		}

		
		[Column(Name="AskAllergies", Storage="_AskAllergies", DbType="bit")]
		public bool? AskAllergies
		{
			get
			{
				return this._AskAllergies;
			}

			set
			{
				if (this._AskAllergies != value)
					this._AskAllergies = value;
			}

		}

		
		[Column(Name="AnswersNotRequired", Storage="_AnswersNotRequired", DbType="bit")]
		public bool? AnswersNotRequired
		{
			get
			{
				return this._AnswersNotRequired;
			}

			set
			{
				if (this._AnswersNotRequired != value)
					this._AnswersNotRequired = value;
			}

		}

		
		[Column(Name="AskChurch", Storage="_AskChurch", DbType="bit")]
		public bool? AskChurch
		{
			get
			{
				return this._AskChurch;
			}

			set
			{
				if (this._AskChurch != value)
					this._AskChurch = value;
			}

		}

		
		[Column(Name="AskCoaching", Storage="_AskCoaching", DbType="bit")]
		public bool? AskCoaching
		{
			get
			{
				return this._AskCoaching;
			}

			set
			{
				if (this._AskCoaching != value)
					this._AskCoaching = value;
			}

		}

		
		[Column(Name="AskDoctor", Storage="_AskDoctor", DbType="bit")]
		public bool? AskDoctor
		{
			get
			{
				return this._AskDoctor;
			}

			set
			{
				if (this._AskDoctor != value)
					this._AskDoctor = value;
			}

		}

		
		[Column(Name="AskEmContact", Storage="_AskEmContact", DbType="bit")]
		public bool? AskEmContact
		{
			get
			{
				return this._AskEmContact;
			}

			set
			{
				if (this._AskEmContact != value)
					this._AskEmContact = value;
			}

		}

		
		[Column(Name="AskInsurance", Storage="_AskInsurance", DbType="bit")]
		public bool? AskInsurance
		{
			get
			{
				return this._AskInsurance;
			}

			set
			{
				if (this._AskInsurance != value)
					this._AskInsurance = value;
			}

		}

		
		[Column(Name="AskParents", Storage="_AskParents", DbType="bit")]
		public bool? AskParents
		{
			get
			{
				return this._AskParents;
			}

			set
			{
				if (this._AskParents != value)
					this._AskParents = value;
			}

		}

		
		[Column(Name="AskSMS", Storage="_AskSMS", DbType="bit")]
		public bool? AskSMS
		{
			get
			{
				return this._AskSMS;
			}

			set
			{
				if (this._AskSMS != value)
					this._AskSMS = value;
			}

		}

		
		[Column(Name="AskTylenolEtc", Storage="_AskTylenolEtc", DbType="bit")]
		public bool? AskTylenolEtc
		{
			get
			{
				return this._AskTylenolEtc;
			}

			set
			{
				if (this._AskTylenolEtc != value)
					this._AskTylenolEtc = value;
			}

		}

		
		[Column(Name="AskSuggestedFee", Storage="_AskSuggestedFee", DbType="bit")]
		public bool? AskSuggestedFee
		{
			get
			{
				return this._AskSuggestedFee;
			}

			set
			{
				if (this._AskSuggestedFee != value)
					this._AskSuggestedFee = value;
			}

		}

		
		[Column(Name="AskRequest", Storage="_AskRequest", DbType="varchar(80)")]
		public string AskRequest
		{
			get
			{
				return this._AskRequest;
			}

			set
			{
				if (this._AskRequest != value)
					this._AskRequest = value;
			}

		}

		
		[Column(Name="AskTickets", Storage="_AskTickets", DbType="varchar(100)")]
		public string AskTickets
		{
			get
			{
				return this._AskTickets;
			}

			set
			{
				if (this._AskTickets != value)
					this._AskTickets = value;
			}

		}

		
		[Column(Name="NoReqBirthYear", Storage="_NoReqBirthYear", DbType="bit")]
		public bool? NoReqBirthYear
		{
			get
			{
				return this._NoReqBirthYear;
			}

			set
			{
				if (this._NoReqBirthYear != value)
					this._NoReqBirthYear = value;
			}

		}

		
		[Column(Name="NotReqDOB", Storage="_NotReqDOB", DbType="bit")]
		public bool? NotReqDOB
		{
			get
			{
				return this._NotReqDOB;
			}

			set
			{
				if (this._NotReqDOB != value)
					this._NotReqDOB = value;
			}

		}

		
		[Column(Name="NotReqAddr", Storage="_NotReqAddr", DbType="bit")]
		public bool? NotReqAddr
		{
			get
			{
				return this._NotReqAddr;
			}

			set
			{
				if (this._NotReqAddr != value)
					this._NotReqAddr = value;
			}

		}

		
		[Column(Name="NotReqZip", Storage="_NotReqZip", DbType="bit")]
		public bool? NotReqZip
		{
			get
			{
				return this._NotReqZip;
			}

			set
			{
				if (this._NotReqZip != value)
					this._NotReqZip = value;
			}

		}

		
		[Column(Name="NotReqPhone", Storage="_NotReqPhone", DbType="bit")]
		public bool? NotReqPhone
		{
			get
			{
				return this._NotReqPhone;
			}

			set
			{
				if (this._NotReqPhone != value)
					this._NotReqPhone = value;
			}

		}

		
		[Column(Name="NotReqGender", Storage="_NotReqGender", DbType="bit")]
		public bool? NotReqGender
		{
			get
			{
				return this._NotReqGender;
			}

			set
			{
				if (this._NotReqGender != value)
					this._NotReqGender = value;
			}

		}

		
		[Column(Name="NotReqMarital", Storage="_NotReqMarital", DbType="bit")]
		public bool? NotReqMarital
		{
			get
			{
				return this._NotReqMarital;
			}

			set
			{
				if (this._NotReqMarital != value)
					this._NotReqMarital = value;
			}

		}

        [Column(Name = "ShowDOBOnFind", Storage = "_ShowDOBOnFind", DbType = "bit")]
        public bool? ShowDOBOnFind
        {
            get
            {
                return this._ShowDOBOnFind;
            }

            set
            {
                if (this._ShowDOBOnFind != value)
                    this._ShowDOBOnFind = value;
            }
        }

        [Column(Name = "ShowPhoneOnFind", Storage = "_ShowPhoneOnFind", DbType = "bit")]
        public bool? ShowPhoneOnFind
        {
            get
            {
                return this._ShowPhoneOnFind;
            }

            set
            {
                if (this._ShowPhoneOnFind != value)
                    this._ShowPhoneOnFind = value;
            }
        }


        [Column(Name="ConfirmationTrackingCode", Storage="_ConfirmationTrackingCode", DbType="varchar")]
		public string ConfirmationTrackingCode
		{
			get
			{
				return this._ConfirmationTrackingCode;
			}

			set
			{
				if (this._ConfirmationTrackingCode != value)
					this._ConfirmationTrackingCode = value;
			}

		}

		
		[Column(Name="ValidateOrgs", Storage="_ValidateOrgs", DbType="varchar(200)")]
		public string ValidateOrgs
		{
			get
			{
				return this._ValidateOrgs;
			}

			set
			{
				if (this._ValidateOrgs != value)
					this._ValidateOrgs = value;
			}

		}

		
		[Column(Name="ShellBs", Storage="_ShellBs", DbType="varchar(50)")]
		public string ShellBs
		{
			get
			{
				return this._ShellBs;
			}

			set
			{
				if (this._ShellBs != value)
					this._ShellBs = value;
			}

		}

		
		[Column(Name="FinishRegistrationButton", Storage="_FinishRegistrationButton", DbType="varchar(80)")]
		public string FinishRegistrationButton
		{
			get
			{
				return this._FinishRegistrationButton;
			}

			set
			{
				if (this._FinishRegistrationButton != value)
					this._FinishRegistrationButton = value;
			}

		}

		
		[Column(Name="SpecialScript", Storage="_SpecialScript", DbType="varchar(50)")]
		public string SpecialScript
		{
			get
			{
				return this._SpecialScript;
			}

			set
			{
				if (this._SpecialScript != value)
					this._SpecialScript = value;
			}

		}

		
		[Column(Name="GroupToJoin", Storage="_GroupToJoin", DbType="varchar(50)")]
		public string GroupToJoin
		{
			get
			{
				return this._GroupToJoin;
			}

			set
			{
				if (this._GroupToJoin != value)
					this._GroupToJoin = value;
			}

		}

		
		[Column(Name="TimeOut", Storage="_TimeOut", DbType="int")]
		public int? TimeOut
		{
			get
			{
				return this._TimeOut;
			}

			set
			{
				if (this._TimeOut != value)
					this._TimeOut = value;
			}

		}

		
		[Column(Name="AllowOnlyOne", Storage="_AllowOnlyOne", DbType="bit")]
		public bool? AllowOnlyOne
		{
			get
			{
				return this._AllowOnlyOne;
			}

			set
			{
				if (this._AllowOnlyOne != value)
					this._AllowOnlyOne = value;
			}

		}

		
		[Column(Name="TargetExtraValues", Storage="_TargetExtraValues", DbType="bit")]
		public bool? TargetExtraValues
		{
			get
			{
				return this._TargetExtraValues;
			}

			set
			{
				if (this._TargetExtraValues != value)
					this._TargetExtraValues = value;
			}

		}

		
		[Column(Name="AllowReRegister", Storage="_AllowReRegister", DbType="bit")]
		public bool? AllowReRegister
		{
			get
			{
				return this._AllowReRegister;
			}

			set
			{
				if (this._AllowReRegister != value)
					this._AllowReRegister = value;
			}

		}

		
		[Column(Name="AllowSaveProgress", Storage="_AllowSaveProgress", DbType="bit")]
		public bool? AllowSaveProgress
		{
			get
			{
				return this._AllowSaveProgress;
			}

			set
			{
				if (this._AllowSaveProgress != value)
					this._AllowSaveProgress = value;
			}

		}

		
		[Column(Name="MemberOnly", Storage="_MemberOnly", DbType="bit")]
		public bool? MemberOnly
		{
			get
			{
				return this._MemberOnly;
			}

			set
			{
				if (this._MemberOnly != value)
					this._MemberOnly = value;
			}

		}

		
		[Column(Name="AddAsProspect", Storage="_AddAsProspect", DbType="bit")]
		public bool? AddAsProspect
		{
			get
			{
				return this._AddAsProspect;
			}

			set
			{
				if (this._AddAsProspect != value)
					this._AddAsProspect = value;
			}

		}

		
		[Column(Name="DisallowAnonymous", Storage="_DisallowAnonymous", DbType="bit")]
		public bool? DisallowAnonymous
		{
			get
			{
				return this._DisallowAnonymous;
			}

			set
			{
				if (this._DisallowAnonymous != value)
					this._DisallowAnonymous = value;
			}

		}

		
		[Column(Name="Fee", Storage="_Fee", DbType="money")]
		public decimal? Fee
		{
			get
			{
				return this._Fee;
			}

			set
			{
				if (this._Fee != value)
					this._Fee = value;
			}

		}

		
		[Column(Name="MaxFee", Storage="_MaxFee", DbType="money")]
		public decimal? MaxFee
		{
			get
			{
				return this._MaxFee;
			}

			set
			{
				if (this._MaxFee != value)
					this._MaxFee = value;
			}

		}

		
		[Column(Name="ExtraFee", Storage="_ExtraFee", DbType="money")]
		public decimal? ExtraFee
		{
			get
			{
				return this._ExtraFee;
			}

			set
			{
				if (this._ExtraFee != value)
					this._ExtraFee = value;
			}

		}

		
		[Column(Name="Deposit", Storage="_Deposit", DbType="money")]
		public decimal? Deposit
		{
			get
			{
				return this._Deposit;
			}

			set
			{
				if (this._Deposit != value)
					this._Deposit = value;
			}

		}

		
		[Column(Name="AccountingCode", Storage="_AccountingCode", DbType="varchar(50)")]
		public string AccountingCode
		{
			get
			{
				return this._AccountingCode;
			}

			set
			{
				if (this._AccountingCode != value)
					this._AccountingCode = value;
			}

		}

		
		[Column(Name="ExtraValueFeeName", Storage="_ExtraValueFeeName", DbType="varchar(50)")]
		public string ExtraValueFeeName
		{
			get
			{
				return this._ExtraValueFeeName;
			}

			set
			{
				if (this._ExtraValueFeeName != value)
					this._ExtraValueFeeName = value;
			}

		}

		
		[Column(Name="ApplyMaxToOtheFees", Storage="_ApplyMaxToOtheFees", DbType="bit")]
		public bool? ApplyMaxToOtheFees
		{
			get
			{
				return this._ApplyMaxToOtheFees;
			}

			set
			{
				if (this._ApplyMaxToOtheFees != value)
					this._ApplyMaxToOtheFees = value;
			}

		}

		
		[Column(Name="IncludeOtherFeesWithDeposit", Storage="_IncludeOtherFeesWithDeposit", DbType="bit")]
		public bool? IncludeOtherFeesWithDeposit
		{
			get
			{
				return this._IncludeOtherFeesWithDeposit;
			}

			set
			{
				if (this._IncludeOtherFeesWithDeposit != value)
					this._IncludeOtherFeesWithDeposit = value;
			}

		}

		
		[Column(Name="OtherFeesAddedToOrgFee", Storage="_OtherFeesAddedToOrgFee", DbType="bit")]
		public bool? OtherFeesAddedToOrgFee
		{
			get
			{
				return this._OtherFeesAddedToOrgFee;
			}

			set
			{
				if (this._OtherFeesAddedToOrgFee != value)
					this._OtherFeesAddedToOrgFee = value;
			}

		}

		
		[Column(Name="AskDonation", Storage="_AskDonation", DbType="bit")]
		public bool? AskDonation
		{
			get
			{
				return this._AskDonation;
			}

			set
			{
				if (this._AskDonation != value)
					this._AskDonation = value;
			}

		}

		
		[Column(Name="DonationLabel", Storage="_DonationLabel", DbType="varchar(50)")]
		public string DonationLabel
		{
			get
			{
				return this._DonationLabel;
			}

			set
			{
				if (this._DonationLabel != value)
					this._DonationLabel = value;
			}

		}

		
		[Column(Name="DonationFundId", Storage="_DonationFundId", DbType="int")]
		public int? DonationFundId
		{
			get
			{
				return this._DonationFundId;
			}

			set
			{
				if (this._DonationFundId != value)
					this._DonationFundId = value;
			}

		}

		
    }

}
