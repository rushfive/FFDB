﻿using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.Internals.PostgresMapper.Attributes;
using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Entities
{
	[Table(TableName.PlayerTeamMap)]
	[CompositePrimaryKeys("player_id", "team_id")]
	public class PlayerTeamMapSql
	{
		[NotNull]
		[ForeignKey(typeof(PlayerSql), "id")]
		[Column("player_id", PostgresDataType.UUID)]
		public Guid PlayerId { get; set; }

		[NotNull]
		[ForeignKey(typeof(TeamSql), "id")]
		[Column("team_id", PostgresDataType.INT)]
		public int TeamId { get; set; }

		public static PlayerTeamMapSql ToSqlEntity(Guid playerId, int teamId)
		{
			return new PlayerTeamMapSql
			{
				PlayerId = playerId,
				TeamId = teamId
			};
		}
	}
}
