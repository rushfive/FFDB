using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components
{
	public static class PrimeGenerator
	{
		public static int StartingAt(int start)
		{
			while (true)
			{
				if (IsPrime(start))
				{
					return start;
				}

				start++;
			}
		}

		public static bool IsPrime(int number)
		{
			if ((number & 1) == 0)
			{
				return number == 2;
			}

			for (int i = 3; (i * i) <= number; i += 2)
			{
				if ((number % i) == 0)
				{
					return false;
				}
			}
			return number != 1;
		}
	}
}
