using HtmlAgilityPack;
using Newtonsoft.Json;
using R5.FFDB.Core.Components.FantasyApi.Models;
using R5.FFDB.Core.Components.Setup.Models;
using R5.FFDB.Core.Components.WebScrape.NFL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Core.Components.Setup.Services
{
	public class InitialSetupService
	{
		// todo: move to config
		private FfdbConfig _config { get; } = new FfdbConfig();

		public InitialSetupService()
		{

		}

		// phase 1: fetch all week stats

		// phase 2: fetch all players based on player nfl ids from #1

		

		
		
	}
}
