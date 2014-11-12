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
    using System.Collections.Generic;
    using System.Linq;

    using Abacus.SinglePrecision;
    using Fudge;
    using Cor;
    using Cor.Platform;

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
    /// calls to make sure the game continues to work for any game.</remarks>
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

        // An array we use to get corners from frustums and bounding boxes
        Vector3[] corners = new Vector3[8];

        // This holds the vertices for our unit sphere that we will use when drawing bounding spheres
        const int sphereResolution = 30;
        const int sphereLineCount = (sphereResolution + 1) * 3;
        Vector3[] unitSphere;

        internal DebugRenderer (Engine engine)
        {
            InitializeSphere();
            debugShader = CreateShader (engine);
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

            // Update the render states on the gpu
            zGfx.SetBlendEquation (BlendMode.Default);

            debugShader.SetVariable ("View", zView);
            debugShader.SetVariable ("Projection", zProjection);

            zGfx.GpuUtils.BeginEvent(Rgba32.Red, "DebugRenderer.Render");

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

                // Start our effect to begin rendering.
                debugShader.Activate (VertexPositionColour.Default.VertexDeclaration);

                // We draw in a loop because the Reach profile only supports 65,535 primitives. While it's
                // not incredibly likely, if a game tries to render more than 65,535 lines we don't want to
                // crash. We handle this by doing a loop and drawing as many lines as we can at a time, capped
                // at our limit. We then move ahead in our vertex array and draw the next set of lines.
                int vertexOffset = 0;
                while (lineCount > 0)
                {
                    // Figure out how many lines we're going to draw
                    int linesToDraw = Math.Min(lineCount, 65535);

                    FrameStats.DrawUserPrimitivesCount ++;
                    zGfx.DrawUserPrimitives(
                        PrimitiveType.LineList,
                        verts,
                        vertexOffset,
                        linesToDraw
                    );

                    // Move our vertex offset ahead based on the lines we drew
                    vertexOffset += linesToDraw * 2;

                    // Remove these lines from our total line count
                    lineCount -= linesToDraw;
                }

                zGfx.GpuUtils.EndEvent();
            }
        }

        static Shader CreateShader (Engine engine)
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
            Byte[] shaderUTF8 = null;

