// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Blimey - Fast, efficient, high level engine built upon Cor & Abacus                                            │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │                     Brought to you by:                                                                         │ \\
// │                              _________                    .__               __                                 │ \\
// │                             /   _____/__ __  ____    ____ |__|____    _____/  |_                               │ \\
// │                             \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\                              │ \\
// │                             /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |                                │ \\
// │                            /_______  /____/|___|  /\___  /|__(____  /___|  /__|                                │ \\
// │                                    \/           \//_____/         \/     \/                                    │ \\
// │                                                                                                                │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2008-2014 A.J.Pook (http://ajpook.github.io)                                                       │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors: A.J.Pook                                                                                              │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated   │ \\
// │ documentation files (the "Software"), to deal in the Software without restriction, including without           │ \\
// │ limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sellcopies of the   │ \\
// │ Software, and to permit persons to whom the Software is furnished to do so, subject to the following           │ \\
// │ conditions:                                                                                                    │ \\
// │                                                                                                                │ \\
// │ The above copyright notice and this permission notice shall be included in all copies or substantial portions  │ \\
// │ of the Software.                                                                                               │ \\
// │                                                                                                                │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED  │ \\
// │ TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL │ \\
// │ THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF  │ \\
// │ CONTRACT, TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER        │ \\
// │ DEALINGS IN THE SOFTWARE.                                                                                      │ \\
// └────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘ \\


namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    using Fudge;
    using Abacus.SinglePrecision;
    
    using System.Linq;
    using Cor;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal static class Rgba32Extensions
    {
        public static Vector4 ToVector4 (this Rgba32 value)
        {
            return new Vector4 (
                (Single)value.R / 255f,
                (Single)value.G / 255f,
                (Single)value.B / 255f,
                (Single)value.A / 255f);
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal static class Vector4Extensions
    {
        public static Rgba32 ToRgba32 (this Vector4 value)
        {
            return new Rgba32 (
                (Byte) (value.X * 255f),
                (Byte) (value.Y * 255f),
                (Byte) (value.Z * 255f),
                (Byte) (value.W * 255f));
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
    internal static class Int32Extensions
    {
        // http://msdn.microsoft.com/en-us/library/system.object.gethashcode(v=vs.110).aspx
        public static Int32 ShiftAndWrap (this Int32 value, Int32 positions = 2)
        {
            positions = positions & 0x1F;
    
            // Save the existing bit pattern, but interpret it as an unsigned integer. 
            uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
            // Preserve the bits to be discarded. 
            uint wrapped = number >> (32 - positions);
            // Shift and wrap the discarded bits. 
            return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal static class BinaryWriterExtensions
    {
        public static void Write7BitEncodedInt32 (this BinaryWriter me, Int32 value)
        {
            throw new NotImplementedException ();
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class BinaryReaderExtensions
    {
        public static Int32 Read7BitEncodedInt32(this BinaryReader me)
        {
            Int32 result = 0;
            Int32 bitsRead = 0;
            Int32 value;

            do
            {
                value = me.ReadByte ();
                result |= (value & 0x7f) << bitsRead;
                bitsRead += 7;
            }
            while ((value & 0x80) != 0);

            return result;
        }
    }
}