using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using R5.FFDB.Core;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo.Collections;
using R5.FFDB.DbProviders.Mongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.Documents
{
	[CollectionName(CollectionConstants.FfdbPrefix + "weekStatsDst")]
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

		public static WeekStatsDstDocument FromCoreEntity(int teamId, WeekInfo week, 
			IEnumerable<KeyValuePair<WeekStatType, double>> stats)
		{
			var dstStats = new Dictionary<MongoWeekStatType, double>();
			foreach (KeyValuePair<WeekStatType, double> statKv in stats)
			{
				if (WeekStatCategory.DST.Contains(statKv.Key))
				{
					dstStats[(MongoWeekStatType)statKv.Key] = statKv.Value;
				}
			}

			return new WeekStatsDstDocument
			{
				TeamId = teamId,
				Season = week.Season,
				Week = week.Week,
				Stats = dstStats
			};
		}

		public static IEnumerable<KeyValuePair<WeekStatType, double>> FilterStatValues(PlayerWeekStats stats)
		{
			return stats.Stats.Where(kv => WeekStatCategory.DST.Contains(kv.Key));
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

			var collection = CollectionResolver.GetCollectionFor<WeekStatsDstDocument>(database);

			return collection.Indexes.CreateOneAsync(model);
		}
	}
}
