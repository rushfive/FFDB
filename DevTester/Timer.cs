using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DevTester
{
	public static class Timer
	{
		public static T Time<T>(string operation, Func<T> func, TimerUnit? timerUnit = null)
		{
			var sw = new Stopwatch();
			sw.Start();

			T t = func();

			sw.Stop();

			Timer.LogElapsedTime(sw, operation, timerUnit);

			return t;
		}

		public static void Time(string operation, Action action, TimerUnit? timerUnit = null)
		{
			var sw = new Stopwatch();
			sw.Start();

			action();

			sw.Stop();

			Timer.LogElapsedTime(sw, operation, timerUnit);
		}

		private static void LogElapsedTime(Stopwatch sw, string operation, TimerUnit? timerUnit)
		{
			long ms = sw.ElapsedMilliseconds;
			string log = $"Operation '{operation}' took ";

			switch (timerUnit)
			{
				case TimerUnit.Milliseconds:
				case null:
					log += $"{ms}ms.";
					break;
				case TimerUnit.Seconds:
					log += $"{TimeSpan.FromMilliseconds(ms).TotalSeconds}sec.";
					break;
				case TimerUnit.Minutes:
					log += $"{TimeSpan.FromMilliseconds(ms).TotalMinutes}min.";
					break;
				case TimerUnit.Hours:
					log += $"{TimeSpan.FromMilliseconds(ms).TotalHours}hr.";
					break;
				case TimerUnit.Days:
					log += $"{TimeSpan.FromMilliseconds(ms).TotalDays}days.";
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(timerUnit));
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(log);
			Console.ResetColor();
		}
	}

	public enum TimerUnit
	{
		Milliseconds,
		Seconds,
		Minutes,
		Hours,
		Days
	}
}
