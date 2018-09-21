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
	[Table(Name="dbo.IpLog2")]
	public partial class IpLog2 : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private string _Ip;
		
		private DateTime? _Tm;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIpChanging(string value);
		partial void OnIpChanged();
		
		partial void OnTmChanging(DateTime? value);
		partial void OnTmChanged();
		
    #endregion
		public IpLog2()
		{
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="ip", UpdateCheck=UpdateCheck.Never, Storage="_Ip", DbType="varchar(50)")]
		public string Ip
		{
			get { return this._Ip; }

			set
			{
				if (this._Ip != value)
				{
				
                    this.OnIpChanging(value);
					this.SendPropertyChanging();
					this._Ip = value;
					this.SendPropertyChanged("Ip");
					this.OnIpChanged();
				}

			}

		}

		
		[Column(Name="tm", UpdateCheck=UpdateCheck.Never, Storage="_Tm", DbType="datetime")]
		public DateTime? Tm
		{
			get { return this._Tm; }

			set
			{
				if (this._Tm != value)
				{
				
                    this.OnTmChanging(value);
					this.SendPropertyChanging();
					this._Tm = value;
					this.SendPropertyChanged("Tm");
					this.OnTmChanged();
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

