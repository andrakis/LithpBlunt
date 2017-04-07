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
		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.OPCHAIN;
		}
	}
}
