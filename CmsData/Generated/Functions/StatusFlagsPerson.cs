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
	[Table(Name="StatusFlagsPerson")]
	public partial class StatusFlagsPerson
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private string _Flag;
		
		private string _Name;
		
		private string _RoleName;
		
		
		public StatusFlagsPerson()
		{
		}

		
		
		[Column(Name="Flag", Storage="_Flag", DbType="nvarchar(200) NOT NULL")]
		public string Flag
		{
			get
			{
				return this._Flag;
			}

			set
			{
				if (this._Flag != value)
					this._Flag = value;
			}

		}

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(100)")]
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

		
		[Column(Name="RoleName", Storage="_RoleName", DbType="nvarchar(50)")]
		public string RoleName
		{
			get
			{
				return this._RoleName;
			}

			set
			{
				if (this._RoleName != value)
					this._RoleName = value;
			}

		}

		
    }

}
