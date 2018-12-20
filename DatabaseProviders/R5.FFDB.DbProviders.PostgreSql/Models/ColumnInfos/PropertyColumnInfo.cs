using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.ColumnInfos
{
	public class PropertyColumnInfo : ColumnInfo
	{
		public bool PrimaryKey { get; set; }
		public bool NotNull { get; set; }
		public Type ForeignTableType { get; set; }
		public string ForeignKeyColumn { get; set; }

		public bool HasForeignKeyConstraint =>
			ForeignTableType != null && !string.IsNullOrWhiteSpace(ForeignKeyColumn);

		private PropertyColumnInfo(
			string name,
			PostgresDataType dataType,
			PropertyInfo property = null)
			: base(name, dataType, property)
		{
		}

		public static PropertyColumnInfo FromProperty(PropertyInfo property)
		{
			string name = null;
			PostgresDataType? dataType = null;
			bool primaryKey = false;
			bool notNull = false;
			Type foreignTableType = null;
			string foreignColumnName = null;

			foreach (Attribute attr in property.GetCustomAttributes())
			{
				switch (attr)
				{
					case PrimaryKeyAttribute _:
						primaryKey = true;
						break;
					case NotNullAttribute _:
						notNull = true;
						break;
					case ColumnAttribute column:
						name = column.Name;
						dataType = column.DataType;
						break;
					case ForeignKeyAttribute foreign:
						foreignTableType = foreign.ForeignTableType;
						foreignColumnName = foreign.ForeignColumnName;
						break;
				}
			}

			return new PropertyColumnInfo(name, dataType.Value, property)
			{
				PrimaryKey = primaryKey,
				NotNull = notNull,
				ForeignTableType = foreignTableType,
				ForeignKeyColumn = foreignColumnName
			};
		}
	}
}
