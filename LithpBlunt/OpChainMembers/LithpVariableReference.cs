using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt.OpChainMembers
{
	public class LithpVariableReference : LithpOpChainMember
	{
		public readonly string Name;

		public LithpVariableReference(string name)
		{
			Name = name;
		}

		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.VAR;
		}

		protected override int hashCode()
		{
			return Name.GetHashCode();
		}

		protected override string toString()
		{
			return Name;
		}
	}
}
