using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo.Models;
using System;

namespace R5.FFDB.DbProviders.Mongo.Serialization
{
	public static class MongoSerializers
	{
		public static void Register()
		{
			RegisterConventions();

			BsonSerializer.RegisterSerializer(new Serializers.GuidSerializer());
			BsonSerializer.RegisterSerializer(new NullableSerializer<Guid>(new Serializers.GuidSerializer()));
			BsonSerializer.RegisterSerializer(new EnumSerializer<MongoWeekStatType>(BsonType.String));
		}

		private static void RegisterConventions()
		{
			var cp = new ConventionPack
			{
				// ignore extra props in mongo that aren't in C# models
				new IgnoreExtraElementsConvention(true),
				new EnumRepresentationConvention(BsonType.String)
			};

			ConventionRegistry.Register("Custom Convention Pack Registries", cp, t => true);
		}
	}
}
