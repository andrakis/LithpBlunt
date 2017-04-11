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

		public static readonly bool Debug;
		public static readonly int MaxDebugLen = 100;
		public static readonly int MaxDebugArrayLength = 100;

#if DEBUG
		static LithpInterpreter()
		{
			Debug = true;
		}
#else
		static LithpInterpreter()
		{
			Debug = false;
		}
#endif

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
				case LithpType.OPCHAIN:
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

		protected static string inspect(LithpPrimitive value)
		{
			if(value.LithpType() == LithpType.LIST)
			{
				LithpList list = (LithpList)value;
				if (list.Count > MaxDebugArrayLength)
					return "[" + list.Count.ToString() + " elements]";
			}
			if(value.LithpType() == LithpType.DICT)
			{
				LithpDict dict = (LithpDict)value;
				if (dict.Keys.Count > MaxDebugArrayLength)
					return "{Dict:" + dict.Keys.Count.ToString() + " elements}";
			}
			string result = value.ToLiteral();
			if (result.Length > MaxDebugLen)
				result = "(" + value.LithpType().ToString() + ": too large to display)";
			return result;
		}

		public static string Inspect(LithpList value)
		{
			LithpList mapped = value.Map((v) => inspect(v));
			string result = "";
			bool first = true;
			foreach(var x in mapped)
			{
				if (!first)
					result += " ";
				else
					first = false;
				result += x;
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
				def = (ILithpFunctionDefinition)chain.Closure.TopMost[call.Function];
			} else
			if(chain.Closure.IsDefinedAny(call.Function))
			{
				def = (ILithpFunctionDefinition)chain.Closure[call.Function];
			} else
			{
				string arityStar = Regex.Replace(call.Function, @"/\d+$/", "*");
				name = arityStar;
				if(chain.Closure.IsDefinedAny(arityStar))
				{
					def = (ILithpFunctionDefinition)chain.Closure[arityStar];
				} else
				{
					throw new MissingMethodException();
				}
			}

#if DEBUG
			string debug_str = null;
			{
				string indent;
				if(depth < 20)
					indent = new string('|', depth + 1);
				else
					indent = "|             " + depth + " | |";
				debug_str = "+ " + indent + " (" + def.Name +
					(parameters.Count > 0 ? (" " + Inspect(parameters)) : "") + ")";
			}
#endif

			depth++;

			try
			{
				LithpPrimitive result;
				if (chain.LithpType() == LithpType.FN)
					chain.FunctionEntry = name;
#if DEBUG
				{
					if (def.LithpType() == LithpType.FN)
						Console.WriteLine(debug_str);
					else
						switch(def.Name)
						{
							case "while":
							case "call":
							case "try":
							case "eval":
							case "apply":
							case "next":
							case "recurse":
								Console.WriteLine(debug_str);
								break;
						}
				}
#endif
				result = def.Invoke(parameters, chain, this);
#if DEBUG
				{
					string literal = result.ToLiteral();
					if (literal.Length > MaxDebugLen)
						literal = "(" + result.LithpType().ToString() + ": too large to display)";
					debug_str += " :: " + literal;
					Console.WriteLine(debug_str);
				}
#endif
				return result;
			}
			finally
			{
				depth--;
			}
		}
	}
}
