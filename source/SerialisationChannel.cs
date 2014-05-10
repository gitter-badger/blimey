using System;
using System.Reflection;

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

	public class SerialisationChannel <TData, TPrimitiveSerialiser>
		: ISerialisationChannel
	where TPrimitiveSerialiser
		: IPrimitiveSerialiser <TData>
	{
		readonly SerialiserDatabase serialiserDatabase;
		readonly IPrimitiveSerialiser <TData> primitiveSerialiser;
		readonly SerialisationChannelMode mode;

		public SerialisationChannelMode Mode { get { return mode; } }

		public SerialisationChannel (
			SerialiserDatabase serialiserDatabase)
		{
			this.serialiserDatabase = serialiserDatabase;
			this.primitiveSerialiser = 
				(TPrimitiveSerialiser)
					Activator.CreateInstance (
						typeof (TPrimitiveSerialiser));

			this.mode = SerialisationChannelMode.Write;
		}

		public SerialisationChannel (
			SerialiserDatabase serialiserDatabase,
			TData data)
		{
			this.serialiserDatabase = serialiserDatabase;
			this.primitiveSerialiser = 
				(TPrimitiveSerialiser)
					Activator.CreateInstance (
						typeof (TPrimitiveSerialiser));

			this.primitiveSerialiser.SetData (data);
			this.mode = SerialisationChannelMode.Read;
		}

		public TData GetData ()
		{
			return primitiveSerialiser.GetData ();
		}

		public void Dispose ()
		{
			var disposable = this.primitiveSerialiser as IDisposable;

			if (disposable != null)
			{
				disposable.Dispose ();
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
			Type thisType = typeof(SerialisationChannel <TData, TPrimitiveSerialiser>);
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
					primitiveSerialiser.Write ( (Boolean)(Object) value);
					return;
				}


				if (typeof(T) == typeof(Byte)) {
					primitiveSerialiser.Write ( (Byte)(Object) value);
					return;
				}

				if (typeof(T) == typeof(Char)) {
					primitiveSerialiser.Write ( (Char)(Object) value);
					return;
				}

				if (typeof(T) == typeof(Double)) {
					primitiveSerialiser.Write ( (Double)(Object) value);
					return;
				}

				if (typeof(T) == typeof(Int16)) {
					primitiveSerialiser.Write ( (Int16)(Object) value);
					return;
				}

				if (typeof(T) == typeof(Int32)) {
					primitiveSerialiser.Write ( (Int32)(Object) value);
					return;
				}

				if (typeof(T) == typeof(Int64)) {
					primitiveSerialiser.Write ( (Int64)(Object) value);
					return;
				}

				if (typeof(T) == typeof(SByte)) {
					primitiveSerialiser.Write ( (SByte)(Object) value);
					return;
				}

				if (typeof(T) == typeof(Single)) {
					primitiveSerialiser.Write ( (Single)(Object) value);
					return;
				}

				if (typeof(T) == typeof(UInt16)) {
					primitiveSerialiser.Write ( (UInt16)(Object) value);
					return;
				}

				if (typeof(T) == typeof(UInt32)) {
					primitiveSerialiser.Write ( (UInt32)(Object) value);
					return;
				}

				if (typeof(T) == typeof(UInt64)) {
					primitiveSerialiser.Write ( (UInt64)(Object) value);
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
			MethodInfo mi = typeof(SerialisationChannel <TData, TPrimitiveSerialiser>)
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
					return (T) (Object) primitiveSerialiser.ReadBoolean ();

				if (typeof(T) == typeof (Byte))     
					return (T) (Object) primitiveSerialiser.ReadByte ();

				if (typeof(T) == typeof (Char))     
					return (T) (Object) primitiveSerialiser.ReadChar ();

				if (typeof(T) == typeof (Double))   
					return (T) (Object) primitiveSerialiser.ReadDouble ();

				if (typeof(T) == typeof (Int16))    
					return (T) (Object) primitiveSerialiser.ReadInt16 ();

				if (typeof(T) == typeof (Int32))    
					return (T) (Object) primitiveSerialiser.ReadInt32 ();

				if (typeof(T) == typeof (Int64))    
					return (T) (Object) primitiveSerialiser.ReadInt64 ();

				if (typeof(T) == typeof (SByte))    
					return (T) (Object) primitiveSerialiser.ReadSByte ();

				if (typeof(T) == typeof (Single))   
					return (T) (Object) primitiveSerialiser.ReadSingle ();

				if (typeof(T) == typeof (UInt16))   
					return (T) (Object) primitiveSerialiser.ReadUInt16 ();

				if (typeof(T) == typeof (UInt32))   
					return (T) (Object) primitiveSerialiser.ReadUInt32 ();

				if (typeof(T) == typeof (UInt64))   
					return (T) (Object) primitiveSerialiser.ReadUInt64 ();

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

