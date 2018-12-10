using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql
{
	// todo: review types (int4?)
	public static class SetupCommands
	{
		public static string CreateTeamsTable = "CREATE TABLE teams ("
			+ "id INT PRIMARY KEY,"
			+ "nfl_id TEXT NOT NULL,"
			+ "name TEXT NOT NULL,"
			+ "abbreviation TEXT NOT NULL)";

		// esb and gsis ids should have values, but its possible we cant resolve
		// it from an available source. todo: mark somehow to retry in the future
		public static string CreatePlayersTable = "CREATE TABLE players ("
			+ "id UUID PRIMARY KEY,"
			+ "nfl_id TEXT NOT NULL,"
			+ "esb_id TEXT,"
			+ "gsis_id TEXT,"
			+ "first_name TEXT NOT NULL,"
			+ "last_name TEXT,"
			+ "position TEXT NOT NULL,"
			+ "number INT,"
			+ "height INT,"
			+ "weight INT,"
			+ "date_of_birth TIMESTAMPTZ,"
			+ "college TEXT)";

		public static string CreateWeekStatsTable()
		{
			string sqlCommand = "CREATE TABLE week_stats ("
				+ "player_id UUID NOT NULL REFERENCES players(id),"
				+ "season INT NOT NULL,"
				+ "week INT NOT NULL,"
				+ "team_id INT NOT NULL REFERENCES teams(id),";

			foreach (string statType in Enum.GetNames(typeof(WeekStatType)))
			{
				sqlCommand += $"{statType} FLOAT8,";
			}

			return sqlCommand.Substring(0, sqlCommand.Length - 1) + ");";
		}
	}
}
