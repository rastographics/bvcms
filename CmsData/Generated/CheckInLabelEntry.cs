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
	[Table(Name="dbo.CheckInLabelEntry")]
	public partial class CheckInLabelEntry : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int _LabelID;
		
		private int _TypeID;
		
		private int _Repeat;
		
		private decimal _Offset;
		
		private string _Font;
		
		private int _FontSize;
		
		private int _FieldID;
		
		private string _FieldFormat;
		
		private decimal _StartX;
		
		private decimal _StartY;
		
		private int _AlignX;
		
		private int _AlignY;
		
		private decimal _EndX;
		
		private decimal _EndY;
		
		private int _Width;
		
		private int _Height;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnLabelIDChanging(int value);
		partial void OnLabelIDChanged();
		
		partial void OnTypeIDChanging(int value);
		partial void OnTypeIDChanged();
		
		partial void OnRepeatChanging(int value);
		partial void OnRepeatChanged();
		
		partial void OnOffsetChanging(decimal value);
		partial void OnOffsetChanged();
		
		partial void OnFontChanging(string value);
		partial void OnFontChanged();
		
		partial void OnFontSizeChanging(int value);
		partial void OnFontSizeChanged();
		
		partial void OnFieldIDChanging(int value);
		partial void OnFieldIDChanged();
		
		partial void OnFieldFormatChanging(string value);
		partial void OnFieldFormatChanged();
		
		partial void OnStartXChanging(decimal value);
		partial void OnStartXChanged();
		
		partial void OnStartYChanging(decimal value);
		partial void OnStartYChanged();
		
		partial void OnAlignXChanging(int value);
		partial void OnAlignXChanged();
		
		partial void OnAlignYChanging(int value);
		partial void OnAlignYChanged();
		
		partial void OnEndXChanging(decimal value);
		partial void OnEndXChanged();
		
		partial void OnEndYChanging(decimal value);
		partial void OnEndYChanged();
		
		partial void OnWidthChanging(int value);
		partial void OnWidthChanged();
		
		partial void OnHeightChanging(int value);
		partial void OnHeightChanged();
		
    #endregion
		public CheckInLabelEntry()
		{
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="id", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
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

		
		[Column(Name="labelID", UpdateCheck=UpdateCheck.Never, Storage="_LabelID", DbType="int NOT NULL")]
		public int LabelID
		{
			get { return this._LabelID; }

			set
			{
				if (this._LabelID != value)
				{
				
                    this.OnLabelIDChanging(value);
					this.SendPropertyChanging();
					this._LabelID = value;
					this.SendPropertyChanged("LabelID");
					this.OnLabelIDChanged();
				}

			}

		}

		
		[Column(Name="typeID", UpdateCheck=UpdateCheck.Never, Storage="_TypeID", DbType="int NOT NULL")]
		public int TypeID
		{
			get { return this._TypeID; }

			set
			{
				if (this._TypeID != value)
				{
				
                    this.OnTypeIDChanging(value);
					this.SendPropertyChanging();
					this._TypeID = value;
					this.SendPropertyChanged("TypeID");
					this.OnTypeIDChanged();
				}

			}

		}

		
		[Column(Name="repeat", UpdateCheck=UpdateCheck.Never, Storage="_Repeat", DbType="int NOT NULL")]
		public int Repeat
		{
			get { return this._Repeat; }

			set
			{
				if (this._Repeat != value)
				{
				
                    this.OnRepeatChanging(value);
					this.SendPropertyChanging();
					this._Repeat = value;
					this.SendPropertyChanged("Repeat");
					this.OnRepeatChanged();
				}

			}

		}

		
		[Column(Name="offset", UpdateCheck=UpdateCheck.Never, Storage="_Offset", DbType="decimal NOT NULL")]
		public decimal Offset
		{
			get { return this._Offset; }

			set
			{
				if (this._Offset != value)
				{
				
                    this.OnOffsetChanging(value);
					this.SendPropertyChanging();
					this._Offset = value;
					this.SendPropertyChanged("Offset");
					this.OnOffsetChanged();
				}

			}

		}

		
		[Column(Name="font", UpdateCheck=UpdateCheck.Never, Storage="_Font", DbType="nvarchar(25) NOT NULL")]
		public string Font
		{
			get { return this._Font; }

			set
			{
				if (this._Font != value)
				{
				
                    this.OnFontChanging(value);
					this.SendPropertyChanging();
					this._Font = value;
					this.SendPropertyChanged("Font");
					this.OnFontChanged();
				}

			}

		}

		
		[Column(Name="fontSize", UpdateCheck=UpdateCheck.Never, Storage="_FontSize", DbType="int NOT NULL")]
		public int FontSize
		{
			get { return this._FontSize; }

			set
			{
				if (this._FontSize != value)
				{
				
                    this.OnFontSizeChanging(value);
					this.SendPropertyChanging();
					this._FontSize = value;
					this.SendPropertyChanged("FontSize");
					this.OnFontSizeChanged();
				}

			}

		}

		
		[Column(Name="fieldID", UpdateCheck=UpdateCheck.Never, Storage="_FieldID", DbType="int NOT NULL")]
		public int FieldID
		{
			get { return this._FieldID; }

			set
			{
				if (this._FieldID != value)
				{
				
                    this.OnFieldIDChanging(value);
					this.SendPropertyChanging();
					this._FieldID = value;
					this.SendPropertyChanged("FieldID");
					this.OnFieldIDChanged();
				}

			}

		}

		
		[Column(Name="fieldFormat", UpdateCheck=UpdateCheck.Never, Storage="_FieldFormat", DbType="nvarchar(100) NOT NULL")]
		public string FieldFormat
		{
			get { return this._FieldFormat; }

			set
			{
				if (this._FieldFormat != value)
				{
				
                    this.OnFieldFormatChanging(value);
					this.SendPropertyChanging();
					this._FieldFormat = value;
					this.SendPropertyChanged("FieldFormat");
					this.OnFieldFormatChanged();
				}

			}

		}

		
		[Column(Name="startX", UpdateCheck=UpdateCheck.Never, Storage="_StartX", DbType="decimal NOT NULL")]
		public decimal StartX
		{
			get { return this._StartX; }

			set
			{
				if (this._StartX != value)
				{
				
                    this.OnStartXChanging(value);
					this.SendPropertyChanging();
					this._StartX = value;
					this.SendPropertyChanged("StartX");
					this.OnStartXChanged();
				}

			}

		}

		
		[Column(Name="startY", UpdateCheck=UpdateCheck.Never, Storage="_StartY", DbType="decimal NOT NULL")]
		public decimal StartY
		{
			get { return this._StartY; }

			set
			{
				if (this._StartY != value)
				{
				
                    this.OnStartYChanging(value);
					this.SendPropertyChanging();
					this._StartY = value;
					this.SendPropertyChanged("StartY");
					this.OnStartYChanged();
				}

			}

		}

		
		[Column(Name="alignX", UpdateCheck=UpdateCheck.Never, Storage="_AlignX", DbType="int NOT NULL")]
		public int AlignX
		{
			get { return this._AlignX; }

			set
			{
				if (this._AlignX != value)
				{
				
                    this.OnAlignXChanging(value);
					this.SendPropertyChanging();
					this._AlignX = value;
					this.SendPropertyChanged("AlignX");
					this.OnAlignXChanged();
				}

			}

		}

		
		[Column(Name="alignY", UpdateCheck=UpdateCheck.Never, Storage="_AlignY", DbType="int NOT NULL")]
		public int AlignY
		{
			get { return this._AlignY; }

			set
			{
				if (this._AlignY != value)
				{
				
                    this.OnAlignYChanging(value);
					this.SendPropertyChanging();
					this._AlignY = value;
					this.SendPropertyChanged("AlignY");
					this.OnAlignYChanged();
				}

			}

		}

		
		[Column(Name="endX", UpdateCheck=UpdateCheck.Never, Storage="_EndX", DbType="decimal NOT NULL")]
		public decimal EndX
		{
			get { return this._EndX; }

			set
			{
				if (this._EndX != value)
				{
				
                    this.OnEndXChanging(value);
					this.SendPropertyChanging();
					this._EndX = value;
					this.SendPropertyChanged("EndX");
					this.OnEndXChanged();
				}

			}

		}

		
		[Column(Name="endY", UpdateCheck=UpdateCheck.Never, Storage="_EndY", DbType="decimal NOT NULL")]
		public decimal EndY
		{
			get { return this._EndY; }

			set
			{
				if (this._EndY != value)
				{
				
                    this.OnEndYChanging(value);
					this.SendPropertyChanging();
					this._EndY = value;
					this.SendPropertyChanged("EndY");
					this.OnEndYChanged();
				}

			}

		}

		
		[Column(Name="width", UpdateCheck=UpdateCheck.Never, Storage="_Width", DbType="int NOT NULL")]
		public int Width
		{
			get { return this._Width; }

			set
			{
				if (this._Width != value)
				{
				
                    this.OnWidthChanging(value);
					this.SendPropertyChanging();
					this._Width = value;
					this.SendPropertyChanged("Width");
					this.OnWidthChanged();
				}

			}

		}

		
		[Column(Name="height", UpdateCheck=UpdateCheck.Never, Storage="_Height", DbType="int NOT NULL")]
		public int Height
		{
			get { return this._Height; }

			set
			{
				if (this._Height != value)
				{
				
                    this.OnHeightChanging(value);
					this.SendPropertyChanging();
					this._Height = value;
					this.SendPropertyChanged("Height");
					this.OnHeightChanged();
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

