using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo.Collections;

namespace R5.FFDB.DbProviders.Mongo.Documents
{
	[CollectionName(Collection.WeekStatsTeam)]
	public class WeekStatsTeamDocument : DocumentBase
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

		public static TeamWeekStats ToCoreEntity(WeekStatsTeamDocument doc)
		{
			return new TeamWeekStats
			{
				TeamId = doc.TeamId,
				Week = new WeekInfo(doc.Season, doc.Week),
				PointsFirstQuarter = doc.PointsFirstQuarter,
				PointsSecondQuarter = doc.PointsSecondQuarter,
				PointsThirdQuarter = doc.PointsThirdQuarter,
				PointsFourthQuarter = doc.PointsFourthQuarter,
				PointsOverTime = doc.PointsOverTime,
				PointsTotal = doc.PointsTotal,
				FirstDowns = doc.FirstDowns,
				TotalYards = doc.TotalYards,
				PassingYards = doc.PassingYards,
				RushingYards = doc.RushingYards,
				Penalties = doc.Penalties,
				PenaltyYards = doc.PenaltyYards,
				Turnovers = doc.Turnovers,
				Punts = doc.Punts,
				PuntYards = doc.PuntYards,
				TimeOfPossessionSeconds = doc.TimeOfPossessionSeconds
			};
		}

		public static WeekStatsTeamDocument FromCoreEntity(TeamWeekStats entity)
		{
			return new WeekStatsTeamDocument
			{
				TeamId = entity.TeamId,
				Season = entity.Week.Season,
				Week = entity.Week.Week,
				PointsFirstQuarter = entity.PointsFirstQuarter,
				PointsSecondQuarter = entity.PointsSecondQuarter,
				PointsThirdQuarter = entity.PointsThirdQuarter,
				PointsFourthQuarter = entity.PointsFourthQuarter,
				PointsOverTime = entity.PointsOverTime,
				PointsTotal = entity.PointsTotal,
				FirstDowns = entity.FirstDowns,
				TotalYards = entity.TotalYards,
				PassingYards = entity.PassingYards,
				RushingYards = entity.RushingYards,
				Penalties = entity.Penalties,
				PenaltyYards = entity.PenaltyYards,
				Turnovers = entity.Turnovers,
				Punts = entity.Punts,
				PuntYards = entity.PuntYards,
				TimeOfPossessionSeconds = entity.TimeOfPossessionSeconds
			};
		}

		public static Task CreateIndexAsync(IMongoDatabase database)
		{
			// compound index
			var keys = Builders<WeekStatsTeamDocument>.IndexKeys
				.Ascending(t => t.TeamId)
				.Ascending(t => t.Week)
				.Ascending(t => t.Season);

			var options = new CreateIndexOptions { Unique = true };

			var model = new CreateIndexModel<WeekStatsTeamDocument>(keys, options);

			var collection = CollectionResolver.Get<WeekStatsTeamDocument>(database);

			return collection.Indexes.CreateOneAsync(model);
		}
	}
}
