//using R5.FFDB.Core.Models;
//using R5.FFDB.DbProviders.PostgreSql.Models;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace R5.FFDB.DbProviders.PostgreSql.Attributes
//{
//	public abstract class EntityColumnAttribute : Attribute
//	{

//	}

//	public class ColumnAttribute : EntityColumnAttribute
//	{
//		public string Name { get; }
//		public PostgresDataType DataType { get; }

//		public ColumnAttribute(string name, PostgresDataType dataType)
//		{
//			Name = name;
//			DataType = dataType;
//		}
//	}

//	public class PrimaryKeyAttribute : EntityColumnAttribute
//	{

//	}

//	public class NotNullAttribute : EntityColumnAttribute
//	{

//	}

//	public class ForeignKeyAttribute : EntityColumnAttribute
//	{
//		public Type ForeignTableType { get; }
//		public string ForeignColumnName { get; }

//		public ForeignKeyAttribute(
//			Type foreignTableType,
//			string foreignColumnName)
//		{
//			ForeignTableType = foreignTableType;
//			ForeignColumnName = foreignColumnName;
//		}
//	}

//	public class WeekStatColumnAttribute : EntityColumnAttribute
//	{
//		public string Name { get; }
//		public WeekStatType StatType { get; }

//		public WeekStatColumnAttribute(string name, WeekStatType statType)
//		{
//			Name = name;
//			StatType = statType;
//		}
//	}
//}
