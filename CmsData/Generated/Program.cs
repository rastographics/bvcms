using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using CmsData.Infrastructure;

namespace CmsData
{
	[Table(Name="dbo.Program")]
	public partial class Program : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Name;
		
		private string _RptGroup;
		
		private decimal? _StartHoursOffset;
		
		private decimal? _EndHoursOffset;
		
   		
   		private EntitySet<Division> _Divisions;
		
   		private EntitySet<ProgDiv> _ProgDivs;
		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
		partial void OnRptGroupChanging(string value);
		partial void OnRptGroupChanged();
		
		partial void OnStartHoursOffsetChanging(decimal? value);
		partial void OnStartHoursOffsetChanged();
		
		partial void OnEndHoursOffsetChanging(decimal? value);
		partial void OnEndHoursOffsetChanged();
		
    #endregion
		public Program()
		{
			
			this._Divisions = new EntitySet<Division>(new Action< Division>(this.attach_Divisions), new Action< Division>(this.detach_Divisions)); 
			
			this._ProgDivs = new EntitySet<ProgDiv>(new Action< ProgDiv>(this.attach_ProgDivs), new Action< ProgDiv>(this.detach_ProgDivs)); 
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get { return this._Id; }

			set
			{
				if (this._Id != value)
				{
				
                    this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}

			}

		}

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(50)")]
		public string Name
		{
			get { return this._Name; }

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

		
		[Column(Name="RptGroup", UpdateCheck=UpdateCheck.Never, Storage="_RptGroup", DbType="nvarchar(200)")]
		public string RptGroup
		{
			get { return this._RptGroup; }

			set
			{
				if (this._RptGroup != value)
				{
				
                    this.OnRptGroupChanging(value);
					this.SendPropertyChanging();
					this._RptGroup = value;
					this.SendPropertyChanged("RptGroup");
					this.OnRptGroupChanged();
				}

			}

		}

		
		[Column(Name="StartHoursOffset", UpdateCheck=UpdateCheck.Never, Storage="_StartHoursOffset", DbType="real")]
		public decimal? StartHoursOffset
		{
			get { return this._StartHoursOffset; }

			set
			{
				if (this._StartHoursOffset != value)
				{
				
                    this.OnStartHoursOffsetChanging(value);
					this.SendPropertyChanging();
					this._StartHoursOffset = value;
					this.SendPropertyChanged("StartHoursOffset");
					this.OnStartHoursOffsetChanged();
				}

			}

		}

		
		[Column(Name="EndHoursOffset", UpdateCheck=UpdateCheck.Never, Storage="_EndHoursOffset", DbType="real")]
		public decimal? EndHoursOffset
		{
			get { return this._EndHoursOffset; }

			set
			{
				if (this._EndHoursOffset != value)
				{
				
                    this.OnEndHoursOffsetChanging(value);
					this.SendPropertyChanging();
					this._EndHoursOffset = value;
					this.SendPropertyChanged("EndHoursOffset");
					this.OnEndHoursOffsetChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_Division_Program", Storage="_Divisions", OtherKey="ProgId")]
   		public EntitySet<Division> Divisions
   		{
   		    get { return this._Divisions; }

			set	{ this._Divisions.Assign(value); }

   		}

		
   		[Association(Name="FK_ProgDiv_Program", Storage="_ProgDivs", OtherKey="ProgId")]
   		public EntitySet<ProgDiv> ProgDivs
   		{
   		    get { return this._ProgDivs; }

			set	{ this._ProgDivs.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
	#endregion
	
		public event PropertyChangingEventHandler PropertyChanging;
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
				this.PropertyChanging(this, emptyChangingEventArgs);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

   		
		private void attach_Divisions(Division entity)
		{
			this.SendPropertyChanging();
			entity.Program = this;
		}

		private void detach_Divisions(Division entity)
		{
			this.SendPropertyChanging();
			entity.Program = null;
		}

		
		private void attach_ProgDivs(ProgDiv entity)
		{
			this.SendPropertyChanging();
			entity.Program = this;
		}

		private void detach_ProgDivs(ProgDiv entity)
		{
			this.SendPropertyChanging();
			entity.Program = null;
		}

		
	}

}
