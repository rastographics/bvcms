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
	[Table(Name="XpFamilyExtra")]
	public partial class XpFamilyExtra
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _FamilyId;
		
		private string _Field;
		
		private string _CodeValue;
		
		private DateTime? _DateValue;
		
		private string _TextValue;
		
		private int? _IntValue;
		
		private bool? _BitValue;
		
		private string _Type;
		
		
		public XpFamilyExtra()
		{
		}

		
		
		[Column(Name="FamilyId", Storage="_FamilyId", DbType="int NOT NULL")]
		public int FamilyId
		{
			get
			{
				return this._FamilyId;
			}

			set
			{
				if (this._FamilyId != value)
					this._FamilyId = value;
			}

		}

		
		[Column(Name="Field", Storage="_Field", DbType="nvarchar(50) NOT NULL")]
		public string Field
		{
			get
			{
				return this._Field;
			}

			set
			{
				if (this._Field != value)
					this._Field = value;
			}

		}

		
		[Column(Name="CodeValue", Storage="_CodeValue", DbType="nvarchar(200)")]
		public string CodeValue
		{
			get
			{
				return this._CodeValue;
			}

			set
			{
				if (this._CodeValue != value)
					this._CodeValue = value;
			}

		}

		
		[Column(Name="DateValue", Storage="_DateValue", DbType="datetime")]
		public DateTime? DateValue
		{
			get
			{
				return this._DateValue;
			}

			set
			{
				if (this._DateValue != value)
					this._DateValue = value;
			}

		}

		
		[Column(Name="TextValue", Storage="_TextValue", DbType="nvarchar")]
		public string TextValue
		{
			get
			{
				return this._TextValue;
			}

			set
			{
				if (this._TextValue != value)
					this._TextValue = value;
			}

		}

		
		[Column(Name="IntValue", Storage="_IntValue", DbType="int")]
		public int? IntValue
		{
			get
			{
				return this._IntValue;
			}

			set
			{
				if (this._IntValue != value)
					this._IntValue = value;
			}

		}

		
		[Column(Name="BitValue", Storage="_BitValue", DbType="bit")]
		public bool? BitValue
		{
			get
			{
				return this._BitValue;
			}

			set
			{
				if (this._BitValue != value)
					this._BitValue = value;
			}

		}

		
		[Column(Name="Type", Storage="_Type", DbType="varchar(22) NOT NULL")]
		public string Type
		{
			get
			{
				return this._Type;
			}

			set
			{
				if (this._Type != value)
					this._Type = value;
			}

		}

		
    }

}
