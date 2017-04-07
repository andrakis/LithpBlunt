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
	}
}
