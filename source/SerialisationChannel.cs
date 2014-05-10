using System;
using System.Reflection;
using System.IO;

namespace Oats
{
	public enum SerialisationChannelMode
	{
		Read,
		Write
	}

	public interface ISerialisationChannel
		: IDisposable
	{
		SerialisationChannelMode Mode { get; }

		void 		WriteReflective 	(Type type, Object value);
		void 		Write 		<T> 	(T value);

		Object 		ReadReflective 		(Type type);
		T 			Read 		<T> 	();
	}

	public class SerialisationChannel <TPrimitiveReader, TPrimitiveWriter>
		: ISerialisationChannel
	where TPrimitiveReader
		: IPrimitiveReader
	where TPrimitiveWriter
		: IPrimitiveWriter
	{
		readonly SerialiserDatabase serialiserDatabase;
		readonly TPrimitiveReader primitiveReader;
		readonly IPrimitiveWriter primitiveWriter;
		readonly SerialisationChannelMode mode;

		public SerialisationChannelMode Mode { get { return mode; } }

		public SerialisationChannel (
			SerialiserDatabase serialiserDatabase,
			Stream stream,
			SerialisationChannelMode mode)
		{
			this.serialiserDatabase = serialiserDatabase;

			this.mode = mode;

			if (mode == SerialisationChannelMode.Read)
			{
				this.primitiveReader = 
					(TPrimitiveReader)
					Activator.CreateInstance (
						typeof(TPrimitiveReader), 
						new Object [] { stream });
			}

			if (mode == SerialisationChannelMode.Write)
			{
				this.primitiveWriter = 
					(TPrimitiveWriter)
						Activator.CreateInstance (
							typeof(TPrimitiveWriter), 
							new Object [] { stream });
			}
		}

		public void Dispose ()
		{
			var tryDispose = new Object[] { primitiveReader, primitiveWriter };

			foreach (Object item in tryDispose) {
				if (item != null) {
					if (item is IDisposable) {
						(item as IDisposable).Dispose ();
					}
				}
			}
		}

		static Boolean IsSerialisablePrimitiveType<T> ()
		{
			if (typeof(T) == typeof (IntPtr)) return false;
			if (typeof(T) == typeof (UIntPtr)) return false;

			return typeof(T).IsPrimitive;
		}


		public void WriteReflective (Type type, Object value)
		{
			Type thisType = typeof(SerialisationChannel <TPrimitiveReader, TPrimitiveWriter>);
			MethodInfo mi = thisType
				.GetMethod (
					"Write");

			var gmi = mi.MakeGenericMethod(type);

			gmi.Invoke(this, new [] { value });
		}

		public void Write <T> (T value)
		{
			if (mode != SerialisationChannelMode.Write) 
			{
				throw new SerialisationException ("This serialisation stream is for writing only.");
			}

			if (IsSerialisablePrimitiveType<T>())
			{
				if (typeof(T) == typeof(Boolean)) {
					primitiveWriter.Write ( (Boolean)(Object) value);
					return;
				}


				if (typeof(T) == typeof(Byte)) {
					primitiveWriter.Write ( (Byte)(Object) value);
					return;
				}

				if (typeof(T) == typeof(Char)) {
					primitiveWriter.Write ( (Char)(Object) value);
					return;
				}

				if (typeof(T) == typeof(Double)) {
					primitiveWriter.Write ( (Double)(Object) value);
					return;
				}

				if (typeof(T) == typeof(Int16)) {
					primitiveWriter.Write ( (Int16)(Object) value);
					return;
				}

				if (typeof(T) == typeof(Int32)) {
					primitiveWriter.Write ( (Int32)(Object) value);
					return;
				}

				if (typeof(T) == typeof(Int64)) {
					primitiveWriter.Write ( (Int64)(Object) value);
					return;
				}

				if (typeof(T) == typeof(SByte)) {
					primitiveWriter.Write ( (SByte)(Object) value);
					return;
				}

				if (typeof(T) == typeof(Single)) {
					primitiveWriter.Write ( (Single)(Object) value);
					return;
				}

				if (typeof(T) == typeof(UInt16)) {
					primitiveWriter.Write ( (UInt16)(Object) value);
					return;
				}

				if (typeof(T) == typeof(UInt32)) {
					primitiveWriter.Write ( (UInt32)(Object) value);
					return;
				}

				if (typeof(T) == typeof(UInt64)) {
					primitiveWriter.Write ( (UInt64)(Object) value);
					return;
				}
			}
			else
			{
				var serialiser = serialiserDatabase.GetSerialiser <T> ();

				serialiser.Write (this, value);

				return;
			}
		}

		public Object ReadReflective (Type type)
		{
			MethodInfo mi = typeof(SerialisationChannel <TPrimitiveReader, TPrimitiveWriter>)
				.GetMethod ("Read", new Type[]{});

			var gmi = mi.MakeGenericMethod(type);

			return gmi.Invoke(this, null);
		}

		public T Read <T> ()
		{
			if (mode != SerialisationChannelMode.Read)
			{
				throw new Exception ("This serialisation stream is for reading only.");
			}

			if(IsSerialisablePrimitiveType<T>())
			{
				if (typeof(T) == typeof (Boolean))
					return (T) (Object) primitiveReader.ReadBoolean ();

				if (typeof(T) == typeof (Byte))     
					return (T) (Object) primitiveReader.ReadByte ();

				if (typeof(T) == typeof (Char))     
					return (T) (Object) primitiveReader.ReadChar ();

				if (typeof(T) == typeof (Double))   
					return (T) (Object) primitiveReader.ReadDouble ();

				if (typeof(T) == typeof (Int16))    
					return (T) (Object) primitiveReader.ReadInt16 ();

				if (typeof(T) == typeof (Int32))    
					return (T) (Object) primitiveReader.ReadInt32 ();

				if (typeof(T) == typeof (Int64))    
					return (T) (Object) primitiveReader.ReadInt64 ();

				if (typeof(T) == typeof (SByte))    
					return (T) (Object) primitiveReader.ReadSByte ();

				if (typeof(T) == typeof (Single))   
					return (T) (Object) primitiveReader.ReadSingle ();

				if (typeof(T) == typeof (UInt16))   
					return (T) (Object) primitiveReader.ReadUInt16 ();

				if (typeof(T) == typeof (UInt32))   
					return (T) (Object) primitiveReader.ReadUInt32 ();

				if (typeof(T) == typeof (UInt64))   
					return (T) (Object) primitiveReader.ReadUInt64 ();

				throw new Exception ();
			}
			else
			{
				// locate the correct serialiser
				var serialiser = serialiserDatabase.GetSerialiser <T> ();

				return serialiser.Read (this);
			}
		}
	}

}

