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
	[Table(Name="dbo.TicketingSeats")]
	public partial class TicketingSeat : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _OrderId;
		
		private string _SeatLabel;
		
		private decimal? _Price;
		
		private string _Category;
		
		private int? _CategoryKey;
		
		private string _Section;
		
		private string _Row;
		
		private int? _Seat;
		
		private EntityRef<TicketingOrder> _TicketingOrder;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnOrderIdChanging(int value);
		partial void OnOrderIdChanged();
		
		partial void OnSeatLabelChanging(string value);
		partial void OnSeatLabelChanged();
		
		partial void OnPriceChanging(decimal? value);
		partial void OnPriceChanged();
		
		partial void OnCategoryChanging(string value);
		partial void OnCategoryChanged();
		
		partial void OnCategoryKeyChanging(int? value);
		partial void OnCategoryKeyChanged();
		
		partial void OnSectionChanging(string value);
		partial void OnSectionChanged();
		
		partial void OnRowChanging(string value);
		partial void OnRowChanged();
		
		partial void OnSeatChanging(int? value);
		partial void OnSeatChanged();
		
    #endregion
		public TicketingSeat()
		{
			this._TicketingOrder = default(EntityRef<TicketingOrder>); 
			OnCreated();
		}
		
    #region Columns
		
		[Column(Name="OrderId", UpdateCheck=UpdateCheck.Never, Storage="_OrderId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int OrderId
		{
			get { return this._OrderId; }

			set
			{
				if (this._OrderId != value)
				{
					if (this._TicketingOrder.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOrderIdChanging(value);
					this.SendPropertyChanging();
					this._OrderId = value;
					this.SendPropertyChanged("OrderId");
					this.OnOrderIdChanged();
				}
			}
		}
		
		[Column(Name="SeatLabel", UpdateCheck=UpdateCheck.Never, Storage="_SeatLabel", DbType="varchar(25) NOT NULL", IsPrimaryKey=true)]
		public string SeatLabel
		{
			get { return this._SeatLabel; }

			set
			{
				if (this._SeatLabel != value)
				{
                    this.OnSeatLabelChanging(value);
					this.SendPropertyChanging();
					this._SeatLabel = value;
					this.SendPropertyChanged("SeatLabel");
					this.OnSeatLabelChanged();
				}
			}
		}
		
		[Column(Name="Price", UpdateCheck=UpdateCheck.Never, Storage="_Price", DbType="money")]
		public decimal? Price
		{
			get { return this._Price; }

			set
			{
				if (this._Price != value)
				{
                    this.OnPriceChanging(value);
					this.SendPropertyChanging();
					this._Price = value;
					this.SendPropertyChanged("Price");
					this.OnPriceChanged();
				}
			}
		}
		
		[Column(Name="Category", UpdateCheck=UpdateCheck.Never, Storage="_Category", DbType="varchar(25)")]
		public string Category
		{
			get { return this._Category; }

			set
			{
				if (this._Category != value)
				{
                    this.OnCategoryChanging(value);
					this.SendPropertyChanging();
					this._Category = value;
					this.SendPropertyChanged("Category");
					this.OnCategoryChanged();
				}
			}
		}
		
		[Column(Name="CategoryKey", UpdateCheck=UpdateCheck.Never, Storage="_CategoryKey", DbType="int")]
		public int? CategoryKey
		{
			get { return this._CategoryKey; }

			set
			{
				if (this._CategoryKey != value)
				{
                    this.OnCategoryKeyChanging(value);
					this.SendPropertyChanging();
					this._CategoryKey = value;
					this.SendPropertyChanged("CategoryKey");
					this.OnCategoryKeyChanged();
				}
			}
		}
		
		[Column(Name="Section", UpdateCheck=UpdateCheck.Never, Storage="_Section", DbType="varchar(25)")]
		public string Section
		{
			get { return this._Section; }

			set
			{
				if (this._Section != value)
				{
                    this.OnSectionChanging(value);
					this.SendPropertyChanging();
					this._Section = value;
					this.SendPropertyChanged("Section");
					this.OnSectionChanged();
				}
			}
		}
		
		[Column(Name="Row", UpdateCheck=UpdateCheck.Never, Storage="_Row", DbType="varchar(10)")]
		public string Row
		{
			get { return this._Row; }

			set
			{
				if (this._Row != value)
				{
                    this.OnRowChanging(value);
					this.SendPropertyChanging();
					this._Row = value;
					this.SendPropertyChanged("Row");
					this.OnRowChanged();
				}
			}
		}
		
		[Column(Name="Seat", UpdateCheck=UpdateCheck.Never, Storage="_Seat", DbType="int")]
		public int? Seat
		{
			get { return this._Seat; }

			set
			{
				if (this._Seat != value)
				{
                    this.OnSeatChanging(value);
					this.SendPropertyChanging();
					this._Seat = value;
					this.SendPropertyChanged("Seat");
					this.OnSeatChanged();
				}
			}
		}
		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_TicketingSeats_TicketingOrder", Storage="_TicketingOrder", ThisKey="OrderId", IsForeignKey=true)]
		public TicketingOrder TicketingOrder
		{
			get { return this._TicketingOrder.Entity; }

			set
			{
				TicketingOrder previousValue = this._TicketingOrder.Entity;
				if (((previousValue != value) 
							|| (this._TicketingOrder.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._TicketingOrder.Entity = null;
						previousValue.TicketingSeats.Remove(this);
					}

					this._TicketingOrder.Entity = value;
					if (value != null)
					{
						value.TicketingSeats.Add(this);
						
						this._OrderId = value.OrderId;
					}
					else
					{
						this._OrderId = default(int);
					}

					this.SendPropertyChanged("TicketingOrder");
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
