using HtmlAgilityPack;
using Newtonsoft.Json;
using R5.FFDB.CLI.Configuration;
using R5.FFDB.Components.CoreData.WeekStats.Models;
using R5.FFDB.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Console;

namespace R5.FFDB.CLI
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			WriteLine("Parsing config");
			FfdbConfig config = FileConfigResolver.FromFile(@"D:\Repos\ffdb_data_2\test_config.json");


			return;
		}




		
	}
}
