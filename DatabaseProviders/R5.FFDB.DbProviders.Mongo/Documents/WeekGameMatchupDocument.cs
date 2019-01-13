using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo.Collections;
using R5.FFDB.DbProviders.Mongo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.Documents
{
	[CollectionName(CollectionConstants.FfdbPrefix + "weekGameMatchup")]
	public class WeekGameMatchupDocument : DocumentBase
	{
		[BsonElement("season")]
		public int Season { get; set; }

		[BsonElement("week")]
		public int Week { get; set; }

		[BsonElement("homeTeamId")]
		public int HomeTeamId { get; set; }

		[BsonElement("awayTeamId")]
		public int AwayTeamId { get; set; }

		[BsonElement("nflGameId")]
		public string NflGameId { get; set; }

		[BsonElement("gsisGameId")]
		public string GsisGameId { get; set; }
		
		public static WeekGameMatchupDocument FromCoreEntity(WeekGameMatchup entity)
		{
			return new WeekGameMatchupDocument
			{
				Season = entity.Season,
				Week = entity.Week,
				HomeTeamId = entity.HomeTeamId,
				AwayTeamId = entity.AwayTeamId,
				NflGameId = entity.NflGameId,
				GsisGameId = entity.GsisGameId
			};
		}

		public static Task CreateIndexAsync(IMongoDatabase database)
		{
			var keys = Builders<WeekGameMatchupDocument>.IndexKeys
				.Ascending(t => t.HomeTeamId)
				.Ascending(t => t.AwayTeamId)
				.Ascending(t => t.Week)
				.Ascending(t => t.Season);

			var options = new CreateIndexOptions { Unique = true };

			var model = new CreateIndexModel<WeekGameMatchupDocument>(keys);

			var collection = CollectionResolver.GetCollectionFor<WeekGameMatchupDocument>(database);
			collection.Indexes.CreateOne(model);

			return collection.Indexes.CreateOneAsync(model);
		}
	}
}
