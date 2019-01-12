using MongoDB.Bson;
using MongoDB.Driver;
using R5.FFDB.DbProviders.Mongo.Collections;
using R5.FFDB.DbProviders.Mongo.Documents;
using R5.FFDB.DbProviders.Mongo.Serialization;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo
{
	public class MongoDbContext
	{
		private IMongoDatabase _database { get; }

		public MongoDbContext(IMongoDatabase database)
		{
			_database = database;
		}

		public Task InsertOneAsync<T>(T document)
			where T : DocumentBase
		{
			var collection = CollectionResolver.GetCollectionFor<T>(_database);
			return collection.InsertOneAsync(document);
		}

		public Task InsertManyAsync<T>(List<T> documents)
			where T : DocumentBase
		{
			var collection = CollectionResolver.GetCollectionFor<T>(_database);
			return collection.InsertManyAsync(documents);
		}

		public async Task<T> FindOneAsync<T>(FilterDefinition<T> filter = null,
			FindOptions<T> findOptions = null)
			where T : DocumentBase
		{
			var collection = CollectionResolver.GetCollectionFor<T>(_database);

			if (filter == null)
			{
				// provide empty filter to fetch all
				filter = new BsonDocumentFilterDefinition<T>(new MongoDB.Bson.BsonDocument());
			}

			var asyncCursor = await collection.FindAsync(filter, findOptions).ConfigureAwait(false);
			return await asyncCursor.SingleOrDefaultAsync().ConfigureAwait(false);
		}

		public async Task<T> FindOneAsync<T>(Expression<Func<T, bool>> filter,
			FindOptions<T> findOptions = null)
			where T : DocumentBase
		{
			var collection = CollectionResolver.GetCollectionFor<T>(_database);

			var asyncCursor = await collection.FindAsync(filter, findOptions).ConfigureAwait(false);
			return await asyncCursor.SingleOrDefaultAsync().ConfigureAwait(false);
		}

		public async Task<List<T>> FindAsync<T>(FilterDefinition<T> filter = null,
			FindOptions<T> findOptions = null)
			where T : DocumentBase
		{
			var cursor = await this.FindWithCursorAsync(filter, findOptions);
			return await cursor.ToListAsync().ConfigureAwait(false);
		}

		public async Task<List<T>> FindAsync<T>(Expression<Func<T, bool>> filter,
			FindOptions<T> findOptions = null)
			where T : DocumentBase
		{
			var cursor = await this.FindWithCursorAsync(filter, findOptions);
			return await cursor.ToListAsync().ConfigureAwait(false);
		}

		private Task<IAsyncCursor<T>> FindWithCursorAsync<T>(FilterDefinition<T> filter = null,
			FindOptions<T> findOptions = null)
			where T : DocumentBase
		{
			var collection = CollectionResolver.GetCollectionFor<T>(_database);

			if (filter == null)
			{
				// provide empty filter to fetch all
				filter = new BsonDocumentFilterDefinition<T>(new MongoDB.Bson.BsonDocument());
			}

			if (findOptions == null)
			{
				findOptions = new FindOptions<T> { NoCursorTimeout = true };
			}

			return collection.FindAsync(filter, findOptions);
		}

		public Task<DeleteResult> DeleteOneAsync<T>(FilterDefinition<T> filter)
			where T : DocumentBase
		{
			var collection = CollectionResolver.GetCollectionFor<T>(_database);
			return collection.DeleteOneAsync(filter);
		}

		public Task<DeleteResult> DeleteOneAsync<T>(Expression<Func<T, bool>> filter)
			where T : DocumentBase
		{
			var collection = CollectionResolver.GetCollectionFor<T>(_database);
			return collection.DeleteOneAsync(filter);
		}

		public Task<DeleteResult> DeleteManyAsync<T>(FilterDefinition<T> filter = null)
			where T : DocumentBase
		{
			if (filter == null)
			{
				// empty filter to delete all
				filter = new BsonDocumentFilterDefinition<T>(new MongoDB.Bson.BsonDocument());
			}

			var collection = CollectionResolver.GetCollectionFor<T>(_database);
			return collection.DeleteManyAsync(filter);
		}

		public Task<DeleteResult> DeleteManyAsync<T>(Expression<Func<T, bool>> filter)
			where T : DocumentBase
		{
			var collection = CollectionResolver.GetCollectionFor<T>(_database);
			return collection.DeleteManyAsync(filter);
		}

		public Task<UpdateResult> UpdateOneAsync<T>(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition,
			UpdateOptions updateOptions = null)
			where T : DocumentBase
		{
			var collection = CollectionResolver.GetCollectionFor<T>(_database);
			return collection.UpdateOneAsync(filter, updateDefinition, updateOptions);
		}

		public Task<UpdateResult> UpdateOneAsync<T>(Expression<Func<T, bool>> filter,
			UpdateDefinition<T> updateDefinition, UpdateOptions updateOptions = null)
			where T : DocumentBase
		{
			var collection = CollectionResolver.GetCollectionFor<T>(_database);
			return collection.UpdateOneAsync(filter, updateDefinition, updateOptions);
		}

		public Task<UpdateResult> UpdateAsync<T>(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition,
			UpdateOptions updateOptions = null)
			where T : DocumentBase
		{
			var collection = CollectionResolver.GetCollectionFor<T>(_database);
			return collection.UpdateManyAsync(filter, updateDefinition, updateOptions);
		}

		public Task<UpdateResult> UpdateAsync<T>(UpdateDefinition<T> updateDefinition,
			FilterDefinition<T> filter = null,
			UpdateOptions updateOptions = null)
			where T : DocumentBase
		{
			if (filter == null)
			{
				//empty filter
				filter = new BsonDocumentFilterDefinition<T>(new BsonDocument());
			}

			var collection = CollectionResolver.GetCollectionFor<T>(_database);
			return collection.UpdateManyAsync(filter, updateDefinition, updateOptions);
		}

		public Task<ReplaceOneResult> ReplaceOneAsync<T>(FilterDefinition<T> filter, T document,
			UpdateOptions updateOptions = null)
			where T : DocumentBase
		{
			var collection = CollectionResolver.GetCollectionFor<T>(_database);
			return collection.ReplaceOneAsync(filter, document, updateOptions);
		}

		public Task<ReplaceOneResult> ReplaceOneAsync<T>(Expression<Func<T, bool>> filter, T document,
			UpdateOptions updateOptions = null)
			where T : DocumentBase
		{
			var collection = CollectionResolver.GetCollectionFor<T>(_database);
			return collection.ReplaceOneAsync(filter, document, updateOptions);
		}
	}
}
