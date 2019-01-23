using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.SourceDataMappers.TeamGames
{
	public interface ITeamGamesDataMapper
	{
		Task Test();
	}
	public class TeamGamesDataMapper : ITeamGamesDataMapper
	{
		private DataDirectoryPath _dataPath { get; set; }
		private IDatabaseProvider _dbProvider { get; set; }

		public TeamGamesDataMapper(
			DataDirectoryPath dataPath,
			IDatabaseProvider dbProvider)
		{
			_dataPath = dataPath;
			_dbProvider = dbProvider;
		}

		public async Task Test()
		{
			
			// 2018123015.json
			JObject json = JObject.Parse(File.ReadAllText(_dataPath.Static.TeamGameHistoryGameStats + "2018123015.json"));

			// dependencies
			Dictionary<string, string> gsisNflIdMap = await GetGsisToNflIdMapAsync(); // player week team
																					  // gameId and teamType (home or away) required to parse an inner jobject
			string gameId = "2018123015";

			var teamData = WeekGameTeamData.FromGameStats(json, gameId, gsisNflIdMap);

			string serializedJson = JsonConvert.SerializeObject(teamData);

			File.WriteAllText(@"D:\Repos\ffdb_data_2\team_game_history\game_stats2\2018123015.json", serializedJson);
		}

		private async Task<Dictionary<string, string>> GetGsisToNflIdMapAsync()
		{
			IDatabaseContext dbContext = _dbProvider.GetContext();
			List<Player> players = await dbContext.Player.GetAllAsync();
			return players.ToDictionary(p => p.GsisId, p => p.NflId);
		}

		private void AddForTeam()
		{

		}
	}

	// todo move - filename should be nfl game id
	public class WeekGameTeamData
	{
		public TeamData Home { get; }
		public TeamData Away { get;}

		private WeekGameTeamData(
			TeamData home,
			TeamData away)
		{
			Home = home;
			Away = away;
		}

		// json param should be the entire parsed file, just pass it straight here
		public static WeekGameTeamData FromGameStats(JObject json, string gameId, Dictionary<string, string> gsisNflIdMap)
		{
			var home = TeamData.AsHome(json, gameId, gsisNflIdMap);
			var away = TeamData.AsAway(json, gameId, gsisNflIdMap);
			return new WeekGameTeamData(home, away);
		}
	}

	public class TeamData
	{
		public int Id { get; set; }

		// list of players from team active, must map from esb to nflid
		public List<string> PlayerNflIds { get; set; } = new List<string>();

		// points
		public int PointsFirstQuarter { get; set; }
		public int PointsSecondQuarter { get; set; }
		public int PointsThirdQuarter { get; set; }
		public int PointsFourthQuarter { get; set; }
		public int PointsOverTime { get; set; }
		public int PointsTotal { get; set; }

		// stats
		public int FirstDowns { get; set; }
		public int TotalYards { get; set; }
		public int PassingYards { get; set; }
		public int RushingYards { get; set; }
		public int Penalties { get; set; }
		public int PenaltyYards { get; set; }
		public int Turnovers { get; set; }
		public int Punts { get; set; }
		public int PuntYards { get; set; }
		public int PuntYardsAverage { get; set; }
		public int TimeOfPossessionSeconds { get; set; }

		private static List<string> _statKeys = new List<string>
		{
			"passing", "rushing", "receiving", "fumbles", "kicking", "punting", "kickret", "puntret", "defense"
		};

		private TeamData() { }

		public static TeamData AsHome(JObject json, string gameId, Dictionary<string, string> gsisNflIdMap)
		{
			return new TeamData().ResolveAs(json, gameId, "home", gsisNflIdMap);
		}

		public static TeamData AsAway(JObject json, string gameId, Dictionary<string, string> gsisNflIdMap)
		{
			return new TeamData().ResolveAs(json, gameId, "away", gsisNflIdMap);
		}

		private TeamData ResolveAs(JObject json, string gameId, string teamType, Dictionary<string, string> gsisNflIdMap)
		{
			Id = TeamDataStore.GetIdFromAbbreviation(
				(string)json.SelectToken($"{gameId}.{teamType}.abbr"),
				includePriorLookup: true);

			SetPointsScored(json, gameId, teamType);
			SetTeamStats(json, gameId, teamType);
			SetActivePlayers(json, Id, gameId, teamType, gsisNflIdMap);

			return this;
		}

		private void SetPointsScored(JObject json, string gameId, string teamType)
		{
			JToken score = json.SelectToken($"{gameId}.{teamType}.score");
			if (score == null)
			{
				throw new InvalidOperationException($"Failed to parse score object for {teamType} team in game '{gameId}'.");
			}

			PointsFirstQuarter = (int)score["1"];
			PointsSecondQuarter = (int)score["2"];
			PointsThirdQuarter = (int)score["3"];
			PointsFourthQuarter = (int)score["4"];
			PointsOverTime = (int)score["5"];
			PointsTotal = (int)score["T"];
		}

		private void SetTeamStats(JObject json, string gameId, string teamType)
		{
			JToken teamStats = json.SelectToken($"{gameId}.{teamType}.stats.team");
			if (teamStats == null)
			{
				throw new InvalidOperationException($"Failed to parse team stats object for {teamType} team in game '{gameId}'.");
			}

			FirstDowns = (int)teamStats["totfd"];
			TotalYards = (int)teamStats["totyds"];
			PassingYards = (int)teamStats["pyds"];
			RushingYards = (int)teamStats["ryds"];
			Penalties = (int)teamStats["pen"];
			PenaltyYards = (int)teamStats["penyds"];
			Turnovers = (int)teamStats["trnovr"];
			Punts = (int)teamStats["pt"];
			PuntYards = (int)teamStats["ptyds"];
			PuntYardsAverage = (int)teamStats["ptavg"];

			string timeOfPosession = (string)teamStats["top"];
			var split = timeOfPosession.Split(':');
			TimeOfPossessionSeconds = int.Parse(split[0]) * 60 + int.Parse(split[1]);
		}

		private void SetActivePlayers(JObject json, int teamId, string gameId, string teamType, Dictionary<string, string> gsisNflIdMap)
		{
			foreach (string statKey in _statKeys)
			{
				if (!json.SelectToken($"{gameId}.{teamType}.stats").TryGetToken(statKey, out JToken stats))
				{
					continue;
				}

				foreach (string gsis in stats.ChildPropertyNames())
				{
					if (!gsisNflIdMap.TryGetValue(gsis, out string nflId))
					{
						// most likely insignificant players that we dont care about
						continue;
					}

					PlayerNflIds.Add(nflId);
				}
			}
		}
	}
}
