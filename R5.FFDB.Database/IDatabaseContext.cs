using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Database
{
	public interface IDatabaseContext
	{
		DatabaseType Type { get; }
	}

	// regardless of db type, they should all contain the SAME operations
	// since we are saving and updating the same stuff

	// todo: move implementations
	public class PostgresDatabaseContext : IDatabaseContext
	{
		public DatabaseType Type => DatabaseType.Postgres;
	}

	public class MongoDatabaseContext : IDatabaseContext
	{
		public DatabaseType Type => DatabaseType.Mongo;
	}


}
