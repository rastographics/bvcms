using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace ImageData
{
    [Table(Name = "dbo.Image")]
    public partial class Image : INotifyPropertyChanging, INotifyPropertyChanged
    {
        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(Name = "bits", UpdateCheck = UpdateCheck.Never, DbType = "varbinary")]
        public byte[] Bits { get; set; }

        [Column(Name = "length", UpdateCheck = UpdateCheck.Never, DbType = "int")]
        public int? Length { get; set; }

        [Column(Name = "mimetype", UpdateCheck = UpdateCheck.Never, DbType = "varchar(20)")]
        public string Mimetype { get; set; }

        [Column(Name = "secure", UpdateCheck = UpdateCheck.Never, DbType = "bit")]
        public bool? Secure { get; set; }

        #endregion

        #region Events

        #pragma warning disable 0067
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

}

