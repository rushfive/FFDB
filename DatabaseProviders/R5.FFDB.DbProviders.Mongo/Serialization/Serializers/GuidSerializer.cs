using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.Mongo.Serialization.Serializers
{
	public class GuidSerializer : StructSerializerBase<Guid>
	{
		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Guid value)
		{
			string serializedValue = value.ToString("D");

			context.Writer.WriteString(serializedValue);
		}

		public override Guid Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			string serializedValue = context.Reader.ReadString();

			return Guid.Parse(serializedValue);
		}
	}
}
