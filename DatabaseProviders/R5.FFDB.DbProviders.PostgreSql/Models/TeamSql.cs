using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models
{
	[TableName("teams")]
	public class TeamSql : SqlEntity
	{
		[PrimaryKey]
		[Column("id", PostgresDataType.INT)]
		public int Id { get; set; }

		[NotNull]
		[Column("nfl_id", PostgresDataType.TEXT)]
		public string NflId { get; set; }

		[NotNull]
		[Column("name", PostgresDataType.TEXT)]
		public string Name { get; set; }

		[NotNull]
		[Column("abbreviation", PostgresDataType.TEXT)]
		public string Abbreviation { get; set; }

		public static TeamSql FromCoreEntity(Team entity)
		{
			return new TeamSql
			{
				Id = entity.Id,
				NflId = entity.NflId,
				Name = entity.Name,
				Abbreviation = entity.Abbreviation
			};
		}

		//public override string CreateTable()
		//{
		//	return "CREATE TABLE teams ("
		//		+ "id INT PRIMARY KEY,"
		//		+ "nfl_id TEXT NOT NULL,"
		//		+ "name TEXT NOT NULL,"
		//		+ "abbreviation TEXT NOT NULL)";
		//}

		public override string InsertCommand()
		{
			throw new NotImplementedException();
		}
	}

	

	public class TableNameAttribute : Attribute
	{
		private string _name { get; }

		public TableNameAttribute(string name)
		{
			_name = name;
		}
	}

	public abstract class EntityColumnAttribute : Attribute
	{

	}

	public class ColumnAttribute : EntityColumnAttribute
	{
		public string Name { get; }
		public PostgresDataType DataType { get; }
		
		public ColumnAttribute(string name, PostgresDataType dataType)
		{
			Name = name;
			DataType = dataType;
		}
	}

	public class PrimaryKeyAttribute : EntityColumnAttribute
	{

	}

	public class NotNullAttribute : EntityColumnAttribute
	{

	}

	public class ForeignKeyAttribute: EntityColumnAttribute
	{
		public Type ForeignTableType { get; }
		public string ForeignColumnName { get; }

		public ForeignKeyAttribute(
			Type foreignTableType,
			string foreignColumnName)
		{
			ForeignTableType = foreignTableType;
			ForeignColumnName = foreignColumnName;
		}
	}

	public enum PostgresDataType
	{
		UUID,
		TEXT,
		INT,
		TIMESTAMPTZ,
		FLOAT8
	}
}
