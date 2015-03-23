// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ ___________        .___                                                │ \\
// │ \_   _____/_ __  __| _/ ____   ____                                    │ \\
// │  |    __)|  |  \/ __ | / ___\_/ __ \                                   │ \\
// │  |     \ |  |  / /_/ |/ /_/  >  ___/                                   │ \\
// │  \___  / |____/\____ |\___  / \___  >                                  │ \\
// │      \/             \/_____/      \/                                   │ \\
// │                                                                        │ \\
// │ A fast data packaging library.                                         │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey3D (http://www.blimey.io)              │ \\
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
#define TESTS_ENABLED

#if TESTS_ENABLED


using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using NUnit.Framework;
using System.Runtime.CompilerServices;

namespace Fudge.Tests
{
    /// <summary>
    /// todo
    /// </summary>
    static class Settings
    {
        internal const UInt32 NumTests = 10000;
    }

    /// <summary>
    /// todo
    /// </summary>
    [TestFixture]
    public class PackUtils
    {
        /*
        /// <summary>
        /// todo
        /// </summary>
        [Test]
        public void TestPacking_Signed_i ()
        {
            var rand = new System.Random();

            UInt32 bitmask = 0xffffffff;

            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                Single s = (Single)rand.NextDouble();

                UInt32 p = global::Fudge.PackUtils.PackSigned (bitmask, s);

                Single u = global::Fudge.PackUtils.UnpackSigned (bitmask, p);

                Assert.That (u, Is.EqualTo(s));
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        [Test]
        public void TestPacking_Signed_ii ()
        {
            var rand = new System.Random();
            var buff = new Byte[4];
            UInt32 bitmask = 0xffffffff;

            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt32 p = BitConverter.ToUInt32(buff, 0);

                Single u = global::Fudge.PackUtils.UnpackSigned (bitmask, p);

                UInt32 rp = global::Fudge.PackUtils.PackSigned (bitmask, u);

                Assert.That (rp, Is.EqualTo(p));
            }
        }*/

        /// <summary>
        /// todo
        /// </summary>
        [Test]
        public void TestPacking_SignedNormalised_i ()
        {
            var rand = new System.Random();

            UInt32 bitmask = 0xffffffff;

            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                Single s = (Single)rand.NextDouble();

                if (rand.Next(0, 1) == 1) s = -s;

                UInt32 p = global::Fudge.PackUtils.PackSignedNormalised (bitmask, s);

                Single u = global::Fudge.PackUtils.UnpackSignedNormalised (bitmask, p);

                Assert.That (u, Is.EqualTo(s));
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        [Test]
        public void TestPacking_UnsignedNormalisedValue_i ()
        {
            var rand = new System.Random();

            UInt32 bitmask = 0xffffffff;

            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                Single s = (Single)rand.NextDouble();

                UInt32 p = global::Fudge.PackUtils.PackUnsignedNormalisedValue (bitmask, s);

                Single u = global::Fudge.PackUtils.UnpackUnsignedNormalisedValue (bitmask, p);

                Assert.That (u, Is.EqualTo(s));
            }
        }
    }

    /// <summary>
    /// Tests the Alpha_8 packed data type.
    /// </summary>
    [TestFixture]
    public class Alpha_8Tests
    {
        /// <summary>
        /// Iterates over every possible Alpha_8 value and makes sure that
        /// unpacking them and then re-packing that result yeilds the 
        /// original packed value.
        /// </summary>
        [Test]
        public void TestAllPossibleValues_i()
        {   
            Byte packed = Byte.MinValue;
            while ( packed < Byte.MaxValue )
            {
                ++packed;
                var packedObj = new Alpha_8();
                packedObj.PackedValue = packed;
                Single unpacked;
                packedObj.UnpackTo(out unpacked);
                var newPackedObj = new Alpha_8(unpacked);
                Assert.That(newPackedObj.PackedValue, Is.EqualTo(packed));
            }
        }

        /// <summary>
        /// For a given example, this test ensures that the ToString function
        /// yields the expected string.
        /// </summary>
        [Test]
        public void TestMemberFn_ToString_i()
        {
            var testCase = new Alpha_8();
            testCase.PackFrom(0.656f);
            String s = testCase.ToString ();
            Assert.That(s, Is.EqualTo("A7"));
        }

        /// <summary>
        /// Makes sure that the hashing function is good by testing 
        /// all scenarios and ensuring that there are no collisions.
        /// </summary>
        [Test]
        public void TestMemberFn_GetHashCode_i ()
        {
            HashSet<Int32> hs = new HashSet<Int32>();
            Byte packed = Byte.MinValue;
            while ( packed < Byte.MaxValue )
            {
                ++packed;
                var packedObj = new Alpha_8();
                packedObj.PackedValue = packed;
                Int32 hc = packedObj.GetHashCode ();
                Assert.That(!hs.Contains(hc));
                hs.Add(hc);
            }
        }
    }

    /// <summary>
    /// Tests the Bgr_5_6_5 packed data type.
    /// </summary>
    [TestFixture]
    public class Bgr_5_6_5Tests
    {
        /// <summary>
        /// Iterates over every possible Bgr_5_6_5 value and makes sure that
        /// unpacking them and then re-packing that result yeilds the
        /// original packed value.
        /// </summary>
        [Test]
        public void TestAllPossibleValues_i()
        {
            UInt16 packed = UInt16.MinValue;
            while ( packed < UInt16.MaxValue )
            {
                ++packed;
                var packedObj = new Bgr_5_6_5();
                packedObj.PackedValue = packed;
                Single realB, realG, realR = 0f;
                packedObj.UnpackTo(out realB, out realG, out realR);
                var newPackedObj = new Bgr_5_6_5(realB, realG, realR);
                Assert.That(newPackedObj.PackedValue, Is.EqualTo(packed));
            }
        }

        /// <summary>
        /// For a given example, this test ensures that the ToString function
        /// yields the expected string.
        /// </summary>
        [Test]
        public void TestMemberFn_ToString_i()
        {
            var testCase = new Bgr_5_6_5();
            testCase.PackFrom(0.861f, 0.125f, 0.656f);
            String s = testCase.ToString ();
            Assert.That(s, Is.EqualTo("A11B"));
        }

        /// <summary>
        /// Makes sure that the hashing function is good by testing
        /// all scenarios and ensuring that there are no collisions.
        /// </summary>
        [Test]
        public void TestMemberFn_GetHashCode_i ()
        {
            HashSet<Int32> hs = new HashSet<Int32>();
            UInt16 packed = UInt16.MinValue;
            while ( packed < UInt16.MaxValue )
            {
                ++packed;
                var packedObj = new Bgr_5_6_5();
                packedObj.PackedValue = packed;
                Int32 hc = packedObj.GetHashCode ();
                Assert.That(!hs.Contains(hc));
                hs.Add(hc);
            }
        }
    }

    /// <summary>
    /// Tests the Bgra16 packed data type.
    /// </summary>
    [TestFixture]
    public class Bgra16Tests
    {
        /// <summary>
        /// Iterates over every possible Bgra16 value and makes sure that
        /// unpacking them and then re-packing that result yeilds the
        /// original packed value.
        /// </summary>
        [Test]
        public void TestAllPossibleValues_i()
        {
            UInt16 packed = UInt16.MinValue;
            while ( packed < UInt16.MaxValue )
            {
                ++packed;
                var packedObj = new Bgra16();
                packedObj.PackedValue = packed;
                Single realB, realG, realR, realA = 0f;
                packedObj.UnpackTo(out realB, out realG, out realR, out realA);
                var newPackedObj = new Bgra16(realB, realG, realR, realA);
                Assert.That(newPackedObj.PackedValue, Is.EqualTo(packed));
            }
        }

        /// <summary>
        /// For a given example, this test ensures that the ToString function
        /// yields the expected string.
        /// </summary>
        [Test]
        public void TestMemberFn_ToString_i()
        {
            var testCase = new Bgra16();
            testCase.PackFrom(0.222f, 0.125f, 0.656f, 0.861f);
            String s = testCase.ToString ();
            Assert.That(s, Is.EqualTo("DA23"));
        }

        /// <summary>
        /// Makes sure that the hashing function is good by testing
        /// all scenarios and ensuring that there are no collisions.
        /// </summary>
        [Test]
        public void TestMemberFn_GetHashCode_i ()
        {
            HashSet<Int32> hs = new HashSet<Int32>();
            UInt16 packed = UInt16.MinValue;
            while ( packed < UInt16.MaxValue )
            {
                ++packed;
                var packedObj = new Bgra16();
                packedObj.PackedValue = packed;
                Int32 hc = packedObj.GetHashCode ();
                Assert.That(!hs.Contains(hc));
                hs.Add(hc);
            }
        }
    }

