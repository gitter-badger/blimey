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
			sc.Write<Int64> (obj.Ticks);
		}
	}
}

