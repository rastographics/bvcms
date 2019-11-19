using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.CheckInLabelEntry")]
    public partial class CheckInLabelEntry : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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

        [Column(Name = "id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id
        {
            get => _Id;

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

        [Column(Name = "labelID", UpdateCheck = UpdateCheck.Never, Storage = "_LabelID", DbType = "int NOT NULL")]
        public int LabelID
        {
            get => _LabelID;

            set
            {
                if (_LabelID != value)
                {
                    OnLabelIDChanging(value);
                    SendPropertyChanging();
                    _LabelID = value;
                    SendPropertyChanged("LabelID");
                    OnLabelIDChanged();
                }
            }
        }

        [Column(Name = "typeID", UpdateCheck = UpdateCheck.Never, Storage = "_TypeID", DbType = "int NOT NULL")]
        public int TypeID
        {
            get => _TypeID;

            set
            {
                if (_TypeID != value)
                {
                    OnTypeIDChanging(value);
                    SendPropertyChanging();
                    _TypeID = value;
                    SendPropertyChanged("TypeID");
                    OnTypeIDChanged();
                }
            }
        }

        [Column(Name = "repeat", UpdateCheck = UpdateCheck.Never, Storage = "_Repeat", DbType = "int NOT NULL")]
        public int Repeat
        {
            get => _Repeat;

            set
            {
                if (_Repeat != value)
                {
                    OnRepeatChanging(value);
                    SendPropertyChanging();
                    _Repeat = value;
                    SendPropertyChanged("Repeat");
                    OnRepeatChanged();
                }
            }
        }

        [Column(Name = "offset", UpdateCheck = UpdateCheck.Never, Storage = "_Offset", DbType = "decimal NOT NULL")]
        public decimal Offset
        {
            get => _Offset;

            set
            {
                if (_Offset != value)
                {
                    OnOffsetChanging(value);
                    SendPropertyChanging();
                    _Offset = value;
                    SendPropertyChanged("Offset");
                    OnOffsetChanged();
                }
            }
        }

        [Column(Name = "font", UpdateCheck = UpdateCheck.Never, Storage = "_Font", DbType = "nvarchar(25) NOT NULL")]
        public string Font
        {
            get => _Font;

            set
            {
                if (_Font != value)
                {
                    OnFontChanging(value);
                    SendPropertyChanging();
                    _Font = value;
                    SendPropertyChanged("Font");
                    OnFontChanged();
                }
            }
        }

        [Column(Name = "fontSize", UpdateCheck = UpdateCheck.Never, Storage = "_FontSize", DbType = "int NOT NULL")]
        public int FontSize
        {
            get => _FontSize;

            set
            {
                if (_FontSize != value)
                {
                    OnFontSizeChanging(value);
                    SendPropertyChanging();
                    _FontSize = value;
                    SendPropertyChanged("FontSize");
                    OnFontSizeChanged();
                }
            }
        }

        [Column(Name = "fieldID", UpdateCheck = UpdateCheck.Never, Storage = "_FieldID", DbType = "int NOT NULL")]
        public int FieldID
        {
            get => _FieldID;

            set
            {
                if (_FieldID != value)
                {
                    OnFieldIDChanging(value);
                    SendPropertyChanging();
                    _FieldID = value;
                    SendPropertyChanged("FieldID");
                    OnFieldIDChanged();
                }
            }
        }

        [Column(Name = "fieldFormat", UpdateCheck = UpdateCheck.Never, Storage = "_FieldFormat", DbType = "nvarchar(100) NOT NULL")]
        public string FieldFormat
        {
            get => _FieldFormat;

            set
            {
                if (_FieldFormat != value)
                {
                    OnFieldFormatChanging(value);
                    SendPropertyChanging();
                    _FieldFormat = value;
                    SendPropertyChanged("FieldFormat");
                    OnFieldFormatChanged();
                }
            }
        }

        [Column(Name = "startX", UpdateCheck = UpdateCheck.Never, Storage = "_StartX", DbType = "decimal NOT NULL")]
        public decimal StartX
        {
            get => _StartX;

            set
            {
                if (_StartX != value)
                {
                    OnStartXChanging(value);
                    SendPropertyChanging();
                    _StartX = value;
                    SendPropertyChanged("StartX");
                    OnStartXChanged();
                }
            }
        }

        [Column(Name = "startY", UpdateCheck = UpdateCheck.Never, Storage = "_StartY", DbType = "decimal NOT NULL")]
        public decimal StartY
        {
            get => _StartY;

            set
            {
                if (_StartY != value)
                {
                    OnStartYChanging(value);
                    SendPropertyChanging();
                    _StartY = value;
                    SendPropertyChanged("StartY");
                    OnStartYChanged();
                }
            }
        }

        [Column(Name = "alignX", UpdateCheck = UpdateCheck.Never, Storage = "_AlignX", DbType = "int NOT NULL")]
        public int AlignX
        {
            get => _AlignX;

            set
            {
                if (_AlignX != value)
                {
                    OnAlignXChanging(value);
                    SendPropertyChanging();
                    _AlignX = value;
                    SendPropertyChanged("AlignX");
                    OnAlignXChanged();
                }
            }
        }

        [Column(Name = "alignY", UpdateCheck = UpdateCheck.Never, Storage = "_AlignY", DbType = "int NOT NULL")]
        public int AlignY
        {
            get => _AlignY;

            set
            {
                if (_AlignY != value)
                {
                    OnAlignYChanging(value);
                    SendPropertyChanging();
                    _AlignY = value;
                    SendPropertyChanged("AlignY");
                    OnAlignYChanged();
                }
            }
        }

        [Column(Name = "endX", UpdateCheck = UpdateCheck.Never, Storage = "_EndX", DbType = "decimal NOT NULL")]
        public decimal EndX
        {
            get => _EndX;

            set
            {
                if (_EndX != value)
                {
                    OnEndXChanging(value);
                    SendPropertyChanging();
                    _EndX = value;
                    SendPropertyChanged("EndX");
                    OnEndXChanged();
                }
            }
        }

        [Column(Name = "endY", UpdateCheck = UpdateCheck.Never, Storage = "_EndY", DbType = "decimal NOT NULL")]
        public decimal EndY
        {
            get => _EndY;

            set
            {
                if (_EndY != value)
                {
                    OnEndYChanging(value);
                    SendPropertyChanging();
                    _EndY = value;
                    SendPropertyChanged("EndY");
                    OnEndYChanged();
                }
            }
        }

        [Column(Name = "width", UpdateCheck = UpdateCheck.Never, Storage = "_Width", DbType = "int NOT NULL")]
        public int Width
        {
            get => _Width;

            set
            {
                if (_Width != value)
                {
                    OnWidthChanging(value);
                    SendPropertyChanging();
                    _Width = value;
                    SendPropertyChanged("Width");
                    OnWidthChanged();
                }
            }
        }

        [Column(Name = "height", UpdateCheck = UpdateCheck.Never, Storage = "_Height", DbType = "int NOT NULL")]
        public int Height
        {
            get => _Height;

            set
            {
                if (_Height != value)
                {
                    OnHeightChanging(value);
                    SendPropertyChanging();
                    _Height = value;
                    SendPropertyChanged("Height");
                    OnHeightChanged();
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
            if ((PropertyChanging != null))
            {
                PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
