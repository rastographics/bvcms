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
	[Table(Name="dbo.Picture")]
	public partial class Picture : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _PictureId;
		
		private DateTime? _CreatedDate;
		
		private string _CreatedBy;
		
		private int? _LargeId;
		
		private int? _MediumId;
		
		private int? _SmallId;
		
		private int? _ThumbId;
		
		private int? _X;
		
		private int? _Y;
		
   		
   		private EntitySet<Family> _Families;
		
   		private EntitySet<Person> _People;
		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnPictureIdChanging(int value);
		partial void OnPictureIdChanged();
		
		partial void OnCreatedDateChanging(DateTime? value);
		partial void OnCreatedDateChanged();
		
		partial void OnCreatedByChanging(string value);
		partial void OnCreatedByChanged();
		
		partial void OnLargeIdChanging(int? value);
		partial void OnLargeIdChanged();
		
		partial void OnMediumIdChanging(int? value);
		partial void OnMediumIdChanged();
		
		partial void OnSmallIdChanging(int? value);
		partial void OnSmallIdChanged();
		
		partial void OnThumbIdChanging(int? value);
		partial void OnThumbIdChanged();
		
		partial void OnXChanging(int? value);
		partial void OnXChanged();
		
		partial void OnYChanging(int? value);
		partial void OnYChanged();
		
    #endregion
		public Picture()
		{
			
			this._Families = new EntitySet<Family>(new Action< Family>(this.attach_Families), new Action< Family>(this.detach_Families)); 
			
			this._People = new EntitySet<Person>(new Action< Person>(this.attach_People), new Action< Person>(this.detach_People)); 
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="PictureId", UpdateCheck=UpdateCheck.Never, Storage="_PictureId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int PictureId
		{
			get { return this._PictureId; }

			set
			{
				if (this._PictureId != value)
				{
				
                    this.OnPictureIdChanging(value);
					this.SendPropertyChanging();
					this._PictureId = value;
					this.SendPropertyChanged("PictureId");
					this.OnPictureIdChanged();
				}

			}

		}

		
		[Column(Name="CreatedDate", UpdateCheck=UpdateCheck.Never, Storage="_CreatedDate", DbType="datetime")]
		public DateTime? CreatedDate
		{
			get { return this._CreatedDate; }

			set
			{
				if (this._CreatedDate != value)
				{
				
                    this.OnCreatedDateChanging(value);
					this.SendPropertyChanging();
					this._CreatedDate = value;
					this.SendPropertyChanged("CreatedDate");
					this.OnCreatedDateChanged();
				}

			}

		}

		
		[Column(Name="CreatedBy", UpdateCheck=UpdateCheck.Never, Storage="_CreatedBy", DbType="nvarchar(50)")]
		public string CreatedBy
		{
			get { return this._CreatedBy; }

			set
			{
				if (this._CreatedBy != value)
				{
				
                    this.OnCreatedByChanging(value);
					this.SendPropertyChanging();
					this._CreatedBy = value;
					this.SendPropertyChanged("CreatedBy");
					this.OnCreatedByChanged();
				}

			}

		}

		
		[Column(Name="LargeId", UpdateCheck=UpdateCheck.Never, Storage="_LargeId", DbType="int")]
		public int? LargeId
		{
			get { return this._LargeId; }

			set
			{
				if (this._LargeId != value)
				{
				
                    this.OnLargeIdChanging(value);
					this.SendPropertyChanging();
					this._LargeId = value;
					this.SendPropertyChanged("LargeId");
					this.OnLargeIdChanged();
				}

			}

		}

		
		[Column(Name="MediumId", UpdateCheck=UpdateCheck.Never, Storage="_MediumId", DbType="int")]
		public int? MediumId
		{
			get { return this._MediumId; }

			set
			{
				if (this._MediumId != value)
				{
				
                    this.OnMediumIdChanging(value);
					this.SendPropertyChanging();
					this._MediumId = value;
					this.SendPropertyChanged("MediumId");
					this.OnMediumIdChanged();
				}

			}

		}

		
		[Column(Name="SmallId", UpdateCheck=UpdateCheck.Never, Storage="_SmallId", DbType="int")]
		public int? SmallId
		{
			get { return this._SmallId; }

			set
			{
				if (this._SmallId != value)
				{
				
                    this.OnSmallIdChanging(value);
					this.SendPropertyChanging();
					this._SmallId = value;
					this.SendPropertyChanged("SmallId");
					this.OnSmallIdChanged();
				}

			}

		}

		
		[Column(Name="ThumbId", UpdateCheck=UpdateCheck.Never, Storage="_ThumbId", DbType="int")]
		public int? ThumbId
		{
			get { return this._ThumbId; }

			set
			{
				if (this._ThumbId != value)
				{
				
                    this.OnThumbIdChanging(value);
					this.SendPropertyChanging();
					this._ThumbId = value;
					this.SendPropertyChanged("ThumbId");
					this.OnThumbIdChanged();
				}

			}

		}

		
		[Column(Name="X", UpdateCheck=UpdateCheck.Never, Storage="_X", DbType="int")]
		public int? X
		{
			get { return this._X; }

			set
			{
				if (this._X != value)
				{
				
                    this.OnXChanging(value);
					this.SendPropertyChanging();
					this._X = value;
					this.SendPropertyChanged("X");
					this.OnXChanged();
				}

			}

		}

		
		[Column(Name="Y", UpdateCheck=UpdateCheck.Never, Storage="_Y", DbType="int")]
		public int? Y
		{
			get { return this._Y; }

			set
			{
				if (this._Y != value)
				{
				
                    this.OnYChanging(value);
					this.SendPropertyChanging();
					this._Y = value;
					this.SendPropertyChanged("Y");
					this.OnYChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_Families_Picture", Storage="_Families", OtherKey="PictureId")]
   		public EntitySet<Family> Families
   		{
   		    get { return this._Families; }

			set	{ this._Families.Assign(value); }

   		}

		
   		[Association(Name="FK_PEOPLE_TBL_Picture", Storage="_People", OtherKey="PictureId")]
   		public EntitySet<Person> People
   		{
   		    get { return this._People; }

			set	{ this._People.Assign(value); }

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

   		
		private void attach_Families(Family entity)
		{
			this.SendPropertyChanging();
			entity.Picture = this;
		}

		private void detach_Families(Family entity)
		{
			this.SendPropertyChanging();
			entity.Picture = null;
		}

		
		private void attach_People(Person entity)
		{
			this.SendPropertyChanging();
			entity.Picture = this;
		}

		private void detach_People(Person entity)
		{
			this.SendPropertyChanging();
			entity.Picture = null;
		}

		
	}

}

