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
	[Table(Name="CheckinMatchBarCodeOnly")]
	public partial class CheckinMatchBarCodeOnly
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _Familyid;
		
		private string _Name;
		
		private bool? _Locked;
		
		
		public CheckinMatchBarCodeOnly()
		{
		}

		
		
		[Column(Name="familyid", Storage="_Familyid", DbType="int")]
		public int? Familyid
		{
			get
			{
				return this._Familyid;
			}

			set
			{
				if (this._Familyid != value)
					this._Familyid = value;
			}

		}

		
		[Column(Name="NAME", Storage="_Name", DbType="nvarchar(100)")]
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

		
		[Column(Name="locked", Storage="_Locked", DbType="bit")]
		public bool? Locked
		{
			get
			{
				return this._Locked;
			}

			set
			{
				if (this._Locked != value)
					this._Locked = value;
			}

		}

		
    }

}
