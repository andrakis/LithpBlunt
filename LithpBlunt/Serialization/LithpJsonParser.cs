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
using System.Diagnostics;

namespace LithpBlunt.Serialization
{
	public class LithpJsonParser
	{
		public static bool EnableVerbosity = false;

		public static LithpOpChain Test()
		{
			string complex = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "samples", "factorial.ast"));
			dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(complex);
			LithpJsonParser parser = new LithpJsonParser(jsonData);
			return parser.Finalize();
		}

		protected void parserDebug(string format, params object[] parameters)
		{
#if DEBUG
			if (EnableVerbosity)
			{
				Console.WriteLine(format, parameters);
			}
#endif
		}

		protected JArray ops = new JArray();
		public LithpJsonParser(dynamic jsonData)
		{
			ops.Add(unexport(jsonData));
		}

		protected dynamic unexport(dynamic ast)
		{
			if (ast.GetType() == typeof(JObject))
			{
				JArray code = unexport(ast["code"]);
				JObject obj = new JObject();
				obj["code"] = code;
				obj["_fndef"] = ast["_fndef"];
				obj["_fnparams"] = ast["_fnparams"];
				return obj;
			} else if(ast.GetType() == typeof(JArray))
			{
				JArray list = (JArray)ast;
				JArray res = new JArray();
				foreach (var x in list)
					res.Add(unexport(x));
				return res;
			} else
			{
				return ast;
			}
		}

		public LithpOpChain Finalize ()
		{
			LithpOpChain chain = new LithpOpChain();
			foreach(dynamic x in ops)
			{
				ILithpOpChainMember c = convert(chain, x);
				chain.Add(c);
			}
			return chain;
		}

		protected Classification classify(dynamic curr)
		{
			if(curr.GetType() == typeof(JValue))
				return LithpParser.Classify(curr.ToString());
			return Classification.COMPILED;
		}

		protected ILithpOpChainMember mapParam (dynamic P, LithpOpChain chain, string fnName)
		{
			var result = mapParamInner(P, chain, fnName);
			parserDebug("mapParam({0}) :: {1}", P, result);
			return result;
		}

		protected ILithpOpChainMember mapParamInner(JArray arr, LithpOpChain chain, string fnName)
		{
			return convert(chain, arr);
		}

		protected ILithpOpChainMember mapParamInner(JValue v, LithpOpChain chain, string fnName)
		{
			Classification cls = classify(v);
			string strV = v.ToString();
			parserDebug("Classified: {0}", cls.ToString());
			if (cls.HasFlag(Classification.STRING_DOUBLE) || cls.HasFlag(Classification.STRING_SINGLE))
			{
				strV = strV.Substring(1, strV.Length - 2);
				string parsed = LithpParser.ParseEscapes(strV);
				if (cls.HasFlag(Classification.STRING_DOUBLE))
					return new LithpLiteral(new LithpString(parsed));
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
						return new LithpVariableReference(strV);
					default:
						return LithpFunctionCall.New("get/1", new LithpVariableReference(strV));
				}
			}
			else if (cls.HasFlag(Classification.NUMBER))
			{
				return new LithpLiteral(new LithpInteger(strV));
			}
			else if (cls.HasFlag(Classification.ATOM))
				return new LithpLiteral(LithpAtom.Atom(strV));
			throw new NotImplementedException();
		}

		protected ILithpOpChainMember mapParamInner(JObject o, LithpOpChain chain, string fnName)
		{
			return convert(chain, o);
		}

		protected ILithpOpChainMember convert(LithpOpChain chain, JArray curr)
		{
			var eleFirst = curr[0];
			Classification clsFirst = classify(eleFirst);
			if (curr.Count == 0)
				throw new NotImplementedException();
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
				clsFirst = classify(eleFirst);
				parserDebug("  First element: {0}", eleFirst);
				parserDebug("  Re-Classified: {0}", clsFirst.ToString());
			}
			if (clsFirst.HasFlag(Classification.ATOM))
			{
				// Function call
				parserDebug(" PARSE TO FUNCTIONCALL: {0}", curr);
				var skipped = curr.Skip(1);
				LithpList parameters = new LithpList();
				foreach (var x in skipped)
				{
					var y = mapParam(x, chain, eleFirst.ToString());
					var z = y as LithpPrimitive;
					if ((object)z == null)
						Debug.WriteLine("Failed to convert");
					parameters.Add(z);
				}
				if (parameters.Count == 0 && clsFirst.HasFlag(Classification.NUMBER))
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
					newChain.Add(mapParam(curr[i], chain, eleFirst.ToString()));
				}
				return newChain;
			}
			throw new NotImplementedException();
		}

		public static int AnonymousFnCounter = 0;
		protected ILithpOpChainMember convert(LithpOpChain chain, JObject curr)
		{
			JArray fncode = (JArray)curr["code"];
			JArray fnparams = (JArray)curr["_fnparams"];
			List<string> defParams = new List<string>();
			foreach (JValue p in fnparams)
			{
				defParams.Add(p.ToString());
				parserDebug("Param: {0}", p.ToString());
			}
			parserDebug("FunctionDef with params {0}, code: {1}", defParams.ToArray(), fncode);
			LithpOpChain defBody = (LithpOpChain)convert(chain, fncode);
			string name = "anonymous:" + (AnonymousFnCounter++).ToString();
			ILithpOpChainMember def = new LithpFunctionDefinition(chain, name, defBody, defParams.ToArray());
			parserDebug("FnDef created: {0}", def);
			return def;
		}
	}
}
