using Microsoft.Extensions.Logging;
using R5.FFDB.Components;
using R5.FFDB.Components.PlayerTeamHistory;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevTester.Testers
{
	public interface IPlayerTeamHistoryTester
	{
		Task FetchForPlayerAsync(PlayerProfile player);
	}

	public class PlayerTeamHistoryTester : IPlayerTeamHistoryTester
	{
		private ILogger<PlayerTeamHistoryTester> _logger { get; }
		private IWebRequestClient _webRequestClient { get; }
		private IPlayerTeamHistorySource _playerTeamHistorySource { get; }
		private DataDirectoryPath _dataPath { get; }

		public PlayerTeamHistoryTester(
			ILogger<PlayerTeamHistoryTester> logger,
			IWebRequestClient webRequestClient,
			IPlayerTeamHistorySource playerTeamHistorySource,
			DataDirectoryPath dataPath)
		{
			_logger = logger;
			_webRequestClient = webRequestClient;
			_playerTeamHistorySource = playerTeamHistorySource;
			_dataPath = dataPath;
		}

		public async Task FetchForPlayerAsync(PlayerProfile player)
		{
			await _playerTeamHistorySource.FetchAndSaveAsync(new List<PlayerProfile> { player });
		}
	}
}
