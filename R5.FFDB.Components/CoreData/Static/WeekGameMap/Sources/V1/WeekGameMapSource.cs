using Newtonsoft.Json;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.Http;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace R5.FFDB.Components.CoreData.Static.WeekGameMap.Sources.V1
{
	public class WeekGameMapSource : CoreDataSource<WeekGamesVersionedModel, List<WeekGameMapping>>
	{
		public WeekGameMapSource(
			ToVersionedModelMapper toVersionedMapper,
			ToCoreDataMapper toCoreDataMapper,
			ProgramOptions programOptions,
			IDatabaseProvider dbProvider,
			DataDirectoryPath dataPath,
			IWebRequestClient webClient)
			: base(
				  toVersionedMapper,
				  toCoreDataMapper,
				  programOptions,
				  dbProvider,
				  dataPath,
				  webClient)
		{ }

		protected override bool SupportsFilePersistence => true;

		protected override string GetVersionedFilePath(WeekInfo week)
		{
			return DataPath.WeekGameMap(week);
		}

		protected override string GetSourceUri(WeekInfo week)
		{
			return Endpoints.Api.ScoreStripWeekGames(week.Season, week.Week);
		}
	}

	// parses XML response from NFLs score strip endpoint:
	// http://www.nfl.com/ajax/scorestrip?season={season}&seasonType=REG&week={week}
	public class ToVersionedModelMapper : IMapper<string, WeekGamesVersionedModel>
	{
		public WeekGamesVersionedModel Map(string httpResponse)
		{
			XElement weekGameXml = XElement.Parse(httpResponse);
			
			XElement gamesNode = weekGameXml.Elements("gms").Single();

			string y = gamesNode.Attribute("y").Value;
			string w = gamesNode.Attribute("w").Value;

			if (!int.TryParse(y, out int season))
			{
				throw new InvalidOperationException("Failed to parse year value from the games node:"
					+ Environment.NewLine + gamesNode);
			}

			if (!int.TryParse(w, out int week))
			{
				throw new InvalidOperationException("Failed to parse week value from the games node:"
					+ Environment.NewLine + gamesNode);
			}

			var model = new WeekGamesVersionedModel
			{
				Week = new WeekInfo(season, week),
				Games = new List<WeekGamesVersionedModel.Game>()
			};


			foreach (XElement game in gamesNode.Elements("g"))
			{
				int homeTeamId = TeamDataStore.GetIdFromAbbreviation(game.Attribute("h").Value, includePriorLookup: true);
				int awayTeamId = TeamDataStore.GetIdFromAbbreviation(game.Attribute("v").Value, includePriorLookup: true);
				string nflGameId = game.Attribute("eid").Value;
				string gsisGameId = game.Attribute("gsis").Value;

				var matchup = new WeekGamesVersionedModel.Game
				{
					HomeTeamId = homeTeamId,
					AwayTeamId = awayTeamId,
					NflGameId = nflGameId,
					GsisGameId = gsisGameId
				};

				model.Games.Add(matchup);
			}

			return model;
		}
	}

	public class WeekGamesVersionedModel
	{
		[JsonProperty("week")]
		public WeekInfo Week { get; set; }

		[JsonProperty("games")]
		public List<Game> Games { get; set; }

		public class Game
		{
			[JsonProperty("nflGameId")]
			public string NflGameId { get; set; }

			[JsonProperty("gsisGameId")]
			public string GsisGameId { get; set; }

			[JsonProperty("homeTeamId")]
			public int HomeTeamId { get; set; }

			[JsonProperty("awayTeamId")]
			public int AwayTeamId { get; set; }
		}
	}

	public class ToCoreDataMapper : IMapper<WeekGamesVersionedModel, List<WeekGameMapping>>
	{
		public List<WeekGameMapping> Map(WeekGamesVersionedModel versionedModel)
		{
			var result = new List<WeekGameMapping>();

			foreach(WeekGamesVersionedModel.Game game in versionedModel.Games)
			{
				result.Add(new WeekGameMapping
				{
					Week = versionedModel.Week,
					NflGameId = game.NflGameId,
					GsisGameId = game.GsisGameId
				});
			}

			return result;
		}
	}
}
