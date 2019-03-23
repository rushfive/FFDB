using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace R5.FFDB.CLI.Tests.Tests
{
	public class RunInfoBuilderTests
	{
		private static RunInfoBuilder.RunInfoBuilder GetBuilder()
		{
			return ConfigureRunInfoBuilder.Create();
		}

		[Theory]
		[InlineData("--help")]
		[InlineData("-h")]
		[InlineData("/help")]
		public void Help_ReturnsNull(string arg)
		{
			var builder = GetBuilder();

			var runInfo = builder.Build(new string[] { arg });

			Assert.Null(runInfo);
		}

		[Theory]
		[InlineData("--version")]
		[InlineData("-v")]
		[InlineData("/version")]
		public void Version_ReturnsNull(string arg)
		{
			var builder = GetBuilder();

			var runInfo = builder.Build(new string[] { arg });

			Assert.Null(runInfo);
		}

		[Fact]
		public void UnknownCommand_Throws()
		{
			var builder = GetBuilder();

			Assert.Throws<RunInfoBuilder.ProcessException>(
				() => builder.Build(new string[] { "unknown" }));
		}

		public class InitialSetup
		{
			[Fact]
			public void Without_CommandOptions_Success()
			{
				var builder = GetBuilder();

				var runInfoObj = builder.Build(new string[] { "setup" });

				var runInfo = runInfoObj as Commands.InitialSetup.RunInfo;

				Assert.NotNull(runInfo);
				Assert.False(runInfo.SkipAddingStats);

				Assert.Null(runInfo.ConfigFilePath);
				Assert.False(runInfo.SkipRosterFetch);
			}

			[Fact]
			public void With_CommandOptions_Success()
			{
				var builder = GetBuilder();

				var runInfoObj = builder.Build(new string[] { "setup", "--skip-stats" });

				var runInfo = runInfoObj as Commands.InitialSetup.RunInfo;

				Assert.NotNull(runInfo);
				Assert.True(runInfo.SkipAddingStats);

				Assert.Null(runInfo.ConfigFilePath);
				Assert.False(runInfo.SkipRosterFetch);
			}

			[Theory]
			[InlineData(@"--config=c:\config.json")]
			[InlineData(@"-c=c:\config.json")]
			public void With_GlobalOptions_Success(string globalConfigOption)
			{
				var builder = GetBuilder();

				var runInfoObj = builder.Build(new string[] { "setup", globalConfigOption, "--skip-roster" });

				var runInfo = runInfoObj as Commands.InitialSetup.RunInfo;

				Assert.NotNull(runInfo);
				Assert.False(runInfo.SkipAddingStats);

				Assert.Equal(@"c:\config.json", runInfo.ConfigFilePath);
				Assert.True(runInfo.SkipRosterFetch);
			}
		}

		public class ViewState
		{
			[Fact]
			public void Without_GlobalOptions_Success()
			{
				var builder = GetBuilder();

				var runInfoObj = builder.Build(new string[] { "view-state" });
				var runInfo = runInfoObj as Commands.ViewState.RunInfo;

				Assert.NotNull(runInfo);

				Assert.Null(runInfo.ConfigFilePath);
				Assert.False(runInfo.SkipRosterFetch);
			}

			[Theory]
			[InlineData(@"--config=c:\config.json")]
			[InlineData(@"-c=c:\config.json")]
			public void With_GlobalOptions_Success(string globalConfigOption)
			{
				var builder = GetBuilder();

				var runInfoObj = builder.Build(new string[] { "view-state", globalConfigOption, "--skip-roster" });

				var runInfo = runInfoObj as Commands.ViewState.RunInfo;

				Assert.NotNull(runInfo);

				Assert.Equal(@"c:\config.json", runInfo.ConfigFilePath);
				Assert.True(runInfo.SkipRosterFetch);
			}
		}

		public class UpdateRosters
		{
			[Fact]
			public void Without_GlobalOptions_Success()
			{
				var builder = GetBuilder();

				var runInfoObj = builder.Build(new string[] { "update-rosters" });
				var runInfo = runInfoObj as Commands.UpdateRosters.RunInfo;

				Assert.NotNull(runInfo);

				Assert.Null(runInfo.ConfigFilePath);
				Assert.False(runInfo.SkipRosterFetch);
			}

			[Theory]
			[InlineData(@"--config=c:\config.json")]
			[InlineData(@"-c=c:\config.json")]
			public void With_GlobalOptions_Success(string globalConfigOption)
			{
				var builder = GetBuilder();

				var runInfoObj = builder.Build(new string[] { "update-rosters", globalConfigOption, "--skip-roster" });

				var runInfo = runInfoObj as Commands.UpdateRosters.RunInfo;

				Assert.NotNull(runInfo);

				Assert.Equal(@"c:\config.json", runInfo.ConfigFilePath);
				Assert.True(runInfo.SkipRosterFetch);
			}
		}

		public class AddStats
		{
			[Fact]
			public void NoSubCommand_Throws()
			{
				var builder = GetBuilder();

				Assert.Throws<R5.RunInfoBuilder.ProcessException>(
					() => builder.Build(new string[] { "add-stats" }));
			}

			public class Missing
			{
				[Fact]
				public void Without_GlobalOptions_Success()
				{
					var builder = GetBuilder();

					var runInfoObj = builder.Build(new string[] { "add-stats", "missing" });

					var runInfo = runInfoObj as Commands.AddStats.RunInfo;

					Assert.NotNull(runInfo);
					Assert.Null(runInfo.Week);
					Assert.False(runInfo.SaveToDisk);
					Assert.False(runInfo.SaveOriginalSourceFiles);

					Assert.False(runInfo.SkipRosterFetch);
					Assert.Null(runInfo.ConfigFilePath);
				}

				[Theory]
				[InlineData(@"--config=c:\config.json")]
				[InlineData(@"-c=c:\config.json")]
				public void With_GlobalOptions_Success(string globalConfigOption)
				{
					var builder = GetBuilder();

					var runInfoObj = builder.Build(new string[] 
					{
						"add-stats", "missing", globalConfigOption, "--skip-roster", "--save-to-disk", "--save-src-files"
					});

					var runInfo = runInfoObj as Commands.AddStats.RunInfo;

					Assert.NotNull(runInfo);
					Assert.Null(runInfo.Week);

					Assert.Equal(@"c:\config.json", runInfo.ConfigFilePath);
					Assert.True(runInfo.SkipRosterFetch);
					Assert.True(runInfo.SaveToDisk);
					Assert.True(runInfo.SaveOriginalSourceFiles);

					Assert.True(runInfo.SkipRosterFetch);
					Assert.NotNull(runInfo.ConfigFilePath);
				}
			}

			public class Week
			{
				[Fact]
				public void No_WeekArgument_Throws()
				{
					var builder = GetBuilder();

					Assert.Throws<R5.RunInfoBuilder.ProcessException>(
						() => builder.Build(new string[] { "add-stats", "week" }));
				}

				[Fact]
				public void Without_GlobalOptions_Success()
				{
					var builder = GetBuilder();

					var runInfoObj = builder.Build(new string[] { "add-stats", "week", "2018-1" });

					var runInfo = runInfoObj as Commands.AddStats.RunInfo;

					Assert.NotNull(runInfo);
					Assert.Equal(2018, runInfo.Week.Value.Season);
					Assert.Equal(1, runInfo.Week.Value.Week);
					Assert.False(runInfo.SaveToDisk);
					Assert.False(runInfo.SaveOriginalSourceFiles);

					Assert.False(runInfo.SkipRosterFetch);
					Assert.Null(runInfo.ConfigFilePath);
				}

				[Theory]
				[InlineData(@"--config=c:\config.json")]
				[InlineData(@"-c=c:\config.json")]
				public void With_GlobalOptions_Success(string globalConfigOption)
				{
					var builder = GetBuilder();

					var runInfoObj = builder.Build(new string[]
					{
						"add-stats", "week", "2018-1", globalConfigOption, "--skip-roster", "--save-to-disk", "--save-src-files"
					});

					var runInfo = runInfoObj as Commands.AddStats.RunInfo;

					Assert.NotNull(runInfo);
					Assert.Equal(2018, runInfo.Week.Value.Season);
					Assert.Equal(1, runInfo.Week.Value.Week);
					Assert.Equal(@"c:\config.json", runInfo.ConfigFilePath);
					Assert.True(runInfo.SkipRosterFetch);
					Assert.True(runInfo.SaveToDisk);
					Assert.True(runInfo.SaveOriginalSourceFiles);

					Assert.True(runInfo.SkipRosterFetch);
					Assert.NotNull(runInfo.ConfigFilePath);
				}
			}
		}

		public class UpdateRosteredPlayers
		{
			[Fact]
			public void Without_GlobalOptions_Success()
			{
				var builder = GetBuilder();

				var runInfoObj = builder.Build(new string[] { "update-players" });
				var runInfo = runInfoObj as Commands.UpdateRosteredPlayers.RunInfo;

				Assert.NotNull(runInfo);

				Assert.Null(runInfo.ConfigFilePath);
				Assert.False(runInfo.SkipRosterFetch);
			}

			[Theory]
			[InlineData(@"--config=c:\config.json")]
			[InlineData(@"-c=c:\config.json")]
			public void With_GlobalOptions_Success(string globalConfigOption)
			{
				var builder = GetBuilder();

				var runInfoObj = builder.Build(new string[] { "update-players", globalConfigOption, "--skip-roster" });

				var runInfo = runInfoObj as Commands.UpdateRosteredPlayers.RunInfo;

				Assert.NotNull(runInfo);

				Assert.Equal(@"c:\config.json", runInfo.ConfigFilePath);
				Assert.True(runInfo.SkipRosterFetch);
			}
		}
	}
}
