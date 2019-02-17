using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo.Collections;
using System;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.Documents
{
	[CollectionName(Collection.Player)]
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

		public static PlayerDocument FromCoreAddEntity(PlayerAdd add)
		{
			return new PlayerDocument
			{
				Id = Guid.NewGuid(),
				NflId = add.NflId,
				EsbId = add.EsbId,
				GsisId = add.GsisId,
				FirstName = add.FirstName,
				LastName = add.LastName,
				Position = add.Position,
				Status = add.Status,
				Number = add.Number,
				Height = add.Height,
				Weight = add.Weight,
				DateOfBirth = add.DateOfBirth,
				College = add.College,
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
				LastName = document.LastName
			};
		}

		public static Task CreateIndexAsync(IMongoDatabase database)
		{
			var keys = Builders<PlayerDocument>.IndexKeys.Ascending(t => t.Id);

			var model = new CreateIndexModel<PlayerDocument>(keys);

			var collection = CollectionResolver.Get<PlayerDocument>(database);
			collection.Indexes.CreateOne(model);

			return collection.Indexes.CreateOneAsync(model);
		}
	}
}
