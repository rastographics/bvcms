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
	[Table(Name="dbo.ProgDiv")]
	public partial class ProgDiv : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _ProgId;
		
		private int _DivId;
		
   		
    	
		private EntityRef<Division> _Division;
		
		private EntityRef<Program> _Program;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnProgIdChanging(int value);
		partial void OnProgIdChanged();
		
		partial void OnDivIdChanging(int value);
		partial void OnDivIdChanged();
		
    #endregion
		public ProgDiv()
		{
			
			
			this._Division = default(EntityRef<Division>); 
			
			this._Program = default(EntityRef<Program>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="ProgId", UpdateCheck=UpdateCheck.Never, Storage="_ProgId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int ProgId
		{
			get { return this._ProgId; }

			set
			{
				if (this._ProgId != value)
				{
				
					if (this._Program.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnProgIdChanging(value);
					this.SendPropertyChanging();
					this._ProgId = value;
					this.SendPropertyChanged("ProgId");
					this.OnProgIdChanged();
				}

			}

		}

		
		[Column(Name="DivId", UpdateCheck=UpdateCheck.Never, Storage="_DivId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int DivId
		{
			get { return this._DivId; }

			set
			{
				if (this._DivId != value)
				{
				
					if (this._Division.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnDivIdChanging(value);
					this.SendPropertyChanging();
					this._DivId = value;
					this.SendPropertyChanged("DivId");
					this.OnDivIdChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_ProgDiv_Division", Storage="_Division", ThisKey="DivId", IsForeignKey=true)]
		public Division Division
		{
			get { return this._Division.Entity; }

			set
			{
				Division previousValue = this._Division.Entity;
				if (((previousValue != value) 
							|| (this._Division.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Division.Entity = null;
						previousValue.ProgDivs.Remove(this);
					}

					this._Division.Entity = value;
					if (value != null)
					{
						value.ProgDivs.Add(this);
						
						this._DivId = value.Id;
						
					}

					else
					{
						
						this._DivId = default(int);
						
					}

					this.SendPropertyChanged("Division");
				}

			}

		}

		
		[Association(Name="FK_ProgDiv_Program", Storage="_Program", ThisKey="ProgId", IsForeignKey=true)]
		public Program Program
		{
			get { return this._Program.Entity; }

			set
			{
				Program previousValue = this._Program.Entity;
				if (((previousValue != value) 
							|| (this._Program.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Program.Entity = null;
						previousValue.ProgDivs.Remove(this);
					}

					this._Program.Entity = value;
					if (value != null)
					{
						value.ProgDivs.Add(this);
						
						this._ProgId = value.Id;
						
					}

					else
					{
						
						this._ProgId = default(int);
						
					}

					this.SendPropertyChanged("Program");
				}

			}

		}

		
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

   		
	}

}