#if PLATFORM_MONOMAC
shaderUTF8 = System.Text.Encoding.UTF8.GetBytes(
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
    v_tint = a_vertColour;
}
=FSH=
varying vec4 v_tint;
void main()
{
    gl_FragColor = v_tint;
}
");
#elif PLATFORM_XIOS
shaderUTF8 = System.Text.Encoding.UTF8.GetBytes(
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
");
#endif
            using (var mem = new System.IO.MemoryStream ())
            {
                using (var bin = new System.IO.BinaryWriter (mem))
                {
                    bin.Write ((Byte)1);
                    bin.Write (shaderUTF8.Length);
                    bin.Write (shaderUTF8);
                }

                #if PLATFORM_MONOMAC
                return engine.Graphics.CreateShader (
                    shaderDecl,
                    ShaderFormat.GLSL,
                    mem.GetBuffer ());
                #elif PLATFORM_XIOS
                return engine.Graphics.CreateShader (
                    shaderDecl,
                    ShaderFormat.GLSL_ES,
                    mem.GetBuffer ());
                #endif
            }
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

            // Get the corners of the box
            corners = box.GetCorners();

            // Fill in the vertices for the bottom of the box
            shape.Vertices[0] = new VertexPositionColour(corners[0], col);
            shape.Vertices[1] = new VertexPositionColour(corners[1], col);
            shape.Vertices[2] = new VertexPositionColour(corners[1], col);
            shape.Vertices[3] = new VertexPositionColour(corners[2], col);
            shape.Vertices[4] = new VertexPositionColour(corners[2], col);
            shape.Vertices[5] = new VertexPositionColour(corners[3], col);
            shape.Vertices[6] = new VertexPositionColour(corners[3], col);
            shape.Vertices[7] = new VertexPositionColour(corners[0], col);

            // Fill in the vertices for the top of the box
            shape.Vertices[8] = new VertexPositionColour(corners[4], col);
            shape.Vertices[9] = new VertexPositionColour(corners[5], col);
            shape.Vertices[10] = new VertexPositionColour(corners[5], col);
            shape.Vertices[11] = new VertexPositionColour(corners[6], col);
            shape.Vertices[12] = new VertexPositionColour(corners[6], col);
            shape.Vertices[13] = new VertexPositionColour(corners[7], col);
            shape.Vertices[14] = new VertexPositionColour(corners[7], col);
            shape.Vertices[15] = new VertexPositionColour(corners[4], col);

            // Fill in the vertices for the vertical sides of the box
            shape.Vertices[16] = new VertexPositionColour(corners[0], col);
            shape.Vertices[17] = new VertexPositionColour(corners[4], col);
            shape.Vertices[18] = new VertexPositionColour(corners[1], col);
            shape.Vertices[19] = new VertexPositionColour(corners[5], col);
            shape.Vertices[20] = new VertexPositionColour(corners[2], col);
            shape.Vertices[21] = new VertexPositionColour(corners[6], col);
            shape.Vertices[22] = new VertexPositionColour(corners[3], col);
            shape.Vertices[23] = new VertexPositionColour(corners[7], col);
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


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class DebugShapeRendererExtensions
    {
        public static void AddGrid(
            this DebugRenderer debugRenderer,
            string renderPass, float gridSquareSize = 0.50f, int numberOfGridSquares = 10, float life = 0f,
            bool ShowXZPlane = true, bool ShowXYPlane = false, bool ShowYZPlane = false)
        {
            Rgba32 AxisColourX = Rgba32.Red;
            Rgba32 AxisColourY = Rgba32.Green;
            Rgba32 AxisColourZ = Rgba32.Blue;
            Rgba32 gridColour = Rgba32.LightGrey;

            float length = numberOfGridSquares * 2 * gridSquareSize;
            float halfLength = length / 2;

            if( ShowXZPlane )
            {
                for (int i = 0; i < (numberOfGridSquares * 2 + 1); i++)
                {
                    if (i * gridSquareSize - halfLength == 0)
                        continue;

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(-halfLength, 0.0f, i * gridSquareSize - halfLength),
                        new Vector3(halfLength, 0.0f, i * gridSquareSize - halfLength),
                        gridColour);

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(i * gridSquareSize - halfLength, 0.0f, -halfLength),
                        new Vector3(i * gridSquareSize - halfLength, 0.0f, halfLength),
                        gridColour);
                }
            }

            if( ShowXYPlane )
            {
                for (int i = 0; i < (numberOfGridSquares * 2 + 1); i++)
                {
                    if (i * gridSquareSize - halfLength == 0)
                        continue;

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(-halfLength, i * gridSquareSize - halfLength, 0f),
                        new Vector3(halfLength, i * gridSquareSize - halfLength, 0f),
                        gridColour);

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(i * gridSquareSize - halfLength, -halfLength, 0f),
                        new Vector3(i * gridSquareSize - halfLength, halfLength, 0f),
                        gridColour);
                }
            }

            if( ShowYZPlane )
            {
                for (int i = 0; i < (numberOfGridSquares * 2 + 1); i++)
                {
                    if (i * gridSquareSize - halfLength == 0)
                        continue;

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(0f, -halfLength, i * gridSquareSize - halfLength),
                        new Vector3(0f, halfLength, i * gridSquareSize - halfLength),
                        gridColour);

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(0f, i * gridSquareSize - halfLength, -halfLength),
                        new Vector3(0f, i * gridSquareSize - halfLength, halfLength),
                        gridColour);
                }
            }


            if( ShowXYPlane )
            {
                for (int i = 0; i < (numberOfGridSquares * 2 + 1); i++)
                {
                    if (i * gridSquareSize - halfLength == 0)
                        continue;

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(-halfLength, i * gridSquareSize - halfLength, 0f),
                        new Vector3(halfLength, i * gridSquareSize - halfLength, 0f),
                        gridColour);

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(i * gridSquareSize - halfLength, -halfLength, 0f),
                        new Vector3(i * gridSquareSize - halfLength, halfLength, 0f),
                        gridColour);
                }
            }

            debugRenderer.AddLine(
                renderPass,
                new Vector3(-numberOfGridSquares * gridSquareSize, 0, 0),
                new Vector3(0, 0, 0),
                Rgba32.White);

            debugRenderer.AddLine(
                renderPass,
                new Vector3(0, -numberOfGridSquares * gridSquareSize, 0),
                new Vector3(0, 0, 0),
                Rgba32.White);

            debugRenderer.AddLine(
                renderPass,
                new Vector3(0, 0, -numberOfGridSquares * gridSquareSize),
                new Vector3(0, 0, 0),
                Rgba32.White);


            debugRenderer.AddLine(
                renderPass,
                new Vector3(0, 0, 0),
                new Vector3(numberOfGridSquares * gridSquareSize, 0, 0),
                AxisColourX);

            debugRenderer.AddLine(
                renderPass,
                new Vector3(0, 0, 0),
                new Vector3(0, numberOfGridSquares * gridSquareSize, 0),
                AxisColourY);

            debugRenderer.AddLine(
                renderPass,
                new Vector3(0, 0, 0),
                new Vector3(0, 0, numberOfGridSquares * gridSquareSize),
                AxisColourZ);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    // Renders things in Screen space
    public sealed class PrimitiveRenderer
    {
        public class Sprite
        {
            protected Quad quad;
            protected float tx, ty, width, height;
            protected float tex_width, tex_height;
            protected float hotX, hotY;
            protected bool bXFlip, bYFlip, bHSFlip;

            readonly PrimitiveRenderer primitiveRenderer;

            public Sprite (PrimitiveRenderer zPrimitiveRenderer, Sprite zFrom)
            {
                primitiveRenderer = zPrimitiveRenderer;
                quad = new Quad(quad);
                tx = zFrom.tx;
                ty = zFrom.ty;
                width = zFrom.width;
                height = zFrom.height;
                tex_width = zFrom.tex_width;
                tex_height = zFrom.tex_height;
                hotX = zFrom.hotX;
                hotY = zFrom.hotY;
                bXFlip = zFrom.bXFlip;
                bYFlip = zFrom.bYFlip;
                bHSFlip = zFrom.bHSFlip;
            }

            public Sprite(PrimitiveRenderer zPrimitiveRenderer, Texture zTexture)
            {
                primitiveRenderer = zPrimitiveRenderer;
                float w = (float) zTexture.Width;
                float h = (float) zTexture.Height;
                Init(zTexture, 0, 0, w, h );
                SetHotSpot(w/2.0f, h/2.0f);
            }

            void Init (Texture texture, float texx, float texy, float w, float h)
            {
                quad = new Quad();
                float texx1, texy1, texx2, texy2;

                tx = texx; ty = texy;
                width = w; height = h;

                if (texture != null)
                {
                    tex_width = (float)texture.Width;
                    tex_height = (float)texture.Height;
                }
                else
                {
                    tex_width = 1.0f;
                    tex_height = 1.0f;
                }

                hotX = 0;
                hotY = 0;
                bXFlip = false;
                bYFlip = false;
                bHSFlip = false;
                quad.tex = texture;

                texx1 = texx / tex_width;
                texy1 = texy / tex_height;
                texx2 = (texx + w) / tex_width;
                texy2 = (texy + h) / tex_height;

                quad.v[0].UV.X = texx1; quad.v[0].UV.Y = texy1;
                quad.v[1].UV.X = texx2; quad.v[1].UV.Y = texy1;
                quad.v[2].UV.X = texx2; quad.v[2].UV.Y = texy2;
                quad.v[3].UV.X = texx1; quad.v[3].UV.Y = texy2;

                quad.v[0].Position.Z = quad.v[1].Position.Z = quad.v[2].Position.Z = quad.v[3].Position.Z = 0.5f;
                quad.v[0].Colour = quad.v[1].Colour = quad.v[2].Colour = quad.v[3].Colour = Rgba32.White;

                quad.blend = BlendMode.Default;
            }

            public Sprite(Texture texture, float texx, float texy, float w, float h)
            {
                Init(texture, texx, texy, w, h);
            }

            public void SetColour(Rgba32 _col)
            {
                quad.v[0].Colour = quad.v[1].Colour = quad.v[2].Colour = quad.v[3].Colour = _col;
            }

            public void SetColour(Rgba32 _col, uint i )
            {
                System.Diagnostics.Debug.Assert(i < 4);

                quad.v[i].Colour = _col;
            }

            public Rgba32 GetColour()
            {
                return quad.v[0].Colour;
            }

            public Rgba32 GetColour( uint i )
            {
                System.Diagnostics.Debug.Assert(i < 4);

                return quad.v[i].Colour;


            }

            public void SetHotSpot(float _x, float _y)
            {
                hotX = _x;
                hotY = _y;
            }

            public void CenterHotSpot()
            {
                hotX = width / 2;
                hotY = height / 2;
            }

            public Vector2 GetHotSpot()
            {
                return new Vector2( hotX, hotY );
            }

            public void GetHotSpot( ref float x, ref float y )
            {
                x = hotX; 
                y = hotY;
            }

            public void SetZ(float _z)
            {
                quad.v[0].Position.Z = quad.v[1].Position.Z = quad.v[2].Position.Z = quad.v[3].Position.Z = _z;
            }

            public float GetZ()
            {
                return quad.v[0].Position.Z;
            }

            public void SetZ(float _z, uint i)
            {
                System.Diagnostics.Debug.Assert(i < 4);

                quad.v[i].Position.Z = _z;
            }

            public float GetZ( uint i )
            {
                System.Diagnostics.Debug.Assert(i < 4);
                return quad.v[i].Position.Z;

            }

            public void SetFlip(bool _horizontal, bool _vertical)
            {
                SetFlip(_horizontal, _vertical, false);
            }

            public void SetFlip(bool _horizontal, bool _vertical, bool _bHotSpot)
            {
                float tx, ty;

                if (bHSFlip && bXFlip) hotX = width - hotX;
                if (bHSFlip && bYFlip) hotY = height - hotY;

                bHSFlip = _bHotSpot;

                if (bHSFlip && bXFlip) hotX = width - hotX;
                if (bHSFlip && bYFlip) hotY = height - hotY;

                if (_horizontal != bXFlip)
                {
                    tx = quad.v[0].UV.X;
                    quad.v[0].UV.X = quad.v[1].UV.X;
                    quad.v[1].UV.X = tx;
                    ty = quad.v[0].UV.Y; 
                    quad.v[0].UV.Y = quad.v[1].UV.Y; 
                    quad.v[1].UV.Y = ty;
                    tx = quad.v[3].UV.X;
                    quad.v[3].UV.X = quad.v[2].UV.X;
                    quad.v[2].UV.X = tx;
                    ty = quad.v[3].UV.Y; 
                    quad.v[3].UV.Y = quad.v[2].UV.Y; 
                    quad.v[2].UV.Y = ty;

                    bXFlip = !bXFlip;
                }

                if (_vertical != bYFlip)
                {
                    tx = quad.v[0].UV.X;
                    quad.v[0].UV.X = quad.v[3].UV.X;
                    quad.v[3].UV.X = tx;
                    ty = quad.v[0].UV.Y;
                    quad.v[0].UV.Y = quad.v[3].UV.Y;
                    quad.v[3].UV.Y = ty;
                    tx = quad.v[1].UV.X;
                    quad.v[1].UV.X = quad.v[2].UV.X;
                    quad.v[2].UV.X = tx;
                    ty = quad.v[1].UV.Y;
                    quad.v[1].UV.Y = quad.v[2].UV.Y;
                    quad.v[2].UV.Y = ty;

                    bYFlip = !bYFlip;
                }
            }

            public void Draw(String renderPass, float _x, float _y)
            {
                float tempx1, tempy1, tempx2, tempy2;

                tempx1 = _x - hotX;
                tempy1 = _y - hotY;
                tempx2 = _x + width - hotX;
                tempy2 = _y + height - hotY;

                quad.v[0].Position.X = tempx1; quad.v[0].Position.Y = tempy1;
                quad.v[1].Position.X = tempx2; quad.v[1].Position.Y = tempy1;
                quad.v[2].Position.X = tempx2; quad.v[2].Position.Y = tempy2;
                quad.v[3].Position.X = tempx1; quad.v[3].Position.Y = tempy2;

                primitiveRenderer.AddQuad(renderPass, quad);
            }

            public void DrawEx(String renderPass, float _x, float _y, float _rot, float _hscale, float _vscale)
            {

                float tx1, ty1, tx2, ty2;
                float sint, cost;

                if (_vscale == 0) _vscale = _hscale;

                tx1 = -hotX * _hscale;
                ty1 = -hotY * _vscale;
                tx2 = (width - hotX) * _hscale;
                ty2 = (height - hotY) * _vscale;

                if (_rot != 0.0f)
                {
                    cost = (float) Math.Cos((double) _rot);
                    sint = (float) Math.Sin((double)_rot);

                    quad.v[0].Position.X = tx1 * cost - ty1 * sint + _x;
                    quad.v[0].Position.Y = tx1 * sint + ty1 * cost + _y;

                    quad.v[1].Position.X = tx2 * cost - ty1 * sint + _x;
                    quad.v[1].Position.Y = tx2 * sint + ty1 * cost + _y;

                    quad.v[2].Position.X = tx2 * cost - ty2 * sint + _x;
                    quad.v[2].Position.Y = tx2 * sint + ty2 * cost + _y;

                    quad.v[3].Position.X = tx1 * cost - ty2 * sint + _x;
                    quad.v[3].Position.Y = tx1 * sint + ty2 * cost + _y;
                }
                else
                {
                    quad.v[0].Position.X = tx1 + _x;
                    quad.v[0].Position.Y = ty1 + _y;
                    quad.v[1].Position.X = tx2 + _x;
                    quad.v[1].Position.Y = ty1 + _y;
                    quad.v[2].Position.X = tx2 + _x;
                    quad.v[2].Position.Y = ty2 + _y;
                    quad.v[3].Position.X = tx1 + _x;
                    quad.v[3].Position.Y = ty2 + _y;
                }

                primitiveRenderer.AddQuad(renderPass, quad);

            }

            public void DrawEx(String renderPass, float _x, float _y, float _rot, float _hscale)
            {
                DrawEx(renderPass, _x, _y, _rot, _hscale, _hscale);
            }

            public void DrawStretch(String renderPass, float _x1, float _y1, float _x2, float _y2)
            {
                quad.v[0].Position.X = _x1; quad.v[0].Position.Y = _y1;
                quad.v[1].Position.X = _x2; quad.v[1].Position.Y = _y1;
                quad.v[2].Position.X = _x2; quad.v[2].Position.Y = _y2;
                quad.v[3].Position.X = _x1; quad.v[3].Position.Y = _y2;

                primitiveRenderer.AddQuad(renderPass, quad);

            }

            public void Draw4V(String renderPass, float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3)
            {
                quad.v[0].Position.X = x0; quad.v[0].Position.Y = y0;
                quad.v[1].Position.X = x1; quad.v[1].Position.Y = y1;
                quad.v[2].Position.X = x2; quad.v[2].Position.Y = y2;
                quad.v[3].Position.X = x3; quad.v[3].Position.Y = y3;


                primitiveRenderer.AddQuad(renderPass, quad);
            }

            public Texture GetTexture()
            { 
                return quad.tex; 
            }

            public void GetTextureRect(ref float x, ref float y, ref float w, ref float h)
            { 
                x=tx; 
                y=ty; 
                w=width; 
                h=height; 
            }

            public BlendMode GetBlendMode() 
            { 
                return quad.blend; 
            }

            public void SetBlendMode(BlendMode _blend) 
            { 
                quad.blend = _blend; 
            }

            public void SetTexture(Texture tex)
            {
                float tx1, ty1, tx2, ty2;
                float tw, th;

                quad.tex = tex;

                if (tex != null )
                {
                    tw = (float) tex.Width;
                    th = (float) tex.Height;
                }
                else
                {
                    tw = 1.0f;
                    th = 1.0f;
                }

                //if the size of the texture has changed
                if (tw != tex_width || th != tex_height)
                {
                    tx1 = quad.v[0].UV.X * tex_width;
                    ty1 = quad.v[0].UV.Y * tex_height;
                    tx2 = quad.v[2].UV.X * tex_width;
                    ty2 = quad.v[2].UV.Y * tex_height;

                    tex_width = tw;
                    tex_height = th;

                    tx1 /= tw; ty1 /= th;
                    tx2 /= tw; ty2 /= th;

                    quad.v[0].UV.X = tx1;
                    quad.v[0].UV.Y = ty1;
                    quad.v[1].UV.X = tx2;
                    quad.v[1].UV.Y = ty1;
                    quad.v[2].UV.X = tx2;
                    quad.v[2].UV.Y = ty2;
                    quad.v[3].UV.X = tx1;
                    quad.v[3].UV.Y = ty2;
                }
            }

            public void SetTextureRect(float x, float y, float w, float h)
            {
                SetTextureRect( x, y, w, h, true);
            }

            public void SetTextureRect(float x, float y, float w, float h, bool adjSize)
            {
                float tx1, ty1, tx2, ty2;
                bool bX, bY, bHS;

                tx = x;
                ty = y;

                if (adjSize)
                {
                    width = w;
                    height = h;
                }

                tx1 = tx / tex_width; ty1 = ty / tex_height;
                tx2 = (tx + w) / tex_width; ty2 = (ty + h) / tex_height;

                quad.v[0].UV.X = tx1;
                quad.v[0].UV.Y = ty1;
                quad.v[1].UV.X = tx2;
                quad.v[1].UV.Y = ty1;
                quad.v[2].UV.X = tx2;
                quad.v[2].UV.Y = ty2;
                quad.v[3].UV.X = tx1;
                quad.v[3].UV.Y = ty2;

                bX = bXFlip; bY = bYFlip; bHS = bHSFlip;
                bXFlip = false; bYFlip = false;
                SetFlip(bX, bY, bHS);
            }

            public float GetWidth()
            {
                return width;
            }

            public float GetHeight()
            {
                return height;
            }

        }

        public class Particle
        {
            public Vector2 vecLocation;
            public Vector2 vecVelocity;

            public float fGravity;
            public float fRadialAccel;
            public float fTangentialAccel;

            public float fSpin;
            public float fSpinDelta;

            public float fSize;
            public float fSizeDelta;

            public Rgba32 colColour;      // + alpha
            public Rgba32 colColourStart;
            public Rgba32 colColourEnd;

            public float fAge;
            public float fTerminalAge;

            public Particle(){}


            public Particle(Particle o)
            {
                this.vecLocation = o.vecLocation;
                this.vecVelocity = o.vecVelocity;

                this.fGravity = o.fGravity;
                this.fRadialAccel = o.fRadialAccel;
                this.fTangentialAccel = o.fTangentialAccel;
                this.fSpin = o.fSpin;
                this.fSpinDelta = o.fSpinDelta;

                this.fSize = o.fSize;
                this.fSizeDelta = o.fSizeDelta;

                this.colColour = o.colColour;     // + alpha
                this.colColourStart = o.colColourStart;
                this.colColourEnd = o.colColourEnd;

                this.fAge = o.fAge;
                this.fTerminalAge = o.fTerminalAge;
            } 
        }

        public class ParticleSystemInfo
        {
            public Sprite sprite;    // texture + blend mode
            public int nEmission; // particles per sec
            public float fLifetime;

            public float fParticleLifeMin;
            public float fParticleLifeMax;

            public float fDirection;
            public float fSpread;
            public bool bRelative;

            public float fSpeed;
            public float fGravity;
            public float fRadialAccel;
            public float fTangentialAccel;

            public float fSizeStart;
            public float fSizeEnd;
            public float fSizeVar;

            public float fSpinStart;
            public float fSpinEnd;
            public float fSpinVar;

            public Rgba32 colColourStart; // + alpha
            public Rgba32 colColourEnd;
            public float fColourStartVar;
            public float fColourEndVar;
        }

        public class BoundingRectangle
        {

            public float    x1, y1, x2, y2;
            bool    bClean;

            public BoundingRectangle(float _x1, float _y1, float _x2, float _y2) 
            {
                x1=_x1; 
                y1=_y1; 
                x2=_x2; 
                y2=_y2; 
                bClean=false; 
            }

            public BoundingRectangle() 
            {
                bClean=true;
            }

            public void Clear() 
            {
                bClean=true;
            }

            public bool IsClean()
            {
                return bClean;
            }

            public void Set(float _x1, float _y1, float _x2, float _y2) 
            { 
                x1=_x1; 
                x2=_x2; 
                y1=_y1; 
                y2=_y2; 
                bClean=false; 
            }

            public void SetRadius(float x, float y, float r) 
            { 
                x1=x-r; 
                x2=x+r; 
                y1=y-r; 
                y2=y+r; 
                bClean=false; 
            }

            public void Encapsulate(float x, float y)
            {
                if(bClean)
                {
                    x1=x2=x;
                    y1=y2=y;
                    bClean=false;
                }
                else
                {
                    if(x<x1) x1=x;
                    if(x>x2) x2=x;
                    if(y<y1) y1=y;
                    if(y>y2) y2=y;
                }
            }
            public bool TestPoint(float x, float y)
            {
                if(x>=x1 && x<x2 && y>=y1 && y<y2) 
                    return true;

                return false;
            }

            public bool Intersect(BoundingRectangle rect)
            {
                if(Math.Abs(x1 + x2 - rect.x1 - rect.x2) < (x2 - x1 + rect.x2 - rect.x1))
                if (Math.Abs(y1 + y2 - rect.y1 - rect.y2) < (y2 - y1 + rect.y2 - rect.y1))
                    return true;

                return false;
            }

        }

        public class ParticleSystem
        {
            static int MAX_PARTICLES = 5000;
            //static int MAX_PSYSTEMS   = 100;

            public ParticleSystemInfo info = new ParticleSystemInfo();

            BoundingRectangle rectBoundingBox = new BoundingRectangle();
            bool bUpdateBoundingBox;

            float               fAge;
            float               fEmissionResidue;

            Vector2             vecPrevLocation;
            Vector2             vecLocation;
            float               fTx, fTy;
            float               fScale;

            int                 nParticlesAlive;

            Particle[]          particles = new Particle[MAX_PARTICLES];

            public ParticleSystem(ParticleSystemInfo psi)
            {
                for (int i = 0; i < MAX_PARTICLES; i++)
                {
                    particles[i] = new Particle();
                }

                info = psi;

                vecLocation.X=vecPrevLocation.X=0.0f;
                vecLocation.Y=vecPrevLocation.Y=0.0f;
                fTx=fTy=0;
                fScale = 1.0f;

                fEmissionResidue=0.0f;
                nParticlesAlive=0;
                fAge=-2.0f;
            }

            public void TrackBoundingBox(bool bTrack) { bUpdateBoundingBox = bTrack; }

            public BoundingRectangle GetBoundingBox(ref BoundingRectangle rect)
            {
                rect = rectBoundingBox;

                rect.x1 *= fScale;
                rect.y1 *= fScale;
                rect.x2 *= fScale;
                rect.y2 *= fScale;

                return rect;
            }

            public void Draw(String renderPass)
            {
                Rgba32 col = info.sprite.GetColour();

                for (int i = 0; i < nParticlesAlive; i++ )
                {
                    Particle par = particles[i];

                    System.Diagnostics.Debug.Assert(par != null);
                    //Rgba32 temp = info.sprite.GetColour();
                    //temp.A = par.colColour.A;
                    info.sprite.SetColour(par.colColour);

                    info.sprite.DrawEx(renderPass,
                        par.vecLocation.X * fScale + fTx,
                        par.vecLocation.Y * fScale + fTy,
                        par.fSpin * par.fAge,
                        par.fSize * fScale);
                }

                // set the particle system's sprite back to the colour it was
                info.sprite.SetColour(col);
            }

            public void FireAt(float x, float y)
            {
                Stop();
                MoveTo(x, y);
                Fire();
            }

            public void Fire()
            {
                if (info.fLifetime == -1.0f) 
                    fAge = -1.0f;
                else 
                    fAge = 0.0f;
            }

            public void Stop()
            {
                Stop(false);
            }

            public void Stop(bool bKillParticles)
            {
                fAge = -2.0f;
                if (bKillParticles)
                {
                    nParticlesAlive = 0;
                    rectBoundingBox.Clear();
                }
            }

            public void Update(float fDeltaTime) 
            {
                int i;
                float ang;
                Particle par = null;
                Vector2 vecAccel, vecAccel2;

                if(fAge >= 0)
                {
                    fAge += fDeltaTime;
                    if(fAge >= info.fLifetime) 
                        fAge = -2.0f;
                }

                // update all alive particles
                if (bUpdateBoundingBox) 
                    rectBoundingBox.Clear();

                for(i=0; i<nParticlesAlive; i++)
                {
                    par=particles[i];

                    par.fAge += fDeltaTime;

                    //need to kill this particle
                    if(par.fAge >= par.fTerminalAge)
                    {
                        nParticlesAlive--;
                        particles[i] = new Particle(particles[nParticlesAlive]);
                        i--;
                        continue;
                    }

                    vecAccel = par.vecLocation-vecLocation;
                    vecAccel.Normalise ();
                    vecAccel2 = vecAccel;
                    vecAccel *= par.fRadialAccel;

                    // vecAccel2.Rotate(M_PI_2);
                    // the following is faster
                    ang = vecAccel2.X;
                    vecAccel2.X = -vecAccel2.Y;
                    vecAccel2.X = ang;

                    vecAccel2 *= par.fTangentialAccel;
                    par.vecVelocity += (vecAccel+vecAccel2)*fDeltaTime;
                    par.vecVelocity.Y += par.fGravity*fDeltaTime;

                    par.vecLocation += par.vecVelocity*fDeltaTime;

                    par.fSpin += par.fSpinDelta*fDeltaTime;
                    par.fSize += par.fSizeDelta*fDeltaTime;

                    float factor = par.fAge / par.fTerminalAge;
                    par.colColour = Rgba32.Lerp(par.colColourStart, par.colColourEnd, factor);
                    //par.colColour = new Rgba32(par.colColour.ToVector4() + (par.colColourEnd.ToVector4() * fDeltaTime));
                }

                if (bUpdateBoundingBox)
                    rectBoundingBox.Encapsulate(par.vecLocation.X, par.vecLocation.Y);

                // generate new particles

                if(fAge != -2.0f)
                {
                    float fParticlesNeeded = ((float)info.nEmission)*fDeltaTime + fEmissionResidue;
                    int nParticlesCreated = (int) fParticlesNeeded;
                    fEmissionResidue=fParticlesNeeded-((float)nParticlesCreated);

                    for(i=0; i<nParticlesCreated; i++)
                    {
                        if(nParticlesAlive>=MAX_PARTICLES) break;

                        par = particles[nParticlesAlive];

                        par.fAge = 0.0f;
                        par.fTerminalAge = RandomGenerator.Default.GetRandomSingle(info.fParticleLifeMin, info.fParticleLifeMax);

                        par.vecLocation = vecPrevLocation + (vecLocation - vecPrevLocation) * RandomGenerator.Default.GetRandomSingle(0.0f, 1.0f);
                        par.vecLocation.X += RandomGenerator.Default.GetRandomSingle(-2.0f, 2.0f);
                        par.vecLocation.Y += RandomGenerator.Default.GetRandomSingle(-2.0f, 2.0f);

                        ang = info.fDirection - ((float)Math.PI / 2.0f) + RandomGenerator.Default.GetRandomSingle(0.0f, info.fSpread) - info.fSpread / 2.0f;

                        if(info.bRelative) 
                            ang += (  (float) Math.Atan2( (vecPrevLocation-vecLocation).Y, (vecPrevLocation-vecLocation).X )    )+( (float)Math.PI / 2.0f );

                        par.vecVelocity.X = (float) Math.Cos(ang);
                        par.vecVelocity.Y = (float) Math.Sin(ang);
                        par.vecVelocity *= info.fSpeed;

                        par.fGravity = info.fGravity;
                        par.fRadialAccel = info.fRadialAccel;
                        par.fTangentialAccel = info.fTangentialAccel;

                        par.fSize = RandomGenerator.Default.GetRandomSingle(info.fSizeStart, info.fSizeStart + (info.fSizeEnd - info.fSizeStart) * info.fSizeVar);
                        par.fSizeDelta = (info.fSizeEnd-par.fSize) / par.fTerminalAge;

                        par.fSpin = RandomGenerator.Default.GetRandomSingle(info.fSpinStart, info.fSpinStart + (info.fSpinEnd - info.fSpinStart) * info.fSpinVar);
                        par.fSpinDelta = (info.fSpinEnd-par.fSpin) / par.fTerminalAge;


                        //Vector4 start = info.colColourStart.ToVector4();
                        //Vector4 finish = info.colColourEnd.ToVector4();

                        //Vector4 colColourV = new Vector4(
                        //    Euclid.RandomHelper.Random_Float(start.W, finish.W + (finish.W - start.W) * info.fColourEndVar),
                        //    Euclid.RandomHelper.Random_Float(start.X, start.X + (finish.X - start.X) * info.fColourStartVar),
                        //    Euclid.RandomHelper.Random_Float(start.Y, finish.Y + (finish.Y - start.Y) * info.fColourStartVar),
                        //    Euclid.RandomHelper.Random_Float(start.Z, finish.Z + (finish.Z - start.Z) * info.fColourStartVar)
                        //    );
                        //par.colColourStart = new Rgba32(colColourV);

                        //Vector4 colColourDeltaV = new Vector4();

                        //colColourDeltaV.W = (finish.W - start.W) / par.fTerminalAge;
                        //colColourDeltaV.X = (finish.X - start.X) / par.fTerminalAge;
                        //colColourDeltaV.Y = (finish.Y - start.Y) / par.fTerminalAge;
                        //colColourDeltaV.Z = (finish.Z - start.Z) / par.fTerminalAge;
                        //par.colColourEnd = new Rgba32(colColourDeltaV);

                        par.colColourStart = RandomGenerator.Default.GetRandomColourNearby(info.colColourStart, info.fColourStartVar);
                        par.colColourEnd = RandomGenerator.Default.GetRandomColourNearby(info.colColourEnd, info.fColourEndVar);

                        if (bUpdateBoundingBox) 
                            rectBoundingBox.Encapsulate(par.vecLocation.X, par.vecLocation.Y);

                        nParticlesAlive++;
                    }
                }

                vecPrevLocation=vecLocation;
            }

            public void MoveTo(float x, float y)
            {
                MoveTo(x, y, false);
            }

            public void MoveTo(float x, float y, bool bMoveParticles)
            {
                int i;
                float dx, dy;

                if (bMoveParticles)
                {
                    dx = x - vecLocation.X;
                    dy = y - vecLocation.Y;

                    for (i = 0; i < nParticlesAlive; i++)
                    {
                        particles[i].vecLocation.X += dx;
                        particles[i].vecLocation.Y += dy;
                    }

                    vecPrevLocation.X = vecPrevLocation.X + dx;
                    vecPrevLocation.Y = vecPrevLocation.Y + dy;
                }
                else
                {
                    if (fAge == -2.0) { vecPrevLocation.X = x; vecPrevLocation.Y = y; }
                    else { vecPrevLocation.X = vecLocation.X; vecPrevLocation.Y = vecLocation.Y; }
                }

                vecLocation.X = x;
                vecLocation.Y = y;
            }

            public void Transpose(float x, float y) 
            { 
                fTx=x; 
                fTy=y; 
            }

            public void SetScale(float scale) 
            { 
                fScale = scale; 
            }

            public int GetParticlesAlive() 
            { 
                return nParticlesAlive; 
            }

            public float GetAge() 
            { 
                return fAge; 
            }

            public void GetPosition(ref float x, ref float y) 
            { 
                x = vecLocation.X; 
                y = vecLocation.Y; 
            }

            public void GetTransposition(ref float x, ref float y) 
            { 
                x = fTx; 
                y = fTy; 
            }

            public float GetScale() 
            { 
                return fScale; 
            }
        }

        public sealed class Triple
        {
            public VertexPositionTextureColour[] v = new VertexPositionTextureColour[3];
            public Texture tex = null;
            public BlendMode blend = BlendMode.Default;

            public Triple()
            {
                v[0].Colour = v[1].Colour = v[2].Colour = Rgba32.White;
                v[0].Position.Z = 0.5f;
                v[1].Position.Z = 0.5f;
                v[2].Position.Z = 0.5f;
            }
        }

        public sealed class Quad
        {
            public VertexPositionTextureColour[] v;
            public Texture tex;
            public BlendMode blend = BlendMode.Default;

            public Quad()
            {
                v = new VertexPositionTextureColour[4];
                v[0].Position.Z = 0.5f;
                v[1].Position.Z = 0.5f;
                v[2].Position.Z = 0.5f;
                v[3].Position.Z = 0.5f;
            }

            public Quad(Quad from)
            {
                v = new VertexPositionTextureColour[4];

                for (int i = 0; i < 4; ++i)
                {
                    v[i] = from.v[i];
                }
                tex = from.tex;
                blend = from.blend;
            }
        }

        enum Type
        {
            PRIM_LINES = 2,
            PRIM_TRIPLES = 3,
            PRIM_QUADS = 4,
        }

        sealed class Batch
        {
            public Type? Type;
            public Texture Texture;
            public BlendMode? BlendMode;
            public VertexPositionTextureColour[] Buffer;
        }

        const int VERT_BUFFER_SIZE = 4096;
        readonly Shader shader;
        readonly Dictionary <String, RenderPassState> passState = new Dictionary<String, RenderPassState> ();
        readonly int[] quadIndices = new int[VERT_BUFFER_SIZE * 3 / 2];

        class RenderPassState
        {
            public readonly Queue<Batch> batchQueue = new Queue<Batch> ();

            public readonly VertexPositionTextureColour[] vertBuffer = new VertexPositionTextureColour[VERT_BUFFER_SIZE];

            public UInt32 nPrimsInBuffer = 0;

            public Type currentPrimitiveType;
            public BlendMode currentBlendMode = BlendMode.Default;
            public Texture currentTexture = null;
        }

        //
        // SETUP GRAPHICS
        // Sets up the transforms for the 2d render and setup the basic effect
        //
        public PrimitiveRenderer (Engine engine)
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
                            Name = "a_vertTexcoord",
                            NiceName = "TextureCoordinate",
                            Optional = false,
                            Usage = VertexElementUsage.TextureCoordinate,
                            DefaultValue = Vector2.Zero
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
                    SamplerDeclarations = new List<ShaderSamplerDeclaration> {
                        new ShaderSamplerDeclaration {
                            Name = "s_tex0",
                            NiceName = "TextureSampler",
                            Optional = true
                        }
                    }
                };

            Byte[] shaderUTF8 = null;

