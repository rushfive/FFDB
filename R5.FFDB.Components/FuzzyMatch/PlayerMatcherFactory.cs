using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace R5.FFDB.Components.FuzzyMatch
{
	public interface IPlayerMatcherFactory
	{
		Task<Func<string, Guid>> GetAsync(int teamId, WeekInfo week);
	}
	public class PlayerMatcherFactory : IPlayerMatcherFactory
	{
		private IDatabaseProvider _databaseProvider { get; }

		public PlayerMatcherFactory(IDatabaseProvider databaseProvider)
		{
			_databaseProvider = databaseProvider;
		}

		public async Task<Func<string, Guid>> GetAsync(int teamId, WeekInfo week)
		{
			Dictionary<string, Guid> map = await GetNameIdMapAsync(teamId, week);

			return (string search) =>
			{
				var normalized = Normalize(search);

				IEnumerable<KeyValuePair<string, Guid>> searchNames = map
					.Where(kv => kv.Key[0] == normalized[0] && kv.Key[kv.Key.Length - 1] == normalized[normalized.Length - 1]);

				// get min dist for all search names, pick lowest.
				// todo for case where more than one min
				var results = new List<(int min, Guid playerId)>();

				foreach (var searchKv in searchNames)
				{
					int minOperations = EditDistance.Find(normalized, searchKv.Key);
					results.Add((minOperations, searchKv.Value));
				}

				IGrouping<int, (int min, Guid playerId)> minGroup = results.GroupBy(r => r.min).OrderBy(g => g.Key).First();
				if (minGroup.Count() > 1)
				{
					throw new InvalidOperationException($"Failed to match '{search}' (normalized to '{normalized}') to only one existing player.");
				}

				return minGroup.Single().playerId;
			};
		}

		private async Task<Dictionary<string, Guid>> GetNameIdMapAsync(int teamId, WeekInfo week)
		{
			IDatabaseContext dbContext = _databaseProvider.GetContext();

			List<Player> players = await dbContext.Player.GetByTeamForWeekAsync(teamId, week);

			var result = new Dictionary<string, Guid>();
			foreach(var player in players)
			{
				// requires normalizing using the same method as the search input
				var key = Normalize($"{player.FirstName}{player.LastName}");
				result[key] = player.Id;
			}

			return result;
		}

		private static Func<string, string> Normalize = name => Regex.Replace(name, " ", "").ToLower();
	}
}
