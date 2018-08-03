<%@ Page Language="C#"%>
<%@ Import namespace="DbmlBuilder.Utilities"%>
<%@ Import Namespace="DbmlBuilder" %>
<%@ Import Namespace="DbmlBuilder.CodeGenerator" %>
<%@ Import Namespace="DbmlBuilder.TableSchema" %>
<%@ Import Namespace="System.Data" %>

<%
    System.Collections.Generic.List<StoredProcedure> spList = Db.Service.GetTableFunctionCollection() ;
	ArrayList classStoredProcedures = new ArrayList();
	
%>
namespace <%=Db.Service.GeneratedNamespace %>
{

	public partial class <%=Db.Service.Name %>DataContext
        <%foreach (StoredProcedure sp in spList)
          {
%>
		[Function(Name="<%=sp.Name%>", IComposable = true)]
		public IQueryable<<%=sp.DisplayName%> > <%=sp.DisplayName%>(
		<%
            bool isFirst = true;
            foreach (StoredProcedure.Parameter p in sp.Parameters)
            {
                string pType = Utility.GetVariableType(p.DBType, Utility.IsNullableDbType(p.DBType));
        %>
            [Parameter(DbType="<%= pType%>")] string <%= p.DisplayName%>
        <%
            }
        %>
            )
		{
			return this.CreateMethodCallQuery<<%= sp.DisplayName %>>(this, 
			    ((MethodInfo)(MethodInfo.GetCurrentMethod())), 
        <%
            bool isFirst = true;
            foreach (StoredProcedure.Parameter p in sp.Parameters)
            {
        %>
            <%= p.DisplayName%>
        <%
            }
        %>
                );
		}
<% } %>
}
