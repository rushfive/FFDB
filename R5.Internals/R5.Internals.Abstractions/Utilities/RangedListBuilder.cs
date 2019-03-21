using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.Internals.Abstractions.Utilities
{
	public static class RangedListBuilder
	{
		public static List<string> Build(List<int> numbers)
		{
			if (numbers == null || !numbers.Any())
			{
				throw new ArgumentException("At least one number must be provided.");
			}

			var result = new List<string>();

			numbers.Sort();
			var currentRange = new List<int>();

			for (int i = 0; i < numbers.Count; i++)
			{
				if (!currentRange.Any())
				{
					currentRange.Add(numbers[i]);
					continue;
				}

				if (currentRange.Last() == numbers[i] - 1)
				{
					currentRange.Add(numbers[i]);
					continue;
				}

				// end of a "range", add to result
				result.Add(GetRangeLabel(currentRange));
				currentRange.Clear();
				currentRange.Add(numbers[i]);
			}

			if (currentRange.Any())
			{
				result.Add(GetRangeLabel(currentRange));
			}

			return result;
		}

		private static string GetRangeLabel(List<int> range)
		{
			if (range.Count == 1)
			{
				return range.First().ToString();
			}
			else
			{
				return $"{range.First()}-{range.Last()}";
			}
		}
	}
}
