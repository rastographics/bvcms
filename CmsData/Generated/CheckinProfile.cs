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
	[Table(Name="dbo.CheckinProfiles")]
	public partial class CheckinProfile : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _CheckinProfileId;
		
		private string _Name;
		
   		
   		private EntitySet<CheckinProfileSetting> _CheckinProfileSettings;
		
    	
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
		public CheckinProfile()
		{
			
			this._CheckinProfileSettings = new EntitySet<CheckinProfileSetting>(new Action< CheckinProfileSetting>(this.attach_CheckinProfileSettings), new Action< CheckinProfileSetting>(this.detach_CheckinProfileSettings)); 
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="CheckinProfileId", UpdateCheck=UpdateCheck.Never, Storage="_CheckinProfileId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int CheckinProfileId
		{
			get { return this._CheckinProfileId; }

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

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(100) NOT NULL")]
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

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK__CheckinPr__Check__37DB1BFD", Storage="_CheckinProfileSettings", OtherKey="CheckinProfileId")]
   		public EntitySet<CheckinProfileSetting> CheckinProfileSettings
   		{
   		    get { return this._CheckinProfileSettings; }

			set	{ this._CheckinProfileSettings.Assign(value); }

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

   		
		private void attach_CheckinProfileSettings(CheckinProfileSetting entity)
		{
			this.SendPropertyChanging();
			entity.CheckinProfile = this;
		}

		private void detach_CheckinProfileSettings(CheckinProfileSetting entity)
		{
			this.SendPropertyChanging();
			entity.CheckinProfile = null;
		}

		
	}

}

