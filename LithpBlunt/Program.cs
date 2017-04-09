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
		static void RunTests () {
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
			builtins[fncall.Function].Invoke(fncall.Parameters, chain, interp);

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
			interp.Run(chain);
			chain.Add(addStrings);
			Console.WriteLine("Result of add strings: {0}", interp.Run(chain));

			LithpFunctionCall setVar = new LithpFunctionCall("set/2",
				LithpList.New(new LithpVariableReference("Test"), "Foo"));
			LithpFunctionCall printVar = new LithpFunctionCall("print/*",
				LithpList.New("Value of Test:", new LithpFunctionCall("get/1",
				LithpList.New(new LithpVariableReference("Test")))));
			chain.Add(setVar);
			chain.Add(printVar);

			// Now run entire chain from the start
			chain.Rewind();
			interp.Run(chain);

			// Try a user-defined function
			LithpOpChain addBody = new LithpOpChain(chain);
			addBody.Add(new LithpFunctionCall(LithpAtom.Atom("+/*"),
				LithpList.New(
					new LithpFunctionCall("get/1", LithpList.New(new LithpVariableReference("A"))),
					new LithpFunctionCall("get/1", LithpList.New(new LithpVariableReference("B")))
				)));
			chain.Add(new LithpFunctionCall("def/2",
				LithpList.New(
					LithpAtom.Atom("add"),
					LithpFunctionDefinition.New(chain, "add", addBody, "A", "B")
				)
			));
			chain.Add(new LithpFunctionCall("print/*", LithpList.New(
				"Calling user function add: ", new LithpFunctionCall(
						"add/2",
						LithpList.New(2, 5)
				)
			)));
			interp.Run(chain);
		}

		static void Main(string[] args)
		{
			// Initialize LithpBuiltins now
			LithpBuiltins builtins = new LithpBuiltins();

			var watch = System.Diagnostics.Stopwatch.StartNew();
			RunTests();
			watch.Stop();
			
			Console.WriteLine("Tests finished in {0}ms, hit enter", watch.ElapsedMilliseconds);
			Console.ReadLine();
		}
	}
}
