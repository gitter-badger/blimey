using System;

namespace Oats
{
	public interface IPrimitiveSerialiser <T>
	{
		void SetData (T binary);
		T GetData ();

		void Write (Boolean value);
		void Write (Byte value);
		void Write (Char value);
		void Write (Double value);
		void Write (Int16 value);
		void Write (Int32 value);
		void Write (Int64 value);
		void Write (SByte value);
		void Write (Single value);
		void Write (UInt16 value);
		void Write (UInt32 value);
		void Write (UInt64 value);

		Boolean ReadBoolean ();
		Byte ReadByte ();
		Char ReadChar ();
		Double ReadDouble ();
		Int16 ReadInt16 ();
		Int32 ReadInt32 ();
		Int64 ReadInt64 ();
		SByte ReadSByte ();
		Single ReadSingle ();
		UInt16 ReadUInt16 ();
		UInt32 ReadUInt32 ();
		UInt64 ReadUInt64 ();
	}


}

