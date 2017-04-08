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
			builtins["print/*"] = new LithpFunctionDefinitionNative("print/*", new string[] { }, Print);
		}

		public static LithpPrimitive Print(LithpList parameters, LithpOpChain state,
			LithpInterpreter interp)
		{
			bool first = true;
			string result = "";

			foreach(LithpPrimitive x in parameters)
			{
				if (!first)
					result += " ";
				else
					first = false;
				result += x.ToString();
			}
			Console.WriteLine(result);
			return LithpAtom.Atom("nil");
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
