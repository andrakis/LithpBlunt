using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt.OpChainMembers
{
	public class LithpFunctionCall : LithpOpChainMember
	{
		protected readonly LithpAtom function;
		protected readonly LithpList parameters;

		public LithpFunctionCall(LithpAtom function, LithpList parameters)
		{
			this.function = function;
			this.parameters = parameters;
		}
	
		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.FUNCTIONCALL;
		}

		protected override int hashCode()
		{
			return function.GetHashCode() + parameters.GetHashCode();
		}

		protected override string toString()
		{
			string result = "(" + function + " ";
			bool first = true;
			foreach(LithpPrimitive x in parameters)
			{
				if (!first)
					result += " ";
				else
					first = false;
				result += x.ToLiteral();
			}
			return result + ")";
		}
	}
}
