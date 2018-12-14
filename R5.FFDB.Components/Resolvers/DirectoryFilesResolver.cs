using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace R5.FFDB.Components.Resolvers
{
	public static class DirectoryFilesResolver
	{
		public static List<string> GetFileNames(string directoryPath,
			string validateNameRegex = null, bool excludeExtensions = false)
		{
			IEnumerable<string> fileNames = new DirectoryInfo(directoryPath)
				.GetFiles()
				.Select(f => f.Name);

			if (validateNameRegex != null
				&& !excludeExtensions
				&& fileNames.Any(n => !Regex.IsMatch(n, validateNameRegex)))
			{
				throw new InvalidOperationException(
					$"There are some invalid files in directory '{directoryPath}'.");
			}

			if (excludeExtensions)
			{
				fileNames = fileNames.Select(WithoutFileExtension);
			}

			return fileNames.ToList();
		}

		public static List<WeekInfo> GetWeeksFromJsonFiles(string directoryPath)
		{
			List<string> fileNames = GetFileNames(directoryPath, RegexConstants.SeasonWeekJsonFile);
			return fileNames.Select(ParseWeekFromFileName).ToList();
		}

		public static List<WeekInfo> GetWeeksFromXmlFiles(string directoryPath)
		{
			List<string> fileNames = GetFileNames(directoryPath, RegexConstants.SeasonWeekXmlFile);
			return fileNames.Select(ParseWeekFromFileName).ToList();
		}

		private static WeekInfo ParseWeekFromFileName(string fileName)
		{
			string[] dashSplit = WithoutFileExtension(fileName).Split("-");
			return new WeekInfo(int.Parse(dashSplit[0]), int.Parse(dashSplit[1]));
		}

		// always assume that filenames never contain periods other than the extension
		private static string WithoutFileExtension(string fileName)
		{
			return fileName.Split(".")[0];
		}
	}
}
