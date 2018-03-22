using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using DbmlBuilder.Utilities;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace DbmlBuilder.TableSchema
{
    [Serializable]
    public class ManyToManyDetailsCollection : List<ManyToManyDetails>
    {
    }

    [Serializable]
    public class ManyToManyDetails
    {
        private string linksToTable;
        public string LinksToTable
        {
            get { return linksToTable; }
            set { linksToTable = value; }
        }
        private string linksToColum;
        public string LinksToColumn
        {
            get { return linksToColum; }
            set { linksToColum = value; }
        }
        private string mapTableName;
        public string MapTableName
        {
            get { return mapTableName; }
            set { mapTableName = value; }
        }
    }

    [Serializable]
    public class ManyToManyRelationship : AbstractTableSchema
    {
        public ManyToManyRelationship(string tableName)
        {
            mapTableName = tableName;
            Name = mapTableName;
            ClassName = TransformClassName(Name, false, TableType);
            ClassNamePlural = TransformClassName(Name, false, TableType);
            DisplayName = Utility.ParseCamelToProper(ClassName);
        }
        private readonly string mapTableName;
        public string MapTableName
        {
            get { return mapTableName; }
            //set { mapTableName = value; }
        }
        private string mapTableLocalTableKeyColumn;
        public string MapTableLocalTableKeyColumn
        {
            get { return mapTableLocalTableKeyColumn; }
            set { mapTableLocalTableKeyColumn = value; }
        }
        private string mapTableForeignTableKeyColumn;
        public string MapTableForeignTableKeyColumn
        {
            get { return mapTableForeignTableKeyColumn; }
            set { mapTableForeignTableKeyColumn = value; }
        }
        private string foreignPrimaryKey;
        public string ForeignPrimaryKey
        {
            get { return foreignPrimaryKey; }
            set { foreignPrimaryKey = value; }
        }
        private string foreignTableName;
        public string ForeignTableName
        {
            get { return foreignTableName; }
            set
            {
                foreignTableName = value;
            }
        }
    }

    [Serializable]
    public class ManyToManyRelationshipCollection : List<ManyToManyRelationship>
    {
    }

    [Serializable]
    public abstract class AbstractTableSchema
    {
        private string _tableName;
        public string TableName
        {
            get { return _tableName; }
            set
            {
                _tableName = value;
                className = TransformClassName(Name, true, tableType);
                classNamePlural = TransformClassName(Name, false, tableType);
                displayName = Utility.ParseCamelToProper(ClassName);
            }
        }
        private TableType tableType;
        public TableType TableType
        {
            get { return tableType; }
            set { tableType = value; }
        }
        public string Name
        {
            get { return _tableName; }
            set { TableName = value; }
        }
        private string className;
        public string ClassName
        {
            get { return className; }
            protected set { className = value; }
        }
        private string classNamePlural;
        public string ClassNamePlural
        {
            get { return classNamePlural; }
            protected set { classNamePlural = value; }
        }
        private string propertyName;
        public string PropertyName
        {
            get { return propertyName; }
            protected set { propertyName = value; }
        }
        private string displayName;
        public string DisplayName
        {
            get { return displayName; }
            protected set { displayName = value; }
        }
        private string schemaName;
        public string SchemaName
        {
            get { return schemaName; }
            set { schemaName = value; }
        }
        public static string TransformClassName(string name, bool isPlural, TableType tableType)
        {
            if (String.IsNullOrEmpty(name))
                return string.Empty;

            string newName = name;
            if (tableType == TableType.Table)
                newName = Utility.StripText(newName, "_TBL$,_V$");

            newName = Utility.GetProperName(newName);
            newName = Utility.IsStringNumeric(newName) ? "_" + newName : newName;
            newName = Utility.StripNonAlphaNumeric(newName);
            newName = newName.Trim();

            if (isPlural)
                newName = Utility.PluralToSingular(newName);
            else
                newName = Utility.SingularToPlural(newName);

            return Utility.KeyWordCheck(newName, String.Empty);
        }
    }
    public class Relationship
    {
        public Relationship()
        {
        }
        public string Name;
        public string ForeignKey;
        public string ClassNameOne;
        public string PropertyNameOne;
        public string PrimaryKey;
        public string ClassNameMany;
        public string PropertyNameMany;
        public class KeyPair
        {
            public string PrimaryKey;
            public string ForeignKey;
            public string vartype;
        }
        public List<KeyPair> KeyPairs = new List<KeyPair>();
    }

    public class ForeignKeyTableCollection : List<Relationship>
    {
    }

    public class ForeignKeyCollection : List<Relationship>
    {
    }

    [Serializable]
    public class TableCollection : List<Table>
    {
        public Table this[string tableName]
        {
            get
            {
                Table result = null;
                foreach (Table tbl in this)
                {
                    if (Utility.IsMatch(tbl.Name, tableName))
                    {
                        result = tbl;
                        break;
                    }
                }
                return result;
            }
            set
            {
                int index = 0;
                foreach (Table tbl in this)
                {
                    if (Utility.IsMatch(tbl.Name, tableName))
                    {
                        this[index] = value;
                        break;
                    }
                    index++;
                }
            }
        }
    }


    [Serializable]
    public class Table : AbstractTableSchema
    {
        public Table()
        {
        }

        public Table(string tableName, TableType tblType)
        {
            TableType = tblType;
            Name = tableName;
        }

        private ManyToManyRelationshipCollection manyToManys = new ManyToManyRelationshipCollection();
        public ManyToManyRelationshipCollection ManyToManys
        {
            get { return manyToManys; }
            set { manyToManys = value; }
        }

        private ForeignKeyTableCollection _foreignKeyTables = new ForeignKeyTableCollection();
        public ForeignKeyTableCollection ForeignKeyTables
        {
            get { return _foreignKeyTables; }
            set { _foreignKeyTables = value; }
        }

        private ForeignKeyCollection _foreignKeys = new ForeignKeyCollection();
        public ForeignKeyCollection ForeignKeys
        {
            get { return _foreignKeys; }
            set { _foreignKeys = value; }
        }

        public bool HasForeignKeys()
        {
            return ForeignKeys.Count > 0;
        }

        private bool _hasManyToMany = false;
        public bool HasManyToMany
        {
            get { return _hasManyToMany; }
            set { _hasManyToMany = value; }
        }

        private TableColumnCollection columns;
        public TableColumnCollection Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        public TableColumn GetColumn(string columnName)
        {
            TableColumn col = null;
            foreach (TableColumn column in Columns)
            {
                if (Utility.IsMatch(column.ColumnName.Trim(), columnName.Trim()))
                {
                    col = column;
                    break;
                }
            }
            return col;
        }
        public TableColumn PrimaryKey
        {
            get { return Columns.First(c => c.IsPrimaryKey); }
        }
    }

    [Serializable]
    public class TableColumnCollection : List<TableColumn>
    {
        #region Collection Methods

        public bool Contains(string columnName)
        {
            bool bOut = false;
            foreach (TableColumn col in this)
            {
                if (Utility.IsMatch(col.ColumnName, columnName))
                {
                    bOut = true;
                    break;
                }
            }
            return bOut;
        }

        public void Add(Table tbl, string name, DbType dbType, bool isNullable, bool isPrimaryKey, bool isForeignKey)
        {
            TableColumn col = new TableColumn(tbl);
            col.IsPrimaryKey = isPrimaryKey;
            col.IsForeignKey = isForeignKey;
            col.IsNullable = isNullable;
            col.DataType = dbType;
            col.ColumnName = name;

            if (!Contains(name))
            {
                Add(col);
            }
        }

        public void Add(Table tbl, string name, DbType dbType, bool isNullable)
        {
            Add(tbl, name, dbType, isNullable, false, false);
        }

        #endregion

        public TableColumn GetColumn(string columnName)
        {
            TableColumn coll = null;
            foreach (TableColumn child in this)
            {
                if (Utility.IsMatch(child.ColumnName, columnName))
                {
                    coll = child;
                    break;
                }
            }
            return coll;
        }

    }

    [Serializable]
    public class TableColumn
    {
        public TableColumn(Table tableSchema)
        {
            table = tableSchema;
        }

        public string FullDbType
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (AutoIncrement)
                    sb.Append(", AutoSync=AutoSync.OnInsert");
                sb.Append(", DbType=\"" + NativeDataType);
                if (!IsNullable)
                    sb.Append(" NOT NULL");
                if (AutoIncrement)
                    sb.Append(" IDENTITY");
                sb.Append("\"");
                if (IsPrimaryKey)
                    sb.Append(", IsPrimaryKey=true");
                if (AutoIncrement || IsReadOnly)
                    sb.Append(", IsDbGenerated=true");
                return sb.ToString();
            }
        }

        private string defaultSetting;

        public string DefaultSetting
        {
            get { return defaultSetting; }
            set { defaultSetting = value; }
        }

        private string _VarType;
        public string VarType
        {
            get
            {
                if (string.IsNullOrEmpty(_VarType))
                    _VarType = Utility.GetVariableType(DataType, IsNullable);
                return _VarType;
            }
        }
        private readonly Table table;
        public Table Table
        {
            get { return table; }
        }

        private bool isForeignKey;
        public bool IsForeignKey
        {
            get { return isForeignKey; }
            set { isForeignKey = value; }
        }

        private string pointsToTableName;
        public string PointsToTableName
        {
            get { return pointsToTableName; }
            set { pointsToTableName = value; }
        }

        private string schemaName;
        public string SchemaName
        {
            get { return schemaName; }
            set { schemaName = value; }
        }

        private bool isPrimaryKey;
        public bool IsPrimaryKey
        {
            get { return isPrimaryKey; }
            set { isPrimaryKey = value; }
        }

        private bool isNullable;
        public bool IsNullable
        {
            get { return isNullable; }
            set { isNullable = value; }
        }

        private bool isReadOnly;
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set { isReadOnly = value; }
        }

        private string _NativeDataType;
        public string NativeDataType
        {
            get { return _NativeDataType; }
            set { _NativeDataType = value; }
        }
        private DbType dbType;
        public DbType DataType
        {
            get { return dbType; }
            set { dbType = value; }
        }

        private int maxLength;
        public int MaxLength
        {
            get { return maxLength; }
            set { maxLength = value; }
        }

        private string columnName;
        public string ColumnName
        {
            get { return columnName; }
            set
            {
                columnName = value;
                string transformColumnName = columnName;

                name = TransformPropertyName(transformColumnName, Table.ClassName);
                displayName = TransformPropertyName(transformColumnName, Table.ClassName);

                displayName = Utility.ParseCamelToProper(displayName);
                if ((!Regex.IsMatch(displayName, @"^[A-Z]+$")) && (IsPrimaryKey || IsForeignKey) && displayName.Length > 1)
                {
                    string strEnd = displayName.Substring(displayName.Length - 2, 2);
                    if (Utility.IsMatch(strEnd, "id") && strEnd[0].ToString() == "I")
                        displayName = displayName.Substring(0, displayName.Length - 2);
                }
                parameterName = Utility.PrefixParameter(columnName);
                argumentName = "var" + name;
            }
        }

        private string parameterName;
        public string ParameterName
        {
            get { return parameterName; }
        }

        private string name;
        public string Name
        {
            get { return name; }
        }

        private string displayName;
        public string DisplayName
        {
            get { return displayName; }
        }
        private string _PropertyNameOne;
        public string PropertyNameOne
        {
            get { return _PropertyNameOne; }
            set { _PropertyNameOne = value; }
        }
        private string argumentName;
        public string ArgumentName
        {
            get { return argumentName; }
        }

        private bool autoIncrement;
        public bool AutoIncrement
        {
            get { return autoIncrement; }
            set { autoIncrement = value; }
        }
        public bool IsComputed { get; set; }

        private int numberScale;
        public int NumberScale
        {
            get { return numberScale; }
            set { numberScale = value; }
        }

        private int numberPrecision;
        public int NumberPrecision
        {
            get { return numberPrecision; }
            set { numberPrecision = value; }
        }

        public bool IsNumeric
        {
            get
            {
                return DataType == DbType.Currency ||
                       DataType == DbType.Decimal ||
                       DataType == DbType.Double ||
                       DataType == DbType.Int16 ||
                       DataType == DbType.Int32 ||
                       DataType == DbType.Int64 ||
                       DataType == DbType.Single ||
                       DataType == DbType.UInt16 ||
                       DataType == DbType.UInt32 ||
                       DataType == DbType.UInt64 ||
                       DataType == DbType.VarNumeric;
            }
        }

        public bool IsDateTime
        {
            get
            {
                return DataType == DbType.DateTime ||
                       DataType == DbType.Time ||
                       DataType == DbType.Date;
            }
        }

        public bool IsString
        {
            get
            {
                return DataType == DbType.AnsiString ||
                       DataType == DbType.AnsiStringFixedLength ||
                       DataType == DbType.String ||
                       DataType == DbType.StringFixedLength;
            }
        }

        private string updateCheck;
        public string UpdateCheck
        {
            get { return updateCheck ?? "Never"; }
            set { updateCheck = value; }
        }

        public static string TransformPropertyName(string name, string table)
        {
            if (String.IsNullOrEmpty(name))
                return string.Empty;

            string newName = name;

            newName = Utility.GetProperName(newName);
            newName = Utility.IsStringNumeric(newName) ? "_" + newName : newName;
            newName = Utility.StripNonAlphaNumeric(newName);
            newName = newName.Trim();
            return Utility.KeyWordCheck(newName, table);
        }
    }
}
