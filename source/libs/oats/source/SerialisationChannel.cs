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
                var isAssigned = value != null;
                streamSerialiser.Write <Boolean> (isAssigned);

				// early out if null.
                if (!isAssigned)
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
                var isAssigned = streamSerialiser.Read <Boolean> ();
				//classes can be null perhaps it's null
                if (!isAssigned)
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

