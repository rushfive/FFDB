using MongoDB.Driver;
using R5.FFDB.Components;
using R5.FFDB.DbProviders.Mongo.Documents;
using R5.FFDB.DbProviders.Mongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.Mongo.Collections
{
	public static class CollectionResolver
	{
		public static List<Type> GetDocumentTypes()
		{
			return Assembly.GetAssembly(typeof(DocumentBase))
				.GetTypes()
				.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(DocumentBase))
					&& t.GetCustomAttributeOrNull<CollectionNameAttribute>() != null)
				.ToList();
		}

		public static IMongoCollection<T> GetCollectionFor<T>(IMongoDatabase database)
			where T : DocumentBase
		{
			var name = CollectionNames.GetForType<T>();

			return database.GetCollection<T>(name);
		}

		
	}
}
