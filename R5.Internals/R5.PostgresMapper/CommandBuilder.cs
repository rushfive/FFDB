using R5.Internals.Extensions.Collections;
using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.Internals.PostgresMapper
{
	public static class CommandBuilder
	{
		public static readonly TableCommandBuilder Table = new TableCommandBuilder();
	}

	public class TableCommandBuilder
	{
		public string Create<TEntity>() where TEntity : SqlEntity
		{
			return Create(typeof(TEntity));
		}

		public string Create(Type entityType)
		{
			entityType.ThrowIfNotSqlEntity();

			List<string> columnDefinitions = GetColumnDefinitions(entityType);

			return new SqlTextConcat()
				.AddText($"CREATE TABLE {MetadataResolver.TableName(entityType)} ")
				.AddText($" ({string.Join(", ", columnDefinitions)}) ")
				.GetResult();
		}

		private List<string> GetColumnDefinitions(Type entityType)
		{
			List<string> definitions = MetadataResolver.TableColumns(entityType)
				.Select(c => c.DefinitionForCreateTable())
				.ToList();

			List<string> compositeKeys = MetadataResolver.CompositePrimaryKeys(entityType);
			if (!compositeKeys.IsNullOrEmpty())
			{
				definitions.Add(
					$"PRIMARY KEY({string.Join(", ", compositeKeys)})");
			}

			return definitions;
		}

		public string Truncate<TEntity>() where TEntity : SqlEntity
		{
			return Truncate(typeof(TEntity));
		}

		public string Truncate(Type entityType)
		{
			entityType.ThrowIfNotSqlEntity();

			return new SqlTextConcat()
				.AddText($"TRUNCATE {MetadataResolver.TableName(entityType)}")
				.GetResult();
		}
	}

	public class SqlTextConcat
	{
		private StringBuilder _sb = new StringBuilder();

		public SqlTextConcat AddText(string text)
		{
			_sb.Append(text);
			return this;
		}

		public string GetResult()
		{
			var result = _sb.ToString().Trim();

			if (string.IsNullOrWhiteSpace(result))
			{
				throw new InvalidOperationException("SQL command/query is invalid: cannot be null or empty.");
			}

			return result.EndsWith(";") ? result : result + ";";
		}
	}
}
