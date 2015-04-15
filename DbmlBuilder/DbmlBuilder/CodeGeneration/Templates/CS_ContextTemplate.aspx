<%@ Page Language="C#" %>

<%@ Import Namespace="DbmlBuilder.Utilities" %>
<%@ Import Namespace="DbmlBuilder.TableSchema" %>
<%@ Import Namespace="DbmlBuilder" %>
<%
    var tables = Db.Service.GetTables();
    var views = Db.Service.GetViews();
    var fnTableList = Db.Service.GetTableFunctionCollection();
    var fnScalarList = Db.Service.GetScalarFunctionCollection();
    var spList = Db.Service.GetStoredProcedureCollection();
%>
using System.Data.SqlClient;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace <%=Db.Service.GeneratedNamespace %>
{
    [DatabaseAttribute(Name="<%=Db.Service.Name %>")]
    public partial class <%=Db.Service.Name %>DataContext : DataContext
    {
        private readonly static MappingSource _mappingSource = new AttributeMappingSource();

#region Extensibility Method Definitions
        partial void OnCreated();
        <%
            foreach (var t in tables)
            {
        %>
        partial void Insert<%=t.ClassName%>(<%=t.ClassName%> instance);
        partial void Update<%=t.ClassName%>(<%=t.ClassName%> instance);
        partial void Delete<%=t.ClassName%>(<%=t.ClassName%> instance);
        <%
            }
        %>
#endregion

        public <%=Db.Service.Name %>DataContext(string connectionString) :
                base(new ProfiledDbConnection(new SqlConnection(connectionString), MiniProfiler.Current), _mappingSource)
        {
            OnCreated();
        }

#region Tables
        <%
            foreach (var t in tables)
            {
        %>
        public Table< <%=t.ClassName%>> <%=t.ClassNamePlural%>
        {
            get { return GetTable< <%=t.ClassName%>>(); }
        }<%
            } %>
#endregion
#region Views
        <%
            foreach (var t in views)
            {
        %>
        public Table< View.<%=t.ClassName%>> View<%=t.ClassNamePlural%>
        {
            get { return GetTable< View.<%=t.ClassName%>>(); }
        }<%
            }
        %>
#endregion
#region Table Functions
        <%
            foreach (var fn in fnTableList)
            {
        %>
        [Function(Name="<%=fn.SchemaName%>.<%=fn.Name%>", IsComposable = true)]
        public IQueryable< View.<%=fn.ClassName%> > <%=fn.DisplayName%>(<%
                var n = fn.Parameters.Count;
                for (var i = 0; i < n; i++)
                {
                    var p = fn.Parameters[i];
                    var comma = i == n - 1 ? "" : ",";
                    var pType = Utility.GetVariableType(p.DBType, Utility.IsNullableDbType(p.DBType));%>
            [Parameter(DbType="<%= p.SqlType%>")] <%=pType %> <%= p.DisplayName%><%=comma%><%
                }
            %>
            )
        {
            return CreateMethodCallQuery< View.<%=fn.ClassName%>>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),<%
                for (var i = 0; i < n; i++)
                {
                    var p = fn.Parameters[i];
                    var comma = i == n - 1 ? "" : ",";%>
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
            foreach (var fn in fnScalarList)
            {
                string comma;
                var fnType = Utility.GetVariableType(fn.dbType, Utility.IsNullableDbType(fn.dbType));
        %>
        [Function(Name="<%=fn.SchemaName%>.<%=fn.Name%>", IsComposable = true)]
        [return: Parameter(DbType = "<%=fn.sqlType%>")]
        public <%=fnType%> <%=fn.DisplayName%>(<%
                var n = fn.Parameters.Count;
                for (var i = 0; i < n; i++)
                {
                    var p = fn.Parameters[i];
                    comma = i == n - 1 ? "" : ",";
                    var pType = Utility.GetVariableType(p.DBType, Utility.IsNullableDbType(p.DBType));%>
            [Parameter(Name = "<%=p.DisplayName%>", DbType="<%= p.SqlType%>")] <%=pType %> <%= p.DisplayName%><%=comma%><%
                }
                comma = n == 0 ? "" : ",";
            %>
            )
        {
            return ((<%=fnType%>)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod()))<%=comma%><%
                for (var i = 0; i < n; i++)
                {
                    var p = fn.Parameters[i];
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
            foreach (var sp in spList)
            {
                if (string.IsNullOrEmpty(sp.ReturnType))
                    continue;
                string comma;
        %>
        [Function(Name="<%=sp.SchemaName%>.<%=sp.Name%>")]
        public ISingleResult< <%=sp.ReturnType%>> <%=sp.DisplayName%>(<%
                var n = sp.Parameters.Count;
                for (var i = 0; i < n; i++)
                {
                    var p = sp.Parameters[i];
                    comma = i == n - 1 ? "" : ",";
                    var pType = Utility.GetVariableType(p.DBType, Utility.IsNullableDbType(p.DBType));%>
            [Parameter(Name = "<%=p.DisplayName%>", DbType="<%= p.SqlType%>")] <%=pType %> <%= p.DisplayName%><%=comma%><%
                }
                comma = n == 0 ? "" : ",";
            %>
            )
        {
            IExecuteResult result = ExecuteMethodCall(this, ((MethodInfo)(MethodBase.GetCurrentMethod()))<%=comma%><%
                for (var i = 0; i < n; i++)
                {
                    var p = sp.Parameters[i];
                    comma = i == n - 1 ? "" : ",";%>
                    <%= p.DisplayName%><%=comma%><%
                }
%>
            );
            return ((ISingleResult< <%=sp.ReturnType%>>)(result.ReturnValue));
        }<%
            }
        %>
#endregion
   }
}