using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Oats
{
	public class SerialisationChannel <TStreamSerialiser>
		: ISerialisationChannel
	where TStreamSerialiser
		: IStreamSerialiser
	{
		readonly SerialiserDatabase serialiserDatabase;
		readonly IStreamSerialiser streamSerialiser;
		readonly SerialisationChannelMode mode;

		public SerialisationChannelMode Mode { get { return mode; } }

		public SerialisationChannel (
			SerialiserDatabase serialiserDatabase,
			Stream stream,
			SerialisationChannelMode mode)
		{
			this.serialiserDatabase = serialiserDatabase;

			this.mode = mode;

			this.streamSerialiser = 
				(TStreamSerialiser)
				Activator.CreateInstance (
					typeof(TStreamSerialiser));

			this.streamSerialiser.Initialise (stream, mode);
		}

		public void Dispose ()
		{
			var tryDispose = new Object[] { streamSerialiser };

			foreach (Object item in tryDispose) {
				if (item != null) {
					if (item is IDisposable) {
						(item as IDisposable).Dispose ();
					}
				}
			}
		}

		public void WriteReflective (Type type, Object value)
		{
			Type thisType = typeof(SerialisationChannel <TStreamSerialiser>);
			MethodInfo mi = thisType
				.GetMethod (
					"Write");

			var gmi = mi.MakeGenericMethod(type);

			try
			{
				gmi.Invoke(this, new [] { value });
			}
			catch (Exception ex)
			{
				throw new Exception (
					"Failed to invoke Write for type [" + type + "]" +
					" with value [" + value + "]" + 
					"\n" + ex.Message + 
					"\n" + ex.InnerException.Message);
			}
		}

		public void Write <T> (T value)
		{
			if (mode != SerialisationChannelMode.Write) 
			{
				throw new SerialisationException ("This serialisation stream is for writing only.");
			}

			// Deal with nulls
			if (!typeof(T).IsValueType)
			{
				// Write some extra data
				streamSerialiser.Write <Boolean> (value != null);

				// early out if null.
				if (value == null)
					return;
			}

			if (streamSerialiser.SupportedTypes.Contains (typeof (T)))
			{
				streamSerialiser.Write <T> (value);
				return;
			}
			else
			{
				Serialiser <T> serialiser = null;
				try
				{
					// locate the correct serialiser
					serialiser = serialiserDatabase.GetSerialiser <T> ();
				}
				catch (Exception ex)
				{
					throw new SerialisationException (
						"No serialiser registed for type: " + 
						typeof (T) + " --> " + ex.Message );
				}

				//try
				//{
					serialiser.Write (this, value);
				//}
				//catch (Exception ex)
				//{
				//	throw new SerialisationException (
				//		"Failed to use serialiser " + serialiser.GetType () + 
				//		" to write: " + value + " --> " + ex.Message);
				//}

				return;
			}
		}

		public Object ReadReflective (Type type)
		{
			MethodInfo mi = typeof(SerialisationChannel <TStreamSerialiser>)
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

			if (!typeof(T).IsValueType)
			{
				//classes can be null perhaps it's null
				if (streamSerialiser.Read <Boolean> () == false)
				{
					return default (T);
				}
			}

			if (streamSerialiser.SupportedTypes.Contains (typeof (T)))
			{
				return streamSerialiser.Read <T> ();
			}
			else
			{
				Serialiser <T> serialiser = null;
				try
				{
					// locate the correct serialiser
					serialiser = serialiserDatabase.GetSerialiser <T> ();
				}
				catch (Exception)
				{
					throw new SerialisationException (
						"Failed to get serialiser for type: " + 
						typeof (T) );
				}

				try
				{
					T result = serialiser.Read (this);
					return result;
				}
				catch (Exception)
				{
					throw new SerialisationException (
						"Failed to use serialiser " + serialiser.GetType () + 
						" to read");
				}
			}
		}
	}

}

