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
using Sungiant.Cor;
using System.Collections.Generic;

namespace Sungiant.Cor.Demo
{
    public static class CustomCylinder_PositionNormalTexture
    {
        const int tessellation = 9; // must be greater than 2
        const float height = 0.5f;
        const float radius = 0.5f;

        static List<Int32> indexArray = new List<Int32>();
        static List<VertexPositionNormalTexture> vertArray = 
            new List<VertexPositionNormalTexture>();
        
        public static VertexDeclaration VertexDeclaration { get {
                return VertexPositionNormalTexture.Default.VertexDeclaration; } }

        static CustomCylinder_PositionNormalTexture()
        {
            // Create a ring of triangles around the outside of the cylinder.
            for (int i = 0; i <= tessellation; i++)
            {
                Vector3 normal = GetCircleVector(i, tessellation);
                
                Vector3 topPos = normal * radius + Vector3.Up * height;
                Vector3 botPos = normal * radius + Vector3.Down * height;
                
                float howFarRound = (float)i / (float)(tessellation);
                
                
                Vector2 topUV = new Vector2(howFarRound, 0f);
                Vector2 botUV = new Vector2(howFarRound, 1f);
                
                AddVertex(topPos, normal, topUV);
                AddVertex(botPos, normal, botUV);
            }
            
            for (int i = 0; i < tessellation; i++)
            {
                AddIndex(i * 2);
                AddIndex(i * 2 + 1);
                AddIndex((i * 2 + 2));
                
                AddIndex(i * 2 + 1);
                AddIndex(i * 2 + 3);
                AddIndex(i * 2 + 2);
            }
            
            
            // Create flat triangle fan caps to seal the top and bottom.
            CreateCap(tessellation, height, radius, Vector3.Up);
            CreateCap(tessellation, height, radius, Vector3.Down);
        }

        /// Helper method creates a triangle fan to close the ends of the cylinder.
        static void CreateCap(int tessellation, float height, float radius, Vector3 normal)
        {
            // Create cap indices.
            for (int i = 0; i < tessellation - 2; i++)
            {
                if (normal.Y > 0)
                {
                    AddIndex(CurrentVertex);
                    AddIndex(CurrentVertex + (i + 1) % tessellation);
                    AddIndex(CurrentVertex + (i + 2) % tessellation);
                }
                else
                {
                    AddIndex(CurrentVertex);
                    AddIndex(CurrentVertex + (i + 2) % tessellation);
                    AddIndex(CurrentVertex + (i + 1) % tessellation);
                }
            }
            
            // Create cap vertices.
            for (int i = 0; i < tessellation; i++)
            {
                Vector3 circleVec = GetCircleVector(i, tessellation);
                Vector3 position = circleVec * radius +
                    normal * height;
                
                AddVertex(position, normal, new Vector2((circleVec.X + 1f) / 2f, (circleVec.Z + 1f) / 2f));
            }
        }
        

        /// Helper method computes a point on a circle.
        static Vector3 GetCircleVector(int i, int tessellation)
        {
            Single tau; RealMaths.Tau(out tau);
            float angle = i * tau / tessellation;
            
            float dx = (float)Math.Cos(angle);
            float dz = (float)Math.Sin(angle);
            
            return new Vector3(dx, 0, dz);
        }

        static int CurrentVertex
        {
            get { return vertArray.Count; }
        }
        
        static void AddVertex(Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            vertArray.Add(new VertexPositionNormalTexture(position, normal, texCoord));
        }
        
        static void AddIndex(int index)
        {
            if (index > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("index");
            
            indexArray.Add((ushort)index);
        }
        
        public static VertexPositionNormalTexture[] VertArray
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

