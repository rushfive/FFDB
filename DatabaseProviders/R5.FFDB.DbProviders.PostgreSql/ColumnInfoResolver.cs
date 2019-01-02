using R5.FFDB.Components;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.ColumnInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql
{
	public static class ColumnInfoResolver
	{
		public static ColumnInfo FromProperty(PropertyInfo property)
		{
			EnforceValidAttributeTypes(property, out bool isWeekStat);
			EnforceValidPropertyTypes(property, isWeekStat);

			if (isWeekStat)
			{
				return WeekStatColumnInfo.FromProperty(property);
			}

			return PropertyColumnInfo.FromProperty(property);
		}

		// validate correct property types based on postgres data types
		private static void EnforceValidPropertyTypes(PropertyInfo property, bool isWeekStat)
		{
			if (isWeekStat && property.PropertyType != typeof(double?))
			{
				throw new InvalidOperationException("Week stat column properties must have a nullable double type.");
			}

			if (!isWeekStat)
			{
				var columnAttr = property.GetCustomAttributes().Single(a => a is ColumnAttribute) as ColumnAttribute;
				Type propertyType = property.PropertyType;

				switch (columnAttr.DataType)
				{
					case PostgresDataType.UUID:
						if (propertyType != typeof(Guid) && propertyType != typeof(Guid?))
						{
							throw new InvalidOperationException($"Column properties with '{PostgresDataType.UUID}' data type must have a 'Guid' or 'Guid?' type.");
						}
						break;
					case PostgresDataType.TEXT:
						if (propertyType != typeof(string) && !propertyType.IsEnum && !propertyType.IsNullableEnum())
						{
							throw new InvalidOperationException($"Column properties with '{PostgresDataType.TEXT}' data type must have a 'string' or 'Enum/Enum?' type.");
						}
						break;
					case PostgresDataType.INT:
						if (propertyType != typeof(int) && propertyType != typeof(int?))
						{
							throw new InvalidOperationException($"Column properties with '{PostgresDataType.INT}' data type must have a 'int' or 'int?' type.");
						}
						break;
					case PostgresDataType.TIMESTAMPTZ:
						break;
					case PostgresDataType.FLOAT8:
						if (propertyType != typeof(double) && property != typeof(double?))
						{
							throw new InvalidOperationException($"Column properties with '{PostgresDataType.FLOAT8}' data type must have a 'double' or 'double?' type.");
						}
						break;
					case PostgresDataType.DATE:
						if (propertyType != typeof(DateTimeOffset) && propertyType != typeof(DateTimeOffset?))
						{
							throw new InvalidOperationException($"Column properties with '{PostgresDataType.DATE}' data type must have a 'DateTimeOffset' type.");
						}
						break;
					case PostgresDataType.BOOLEAN:
						if (propertyType != typeof(bool) && propertyType != typeof(bool?))
						{
							throw new InvalidOperationException($"Column properties with '{PostgresDataType.BOOLEAN}' data type must have a 'bool' or 'bool?' type.");
						}
						break;
					default:
						throw new ArgumentOutOfRangeException($"'{columnAttr.DataType}' is not a valid postgres data type.");
				}
			}
		}

		// should have either the Column or WeekStat attribute, but not both
		private static void EnforceValidAttributeTypes(PropertyInfo property, out bool isWeekStat)
		{
			bool hasColumn = false;
			bool hasWeekStat = false;

			foreach (Attribute attr in property.GetCustomAttributes())
			{
				if (attr is ColumnAttribute)
				{
					if (hasWeekStat)
					{
						throw new InvalidOperationException($"Property '{property.Name}' has both 'Column' and 'WeekStat' attributes.");
					}
					hasColumn = true;
					continue;
				}

				if (attr is WeekStatColumnAttribute)
				{
					if (hasColumn)
					{
						throw new InvalidOperationException($"Property '{property.Name}' has both 'Column' and 'WeekStat' attributes.");
					}
					hasWeekStat = true;
					continue;
				}
			}

			if (!hasColumn && !hasWeekStat)
			{
				throw new InvalidOperationException($"Property '{property.Name}' must have either a 'Column' or 'WeekStat' column attributes.");
			}

			isWeekStat = hasWeekStat;
		}
	}
}
