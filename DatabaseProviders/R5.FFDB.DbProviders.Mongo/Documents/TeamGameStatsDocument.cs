using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo.Collections;
using R5.FFDB.DbProviders.Mongo.Models;

namespace R5.FFDB.DbProviders.Mongo.Documents
{
	[CollectionName(CollectionConstants.FfdbPrefix + "teamGameStats")]
	public class TeamGameStatsDocument : DocumentBase
	{
		[BsonElement("teamId")]
		public int TeamId { get; set; }

		[BsonElement("season")]
		public int Season { get; set; }

		[BsonElement("week")]
		public int Week { get; set; }
		
		[BsonElement("pointsFirstQuarter")]
		public int PointsFirstQuarter { get; set; }

		[BsonElement("pointsSecondQuarter")]
		public int PointsSecondQuarter { get; set; }

		[BsonElement("pointsThirdQuarter")]
		public int PointsThirdQuarter { get; set; }

		[BsonElement("pointsFourthQuarter")]
		public int PointsFourthQuarter { get; set; }

		[BsonElement("pointsOverTime")]
		public int PointsOverTime { get; set; }

		[BsonElement("pointsTotal")]
		public int PointsTotal { get; set; }

		[BsonElement("firstDowns")]
		public int FirstDowns { get; set; }

		[BsonElement("totalYards")]
		public int TotalYards { get; set; }

		[BsonElement("passingYards")]
		public int PassingYards { get; set; }

		[BsonElement("rushingYards")]
		public int RushingYards { get; set; }

		[BsonElement("penalties")]
		public int Penalties { get; set; }

		[BsonElement("penaltyYards")]
		public int PenaltyYards { get; set; }

		[BsonElement("turnovers")]
		public int Turnovers { get; set; }

		[BsonElement("punts")]
		public int Punts { get; set; }

		[BsonElement("puntYards")]
		public int PuntYards { get; set; }

		[BsonElement("timeOfPossessionSeconds")]
		public int TimeOfPossessionSeconds { get; set; }

		public static TeamGameStatsDocument FromCoreEntity(TeamWeekStats stats)
		{
			return new TeamGameStatsDocument
			{
				TeamId = stats.TeamId,
				Season = stats.Week.Season,
				Week = stats.Week.Week,
				PointsFirstQuarter = stats.PointsFirstQuarter,
				PointsSecondQuarter = stats.PointsSecondQuarter,
				PointsThirdQuarter = stats.PointsThirdQuarter,
				PointsFourthQuarter = stats.PointsFourthQuarter,
				PointsOverTime = stats.PointsOverTime,
				PointsTotal = stats.PointsTotal,
				FirstDowns = stats.FirstDowns,
				TotalYards = stats.TotalYards,
				PassingYards = stats.PassingYards,
				RushingYards = stats.RushingYards,
				Penalties = stats.Penalties,
				PenaltyYards = stats.PenaltyYards,
				Turnovers = stats.Turnovers,
				Punts = stats.Punts,
				PuntYards = stats.PuntYards,
				TimeOfPossessionSeconds = stats.TimeOfPossessionSeconds
			};
		}

		public static Task CreateIndexAsync(IMongoDatabase database)
		{
			// compound index
			var keys = Builders<TeamGameStatsDocument>.IndexKeys
				.Ascending(t => t.TeamId)
				.Ascending(t => t.Week)
				.Ascending(t => t.Season);

			var options = new CreateIndexOptions { Unique = true };

			var model = new CreateIndexModel<TeamGameStatsDocument>(keys, options);

			var collection = CollectionResolver.GetCollectionFor<TeamGameStatsDocument>(database);

			return collection.Indexes.CreateOneAsync(model);
		}
	}
}
