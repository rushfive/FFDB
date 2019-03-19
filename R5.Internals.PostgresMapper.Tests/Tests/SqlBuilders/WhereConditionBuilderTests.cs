using R5.Internals.PostgresMapper.Attributes;
using R5.Internals.PostgresMapper.Models;
using R5.Internals.PostgresMapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace R5.Internals.PostgresMapper.Tests.Tests.SqlBuilders
{
	public class WhereConditionBuilderTests
	{
		[Table("TestEntityTable")]
		public class TestEntity
		{
			[Column("number1", PostgresDataType.INT)]
			public int Number1 { get; set; }

			[Column("number2", PostgresDataType.INT)]
			public int Number2 { get; set; }
		}

		public class WeekInfo
		{
			public int Season { get; set; }
			public int Week { get; set; }
		}

		[Fact]
		public void Condition_EqualsConstant_WithAndOperator_Success()
		{
			Expression<Func<TestEntity, bool>> expr
				= e => e.Number1 == 2019 && e.Number2 == 1;

			string expected = "((number1 = 2019) AND (number2 = 1))";

			string result = WhereConditionBuilder<TestEntity>.FromExpression(expr);

			Assert.Equal(
				expected.RemoveWhiteSpaces(), 
				result.RemoveWhiteSpaces());
		}

		[Fact]
		public void Condition_EqualsConstantVarRef_WithAndOperator_Success()
		{
			int year = 2019;
			int week = 1;

			Expression<Func<TestEntity, bool>> expr
				= e => e.Number1 == year && e.Number2 == week;

			string expected = "((number1 = 2019) AND (number2 = 1))";

			string result = WhereConditionBuilder<TestEntity>.FromExpression(expr);

			Assert.Equal(
				expected.RemoveWhiteSpaces(),
				result.RemoveWhiteSpaces());
		}

		[Fact]
		public void Condition_EqualsObjPropRef_WithAndOperator_Success()
		{
			var week = new WeekInfo { Season = 2019, Week = 1 };

			Expression<Func<TestEntity, bool>> expr
				= e => e.Number1 == week.Season && e.Number2 == week.Week;
			//Expression<Func<TestEntity, bool>> expr
			//	= e => e.Number1 == week.Season;

			string expected = "((number1 = 2019) AND (number2 = 1))";

			string result = WhereConditionBuilder<TestEntity>.FromExpression(expr);

			Assert.Equal(
				expected.RemoveWhiteSpaces(),
				result.RemoveWhiteSpaces());
		}

		//[Fact]
		//public void Condition_WithAndOperator_MultipleMemberAccess_Success()
		//{
		//	var week = new WeekInfo { Season = 2019, Week = 5 };

		//	Expression<Func<TestEntity, bool>> expr
		//		= e => e.Number1 == week.Season && e.Number2 == week.Week;

		//	string result = WhereConditionBuilder<TestEntity>.FromExpression(expr);

		//	Assert.Equal("", result);
		//}
	}

	
}
