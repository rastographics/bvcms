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
	[Table(Name="dbo.QueryAnalysis")]
	public partial class QueryAnalysi : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private Guid _Id;
		
		private int? _Seconds;
		
		private int? _OriginalCount;
		
		private int? _ParsedCount;
		
		private string _Message;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(Guid value);
		partial void OnIdChanged();
		
		partial void OnSecondsChanging(int? value);
		partial void OnSecondsChanged();
		
		partial void OnOriginalCountChanging(int? value);
		partial void OnOriginalCountChanged();
		
		partial void OnParsedCountChanging(int? value);
		partial void OnParsedCountChanged();
		
		partial void OnMessageChanging(string value);
		partial void OnMessageChanged();
		
    #endregion
		public QueryAnalysi()
		{
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", DbType="uniqueidentifier NOT NULL")]
		public Guid Id
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

		
		[Column(Name="Seconds", UpdateCheck=UpdateCheck.Never, Storage="_Seconds", DbType="int")]
		public int? Seconds
		{
			get { return this._Seconds; }

			set
			{
				if (this._Seconds != value)
				{
				
                    this.OnSecondsChanging(value);
					this.SendPropertyChanging();
					this._Seconds = value;
					this.SendPropertyChanged("Seconds");
					this.OnSecondsChanged();
				}

			}

		}

		
		[Column(Name="OriginalCount", UpdateCheck=UpdateCheck.Never, Storage="_OriginalCount", DbType="int")]
		public int? OriginalCount
		{
			get { return this._OriginalCount; }

			set
			{
				if (this._OriginalCount != value)
				{
				
                    this.OnOriginalCountChanging(value);
					this.SendPropertyChanging();
					this._OriginalCount = value;
					this.SendPropertyChanged("OriginalCount");
					this.OnOriginalCountChanged();
				}

			}

		}

		
		[Column(Name="ParsedCount", UpdateCheck=UpdateCheck.Never, Storage="_ParsedCount", DbType="int")]
		public int? ParsedCount
		{
			get { return this._ParsedCount; }

			set
			{
				if (this._ParsedCount != value)
				{
				
                    this.OnParsedCountChanging(value);
					this.SendPropertyChanging();
					this._ParsedCount = value;
					this.SendPropertyChanged("ParsedCount");
					this.OnParsedCountChanged();
				}

			}

		}

		
		[Column(Name="Message", UpdateCheck=UpdateCheck.Never, Storage="_Message", DbType="varchar")]
		public string Message
		{
			get { return this._Message; }

			set
			{
				if (this._Message != value)
				{
				
                    this.OnMessageChanging(value);
					this.SendPropertyChanging();
					this._Message = value;
					this.SendPropertyChanged("Message");
					this.OnMessageChanged();
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

