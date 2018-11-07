using R5.FFDB.Core.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Data
{
	public static class Teams
	{
		public static HashSet<string> Abbreviations = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"ATL", "BAL", "BUF", "CAR", "CHI", "CLE", "DAL", "DEN", "DET", "GB", "TEN", "HOU", "IND", "JAX", "KC", "LA", "OAK", "MIA", "MIN", "NE", "NO", "NYG", "NYJ", "PHI", "ARI", "PIT", "LAC", "SF", "SEA", "TB", "WAS"
		};

		public static List<Team> Get()
		{
			return new List<Team>
			{
				new Team
				{
					Id = 1,
					Name = "Atlanta Falcons",
					Abbreviation = "ATL"
				},
				new Team
				{
					Id = 2,
					Name = "Baltimore Ravens",
					Abbreviation = "BAL"
				},
				new Team
				{
					Id = 3,
					Name = "Buffalo Bills",// CHECK 
					Abbreviation = "BUF"
				},
				new Team
				{
					Id = 4,
					Name = "Carolina Panthers",
					Abbreviation = "CAR"
				},
				new Team
				{
					Id = 5,
					Name = "Chicago Bears",
					Abbreviation = "CHI"
				},
				new Team
				{
					Id = 6,
					Name = "Cincinatti Bengals", // CHECK 
					Abbreviation = "CIN"
				},
				new Team
				{
					Id = 7,
					Name = "Cleveland Browns",
					Abbreviation = "CLE"
				},
				new Team
				{
					Id = 8,
					Name = "Dallas Cowboys",
					Abbreviation = "DAL"
				},
				new Team
				{
					Id = 9,
					Name = "Denver Broncos",
					Abbreviation = "DEN"
				},
				new Team
				{
					Id = 10,
					Name = "Detroit Lions",
					Abbreviation = "DET"
				},
				new Team
				{
					Id = 11,
					Name = "Green Bay Packers",
					Abbreviation = "GB"
				},
				new Team
				{
					Id = 12,
					Name = "Tenessee Titans", // check
					Abbreviation = "TEN"
				},
				new Team
				{
					Id = 13,
					Name = "Houston Texans",
					Abbreviation = "HOU"
				},
				new Team
				{
					Id = 14,
					Name = "Indianapolis Colts", // check
					Abbreviation = "IND"
				},
				new Team
				{
					Id = 15,
					Name = "Jacksonville Jaguars",//check
					Abbreviation = "JAX"
				},
				new Team
				{
					Id = 16,
					Name = "Kansas City Chiefs",
					Abbreviation = "KC"
				},
				new Team
				{
					Id = 17,
					Name = "Los Angeles Rams", // check
					Abbreviation = "LA" // check chargers
				},
				new Team
				{
					Id = 18,
					Name = "Oakland Raiders",
					Abbreviation = "OAK"
				},
				new Team
				{
					Id = 19,
					Name = "Miami Dolphins",
					Abbreviation = "MIA"
				},
				new Team
				{
					Id = 20,
					Name = "Minnesota Vikings",//check
					Abbreviation = "MIN"
				},
				new Team
				{
					Id = 21,
					Name = "New England Patriots",
					Abbreviation = "NE"
				},
				new Team
				{
					Id = 22,
					Name = "New Orleans Saints", // check
					Abbreviation = "NO"
				},
				new Team
				{
					Id = 23,
					Name = "New York Giants",
					Abbreviation = "NYG"
				},
				new Team
				{
					Id = 24,
					Name = "New York Jets",
					Abbreviation = "NYJ"
				},
				new Team
				{
					Id = 25,
					Name = "Philadelphia Eagles",//check
					Abbreviation = "PHI"
				},
				new Team
				{
					Id = 26,
					Name = "Arizona Cardinals",
					Abbreviation = "ARI"
				},
				new Team
				{
					Id = 27,
					Name = "Pittsburgh Steelers",//check
					Abbreviation = "PIT"
				},
				new Team
				{
					Id = 28,
					Name = "Los Angeles Chargers",// check
					Abbreviation = "LAC"
				},
				new Team
				{
					Id = 29,
					Name = "San Francisco 49ers",//check
					Abbreviation = "SF"
				},
				new Team
				{
					Id = 30,
					Name = "Seattle Seahawks",
					Abbreviation = "SEA",
					RosterPageUri = @"http://www.nfl.com/teams/seattleseahawks/roster?team=SEA"
				},
				new Team
				{
					Id = 31,
					Name = "Tampa Bay Buccaneers",//check
					Abbreviation = "TB"
				},
				new Team
				{
					Id = 32,
					Name = "Washington Redskins",
					Abbreviation = "WAS"
				}
			};
		}
	}
}
