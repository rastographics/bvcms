using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.CheckinProfiles")]
    public partial class CheckinProfiles
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _CheckinProfileId;
        private string _Name;

        private EntitySet<CheckinProfileSettings> _CheckinProfileSettings;
        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnCheckinProfileIdChanging(int value);
        partial void OnCheckinProfileIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();
        #endregion

        public CheckinProfiles()
        {
            _CheckinProfileSettings = new EntitySet<CheckinProfileSettings>(new Action<CheckinProfileSettings>(attach_CheckinProfileSettings), new Action<CheckinProfileSettings>(detach_CheckinProfileSettings));

            OnCreated();
        }

        #region Columns

        [Column(Name = "CheckinProfileId", UpdateCheck = UpdateCheck.Never, Storage = "_CheckinProfileId", AutoSync = AutoSync.OnInsert, DbType = "int IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int CheckinProfileId
        {
            get => _CheckinProfileId;

            set
            {
                if (_CheckinProfileId != value)
                {
                    OnCheckinProfileIdChanging(value);
                    SendPropertyChanging();
                    _CheckinProfileId = value;
                    SendPropertyChanged("CheckinProfileId");
                    OnCheckinProfileIdChanged();
                }
            }
        }

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(100) NOT NULL UNIQUE")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    OnNameChanging(value);
                    SendPropertyChanging();
                    _Name = value;
                    SendPropertyChanged("Name");
                    OnNameChanged();
                }
            }
        }

        #endregion

        #region Foreign Keys

        #endregion

        #region Foreign Key Tables

        [Association(Name = "Checking_Profile_Settings_CP_FK", Storage = "_CheckinProfileSettings", OtherKey = "CheckinProfileId")]
        public EntitySet<CheckinProfileSettings> CheckinProfileSettings
        {
            get => _CheckinProfileSettings;

            set => _CheckinProfileSettings.Assign(value);
        }

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

        private void attach_CheckinProfileSettings(CheckinProfileSettings entity)
        {
            SendPropertyChanging();
            entity.CheckinProfiles = this;
        }

        private void detach_CheckinProfileSettings(CheckinProfileSettings entity)
        {
            SendPropertyChanging();
            entity.CheckinProfiles = null;
        }
    }
}
