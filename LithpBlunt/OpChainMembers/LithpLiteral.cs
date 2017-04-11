using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt.OpChainMembers
{
	public class LithpLiteral : LithpOpChainMember
	{
		protected LithpPrimitive value;
		public LithpPrimitive Value {
			get { return value; }
		}
		public LithpLiteral(LithpPrimitive value)
		{
			this.value = value;
		}

		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.LITERAL;
		}

		public override bool compareEqual(LithpPrimitive other)
		{
			return value.compareEqual(other);
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
			return value + other;
		}

		protected override LithpPrimitive operatorMinus(LithpPrimitive other)
		{
			return value - other;
		}

		protected override LithpPrimitive operatorMultiply(LithpPrimitive other)
		{
			return value * other;
		}

		protected override LithpPrimitive operatorDivide(LithpPrimitive other)
		{
			return value / other;
		}

		public static implicit operator LithpInteger(LithpLiteral x)
		{
			return ((LithpInteger)x).value;
		}

		public static implicit operator LithpString(LithpLiteral x)
		{
			return ((LithpString)x).Value;
		}
	}
}
