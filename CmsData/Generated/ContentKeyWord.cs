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
	[Table(Name="dbo.ContentKeyWords")]
	public partial class ContentKeyWord : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Word;
		
   		
    	
		private EntityRef<Content> _Content;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnWordChanging(string value);
		partial void OnWordChanged();
		
    #endregion
		public ContentKeyWord()
		{
			
			
			this._Content = default(EntityRef<Content>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int Id
		{
			get { return this._Id; }

			set
			{
				if (this._Id != value)
				{
				
					if (this._Content.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}

			}

		}

		
		[Column(Name="Word", UpdateCheck=UpdateCheck.Never, Storage="_Word", DbType="nvarchar(50) NOT NULL", IsPrimaryKey=true)]
		public string Word
		{
			get { return this._Word; }

			set
			{
				if (this._Word != value)
				{
				
                    this.OnWordChanging(value);
					this.SendPropertyChanging();
					this._Word = value;
					this.SendPropertyChanged("Word");
					this.OnWordChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_ContentKeyWords_Content", Storage="_Content", ThisKey="Id", IsForeignKey=true)]
		public Content Content
		{
			get { return this._Content.Entity; }

			set
			{
				Content previousValue = this._Content.Entity;
				if (((previousValue != value) 
							|| (this._Content.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Content.Entity = null;
						previousValue.ContentKeyWords.Remove(this);
					}

					this._Content.Entity = value;
					if (value != null)
					{
						value.ContentKeyWords.Add(this);
						
						this._Id = value.Id;
						
					}

					else
					{
						
						this._Id = default(int);
						
					}

					this.SendPropertyChanged("Content");
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

