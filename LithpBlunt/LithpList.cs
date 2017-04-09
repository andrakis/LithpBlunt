using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt
{
	public abstract class LithpListType<T> : LithpPrimitive, IList<T> where T : LithpPrimitive
	{
		protected List<T> value;

		public LithpListType ()
		{
			value = new List<T>();
		}

		public LithpListType (T[] values)
		{
			value = new List<T>(values);
		}

		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.LIST;
		}

		protected override int hashCode()
		{
			return value.GetHashCode();
		}

		protected override string toString()
		{
			string result = "";
			bool first = true;
			foreach(T x in value)
			{
				if (!first)
					result += " ";
				else
					first = false;
				result += x.ToString();
			}

			return result;
		}

		public T this[int index] {
			get {
				return ((IList<T>)value)[index];
			}

			set {
				((IList<T>)this.value)[index] = value;
			}
		}

		public int Count {
			get {
				return ((IList<T>)value).Count;
			}
		}

		public bool IsReadOnly {
			get {
				return ((IList<T>)value).IsReadOnly;
			}
		}

		public void Add(T item)
		{
			((IList<T>)value).Add(item);
		}

		public void Clear()
		{
			((IList<T>)value).Clear();
		}

		public bool Contains(T item)
		{
			return ((IList<T>)value).Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			((IList<T>)value).CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return ((IList<T>)value).GetEnumerator();
		}

		public int IndexOf(T item)
		{
			return ((IList<T>)value).IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			((IList<T>)value).Insert(index, item);
		}

		public bool Remove(T item)
		{
			return ((IList<T>)value).Remove(item);
		}

		public void RemoveAt(int index)
		{
			((IList<T>)value).RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IList<T>)value).GetEnumerator();
		}

		public void Each(Action<LithpPrimitive,int> Callback)
		{
			int i = 0;
			foreach (var x in this)
				Callback(x, i++);
		}
	}

	public class LithpList : LithpListType<LithpPrimitive>
	{
		public LithpList()
		{

		}
		public LithpList(LithpPrimitive[] values)
			: base(values)
		{

		}
		public static LithpList New(params LithpPrimitive[] values)
		{
			return new LithpList(values);
		}
	}
}
