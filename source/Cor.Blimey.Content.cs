// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Blimey - Fast, efficient, high level engine built upon Cor & Abacus                                            │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │                     Brought to you by:                                                                         │ \\
// │                              _________                    .__               __                                 │ \\
// │                             /   _____/__ __  ____    ____ |__|____    _____/  |_                               │ \\
// │                             \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\                              │ \\
// │                             /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |                                │ \\
// │                            /_______  /____/|___|  /\___  /|__(____  /___|  /__|                                │ \\
// │                                    \/           \//_____/         \/     \/                                    │ \\
// │                                                                                                                │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2008-2014 A.J.Pook (http://ajpook.github.io)                                                       │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors: A.J.Pook                                                                                              │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated   │ \\
// │ documentation files (the "Software"), to deal in the Software without restriction, including without           │ \\
// │ limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sellcopies of the   │ \\
// │ Software, and to permit persons to whom the Software is furnished to do so, subject to the following           │ \\
// │ conditions:                                                                                                    │ \\
// │                                                                                                                │ \\
// │ The above copyright notice and this permission notice shall be included in all copies or substantial portions  │ \\
// │ of the Software.                                                                                               │ \\
// │                                                                                                                │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED  │ \\
// │ TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL │ \\
// │ THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF  │ \\
// │ CONTRACT, TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER        │ \\
// │ DEALINGS IN THE SOFTWARE.                                                                                      │ \\
// └────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘ \\

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Abacus;
    using Fudge;
    using Abacus.SinglePrecision;
    using System.Linq;
    using Cor;

    // Cor Blimey Content
    // Provides a suite of programatically defined resource useful for debugging and testing.  These resources
    // bypass Cor's Asset system.
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public abstract class BezierPrimitive
        : GeometricPrimitive
    {
        /// <summary>
        /// Creates indices for a patch that is tessellated at the specified level.
        /// </summary>
        protected void CreatePatchIndices(int tessellation, Boolean isMirrored)
        {
            int stride = tessellation + 1;

            for (int i = 0; i < tessellation; i++)
            {
                for (int j = 0; j < tessellation; j++)
                {
                    // Make a list of six index values (two triangles).
                    int[] indices =
                    {
                        i * stride + j,
                        (i + 1) * stride + j,
                        (i + 1) * stride + j + 1,

                        i * stride + j,
                        (i + 1) * stride + j + 1,
                        i * stride + j + 1,
                    };

                    // If this patch is mirrored, reverse the
                    // indices to keep the correct winding order.
                    if (isMirrored)
                    {
                        Array.Reverse(indices);
                    }

                    // Create the indices.
                    foreach (int index in indices)
                    {
                        AddIndex(CurrentVertex + index);
                    }
                }
            }
        }

        /// <summary>
        /// Creates vertices for a patch that is tessellated at the specified level.
        /// </summary>
        protected void CreatePatchVertices(Vector3[] patch, int tessellation, Boolean isMirrored)
        {
            Debug.Assert(patch.Length == 16);

            for (int i = 0; i <= tessellation; i++)
            {
                float ti = (float)i / tessellation;

                for (int j = 0; j <= tessellation; j++)
                {
                    float tj = (float)j / tessellation;

                    // Perform four horizontal bezier interpolations
                    // between the control points of this patch.
                    Vector3 p1 = Bezier(patch[0], patch[1], patch[2], patch[3], ti);
                    Vector3 p2 = Bezier(patch[4], patch[5], patch[6], patch[7], ti);
                    Vector3 p3 = Bezier(patch[8], patch[9], patch[10], patch[11], ti);
                    Vector3 p4 = Bezier(patch[12], patch[13], patch[14], patch[15], ti);

                    // Perform a vertical interpolation between the results of the
                    // previous horizontal interpolations, to compute the position.
                    Vector3 position = Bezier(p1, p2, p3, p4, tj);

                    // Perform another four bezier interpolations between the control
                    // points, but this time vertically rather than horizontally.
                    Vector3 q1 = Bezier(patch[0], patch[4], patch[8], patch[12], tj);
                    Vector3 q2 = Bezier(patch[1], patch[5], patch[9], patch[13], tj);
                    Vector3 q3 = Bezier(patch[2], patch[6], patch[10], patch[14], tj);
                    Vector3 q4 = Bezier(patch[3], patch[7], patch[11], patch[15], tj);

                    // Compute vertical and horizontal tangent vectors.
                    Vector3 tangentA = BezierTangent(p1, p2, p3, p4, tj);
                    Vector3 tangentB = BezierTangent(q1, q2, q3, q4, ti);

                    // Cross the two tangent vectors to compute the normal.
                    Vector3 normal;
                    Vector3.Cross(ref tangentA, ref tangentB, out normal);

                    if (normal.Length() > 0.0001f)
                    {
                        Vector3.Normalise(ref normal, out normal);

                        // If this patch is mirrored, we must invert the normal.
                        if (isMirrored)
                            normal = -normal;
                    }
                    else
                    {
                        // In a tidy and well constructed bezier patch, the preceding
                        // normal computation will always work. But the classic teapot
                        // model is not tidy or well constructed! At the top and bottom
                        // of the teapot, it contains degenerate geometry where a patch
                        // has several control points in the same place, which causes
                        // the tangent computation to fail and produce a zero normal.
                        // We 'fix' these cases by just hard-coding a normal that points
                        // either straight up or straight down, depending on whether we
                        // are on the top or bottom of the teapot. This is not a robust
                        // solution for all possible degenerate bezier patches, but hey,
                        // it's good enough to make the teapot work correctly!

                        if (position.Y > 0)
                            normal = Vector3.Up;
                        else
                            normal = Vector3.Down;
                    }

                    // Create the vertex.
                    AddVertex(position, normal);
                }
            }
        }


        /// <summary>
        /// Performs a cubic bezier interpolation between four scalar control
        /// points, returning the value at the specified time (t ranges 0 to 1).
        /// </summary>
        static float Bezier(float p1, float p2, float p3, float p4, float t)
        {
            return p1 * (1 - t) * (1 - t) * (1 - t) +
                   p2 * 3 * t * (1 - t) * (1 - t) +
                   p3 * 3 * t * t * (1 - t) +
                   p4 * t * t * t;
        }


        /// <summary>
        /// Performs a cubic bezier interpolation between four Vector3 control
        /// points, returning the value at the specified time (t ranges 0 to 1).
        /// </summary>
        static Vector3 Bezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
        {
            Vector3 result = new Vector3();

            result.X = Bezier(p1.X, p2.X, p3.X, p4.X, t);
            result.Y = Bezier(p1.Y, p2.Y, p3.Y, p4.Y, t);
            result.Z = Bezier(p1.Z, p2.Z, p3.Z, p4.Z, t);

            return result;
        }


        /// <summary>
        /// Computes the tangent of a cubic bezier curve at the specified time,
        /// when given four scalar control points.
        /// </summary>
        static float BezierTangent(float p1, float p2, float p3, float p4, float t)
        {
            return p1 * (-1 + 2 * t - t * t) +
                   p2 * (1 - 4 * t + 3 * t * t) +
                   p3 * (2 * t - 3 * t * t) +
                   p4 * (t * t);
        }


        /// <summary>
        /// Computes the tangent of a cubic bezier curve at the specified time,
        /// when given four Vector3 control points. This is used for calculating
        /// normals (by crossing the horizontal and vertical tangent vectors).
        /// </summary>
        static Vector3 BezierTangent(Vector3 p1, Vector3 p2,
                                     Vector3 p3, Vector3 p4, float t)
        {
            Vector3 result = new Vector3();

            result.X = BezierTangent(p1.X, p2.X, p3.X, p4.X, t);
            result.Y = BezierTangent(p1.Y, p2.Y, p3.Y, p4.Y, t);
            result.Z = BezierTangent(p1.Z, p2.Z, p3.Z, p4.Z, t);

            try
            {
                Vector3.Normalise(ref result, out result);
            }
            catch
            {
                result = Vector3.Zero;
            }

            return result;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public abstract class GeometricPrimitive
        : Mesh
    {
        public static IVertexType vertType = new VertexPositionNormal();

        public override VertexDeclaration VertDecl { get { return vertType.VertexDeclaration; } }

        // Once all the geometry has been specified by calling AddVertex and AddIndex,
        // this method copies the vertex and index data into GPU format buffers, ready
        // for efficient rendering.
        protected void InitializePrimitive(IGraphicsManager gfx)
        {

            GeomBuffer = gfx.CreateGeometryBuffer(VertDecl, vertices.Count, indices.Count);

            GeomBuffer.VertexBuffer.SetData(vertices.ToArray());

            GeomBuffer.IndexBuffer.SetData(indices.ToArray());

            //todo, move to base mesh abstract class
            TriangleCount = indices.Count / 3;
            VertexCount = vertices.Count;

        }

        /// <summary>
        /// Queries the index of the current vertex. This starts at
        /// zero, and increments every time AddVertex is called.
        /// </summary>
        protected Int32 CurrentVertex
        {
            get { return vertices.Count; }
        }

        /// <summary>
        /// Adds a new vertex to the primitive model. This should only be called
        /// during the initialization process, before InitializePrimitive.
        /// </summary>
        protected void AddVertex(Vector3 position, Vector3 normal)
        {
            var vertElement = new VertexPositionNormal(position, normal);
            vertices.Add(vertElement);
        }


        /// <summary>
        /// Adds a new index to the primitive model. This should only be called
        /// during the initialization process, before InitializePrimitive.
        /// </summary>
        protected void AddIndex(Int32 index)
        {
            if (index > UInt16.MaxValue)
                throw new ArgumentOutOfRangeException("index");

            indices.Add((UInt16)index);
        }

        // During the process of constructing a primitive model, vertex
        // and index data is stored on the CPU in these managed lists.
        List<VertexPositionNormal> vertices = new List<VertexPositionNormal>();
        List<Int32> indices = new List<Int32>();
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class BillboardPrimitive
        : GeometricPrimitive
    {
        public BillboardPrimitive (IGraphicsManager graphicsDevice)
        {
            // Six indices (two triangles) per face.
            AddIndex (0);
            AddIndex (1);
            AddIndex (2);

            AddIndex (0);
            AddIndex (2);
            AddIndex (3);


            // Four vertices per face.
            AddVertex ((-Vector3.Right - Vector3.Forward) / 2, Vector3.Up);
            AddVertex ((-Vector3.Right + Vector3.Forward) / 2, Vector3.Up);
            AddVertex ((Vector3.Right + Vector3.Forward) / 2, Vector3.Up);
            AddVertex ((Vector3.Right - Vector3.Forward) / 2, Vector3.Up);

            InitializePrimitive (graphicsDevice);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class CubePrimitive
        : GeometricPrimitive
    {
        /// <summary>
        /// Constructs a new cube primitive, with the specified size.
        /// </summary>
        public CubePrimitive (IGraphicsManager graphicsDevice)
        {
            // A cube has six faces, each one pointing in a different direction.
            Vector3[] normals =
            {
                new Vector3 (0, 0, 1),
                new Vector3 (0, 0, -1),
                new Vector3 (1, 0, 0),
                new Vector3 (-1, 0, 0),
                new Vector3 (0, 1, 0),
                new Vector3 (0, -1, 0),
            };

            // Create each face in turn.
            for (int i = 0; i < normals.Length; ++i )
            {
                Vector3 n = normals[i];

                // Get two vectors perpendicular to the face normal and to each other.
                Vector3 side1 = new Vector3(n.Y, n.Z, n.X);
                Vector3 side2;

                Vector3.Cross(ref n, ref side1, out side2);

                // Six indices (two triangles) per face.
                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 1);
                AddIndex(CurrentVertex + 2);

                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 2);
                AddIndex(CurrentVertex + 3);

                // Four vertices per face.
                AddVertex((n - side1 - side2) / 2, n);
                AddVertex((n - side1 + side2) / 2, n);
                AddVertex((n + side1 + side2) / 2, n);
                AddVertex((n + side1 - side2) / 2, n);
            }

            InitializePrimitive (graphicsDevice);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class CylinderPrimitive
        : GeometricPrimitive
    {
        const int _tessellation = 32;
        const float _height = 0.5f;
        const float _radius = 0.5f;

        public CylinderPrimitive (IGraphicsManager graphicsDevice)
        {
            Debug.Assert (_tessellation >= 3);

            // Create a ring of triangles around the outside of the cylinder.
            for (int i = 0; i <= _tessellation; i++) {
                Vector3 normal = GetCircleVector (i, _tessellation);

                Vector3 topPos = normal * _radius + Vector3.Up * _height;
                Vector3 botPos = normal * _radius + Vector3.Down * _height;

                AddVertex (topPos, normal);
                AddVertex (botPos, normal);
            }

            for (int i = 0; i < _tessellation; i++) {
                AddIndex (i * 2);
                AddIndex (i * 2 + 1);
                AddIndex ((i * 2 + 2));

                AddIndex (i * 2 + 1);
                AddIndex (i * 2 + 3);
                AddIndex (i * 2 + 2);
            }


            // Create flat triangle fan caps to seal the top and bottom.
            CreateCap (_tessellation, _height, _radius, Vector3.Up);
            CreateCap (_tessellation, _height, _radius, Vector3.Down);

            InitializePrimitive (graphicsDevice);
        }

        /// <summary>
        /// Helper method creates a triangle fan to close the ends of the cylinder.
        /// </summary>
        void CreateCap (int tessellation, float height, float radius, Vector3 normal)
        {
            // Create cap indices.
            for (int i = 0; i < tessellation - 2; i++) {
                if (normal.Y > 0) {
                    AddIndex (CurrentVertex);
                    AddIndex (CurrentVertex + (i + 1) % tessellation);
                    AddIndex (CurrentVertex + (i + 2) % tessellation);
                } else {
                    AddIndex (CurrentVertex);
                    AddIndex (CurrentVertex + (i + 2) % tessellation);
                    AddIndex (CurrentVertex + (i + 1) % tessellation);
                }
            }

            // Create cap vertices.
            for (int i = 0; i < tessellation; i++) {
                Vector3 circleVec = GetCircleVector (i, tessellation);
                Vector3 position = circleVec * radius +
                                   normal * height;

                AddVertex (position, normal);
            }
        }


        /// <summary>
        /// Helper method computes a point on a circle.
        /// </summary>
        static Vector3 GetCircleVector (int i, int tessellation)
        {
            float tau; Maths.Tau(out tau);
            float angle = i * tau / tessellation;

            float dx = (float)Math.Cos (angle);
            float dz = (float)Math.Sin (angle);

            return new Vector3 (dx, 0, dz);
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class SpherePrimitive
        : GeometricPrimitive
    {
        const float diameter = 1f;
        const int tessellation = 16;

        /// <summary>
        /// Constructs a new cube primitive, with the specified size.
        /// </summary>
        public SpherePrimitive(IGraphicsManager graphicsDevice)
        {
            Debug.Assert(tessellation >= 3);


            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;

            float radius = diameter / 2;

            // Start with a single vertex at the bottom of the sphere.
            AddVertex(Vector3.Down * radius, Vector3.Down);

            float pi; Maths.Pi(out pi);
            float piOver2 = pi / 2;
            // Create rings of vertices at progressively higher latitudes.
            for (int i = 0; i < verticalSegments - 1; i++)
            {
                float latitude = ((i + 1) * pi /
                                            verticalSegments) - piOver2;

                float dy = (float)Math.Sin(latitude);
                float dxz = (float)Math.Cos(latitude);

                float tau; Maths.Tau(out tau);

                // Create a single ring of vertices at this latitude.
                for (int j = 0; j < horizontalSegments; j++)
                {
                    float longitude = j * tau / horizontalSegments;

                    float dx = (float)Math.Cos(longitude) * dxz;
                    float dz = (float)Math.Sin(longitude) * dxz;

                    Vector3 normal = new Vector3(dx, dy, dz);

                    AddVertex(normal * radius, normal);
                }
            }

            // Finish with a single vertex at the top of the sphere.
            AddVertex(Vector3.Up * radius, Vector3.Up);

            // Create a fan connecting the bottom vertex to the bottom latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                AddIndex(0);
                AddIndex(1 + (i + 1) % horizontalSegments);
                AddIndex(1 + i);
            }

            // Fill the sphere body with triangles joining each pair of latitude rings.
            for (int i = 0; i < verticalSegments - 2; i++)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % horizontalSegments;

                    AddIndex(1 + i * horizontalSegments + j);
                    AddIndex(1 + i * horizontalSegments + nextJ);
                    AddIndex(1 + nextI * horizontalSegments + j);

                    AddIndex(1 + i * horizontalSegments + nextJ);
                    AddIndex(1 + nextI * horizontalSegments + nextJ);
                    AddIndex(1 + nextI * horizontalSegments + j);
                }
            }

            // Create a fan connecting the top vertex to the top latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                AddIndex(CurrentVertex - 1);
                AddIndex(CurrentVertex - 2 - (i + 1) % horizontalSegments);
                AddIndex(CurrentVertex - 2 - i);
            }

            InitializePrimitive(graphicsDevice);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class TeapotPrimitive
        : BezierPrimitive
    {
        const float size = 1;
        const int tessellation = 8;
        /// <summary>
        /// Constructs a new cube primitive, with the specified size.
        /// </summary>
        public TeapotPrimitive(IGraphicsManager graphicsDevice)
        {
            Debug.Assert(tessellation >= 1);

            foreach (TeapotPatch patch in TeapotPatches)
            {
                // Because the teapot is symmetrical from left to right, we only store
                // data for one side, then tessellate each patch twice, mirroring in X.
                TessellatePatch(patch, tessellation, new Vector3(size, size, size));
                TessellatePatch(patch, tessellation, new Vector3(-size, size, size));

                if (patch.MirrorZ)
                {
                    // Some parts of the teapot (the body, lid, and rim, but not the
                    // handle or spout) are also symmetrical from front to back, so
                    // we tessellate them four times, mirroring in Z as well as X.
                    TessellatePatch(patch, tessellation, new Vector3(size, size, -size));
                    TessellatePatch(patch, tessellation, new Vector3(-size, size, -size));
                }
            }

            InitializePrimitive(graphicsDevice);
        }

        /// <summary>
        /// Tessellates the specified bezier patch.
        /// </summary>
        void TessellatePatch(TeapotPatch patch, int tessellation, Vector3 scale)
        {
            // Look up the 16 control points for this patch.
            Vector3[] controlPoints = new Vector3[16];

            for (int i = 0; i < 16; i++)
            {
                int index = patch.Indices[i];
                controlPoints[i] = TeapotControlPoints[index] * scale;
            }

            // Is this patch being mirrored?
            Boolean isMirrored = Math.Sign(scale.X) != Math.Sign(scale.Z);

            // Create the index and vertex data.
            CreatePatchIndices(tessellation, isMirrored);
            CreatePatchVertices(controlPoints, tessellation, isMirrored);
        }


        /// <summary>
        /// The teapot model consists of 10 bezier patches. Each patch has 16 control
        /// points, plus a flag indicating whether it should be mirrored in the Z axis
        /// as well as in X (all of the teapot is symmetrical from left to right, but
        /// only some parts are symmetrical from front to back). The control points
        /// are stored as integer indices into the TeapotControlPoints array.
        /// </summary>
        class TeapotPatch
        {
            public readonly int[] Indices;
            public readonly Boolean MirrorZ;


            public TeapotPatch(Boolean mirrorZ, int[] indices)
            {
                Debug.Assert(indices.Length == 16);

                this.Indices = indices;
                this.MirrorZ = mirrorZ;
            }
        }


        /// <summary>
        /// Static data array defines the bezier patches that make up the teapot.
        /// </summary>
        static TeapotPatch[] TeapotPatches =
        {
            // Rim.
            new TeapotPatch(true, new int[]
            {
                102, 103, 104, 105, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15
            }),

            // Body.
            new TeapotPatch (true, new int[]
            {
                12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27
            }),

            new TeapotPatch(true, new int[]
            {
                24, 25, 26, 27, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40
            }),

            // Lid.
            new TeapotPatch(true, new int[]
            {
                96, 96, 96, 96, 97, 98, 99, 100, 101, 101, 101, 101, 0, 1, 2, 3
            }),

            new TeapotPatch(true, new int[]
            {
                0, 1, 2, 3, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117
            }),

            // Handle.
            new TeapotPatch(false, new int[]
            {
                41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56
            }),

            new TeapotPatch(false, new int[]
            {
                53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 28, 65, 66, 67
            }),

            // Spout.
            new TeapotPatch(false, new int[]
            {
                68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83
            }),

            new TeapotPatch(false, new int[]
            {
                80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95
            }),

            // Bottom.
            new TeapotPatch(true, new int[]
            {
                118, 118, 118, 118, 124, 122, 119, 121,
                123, 126, 125, 120, 40, 39, 38, 37
            }),
        };


        /// <summary>
        /// Static array defines the control point positions that make up the teapot.
        /// </summary>
        static Vector3[] TeapotControlPoints =
        {
            new Vector3(0f, 0.345f, -0.05f),
            new Vector3(-0.028f, 0.345f, -0.05f),
            new Vector3(-0.05f, 0.345f, -0.028f),
            new Vector3(-0.05f, 0.345f, -0f),
            new Vector3(0f, 0.3028125f, -0.334375f),
            new Vector3(-0.18725f, 0.3028125f, -0.334375f),
            new Vector3(-0.334375f, 0.3028125f, -0.18725f),
            new Vector3(-0.334375f, 0.3028125f, -0f),
            new Vector3(0f, 0.3028125f, -0.359375f),
            new Vector3(-0.20125f, 0.3028125f, -0.359375f),
            new Vector3(-0.359375f, 0.3028125f, -0.20125f),
            new Vector3(-0.359375f, 0.3028125f, -0f),
            new Vector3(0f, 0.27f, -0.375f),
            new Vector3(-0.21f, 0.27f, -0.375f),
            new Vector3(-0.375f, 0.27f, -0.21f),
            new Vector3(-0.375f, 0.27f, -0f),
            new Vector3(0f, 0.13875f, -0.4375f),
            new Vector3(-0.245f, 0.13875f, -0.4375f),
            new Vector3(-0.4375f, 0.13875f, -0.245f),
            new Vector3(-0.4375f, 0.13875f, -0f),
            new Vector3(0f, 0.007499993f, -0.5f),
            new Vector3(-0.28f, 0.007499993f, -0.5f),
            new Vector3(-0.5f, 0.007499993f, -0.28f),
            new Vector3(-0.5f, 0.007499993f, -0f),
            new Vector3(0f, -0.105f, -0.5f),
            new Vector3(-0.28f, -0.105f, -0.5f),
            new Vector3(-0.5f, -0.105f, -0.28f),
            new Vector3(-0.5f, -0.105f, -0f),
            new Vector3(0f, -0.105f, 0.5f),
            new Vector3(0f, -0.2175f, -0.5f),
            new Vector3(-0.28f, -0.2175f, -0.5f),
            new Vector3(-0.5f, -0.2175f, -0.28f),
            new Vector3(-0.5f, -0.2175f, -0f),
            new Vector3(0f, -0.27375f, -0.375f),
            new Vector3(-0.21f, -0.27375f, -0.375f),
            new Vector3(-0.375f, -0.27375f, -0.21f),
            new Vector3(-0.375f, -0.27375f, -0f),
            new Vector3(0f, -0.2925f, -0.375f),
            new Vector3(-0.21f, -0.2925f, -0.375f),
            new Vector3(-0.375f, -0.2925f, -0.21f),
            new Vector3(-0.375f, -0.2925f, -0f),
            new Vector3(0f, 0.17625f, 0.4f),
            new Vector3(-0.075f, 0.17625f, 0.4f),
            new Vector3(-0.075f, 0.2325f, 0.375f),
            new Vector3(0f, 0.2325f, 0.375f),
            new Vector3(0f, 0.17625f, 0.575f),
            new Vector3(-0.075f, 0.17625f, 0.575f),
            new Vector3(-0.075f, 0.2325f, 0.625f),
            new Vector3(0f, 0.2325f, 0.625f),
            new Vector3(0f, 0.17625f, 0.675f),
            new Vector3(-0.075f, 0.17625f, 0.675f),
            new Vector3(-0.075f, 0.2325f, 0.75f),
            new Vector3(0f, 0.2325f, 0.75f),
            new Vector3(0f, 0.12f, 0.675f),
            new Vector3(-0.075f, 0.12f, 0.675f),
            new Vector3(-0.075f, 0.12f, 0.75f),
            new Vector3(0f, 0.12f, 0.75f),
            new Vector3(0f, 0.06375f, 0.675f),
            new Vector3(-0.075f, 0.06375f, 0.675f),
            new Vector3(-0.075f, 0.007499993f, 0.75f),
            new Vector3(0f, 0.007499993f, 0.75f),
            new Vector3(0f, -0.04875001f, 0.625f),
            new Vector3(-0.075f, -0.04875001f, 0.625f),
            new Vector3(-0.075f, -0.09562501f, 0.6625f),
            new Vector3(0f, -0.09562501f, 0.6625f),
            new Vector3(-0.075f, -0.105f, 0.5f),
            new Vector3(-0.075f, -0.18f, 0.475f),
            new Vector3(0f, -0.18f, 0.475f),
            new Vector3(0f, 0.02624997f, -0.425f),
            new Vector3(-0.165f, 0.02624997f, -0.425f),
            new Vector3(-0.165f, -0.18f, -0.425f),
            new Vector3(0f, -0.18f, -0.425f),
            new Vector3(0f, 0.02624997f, -0.65f),
            new Vector3(-0.165f, 0.02624997f, -0.65f),
            new Vector3(-0.165f, -0.12375f, -0.775f),
            new Vector3(0f, -0.12375f, -0.775f),
            new Vector3(0f, 0.195f, -0.575f),
            new Vector3(-0.0625f, 0.195f, -0.575f),
            new Vector3(-0.0625f, 0.17625f, -0.6f),
            new Vector3(0f, 0.17625f, -0.6f),
            new Vector3(0f, 0.27f, -0.675f),
            new Vector3(-0.0625f, 0.27f, -0.675f),
            new Vector3(-0.0625f, 0.27f, -0.825f),
            new Vector3(0f, 0.27f, -0.825f),
            new Vector3(0f, 0.28875f, -0.7f),
            new Vector3(-0.0625f, 0.28875f, -0.7f),
            new Vector3(-0.0625f, 0.2934375f, -0.88125f),
            new Vector3(0f, 0.2934375f, -0.88125f),
            new Vector3(0f, 0.28875f, -0.725f),
            new Vector3(-0.0375f, 0.28875f, -0.725f),
            new Vector3(-0.0375f, 0.298125f, -0.8625f),
            new Vector3(0f, 0.298125f, -0.8625f),
            new Vector3(0f, 0.27f, -0.7f),
            new Vector3(-0.0375f, 0.27f, -0.7f),
            new Vector3(-0.0375f, 0.27f, -0.8f),
            new Vector3(0f, 0.27f, -0.8f),
            new Vector3(0f, 0.4575f, -0f),
            new Vector3(0f, 0.4575f, -0.2f),
            new Vector3(-0.1125f, 0.4575f, -0.2f),
            new Vector3(-0.2f, 0.4575f, -0.1125f),
            new Vector3(-0.2f, 0.4575f, -0f),
            new Vector3(0f, 0.3825f, -0f),
            new Vector3(0f, 0.27f, -0.35f),
            new Vector3(-0.196f, 0.27f, -0.35f),
            new Vector3(-0.35f, 0.27f, -0.196f),
            new Vector3(-0.35f, 0.27f, -0f),
            new Vector3(0f, 0.3075f, -0.1f),
            new Vector3(-0.056f, 0.3075f, -0.1f),
            new Vector3(-0.1f, 0.3075f, -0.056f),
            new Vector3(-0.1f, 0.3075f, -0f),
            new Vector3(0f, 0.3075f, -0.325f),
            new Vector3(-0.182f, 0.3075f, -0.325f),
            new Vector3(-0.325f, 0.3075f, -0.182f),
            new Vector3(-0.325f, 0.3075f, -0f),
            new Vector3(0f, 0.27f, -0.325f),
            new Vector3(-0.182f, 0.27f, -0.325f),
            new Vector3(-0.325f, 0.27f, -0.182f),
            new Vector3(-0.325f, 0.27f, -0f),
            new Vector3(0f, -0.33f, -0f),
            new Vector3(-0.1995f, -0.33f, -0.35625f),
            new Vector3(0f, -0.31125f, -0.375f),
            new Vector3(0f, -0.33f, -0.35625f),
            new Vector3(-0.35625f, -0.33f, -0.1995f),
            new Vector3(-0.375f, -0.31125f, -0f),
            new Vector3(-0.35625f, -0.33f, -0f),
            new Vector3(-0.21f, -0.31125f, -0.375f),
            new Vector3(-0.375f, -0.31125f, -0.21f),
        };
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class TorusPrimitive
        : GeometricPrimitive
    {
        const float diameter = 1f;
        const float thickness = 0.333f;
        const int tessellation = 32;

        /// <summary>
        /// Constructs a new cube primitive, with the specified size.
        /// </summary>
        public TorusPrimitive(IGraphicsManager graphicsDevice)
        {
            Debug.Assert(tessellation >= 3);

            // First we loop around the main ring of the torus.
            for (int i = 0; i < tessellation; i++)
            {
                float tau; Maths.Tau(out tau);
                float outerAngle = i * tau / tessellation;

                // Create a transform matrix that will align geometry to
                // slice perpendicularly though the current ring position.
                Matrix44 translation = Matrix44.CreateTranslation(diameter / 2, 0, 0);

                Matrix44 rotationY = Matrix44.CreateRotationY(outerAngle);

                Matrix44 transform = translation * rotationY;

                // Now we loop along the other axis, around the side of the tube.
                for (int j = 0; j < tessellation; j++)
                {
                    float innerAngle = j * tau / tessellation;

                    float dx = (float)Math.Cos(innerAngle);
                    float dy = (float)Math.Sin(innerAngle);

                    // Create a vertex.
                    Vector3 normalIn = new Vector3(dx, dy, 0);
                    Vector3 positionIn = normalIn * thickness / 2;

                    Vector3 position;
                    Vector3.Transform(ref positionIn, ref transform, out position);

                    Vector3 normal;
                    Vector3.TransformNormal(ref normalIn, ref transform, out normal);

                    AddVertex(position, normal);

                    // And create indices for two triangles.
                    int nextI = (i + 1) % tessellation;
                    int nextJ = (j + 1) % tessellation;

                    AddIndex(i * tessellation + j);
                    AddIndex(i * tessellation + nextJ);
                    AddIndex(nextI * tessellation + j);

                    AddIndex(i * tessellation + nextJ);
                    AddIndex(nextI * tessellation + nextJ);
                    AddIndex(nextI * tessellation + j);
                }
            }

            InitializePrimitive(graphicsDevice);
        }
    }
}
