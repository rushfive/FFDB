using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Database;
using R5.FFDB.Database.DbContext;
using R5.FFDB.DbProviders.Mongo.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.Mongo.DatabaseProvider
{
	public class MongoDbProvider : IDatabaseProvider
	{

		private MongoConfig _config { get; }
		private ILoggerFactory _loggerFactory { get; }

		// use a single instance for the entire lifetime
		private IMongoClient _client { get; }

		public MongoDbProvider(
			MongoConfig config,
			ILoggerFactory loggerFactory)
		{
			_config = config;
			_loggerFactory = loggerFactory;

			_client = new MongoClient(config.ConnectionString);
		}

		public IDatabaseContext GetContext()
		{
			return new DbContext(GetDatabase, _loggerFactory);
		}

		private IMongoDatabase GetDatabase()
		{
			return _client.GetDatabase(_config.DatabaseName);
		}
	}
}
