using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt
{
	public class LithpInteger : LithpPrimitive
	{
		protected long value = 0;

		public LithpInteger(long value)
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

		public static implicit operator LithpInteger(long v)
		{
			return new LithpInteger(v);
		}
	}
}
