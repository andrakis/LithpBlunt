using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LithpBlunt.OpChainMembers;
using System.Numerics;
using System.Text.RegularExpressions;

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
			// LithpList operator + implements ++
			builtin("++/*", Add);
			builtin("-/*", Sub);
			builtin("*/*", Multiply);
			builtin("//*", Divide);
			builtin("==", CompareEqual, "A", "B");
			builtin("!=", CompareNotEqual, "A", "B");
			builtin("<", CompareLess, "A", "B");
			builtin(">", CompareMore, "A", "B");
			builtin("<=", CompareLessEqual, "A", "B");
			builtin(">=", CompareMoreEqual, "A", "B");
			builtin("!", OperatorNot, "N");
			builtin("and/*", CompareAnd);
			builtin("or/*", CompareOr);
			builtin("&", BitwiseAnd, "A", "B");
			builtin("|", BitwiseOr, "A", "B");
			builtin("^", BitwiseXor, "A", "B");
			builtin("@", Modulo, "A", "B");
			builtin("~", BitwiseNot, "N");
			builtin("sqrt", SquareRoot, "N");
			builtin("round", Round, "N");
			builtin("parse-int", ParseInt, "N");
			builtin("atom", Atom, "Name");
			builtin("set", Set, "Name", "Value");
			builtin("var", Var, "Name", "Value");
			builtin("get", Get, "Name");
			builtin("def", Def, "Name", "Body");
			builtin("scope", Scope, "OpChain");
			builtin("if", If2, "Test", "Action");
			builtin("if", If3, "Test", "Action", "Else");
			builtin("?", Unary, "Test", "Action", "Else");
			builtin("else", Else, "OpChain");
			builtin("head", Head, "List");
			builtin("tail", Tail, "List");
			builtin("call/*", Call);
			builtin("recurse/*", Recurse);
			builtin("while", While, "Test", "Action");
			builtin("assert", Assert, "Value");
			builtin("dict/*", Dict);
			builtin("dict-get", DictGet, "Dict", "Key");
			builtin("dict-set", DictSet, "Dict", "Key", "Value");
			builtin("dict-present", DictPresent, "Dict", "Key");
			builtin("dict-remove", DictRemove, "Dict", "Key");
			builtin("dict-keys", DictKeys, "Dict");
			builtin("list/*", List);
			builtin("length", ListLength, "List");
			builtin("repeat", Repeat, "Str", "Count");
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

		public static LithpPrimitive CompareAnd(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			foreach (var x in Values)
				if (Values[0] != LithpAtom.True)
					return LithpAtom.False;
			return LithpAtom.True;
		}

		public static LithpPrimitive CompareOr(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			foreach (var x in Values)
				if (Values[0] == LithpAtom.True)
					return LithpAtom.True;
			return LithpAtom.False;
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

		public static LithpPrimitive CompareLess(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			return Values[0] < Values[1] ? LithpAtom.True : LithpAtom.False;
		}

		public static LithpPrimitive CompareMore(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			return Values[0] > Values[1] ? LithpAtom.True : LithpAtom.False;
		}

		public static LithpPrimitive CompareLessEqual(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			return Values[0] <= Values[1] ? LithpAtom.True : LithpAtom.False;
		}

		public static LithpPrimitive CompareMoreEqual(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			return Values[0] >= Values[1] ? LithpAtom.True : LithpAtom.False;
		}

		public static LithpPrimitive OperatorNot(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			return Values[0] == LithpAtom.True ? LithpAtom.False : LithpAtom.True;
		}

		public static LithpPrimitive Atom(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			return LithpAtom.Atom(Values[0].Cast(LithpType.STRING));
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
			while (Target && Target.FunctionEntry == null)
			{
				Target = Target.Parent;
			}
			if (!Target)
			{
				throw new Exception("Function entry not found");
			}

			// Rewind the target opchain
			Target.Rewind();

			// Get the OpChain function name with arity
			string FnAndArity = Target.FunctionEntry;
			ILithpFunctionDefinition def;
			if (state.Closure.TopMost.IsDefined(FnAndArity))
			{
				def = (ILithpFunctionDefinition)state.Closure.TopMost[FnAndArity];
			} else if(state.Closure.IsDefinedAny(FnAndArity))
			{
				def = (ILithpFunctionDefinition)state.Closure[FnAndArity];
			} else
			{
				FnAndArity = Target.FunctionEntry + "/*";
				if (state.Closure.TopMost.IsDefined(FnAndArity))
				{
					def = (ILithpFunctionDefinition)state.Closure.TopMost[FnAndArity];
				}
				else if (state.Closure.IsDefinedAny(FnAndArity))
				{
					def = (ILithpFunctionDefinition)state.Closure[FnAndArity];
				} else
				{
					throw new MissingMethodException();
				}
			}

			// Set parameters for given function
			int i = 0;
			foreach(string p in def.Parameters)
			{
				Target.Closure.SetImmediate(p, Values[i]);
				i++;
			}

			// Return nothing
			return LithpAtom.Nil;
		}

		public static LithpPrimitive While(LithpList Values, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpOpChain test = (LithpOpChain)Values[0];
			LithpOpChain action = (LithpOpChain)Values[1];
			test.Parent = state;
			test.Closure.TopMost = state.Closure.TopMost;
			test.Closure.Parent = state.Closure;
			action.Parent = state;
			action.Closure.TopMost = state.Closure.TopMost;
			action.Closure.Parent = state.Closure;
			test.Rewind();
			action.Rewind();
			while(interp.Run(test) == LithpAtom.True)
			{
				test.Rewind();
				action.Rewind();
				interp.Run(action);
			}
			return LithpAtom.Nil;
		}

		protected static LithpPrimitive ApplyAction(LithpAction action, LithpList list,
			LithpOpChain state, LithpInterpreter interp)
		{
			LithpPrimitive value = CallBuiltin(Head, state, interp, list);
			LithpList tail = (LithpList)CallBuiltin(Tail, state, interp, list);
			foreach (var x in tail)
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

		public static LithpPrimitive Call(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpPrimitive def = parameters[0];
			LithpList defParams = (LithpList)Tail(LithpList.New(parameters), state, interp);
			switch (def.LithpType())
			{
				case LithpType.FN_NATIVE:
					return ((LithpFunctionDefinitionNative)def).Invoke(defParams, state, interp);
				case LithpType.FN:
					return ((LithpFunctionDefinition)def).Invoke(defParams, state, interp);
				case LithpType.ATOM:
				case LithpType.STRING:
					string strName = def.ToString();
					ILithpFunctionDefinition search;
					if ((object)state.Closure.TopMost != null && state.Closure.TopMost.IsDefined(strName))
					{
						search = (ILithpFunctionDefinition)state.Closure.TopMost[strName];
					}
					else if (state.Closure.IsDefined(strName))
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

		public static LithpPrimitive List(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			return parameters;
		}

		public static LithpPrimitive ListLength(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			return ((LithpList)parameters[0]).Count;
		}

		public static LithpPrimitive Round(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpFloat n = (LithpFloat)parameters[0].Cast(LithpType.FLOAT);
			return new LithpInteger(Convert.ToUInt64(Math.Round(n.value)));
		}

		public static LithpPrimitive IndexSet(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpList list = (LithpList)parameters[0];
			LithpInteger index = (LithpInteger)parameters[1];
			LithpPrimitive value = parameters[2];
			list[index] = value;
			return list;
		}

		public static LithpPrimitive Null(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			return LithpAtom.Atom("null");
		}

		public static LithpPrimitive Undefined(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			return LithpAtom.Atom("undefined");
		}

		public static LithpPrimitive Modulo(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			return parameters[0] % parameters[1];
		}

		public static LithpPrimitive BitwiseNot(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpInteger n = (LithpInteger)parameters[0];
			return new LithpInteger((n + 1) * -1);
		}

		public static LithpPrimitive BitwiseAnd(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpInteger a = (LithpInteger)parameters[0];
			LithpInteger b = (LithpInteger)parameters[1];
			return new LithpInteger(a & b);
		}

		public static LithpPrimitive BitwiseOr(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpInteger a = (LithpInteger)parameters[0];
			LithpInteger b = (LithpInteger)parameters[1];
			return new LithpInteger(a | b);
		}

		public static LithpPrimitive BitwiseXor(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpInteger a = (LithpInteger)parameters[0];
			LithpInteger b = (LithpInteger)parameters[1];
			return new LithpInteger(a ^ b);
		}

		public static LithpPrimitive ParseInt(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpString str = (LithpString)parameters[0].Cast(LithpType.STRING);
			string dropDecimals = Regex.Replace(str, @"[.][0-9]+$", "");
			return new LithpInteger(dropDecimals);
		}

		public static LithpPrimitive SquareRoot (LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			switch(parameters[0].LithpType())
			{
				case LithpType.INTEGER:
					return ((LithpInteger)(parameters[0])).Sqrt();
				case LithpType.FLOAT:
					return ((LithpFloat)(parameters[0])).Sqrt();
				default:
					throw new NotImplementedException();
			}
		}

		public static LithpPrimitive Dict (LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			return new LithpDict();
		}

		public static LithpPrimitive DictGet (LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpDict dict = (LithpDict)parameters[0];
			LithpString key = (LithpString)parameters[1].Cast(LithpType.STRING);
			return dict[key];
		}

		public static LithpPrimitive DictSet (LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpDict dict = (LithpDict)parameters[0];
			LithpString key = (LithpString)parameters[1].Cast(LithpType.STRING);
			LithpPrimitive value = parameters[2];
			dict[key] = value;
			return dict;
		}

		public static LithpPrimitive DictPresent (LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpDict dict = (LithpDict)parameters[0];
			LithpString key = (LithpString)parameters[1].Cast(LithpType.STRING);
			return dict.ContainsKey(key) ? LithpAtom.True : LithpAtom.False;
		}

		public static LithpPrimitive DictRemove (LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpDict dict = (LithpDict)parameters[0];
			LithpString key = (LithpString)parameters[1].Cast(LithpType.STRING);
			dict.Remove(key);
			return dict;
		}

		public static LithpPrimitive DictKeys(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpDict dict = (LithpDict)parameters[0];
			LithpList keys = new LithpList();
			foreach (var x in dict.Keys)
				keys.Add(x);
			return keys;
		}

		public static LithpPrimitive Repeat(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			LithpString str = (LithpString)parameters[0];
			LithpInteger n = (LithpInteger)parameters[1].Cast(LithpType.INTEGER);
			return new LithpString(new string(str.Value[0], n));
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
