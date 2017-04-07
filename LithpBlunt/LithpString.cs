using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt
{
	public class LithpString : LithpPrimitive
	{
		protected readonly string value;

		public LithpString(string value)
		{
			this.value = value;
		}

		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.STRING;
		}

		public override bool compareEqual(LithpPrimitive other)
		{
			// TODO: Find better way to do this
			LithpString otherString = (LithpString)other;
			return value == otherString.value;
		}

		protected override int hashCode()
		{
			return value.GetHashCode();
		}

		protected override string toString()
		{
			return value;
		}

		public static implicit operator LithpString(string value)
		{
			return new LithpString(value);
		}

		protected override LithpPrimitive operatorPlus(LithpPrimitive other)
		{
			return value + other.ToString();
		}
	}
}
