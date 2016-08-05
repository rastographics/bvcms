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
	[Table(Name="dbo.IpWarmup")]
	public partial class IpWarmup : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private DateTime? _Epoch;
		
		private int? _Sentsince;
		
		private DateTime? _Since;
		
		private int? _Totalsent;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnEpochChanging(DateTime? value);
		partial void OnEpochChanged();
		
		partial void OnSentsinceChanging(int? value);
		partial void OnSentsinceChanged();
		
		partial void OnSinceChanging(DateTime? value);
		partial void OnSinceChanged();
		
		partial void OnTotalsentChanging(int? value);
		partial void OnTotalsentChanged();
		
    #endregion
		public IpWarmup()
		{
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="epoch", UpdateCheck=UpdateCheck.Never, Storage="_Epoch", DbType="datetime")]
		public DateTime? Epoch
		{
			get { return this._Epoch; }

			set
			{
				if (this._Epoch != value)
				{
				
                    this.OnEpochChanging(value);
					this.SendPropertyChanging();
					this._Epoch = value;
					this.SendPropertyChanged("Epoch");
					this.OnEpochChanged();
				}

			}

		}

		
		[Column(Name="sentsince", UpdateCheck=UpdateCheck.Never, Storage="_Sentsince", DbType="int")]
		public int? Sentsince
		{
			get { return this._Sentsince; }

			set
			{
				if (this._Sentsince != value)
				{
				
                    this.OnSentsinceChanging(value);
					this.SendPropertyChanging();
					this._Sentsince = value;
					this.SendPropertyChanged("Sentsince");
					this.OnSentsinceChanged();
				}

			}

		}

		
		[Column(Name="since", UpdateCheck=UpdateCheck.Never, Storage="_Since", DbType="datetime")]
		public DateTime? Since
		{
			get { return this._Since; }

			set
			{
				if (this._Since != value)
				{
				
                    this.OnSinceChanging(value);
					this.SendPropertyChanging();
					this._Since = value;
					this.SendPropertyChanged("Since");
					this.OnSinceChanged();
				}

			}

		}

		
		[Column(Name="totalsent", UpdateCheck=UpdateCheck.Never, Storage="_Totalsent", DbType="int")]
		public int? Totalsent
		{
			get { return this._Totalsent; }

			set
			{
				if (this._Totalsent != value)
				{
				
                    this.OnTotalsentChanging(value);
					this.SendPropertyChanging();
					this._Totalsent = value;
					this.SendPropertyChanged("Totalsent");
					this.OnTotalsentChanged();
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

