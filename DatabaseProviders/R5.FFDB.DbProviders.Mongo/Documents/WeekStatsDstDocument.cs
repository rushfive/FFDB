using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using R5.FFDB.Core;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo.Collections;
using R5.FFDB.DbProviders.Mongo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.Documents
{
	[CollectionName(Collection.WeekStatsDst)]
	public class WeekStatsDstDocument : DocumentBase
	{
		[BsonElement("teamId")]
		public int TeamId { get; set; }

		[BsonElement("season")]
		public int Season { get; set; }

		[BsonElement("week")]
		public int Week { get; set; }

		[BsonElement("stats")]
		public Dictionary<MongoWeekStatType, double> Stats { get; set; }

		public static WeekStatsDstDocument FromCoreEntity(PlayerWeekStats stats)
		{
			int teamId = TeamDataStore.GetIdFromNflId(stats.NflId);

			var dstStats = new Dictionary<MongoWeekStatType, double>();
			foreach (KeyValuePair<WeekStatType, double> statKv in stats.Stats)
			{
				if (WeekStatCategory.DST.Contains(statKv.Key))
				{
					dstStats[(MongoWeekStatType)statKv.Key] = statKv.Value;
				}
			}

			return new WeekStatsDstDocument
			{
				TeamId = teamId,
				Season = stats.Week.Season,
				Week = stats.Week.Week,
				Stats = dstStats
			};
		}

		public static Task CreateIndexAsync(IMongoDatabase database)
		{
			// compound index
			var keys = Builders<WeekStatsDstDocument>.IndexKeys
				.Ascending(t => t.TeamId)
				.Ascending(t => t.Week)
				.Ascending(t => t.Season);

			var options = new CreateIndexOptions { Unique = true };

			var model = new CreateIndexModel<WeekStatsDstDocument>(keys, options);

			var collection = CollectionResolver.Get<WeekStatsDstDocument>(database);

			return collection.Indexes.CreateOneAsync(model);
		}
	}
}
