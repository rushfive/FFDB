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
	[CollectionName(CollectionConstants.FfdbPrefix + "player")]
	public class PlayerDocument : DocumentBase
	{
		[BsonId]
		public Guid Id { get; set; }

		[BsonElement("nflId")]
		public string NflId { get; set; }

		[BsonElement("teamId")]
		public int? TeamId { get; set; }

		[BsonElement("esbId")]
		public string EsbId { get; set; }

		[BsonElement("gsisId")]
		public string GsisId { get; set; }

		[BsonElement("firstName")]
		public string FirstName { get; set; }

		[BsonElement("lastName")]
		public string LastName { get; set; }

		[BsonElement("position")]
		public Position? Position { get; set; }

		[BsonElement("number")]
		public int? Number { get; set; }

		[BsonElement("status")]
		public RosterStatus? Status { get; set; }

		[BsonElement("height")]
		public int Height { get; set; }

		[BsonElement("weight")]
		public int Weight { get; set; }

		[BsonElement("dateOfBirth")]
		public DateTimeOffset DateOfBirth { get; set; }

		[BsonElement("college")]
		public string College { get; set; }

		public static PlayerDocument FromCoreEntity(Player player)
		{
			return new PlayerDocument
			{
				Id = player.Id == Guid.Empty ? Guid.NewGuid() : player.Id,
				NflId = player.NflId,
				EsbId = player.EsbId,
				GsisId = player.GsisId,
				FirstName = player.FirstName,
				LastName = player.LastName,
				//Position = player.Position,
				//Status = player.Status,
				//Number = player.Number,
				//Height = player.Height,
				//Weight = player.Weight,
				//DateOfBirth = player.DateOfBirth,
				//College = player.College,
				TeamId = null
			};
		}

		public static Player ToCoreEntity(PlayerDocument document)
		{
			return new Player
			{
				Id = document.Id,
				NflId = document.NflId,
				EsbId = document.EsbId,
				GsisId = document.GsisId,
				FirstName = document.FirstName,
				LastName = document.LastName,
				//Height = document.Height,
				//Weight = document.Weight,
				//DateOfBirth = document.DateOfBirth,
				//College = document.College
			};
		}

		public static Task CreateIndexAsync(IMongoDatabase database)
		{
			var keys = Builders<PlayerDocument>.IndexKeys.Ascending(t => t.Id);

			var model = new CreateIndexModel<PlayerDocument>(keys);

			var collection = CollectionResolver.GetCollectionFor<PlayerDocument>(database);
			collection.Indexes.CreateOne(model);

			return collection.Indexes.CreateOneAsync(model);
		}
	}
}
