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
	[Table(Name="ContactSummary")]
	public partial class ContactSummary
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _Count;
		
		private string _ContactType;
		
		private string _ReasonType;
		
		private string _Ministry;
		
		private string _Comments;
		
		private string _ContactDate;
		
		private string _Contactor;
		
		
		public ContactSummary()
		{
		}

		
		
		[Column(Name="Count", Storage="_Count", DbType="int")]
		public int? Count
		{
			get
			{
				return this._Count;
			}

			set
			{
				if (this._Count != value)
					this._Count = value;
			}

		}

		
		[Column(Name="ContactType", Storage="_ContactType", DbType="varchar(100)")]
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

		
		[Column(Name="ReasonType", Storage="_ReasonType", DbType="varchar(100)")]
		public string ReasonType
		{
			get
			{
				return this._ReasonType;
			}

			set
			{
				if (this._ReasonType != value)
					this._ReasonType = value;
			}

		}

		
		[Column(Name="Ministry", Storage="_Ministry", DbType="varchar(50)")]
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

		
		[Column(Name="Comments", Storage="_Comments", DbType="varchar(11) NOT NULL")]
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

		
		[Column(Name="ContactDate", Storage="_ContactDate", DbType="varchar(7) NOT NULL")]
		public string ContactDate
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

		
		[Column(Name="Contactor", Storage="_Contactor", DbType="varchar(12) NOT NULL")]
		public string Contactor
		{
			get
			{
				return this._Contactor;
			}

			set
			{
				if (this._Contactor != value)
					this._Contactor = value;
			}

		}

		
    }

}
