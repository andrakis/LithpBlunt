using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt
{
	public delegate LithpPrimitive LithpFunctionDefinitionDelegate
		(LithpList parameters, LithpOpChain state, LithpInterpreter interp);

	public interface ILithpFunctionDefinition
	{
		LithpPrimitive Invoke(LithpList parameters, LithpOpChain state, LithpInterpreter interp);
	}

	public class LithpFunctionDefinitionNative : ILithpFunctionDefinition
	{
		public readonly LithpAtom fn_name;
		public readonly string[] fn_params;
		public readonly LithpFunctionDefinitionDelegate fn_body;

		public LithpFunctionDefinitionNative(LithpAtom name, string[] parameters,
			LithpFunctionDefinitionDelegate body)
		{
			fn_name = name;
			fn_params = parameters;
			fn_body = body;
		}

		public LithpPrimitive Invoke(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			return fn_body(parameters, state, interp);
		}
	}

	public class LithpFunctionDefinitionLithp
	{

	}
}
