using R5.FFDB.CLI.Tests.Models;
using R5.FFDB.Core.Models;
using R5.RunInfoBuilder;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace R5.FFDB.CLI.Tests
{
	public class ConfigureBuilderTests
	{
		[Fact]
		public void OnBuildError_Throws_DoesntSuppress()
		{
			Assert.Throws<ProcessException>(() =>
			{
				var builder = ConfigureBuilder.Get();

				builder.Build(new string[] { "add-stats" });
			});
		}

		public class Parser
		{
			public class NullableWeekInfo
			{
				[Theory]
				[InlineData("")]
				[InlineData("20185")]
				[InlineData("2018-5-5")]
				[InlineData("text-5")]
				[InlineData("5-text")]
				[InlineData("text-text")]
				public void Invalid_Returns_FalseAndDefault(string value)
				{
					var builder = ConfigureBuilder.Get();

					bool valid = builder.Parser.TryParseAs(value, out WeekInfo? parsed);

					Assert.False(valid);
					Assert.Equal(default, parsed);
				}

				[Theory]
				[InlineData("2018-5", 2018, 5)]
				[InlineData("2018-15", 2018, 15)]
				public void Valid_Returns_TrueWithCorrectParsedValue(string value,
					int expectedSeason, int expectedWeek)
				{
					var builder = ConfigureBuilder.Get();

					bool valid = builder.Parser.TryParseAs(value, out WeekInfo? parsed);

					Assert.True(valid);
					Assert.Equal(new WeekInfo(expectedSeason, expectedWeek), parsed);
				}
			}
		}

		public class Commands
		{
			public class InitialSetup
			{

			}

			public class CheckHealth
			{

			}

			public class ViewUpdated
			{

			}

			public class UpdateRosters
			{

			}

			public class AddStats
			{

			}

			public class RemoveStats
			{

			}

			public class UpdatePlayers
			{

			}
		}
	}
}
