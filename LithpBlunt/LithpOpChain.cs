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
		public LithpDict Closure = new LithpDict();

		protected int Location = 0; // location in list

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
