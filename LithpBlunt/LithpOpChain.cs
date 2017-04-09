using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LithpBlunt.OpChainMembers;

namespace LithpBlunt
{
	public class LithpOpChain : LithpListType<LithpOpChainMember>
	{
		public readonly LithpClosure Closure;
		public readonly LithpOpChain Parent;

		public string FunctionEntry = null;

		protected int Location = 0; // location in list

		public LithpOpChain()
		{
			Closure = new LithpClosure();
			Parent = null;
		}

		public LithpOpChain(LithpOpChain parent)
		{
			Closure = new LithpClosure(parent.Closure);
			Parent = parent;
		}

		public LithpOpChain(LithpOpChain parent, LithpOpChain body)
			: base(body.ToArray())
		{
			Closure = new LithpClosure(parent.Closure);
			Parent = parent;
		}

		public LithpOpChain CloneWithScope(LithpOpChain scope)
		{
			return new LithpOpChain(scope, this);
		}

		public LithpOpChainMember Get ()
		{
			return Get(Location);
		}

		public LithpOpChainMember Get(int location)
		{
			return this[location];
		}

		public LithpOpChainMember Next ()
		{
			return Get(Location++);
		}

		public bool AtEnd()
		{
			return Location == this.Count;
		}

		/// <summary>
		/// Rewind the OpChain to the start.
		/// Allows for stackless recursive invocation of Lithp functions.
		/// </summary>
		public void Rewind()
		{
			Location = 0;
		}

		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.OPCHAIN;
		}

		public void ImportBuiltins(LithpBuiltins builtins)
		{
			foreach (var x in builtins)
			{
				Closure[x.Key] = x.Value;
			}
		}
	}

}
