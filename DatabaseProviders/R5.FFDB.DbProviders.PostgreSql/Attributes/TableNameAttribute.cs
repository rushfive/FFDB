//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace R5.FFDB.DbProviders.PostgreSql.Attributes
//{
//	public class TableNameAttribute : Attribute
//	{
//		public string Name { get; }

//		public TableNameAttribute(string name)
//		{
//			Name = name;
//		}
//	}

//	public class CompositePrimaryKeysAttribute : Attribute
//	{
//		public List<string> ColumnNames { get; }

//		public CompositePrimaryKeysAttribute(params string[] columnNames)
//		{
//			if (columnNames == null || columnNames.Length < 2)
//			{
//				throw new ArgumentNullException(nameof(columnNames), 
//					"At least two column names must be provided for use as a table's composite primary key.");
//			}

//			ColumnNames = columnNames.ToList();
//		}
//	}
//}
