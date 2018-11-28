using R5.FFDB.Core.Models;
using R5.FFDB.Core.Sources;
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
					Abbreviation = "ATL",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/atlantafalcons/roster?team=ATL"
						}
					}
				},
				new Team
				{
					Id = 2,
					Name = "Baltimore Ravens",
					Abbreviation = "BAL",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/baltimoreravens/roster?team=BAL"
						}
					}
				},
				new Team
				{
					Id = 3,
					Name = "Buffalo Bills",
					Abbreviation = "BUF",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/buffalobills/roster?team=BUF"
						}
					}
				},
				new Team
				{
					Id = 4,
					Name = "Carolina Panthers",
					Abbreviation = "CAR",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/carolinapanthers/roster?team=CAR"
						}
					}
				},
				new Team
				{
					Id = 5,
					Name = "Chicago Bears",
					Abbreviation = "CHI",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/chicagobears/roster?team=CHI"
						}
					}
				},
				new Team
				{
					Id = 6,
					Name = "Cincinnati Bengals",
					Abbreviation = "CIN",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/cincinnatibengals/roster?team=CIN"
						}
					}
				},
				new Team
				{
					Id = 7,
					Name = "Cleveland Browns",
					Abbreviation = "CLE",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/clevelandbrowns/roster?team=CLE"
						}
					}
				},
				new Team
				{
					Id = 8,
					Name = "Dallas Cowboys",
					Abbreviation = "DAL",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/dallascowboys/roster?team=DAL"
						}
					}
				},
				new Team
				{
					Id = 9,
					Name = "Denver Broncos",
					Abbreviation = "DEN",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/denverbroncos/roster?team=DEN"
						}
					}
				},
				new Team
				{
					Id = 10,
					Name = "Detroit Lions",
					Abbreviation = "DET",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/detroitlions/roster?team=DET"
						}
					}
				},
				new Team
				{
					Id = 11,
					Name = "Green Bay Packers",
					Abbreviation = "GB",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/greenbaypackers/roster?team=GB"
						}
					}
				},
				new Team
				{
					Id = 12,
					Name = "Tennessee Titans",
					Abbreviation = "TEN",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/tennesseetitans/roster?team=TEN"
						}
					}
				},
				new Team
				{
					Id = 13,
					Name = "Houston Texans",
					Abbreviation = "HOU",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/houstontexans/roster?team=HOU"
						}
					}
				},
				new Team
				{
					Id = 14,
					Name = "Indianapolis Colts",
					Abbreviation = "IND",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/indianapoliscolts/roster?team=IND"
						}
					}
				},
				new Team
				{
					Id = 15,
					Name = "Jacksonville Jaguars",
					Abbreviation = "JAX",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/jacksonvillejaguars/roster?team=JAX"
						}
					}
				},
				new Team
				{
					Id = 16,
					Name = "Kansas City Chiefs",
					Abbreviation = "KC",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/kansascitychiefs/roster?team=KC"
						}
					}
				},
				new Team
				{
					Id = 17,
					Name = "Los Angeles Rams",
					Abbreviation = "LA",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/losangelesrams/roster?team=LA"
						}
					}
				},
				new Team
				{
					Id = 18,
					Name = "Oakland Raiders",
					Abbreviation = "OAK",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/oaklandraiders/roster?team=OAK"
						}
					}
				},
				new Team
				{
					Id = 19,
					Name = "Miami Dolphins",
					Abbreviation = "MIA",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/miamidolphins/roster?team=MIA"
						}
					}
				},
				new Team
				{
					Id = 20,
					Name = "Minnesota Vikings",
					Abbreviation = "MIN",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/minnesotavikings/roster?team=MIN"
						}
					}
				},
				new Team
				{
					Id = 21,
					Name = "New England Patriots",
					Abbreviation = "NE",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/newenglandpatriots/roster?team=NE"
						}
					}
				},
				new Team
				{
					Id = 22,
					Name = "New Orleans Saints",
					Abbreviation = "NO",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/neworleanssaints/roster?team=NO"
						}
					}
				},
				new Team
				{
					Id = 23,
					Name = "New York Giants",
					Abbreviation = "NYG",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/newyorkgiants/roster?team=NYG"
						}
					}
				},
				new Team
				{
					Id = 24,
					Name = "New York Jets",
					Abbreviation = "NYJ",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/newyorkjets/roster?team=NYJ"
						}
					}
				},
				new Team
				{
					Id = 25,
					Name = "Philadelphia Eagles",
					Abbreviation = "PHI",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/philadelphiaeagles/roster?team=PHI"
						}
					}
				},
				new Team
				{
					Id = 26,
					Name = "Arizona Cardinals",
					Abbreviation = "ARI",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/arizonacardinals/roster?team=ARI"
						}
					}
				},
				new Team
				{
					Id = 27,
					Name = "Pittsburgh Steelers",
					Abbreviation = "PIT",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/pittsburghsteelers/roster?team=PIT"
						}
					}
				},
				new Team
				{
					Id = 28,
					Name = "Los Angeles Chargers",
					Abbreviation = "LAC",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/losangeleschargers/roster?team=LAC"
						}
					}
				},
				new Team
				{
					Id = 29,
					Name = "San Francisco 49ers",
					Abbreviation = "SF",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/sanfrancisco49ers/roster?team=SF"
						}
					}
				},
				new Team
				{
					Id = 30,
					Name = "Seattle Seahawks",
					Abbreviation = "SEA",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/seattleseahawks/roster?team=SEA"
						}
					}
				},
				new Team
				{
					Id = 31,
					Name = "Tampa Bay Buccaneers",
					Abbreviation = "TB",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/tampabaybuccaneers/roster?team=TB"
						}
					}
				},
				new Team
				{
					Id = 32,
					Name = "Washington Redskins",
					Abbreviation = "WAS",
					RosterSourceUris = new Dictionary<string, string>
					{
						{
							RosterSourceKeys.NFLWebTeam,
							@"http://www.nfl.com/teams/washingtonredskins/roster?team=WAS"
						}
					}
				}
			};
		}
	}
}
