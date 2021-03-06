﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using R5.FFDB.Core;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo.Collections;
using R5.FFDB.DbProviders.Mongo.Models;

namespace R5.FFDB.DbProviders.Mongo.Documents
{
	[CollectionName(Collection.WeekStatsPlayer)]
	public class WeekStatsPlayerDocument : DocumentBase
	{
		[BsonElement("playerId")]
		public Guid PlayerId { get; set; }

		[BsonElement("teamId")]
		public int? TeamId { get; set; }

		[BsonElement("season")]
		public int Season { get; set; }

		[BsonElement("week")]
		public int Week { get; set; }

		[BsonElement("stats")]
		public Dictionary<MongoWeekStatType, double> Stats { get; set; }

		[BsonElement("hasPass")]
		public bool HasPass { get; set; }

		[BsonElement("hasRush")]
		public bool HasRush { get; set; }

		[BsonElement("hasReceive")]
		public bool HasReceive { get; set; }

		[BsonElement("hasReturn")]
		public bool HasReturn { get; set; }

		[BsonElement("hasMisc")]
		public bool HasMisc { get; set; }

		[BsonElement("hasKick")]
		public bool HasKick { get; set; }

		[BsonElement("hasIdp")]
		public bool HasIdp { get; set; }

		public static WeekStatsPlayerDocument FromCoreEntity(PlayerWeekStats stats, Dictionary<string, Guid> nflIdMap)
		{
			var result = new WeekStatsPlayerDocument
			{
				PlayerId = nflIdMap[stats.NflId],
				TeamId = stats.TeamId,
				Season = stats.Week.Season,
				Week = stats.Week.Week
			};

			var playerStats = new Dictionary<MongoWeekStatType, double>();
			foreach (KeyValuePair<WeekStatType, double> statKv in stats.Stats)
			{
				if (WeekStatCategory.DST.Contains(statKv.Key))
				{
					continue;
				}

				if (WeekStatCategory.Pass.Contains(statKv.Key))
				{
					result.HasPass = true;
				}
				if (WeekStatCategory.Rush.Contains(statKv.Key))
				{
					result.HasRush = true;
				}
				if (WeekStatCategory.Receive.Contains(statKv.Key))
				{
					result.HasReceive = true;
				}
				if (WeekStatCategory.Return.Contains(statKv.Key))
				{
					result.HasReturn = true;
				}
				if (WeekStatCategory.Misc.Contains(statKv.Key))
				{
					result.HasMisc= true;
				}
				if (WeekStatCategory.Kick.Contains(statKv.Key))
				{
					result.HasKick = true;
				}
				if (WeekStatCategory.IDP.Contains(statKv.Key))
				{
					result.HasIdp = true;
				}

				playerStats[(MongoWeekStatType)statKv.Key] = statKv.Value;
			}

			result.Stats = playerStats;
			return result;
		}

		public static Task CreateIndexAsync(IMongoDatabase database)
		{
			// compound index
			var keys = Builders<WeekStatsPlayerDocument>.IndexKeys
				.Ascending(t => t.PlayerId)
				.Ascending(t => t.Week)
				.Ascending(t => t.Season);

			var options = new CreateIndexOptions { Unique = true };

			var model = new CreateIndexModel<WeekStatsPlayerDocument>(keys, options);

			var collection = CollectionResolver.Get<WeekStatsPlayerDocument>(database);

			return collection.Indexes.CreateOneAsync(model);
		}
	}
}
