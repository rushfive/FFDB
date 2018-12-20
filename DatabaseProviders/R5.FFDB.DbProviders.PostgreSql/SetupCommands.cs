using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql
{
	// todo: review types (int4?)
	public static class CreateTableCommands
	{
		public static string Teams = "CREATE TABLE teams ("
			+ "id INT PRIMARY KEY,"
			+ "nfl_id TEXT NOT NULL,"
			+ "name TEXT NOT NULL,"
			+ "abbreviation TEXT NOT NULL)";

		// esb and gsis ids should have values, but its possible we cant resolve
		// it from an available source. todo: mark somehow to retry in the future
		public static string Players = "CREATE TABLE players ("
			+ "id UUID PRIMARY KEY,"
			+ "nfl_id TEXT NOT NULL,"
			+ "esb_id TEXT,"
			+ "gsis_id TEXT,"
			+ "first_name TEXT NOT NULL,"
			+ "last_name TEXT,"
			+ "position TEXT,"
			+ "number INT,"
			+ "height INT,"
			+ "weight INT,"
			+ "date_of_birth TIMESTAMPTZ,"
			+ "college TEXT)";

		public static string PlayerTeamMap = "CREATE TABLE player_team_map ("
			+ "player_id UUID NOT NULL REFERENCES players(id),"
			+ "team_id INT NOT NULL REFERENCES teams(id))";

		public static string WeekStats()
		{
			string sqlCommand = "CREATE TABLE week_stats ("
				+ "player_id UUID NOT NULL REFERENCES players(id),"
				+ "season INT NOT NULL,"
				+ "week INT NOT NULL,"
				+ "team_id INT REFERENCES teams(id),";

			foreach (string statType in Enum.GetNames(typeof(WeekStatType)))
			{
				sqlCommand += $"{statType} FLOAT8,";
			}

			return sqlCommand.Substring(0, sqlCommand.Length - 1) + ");";
		}
	}

	public static class InitialSeedCommands
	{
		// todo: convert to insert many
		public static string Team(Team team)
		{
			return "INSERT INTO teams (id, nfl_id, name, abbreviation) "
				+ $"VALUES ({team.Id}, {team.NflId}, {team.Name}, {team.Abbreviation})";

			
		}
	}
}
