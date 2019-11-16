using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpRelatedFamily")]
    public partial class XpRelatedFamily
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _FamilyId;

        private int _RelatedFamilyId;

        private string _FamilyRelationshipDesc;

        public XpRelatedFamily()
        {
        }

        [Column(Name = "FamilyId", Storage = "_FamilyId", DbType = "int NOT NULL")]
        public int FamilyId
        {
            get => _FamilyId;

            set
            {
                if (_FamilyId != value)
                {
                    _FamilyId = value;
                }
            }
        }

        [Column(Name = "RelatedFamilyId", Storage = "_RelatedFamilyId", DbType = "int NOT NULL")]
        public int RelatedFamilyId
        {
            get => _RelatedFamilyId;

            set
            {
                if (_RelatedFamilyId != value)
                {
                    _RelatedFamilyId = value;
                }
            }
        }

        [Column(Name = "FamilyRelationshipDesc", Storage = "_FamilyRelationshipDesc", DbType = "nvarchar(256) NOT NULL")]
        public string FamilyRelationshipDesc
        {
            get => _FamilyRelationshipDesc;

            set
            {
                if (_FamilyRelationshipDesc != value)
                {
                    _FamilyRelationshipDesc = value;
                }
            }
        }
    }
}
