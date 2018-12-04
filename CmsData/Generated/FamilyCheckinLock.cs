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
	[Table(Name="dbo.FamilyCheckinLock")]
	public partial class FamilyCheckinLock : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _FamilyId;
		
		private bool _Locked;
		
		private DateTime _Created;
		
   		
    	
		private EntityRef<Family> _Family;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnFamilyIdChanging(int value);
		partial void OnFamilyIdChanged();
		
		partial void OnLockedChanging(bool value);
		partial void OnLockedChanged();
		
		partial void OnCreatedChanging(DateTime value);
		partial void OnCreatedChanged();
		
    #endregion
		public FamilyCheckinLock()
		{
			
			
			this._Family = default(EntityRef<Family>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="FamilyId", UpdateCheck=UpdateCheck.Never, Storage="_FamilyId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int FamilyId
		{
			get { return this._FamilyId; }

			set
			{
				if (this._FamilyId != value)
				{
				
					if (this._Family.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnFamilyIdChanging(value);
					this.SendPropertyChanging();
					this._FamilyId = value;
					this.SendPropertyChanged("FamilyId");
					this.OnFamilyIdChanged();
				}

			}

		}

		
		[Column(Name="Locked", UpdateCheck=UpdateCheck.Never, Storage="_Locked", DbType="bit NOT NULL")]
		public bool Locked
		{
			get { return this._Locked; }

			set
			{
				if (this._Locked != value)
				{
				
                    this.OnLockedChanging(value);
					this.SendPropertyChanging();
					this._Locked = value;
					this.SendPropertyChanged("Locked");
					this.OnLockedChanged();
				}

			}

		}

		
		[Column(Name="Created", UpdateCheck=UpdateCheck.Never, Storage="_Created", DbType="datetime NOT NULL")]
		public DateTime Created
		{
			get { return this._Created; }

			set
			{
				if (this._Created != value)
				{
				
                    this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_FamilyCheckinLock_FamilyCheckinLock1", Storage="_Family", ThisKey="FamilyId", IsForeignKey=true)]
		public Family Family
		{
			get { return this._Family.Entity; }

			set
			{
				Family previousValue = this._Family.Entity;
				if (((previousValue != value) 
							|| (this._Family.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Family.Entity = null;
						previousValue.FamilyCheckinLocks.Remove(this);
					}

					this._Family.Entity = value;
					if (value != null)
					{
						value.FamilyCheckinLocks.Add(this);
						
						this._FamilyId = value.FamilyId;
						
					}

					else
					{
						
						this._FamilyId = default(int);
						
					}

					this.SendPropertyChanged("Family");
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

