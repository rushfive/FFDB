using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities
{
	[TableName("players")]
	public class PlayerSql : SqlEntity
	{
		[PrimaryKey]
		[Column("id", PostgresDataType.UUID)]
		public Guid Id { get; set; }

		[NotNull]
		[Column("nfl_id", PostgresDataType.TEXT)]
		public string NflId { get; set; }

		[Column("esb_id", PostgresDataType.TEXT)]
		public string EsbId { get; set; }

		[Column("gsis_id", PostgresDataType.TEXT)]
		public string GsisId { get; set; }

		[NotNull]
		[Column("first_name", PostgresDataType.TEXT)]
		public string FirstName { get; set; }

		[Column("last_name", PostgresDataType.TEXT)]
		public string LastName { get; set; }

		[Column("position", PostgresDataType.TEXT)]
		public Position Position { get; set; }

		[Column("number", PostgresDataType.INT)]
		public int Number { get; set; }

		[Column("height", PostgresDataType.INT)]
		public int Height { get; set; }

		[Column("weight", PostgresDataType.INT)]
		public int Weight { get; set; }

		[Column("date_of_birth", PostgresDataType.DATE)]
		public DateTimeOffset DateOfBirth { get; set; }

		[Column("college", PostgresDataType.TEXT)]
		public string College { get; set; }

		public override string InsertCommand()
		{
			return SqlCommandBuilder.Rows.Insert(this);
		}
	}
}
