using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LithpBlunt.OpChainMembers;

namespace LithpBlunt
{
	class Program
	{
		static void Main(string[] args)
		{
			LithpOpChain chain = new LithpOpChain();
			LithpInteger one = 1;
			LithpAtom test = "test";
			LithpDict dict = new LithpDict();
			dict["foo"] = "bar";
			dict["num"] = 1;
			dict["list"] = LithpList.New("Hello, world!", one, test);
			Console.WriteLine(dict.ToLiteral());
			LithpFunctionCall fncall = new LithpFunctionCall("print/*", LithpList.New(dict));
			Console.WriteLine("fncall tostring: {0}", fncall);

			LithpBuiltins builtins = new LithpBuiltins();
			LithpInterpreter interp = new LithpInterpreter();
			builtins[fncall.Function].fn_body(fncall.Parameters, chain, interp);

			// Now put it all together
			chain.Add(fncall);
			chain.ImportBuiltins(builtins);
			Console.WriteLine("Result of print: {0}", interp.Run(chain));

			// More complex
			LithpFunctionCall addStrings = new LithpFunctionCall("+/*",
				LithpList.New("foo", "bar"));
			LithpFunctionCall printAddString = new LithpFunctionCall("print/*",
				LithpList.New("Adding two strings: ", addStrings));
			chain.Add(printAddString);
			Console.WriteLine("Result of add strings: {0}", interp.Run(chain));

			Console.WriteLine("Tests finished, hit enter");
			Console.ReadLine();
		}
	}
}
