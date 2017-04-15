using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt
{
	public class LithpString : LithpPrimitive
	{
		public readonly string Value;

		public LithpString(string value)
		{
			this.Value = value;
		}

		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.STRING;
		}

		public override bool compareEqual(LithpPrimitive other)
		{
			// TODO: Find better way to do this
			LithpString otherString = (LithpString)other;
			return Value == otherString.Value;
		}

		protected override int hashCode()
		{
			return Value.GetHashCode();
		}

		protected override string toString()
		{
			return Value;
		}

		protected override LithpPrimitive cast(LithpType newType)
		{
			switch(newType)
			{
				case LithpBlunt.LithpType.INTEGER:
					return new LithpInteger(Value);
				case LithpBlunt.LithpType.FLOAT:
					return new LithpFloat(Value);
				case LithpBlunt.LithpType.ATOM:
					return LithpAtom.Atom(Value);
				default:
					throw new NotImplementedException();
			}
		}

		public static implicit operator LithpString(string value)
		{
			return new LithpString(value);
		}

		protected override LithpPrimitive operatorPlus(LithpPrimitive other)
		{
			return Value + other.ToString();
		}
	}
}
