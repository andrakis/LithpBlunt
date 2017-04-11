using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LithpBlunt.OpChainMembers
{
	/// <summary>
	/// Delegate for Lithp functions.
	/// </summary>
	/// <param name="parameters">Parameters given as a list</param>
	/// <param name="state">The current LithpOpChain</param>
	/// <param name="interp">The current LithpInterpreter</param>
	/// <returns></returns>
	public delegate LithpPrimitive LithpFunctionDefinitionDelegate
		(LithpList parameters, LithpOpChain state, LithpInterpreter interp);

	/// <summary>
	/// Interface to native and user-defined function definitions.
	/// </summary>
	public interface ILithpFunctionDefinition : ILithpPrimitive
	{
		/// <summary>
		/// Get the arity of the function definition.
		/// If no value is present, it's an arity * function.
		/// </summary>
		int? Arity { get; }
		/// <summary>
		/// Get the names of all parameters.
		/// </summary>
		string[] Parameters { get; }
		/// <summary>
		/// Get the readable function name.
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Invoke the definition.
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="state"></param>
		/// <param name="interp"></param>
		/// <returns></returns>
		LithpPrimitive Invoke(LithpList parameters, LithpOpChain state, LithpInterpreter interp);
		/// <summary>
		/// Get the inner type
		/// </summary>
		/// <returns></returns>
		LithpType LithpType();
	}

	/// <summary>
	/// A function definition for functions written in the native parent language.
	/// </summary>
	public class LithpFunctionDefinitionNative : LithpPrimitive, ILithpFunctionDefinition
	{
		protected readonly LithpAtom fn_name;
		protected readonly string[] fn_params;
		protected readonly LithpFunctionDefinitionDelegate fn_body;

		protected readonly int? arity;

		public int? Arity {
			get { return arity; }
		}

		public string[] Parameters {
			get { return fn_params; }
		}

		public LithpFunctionDefinitionNative(LithpAtom name, string[] parameters,
			LithpFunctionDefinitionDelegate body)
		{
			fn_name = name;
			fn_params = parameters;
			fn_body = body;
			Match x = Regex.Match(name, @"/(\d+|\*)$");
			if (x == Match.Empty)
			{
				arity = fn_params.Count();
				this.fn_name = LithpAtom.Atom(name.Name + "/" + arity.Value.ToString());
			}
			else
			{
				string strArity = x.Groups[1].Value;
				if (strArity == "*")
				{
					arity = null;
				}
				else
				{
					arity = int.Parse(strArity);
				}
			}
		}

		public string Name {
			get { return fn_name.ToString(); }
		}

		public LithpPrimitive Invoke(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			return fn_body(parameters, state, interp);
		}

		protected override string toString()
		{
			return "stub:LithpFunctionDefinitionNative::toString()";
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
}
