using System;

namespace Oats
{
	public class NullableSerialiser<T>
		: Serialiser<T?>
		where T
		: struct
	{
		public override T? Read (ISerialisationChannel sc)
		{
			if(sc.Read <Boolean> ())
			{
				return sc.Read <T> ();
			}

			return null;
		}

		public override void Write (ISerialisationChannel sc, T? obj)
		{
			if( obj.HasValue )
			{
				sc.Write <Boolean> (true);
				sc.Write <T> (obj.Value);
			}
			else
			{
				sc.Write <Boolean>(false);
			}
		}
	}
}

