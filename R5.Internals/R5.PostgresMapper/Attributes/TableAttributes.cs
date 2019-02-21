using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.PostgresMapper.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public abstract class EntityTableAttribute : Attribute { }
	
	public class TableAttribute : EntityTableAttribute
	{
		public string Name { get; }

		public TableAttribute(string name)
		{
			Name = name;
		}
	}
	
	public class CompositePrimaryKeysAttribute : EntityTableAttribute
	{
		public List<string> ColumnNames { get; }

		public CompositePrimaryKeysAttribute(params string[] columnNames)
		{
			if (columnNames == null || columnNames.Length < 2)
			{
				throw new ArgumentNullException(nameof(columnNames),
					"At least two column names must be provided for use as a table's composite primary key.");
			}

			ColumnNames = columnNames.ToList();
		}
	}
}
