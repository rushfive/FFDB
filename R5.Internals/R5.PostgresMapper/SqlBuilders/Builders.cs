using R5.Internals.PostgresMapper.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace R5.Internals.PostgresMapper.SqlBuilders
{
	public static class Builders<TEntity>
		where TEntity : SqlEntity
	{
		public static SelectBuilder<TEntity> Select()
		{
			return new SelectBuilder<TEntity>(null);
		}

		public static SelectBuilder<TEntity> Select(params Expression<Func<TEntity, object>>[] properties)
		{
			if (properties == null || properties.Length == 0)
			{
				throw new ArgumentNullException(nameof(properties), "Property expressions must be provided.");
			}

			return new SelectBuilder<TEntity>(properties.ToList());
		}
	}
}
