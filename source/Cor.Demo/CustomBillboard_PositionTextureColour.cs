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

namespace Cor.Demo
{
    using System;
    using Abacus.SinglePrecision;
    using Fudge;
    using Cor.Platform;
    using System.Runtime.InteropServices;

    [StructLayout (LayoutKind.Sequential)]
    public struct VertPosTexCol : IVertexType
    {
        readonly static VertexDeclaration _vertexDeclaration;

        static VertPosTexCol ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate,
                    0),
                new VertexElement (
                    20,
                    VertexElementFormat.Colour,
                    VertexElementUsage.Colour,
                    0)
            );
        }

        public Vector3 Position;
        public Vector2 UV;
        public Rgba32 Colour;

        public VertPosTexCol (Vector3 position, Vector2 uv, Rgba32 color)
        {
            this.Position = position;
            this.UV = uv;
            this.Colour = color;
        }

        public VertexDeclaration VertexDeclaration { get { return _vertexDeclaration; } }
    }

	public class Billboard
        : IMesh <VertPosTexCol>
    {
        readonly VertPosTexCol[] vertArray = new VertPosTexCol[4];
        readonly Int32[] indexArray = new Int32[6];
        readonly VertexDeclaration vertexDeclaration;

        #region IMesh <VertPosTexCol>

        public VertPosTexCol[] VertArray { get { return vertArray; } }
        public Int32[] IndexArray { get { return indexArray; } }
        public VertexDeclaration VertexDeclaration { get { return vertexDeclaration; } }

        #endregion

        public Billboard()
		{
            vertexDeclaration = new VertPosTexCol (Vector3.Zero, Vector2.Zero, Rgba32.White).VertexDeclaration;

			// Six indices (two triangles) per face.
            indexArray[0] = 0;
            indexArray[1] = 3;
            indexArray[2] = 2;

            indexArray[3] = 0;
            indexArray[4] = 1;
            indexArray[5] = 3;

			// Four vertices per face.
            vertArray[0] = new VertPosTexCol(new Vector3(-0.5f, -0.5f, 0f), new Vector2(0.5f, 1f), Rgba32.Yellow);
            vertArray[1] = new VertPosTexCol(new Vector3(-0.5f,  0.5f, 0f), new Vector2(0.5f, 0f), Rgba32.Green);
            vertArray[2] = new VertPosTexCol(new Vector3( 0.5f, -0.5f, 0f), new Vector2(0f, 1f), Rgba32.Blue);
            vertArray[3] = new VertPosTexCol(new Vector3( 0.5f,  0.5f, 0f), new Vector2(0f, 0f), Rgba32.Red);
		}
	}
}

