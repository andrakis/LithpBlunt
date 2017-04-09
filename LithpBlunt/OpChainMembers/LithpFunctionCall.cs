using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt.OpChainMembers
{
	public class LithpFunctionCall : LithpOpChainMember
	{
		public readonly LithpAtom Function;
		public readonly LithpList Parameters;

		public LithpFunctionCall(LithpAtom function, LithpList parameters)
		{
			this.Function = function;
			this.Parameters = parameters;
		}
	
		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.FUNCTIONCALL;
		}

		protected override int hashCode()
		{
			return Function.GetHashCode() * 17 + Parameters.GetHashCode();
		}

		protected override string toString()
		{
			string result = "(" + Function + " ";
			bool first = true;
			foreach(LithpPrimitive x in Parameters)
			{
				if (!first)
					result += " ";
				else
					first = false;
				result += x.ToLiteral();
			}
			return result + ")";
		}

		internal static LithpFunctionCall New(LithpAtom name, params LithpPrimitive[] parameters)
		{
			return new LithpFunctionCall(name, new LithpList(parameters));
		}
	}
}
