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
	[Table(Name="FamilyMembers")]
	public partial class FamilyMember
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _Id;
		
		private int _PortraitId;
		
		private DateTime? _PicDate;
		
		private int? _PicXPos;
		
		private int? _PicYPos;
		
		private DateTime? _DeceasedDate;
		
		private string _Name;
		
		private int? _Age;
		
		private int _GenderId;
		
		private string _Color;
		
		private int _PositionInFamilyId;
		
		private string _PositionInFamily;
		
		private string _SpouseIndicator;
		
		private string _Email;
		
		private bool? _IsDeceased;
		
		private string _MemberStatus;
		
		private string _Gender;
		
		
		public FamilyMember()
		{
		}

		
		
		[Column(Name="Id", Storage="_Id", DbType="int NOT NULL")]
		public int Id
		{
			get
			{
				return this._Id;
			}

			set
			{
				if (this._Id != value)
					this._Id = value;
			}

		}

		
		[Column(Name="PortraitId", Storage="_PortraitId", DbType="int NOT NULL")]
		public int PortraitId
		{
			get
			{
				return this._PortraitId;
			}

			set
			{
				if (this._PortraitId != value)
					this._PortraitId = value;
			}

		}

		
		[Column(Name="PicDate", Storage="_PicDate", DbType="datetime")]
		public DateTime? PicDate
		{
			get
			{
				return this._PicDate;
			}

			set
			{
				if (this._PicDate != value)
					this._PicDate = value;
			}

		}

		
		[Column(Name="PicXPos", Storage="_PicXPos", DbType="int")]
		public int? PicXPos
		{
			get
			{
				return this._PicXPos;
			}

			set
			{
				if (this._PicXPos != value)
					this._PicXPos = value;
			}

		}

		
		[Column(Name="PicYPos", Storage="_PicYPos", DbType="int")]
		public int? PicYPos
		{
			get
			{
				return this._PicYPos;
			}

			set
			{
				if (this._PicYPos != value)
					this._PicYPos = value;
			}

		}

		
		[Column(Name="DeceasedDate", Storage="_DeceasedDate", DbType="datetime")]
		public DateTime? DeceasedDate
		{
			get
			{
				return this._DeceasedDate;
			}

			set
			{
				if (this._DeceasedDate != value)
					this._DeceasedDate = value;
			}

		}

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(138)")]
		public string Name
		{
			get
			{
				return this._Name;
			}

			set
			{
				if (this._Name != value)
					this._Name = value;
			}

		}

		
		[Column(Name="Age", Storage="_Age", DbType="int")]
		public int? Age
		{
			get
			{
				return this._Age;
			}

			set
			{
				if (this._Age != value)
					this._Age = value;
			}

		}

		
		[Column(Name="GenderId", Storage="_GenderId", DbType="int NOT NULL")]
		public int GenderId
		{
			get
			{
				return this._GenderId;
			}

			set
			{
				if (this._GenderId != value)
					this._GenderId = value;
			}

		}

		
		[Column(Name="Color", Storage="_Color", DbType="varchar(1) NOT NULL")]
		public string Color
		{
			get
			{
				return this._Color;
			}

			set
			{
				if (this._Color != value)
					this._Color = value;
			}

		}

		
		[Column(Name="PositionInFamilyId", Storage="_PositionInFamilyId", DbType="int NOT NULL")]
		public int PositionInFamilyId
		{
			get
			{
				return this._PositionInFamilyId;
			}

			set
			{
				if (this._PositionInFamilyId != value)
					this._PositionInFamilyId = value;
			}

		}

		
		[Column(Name="PositionInFamily", Storage="_PositionInFamily", DbType="nvarchar(100)")]
		public string PositionInFamily
		{
			get
			{
				return this._PositionInFamily;
			}

			set
			{
				if (this._PositionInFamily != value)
					this._PositionInFamily = value;
			}

		}

		
		[Column(Name="SpouseIndicator", Storage="_SpouseIndicator", DbType="varchar(1) NOT NULL")]
		public string SpouseIndicator
		{
			get
			{
				return this._SpouseIndicator;
			}

			set
			{
				if (this._SpouseIndicator != value)
					this._SpouseIndicator = value;
			}

		}

		
		[Column(Name="Email", Storage="_Email", DbType="nvarchar(150)")]
		public string Email
		{
			get
			{
				return this._Email;
			}

			set
			{
				if (this._Email != value)
					this._Email = value;
			}

		}

		
		[Column(Name="isDeceased", Storage="_IsDeceased", DbType="bit")]
		public bool? IsDeceased
		{
			get
			{
				return this._IsDeceased;
			}

			set
			{
				if (this._IsDeceased != value)
					this._IsDeceased = value;
			}

		}

		
		[Column(Name="MemberStatus", Storage="_MemberStatus", DbType="nvarchar(50)")]
		public string MemberStatus
		{
			get
			{
				return this._MemberStatus;
			}

			set
			{
				if (this._MemberStatus != value)
					this._MemberStatus = value;
			}

		}

		
		[Column(Name="Gender", Storage="_Gender", DbType="nvarchar(20)")]
		public string Gender
		{
			get
			{
				return this._Gender;
			}

			set
			{
				if (this._Gender != value)
					this._Gender = value;
			}

		}

		
    }

}
