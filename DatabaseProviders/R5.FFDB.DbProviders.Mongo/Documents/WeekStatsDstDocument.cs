using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using R5.FFDB.Core;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.Documents
{
	[CollectionName("ffdb.weekStatsDst")]
	public class WeekStatsDstDocument : DocumentBase
	{
		[BsonElement("teamId")]
		public int TeamId { get; set; }

		[BsonElement("season")]
		public int Season { get; set; }

		[BsonElement("week")]
		public int Week { get; set; }

		[BsonElement("stats")]
		public Dictionary<WeekStatType, double> Stats { get; set; }

		public static WeekStatsDstDocument FromCoreEntity(PlayerStats stats,
			int teamId, WeekInfo week)
		{
			var dstStats = new Dictionary<WeekStatType, double>();
			foreach (KeyValuePair<WeekStatType, double> statKv in stats.Stats)
			{
				if (WeekStatCategory.DST.Contains(statKv.Key))
				{
					dstStats[statKv.Key] = statKv.Value;
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

		public override Task CreateIndexAsync(IMongoDatabase database)
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