#if PLATFORM_MONOMAC
shaderUTF8 = System.Text.Encoding.UTF8.GetBytes(
@"Primitive Batch Shader
=VSH=
attribute vec4 a_vertPosition;
attribute vec2 a_vertTexcoord;
attribute vec4 a_vertColour;
uniform mat4 u_view;
uniform mat4 u_proj;
varying vec2 v_texCoord;
varying vec4 v_tint;
void main()
{
    gl_Position = u_proj * u_view * a_vertPosition;
    v_texCoord = a_vertTexcoord;
    v_tint = a_vertColour;
}
=FSH=
uniform sampler2D s_tex0;
varying vec2 v_texCoord;
varying vec4 v_tint;
void main()
{
    vec4 a = texture2D(s_tex0, v_texCoord);
    gl_FragColor = v_tint;
}
");
#elif PLATFORM_XIOS
shaderUTF8 = System.Text.Encoding.UTF8.GetBytes(
@"Primitive Batch Shader
=VSH=
attribute mediump vec4 a_vertPosition;
attribute mediump vec2 a_vertTexcoord;
attribute mediump vec4 a_vertColour;
uniform mediump mat4 u_view;
uniform mediump mat4 u_proj;
varying mediump vec2 v_texCoord;
varying mediump vec4 v_tint;
void main()
{
    gl_Position = u_proj * u_view * a_vertPosition;
    v_texCoord = a_vertTexcoord;
    v_tint = a_vertColour;
}
=FSH=
uniform mediump sampler2D s_tex0;
varying mediump vec2 v_texCoord;
varying mediump vec4 v_tint;
void main()
{
    vec4 a = texture2D(s_tex0, v_texCoord);
    gl_FragColor = v_tint;
}
");
#endif

            using (var mem = new System.IO.MemoryStream ())
            {
                using (var bin = new System.IO.BinaryWriter (mem))
                {
                    bin.Write ((Byte)1);
                    bin.Write (shaderUTF8.Length);
                    bin.Write (shaderUTF8);
                }
                shader = engine.Graphics.CreateShader (
                    shaderDecl,
                    ShaderFormat.GLSL,
                    mem.GetBuffer ());
            }

            // Set the index buffer for each vertex, using
            // clockwise winding
            quadIndices[0] = 1;
            quadIndices[1] = 3;
            quadIndices[2] = 0;
            quadIndices[3] = 2;
            quadIndices[4] = 3;
            quadIndices[5] = 1;

            for (int i = 0, vertex = 0; i < quadIndices.Length; i += 6, vertex += 4)
            {
                quadIndices[i] = vertex;
                quadIndices[i + 1] = vertex + 1;
                quadIndices[i + 2] = vertex + 2;
                quadIndices[i + 3] = vertex;
                quadIndices[i + 4] = vertex + 2;
                quadIndices[i + 5] = vertex + 3;
            }
        }

        internal void PostUpdate (AppTime appTime)
        {
            // make sure that everything gets removed.
            foreach (var pass in passState.Keys)
                if (passState[pass].nPrimsInBuffer > 0)
                _enqueue_batch (pass);
        }

        internal void Render (Graphics zGfx, String pass, Matrix44 zView, Matrix44 zProjection)
        {
            if (!passState.ContainsKey (pass))
                return;

            zGfx.GpuUtils.BeginEvent( Rgba32.Blue, "Blimey: Primitive Batch" );
            zGfx.ClearDepthBuffer (1f);
            zGfx.SetCullMode (CullMode.None);

            shader.Activate (VertexPositionTextureColour.Default.VertexDeclaration);
            shader.SetVariable ("View", zView);
            shader.SetVariable ("Projection", zProjection);

            _render_batch(zGfx, pass);

            zGfx.GpuUtils.EndEvent();
        }

        void _render_batch (Graphics gfx, String pass)
        {

            Texture texture = null;
            BlendMode? blendMode = null;

            while (passState [pass].batchQueue.Count > 0)
            {
                Batch batch = passState [pass].batchQueue.Dequeue ();

                if (!blendMode.HasValue || texture != batch.Texture)
                {
                    gfx.SetActiveTexture(batch.Texture, 0);
                    texture = batch.Texture;
                }

                if (!blendMode.HasValue || blendMode != batch.BlendMode)
                {
                    gfx.SetBlendEquation(batch.BlendMode.Value);
                    blendMode = batch.BlendMode;
                }

                Int32 n = batch.Buffer.Length;

                switch(batch.Type.Value)
                {
                    case Type.PRIM_QUADS:
                        gfx.DrawUserIndexedPrimitives<VertexPositionTextureColour>(
                            PrimitiveType.TriangleList, //primitiveType
                            batch.Buffer, //vertexData
                            0, //vertexOffset
                            n * 4, //numVertices
                            quadIndices, //indexData
                            0, //indexOffset
                            n * 4 / 2);//primitiveCount
                        break;

                    case Type.PRIM_TRIPLES:
                        gfx.DrawUserPrimitives<VertexPositionTextureColour>(
                            PrimitiveType.TriangleList,//primitiveType
                            batch.Buffer, //vertexData
                            0,//vertexOffset
                            n);//primitiveCount
                        break;

                    case Type.PRIM_LINES:
                        gfx.DrawUserPrimitives<VertexPositionTextureColour>(
                            PrimitiveType.LineList,//primitiveType
                            batch.Buffer, //vertexData
                            0,//vertexOffset
                            n);//primitiveCount
                        break;
                }
            }
        }

        void _enqueue_batch (String pass)
        {
            var b = new Batch ();
            b.Type = passState[pass].currentPrimitiveType;
            b.Texture = passState[pass].currentTexture;
            b.BlendMode = passState[pass].currentBlendMode;
            b.Buffer = new VertexPositionTextureColour[passState[pass].nPrimsInBuffer * (Int32) passState[pass].currentPrimitiveType];
            Array.Copy (passState[pass].vertBuffer, b.Buffer, b.Buffer.Length);


            passState[pass].batchQueue.Enqueue(b);

            passState[pass].nPrimsInBuffer = 0;
        }

        public void AddTriple (String pass, Triple zTriple)
        {
            if (!passState.ContainsKey (pass))
                passState [pass] = new RenderPassState ();

            if (passState[pass].currentPrimitiveType != Type.PRIM_TRIPLES
                || passState[pass].nPrimsInBuffer >= VERT_BUFFER_SIZE / (uint)Type.PRIM_TRIPLES
                || passState[pass].currentTexture != zTriple.tex
                || passState[pass].currentBlendMode != zTriple.blend)
            {
                if (passState[pass].nPrimsInBuffer > 0)
                    _enqueue_batch (pass);

                passState[pass].currentPrimitiveType = Type.PRIM_TRIPLES;
                passState[pass].currentBlendMode = zTriple.blend;
                passState[pass].currentTexture = zTriple.tex;
            }

            uint offset = passState[pass].nPrimsInBuffer * (uint)Type.PRIM_TRIPLES;

            for (uint i = 0; i < 3; ++i)
            {
                passState[pass].vertBuffer[i + offset].Position.X = zTriple.v[i].Position.X;
                passState[pass].vertBuffer[i + offset].Position.Y = zTriple.v[i].Position.Y;
                passState[pass].vertBuffer[i + offset].Position.Z = zTriple.v[i].Position.Z;
                passState[pass].vertBuffer[i + offset].Colour = zTriple.v[i].Colour;
                passState[pass].vertBuffer[i + offset].UV.X = zTriple.v[i].UV.X;
                passState[pass].vertBuffer[i + offset].UV.Y = zTriple.v[i].UV.Y;
            }
            passState[pass].nPrimsInBuffer++;
        }

        public void AddQuad (String pass, Quad zQuad)
        {
            if (!passState.ContainsKey (pass))
                passState [pass] = new RenderPassState ();

            if (passState[pass].currentPrimitiveType != Type.PRIM_QUADS
                || passState[pass].nPrimsInBuffer >= VERT_BUFFER_SIZE / (uint)Type.PRIM_QUADS
                || passState[pass].currentTexture != zQuad.tex
                || passState[pass].currentBlendMode != zQuad.blend)
            {
                if (passState[pass].nPrimsInBuffer > 0)
                    _enqueue_batch (pass);

                //Set up for new type
                passState[pass].currentPrimitiveType = Type.PRIM_QUADS;
                passState[pass].currentBlendMode = zQuad.blend;
                passState[pass].currentTexture = zQuad.tex;
            }

            uint offset = passState[pass].nPrimsInBuffer * (uint)Type.PRIM_QUADS;

            for (uint i = 0; i < 4; ++i)
            {
                passState[pass].vertBuffer[i + offset].Position.X = zQuad.v[i].Position.X;
                passState[pass].vertBuffer[i + offset].Position.Y = zQuad.v[i].Position.Y;
                passState[pass].vertBuffer[i + offset].Position.Z = zQuad.v[i].Position.Z;
                passState[pass].vertBuffer[i + offset].Colour = zQuad.v[i].Colour;
                passState[pass].vertBuffer[i + offset].UV.X = zQuad.v[i].UV.X;
                passState[pass].vertBuffer[i + offset].UV.Y = zQuad.v[i].UV.Y;
            }

            passState[pass].nPrimsInBuffer++;
        }

        public void AddLine (String pass, Vector3 a, Vector3 b, Rgba32 zColour)
        {
            if (!passState.ContainsKey (pass))
                passState [pass] = new RenderPassState ();

            // If the array does not hold lines, or it is full
            // or the texture has changed, or the blend mode
            if (passState[pass].currentPrimitiveType != Type.PRIM_LINES
                || passState[pass].nPrimsInBuffer >= VERT_BUFFER_SIZE / (uint)Type.PRIM_LINES
                || passState[pass].currentTexture != null
                || passState[pass].currentBlendMode != BlendMode.Default)
            {
                if (passState[pass].nPrimsInBuffer > 0)
                    _enqueue_batch (pass);

                passState[pass].currentPrimitiveType = Type.PRIM_LINES;
                passState[pass].currentBlendMode = BlendMode.Default;
                passState[pass].currentTexture = null;
            }

            uint i = passState[pass].nPrimsInBuffer * (uint)Type.PRIM_LINES;
            passState[pass].vertBuffer[i].Position = a; passState[pass].vertBuffer[i + 1].Position = b;
            passState[pass].vertBuffer[i].Colour = passState[pass].vertBuffer[i + 1].Colour = zColour;
            passState[pass].vertBuffer[i].UV.X = passState[pass].vertBuffer[i + 1].UV.X =
            passState[pass].vertBuffer[i].UV.Y = passState[pass].vertBuffer[i + 1].UV.Y = 0.0f;
             
            passState[pass].nPrimsInBuffer++;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    sealed class SceneRenderer
    {
        readonly Engine engine;
        readonly List<MeshRendererTrait> list = new List<MeshRendererTrait>();

        internal SceneRenderer(Engine cor)
        {
            this.engine = cor;
        }

        internal void Render(Scene scene)
        {
            // Clear the background colour if the scene settings want us to.
            if (scene.Configuration.BackgroundColour.HasValue)
            {
                this.engine.Graphics.ClearColourBuffer(scene.Configuration.BackgroundColour.Value);
            }

            foreach (var renderPass in scene.Configuration.RenderPasses)
            {
                this.RenderPass(scene, renderPass);
            }
        }

        List<MeshRendererTrait> GetMeshRenderersWithMaterials(Scene scene, string pass)
        {
            list.Clear ();
            foreach (var go in scene.SceneGraph.GetAllObjects())
            {
                if (!go.Enabled) continue;

                var mr = go.GetTrait<MeshRendererTrait>();

                if (mr == null)
                {
                    continue;
                }

                if (mr.Material == null)
                {
                    continue;
                }

                // if the material is for this pass
                if (mr.Material.RenderPass == pass)
                {
                    list.Add(mr);
                }
            }

            return list;
        }

        void RenderPass(Scene scene, RenderPass pass)
        {
            // #0: Apply this pass' clear settings.
            var gfxManager = this.engine.Graphics;

            if (pass.Configuration.ClearDepthBuffer)
            {
                gfxManager.ClearDepthBuffer();
            }

            var cam = scene.CameraManager.GetActiveCamera(pass.Name);


            // #1 Render everything in the scene graph that has a material on this pass.

            var meshRenderers = this.GetMeshRenderersWithMaterials(scene, pass.Name);

            // TODO: big one
            // we really need to group the mesh renderers by material
            // and only make a new draw call when there are changes.
            foreach (var mr in meshRenderers)
            {
                _renderMeshRenderer (gfxManager, pass.Name, cam.ViewMatrix44, cam.ProjectionMatrix44, mr);
            }

            // #2: Render all primitives that are associated with this pass.
            //scene.Blimey.PrimitiveBatch.Render(gfxManager, pass.Name, cam.ViewMatrix44, cam.ProjectionMatrix44);
            scene.Blimey.PrimitiveRenderer.Render(gfxManager, pass.Name, cam.ViewMatrix44, cam.ProjectionMatrix44);

            // #3: Render all debug primitives that are associated with this pass.
            scene.Blimey.DebugRenderer.Render(gfxManager, pass.Name, cam.ViewMatrix44, cam.ProjectionMatrix44);
        }

        static void _renderMeshRenderer (Graphics zGfx, string renderPass, Matrix44 zView, Matrix44 zProjection, MeshRendererTrait mr)
        {
            if (!mr.Active)
                return;

            if (mr.Material.RenderPass != renderPass )
                return;

            zGfx.GpuUtils.BeginEvent(Rgba32.Red, "MeshRenderer.Render");

            using (new ProfilingTimer(t => FrameStats.SetCullModeTime += t))
            {
                zGfx.SetCullMode(mr.CullMode);
            }

            using (new ProfilingTimer(t => FrameStats.ActivateVertexBufferTime += t))
            {
                // Set our vertex declaration, vertex buffer, and index buffer.
                zGfx.SetActiveVertexBuffer(mr.Mesh.VertexBuffer);
            }

            using (new ProfilingTimer(t => FrameStats.ActivateIndexBufferTime += t))
            {
                // Set our vertex declaration, vertex buffer, and index buffer.
                zGfx.SetActiveIndexBuffer(mr.Mesh.IndexBuffer);
            }

            using (new ProfilingTimer(t => FrameStats.MaterialTime += t))
            {
                mr.Material.UpdateGpuSettings (zGfx);

                // The lighing manager right now just grabs the shader and tries to set
                // all variables to do with lighting, without even knowing if the shader
                // supports lighting.
                mr.Material.SetColour( "AmbientLightColour", LightingManager.ambientLightColour );
                mr.Material.SetColour( "EmissiveColour", LightingManager.emissiveColour );
                mr.Material.SetColour( "SpecularColour", LightingManager.specularColour );
                mr.Material.SetFloat( "SpecularPower", LightingManager.specularPower );

                mr.Material.SetFloat( "FogEnabled", LightingManager.fogEnabled ? 1f : 0f );
                mr.Material.SetFloat( "FogStart", LightingManager.fogStart );
                mr.Material.SetFloat( "FogEnd", LightingManager.fogEnd );
                mr.Material.SetColour( "FogColour", LightingManager.fogColour );

                mr.Material.SetVector3( "DirectionalLight0Direction", LightingManager.dirLight0Direction );
                mr.Material.SetColour( "DirectionalLight0DiffuseColour", LightingManager.dirLight0DiffuseColour );
                mr.Material.SetColour( "DirectionalLight0SpecularColour", LightingManager.dirLight0SpecularColour );

                mr.Material.SetVector3( "DirectionalLight1Direction", LightingManager.dirLight1Direction );
                mr.Material.SetColour( "DirectionalLight1DiffuseColour", LightingManager.dirLight1DiffuseColour );
                mr.Material.SetColour( "DirectionalLight1SpecularColour", LightingManager.dirLight1SpecularColour );

                mr.Material.SetVector3( "DirectionalLight2Direction", LightingManager.dirLight2Direction );
                mr.Material.SetColour( "DirectionalLight2DiffuseColour", LightingManager.dirLight2DiffuseColour );
                mr.Material.SetColour( "DirectionalLight2SpecularColour", LightingManager.dirLight2SpecularColour );

                mr.Material.SetVector3( "EyePosition", zView.Translation );

                // Get the material's shader and apply all of the settings
                // it needs.
                mr.Material.UpdateShaderVariables (
                    mr.Parent.Transform.Location,
                    zView,
                    zProjection
                );
            }

            var shader = mr.Material.GetShader ();

            if( shader != null)
            {
                using (new ProfilingTimer(t => FrameStats.ActivateShaderTime += t))
                {
                    shader.Activate (mr.Mesh.VertexBuffer.VertexDeclaration);
                }

                using (new ProfilingTimer(t => FrameStats.DrawTime += t))
                {
                    FrameStats.DrawIndexedPrimitivesCount ++;
                    zGfx.DrawIndexedPrimitives (
                        PrimitiveType.TriangleList, 0, 0,
                        mr.Mesh.VertexCount, 0, mr.Mesh.TriangleCount);
                }
            }

            zGfx.GpuUtils.EndEvent();
        }
    }
}
