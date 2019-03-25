using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Core.Entities
{
	/// <summary>
	/// Represents a single player's stats for a given week.
	/// </summary>
	public class PlayerWeekStats
	{
		/// <summary>
		/// The week the stats are tied to.
		/// </summary>
		public WeekInfo Week { get; set; }

		/// <summary>
		/// The NFL's official player id.
		/// </summary>
		public string NflId { get; set; }

		/// <summary>
		/// A map of stat types to numeric values.
		/// </summary>
		public Dictionary<WeekStatType, double> Stats { get; set; }

		/// <summary>
		/// The team the player was on for the week. This data point is
		/// resolved from more than one source, so it's nullable for the
		/// case we cannot resolve it.
		/// </summary>
		public int? TeamId { get; set; }

		public List<KeyValuePair<WeekStatType, double>> GetPassingStats()
		{
			return GetStatsByCategory(WeekStatCategory.Pass);
		}

		public List<KeyValuePair<WeekStatType, double>> GetRushingStats()
		{
			return GetStatsByCategory(WeekStatCategory.Rush);
		}

		public List<KeyValuePair<WeekStatType, double>> GetReceivingStats()
		{
			return GetStatsByCategory(WeekStatCategory.Receive);
		}

		public List<KeyValuePair<WeekStatType, double>> GetReturnStats()
		{
			return GetStatsByCategory(WeekStatCategory.Return);
		}

		public List<KeyValuePair<WeekStatType, double>> GetMiscStats()
		{
			return GetStatsByCategory(WeekStatCategory.Misc);
		}

		public List<KeyValuePair<WeekStatType, double>> GetKickingStats()
		{
			return GetStatsByCategory(WeekStatCategory.Kick);
		}

		public List<KeyValuePair<WeekStatType, double>> GetIdpStats()
		{
			return GetStatsByCategory(WeekStatCategory.IDP);
		}

		public List<KeyValuePair<WeekStatType, double>> GetDstStats()
		{
			return GetStatsByCategory(WeekStatCategory.DST);
		}

		private List<KeyValuePair<WeekStatType, double>> GetStatsByCategory(HashSet<WeekStatType> types)
		{
			return Stats.Where(kv => types.Contains(kv.Key)).ToList();
		}
	}
}
