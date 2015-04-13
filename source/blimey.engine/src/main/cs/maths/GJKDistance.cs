// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey Engine (http://www.blimey.io)         │ \\
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

namespace Blimey.Engine
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Fudge;
    using global::Blimey.Platform;
    using global::Blimey.Asset;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// This comes straight from XNA.
    /// </summary>
    internal class GJKDistance
    {
        /// <summary>
        /// todo
        /// </summary>
        internal GJKDistance ()
        {
            for (Int32 i = 0; i < 0x10; i++)
            {
                this.det [i] = new Single[4];
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        internal Boolean AddSupportPoint (ref Vector3 newPoint)
        {
            Int32 index = (BitsToIndices [this.simplexBits ^ 15] & 7) - 1;

            this.y [index] = newPoint;
            Single newPointLs;
            Vector3.LengthSquared(ref newPoint, out newPointLs);
            this.yLengthSq [index] = newPointLs;

            for (Int32 i = BitsToIndices[this.simplexBits]; i != 0; i = i >> 3)
            {
                Int32 idx = (i & 7) - 1;

                Vector3 vector;
                vector.X = this.y [idx].X - newPoint.X;
                vector.Y = this.y [idx].Y - newPoint.Y;
                vector.Z = this.y [idx].Z - newPoint.Z;

                this.edges [idx] [index] = vector;
                this.edges [index] [idx].X = -vector.X;
                this.edges [index] [idx].Y = -vector.Y;
                this.edges [index] [idx].Z = -vector.Z;

                Single vectorLs;
                Vector3.LengthSquared (ref vector, out vectorLs);

                this.edgeLengthSq [index] [idx] = vectorLs;
                this.edgeLengthSq [idx] [index] = vectorLs;
            }

            this.UpdateDeterminant (index);

            return this.UpdateSimplex (index);
        }

        /// <summary>
        /// todo
        /// </summary>
        internal void Reset ()
        {
            Single zero = 0;

            this.simplexBits = 0;
            this.maxLengthSq = zero;
        }

        /// <summary>
        /// todo
        /// </summary>
        internal Vector3 ClosestPoint
        {
            get { return this.closestPoint; }
        }

        /// <summary>
        /// todo
        /// </summary>
        internal Boolean FullSimplex
        {
            get { return (this.simplexBits == 15); }
        }

        /// <summary>
        /// todo
        /// </summary>
        internal Single MaxLengthSquared
        {
            get { return this.maxLengthSq; }
        }

        /// <summary>
        /// todo
        /// </summary>
        Vector3 closestPoint;

        /// <summary>
        /// todo
        /// </summary>
        Single[][] det = new Single[0x10][];

        /// <summary>
        /// todo
        /// </summary>
        Single[][] edgeLengthSq =
            new Single[][]
            {
                new Single[4],
                new Single[4],
                new Single[4],
                new Single[4]
            };

        /// <summary>
        /// todo
        /// </summary>
        Vector3[][] edges =
            new Vector3[][]
            {
                new Vector3[4],
                new Vector3[4],
                new Vector3[4],
                new Vector3[4]
            };

        /// <summary>
        /// todo
        /// </summary>
        Single maxLengthSq;

        /// <summary>
        /// todo
        /// </summary>
        Int32 simplexBits;

        /// <summary>
        /// todo
        /// </summary>
        Vector3[] y = new Vector3[4];

        /// <summary>
        /// todo
        /// </summary>
        Single[] yLengthSq = new Single[4];

        /// <summary>
        /// todo
        /// </summary>
        static Int32[] BitsToIndices =
            new Int32[]
            {
                0, 1, 2, 0x11, 3, 0x19, 0x1a, 0xd1,
                4, 0x21, 0x22, 0x111, 0x23, 0x119, 0x11a, 0x8d1
            };

        /// <summary>
        /// todo
        /// </summary>
        Vector3 ComputeClosestPoint ()
        {
            Single fzero; Maths.Zero(out fzero);

            Single num3 = fzero;
            Vector3 zero = Vector3.Zero;

            this.maxLengthSq = fzero;

            for (Int32 i = BitsToIndices[this.simplexBits]; i != 0; i = i >> 3)
            {
                Int32 index = (i & 7) - 1;
                Single num4 = this.det [this.simplexBits] [index];

                num3 += num4;
                zero += (Vector3)(this.y [index] * num4);

                this.maxLengthSq =
                Maths.Max (this.maxLengthSq, this.yLengthSq [index]);
            }

            return (Vector3)(zero / num3);
        }

        /// <summary>
        /// todo
        /// </summary>
        Boolean IsSatisfiesRule (Int32 xBits, Int32 yBits)
        {
            Single fzero; Maths.Zero(out fzero);

            for (Int32 i = BitsToIndices[yBits]; i != 0; i = i >> 3)
            {
                Int32 index = (i & 7) - 1;
                Int32 num3 = ((Int32)1) << index;

                if ((num3 & xBits) != 0)
                {
                    if (this.det [xBits] [index] <= fzero)
                    {
                        return false;
                    }
                }
                else if (this.det [xBits | num3] [index] > fzero)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// todo
        /// </summary>
        void UpdateDeterminant (Int32 xmIdx)
        {
            Single fone; Maths.One(out fone);
            Int32 index = ((Int32)1) << xmIdx;

            this.det [index] [xmIdx] = fone;

            Int32 num14 = BitsToIndices [this.simplexBits];
            Int32 num8 = num14;

            for (Int32 i = 0; num8 != 0; i++)
            {
                Int32 num = (num8 & 7) - 1;
                Int32 num12 = ((int)1) << num;
                Int32 num6 = num12 | index;

                this.det [num6] [num] =
                    Dot (ref this.edges [xmIdx] [num], ref this.y [xmIdx]);

                this.det [num6] [xmIdx] =
                    Dot (ref this.edges [num] [xmIdx], ref this.y [num]);

                Int32 num11 = num14;

                for (Int32 j = 0; j < i; j++)
                {
                    int num3 = (num11 & 7) - 1;
                    int num5 = ((int)1) << num3;
                    int num9 = num6 | num5;
                    int num4 = (this.edgeLengthSq [num] [num3] < this.edgeLengthSq [xmIdx] [num3]) ? num : xmIdx;

                    this.det [num9] [num3] =
                        (this.det [num6] [num] * Dot (ref this.edges [num4] [num3], ref this.y [num])) +
                        (this.det [num6] [xmIdx] * Dot (ref this.edges [num4] [num3], ref this.y [xmIdx]));

                    num4 = (this.edgeLengthSq [num3] [num] < this.edgeLengthSq [xmIdx] [num]) ? num3 : xmIdx;

                    this.det [num9] [num] =
                        (this.det [num5 | index] [num3] * Dot (ref this.edges [num4] [num], ref this.y [num3])) +
                        (this.det [num5 | index] [xmIdx] * Dot (ref this.edges [num4] [num], ref this.y [xmIdx]));

                    num4 = (this.edgeLengthSq [num] [xmIdx] < this.edgeLengthSq [num3] [xmIdx]) ? num : num3;

                    this.det [num9] [xmIdx] =
                        (this.det [num12 | num5] [num3] * Dot (ref this.edges [num4] [xmIdx], ref this.y [num3])) +
                        (this.det [num12 | num5] [num] * Dot (ref this.edges [num4] [xmIdx], ref this.y [num]));

                    num11 = num11 >> 3;
                }

                num8 = num8 >> 3;
            }

            if ((this.simplexBits | index) == 15)
            {
                int num2 =
                    (this.edgeLengthSq [1] [0] < this.edgeLengthSq [2] [0]) ?
                    ((this.edgeLengthSq [1] [0] < this.edgeLengthSq [3] [0]) ? 1 : 3) :
                    ((this.edgeLengthSq [2] [0] < this.edgeLengthSq [3] [0]) ? 2 : 3);

                this.det [15] [0] =
                    ((this.det [14] [1] * Dot (ref this.edges [num2] [0], ref this.y [1])) +
                    (this.det [14] [2] * Dot (ref this.edges [num2] [0], ref this.y [2]))) +
                    (this.det [14] [3] * Dot (ref this.edges [num2] [0], ref this.y [3]));

                num2 =
                    (this.edgeLengthSq [0] [1] < this.edgeLengthSq [2] [1]) ?
                    ((this.edgeLengthSq [0] [1] < this.edgeLengthSq [3] [1]) ? 0 : 3) :
                    ((this.edgeLengthSq [2] [1] < this.edgeLengthSq [3] [1]) ? 2 : 3);

                this.det [15] [1] =
                    ((this.det [13] [0] * Dot (ref this.edges [num2] [1], ref this.y [0])) +
                    (this.det [13] [2] * Dot (ref this.edges [num2] [1], ref this.y [2]))) +
                    (this.det [13] [3] * Dot (ref this.edges [num2] [1], ref this.y [3]));

                num2 =
                    (this.edgeLengthSq [0] [2] < this.edgeLengthSq [1] [2]) ?
                    ((this.edgeLengthSq [0] [2] < this.edgeLengthSq [3] [2]) ? 0 : 3) :
                    ((this.edgeLengthSq [1] [2] < this.edgeLengthSq [3] [2]) ? 1 : 3);

                this.det [15] [2] =
                    ((this.det [11] [0] * Dot (ref this.edges [num2] [2], ref this.y [0])) +
                    (this.det [11] [1] * Dot (ref this.edges [num2] [2], ref this.y [1]))) +
                    (this.det [11] [3] * Dot (ref this.edges [num2] [2], ref this.y [3]));

                num2 =
                    (this.edgeLengthSq [0] [3] < this.edgeLengthSq [1] [3]) ?
                    ((this.edgeLengthSq [0] [3] < this.edgeLengthSq [2] [3]) ? 0 : 2) :
                    ((this.edgeLengthSq [1] [3] < this.edgeLengthSq [2] [3]) ? 1 : 2);

                this.det [15] [3] =
                    ((this.det [7] [0] * Dot (ref this.edges [num2] [3], ref this.y [0])) +
                    (this.det [7] [1] * Dot (ref this.edges [num2] [3], ref this.y [1]))) +
                    (this.det [7] [2] * Dot (ref this.edges [num2] [3], ref this.y [2]));
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        Boolean UpdateSimplex (Int32 newIndex)
        {
            Int32 yBits = this.simplexBits | (((Int32)1) << newIndex);

            Int32 xBits = ((Int32)1) << newIndex;

            for (Int32 i = this.simplexBits; i != 0; i--)
            {
                if (((i & yBits) == i) && this.IsSatisfiesRule (i | xBits, yBits))
                {
                    this.simplexBits = i | xBits;
                    this.closestPoint = this.ComputeClosestPoint ();

                    return true;
                }
            }

            Boolean flag = false;

            if (this.IsSatisfiesRule (xBits, yBits))
            {
                this.simplexBits = xBits;
                this.closestPoint = this.y [newIndex];
                this.maxLengthSq = this.yLengthSq [newIndex];

                flag = true;
            }

            return flag;
        }

        /// <summary>
        /// todo
        /// </summary>
        static Single Dot (ref Vector3 a, ref Vector3 b)
        {
            return (((a.X * b.X) + (a.Y * b.Y)) + (a.Z * b.Z));
        }
    }
}
