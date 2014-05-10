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

			using (var stream = new MemoryStream ())
			{
				using (var channel = 
					      new SerialisationChannel
					<BinaryPrimitiveReader, BinaryPrimitiveWriter> 
						(db, stream, SerialisationChannelMode.Write)) 
				{
					channel.Write<T> (o);

					return stream.GetBuffer ();
				}

			}
		}

		public static T FromBinary <T> (this Byte[] bytes)
		{
			var db = SerialiserDatabase.Instance;

			using (var stream = new MemoryStream (bytes))
			{
				using (var channel = 
					      new SerialisationChannel
					<BinaryPrimitiveReader, BinaryPrimitiveWriter> 
						(db, stream, SerialisationChannelMode.Read))
				{
					Object o = channel.Read<T> ();

					return (T) o;
				}
			}
		}
	}


}
