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
				chain.Add(Convert(element, chain));
			}
			return chain;
		}

		public ILithpOpChainMember Convert(JValue value, LithpOpChain chain)
		{
			Console.WriteLine("Value: {0}", value);
			return new LithpLiteral(value.ToString());
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
		}
	}
}
