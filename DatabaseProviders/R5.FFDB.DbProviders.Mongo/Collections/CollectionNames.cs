using R5.FFDB.Core.Extensions;
using R5.FFDB.DbProviders.Mongo.Documents;
using R5.FFDB.DbProviders.Mongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.DbProviders.Mongo.Collections
{
	public static class CollectionNames
	{
		public static List<string> GetAll()
		{
			return CollectionResolver.GetDocumentTypes()
				.Select(GetForType)
				.ToList();
		}

		public static string GetForType<T>()
			where T : DocumentBase
		{
			return GetForType(typeof(T));
		}

		public static string GetForType(Type type)
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
	}
}