    /// <summary>
    /// Tests the Bgra_5_5_5_1 packed data type.
    /// </summary>
    [TestFixture]
    public class Bgra_5_5_5_1Tests
    {
        /// <summary>
        /// Iterates over every possible Bgra_5_5_5_1 value and makes sure that
        /// unpacking them and then re-packing that result yeilds the
        /// original packed value.
        /// </summary>
        [Test]
        public void TestAllPossibleValues_i()
        {
            UInt16 packed = UInt16.MinValue;
            while ( packed < UInt16.MaxValue )
            {
                ++packed;
                var packedObj = new Bgra_5_5_5_1();
                packedObj.PackedValue = packed;
                Single realB, realG, realR, realA = 0f;
                packedObj.UnpackTo(out realB, out realG, out realR, out realA);
                var newPackedObj = new Bgra_5_5_5_1(realB, realG, realR, realA);
                Assert.That(newPackedObj.PackedValue, Is.EqualTo(packed));
            }
        }

        /// <summary>
        /// For a given example, this test ensures that the ToString function
        /// yields the expected string.
        /// </summary>
        [Test]
        public void TestMemberFn_ToString_i()
        {
            var testCase = new Bgra_5_5_5_1();
            testCase.PackFrom(0.222f, 0.125f, 0.656f, 0.861f);
            String s = testCase.ToString ();
            Assert.That(s, Is.EqualTo("D087"));
        }

        /// <summary>
        /// Makes sure that the hashing function is good by testing
        /// all scenarios and ensuring that there are no collisions.
        /// </summary>
        [Test]
        public void TestMemberFn_GetHashCode_i ()
        {
            HashSet<Int32> hs = new HashSet<Int32>();
            UInt16 packed = UInt16.MinValue;
            while ( packed < UInt16.MaxValue )
            {
                ++packed;
                var packedObj = new Bgra_5_5_5_1();
                packedObj.PackedValue = packed;
                Int32 hc = packedObj.GetHashCode ();
                Assert.That(!hs.Contains(hc));
                hs.Add(hc);
            }
        }
    }
    /// <summary>
    /// Tests the Byte4 packed data type.
    /// </summary>
    [TestFixture]
    public class Byte4Tests
    {
        /// <summary>
        /// Iterates over a random selection of values within the range of
        /// possible Byte4 values and makes sure that unpacking them and
        /// then re-packing that result yeilds the original packed value.
        /// </summary>
        [Test]
        public void TestRandomValues_i()
        {
            var rand = new System.Random();
            var buff = new Byte[4];

            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt32 packed = BitConverter.ToUInt32(buff, 0);
                var packedObj = new Byte4();
                packedObj.PackedValue = packed;
                Single realX, realY, realZ, realW = 0f;
                packedObj.UnpackTo(out realX, out realY, out realZ, out realW);
                var newPackedObj = new Byte4(realX, realY, realZ, realW);
                Assert.That(newPackedObj.PackedValue, Is.EqualTo(packed));
            }
        }

        /// <summary>
        /// For a given example, this test ensures that the ToString function
        /// yields the expected string.
        /// </summary>
        [Test]
        public void TestMemberFn_ToString_i()
        {
            var testCase = new Byte4();
            testCase.PackFrom(0.656f, 0.125f, 0.222f, 0.861f);
            String s = testCase.ToString ();
            Assert.That(s, Is.EqualTo("01000001"));
        }

