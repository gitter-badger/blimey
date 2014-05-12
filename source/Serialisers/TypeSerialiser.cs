using System;

namespace Oats
{
	public class TypeSerialiser
		: Serialiser<Type>
	{
		public override Type Read (ISerialisationChannel sc)
		{
			String fullName = sc.Read <String> ();
			return Type.GetType (fullName);
		}

		public override void Write (ISerialisationChannel sc, Type obj)
		{
			if (obj == null)
				throw new SerialisationException (
					"Not expected, the Serialisation Channel should deal with nulls.");

			String fullName = obj.FullName + ", " + obj.Namespace;
			sc.Write(fullName);
		}
	}
}

