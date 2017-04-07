using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt
{
	public class LithpBuiltins
	{
		public Dictionary<LithpAtom, LithpFunctionDefinitionDelegate> builtins =
			new Dictionary<LithpAtom, LithpFunctionDefinitionDelegate>();

		public LithpBuiltins()
		{
			builtins["print/*"] = Print;
		}

		public LithpPrimitive Print(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			bool first = true;
			string result = "";

			foreach(LithpPrimitive x in parameters)
			{
				if (!first)
					result += " ";
				else
					first = false;
				result += x.ToString();
			}
			Console.WriteLine(result);
			return LithpAtom.Atom("nil");
		}
	}
}
