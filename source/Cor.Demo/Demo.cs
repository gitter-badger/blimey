// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor - A Low Level, Cross Platform, 3D App Engine                                                               │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │                     Built by:                                                                                  │ \\
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

namespace Cor.Demo
{
    using System;
    using System.Text;
    using Abacus.SinglePrecision;
    using Fudge;
    using Cor;
    using System.Collections.Generic;
    using Cor.Platform;
    using System.Runtime.InteropServices;

    public static class Demo
    {
        public static IApp GetEntryPoint() { return basicApp; }
        public static AppSettings GetAppSettings() { return appSettings; }
        static IApp basicApp;
        static AppSettings appSettings;

        static Demo()
        {
            appSettings = new AppSettings ("Cor Demo") {
				FullScreen = true,
				MouseGeneratesTouches = true
			};

            basicApp = new BasicApp();
        }
    }

    public class BasicApp : IApp
    {
        IElement[] elements;
        Rgba32 currentColour = Rgba32.Black;
        Rgba32 nextColour = Rgba32.DarkSlateBlue;
        readonly Single colourChangeTime = 10f;
        Single colourChangeProgress = 0f;
        Single w;
        Single h;

        public void Start (Engine cor)
        {
            elements = new IElement[]
            {
            };

            Double s = Math.Sqrt (elements.Length);

            w = (Single) Math.Round (s);
            h = (Single) Math.Floor (s);

            foreach (var element in elements)
                element.Load (cor);
        }

        public Boolean Update (Engine cor, AppTime time)
        {
            if (cor.Input.Keyboard.IsFunctionalKeyDown (FunctionalKey.Escape))
                return true;

            colourChangeProgress += time.Delta / colourChangeTime;

            if (colourChangeProgress >= 1f)
            {
                colourChangeProgress = 0f;
                currentColour = nextColour;
                nextColour = RandomColours.GetNext();
            }

            foreach (var element in elements)
                element.Update (cor, time);

            return false;
        }

        public void Render (Engine cor)
        {
            cor.Graphics.ClearColourBuffer(Rgba32.Lerp (currentColour, nextColour, colourChangeProgress));
            cor.Graphics.ClearDepthBuffer(1f);

            Int32 x = 0;
            Int32 y = 0;
            foreach (var element in elements)
            {
                Single qw = 2f / w;
                Single qh = 2f / h;

                Matrix44 proj = 
                    Matrix44.CreateOrthographicOffCenter (
                        -1f + qw * x,
                        -1f + qw * (x + 1), 
                        -1f + qh * y,
                        -1f + qh * (y + 1), 
                        1f,
                        -1f);

                element.Render (cor, proj);

                if (++x >= w) {x = 0; ++y;} 
            }
        }

        public void Stop (Engine cor)
        {
            foreach (var element in elements)
                element.Unload ();
        }
    }

    public interface IElement
    {
        void Load (Engine engine);
        void Unload ();
        void Update(Engine engine, AppTime time);
        void Render (Engine engine, Matrix44 projection);

        Matrix44 World { get; }
        Matrix44 View { get; }
    }

    public class Element <TMesh, TVertType> : IElement where TMesh : IMesh<TVertType> where TVertType : struct, IVertexType
    {
        IMesh<TVertType> meshResource;
        Shader shader;

        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        public Matrix44 World { get; private set; }
        public Matrix44 View { get; private set; }

        public Element (IMesh<TVertType> meshResource, Shader shader)
        {
            this.meshResource = meshResource;
            this.shader = shader;
            View = Matrix44.CreateLookAt (Vector3.UnitZ, Vector3.Forward, Vector3.Up);
        }

        public void Load (Engine engine)
        {
            // put the mesh resource onto the gpu
            vertexBuffer = engine.Graphics.CreateVertexBuffer (meshResource.VertexDeclaration, meshResource.VertArray.Length);
            indexBuffer = engine.Graphics.CreateIndexBuffer (meshResource.IndexArray.Length);

            vertexBuffer.SetData <TVertType>(meshResource.VertArray);
            indexBuffer.SetData(meshResource.IndexArray);

            // don't need a reference to the mesh now as it lives on the GPU
            this.meshResource = null;
        }

        public void Unload ()
        {
            indexBuffer.Dispose ();
            vertexBuffer.Dispose ();

            indexBuffer = null;
            vertexBuffer = null;
        }

        public void Update(Engine engine, AppTime time)
        {
            Matrix44 rotation = Matrix44.CreateFromAxisAngle(Vector3.Backward, Maths.Sin(time.Elapsed));
            Matrix44 worldScale = Matrix44.CreateScale (0.9f);

            World = worldScale * rotation;
        }

