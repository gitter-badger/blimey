using System;

namespace Oats
{
	// EnumSerialiser<T>
	// test
	// enum Foo : long { One, Two };
	// enum Bar : byte { x = 255 };
	public class EnumSerialiser<T>
		: Serialiser<T>
	{
		Type underlyingType;

		public EnumSerialiser () {

			underlyingType = Enum.GetUnderlyingType(typeof(T));
		}

		public override T Read (ISerialisationChannel sc)
		{
			Object underlyingValue = sc.ReadReflective(underlyingType);

			T actualValue = (T)underlyingValue;
			return actualValue;
		}

		public override void Write (ISerialisationChannel sc, T actualValue)
		{
			Object underlyingValue = Convert.ChangeType (actualValue, underlyingType);

			sc.WriteReflective (underlyingType, (Int32) underlyingValue);
		}
	}
}

