using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt.OpChainMembers
{
	public interface ILithpOpChainMember : ILithpPrimitive
	{

	}
	public abstract class LithpOpChainMember : LithpPrimitive, ILithpOpChainMember
	{
	}
}
