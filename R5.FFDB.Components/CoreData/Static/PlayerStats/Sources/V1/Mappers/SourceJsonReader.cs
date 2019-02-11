using Newtonsoft.Json.Linq;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1.Mappers
{
	// todo: unit tests for this
	public static class SourceJsonReader
	{
		public static JObject GetPlayersObject(string sourceJson)
		{
			JObject sourceObject = JObject.Parse(sourceJson);

			JObject gameObject = GetGameObject(sourceObject);

			var playersObject = gameObject.SelectToken("players") as JObject;
			if (playersObject == null)
			{
				throw new InvalidOperationException($"Failed to read the Players JObject from the week stats source response.");
			}

			return playersObject;
		}

		private static JObject GetGameObject(JObject sourceObject)
		{
			var gameObject = sourceObject.SelectToken("games")
				.Children().Single()
				.Children().Single() as JObject;

			if (gameObject == null)
			{
				throw new InvalidOperationException($"Failed to read the Game JObject from the week stats source response.");
			}

			return gameObject;
		}

		public static JObject GetStatsObjectForPlayer(JToken playerValue, WeekInfo week)
		{
			var playerObject = playerValue as JObject;

			var statsObject = playerObject?.SelectToken($"stats.week.{week.Season}.{week.Week}") as JObject;
			if (statsObject == null)
			{
				throw new InvalidOperationException($"Failed to read the Stats JObject from the player object.");
			}

			return statsObject;
		}

		public static Dictionary<WeekStatType, double> ResolveStatsMapFromObject(JObject statsObject)
		{
			var result = new Dictionary<WeekStatType, double>();

			foreach (var s in statsObject)
			{
				if (s.Key == "pts")
				{
					continue;
				}

				if (!int.TryParse(s.Key, out int statKey)
					|| !Enum.IsDefined(typeof(WeekStatType), statKey))
				{
					continue;
				}

				string value = s.Value.ToObject<string>();

				if (!string.IsNullOrWhiteSpace(value)
					&& double.TryParse(value, out double statValue))
				{
					result.Add((WeekStatType)statKey, statValue);
				}
			}

			return result;
		}
		
	}
}
