using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace R5.FFDB.DbProviders.Mongo.Serialization.Serializers
{
	public class DateTimeOffsetSerializer : StructSerializerBase<DateTimeOffset>
	{
		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTimeOffset value)
		{
			string serializedValue = value.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture);

			context.Writer.WriteString(serializedValue);
		}

		public override DateTimeOffset Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			string serializedValue = context.Reader.ReadString();

			return DateTimeOffset.Parse(serializedValue);
		}
	}
}
