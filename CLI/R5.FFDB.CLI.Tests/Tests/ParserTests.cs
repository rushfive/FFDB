using R5.FFDB.Core.Models;
using R5.RunInfoBuilder;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace R5.FFDB.CLI.Tests.Tests
{
	public class ParserTests
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
				var builder = ConfigureRunInfoBuilder.Create();

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
				var builder = ConfigureRunInfoBuilder.Create();

				bool valid = builder.Parser.TryParseAs(value, out WeekInfo? parsed);

				Assert.True(valid);
				Assert.Equal(new WeekInfo(expectedSeason, expectedWeek), parsed);
			}
		}
	}
}
