using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LithpBlunt.ExtensionMethods;

namespace LithpBlunt.OpChainMembers
{
	/// <summary>
	/// A function definition for Lithp functions, ie user-defined functions.
	/// </summary>
	public class LithpFunctionDefinition : LithpOpChainMember, ILithpFunctionDefinition
	{
		protected readonly int? arity;
		public int? Arity {
			get { return arity; }
		}

		protected readonly LithpOpChain body;
		protected string[] parameters;

		protected string name;

		public string Name {
			get { return name; }
		}

		public LithpFunctionDefinition(LithpOpChain parent, string name,
			LithpOpChain body, string[] parameters)
		{
			this.name = name;
			this.body = new LithpOpChain(parent, body);
			this.parameters = parameters;
			Match x = Regex.Match(name, @"/(\d+|\*)$");
			if (x == Match.Empty)
			{
				arity = parameters.Count();
				this.name += "/" + arity.Value.ToString();
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

		public static LithpFunctionDefinition New(LithpOpChain parent, string name,
			LithpOpChain body, params string[] parameters)
		{
			return new LithpFunctionDefinition(parent, name, body, parameters);
		}

		public string[] Parameters {
			get { return parameters; }
		}

		public void SetName (string n)
		{
			name = n;
		}

		public LithpPrimitive Invoke(LithpList parameters, LithpOpChain state, LithpInterpreter interp)
		{
			LithpOpChain parent;
			if (body.Scoped)
				parent = body;
			else
				parent = state;
			LithpOpChain chain = new LithpOpChain(parent, body);
			// Arity star functions pass all arguments in first parameter
			if (arity.HasValue == false)
				parameters = LithpList.New(parameters);
			parameters.Each((Value, Index) =>
			{
				chain.Closure.SetImmediate(this.parameters[Index], Value);
			});
			chain.FunctionEntry = name;
			return interp.Run(chain);
		}

		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.FN;
		}

		protected override int hashCode()
		{
			return body.GetHashCode() * 17 + parameters.GetHashCode();
		}

		protected override string toString()
		{
			return "stub:LithpFunctionDefinition::toString()";
		}
	}
}
