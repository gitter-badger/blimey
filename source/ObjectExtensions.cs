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

			using (var stream = new MemoryStream ())
			{
				using (var channel = new SerialisationChannel<BinaryStreamSerialiser> 
					(stream, ChannelMode.Write)) 
				{
					channel.Write<T> (o);

					return stream.GetBuffer ();
				}
			}
		}

		public static T FromBinary <T> (this Byte[] bytes)
		{
			using (var stream = new MemoryStream (bytes))
			{
				using (var channel = new SerialisationChannel <BinaryStreamSerialiser> 
					(stream, ChannelMode.Read))
				{
					Object o = channel.Read<T> ();

					return (T) o;
				}
			}
		}
	}


}
