using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LithpBlunt.OpChainMembers;

namespace LithpBlunt
{
	public enum LithpType
	{
		INTEGER,
		ATOM,
		STRING,
		LIST,
		DICT,
		OPCHAIN,
		FUNCTIONCALL,
		LITERAL,
		FN,
		FN_NATIVE,
		CLOSURE,
		VAR
	}

	public interface ILithpPrimitive
	{
		string ToString();
		string ToLiteral();
		LithpType LithpType();
	}

	public abstract class LithpPrimitive : ILithpPrimitive
	{
		public override string ToString()
		{
			return toString();
		}
		public string ToLiteral()
		{
			string prefix, postfix;
			prefix = postfix = "";
			switch(LithpType())
			{
				case LithpBlunt.LithpType.ATOM:
					prefix = postfix = "'";
					break;
				case LithpBlunt.LithpType.STRING:
					prefix = postfix = "\"";
					break;
				case LithpBlunt.LithpType.LIST:
					prefix = "[";
					postfix = "]";
					break;
				case LithpBlunt.LithpType.DICT:
					prefix = "{";
					postfix = "}";
					break;
			}
			return prefix + ToString() + postfix;
		}
		protected abstract string toString();
		public virtual bool compareEqual(LithpPrimitive other)
		{
			return false;
		}
		protected abstract int hashCode();

		public abstract LithpType LithpType();

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return hashCode();
		}

		protected static bool compareEqual(LithpPrimitive a, LithpPrimitive b)
		{
			if (a.LithpType() == LithpBlunt.LithpType.LITERAL)
				a = ((LithpLiteral)a).Value;
			if (b.LithpType() == LithpBlunt.LithpType.LITERAL)
				b = ((LithpLiteral)b).Value;
			if (a.LithpType() != b.LithpType())
				return false;
			return a.compareEqual(b);
		}

		public static bool operator ==(LithpPrimitive a, LithpPrimitive b)
		{
			return compareEqual(a, b);
		}
		public static bool operator !=(LithpPrimitive a, LithpPrimitive b)
		{
			if (a.LithpType() == LithpBlunt.LithpType.LITERAL)
				a = ((LithpLiteral)a).Value;
			if (b.LithpType() == LithpBlunt.LithpType.LITERAL)
				b = ((LithpLiteral)b).Value;
			if (a.LithpType() != b.LithpType())
				return true;
			return !compareEqual(a, b);
		}

		public static bool operator <(LithpPrimitive a, LithpPrimitive b)
		{
			if (a.LithpType() == LithpBlunt.LithpType.LITERAL)
				a = ((LithpLiteral)a).Value;
			if (b.LithpType() == LithpBlunt.LithpType.LITERAL)
				b = ((LithpLiteral)b).Value;
			if (a.LithpType() != b.LithpType())
				return false;
			return a.compareLessThan(b);
		}

		public static bool operator >(LithpPrimitive a, LithpPrimitive b)
		{
			if (a.LithpType() == LithpBlunt.LithpType.LITERAL)
				a = ((LithpLiteral)a).Value;
			if (b.LithpType() == LithpBlunt.LithpType.LITERAL)
				b = ((LithpLiteral)b).Value;
			if (a.LithpType() != b.LithpType())
				return false;
			return a.compareMoreThan(b);
		}

		public static bool operator <=(LithpPrimitive a, LithpPrimitive b)
		{
			if (a.LithpType() == LithpBlunt.LithpType.LITERAL)
				a = ((LithpLiteral)a).Value;
			if (b.LithpType() == LithpBlunt.LithpType.LITERAL)
				b = ((LithpLiteral)b).Value;
			if (a.LithpType() != b.LithpType())
				return false;
			return a.compareLessThan(b) || a.compareEqual(b);
		}

		public static bool operator >=(LithpPrimitive a, LithpPrimitive b)
		{
			if (a.LithpType() == LithpBlunt.LithpType.LITERAL)
				a = ((LithpLiteral)a).Value;
			if (b.LithpType() == LithpBlunt.LithpType.LITERAL)
				b = ((LithpLiteral)b).Value;
			if (a.LithpType() != b.LithpType())
				return false;
			return a.compareMoreThan(b) || a.compareEqual(b);
		}

		public static LithpPrimitive operator +(LithpPrimitive a, LithpPrimitive b)
		{
			if (a.LithpType() == LithpBlunt.LithpType.LITERAL)
				a = ((LithpLiteral)a).Value;
			if (b.LithpType() == LithpBlunt.LithpType.LITERAL)
				b = ((LithpLiteral)b).Value;
			return a.operatorPlus(b);
		}

		public static LithpPrimitive operator -(LithpPrimitive a, LithpPrimitive b)
		{
			if (a.LithpType() == LithpBlunt.LithpType.LITERAL)
				a = ((LithpLiteral)a).Value;
			if (b.LithpType() == LithpBlunt.LithpType.LITERAL)
				b = ((LithpLiteral)b).Value;
			return a.operatorMinus(b);
		}

		public static LithpPrimitive operator /(LithpPrimitive a, LithpPrimitive b)
		{
			if (a.LithpType() == LithpBlunt.LithpType.LITERAL)
				a = ((LithpLiteral)a).Value;
			if (b.LithpType() == LithpBlunt.LithpType.LITERAL)
				b = ((LithpLiteral)b).Value;
			return a.operatorDivide(b);
		}

		public static LithpPrimitive operator *(LithpPrimitive a, LithpPrimitive b)
		{
			if (a.LithpType() == LithpBlunt.LithpType.LITERAL)
				a = ((LithpLiteral)a).Value;
			if (b.LithpType() == LithpBlunt.LithpType.LITERAL)
				b = ((LithpLiteral)b).Value;
			return a.operatorMultiply(b);
		}

		protected virtual bool compareLessThan(LithpPrimitive other)
		{
			return false;
		}

		protected virtual bool compareMoreThan(LithpPrimitive other)
		{
			return false;
		}

		protected virtual LithpPrimitive operatorPlus(LithpPrimitive other)
		{
			throw new NotImplementedException();
		}

		protected virtual LithpPrimitive operatorMinus(LithpPrimitive other)
		{
			throw new NotImplementedException();
		}

		protected virtual LithpPrimitive operatorMultiply(LithpPrimitive other)
		{
			throw new NotImplementedException();
		}

		protected virtual LithpPrimitive operatorDivide(LithpPrimitive other)
		{
			throw new NotImplementedException();
		}

		// Implicit class conversions
		public static implicit operator LithpPrimitive(string str)
		{
			return new LithpString(str);
		}
		public static implicit operator string(LithpPrimitive prim)
		{
			return prim.ToString();
		}

		public static implicit operator LithpPrimitive(long v)
		{
			return new LithpInteger(v);
		}

		public static implicit operator LithpPrimitive(int v)
		{
			return new LithpInteger(v);
		}

		public static implicit operator bool(LithpPrimitive v)
		{
			return (object)v != null;
		}
	}
}
