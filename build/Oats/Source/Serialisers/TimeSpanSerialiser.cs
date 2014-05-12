using System;

namespace Oats
{
	public class TimeSpanSerialiser
		: Serialiser<TimeSpan>
	{
		public override TimeSpan Read  (ISerialisationChannel sc)
		{
			Int64 ticks = sc.Read <Int64> ();

			return new TimeSpan(ticks);
		}

		public override void Write (ISerialisationChannel sc, TimeSpan obj)
		{
			if (obj == null)
				throw new SerialisationException (
					"Not expected, the Serialisation Channel should deal with nulls.");

			sc.Write<Int64> (obj.Ticks);
		}
	}
}

