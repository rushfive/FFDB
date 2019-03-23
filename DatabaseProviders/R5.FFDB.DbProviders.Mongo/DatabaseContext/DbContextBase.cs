using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Components;
using R5.FFDB.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.Mongo.DatabaseContext
{
	public abstract class DbContextBase
	{
		protected Func<IMongoDatabase> GetDatabase { get; }
		protected IAppLogger Logger { get; }

		protected DbContextBase(
			Func<IMongoDatabase> getDatabase,
			IAppLogger logger)
		{
			GetDatabase = getDatabase;
			Logger = logger;
		}

		protected MongoDbContext GetMongoDbContext()
		{
			return new MongoDbContext(GetDatabase());
		}
	}
}
