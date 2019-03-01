using R5.FFDB.Core.Entities;
using R5.Internals.PostgresMapper.Attributes;
using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Entities
{
	[Table(TableName.Team)]
	public class TeamSql : SqlEntity
	{
		[PrimaryKey]
		[Column("id", PostgresDataType.INT)]
		public int Id { get; set; }

		[NotNull]
		[Column("nfl_id", PostgresDataType.TEXT)]
		public string NflId { get; set; }

		[NotNull]
		[Column("name", PostgresDataType.TEXT)]
		public string Name { get; set; }

		[NotNull]
		[Column("abbreviation", PostgresDataType.TEXT)]
		public string Abbreviation { get; set; }

		public static TeamSql FromCoreEntity(Team entity)
		{
			return new TeamSql
			{
				Id = entity.Id,
				NflId = entity.NflId,
				Name = entity.Name,
				Abbreviation = entity.Abbreviation
			};
		}

		public override string PrimaryKeyMatchCondition()
		{
			return $"id = {Id}";
		}
	}
}
