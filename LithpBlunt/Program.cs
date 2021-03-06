﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LithpBlunt.OpChainMembers;
using LithpBlunt.Serialization;

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

		static LithpInterpreter Interp = new LithpInterpreter();
		static LithpBuiltins Builtins = new LithpBuiltins();
		static LithpOpChain LoadSample(string file)
		{
			var compileWatch = Stopwatch.StartNew();
			LithpOpChain chain = LithpJsonParser.LoadSample(file);
			chain.ImportBuiltins(Builtins);
			compileWatch.Stop();
			Console.WriteLine("Compile for {1} finished in {0}ms",
				compileWatch.ElapsedMilliseconds, file);
			return chain;
		}

		static void Main(string[] args)
		{
			// Initialize LithpBuiltins now
			LithpBuiltins builtins = new LithpBuiltins();
			LithpInterpreter interp = new LithpInterpreter();
			// Initialize parser
			LithpJsonParser.Test();

			var watch = Stopwatch.StartNew();
			if(false) RunTests();
			var sampleFactorial = LoadSample("factorial.ast");
			var sampleScope = LoadSample("scope.ast");
			var sampleComplex = LoadSample("complex.ast");
			var sampleIntegral2 = LoadSample("integral2.ast");
			var sampleWhile = LoadSample("while.ast");
			var runWatch = Stopwatch.StartNew();
			interp.Run(sampleFactorial);
			interp.Run(sampleScope);
			interp.Run(sampleComplex);
			interp.Run(sampleIntegral2);
			interp.Run(sampleWhile);
			interp.Run(LoadSample("var_args.ast"));
			interp.Run(LoadSample("recurse.ast"));
			runWatch.Stop();
			Console.WriteLine("Run finished: {0} function calls in {1}ms", LithpInterpreter.FunctionCalls, runWatch.ElapsedMilliseconds);
			watch.Stop();

			Console.WriteLine("Tests finished in {0}ms, hit enter", watch.ElapsedMilliseconds);
#if DEBUG
			Console.ReadLine();
#endif
		}
	}
}
