using System;
using System.Text;

namespace Oats
{
	public class StringSerialiser
		: Serialiser<String>
	{
		public override String Read (ISerialisationChannel sc)
		{
			UInt32 length = sc.Read <UInt32> ();

			if (length == 0)
				return String.Empty;

			Byte[] encoded = new Byte[length];

			for (Int32 i = 0; i < length; ++i)
			{
				encoded [i] =  sc.Read <Byte> ();
			}

			return Encoding.UTF8.GetString (encoded);
		}

		public override void Write (ISerialisationChannel sc, String str)
		{
			if (str == null)
				throw new SerialisationException (
					"Not expected, the Serialisation Channel should deal with nulls.");

			Byte[] encoded = Encoding.UTF8.GetBytes (str);

			sc.Write <UInt32> ( (UInt32) encoded.Length);

			for (UInt32 i = 0; i < encoded.Length; ++i)
			{
				sc.Write <Byte> (encoded [i]);
			}
		}
	}
}

