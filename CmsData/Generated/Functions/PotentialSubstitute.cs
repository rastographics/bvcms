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
	[Table(Name="PotentialSubstitutes")]
	public partial class PotentialSubstitute
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Name2;
		
		private string _EmailAddress;
		
		private string _SameSchedule;
		
		private string _Committed;
		
		private string _Groups;
		
		
		public PotentialSubstitute()
		{
		}

		
		
		[Column(Name="PeopleId", Storage="_PeopleId", DbType="int NOT NULL")]
		public int PeopleId
		{
			get
			{
				return this._PeopleId;
			}

			set
			{
				if (this._PeopleId != value)
					this._PeopleId = value;
			}

		}

		
		[Column(Name="Name2", Storage="_Name2", DbType="nvarchar(139)")]
		public string Name2
		{
			get
			{
				return this._Name2;
			}

			set
			{
				if (this._Name2 != value)
					this._Name2 = value;
			}

		}

		
		[Column(Name="EmailAddress", Storage="_EmailAddress", DbType="nvarchar(150)")]
		public string EmailAddress
		{
			get
			{
				return this._EmailAddress;
			}

			set
			{
				if (this._EmailAddress != value)
					this._EmailAddress = value;
			}

		}

		
		[Column(Name="SameSchedule", Storage="_SameSchedule", DbType="varchar(13)")]
		public string SameSchedule
		{
			get
			{
				return this._SameSchedule;
			}

			set
			{
				if (this._SameSchedule != value)
					this._SameSchedule = value;
			}

		}

		
		[Column(Name="Committed", Storage="_Committed", DbType="varchar(9)")]
		public string Committed
		{
			get
			{
				return this._Committed;
			}

			set
			{
				if (this._Committed != value)
					this._Committed = value;
			}

		}

		
		[Column(Name="Groups", Storage="_Groups", DbType="nvarchar")]
		public string Groups
		{
			get
			{
				return this._Groups;
			}

			set
			{
				if (this._Groups != value)
					this._Groups = value;
			}

		}

		
    }

}
