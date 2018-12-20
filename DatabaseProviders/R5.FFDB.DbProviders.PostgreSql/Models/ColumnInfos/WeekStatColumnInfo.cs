using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.ColumnInfos
{
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
