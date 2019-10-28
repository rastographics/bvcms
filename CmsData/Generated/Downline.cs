using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Downline")]
    public partial class Downline : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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
            get => _CategoryId;

            set
            {
                if (_CategoryId != value)
                {
                    OnCategoryIdChanging(value);
                    SendPropertyChanging();
                    _CategoryId = value;
                    SendPropertyChanged("CategoryId");
                    OnCategoryIdChanged();
                }
            }
        }

        [Column(Name = "DownlineId", UpdateCheck = UpdateCheck.Never, Storage = "_DownlineId", DbType = "int")]
        public int? DownlineId
        {
            get => _DownlineId;

            set
            {
                if (_DownlineId != value)
                {
                    OnDownlineIdChanging(value);
                    SendPropertyChanging();
                    _DownlineId = value;
                    SendPropertyChanged("DownlineId");
                    OnDownlineIdChanged();
                }
            }
        }

        [Column(Name = "Generation", UpdateCheck = UpdateCheck.Never, Storage = "_Generation", DbType = "int")]
        public int? Generation
        {
            get => _Generation;

            set
            {
                if (_Generation != value)
                {
                    OnGenerationChanging(value);
                    SendPropertyChanging();
                    _Generation = value;
                    SendPropertyChanged("Generation");
                    OnGenerationChanged();
                }
            }
        }

        [Column(Name = "OrgId", UpdateCheck = UpdateCheck.Never, Storage = "_OrgId", DbType = "int")]
        public int? OrgId
        {
            get => _OrgId;

            set
            {
                if (_OrgId != value)
                {
                    OnOrgIdChanging(value);
                    SendPropertyChanging();
                    _OrgId = value;
                    SendPropertyChanged("OrgId");
                    OnOrgIdChanged();
                }
            }
        }

        [Column(Name = "LeaderId", UpdateCheck = UpdateCheck.Never, Storage = "_LeaderId", DbType = "int")]
        public int? LeaderId
        {
            get => _LeaderId;

            set
            {
                if (_LeaderId != value)
                {
                    OnLeaderIdChanging(value);
                    SendPropertyChanging();
                    _LeaderId = value;
                    SendPropertyChanged("LeaderId");
                    OnLeaderIdChanged();
                }
            }
        }

        [Column(Name = "DiscipleId", UpdateCheck = UpdateCheck.Never, Storage = "_DiscipleId", DbType = "int")]
        public int? DiscipleId
        {
            get => _DiscipleId;

            set
            {
                if (_DiscipleId != value)
                {
                    OnDiscipleIdChanging(value);
                    SendPropertyChanging();
                    _DiscipleId = value;
                    SendPropertyChanged("DiscipleId");
                    OnDiscipleIdChanged();
                }
            }
        }

        [Column(Name = "StartDt", UpdateCheck = UpdateCheck.Never, Storage = "_StartDt", DbType = "datetime")]
        public DateTime? StartDt
        {
            get => _StartDt;

            set
            {
                if (_StartDt != value)
                {
                    OnStartDtChanging(value);
                    SendPropertyChanging();
                    _StartDt = value;
                    SendPropertyChanged("StartDt");
                    OnStartDtChanged();
                }
            }
        }

        [Column(Name = "Trace", UpdateCheck = UpdateCheck.Never, Storage = "_Trace", DbType = "varchar(400)")]
        public string Trace
        {
            get => _Trace;

            set
            {
                if (_Trace != value)
                {
                    OnTraceChanging(value);
                    SendPropertyChanging();
                    _Trace = value;
                    SendPropertyChanged("Trace");
                    OnTraceChanged();
                }
            }
        }

        [Column(Name = "EndDt", UpdateCheck = UpdateCheck.Never, Storage = "_EndDt", DbType = "datetime")]
        public DateTime? EndDt
        {
            get => _EndDt;

            set
            {
                if (_EndDt != value)
                {
                    OnEndDtChanging(value);
                    SendPropertyChanging();
                    _EndDt = value;
                    SendPropertyChanged("EndDt");
                    OnEndDtChanged();
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
    }
}
