using System;
using System.Text;

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

		public override T Read (ISerialisationChannel ss)
		{
			Object underlyingValue = ss.ReadReflective(underlyingType);

			T actualValue = (T)underlyingValue;
			return actualValue;
		}

		public override void Write (ISerialisationChannel ss, T actualValue)
		{
		 	Object underlyingValue = Convert.ChangeType (actualValue, underlyingType);

			ss.WriteReflective (underlyingType, (Int32) underlyingValue);
		}
	}

	public class StringSerialiser
		: Serialiser<String>
	{
		public override String Read (ISerialisationChannel ss)
		{
			Int32 length = ss.Read <Int32> ();

			if (length < 0)
				return null;

			if (length == 0)
				return String.Empty;

			Byte[] encoded = new Byte[length];

			for (Int32 i = 0; i < length; ++i)
			{
				encoded [i] =  ss.Read <Byte> ();
			}

			return Encoding.UTF8.GetString (encoded);
		}

		public override void Write (ISerialisationChannel ss, String str)
		{
			if (str == null)
			{
				ss.Write <Int32> (-1);
				return;
			}

			Byte[] encoded = Encoding.UTF8.GetBytes (str);

			ss.Write <Int32> (encoded.Length);

			for (Int32 i = 0; i < encoded.Length; ++i)
			{
				ss.Write <Byte> (encoded [i]);
			}
		}
	}


	public class NullableSerialiser<T>
		: Serialiser<T?>
		where T
		: struct
	{
		public override T? Read (ISerialisationChannel ss)
		{
			if(ss.Read <Boolean> ())
			{
				return ss.Read <T> ();
			}

			return null;
		}

		public override void Write (ISerialisationChannel ss, T? obj)
		{
			if( obj.HasValue )
			{
				ss.Write <Boolean> (true);
				ss.Write <T> (obj.Value);
			}
			else
			{
				ss.Write <Boolean>(false);
			}
		}
	}
}

