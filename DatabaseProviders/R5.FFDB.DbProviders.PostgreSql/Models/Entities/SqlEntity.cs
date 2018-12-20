using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities
{
	public abstract class SqlEntity
	{
		public string CreateTableCommand()
		{
			return SqlCommandBuilder.Table.Create(this.GetType());
		}
	}
}
