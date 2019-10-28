using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Picture")]
    public partial class Picture : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _PictureId;

        private DateTime? _CreatedDate;

        private string _CreatedBy;

        private int? _LargeId;

        private int? _MediumId;

        private int? _SmallId;

        private int? _ThumbId;

        private int? _X;

        private int? _Y;

        private EntitySet<Family> _Families;

        private EntitySet<Person> _People;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnPictureIdChanging(int value);
        partial void OnPictureIdChanged();

        partial void OnCreatedDateChanging(DateTime? value);
        partial void OnCreatedDateChanged();

        partial void OnCreatedByChanging(string value);
        partial void OnCreatedByChanged();

        partial void OnLargeIdChanging(int? value);
        partial void OnLargeIdChanged();

        partial void OnMediumIdChanging(int? value);
        partial void OnMediumIdChanged();

        partial void OnSmallIdChanging(int? value);
        partial void OnSmallIdChanged();

        partial void OnThumbIdChanging(int? value);
        partial void OnThumbIdChanged();

        partial void OnXChanging(int? value);
        partial void OnXChanged();

        partial void OnYChanging(int? value);
        partial void OnYChanged();

        #endregion

        public Picture()
        {
            _Families = new EntitySet<Family>(new Action<Family>(attach_Families), new Action<Family>(detach_Families));

            _People = new EntitySet<Person>(new Action<Person>(attach_People), new Action<Person>(detach_People));

            OnCreated();
        }

        #region Columns

        [Column(Name = "PictureId", UpdateCheck = UpdateCheck.Never, Storage = "_PictureId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int PictureId
        {
            get => _PictureId;

            set
            {
                if (_PictureId != value)
                {
                    OnPictureIdChanging(value);
                    SendPropertyChanging();
                    _PictureId = value;
                    SendPropertyChanged("PictureId");
                    OnPictureIdChanged();
                }
            }
        }

        [Column(Name = "CreatedDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime")]
        public DateTime? CreatedDate
        {
            get => _CreatedDate;

            set
            {
                if (_CreatedDate != value)
                {
                    OnCreatedDateChanging(value);
                    SendPropertyChanging();
                    _CreatedDate = value;
                    SendPropertyChanged("CreatedDate");
                    OnCreatedDateChanged();
                }
            }
        }

        [Column(Name = "CreatedBy", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "nvarchar(50)")]
        public string CreatedBy
        {
            get => _CreatedBy;

            set
            {
                if (_CreatedBy != value)
                {
                    OnCreatedByChanging(value);
                    SendPropertyChanging();
                    _CreatedBy = value;
                    SendPropertyChanged("CreatedBy");
                    OnCreatedByChanged();
                }
            }
        }

        [Column(Name = "LargeId", UpdateCheck = UpdateCheck.Never, Storage = "_LargeId", DbType = "int")]
        public int? LargeId
        {
            get => _LargeId;

            set
            {
                if (_LargeId != value)
                {
                    OnLargeIdChanging(value);
                    SendPropertyChanging();
                    _LargeId = value;
                    SendPropertyChanged("LargeId");
                    OnLargeIdChanged();
                }
            }
        }

        [Column(Name = "MediumId", UpdateCheck = UpdateCheck.Never, Storage = "_MediumId", DbType = "int")]
        public int? MediumId
        {
            get => _MediumId;

            set
            {
                if (_MediumId != value)
                {
                    OnMediumIdChanging(value);
                    SendPropertyChanging();
                    _MediumId = value;
                    SendPropertyChanged("MediumId");
                    OnMediumIdChanged();
                }
            }
        }

        [Column(Name = "SmallId", UpdateCheck = UpdateCheck.Never, Storage = "_SmallId", DbType = "int")]
        public int? SmallId
        {
            get => _SmallId;

            set
            {
                if (_SmallId != value)
                {
                    OnSmallIdChanging(value);
                    SendPropertyChanging();
                    _SmallId = value;
                    SendPropertyChanged("SmallId");
                    OnSmallIdChanged();
                }
            }
        }

        [Column(Name = "ThumbId", UpdateCheck = UpdateCheck.Never, Storage = "_ThumbId", DbType = "int")]
        public int? ThumbId
        {
            get => _ThumbId;

            set
            {
                if (_ThumbId != value)
                {
                    OnThumbIdChanging(value);
                    SendPropertyChanging();
                    _ThumbId = value;
                    SendPropertyChanged("ThumbId");
                    OnThumbIdChanged();
                }
            }
        }

        [Column(Name = "X", UpdateCheck = UpdateCheck.Never, Storage = "_X", DbType = "int")]
        public int? X
        {
            get => _X;

            set
            {
                if (_X != value)
                {
                    OnXChanging(value);
                    SendPropertyChanging();
                    _X = value;
                    SendPropertyChanged("X");
                    OnXChanged();
                }
            }
        }

        [Column(Name = "Y", UpdateCheck = UpdateCheck.Never, Storage = "_Y", DbType = "int")]
        public int? Y
        {
            get => _Y;

            set
            {
                if (_Y != value)
                {
                    OnYChanging(value);
                    SendPropertyChanging();
                    _Y = value;
                    SendPropertyChanged("Y");
                    OnYChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_Families_Picture", Storage = "_Families", OtherKey = "PictureId")]
        public EntitySet<Family> Families
           {
               get => _Families;

            set => _Families.Assign(value);

           }

        [Association(Name = "FK_PEOPLE_TBL_Picture", Storage = "_People", OtherKey = "PictureId")]
        public EntitySet<Person> People
           {
               get => _People;

            set => _People.Assign(value);

           }

        #endregion

        #region Foreign Keys

        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            if ((PropertyChanging != null))
            {
                PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void attach_Families(Family entity)
        {
            SendPropertyChanging();
            entity.Picture = this;
        }

        private void detach_Families(Family entity)
        {
            SendPropertyChanging();
            entity.Picture = null;
        }

        private void attach_People(Person entity)
        {
            SendPropertyChanging();
            entity.Picture = this;
        }

        private void detach_People(Person entity)
        {
            SendPropertyChanging();
            entity.Picture = null;
        }
    }
}
