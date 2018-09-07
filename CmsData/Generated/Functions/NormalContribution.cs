using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "NormalContributions")]
    public partial class NormalContribution
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);


        private int _ContributionId;

        private decimal? _ContributionAmount;

        private DateTime? _ContributionDate;

        private string _FundName;

        private string _CheckNo;

        private string _Name;

        private string _Description;

        private string _FundDescription;


        public NormalContribution()
        {
        }



        [Column(Name = "ContributionId", Storage = "_ContributionId", DbType = "int NOT NULL")]
        public int ContributionId
        {
            get
            {
                return this._ContributionId;
            }

            set
            {
                if (this._ContributionId != value)
                {
                    this._ContributionId = value;
                }
            }

        }


        [Column(Name = "ContributionAmount", Storage = "_ContributionAmount", DbType = "Decimal(11,2)")]
        public decimal? ContributionAmount
        {
            get
            {
                return this._ContributionAmount;
            }

            set
            {
                if (this._ContributionAmount != value)
                {
                    this._ContributionAmount = value;
                }
            }

        }


        [Column(Name = "ContributionDate", Storage = "_ContributionDate", DbType = "datetime")]
        public DateTime? ContributionDate
        {
            get
            {
                return this._ContributionDate;
            }

            set
            {
                if (this._ContributionDate != value)
                {
                    this._ContributionDate = value;
                }
            }

        }


        [Column(Name = "FundName", Storage = "_FundName", DbType = "nvarchar(256) NOT NULL")]
        public string FundName
        {
            get
            {
                return this._FundName;
            }

            set
            {
                if (this._FundName != value)
                {
                    this._FundName = value;
                }
            }

        }


        [Column(Name = "CheckNo", Storage = "_CheckNo", DbType = "nvarchar(20)")]
        public string CheckNo
        {
            get
            {
                return this._CheckNo;
            }

            set
            {
                if (this._CheckNo != value)
                {
                    this._CheckNo = value;
                }
            }

        }


        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(138)")]
        public string Name
        {
            get
            {
                return this._Name;
            }

            set
            {
                if (this._Name != value)
                {
                    this._Name = value;
                }
            }

        }


        [Column(Name = "Description", Storage = "_Description", DbType = "nvarchar(256)")]
        public string Description
        {
            get
            {
                return this._Description;
            }

            set
            {
                if (this._Description != value)
                {
                    this._Description = value;
                }
            }

        }

        [Column(Name = "FundDescription", Storage = "_FundDescription", DbType = "nvarchar(256)")]
        public string FundDescription
        {
            get
            {
                return this._FundDescription;
            }

            set
            {
                if (this._FundDescription != value)
                {
                    this._FundDescription = value;
                }
            }
        }


    }

}
