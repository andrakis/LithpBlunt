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
		static void RunTests() {
			LithpOpChain chain = new LithpOpChain();
			LithpInteger one = 1;
			LithpAtom test = "test";
			LithpDict dict = new LithpDict();
			dict["foo"] = "bar";
			dict["num"] = 1;
			dict["list"] = LithpList.New("Hello, world!", one, test);
			Console.WriteLine(dict.ToLiteral());
			LithpFunctionCall fncall = LithpFunctionCall.New("print/*", dict);
			Console.WriteLine("fncall tostring: {0}", fncall);

			LithpBuiltins builtins = new LithpBuiltins();
			LithpInterpreter interp = new LithpInterpreter();
			builtins[fncall.Function].Invoke(fncall.Parameters, chain, interp);

			// Now put it all together
			chain.Add(fncall);
			chain.ImportBuiltins(builtins);
			Console.WriteLine("Result of print: {0}", interp.Run(chain));

			// More complex
			LithpFunctionCall addStrings = LithpFunctionCall.New("+/*", "foo", "bar");
			LithpFunctionCall printAddString = LithpFunctionCall.New("print/*",
				"Adding two strings: ", addStrings);
			chain.Add(printAddString);
			interp.Run(chain);
			chain.Add(addStrings);
			Console.WriteLine("Result of add strings: {0}", interp.Run(chain));

			LithpFunctionCall setVar = LithpFunctionCall.New("set/2",
				new LithpVariableReference("Test"), "Foo");
			LithpFunctionCall printVar = LithpFunctionCall.New("print/*",
				"Value of Test:", LithpFunctionCall.New(
					"get/1", new LithpVariableReference("Test")
			));
			chain.Add(setVar);
			chain.Add(printVar);

			// Now run entire chain from the start
			chain.Rewind();
			interp.Run(chain);

			// Try a user-defined function
			LithpOpChain addBody = new LithpOpChain(chain);
			addBody.Add(LithpFunctionCall.New(
				LithpAtom.Atom("+/*"),
				LithpFunctionCall.New("get/1", new LithpVariableReference("A")),
				LithpFunctionCall.New("get/1", new LithpVariableReference("B"))
			));
			chain.Add(LithpFunctionCall.New("def/2",
					LithpAtom.Atom("add"),
					LithpFunctionDefinition.New(chain, "add", addBody, "A", "B")
			));
			chain.Add(LithpFunctionCall.New("print/*",
				"Calling user function add:", LithpFunctionCall.New(
						"add/2", 2, 5
				)
			));
			interp.Run(chain);
		}

		public static void DoSamples()
		{
			LithpInterpreter interp = new LithpInterpreter();
			LithpBuiltins builtins = new LithpBuiltins();
			Console.WriteLine("Attempting factorial sample");
			LithpOpChain fac = Samples.Factorial();
			fac.ImportBuiltins(builtins);
			interp.Run(fac);
		}

		static void Main(string[] args)
		{
			// Initialize LithpBuiltins now
			LithpBuiltins builtins = new LithpBuiltins();

			var watch = System.Diagnostics.Stopwatch.StartNew();
			//RunTests();
			DoSamples();
			watch.Stop();
			
			Console.WriteLine("Tests finished in {0}ms, hit enter", watch.ElapsedMilliseconds);
			Console.ReadLine();
		}
	}
}
