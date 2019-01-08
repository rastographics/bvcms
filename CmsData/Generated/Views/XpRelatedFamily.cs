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
	[Table(Name="XpRelatedFamily")]
	public partial class XpRelatedFamily
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _FamilyId;
		
		private int _RelatedFamilyId;
		
		private string _FamilyRelationshipDesc;
		
		
		public XpRelatedFamily()
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

		
		[Column(Name="RelatedFamilyId", Storage="_RelatedFamilyId", DbType="int NOT NULL")]
		public int RelatedFamilyId
		{
			get
			{
				return this._RelatedFamilyId;
			}

			set
			{
				if (this._RelatedFamilyId != value)
					this._RelatedFamilyId = value;
			}

		}

		
		[Column(Name="FamilyRelationshipDesc", Storage="_FamilyRelationshipDesc", DbType="nvarchar(256) NOT NULL")]
		public string FamilyRelationshipDesc
		{
			get
			{
				return this._FamilyRelationshipDesc;
			}

			set
			{
				if (this._FamilyRelationshipDesc != value)
					this._FamilyRelationshipDesc = value;
			}

		}

		
    }

}
