using Newtonsoft.Json;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.WeekGameMap.Sources.V1
{
	public class WeekGameMapSource : CoreDataSource<WeekGamesVersionedModel, List<WeekGameMapping>>
	{
		protected override bool SupportsFilePersistence => true;

		public WeekGameMapSource(
			ToVersionedModelMapper toVersionedMapper,
			ToCoreDataMapper toCoreDataMapper,
			ProgramOptions programOptions,
			IDatabaseProvider dbProvider)
			: base(
				  toVersionedMapper,
				  toCoreDataMapper,
				  programOptions,
				  dbProvider) { }

		
	}

	public class ToVersionedModelMapper : IMapper<string, WeekGamesVersionedModel>
	{
		public WeekGamesVersionedModel Map(string serialized)
		{
			throw new NotImplementedException();
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
