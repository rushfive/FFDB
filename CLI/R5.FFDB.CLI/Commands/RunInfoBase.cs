﻿using R5.RunInfoBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI.Commands
{
	public abstract class RunInfoBase
	{
		public abstract string CommandKey { get; }

		public string ConfigFilePath { get; set; }
		public bool SkipRosterFetch { get; set; }
		public bool SaveToDisk { get; set; }
		public bool SaveOriginalSourceFiles { get; set; }

		public static void AddCommonOptions<T>(Command<T> command)
			where T : RunInfoBase
		{
			if (command.GlobalOptions == null)
			{
				command.GlobalOptions = new List<OptionBase<T>>();
			}

			command.GlobalOptions.Add(new Option<T, string>
			{
				Key = "config | c",
				Property = ri => ri.ConfigFilePath
			});

			command.GlobalOptions.Add(new Option<T, bool>
			{
				Key = "skip-roster",
				Property = ri => ri.SkipRosterFetch
			});

			//command.GlobalOptions.Add(new Option<T, bool>
			//{
			//	Key = "save-to-disk",
			//	Property = ri => ri.SaveToDisk
			//});

			//command.GlobalOptions.Add(new Option<T, bool>
			//{
			//	Key = "save-src-files",
			//	Property = ri => ri.SaveOriginalSourceFiles
			//});
		}
	}
}
