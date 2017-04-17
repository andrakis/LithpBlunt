using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt.ExtensionMethods
{
	public static class BigNumberExtensionMethods
	{
#if true
		/// <summary>
		/// Slow Squareroot implementation.
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public static BigInteger Sqrt(this BigInteger n)
		{
			if (n == 0) return 0;
			if(n > 0)
			{
				int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
				BigInteger root = BigInteger.One << (bitLength / 2);

				while (!isSqrt(n, root))
				{
					root += n / root;
					//root /= 2;
					root = root >> 1;
				}

				return root;
			}

			throw new ArithmeticException("NaN");
		}

		public static Boolean isSqrt(BigInteger n, BigInteger root)
		{
			BigInteger lowerBound = root * root;
			//BigInteger upperBound = (root + 1) * (root + 1);
			//return (n >= lowerBound && n < upperBound);
			return n >= lowerBound && n <= lowerBound + root + root;
		}
#else
		public static BigInteger Sqrt(this BigInteger N)
		{
			BigInteger q = N;
			int two_pows = 1;
			int iters = 0;

			// Handle 1
			if (q == 1)
			{
				return q;
			}

			// Get powers of 2 for N
			while (q > 1)
			{
				two_pows++;
				q /= 2;
			}

			// Divide by 2 to get the root
			two_pows /= 2;

			// First approximation
			BigInteger t = N / BigInteger.Pow(2, two_pows);
			iters = 0;

			while (true)
			{
				BigInteger p = t * (t + 2) + 1;

				if (p == N)
				{
					return t + 1;
				}

				BigInteger e = N - p;
				BigInteger correction = (2 * (t + 1)) + 1;

				t += e / correction;

				if (t * t == N)
				{
					return t;
				}

				iters++;
			}
		}
#endif
	}
}
