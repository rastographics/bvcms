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
	[Table(Name="dbo.GoerSupporter")]
	public partial class GoerSupporter : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int _GoerId;
		
		private int? _SupporterId;
		
		private string _NoDbEmail;
		
		private bool? _Active;
		
		private bool? _Unsubscribe;
		
		private DateTime _Created;
		
		private string _Salutation;
		
   		
    	
		private EntityRef<Person> _Supporter;
		
		private EntityRef<Person> _Goer;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnGoerIdChanging(int value);
		partial void OnGoerIdChanged();
		
		partial void OnSupporterIdChanging(int? value);
		partial void OnSupporterIdChanged();
		
		partial void OnNoDbEmailChanging(string value);
		partial void OnNoDbEmailChanged();
		
		partial void OnActiveChanging(bool? value);
		partial void OnActiveChanged();
		
		partial void OnUnsubscribeChanging(bool? value);
		partial void OnUnsubscribeChanged();
		
		partial void OnCreatedChanging(DateTime value);
		partial void OnCreatedChanged();
		
		partial void OnSalutationChanging(string value);
		partial void OnSalutationChanged();
		
    #endregion
		public GoerSupporter()
		{
			
			
			this._Supporter = default(EntityRef<Person>); 
			
			this._Goer = default(EntityRef<Person>); 
			
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

		
		[Column(Name="GoerId", UpdateCheck=UpdateCheck.Never, Storage="_GoerId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int GoerId
		{
			get { return this._GoerId; }

			set
			{
				if (this._GoerId != value)
				{
				
					if (this._Goer.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnGoerIdChanging(value);
					this.SendPropertyChanging();
					this._GoerId = value;
					this.SendPropertyChanged("GoerId");
					this.OnGoerIdChanged();
				}

			}

		}

		
		[Column(Name="SupporterId", UpdateCheck=UpdateCheck.Never, Storage="_SupporterId", DbType="int")]
		[IsForeignKey]
		public int? SupporterId
		{
			get { return this._SupporterId; }

			set
			{
				if (this._SupporterId != value)
				{
				
					if (this._Supporter.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnSupporterIdChanging(value);
					this.SendPropertyChanging();
					this._SupporterId = value;
					this.SendPropertyChanged("SupporterId");
					this.OnSupporterIdChanged();
				}

			}

		}

		
		[Column(Name="NoDbEmail", UpdateCheck=UpdateCheck.Never, Storage="_NoDbEmail", DbType="varchar(80)")]
		public string NoDbEmail
		{
			get { return this._NoDbEmail; }

			set
			{
				if (this._NoDbEmail != value)
				{
				
                    this.OnNoDbEmailChanging(value);
					this.SendPropertyChanging();
					this._NoDbEmail = value;
					this.SendPropertyChanged("NoDbEmail");
					this.OnNoDbEmailChanged();
				}

			}

		}

		
		[Column(Name="Active", UpdateCheck=UpdateCheck.Never, Storage="_Active", DbType="bit")]
		public bool? Active
		{
			get { return this._Active; }

			set
			{
				if (this._Active != value)
				{
				
                    this.OnActiveChanging(value);
					this.SendPropertyChanging();
					this._Active = value;
					this.SendPropertyChanged("Active");
					this.OnActiveChanged();
				}

			}

		}

		
		[Column(Name="Unsubscribe", UpdateCheck=UpdateCheck.Never, Storage="_Unsubscribe", DbType="bit")]
		public bool? Unsubscribe
		{
			get { return this._Unsubscribe; }

			set
			{
				if (this._Unsubscribe != value)
				{
				
                    this.OnUnsubscribeChanging(value);
					this.SendPropertyChanging();
					this._Unsubscribe = value;
					this.SendPropertyChanged("Unsubscribe");
					this.OnUnsubscribeChanged();
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

		
		[Column(Name="Salutation", UpdateCheck=UpdateCheck.Never, Storage="_Salutation", DbType="nvarchar(80)")]
		public string Salutation
		{
			get { return this._Salutation; }

			set
			{
				if (this._Salutation != value)
				{
				
                    this.OnSalutationChanging(value);
					this.SendPropertyChanging();
					this._Salutation = value;
					this.SendPropertyChanged("Salutation");
					this.OnSalutationChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_Goers__Supporter", Storage="_Supporter", ThisKey="SupporterId", IsForeignKey=true)]
		public Person Supporter
		{
			get { return this._Supporter.Entity; }

			set
			{
				Person previousValue = this._Supporter.Entity;
				if (((previousValue != value) 
							|| (this._Supporter.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Supporter.Entity = null;
						previousValue.FK_Goers.Remove(this);
					}

					this._Supporter.Entity = value;
					if (value != null)
					{
						value.FK_Goers.Add(this);
						
						this._SupporterId = value.PeopleId;
						
					}

					else
					{
						
						this._SupporterId = default(int?);
						
					}

					this.SendPropertyChanged("Supporter");
				}

			}

		}

		
		[Association(Name="FK_Supporters__Goer", Storage="_Goer", ThisKey="GoerId", IsForeignKey=true)]
		public Person Goer
		{
			get { return this._Goer.Entity; }

			set
			{
				Person previousValue = this._Goer.Entity;
				if (((previousValue != value) 
							|| (this._Goer.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Goer.Entity = null;
						previousValue.FK_Supporters.Remove(this);
					}

					this._Goer.Entity = value;
					if (value != null)
					{
						value.FK_Supporters.Add(this);
						
						this._GoerId = value.PeopleId;
						
					}

					else
					{
						
						this._GoerId = default(int);
						
					}

					this.SendPropertyChanged("Goer");
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

