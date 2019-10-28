using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "ContactTypeTotals")]
    public partial class ContactTypeTotal
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _Id;

        private string _Description;

        private int? _Count;

        private int? _CountPeople;

        public ContactTypeTotal()
        {
        }

        [Column(Name = "Id", Storage = "_Id", DbType = "int NOT NULL")]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    _Id = value;
                }
            }
        }

        [Column(Name = "Description", Storage = "_Description", DbType = "varchar(100) NOT NULL")]
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

        [Column(Name = "Count", Storage = "_Count", DbType = "int")]
        public int? Count
        {
            get => _Count;

            set
            {
                if (_Count != value)
                {
                    _Count = value;
                }
            }
        }

        [Column(Name = "CountPeople", Storage = "_CountPeople", DbType = "int")]
        public int? CountPeople
        {
            get => _CountPeople;

            set
            {
                if (_CountPeople != value)
                {
                    _CountPeople = value;
                }
            }
        }
    }
}
