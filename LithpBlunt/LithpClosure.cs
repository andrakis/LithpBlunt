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
		public readonly LithpClosure Parent;
		/// <summary>
		/// The topmost closure, where functions are defined
		/// </summary>
		public readonly LithpClosure TopMost;

		/// <summary>
		/// Use our dictionary for values.
		/// </summary>
		protected LithpDict closure = new LithpDict();

		/// <summary>
		/// Create a new closure with no parent
		/// </summary>
		public LithpClosure()
		{
			Parent = null;
			TopMost = null;
		}

		/// <summary>
		/// Create a new closure with the specified parent
		/// </summary>
		/// <param name="parent"></param>
		public LithpClosure(LithpClosure parent)
		{
			Parent = parent;
			TopMost = parent.TopMost;
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
				Set(name, value);
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
			return "stub:LithpClosure::toString()";
		}
	}
}
