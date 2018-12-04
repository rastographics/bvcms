using System;
using System.Data;
using System.Reflection;

namespace CmsData.Classes.DataMapper
{
	public class DataMapper
	{
		public void populate( DataRow row )
		{
			Type type = GetType();
			FieldInfo[] fields = type.GetFields();
			PropertyInfo[] properties = type.GetProperties();

			foreach( FieldInfo field in fields ) {
				if( !row.Table.Columns.Contains( field.Name ) || row.IsNull( field.Name ) ) continue;

				field.SetValue( this, row[field.Name] );
			}

			foreach( PropertyInfo property in properties ) {
				if( property != null && property.CanWrite ) {
					object value = Convert.ChangeType( row[property.Name], property.PropertyType );

					property.SetValue( this, value );
				}
			}
		}
	}
}