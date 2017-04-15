using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LithpBlunt.OpChainMembers;
using System.Numerics;

namespace LithpBlunt
{
	public class LithpBuiltins : IDictionary<LithpAtom, LithpFunctionDefinitionNative>
	{
		protected static Dictionary<LithpAtom, LithpFunctionDefinitionNative> builtins =
			new Dictionary<LithpAtom, LithpFunctionDefinitionNative>();

		static LithpBuiltins()
		{
			builtin("print/*", Print);
			builtin("+/*", Add);
			builtin("-/*", Sub);
			builtin("*/*", Multiply);
			builtin("//*", Divide);
			builtin("==", CompareEqual, "A", "B");
			builtin("!=", CompareNotEqual, "A", "B");
			builtin("set", Set, "Name", "Value");
			builtin("var", Var, "Name", "Value");
			builtin("get", Get, "Name");
			builtin("def", Def, "Name", "Body");
			builtin("scope", Scope, "OpChain");
			builtin("if", If2, "Test", "Action");
			builtin("if", If3, "Test", "Action", "Else");
			builtin("?", Unary, "Test", "Action", "Else");
			builtin("else", Else, "OpChain");
			builtin("call/*", Call);
			builtin("assert", Assert, "Value");
		}

		protected static LithpFunctionDefinitionNative builtin(string name, LithpFunctionDefinitionDelegate fn, params string[] args)
		{
			LithpFunctionDefinitionNative x = new LithpFunctionDefinitionNative(name, args, fn);
			builtins[x.Name] = x;
			return x;
		}

		public static LithpPrimitive Print(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpPrimitive result = ApplyAction((A, B, X, Y) =>
			{
				return A.ToString() + " " + B.ToString();
			}, parameters, state, interp);
			Console.WriteLine(result.ToString());
			return LithpAtom.Nil;
		}

		protected delegate LithpPrimitive LithpAction(LithpPrimitive A,
			LithpPrimitive B, LithpOpChain state, LithpInterpreter interp);

		public static LithpPrimitive Add(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			return ApplyAction((A, B, X, Y) => A + B, Values, state, interp);
		}

		public static LithpPrimitive Sub(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			return ApplyAction((A, B, X, Y) => A - B, Values, state, interp);
		}

		public static LithpPrimitive Multiply(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			return ApplyAction((A, B, X, Y) => A * B, Values, state, interp);
		}

		public static LithpPrimitive Divide(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			return ApplyAction((A, B, X, Y) => A / B, Values, state, interp);
		}

		public static LithpPrimitive CompareEqual(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			return Values[0] == Values[1] ? LithpAtom.True : LithpAtom.False;
		}

		public static LithpPrimitive CompareNotEqual(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			return Values[0] != Values[1] ? LithpAtom.True : LithpAtom.False;
		}

		public static LithpPrimitive Unary(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			return Values[0] == LithpAtom.True ? Values[1] : Values[2];
		}

		protected static LithpPrimitive getIfResult(LithpPrimitive value)
		{
			if (value.LithpType() == LithpType.OPCHAIN)
				return ((LithpOpChain)value).CallImmediate();
			return value;
		}

		public static LithpPrimitive If2(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			if (Values[0] == LithpAtom.True)
				return getIfResult(Values[1]);
			return LithpAtom.Nil;
		}

		public static LithpPrimitive If3(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			return getIfResult((Values[0] == LithpAtom.True) ? Values[1] : Values[2]);
		}

		public static LithpPrimitive Else(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			return Values[0];
		}

		public static LithpPrimitive Recurse(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			var Target = state.Parent;
			while(Target && Target.FunctionEntry == null)
			{
				Target = Target.Parent;
			}
			if(!Target)
			{
				throw new Exception("Function entry not found");
			}
			
			Target.Rewind();

			throw new NotImplementedException();
			return LithpAtom.Nil;
		}

		protected static LithpPrimitive ApplyAction(LithpAction action, LithpList list,
			LithpOpChain state, LithpInterpreter interp)
		{
			LithpPrimitive value = CallBuiltin(Head, state, interp, list);
			LithpList tail = (LithpList)CallBuiltin(Tail, state, interp, list);
			foreach(var x in tail)
			{
				value = action(value, x, state, interp);
			}
			return value;
		}

		protected static LithpPrimitive CallBuiltin(LithpFunctionDefinitionDelegate fn,
			LithpOpChain state, LithpInterpreter interp, params LithpPrimitive[] parameters)
		{
			return fn(new LithpList(parameters), state, interp);
		}

		public static LithpPrimitive Head(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpList value = (LithpList)parameters[0];
			return value[0];
		}

		public static LithpPrimitive Tail(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpList value = (LithpList)parameters[0];
			return new LithpList(value.Skip(1).ToArray());
		}

		public static LithpPrimitive Var(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpPrimitive name = CallBuiltin(Head, state, interp, parameters);
			LithpList tail = (LithpList)CallBuiltin(Tail, state, interp, parameters);
			LithpPrimitive value = CallBuiltin(Head, state, interp, tail);
			state.Closure.SetImmediate(name, value);
			return value;
		}

