using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace ImageData
{
	[DatabaseAttribute(Name="CMSImage")]
	public partial class CMSImageDataContext : DataContext
	{
		private static MappingSource mappingSource = new AttributeMappingSource();
		
#region Extensibility Method Definitions
        partial void OnCreated();
		
        partial void InsertImage(Image instance);
        partial void UpdateImage(Image instance);
        partial void DeleteImage(Image instance);
        
        partial void InsertOther(Other instance);
        partial void UpdateOther(Other instance);
        partial void DeleteOther(Other instance);
        
#endregion
		
		public CMSImageDataContext() : 
				base(System.Configuration.ConfigurationManager.ConnectionStrings["CMSImage"].ConnectionString, mappingSource)
		{
			OnCreated();
		}

		
		public CMSImageDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}

		
		public CMSImageDataContext(IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}

		
		public CMSImageDataContext(string connection, MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}

		
		public CMSImageDataContext(IDbConnection connection, MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}

    #region Tables
		
		public Table< Image> Images
		{
			get	{ return this.GetTable< Image>(); }

		}

		public Table< Other> Others
		{
			get	{ return this.GetTable< Other>(); }

		}

        public static CMSImageDataContext Create(HttpContextBase currentHttpContext)
        {
            var host = currentHttpContext.Request.Url.Authority.Split('.', ':')[0];
            var cs = ConfigurationManager.ConnectionStrings["CMS"];
            var cb = new SqlConnectionStringBuilder(cs.ConnectionString);
            cb.InitialCatalog = $"CMSi_{host}";
            cb.PersistSecurityInfo = true;
            var connectionString = cb.ConnectionString;

            return CMSImageDataContext.Create(connectionString);
        }

        private static CMSImageDataContext Create(string connectionString)
        {
            return new CMSImageDataContext(connectionString);
        }

        #endregion
        #region Views

        #endregion
        #region Table Functions

        #endregion
        #region Scalar Functions

        #endregion
        #region Stored Procedures

        #endregion
    }

}

