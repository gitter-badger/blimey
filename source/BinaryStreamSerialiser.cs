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
using System.IO;

namespace Oats
{
	public class BinaryStreamSerialiser
		: IStreamSerialiser
		, IDisposable
	{
		BinaryWriter binaryWriter;
		BinaryReader binaryReader;

		#region IDisposable

		public void Dispose ()
		{
			if (binaryReader != null)
				binaryReader.Dispose ();

			if (binaryWriter != null)
				binaryWriter.Dispose ();
		}

		#endregion

		public void Initialise (Stream stream, ChannelMode mode)
		{
			if (mode == ChannelMode.Read)
				binaryReader = new BinaryReader (stream);

			if (mode == ChannelMode.Write)
				binaryWriter = new BinaryWriter (stream);
		}

		public Type[] SupportedTypes
		{
			get
			{
				return new []
				{
					typeof (Boolean),
					typeof (Byte),
					typeof (Char),
					typeof (Double),
					typeof (Int16),
					typeof (Int32),
					typeof (Int64),
					typeof (SByte),
					typeof (Single),
					typeof (UInt16),
					typeof (UInt32),
					typeof (UInt64),
					typeof (String),
					typeof (Decimal),
				};
			}
		}

		public void Write <T> (T value)
		{
			if (typeof (T) == typeof (Boolean))	{ binaryWriter.Write ((Boolean)(Object) value); return; }
			if (typeof (T) == typeof (Byte)) 	{ binaryWriter.Write ((Byte)(Object) value); return; }
			if (typeof (T) == typeof (Char)) 	{ binaryWriter.Write ((Char)(Object) value); return; }
			if (typeof (T) == typeof (Double)) 	{ binaryWriter.Write ((Double)(Object) value); return; }
			if (typeof (T) == typeof (Int16)) 	{ binaryWriter.Write ((Int16)(Object) value); return; }
			if (typeof (T) == typeof (Int32)) 	{ binaryWriter.Write ((Int32)(Object) value); return; }
			if (typeof (T) == typeof (Int64)) 	{ binaryWriter.Write ((Int64)(Object) value); return; }
			if (typeof (T) == typeof (SByte)) 	{ binaryWriter.Write ((SByte)(Object) value); return; }
			if (typeof (T) == typeof (Single)) 	{ binaryWriter.Write ((Single)(Object) value); return; }
			if (typeof (T) == typeof (UInt16)) 	{ binaryWriter.Write ((UInt16)(Object) value); return; }
			if (typeof (T) == typeof (UInt32)) 	{ binaryWriter.Write ((UInt32)(Object) value); return; }
			if (typeof (T) == typeof (UInt64)) 	{ binaryWriter.Write ((UInt64)(Object) value); return; }
			if (typeof (T) == typeof (String)) 	{ binaryWriter.Write ((String)(Object) value); return; }
			if (typeof (T) == typeof (Decimal))	{ binaryWriter.Write ((Decimal)(Object) value); return; }
				
			throw new SerialisationException ("Not supported, WTF!");
		}

		public T Read <T> ()
		{
			if (typeof (T) == typeof (Boolean))	{ return (T)(Object) binaryReader.ReadBoolean(); }
			if (typeof (T) == typeof (Byte)) 	{ return (T)(Object) binaryReader.ReadByte(); }
			if (typeof (T) == typeof (Char)) 	{ return (T)(Object) binaryReader.ReadChar(); }
			if (typeof (T) == typeof (Double)) 	{ return (T)(Object) binaryReader.ReadDouble(); }
			if (typeof (T) == typeof (Int16)) 	{ return (T)(Object) binaryReader.ReadInt16(); }
			if (typeof (T) == typeof (Int32)) 	{ return (T)(Object) binaryReader.ReadInt32(); }
			if (typeof (T) == typeof (Int64)) 	{ return (T)(Object) binaryReader.ReadInt64(); }
			if (typeof (T) == typeof (SByte)) 	{ return (T)(Object) binaryReader.ReadSByte(); }
			if (typeof (T) == typeof (Single)) 	{ return (T)(Object) binaryReader.ReadSingle(); }
			if (typeof (T) == typeof (UInt16)) 	{ return (T)(Object) binaryReader.ReadUInt16(); }
			if (typeof (T) == typeof (UInt32)) 	{ return (T)(Object) binaryReader.ReadUInt32(); }
			if (typeof (T) == typeof (UInt64)) 	{ return (T)(Object) binaryReader.ReadUInt64(); }
			if (typeof (T) == typeof (String)) 	{ return (T)(Object) binaryReader.ReadString(); }
			if (typeof (T) == typeof (Decimal))	{ return (T)(Object) binaryReader.ReadDecimal(); }

			throw new SerialisationException ("Not supported, WTF!");
		}
	}
}