using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.Mongo.DatabaseContext
{
	public abstract class DbContextBase
	{
		private Func<IMongoDatabase> _getDatabase { get; }
		private ILoggerFactory _loggerFactory { get; }

		protected DbContextBase(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
		{
			_getDatabase = getDatabase;
			_loggerFactory = loggerFactory;
		}

		protected IMongoDatabase GetDatabase()
		{
			return _getDatabase();
		}

		protected MongoDbContext GetMongoDbContext()
		{
			return new MongoDbContext(_getDatabase());
		}

		protected ILogger<T> GetLogger<T>()
		{
			return _loggerFactory.CreateLogger<T>();
		}
	}
}
