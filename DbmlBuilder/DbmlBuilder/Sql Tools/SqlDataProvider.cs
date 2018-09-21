using System;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using DbmlBuilder.Utilities;
using DbmlBuilder.TableSchema;
using System.Configuration.Provider;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using DbmlBuilder.CodeGeneration;

namespace DbmlBuilder
{
    public class SqlDataProvider : ProviderBase
    {
        internal SqlConnection CreateConnection()
        {
            return new SqlConnection(DefaultConnectionString);
        }
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);
            GeneratedNamespace = config["generatedNamespace"];
        }
        public string GeneratedNamespace { get; set; }

        private static void ApplyConfig(NameValueCollection config, ref bool parameterValue, string configName)
        {
            if (config[configName] != null)
                parameterValue = Convert.ToBoolean(config[configName]);
        }
        public void SetParameter(IDataReader rdr, StoredProcedure.Parameter par)
        {
            par.SqlType = rdr[SqlSchemaVariable.DATA_TYPE].ToString();
            par.DBType = GetDbType(par.SqlType);
            string sMode = rdr[SqlSchemaVariable.MODE].ToString();
            if (sMode == SqlSchemaVariable.MODE_INOUT)
                par.Mode = ParameterDirection.InputOutput;
            par.Name = rdr[SqlSchemaVariable.NAME].ToString();
        }

        public string GetParameterPrefix()
        {
            return SqlSchemaVariable.PARAMETER_PREFIX;
        }

        private static readonly DataSet dsManyToManyCheck = new DataSet();

        private void LoadExtendedPropertyData()
        {
            if (ExtendedProperties == null)
            {
                ExtendedProperties = new DataTable();
                var cmdExtProps = new SqlCommand(EXTENDED_PROPERTIES_ALL);
                ExtendedProperties.Load(cmdExtProps.GetReader());
            }
        }

        public void FillTableSchema(Table tbl, TableType tableType)
        {
            TableColumnCollection columns = new TableColumnCollection();
            DataSet dsColumns;

            tbl.ForeignKeys = new ForeignKeyCollection();

            if (tableType == TableType.Function)
            {
                if (dsColumns2.Tables[Name] == null)
                {
                    var cmdColumns = new SqlCommand(ROUTINE_COLUMN_SQL_ALL);
                    var dt = new DataTable(Name);
                    dt.Load(cmdColumns.GetReader());
                    dsColumns2.Tables.Add(dt);
                }
                dsColumns = dsColumns2;
            }
            else
            {
                if (dsColumns1.Tables[Name] == null)
                {
                    var cmdColumns = new SqlCommand(TABLE_COLUMN_SQL_ALL);
                    var dt = new DataTable(Name);
                    dt.Load(cmdColumns.GetReader());
                    dsColumns1.Tables.Add(dt);
                }
                dsColumns = dsColumns1;
            }

            DataRow[] drColumns = dsColumns.Tables[Name].Select("TableName ='" + tbl.Name + "'", "OrdinalPosition ASC");

            for (int i = 0; i < drColumns.Length; i++)
            {
                TableColumn column = new TableColumn(tbl);
                column.ColumnName = drColumns[i][SqlSchemaVariable.COLUMN_NAME].ToString();
                column.NativeDataType = drColumns[i][SqlSchemaVariable.DATA_TYPE].ToString();
                if (column.NativeDataType == "numeric")
                    column.NativeDataType = string.Format("Decimal({0},{1})", drColumns[i]["precision"], drColumns[i]["scale"]);
                column.DataType = GetDbType(column.NativeDataType);
                if (drColumns[i][SqlSchemaVariable.COLUMN_DEFAULT] != DBNull.Value)
                {
                    string defaultSetting = drColumns[i][SqlSchemaVariable.COLUMN_DEFAULT].ToString().Trim();
                    if (defaultSetting.ToLower().IndexOf("newsequentialid()") > -1)
                        column.DefaultSetting = SqlSchemaVariable.DEFAULT;
                    else
                        column.DefaultSetting = defaultSetting;
                }
                column.AutoIncrement = Convert.ToBoolean(drColumns[i][SqlSchemaVariable.IS_IDENTITY]);
                int maxLength;
                int.TryParse(drColumns[i][SqlSchemaVariable.MAX_LENGTH].ToString(), out maxLength);
                column.MaxLength = maxLength;
                if (maxLength > 0)
                    column.NativeDataType += "(" + maxLength + ")";
                column.IsNullable = drColumns[i][SqlSchemaVariable.IS_NULLABLE].ToString() == "YES";
                bool isComputed = (drColumns[i][SqlSchemaVariable.IS_COMPUTED].ToString() == "1");
                column.IsReadOnly = (column.NativeDataType == "timestamp" || isComputed);
                column.UpdateCheck = ColumnAttributes.GetConcurrencyUpdateCheckRule(column);
                columns.Add(column);
                tbl.SchemaName = drColumns[i]["Owner"].ToString();
            }
            tbl.Columns = columns;

            if (dsIndex.Tables[Name] == null)
            {
                var cmdIndex = new SqlCommand(INDEX_SQL_ALL);
                DataTable dt = new DataTable(Name);
                dt.Load(cmdIndex.GetReader());
                dsIndex.Tables.Add(dt);
            }

            DataRow[] drIndexes = dsIndex.Tables[Name].Select("TableName ='" + tbl.Name + "'");
            for (int i = 0; i < drIndexes.Length; i++)
            {
                string colName = drIndexes[i][SqlSchemaVariable.COLUMN_NAME].ToString();
                string constraintType = drIndexes[i][SqlSchemaVariable.CONSTRAINT_TYPE].ToString();
                TableColumn column = columns.GetColumn(colName);

                if (Utility.IsMatch(constraintType, SqlSchemaVariable.PRIMARY_KEY))
                    column.IsPrimaryKey = true;
                else if (Utility.IsMatch(constraintType, SqlSchemaVariable.FOREIGN_KEY))
                    column.IsForeignKey = true;
                //HACK: Allow second pass naming adjust based on whether a column is keyed
                column.ColumnName = column.ColumnName;
            }

            if (dtFKR == null)
            {
                var cmdFKR = new SqlCommand(FOREIGN_KEY_RELATIONSHIPS);
                dtFKR = new DataTable();
                dtFKR.Load(cmdFKR.GetReader());

                var cmdFK = new SqlCommand(FOREIGN_KEYS);
                dtFK = new DataTable();
                dtFK.Load(cmdFK.GetReader());
            }

            DataRow[] drfkr;
            drfkr = dtFKR.Select("PkTable ='" + tbl.Name + "'");
            for (int i = 0; i < drfkr.Length; i++)
            {
                Relationship rel = new Relationship();
                rel.Name = drfkr[i]["RelName"].ToString();
                Table fktable = Db.Service.GetSchema(drfkr[i]["FkTable"].ToString());
                rel.ClassNameOne = tbl.ClassName;
                rel.ClassNameMany = fktable.ClassName;
                string[] a = Regex.Split(rel.Name, "__");
                if (a.Length == 2)
                {
                    rel.PropertyNameMany = a[0]; // name used in primary table to fetch many of this table
                    rel.PropertyNameOne = a[1]; // named used for foreign key in this table
                }
                else
                {
                    rel.PropertyNameOne = tbl.ClassName;
                    rel.PropertyNameMany = fktable.ClassNamePlural;
                }

                DataRow[] drfk = dtFK.Select("PkTable = '" + tbl.Name + "' and RelName = '" + rel.Name + "'");
                for (int ii = 0; ii < drfk.Length; ii++)
                {
                    Relationship.KeyPair kp = new Relationship.KeyPair();
                    TableColumn fkcol = fktable.Columns.GetColumn(drfk[ii]["FkColumn"].ToString());
                    TableColumn pkcol = columns.GetColumn(drfk[ii]["PkColumn"].ToString());
                    kp.vartype = fkcol.VarType;
                    kp.ForeignKey = fkcol.Name;
                    kp.PrimaryKey = pkcol.Name;
                    rel.KeyPairs.Add(kp);
                    if (ii > 0)
                    {
                        rel.ForeignKey += ",";
                        rel.PrimaryKey += ",";
                    }
                    rel.PrimaryKey += kp.PrimaryKey;
                    rel.ForeignKey += kp.ForeignKey;
                }


                tbl.ForeignKeyTables.Add(rel);
            }

            drfkr = dtFKR.Select("FkTable ='" + tbl.Name + "'");
            for (int i = 0; i < drfkr.Length; i++)
            {
                Relationship rel = new Relationship();
                rel.Name = drfkr[i]["RelName"].ToString();
                Table pktable = Db.Service.GetSchema(drfkr[i]["PkTable"].ToString());
                rel.ClassNameOne = pktable.ClassName;
                rel.ClassNameMany = tbl.ClassName;
                string[] a = Regex.Split(rel.Name, "__");
                if (a.Length == 2)
                {
                    rel.PropertyNameMany = a[0]; // name used in primary table to fetch many of this table
                    rel.PropertyNameOne = a[1]; // named used for foreign key in this table
                }
                else
                {
                    rel.PropertyNameOne = pktable.ClassName;
                    rel.PropertyNameMany = tbl.ClassNamePlural;
                }

                DataRow[] drfk = dtFK.Select("FkTable = '" + tbl.Name + "' and RelName = '" + rel.Name + "'");
                for (int ii = 0; ii < drfk.Length; ii++)
                {
                    Relationship.KeyPair kp = new Relationship.KeyPair();
                    TableColumn fkcol = columns.GetColumn(drfk[ii]["FkColumn"].ToString());
                    TableColumn pkcol = pktable.Columns.GetColumn(drfk[ii]["PkColumn"].ToString());
                    kp.vartype = fkcol.VarType;
                    kp.ForeignKey = fkcol.Name;
                    kp.PrimaryKey = pkcol.Name;
                    rel.KeyPairs.Add(kp);
                    if (ii > 0)
                    {
                        rel.ForeignKey += ",";
                        rel.PrimaryKey += ",";
                    }
                    rel.PrimaryKey += kp.PrimaryKey;
                    rel.ForeignKey += kp.ForeignKey;
                    fkcol.PropertyNameOne = rel.PropertyNameOne;
                }

                tbl.ForeignKeys.Add(rel);
            }

            if (dsManyToManyCheck.Tables[Name] == null)
            {
                var cmdM2M = new SqlCommand(MANY_TO_MANY_CHECK_ALL);
                var dt = new DataTable(Name);
                dt.Load(cmdM2M.GetReader());
                dsManyToManyCheck.Tables.Add(dt);
            }

            DataRow[] drs = dsManyToManyCheck.Tables[Name].Select("PK_Table = '" + tbl.Name + "'");
            if (drs.Length > 0)
            {
                for (int count = 0; count < drs.Length; count++)
                {
                    string mapTable = drs[count]["FK_Table"].ToString();
                    string localKey = drs[count]["FK_Column"].ToString();
                    if (dsManyToManyMap.Tables[Name] == null)
                    {
                        var cmdM2MMap = new SqlCommand(MANY_TO_MANY_FOREIGN_MAP_ALL);
                        DataTable dt = new DataTable(Name);
                        dt.Load(cmdM2MMap.GetReader());
                        dsManyToManyMap.Tables.Add(dt);
                    }


                    DataRow[] drMap = dsManyToManyMap.Tables[Name].Select("FK_Table = '" + mapTable + "' AND PK_Table <> '" + tbl.Name + "'");

                    for (int i = 0; i < drMap.Length; i++)
                    {
                        ManyToManyRelationship m = new ManyToManyRelationship(mapTable);
                        m.ForeignTableName = drMap[i]["PK_Table"].ToString();
                        m.ForeignPrimaryKey = drMap[i]["PK_Column"].ToString();
                        m.MapTableLocalTableKeyColumn = localKey;
                        m.MapTableForeignTableKeyColumn = drMap[i]["FK_Column"].ToString();
                        tbl.ManyToManys.Add(m);
                    }
                }
            }
        }

        private DataTable dtParamSql;

        public IDataReader GetSPParams(string spName)
        {
            if (dtParamSql == null)
            {
                var cmdSP = new SqlCommand(SP_PARAM_SQL_ALL);
                dtParamSql = new DataTable();
                dtParamSql.Load(cmdSP.GetReader());
            }

            DataView dv = new DataView(dtParamSql);
            dv.RowFilter = "SPName = '" + spName + "'";
            dv.Sort = "OrdinalPosition";
            DataTable dtNew = dv.ToTable();
            return dtNew.CreateDataReader();
        }

        public List<StoredProcedure> GetSPList()
        {
            var cmd = new SqlCommand("/* GetSPList() */ " + SP_SQL);
            var sps = new List<StoredProcedure>();
            using (IDataReader rdr = cmd.GetReader())
            {
                while (rdr.Read())
                {
                    var s = rdr.GetString(0);
                    string schema = rdr[SqlSchemaVariable.SCHEMA_NAME].ToString();
                    var sp = new StoredProcedure(schema, s);
                    if (rdr["ReturnType"] != DBNull.Value)
                        sp.ReturnType = rdr["ReturnType"].ToString();
                    sps.Add(sp);
                }
                rdr.Close();
            }
            return sps;
        }

        private IDataReader GetScalarFunctionRdr()
        {
            var cmd = new SqlCommand("/* GetScalarFunctionNameList() */ " + SCALER_FUNCTION_SQL);
            return cmd.GetReader();
        }
        public string[] GetFunctionNameList()
        {
            if (FunctionNames == null || !CurrentConnectionStringIsDefault)
            {
                var cmd = new SqlCommand("/* GetFunctionNameList() */ " + FUNCTION_SQL);
                var sList = new StringBuilder();
                using (IDataReader rdr = cmd.GetReader())
                {
                    while (rdr.Read())
                    {
                        string functionName = rdr[SqlSchemaVariable.NAME].ToString();
                        string schemaName = rdr[SqlSchemaVariable.SCHEMA_NAME].ToString();
                        sList.Append(schemaName + "." + functionName);
                        sList.Append("|");
                    }
                    rdr.Close();
                }
                string strList = sList.ToString();
                string[] tempFunctionNames = strList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                Array.Sort(tempFunctionNames);
                if (CurrentConnectionStringIsDefault)
                    FunctionNames = tempFunctionNames;
                else
                    return tempFunctionNames;
            }
            return FunctionNames;
        }
        public string[] GetViewNameList()
        {
            if (ViewNames == null || !CurrentConnectionStringIsDefault)
            {
                var cmd = new SqlCommand("/* GetViewNameList() */ " + VIEW_SQL);
                var sList = new StringBuilder();
                using (var rdr = cmd.GetReader())
                {
                    while (rdr.Read())
                    {
                        string viewName = rdr[SqlSchemaVariable.NAME].ToString();
                        sList.Append(rdr[SqlSchemaVariable.NAME]);
                        sList.Append("|");
                    }
                    rdr.Close();
                }
                string strList = sList.ToString();
                string[] tempViewNames = strList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                Array.Sort(tempViewNames);
                if (CurrentConnectionStringIsDefault)
                {
                    ViewNames = tempViewNames;
                }
                else
                {
                    return tempViewNames;
                }
            }
            return ViewNames;
        }

        private string[] tableNames = null;
        protected string[] TableNames
        {
            get { return tableNames; }
            set { tableNames = value; }
        }
        public string[] GetTableNameList()
        {
            if (TableNames == null || !CurrentConnectionStringIsDefault)
            {
                var cmd = new SqlCommand("/* GetTableNameList() */ " + TABLE_SQL);
                var sList = new StringBuilder();
                using (IDataReader rdr = cmd.GetReader())
                {
                    while (rdr.Read())
                    {
                        sList.Append(rdr[SqlSchemaVariable.NAME]);
                        sList.Append("|");
                    }
                    rdr.Close();
                }
                string strList = sList.ToString();
                string[] tempTableNames = strList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                Array.Sort(tempTableNames);
                if (CurrentConnectionStringIsDefault)
                {
                    TableNames = tempTableNames;
                }
                else
                {
                    return tempTableNames;
                }
            }
            return TableNames;
        }

        public DbType GetDbType(string sqlType)
        {
            switch (sqlType.ToLower())
            {
                case "varchar":
                    return DbType.String;
                case "nvarchar":
                    return DbType.String;
                case "int":
                    return DbType.Int32;
                case "uniqueidentifier":
                    return DbType.Guid;
                case "datetime":
                    return DbType.DateTime;
                case "date":
                    return DbType.Date;
                case "bigint":
                    return DbType.Int64;
                case "binary":
                    return DbType.Binary;
                case "bit":
                    return DbType.Boolean;
                case "char":
                    return DbType.AnsiStringFixedLength;
                case "decimal":
                    return DbType.Decimal;
                case "float":
                    return DbType.Double;
                case "image":
                    return DbType.Binary;
                case "money":
                    return DbType.Currency;
                case "nchar":
                    return DbType.String;
                case "ntext":
                    return DbType.String;
                case "numeric":
                    return DbType.Decimal;
                case "real":
                    return DbType.Decimal;
                case "smalldatetime":
                    return DbType.DateTime;
                case "smallint":
                    return DbType.Int16;
                case "smallmoney":
                    return DbType.Currency;
                case "sql_variant":
                    return DbType.String;
                case "sysname":
                    return DbType.String;
                case "text":
                    return DbType.String;
                case "timestamp":
                    return DbType.Binary;
                case "tinyint":
                    return DbType.Byte;
                case "varbinary":
                    return DbType.Binary;
                default:
                    if (sqlType.ToLower().StartsWith("decimal("))
                        return DbType.Decimal;
                    else
                        return DbType.String;
            }
        }

        #region Schema Bits

        private const string MANY_TO_MANY_CHECK_ALL =
