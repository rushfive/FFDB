using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities
{
	[TableName("week_stats")]
	public class WeekStatsSql : SqlEntity
	{
		[NotNull]
		[ForeignKey(typeof(PlayerSql), "id")]
		[Column("player_id", PostgresDataType.UUID)]
		public Guid PlayerId { get; set; }

		// Can be null, as safety in case we can't resolve it from sources
		[ForeignKey(typeof(TeamSql), "id")]
		[Column("team_id", PostgresDataType.INT)]
		public int TeamId { get; set; }

		[NotNull]
		[Column("season", PostgresDataType.INT)]
		public int Season { get; set; }

		[NotNull]
		[Column("week", PostgresDataType.INT)]
		public int Week { get; set; }

		// stat type properties ommitted because theres way too many
		// dynamically resolve columns from the 'WeekStatType'
		// enum - the canonical source for all the stat types

		

		public override string CreateTableCommand()
		{
			throw new NotImplementedException();
			//List<PropertyColumnInfo> statTypeColumns = Enum.GetNames(typeof(WeekStatType))
			//	.Select(statType => new PropertyColumnInfo(statType, PostgresDataType.FLOAT8))
			//	//{
			//	//	Name = statType,
			//	//	DataType = PostgresDataType.FLOAT8
			//	//})
			//	.ToList();

			//return SqlEntityCommandBuilder.CreateTable(this.GetType(), statTypeColumns);
		}
	}
}
