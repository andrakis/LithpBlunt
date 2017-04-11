using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NumberType = System.Numerics.BigInteger;

namespace LithpBlunt
{
	public class LithpInteger : LithpPrimitive
	{
		public readonly NumberType value = 0;

		public LithpInteger(NumberType value)
		{
			this.value = value;
		}

		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.INTEGER;
		}

		protected override int hashCode()
		{
			return value.GetHashCode();
		}

		protected override string toString()
		{
			return value.ToString();
		}

		protected override LithpPrimitive operatorPlus(LithpPrimitive other)
		{
			LithpInteger iOther = (LithpInteger)other;
			return new LithpInteger(value + iOther.value);
		}

		protected override LithpPrimitive operatorMinus(LithpPrimitive other)
		{
			LithpInteger iOther = (LithpInteger)other;
			return new LithpInteger(value - iOther.value);
		}

		protected override LithpPrimitive operatorMultiply(LithpPrimitive other)
		{
			LithpInteger iOther = (LithpInteger)other;
			return new LithpInteger(value * iOther.value);
		}

		protected override LithpPrimitive operatorDivide(LithpPrimitive other)
		{
			LithpInteger iOther = (LithpInteger)other;
			return new LithpInteger(value / iOther.value);
		}

		public override bool compareEqual(LithpPrimitive other)
		{
			LithpInteger iOther = (LithpInteger)other;
			return value == iOther.value;
		}

		protected override bool compareLessThan(LithpPrimitive other)
		{
			LithpInteger iOther = (LithpInteger)other;
			return value < iOther.value;
		}

		protected override bool compareMoreThan(LithpPrimitive other)
		{
			LithpInteger iOther = (LithpInteger)other;
			return value > iOther.value;
		}

		public static implicit operator LithpInteger(NumberType v)
		{
			return new LithpInteger(v);
		}

		public static implicit operator LithpInteger(int v)
		{
			return new LithpInteger(v);
		}
	}
}
