using Dapper;
using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace TouchpointSoftware.Cms.Reporting
{
    public class SqlParameterAttribute : Attribute
    {
        public string Name { get; set; }
        public ParameterDirection Direction { get; set; }
        public DbType Type { get; set; }
        public int Size { get; set; }
    }

    public abstract class ReportParameterModel
    {
        public virtual DynamicParameters ToDynamicParameters()
        {
            var parameters = new DynamicParameters();

            var type = GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi => pi.IsDefined(typeof(SqlParameterAttribute), true));

            foreach (var property in properties)
            {
                var metadata = property.GetCustomAttribute(typeof(SqlParameterAttribute), true) as SqlParameterAttribute;

                parameters.Add(name: metadata.Name, value: property.GetValue(this), dbType: metadata.Type, direction: metadata.Direction, size: metadata.Size);
            }

            return parameters;
        }
    }
}
