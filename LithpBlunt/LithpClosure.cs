using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt
{
	public class LithpClosure : LithpPrimitive
	{
		/// <summary>
		/// The parent closure (scoped variables)
		/// </summary>
		public LithpClosure Parent;
		/// <summary>
		/// The topmost closure, where functions are defined
		/// </summary>
		public LithpClosure TopMost;

		/// <summary>
		/// Use our dictionary for values.
		/// </summary>
		protected LithpDict closure = new LithpDict();

		protected static int instanceId = 0;
		public readonly int InstanceId = instanceId++;

		/// <summary>
		/// Create a new closure with no parent
		/// </summary>
		public LithpClosure()
		{
			Parent = null;
			TopMost = this;
		}

		/// <summary>
		/// Create a new closure with the specified parent
		/// </summary>
		/// <param name="parent"></param>
		public LithpClosure(LithpClosure parent)
		{
			Parent = parent;
			TopMost = parent.TopMost ?? this;
		}

		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.CLOSURE;
		}

		public void SetImmediate(string name, LithpPrimitive value)
		{
			closure[name] = value;
		}

		public bool IsDefined (string name)
		{
			return closure.ContainsKey(name);
		}

		public void Set (string name, LithpPrimitive value)
		{
			if (!TrySet(name, value))
				SetImmediate(name, value);
		}

		public LithpPrimitive Get (string name)
		{
			if (IsDefined(name))
				return closure[name];
			else if (Parent)
				return Parent.Get(name);
			throw new KeyNotFoundException(name);
		}

		public bool IsDefinedAny (string name)
		{
			if (IsDefined(name))
				return true;
			if (Parent)
				return Parent.IsDefinedAny(name);
			return false;
		}

		public bool TrySet(string name, LithpPrimitive value)
		{
			if (IsDefined(name))
			{
				SetImmediate(name, value);
				return true;
			}
			else if (Parent)
				return Parent.TrySet(name, value);
			else
				return false;
		}

		public LithpPrimitive this[string name] {
			get {
				return Get(name);
			}
			set {
				Set(name, value);
			}
		}

		protected override int hashCode()
		{
			return closure.GetHashCode();
		}

		protected override string toString()
		{
			string result = "Closure::" + InstanceId.ToString() + "::{";
			bool first = true;
			foreach (var x in closure)
			{
				if (first)
					first = false;
				else
					result += ", ";
				result += x.Key + "=>" + x.Value;
			}
			return result;
		}
	}
}
