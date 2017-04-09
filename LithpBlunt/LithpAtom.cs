using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt
{
	public abstract class LithpAtomType : LithpPrimitive
	{

	}

	public abstract class LithpAtomType<NameType,ValueType> : LithpAtomType
	{
		public readonly NameType Name;
		public readonly ValueType Id;

		// Protected constructor, use LithpAtom::Atom()
		protected LithpAtomType(NameType name, ValueType id)
		{
			Name = name;
			Id = id;
		}

		protected static Dictionary<int, LithpAtomType> atomsById = new Dictionary<int, LithpAtomType>();
		protected static Dictionary<NameType, LithpAtomType> atomsByName = new Dictionary<NameType, LithpAtomType>();
	}

	public class LithpAtom : LithpAtomType<string, int>
	{
		protected LithpAtom(string name, int id) : base(name, id)
		{
		}

		public static LithpAtom Nil = Atom("nil");
		public static LithpAtom True = Atom("true");
		public static LithpAtom False = Atom("false");

		public override LithpType LithpType()
		{
			return LithpBlunt.LithpType.ATOM;
		}

		protected override int hashCode()
		{
			return Id;
		}

		protected override string toString()
		{
			return Name;
		}

		public override bool compareEqual(LithpPrimitive other)
		{
			LithpAtom otherAtom = (LithpAtom)other;
			return Id == otherAtom.Id;
		}

		protected static int counter = 0;
		public static LithpAtom Atom(string name)
		{
			if(atomsByName.ContainsKey(name) == false)
			{
				int id = counter++;
				var atom = new LithpAtom(name, id);
				atomsByName[name] = atom;
				atomsById[id] = atom;
			}
			return atomsByName[name] as LithpAtom;
		}

		public static implicit operator string(LithpAtom atom)
		{
			return atom.ToString();
		}
		public static implicit operator LithpAtom(string name)
		{
			return Atom(name);
		}
	}
}
