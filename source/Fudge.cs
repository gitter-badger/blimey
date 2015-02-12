// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ ___________        .___                                                │ \\
// │ \_   _____/_ __  __| _/ ____   ____                                    │ \\
// │  |    __)|  |  \/ __ | / ___\_/ __ \                                   │ \\
// │  |     \ |  |  / /_/ |/ /_/  >  ___/                                   │ \\
// │  \___  / |____/\____ |\___  / \___  >                                  │ \\
// │      \/             \/_____/      \/                                   │ \\
// │                                                                        │ \\
// │ A data packaging library.                                              │ \\
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
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace Fudge
{
    /// <summary>
    ///
    /// </summary>
    public interface IPackedReal
    {
        /// <summary>
        ///
        /// </summary>
        void PackFrom (Single x);

        /// <summary>
        ///
        /// </summary>
        void UnpackTo (out Single x);
    }

    /// <summary>
    ///
    /// </summary>
    public interface IPackedReal2
    {
        /// <summary>
        ///
        /// </summary>
        void PackFrom (Single x, Single y);

        /// <summary>
        ///
        /// </summary>
        void UnpackTo (out Single x, out Single y);
    }

    /// <summary>
    ///
    /// </summary>
    public interface IPackedReal3
    {
        /// <summary>
        ///
        /// </summary>
        void PackFrom (Single x, Single y, Single z);

        /// <summary>
        ///
        /// </summary>
        void UnpackTo (out Single x, out Single y, out Single z);
    }

    /// <summary>
    ///
    /// </summary>
    public interface IPackedReal4
    {
        /// <summary>
        ///
        /// </summary>
        void PackFrom (Single x, Single y, Single z, Single w);

        /// <summary>
        ///
        /// </summary>
        void UnpackTo (out Single x, out Single y, out Single z, out Single w);
    }

    /// <summary>
    /// T is the type that the value is packed into
    /// </summary>
    public interface IPackedValue<T>
    {
        /// <summary>
        /// todo
        /// </summary>
        T PackedValue { get; set; }
    }

    /// <summary>
    /// todo
    /// </summary>
    internal static class PackUtils
    {
        /// <summary>
        /// todo
        /// </summary>
        static Double ClampAndRound (Single value, Single min, Single max)
        {
            if (Single.IsNaN (value)) 
            {
                return 0.0;
            }

            if (Single.IsInfinity (value))
            {
                return (Single.IsNegativeInfinity (value) ? ((Double)min) : ((Double)max));
            }

            if (value < min)
            {
                return (Double)min;
            }

            if (value > max)
            {
                return (Double)max;
            }

            return Math.Round ((Double)value);
        }

        /// <summary>
        /// todo
        /// </summary>
        internal static UInt32 PackSigned (UInt32 bitmask, Single value)
        {
            Single max = bitmask >> 1;
            Single min = -max - 1f;
            return (((UInt32)((Int32)ClampAndRound (value, min, max))) & bitmask);
        }

        /// <summary>
        /// todo
        /// </summary>
        internal static UInt32 PackUnsigned (Single bitmask, Single value)
        {
            return (UInt32)ClampAndRound (value, 0f, bitmask);
        }

        /// <summary>
        /// todo
        /// </summary>
        internal static UInt32 PackSignedNormalised (UInt32 bitmask, Single value)
        {
            if (value > 1f || value < 0f)
                throw new ArgumentException ("Input value must be normalised.");

            Single max = bitmask >> 1;
            value *= max;
            UInt32 result = (((UInt32)((Int32)ClampAndRound (value, -max, max))) & bitmask);
            return result;
        }

        /// <summary>
        /// todo
        /// </summary>
        internal static Single UnpackSignedNormalised (UInt32 bitmask, UInt32 value)
        {
            UInt32 num = (UInt32)((bitmask + 1) >> 1);

            if ((value & num) != 0)
            {
                if ((value & bitmask) == num)
                {
                    return -1f;
                }

                value |= ~bitmask;
            }
            else
            {
                value &= bitmask;
            }

            Single num2 = bitmask >> 1;

            Single result = (((Single)value) / num2);

            if (result > 1f || result < 0f)
                throw new ArgumentException ("Input value does not yield a normalised result.");

            return result;
        }

        /// <summary>
        /// todo
        /// </summary>
        internal static UInt32 PackUnsignedNormalisedValue (Single bitmask, Single value)
        {
            if (value > 1f || value < 0f)
                throw new ArgumentException ("Input value must be normalised.");

            value *= bitmask;
            UInt32 result = (UInt32)ClampAndRound (value, 0f, bitmask);
            return result;
        }
        
        /// <summary>
        /// todo
        /// </summary>
        internal static Single UnpackUnsignedNormalisedValue (UInt32 bitmask, UInt32 value)
        {
            value &= bitmask;
            Single result = (((Single)value) / ((Single)bitmask));

            if (result > 1f || result < 0f)
                throw new ArgumentException ("Input value does not yield a normalised result.");

            return result;
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct Alpha_8
        : IPackedValue<Byte>
        , IEquatable<Alpha_8>
        , IPackedReal
    {
        /// <summary>
        /// todo
        /// </summary>
        public override String ToString()
        {
            return this.packedValue.ToString("X2", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Pack(Single zAlpha, out Byte zPackedAlpha)
        {
            if (zAlpha < 0f || zAlpha > 1f)
            {
                throw new ArgumentException ("A component of the input source is not unsigned and normalised.");
            }

            zPackedAlpha = (Byte)PackUtils.PackUnsignedNormalisedValue(255f, zAlpha);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Unpack(Byte zPackedAlpha, out Single zAlpha)
        {
            zAlpha = PackUtils.UnpackUnsignedNormalisedValue(0xff, zPackedAlpha);

            if (zAlpha < 0f || zAlpha > 1f)
            {
                throw new Exception ("A the input source doesn't yeild an unsigned normalised output: " + zPackedAlpha);
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        Byte packedValue;

        #region IPackedValue

        /// <summary>
        /// todo
        /// </summary>
        [CLSCompliant (true)]
        public Byte PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            return ((obj is Alpha_8) && this.Equals((Alpha_8)obj));
        }

        #region IEquatable<Alpha_8>

        /// <summary>
        /// todo
        /// </summary>
        public Boolean Equals(Alpha_8 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator ==(Alpha_8 a, Alpha_8 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator !=(Alpha_8 a, Alpha_8 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public Alpha_8(Single zRealAlpha)
        {
            Pack (zRealAlpha, out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void PackFrom (Single zRealAlpha)
        {
            Pack (zRealAlpha, out this.packedValue);
        }

        /// <summary>
        ///
        /// </summary>
        public void UnpackTo (out Single zRealAlpha)
        {
            Unpack (this.packedValue, out zRealAlpha);
        }

    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout (LayoutKind.Sequential), Serializable]
    public struct Bgr_5_6_5
        : IPackedValue<UInt16>
        , IEquatable<Bgr_5_6_5>
        , IPackedReal3
    {
        /// <summary>
        /// todo
        /// </summary>
        public override String ToString ()
        {
            return this.packedValue.ToString ("X4", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Pack(Single zB, Single zG, Single zR, out UInt16 zPackedBgr)
        {
            if (zR < 0f || zR > 1f || zG < 0f || zG > 1f || zB < 0f || zB > 1f)
            {
                throw new ArgumentException ("A component of the input source is not unsigned and normalised.");
            }

            UInt32 b = PackUtils.PackUnsignedNormalisedValue(31f, zB);
            UInt32 g = PackUtils.PackUnsignedNormalisedValue(63f, zG) << 5;
            UInt32 r = PackUtils.PackUnsignedNormalisedValue(31f, zR) << 11;

            zPackedBgr = (UInt16)((r | g) | b);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Unpack(UInt16 zPackedBgr, out Single zB, out Single zG, out Single zR)
        {
            zB = PackUtils.UnpackUnsignedNormalisedValue(0x1f, zPackedBgr);
            zG = PackUtils.UnpackUnsignedNormalisedValue(0x3f, (UInt32)(zPackedBgr >> 5));
            zR = PackUtils.UnpackUnsignedNormalisedValue(0x1f, (UInt32)(zPackedBgr >> 11));

            if (zR < 0f || zR > 1f || zG < 0f || zG > 1f || zB < 0f || zB > 1f)
            {
                throw new Exception ("A the input source doesn't yeild an unsigned normalised output: " + zPackedBgr);
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        UInt16 packedValue;

        #region IPackedValue

        /// <summary>
        /// todo
        /// </summary>
        [CLSCompliant (false)]
        public UInt16 PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            return ((obj is Bgr_5_6_5) && this.Equals((Bgr_5_6_5)obj));
        }

        #region IEquatable<Bgr_5_6_5>

        /// <summary>
        /// todo
        /// </summary>
        public Boolean Equals(Bgr_5_6_5 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator ==(Bgr_5_6_5 a, Bgr_5_6_5 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator !=(Bgr_5_6_5 a, Bgr_5_6_5 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public Bgr_5_6_5(
            Single zB,
            Single zG,
            Single zR)
        {
            Pack(
                zB,
                zG,
                zR,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void PackFrom (
            Single zB,
            Single zG,
            Single zR)
        {
            Pack(
                zB,
                zG,
                zR,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void UnpackTo (
            out Single zB,
            out Single zG,
            out Single zR)
        {
            Unpack(
                this.packedValue,
                out zB,
                out zG,
                out zR);
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout (LayoutKind.Sequential), Serializable]
    public struct Bgra16
        : IPackedValue<UInt16>
        , IEquatable<Bgra16>
        , IPackedReal4
    {
        /// <summary>
        /// todo
        /// </summary>
        public override String ToString ()
        {
            return this.packedValue.ToString ("X4", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Pack(Single zB, Single zG, Single zR, Single zA, out UInt16 zPackedBgra)
        {
            if (zR < 0f || zR > 1f || zG < 0f || zG > 1f || zB < 0f || zB > 1f || zA < 0f || zA > 1f )
            {
                throw new ArgumentException ("A component of the input source is not unsigned and normalised.");
            }

            UInt32 b = PackUtils.PackUnsignedNormalisedValue (15f, zB);
            UInt32 g = PackUtils.PackUnsignedNormalisedValue (15f, zG) << 4;
            UInt32 r = PackUtils.PackUnsignedNormalisedValue (15f, zR) << 8;
            UInt32 a = PackUtils.PackUnsignedNormalisedValue (15f, zA) << 12;

            zPackedBgra = (UInt16)(((r | g) | b) | a);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Unpack(UInt16 zPackedBgra, out Single zB, out Single zG, out Single zR, out Single zA)
        {
            zB = PackUtils.UnpackUnsignedNormalisedValue (15, zPackedBgra);
            zG = PackUtils.UnpackUnsignedNormalisedValue (15, (UInt32)(zPackedBgra >> 4));
            zR = PackUtils.UnpackUnsignedNormalisedValue (15, (UInt32)(zPackedBgra >> 8));
            zA = PackUtils.UnpackUnsignedNormalisedValue (15, (UInt32)(zPackedBgra >> 12));

            if (zR < 0f || zR > 1f || zG < 0f || zG > 1f || zB < 0f || zB > 1f || zA < 0f || zA > 1f )
            {
                throw new Exception ("A the input source doesn't yeild an unsigned normalised output: " + zPackedBgra);
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        UInt16 packedValue;

        #region IPackedValue

        /// <summary>
        /// todo
        /// </summary>
        [CLSCompliant (false)]
        public UInt16 PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            return ((obj is Bgra16) && this.Equals((Bgra16)obj));
        }

        #region IEquatable<Bgra16>

        /// <summary>
        /// todo
        /// </summary>
        public Boolean Equals(Bgra16 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator ==(Bgra16 a, Bgra16 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator !=(Bgra16 a, Bgra16 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public Bgra16(
            Single zB,
            Single zG,
            Single zR,
            Single zA)
        {
            Pack(
                zB,
                zG,
                zR,
                zA,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void PackFrom (
            Single zB,
            Single zG,
            Single zR,
            Single zA)
        {
            Pack(
                zB,
                zG,
                zR,
                zA,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void UnpackTo (
            out Single zB,
            out Single zG,
            out Single zR,
            out Single zA)
        {
            Unpack(
                this.packedValue,
                out zB,
                out zG,
                out zR,
                out zA);
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout (LayoutKind.Sequential), Serializable]
    public struct Bgra_5_5_5_1
        : IPackedValue<UInt16>
        , IEquatable<Bgra_5_5_5_1>
        , IPackedReal4
    {
        /// <summary>
        /// todo
        /// </summary>
        public override String ToString ()
        {
            return this.packedValue.ToString ("X4", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Pack(Single zB, Single zG, Single zR, Single zA, out UInt16 zPackedBgra)
        {
            if (zR < 0f || zR > 1f || zG < 0f || zG > 1f || zB < 0f || zB > 1f || zA < 0f || zA > 1f )
            {
                throw new ArgumentException ("A component of the input source is not unsigned and normalised.");
            }

            UInt32 b = PackUtils.PackUnsignedNormalisedValue (31f, zB);
            UInt32 g = PackUtils.PackUnsignedNormalisedValue (31f, zG) << 5;
            UInt32 r = PackUtils.PackUnsignedNormalisedValue (31f, zR) << 10;
            UInt32 a = PackUtils.PackUnsignedNormalisedValue (1f, zA) << 15;

            zPackedBgra = (UInt16)(((r | g) | b) | a);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Unpack(UInt16 zPackedBgra, out Single zB, out Single zG, out Single zR, out Single zA)
        {
            zB = PackUtils.UnpackUnsignedNormalisedValue (0x1f, zPackedBgra);
            zG = PackUtils.UnpackUnsignedNormalisedValue (0x1f, (UInt32)(zPackedBgra >> 5));
            zR = PackUtils.UnpackUnsignedNormalisedValue (0x1f, (UInt32)(zPackedBgra >> 10));
            zA = PackUtils.UnpackUnsignedNormalisedValue (1, (UInt32)(zPackedBgra >> 15));

            if (zR < 0f || zR > 1f || zG < 0f || zG > 1f || zB < 0f || zB > 1f || zA < 0f || zA > 1f)
            {
                throw new Exception ("A the input source doesn't yeild an unsigned normalised output: " + zPackedBgra);
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        UInt16 packedValue;

        #region IPackedValue

        /// <summary>
        /// todo
        /// </summary>
        [CLSCompliant (false)]
        public UInt16 PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            return ((obj is Bgra_5_5_5_1) && this.Equals((Bgra_5_5_5_1)obj));
        }

        #region IEquatable<Bgra_5_5_5_1>

        /// <summary>
        /// todo
        /// </summary>
        public Boolean Equals(Bgra_5_5_5_1 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator ==(Bgra_5_5_5_1 a, Bgra_5_5_5_1 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator !=(Bgra_5_5_5_1 a, Bgra_5_5_5_1 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public Bgra_5_5_5_1(
            Single zB,
            Single zG,
            Single zR,
            Single zA)
        {
            Pack(
                zB,
                zG,
                zR,
                zA,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void PackFrom (
            Single zB,
            Single zG,
            Single zR,
            Single zA)
        {
            Pack(
                zB,
                zG,
                zR,
                zA,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void UnpackTo (
            out Single zB,
            out Single zG,
            out Single zR,
            out Single zA)
        {
            Unpack(
                this.packedValue,
                out zB,
                out zG,
                out zR,
                out zA);
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout (LayoutKind.Sequential), Serializable]
    public struct Byte4
        : IPackedValue<UInt32>
        , IEquatable<Byte4>
    {
        /// <summary>
        /// todo
        /// </summary>
        public override String ToString ()
        {
            return this.packedValue.ToString ("X8", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Pack(Single zX, Single zY, Single zZ, Single zW, out UInt32 zPackedXyzw)
        {
            UInt32 y = PackUtils.PackUnsigned (255f, zX);
            UInt32 x = PackUtils.PackUnsigned (255f, zY) << 8;
            UInt32 z = PackUtils.PackUnsigned (255f, zZ) << 0x10;
            UInt32 w = PackUtils.PackUnsigned (255f, zW) << 0x18;

            zPackedXyzw = (UInt32)(((y | x) | z) | w);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Unpack(UInt32 zPackedXyzw, out Single zX, out Single zY, out Single zZ, out Single zW)
        {
            zX = zPackedXyzw & 0xff;
            zY = (zPackedXyzw >> 8) & 0xff;
            zZ = (zPackedXyzw >> 0x10) & 0xff;
            zW = (zPackedXyzw >> 0x18) & 0xff;
        }

        /// <summary>
        /// todo
        /// </summary>
        UInt32 packedValue;

        #region IPackedValue

        /// <summary>
        /// todo
        /// </summary>
        [CLSCompliant (false)]
        public UInt32 PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            return ((obj is Byte4) && this.Equals((Byte4)obj));
        }

        #region IEquatable<Byte4>

        /// <summary>
        /// todo
        /// </summary>
        public Boolean Equals(Byte4 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator ==(Byte4 a, Byte4 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator !=(Byte4 a, Byte4 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public Byte4(
            Single zX,
            Single zY,
            Single zZ,
            Single zW)
        {
            Pack(
                zX,
                zY,
                zZ,
                zW,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void PackFrom (
            Single zX,
            Single zY,
            Single zZ,
            Single zW)
        {
            Pack(
                zX,
                zY,
                zZ,
                zW,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void UnpackTo (
            out Single zX,
            out Single zY,
            out Single zZ,
            out Single zW)
        {
            Unpack(
                this.packedValue,
                out zX,
                out zY,
                out zZ,
                out zW);
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout (LayoutKind.Sequential), Serializable]
    public struct NormalisedByte2
        : IPackedValue<UInt16>
        , IEquatable<NormalisedByte2>
        , IPackedReal2
    {
        /// <summary>
        /// todo
        /// </summary>
        public override String ToString ()
        {
            return this.packedValue.ToString ("X4", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Pack(Single zX, Single zY, out UInt16 zPackedXy)
        {
            if (zX < -1f || zX > 1f || zY < -1f || zY > 1f)
            {
                throw new ArgumentException ("A component of the input source is not normalised.");
            }

            UInt32 x = PackUtils.PackSignedNormalised(0xff, zX);
            UInt32 y = PackUtils.PackSignedNormalised(0xff, zY) << 8;

            zPackedXy = (UInt16)(x | y);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Unpack(UInt16 zPackedXy, out Single zX, out Single zY)
        {
            zX = PackUtils.UnpackSignedNormalised (0xff, zPackedXy);
            zY = PackUtils.UnpackSignedNormalised (0xff, (UInt32) (zPackedXy >> 8));

            if (zX < -1f || zX > 1f || zY < -1f || zY > 1f)
            {
                throw new Exception ("A the input source doesn't yeild a normalised output: " + zPackedXy);
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        UInt16 packedValue;

        #region IPackedValue

        /// <summary>
        /// todo
        /// </summary>
        [CLSCompliant (false)]
        public UInt16 PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            return ((obj is NormalisedByte2) && this.Equals((NormalisedByte2)obj));
        }

        #region IEquatable<NormalisedByte2>

        /// <summary>
        /// todo
        /// </summary>
        public Boolean Equals(NormalisedByte2 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator ==(NormalisedByte2 a, NormalisedByte2 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator !=(NormalisedByte2 a, NormalisedByte2 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public NormalisedByte2(
            Single zX,
            Single zY)
        {
            Pack(
                zX,
                zY,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void PackFrom (
            Single zX,
            Single zY)
        {
            Pack(
                zX,
                zY,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void UnpackTo (
            out Single zX,
            out Single zY)
        {
            Unpack(
                this.packedValue,
                out zX,
                out zY);
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout (LayoutKind.Sequential), Serializable]
    public struct NormalisedByte4
        : IPackedValue<UInt32>
        , IEquatable<NormalisedByte4>
        , IPackedReal4
    {
        /// <summary>
        /// todo
        /// </summary>
        public override String ToString ()
        {
            return this.packedValue.ToString ("X8", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Pack(Single zX, Single zY, Single zZ, Single zW, out UInt32 zPackedXyzw)
        {
            if (zX < -1f || zX > 1f || zY < -1f || zY > 1f || zZ < -1f || zZ > 1f || zW < -1f || zW > 1f)
            {
                throw new ArgumentException ("A component of the input source is not normalised.");
            }

            UInt32 x = PackUtils.PackSignedNormalised (0xff, zX);
            UInt32 y = PackUtils.PackSignedNormalised (0xff, zY) << 8;
            UInt32 z = PackUtils.PackSignedNormalised (0xff, zZ) << 16;
            UInt32 w = PackUtils.PackSignedNormalised (0xff, zW) << 24;

            zPackedXyzw = (((x | y) | z) | w);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Unpack(UInt32 zPackedXyzw, out Single zX, out Single zY, out Single zZ, out Single zW)
        {
            zX = PackUtils.UnpackSignedNormalised (0xff, zPackedXyzw);
            zY = PackUtils.UnpackSignedNormalised (0xff, (UInt32) (zPackedXyzw >> 8));
            zZ = PackUtils.UnpackSignedNormalised (0xff, (UInt32) (zPackedXyzw >> 16));
            zW = PackUtils.UnpackSignedNormalised (0xff, (UInt32) (zPackedXyzw >> 24));

            if (zX < -1f || zX > 1f || zY < -1f || zY > 1f || zZ < -1f || zZ > 1f || zW < -1f || zW > 1f)
            {
                throw new Exception ("A the input source doesn't yeild a normalised output: " + zPackedXyzw);
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        UInt32 packedValue;

        #region IPackedValue

        /// <summary>
        /// todo
        /// </summary>
        [CLSCompliant (false)]
        public UInt32 PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            return ((obj is NormalisedByte4) && this.Equals((NormalisedByte4)obj));
        }

        #region IEquatable<NormalisedByte4>

        /// <summary>
        /// todo
        /// </summary>
        public Boolean Equals(NormalisedByte4 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator ==(NormalisedByte4 a, NormalisedByte4 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator !=(NormalisedByte4 a, NormalisedByte4 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public NormalisedByte4(
            Single zX,
            Single zY,
            Single zZ,
            Single zW)
        {
            Pack(
                zX,
                zY,
                zZ,
                zW,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void PackFrom (
            Single zX,
            Single zY,
            Single zZ,
            Single zW)
        {
            Pack(
                zX,
                zY,
                zZ,
                zW,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void UnpackTo (
            out Single zX,
            out Single zY,
            out Single zZ,
            out Single zW)
        {
            Unpack(
                this.packedValue,
                out zX,
                out zY,
                out zZ,
                out zW);
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout (LayoutKind.Sequential), Serializable]
    public struct NormalisedShort2
        : IPackedValue<UInt32>
        , IEquatable<NormalisedShort2>
        , IPackedReal2
    {
        /// <summary>
        /// todo
        /// </summary>
        public override String ToString ()
        {
            return this.packedValue.ToString ("X8", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Pack(Single zX, Single zY, out UInt32 zPackedXy)
        {
            if (zX < -1f || zX > 1f || zY < -1f || zY > 1f)
            {
                throw new ArgumentException ("A component of the input source is not normalised.");
            }

            UInt32 x = PackUtils.PackSignedNormalised(0xffff, zX);
            UInt32 y = PackUtils.PackSignedNormalised(0xffff, zY) << 16;

            zPackedXy = (x | y);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Unpack(UInt32 zPackedXy, out Single zX, out Single zY)
        {
            zX = PackUtils.UnpackSignedNormalised (0xffff, zPackedXy);
            zY = PackUtils.UnpackSignedNormalised (0xffff, (UInt32) (zPackedXy >> 16));

            if (zX < -1f || zX > 1f || zY < -1f || zY > 1f)
            {
                throw new Exception ("A the input source doesn't yeild a normalised output: " + zPackedXy);
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        UInt32 packedValue;

        #region IPackedValue

        /// <summary>
        /// todo
        /// </summary>
        [CLSCompliant (false)]
        public UInt32 PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            return ((obj is NormalisedShort2) && this.Equals((NormalisedShort2)obj));
        }

        #region IEquatable<NormalisedShort2>

        /// <summary>
        /// todo
        /// </summary>
        public Boolean Equals(NormalisedShort2 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator ==(NormalisedShort2 a, NormalisedShort2 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator !=(NormalisedShort2 a, NormalisedShort2 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public NormalisedShort2(
            Single zX,
            Single zY)
        {
            Pack(
                zX,
                zY,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void PackFrom (
            Single zX,
            Single zY)
        {
            Pack(
                zX,
                zY,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void UnpackTo (
            out Single zX,
            out Single zY)
        {
            Unpack(
                this.packedValue,
                out zX,
                out zY);
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout (LayoutKind.Sequential), Serializable]
    public struct NormalisedShort4
        : IPackedValue<UInt64>
        , IEquatable<NormalisedShort4>
        , IPackedReal4
    {
        /// <summary>
        /// todo
        /// </summary>
        public override String ToString ()
        {
            return this.packedValue.ToString ("X8", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Pack(Single zX, Single zY, Single zZ, Single zW, out UInt64 zPackedXyzw)
        {
            if (zX < -1f || zX > 1f || zY < -1f || zY > 1f || zZ < -1f || zZ > 1f || zW < -1f || zW > 1f )
            {
                throw new ArgumentException ("A component of the input source is not normalised.");
            }

            UInt64 x = (UInt64) PackUtils.PackSignedNormalised(0xffff, zX);
            UInt64 y = ((UInt64) PackUtils.PackSignedNormalised(0xffff, zY)) << 16;
            UInt64 z = ((UInt64) PackUtils.PackSignedNormalised(0xffff, zZ)) << 32;
            UInt64 w = ((UInt64) PackUtils.PackSignedNormalised(0xffff, zW)) << 48;

            zPackedXyzw = (((x | y) | z) | w);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Unpack(UInt64 zPackedXyzw, out Single zX, out Single zY, out Single zZ, out Single zW)
        {
            zX = PackUtils.UnpackSignedNormalised (0xffff, (UInt32) zPackedXyzw);
            zY = PackUtils.UnpackSignedNormalised (0xffff, (UInt32) (zPackedXyzw >> 16));
            zZ = PackUtils.UnpackSignedNormalised (0xffff, (UInt32) (zPackedXyzw >> 32));
            zW = PackUtils.UnpackSignedNormalised (0xffff, (UInt32) (zPackedXyzw >> 48));

            if (zX < -1f || zX > 1f || zY < -1f || zY > 1f || zZ < -1f || zZ > 1f || zW < -1f || zW > 1f )
            {
                throw new Exception ("A the input source doesn't yeild a normalised output: " + zPackedXyzw);
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        UInt64 packedValue;

        #region IPackedValue

        /// <summary>
        /// todo
        /// </summary>
        [CLSCompliant (false)]
        public UInt64 PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            return ((obj is NormalisedShort4) && this.Equals((NormalisedShort4)obj));
        }

        #region IEquatable<NormalisedShort4>

        /// <summary>
        /// todo
        /// </summary>
        public Boolean Equals(NormalisedShort4 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator ==(NormalisedShort4 a, NormalisedShort4 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator !=(NormalisedShort4 a, NormalisedShort4 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public NormalisedShort4(
            Single zX,
            Single zY,
            Single zZ,
            Single zW)
        {
            Pack(
                zX,
                zY,
                zZ,
                zW,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void PackFrom (
            Single zX,
            Single zY,
            Single zZ,
            Single zW)
        {
            Pack(
                zX,
                zY,
                zZ,
                zW,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void UnpackTo (
            out Single zX,
            out Single zY,
            out Single zZ,
            out Single zW)
        {
            Unpack(
                this.packedValue,
                out zX,
                out zY,
                out zZ,
                out zW);
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout (LayoutKind.Sequential), Serializable]
    public struct Rg32
        : IPackedValue<UInt32>
        , IEquatable<Rg32>
        , IPackedReal2
    {
        /// <summary>
        /// todo
        /// </summary>
        public override String ToString ()
        {
            return this.packedValue.ToString ("X8", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Pack(Single zR, Single zG, out UInt32 zPackedRg)
        {
            if (zR < -1f || zR > 1f || zG < -1f || zG > 1f)
            {
                throw new ArgumentException ("A component of the input source is not normalised.");
            }

            UInt32 x = PackUtils.PackUnsignedNormalisedValue(0xffff, zR);
            UInt32 y = PackUtils.PackUnsignedNormalisedValue(0xffff, zG) << 16;

            zPackedRg = (x | y);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Unpack(UInt32 zPackedRg, out Single zR, out Single zG)
        {
            zR = PackUtils.UnpackUnsignedNormalisedValue (0xffff, zPackedRg);
            zG = PackUtils.UnpackUnsignedNormalisedValue (0xffff, (UInt32) (zPackedRg >> 16));

            if (zR < -1f || zR > 1f || zG < -1f || zG > 1f)
            {
                throw new Exception ("A the input source doesn't yeild a normalised output: " + zPackedRg);
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        UInt32 packedValue;

        #region IPackedValue

        /// <summary>
        /// todo
        /// </summary>
        [CLSCompliant (false)]
        public UInt32 PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            return ((obj is Rg32) && this.Equals((Rg32)obj));
        }

        #region IEquatable<Rg32>

        /// <summary>
        /// todo
        /// </summary>
        public Boolean Equals(Rg32 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator ==(Rg32 a, Rg32 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator !=(Rg32 a, Rg32 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public Rg32(
            Single zR,
            Single zG)
        {
            Pack(
                zR,
                zG,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void PackFrom (
            Single zR,
            Single zG)
        {
            Pack(
                zR,
                zG,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void UnpackTo (
            out Single zR,
            out Single zG)
        {
            Unpack(
                this.packedValue,
                out zR,
                out zG);
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout (LayoutKind.Sequential), Serializable]
    public partial struct Rgba32
        : IPackedValue<UInt32>
        , IEquatable<Rgba32>
        , IPackedReal4
    {
        /// <summary>
        /// todo
        /// </summary>
        public override String ToString ()
        {
            return string.Format ("{{R:{0} G:{1} B:{2} A:{3}}}", new Object[] { this.R, this.G, this.B, this.A });
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Pack(Single zR, Single zG, Single zB, Single zA, out UInt32 zPackedRgba)
        {
            if (zR < 0f || zR > 1f || zG < 0f || zG > 1f || zB < 0f || zB > 1f || zA < 0f || zA > 1f )
            {
                throw new ArgumentException ("A component of the input source is not unsigned and normalised.");
            }

            UInt32 r = PackUtils.PackUnsignedNormalisedValue (0xff, zR);
            UInt32 g = PackUtils.PackUnsignedNormalisedValue (0xff, zG) << 8;
            UInt32 b = PackUtils.PackUnsignedNormalisedValue (0xff, zB) << 16;
            UInt32 a = PackUtils.PackUnsignedNormalisedValue (0xff, zA) << 24;

            zPackedRgba = ((r | g) | b) | a;
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Unpack(UInt32 zPackedRgba, out Single zR, out Single zG, out Single zB, out Single zA)
        {
            zR = PackUtils.UnpackUnsignedNormalisedValue (0xff, zPackedRgba);
            zG = PackUtils.UnpackUnsignedNormalisedValue (0xff, (UInt32)(zPackedRgba >> 8));
            zB = PackUtils.UnpackUnsignedNormalisedValue (0xff, (UInt32)(zPackedRgba >> 16));
            zA = PackUtils.UnpackUnsignedNormalisedValue (0xff, (UInt32)(zPackedRgba >> 24));

            if (zR < 0f || zR > 1f || zG < 0f || zG > 1f || zB < 0f || zB > 1f || zA < 0f || zA > 1f )
            {
                throw new Exception ("A the input source doesn't yeild an unsigned normalised output: " + zPackedRgba);
            }
        }

        /// <summary>
        /// Transparent
        /// </summary>
        public static Rgba32 Transparent
        {
            get { return new Rgba32 (0); }
        }

        /// <summary>
        /// AliceBlue
        /// </summary>
        public static Rgba32 AliceBlue
        {
            get { return new Rgba32 (4294965488); }
        }

        /// <summary>
        /// AntiqueWhite
        /// </summary>
        public static Rgba32 AntiqueWhite
        {
            get { return new Rgba32 (4292340730); }
        }

        /// <summary>
        /// Aqua
        /// </summary>
        public static Rgba32 Aqua
        {
            get { return new Rgba32 (4294967040); }
        }

        /// <summary>
        /// Aquamarine
        /// </summary>
        public static Rgba32 Aquamarine
        {
            get { return new Rgba32 (4292149119); }
        }

        /// <summary>
        /// Azure
        /// </summary>
        public static Rgba32 Azure
        {
            get { return new Rgba32 (4294967280); }
        }

        /// <summary>
        /// Beige
        /// </summary>
        public static Rgba32 Beige
        {
            get { return new Rgba32 (4292670965); }
        }

        /// <summary>
        /// Bisque
        /// </summary>
        public static Rgba32 Bisque
        {
            get { return new Rgba32 (4291093759); }
        }

        /// <summary>
        /// Black
        /// </summary>
        public static Rgba32 Black
        {
            get { return new Rgba32 (4278190080); }
        }

        /// <summary>
        /// BlanchedAlmond
        /// </summary>
        public static Rgba32 BlanchedAlmond
        {
            get { return new Rgba32 (4291685375); }
        }

        /// <summary>
        /// Blue
        /// </summary>
        public static Rgba32 Blue
        {
            get { return new Rgba32 (4294901760); }
        }

        /// <summary>
        /// BlueViolet
        /// </summary>
        public static Rgba32 BlueViolet
        {
            get { return new Rgba32 (4293012362); }
        }

        /// <summary>
        /// Brown
        /// </summary>
        public static Rgba32 Brown
        {
            get { return new Rgba32 (4280953509); }
        }

        /// <summary>
        /// BurlyWood
        /// </summary>
        public static Rgba32 BurlyWood
        {
            get { return new Rgba32 (4287084766); }
        }

        /// <summary>
        /// CadetBlue
        /// </summary>
        public static Rgba32 CadetBlue
        {
            get { return new Rgba32 (4288716383); }
        }

        /// <summary>
        /// Chartreuse
        /// </summary>
        public static Rgba32 Chartreuse
        {
            get { return new Rgba32 (4278255487); }
        }

        /// <summary>
        /// Chocolate
        /// </summary>
        public static Rgba32 Chocolate
        {
            get { return new Rgba32 (4280183250); }
        }

        /// <summary>
        /// Coral
        /// </summary>
        public static Rgba32 Coral
        {
            get { return new Rgba32 (4283465727); }
        }

        /// <summary>
        /// CornflowerBlue
        /// </summary>
        public static Rgba32 CornflowerBlue
        {
            get { return new Rgba32 (4293760356); }
        }

        /// <summary>
        /// Cornsilk
        /// </summary>
        public static Rgba32 Cornsilk
        {
            get { return new Rgba32 (4292671743); }
        }

        /// <summary>
        /// Crimson
        /// </summary>
        public static Rgba32 Crimson
        {
            get { return new Rgba32 (4282127580); }
        }

        /// <summary>
        /// Cyan
        /// </summary>
        public static Rgba32 Cyan
        {
            get { return new Rgba32 (4294967040); }
        }

        /// <summary>
        /// DarkBlue
        /// </summary>
        public static Rgba32 DarkBlue
        {
            get { return new Rgba32 (4287299584); }
        }

        /// <summary>
        /// DarkCyan
        /// </summary>
        public static Rgba32 DarkCyan
        {
            get { return new Rgba32 (4287335168); }
        }

        /// <summary>
        /// DarkGoldenrod
        /// </summary>
        public static Rgba32 DarkGoldenrod
        {
            get { return new Rgba32 (4278945464); }
        }

        /// <summary>
        /// DarkGrey
        /// </summary>
        public static Rgba32 DarkGrey
        {
            get { return new Rgba32 (4289309097); }
        }

        /// <summary>
        /// DarkGreen
        /// </summary>
        public static Rgba32 DarkGreen
        {
            get { return new Rgba32 (4278215680); }
        }

        /// <summary>
        /// DarkKhaki
        /// </summary>
        public static Rgba32 DarkKhaki
        {
            get { return new Rgba32 (4285249469); }
        }

        /// <summary>
        /// DarkMagenta
        /// </summary>
        public static Rgba32 DarkMagenta
        {
            get { return new Rgba32 (4287299723); }
        }

        /// <summary>
        /// DarkOliveGreen
        /// </summary>
        public static Rgba32 DarkOliveGreen
        {
            get { return new Rgba32 (4281297749); }
        }

        /// <summary>
        /// DarkOrange
        /// </summary>
        public static Rgba32 DarkOrange
        {
            get { return new Rgba32 (4278226175); }
        }

        /// <summary>
        /// DarkOrchid
        /// </summary>
        public static Rgba32 DarkOrchid
        {
            get { return new Rgba32 (4291572377); }
        }

        /// <summary>
        /// DarkRed
        /// </summary>
        public static Rgba32 DarkRed
        {
            get { return new Rgba32 (4278190219); }
        }

        /// <summary>
        /// DarkSalmon
        /// </summary>
        public static Rgba32 DarkSalmon
        {
            get { return new Rgba32 (4286224105); }
        }

        /// <summary>
        /// DarkSeaGreen
        /// </summary>
        public static Rgba32 DarkSeaGreen
        {
            get { return new Rgba32 (4287347855); }
        }

        /// <summary>
        /// DarkSlateBlue
        /// </summary>
        public static Rgba32 DarkSlateBlue
        {
            get { return new Rgba32 (4287315272); }
        }

        /// <summary>
        /// DarkSlateGrey
        /// </summary>
        public static Rgba32 DarkSlateGrey
        {
            get { return new Rgba32 (4283387695); }
        }

        /// <summary>
        /// DarkTurquoise
        /// </summary>
        public static Rgba32 DarkTurquoise
        {
            get { return new Rgba32 (4291939840); }
        }

        /// <summary>
        /// DarkViolet
        /// </summary>
        public static Rgba32 DarkViolet
        {
            get { return new Rgba32 (4292018324); }
        }

        /// <summary>
        /// DeepPink
        /// </summary>
        public static Rgba32 DeepPink
        {
            get { return new Rgba32 (4287829247); }
        }

        /// <summary>
        /// DeepSkyBlue
        /// </summary>
        public static Rgba32 DeepSkyBlue
        {
            get { return new Rgba32 (4294950656); }
        }

        /// <summary>
        /// DimGrey
        /// </summary>
        public static Rgba32 DimGrey
        {
            get { return new Rgba32 (4285098345); }
        }

        /// <summary>
        /// DodgerBlue
        /// </summary>
        public static Rgba32 DodgerBlue
        {
            get { return new Rgba32 (4294938654); }
        }

        /// <summary>
        /// Firebrick
        /// </summary>
        public static Rgba32 Firebrick
        {
            get { return new Rgba32 (4280427186); }
        }

        /// <summary>
        /// FloralWhite
        /// </summary>
        public static Rgba32 FloralWhite
        {
            get { return new Rgba32 (4293982975); }
        }

        /// <summary>
        /// ForestGreen
        /// </summary>
        public static Rgba32 ForestGreen
        {
            get { return new Rgba32 (4280453922); }
        }

        /// <summary>
        /// Fuchsia
        /// </summary>
        public static Rgba32 Fuchsia
        {
            get { return new Rgba32 (4294902015); }
        }

        /// <summary>
        /// Gainsboro
        /// </summary>
        public static Rgba32 Gainsboro
        {
            get { return new Rgba32 (4292664540); }
        }

        /// <summary>
        /// GhostWhite
        /// </summary>
        public static Rgba32 GhostWhite
        {
            get { return new Rgba32 (4294965496); }
        }

        /// <summary>
        /// Gold
        /// </summary>
        public static Rgba32 Gold
        {
            get { return new Rgba32 (4278245375); }
        }

        /// <summary>
        /// Goldenrod
        /// </summary>
        public static Rgba32 Goldenrod
        {
            get { return new Rgba32 (4280329690); }
        }

        /// <summary>
        /// Grey
        /// </summary>
        public static Rgba32 Grey
        {
            get { return new Rgba32 (4286611584); }
        }

        /// <summary>
        /// Green
        /// </summary>
        public static Rgba32 Green
        {
            get { return new Rgba32 (4278222848); }
        }

        /// <summary>
        /// GreenYellow
        /// </summary>
        public static Rgba32 GreenYellow
        {
            get { return new Rgba32 (4281335725); }
        }

        /// <summary>
        /// Honeydew
        /// </summary>
        public static Rgba32 Honeydew
        {
            get { return new Rgba32 (4293984240); }
        }

        /// <summary>
        /// HotPink
        /// </summary>
        public static Rgba32 HotPink
        {
            get { return new Rgba32 (4290013695); }
        }

        /// <summary>
        /// IndianRed
        /// </summary>
        public static Rgba32 IndianRed
        {
            get { return new Rgba32 (4284243149); }
        }

        /// <summary>
        /// Indigo
        /// </summary>
        public static Rgba32 Indigo
        {
            get { return new Rgba32 (4286709835); }
        }

        /// <summary>
        /// Ivory
        /// </summary>
        public static Rgba32 Ivory
        {
            get { return new Rgba32 (4293984255); }
        }

        /// <summary>
        /// Khaki
        /// </summary>
        public static Rgba32 Khaki
        {
            get { return new Rgba32 (4287424240); }
        }

        /// <summary>
        /// Lavender
        /// </summary>
        public static Rgba32 Lavender
        {
            get { return new Rgba32 (4294633190); }
        }

        /// <summary>
        /// LavenderBlush
        /// </summary>
        public static Rgba32 LavenderBlush
        {
            get { return new Rgba32 (4294308095); }
        }

        /// <summary>
        /// LawnGreen
        /// </summary>
        public static Rgba32 LawnGreen
        {
            get { return new Rgba32 (4278254716); }
        }

        /// <summary>
        /// LemonChiffon
        /// </summary>
        public static Rgba32 LemonChiffon
        {
            get { return new Rgba32 (4291689215); }
        }

        /// <summary>
        /// LightBlue
        /// </summary>
        public static Rgba32 LightBlue
        {
            get { return new Rgba32 (4293318829); }
        }

        /// <summary>
        /// LightCoral
        /// </summary>
        public static Rgba32 LightCoral
        {
            get { return new Rgba32 (4286611696); }
        }

        /// <summary>
        /// LightCyan
        /// </summary>
        public static Rgba32 LightCyan
        {
            get { return new Rgba32 (4294967264); }
        }

        /// <summary>
        /// LightGoldenrodYellow
        /// </summary>
        public static Rgba32 LightGoldenrodYellow
        {
            get { return new Rgba32 (4292016890); }
        }

        /// <summary>
        /// LightGreen
        /// </summary>
        public static Rgba32 LightGreen
        {
            get { return new Rgba32 (4287688336); }
        }

        /// <summary>
        /// LightGrey
        /// </summary>
        public static Rgba32 LightGrey
        {
            get { return new Rgba32 (4292072403); }
        }

        /// <summary>
        /// LightPink
        /// </summary>
        public static Rgba32 LightPink
        {
            get { return new Rgba32 (4290885375); }
        }

        /// <summary>
        /// LightSalmon
        /// </summary>
        public static Rgba32 LightSalmon
        {
            get { return new Rgba32 (4286226687); }
        }

        /// <summary>
        /// LightSeaGreen
        /// </summary>
        public static Rgba32 LightSeaGreen
        {
            get { return new Rgba32 (4289376800); }
        }

        /// <summary>
        /// LightSkyBlue
        /// </summary>
        public static Rgba32 LightSkyBlue
        {
            get { return new Rgba32 (4294626951); }
        }

        /// <summary>
        /// LightSlateGrey
        /// </summary>
        public static Rgba32 LightSlateGrey
        {
            get { return new Rgba32 (4288252023); }
        }

        /// <summary>
        /// LightSteelBlue
        /// </summary>
        public static Rgba32 LightSteelBlue
        {
            get { return new Rgba32 (4292789424); }
        }

        /// <summary>
        /// LightYellow
        /// </summary>
        public static Rgba32 LightYellow
        {
            get { return new Rgba32 (4292935679); }
        }

        /// <summary>
        /// Lime
        /// </summary>
        public static Rgba32 Lime
        {
            get { return new Rgba32 (4278255360); }
        }

        /// <summary>
        /// LimeGreen
        /// </summary>
        public static Rgba32 LimeGreen
        {
            get { return new Rgba32 (4281519410); }
        }

        /// <summary>
        /// Linen
        /// </summary>
        public static Rgba32 Linen
        {
            get { return new Rgba32 (4293325050); }
        }

        /// <summary>
        /// Magenta
        /// </summary>
        public static Rgba32 Magenta
        {
            get { return new Rgba32 (4294902015); }
        }

        /// <summary>
        /// Maroon
        /// </summary>
        public static Rgba32 Maroon
        {
            get { return new Rgba32 (4278190208); }
        }

        /// <summary>
        /// MediumAquamarine
        /// </summary>
        public static Rgba32 MediumAquamarine
        {
            get { return new Rgba32 (4289383782); }
        }

        /// <summary>
        /// MediumBlue
        /// </summary>
        public static Rgba32 MediumBlue
        {
            get { return new Rgba32 (4291624960); }
        }

        /// <summary>
        /// MediumOrchid
        /// </summary>
        public static Rgba32 MediumOrchid
        {
            get { return new Rgba32 (4292040122); }
        }

        /// <summary>
        /// MediumPurple
        /// </summary>
        public static Rgba32 MediumPurple
        {
            get { return new Rgba32 (4292571283); }
        }

        /// <summary>
        /// MediumSeaGreen
        /// </summary>
        public static Rgba32 MediumSeaGreen
        {
            get { return new Rgba32 (4285641532); }
        }

        /// <summary>
        /// MediumSlateBlue
        /// </summary>
        public static Rgba32 MediumSlateBlue
        {
            get { return new Rgba32 (4293814395); }
        }

        /// <summary>
        /// MediumSpringGreen
        /// </summary>
        public static Rgba32 MediumSpringGreen
        {
            get { return new Rgba32 (4288346624); }
        }

        /// <summary>
        /// MediumTurquoise
        /// </summary>
        public static Rgba32 MediumTurquoise
        {
            get { return new Rgba32 (4291613000); }
        }

        /// <summary>
        /// MediumVioletRed
        /// </summary>
        public static Rgba32 MediumVioletRed
        {
            get { return new Rgba32 (4286911943); }
        }

        /// <summary>
        /// MidnightBlue
        /// </summary>
        public static Rgba32 MidnightBlue
        {
            get { return new Rgba32 (4285536537); }
        }

        /// <summary>
        /// MintCream
        /// </summary>
        public static Rgba32 MintCream
        {
            get { return new Rgba32 (4294639605); }
        }

        /// <summary>
        /// MistyRose
        /// </summary>
        public static Rgba32 MistyRose
        {
            get { return new Rgba32 (4292994303); }
        }

        /// <summary>
        /// Moccasin
        /// </summary>
        public static Rgba32 Moccasin
        {
            get { return new Rgba32 (4290110719); }
        }

        /// <summary>
        /// NavajoWhite
        /// </summary>
        public static Rgba32 NavajoWhite
        {
            get { return new Rgba32 (4289584895); }
        }

        /// <summary>
        /// Navy
        /// </summary>
        public static Rgba32 Navy
        {
            get { return new Rgba32 (4286578688); }
        }

        /// <summary>
        /// OldLace
        /// </summary>
        public static Rgba32 OldLace
        {
            get { return new Rgba32 (4293326333); }
        }

        /// <summary>
        /// Olive
        /// </summary>
        public static Rgba32 Olive
        {
            get { return new Rgba32 (4278222976); }
        }

        /// <summary>
        /// OliveDrab
        /// </summary>
        public static Rgba32 OliveDrab
        {
            get { return new Rgba32 (4280520299); }
        }

        /// <summary>
        /// Orange
        /// </summary>
        public static Rgba32 Orange
        {
            get { return new Rgba32 (4278232575); }
        }

        /// <summary>
        /// OrangeRed
        /// </summary>
        public static Rgba32 OrangeRed
        {
            get { return new Rgba32 (4278207999); }
        }

        /// <summary>
        /// Orchid
        /// </summary>
        public static Rgba32 Orchid
        {
            get { return new Rgba32 (4292243674); }
        }

        /// <summary>
        /// PaleGoldenrod
        /// </summary>
        public static Rgba32 PaleGoldenrod
        {
            get { return new Rgba32 (4289390830); }
        }

        /// <summary>
        /// PaleGreen
        /// </summary>
        public static Rgba32 PaleGreen
        {
            get { return new Rgba32 (4288215960); }
        }

        /// <summary>
        /// PaleTurquoise
        /// </summary>
        public static Rgba32 PaleTurquoise
        {
            get { return new Rgba32 (4293848751); }
        }

        /// <summary>
        /// PaleVioletRed
        /// </summary>
        public static Rgba32 PaleVioletRed
        {
            get { return new Rgba32 (4287852763); }
        }

        /// <summary>
        /// PapayaWhip
        /// </summary>
        public static Rgba32 PapayaWhip
        {
            get { return new Rgba32 (4292210687); }
        }

        /// <summary>
        /// PeachPuff
        /// </summary>
        public static Rgba32 PeachPuff
        {
            get { return new Rgba32 (4290370303); }
        }

        /// <summary>
        /// Peru
        /// </summary>
        public static Rgba32 Peru
        {
            get { return new Rgba32 (4282353101); }
        }

        /// <summary>
        /// Pink
        /// </summary>
        public static Rgba32 Pink
        {
            get { return new Rgba32 (4291543295); }
        }

        /// <summary>
        /// Plum
        /// </summary>
        public static Rgba32 Plum
        {
            get { return new Rgba32 (4292714717); }
        }

        /// <summary>
        /// PowderBlue
        /// </summary>
        public static Rgba32 PowderBlue
        {
            get { return new Rgba32 (4293320880); }
        }

        /// <summary>
        /// Purple
        /// </summary>
        public static Rgba32 Purple
        {
            get { return new Rgba32 (4286578816); }
        }

        /// <summary>
        /// Red
        /// </summary>
        public static Rgba32 Red
        {
            get { return new Rgba32 (4278190335); }
        }

        /// <summary>
        /// RosyBrown
        /// </summary>
        public static Rgba32 RosyBrown
        {
            get { return new Rgba32 (4287598524); }
        }

        /// <summary>
        /// RoyalBlue
        /// </summary>
        public static Rgba32 RoyalBlue
        {
            get { return new Rgba32 (4292962625); }
        }

        /// <summary>
        /// SaddleBrown
        /// </summary>
        public static Rgba32 SaddleBrown
        {
            get { return new Rgba32 (4279453067); }
        }

        /// <summary>
        /// Salmon
        /// </summary>
        public static Rgba32 Salmon
        {
            get { return new Rgba32 (4285694202); }
        }

        /// <summary>
        /// SandyBrown
        /// </summary>
        public static Rgba32 SandyBrown
        {
            get { return new Rgba32 (4284523764); }
        }

        /// <summary>
        /// SeaGreen
        /// </summary>
        public static Rgba32 SeaGreen
        {
            get { return new Rgba32 (4283927342); }
        }

        /// <summary>
        /// SeaShell
        /// </summary>
        public static Rgba32 SeaShell
        {
            get { return new Rgba32 (4293850623); }
        }

        /// <summary>
        /// Sienna
        /// </summary>
        public static Rgba32 Sienna
        {
            get { return new Rgba32 (4281160352); }
        }

        /// <summary>
        /// Silver
        /// </summary>
        public static Rgba32 Silver
        {
            get { return new Rgba32 (4290822336); }
        }

        /// <summary>
        /// SkyBlue
        /// </summary>
        public static Rgba32 SkyBlue
        {
            get { return new Rgba32 (4293643911); }
        }

        /// <summary>
        /// SlateBlue
        /// </summary>
        public static Rgba32 SlateBlue
        {
            get { return new Rgba32 (4291648106); }
        }

        /// <summary>
        /// SlateGrey
        /// </summary>
        public static Rgba32 SlateGrey
        {
            get { return new Rgba32 (4287660144); }
        }

        /// <summary>
        /// Snow
        /// </summary>
        public static Rgba32 Snow
        {
            get { return new Rgba32 (4294638335); }
        }

        /// <summary>
        /// SpringGreen
        /// </summary>
        public static Rgba32 SpringGreen
        {
            get { return new Rgba32 (4286578432); }
        }

        /// <summary>
        /// SteelBlue
        /// </summary>
        public static Rgba32 SteelBlue
        {
            get { return new Rgba32 (4290019910); }
        }

        /// <summary>
        /// Tan
        /// </summary>
        public static Rgba32 Tan
        {
            get { return new Rgba32 (4287411410); }
        }

        /// <summary>
        /// Teal
        /// </summary>
        public static Rgba32 Teal
        {
            get { return new Rgba32 (4286611456); }
        }

        /// <summary>
        /// Thistle
        /// </summary>
        public static Rgba32 Thistle
        {
            get { return new Rgba32 (4292394968); }
        }

        /// <summary>
        /// Tomato
        /// </summary>
        public static Rgba32 Tomato
        {
            get { return new Rgba32 (4282868735); }
        }

        /// <summary>
        /// Turquoise
        /// </summary>
        public static Rgba32 Turquoise
        {
            get { return new Rgba32 (4291878976); }
        }

        /// <summary>
        /// Violet
        /// </summary>
        public static Rgba32 Violet
        {
            get { return new Rgba32 (4293821166); }
        }

        /// <summary>
        /// Wheat
        /// </summary>
        public static Rgba32 Wheat
        {
            get { return new Rgba32 (4289978101); }
        }

        /// <summary>
        /// White
        /// </summary>
        public static Rgba32 White
        {
            get { return new Rgba32 (4294967295); }
        }

        /// <summary>
        /// WhiteSmoke
        /// </summary>
        public static Rgba32 WhiteSmoke
        {
            get { return new Rgba32 (4294309365); }
        }

        /// <summary>
        /// Yellow
        /// </summary>
        public static Rgba32 Yellow
        {
            get { return new Rgba32 (4278255615); }
        }

        /// <summary>
        /// YellowGreen
        /// </summary>
        public static Rgba32 YellowGreen
        {
            get { return new Rgba32 (4281519514); }
        }


        /// <summary>
        /// todo
        /// </summary>
        UInt32 packedValue;

        #region IPackedValue

        /// <summary>
        /// todo
        /// </summary>
        [CLSCompliant (false)]
        public UInt32 PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            return ((obj is Rgba32) && this.Equals((Rgba32)obj));
        }

        #region IEquatable<Rgba32>

        /// <summary>
        /// todo
        /// </summary>
        public Boolean Equals(Rgba32 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator ==(Rgba32 a, Rgba32 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator !=(Rgba32 a, Rgba32 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public Rgba32(
            Single zR,
            Single zG,
            Single zB,
            Single zA)
        {
            Pack(
                zR,
                zG,
                zB,
                zA,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void PackFrom (
            Single zR,
            Single zG,
            Single zB,
            Single zA)
        {
            Pack(
                zR,
                zG,
                zB,
                zA,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void UnpackTo (
            out Single zR,
            out Single zG,
            out Single zB,
            out Single zA)
        {
            Unpack(
                this.packedValue,
                out zR,
                out zG,
                out zB,
                out zA);
        }

        /// <summary>
        /// todo
        /// </summary>
        public Byte R
        {
            get { return unchecked((Byte)this.packedValue); }
            set { this.packedValue = (this.packedValue & 0xffffff00) | value; }
        }

        /// <summary>
        /// todo
        /// </summary>
        public Byte G
        {
            get { return unchecked((Byte)(this.packedValue >> 8)); }
            set { this.packedValue = (this.packedValue & 0xffff00ff) | ((UInt32)(value << 8)); }
        }

        /// <summary>
        /// todo
        /// </summary>
        public Byte B
        {
            get { return unchecked((Byte)(this.packedValue >> 0x10)); }
            set { this.packedValue = (this.packedValue & 0xff00ffff) | ((UInt32)(value << 0x10)); }
        }

        /// <summary>
        /// todo
        /// </summary>
        public Byte A
        {
            get { return unchecked((Byte)(this.packedValue >> 0x18)); }
            set { this.packedValue = (this.packedValue & 0xffffff) | ((UInt32)(value << 0x18)); }
        }

//        /// <summary>
        /// todo
        /// </summary>
        Rgba32(UInt32 packedValue)
        {
            this.packedValue = packedValue;
        }

        /// <summary>
        /// todo
        /// </summary>
        public Rgba32(Int32 r, Int32 g, Int32 b)
        {
            if ((((r | g) | b) & -256) != 0)
            {
                r = ClampToByte64((Int64)r);
                g = ClampToByte64((Int64)g);
                b = ClampToByte64((Int64)b);
            }

            g = g << 8;
            b = b << 0x10;

            this.packedValue = (UInt32)(((r | g) | b) | -16777216);
        }

        /// <summary>
        /// todo
        /// </summary>
        public Rgba32(Int32 r, Int32 g, Int32 b, Int32 a)
        {
            if (((((r | g) | b) | a) & -256) != 0)
            {
                r = ClampToByte32(r);
                g = ClampToByte32(g);
                b = ClampToByte32(b);
                a = ClampToByte32(a);
            }

            g = g << 8;
            b = b << 0x10;
            a = a << 0x18;

            this.packedValue = (UInt32)(((r | g) | b) | a);
        }

        /// <summary>
        /// todo
        /// </summary>
        public Rgba32 (Single r, Single g, Single b)
        {
            Pack (r, g, b, 1f, out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Rgba32 FromNonPremultiplied(Single zR, Single zG, Single zB, Single zA)
        {
            Rgba32 color;
            Pack(zR * zA, zG * zA, zB * zA, zA, out color.packedValue);
            return color;
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Rgba32 FromNonPremultiplied(int r, int g, int b, int a)
        {
            Rgba32 color;
            r = ClampToByte64((r * a) / 0xffL);
            g = ClampToByte64((g * a) / 0xffL);
            b = ClampToByte64((b * a) / 0xffL);
            a = ClampToByte32(a);
            g = g << 8;
            b = b << 0x10;
            a = a << 0x18;
            color.packedValue = (UInt32)(((r | g) | b) | a);
            return color;
        }

        /// <summary>
        /// todo
        /// </summary>
        static Int32 ClampToByte32(Int32 value)
        {
            if (value < 0)
            {
                return 0;
            }

            if (value > 0xff)
            {
                return 0xff;
            }

            return value;
        }

        /// <summary>
        /// todo
        /// </summary>
        static Int32 ClampToByte64(Int64 value)
        {
            if (value < 0L)
            {
                return 0;
            }

            if (value > 0xffL)
            {
                return 0xff;
            }

            return (Int32)value;
        }


        /// <summary>
        /// This function linearly interpolates each component of a Rgba32
        /// separately and returns a Rgba32 with the new component values.
        /// </summary>
        public static Rgba32 Lerp(Rgba32 value1, Rgba32 value2, Single amount)
        {
            if (amount > 1f)
                throw new ArgumentException("Amount: " + amount + " must be <= 1.");

            if (amount < 0f)
                throw new ArgumentException("Amount: " + amount + " must be >= 0.");

            Rgba32 colour;
            UInt32 packedValue1 = value1.packedValue;
            UInt32 packedValue2 = value2.packedValue;

            Int32 r1 = (Byte) (packedValue1);
            Int32 g1 = (Byte) (packedValue1 >> 8);
            Int32 b1 = (Byte) (packedValue1 >> 0x10);
            Int32 a1 = (Byte) (packedValue1 >> 0x18);

            Int32 r2 = (Byte) (packedValue2);
            Int32 g2 = (Byte) (packedValue2 >> 8);
            Int32 b2 = (Byte) (packedValue2 >> 0x10);
            Int32 a2 = (Byte) (packedValue2 >> 0x18);

            Int32 num = (Int32) PackUtils.PackUnsignedNormalisedValue(65536f, amount);

            Int32 r = r1 + (((r2 - r1) * num) >> 0x10);
            Int32 g = g1 + (((g2 - g1) * num) >> 0x10);
            Int32 b = b1 + (((b2 - b1) * num) >> 0x10);
            Int32 a = a1 + (((a2 - a1) * num) >> 0x10);

            colour.packedValue =
                (UInt32)(((r | (g << 8)) | (b << 0x10)) | (a << 0x18));

            return colour;
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Rgba32 Desaturate(Rgba32 colour, float desaturation)
        {
            throw new NotImplementedException ();
            /*
            System.Diagnostics.Debug.Assert(desaturation <= 1f && desaturation >= 0f);

            var luminanceWeights = new SinglePrecision.Vector3(0.299f, 0.587f, 0.114f);

            SinglePrecision.Vector4 colourVec4;

            colour.UnpackTo(out colourVec4);

            SinglePrecision.Vector3 colourVec = new SinglePrecision.Vector3(colourVec4.X, colourVec4.Y, colourVec4.Z);

            float luminance;

            SinglePrecision.Vector3.Dot(ref luminanceWeights, ref colourVec, out luminance);

            SinglePrecision.Vector3 lumVec = new SinglePrecision.Vector3(luminance, luminance, luminance);

            SinglePrecision.Vector3.Lerp(ref colourVec, ref lumVec, ref desaturation, out colourVec);

            return new Rgba32(colourVec.X, colourVec.Y, colourVec.Z, colourVec4.W);
            */
        }

//        /// <summary>
        /// todo
        /// </summary>
        public static Rgba32 operator *(Rgba32 value, Single scale)
        {
            UInt32 num;
            Rgba32 color;
            UInt32 packedValue = value.packedValue;
            UInt32 num5 = (byte)packedValue;
            UInt32 num4 = (byte)(packedValue >> 8);
            UInt32 num3 = (byte)(packedValue >> 0x10);
            UInt32 num2 = (byte)(packedValue >> 0x18);
            scale *= 65536f;
            if (scale < 0f)
            {
                num = 0;
            }
            else if (scale > 1.677722E+07f)
            {
                num = 0xffffff;
            }
            else
            {
                num = (UInt32)scale;
            }
            num5 = (num5 * num) >> 0x10;
            num4 = (num4 * num) >> 0x10;
            num3 = (num3 * num) >> 0x10;
            num2 = (num2 * num) >> 0x10;
            if (num5 > 0xff)
            {
                num5 = 0xff;
            }
            if (num4 > 0xff)
            {
                num4 = 0xff;
            }
            if (num3 > 0xff)
            {
                num3 = 0xff;
            }
            if (num2 > 0xff)
            {
                num2 = 0xff;
            }
            color.packedValue = ((num5 | (num4 << 8)) | (num3 << 0x10)) | (num2 << 0x18);
            return color;
        }

        /// <summary>
        /// todo
        /// </summary>
        public static void Multiply(ref Rgba32 value, ref Single scale, out Rgba32 colour )
        {
            UInt32 num;
            UInt32 packedValue = value.packedValue;
            UInt32 num5 = (byte)packedValue;
            UInt32 num4 = (byte)(packedValue >> 8);
            UInt32 num3 = (byte)(packedValue >> 0x10);
            UInt32 num2 = (byte)(packedValue >> 0x18);
            scale *= 65536f;
            if (scale < 0f)
            {
                num = 0;
            }
            else if (scale > 1.677722E+07f)
            {
                num = 0xffffff;
            }
            else
            {
                num = (UInt32)scale;
            }
            num5 = (num5 * num) >> 0x10;
            num4 = (num4 * num) >> 0x10;
            num3 = (num3 * num) >> 0x10;
            num2 = (num2 * num) >> 0x10;
            if (num5 > 0xff)
            {
                num5 = 0xff;
            }
            if (num4 > 0xff)
            {
                num4 = 0xff;
            }
            if (num3 > 0xff)
            {
                num3 = 0xff;
            }
            if (num2 > 0xff)
            {
                num2 = 0xff;
            }
            colour.packedValue = ((num5 | (num4 << 8)) | (num3 << 0x10)) | (num2 << 0x18);
        }


    }

    [StructLayout (LayoutKind.Sequential), Serializable]
    public struct Rgba64
        : IPackedValue<UInt64>
        , IEquatable<Rgba64>
        , IPackedReal4
    {
        public override String ToString ()
        {
            return this.packedValue.ToString ("X8", CultureInfo.InvariantCulture);
        }

        static void Pack(Single zR, Single zG, Single zB, Single zA, out UInt64 zPackedRgba)
        {
            if (zR < 0f || zR > 1f || zG < 0f || zG > 1f || zB < 0f || zB > 1f || zA < 0f || zA > 1f)
            {
                throw new ArgumentException ("A component of the input source is not unsigned and normalised.");
            }

            UInt64 r = (UInt64) PackUtils.PackUnsignedNormalisedValue(0xffff, zR);
            UInt64 g = ((UInt64) PackUtils.PackUnsignedNormalisedValue(0xffff, zG)) << 16;
            UInt64 b = ((UInt64) PackUtils.PackUnsignedNormalisedValue(0xffff, zB)) << 32;
            UInt64 a = ((UInt64) PackUtils.PackUnsignedNormalisedValue(0xffff, zA)) << 48;

            zPackedRgba = (((r | g) | b) | a);
        }

        static void Unpack(UInt64 zPackedRgba, out Single zR, out Single zG, out Single zB, out Single zA)
        {
            zR = PackUtils.UnpackUnsignedNormalisedValue (0xffff, (UInt32) zPackedRgba);
            zG = PackUtils.UnpackUnsignedNormalisedValue (0xffff, (UInt32) (zPackedRgba >> 16));
            zB = PackUtils.UnpackUnsignedNormalisedValue (0xffff, (UInt32) (zPackedRgba >> 32));
            zA = PackUtils.UnpackUnsignedNormalisedValue (0xffff, (UInt32) (zPackedRgba >> 48));

            if (zR < 0f || zR > 1f || zG < 0f || zG > 1f || zB < 0f || zB > 1f || zA < 0f || zA > 1f)
            {
                throw new Exception ("A the input source doesn't yeild a unsigned normalised output: " + zPackedRgba);
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        UInt64 packedValue;

        #region IPackedValue

        /// <summary>
        /// todo
        /// </summary>
        [CLSCompliant (false)]
        public UInt64 PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            return ((obj is Rgba64) && this.Equals((Rgba64)obj));
        }

        #region IEquatable<Rgba64>

        /// <summary>
        /// todo
        /// </summary>
        public Boolean Equals(Rgba64 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator ==(Rgba64 a, Rgba64 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator !=(Rgba64 a, Rgba64 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public Rgba64(
            Single zR,
            Single zG,
            Single zB,
            Single zA)
        {
            Pack(
                zR,
                zG,
                zB,
                zA,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void PackFrom (
            Single zR,
            Single zG,
            Single zB,
            Single zA)
        {
            Pack(
                zR,
                zG,
                zB,
                zA,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void UnpackTo (
            out Single zR,
            out Single zG,
            out Single zB,
            out Single zA)
        {
            Unpack(
                this.packedValue,
                out zR,
                out zG,
                out zB,
                out zA);
        }
    }

    // 2 bit alpha
    [StructLayout (LayoutKind.Sequential), Serializable]
    public struct Rgba_10_10_10_2
        : IPackedValue<UInt32>
        , IEquatable<Rgba_10_10_10_2>
        , IPackedReal4
    {

        public override String ToString ()
        {
            return this.packedValue.ToString ("X8", CultureInfo.InvariantCulture);
        }

        static void Pack(Single zR, Single zG, Single zB, Single zA, out UInt32 zPackedRgba)
        {
            if (zR < 0f || zR > 1f || zG < 0f || zG > 1f || zB < 0f || zB > 1f || zA < 0f || zA > 1f)
            {
                throw new ArgumentException ("A component of the input source is not unsigned and normalised.");
            }

            UInt32 r = PackUtils.PackUnsignedNormalisedValue (0xffff, zR);
            UInt32 g = PackUtils.PackUnsignedNormalisedValue (0xffff, zG) << 10;
            UInt32 b = PackUtils.PackUnsignedNormalisedValue (0xffff, zB) << 20;
            UInt32 a = PackUtils.PackUnsignedNormalisedValue (0xffff, zA) << 30;

            zPackedRgba = ((r | g) | b) | a;
        }

        static void Unpack(UInt32 zPackedRgba, out Single zR, out Single zG, out Single zB, out Single zA)
        {
            zR = PackUtils.UnpackUnsignedNormalisedValue (0xffff, zPackedRgba);
            zG = PackUtils.UnpackUnsignedNormalisedValue (0xffff, (UInt32)(zPackedRgba >> 10));
            zB = PackUtils.UnpackUnsignedNormalisedValue (0xffff, (UInt32)(zPackedRgba >> 20));
            zA = PackUtils.UnpackUnsignedNormalisedValue (0xffff, (UInt32)(zPackedRgba >> 30));

            if (zR < 0f || zR > 1f || zG < 0f || zG > 1f || zB < 0f || zB > 1f || zA < 0f || zA > 1f)
            {
                throw new Exception ("A the input source doesn't yeild an unsigned normalised output: " + zPackedRgba);
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        UInt32 packedValue;

        #region IPackedValue

        /// <summary>
        /// todo
        /// </summary>
        [CLSCompliant (false)]
        public UInt32 PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            return ((obj is Rgba_10_10_10_2) && this.Equals((Rgba_10_10_10_2)obj));
        }

        #region IEquatable<Rgba_10_10_10_2>

        /// <summary>
        /// todo
        /// </summary>
        public Boolean Equals(Rgba_10_10_10_2 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator ==(Rgba_10_10_10_2 a, Rgba_10_10_10_2 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator !=(Rgba_10_10_10_2 a, Rgba_10_10_10_2 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public Rgba_10_10_10_2(
            Single zR,
            Single zG,
            Single zB,
            Single zA)
        {
            Pack(
                zR,
                zG,
                zB,
                zA,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void PackFrom (
            Single zR,
            Single zG,
            Single zB,
            Single zA)
        {
            Pack(
                zR,
                zG,
                zB,
                zA,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void UnpackTo (
            out Single zR,
            out Single zG,
            out Single zB,
            out Single zA)
        {
            Unpack(
                this.packedValue,
                out zR,
                out zG,
                out zB,
                out zA);
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout (LayoutKind.Sequential), Serializable]
    public struct Short2
        : IPackedValue<UInt32>
        , IEquatable<Short2>
        , IPackedReal2
    {
        /// <summary>
        /// todo
        /// </summary>
        public override String ToString ()
        {
            return this.packedValue.ToString ("X8", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Pack(Single zX, Single zY, out UInt32 zPackedXy)
        {
            UInt32 x = PackUtils.PackSigned (0xffff, zX);
            UInt32 y = PackUtils.PackSigned (0xffff, zY) << 16;

            zPackedXy = (x | y);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Unpack(UInt32 zPackedXy, out Single zX, out Single zY)
        {
            zX = (Int16) zPackedXy;
            zY = (Int16) (zPackedXy >> 16);
        }

        /// <summary>
        /// todo
        /// </summary>
        UInt32 packedValue;

        #region IPackedValue

        /// <summary>
        /// todo
        /// </summary>
        [CLSCompliant (false)]
        public UInt32 PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            return ((obj is Short2) && this.Equals((Short2)obj));
        }

        #region IEquatable<Short2>

        /// <summary>
        /// todo
        /// </summary>
        public Boolean Equals(Short2 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator ==(Short2 a, Short2 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator !=(Short2 a, Short2 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public Short2(
            Single zX,
            Single zY)
        {
            Pack(
                zX,
                zY,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void PackFrom (
            Single zX,
            Single zY)
        {
            Pack(
                zX,
                zY,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void UnpackTo (
            out Single zX,
            out Single zY)
        {
            Unpack(
                this.packedValue,
                out zX,
                out zY);
        }

    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout (LayoutKind.Sequential), Serializable]
    public struct Short4
        : IPackedValue<UInt64>
        , IEquatable<Short4>
        , IPackedReal4
    {
        /// <summary>
        /// todo
        /// </summary>
        public override String ToString ()
        {
            return this.packedValue.ToString ("X8", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Pack(Single zX, Single zY, Single zZ, Single zW, out UInt64 zPackedXyzw)
        {
            UInt64 x = (UInt64) PackUtils.PackSigned(0xffff, zX);
            UInt64 y = ((UInt64) PackUtils.PackSigned(0xffff, zY)) << 16;
            UInt64 z = ((UInt64) PackUtils.PackSigned(0xffff, zZ)) << 32;
            UInt64 w = ((UInt64) PackUtils.PackSigned(0xffff, zW)) << 48;

            zPackedXyzw = (((x | y) | z) | w);
        }

        /// <summary>
        /// todo
        /// </summary>
        static void Unpack(UInt64 zPackedXyzw, out Single zX, out Single zY, out Single zZ, out Single zW)
        {
            zX = ((Int16) zPackedXyzw);
            zY = ((Int16) (zPackedXyzw >> 16));
            zZ = ((Int16) (zPackedXyzw >> 32));
            zW = ((Int16) (zPackedXyzw >> 48));
        }

        /// <summary>
        /// todo
        /// </summary>
        UInt64 packedValue;

        #region IPackedValue

        /// <summary>
        /// todo
        /// </summary>
        [CLSCompliant (false)]
        public UInt64 PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            return ((obj is Short4) && this.Equals((Short4)obj));
        }

        #region IEquatable<Short4>

        /// <summary>
        /// todo
        /// </summary>
        public Boolean Equals(Short4 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        #endregion

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator ==(Short4 a, Short4 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator !=(Short4 a, Short4 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// todo
        /// </summary>
        public Short4(
            Single zX,
            Single zY,
            Single zZ,
            Single zW)
        {
            Pack(
                zX,
                zY,
                zZ,
                zW,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void PackFrom (
            Single zX,
            Single zY,
            Single zZ,
            Single zW)
        {
            Pack(
                zX,
                zY,
                zZ,
                zW,
                out this.packedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void UnpackTo (
            out Single zX,
            out Single zY,
            out Single zZ,
            out Single zW)
        {
            Unpack(
                this.packedValue,
                out zX,
                out zY,
                out zZ,
                out zW);
        }

    }


}
