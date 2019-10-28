using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RegisterLinksFromMaster")]
    public partial class RegisterLinksFromMaster
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private string _Title;

        private string _OrganizationName;

        private string _Description;

        private string _AppCategory;

        private string _PublicSortOrder;

        private bool? _UseRegisterLink2;

        private string _RegisterLinkHeader;

        public RegisterLinksFromMaster()
        {
        }

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", DbType = "int NOT NULL")]
        public int OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    _OrganizationId = value;
                }
            }
        }

        [Column(Name = "Title", Storage = "_Title", DbType = "nvarchar(4000)")]
        public string Title
        {
            get => _Title;

            set
            {
                if (_Title != value)
                {
                    _Title = value;
                }
            }
        }

        [Column(Name = "OrganizationName", Storage = "_OrganizationName", DbType = "nvarchar(100) NOT NULL")]
        public string OrganizationName
        {
            get => _OrganizationName;

            set
            {
                if (_OrganizationName != value)
                {
                    _OrganizationName = value;
                }
            }
        }

        [Column(Name = "Description", Storage = "_Description", DbType = "nvarchar")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    _Description = value;
                }
            }
        }

        [Column(Name = "AppCategory", Storage = "_AppCategory", DbType = "varchar(15)")]
        public string AppCategory
        {
            get => _AppCategory;

            set
            {
                if (_AppCategory != value)
                {
                    _AppCategory = value;
                }
            }
        }

        [Column(Name = "PublicSortOrder", Storage = "_PublicSortOrder", DbType = "varchar(15)")]
        public string PublicSortOrder
        {
            get => _PublicSortOrder;

            set
            {
                if (_PublicSortOrder != value)
                {
                    _PublicSortOrder = value;
                }
            }
        }

        [Column(Name = "UseRegisterLink2", Storage = "_UseRegisterLink2", DbType = "bit")]
        public bool? UseRegisterLink2
        {
            get => _UseRegisterLink2;

            set
            {
                if (_UseRegisterLink2 != value)
                {
                    _UseRegisterLink2 = value;
                }
            }
        }

        [Column(Name = "RegisterLinkHeader", Storage = "_RegisterLinkHeader", DbType = "nvarchar")]
        public string RegisterLinkHeader
        {
            get => _RegisterLinkHeader;

            set
            {
                if (_RegisterLinkHeader != value)
                {
                    _RegisterLinkHeader = value;
                }
            }
        }
    }
}
