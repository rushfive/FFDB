using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Components;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.DbProviders.Mongo.DatabaseContext;
using R5.FFDB.DbProviders.Mongo.Serialization;

namespace R5.FFDB.DbProviders.Mongo
{
	public class MongoDbProvider : IDatabaseProvider
	{
		private MongoConfig _config { get; }
		private IAppLogger _logger { get; }

		// use a single instance for the entire lifetime
		private IMongoClient _client { get; }

		public MongoDbProvider(
			MongoConfig config,
			IAppLogger logger)
		{
			_config = config;
			_logger = logger;

			// must be registered before first time db is used, or it'll
			// initialize some clashing default serializers
			MongoSerializers.Register();

			_client = new MongoClient(config.ConnectionString);
		}

		public IDatabaseContext GetContext()
		{
			return new DbContext(GetDatabase, _logger);
		}

		private IMongoDatabase GetDatabase()
		{
			return _client.GetDatabase(_config.DatabaseName);
		}
	}
}
