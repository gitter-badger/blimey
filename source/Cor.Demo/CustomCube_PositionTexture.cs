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
// │ Copyright © 2013 A.J.Pook (http://sungiant.github.com)                 │ \\
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
    public static class CustomCube_PositionTexture
    {
        static Vector3[] normals;
        static List<Int32> indexArray = new List<Int32>();
        static List<VertexPositionTexture> vertArray =
            new List<VertexPositionTexture>();

        public static VertexDeclaration VertexDeclaration { get {
                return VertexPositionTexture.Default.VertexDeclaration; } }

        static CustomCube_PositionTexture()
        {
            // A cube has six faces, each one pointing in a different direction.
            normals = new Vector3[]
            {
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1),
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, -1, 0),
            };

            // Create each face in turn.
            foreach (Vector3 normal in normals)
            {
                // Get two vectors perpendicular to the face normal and to each other.
                Vector3 side1 = new Vector3(normal.Y, normal.Z, normal.X);
                Vector3 side2;

                Vector3 n = normal;
                Vector3.Cross(ref n, ref side1, out side2);

                // Six indices (two triangles) per face.
                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 1);
                AddIndex(CurrentVertex + 2);

                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 2);
                AddIndex(CurrentVertex + 3);

                // Four vertices per face.
                AddVertex((normal - side1 - side2) / 2, normal, new Vector2(0f, 0f));
                AddVertex((normal - side1 + side2) / 2, normal, new Vector2(1f, 0f));
                AddVertex((normal + side1 + side2) / 2, normal, new Vector2(1f, 1f));
                AddVertex((normal + side1 - side2) / 2, normal, new Vector2(0f, 1f));
            }
        }

        static int CurrentVertex
        {
            get { return vertArray.Count; }
        }

        static void AddVertex(Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            vertArray.Add(new VertexPositionTexture(position, /*normal,*/ texCoord));
        }

        static void AddIndex(int index)
        {
            if (index > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("index");

            indexArray.Add((ushort)index);
        }

        public static VertexPositionTexture[] VertArray
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

