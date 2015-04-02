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

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.IO;

    using Abacus.SinglePrecision;
    using Fudge;
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides access to some GPU services.
    /// </summary>
    public sealed class Graphics
    {
        readonly IApi platform;

        public Graphics (IApi platform)
        {
            this.platform = platform;
        }

        /// <summary>
        /// Resets the graphics manager to it's default state.
        /// </summary>
        public void Reset ()
        {
            platform.gfx_SetBlendEquation (
                BlendFunction.Add, BlendFactor.SourceAlpha, BlendFactor.InverseSourceAlpha,
                BlendFunction.Add, BlendFactor.One, BlendFactor.InverseSourceAlpha);

            platform.gfx_SetCullMode (CullMode.CW);

            platform.gfx_ClearColourBuffer (Rgba32.Black);
            platform.gfx_ClearDepthBuffer (1f);
            platform.gfx_SetCullMode (CullMode.CW);
            platform.gfx_vbff_Activate (null);
            platform.gfx_ibff_Activate (null);

            currentActiveVertexBuffer = null;
            currentActiveIndexBuffer = null;

            // todo, here we need to set all the texture slots to point to null
            foreach (var slot in currentTextureMap.Keys)
                platform.gfx_tex_Activate (null, slot);

            currentTextureMap.Clear ();

            platform.gfx_vbff_Bind (null, null);
            currentShaderBinding = null;
        }

        public ShaderFormat GetRuntimeShaderFormat ()
        {
            return platform.gfx_GetRuntimeShaderFormat ();
        }


        VertexBuffer currentActiveVertexBuffer = null;
        IndexBuffer currentActiveIndexBuffer = null;
        Texture currentActiveTexture = null;
        Shader currentActiveShader = null;

        //internal event ActiveVertexBufferChanged (Handle h);
        //internal event ActiveIndexBufferChanged;
        //internal event ActiveTextureChanged;
        //internal event ActiveShaderChanged;

        CullMode? currentCullMode = null;
        readonly Dictionary<Int32, Texture> currentTextureMap = new Dictionary<Int32, Texture> ();
        Tuple<Shader, VertexBuffer> currentShaderBinding = null;

        /// <summary>
        /// Sets the active vertex buffer.
        /// </summary>
        public void SetActive (VertexBuffer vertexBuffer)
        {
            if (currentActiveVertexBuffer == vertexBuffer)
                return;

            platform.gfx_vbff_Activate (vertexBuffer != null ? vertexBuffer.Handle : null);
            currentActiveVertexBuffer = vertexBuffer;
        }

        /// <summary>
        /// Sets the active index buffer.
        /// </summary>
        public void SetActive (IndexBuffer indexBuffer)
        {
            if (currentActiveIndexBuffer == indexBuffer)
                return;

            platform.gfx_ibff_Activate (indexBuffer != null ? indexBuffer.Handle : null);
            currentActiveIndexBuffer = indexBuffer;
        }

        /// <summary>
        /// Sets the active index buffer.
        /// </summary>
        public void SetActive (Texture texture, Int32 slot)
        {
            if (currentTextureMap.ContainsKey (slot) && currentTextureMap [slot] == texture)
                return;

            platform.gfx_tex_Activate (texture != null ? texture.Handle : null, slot);
            currentTextureMap [slot] = texture;
        }

        /// <summary>
        /// Bind the desired elements of the active vertex buffer.
        /// </summary>
        public void SetActive (Shader shader)
        {
            if (currentActiveVertexBuffer == null)
                throw new Exception ();

            // todo: optimise this: see teapot dissapearing example
            //if (currentShaderBinding != null
            //    && currentShaderBinding.Item1 == shader
            //    && currentShaderBinding.Item2 == currentVertexBuffer) return;

            shader.Activate (currentActiveVertexBuffer.VertexDeclaration);
            platform.gfx_vbff_Bind (
                currentActiveVertexBuffer.VertexDeclaration,
                shader.GetElementsIndicesToEnable (currentActiveVertexBuffer.VertexDeclaration));

            currentShaderBinding = new Tuple<Shader, VertexBuffer> (shader, currentActiveVertexBuffer);
        }

        public void SetActive (Shader shader, VertexDeclaration vertexDeclaration)
        {
            if (currentActiveVertexBuffer != null || currentActiveIndexBuffer != null)
                throw new Exception ();


            //if (currentShaderBinding != null
            //    && currentShaderBinding.Item1 == shader
            //    && currentShaderBinding.Item2 == currentVertexBuffer) return;

            shader.Activate (vertexDeclaration);
            var indices = shader.GetElementsIndicesToEnable (vertexDeclaration);
            platform.gfx_vbff_Bind (vertexDeclaration, indices);

            currentShaderBinding = new Tuple<Shader, VertexBuffer> (shader, currentActiveVertexBuffer);
        }


        /// <summary>
        /// Clears the colour buffer to the specified colour.
        /// </summary>
        public void ClearColourBuffer (Rgba32 colour = new Rgba32())
        {
            platform.gfx_ClearColourBuffer (colour);
        }

        /// <summary>
        /// Clears the depth buffer to the specified depth.
        /// </summary>
        public void ClearDepthBuffer (Single depth = 1f)
        {
            platform.gfx_ClearDepthBuffer (depth);
        }

        /// <summary>
        /// Sets the GPU's current culling mode to the value specified.
        /// </summary>
        public void SetCullMode (CullMode cullMode)
        {
            if (currentCullMode.HasValue && currentCullMode.Value == cullMode)
                return;

            platform.gfx_SetCullMode (cullMode);
        }

        public VertexBuffer CreateVertexBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount)
        {
            return new VertexBuffer (this.platform, vertexDeclaration, vertexCount);
        }

        public void DestroyVertexBuffer (VertexBuffer vb)
        {
            vb.Dispose ();
        }

        public IndexBuffer CreateIndexBuffer (Int32 indexCount)
        {
            return new IndexBuffer (this.platform, indexCount);
        }

        public void DestroyIndexBuffer (IndexBuffer ib)
        {
            ib.Dispose ();
        }

        public Texture CreateTexture (TextureFormat textureFormat, Int32 width, Int32 height, Byte[] source)
        {
            return new Texture (this.platform, textureFormat, width, height, source);
        }

        public void DestroyTexture (Texture tex)
        {
            tex.Dispose ();
        }

        /// <summary>
        /// Creates a new shader program on the GPU.
        /// </summary>
        public Shader CreateShader (ShaderDeclaration shaderDeclaration, ShaderFormat shaderFormat, Byte[] source)
        {
            return new Shader (this.platform, shaderDeclaration, shaderFormat, source);
        }

        public void DestroyShader (Shader shader)
        {
            shader.Dispose ();
        }

        /// <summary>
        /// Defines how we blend colours
        /// </summary>
        public void SetBlendEquation (
            BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb,
            BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha)
        {
            platform.gfx_SetBlendEquation (
                rgbBlendFunction, sourceRgb, destinationRgb,
                alphaBlendFunction, sourceAlpha, destinationAlpha);
        }

        /// <summary>
        /// Renders a sequence of non-indexed geometric primitives of the specified type from the active geometry
        /// buffer (which sits in GRAM).
        ///
        /// Info: From GRAM - Non-Indexed.
        ///
        /// Arguments:
        ///   primitiveType  -> Describes the type of primitive to render.
        ///   startVertex    -> Index of the first vertex to load. Beginning at startVertex, the correct number of
        ///                     vertices is read out of the vertex buffer.
        ///   primitiveCount -> Number of primitives to render. The primitiveCount is the number of primitives as
        ///                     determined by the primitive type. If it is a line list, each primitive has two vertices.
        ///                     If it is a triangle list, each primitive has three vertices.
        /// </summary>
        public void DrawPrimitives (
            PrimitiveType primitiveType, Int32 startVertex, Int32 primitiveCount)
        {
            platform.gfx_DrawPrimitives (primitiveType, startVertex, primitiveCount);
        }

        /// <summary>
        /// Renders a sequence of indexed geometric primitives of the specified type from the active geometry buffer
        /// (which sits in GRAM).
        ///
        /// Info: From GRAM - Indexed.
        ///
        /// Arguments:
        ///   primitiveType  -> Describes the type of primitive to render.  PrimitiveType.PointList is not supported
        ///                     with this method.
        ///   baseVertex     -> Offset to add to each vertex index in the index buffer.
        ///   minVertexIndex -> Minimum vertex index for vertices used during the call. The minVertexIndex parameter
        ///                     and all of the indices in the index stream are relative to the baseVertex parameter.
        ///   numVertices    -> Number of vertices used during the call. The first vertex is located at index:
        ///                     baseVertex + minVertexIndex.
        ///   startIndex     -> Location in the index array at which to start reading vertices.
        ///   primitiveCount -> Number of primitives to render. The number of vertices used is a function of
        ///                     primitiveCount and primitiveType.
        /// </summary>
        public void DrawIndexedPrimitives (
            PrimitiveType primitiveType, Int32 baseVertex, Int32 minVertexIndex,
            Int32 numVertices, Int32 startIndex, Int32 primitiveCount)
        {
            platform.gfx_DrawIndexedPrimitives (
                primitiveType,
                baseVertex,
                minVertexIndex,
                numVertices,
                startIndex,
                primitiveCount);
        }

        /// <summary>
        /// Draws un-indexed vertex data uploaded straight from RAM.
        ///
        /// Info: From System RAM - Non-Indexed
        ///
        /// Arguments:
        /// primitiveType     -> Describes the type of primitive to render.
        /// vertexData        -> The vertex data.
        /// vertexOffset      -> Offset (in vertices) from the beginning of the buffer to start reading data.
        /// primitiveCount    -> Number of primitives to render.
        /// vertexDeclaration -> The vertex declaration, which defines per-vertex data.
        /// </summary>
        public void DrawUserPrimitives <T> (
            PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset, Int32 primitiveCount)
        where T
            : struct
            , IVertexType
        {
            platform.gfx_DrawUserPrimitives <T> (
                primitiveType,
                vertexData,
                vertexOffset,
                primitiveCount);
        }

        /// <summary>
        /// Draws indexed vertex data uploaded straight from RAM.
        ///
        /// Info: From System RAM - Indexed
        ///
        /// Arguments:
        /// primitiveType     -> Describes the type of primitive to render.
        /// vertexData        -> The vertex data.
        /// vertexOffset      -> Offset (in vertices) from the beginning of the vertex buffer to the first vertex to
        ///                      draw.
        /// numVertices       -> Number of vertices to draw.
        /// indexData         -> The index data.
        /// indexOffset       -> Offset (in indices) from the beginning of the index buffer to the first index to use.
        /// primitiveCount    -> Number of primitives to render.
        /// vertexDeclaration -> The vertex declaration, which defines per-vertex data.
        /// </summary>
        public void DrawUserIndexedPrimitives <T> (
            PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData,
            Int32 indexOffset, Int32 primitiveCount)
        where T
            : struct
            , IVertexType
        {
            platform.gfx_DrawUserIndexedPrimitives <T> (
                primitiveType,
                vertexData,
                vertexOffset,
                numVertices,
                indexData,
                indexOffset,
                primitiveCount);
        }
    }
}