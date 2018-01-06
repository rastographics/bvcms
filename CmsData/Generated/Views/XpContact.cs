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
	[Table(Name="XpContact")]
	public partial class XpContact
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _ContactId;
		
		private string _ContactType;
		
		private DateTime _ContactDate;
		
		private string _ContactReason;
		
		private string _Ministry;
		
		private bool? _NotAtHome;
		
		private bool? _LeftDoorHanger;
		
		private bool? _LeftMessage;
		
		private bool? _GospelShared;
		
		private bool? _PrayerRequest;
		
		private bool? _ContactMade;
		
		private bool? _GiftBagGiven;
		
		private string _Comments;
		
		private int? _OrganizationId;
		
		
		public XpContact()
		{
		}

		
		
		[Column(Name="ContactId", Storage="_ContactId", DbType="int NOT NULL")]
		public int ContactId
		{
			get
			{
				return this._ContactId;
			}

			set
			{
				if (this._ContactId != value)
					this._ContactId = value;
			}

		}

		
		[Column(Name="ContactType", Storage="_ContactType", DbType="nvarchar(100)")]
		public string ContactType
		{
			get
			{
				return this._ContactType;
			}

			set
			{
				if (this._ContactType != value)
					this._ContactType = value;
			}

		}

		
		[Column(Name="ContactDate", Storage="_ContactDate", DbType="datetime NOT NULL")]
		public DateTime ContactDate
		{
			get
			{
				return this._ContactDate;
			}

			set
			{
				if (this._ContactDate != value)
					this._ContactDate = value;
			}

		}

		
		[Column(Name="ContactReason", Storage="_ContactReason", DbType="nvarchar(100)")]
		public string ContactReason
		{
			get
			{
				return this._ContactReason;
			}

			set
			{
				if (this._ContactReason != value)
					this._ContactReason = value;
			}

		}

		
		[Column(Name="Ministry", Storage="_Ministry", DbType="nvarchar(50)")]
		public string Ministry
		{
			get
			{
				return this._Ministry;
			}

			set
			{
				if (this._Ministry != value)
					this._Ministry = value;
			}

		}

		
		[Column(Name="NotAtHome", Storage="_NotAtHome", DbType="bit")]
		public bool? NotAtHome
		{
			get
			{
				return this._NotAtHome;
			}

			set
			{
				if (this._NotAtHome != value)
					this._NotAtHome = value;
			}

		}

		
		[Column(Name="LeftDoorHanger", Storage="_LeftDoorHanger", DbType="bit")]
		public bool? LeftDoorHanger
		{
			get
			{
				return this._LeftDoorHanger;
			}

			set
			{
				if (this._LeftDoorHanger != value)
					this._LeftDoorHanger = value;
			}

		}

		
		[Column(Name="LeftMessage", Storage="_LeftMessage", DbType="bit")]
		public bool? LeftMessage
		{
			get
			{
				return this._LeftMessage;
			}

			set
			{
				if (this._LeftMessage != value)
					this._LeftMessage = value;
			}

		}

		
		[Column(Name="GospelShared", Storage="_GospelShared", DbType="bit")]
		public bool? GospelShared
		{
			get
			{
				return this._GospelShared;
			}

			set
			{
				if (this._GospelShared != value)
					this._GospelShared = value;
			}

		}

		
		[Column(Name="PrayerRequest", Storage="_PrayerRequest", DbType="bit")]
		public bool? PrayerRequest
		{
			get
			{
				return this._PrayerRequest;
			}

			set
			{
				if (this._PrayerRequest != value)
					this._PrayerRequest = value;
			}

		}

		
		[Column(Name="ContactMade", Storage="_ContactMade", DbType="bit")]
		public bool? ContactMade
		{
			get
			{
				return this._ContactMade;
			}

			set
			{
				if (this._ContactMade != value)
					this._ContactMade = value;
			}

		}

		
		[Column(Name="GiftBagGiven", Storage="_GiftBagGiven", DbType="bit")]
		public bool? GiftBagGiven
		{
			get
			{
				return this._GiftBagGiven;
			}

			set
			{
				if (this._GiftBagGiven != value)
					this._GiftBagGiven = value;
			}

		}

		
		[Column(Name="Comments", Storage="_Comments", DbType="nvarchar")]
		public string Comments
		{
			get
			{
				return this._Comments;
			}

			set
			{
				if (this._Comments != value)
					this._Comments = value;
			}

		}

		
		[Column(Name="OrganizationId", Storage="_OrganizationId", DbType="int")]
		public int? OrganizationId
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

		
    }

}
