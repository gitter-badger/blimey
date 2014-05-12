
using System;

namespace Oats
{
	public class DecimalSerialiser
		: Serialiser<Decimal>
	{
		public override Decimal Read (ISerialisationChannel sc)
		{
			String str = sc.Read<String> ();

			return Decimal.Parse (str);
		}

		public override void Write (ISerialisationChannel sc, Decimal obj)
		{
			sc.Write <String>(obj.ToString ());
		}
	}
}

