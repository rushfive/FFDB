using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Database.DbContext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.DatabaseContext
{
	public class DbContext : DbContextBase, IDatabaseContext
	{
		public ITeamDatabaseContext Team { get; }
		public IPlayerDatabaseContext Player { get; }
		public IWeekStatsDatabaseContext Stats { get; }
		public ILogDatabaseContext Log { get; }

		public DbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
			Team = new TeamDbContext(getDatabase, loggerFactory);
			Player = new PlayerDbContext(getDatabase, loggerFactory);
			Stats = new WeekStatsDbContext(getDatabase, loggerFactory);
			Log = new LogDbContext(getDatabase, loggerFactory);
		}

		public Task InitializeAsync()
		{
			throw new NotImplementedException();
		}
	}

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
