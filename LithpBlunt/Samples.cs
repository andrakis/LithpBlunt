using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LithpBlunt.OpChainMembers;

namespace LithpBlunt
{
	public static class Samples
	{
		public static LithpOpChain Factorial ()
		{
			// (
			LithpOpChain result = new LithpOpChain();
			// (if (== 1 N) (
			//   (1)
			// ) (else (
			//   (* N (fac (- N 1)))
			// )))
			LithpOpChain facBody = new LithpOpChain(result);
			LithpOpChain return1 = new LithpOpChain(facBody);
			return1.Add(new LithpLiteral(1));
			LithpOpChain elseBody = new LithpOpChain(facBody);
			elseBody.Add(
				LithpFunctionCall.New("*/*",
					LithpFunctionCall.New("get/1", new LithpVariableReference("N")),
					LithpFunctionCall.New("fac/1",
						LithpFunctionCall.New("-/*",
							LithpFunctionCall.New("get/1", new LithpVariableReference("N")),
							new LithpLiteral(1)
						)
					)
				)
			);
			facBody.Add(
				LithpFunctionCall.New("if/3",
					LithpFunctionCall.New("==/2",
						new LithpLiteral(1),
						LithpFunctionCall.New("get/1", new LithpVariableReference("N"))
					),
					return1,
					LithpFunctionCall.New("else/1", elseBody)
				)
			);
			// (def fac #N :: (
			LithpFunctionCall def = LithpFunctionCall.New("def/2", LithpAtom.Atom("fac"),
				LithpFunctionDefinition.New(result, "fac", facBody, "N")
			);
			// )
			result.Add(def);
			int n;
#if DEBUG
			n = 5;
#else
			n = 200;
#endif
			result.Add(LithpFunctionCall.New("print/*", "Factorial result: ", LithpFunctionCall.New("fac/1", n)));
			return result;
		}
	}
}
