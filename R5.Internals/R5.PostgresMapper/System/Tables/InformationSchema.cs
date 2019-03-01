using R5.Internals.PostgresMapper.Attributes;
using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.Internals.PostgresMapper.System.Tables
{
	public static class InformationSchema
	{
		[Table("information_schema.schemata")]
		public class Schemata
		{
			[Column("schemata", PostgresDataType.SQL_IDENTIFIER)]
			public string SchemaName { get; set; }
		}
	}
}
