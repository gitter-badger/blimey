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
    using System.Collections.Generic;
    using System.Linq;
    using Abacus.SinglePrecision;
    using Fudge;
    using global::Blimey.Platform;
    using global::Blimey.Asset;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    /// <summary>
    /// A system for handling rendering of various debug shapes.
    /// </summary>
    /// <remarks>
    /// The DebugShapeRenderer allows for rendering line-base shapes in a batched fashion. Games
    /// will call one of the many Add* methods to add a shape to the renderer and then a call to
    /// Draw will cause all shapes to be rendered. This mechanism was chosen because it allows
    /// game code to call the Add* methods wherever is most convenient, rather than having to
    /// add draw methods to all of the necessary objects.
    ///
    /// Additionally the renderer supports a lifetime for all shapes added. This allows for things
    /// like visualization of raycast bullets. The game would call the AddLine overload with the
    /// lifetime parameter and pass in a positive value. The renderer will then draw that shape
    /// for the given amount of time without any more calls to AddLine being required.
    ///
    /// The renderer's batching mechanism uses a cache system to avoid garbage and also draws as
    /// many lines in one call to DrawUserPrimitives as possible. If the renderer is trying to draw
    /// more lines than are allowed in the Reach profile, it will break them up into multiple draw
    /// calls to make sure the game continues to work for any game.
    /// </remarks>
    public sealed class DebugRenderer
    {
        Shader debugShader;

        public int NumActiveShapes { get { return activeShapes.Count; } }

        // A single shape in our debug renderer
        class DebugShape
        {
            /// <summary>
            /// The array of vertices the shape can use.
            /// </summary>
            public VertexPositionColour[] Vertices;

            /// <summary>
            /// The number of lines to draw for this shape.
            /// </summary>
            public int LineCount;

            /// <summary>
            /// The length of time to keep this shape visible.
            /// </summary>
            public float Lifetime;
        }

        // We use a cache system to reuse our DebugShape instances to avoid creating garbage
        readonly Dictionary <String, List<DebugShape>> cachedShapes = new Dictionary <String, List<DebugShape>>();
        readonly Dictionary <String, List<DebugShape>> activeShapes = new Dictionary <String, List<DebugShape>>();

        // Allocate an array to hold our vertices; this will grow as needed by our renderer
        VertexPositionColour[] verts = new VertexPositionColour[64];

        // An array we use to get platformners from frustums and bounding boxes
        Vector3[] platformners = new Vector3[8];

        // This holds the vertices for our unit sphere that we will use when drawing bounding spheres
        const int sphereResolution = 30;
        const int sphereLineCount = (sphereResolution + 1) * 3;
        Vector3[] unitSphere;

        internal DebugRenderer (Platform platform)
        {
            InitializeSphere();
            debugShader = CreateShader (platform);
        }

        internal void Update(AppTime time)
        {
            foreach (var pass in activeShapes.Keys)
            {
                // Go through our active shapes and retire any shapes that have expired to the
                // cache list.
                Boolean resort = false;
                for (int i = activeShapes[pass].Count - 1; i >= 0; i--)
                {
                    DebugShape s = activeShapes[pass] [i];

                    if (s.Lifetime < 0)
                    {
                        cachedShapes[pass].Add (s);
                        activeShapes[pass].RemoveAt (i);
                        resort = true;
                    }
                    else
                    {
                        s.Lifetime -= time.Delta;
                    }
                }

                // If we move any shapes around, we need to resort the cached list
                // to ensure that the smallest shapes are first in the list.
                if (resort)
                    this.cachedShapes[pass].Sort (CachedShapesSort);
            }
        }

        internal void Render(Graphics zGfx, string pass, Matrix44 zView, Matrix44 zProjection)
        {
            if (!activeShapes.ContainsKey(pass))
                return;

            var shapesForThisPass = this.activeShapes[pass];

            // Calculate the total number of vertices we're going to be rendering.
            int vertexCount = 0;
            foreach (var shape in shapesForThisPass)
                vertexCount += shape.LineCount * 2;

            // If we have some vertices to draw
            if (vertexCount > 0)
            {
                // Make sure our array is large enough
                if (verts.Length < vertexCount)
                {
                    // If we have to resize, we make our array twice as large as necessary so
                    // we hopefully won't have to resize it for a while.
                    verts = new VertexPositionColour[vertexCount * 2];
                }

                // Now go through the shapes again to move the vertices to our array and
                // add up the number of lines to draw.
                int lineCount = 0;
                int vertIndex = 0;
                foreach (DebugShape shape in shapesForThisPass)
                {
                    lineCount += shape.LineCount;
                    int shapeVerts = shape.LineCount * 2;
                    for (int i = 0; i < shapeVerts; i++)
                        verts[vertIndex++] = shape.Vertices[i];
                }

                zGfx.SetCullMode (CullMode.None);

                // Update the render states on the gpu
                zGfx.SetBlendEquation (BlendMode.Default);

                zGfx.SetActive ((VertexBuffer)null);
                zGfx.SetActive ((IndexBuffer)null);

                debugShader.SetVariable ("View", zView);
                debugShader.SetVariable ("Projection", zProjection);

                // Start our effect to begin rendering.
                zGfx.SetActive (debugShader, VertexPositionColour.Default.VertexDeclaration);

                // We draw in a loop because the Reach profile only supports 65,535 primitives. While it's
                // not incredibly likely, if a game tries to render more than 65,535 lines we don't want to
                // crash. We handle this by doing a loop and drawing as many lines as we can at a time, capped
                // at our limit. We then move ahead in our vertex array and draw the next set of lines.
                int vertexOffset = 0;
                while (lineCount > 0)
                {
                    // Figure out how many lines we're going to draw
                    int linesToDraw = Math.Min (lineCount, 65535);

                    FrameStats.Add ("DrawUserPrimitivesCount", 1);
                    zGfx.DrawUserPrimitives (PrimitiveType.LineList, verts, vertexOffset, linesToDraw);

                    // Move our vertex offset ahead based on the lines we drew
                    vertexOffset += linesToDraw * 2;

                    // Remove these lines from our total line count
                    lineCount -= linesToDraw;
                }
            }
        }

        static Shader CreateShader (Platform platform)
        {
            var shaderDecl =
                new ShaderDeclaration {
                Name = "Primitive Batch Shader",
                InputDeclarations = new List<ShaderInputDeclaration> {
                    new ShaderInputDeclaration {
                        Name = "a_vertPosition",
                        NiceName = "Position",
                        Optional = false,
                        Usage = VertexElementUsage.Position,
                        DefaultValue = Vector3.Zero
                    },
                    new ShaderInputDeclaration {
                        Name = "a_vertColour",
                        NiceName = "Colour",
                        Optional = false,
                        Usage = VertexElementUsage.Colour,
                        DefaultValue = Rgba32.White
                    }
                },
                VariableDeclarations = new List<ShaderVariableDeclaration> {
                    new ShaderVariableDeclaration {
                        Name = "u_view",
                        NiceName = "View",
                        DefaultValue = Matrix44.Identity
                    },
                    new ShaderVariableDeclaration {
                        Name = "u_proj",
                        NiceName = "Projection",
                        DefaultValue = Matrix44.Identity
                    }
                },
                SamplerDeclarations = new List<ShaderSamplerDeclaration> ()
            };


            var runtimeShaderFormat = platform.Graphics.GetRuntimeShaderFormat ();

            String source = "";

            if (runtimeShaderFormat == ShaderFormat.HLSL)
            {
                throw new NotImplementedException ();
            }
            else if (runtimeShaderFormat == ShaderFormat.GLSL)
            {
                source =
@"Debug Shader
=VSH=
attribute vec4 a_vertPosition;
attribute vec4 a_vertColour;
uniform mat4 u_view;
uniform mat4 u_proj;
varying vec4 v_tint;
void main()
{
    gl_Position = u_proj * u_view * a_vertPosition;
    gl_Position = u_proj * u_view * a_vertPosition;
    v_tint = a_vertColour;
}
=FSH=
varying vec4 v_tint;
void main()
{
    gl_FragColor = v_tint;
}
";
            }
            else if (runtimeShaderFormat == ShaderFormat.GLSL_ES)
            {
                source =
@"Debug Shader
=VSH=
attribute mediump vec4 a_vertPosition;
attribute mediump vec4 a_vertColour;
uniform mediump mat4 u_view;
uniform mediump mat4 u_proj;
varying mediump vec4 v_tint;
void main()
{
    gl_Position = u_proj * u_view * a_vertPosition;
    v_tint = a_vertColour;
}
=FSH=
varying mediump vec4 v_tint;
void main()
{
    gl_FragColor = v_tint;
}
";
            }
            else throw new NotSupportedException ();

            Byte[] shaderUTF8 = System.Text.Encoding.UTF8.GetBytes (source);

            using (var mem = new System.IO.MemoryStream ())
            {
                using (var bin = new System.IO.BinaryWriter (mem))
                {
                    bin.Write ((Byte)1);
                    bin.Write (shaderUTF8.Length);
                    bin.Write (shaderUTF8);
                }

                return platform.Graphics.CreateShader (
                    shaderDecl,
                    runtimeShaderFormat,
                    mem.GetBuffer ());
            }

            return null;
        }

        static int CachedShapesSort(DebugShape s1, DebugShape s2)
        {
            return s1.Vertices.Length.CompareTo(s2.Vertices.Length);
        }

        #region PUBLIC ~ Call these functions from the `Update` thread.

        public void AddQuad(string renderPass, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Rgba32 rgba)
        {
            AddQuad (renderPass, a, b, c, d, rgba, 0f);
        }

        public void AddQuad(string renderPass, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Rgba32 rgba, float life)
        {
            AddLine(renderPass, a, b, rgba, life);
            AddLine(renderPass, b, c, rgba, life);
            AddLine(renderPass, c, d, rgba, life);
            AddLine(renderPass, d, a, rgba, life);
        }

        public void AddLine(string renderPass, Vector3 a, Vector3 b, Rgba32 rgba)
        {
            AddLine(renderPass, a, b, rgba, 0f);
        }

        public void AddLine(string renderPass, Vector3 a, Vector3 b, Rgba32 rgba, float life)
        {
            // Get a DebugShape we can use to draw the line
            DebugShape shape = GetShapeForLines(renderPass, 1, life);

            // Add the two vertices to the shape
            shape.Vertices[0] = new VertexPositionColour(a, rgba);
            shape.Vertices[1] = new VertexPositionColour(b, rgba);
        }

        public void AddTriangle(string renderPass, Vector3 a, Vector3 b, Vector3 c, Rgba32 rgba)
        {
            AddTriangle(renderPass, a, b, c, rgba, 0f);
        }

        public void AddTriangle(string renderPass, Vector3 a, Vector3 b, Vector3 c, Rgba32 rgba, float life)
        {
            // Get a DebugShape we can use to draw the triangle
            DebugShape shape = GetShapeForLines(renderPass, 3, life);

            // Add the vertices to the shape
            shape.Vertices[0] = new VertexPositionColour(a, rgba);
            shape.Vertices[1] = new VertexPositionColour(b, rgba);
            shape.Vertices[2] = new VertexPositionColour(b, rgba);
            shape.Vertices[3] = new VertexPositionColour(c, rgba);
            shape.Vertices[4] = new VertexPositionColour(c, rgba);
            shape.Vertices[5] = new VertexPositionColour(a, rgba);
        }

        public void AddBoundingBox(string renderPass, BoundingBox box, Rgba32 col)
        {
            AddBoundingBox(renderPass, box, col, 0f);
        }

        public void AddBoundingBox(string renderPass, BoundingBox box, Rgba32 col, float life)
        {
            // Get a DebugShape we can use to draw the box
            DebugShape shape = GetShapeForLines(renderPass, 12, life);

            // Get the platformners of the box
            platformners = box.GetPlatformners();

            // Fill in the vertices for the bottom of the box
            shape.Vertices[0] = new VertexPositionColour(platformners[0], col);
            shape.Vertices[1] = new VertexPositionColour(platformners[1], col);
            shape.Vertices[2] = new VertexPositionColour(platformners[1], col);
            shape.Vertices[3] = new VertexPositionColour(platformners[2], col);
            shape.Vertices[4] = new VertexPositionColour(platformners[2], col);
            shape.Vertices[5] = new VertexPositionColour(platformners[3], col);
            shape.Vertices[6] = new VertexPositionColour(platformners[3], col);
            shape.Vertices[7] = new VertexPositionColour(platformners[0], col);

            // Fill in the vertices for the top of the box
            shape.Vertices[8] = new VertexPositionColour(platformners[4], col);
            shape.Vertices[9] = new VertexPositionColour(platformners[5], col);
            shape.Vertices[10] = new VertexPositionColour(platformners[5], col);
            shape.Vertices[11] = new VertexPositionColour(platformners[6], col);
            shape.Vertices[12] = new VertexPositionColour(platformners[6], col);
            shape.Vertices[13] = new VertexPositionColour(platformners[7], col);
            shape.Vertices[14] = new VertexPositionColour(platformners[7], col);
            shape.Vertices[15] = new VertexPositionColour(platformners[4], col);

            // Fill in the vertices for the vertical sides of the box
            shape.Vertices[16] = new VertexPositionColour(platformners[0], col);
            shape.Vertices[17] = new VertexPositionColour(platformners[4], col);
            shape.Vertices[18] = new VertexPositionColour(platformners[1], col);
            shape.Vertices[19] = new VertexPositionColour(platformners[5], col);
            shape.Vertices[20] = new VertexPositionColour(platformners[2], col);
            shape.Vertices[21] = new VertexPositionColour(platformners[6], col);
            shape.Vertices[22] = new VertexPositionColour(platformners[3], col);
            shape.Vertices[23] = new VertexPositionColour(platformners[7], col);
                 }

        #endregion

        void InitializeSphere()
        {
            // We need two vertices per line, so we can allocate our vertices
            unitSphere = new Vector3[sphereLineCount * 2];

            float tau; Maths.Tau(out tau);

            // Compute our step around each circle
            float step = tau / sphereResolution;

            // Used to track the index into our vertex array
            int index = 0;

            // Create the loop on the XY plane first
            for (float a = 0f; a < tau; a += step)
            {
                unitSphere[index++] = new Vector3((float)Math.Cos(a), (float)Math.Sin(a), 0f);
                unitSphere[index++] = new Vector3((float)Math.Cos(a + step), (float)Math.Sin(a + step), 0f);
            }

            // Next on the XZ plane
            for (float a = 0f; a < tau; a += step)
            {
                unitSphere[index++] = new Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a));
                unitSphere[index++] = new Vector3((float)Math.Cos(a + step), 0f, (float)Math.Sin(a + step));
            }

            // Finally on the YZ plane
            for (float a = 0f; a < tau; a += step)
            {
                unitSphere[index++] = new Vector3(0f, (float)Math.Cos(a), (float)Math.Sin(a));
                unitSphere[index++] = new Vector3(0f, (float)Math.Cos(a + step), (float)Math.Sin(a + step));
            }
        }

        DebugShape GetShapeForLines(String renderPass, int lineCount, float life)
        {
            if (!activeShapes.ContainsKey (renderPass))
            {
                cachedShapes [renderPass] = new List<DebugShape> ();
                activeShapes [renderPass] = new List<DebugShape> ();
            }

            DebugShape shape = null;

            // We go through our cached list trying to find a shape that contains
            // a large enough array to hold our desired line count. If we find such
            // a shape, we move it from our cached list to our active list and break
            // out of the loop.
            int vertCount = lineCount * 2;
            for (int i = 0; i < cachedShapes[renderPass].Count; i++)
            {
                if (cachedShapes[renderPass][i].Vertices.Length >= vertCount)
                {
                    shape = cachedShapes[renderPass][i];
                    cachedShapes[renderPass].RemoveAt(i);
                    activeShapes[renderPass].Add(shape);
                    break;
                }
            }

            // If we didn't find a shape in our cache, we create a new shape and add it
            // to the active list.
            if (shape == null)
            {
                shape = new DebugShape { Vertices = new VertexPositionColour[vertCount] };
                activeShapes[renderPass].Add(shape);
            }

            // Set the line count and lifetime of the shape based on our parameters.
            shape.LineCount = lineCount;
            shape.Lifetime = life;

            return shape;
        }
    }
}