@"SELECT 
    FK_Table = FK.TABLE_NAME, 
    FK_Column = CU.COLUMN_NAME, 
    PK_Table  = PK.TABLE_NAME, 
    PK_Column = PT.COLUMN_NAME, 
    Constraint_Name = C.CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
INNER JOIN    
(    
    SELECT i1.TABLE_NAME, i2.COLUMN_NAME
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
    WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY'
)
PT ON PT.TABLE_NAME = PK.TABLE_NAME

WHERE FK.TABLE_NAME IN
    (
        SELECT tc.table_Name FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
        JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS kcu ON tc.Constraint_name = kcu.Constraint_Name
        JOIN 
        (
            SELECT tc.Table_Name, kcu.Column_Name AS Column_Name FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
            JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS kcu ON tc.Constraint_name = kcu.Constraint_Name
            WHERE Constraint_Type = 'FOREIGN KEY'
        ) 
        AS t ON t.Table_Name = tc.table_Name AND t.Column_Name = kcu.Column_Name
        WHERE Constraint_Type = 'PRIMARY KEY'
        GROUP BY tc.Constraint_Name, tc.Table_Name HAVING COUNT(*) > 1   
    )"; //Thanks Jim!


        private static readonly DataSet dsColumns1 = new DataSet();
        private const string TABLE_COLUMN_SQL_ALL =
