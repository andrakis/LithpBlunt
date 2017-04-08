using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt
{
	public delegate LithpPrimitive LithpFunctionDefinitionDelegate
		(LithpList parameters, LithpOpChain state, LithpInterpreter interp);

	public interface ILithpFunctionDefinition : ILithpPrimitive
	{
		LithpPrimitive Invoke(LithpList parameters, LithpOpChain state, LithpInterpreter interp);
	}

	public class LithpFunctionDefinitionNative : LithpPrimitive, ILithpFunctionDefinition
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

		protected override string toString()
		{
			throw new NotImplementedException();
		}

		protected override int hashCode()
		{
			return fn_name.GetHashCode() * 34 +
				fn_params.GetHashCode() * 17 +
				fn_body.GetHashCode();
		}

		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.FN_NATIVE;
		}
	}

	public class LithpFunctionDefinitionLithp
	{

	}
}
