using System;

namespace R5.FFDB.DbProviders.Mongo.Models
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
