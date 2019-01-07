using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities
{
	[TableName(Table.UpdateLog)]
	[CompositePrimaryKeys("season", "week")]
	public class UpdateLogSql : SqlEntity
	{
		[NotNull]
		[Column("season", PostgresDataType.INT)]
		public int Season { get; set; }

		[NotNull]
		[Column("week", PostgresDataType.INT)]
		public int Week { get; set; }

		[Column("update_time", PostgresDataType.TIMESTAMPTZ)]
		public DateTimeOffset UpdateTime { get; set; }

		public override string UpdateWhereClause()
		{
			throw new NotImplementedException();
		}
	}
}
