using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models
{
	public abstract class ColumnInfo
	{
		public string Name { get; }
		public PostgresDataType DataType { get; }
		public PropertyInfo Property { get;  }

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

			foreach(Attribute attr in property.GetCustomAttributes())
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

	public class WeekStatColumnInfo : ColumnInfo
	{
		public WeekStatType StatType { get; }

		private WeekStatColumnInfo(
			string name, 
			WeekStatType statType,
			PropertyInfo property)
			: base(name, PostgresDataType.FLOAT8, property)
		{
			StatType = statType;
		}

		public static WeekStatColumnInfo FromProperty(PropertyInfo property)
		{
			var attr = property
				.GetCustomAttributes()
				.Single(a => a is WeekStatColumnAttribute) as WeekStatColumnAttribute;

			return new WeekStatColumnInfo(attr.Name, attr.StatType, property);
		}
	}

}
