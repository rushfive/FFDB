using R5.Internals.PostgresMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.Internals.PostgresMapper.Models
{
	public abstract class SqlEntity
	{
		//public abstract string TableName { get; }
		public string TableName => MetadataResolver.GetTableName(this.GetType());
		public List<TableColumn> Columns() => MetadataResolver.GetColumns(this.GetType());
	}


	[Table("Test")]
	public class TestEntity : SqlEntity
	{
		[Column("String")]
		public string String { get; set; }

		[Column("Int")]
		public int Int { get; set; }

		[Column("NullableDouble")]
		public double? NullableDouble { get; set; }
	}

	[Table("Test2")]
	public class TestEntity2 : SqlEntity
	{
		public string String { get; set; }

		[Column("Int")]
		public int Int { get; set; }

		[Column("NullableDouble")]
		public double? NullableDouble { get; set; }
	}
}