        public void Render (Engine engine, Matrix44 projection)
        {
            engine.Graphics.SetActiveVertexBuffer (this.vertexBuffer);
            engine.Graphics.SetActiveIndexBuffer (this.indexBuffer);

            // set the variable on the shader to our desired variables
            shader.ResetVariables ();
            shader.SetVariable ("World", World);
            shader.SetVariable ("View", View);
            shader.SetVariable ("Projection", projection);
            shader.SetVariable ("MaterialColour", Rgba32.White);

            foreach (var pass in shader.Passes)
            {
                pass.Activate (vertexBuffer.VertexDeclaration);

                engine.Graphics.DrawIndexedPrimitives (
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    vertexBuffer.VertexCount,
                    0,
                    indexBuffer.IndexCount / 3);
            }
        }
    }

    public interface IMesh<T> where T : IVertexType
    {
        VertexDeclaration VertexDeclaration { get; }
        T[] VertArray { get; }
        Int32[] IndexArray { get; }
    }

    public static class RandomColours
    {
        readonly static Random random = new Random();

        public static Rgba32 GetNext()
        {
            const Single min = 0.25f;
            const Single max = 1f;

            Single r = (Single)random.NextDouble() * (max - min) + min;
            Single g = (Single)random.NextDouble() * (max - min) + min;
            Single b = (Single)random.NextDouble() * (max - min) + min;
            Single a = 1f;

            return new Rgba32(r, g, b, a);
        }
    }

    #region Vertex Formats

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

    [StructLayout (LayoutKind.Sequential)]
    public struct VertPosTex : IVertexType
    {
        readonly static VertexDeclaration _vertexDeclaration;

