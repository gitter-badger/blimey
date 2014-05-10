using System;
using System.IO;

namespace Oats
{
	public class BinaryPrimitiveSerialiser
		: IPrimitiveSerialiser <Byte[]>
		, IDisposable
	{
		MemoryStream memoryStream;
	    BinaryWriter binaryWriter;
		BinaryReader binaryReader;

		public BinaryPrimitiveSerialiser ()
		{
			memoryStream = new MemoryStream ();
			binaryWriter = new BinaryWriter (memoryStream);
			binaryReader = new BinaryReader (memoryStream);
		}

		#region IDisposable

		public void Dispose ()
		{
			if (binaryWriter != null)
				binaryWriter.Dispose ();

			if (binaryReader != null)
				binaryReader.Dispose ();

			memoryStream.Dispose ();
		}

		#endregion

		#region IPrimitiveSerialiser <Byte[]>

		public Byte[] GetData ()
		{
			Byte[] binary = memoryStream.GetBuffer ();

			return binary;
		}

		public void SetData (Byte[] binary)
		{
			memoryStream = new MemoryStream (binary);
			binaryWriter = new BinaryWriter (memoryStream);
			binaryReader = new BinaryReader (memoryStream);
		}

		public void Write (Boolean value) { binaryWriter.Write (value); }
		public void Write (Byte value) { binaryWriter.Write (value); }
		public void Write (Char value) { binaryWriter.Write (value); }
		public void Write (Double value) { binaryWriter.Write (value); }
		public void Write (Int16 value) { binaryWriter.Write (value); }
		public void Write (Int32 value) { binaryWriter.Write (value); }
		public void Write (Int64 value) { binaryWriter.Write (value); }
		public void Write (SByte value) { binaryWriter.Write (value); }
		public void Write (Single value) { binaryWriter.Write (value); }
		public void Write (UInt16 value) { binaryWriter.Write (value); }
		public void Write (UInt32 value) { binaryWriter.Write (value); }
		public void Write (UInt64 value) { binaryWriter.Write (value); }

		public Boolean ReadBoolean () { return binaryReader.ReadBoolean(); }
		public Byte ReadByte () { return binaryReader.ReadByte();; }
		public Char ReadChar () { return binaryReader.ReadChar(); }
		public Double ReadDouble () { return binaryReader.ReadDouble(); }
		public Int16 ReadInt16 () { return binaryReader.ReadInt16(); }
		public Int32 ReadInt32 () { return binaryReader.ReadInt32(); }
		public Int64 ReadInt64 () { return binaryReader.ReadInt64(); }
		public SByte ReadSByte () { return binaryReader.ReadSByte(); }
		public Single ReadSingle () { return binaryReader.ReadSingle(); }
		public UInt16 ReadUInt16 () { return binaryReader.ReadUInt16(); }
		public UInt32 ReadUInt32 () { return binaryReader.ReadUInt32(); }
		public UInt64 ReadUInt64 () { return binaryReader.ReadUInt64(); }

		#endregion
	}
}

