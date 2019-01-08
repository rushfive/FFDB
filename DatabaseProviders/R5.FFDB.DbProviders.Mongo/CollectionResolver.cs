using MongoDB.Driver;
using R5.FFDB.Components;
using R5.FFDB.DbProviders.Mongo.Documents;
using R5.FFDB.DbProviders.Mongo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.Mongo
{
	public static class CollectionResolver
	{
		public static string CollectionNameFor<T>()
			where T : DocumentBase
		{
			var documentType = typeof(T);

			CollectionNameAttribute attr = documentType.GetCustomAttributeOrNull<CollectionNameAttribute>();

			if (attr == null)
			{
				throw new InvalidOperationException($"Entity '{documentType.Name}' is missing its table name.");
			}

			return attr.Name;
		}

		public static IMongoCollection<T> GetCollectionFor<T>(IMongoDatabase database)
			where T : DocumentBase
		{
			var name = CollectionNameFor<T>();

			return database.GetCollection<T>(name);
		}
	}
}
