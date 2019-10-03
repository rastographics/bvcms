using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace ImageData
{
    [Table(Name = "dbo.Other")]
    public partial class Other : INotifyPropertyChanging, INotifyPropertyChanged
    {
        #region Columns

        [Column(Name = "id", UpdateCheck = UpdateCheck.Never, AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(Name = "created", UpdateCheck = UpdateCheck.Never, DbType = "datetime NOT NULL")]
        public DateTime Created { get; set; }

        [Column(Name = "userID", UpdateCheck = UpdateCheck.Never, DbType = "int NOT NULL")]
        public int UserID { get; set; }

        [Column(Name = "first", UpdateCheck = UpdateCheck.Never, DbType = "varbinary")]
        public byte[] First { get; set; }

        [Column(Name = "second", UpdateCheck = UpdateCheck.Never, DbType = "varbinary")]
        public byte[] Second { get; set; }

        #endregion

        #region Events

        #pragma warning disable 0067
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}

