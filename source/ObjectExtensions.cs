using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Oats
{

	public static class ObjectExtensions
	{
		public static Byte[] ToBinary <T> (this Object obj)
		{
			T o = (T) obj;

			var db = SerialiserDatabase.Instance;

			Serialiser <T> serialiser = db.GetSerialiser <T> ();

			using (var channel = 
				new SerialisationChannel
					<Byte[], BinaryPrimitiveSerialiser> (db))
			{
				serialiser.Write (channel, o);

				return channel.GetData ();
			}
		}

		public static T FromBinary <T> (this Byte[] bytes)
		{
			var db = SerialiserDatabase.Instance;

			Serialiser <T> serialiser = db.GetSerialiser <T> ();

			using (var channel = 
				new SerialisationChannel
					<Byte[], BinaryPrimitiveSerialiser> (db, bytes))
			{
				Object o = serialiser.Read (channel);

				return (T) o;
			}
		}
	}


}
