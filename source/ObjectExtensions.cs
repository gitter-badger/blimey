// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ ________          __                                                   │ \\
// │ \_____  \ _____ _/  |_  ______                                         │ \\
// │  /   |   \\__  \\   __\/  ___/                                         │ \\
// │ /    |    \/ __ \|  |  \___ \                                          │ \\
// │ \_______  (____  /__| /____  >                                         │ \\
// │         \/     \/          \/                                          │ \\
// │                                                                        │ \\
// │ An awesome C# serialisation library.                                   │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey3D (http://www.blimey3d.com)           │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors:                                                               │ \\
// │ ~ Ash Pook (http://www.ajpook.com)                                     │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining  │ \\
// │ a copy of this software and associated documentation files (the        │ \\
// │ "Software"), to deal in the Software without restriction, including    │ \\
// │ without limitation the rights to use, copy, modify, merge, publish,    │ \\
// │ distribute, sublicense, and/or sellcopies of the Software, and to      │ \\
// │ permit persons to whom the Software is furnished to do so, subject to  │ \\
// │ the following conditions:                                              │ \\
// │                                                                        │ \\
// │ The above copyright notice and this permission notice shall be         │ \\
// │ included in all copies or substantial portions of the Software.        │ \\
// │                                                                        │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        │ \\
// │ EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     │ \\
// │ MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. │ \\
// │ IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY   │ \\
// │ CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   │ \\
// │ TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE       │ \\
// │ SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 │ \\
// └────────────────────────────────────────────────────────────────────────┘ \\

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Oats
{
	public static class ObjectExtensions
	{
		static MethodInfo toBinaryImplementation = null;
		static MethodInfo fromBinaryImplementation = null;
		static MethodInfo toChecksumImplementation = null;

		static ObjectExtensions ()
		{
			CacheMethodInfos ();
		}

		static void CacheMethodInfos ()
		{
			toBinaryImplementation = typeof(ObjectExtensions).GetMethod (
				"ToBinaryImplementation", 
				BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Default,
				null,
				new Type[] { typeof (Object) },
				null);

			fromBinaryImplementation = typeof(ObjectExtensions).GetMethod (
				"FromBinaryImplementation", 
				BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Default,
				null,
				new Type[] { typeof (Byte[]) },
				null);

			toChecksumImplementation = typeof(ObjectExtensions).GetMethod (
				"ToChecksumImplementation", 
				BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Default,
				null,
				new Type[] { typeof (Object) },
				null);

			if (toBinaryImplementation == null)
				throw new Exception ();

			if (fromBinaryImplementation == null)
				throw new Exception ();

			if (toChecksumImplementation == null)
				throw new Exception ();

		}

		static Byte[] ToBinaryImplementation <T> (Object obj)
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

		static T FromBinaryImplementation <T> (Byte[] bytes)
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

		static Byte[] ToChecksumImplementation <T> (Object obj)
		{
			Byte[] binary = obj.ToBinary <T> ();
			HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider ();
			Byte[] hash = hashAlgorithm.ComputeHash (binary);

			return hash;
		}

		// ----------------------------------------------------------------------- //

		static Byte[] ToBinaryReflectiveImplementation (Type type, Object obj)
		{
			MethodInfo genericMethodInfo = toBinaryImplementation.MakeGenericMethod (new Type [] { type });

			Object result = genericMethodInfo.Invoke (null, new Object[] { obj });

			return (Byte[]) result;
		}

		static Object FromBinaryReflectiveImplementation (Type type, Byte[] bytes)
		{
			MethodInfo genericMethodInfo = fromBinaryImplementation.MakeGenericMethod (new Type [] { type });

			Object result = genericMethodInfo.Invoke (null, new Object[] { bytes });

			return (Byte[]) result;
		}

		static Byte[] ToChecksumReflectiveImplementation (Type type, Object obj)
		{
			MethodInfo genericMethodInfo = toChecksumImplementation.MakeGenericMethod (new Type [] { type });

			Object result = genericMethodInfo.Invoke (null, new Object[] { obj });

			return (Byte[]) result;
		}













		// ----------------------------------------------------------------------- //

		public static Byte[] ToBinaryReflective (this Object obj, Type type)
		{
			return ToBinaryReflectiveImplementation (type, obj);
		}

		public static Object FromBinaryReflective (this Byte[] bytes, Type type)
		{
			return FromBinaryReflectiveImplementation (type, bytes);
		}

		public static Byte[] ToChecksumReflective (this Object obj, Type type)
		{
			return ToChecksumReflectiveImplementation (type, obj);
		}

		// ----------------------------------------------------------------------- //

		public static Byte[] ToBinary <T> (this Object obj)
		{
			return ToBinaryImplementation <T> (obj);
		}

		public static T FromBinary <T> (this Byte[] bytes)
		{
			return FromBinaryImplementation <T> (bytes);
		}

		public static Byte[] ToChecksum <T> (this Object obj)
		{
			return ToChecksumImplementation <T> (obj);
		}
	}
}
