using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.Internals.PostgresMapper.Attributes;
using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Entities.WeekStats
{
	public abstract class WeekStatsSql// : SqlEntity
	{
		public abstract int Season { get; set; }
		public abstract int Week { get; set; }
	}

	//public class WeekStatColumnAttribute : EntityColumnAttribute
	//{
	//	public string Name { get; }
	//	public WeekStatType StatType { get; }

	//	public WeekStatColumnAttribute(string name, WeekStatType statType)
	//	{
	//		Name = name;
	//		StatType = statType;
	//	}

	//	public override void UpdateTableColumn(TableColumn column)
	//	{
	//		throw new NotImplementedException();
	//	}
	//}
}
