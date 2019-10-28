using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.CheckinProfiles")]
    public partial class CheckinProfiles
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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
            this._CheckinProfileSettings = new EntitySet<CheckinProfileSettings>(new Action<CheckinProfileSettings>(this.attach_CheckinProfileSettings), new Action<CheckinProfileSettings>(this.detach_CheckinProfileSettings));

            OnCreated();
        }

        #region Columns
        [Column(Name = "CheckinProfileId", UpdateCheck = UpdateCheck.Never, Storage = "_CheckinProfileId", AutoSync = AutoSync.OnInsert, DbType = "int IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int CheckinProfileId
        {
            get => this._CheckinProfileId;

            set
            {
                if (this._CheckinProfileId != value)
                {
                    this.OnCheckinProfileIdChanging(value);
                    this.SendPropertyChanging();
                    this._CheckinProfileId = value;
                    this.SendPropertyChanged("CheckinProfileId");
                    this.OnCheckinProfileIdChanged();
                }
            }
        }

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(100) NOT NULL UNIQUE")]
        public string Name
        {
            get => this._Name;

            set
            {
                if (this._Name != value)
                {
                    this.OnNameChanging(value);
                    this.SendPropertyChanging();
                    this._Name = value;
                    this.SendPropertyChanged("Name");
                    this.OnNameChanged();
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
            get => this._CheckinProfileSettings;

            set => this._CheckinProfileSettings.Assign(value);
        }
        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void attach_CheckinProfileSettings(CheckinProfileSettings entity)
        {
            this.SendPropertyChanging();
            entity.CheckinProfiles = this;
        }

        private void detach_CheckinProfileSettings(CheckinProfileSettings entity)
        {
            this.SendPropertyChanging();
            entity.CheckinProfiles = null;
        }
    }
}
