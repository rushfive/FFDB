using R5.FFDB.DbProviders.PostgreSql.Attributes;
using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.ColumnInfos;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql
{
	public static class ColumnInfoResolver
	{
		public static ColumnInfo FromProperty(PropertyInfo property)
		{
			EnforceValidAttributeTypes(property, out bool isWeekStat);

			if (isWeekStat)
			{
				return WeekStatColumnInfo.FromProperty(property);
			}

			return PropertyColumnInfo.FromProperty(property);
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
