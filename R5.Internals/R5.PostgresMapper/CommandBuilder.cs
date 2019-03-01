//using R5.Internals.Extensions.Collections;
//using R5.Internals.PostgresMapper.Models;
//using R5.Internals.PostgresMapper.SqlBuilders;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace R5.Internals.PostgresMapper
//{
//	public static class CommandBuilder
//	{
//		public static readonly TableCommandBuilder Table = new TableCommandBuilder();
//	}

//	public class TableCommandBuilder
//	{
//		public string Create<TEntity>()
//		{
//			return Create(typeof(TEntity));
//		}

//		public string Create(Type entityType)
//		{
//			List<string> columnDefinitions = GetColumnDefinitions(entityType);

//			return new ConcatSqlBuilder()
//				.Append($"CREATE TABLE {MetadataResolver.TableName(entityType)} ")
//				.Append($" ({string.Join(", ", columnDefinitions)}) ")
//				.GetResult();
//		}

		

//		public string Truncate<TEntity>()
//		{
//			return Truncate(typeof(TEntity));
//		}

//		public string Truncate(Type entityType)
//		{
//			return new ConcatSqlBuilder()
//				.Append($"TRUNCATE {MetadataResolver.TableName(entityType)}")
//				.GetResult();
//		}
//	}
//}
