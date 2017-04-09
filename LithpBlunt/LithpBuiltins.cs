using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt
{
	public class LithpBuiltins : IDictionary<LithpAtom, LithpFunctionDefinitionNative>
	{
		protected static Dictionary<LithpAtom, LithpFunctionDefinitionNative> builtins =
			new Dictionary<LithpAtom, LithpFunctionDefinitionNative>();

		static LithpBuiltins()
		{
			//builtins["print/*"] = new LithpFunctionDefinitionNative("print/*", new string[] { }, Print);
			builtins["print/*"] = builtin("print/*", Print);
			builtins["+/*"] = builtin("+/*", Add);
			builtins["-/*"] = builtin("-/*", Sub);
			builtins["*/*"] = builtin("*/*", Multiply);
			builtins["//*"] = builtin("//*", Divide);
			builtins["set/2"] = builtin("set/2", Set, "Name", "Value");
			builtins["var/2"] = builtin("var/2", Var, "Name", "Value");
			builtins["get/1"] = builtin("get/1", Get, "Name");
		}

		protected static LithpFunctionDefinitionNative builtin(string name, LithpFunctionDefinitionDelegate fn, params string[] args)
		{
			return new LithpFunctionDefinitionNative(name, args, fn);
		}

		public static LithpPrimitive Print(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpPrimitive result = ApplyAction((A, B, X, Y) =>
			{
				return A.ToString() + " " + B.ToString();
			}, parameters, state, interp);
			Console.WriteLine(result.ToString());
			return LithpAtom.Atom("nil");
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

		protected static LithpPrimitive ApplyAction(LithpAction action, LithpList list,
			LithpOpChain state, LithpInterpreter interp)
		{
			LithpPrimitive value = CallBuiltin(Head, state, interp, list);
			LithpList tail = CallBuiltin(Tail, state, interp, list) as LithpList;
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
			LithpList value = parameters[0] as LithpList;
			return value[0];
		}

		public static LithpPrimitive Tail(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpList value = parameters[0] as LithpList;
			return new LithpList(value.Skip(1).ToArray());
		}

		public static LithpPrimitive Var(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpPrimitive name = CallBuiltin(Head, state, interp, parameters);
			LithpList tail = CallBuiltin(Tail, state, interp, parameters) as LithpList;
			LithpPrimitive value = CallBuiltin(Head, state, interp, tail);
			state.Closure.SetImmediate(name, value);
			return value;
		}

		public static LithpPrimitive Set(LithpPrimitive parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpPrimitive name = CallBuiltin(Head, state, interp, parameters);
			LithpList tail = CallBuiltin(Tail, state, interp, parameters) as LithpList;
			LithpPrimitive value = CallBuiltin(Head, state, interp, tail);
			state.Closure.Set(name, value);
			return value;
		}

		public static LithpPrimitive Get(LithpPrimitive parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpPrimitive name = CallBuiltin(Head, state, interp, parameters);
			return state.Closure.Get(name);
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
