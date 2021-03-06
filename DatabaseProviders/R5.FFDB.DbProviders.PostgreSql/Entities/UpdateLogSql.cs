﻿using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.Internals.PostgresMapper.Attributes;
using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Entities
{
	[Table(TableName.UpdateLog)]
	[CompositePrimaryKeys("season", "week")]
	public class UpdateLogSql
	{
		[NotNull]
		[Column("season", PostgresDataType.INT)]
		public int Season { get; set; }

		[NotNull]
		[Column("week", PostgresDataType.INT)]
		public int Week { get; set; }

		[NotNull]
		[Column("datetime", PostgresDataType.TIMESTAMPTZ)]
		public DateTimeOffset UpdateTime { get; set; }
	}
}
