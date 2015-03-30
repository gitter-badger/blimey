namespace PlatformDemo
{
    using System;
    using System.Text;
    using System.IO;
    using Abacus.SinglePrecision;
    using Fudge;
    using Blimey;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public interface IMesh<T> where T : IVertexType
    {
        VertexDeclaration VertexDeclaration { get; }
        T[] VertArray { get; }
        Int32[] IndexArray { get; }
    }

    public class BillboardPosTexCol : IMesh <VertPosTexCol>
    {
        readonly VertPosTexCol[] vertArray = new VertPosTexCol[4];
        readonly Int32[] indexArray = new Int32[6];

        #region IMesh <VertPosTexCol>

        public VertPosTexCol[] VertArray { get { return vertArray; } }
        public Int32[] IndexArray { get { return indexArray; } }
        public VertexDeclaration VertexDeclaration { get { return vertArray [0].VertexDeclaration; } }

        #endregion

        public BillboardPosTexCol()
        {
            // Six indices (two triangles) per face.
            indexArray[0] = 0;
            indexArray[1] = 3;
            indexArray[2] = 2;

            indexArray[3] = 0;
            indexArray[4] = 1;
            indexArray[5] = 3;

            // Four vertices per face.
            vertArray[0] = new VertPosTexCol(new Vector3(-0.5f, -0.5f, 0f), new Vector2(0f, 0f), Rgba32.Yellow);
            vertArray[1] = new VertPosTexCol(new Vector3(-0.5f,  0.5f, 0f), new Vector2(0f, 1f), Rgba32.Green);
            vertArray[2] = new VertPosTexCol(new Vector3( 0.5f, -0.5f, 0f), new Vector2(1f, 0f), Rgba32.Blue);
            vertArray[3] = new VertPosTexCol(new Vector3( 0.5f,  0.5f, 0f), new Vector2(1f, 1f), Rgba32.Red);
        }
    }

    public class BillboardPosTex : IMesh <VertPosTex>
    {
        readonly VertPosTex[] vertArray = new VertPosTex[4];
        readonly Int32[] indexArray = new Int32[6];

        #region IMesh <VertPosTexCol>

        public VertPosTex[] VertArray { get { return vertArray; } }
        public Int32[] IndexArray { get { return indexArray; } }
        public VertexDeclaration VertexDeclaration { get { return vertArray [0].VertexDeclaration; } }

        #endregion

        public BillboardPosTex()
        {
            // Six indices (two triangles) per face.
            indexArray[0] = 0;
            indexArray[1] = 3;
            indexArray[2] = 2;

            indexArray[3] = 0;
            indexArray[4] = 1;
            indexArray[5] = 3;

            // Four vertices per face.
            vertArray[0] = new VertPosTex(new Vector3(-0.5f, -0.5f, 0f), new Vector2(0f, 0f));
            vertArray[1] = new VertPosTex(new Vector3(-0.5f,  0.5f, 0f), new Vector2(0f, 1f));
            vertArray[2] = new VertPosTex(new Vector3( 0.5f, -0.5f, 0f), new Vector2(1f, 0f));
            vertArray[3] = new VertPosTex(new Vector3( 0.5f,  0.5f, 0f), new Vector2(1f, 1f));
        }
    }

    public class FlowerPosCol : IMesh <VertPosCol>
    {
        readonly VertPosCol[] vertArray;
        readonly Int32[] indexArray;

        #region IMesh <VertPosCol>

        public VertPosCol[] VertArray { get { return vertArray; } }
        public Int32[] IndexArray { get { return indexArray; } }
        public VertexDeclaration VertexDeclaration { get { return vertArray [0].VertexDeclaration; } }

        #endregion

        public FlowerPosCol ()
        {
            vertArray = new[]
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

            indexArray = new [] {
                // Top
                0, 1, 3, 0, 3, 2, 3, 1, 4, 3, 4, 2,
                // Bottom
                0, 7, 5, 0, 6, 7, 7, 8, 5, 7, 6, 8,
                // Left
                0, 9, 11, 0, 11, 10, 11, 9, 12, 11, 12, 10,
                // Right
                0, 15, 13, 0, 14, 15, 15, 16, 13, 15, 14, 16
            };
        }
    }

    public class FlowerPos : IMesh <VertPos>
    {
        readonly VertPos[] vertArray;
        readonly Int32[] indexArray;

        #region IMesh <VertPos>

        public VertPos[] VertArray { get { return vertArray; } }
        public Int32[] IndexArray { get { return indexArray; } }
        public VertexDeclaration VertexDeclaration { get { return vertArray [0].VertexDeclaration; } }

        #endregion

        public FlowerPos ()
        {
            vertArray = new[]
            {
                new VertPos( new Vector3(0.0f, 0.0f, 0.0f) ),
                // Top
                new VertPos( new Vector3(-0.2f, 0.8f, 0.0f) ),
                new VertPos( new Vector3(0.2f, 0.8f, 0.0f) ),
                new VertPos( new Vector3(0.0f, 0.8f, 0.0f) ),
                new VertPos( new Vector3(0.0f, 1.0f, 0.0f) ),
                // Bottom
                new VertPos( new Vector3(-0.2f, -0.8f, 0.0f) ),
                new VertPos( new Vector3(0.2f, -0.8f, 0.0f) ),
                new VertPos( new Vector3(0.0f, -0.8f, 0.0f) ),
                new VertPos( new Vector3(0.0f, -1.0f, 0.0f) ),
                // Left
                new VertPos( new Vector3(-0.8f, -0.2f, 0.0f) ),
                new VertPos( new Vector3(-0.8f, 0.2f, 0.0f) ),
                new VertPos( new Vector3(-0.8f, 0.0f, 0.0f) ),
                new VertPos( new Vector3(-1.0f, 0.0f, 0.0f) ),
                // Right
                new VertPos( new Vector3(0.8f, -0.2f, 0.0f) ),
                new VertPos( new Vector3(0.8f, 0.2f, 0.0f) ),
                new VertPos( new Vector3(0.8f, 0.0f, 0.0f) ),
                new VertPos( new Vector3(1.0f, 0.0f, 0.0f) ),
            };

            indexArray = new [] {
                // Top
                0, 1, 3, 0, 3, 2, 3, 1, 4, 3, 4, 2,
                // Bottom
                0, 7, 5, 0, 6, 7, 7, 8, 5, 7, 6, 8,
                // Left
                0, 9, 11, 0, 11, 10, 11, 9, 12, 11, 12, 10,
                // Right
                0, 15, 13, 0, 14, 15, 15, 16, 13, 15, 14, 16
            };
        }
    }

    public class CubePosTex : IMesh <VertPosTex>
    {
        readonly VertPosTex[] vertArray;
        readonly Int32[] indexArray;

        #region IMesh <VertPosTex>

        public VertPosTex[] VertArray { get { return vertArray; } }
        public Int32[] IndexArray { get { return indexArray; } }
        public VertexDeclaration VertexDeclaration { get { return vertArray [0].VertexDeclaration; } }

        #endregion

        public CubePosTex()
        {
            var normals = new []
            {
                new Vector3 (0, 0, 1),
                new Vector3 (0, 0, -1),
                new Vector3 (1, 0, 0),
                new Vector3 (-1, 0, 0),
                new Vector3 (0, 1, 0),
                new Vector3 (0, -1, 0),
            };

            var indexList = new List<Int32>();
            var vertList = new List<VertPosTex>();

            // Create each face in turn.
            foreach (Vector3 normal in normals)
            {
                // Get two vectors perpendicular to the face normal and to each other.
                Vector3 side1 = new Vector3(normal.Y, normal.Z, normal.X);
                Vector3 side2;

                Vector3 n = normal;
                Vector3.Cross(ref n, ref side1, out side2);

                // Six indices (two triangles) per face.
                indexList.Add (vertList.Count + 0);
                indexList.Add (vertList.Count + 1);
                indexList.Add (vertList.Count + 2);

                indexList.Add (vertList.Count + 0);
                indexList.Add (vertList.Count + 2);
                indexList.Add (vertList.Count + 3);

                // Four vertices per face.
                vertList.Add(new VertPosTex((normal - side1 - side2) / 2f, /*normal,*/ new Vector2(0f, 0f)));
                vertList.Add(new VertPosTex((normal - side1 + side2) / 2f, /*normal,*/ new Vector2(1f, 0f)));
                vertList.Add(new VertPosTex((normal + side1 + side2) / 2f, /*normal,*/ new Vector2(1f, 1f)));
                vertList.Add(new VertPosTex((normal + side1 - side2) / 2f, /*normal,*/ new Vector2(0f, 1f)));
            }

            vertArray = vertList.ToArray ();
            indexArray = indexList.ToArray ();
        }
    }

    public class CylinderNormTexPos : IMesh <VertNormTexPos>
    {
        readonly VertNormTexPos[] vertArray;
        readonly Int32[] indexArray;

        #region IMesh <VertPosNormTex>

        public VertNormTexPos[] VertArray { get { return vertArray; } }
        public Int32[] IndexArray { get { return indexArray; } }
        public VertexDeclaration VertexDeclaration { get { return vertArray [0].VertexDeclaration; } }

        #endregion

        const int tessellation = 9; // must be greater han 2
        const float height = 0.5f;
        const float radius = 0.5f;

        public CylinderNormTexPos ()
        {
            var vertList = new List<VertNormTexPos>();
            var indexList = new List<Int32>();

            // Create a ring of triangles around the outside of the cylinder.
            for (Int32 i = 0; i <= tessellation; i++)
            {
                Vector3 normal = GetCircleVector(i);

                Vector3 topPos = normal * radius + Vector3.Up * height;
                Vector3 botPos = normal * radius + Vector3.Down * height;

                Single howFarRound = (Single)i / (Single)(tessellation);

                Vector2 topUV = new Vector2(howFarRound * 3f, 0f);
                Vector2 botUV = new Vector2(howFarRound * 3f, 1f);

                vertList.Add(new VertNormTexPos(normal, topUV, topPos));
                vertList.Add(new VertNormTexPos(normal, botUV, botPos));
            }

            for (Int32 i = 0; i < tessellation; i++)
            {
                indexList.Add(i * 2);
                indexList.Add(i * 2 + 1);
                indexList.Add(i * 2 + 2);

                indexList.Add(i * 2 + 1);
                indexList.Add(i * 2 + 3);
                indexList.Add(i * 2 + 2);
            }


            // Create flat triangle fan caps to seal the top and bottom.
            CreateCap(vertList, indexList, Vector3.Up);
            CreateCap(vertList, indexList, Vector3.Down);

            vertArray = vertList.ToArray ();
            indexArray = indexList.ToArray ();
        }

        /// Helper method creates a triangle fan to close the ends of the cylinder.
        static void CreateCap(List<VertNormTexPos> vertList, List<Int32> indexList, Vector3 normal)
        {
            // Create cap indices.
            for (int i = 0; i < tessellation - 2; i++)
            {
                if (normal.Y > 0)
                {
                    indexList.Add(vertList.Count);
                    indexList.Add(vertList.Count + (i + 1) % tessellation);
                    indexList.Add(vertList.Count + (i + 2) % tessellation);
                }
                else
                {
                    indexList.Add(vertList.Count);
                    indexList.Add(vertList.Count + (i + 2) % tessellation);
                    indexList.Add(vertList.Count + (i + 1) % tessellation);
                }
            }

            // Create cap vertices.
            for (int i = 0; i < tessellation; i++)
            {
                Vector3 circleVec = GetCircleVector(i);
                Vector3 position = circleVec * radius +
                    normal * height;

                vertList.Add(
                    new VertNormTexPos(
                        normal,
                        new Vector2((circleVec.X + 1f) / 2f, (circleVec.Z + 1f) / 2f),
                        position));
            }
        }


        /// Helper method computes a point on a circle.
        static Vector3 GetCircleVector(int i)
        {
            Single tau; Maths.Tau(out tau);
            float angle = i * tau / tessellation;

            float dx = (float)Math.Cos(angle);
            float dz = (float)Math.Sin(angle);

            return new Vector3(dx, 0, dz);
        }
    }

    public class CylinderPosNormTex : IMesh <VertPosNormTex>
    {
        readonly VertPosNormTex[] vertArray;
        readonly Int32[] indexArray;

        #region IMesh <VertPosNormTex>

        public VertPosNormTex[] VertArray { get { return vertArray; } }
        public Int32[] IndexArray { get { return indexArray; } }
        public VertexDeclaration VertexDeclaration { get { return vertArray [0].VertexDeclaration; } }

        #endregion

        const int tessellation = 9; // must be greater han 2
        const float height = 0.5f;
        const float radius = 0.5f;

        public CylinderPosNormTex ()
        {
            var vertList = new List<VertPosNormTex>();
            var indexList = new List<Int32>();

            // Create a ring of triangles around the outside of the cylinder.
            for (Int32 i = 0; i <= tessellation; i++)
            {
                Vector3 normal = GetCircleVector(i);

                Vector3 topPos = normal * radius + Vector3.Up * height;
                Vector3 botPos = normal * radius + Vector3.Down * height;

                Single howFarRound = (Single)i / (Single)(tessellation);

                Vector2 topUV = new Vector2(howFarRound * 3f, 0f);
                Vector2 botUV = new Vector2(howFarRound * 3f, 1f);

                vertList.Add(new VertPosNormTex(topPos, normal, topUV));
                vertList.Add(new VertPosNormTex(botPos, normal, botUV));
            }

            for (Int32 i = 0; i < tessellation; i++)
            {
                indexList.Add(i * 2);
                indexList.Add(i * 2 + 1);
                indexList.Add(i * 2 + 2);

                indexList.Add(i * 2 + 1);
                indexList.Add(i * 2 + 3);
                indexList.Add(i * 2 + 2);
            }


            // Create flat triangle fan caps to seal the top and bottom.
            CreateCap(vertList, indexList, Vector3.Up);
            CreateCap(vertList, indexList, Vector3.Down);

            vertArray = vertList.ToArray ();
            indexArray = indexList.ToArray ();
        }

        /// Helper method creates a triangle fan to close the ends of the cylinder.
        static void CreateCap(List<VertPosNormTex> vertList, List<Int32> indexList, Vector3 normal)
        {
            // Create cap indices.
            for (int i = 0; i < tessellation - 2; i++)
            {
                if (normal.Y > 0)
                {
                    indexList.Add(vertList.Count);
                    indexList.Add(vertList.Count + (i + 1) % tessellation);
                    indexList.Add(vertList.Count + (i + 2) % tessellation);
                }
                else
                {
                    indexList.Add(vertList.Count);
                    indexList.Add(vertList.Count + (i + 2) % tessellation);
                    indexList.Add(vertList.Count + (i + 1) % tessellation);
                }
            }

            // Create cap vertices.
            for (int i = 0; i < tessellation; i++)
            {
                Vector3 circleVec = GetCircleVector(i);
                Vector3 position = circleVec * radius +
                    normal * height;

                vertList.Add(
                    new VertPosNormTex(
                        position,
                        normal,
                        new Vector2((circleVec.X + 1f) / 2f, (circleVec.Z + 1f) / 2f)));
            }
        }


        /// Helper method computes a point on a circle.
        static Vector3 GetCircleVector(int i)
        {
            Single tau; Maths.Tau(out tau);
            float angle = i * tau / tessellation;

            float dx = (float)Math.Cos(angle);
            float dz = (float)Math.Sin(angle);

            return new Vector3(dx, 0, dz);
        }
    }

    public class CylinderPosTex : IMesh <VertPosTex>
    {
        readonly VertPosTex[] vertArray;
        readonly Int32[] indexArray;

        #region IMesh <VertPosTex>

        public VertPosTex[] VertArray { get { return vertArray; } }
        public Int32[] IndexArray { get { return indexArray; } }
        public VertexDeclaration VertexDeclaration { get { return vertArray [0].VertexDeclaration; } }

        #endregion

        const int tessellation = 9; // must be greater han 2
        const float height = 0.5f;
        const float radius = 0.5f;

        public CylinderPosTex ()
        {
            var vertList = new List<VertPosTex>();
            var indexList = new List<Int32>();

            // Create a ring of triangles around the outside of the cylinder.
            for (Int32 i = 0; i <= tessellation; i++)
            {
                Vector3 normal = GetCircleVector(i);

                Vector3 topPos = normal * radius + Vector3.Up * height;
                Vector3 botPos = normal * radius + Vector3.Down * height;

                Single howFarRound = (Single)i / (Single)(tessellation);

                Vector2 topUV = new Vector2(howFarRound * 3f, 0f);
                Vector2 botUV = new Vector2(howFarRound * 3f, 1f);

                vertList.Add(new VertPosTex(topPos, topUV));
                vertList.Add(new VertPosTex(botPos, botUV));
            }

            for (Int32 i = 0; i < tessellation; i++)
            {
                indexList.Add(i * 2);
                indexList.Add(i * 2 + 1);
                indexList.Add((i * 2 + 2));

                indexList.Add(i * 2 + 1);
                indexList.Add(i * 2 + 3);
                indexList.Add(i * 2 + 2);
            }


            // Create flat triangle fan caps to seal the top and bottom.
            CreateCap(vertList, indexList, Vector3.Up);
            CreateCap(vertList, indexList, Vector3.Down);

            vertArray = vertList.ToArray ();
            indexArray = indexList.ToArray ();
        }

        /// Helper method creates a triangle fan to close the ends of the cylinder.
        static void CreateCap(List<VertPosTex> vertList, List<Int32> indexList, Vector3 normal)
        {
            // Create cap indices.
            for (int i = 0; i < tessellation - 2; i++)
            {
                if (normal.Y > 0)
                {
                    indexList.Add(vertList.Count);
                    indexList.Add(vertList.Count + (i + 1) % tessellation);
                    indexList.Add(vertList.Count + (i + 2) % tessellation);
                }
                else
                {
                    indexList.Add(vertList.Count);
                    indexList.Add(vertList.Count + (i + 2) % tessellation);
                    indexList.Add(vertList.Count + (i + 1) % tessellation);
                }
            }

            // Create cap vertices.
            for (int i = 0; i < tessellation; i++)
            {
                Vector3 circleVec = GetCircleVector(i);
                Vector3 position = circleVec * radius +
                    normal * height;

                vertList.Add(
                    new VertPosTex(
                        position,
                        new Vector2((circleVec.X + 1f) / 2f, (circleVec.Z + 1f) / 2f)));
            }
        }


        /// Helper method computes a point on a circle.
        static Vector3 GetCircleVector(int i)
        {
            Single tau; Maths.Tau(out tau);
            float angle = i * tau / tessellation;

            float dx = (float)Math.Cos(angle);
            float dz = (float)Math.Sin(angle);

            return new Vector3(dx, 0, dz);
        }
    }
}
