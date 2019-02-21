using Newtonsoft.Json;
using R5.FFDB.Core.Models;
using System;

namespace R5.FFDB.Components.Extensions.JsonConverters
{
	public class WeekInfoJsonConverter : JsonConverter<WeekInfo>
	{
		public override WeekInfo ReadJson(JsonReader reader, Type objectType, WeekInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (reader.Value == null || reader.TokenType == JsonToken.Null)
			{
				throw new InvalidOperationException($"A serialized '{nameof(WeekInfo)}' value should never be null.");
			}

			var (season, week) = ParseSerializedValue((string)reader.Value);
			return new WeekInfo(season, week);
		}

		private static (int season, int week) ParseSerializedValue(string serialized)
		{
			if (string.IsNullOrWhiteSpace(serialized))
			{
				throw new ArgumentException($"'{nameof(WeekInfo)}' value must be provided but was null or empty.");
			}

			string[] split = serialized.Split('-');
			if (split.Length != 2)
			{
				throw new ArgumentException($"Failed to split '{serialized}' into season and week parts.");
			}

			if (!int.TryParse(split[0], out int season))
			{
				throw new ArgumentException($"The serialized value '{serialized}' contains an invalid season: '{split[0]}'");
			}

			if (!int.TryParse(split[1], out int week))
			{
				throw new ArgumentException($"The serialized value '{serialized}' contains an invalid week: '{split[1]}'");
			}

			return (season, week);
		}

		public override void WriteJson(JsonWriter writer, WeekInfo value, JsonSerializer serializer)
		{
			string serialized = $"{value.Season}-{value.Week}";
			writer.WriteValue(serialized);
		}
	}
}
