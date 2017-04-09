using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LithpBlunt.OpChainMembers;

namespace LithpBlunt
{
	public class LithpInterpreter
	{
		protected int depth = 0;

		public int Depth {
			get { return depth; }
		}
		public LithpPrimitive Run (LithpOpChain chain)
		{
			LithpPrimitive value = LithpAtom.Atom("nil");

			while (chain.AtEnd() == false)
			{
				LithpOpChainMember current = chain.Next();
				value = resolve(current, chain);
			}

			return value;
		}

		private LithpPrimitive resolve(LithpPrimitive current, LithpOpChain chain)
		{
			switch(current.LithpType())
			{
				case LithpType.LITERAL:
				case LithpType.ATOM:
				case LithpType.DICT:
				case LithpType.INTEGER:
				case LithpType.LIST:
				case LithpType.STRING:
				case LithpType.FN:
				case LithpType.FN_NATIVE:
					return current;
				case LithpType.FUNCTIONCALL:
					LithpFunctionCall call = (LithpFunctionCall)current;
					LithpList resolved = ResolveParameters(call, chain);
					return InvokeResolved(call, resolved, chain);
				case LithpType.VAR:
					// TODO: Could just lookup the value now
					LithpVariableReference v = (LithpVariableReference)current;
					return v.Name;
				default:
					throw new NotImplementedException();
			}
		}

		public LithpList ResolveParameters(LithpFunctionCall call, LithpOpChain chain)
		{
			LithpList result = new LithpList();
			foreach(var x in call.Parameters)
			{
				result.Add(resolve(x, chain));
			}
			return result;
		}

		public LithpPrimitive InvokeResolved (LithpFunctionCall call,
			LithpList parameters, LithpOpChain chain)
		{
			string name = call.Function;
			// Use the interface so that we can invoke native and Lithp methods.
			ILithpFunctionDefinition def;
			if(chain.Closure.TopMost && chain.Closure.TopMost.IsDefined(call.Function))
			{
				def = chain.Closure.TopMost[call.Function] as ILithpFunctionDefinition;
			} else
			if(chain.Closure.IsDefinedAny(call.Function))
			{
				def = chain.Closure[call.Function] as ILithpFunctionDefinition;
			} else
			{
				string arityStar = Regex.Replace(call.Function, @"/\d+$/", "*");
				name = arityStar;
				if(chain.Closure.IsDefinedAny(arityStar))
				{
					def = chain.Closure[arityStar] as ILithpFunctionDefinition;
				} else
				{
					throw new MissingMethodException();
				}
			}
			try
			{
				LithpPrimitive result;
				if (chain.LithpType() == LithpType.FN)
					chain.FunctionEntry = name;
				result = def.Invoke(parameters, chain, this);
				return result;
			}
			finally
			{
				depth--;
			}
		}
	}
}
