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
		readonly ISerialiserProvider serialiserProvider;
		readonly IStreamSerialiser streamSerialiser;
		readonly ChannelMode mode;

		public ChannelMode Mode { get { return mode; } }
	
		public SerialisationChannel (
			Stream stream,
			ChannelMode mode,
			SerialiserCollection serialiserCollection = null)
		{
			if (serialiserCollection == null)
			{
				// the user has not provided their own serialiser collection
				// so we will generate them one on the fly.
                // First lets create them an empty collection.
				serialiserProvider = new AutoSerialiserProvider ();
			}
			else
			{
				this.serialiserProvider = serialiserCollection;
			}

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
				throw new SerialisationException (
					"Failed to invoke Write for type [" + type + "]" +
					" with value [" + value + "]" + 
					"\n" + ex.Message + 
					"\n" + ex.InnerException.Message);
			}
		}

		public void Write <T> (T value)
		{
			if (mode != ChannelMode.Write) 
			{
				throw new SerialisationException (
					"This serialisation stream is for writing only.");
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
					serialiser = serialiserProvider.GetSerialiser <T> ();
				}
				catch (Exception ex)
				{
					throw new SerialisationException (
						"No serialiser registered for type: " + 
						typeof (T) + " --> " + ex.Message );
				}
					
				serialiser.Write (this, value);
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
			if (mode != ChannelMode.Read)
			{
				throw new SerialisationException (
					"This serialisation stream is for reading only.");
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
					serialiser = serialiserProvider.GetSerialiser <T> ();
				}
				catch (Exception)
				{
					throw new SerialisationException (
						"Failed to get serialiser for type: " + 
						typeof (T) );
				}

				T result = serialiser.Read (this);
				return result;
			}
		}
	}

}

