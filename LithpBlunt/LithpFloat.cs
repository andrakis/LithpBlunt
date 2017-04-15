using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NumberType = System.Double;

namespace LithpBlunt
{
	public class LithpFloat : LithpPrimitive
	{
		public readonly NumberType value = 0.0;

		public LithpFloat(NumberType value)
		{
			this.value = value;
		}

		public LithpFloat(string value)
		{
			this.value = NumberType.Parse(value);
		}

		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.FLOAT;
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
			LithpFloat iOther = (LithpFloat)other;
			return new LithpFloat(value + iOther.value);
		}

		protected override LithpPrimitive operatorMinus(LithpPrimitive other)
		{
			LithpFloat iOther = (LithpFloat)other;
			return new LithpFloat(value - iOther.value);
		}

		protected override LithpPrimitive operatorMultiply(LithpPrimitive other)
		{
			LithpFloat iOther = (LithpFloat)other;
			return new LithpFloat(value * iOther.value);
		}

		protected override LithpPrimitive operatorDivide(LithpPrimitive other)
		{
			LithpFloat iOther = (LithpFloat)other;
			return new LithpFloat(value / iOther.value);
		}

		public override bool compareEqual(LithpPrimitive other)
		{
			LithpFloat iOther = (LithpFloat)other;
			return value == iOther.value;
		}

		protected override bool compareLessThan(LithpPrimitive other)
		{
			LithpFloat iOther = (LithpFloat)other;
			return value < iOther.value;
		}

		protected override bool compareMoreThan(LithpPrimitive other)
		{
			LithpFloat iOther = (LithpFloat)other;
			return value > iOther.value;
		}

		protected override LithpPrimitive cast(LithpType newType)
		{
			switch(newType)
			{
				case LithpBlunt.LithpType.STRING:
					return value.ToString();
				case LithpBlunt.LithpType.INTEGER:
					return new LithpInteger(Convert.ToInt64(value));
				default:
					throw new NotImplementedException();
			}
		}

		public static implicit operator LithpFloat(NumberType v)
		{
			return new LithpFloat(v);
		}

		public static implicit operator LithpFloat(int v)
		{
			return new LithpFloat(v);
		}
	}
}
