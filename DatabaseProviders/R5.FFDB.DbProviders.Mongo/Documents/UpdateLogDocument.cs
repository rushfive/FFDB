﻿using System;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using R5.FFDB.DbProviders.Mongo.Collections;

namespace R5.FFDB.DbProviders.Mongo.Documents
{
	[CollectionName(Collection.UpdateLog)]
	public class UpdateLogDocument : DocumentBase
	{
		[BsonElement("season")]
		public int Season { get; set; }

		[BsonElement("week")]
		public int Week { get; set; }

		[BsonElement("dateTime")]
		public DateTimeOffset UpdateTime { get; set; }

		public static Task CreateIndexAsync(IMongoDatabase database)
		{
			// compound index
			var keys = Builders<UpdateLogDocument>.IndexKeys
				.Ascending(t => t.Week)
				.Ascending(t => t.Season);

			var options = new CreateIndexOptions { Unique = true };

			var model = new CreateIndexModel<UpdateLogDocument>(keys, options);

			var collection = CollectionResolver.Get<UpdateLogDocument>(database);

			return collection.Indexes.CreateOneAsync(model);
		}
	}
}