@"SELECT 
    TABLE_CATALOG AS [Database],
    TABLE_SCHEMA AS Owner, 
    TABLE_NAME AS TableName, 
    COLUMN_NAME AS ColumnName, 
    ORDINAL_POSITION AS OrdinalPosition, 
    COLUMN_DEFAULT AS DefaultSetting, 
    IS_NULLABLE AS IsNullable, DATA_TYPE AS DataType, 
    CHARACTER_MAXIMUM_LENGTH AS MaxLength, 
    DATETIME_PRECISION AS DatePrecision,
    COLUMNPROPERTY(object_id(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity') AS IsIdentity,
    COLUMNPROPERTY(object_id(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsComputed') as IsComputed,
    NUMERIC_SCALE as scale,
    NUMERIC_PRECISION as precision
FROM  INFORMATION_SCHEMA.COLUMNS
ORDER BY OrdinalPosition ASC";
        private static readonly DataSet dsColumns2 = new DataSet();
        private const string ROUTINE_COLUMN_SQL_ALL =
@"SELECT 
    TABLE_CATALOG AS [Database],
    TABLE_SCHEMA AS Owner, 
    TABLE_NAME AS TableName, 
    COLUMN_NAME AS ColumnName, 
    ORDINAL_POSITION AS OrdinalPosition, 
    COLUMN_DEFAULT AS DefaultSetting, 
    IS_NULLABLE AS IsNullable, DATA_TYPE AS DataType, 
    CHARACTER_MAXIMUM_LENGTH AS MaxLength, 
    DATETIME_PRECISION AS DatePrecision,
    COLUMNPROPERTY(object_id(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity') AS IsIdentity,
    COLUMNPROPERTY(object_id(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsComputed') as IsComputed,
    NUMERIC_SCALE as scale,
    NUMERIC_PRECISION as precision
FROM  INFORMATION_SCHEMA.ROUTINE_COLUMNS
ORDER BY OrdinalPosition ASC";

        private static readonly DataSet dsIndex = new DataSet();
        private const string INDEX_SQL_ALL =
@"SELECT
    KCU.TABLE_NAME as TableName,
    KCU.TABLE_SCHEMA as Owner,
    KCU.COLUMN_NAME as ColumnName, 
    TC.CONSTRAINT_TYPE as ConstraintType 
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU
JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC
ON KCU.CONSTRAINT_NAME=TC.CONSTRAINT_NAME";

        private static DataTable dtFK;
        private const string FOREIGN_KEYS =
@"select 
	kcu1.constraint_name AS RelName,
	kcu1.table_name AS FkTable,
	kcu1.column_name AS FkColumn,
	kcu2.table_name AS PkTable,
	kcu2.column_name AS PkColumn
from information_schema.referential_constraints rc
join information_schema.key_column_usage kcu1 
	on kcu1.constraint_catalog = rc.constraint_catalog 
		and kcu1.constraint_schema = rc.constraint_schema 
		and kcu1.constraint_name = rc.constraint_name
join information_schema.key_column_usage kcu2 
	on kcu2.constraint_catalog = rc.unique_constraint_catalog 
		and kcu2.constraint_schema = rc.unique_constraint_schema 
		and kcu2.constraint_name = rc.unique_constraint_name
where kcu1.ordinal_position = kcu2.ordinal_position";

        private static DataTable dtFKR;
        private const string FOREIGN_KEY_RELATIONSHIPS =
@"select distinct
	kcu1.constraint_name AS RelName,
	kcu1.table_name AS FkTable,
	kcu2.table_name AS PkTable,
	ep.value as Description
from information_schema.referential_constraints rc
join information_schema.key_column_usage kcu1 
	on kcu1.constraint_catalog = rc.constraint_catalog 
		and kcu1.constraint_schema = rc.constraint_schema 
		and kcu1.constraint_name = rc.constraint_name
join information_schema.key_column_usage kcu2 
	on kcu2.constraint_catalog = rc.unique_constraint_catalog 
		and kcu2.constraint_schema = rc.unique_constraint_schema 
		and kcu2.constraint_name = rc.unique_constraint_name
join sys.foreign_keys fk on rc.constraint_name = fk.name
left outer join sys.extended_properties ep 
	on fk.object_id = ep.major_id 
		and ep.name = 'MS_Description'
where kcu1.ordinal_position = kcu2.ordinal_position";

        private static readonly DataSet dsManyToManyMap = new DataSet();
        private const string MANY_TO_MANY_FOREIGN_MAP_ALL =
@"SELECT 
    FK_Table  = FK.TABLE_NAME,
    FK_Column = CU.COLUMN_NAME,
    PK_Table  = PK.TABLE_NAME,
    PK_Column = PT.COLUMN_NAME, Constraint_Name = C.CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
INNER JOIN 	INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
INNER JOIN  INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
INNER JOIN  INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
INNER JOIN	
    (
        SELECT i1.TABLE_NAME, i2.COLUMN_NAME
        FROM  INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
        INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
        WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY'
    ) 
PT ON PT.TABLE_NAME = PK.TABLE_NAME";

        private static DataTable ExtendedProperties;

        private const string EXTENDED_PROPERTIES_ALL =
@"SELECT 
    t.name AS [TABLE_NAME], 
    c.name AS [COLUMN_NAME], 
    ep.name AS [EXTENDED_NAME],
    value AS [EXTENDED_VALUE]
FROM sys.extended_properties AS ep
INNER JOIN sys.tables AS t ON ep.major_id = t.object_id 
LEFT JOIN sys.columns AS c ON ep.major_id = c.object_id AND ep.minor_id = c.column_id
WHERE class = 1 OR class = 3";

        private const string SP_PARAM_SQL_ALL = @"SELECT SPECIFIC_NAME AS SPName, ORDINAL_POSITION AS OrdinalPosition, 
                                                PARAMETER_MODE AS ParamType, IS_RESULT AS IsResult, PARAMETER_NAME AS Name, 
                                                DATA_TYPE AS DataType, CHARACTER_MAXIMUM_LENGTH AS DataLength, REPLACE(PARAMETER_NAME, '@', '') 
                                                AS CleanName, PARAMETER_MODE as [mode]
                                                FROM INFORMATION_SCHEMA.PARAMETERS
WHERE PARAMETER_NAME <> ''";

        const string TABLE_BY_PK = @"SELECT TC.TABLE_NAME as tableName 
                        FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU
                        JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC
                        ON KCU.CONSTRAINT_NAME=TC.CONSTRAINT_NAME 
                        WHERE column_name=@columnName
                        AND CONSTRAINT_TYPE='PRIMARY KEY'
                        AND TC.TABLE_NAME NOT LIKE '%'+@mapSuffix
                        AND KCU.ORDINAL_POSITION=1";

        private const string SP_SQL = @"SELECT SPECIFIC_NAME AS SPName, ROUTINE_DEFINITION AS SQL, CREATED AS CreatedOn, LAST_ALTERED AS ModifiedOn, ep.value as ReturnType , specific_schema as owner
                    FROM INFORMATION_SCHEMA.ROUTINES
                    left outer join sys.extended_properties ep on ep.major_id = object_id(specific_name) and ep.name = 'ReturnType'
                    WHERE ROUTINE_TYPE='PROCEDURE' AND SPECIFIC_NAME NOT LIKE 'sp_%diagram%'";

        private const string TABLE_SQL = @"SELECT TABLE_CATALOG AS [Database], TABLE_SCHEMA AS Owner, TABLE_NAME AS Name, TABLE_TYPE 
                                        FROM INFORMATION_SCHEMA.TABLES
                                        WHERE (TABLE_TYPE = 'BASE TABLE') AND (TABLE_NAME <> N'sysdiagrams') AND (TABLE_NAME <> N'dtproperties') AND (TABLE_NAME NOT LIKE N'ASPNET_%')";

        private const string VIEW_SQL = @"SELECT TABLE_CATALOG AS [Database], TABLE_SCHEMA AS Owner, TABLE_NAME AS Name, TABLE_TYPE
                                        FROM INFORMATION_SCHEMA.TABLES
                                        WHERE (TABLE_TYPE = 'VIEW') AND (TABLE_NAME <> N'sysdiagrams') AND (TABLE_NAME NOT LIKE N'vw_ASPNET_%')";

        private const string FUNCTION_SQL = @"SELECT SPECIFIC_CATALOG AS [Database], SPECIFIC_SCHEMA AS Owner, ROUTINE_NAME AS Name, 'Function' AS TABLE_TYPE
FROM INFORMATION_SCHEMA.ROUTINES
WHERE routine_type = 'FUNCTION' and data_type = 'TABLE'";

        private const string SCALER_FUNCTION_SQL = @"SELECT SPECIFIC_CATALOG AS [Database], SPECIFIC_SCHEMA AS Owner, ROUTINE_NAME AS Name, data_type as DataType, CHARACTER_MAXIMUM_LENGTH as maxlen, NUMERIC_PRECISION as prec, NUMERIC_SCALE as scale
FROM INFORMATION_SCHEMA.ROUTINES
WHERE routine_type = 'FUNCTION' and data_type <> 'TABLE'";


        #endregion

        private string[] viewNames = null;
        protected string[] ViewNames
        {
            get { return viewNames; }
            set { viewNames = value; }
        }
        private string[] functionNames = null;
        protected string[] FunctionNames
        {
            get { return functionNames; }
            set { functionNames = value; }
        }
        [ThreadStatic]
        private static SqlConnection __sharedConnection;

        public SqlConnection CurrentSharedConnection
        {
            get { return __sharedConnection; }

            protected set
            {
                if (value == null)
                {
                    __sharedConnection.Dispose();
                    __sharedConnection = null;
                }
                else
                {
                    __sharedConnection = value;
                    __sharedConnection.Disposed += __sharedConnection_Disposed;
                }
            }
        }

        private static void __sharedConnection_Disposed(object sender, EventArgs e)
        {
            __sharedConnection = null;
        }

        public string DefaultConnectionStringName { get; set; }
        private string _defaultConnectionString;
        public string DefaultConnectionString
        {
            get
            {
                return _defaultConnectionString;
            }
            set { _defaultConnectionString = value; }
        }
        public bool CurrentConnectionStringIsDefault
        {
            get
            {
                if (CurrentSharedConnection != null)
                    if (CurrentSharedConnection.ConnectionString != DefaultConnectionString)
                        return false;
                return true;
            }
        }
        public virtual string MakeParam(string paramName)
        {
            return Utility.PrefixParameter(paramName);
        }
        internal System.Collections.Generic.SortedList<string, Table> schemaCollection = new System.Collections.Generic.SortedList<string, Table>();
        public void AddSchema(string tableName, Table schema)
        {
            if (!schemaCollection.ContainsKey(tableName))
                schemaCollection.Add(tableName, schema);
        }
        public Table GetSchema(string tableName)
        {
            TableType tableType = TableType.Table;
            return GetSchema(tableName, tableType);
        }
        public Table GetSchema(string tableName, TableType tableType)
        {
            Table tbl;

            if (schemaCollection.ContainsKey(tableName))
                tbl = schemaCollection[tableName];
            else
            {
                tbl = new Table(tableName, tableType);
                AddSchema(tableName, tbl);
                FillTableSchema(tbl, tableType);
            }
            return tbl;
        }

        public Table[] GetTables()
        {
            string[] tableNames = GetTableNameList();
            Table[] tables = new Table[tableNames.Length];
            for (int i = 0; i < tables.Length; i++)
                tables[i] = GetSchema(tableNames[i], TableType.Table);
            return tables;
        }
        public Table[] GetViews()
        {
            string[] viewNames = GetViewNameList();
            Table[] views = new Table[viewNames.Length];
            for (int i = 0; i < views.Length; i++)
                views[i] = GetSchema(viewNames[i], TableType.View);
            return views;
        }

        public List<StoredProcedure> GetStoredProcedureCollection()
        {
            List<StoredProcedure> sps = GetSPList();

            foreach (var sp in sps)
            {
                var rdr = GetSPParams(sp.Name);
                while (rdr.Read())
                {
                    var par = new StoredProcedure.Parameter();
                    SetParameter(rdr, par);
                    par.QueryParameter = MakeParam(par.Name);
                    par.DisplayName = Utility.GetParameterName(par.Name);
                    sp.Parameters.Add(par);
                }
                rdr.Close();
            }
            return sps;
        }
        public List<StoredProcedure> GetTableFunctionCollection()
        {
            List<StoredProcedure> _fns = new List<StoredProcedure>();
            string[] fns = GetFunctionNameList();

            foreach (string s in fns)
            {
                string[] a = Regex.Split(s, @"\.");
                StoredProcedure sp = new StoredProcedure(a[0], a[1]);
                sp.ClassName = Db.Service.GetSchema(a[1], TableType.Function).ClassName;

                //get the params
                IDataReader rdr = GetSPParams(a[1]);
                while (rdr.Read())
                {
                    StoredProcedure.Parameter par = new StoredProcedure.Parameter();
                    SetParameter(rdr, par);
                    par.QueryParameter = MakeParam(par.Name);
                    par.DisplayName = Utility.GetParameterName(par.Name);
                    sp.Parameters.Add(par);
                }
                for (int i = 0; i < sp.Parameters.Count; i++)
                    rdr.Close();
                _fns.Add(sp);
            }
            return _fns;
        }
        public List<StoredProcedure> GetScalarFunctionCollection()
        {
            List<StoredProcedure> _fns = new List<StoredProcedure>();
            IDataReader rdr = GetScalarFunctionRdr();
            while (rdr.Read())
            {
                StoredProcedure sp = new StoredProcedure(
                    rdr[SqlSchemaVariable.SCHEMA_NAME].ToString(),
                    rdr[SqlSchemaVariable.NAME].ToString());
                sp.sqlType = rdr[SqlSchemaVariable.DATA_TYPE].ToString();
                sp.dbType = GetDbType(sp.sqlType);
                _fns.Add(sp);
            }
            rdr.Close();

            foreach (StoredProcedure sp in _fns)
            {
                rdr = GetSPParams(sp.Name);
                while (rdr.Read())
                {
                    StoredProcedure.Parameter par = new StoredProcedure.Parameter();
                    SetParameter(rdr, par);
                    par.QueryParameter = MakeParam(par.Name);
                    par.DisplayName = Utility.GetParameterName(par.Name);
                    sp.Parameters.Add(par);
                }
                rdr.Close();
            }
            return _fns;
        }
    }
}
