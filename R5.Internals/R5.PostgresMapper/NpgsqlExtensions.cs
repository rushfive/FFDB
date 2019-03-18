using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.Internals.PostgresMapper
{
	internal static class NpgsqlExtensions
	{
		public static async Task<TReturn> ExecuteReaderAsync<TReturn>(this NpgsqlConnection connection,
			string sqlCommand, Func<NpgsqlDataReader, TReturn> onReadMapper)
		{
			if (onReadMapper == null)
			{
				throw new ArgumentNullException(nameof(onReadMapper), "On-read mapper callback must be provided to execute a command with returning value.");
			}

			using (connection)
			{
				await connection.OpenAsync();

				using (var command = new NpgsqlCommand())
				{
					command.Connection = connection;
					command.CommandText = sqlCommand;

					using (NpgsqlDataReader reader = command.ExecuteReader())
					{
						return onReadMapper.Invoke(reader);
					}
				}
			}
		}

		public static async Task ExecuteNonQueryAsync(this NpgsqlConnection connection, 
			string sqlCommand, List<(string key, string value)> parameters = null)
		{
			using (connection)
			{
				await connection.OpenAsync();

				using (var command = new NpgsqlCommand())
				{
					command.Connection = connection;
					command.CommandText = sqlCommand;

					if (parameters?.Any() ?? false)
					{
						parameters.ForEach(p => command.Parameters.AddWithValue(p.key, p.value));
					}

					await command.ExecuteNonQueryAsync();
				}
			}
		}
	}
}
