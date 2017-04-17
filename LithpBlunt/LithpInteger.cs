using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LithpBlunt.ExtensionMethods;

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

		public LithpInteger(string value)
		{
			this.value = NumberType.Parse(value);
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

		protected override LithpPrimitive cast(LithpType newType)
		{
			switch(newType)
			{
				case LithpBlunt.LithpType.STRING:
					return value.ToString();
				case LithpBlunt.LithpType.FLOAT:
					return new LithpFloat(value.ToString());
				default:
					throw new NotImplementedException();
			}
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

		protected override LithpPrimitive operatorModulo(LithpPrimitive other)
		{
			LithpInteger iOther = (LithpInteger)other.Cast(LithpBlunt.LithpType.INTEGER);
			return new LithpInteger(value % iOther.value);
		}

		protected override LithpPrimitive operatorBinaryAnd(LithpPrimitive other)
		{
			LithpInteger iOther = (LithpInteger)other.Cast(LithpBlunt.LithpType.INTEGER);
			return new LithpInteger(value & iOther.value);
		}

		protected override LithpPrimitive operatorBinaryOr(LithpPrimitive other)
		{
			LithpInteger iOther = (LithpInteger)other.Cast(LithpBlunt.LithpType.INTEGER);
			return new LithpInteger(value | iOther.value);
		}

		protected override LithpPrimitive operatorBinaryXor(LithpPrimitive other)
		{
			LithpInteger iOther = (LithpInteger)other.Cast(LithpBlunt.LithpType.INTEGER);
			return new LithpInteger(value ^ iOther.value);
		}

		public static implicit operator LithpInteger(NumberType v)
		{
			return new LithpInteger(v);
		}

		public static implicit operator NumberType(LithpInteger v)
		{
			return v.value;
		}

		public static implicit operator LithpInteger(int v)
		{
			return new LithpInteger(v);
		}

		public static implicit operator int(LithpInteger v)
		{
			return (int)v.value;
		}

		public static implicit operator LithpInteger(long v)
		{
			return new LithpInteger(v);
		}

		public static implicit operator long(LithpInteger v)
		{
			return (long)v.value;
		}

		/// <summary>
		/// An implementation-independant Sqrt implementation.
		/// </summary>
		/// <returns></returns>
		public LithpPrimitive Sqrt()
		{
			return new LithpInteger(value.Sqrt());
		}
	}
}
