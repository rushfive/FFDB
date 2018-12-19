using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models
{
	public abstract class SqlEntity
	{
		
		public abstract string InsertCommand();
	}
}
