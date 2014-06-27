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
    
    using Fudge;
    using Abacus.SinglePrecision;
    
    using System.Linq;
    using Cor;

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
    public class DebugShapeRenderer
    {
        public IShader DebugShader
        {
            get { return debugShader; }
            set { debugShader = value; Init (); }
        }

        IShader debugShader;

        public int NumActiveShapes { get { return activeShapes.Count; } }

        // A single shape in our debug renderer
        class DebugShape
        {
            public string RenderPass = "Default";

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
        readonly List<DebugShape> cachedShapes = new List<DebugShape>();
        readonly List<DebugShape> activeShapes = new List<DebugShape>();

        // Allocate an array to hold our vertices; this will grow as needed by our renderer
        VertexPositionColour[] verts = new VertexPositionColour[64];

        // An array we use to get corners from frustums and bounding boxes
        Vector3[] corners = new Vector3[8];

        // This holds the vertices for our unit sphere that we will use when drawing bounding spheres
        const int sphereResolution = 30;
        const int sphereLineCount = (sphereResolution + 1) * 3;
        Vector3[] unitSphere;

        readonly Dictionary<string, Material> materials = new Dictionary<string, Material>();

        readonly EngineBase cor;

		readonly List<RenderPass> renderPasses;

		public DebugShapeRenderer(EngineBase cor, List<RenderPass> renderPasses)
        {
            this.cor = cor;
            this.renderPasses = renderPasses;

            // Create our unit sphere vertices
            InitializeSphere();
        }

        void Init ()
        {
            foreach (var pass in renderPasses)
            {
				var m = new Material(pass.Name, debugShader);

                m.BlendMode = BlendMode.Default;
				materials[pass.Name] = m;
                m.SetColour("MaterialColour", Rgba32.White);

            }
        }

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
            DebugShape shape = GetShapeForLines(1, life);
            shape.RenderPass = renderPass;

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
            DebugShape shape = GetShapeForLines(3, life);
            shape.RenderPass = renderPass;

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
            DebugShape shape = GetShapeForLines(12, life);
            shape.RenderPass = renderPass;

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

        internal void Update(AppTime time)
        {
            // Go through our active shapes and retire any shapes that have expired to the
            // cache list.
            Boolean resort = false;
            for (int i = this.activeShapes.Count - 1; i >= 0; i--)
            {
                DebugShape s = activeShapes[i];

                if (s.Lifetime < 0)
                {
                    this.cachedShapes.Add(s);
                    this.activeShapes.RemoveAt(i);
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
                this.cachedShapes.Sort(CachedShapesSort);
        }


        internal void Render(GraphicsBase zGfx, string pass, Matrix44 zView, Matrix44 zProjection)
        {
            if (!materials.ContainsKey(pass))
                return;

            var material = materials[pass];

            if( material == null )
                return;

            material.UpdateGpuSettings (zGfx);

            // Update our effect with the matrices.
            material.UpdateShaderVariables (
                Matrix44.Identity,
                zView,
                zProjection
                );

            var shader = material.GetShader ();

            if( shader == null )
                return;

            zGfx.GpuUtils.BeginEvent(Rgba32.Red, "DebugRenderer.Render");

            var shapesForThisPass = this.activeShapes.Where(e => pass == e.RenderPass);

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
                foreach (IShaderPass effectPass in shader.Passes)
                {
                    effectPass.Activate (VertexPositionColour.Default.VertexDeclaration);

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
                            linesToDraw,
                            VertexPositionColour.Default.VertexDeclaration
                            );

                        // Move our vertex offset ahead based on the lines we drew
                        vertexOffset += linesToDraw * 2;

                        // Remove these lines from our total line count
                        lineCount -= linesToDraw;
                    }
                }

                zGfx.GpuUtils.EndEvent();
            }
        }

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

        static int CachedShapesSort(DebugShape s1, DebugShape s2)
        {
            return s1.Vertices.Length.CompareTo(s2.Vertices.Length);
        }

        DebugShape GetShapeForLines(int lineCount, float life)
        {
            DebugShape shape = null;

            // We go through our cached list trying to find a shape that contains
            // a large enough array to hold our desired line count. If we find such
            // a shape, we move it from our cached list to our active list and break
            // out of the loop.
            int vertCount = lineCount * 2;
            for (int i = 0; i < cachedShapes.Count; i++)
            {
                if (cachedShapes[i].Vertices.Length >= vertCount)
                {
                    shape = cachedShapes[i];
                    cachedShapes.RemoveAt(i);
                    activeShapes.Add(shape);
                    break;
                }
            }

            // If we didn't find a shape in our cache, we create a new shape and add it
            // to the active list.
            if (shape == null)
            {
                shape = new DebugShape { Vertices = new VertexPositionColour[vertCount] };
                activeShapes.Add(shape);
            }

            // Set the line count and lifetime of the shape based on our parameters.
            shape.LineCount = lineCount;
            shape.Lifetime = life;

            return shape;
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class GridRenderer
    {
        public Rgba32 AxisColourX = Rgba32.Red; // going to the right of the screen
        public Rgba32 AxisColourY = Rgba32.Green; // up
        public Rgba32 AxisColourZ = Rgba32.Blue; // coming out of the screen

        float gridSquareSize = 0.50f;  // in meters
        int numberOfGridSquares = 10;

        Rgba32 gridColour = Rgba32.LightGrey;

        public bool ShowXZPlane = true;
        public bool ShowXYPlane = false;
        public bool ShowYZPlane = false;

        string renderPass;
        DebugShapeRenderer debugRenderer;

        public GridRenderer(DebugShapeRenderer debugRenderer, string renderPass, float gridSquareSize = 0.50f, int numberOfGridSquares = 10)
        {
            this.debugRenderer = debugRenderer;
            this.renderPass = renderPass;
            this.gridSquareSize = gridSquareSize;
            this.numberOfGridSquares = numberOfGridSquares;
        }

        public void Update()
        {
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

    public class PrimitiveBatch
    {
        public class PrimitiveBatchTriple
        {
            public VertexPositionTextureColour[] v = new VertexPositionTextureColour[3];
            public ITexture tex = null;
            public BlendMode blend = BlendMode.Default;

            public PrimitiveBatchTriple()
            {
                v[0].Colour = v[1].Colour = v[2].Colour = Rgba32.White;
                v[0].Position.Z = 0.5f;
                v[1].Position.Z = 0.5f;
                v[2].Position.Z = 0.5f;
            }
        }

        public class PrimitiveBatchQuad
        {
            public VertexPositionTextureColour[] v;
            public ITexture tex;
            public BlendMode blend = BlendMode.Default;

            public PrimitiveBatchQuad()
            {
                v = new VertexPositionTextureColour[4];
                v[0].Position.Z = 0.5f;
                v[1].Position.Z = 0.5f;
                v[2].Position.Z = 0.5f;
                v[3].Position.Z = 0.5f;
            }

            public PrimitiveBatchQuad(PrimitiveBatchQuad from)
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

        public enum PrimitiveBatchType
        {
            PRIM_INVALID = 0,
            PRIM_LINES = 2,
            PRIM_TRIPLES = 3,
            PRIM_QUADS = 4,
        }

        protected IShader effectToUse;

        ITexture curTexture = null;

        const int VERT_BUFFER_SIZE = 4000;
        VertexPositionTextureColour[] vertBuffer = new VertexPositionTextureColour[VERT_BUFFER_SIZE];

        int[] quadIndices = new int[VERT_BUFFER_SIZE * 3 / 2];

        uint nPrimsInBuffer = 0;

        bool hasBegun = false;

        PrimitiveBatchType CurPrimType;
        BlendMode CurBlendMode = BlendMode.Default;

        //
        //SETUP GRAPHICS
        // Sets up the transforms for the 2d render and setup the basic effect
        //
        public PrimitiveBatch (GraphicsBase zGfxDevice, Assets zContentManager)
        {
            // todo load shader here
            
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


        //
        // RENDER TRI
        // Renders a quad.
        //
        public void RenderTriple(GraphicsBase gfx, PrimitiveBatchTriple zTriple)
        {
            if (hasBegun)
            {
                if (CurPrimType != PrimitiveBatchType.PRIM_TRIPLES ||
                    nPrimsInBuffer >= VERT_BUFFER_SIZE / (uint)PrimitiveBatchType.PRIM_TRIPLES ||
                    curTexture != zTriple.tex
                    || CurBlendMode != zTriple.blend
                )
                {
                    _render_batch(gfx, false);

                    CurPrimType = PrimitiveBatchType.PRIM_TRIPLES;
                    if (zTriple.tex != curTexture)
                    {
                        curTexture = zTriple.tex;
                    }
                }

                uint offset = nPrimsInBuffer * (uint)PrimitiveBatchType.PRIM_TRIPLES;

                for (uint i = 0; i < 3; ++i)
                {
                    vertBuffer[i + offset].Position.X = zTriple.v[i].Position.X;
                    vertBuffer[i + offset].Position.Y = zTriple.v[i].Position.Y;
                    vertBuffer[i + offset].Position.Z = zTriple.v[i].Position.Z;
                    vertBuffer[i + offset].Colour = zTriple.v[i].Colour;
                    vertBuffer[i + offset].UV.X = zTriple.v[i].UV.X;
                    vertBuffer[i + offset].UV.Y = zTriple.v[i].UV.Y;
                }
                nPrimsInBuffer++;
            }
            else
            {
                throw new InvalidOperationException
                ("Begin must be called.");
            }
        }


        //
        // RENDER QUAD
        // Renders a quad.
        //
        public void RenderQuad(GraphicsBase gfx, PrimitiveBatchQuad zQuad)
        {
            if (hasBegun)
            {
                if (CurPrimType != PrimitiveBatchType.PRIM_QUADS ||
                    nPrimsInBuffer >= VERT_BUFFER_SIZE / (uint)PrimitiveBatchType.PRIM_QUADS ||
                    curTexture != zQuad.tex ||
                    CurBlendMode != zQuad.blend)
                {
                    _render_batch(gfx ,false);

                    //Set up for new type
                    CurPrimType = PrimitiveBatchType.PRIM_QUADS;
                    if (zQuad.tex != curTexture)
                    {

                        curTexture = zQuad.tex;

                    }
                }

                uint offset = nPrimsInBuffer * (uint)PrimitiveBatchType.PRIM_QUADS;

                for (uint i = 0; i < 4; ++i)
                {
                    vertBuffer[i + offset].Position.X = zQuad.v[i].Position.X;
                    vertBuffer[i + offset].Position.Y = zQuad.v[i].Position.Y;
                    vertBuffer[i + offset].Position.Z = zQuad.v[i].Position.Z;
                    vertBuffer[i + offset].Colour = zQuad.v[i].Colour;
                    vertBuffer[i + offset].UV.X = zQuad.v[i].UV.X;
                    vertBuffer[i + offset].UV.Y = zQuad.v[i].UV.Y;
                }

                nPrimsInBuffer++;
            }
            else
            {
                throw new InvalidOperationException ("Begin must be called.");
            }
        }


        public bool HasBegun
        {
            get { return hasBegun; }
        }

        void _render_batch(GraphicsBase gfx, bool bEndScene)
        {
            //todo activate effect

            if(nPrimsInBuffer > 0)
            {
                foreach (var pass in effectToUse.Passes)
                {
                    pass.Activate (VertexPositionTextureColour.Default.VertexDeclaration);

                    switch(CurPrimType)
                    {
                    case PrimitiveBatchType.PRIM_QUADS:
                        gfx.DrawUserIndexedPrimitives<VertexPositionTextureColour>(
                            PrimitiveType.TriangleList, //primitiveType
                            vertBuffer, //vertexData
                            0, //vertexOffset
                            (int)nPrimsInBuffer * 4, //numVertices
                            quadIndices, //indexData
                            0, //indexOffset
                            (int)nPrimsInBuffer * 4 / 2,
                            VertexPositionTextureColour.Default.VertexDeclaration);//primitiveCount
                        break;

                    case PrimitiveBatchType.PRIM_TRIPLES:
                        gfx.DrawUserPrimitives<VertexPositionTextureColour>(
                            PrimitiveType.TriangleList,//primitiveType
                            vertBuffer, //vertexData
                            0,//vertexOffset
                            (int)nPrimsInBuffer,
                            VertexPositionTextureColour.Default.VertexDeclaration);//primitiveCount
                        break;

                    case PrimitiveBatchType.PRIM_LINES:
                        gfx.DrawUserPrimitives<VertexPositionTextureColour>(
                            PrimitiveType.LineList,//primitiveType
                            vertBuffer, //vertexData
                            0,//vertexOffset
                            (int)nPrimsInBuffer,
                            VertexPositionTextureColour.Default.VertexDeclaration);//primitiveCount
                        break;
                    }
                }

                nPrimsInBuffer=0;

                hasBegun &= !bEndScene;
            }
        }


        public void BeginScene( GraphicsBase gfx, Matrix44 zView, Matrix44 zProj)
        {
            gfx.GpuUtils.BeginEvent( Rgba32.Blue, "Blimey: Primitive Batch" );
            hasBegun = true;

            //todo: set world view proj on shader
			gfx.SetBlendEquation(BlendMode.Default);
        }

        //
        // END SCENE
        // Ends rendering and updates the screen.
        //
        public void EndScene(GraphicsBase gfx)
        {
            _render_batch(gfx, true);
            gfx.GpuUtils.EndEvent();
        }


        public void RenderLine(GraphicsBase gfx, Vector3 a, Vector3 b, Rgba32 zColour)
        {
            if (hasBegun)
            {
                // If the array does not hold lines, or it is full
                // or the texture has changed, or the blend mode
                if (CurPrimType != PrimitiveBatchType.PRIM_LINES ||
                    nPrimsInBuffer >= VERT_BUFFER_SIZE / (uint)PrimitiveBatchType.PRIM_LINES
                    || curTexture != null
                    || CurBlendMode != BlendMode.Default)
                {

                    _render_batch(gfx, false);

                    CurPrimType = PrimitiveBatchType.PRIM_LINES;
                    if (CurBlendMode != BlendMode.Default)
						gfx.SetBlendEquation(BlendMode.Default);
                    curTexture = null;
                }

                uint i = nPrimsInBuffer * (uint)PrimitiveBatchType.PRIM_LINES;
                vertBuffer[i].Position = a; vertBuffer[i + 1].Position = b;
                vertBuffer[i].Colour = vertBuffer[i + 1].Colour = zColour;
                vertBuffer[i].UV.X = vertBuffer[i + 1].UV.X =
                vertBuffer[i].UV.Y = vertBuffer[i + 1].UV.Y = 0.0f;

                nPrimsInBuffer++;
            }
            else
            {
                throw new InvalidOperationException ("Begin must be called.");
            }
        }
    }
}
