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
	[Table(Name="lookup.ScheduledGiftType")]
	public partial class ScheduledGiftType : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs => new PropertyChangingEventArgs(string.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Description;
		
		private bool? _Hardwired;
		   		
   		private EntitySet<ScheduledGift> _ScheduledGifts;
		
	#endregion
	
    #region Extensibility Method Definitions
		partial void OnLoaded();
		partial void OnValidate(System.Data.Linq.ChangeAction action);
		partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnDescriptionChanging(string value);
		partial void OnDescriptionChanged();
		
		partial void OnHardwiredChanging(bool? value);
		partial void OnHardwiredChanged();
		
    #endregion
		public ScheduledGiftType()
		{
			OnCreated();
		}

    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get { return _Id; }
			set
			{
				if (_Id != value)
				{
                    OnIdChanging(value);
					SendPropertyChanging();
					_Id = value;
					SendPropertyChanged("Id");
					OnIdChanged();
				}
			}
		}
		
		[Column(Name="Description", UpdateCheck=UpdateCheck.Never, Storage="_Description", DbType="nvarchar(50)")]
		public string Description
		{
			get { return _Description; }
			set
			{
				if (_Description != value)
				{
                    OnDescriptionChanging(value);
					SendPropertyChanging();
					_Description = value;
					SendPropertyChanged("Description");
					OnDescriptionChanged();
				}
			}
		}
		
		[Column(Name="Hardwired", UpdateCheck=UpdateCheck.Never, Storage="_Hardwired", DbType="bit")]
		public bool? Hardwired
		{
			get { return _Hardwired; }
			set
			{
				if (_Hardwired != value)
				{
                    OnHardwiredChanging(value);
					SendPropertyChanging();
					_Hardwired = value;
					SendPropertyChanged("Hardwired");
					OnHardwiredChanged();
				}
			}
		}
		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_ScheduledGift_ScheduledGiftType", Storage="_ScheduledGifts", OtherKey="ScheduledGiftTypeId")]
   		public EntitySet<ScheduledGift> ScheduledGifts
   		{
   		    get { return _ScheduledGifts; }
			set	{ _ScheduledGifts.Assign(value); }
   		}
		
	#endregion
	
	#region Foreign Keys
    	
	#endregion
	
		public event PropertyChangingEventHandler PropertyChanging;
		protected virtual void SendPropertyChanging()
		{
			if (PropertyChanging != null)
				PropertyChanging(this, emptyChangingEventArgs);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
