<%@ Page Language="C#" %>
<%@ Import namespace="DbmlBuilder.Utilities"%>
<%@ Import Namespace="DbmlBuilder.TableSchema" %>
<%@ Import Namespace="DbmlBuilder" %>
<%
    Table[] tables = Db.Service.GetTables();
    Table[] views = Db.Service.GetViews();
    System.Collections.Generic.List<StoredProcedure> fnTableList = Db.Service.GetTableFunctionCollection();
    System.Collections.Generic.List<StoredProcedure> fnScalarList = Db.Service.GetScalarFunctionCollection();
    System.Collections.Generic.List<StoredProcedure> SPList = Db.Service.GetStoredProcedureCollection();
%>

namespace <%=Db.Service.GeneratedNamespace %>
{
	[DatabaseAttribute(Name="<%=Db.Service.Name %>")]
	public partial class <%=Db.Service.Name %>DataContext : DataContext
	{
		private static MappingSource mappingSource = new AttributeMappingSource();
		
#region Extensibility Method Definitions
        partial void OnCreated();
		<%
			foreach (Table t in tables)
			{
%>
        partial void Insert<%=t.ClassName%>(<%=t.ClassName%> instance);
        partial void Update<%=t.ClassName%>(<%=t.ClassName%> instance);
        partial void Delete<%=t.ClassName%>(<%=t.ClassName%> instance);
        <%
			}
%>
#endregion
		
		public <%=Db.Service.Name %>DataContext() : 
				base(System.Configuration.ConfigurationManager.ConnectionStrings["<%=Db.Service.DefaultConnectionStringName%>"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public <%=Db.Service.Name %>DataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public <%=Db.Service.Name %>DataContext(IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public <%=Db.Service.Name %>DataContext(string connection, MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public <%=Db.Service.Name %>DataContext(IDbConnection connection, MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}

    #region Tables
		<%
			foreach (Table t in tables)
			{
%>
		public Table<<%=t.ClassName%>> <%=t.ClassNamePlural%>
		{
			get	{ return this.GetTable<<%=t.ClassName%>>(); }
		}<%
			} %>
	#endregion
	#region Views
		<%
			foreach (Table t in views)
			{
%>
	    public Table<View.<%=t.ClassName%>> View<%=t.ClassNamePlural%>
	    {
		    get { return this.GetTable<View.<%=t.ClassName%>>(); }
	    }<%
			}
%>
    #endregion
	#region Table Functions
		<%
			foreach (StoredProcedure fn in fnTableList)
			{
%>
		[Function(Name="<%=fn.SchemaName%>.<%=fn.Name%>", IsComposable = true)]
		public IQueryable<View.<%=fn.ClassName%> > <%=fn.DisplayName%>(<%
            int n = fn.Parameters.Count;
            for (int i = 0; i < n; i++)
            {
                StoredProcedure.Parameter p = fn.Parameters[i];
                string comma = i == n - 1 ? "" : ",";
                string pType = Utility.GetVariableType(p.DBType, Utility.IsNullableDbType(p.DBType));%>
            [Parameter(DbType="<%= p.SqlType%>")] <%=pType %> <%= p.DisplayName%><%=comma%><%
            }
        %>
            )
		{
			return this.CreateMethodCallQuery<View.<%=fn.ClassName%>>(this, 
			    ((MethodInfo)(MethodInfo.GetCurrentMethod())),<%
            for (int i = 0; i < n; i++)
            {
                StoredProcedure.Parameter p = fn.Parameters[i];
                string comma = i == n - 1 ? "" : ",";%>
                <%= p.DisplayName%><%=comma%><%
            }
        %>
                );
		}<%
			}
%>
    #endregion
	#region Scalar Functions
		<%
			foreach (StoredProcedure fn in fnScalarList)
			{
                string comma;
                string fnType = Utility.GetVariableType(fn.dbType, Utility.IsNullableDbType(fn.dbType));
%>
		[Function(Name="<%=fn.SchemaName%>.<%=fn.Name%>", IsComposable = true)]
		[return: Parameter(DbType = "<%=fn.sqlType%>")]
		public <%=fnType%> <%=fn.DisplayName%>(<%
            int n = fn.Parameters.Count;
            for (int i = 0; i < n; i++)
            {
                StoredProcedure.Parameter p = fn.Parameters[i];
                comma = i == n - 1 ? "" : ",";
                string pType = Utility.GetVariableType(p.DBType, Utility.IsNullableDbType(p.DBType));%>
            [Parameter(Name = "<%=p.DisplayName%>", DbType="<%= p.SqlType%>")] <%=pType %> <%= p.DisplayName%><%=comma%><%
            }
            comma = n == 0 ? "" : ",";
        %>
            )
		{
			return ((<%=fnType%>)(this.ExecuteMethodCall(this, 
                ((MethodInfo)(MethodInfo.GetCurrentMethod()))<%=comma%><%
            for (int i = 0; i < n; i++)
            {
                StoredProcedure.Parameter p = fn.Parameters[i];
                comma = i == n - 1 ? "" : ",";%>
                <%= p.DisplayName%><%=comma%><%
            }
        %>
                ).ReturnValue));
		}<%
			}
%>
    #endregion
	#region Stored Procedures
		<%
            foreach (StoredProcedure sp in SPList)
            {
                if (string.IsNullOrEmpty(sp.ReturnType))
                    continue;
                string comma;
%>
		[Function(Name="<%=sp.SchemaName%>.<%=sp.Name%>")]
		public ISingleResult<<%=sp.ReturnType%>> <%=sp.DisplayName%>(<%
            int n = sp.Parameters.Count;
            for (int i = 0; i < n; i++)
            {
                StoredProcedure.Parameter p = sp.Parameters[i];
                comma = i == n - 1 ? "" : ",";
                string pType = Utility.GetVariableType(p.DBType, Utility.IsNullableDbType(p.DBType));%>
            [Parameter(Name = "<%=p.DisplayName%>", DbType="<%= p.SqlType%>")] <%=pType %> <%= p.DisplayName%><%=comma%><%
            }
            comma = n == 0 ? "" : ",";
        %>
            )
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod()))<%=comma%><%
            for (int i = 0; i < n; i++)
            {
                StoredProcedure.Parameter p = sp.Parameters[i];
                comma = i == n - 1 ? "" : ",";%>
                <%= p.DisplayName%><%=comma%><%
            }
                %>
			);
			return ((ISingleResult<<%=sp.ReturnType%>>)(result.ReturnValue));
		}<%
			}
%>
    #endregion
   }
}



