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
	[Table(Name="ContactTypeTotals")]
	public partial class ContactTypeTotal
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _Id;
		
		private string _Description;
		
		private int? _Count;
		
		private int? _CountPeople;
		
		
		public ContactTypeTotal()
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

		
		[Column(Name="Description", Storage="_Description", DbType="varchar(100) NOT NULL")]
		public string Description
		{
			get
			{
				return this._Description;
			}

			set
			{
				if (this._Description != value)
					this._Description = value;
			}

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

		
		[Column(Name="CountPeople", Storage="_CountPeople", DbType="int")]
		public int? CountPeople
		{
			get
			{
				return this._CountPeople;
			}

			set
			{
				if (this._CountPeople != value)
					this._CountPeople = value;
			}

		}

		
    }

}
