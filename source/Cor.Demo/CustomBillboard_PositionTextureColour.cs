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
using Abacus;
using Abacus.SinglePrecision;
using Abacus.Packed;
using Cor;
using System.Collections.Generic;

namespace Cor.Demo
{
	public static class CustomBillboard_PositionTextureColour
	{
		static List<Int32> indexArray = new List<Int32>();

		static List<VertexPositionTextureColour> vertArray =
			new List<VertexPositionTextureColour>();

		public static VertexDeclaration VertexDeclaration { get {
				return VertexPositionTextureColour.Default.VertexDeclaration; } }

		static CustomBillboard_PositionTextureColour()
		{
			// Six indices (two triangles) per face.
			AddIndex(0);
			AddIndex(3);
			AddIndex(2);

			AddIndex(0);
			AddIndex(1);
			AddIndex(3);

			// Four vertices per face.
			AddVertex(new Vector3(-0.5f, -0.5f, 0f), new Vector2(0.5f, 1f), Rgba32.Yellow);
			AddVertex(new Vector3(-0.5f,  0.5f, 0f), new Vector2(0.5f, 0f), Rgba32.Green);
			AddVertex(new Vector3( 0.5f, -0.5f, 0f), new Vector2(0f, 1f), Rgba32.Blue);
			AddVertex(new Vector3( 0.5f,  0.5f, 0f), new Vector2(0f, 0f), Rgba32.Red);

		}

		static int CurrentVertex
		{
			get { return vertArray.Count; }
		}

		static void AddVertex(Vector3 position, Vector2 texCoord, Rgba32 colour)
		{
			vertArray.Add(new VertexPositionTextureColour(position, texCoord, colour));
		}

		static void AddIndex(int index)
		{
			if (index > ushort.MaxValue)
				throw new ArgumentOutOfRangeException("index");

			indexArray.Add((ushort)index);
		}

		public static VertexPositionTextureColour[] VertArray
		{
			get
			{
				return vertArray.ToArray();
			}
		}

		public static Int32[] IndexArray
		{
			get
			{
				return indexArray.ToArray();
			}
		}

	}
}

