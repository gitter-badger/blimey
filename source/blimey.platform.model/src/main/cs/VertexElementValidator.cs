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

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    using Fudge;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    public static class VertexElementValidator
    {
        /// <summary>
        ///
        /// </summary>
        public static Int32 GetTypeSize (VertexElementFormat format)
        {
            switch (format)
            {
                case VertexElementFormat.Single: return 4;
                case VertexElementFormat.Vector2: return 8;
                case VertexElementFormat.Vector3: return 12;
                case VertexElementFormat.Vector4: return 0x10;
                case VertexElementFormat.Colour: return 4;
                case VertexElementFormat.Byte4: return 4;
                case VertexElementFormat.Short2: return 4;
                case VertexElementFormat.Short4: return 8;
                case VertexElementFormat.NormalisedShort2: return 4;
                case VertexElementFormat.NormalisedShort4: return 8;
                case VertexElementFormat.HalfVector2: return 4;
                case VertexElementFormat.HalfVector4: return 8;
            }

            throw new Exception ("Unsupported");
        }

        /// <summary>
        ///
        /// </summary>
        public static int GetVertexStride (VertexElement[] elements)
        {
            Int32 num2 = 0;

            for (Int32 i = 0; i < elements.Length; i++)
            {
                Int32 num3 = elements [i].Offset + GetTypeSize (elements [i].VertexElementFormat);

                if (num2 < num3)
                {
                    num2 = num3;
                }
            }

            return num2;
        }

        /// <summary>
        /// checks that an effect supports the given vert decl
        /// </summary>
        //public static void Validate (IShader effect, VertexDeclaration vertexDeclaration)
        //{
        //    throw new NotImplementedException ();
       // }

        /// <summary>
        ///
        /// </summary>
        public static void Validate (int vertexStride, VertexElement[] elements)
        {
            if (vertexStride <= 0)
            {
                throw new ArgumentOutOfRangeException ("vertexStride");
            }

            if ((vertexStride & 3) != 0)
            {
                throw new ArgumentException ("VertexElementOffsetNotMultipleFour");
            }

            var numArray = new Int32[vertexStride];

            for (Int32 i = 0; i < vertexStride; i++)
            {
                numArray [i] = -1;
            }

            for (Int32 j = 0; j < elements.Length; j++)
            {
                Int32 offset = elements [j].Offset;

                Int32 typeSize = GetTypeSize (elements [j].VertexElementFormat);

                if ((elements [j].VertexElementUsage < VertexElementUsage.Position) ||
                    (elements [j].VertexElementUsage > VertexElementUsage.TessellateFactor))
                {
                    throw new ArgumentException ("FrameworkResources.VertexElementBadUsage");
                }

                if ((offset < 0) || ((offset + typeSize) > vertexStride))
                {
                    throw new ArgumentException ("FrameworkResources.VertexElementOutsideStride");
                }

                if ((offset & 3) != 0)
                {
                    throw new ArgumentException ("VertexElementOffsetNotMultipleFour");
                }

                for (Int32 k = 0; k < j; k++)
                {
                    if ((elements [j].VertexElementUsage == elements [k].VertexElementUsage) &&
                        (elements [j].UsageIndex == elements [k].UsageIndex))
                    {
                        throw new ArgumentException ("DuplicateVertexElement");
                    }
                }

                for (Int32 m = offset; m < (offset + typeSize); m++)
                {
                    if (numArray [m] >= 0)
                    {
                        throw new ArgumentException ("VertexElementsOverlap");
                    }

                    numArray [m] = j;
                }
            }
        }
    }
}