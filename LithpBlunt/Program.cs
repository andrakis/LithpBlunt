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
			LithpFunctionCall fncall = new LithpFunctionCall("print/*", 
				new LithpList("Hello, world!"));
			Console.WriteLine("fncall tostring: {0}", fncall);

			LithpBuiltins builtins = new LithpBuiltins();
			LithpInterpreter interp = new LithpInterpreter();
			builtins.builtins["print/*"](
				new LithpList(
					LithpAtom.Atom("atom"),
					"Hello, again!"
				), chain, interp);

			Console.WriteLine("Tests finished, hit enter");
			Console.ReadLine();
		}
	}
}
