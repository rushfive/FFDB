using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.ColumnInfos
{
	public abstract class ColumnInfo
	{
		public string Name { get; }
		public PostgresDataType DataType { get; }
		public PropertyInfo Property { get; }

		protected ColumnInfo(
			string name,
			PostgresDataType dataType,
			PropertyInfo property)
		{
			Name = name;
			DataType = dataType;
			Property = property;
		}
	}
}
