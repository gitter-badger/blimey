// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// │ Cor, Blimey!                                                           │ \\
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

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    
    using Fudge;
    using Abacus.SinglePrecision;
    
    using System.Linq;
    using Cor;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class RandomGenerator
    {
        internal Random Random
        {
            get { return random; }
        }

        Random random;

        public static RandomGenerator Default
        {
            get { return defaultGenerator; }
        }

        static RandomGenerator defaultGenerator = new RandomGenerator();

        public RandomGenerator(Int32 seed)
        {
            random = SetSeed(0);
        }

        public RandomGenerator()
        {
            random = SetSeed(0);
        }

        public Random SetSeed(Int32 seed)
        {
            if (seed == 0)
            {
                random = new Random((Int32)DateTime.Now.Ticks);
            }
            else
            {
                random = new Random(seed);
            }

            return random;
        }

        public Single GetRandomSingle(Single min, Single max)
        {
            return ((Single)random.NextDouble() * (max - min)) + min;
        }

        public Int32 GetRandomInt32(Int32 max)
        {
            return random.Next(max);
        }

        public Byte GetRandomByte()
        {
            Byte[] b = new Byte[1];
            random.NextBytes(b);
            return b[0];
        }

        public Boolean GetRandomBoolean()
        {
            Int32 i = random.Next(2);
            if (i > 0)
            {
                return true;
            }

            return false;
        }

        [CLSCompliant(false)]
        public Rgba32 GetRandomColour()
        {
            Single min = 0.25f;
            Single max = 1f;

            Single r = (Single)random.NextDouble() * (max - min) + min;
            Single g = (Single)random.NextDouble() * (max - min) + min;
            Single b = (Single)random.NextDouble() * (max - min) + min;
            Single a = 1f;

            return new Rgba32(r, g, b, a);
        }

        public Rgba32 GetRandomColourNearby(Rgba32 zColour, float zVariation)
        {
            Vector4 data = zColour.ToVector4();


            float variation = zVariation / 2.0f;

            //float lowerW = data.W - variation;
            //float upperW = data.W + variation;
            //float w = Random_Float(lowerW, upperW);
            //data.W = Euclid.Limit(ref w, 0.0f, 1.0f);

            float lowerX = data.X - variation;
            float upperX = data.X + variation;
            float x = GetRandomSingle(lowerX, upperX);
            data.X = BlimeyMathsHelper.Limit(ref x, 0.0f, 1.0f);

            float lowerY = data.Y - variation;
            float upperY = data.Y + variation;
            float y = GetRandomSingle(lowerY, upperY);
            data.Y = BlimeyMathsHelper.Limit(ref y, 0.0f, 1.0f);

            float lowerZ = data.Z - variation;
            float upperZ = data.Z + variation;
            float z = GetRandomSingle(lowerZ, upperZ);
            data.Z = BlimeyMathsHelper.Limit(ref z, 0.0f, 1.0f);


            Rgba32 col = new Rgba32(data.X, data.Y, data.Z, data.W);
            return col;
        }

        [CLSCompliant(false)]
        public Vector2 GetRandomVector2(Single min, Single max)
        {
            Single x = (Single)random.NextDouble() * (max - min) + min;
            Single y = (Single)random.NextDouble() * (max - min) + min;

            return new Vector2(x, y);
        }

        [CLSCompliant(false)]
        public Vector3 GetRandomVector3(Single min, Single max)
        {
            Single x = (Single)random.NextDouble() * (max - min) + min;
            Single y = (Single)random.NextDouble() * (max - min) + min;
            Single z = (Single)random.NextDouble() * (max - min) + min;

            return new Vector3(x, y, z);
        }

        [CLSCompliant(false)]
        public Vector3 GetRandomNormalisedVector3()
        {
            Single max = 1f;
            Single min = 1f;

            Single x = (Single)random.NextDouble() * (max - min) + min;
            Single y = (Single)random.NextDouble() * (max - min) + min;
            Single z = (Single)random.NextDouble() * (max - min) + min;

            var result = new Vector3(x, y, z);

            Vector3.Normalise(ref result, out result);

            return result;
        }

        [CLSCompliant(false)]
        public Vector4 GetRandomVector4(Single min, Single max)
        {
            Single x = (Single)random.NextDouble() * (max - min) + min;
            Single y = (Single)random.NextDouble() * (max - min) + min;
            Single z = (Single)random.NextDouble() * (max - min) + min;
            Single w = (Single)random.NextDouble() * (max - min) + min;

            return new Vector4(x, y, z, w);
        }

        /// <summary>
        /// Pick a random element from an indexable collection
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="random">'this' parameter</param>
        /// <param name="choices">Collection to pick an element from</param>
        /// <returns></returns>
        public object Choose(System.Collections.IList choices)
        {
            return choices[random.Next(choices.Count)];
        }

        /// <summary>
        /// Pick a random value from an enum
        /// </summary>
        /// <typeparam name="T">Enum type to pick from</typeparam>
        /// <param name="random">'this' parameter</param>
        /// <param name="enumType">the enum type to pick from</param>
        /// <returns></returns>
        public object ChooseFromEnum(Type enumType)
        {
            return Choose(Enum.GetValues(enumType));
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// A simple timer for collecting profiler data.  Usage:
    ///
    ///     using(new ProfilingTimer(time => myTime = time))
    ///     {
    ///         // stuff
    ///     }
    ///
    /// </summary>
    internal struct ProfilingTimer
        : IDisposable
    {
        public delegate void ResultHandler(double timeInMilliSeconds);

        public ProfilingTimer(ResultHandler resultHandler)
        {
            _stopWatch = Stopwatch.StartNew();
            _resultHandler = resultHandler;
        }

        public void Dispose()
        {
            double elapsedTime = (double)_stopWatch.ElapsedTicks / (double)Stopwatch.Frequency;
            _resultHandler(elapsedTime * 1000.0);
        }

        private Stopwatch _stopWatch;
        private ResultHandler _resultHandler;
    }


}
