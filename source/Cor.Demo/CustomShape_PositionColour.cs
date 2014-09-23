// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! - Low Level 3D App Engine                                         │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Brought to you by:                                                     │ \\
// │          _________                    .__               __             │ \\
// │         /   _____/__ __  ____    ____ |__|____    _____/  |_           │ \\
// │         \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\          │ \\
// │         /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |            │ \\
// │        /_______  /____/|___|  /\___  /|__(____  /___|  /__|            │ \\
// │                \/           \//_____/         \/     \/                │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2014 A.J.Pook (http://ajpook.github.io)                    │ \\
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
using Abacus.SinglePrecision;
using System.Runtime.InteropServices;
using Cor.Platform;
using Fudge;

namespace Cor.Demo
{
    [StructLayout (LayoutKind.Sequential)]
    public struct VertPosCol : IVertexType
    {
        readonly static VertexDeclaration _vertexDeclaration;

        static VertPosCol ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Colour,
                    VertexElementUsage.Colour,
                    0)
            );
        }

        public Vector3 Position;
        public Rgba32 Colour;

        public VertPosCol (Vector3 position, Rgba32 color)
        {
            this.Position = position;
            this.Colour = color;
        }

        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }
    }

    public static class CustomShape_PositionColour
    {
        public static VertPosCol[] VertArray
        {
            get
            {
                return new[]
                {
                    new VertPosCol( new Vector3(0.0f, 0.0f, 0.0f), RandomColours.GetNext() ),
                    // Top
                    new VertPosCol( new Vector3(-0.2f, 0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertPosCol( new Vector3(0.2f, 0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertPosCol( new Vector3(0.0f, 0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertPosCol( new Vector3(0.0f, 1.0f, 0.0f), RandomColours.GetNext() ),
                    // Bottom
                    new VertPosCol( new Vector3(-0.2f, -0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertPosCol( new Vector3(0.2f, -0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertPosCol( new Vector3(0.0f, -0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertPosCol( new Vector3(0.0f, -1.0f, 0.0f), RandomColours.GetNext() ),
                    // Left
                    new VertPosCol( new Vector3(-0.8f, -0.2f, 0.0f), RandomColours.GetNext() ),
                    new VertPosCol( new Vector3(-0.8f, 0.2f, 0.0f), RandomColours.GetNext() ),
                    new VertPosCol( new Vector3(-0.8f, 0.0f, 0.0f), RandomColours.GetNext() ),
                    new VertPosCol( new Vector3(-1.0f, 0.0f, 0.0f), RandomColours.GetNext() ),
                    // Right
                    new VertPosCol( new Vector3(0.8f, -0.2f, 0.0f), RandomColours.GetNext() ),
                    new VertPosCol( new Vector3(0.8f, 0.2f, 0.0f), RandomColours.GetNext() ),
                    new VertPosCol( new Vector3(0.8f, 0.0f, 0.0f), RandomColours.GetNext() ),
                    new VertPosCol( new Vector3(1.0f, 0.0f, 0.0f), RandomColours.GetNext() ),
                };
            }
        }

        public static Int32[] IndexArray
        {
            get
            {
                return new [] {
                    // Top
                    0, 1, 3,
                    0, 3, 2,
                    3, 1, 4,
                    3, 4, 2,
                    // Bottom
                    0, 7, 5,
                    0, 6, 7,
                    7, 8, 5,
                    7, 6, 8,
                    // Left
                    0, 9, 11,
                    0, 11, 10,
                    11, 9, 12,
                    11, 12, 10,
                    // Right
                    0, 15, 13,
                    0, 14, 15,
                    15, 16, 13,
                    15, 14, 16
                };
            }
        }
    }
}

