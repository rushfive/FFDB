using DevTester.Testers;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.PlayerTeamHistory.Sources.NFLWeb.Models;
using R5.FFDB.DbProviders.PostgreSql;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DevTester
{
	public class DevProgram
	{
		private static IServiceProvider _serviceProvider { get; set; }
		private static ILogger<DevProgram> _logger { get; set; }

		public static async Task Main(string[] args)
		{
			var testHistory = new PlayerTeamHistoryJson
			{
				NflId = "TEST",
				SeasonWeekTeamMap = new Dictionary<int, Dictionary<int, int>>
				{
					{
						2015,
						new Dictionary<int, int>
						{
							{ 1, 30 },
							{ 2, 31 }
						}
					},
					{
						2016,
						new Dictionary<int, int>
						{
							{ 1, 31 }
						}
					}
				}
			};

			string testHistoryPath = @"D:\Repos\ffdb_data\player_team_history\test-history.json";

			string serializedTestHistory = JsonConvert.SerializeObject(testHistory);
			
			File.WriteAllText(testHistoryPath, serializedTestHistory);

			//
			
			PlayerTeamHistoryJson playerData = JsonConvert.DeserializeObject<PlayerTeamHistoryJson>(File.ReadAllText(testHistoryPath));

			///////////////

			//var postgresConfig = new PostgresConfig
			//{
			//	DatabaseName = "ffdb_test_1",
			//	Host = "localhost",
			//	Username = "ffdb",
			//	Password = "welc0me!"
			//};

			//var postgresProvider = new PostgresDbProvider(postgresConfig);
			//PostgresDbContext context = postgresProvider.GetContext();

			//await context.RunInitialSetupAsync();

			//////////

			//_serviceProvider = DevTestServiceProvider.Build();
			//_logger = _serviceProvider.GetRequiredService<ILogger<DevProgram>>();

			//await FetchPlayerProfilesFromRostersAsync(downloadRosterPages: false);


			Console.ReadKey();
		}

		private static Task FetchPlayerProfilesFromRostersAsync(bool downloadRosterPages)
		{
			try
			{
				IPlayerProfileTester tester = _serviceProvider.GetRequiredService<IPlayerProfileTester>();
				return tester.FetchSavePlayerProfilesAsync(downloadRosterPages);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "There was an error fetching player profile from roster.");
				throw;
			}
		}
	}
}