		public static LithpPrimitive Set(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpPrimitive name = CallBuiltin(Head, state, interp, parameters);
			LithpList tail = (LithpList)CallBuiltin(Tail, state, interp, parameters);
			LithpPrimitive value = CallBuiltin(Head, state, interp, tail);
			state.Closure.Set(name, value);
			return value;
		}

		public static LithpPrimitive Def(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			ILithpPrimitive name = parameters[0];
			if (name.LithpType() != LithpType.ATOM)
				throw new ArgumentException("Function name must be an atom");
			if (parameters[1].LithpType() != LithpType.FN)
				throw new ArgumentException("Function body must be a FunctionDefinition");
			LithpFunctionDefinition body = (LithpFunctionDefinition)parameters[1];
			body.SetName(parameters[0]);
			state.Closure.SetImmediate(body.Name, body);
			return body;
		}

		public static LithpPrimitive Get(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpPrimitive name = CallBuiltin(Head, state, interp, parameters);
			return state.Closure.Get(name);
		}

		public static LithpPrimitive Scope(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpFunctionDefinition def = (LithpFunctionDefinition)parameters[0];
			return def.CloneWithScope(state);
		}

		public static LithpPrimitive ParseInt (LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			return new LithpInteger(parameters[0]);
		}

		public static LithpPrimitive Call(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpPrimitive def = parameters[0];
			LithpList defParams = (LithpList)Tail(LithpList.New(parameters), state, interp);
			switch(def.LithpType())
			{
				case LithpType.FN_NATIVE:
					return ((LithpFunctionDefinitionNative)def).Invoke(defParams, state, interp);
				case LithpType.FN:
					return ((LithpFunctionDefinition)def).Invoke(defParams, state, interp);
				case LithpType.ATOM:
				case LithpType.STRING:
					string strName = def.ToString();
					ILithpFunctionDefinition search;
					if((object)state.Closure.TopMost != null && state.Closure.TopMost.IsDefined(strName))
					{
						search = (ILithpFunctionDefinition)state.Closure.TopMost[strName];
					}
					else if(state.Closure.IsDefined(strName))
					{
						search = (ILithpFunctionDefinition)state.Closure[strName];
					}
					else
					{
						string arityStar = strName + "/*";
						if ((object)state.Closure.TopMost != null && state.Closure.TopMost.IsDefined(strName))
						{
							search = (ILithpFunctionDefinition)state.Closure.TopMost[strName];
						}
						else if (state.Closure.IsDefined(strName))
						{
							search = (ILithpFunctionDefinition)state.Closure[strName];
						}
						else
							throw new MissingMethodException();
					}
					return search.Invoke(defParams, state, interp);
				default:
					throw new NotImplementedException();
			}
		}

		public static LithpPrimitive Assert(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			if (parameters[0] == LithpAtom.False)
				throw new Exception("Assert failed");
			return LithpAtom.Nil;
		}

		public LithpFunctionDefinitionNative this[LithpAtom key] {
			get {
				return ((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins)[key];
			}

			set {
				((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins)[key] = value;
			}
		}

		public int Count {
			get {
				return ((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins).Count;
			}
		}

		public bool IsReadOnly {
			get {
				return ((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins).IsReadOnly;
			}
		}

		public ICollection<LithpAtom> Keys {
			get {
				return ((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins).Keys;
			}
		}

		public ICollection<LithpFunctionDefinitionNative> Values {
			get {
				return ((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins).Values;
			}
		}

		public void Add(KeyValuePair<LithpAtom, LithpFunctionDefinitionNative> item)
		{
			((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins).Add(item);
		}

		public void Add(LithpAtom key, LithpFunctionDefinitionNative value)
		{
			((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins).Add(key, value);
		}

		public void Clear()
		{
			((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins).Clear();
		}

		public bool Contains(KeyValuePair<LithpAtom, LithpFunctionDefinitionNative> item)
		{
			return ((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins).Contains(item);
		}

		public bool ContainsKey(LithpAtom key)
		{
			return ((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins).ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<LithpAtom, LithpFunctionDefinitionNative>[] array, int arrayIndex)
		{
			((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins).CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<LithpAtom, LithpFunctionDefinitionNative>> GetEnumerator()
		{
			return ((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins).GetEnumerator();
		}

		public bool Remove(KeyValuePair<LithpAtom, LithpFunctionDefinitionNative> item)
		{
			return ((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins).Remove(item);
		}

		public bool Remove(LithpAtom key)
		{
			return ((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins).Remove(key);
		}

		public bool TryGetValue(LithpAtom key, out LithpFunctionDefinitionNative value)
		{
			return ((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins).TryGetValue(key, out value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IDictionary<LithpAtom, LithpFunctionDefinitionNative>)builtins).GetEnumerator();
		}
	}
}
