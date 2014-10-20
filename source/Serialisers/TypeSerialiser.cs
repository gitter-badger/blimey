using System;

namespace Oats
{
	public class TypeSerialiser
		: Serialiser<Type>
	{
		public override Type Read (ISerialisationChannel sc)
		{
			String typeName = sc.Read <String> ();
			Type t = Type.GetType (typeName);

            if (t == null)
                throw new Exception ("Unknown type: " + typeName);

			return t;
		}

		public override void Write (ISerialisationChannel sc, Type t)
		{
			String typeName = t.FullName + ", " + t.Namespace;
			sc.Write(typeName);
		}
	}
}

