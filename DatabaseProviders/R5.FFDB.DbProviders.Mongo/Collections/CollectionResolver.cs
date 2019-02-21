using MongoDB.Driver;
using R5.FFDB.DbProviders.Mongo.Documents;
using R5.Internals.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace R5.FFDB.DbProviders.Mongo.Collections
{
	public static class CollectionResolver
	{
		public static List<string> GetAllNames()
		{
			return CollectionResolver.GetDocumentTypes()
				.Select(GetName)
				.ToList();
		}

		public static string GetName<T>()
			where T : DocumentBase
		{
			return GetName(typeof(T));
		}

		public static string GetName(Type type)
		{
			if (!type.IsClass || type.IsAbstract || !type.IsSubclassOf(typeof(DocumentBase)))
			{
				throw new ArgumentException($"Type must be a non-abstract class deriving from '{nameof(DocumentBase)}'.");
			}

			CollectionNameAttribute attr = type.GetCustomAttributeOrNull<CollectionNameAttribute>();

			if (attr == null)
			{
				throw new InvalidOperationException($"Document '{type.Name}' is missing its collection name.");
			}

			return attr.Name;
		}

		public static List<Type> GetDocumentTypes()
		{
			return Assembly.GetAssembly(typeof(DocumentBase))
				.GetTypes()
				.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(DocumentBase))
					&& t.GetCustomAttributeOrNull<CollectionNameAttribute>() != null)
				.ToList();
		}

		public static IMongoCollection<T> Get<T>(IMongoDatabase database)
			where T : DocumentBase
		{
			var name = GetName<T>();

			return database.GetCollection<T>(name);
		}
	}
}
