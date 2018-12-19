using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities
{
	public abstract class SqlEntity
	{
		public virtual string CreateTableCommand()
		{
			return SqlEntityCommandBuilder.CreateTable(this.GetType());
		}
	}
}
