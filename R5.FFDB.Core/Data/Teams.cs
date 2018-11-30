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
					NflId = "100001",
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
					NflId = "100002",
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
					NflId = "100003",
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
					NflId = "100004",
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
					NflId = "100005",
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
					NflId = "100006",
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
					NflId = "100007",
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
					NflId = "100008",
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
					NflId = "100009",
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
					NflId = "100010",
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
					NflId = "100011",
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
					NflId = "100012",
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
					NflId = "100013",
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
					NflId = "100014",
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
					NflId = "100015",
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
					NflId = "100016",
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
					NflId = "100017",
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
					NflId = "100018",
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
					NflId = "100019",
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
					NflId = "100020",
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
					NflId = "100021",
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
					NflId = "100022",
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
					NflId = "100023",
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
					NflId = "100024",
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
					NflId = "100025",
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
					NflId = "100026",
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
					NflId = "100027",
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
					NflId = "100028",
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
					NflId = "100029",
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
					NflId = "100030",
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
					NflId = "100031",
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
					NflId = "100032",
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
