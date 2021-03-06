﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using R5.FFDB.Core.Entities;
using R5.FFDB.DbProviders.Mongo.Collections;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.Documents
{
	[CollectionName(Collection.Team)]
	public class TeamDocument : DocumentBase
	{
		[BsonId]
		public int Id { get; set; }

		[BsonElement("nflId")]
		public string NflId { get; set; }

		[BsonElement("name")]
		public string Name { get; set; }

		[BsonElement("abbreviation")]
		public string Abbreviation { get; set; }

		public static TeamDocument FromCoreEntity(Team entity)
		{
			return new TeamDocument
			{
				Id = entity.Id,
				NflId = entity.NflId,
				Name = entity.Name,
				Abbreviation = entity.Abbreviation
			};
		}

		public static Task CreateIndexAsync(IMongoDatabase database)
		{
			var keys = Builders<TeamDocument>.IndexKeys.Ascending(t => t.Id);

			var model = new CreateIndexModel<TeamDocument>(keys);

			var collection = CollectionResolver.Get<TeamDocument>(database);
			collection.Indexes.CreateOne(model);

			return collection.Indexes.CreateOneAsync(model);
		}
	}
}
