using R5.Internals.PostgresMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.Internals.PostgresMapper.Models
{
	// do we even really need this base class??
	//public abstract class SqlEntity
	//{
	//	//public abstract string TableName { get; }
	//	//public string TableName => MetadataResolver.TableName(this.GetType());
	//	//public List<TableColumn> Columns() => MetadataResolver.TableColumns(this.GetType());
	//}


	[Table("Test")]
	public class TestEntity// : SqlEntity
	{
		[Column("number1")]
		public int Number1 { get; set; }
		[Column("number2")]
		public int Number2 { get; set; }

		[Column("StringColName")]
		public string String { get; set; }

		[Column("IntColName")]
		public int Int { get; set; }

		[Column("NullableDoubleColName")]
		public double? NullableDouble { get; set; }

		[Column("NullableDoubleColName2")]
		public double? NullableDouble2 { get; set; }

		[Column("BoolColName")]
		public bool Bool { get; set; }

		[Column("DateTime")]
		public DateTime DateTime { get; set; }
	}

	[Table("Test2")]
	public class TestEntity2// : SqlEntity
	{
		public string String { get; set; }

		[Column("Int")]
		public int Int { get; set; }

		[Column("NullableDouble")]
		public double? NullableDouble { get; set; }
	}
}
