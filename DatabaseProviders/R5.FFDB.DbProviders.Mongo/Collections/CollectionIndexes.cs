using MongoDB.Driver;
using R5.FFDB.DbProviders.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.Collections
{
	public static class CollectionIndexes
	{
		public static Task CreateForTypeAsync(Type documentType, IMongoDatabase db)
		{
			if (!_createIndexFuncMap.TryGetValue(documentType, out Func<IMongoDatabase, Task> createIndexFunc))
			{
				throw new InvalidOperationException($"Type '{documentType.Name}' doesn't have an associated create index func.");
			}

			return createIndexFunc(db);
		}

		private static Dictionary<Type, Func<IMongoDatabase, Task>> _createIndexFuncMap = new Dictionary<Type, Func<IMongoDatabase, Task>>
		{
			{ typeof(PlayerDocument), db => PlayerDocument.CreateIndexAsync(db) },
			{ typeof(TeamDocument), db => TeamDocument.CreateIndexAsync(db) },
			{ typeof(TeamGameStatsDocument), db => TeamGameStatsDocument.CreateIndexAsync(db) },
			{ typeof(UpdateLogDocument), db => UpdateLogDocument.CreateIndexAsync(db) },
			{ typeof(WeekStatsPlayerDocument), db => WeekStatsPlayerDocument.CreateIndexAsync(db) },
			{ typeof(WeekStatsDstDocument), db => WeekStatsDstDocument.CreateIndexAsync(db) }
		};
	}
}
