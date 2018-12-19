using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models
{
	public enum PostgresDataType
	{
		UUID,
		TEXT,
		INT,
		TIMESTAMPTZ,
		FLOAT8
	}
}
