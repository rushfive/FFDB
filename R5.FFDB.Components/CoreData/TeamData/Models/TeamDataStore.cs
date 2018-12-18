using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.CoreData.TeamData.Models
{
	public static class TeamDataStore
	{
		private static HashSet<string> _nflIds { get; }
		private static Dictionary<string, string> _abbreviationShortNameMap { get; }
		private static Dictionary<string, int> _shortNameIdMap { get; }
		private static Dictionary<string, int> _abbreviationIdMap { get; }

		static TeamDataStore()
		{
			_nflIds = new HashSet<string>();
			_abbreviationShortNameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			_shortNameIdMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			_abbreviationIdMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

			_teams.ForEach(t =>
			{
				_nflIds.Add(t.NflId);
				_abbreviationShortNameMap[t.Abbreviation] = t.ShortName;
				_shortNameIdMap[t.ShortName] = t.Id;
				_abbreviationIdMap[t.Abbreviation] = t.Id;
			});
		}

		public static List<Team> GetAll()
		{
			return _teams;
		}

		public static bool IsTeam(string nflId)
		{
			return _nflIds.Contains(nflId);
		}

		public static string GetShortNameFromAbbreviation(string abbreviation)
		{
			if (!_abbreviationShortNameMap.TryGetValue(abbreviation, out string shortName))
			{
				throw new InvalidOperationException($"Failed to find team's short name by abbreviation '{abbreviation}'.");
			}

			return shortName;
		}

		public static int GetIdFromShortName(string shortName)
		{
			if (!_shortNameIdMap.TryGetValue(shortName, out int id))
			{
				throw new InvalidOperationException($"Failed to find team's id by short name '{shortName}'.");
			}

			return id;
		}

		public static int GetIdFromAbbreviation(string abbreviation, bool includePriorLookup = false)
		{
			if (_abbreviationIdMap.TryGetValue(abbreviation, out int id))
			{
				return id;
			}

			if (!includePriorLookup)
			{
				throw new InvalidOperationException($"Failed to find team's id by abbreviation '{abbreviation}'.");
			}

			Team priorMatch = _teams.SingleOrDefault(t => t.PriorAbbreviations.Contains(abbreviation));
			if (priorMatch == null)
			{
				throw new InvalidOperationException($"Failed to find team's id by abbreviation '{abbreviation}'.");
			}

			return priorMatch.Id;
		}

		private static List<Team> _teams = new List<Team>
		{
			new Team
			{
				Id = 1,
				NflId = "100001",
				Name = "Atlanta Falcons",
				ShortName = "falcons",
				Abbreviation = "ATL"
			},
			new Team
			{
				Id = 2,
				NflId = "100002",
				Name = "Baltimore Ravens",
				ShortName = "ravens",
				Abbreviation = "BAL"
			},
			new Team
			{
				Id = 3,
				NflId = "100003",
				Name = "Buffalo Bills",
				ShortName = "bills",
				Abbreviation = "BUF"
			},
			new Team
			{
				Id = 4,
				NflId = "100004",
				Name = "Carolina Panthers",
				ShortName = "panthers",
				Abbreviation = "CAR"
			},
			new Team
			{
				Id = 5,
				NflId = "100005",
				Name = "Chicago Bears",
				ShortName = "bears",
				Abbreviation = "CHI"
			},
			new Team
			{
				Id = 6,
				NflId = "100006",
				Name = "Cincinnati Bengals",
				ShortName = "bengals",
				Abbreviation = "CIN"
			},
			new Team
			{
				Id = 7,
				NflId = "100007",
				Name = "Cleveland Browns",
				ShortName = "browns",
				Abbreviation = "CLE"
			},
			new Team
			{
				Id = 8,
				NflId = "100008",
				Name = "Dallas Cowboys",
				ShortName = "cowboys",
				Abbreviation = "DAL"
			},
			new Team
			{
				Id = 9,
				NflId = "100009",
				Name = "Denver Broncos",
				ShortName = "broncos",
				Abbreviation = "DEN"
			},
			new Team
			{
				Id = 10,
				NflId = "100010",
				Name = "Detroit Lions",
				ShortName = "lions",
				Abbreviation = "DET"
			},
			new Team
			{
				Id = 11,
				NflId = "100011",
				Name = "Green Bay Packers",
				ShortName = "packers",
				Abbreviation = "GB"
			},
			new Team
			{
				Id = 12,
				NflId = "100012",
				Name = "Tennessee Titans",
				ShortName = "titans",
				Abbreviation = "TEN"
			},
			new Team
			{
				Id = 13,
				NflId = "100013",
				Name = "Houston Texans",
				ShortName = "texans",
				Abbreviation = "HOU"
			},
			new Team
			{
				Id = 14,
				NflId = "100014",
				Name = "Indianapolis Colts",
				ShortName = "colts",
				Abbreviation = "IND"
			},
			new Team
			{
				Id = 15,
				NflId = "100015",
				Name = "Jacksonville Jaguars",
				ShortName = "jaguars",
				Abbreviation = "JAX",
				PriorAbbreviations = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
				{
					"JAC"
				}
			},
			new Team
			{
				Id = 16,
				NflId = "100016",
				Name = "Kansas City Chiefs",
				ShortName = "chiefs",
				Abbreviation = "KC"
			},
			new Team
			{
				Id = 17,
				NflId = "100017",
				Name = "Los Angeles Rams",
				ShortName = "rams",
				Abbreviation = "LA",
				PriorAbbreviations = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
				{
					"STL"
				}
			},
			new Team
			{
				Id = 18,
				NflId = "100018",
				Name = "Oakland Raiders",
				ShortName = "raiders",
				Abbreviation = "OAK"
			},
			new Team
			{
				Id = 19,
				NflId = "100019",
				Name = "Miami Dolphins",
				ShortName = "dolphins",
				Abbreviation = "MIA"
			},
			new Team
			{
				Id = 20,
				NflId = "100020",
				Name = "Minnesota Vikings",
				ShortName = "vikings",
				Abbreviation = "MIN"
			},
			new Team
			{
				Id = 21,
				NflId = "100021",
				Name = "New England Patriots",
				ShortName = "patriots",
				Abbreviation = "NE"
			},
			new Team
			{
				Id = 22,
				NflId = "100022",
				Name = "New Orleans Saints",
				ShortName = "saints",
				Abbreviation = "NO"
			},
			new Team
			{
				Id = 23,
				NflId = "100023",
				Name = "New York Giants",
				ShortName = "giants",
				Abbreviation = "NYG"
			},
			new Team
			{
				Id = 24,
				NflId = "100024",
				Name = "New York Jets",
				ShortName = "jets",
				Abbreviation = "NYJ"
			},
			new Team
			{
				Id = 25,
				NflId = "100025",
				Name = "Philadelphia Eagles",
				ShortName = "eagles",
				Abbreviation = "PHI"
			},
			new Team
			{
				Id = 26,
				NflId = "100026",
				Name = "Arizona Cardinals",
				ShortName = "cardinals",
				Abbreviation = "ARI"
			},
			new Team
			{
				Id = 27,
				NflId = "100027",
				Name = "Pittsburgh Steelers",
				ShortName = "steelers",
				Abbreviation = "PIT"
			},
			new Team
			{
				Id = 28,
				NflId = "100028",
				Name = "Los Angeles Chargers",
				ShortName = "chargers",
				Abbreviation = "LAC",
				PriorAbbreviations = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
				{
					"SD"
				}
			},
			new Team
			{
				Id = 29,
				NflId = "100029",
				Name = "San Francisco 49ers",
				ShortName = "49ers",
				Abbreviation = "SF"
			},
			new Team
			{
				Id = 30,
				NflId = "100030",
				Name = "Seattle Seahawks",
				ShortName = "seahawks",
				Abbreviation = "SEA"
			},
			new Team
			{
				Id = 31,
				NflId = "100031",
				Name = "Tampa Bay Buccaneers",
				ShortName = "buccaneers",
				Abbreviation = "TB"
			},
			new Team
			{
				Id = 32,
				NflId = "100032",
				Name = "Washington Redskins",
				ShortName = "redskins",
				Abbreviation = "WAS"
			}
		};
	}
}
