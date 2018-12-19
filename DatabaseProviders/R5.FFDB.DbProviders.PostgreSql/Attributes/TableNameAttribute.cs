using System;

namespace R5.FFDB.DbProviders.PostgreSql.Attributes
{
	public class TableNameAttribute : Attribute
	{
		private string _name { get; }

		public TableNameAttribute(string name)
		{
			_name = name;
		}
	}
}
