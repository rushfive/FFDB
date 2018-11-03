using R5.FFDB.Core.Abstractions;
using R5.FFDB.Core.Components.FantasyApi.Models;
using R5.FFDB.Core.Stats;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Components.FantasyApi.Services
{
	public class StatsService
	{
		private FileService _fileService { get; }

		public StatsService(FileService fileService)
		{
			_fileService = fileService;
		}

		public WeekStats GetWeekStats(WeekInfo week)
		{
			WeekStatsJsonV2 statsJson = _fileService.GetWeekStats(week);
			return WeekStatsJsonV2.ToCoreEntity(statsJson);
		}
	}
}
