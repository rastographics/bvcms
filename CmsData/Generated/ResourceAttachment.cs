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
	[Table(Name="dbo.ResourceAttachment")]
	public partial class ResourceAttachment : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _ResourceAttachmentId;
		
		private int _ResourceId;
		
		private string _FilePath;
		
		private int? _FileTypeId;
		
		private string _Name;
		
		private DateTime? _CreationDate;
		
		private DateTime? _UpdateDate;
		
		private int? _DisplayOrder;
		
   		
    	
		private EntityRef<Resource> _Resource;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnResourceAttachmentIdChanging(int value);
		partial void OnResourceAttachmentIdChanged();
		
		partial void OnResourceIdChanging(int value);
		partial void OnResourceIdChanged();
		
		partial void OnFilePathChanging(string value);
		partial void OnFilePathChanged();
		
		partial void OnFileTypeIdChanging(int? value);
		partial void OnFileTypeIdChanged();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
		partial void OnCreationDateChanging(DateTime? value);
		partial void OnCreationDateChanged();
		
		partial void OnUpdateDateChanging(DateTime? value);
		partial void OnUpdateDateChanged();
		
		partial void OnDisplayOrderChanging(int? value);
		partial void OnDisplayOrderChanged();
		
    #endregion
		public ResourceAttachment()
		{
			
			
			this._Resource = default(EntityRef<Resource>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="ResourceAttachmentId", UpdateCheck=UpdateCheck.Never, Storage="_ResourceAttachmentId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ResourceAttachmentId
		{
			get { return this._ResourceAttachmentId; }

			set
			{
				if (this._ResourceAttachmentId != value)
				{
				
                    this.OnResourceAttachmentIdChanging(value);
					this.SendPropertyChanging();
					this._ResourceAttachmentId = value;
					this.SendPropertyChanged("ResourceAttachmentId");
					this.OnResourceAttachmentIdChanged();
				}

			}

		}

		
		[Column(Name="ResourceId", UpdateCheck=UpdateCheck.Never, Storage="_ResourceId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int ResourceId
		{
			get { return this._ResourceId; }

			set
			{
				if (this._ResourceId != value)
				{
				
					if (this._Resource.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnResourceIdChanging(value);
					this.SendPropertyChanging();
					this._ResourceId = value;
					this.SendPropertyChanged("ResourceId");
					this.OnResourceIdChanged();
				}

			}

		}

		
		[Column(Name="FilePath", UpdateCheck=UpdateCheck.Never, Storage="_FilePath", DbType="nvarchar")]
		public string FilePath
		{
			get { return this._FilePath; }

			set
			{
				if (this._FilePath != value)
				{
				
                    this.OnFilePathChanging(value);
					this.SendPropertyChanging();
					this._FilePath = value;
					this.SendPropertyChanged("FilePath");
					this.OnFilePathChanged();
				}

			}

		}

		
		[Column(Name="FileTypeId", UpdateCheck=UpdateCheck.Never, Storage="_FileTypeId", DbType="int")]
		public int? FileTypeId
		{
			get { return this._FileTypeId; }

			set
			{
				if (this._FileTypeId != value)
				{
				
                    this.OnFileTypeIdChanging(value);
					this.SendPropertyChanging();
					this._FileTypeId = value;
					this.SendPropertyChanged("FileTypeId");
					this.OnFileTypeIdChanged();
				}

			}

		}

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(100)")]
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

		
		[Column(Name="CreationDate", UpdateCheck=UpdateCheck.Never, Storage="_CreationDate", DbType="datetime")]
		public DateTime? CreationDate
		{
			get { return this._CreationDate; }

			set
			{
				if (this._CreationDate != value)
				{
				
                    this.OnCreationDateChanging(value);
					this.SendPropertyChanging();
					this._CreationDate = value;
					this.SendPropertyChanged("CreationDate");
					this.OnCreationDateChanged();
				}

			}

		}

		
		[Column(Name="UpdateDate", UpdateCheck=UpdateCheck.Never, Storage="_UpdateDate", DbType="datetime")]
		public DateTime? UpdateDate
		{
			get { return this._UpdateDate; }

			set
			{
				if (this._UpdateDate != value)
				{
				
                    this.OnUpdateDateChanging(value);
					this.SendPropertyChanging();
					this._UpdateDate = value;
					this.SendPropertyChanged("UpdateDate");
					this.OnUpdateDateChanged();
				}

			}

		}

		
		[Column(Name="DisplayOrder", UpdateCheck=UpdateCheck.Never, Storage="_DisplayOrder", DbType="int")]
		public int? DisplayOrder
		{
			get { return this._DisplayOrder; }

			set
			{
				if (this._DisplayOrder != value)
				{
				
                    this.OnDisplayOrderChanging(value);
					this.SendPropertyChanging();
					this._DisplayOrder = value;
					this.SendPropertyChanged("DisplayOrder");
					this.OnDisplayOrderChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_ResourceAttachment_Resource", Storage="_Resource", ThisKey="ResourceId", IsForeignKey=true)]
		public Resource Resource
		{
			get { return this._Resource.Entity; }

			set
			{
				Resource previousValue = this._Resource.Entity;
				if (((previousValue != value) 
							|| (this._Resource.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Resource.Entity = null;
						previousValue.ResourceAttachments.Remove(this);
					}

					this._Resource.Entity = value;
					if (value != null)
					{
						value.ResourceAttachments.Add(this);
						
						this._ResourceId = value.ResourceId;
						
					}

					else
					{
						
						this._ResourceId = default(int);
						
					}

					this.SendPropertyChanged("Resource");
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

