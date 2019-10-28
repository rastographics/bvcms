using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Downline")]
    public partial class Downline : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int? _CategoryId;

        private int? _DownlineId;

        private int? _Generation;

        private int? _OrgId;

        private int? _LeaderId;

        private int? _DiscipleId;

        private DateTime? _StartDt;

        private string _Trace;

        private DateTime? _EndDt;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnCategoryIdChanging(int? value);
        partial void OnCategoryIdChanged();

        partial void OnDownlineIdChanging(int? value);
        partial void OnDownlineIdChanged();

        partial void OnGenerationChanging(int? value);
        partial void OnGenerationChanged();

        partial void OnOrgIdChanging(int? value);
        partial void OnOrgIdChanged();

        partial void OnLeaderIdChanging(int? value);
        partial void OnLeaderIdChanged();

        partial void OnDiscipleIdChanging(int? value);
        partial void OnDiscipleIdChanged();

        partial void OnStartDtChanging(DateTime? value);
        partial void OnStartDtChanged();

        partial void OnTraceChanging(string value);
        partial void OnTraceChanged();

        partial void OnEndDtChanging(DateTime? value);
        partial void OnEndDtChanged();

        #endregion
        public Downline()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "CategoryId", UpdateCheck = UpdateCheck.Never, Storage = "_CategoryId", DbType = "int")]
        public int? CategoryId
        {
            get => this._CategoryId;

            set
            {
                if (this._CategoryId != value)
                {

                    this.OnCategoryIdChanging(value);
                    this.SendPropertyChanging();
                    this._CategoryId = value;
                    this.SendPropertyChanged("CategoryId");
                    this.OnCategoryIdChanged();
                }

            }

        }


        [Column(Name = "DownlineId", UpdateCheck = UpdateCheck.Never, Storage = "_DownlineId", DbType = "int")]
        public int? DownlineId
        {
            get => this._DownlineId;

            set
            {
                if (this._DownlineId != value)
                {

                    this.OnDownlineIdChanging(value);
                    this.SendPropertyChanging();
                    this._DownlineId = value;
                    this.SendPropertyChanged("DownlineId");
                    this.OnDownlineIdChanged();
                }

            }

        }


        [Column(Name = "Generation", UpdateCheck = UpdateCheck.Never, Storage = "_Generation", DbType = "int")]
        public int? Generation
        {
            get => this._Generation;

            set
            {
                if (this._Generation != value)
                {

                    this.OnGenerationChanging(value);
                    this.SendPropertyChanging();
                    this._Generation = value;
                    this.SendPropertyChanged("Generation");
                    this.OnGenerationChanged();
                }

            }

        }


        [Column(Name = "OrgId", UpdateCheck = UpdateCheck.Never, Storage = "_OrgId", DbType = "int")]
        public int? OrgId
        {
            get => this._OrgId;

            set
            {
                if (this._OrgId != value)
                {

                    this.OnOrgIdChanging(value);
                    this.SendPropertyChanging();
                    this._OrgId = value;
                    this.SendPropertyChanged("OrgId");
                    this.OnOrgIdChanged();
                }

            }

        }


        [Column(Name = "LeaderId", UpdateCheck = UpdateCheck.Never, Storage = "_LeaderId", DbType = "int")]
        public int? LeaderId
        {
            get => this._LeaderId;

            set
            {
                if (this._LeaderId != value)
                {

                    this.OnLeaderIdChanging(value);
                    this.SendPropertyChanging();
                    this._LeaderId = value;
                    this.SendPropertyChanged("LeaderId");
                    this.OnLeaderIdChanged();
                }

            }

        }


        [Column(Name = "DiscipleId", UpdateCheck = UpdateCheck.Never, Storage = "_DiscipleId", DbType = "int")]
        public int? DiscipleId
        {
            get => this._DiscipleId;

            set
            {
                if (this._DiscipleId != value)
                {

                    this.OnDiscipleIdChanging(value);
                    this.SendPropertyChanging();
                    this._DiscipleId = value;
                    this.SendPropertyChanged("DiscipleId");
                    this.OnDiscipleIdChanged();
                }

            }

        }


        [Column(Name = "StartDt", UpdateCheck = UpdateCheck.Never, Storage = "_StartDt", DbType = "datetime")]
        public DateTime? StartDt
        {
            get => this._StartDt;

            set
            {
                if (this._StartDt != value)
                {

                    this.OnStartDtChanging(value);
                    this.SendPropertyChanging();
                    this._StartDt = value;
                    this.SendPropertyChanged("StartDt");
                    this.OnStartDtChanged();
                }

            }

        }


        [Column(Name = "Trace", UpdateCheck = UpdateCheck.Never, Storage = "_Trace", DbType = "varchar(400)")]
        public string Trace
        {
            get => this._Trace;

            set
            {
                if (this._Trace != value)
                {

                    this.OnTraceChanging(value);
                    this.SendPropertyChanging();
                    this._Trace = value;
                    this.SendPropertyChanged("Trace");
                    this.OnTraceChanged();
                }

            }

        }


        [Column(Name = "EndDt", UpdateCheck = UpdateCheck.Never, Storage = "_EndDt", DbType = "datetime")]
        public DateTime? EndDt
        {
            get => this._EndDt;

            set
            {
                if (this._EndDt != value)
                {

                    this.OnEndDtChanging(value);
                    this.SendPropertyChanging();
                    this._EndDt = value;
                    this.SendPropertyChanged("EndDt");
                    this.OnEndDtChanged();
                }

            }

        }


        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

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
        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }

}

