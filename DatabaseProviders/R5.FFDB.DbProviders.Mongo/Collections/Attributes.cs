using System;

namespace R5.FFDB.DbProviders.Mongo.Collections
{
	public class CollectionNameAttribute : Attribute
	{
		public string Name { get; }

		public CollectionNameAttribute(string name)
		{
			Name = name;
		}
	}
}
