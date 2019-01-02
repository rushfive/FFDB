using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities
{
	[TableName("ffdb.update_log")]
	[CompositePrimaryKeys("season", "week")]
	public class UpdateLogSql : SqlEntity
	{
		[NotNull]
		[Column("season", PostgresDataType.INT)]
		public int Season { get; set; }

		[NotNull]
		[Column("week", PostgresDataType.INT)]
		public int Week { get; set; }

		[Column("date_of_birth", PostgresDataType.TIMESTAMPTZ)]
		public DateTimeOffset DateOfBirth { get; set; }
	}
}
