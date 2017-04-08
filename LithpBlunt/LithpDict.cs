using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt
{
	public class LithpDict : LithpPrimitive, IDictionary<string,LithpPrimitive>
	{
		protected Dictionary<string, LithpPrimitive> dict = new Dictionary<string, LithpPrimitive>();

		public LithpPrimitive this[string key] {
			get {
				return ((IDictionary<string, LithpPrimitive>)dict)[key];
			}

			set {
				((IDictionary<string, LithpPrimitive>)dict)[key] = value;
			}
		}

		public int Count {
			get {
				return ((IDictionary<string, LithpPrimitive>)dict).Count;
			}
		}

		public bool IsReadOnly {
			get {
				return ((IDictionary<string, LithpPrimitive>)dict).IsReadOnly;
			}
		}

		public ICollection<string> Keys {
			get {
				return ((IDictionary<string, LithpPrimitive>)dict).Keys;
			}
		}

		public ICollection<LithpPrimitive> Values {
			get {
				return ((IDictionary<string, LithpPrimitive>)dict).Values;
			}
		}

		public void Add(KeyValuePair<string, LithpPrimitive> item)
		{
			((IDictionary<string, LithpPrimitive>)dict).Add(item);
		}

		public void Add(string key, LithpPrimitive value)
		{
			((IDictionary<string, LithpPrimitive>)dict).Add(key, value);
		}

		public void Clear()
		{
			((IDictionary<string, LithpPrimitive>)dict).Clear();
		}

		public bool Contains(KeyValuePair<string, LithpPrimitive> item)
		{
			return ((IDictionary<string, LithpPrimitive>)dict).Contains(item);
		}

		public bool ContainsKey(string key)
		{
			return ((IDictionary<string, LithpPrimitive>)dict).ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<string, LithpPrimitive>[] array, int arrayIndex)
		{
			((IDictionary<string, LithpPrimitive>)dict).CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<string, LithpPrimitive>> GetEnumerator()
		{
			return ((IDictionary<string, LithpPrimitive>)dict).GetEnumerator();
		}

		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.DICT;
		}

		public bool Remove(KeyValuePair<string, LithpPrimitive> item)
		{
			return ((IDictionary<string, LithpPrimitive>)dict).Remove(item);
		}

		public bool Remove(string key)
		{
			return ((IDictionary<string, LithpPrimitive>)dict).Remove(key);
		}

		public bool TryGetValue(string key, out LithpPrimitive value)
		{
			return ((IDictionary<string, LithpPrimitive>)dict).TryGetValue(key, out value);
		}

		protected override int hashCode()
		{
			return dict.GetHashCode();
		}

		protected override string toString()
		{
			string result = "";
			bool first = true;
			foreach(KeyValuePair<string,LithpPrimitive> x in dict)
			{
				if (!first)
					result += " ";
				else
					first = false;
				result += x.Key + ": " + x.Value.ToLiteral();
			}
			return result;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IDictionary<string, LithpPrimitive>)dict).GetEnumerator();
		}
	}
}
