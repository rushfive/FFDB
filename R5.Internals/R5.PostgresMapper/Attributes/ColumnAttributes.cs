using R5.Internals.PostgresMapper.Mappers;
using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.Internals.PostgresMapper.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public abstract class EntityColumnAttribute : Attribute
	{
		public abstract void UpdateTableColumn(TableColumn column);
	}
	
	public class ColumnAttribute : EntityColumnAttribute
	{
		public string Name { get; }
		public PostgresDataType? DataType { get; }

		public ColumnAttribute(string name)
		{
			Name = name;
		}

		public ColumnAttribute(string name, PostgresDataType dataType)
		{
			Name = name;
			DataType = dataType;
		}

		public override void UpdateTableColumn(TableColumn column)
		{
			// name and datatype are set separately
		}
	}

	public class PrimaryKeyAttribute : EntityColumnAttribute
	{
		public override void UpdateTableColumn(TableColumn column)
		{
			column.SetAsPrimaryKey();
		}
	}

	public class NotNullAttribute : EntityColumnAttribute
	{
		public override void UpdateTableColumn(TableColumn column)
		{
			column.SetAsNotNull();
		}
	}
	// make props private
	public class ForeignKeyAttribute : EntityColumnAttribute
	{
		private Type _foreignTableType { get; }
		private string _foreignColumnName { get; }

		public ForeignKeyAttribute(
			Type foreignTableType,
			string foreignColumnName)
		{
			_foreignTableType = foreignTableType;
			_foreignColumnName = foreignColumnName;
		}

		public override void UpdateTableColumn(TableColumn column)
		{
			column.SetForeignTableType(_foreignTableType);
			column.SetForeignKeyColumn(_foreignColumnName);
		}
	}
}