        /// <summary>
        /// Makes sure that the hashing function is good by testing
        /// random scenarios and ensuring that there are no more than a
        /// reasonable number of collisions.
        /// </summary>
        [Test]
        public void TestMemberFn_GetHashCode_i ()
        {
            HashSet<Int32> hs = new HashSet<Int32>();
            var rand = new System.Random();
            var buff = new Byte[4];
            UInt32 collisions = 0;
            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                BitConverter.ToUInt32(buff, 0);
                UInt32 packed = BitConverter.ToUInt32(buff, 0);
                var packedObj = new Byte4();
                packedObj.PackedValue = packed;
                Int32 hc = packedObj.GetHashCode ();
                if(hs.Contains(hc)) ++collisions;
                hs.Add(hc);
            }
            Assert.That(collisions, Is.LessThan(10));
        }
    }

    /// <summary>
    /// Tests the NormalisedByte2 packed data type.
    /// </summary>
    [TestFixture]
    public class NormalisedByte2Tests
    {
        /// <summary>
        /// Iterates over every possible NormalisedByte2 value and makes sure that
        /// unpacking them and then re-packing that result yeilds the
        /// original packed value.
        /// </summary>
        [Test]
        public void TestAllPossibleValues_i()
        {
            UInt16 packed = UInt16.MinValue;
            while ( packed < UInt16.MaxValue )
            {
                ++packed;
                // Cannot guarantee that this packed value is valid.
                try
                {
                    var packedObj = new NormalisedByte2();
                    packedObj.PackedValue = packed;
                    Single realX, realY = 0f;
                    packedObj.UnpackTo(out realX, out realY);
                    var newPackedObj = new NormalisedByte2(realX, realY);
                    Assert.That(newPackedObj.PackedValue, Is.EqualTo(packedObj.PackedValue));
                }
                catch(ArgumentException)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// For a given example, this test ensures that the ToString function
        /// yields the expected string.
        /// </summary>
        [Test]
        public void TestMemberFn_ToString_i()
        {
            var testCase = new NormalisedByte2();
            testCase.PackFrom(0.222f, 0.861f);
            String s = testCase.ToString ();
            Assert.That(s, Is.EqualTo("6D1C"));
        }

        /// <summary>
        /// Makes sure that the hashing function is good by testing
        /// all scenarios and ensuring that there are no collisions.
        /// </summary>
        [Test]
        public void TestMemberFn_GetHashCode_i ()
        {
            HashSet<Int32> hs = new HashSet<Int32>();
            UInt16 packed = UInt16.MinValue;
            while ( packed < UInt16.MaxValue )
            {
                ++packed;
                var packedObj = new NormalisedByte2();
                packedObj.PackedValue = packed;
                Int32 hc = packedObj.GetHashCode ();
                Assert.That(!hs.Contains(hc));
                hs.Add(hc);
            }
        }
    }

    /// <summary>
    /// Tests the NormalisedByte4 packed data type.
    /// </summary>
    [TestFixture]
    public class NormalisedByte4Tests
    {
        /// <summary>
        /// Iterates over a random selection of values within the range of
        /// possible NormalisedByte4 values and makes sure that unpacking them and
        /// then re-packing that result yeilds the original packed value.
        /// </summary>
        [Test]
        public void TestRandomValues_i()
        {
            var rand = new System.Random();
            var buff = new Byte[4];

            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt32 packed = BitConverter.ToUInt32(buff, 0);

                // Cannot guarantee that this packed value is valid.
                try
                {
                    var packedObj = new NormalisedByte4();
                    packedObj.PackedValue = packed;
                    Single realX, realY, realZ, realW = 0f;
                    packedObj.UnpackTo(out realX, out realY, out realZ, out realW);
                    var newPackedObj = new NormalisedByte4(realX, realY, realZ, realW);
                    Assert.That(newPackedObj.PackedValue, Is.EqualTo(packed));
                }
                catch(ArgumentException)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// For a given example, this test ensures that the ToString function
        /// yields the expected string.
        /// </summary>
        [Test]
        public void TestMemberFn_ToString_i()
        {
            var testCase = new NormalisedByte4();
            testCase.PackFrom(0.656f, 0.125f, 0.222f, 0.861f);
            String s = testCase.ToString ();
            Assert.That(s, Is.EqualTo("6D1C1053"));
        }

        /// <summary>
        /// Makes sure that the hashing function is good by testing
        /// random scenarios and ensuring that there are no more than a
        /// reasonable number of collisions.
        /// </summary>
        [Test]
        public void TestMemberFn_GetHashCode_i ()
        {
            HashSet<Int32> hs = new HashSet<Int32>();
            var rand = new System.Random();
            var buff = new Byte[4];
            UInt32 collisions = 0;
            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt32 packed = BitConverter.ToUInt32(buff, 0);
                var packedObj = new NormalisedByte4();
                packedObj.PackedValue = packed;
                Int32 hc = packedObj.GetHashCode ();
                if(hs.Contains(hc)) ++collisions;
                hs.Add(hc);
            }
            Assert.That(collisions, Is.LessThan(10));
        }
    }

    /// <summary>
    /// Tests the NormalisedShort2 packed data type.
    /// </summary>
    [TestFixture]
    public class NormalisedShort2Tests
    {
        /// <summary>
        /// Iterates over a random selection of values within the range of
        /// possible NormalisedShort2 values and makes sure that unpacking them and
        /// then re-packing that result yeilds the original packed value.
        /// </summary>
        [Test]
        public void TestRandomValues_i()
        {
            var rand = new System.Random();
            var buff = new Byte[4];

            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt32 packed = BitConverter.ToUInt32(buff, 0);

                // Cannot guarantee that this packed value is valid.
                try
                {
                    var packedObj = new NormalisedShort2();
                    packedObj.PackedValue = packed;
                    Single realX, realY = 0f;
                    packedObj.UnpackTo(out realX, out realY);
                    var newPackedObj = new NormalisedShort2(realX, realY);
                    Assert.That(newPackedObj.PackedValue, Is.EqualTo(packed));
                }
                catch(ArgumentException)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// For a given example, this test ensures that the ToString function
        /// yields the expected string.
        /// </summary>
        [Test]
        public void TestMemberFn_ToString_i()
        {
            var testCase = new NormalisedShort2();
            testCase.PackFrom(0.656f, 0.861f);
            String s = testCase.ToString ();
            Assert.That(s, Is.EqualTo("6E3453F7"));
        }

        /// <summary>
        /// Makes sure that the hashing function is good by testing
        /// random scenarios and ensuring that there are no more than a
        /// reasonable number of collisions.
        /// </summary>
        [Test]
        public void TestMemberFn_GetHashCode_i ()
        {
            HashSet<Int32> hs = new HashSet<Int32>();
            var rand = new System.Random();
            var buff = new Byte[4];
            UInt32 collisions = 0;
            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt32 packed = BitConverter.ToUInt32(buff, 0);
                var packedObj = new NormalisedShort2();
                packedObj.PackedValue = packed;
                Int32 hc = packedObj.GetHashCode ();
                if(hs.Contains(hc)) ++collisions;
                hs.Add(hc);
            }
            Assert.That(collisions, Is.LessThan(10));
        }
    }

    /// <summary>
    /// Tests the NormalisedShort4 packed data type.
    /// </summary>
    [TestFixture]
    public class NormalisedShort4Tests
    {
        /// <summary>
        /// Iterates over a random selection of values within the range of
        /// possible NormalisedShort4 values and makes sure that unpacking them and
        /// then re-packing that result yeilds the original packed value.
        /// </summary>
        [Test]
        public void TestRandomValues_i()
        {
            var rand = new System.Random();
            var buff = new Byte[8];

            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt64 packed = BitConverter.ToUInt64(buff, 0);

                // Cannot guarantee that this packed value is valid.
                try
                {
                    var packedObj = new NormalisedShort4();
                    packedObj.PackedValue = packed;
                    Single realX, realY, realZ, realW = 0f;
                    packedObj.UnpackTo(out realX, out realY, out realZ, out realW);
                    var newPackedObj = new NormalisedShort4(realX, realY, realZ, realW);
                    Assert.That(newPackedObj.PackedValue, Is.EqualTo(packed));
                }
                catch(ArgumentException)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// For a given example, this test ensures that the ToString function
        /// yields the expected string.
        /// </summary>
        [Test]
        public void TestMemberFn_ToString_i()
        {
            var testCase = new NormalisedShort4();
            testCase.PackFrom(0.656f, 0.125f, 0.222f, 0.861f);
            String s = testCase.ToString ();
            Assert.That(s, Is.EqualTo("6E341C6A100053F7"));
        }

        /// <summary>
        /// Makes sure that the hashing function is good by testing
        /// random scenarios and ensuring that there are no more than a
        /// reasonable number of collisions.
        /// </summary>
        [Test]
        public void TestMemberFn_GetHashCode_i ()
        {
            HashSet<Int32> hs = new HashSet<Int32>();
            var rand = new System.Random();
            var buff = new Byte[8];
            UInt32 collisions = 0;
            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt64 packed = BitConverter.ToUInt64(buff, 0);
                var packedObj = new NormalisedShort4();
                packedObj.PackedValue = packed;
                Int32 hc = packedObj.GetHashCode ();
                if(hs.Contains(hc)) ++collisions;
                hs.Add(hc);
            }
            Assert.That(collisions, Is.LessThan(10));
        }
    }

    /// <summary>
    /// Tests the Rg32 packed data type.
    /// </summary>
    [TestFixture]
    public class Rg32Tests
    {
        /// <summary>
        /// Iterates over a random selection of values within the range of
        /// possible Rg32 values and makes sure that unpacking them and
        /// then re-packing that result yeilds the original packed value.
        /// </summary>
        [Test]
        public void TestRandomValues_i()
        {
            var rand = new System.Random();
            var buff = new Byte[4];

            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt32 packed = BitConverter.ToUInt32(buff, 0);
                var packedObj = new Rg32();
                packedObj.PackedValue = packed;
                Single realR, realG = 0f;
                packedObj.UnpackTo(out realR, out realG);
                var newPackedObj = new Rg32(realR, realG);
                Assert.That(newPackedObj.PackedValue, Is.EqualTo(packed));
            }
        }

        /// <summary>
        /// For a given example, this test ensures that the ToString function
        /// yields the expected string.
        /// </summary>
        [Test]
        public void TestMemberFn_ToString_i()
        {
            var testCase = new Rg32();
            testCase.PackFrom(0.222f, 0.861f);
            String s = testCase.ToString ();
            Assert.That(s, Is.EqualTo("DC6A38D5"));
        }

        /// <summary>
        /// Makes sure that the hashing function is good by testing
        /// random scenarios and ensuring that there are no more than a
        /// reasonable number of collisions.
        /// </summary>
        [Test]
        public void TestMemberFn_GetHashCode_i ()
        {
            HashSet<Int32> hs = new HashSet<Int32>();
            var rand = new System.Random();
            var buff = new Byte[4];
            UInt32 collisions = 0;
            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt32 packed = BitConverter.ToUInt32(buff, 0);
                var packedObj = new Rgba64();
                packedObj.PackedValue = packed;
                Int32 hc = packedObj.GetHashCode ();
                if(hs.Contains(hc)) ++collisions;
                hs.Add(hc);
            }
            Assert.That(collisions, Is.LessThan(10));
        }
    }

    /// <summary>
    /// Tests the Rgba32 packed data type.
    /// </summary>
    [TestFixture]
    public class Rgba32Tests
    {
        /// <summary>
        /// Iterates over a random selection of values within the range of
        /// possible Rgba32 values and makes sure that unpacking them and
        /// then re-packing that result yeilds the original packed value.
        /// </summary>
        [Test]
        public void TestRandomValues_i()
        {
            var rand = new System.Random();
            var buff = new Byte[4];

            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt32 packed = BitConverter.ToUInt32(buff, 0);
                var packedObj = new Rgba32();
                packedObj.PackedValue = packed;
                Single realR, realG, realB, realA = 0f;
                packedObj.UnpackTo(out realR, out realG, out realB, out realA);
                var newPackedObj = new Rgba32(realR, realG, realB, realA);
                Assert.That(newPackedObj.PackedValue, Is.EqualTo(packed));
            }
        }

        /// <summary>
        /// For a given example, this test ensures that the ToString function
        /// yields the expected string.
        /// </summary>
        [Test]
        public void TestMemberFn_ToString_i()
        {
            var testCase = new Rgba32();
            testCase.PackFrom(0.656f, 0.125f, 0.222f, 0.861f);
            String s = testCase.ToString ();
            Assert.That(s, Is.EqualTo("{R:167 G:32 B:57 A:220}"));
        }

        /// <summary>
        /// Makes sure that the hashing function is good by testing
        /// random scenarios and ensuring that there are no more than a
        /// reasonable number of collisions.
        /// </summary>
        [Test]
        public void TestMemberFn_GetHashCode_i ()
        {
            HashSet<Int32> hs = new HashSet<Int32>();
            var rand = new System.Random();
            var buff = new Byte[4];
            UInt32 collisions = 0;
            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt32 packed = BitConverter.ToUInt32(buff, 0);
                var packedObj = new Rgba32();
                packedObj.PackedValue = packed;
                Int32 hc = packedObj.GetHashCode ();
                if(hs.Contains(hc)) ++collisions;
                hs.Add(hc);
            }
            Assert.That(collisions, Is.LessThan(10));
        }

        /// <summary>
        /// Tests that the Transparent constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Transparent_i ()
        {
            var val = Rgba32.Transparent;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(0));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that the AliceBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_AliceBlue_i ()
        {
            var val = Rgba32.AliceBlue;
            Assert.That(val.R, Is.EqualTo(240));
            Assert.That(val.G, Is.EqualTo(248));
            Assert.That(val.B, Is.EqualTo(255));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the AntiqueWhite constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_AntiqueWhite_i ()
        {
            var val = Rgba32.AntiqueWhite;
            Assert.That(val.R, Is.EqualTo(250));
            Assert.That(val.G, Is.EqualTo(235));
            Assert.That(val.B, Is.EqualTo(215));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Aqua constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Aqua_i ()
        {
            var val = Rgba32.Aqua;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(255));
            Assert.That(val.B, Is.EqualTo(255));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Aquamarine constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Aquamarine_i ()
        {
            var val = Rgba32.Aquamarine;
            Assert.That(val.R, Is.EqualTo(127));
            Assert.That(val.G, Is.EqualTo(255));
            Assert.That(val.B, Is.EqualTo(212));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Azure constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Azure_i ()
        {
            var val = Rgba32.Azure;
            Assert.That(val.R, Is.EqualTo(240));
            Assert.That(val.G, Is.EqualTo(255));
            Assert.That(val.B, Is.EqualTo(255));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Beige constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Beige_i ()
        {
            var val = Rgba32.Beige;
            Assert.That(val.R, Is.EqualTo(245));
            Assert.That(val.G, Is.EqualTo(245));
            Assert.That(val.B, Is.EqualTo(220));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Bisque constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Bisque_i ()
        {
            var val = Rgba32.Bisque;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(228));
            Assert.That(val.B, Is.EqualTo(196));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Black constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Black_i ()
        {
            var val = Rgba32.Black;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(0));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the BlanchedAlmond constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_BlanchedAlmond_i ()
        {
            var val = Rgba32.BlanchedAlmond;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(235));
            Assert.That(val.B, Is.EqualTo(205));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Blue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Blue_i ()
        {
            var val = Rgba32.Blue;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(0));
            Assert.That(val.B, Is.EqualTo(255));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the BlueViolet constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_BlueViolet_i ()
        {
            var val = Rgba32.BlueViolet;
            Assert.That(val.R, Is.EqualTo(138));
            Assert.That(val.G, Is.EqualTo(43));
            Assert.That(val.B, Is.EqualTo(226));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Brown constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Brown_i ()
        {
            var val = Rgba32.Brown;
            Assert.That(val.R, Is.EqualTo(165));
            Assert.That(val.G, Is.EqualTo(42));
            Assert.That(val.B, Is.EqualTo(42));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the BurlyWood constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_BurlyWood_i ()
        {
            var val = Rgba32.BurlyWood;
            Assert.That(val.R, Is.EqualTo(222));
            Assert.That(val.G, Is.EqualTo(184));
            Assert.That(val.B, Is.EqualTo(135));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the CadetBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_CadetBlue_i ()
        {
            var val = Rgba32.CadetBlue;
            Assert.That(val.R, Is.EqualTo(95));
            Assert.That(val.G, Is.EqualTo(158));
            Assert.That(val.B, Is.EqualTo(160));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Chartreuse constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Chartreuse_i ()
        {
            var val = Rgba32.Chartreuse;
            Assert.That(val.R, Is.EqualTo(127));
            Assert.That(val.G, Is.EqualTo(255));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Chocolate constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Chocolate_i ()
        {
            var val = Rgba32.Chocolate;
            Assert.That(val.R, Is.EqualTo(210));
            Assert.That(val.G, Is.EqualTo(105));
            Assert.That(val.B, Is.EqualTo(30));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Coral constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Coral_i ()
        {
            var val = Rgba32.Coral;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(127));
            Assert.That(val.B, Is.EqualTo(80));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the CornflowerBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_CornflowerBlue_i ()
        {
            var val = Rgba32.CornflowerBlue;
            Assert.That(val.R, Is.EqualTo(100));
            Assert.That(val.G, Is.EqualTo(149));
            Assert.That(val.B, Is.EqualTo(237));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Cornsilk constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Cornsilk_i ()
        {
            var val = Rgba32.Cornsilk;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(248));
            Assert.That(val.B, Is.EqualTo(220));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Crimson constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Crimson_i ()
        {
            var val = Rgba32.Crimson;
            Assert.That(val.R, Is.EqualTo(220));
            Assert.That(val.G, Is.EqualTo(20));
            Assert.That(val.B, Is.EqualTo(60));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Cyan constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Cyan_i ()
        {
            var val = Rgba32.Cyan;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(255));
            Assert.That(val.B, Is.EqualTo(255));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkBlue_i ()
        {
            var val = Rgba32.DarkBlue;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(0));
            Assert.That(val.B, Is.EqualTo(139));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkCyan constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkCyan_i ()
        {
            var val = Rgba32.DarkCyan;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(139));
            Assert.That(val.B, Is.EqualTo(139));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkGoldenrod constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkGoldenrod_i ()
        {
            var val = Rgba32.DarkGoldenrod;
            Assert.That(val.R, Is.EqualTo(184));
            Assert.That(val.G, Is.EqualTo(134));
            Assert.That(val.B, Is.EqualTo(11));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkGrey constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkGrey_i ()
        {
            var val = Rgba32.DarkGrey;
            Assert.That(val.R, Is.EqualTo(169));
            Assert.That(val.G, Is.EqualTo(169));
            Assert.That(val.B, Is.EqualTo(169));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkGreen constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkGreen_i ()
        {
            var val = Rgba32.DarkGreen;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(100));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkKhaki constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkKhaki_i ()
        {
            var val = Rgba32.DarkKhaki;
            Assert.That(val.R, Is.EqualTo(189));
            Assert.That(val.G, Is.EqualTo(183));
            Assert.That(val.B, Is.EqualTo(107));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkMagenta constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkMagenta_i ()
        {
            var val = Rgba32.DarkMagenta;
            Assert.That(val.R, Is.EqualTo(139));
            Assert.That(val.G, Is.EqualTo(0));
            Assert.That(val.B, Is.EqualTo(139));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkOliveGreen constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkOliveGreen_i ()
        {
            var val = Rgba32.DarkOliveGreen;
            Assert.That(val.R, Is.EqualTo(85));
            Assert.That(val.G, Is.EqualTo(107));
            Assert.That(val.B, Is.EqualTo(47));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkOrange constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkOrange_i ()
        {
            var val = Rgba32.DarkOrange;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(140));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkOrchid constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkOrchid_i ()
        {
            var val = Rgba32.DarkOrchid;
            Assert.That(val.R, Is.EqualTo(153));
            Assert.That(val.G, Is.EqualTo(50));
            Assert.That(val.B, Is.EqualTo(204));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkRed constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkRed_i ()
        {
            var val = Rgba32.DarkRed;
            Assert.That(val.R, Is.EqualTo(139));
            Assert.That(val.G, Is.EqualTo(0));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkSalmon constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkSalmon_i ()
        {
            var val = Rgba32.DarkSalmon;
            Assert.That(val.R, Is.EqualTo(233));
            Assert.That(val.G, Is.EqualTo(150));
            Assert.That(val.B, Is.EqualTo(122));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkSeaGreen constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkSeaGreen_i ()
        {
            var val = Rgba32.DarkSeaGreen;
            Assert.That(val.R, Is.EqualTo(143));
            Assert.That(val.G, Is.EqualTo(188));
            Assert.That(val.B, Is.EqualTo(139));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkSlateBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkSlateBlue_i ()
        {
            var val = Rgba32.DarkSlateBlue;
            Assert.That(val.R, Is.EqualTo(72));
            Assert.That(val.G, Is.EqualTo(61));
            Assert.That(val.B, Is.EqualTo(139));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkSlateGrey constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkSlateGrey_i ()
        {
            var val = Rgba32.DarkSlateGrey;
            Assert.That(val.R, Is.EqualTo(47));
            Assert.That(val.G, Is.EqualTo(79));
            Assert.That(val.B, Is.EqualTo(79));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkTurquoise constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkTurquoise_i ()
        {
            var val = Rgba32.DarkTurquoise;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(206));
            Assert.That(val.B, Is.EqualTo(209));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DarkViolet constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DarkViolet_i ()
        {
            var val = Rgba32.DarkViolet;
            Assert.That(val.R, Is.EqualTo(148));
            Assert.That(val.G, Is.EqualTo(0));
            Assert.That(val.B, Is.EqualTo(211));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DeepPink constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DeepPink_i ()
        {
            var val = Rgba32.DeepPink;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(20));
            Assert.That(val.B, Is.EqualTo(147));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DeepSkyBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DeepSkyBlue_i ()
        {
            var val = Rgba32.DeepSkyBlue;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(191));
            Assert.That(val.B, Is.EqualTo(255));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DimGrey constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DimGrey_i ()
        {
            var val = Rgba32.DimGrey;
            Assert.That(val.R, Is.EqualTo(105));
            Assert.That(val.G, Is.EqualTo(105));
            Assert.That(val.B, Is.EqualTo(105));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the DodgerBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_DodgerBlue_i ()
        {
            var val = Rgba32.DodgerBlue;
            Assert.That(val.R, Is.EqualTo(30));
            Assert.That(val.G, Is.EqualTo(144));
            Assert.That(val.B, Is.EqualTo(255));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Firebrick constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Firebrick_i ()
        {
            var val = Rgba32.Firebrick;
            Assert.That(val.R, Is.EqualTo(178));
            Assert.That(val.G, Is.EqualTo(34));
            Assert.That(val.B, Is.EqualTo(34));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the FloralWhite constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_FloralWhite_i ()
        {
            var val = Rgba32.FloralWhite;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(250));
            Assert.That(val.B, Is.EqualTo(240));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the ForestGreen constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_ForestGreen_i ()
        {
            var val = Rgba32.ForestGreen;
            Assert.That(val.R, Is.EqualTo(34));
            Assert.That(val.G, Is.EqualTo(139));
            Assert.That(val.B, Is.EqualTo(34));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Fuchsia constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Fuchsia_i ()
        {
            var val = Rgba32.Fuchsia;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(0));
            Assert.That(val.B, Is.EqualTo(255));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Gainsboro constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Gainsboro_i ()
        {
            var val = Rgba32.Gainsboro;
            Assert.That(val.R, Is.EqualTo(220));
            Assert.That(val.G, Is.EqualTo(220));
            Assert.That(val.B, Is.EqualTo(220));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the GhostWhite constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_GhostWhite_i ()
        {
            var val = Rgba32.GhostWhite;
            Assert.That(val.R, Is.EqualTo(248));
            Assert.That(val.G, Is.EqualTo(248));
            Assert.That(val.B, Is.EqualTo(255));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Gold constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Gold_i ()
        {
            var val = Rgba32.Gold;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(215));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Goldenrod constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Goldenrod_i ()
        {
            var val = Rgba32.Goldenrod;
            Assert.That(val.R, Is.EqualTo(218));
            Assert.That(val.G, Is.EqualTo(165));
            Assert.That(val.B, Is.EqualTo(32));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Grey constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Grey_i ()
        {
            var val = Rgba32.Grey;
            Assert.That(val.R, Is.EqualTo(128));
            Assert.That(val.G, Is.EqualTo(128));
            Assert.That(val.B, Is.EqualTo(128));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Green constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Green_i ()
        {
            var val = Rgba32.Green;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(128));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the GreenYellow constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_GreenYellow_i ()
        {
            var val = Rgba32.GreenYellow;
            Assert.That(val.R, Is.EqualTo(173));
            Assert.That(val.G, Is.EqualTo(255));
            Assert.That(val.B, Is.EqualTo(47));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Honeydew constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Honeydew_i ()
        {
            var val = Rgba32.Honeydew;
            Assert.That(val.R, Is.EqualTo(240));
            Assert.That(val.G, Is.EqualTo(255));
            Assert.That(val.B, Is.EqualTo(240));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the HotPink constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_HotPink_i ()
        {
            var val = Rgba32.HotPink;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(105));
            Assert.That(val.B, Is.EqualTo(180));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the IndianRed constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_IndianRed_i ()
        {
            var val = Rgba32.IndianRed;
            Assert.That(val.R, Is.EqualTo(205));
            Assert.That(val.G, Is.EqualTo(92));
            Assert.That(val.B, Is.EqualTo(92));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Indigo constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Indigo_i ()
        {
            var val = Rgba32.Indigo;
            Assert.That(val.R, Is.EqualTo(75));
            Assert.That(val.G, Is.EqualTo(0));
            Assert.That(val.B, Is.EqualTo(130));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Ivory constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Ivory_i ()
        {
            var val = Rgba32.Ivory;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(255));
            Assert.That(val.B, Is.EqualTo(240));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Khaki constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Khaki_i ()
        {
            var val = Rgba32.Khaki;
            Assert.That(val.R, Is.EqualTo(240));
            Assert.That(val.G, Is.EqualTo(230));
            Assert.That(val.B, Is.EqualTo(140));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Lavender constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Lavender_i ()
        {
            var val = Rgba32.Lavender;
            Assert.That(val.R, Is.EqualTo(230));
            Assert.That(val.G, Is.EqualTo(230));
            Assert.That(val.B, Is.EqualTo(250));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LavenderBlush constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LavenderBlush_i ()
        {
            var val = Rgba32.LavenderBlush;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(240));
            Assert.That(val.B, Is.EqualTo(245));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LawnGreen constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LawnGreen_i ()
        {
            var val = Rgba32.LawnGreen;
            Assert.That(val.R, Is.EqualTo(124));
            Assert.That(val.G, Is.EqualTo(252));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LemonChiffon constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LemonChiffon_i ()
        {
            var val = Rgba32.LemonChiffon;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(250));
            Assert.That(val.B, Is.EqualTo(205));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LightBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LightBlue_i ()
        {
            var val = Rgba32.LightBlue;
            Assert.That(val.R, Is.EqualTo(173));
            Assert.That(val.G, Is.EqualTo(216));
            Assert.That(val.B, Is.EqualTo(230));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LightCoral constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LightCoral_i ()
        {
            var val = Rgba32.LightCoral;
            Assert.That(val.R, Is.EqualTo(240));
            Assert.That(val.G, Is.EqualTo(128));
            Assert.That(val.B, Is.EqualTo(128));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LightCyan constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LightCyan_i ()
        {
            var val = Rgba32.LightCyan;
            Assert.That(val.R, Is.EqualTo(224));
            Assert.That(val.G, Is.EqualTo(255));
            Assert.That(val.B, Is.EqualTo(255));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LightGoldenrodYellow constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LightGoldenrodYellow_i ()
        {
            var val = Rgba32.LightGoldenrodYellow;
            Assert.That(val.R, Is.EqualTo(250));
            Assert.That(val.G, Is.EqualTo(250));
            Assert.That(val.B, Is.EqualTo(210));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LightGreen constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LightGreen_i ()
        {
            var val = Rgba32.LightGreen;
            Assert.That(val.R, Is.EqualTo(144));
            Assert.That(val.G, Is.EqualTo(238));
            Assert.That(val.B, Is.EqualTo(144));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LightGrey constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LightGrey_i ()
        {
            var val = Rgba32.LightGrey;
            Assert.That(val.R, Is.EqualTo(211));
            Assert.That(val.G, Is.EqualTo(211));
            Assert.That(val.B, Is.EqualTo(211));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LightPink constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LightPink_i ()
        {
            var val = Rgba32.LightPink;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(182));
            Assert.That(val.B, Is.EqualTo(193));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LightSalmon constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LightSalmon_i ()
        {
            var val = Rgba32.LightSalmon;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(160));
            Assert.That(val.B, Is.EqualTo(122));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LightSeaGreen constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LightSeaGreen_i ()
        {
            var val = Rgba32.LightSeaGreen;
            Assert.That(val.R, Is.EqualTo(32));
            Assert.That(val.G, Is.EqualTo(178));
            Assert.That(val.B, Is.EqualTo(170));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LightSkyBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LightSkyBlue_i ()
        {
            var val = Rgba32.LightSkyBlue;
            Assert.That(val.R, Is.EqualTo(135));
            Assert.That(val.G, Is.EqualTo(206));
            Assert.That(val.B, Is.EqualTo(250));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LightSlateGrey constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LightSlateGrey_i ()
        {
            var val = Rgba32.LightSlateGrey;
            Assert.That(val.R, Is.EqualTo(119));
            Assert.That(val.G, Is.EqualTo(136));
            Assert.That(val.B, Is.EqualTo(153));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LightSteelBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LightSteelBlue_i ()
        {
            var val = Rgba32.LightSteelBlue;
            Assert.That(val.R, Is.EqualTo(176));
            Assert.That(val.G, Is.EqualTo(196));
            Assert.That(val.B, Is.EqualTo(222));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LightYellow constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LightYellow_i ()
        {
            var val = Rgba32.LightYellow;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(255));
            Assert.That(val.B, Is.EqualTo(224));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Lime constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Lime_i ()
        {
            var val = Rgba32.Lime;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(255));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the LimeGreen constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_LimeGreen_i ()
        {
            var val = Rgba32.LimeGreen;
            Assert.That(val.R, Is.EqualTo(50));
            Assert.That(val.G, Is.EqualTo(205));
            Assert.That(val.B, Is.EqualTo(50));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Linen constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Linen_i ()
        {
            var val = Rgba32.Linen;
            Assert.That(val.R, Is.EqualTo(250));
            Assert.That(val.G, Is.EqualTo(240));
            Assert.That(val.B, Is.EqualTo(230));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Magenta constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Magenta_i ()
        {
            var val = Rgba32.Magenta;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(0));
            Assert.That(val.B, Is.EqualTo(255));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Maroon constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Maroon_i ()
        {
            var val = Rgba32.Maroon;
            Assert.That(val.R, Is.EqualTo(128));
            Assert.That(val.G, Is.EqualTo(0));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the MediumAquamarine constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_MediumAquamarine_i ()
        {
            var val = Rgba32.MediumAquamarine;
            Assert.That(val.R, Is.EqualTo(102));
            Assert.That(val.G, Is.EqualTo(205));
            Assert.That(val.B, Is.EqualTo(170));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the MediumBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_MediumBlue_i ()
        {
            var val = Rgba32.MediumBlue;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(0));
            Assert.That(val.B, Is.EqualTo(205));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the MediumOrchid constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_MediumOrchid_i ()
        {
            var val = Rgba32.MediumOrchid;
            Assert.That(val.R, Is.EqualTo(186));
            Assert.That(val.G, Is.EqualTo(85));
            Assert.That(val.B, Is.EqualTo(211));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the MediumPurple constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_MediumPurple_i ()
        {
            var val = Rgba32.MediumPurple;
            Assert.That(val.R, Is.EqualTo(147));
            Assert.That(val.G, Is.EqualTo(112));
            Assert.That(val.B, Is.EqualTo(219));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the MediumSeaGreen constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_MediumSeaGreen_i ()
        {
            var val = Rgba32.MediumSeaGreen;
            Assert.That(val.R, Is.EqualTo(60));
            Assert.That(val.G, Is.EqualTo(179));
            Assert.That(val.B, Is.EqualTo(113));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the MediumSlateBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_MediumSlateBlue_i ()
        {
            var val = Rgba32.MediumSlateBlue;
            Assert.That(val.R, Is.EqualTo(123));
            Assert.That(val.G, Is.EqualTo(104));
            Assert.That(val.B, Is.EqualTo(238));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the MediumSpringGreen constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_MediumSpringGreen_i ()
        {
            var val = Rgba32.MediumSpringGreen;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(250));
            Assert.That(val.B, Is.EqualTo(154));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the MediumTurquoise constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_MediumTurquoise_i ()
        {
            var val = Rgba32.MediumTurquoise;
            Assert.That(val.R, Is.EqualTo(72));
            Assert.That(val.G, Is.EqualTo(209));
            Assert.That(val.B, Is.EqualTo(204));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the MediumVioletRed constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_MediumVioletRed_i ()
        {
            var val = Rgba32.MediumVioletRed;
            Assert.That(val.R, Is.EqualTo(199));
            Assert.That(val.G, Is.EqualTo(21));
            Assert.That(val.B, Is.EqualTo(133));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the MidnightBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_MidnightBlue_i ()
        {
            var val = Rgba32.MidnightBlue;
            Assert.That(val.R, Is.EqualTo(25));
            Assert.That(val.G, Is.EqualTo(25));
            Assert.That(val.B, Is.EqualTo(112));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the MintCream constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_MintCream_i ()
        {
            var val = Rgba32.MintCream;
            Assert.That(val.R, Is.EqualTo(245));
            Assert.That(val.G, Is.EqualTo(255));
            Assert.That(val.B, Is.EqualTo(250));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the MistyRose constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_MistyRose_i ()
        {
            var val = Rgba32.MistyRose;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(228));
            Assert.That(val.B, Is.EqualTo(225));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Moccasin constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Moccasin_i ()
        {
            var val = Rgba32.Moccasin;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(228));
            Assert.That(val.B, Is.EqualTo(181));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the NavajoWhite constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_NavajoWhite_i ()
        {
            var val = Rgba32.NavajoWhite;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(222));
            Assert.That(val.B, Is.EqualTo(173));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Navy constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Navy_i ()
        {
            var val = Rgba32.Navy;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(0));
            Assert.That(val.B, Is.EqualTo(128));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the OldLace constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_OldLace_i ()
        {
            var val = Rgba32.OldLace;
            Assert.That(val.R, Is.EqualTo(253));
            Assert.That(val.G, Is.EqualTo(245));
            Assert.That(val.B, Is.EqualTo(230));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Olive constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Olive_i ()
        {
            var val = Rgba32.Olive;
            Assert.That(val.R, Is.EqualTo(128));
            Assert.That(val.G, Is.EqualTo(128));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the OliveDrab constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_OliveDrab_i ()
        {
            var val = Rgba32.OliveDrab;
            Assert.That(val.R, Is.EqualTo(107));
            Assert.That(val.G, Is.EqualTo(142));
            Assert.That(val.B, Is.EqualTo(35));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Orange constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Orange_i ()
        {
            var val = Rgba32.Orange;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(165));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the OrangeRed constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_OrangeRed_i ()
        {
            var val = Rgba32.OrangeRed;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(69));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Orchid constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Orchid_i ()
        {
            var val = Rgba32.Orchid;
            Assert.That(val.R, Is.EqualTo(218));
            Assert.That(val.G, Is.EqualTo(112));
            Assert.That(val.B, Is.EqualTo(214));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the PaleGoldenrod constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_PaleGoldenrod_i ()
        {
            var val = Rgba32.PaleGoldenrod;
            Assert.That(val.R, Is.EqualTo(238));
            Assert.That(val.G, Is.EqualTo(232));
            Assert.That(val.B, Is.EqualTo(170));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the PaleGreen constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_PaleGreen_i ()
        {
            var val = Rgba32.PaleGreen;
            Assert.That(val.R, Is.EqualTo(152));
            Assert.That(val.G, Is.EqualTo(251));
            Assert.That(val.B, Is.EqualTo(152));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the PaleTurquoise constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_PaleTurquoise_i ()
        {
            var val = Rgba32.PaleTurquoise;
            Assert.That(val.R, Is.EqualTo(175));
            Assert.That(val.G, Is.EqualTo(238));
            Assert.That(val.B, Is.EqualTo(238));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the PaleVioletRed constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_PaleVioletRed_i ()
        {
            var val = Rgba32.PaleVioletRed;
            Assert.That(val.R, Is.EqualTo(219));
            Assert.That(val.G, Is.EqualTo(112));
            Assert.That(val.B, Is.EqualTo(147));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the PapayaWhip constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_PapayaWhip_i ()
        {
            var val = Rgba32.PapayaWhip;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(239));
            Assert.That(val.B, Is.EqualTo(213));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the PeachPuff constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_PeachPuff_i ()
        {
            var val = Rgba32.PeachPuff;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(218));
            Assert.That(val.B, Is.EqualTo(185));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Peru constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Peru_i ()
        {
            var val = Rgba32.Peru;
            Assert.That(val.R, Is.EqualTo(205));
            Assert.That(val.G, Is.EqualTo(133));
            Assert.That(val.B, Is.EqualTo(63));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Pink constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Pink_i ()
        {
            var val = Rgba32.Pink;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(192));
            Assert.That(val.B, Is.EqualTo(203));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Plum constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Plum_i ()
        {
            var val = Rgba32.Plum;
            Assert.That(val.R, Is.EqualTo(221));
            Assert.That(val.G, Is.EqualTo(160));
            Assert.That(val.B, Is.EqualTo(221));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the PowderBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_PowderBlue_i ()
        {
            var val = Rgba32.PowderBlue;
            Assert.That(val.R, Is.EqualTo(176));
            Assert.That(val.G, Is.EqualTo(224));
            Assert.That(val.B, Is.EqualTo(230));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Purple constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Purple_i ()
        {
            var val = Rgba32.Purple;
            Assert.That(val.R, Is.EqualTo(128));
            Assert.That(val.G, Is.EqualTo(0));
            Assert.That(val.B, Is.EqualTo(128));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Red constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Red_i ()
        {
            var val = Rgba32.Red;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(0));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the RosyBrown constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_RosyBrown_i ()
        {
            var val = Rgba32.RosyBrown;
            Assert.That(val.R, Is.EqualTo(188));
            Assert.That(val.G, Is.EqualTo(143));
            Assert.That(val.B, Is.EqualTo(143));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the RoyalBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_RoyalBlue_i ()
        {
            var val = Rgba32.RoyalBlue;
            Assert.That(val.R, Is.EqualTo(65));
            Assert.That(val.G, Is.EqualTo(105));
            Assert.That(val.B, Is.EqualTo(225));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the SaddleBrown constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_SaddleBrown_i ()
        {
            var val = Rgba32.SaddleBrown;
            Assert.That(val.R, Is.EqualTo(139));
            Assert.That(val.G, Is.EqualTo(69));
            Assert.That(val.B, Is.EqualTo(19));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Salmon constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Salmon_i ()
        {
            var val = Rgba32.Salmon;
            Assert.That(val.R, Is.EqualTo(250));
            Assert.That(val.G, Is.EqualTo(128));
            Assert.That(val.B, Is.EqualTo(114));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the SandyBrown constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_SandyBrown_i ()
        {
            var val = Rgba32.SandyBrown;
            Assert.That(val.R, Is.EqualTo(244));
            Assert.That(val.G, Is.EqualTo(164));
            Assert.That(val.B, Is.EqualTo(96));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the SeaGreen constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_SeaGreen_i ()
        {
            var val = Rgba32.SeaGreen;
            Assert.That(val.R, Is.EqualTo(46));
            Assert.That(val.G, Is.EqualTo(139));
            Assert.That(val.B, Is.EqualTo(87));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the SeaShell constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_SeaShell_i ()
        {
            var val = Rgba32.SeaShell;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(245));
            Assert.That(val.B, Is.EqualTo(238));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Sienna constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Sienna_i ()
        {
            var val = Rgba32.Sienna;
            Assert.That(val.R, Is.EqualTo(160));
            Assert.That(val.G, Is.EqualTo(82));
            Assert.That(val.B, Is.EqualTo(45));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Silver constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Silver_i ()
        {
            var val = Rgba32.Silver;
            Assert.That(val.R, Is.EqualTo(192));
            Assert.That(val.G, Is.EqualTo(192));
            Assert.That(val.B, Is.EqualTo(192));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the SkyBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_SkyBlue_i ()
        {
            var val = Rgba32.SkyBlue;
            Assert.That(val.R, Is.EqualTo(135));
            Assert.That(val.G, Is.EqualTo(206));
            Assert.That(val.B, Is.EqualTo(235));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the SlateBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_SlateBlue_i ()
        {
            var val = Rgba32.SlateBlue;
            Assert.That(val.R, Is.EqualTo(106));
            Assert.That(val.G, Is.EqualTo(90));
            Assert.That(val.B, Is.EqualTo(205));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the SlateGrey constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_SlateGrey_i ()
        {
            var val = Rgba32.SlateGrey;
            Assert.That(val.R, Is.EqualTo(112));
            Assert.That(val.G, Is.EqualTo(128));
            Assert.That(val.B, Is.EqualTo(144));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Snow constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Snow_i ()
        {
            var val = Rgba32.Snow;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(250));
            Assert.That(val.B, Is.EqualTo(250));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the SpringGreen constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_SpringGreen_i ()
        {
            var val = Rgba32.SpringGreen;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(255));
            Assert.That(val.B, Is.EqualTo(127));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the SteelBlue constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_SteelBlue_i ()
        {
            var val = Rgba32.SteelBlue;
            Assert.That(val.R, Is.EqualTo(70));
            Assert.That(val.G, Is.EqualTo(130));
            Assert.That(val.B, Is.EqualTo(180));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Tan constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Tan_i ()
        {
            var val = Rgba32.Tan;
            Assert.That(val.R, Is.EqualTo(210));
            Assert.That(val.G, Is.EqualTo(180));
            Assert.That(val.B, Is.EqualTo(140));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Teal constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Teal_i ()
        {
            var val = Rgba32.Teal;
            Assert.That(val.R, Is.EqualTo(0));
            Assert.That(val.G, Is.EqualTo(128));
            Assert.That(val.B, Is.EqualTo(128));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Thistle constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Thistle_i ()
        {
            var val = Rgba32.Thistle;
            Assert.That(val.R, Is.EqualTo(216));
            Assert.That(val.G, Is.EqualTo(191));
            Assert.That(val.B, Is.EqualTo(216));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Tomato constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Tomato_i ()
        {
            var val = Rgba32.Tomato;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(99));
            Assert.That(val.B, Is.EqualTo(71));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Turquoise constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Turquoise_i ()
        {
            var val = Rgba32.Turquoise;
            Assert.That(val.R, Is.EqualTo(64));
            Assert.That(val.G, Is.EqualTo(224));
            Assert.That(val.B, Is.EqualTo(208));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Violet constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Violet_i ()
        {
            var val = Rgba32.Violet;
            Assert.That(val.R, Is.EqualTo(238));
            Assert.That(val.G, Is.EqualTo(130));
            Assert.That(val.B, Is.EqualTo(238));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Wheat constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Wheat_i ()
        {
            var val = Rgba32.Wheat;
            Assert.That(val.R, Is.EqualTo(245));
            Assert.That(val.G, Is.EqualTo(222));
            Assert.That(val.B, Is.EqualTo(179));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the White constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_White_i ()
        {
            var val = Rgba32.White;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(255));
            Assert.That(val.B, Is.EqualTo(255));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the WhiteSmoke constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_WhiteSmoke_i ()
        {
            var val = Rgba32.WhiteSmoke;
            Assert.That(val.R, Is.EqualTo(245));
            Assert.That(val.G, Is.EqualTo(245));
            Assert.That(val.B, Is.EqualTo(245));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the Yellow constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_Yellow_i ()
        {
            var val = Rgba32.Yellow;
            Assert.That(val.R, Is.EqualTo(255));
            Assert.That(val.G, Is.EqualTo(255));
            Assert.That(val.B, Is.EqualTo(0));
            Assert.That(val.A, Is.EqualTo(255));
        }

        /// <summary>
        /// Tests that the YellowGreen constant yeilds the correct value.
        /// </summary>
        [Test]
        public void TestConstant_YellowGreen_i ()
        {
            var val = Rgba32.YellowGreen;
            Assert.That(val.R, Is.EqualTo(154));
            Assert.That(val.G, Is.EqualTo(205));
            Assert.That(val.B, Is.EqualTo(50));
            Assert.That(val.A, Is.EqualTo(255));
        }


        /// <summary>
        /// todo
        /// </summary>
        [Test]
        public void Test_Constructors_ii ()
        {
            throw new InconclusiveException("Not Implemented");
        }

        /// <summary>
        /// todo
        /// </summary>
        [Test]
        public void TestStaticFn_FromNonPremultiplied_i ()
        {
            throw new InconclusiveException ("Not Implemented");
        }

        /// <summary>
        /// todo
        /// </summary>
        [Test]
        public void TestStaticFn_FromNonPremultiplied_ii ()
        {
            throw new InconclusiveException ("Not Implemented");
        }

        /// <summary>
        /// todo
        /// </summary>
        [Test]
        public void TestStaticFn_Lerp_i ()
        {
            throw new InconclusiveException ("Not Implemented");
        }

        /// <summary>
        /// todo
        /// </summary>
        [Test]
        public void TestMemberFn_ToVector3_i ()
        {
            throw new InconclusiveException ("Not Implemented");
        }

        /// <summary>
        /// todo
        /// </summary>
        [Test]
        public void TestStaticFn_Desaturate_i ()
        {
            throw new InconclusiveException ("Not Implemented");
        }

        void TestMultiplication ()
        {
            throw new InconclusiveException("Not Implemented");
        }

        /// <summary>
        /// Assert that, for a known example, all the multiplication opperators
        /// and functions yield the correct result.
        /// </summary>
        [Test]
        public void TestOperator_Multiplication_i ()
        {
            this.TestMultiplication();
        }
    }

    /// <summary>
    /// Tests the Rgba64 packed data type.
    /// </summary>
    [TestFixture]
    public class Rgba64Tests
    {
        /// <summary>
        /// Iterates over a random selection of values within the range of
        /// possible Rgba64 values and makes sure that unpacking them and
        /// then re-packing that result yeilds the original packed value.
        /// </summary>
        [Test]
        public void TestRandomValues_i()
        {
            var rand = new System.Random();
            var buff = new Byte[8];

            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt64 packed = BitConverter.ToUInt64(buff, 0);
                var packedObj = new Rgba64();
                packedObj.PackedValue = packed;
                Single realR, realG, realB, realA = 0f;
                packedObj.UnpackTo(out realR, out realG, out realB, out realA);
                var newPackedObj = new Rgba64(realR, realG, realB, realA);
                Assert.That(newPackedObj.PackedValue, Is.EqualTo(packed));
            }
        }

        /// <summary>
        /// For a given example, this test ensures that the ToString function
        /// yields the expected string.
        /// </summary>
        [Test]
        public void TestMemberFn_ToString_i()
        {
            var testCase = new Rgba64();
            testCase.PackFrom(0.656f, 0.125f, 0.222f, 0.861f);
            String s = testCase.ToString ();
            Assert.That(s, Is.EqualTo("DC6A38D52000A7EF"));
        }

        /// <summary>
        /// Makes sure that the hashing function is good by testing
        /// random scenarios and ensuring that there are no more than a
        /// reasonable number of collisions.
        /// </summary>
        [Test]
        public void TestMemberFn_GetHashCode_i ()
        {
            HashSet<Int32> hs = new HashSet<Int32>();
            var rand = new System.Random();
            var buff = new Byte[8];
            UInt32 collisions = 0;
            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt64 packed = BitConverter.ToUInt64(buff, 0);
                var packedObj = new Rgba64();
                packedObj.PackedValue = packed;
                Int32 hc = packedObj.GetHashCode ();
                if(hs.Contains(hc)) ++collisions;
                hs.Add(hc);
            }
            Assert.That(collisions, Is.LessThan(10));
        }
    }

    /// <summary>
    /// Tests the Rgba_10_10_10_2 packed data type.
    /// </summary>
    [TestFixture]
    public class Rgba_10_10_10_2Tests
    {
        /// <summary>
        /// Iterates over a random selection of values within the range of
        /// possible Rgba_10_10_10_2 values and makes sure that unpacking them and
        /// then re-packing that result yeilds the original packed value.
        /// </summary>
        [Test]
        public void TestRandomValues_i()
        {
            var rand = new System.Random();
            var buff = new Byte[4];

            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt32 packed = BitConverter.ToUInt32(buff, 0);
                var packedObj = new Rgba_10_10_10_2();
                packedObj.PackedValue = packed;
                Single realR, realG, realB, realA = 0f;
                packedObj.UnpackTo(out realR, out realG, out realB, out realA);
                var newPackedObj = new Rgba_10_10_10_2(realR, realG, realB, realA);
                Assert.That(newPackedObj.PackedValue, Is.EqualTo(packed));
            }
        }

        /// <summary>
        /// For a given example, this test ensures that the ToString function
        /// yields the expected string.
        /// </summary>
        [Test]
        public void TestMemberFn_ToString_i()
        {
            var testCase = new Rgba_10_10_10_2();
            testCase.PackFrom(0.656f, 0.125f, 0.222f, 0.861f);
            String s = testCase.ToString ();
            Assert.That(s, Is.EqualTo("8DD0A7EF"));
        }

        /// <summary>
        /// Makes sure that the hashing function is good by testing
        /// random scenarios and ensuring that there are no more than a
        /// reasonable number of collisions.
        /// </summary>
        [Test]
        public void TestMemberFn_GetHashCode_i ()
        {
            HashSet<Int32> hs = new HashSet<Int32>();
            var rand = new System.Random();
            var buff = new Byte[4];
            UInt32 collisions = 0;
            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt32 packed = BitConverter.ToUInt32(buff, 0);
                var packedObj = new Rgba_10_10_10_2();
                packedObj.PackedValue = packed;
                Int32 hc = packedObj.GetHashCode ();
                if(hs.Contains(hc)) ++collisions;
                hs.Add(hc);
            }
            Assert.That(collisions, Is.LessThan(10));
        }
    }

    /// <summary>
    /// Tests the Short2 packed data type.
    /// </summary>
    [TestFixture]
    public class Short2Tests
    {
        /// <summary>
        /// Iterates over a random selection of values within the range of
        /// possible Short2 values and makes sure that unpacking them and
        /// then re-packing that result yeilds the original packed value.
        /// </summary>
        [Test]
        public void TestRandomValues_i()
        {
            var rand = new System.Random();
            var buff = new Byte[4];

            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt32 packed = BitConverter.ToUInt32(buff, 0);
                var packedObj = new Short2();
                packedObj.PackedValue = packed;
                Single realX, realY = 0f;
                packedObj.UnpackTo(out realX, out realY);
                var newPackedObj = new Short2(realX, realY);
                Assert.That(newPackedObj.PackedValue, Is.EqualTo(packed));
            }
        }

        /// <summary>
        /// For a given example, this test ensures that the ToString function
        /// yields the expected string.
        /// </summary>
        [Test]
        public void TestMemberFn_ToString_i()
        {
            var testCase = new Short2();
            testCase.PackFrom(0.656f, 0.125f);
            String s = testCase.ToString ();
            Assert.That(s, Is.EqualTo("00000001"));
        }

        /// <summary>
        /// Makes sure that the hashing function is good by testing
        /// random scenarios and ensuring that there are no more than a
        /// reasonable number of collisions.
        /// </summary>
        [Test]
        public void TestMemberFn_GetHashCode_i ()
        {
            HashSet<Int32> hs = new HashSet<Int32>();
            var rand = new System.Random();
            var buff = new Byte[4];
            UInt32 collisions = 0;
            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt32 packed = BitConverter.ToUInt32(buff, 0);
                var packedObj = new Short2();
                packedObj.PackedValue = packed;
                Int32 hc = packedObj.GetHashCode ();
                if(hs.Contains(hc)) ++collisions;
                hs.Add(hc);
            }
            Assert.That(collisions, Is.LessThan(10));
        }
    }

    /// <summary>
    /// Tests the Short4 packed data type.
    /// </summary>
    [TestFixture]
    public class Short4Tests
    {
        /// <summary>
        /// Iterates over a random selection of values within the range of
        /// possible Short4 values and makes sure that unpacking them and
        /// then re-packing that result yeilds the original packed value.
        /// </summary>
        [Test]
        public void TestRandomValues_i()
        {
            var rand = new System.Random();
            var buff = new Byte[8];

            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt64 packed = BitConverter.ToUInt64(buff, 0);
                var packedObj = new Short4();
                packedObj.PackedValue = packed;
                Single realX, realY, realZ, realW = 0f;
                packedObj.UnpackTo(out realX, out realY, out realZ, out realW);
                var newPackedObj = new Short4(realX, realY, realZ, realW);
                Assert.That(newPackedObj.PackedValue, Is.EqualTo(packedObj.PackedValue));
            }
        }

        /// <summary>
        /// For a given example, this test ensures that the ToString function
        /// yields the expected string.
        /// </summary>
        [Test]
        public void TestMemberFn_ToString_i()
        {
            var testCase = new Short4 ();
            testCase.PackFrom (0.656f, 0.125f, 0.222f, 0.861f);
            String s = testCase.ToString ();
            Assert.That(s, Is.EqualTo ("1000000000001"));
        }

        /// <summary>
        /// Makes sure that the hashing function is good by testing
        /// random scenarios and ensuring that there are no more than a
        /// reasonable number of collisions.
        /// </summary>
        [Test]
        public void TestMemberFn_GetHashCode_i ()
        {
            HashSet<Int32> hs = new HashSet<Int32>();
            var rand = new System.Random();
            var buff = new Byte[8];
            UInt32 collisions = 0;
            for(Int32 i = 0; i < Settings.NumTests; ++i)
            {
                rand.NextBytes(buff);
                UInt64 packed = BitConverter.ToUInt64(buff, 0);
                var packedObj = new Short4();
                packedObj.PackedValue = packed;
                Int32 hc = packedObj.GetHashCode ();
                if(hs.Contains(hc)) ++collisions;
                hs.Add(hc);
            }
            Assert.That(collisions, Is.LessThan(10));
        }
    }

}

#endif
