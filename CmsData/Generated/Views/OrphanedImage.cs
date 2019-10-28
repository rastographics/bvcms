using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OrphanedImages")]
    public partial class OrphanedImage
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _Id;

        private string _Mimetype;

        private int? _Length;

        public OrphanedImage()
        {
        }

        [Column(Name = "Id", Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsDbGenerated = true)]
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

        [Column(Name = "mimetype", Storage = "_Mimetype", DbType = "varchar(20)")]
        public string Mimetype
        {
            get => _Mimetype;

            set
            {
                if (_Mimetype != value)
                {
                    _Mimetype = value;
                }
            }
        }

        [Column(Name = "length", Storage = "_Length", DbType = "int")]
        public int? Length
        {
            get => _Length;

            set
            {
                if (_Length != value)
                {
                    _Length = value;
                }
            }
        }
    }
}