        static VertPosTex ()
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
                    0)
            );
        }

        public Vector3 Position;
        public Vector2 UV;

        public VertPosTex (Vector3 position, Vector2 uv)
        {
            this.Position = position;
            this.UV = uv;
        }

        public VertexDeclaration VertexDeclaration { get { return _vertexDeclaration; } }
    }

    [StructLayout (LayoutKind.Sequential)]
    public struct VertPosNormTex : IVertexType
    {
        readonly static VertexDeclaration _vertexDeclaration;

        static VertPosNormTex ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Normal,
                    0),
                new VertexElement (
                    24,
                    VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate,
                    0)
            );
        }

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;

        public VertPosNormTex (Vector3 position, Vector3 normal, Vector2 uv)
        {
            this.Position = position;
            this.Normal = normal;
            this.UV = uv;
        }

        public VertexDeclaration VertexDeclaration { get { return _vertexDeclaration; } }
    }

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

    #endregion

    #region Meshs

    public class Billboard : IMesh <VertPosTexCol>
    {
        readonly VertPosTexCol[] vertArray = new VertPosTexCol[4];
        readonly Int32[] indexArray = new Int32[6];

        #region IMesh <VertPosTexCol>

        public VertPosTexCol[] VertArray { get { return vertArray; } }
        public Int32[] IndexArray { get { return indexArray; } }
        public VertexDeclaration VertexDeclaration { get { return vertArray [0].VertexDeclaration; } }

        #endregion

        public Billboard()
        {
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

    public class Star : IMesh <VertPosCol>
    {
        readonly VertPosCol[] vertArray;
        readonly Int32[] indexArray;

        #region IMesh <VertPosCol>

        public VertPosCol[] VertArray { get { return vertArray; } }
        public Int32[] IndexArray { get { return indexArray; } }
        public VertexDeclaration VertexDeclaration { get { return vertArray [0].VertexDeclaration; } }

        #endregion

        public Star ()
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

    public class Cube : IMesh <VertPosTex>
    {
        readonly VertPosTex[] vertArray;
        readonly Int32[] indexArray;

        #region IMesh <VertPosTex>

        public VertPosTex[] VertArray { get { return vertArray; } }
        public Int32[] IndexArray { get { return indexArray; } }
        public VertexDeclaration VertexDeclaration { get { return vertArray [0].VertexDeclaration; } }

        #endregion

        public Cube()
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

    public class Cylinder : IMesh <VertPosNormTex>
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

        public Cylinder ()
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

                Vector2 topUV = new Vector2(howFarRound, 0f);
                Vector2 botUV = new Vector2(howFarRound, 1f);

                vertList.Add(new VertPosNormTex(topPos, normal, topUV));
                vertList.Add(new VertPosNormTex(botPos, normal, botUV));
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

    #endregion

    #region Shader Definitions

    public static class ShaderHelper
    {
        public static Shader CreateUnlit (Engine engine)
        {
            return engine.Graphics.CreateShader (
                new ShaderDefinition {
                    Name = "Cor's Unlit Shader",
                    InputDefinitions = new List<ShaderDefinition.ShaderInputDefinition> {
                        new ShaderDefinition.ShaderInputDefinition
                        {
                            Name = "a_vertPos",
                            NiceName = "Position",
                            Optional = false,
                            Usage = VertexElementUsage.Position,
                            DefaultValue = Vector3.Zero
                        },
                        new ShaderDefinition.ShaderInputDefinition
                        {
                            Name = "a_vertTexcoord",
                            NiceName = "TextureCoordinate",
                            Optional = true,
                            Usage = VertexElementUsage.TextureCoordinate,
                            DefaultValue = Vector2.Zero
                        },
                        new ShaderDefinition.ShaderInputDefinition
                        {
                            Name = "a_vertColour",
                            NiceName = "Colour",
                            Optional = true,
                            Usage = VertexElementUsage.Colour,
                            DefaultValue = Rgba32.White
                        }
                    },
                    VariableDefinitions = new List<ShaderDefinition.ShaderVariableDefinition> {
                        new ShaderDefinition.ShaderVariableDefinition {
                            Name = "u_world",
                            NiceName = "World",
                            DefaultValue = Matrix44.Identity
                        },
                        new ShaderDefinition.ShaderVariableDefinition {
                            Name = "u_view",
                            NiceName = "View",
                            DefaultValue = Matrix44.Identity
                        },
                        new ShaderDefinition.ShaderVariableDefinition {
                            Name = "u_proj",
                            NiceName = "Projection",
                            DefaultValue = Matrix44.Identity
                        }
                    },
                    SamplerDefinitions = new List<ShaderDefinition.ShaderSamplerDefinition> {
                        new ShaderDefinition.ShaderSamplerDefinition
                        {
                            Name = "s_tex0",
                            NiceName = "TextureSampler",
                            Optional = true
                        }
                    },
                    PassNames = new List<String> () {
                        "Primary"
                    }
                },
                #if COR_PLATFORM_MONOMAC
                Encoding.ASCII.GetBytes (
                    @"
                    =VSH=
                    attribute vec4 a_vertPos;
                    uniform mat4 u_world;
                    uniform mat4 u_view;
                    uniform mat4 u_proj;
                    uniform vec4 u_colour;
                    varying vec4 v_tint;
                    void main()
                    {
                        gl_Position = u_proj * u_view * u_world * a_vertPos;
                        v_tint = u_colour;
                    }
                    =FSH=
                    varying vec4 v_tint;
                    void main()
                    {
                        gl_FragColor = v_tint;
                    }



                    =VSH=
                    attribute vec4 a_vertPos;
                    attribute vec4 a_vertColour;
                    uniform mat4 u_world;
                    uniform mat4 u_view;
                    uniform mat4 u_proj;
                    uniform vec4 u_colour;
                    varying vec4 v_tint;
                    void main()
                    {
                        gl_Position = u_proj * u_view * u_world * a_vertPos;
                        v_tint = a_vertColour * u_colour;
                    }
                    =FSH=
                    varying vec4 v_tint;
                    void main()
                    {
                        gl_FragColor = v_tint;
                    }



                    =VSH=
                    attribute vec4 a_vertPos;
                    attribute vec2 a_vertTexcoord;
                    uniform mat4 u_world;
                    uniform mat4 u_view;
                    uniform mat4 u_proj;
                    uniform vec4 u_colour;
                    varying vec2 v_texCoord;
                    varying vec4 v_tint;
                    void main()
                    {
                        gl_Position = u_proj * u_view * u_world * a_vertPos;
                        v_texCoord = a_vertTexcoord;
                        v_tint = u_colour;
                    }
                    =FSH=
                    uniform sampler2D s_tex0;
                    varying vec2 v_texCoord;
                    varying vec4 v_tint;
                    void main()
                    {
                        gl_FragColor = v_tint * texture2D(s_tex0, v_texCoord);
                    }



                    =VSH=
                    attribute vec4 a_vertPos;
                    attribute vec2 a_vertTexcoord;
                    attribute vec4 a_vertColour;
                    uniform mat4 u_world;
                    uniform mat4 u_view;
                    uniform mat4 u_proj;
                    uniform vec4 u_colour;
                    varying vec2 v_texCoord;
                    varying vec4 v_tint;
                    void main()
                    {
                        gl_Position = u_proj * u_view * u_world * a_vertPos;
                        v_texCoord = a_vertTexcoord;
                        vec4 c = a_vertColour;
                        c.a = 1.0;
                        v_tint = c * u_colour;
                    }
                    =FSH=
                    uniform sampler2D s_tex0;
                    varying vec2 v_texCoord;
                    varying vec4 v_tint;
                    void main()
                    {
                        gl_FragColor = v_tint * texture2D(s_tex0, v_texCoord);
                    }
                    "
                )
                #elif COR_PLATFORM_XAMARIN_IOS
                null
                #elif COR_PLATFORM_XNA
                null
                #else
                null
                #endif
            );
        }
    }

    #endregion
}

