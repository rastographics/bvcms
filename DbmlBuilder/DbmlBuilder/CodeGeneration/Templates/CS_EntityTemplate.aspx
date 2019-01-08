<%@ Page Language="C#" %>
<%@ Import namespace="DbmlBuilder.Utilities"%>
<%@ Import Namespace="DbmlBuilder.TableSchema" %>
<%@ Import Namespace="DbmlBuilder" %>
<%
    Table tbl = Db.Service.GetSchema("#TABLE#");
%>
using CmsData.Infrastructure;

namespace <%=Db.Service.GeneratedNamespace %>
{
	[Table(Name="<%=tbl.SchemaName%>.<%=tbl.Name %>")]
	public partial class <%=tbl.ClassName %> : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		<%
			foreach(TableColumn col in tbl.Columns)
			{
        %>
		private <%=col.VarType%> _<%=col.Name%>;
		<% 
			}
		%>
   		<%foreach (Relationship rel in tbl.ForeignKeyTables) { %>
   		private EntitySet<<%=rel.ClassNameMany%>> _<%=rel.PropertyNameMany%>;
		<% } %>
    	<%foreach (Relationship rel in tbl.ForeignKeys) { %>
		private EntityRef<<%=rel.ClassNameOne%>> _<%=rel.PropertyNameOne%>;
		<% } %>
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		<%
            foreach (TableColumn col in tbl.Columns)
			{
        %>
		partial void On<%=col.Name%>Changing(<%=col.VarType%> value);
		partial void On<%=col.Name%>Changed();
		<% 
			}
		%>
    #endregion

		public <%=tbl.ClassName%>()
		{
			<%foreach (Relationship rel in tbl.ForeignKeyTables) { %>
			this._<%=rel.PropertyNameMany%> = new EntitySet<<%=rel.ClassNameMany%>>(new Action< <%=rel.ClassNameMany%>>(this.attach_<%=rel.PropertyNameMany%>), new Action< <%=rel.ClassNameMany%>>(this.detach_<%=rel.PropertyNameMany%>)); 
			<% } %>
			<%foreach (Relationship rel in tbl.ForeignKeys) { %>
			this._<%=rel.PropertyNameOne%> = default(EntityRef<<%=rel.ClassNameOne%>>); 
			<% } %>
			OnCreated();
		}
		
    #region Columns
		<%
            foreach (TableColumn col in tbl.Columns)
			{
        %>
		[Column(Name="<%=col.ColumnName%>", UpdateCheck=UpdateCheck.<%=col.UpdateCheck%>, Storage="_<%=col.Name%>"<%=col.FullDbType%>)]<% if (col.IsForeignKey) { %>
		[IsForeignKey]<% } %>
		public <%=col.VarType%> <%=col.Name%>
		{
			get { return this._<%=col.Name%>; }
			set
			{
				if (this._<%=col.Name%> != value)
				{
				<%if (col.IsForeignKey) { %>
					if (this._<%=col.PropertyNameOne%>.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				<% } %>
                    this.On<%=col.Name%>Changing(value);
					this.SendPropertyChanging();
					this._<%=col.Name%> = value;
					this.SendPropertyChanged("<%=col.Name%>");
					this.On<%=col.Name%>Changed();
				}
			}
		}
		<% 
			}
		%>
    #endregion
        
    #region Foreign Key Tables
   		<%foreach (Relationship rel in tbl.ForeignKeyTables) { %>
   		[Association(Name="<%=rel.Name%>", Storage="_<%=rel.PropertyNameMany%>", OtherKey="<%=rel.ForeignKey%>")]
   		public EntitySet<<%=rel.ClassNameMany%>> <%=rel.PropertyNameMany%>
   		{
   		    get { return this._<%=rel.PropertyNameMany%>; }
			set	{ this._<%=rel.PropertyNameMany%>.Assign(value); }
   		}
		<% } %>
	#endregion
	
	#region Foreign Keys
    	<%foreach (Relationship rel in tbl.ForeignKeys) { %>
		[Association(Name="<%=rel.Name%>", Storage="_<%=rel.PropertyNameOne%>", ThisKey="<%=rel.ForeignKey%>", IsForeignKey=true)]
		public <%=rel.ClassNameOne%> <%=rel.PropertyNameOne%>
		{
			get { return this._<%=rel.PropertyNameOne%>.Entity; }
			set
			{
				<%=rel.ClassNameOne%> previousValue = this._<%=rel.PropertyNameOne%>.Entity;
				if (((previousValue != value) 
							|| (this._<%=rel.PropertyNameOne%>.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._<%=rel.PropertyNameOne%>.Entity = null;
						previousValue.<%=rel.PropertyNameMany%>.Remove(this);
					}
					this._<%=rel.PropertyNameOne%>.Entity = value;
					if (value != null)
					{
						value.<%=rel.PropertyNameMany%>.Add(this);
						<%foreach (Relationship.KeyPair kp in rel.KeyPairs) { %>
						this._<%=kp.ForeignKey%> = value.<%=kp.PrimaryKey%>;
						<% } %>
					}
					else
					{
						<%foreach (Relationship.KeyPair kp in rel.KeyPairs) { %>
						this._<%=kp.ForeignKey%> = default(<%=kp.vartype%>);
						<% } %>
					}
					this.SendPropertyChanged("<%=rel.PropertyNameOne%>");
				}
			}
		}
		<% } %>
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

   		<%foreach (Relationship rel in tbl.ForeignKeyTables) { %>
		private void attach_<%=rel.PropertyNameMany%>(<%=rel.ClassNameMany%> entity)
		{
			this.SendPropertyChanging();
			entity.<%=rel.PropertyNameOne%> = this;
		}
		private void detach_<%=rel.PropertyNameMany%>(<%=rel.ClassNameMany%> entity)
		{
			this.SendPropertyChanging();
			entity.<%=rel.PropertyNameOne%> = null;
		}
		<% } %>
	}
}
