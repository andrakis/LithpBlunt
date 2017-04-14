/// Use Json.Net to parse Lithp AST trees into a LithpOpChain object.
/// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LithpBlunt;
using LithpBlunt.OpChainMembers;
using Newtonsoft.Json.Linq;

namespace LithpBlunt.Serialization
{
	public class LithpJsonParser
	{
		public LithpOpChain Test ()
		{
			string complex = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "samples", "factorial.ast"));
			dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(complex);
			//Console.WriteLine(jsonData);
			return Parse(jsonData);
		}

		public LithpOpChain Parse(dynamic jsonData)
		{
			var chain = new LithpOpChain();
			return Parse(jsonData, chain);
		}

		public LithpOpChain Parse(dynamic data, LithpOpChain chain)
		{
			foreach(dynamic element in data)
			{
				//Console.WriteLine("Element: {0}", element);
				Console.WriteLine("Typeof: " + element.GetType());
				var c = convert(chain, element);
				if((object)c != null)
					chain.Add(c);
			}
			return chain;
		}

		protected void parserDebug (string format, params object[] parameters)
		{
#if DEBUG
			Console.WriteLine(format, parameters);
#else
#endif
		}

		protected ILithpOpChainMember mapParam(dynamic P, LithpOpChain chain, string fnName = "")
		{
			var result = this.mapParamInner(P, chain, fnName);
			parserDebug("mapParam({0}) :: {1}", P, result);
			return result;
		}

		protected ILithpOpChainMember mapParamInner(JArray P, LithpOpChain chain, string fnName = "")
		{
			return this.convert(chain, P);
		}
		protected ILithpOpChainMember mapParamInner(JObject P, LithpOpChain chain, string fnName = "")
		{
			return this.convert(chain, P);
		}
		protected ILithpOpChainMember mapParamInner(dynamic P, LithpOpChain chain, string fnName = "")
		{
			string pStr = P.ToString();
			Classification cls = LithpParser.Classify(pStr);
			parserDebug("Classified: {0}", cls.ToString());
			if (cls.HasFlag(Classification.STRING_DOUBLE) || cls.HasFlag(Classification.STRING_SINGLE))
			{
				string parsed = LithpParser.ParseEscapes(pStr.Substring(1, pStr.Length - 2));
				if (cls.HasFlag(Classification.STRING_DOUBLE))
					return new LithpLiteral(parsed);
				else if (cls.HasFlag(Classification.STRING_SINGLE))
					return LithpAtom.Atom(parsed);
			}
			else if (cls.HasFlag(Classification.VARIABLE))
			{
				switch (fnName)
				{
					case "get":
					case "set":
					case "var":
						return new LithpVariableReference(pStr);
					default:
						return LithpFunctionCall.New("get/1", new LithpVariableReference(pStr));
				}
			}
			else if (cls.HasFlag(Classification.NUMBER))
			{
				return new LithpInteger(pStr);
			}
			else if (cls.HasFlag(Classification.ATOM))
			{
				return LithpAtom.Atom(pStr);
			}
			throw new NotImplementedException();
		}

		protected ILithpOpChainMember convert(LithpOpChain chain, JArray curr)
		{
			dynamic eleFirst = curr[0];
			if(eleFirst.GetType() == typeof(JArray))
			{
				// Must be an OpChain
				LithpOpChain newChain = new LithpOpChain(chain);
				for(int i = 0; i < curr.Count; i++)
				{
					//parserDebug("Member {0} of chain: {1}", i, curr[i]);
					JToken t = curr[i];
					ILithpOpChainMember member;
					if (t.GetType() == typeof(JArray))
						member = this.convert(newChain, (JArray)t);
					else if (t.GetType() == typeof(JObject))
						member = this.convert(newChain, (JObject)t);
					else
						throw new NotImplementedException();
					newChain.Add(member);
				}
				return newChain;
			}
			Classification clsFirst = LithpParser.Classify(eleFirst.ToString());
			parserDebug("  First element: {0}", eleFirst);
			parserDebug("     Classified: {0}", clsFirst.ToString());
			if (curr.Count == 0)
				throw new NotImplementedException();
			if(clsFirst.HasFlag(Classification.STRING_SINGLE))
			{
				// Convert to a (call (get 'FnName') Params...)
				parserDebug("STRING_SINGLE, convert to FunctionCall");
				JValue v = eleFirst as JValue;
				string tmp = v.ToString();
				eleFirst = tmp.Substring(1, tmp.Length - 2);
				clsFirst = LithpParser.Classify(eleFirst);
				parserDebug("  First element: {0}", eleFirst);
				parserDebug("  Re-Classified: {0}", clsFirst.ToString());
			}
			if(clsFirst.HasFlag(Classification.ATOM))
			{
				// Function call
				parserDebug(" PARSE TO FUNCTIONCALL: {0}", curr);
				var skipped = curr.Skip(1);
				LithpList parameters = new LithpList();
				foreach (var x in skipped)
					parameters.Add(this.mapParam(x, chain, eleFirst.ToString()));
				if(parameters.Count == 0 && clsFirst.HasFlag(Classification.NUMBER))
				{
					parserDebug("CONVERT TO LITERAL");
					return this.mapParam(eleFirst, chain, eleFirst.ToString());
				} else
				{
					string plen = parameters.Count.ToString();
					if (LithpParser.arityBuiltins.ContainsKey(eleFirst.ToString()))
						plen = LithpParser.arityBuiltins[eleFirst.ToString()];
					parserDebug("FUNCTIONCALL {0}/{1}", eleFirst, plen);
					string name = eleFirst.ToString() + "/" + plen;
					return new LithpFunctionCall(name, parameters);
				}
			} else if(curr.Count > 0)
			{
				// Must be an OpChain
				parserDebug(" PARSE TO OPCHAIN");
				LithpOpChain newChain = new LithpOpChain(chain);
				for(int i = 0; i < curr.Count; i++)
				{
					parserDebug("Member {0} of chain: {1}", i, curr[i]);
					newChain.Add(this.mapParamInner(curr[i], chain, eleFirst));
				}
				return newChain;
			}
			throw new NotImplementedException();
		}

		private ILithpOpChainMember convert(LithpOpChain newChain, JToken jToken)
		{
			throw new NotImplementedException();
		}

		protected static int AnonymousFnCounter = 0;
		protected ILithpOpChainMember convert(LithpOpChain chain, JObject curr)
		{
			JArray fncode = (JArray)curr["code"];
			JArray fnparams = (JArray)curr["_fnparams"];
			List<string> defParams = new List<string>();
			foreach (JValue p in fnparams)
			{
				defParams.Add(p.ToString());
				Console.WriteLine("Param: {0}", p.ToString());
			}
			//Console.WriteLine("FunctionDef with params {0}, code: {1}", defParams.ToArray(), fncode);
			LithpOpChain defBody = (LithpOpChain)convert(chain, fncode);
			string name = "anonymous:" + (AnonymousFnCounter++).ToString();
			ILithpOpChainMember def = new LithpFunctionDefinition(chain, name, defBody, defParams.ToArray());
			Console.WriteLine("FnDef created: {0}", def);
			return def;
		}

		/*
		public ILithpOpChainMember Convert(JValue value, LithpOpChain chain)
		{
			Classification c = LithpParser.Classify(value.ToString());
			Console.WriteLine("Classified: '{0}' as: {1}", value, c.ToString());
			string v = value.ToString();
			if (c.HasFlag(Classification.STRING_DOUBLE))
				return new LithpLiteral(v.Substring(1, v.Length - 2));
			else if (c.HasFlag(Classification.STRING_SINGLE))
				return LithpAtom.Atom(v.Substring(1, v.Length - 2));
			else if (c.HasFlag(Classification.ATOM))
				return LithpAtom.Atom(value.ToString());
			else if (c.HasFlag(Classification.NUMBER))
				return new LithpInteger(value.ToString());
			else if (c.HasFlag(Classification.VARIABLE))
				return new LithpVariableReference(v);
			else
			{
				Console.WriteLine("Type not handled: {0}", c);
				throw new NotImplementedException();
			}
		}

		public ILithpOpChainMember Convert(JArray list, LithpOpChain chain)
		{
			LithpOpChain newChain = new LithpOpChain(chain);
			foreach(dynamic element in list)
			{
				newChain.Add(Convert(element, newChain));
			}
			return newChain;
		}

		protected int AnonymousFnCounter = 0;

		public LithpOpChainMember Convert(JObject value, LithpOpChain chain)
		{
			//Console.WriteLine("FnDef: {0}", value);
			JArray fncode = (JArray)value["code"];
			JArray fnparams = (JArray)value["_fnparams"];
			List<string> defParams = new List<string>();
			foreach (JValue p in fnparams)
			{
				defParams.Add(p.ToString());
				Console.WriteLine("Param: {0}", p.ToString());
			}
			//Console.WriteLine("FunctionDef with params {0}, code: {1}", defParams.ToArray(), fncode);
			LithpOpChain defBody = (LithpOpChain)Convert(fncode, chain);
			string name = "anonymous:" + (AnonymousFnCounter++).ToString();
			var x = new LithpFunctionDefinition(chain, name, defBody, defParams.ToArray());
			Console.WriteLine("FnDef created: {0}", x);
			return x;
		} */
	}
}
