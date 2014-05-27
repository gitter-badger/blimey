// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor.Lib.Khronos                                                                                                │ \\
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

namespace Cor.Lib.Khronos
{
    using System;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Runtime.ConstrainedExecution;

    using Abacus;
    using Abacus.Packed;
    using Abacus.SinglePrecision;

    using Oats;

    #if COR_PLATFORM_XIOS || COR_PLATFORM_MONOMAC

    using System.Drawing;

    #endif

    #if COR_PLATFORM_XIOS

    using OpenTK.Graphics.ES20;
    using GLShaderType = OpenTK.Graphics.ES20.ShaderType;
    using GLBufferUsage = OpenTK.Graphics.ES20.BufferUsage;
    using ActiveUniformType = OpenTK.Graphics.ES20.ActiveUniformType;
    using KhronosVector2 = OpenTK.Vector2;
    using KhronosVector3 = OpenTK.Vector3;
    using KhronosVector4 = OpenTK.Vector4;
    using KhronosMatrix4 = OpenTK.Matrix4;

    #elif COR_PLATFORM_MONOMAC

    using MonoMac.OpenGL;
    using GLShaderType = MonoMac.OpenGL.ShaderType;
    using GLBufferUsage = MonoMac.OpenGL.BufferUsageHint;
    using ActiveUniformType = MonoMac.OpenGL.ActiveUniformType;
    using KhronosVector2 = MonoMac.OpenGL.Vector2;
    using KhronosVector3 = MonoMac.OpenGL.Vector3;
    using KhronosVector4 = MonoMac.OpenGL.Vector4;
    using KhronosMatrix4 = MonoMac.OpenGL.Matrix4;

    #endif

    using Boolean = System.Boolean;



    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class KrShaderDefinition
    {
        public List<KrShaderVariantDefinition> VariantDefinitions { get; set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class KrShaderSourceDefinition
    {
        public string VertexShaderPath { get; set; }
        public string PixelShaderPath { get; set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class KrShaderVariantDefinition
    {
        public String VariantName { get; set; }
        public List<KrShaderVariantPassDefinition> VariantPassDefinitions { get; set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class KrShaderVariantPassDefinition
    {
        public String PassName { get; set; }
        public KrShaderSourceDefinition PassDefinition { get; set; }
    }


#if COR_PLATFORM_XIOS || COR_PLATFORM_MONOMAC


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class GeometryBuffer
        : IGeometryBuffer
    {
        IndexBuffer _iBuf;
        VertexBuffer _vBuf;
        
        public GeometryBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount)
        {

            if (vertexCount == 0)
            {
                throw new Exception ("A geometry buffer must have verts");
            }

            if (indexCount != 0 )
            {
                _iBuf = new IndexBuffer (indexCount);
            }

            _vBuf = new VertexBuffer (vertexDeclaration, vertexCount);

        }

        internal void Activate ()
        {
            _vBuf.Activate ();

            if (_iBuf != null)
                _iBuf.Activate ();
        }

        internal void Deactivate ()
        {
            _vBuf.Deactivate ();

            if (_iBuf != null)
                _iBuf.Deactivate ();
        }

        public IVertexBuffer VertexBuffer { get { return _vBuf; } }
        public IIndexBuffer IndexBuffer { get { return _iBuf; } }

        internal VertexBuffer OpenTKVertexBuffer { get { return _vBuf; } }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class GpuUtils
        : IGpuUtils
    {
        public GpuUtils ()
        {
        }

        #region IGpuUtils

        public Int32 BeginEvent (Rgba32 colour, String eventName)
        {
            return 0;
        }

        public Int32 EndEvent ()
        {
            return 0;
        }

        public void SetMarker (Rgba32 colour, String eventName)
        {

        }

        public void SetRegion (Rgba32 colour, String eventName)
        {

        }

        #endregion
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class GraphicsManager
        : IGraphicsManager
    {
        readonly GpuUtils gpuUtils;

        GeometryBuffer currentGeomBuffer;
        CullMode? currentCullMode;

        public GraphicsManager ()
        {
            InternalUtils.Log.Info ("GFX", 
                "Khronos Graphics Manager -> ()");

            this.gpuUtils = new GpuUtils ();

            GL.Enable (EnableCap.Blend);
            KrErrorHandler.Check ();

            this.SetBlendEquation (
                BlendFunction.Add, BlendFactor.SourceAlpha, BlendFactor.InverseSourceAlpha,
                BlendFunction.Add, BlendFactor.One, BlendFactor.InverseSourceAlpha);

            GL.Enable (EnableCap.DepthTest);
            KrErrorHandler.Check ();

            GL.DepthMask (true);
            KrErrorHandler.Check ();

            GL.DepthRange (0f, 1f);
            KrErrorHandler.Check ();

            GL.DepthFunc (DepthFunction.Lequal);
            KrErrorHandler.Check ();

            SetCullMode (CullMode.CW);
        }

        [ReliabilityContract (Consistency.MayCorruptInstance, Cer.MayFail)]
        static IntPtr Add (IntPtr pointer, int offset)
        {
            unsafe
            {
                return (IntPtr) (unchecked (((byte *) pointer) + offset));
            }
        }

        [ReliabilityContract (Consistency.MayCorruptInstance, Cer.MayFail)]
        static IntPtr Subtract (IntPtr pointer, int offset)
        {
            unsafe
            {
                return (IntPtr) (unchecked (((byte *) pointer) - offset));
            }
        }

        void EnableVertAttribs (VertexDeclaration vertDecl, IntPtr pointer)
        {
            var vertElems = vertDecl.GetVertexElements ();

            IntPtr ptr = pointer;

            int counter = 0;
            foreach (var elem in vertElems)
            {
                GL.EnableVertexAttribArray (counter);
                KrErrorHandler.Check ();

                //var vertElemUsage = elem.VertexElementUsage;
                var vertElemFormat = elem.VertexElementFormat;
                var vertElemOffset = elem.Offset;

                Int32 numComponentsInVertElem = 0;
                Boolean vertElemNormalized = false;
                VertexAttribPointerType glVertElemFormat;

                KrEnumConverter.ToKhronos (vertElemFormat, out glVertElemFormat, out vertElemNormalized, out numComponentsInVertElem);

                if (counter != 0)
                {
                    ptr = Add (ptr, vertElemOffset);
                }

                GL.VertexAttribPointer (
                    counter,                // index - specifies the generic vertex attribute index.  This value is 0 to
                                            //         max vertex attributes supported - 1.
                    numComponentsInVertElem,// size - number of components specified in the vertex array for the
                                            //        vertex attribute referenced by index.  Valid values are 1 - 4.
                    glVertElemFormat,       // type - Data format, valid values are GL_BYTE, GL_UNSIGNED_BYTE, GL_SHORT, GL_UNSIGNED_SHORT,
                                            //        GL_FLOAT, GL_FIXED, GL_HALF_FLOAT_OES*(Optional feature of es2)
                    vertElemNormalized,     // normalised - used to indicate whether the non-floating data format type should be normalised
                                            //              or not when converted to floating point.
                    vertDecl.VertexStride,  // stride - the components of vertex attribute specified by size are stored sequentially for each
											//          vertex.  stride specifies the delta between data for vertex index i and vertex (i + 1).
                                            //          If stride is 0, attribute data for all vertices are stored sequentially.
                                            //          If stride is > 0, then we use the stride valude tas the pitch to get vertex data
                                            //          for the next index.
                    ptr

                    );

                KrErrorHandler.Check ();

                counter++;

            }
        }

        void DisableVertAttribs (VertexDeclaration vertDecl)
        {
            var vertElems = vertDecl.GetVertexElements ();

            for (int i = 0; i < vertElems.Length; ++i)
            {
                GL.DisableVertexAttribArray (i);
                KrErrorHandler.Check ();
            }
        }


        #region IGraphicsManager

        public IGpuUtils GpuUtils { get { return this.gpuUtils; } }

        public void Reset ()
        {
            this.ClearDepthBuffer ();
            this.ClearColourBuffer ();
            this.SetActiveGeometryBuffer (null);

            // todo, here we need to set all the texture slots to point to null
            this.SetActiveTexture (0, null);
        }

        public void ClearColourBuffer (Rgba32 col = new Rgba32())
        {
            Abacus.SinglePrecision.Vector4 c;

            col.UnpackTo (out c);

            GL.ClearColor (c.X, c.Y, c.Z, c.W);

            var mask = ClearBufferMask.ColorBufferBit;

            GL.Clear (mask);

            KrErrorHandler.Check ();
        }

        public void ClearDepthBuffer (Single val = 1)
        {
            GL.ClearDepth (val);

            var mask = ClearBufferMask.DepthBufferBit;

            GL.Clear (mask);

            KrErrorHandler.Check ();
        }

        public void SetCullMode (CullMode cullMode)
        {
            if (!currentCullMode.HasValue || currentCullMode.Value != cullMode)
            {
                if (cullMode == CullMode.None)
                {
                    GL.Disable (EnableCap.CullFace);
                    KrErrorHandler.Check ();

                }
                else
                {
                    GL.Enable (EnableCap.CullFace);
                    KrErrorHandler.Check ();

                    GL.FrontFace (FrontFaceDirection.Cw);
                    KrErrorHandler.Check ();

                    if (cullMode == CullMode.CW)
                    {
                        GL.CullFace (CullFaceMode.Back);
                        KrErrorHandler.Check ();
                    }
                    else if (cullMode == CullMode.CCW)
                    {
                        GL.CullFace (CullFaceMode.Front);
                        KrErrorHandler.Check ();
                    }
                    else
                    {
                        throw new NotSupportedException ();
                    }
                }

                currentCullMode = cullMode;
            }
        }

        public IGeometryBuffer CreateGeometryBuffer (
            VertexDeclaration vertexDeclaration,
            Int32 vertexCount,
            Int32 indexCount)
        {
            return new GeometryBuffer (vertexDeclaration, vertexCount, indexCount);
        }

        public void SetActiveGeometryBuffer (IGeometryBuffer buffer)
        {
            var temp = buffer as GeometryBuffer;

            if (temp != this.currentGeomBuffer)
            {
                if (this.currentGeomBuffer != null)
                {
                    this.currentGeomBuffer.Deactivate ();

                    this.currentGeomBuffer = null;
                }

                if (temp != null)
                {
                    temp.Activate ();
                }

                this.currentGeomBuffer = temp;
            }
        }

        public ITexture UploadTexture (TextureAsset tex)
        {
            int width = tex.Width;
            int height = tex.Height;

            if (tex.SurfaceFormat != SurfaceFormat.Rgba32)
                throw new NotImplementedException ();

            IntPtr pixelDataRgba32 = Marshal.AllocHGlobal (tex.Data.Length);
            Marshal.Copy (tex.Data, 0, pixelDataRgba32, tex.Data.Length);

            // Call unmanaged code
            Marshal.FreeHGlobal (pixelDataRgba32);

            int textureId = -1;

            // this sets the unpack alignment.  which is used when reading pixels
            // in the fragment shader.  when the textue data is uploaded via glTexImage2d,
            // the rows of pixels are assumed to be aligned to the value set for GL_UNPACK_ALIGNMENT.
            // By default, the value is 4, meaning that rows of pixels are assumed to begin
            // on 4-byte boundaries.  this is a global STATE.
            GL.PixelStore (
                PixelStoreParameter.UnpackAlignment, 4);

            KrErrorHandler.Check ();

            // the first sept in the application of texture is to create the
            // texture object.  this is a container object that holds the
            // texture data.  this function returns a handle to a texture
            // object.
            GL.GenTextures (1, out textureId);
            KrErrorHandler.Check ();

            var textureHandle = new TextureHandle (textureId);

            var textureTarget = TextureTarget.Texture2D;

            // we need to bind the texture object so that we can opperate on it.
            GL.BindTexture (textureTarget, textureId);
            KrErrorHandler.Check ();

            // the incoming texture format
            // (the format that [pixelDataRgba32] is in)
            var format = PixelFormat.Rgba;

            var internalFormat = PixelInternalFormat.Rgba;

            var textureDataFormat = PixelType.UnsignedByte;

            // now use the bound texture object to load the image data.
            GL.TexImage2D (

                // specifies the texture target, either GL_TEXTURE_2D or one of the cubemap face targets.
                textureTarget,

                // specifies which mip level to load.  the base level is
                // specified by 0 following by an increasing level for each
                // successive mipmap.
                0,

                // internal format for the texture storage, can be:
                // - GL_RGBA
                // - GL_RGB
                // - GL_LUMINANCE_ALPHA
                // - GL_LUMINANCE
                // - GL_ALPHA
                internalFormat,

                // the width of the image in pixels
                width,

                // the height of the image in pixels
                height,

                // boarder - set to zero, only here for compatibility with OpenGL desktop
                0,

                // the format of the incoming texture data, in opengl es this
                // has to be the same as the internal format
                format,

                // the type of the incoming pixel data, can be:
                // - unsigned byte
                // - unsigned short 4444
                // - unsigned short 5551
                // - unsigned short 565
                textureDataFormat, // this refers to each individual channel


                pixelDataRgba32

                );

            KrErrorHandler.Check ();

            // sets the minification and maginfication filtering modes.  required
            // because we have not loaded a complete mipmap chain for the texture
            // so we must select a non mipmapped minification filter.
            GL.TexParameter (textureTarget, TextureParameterName.TextureMinFilter, (int) All.Nearest);

            KrErrorHandler.Check ();

            GL.TexParameter (textureTarget, TextureParameterName.TextureMagFilter, (int) All.Nearest);

            KrErrorHandler.Check ();

            return textureHandle;
        }

        public void UnloadTexture (ITexture texture)
        {
            int textureId = (texture as TextureHandle).glTextureId;

            GL.DeleteTextures (1, ref textureId);
        }

        public void SetActiveTexture (Int32 slot, ITexture tex)
        {
            TextureUnit oglTexSlot = KrEnumConverter.ToKhronosTextureSlot (slot);
            GL.ActiveTexture (oglTexSlot);

            var oglt0 = tex as TextureHandle;

            if (oglt0 != null)
            {
                var textureTarget = TextureTarget.Texture2D;

                // we need to bind the texture object so that we can opperate on it.
                GL.BindTexture (textureTarget, oglt0.glTextureId);
                KrErrorHandler.Check ();
            }
        }

        public IShader CreateShader (ShaderAsset asset)
        {
            ShaderDefinition shaderDefinition = asset.Definition;

            Byte[] data = asset.Data;
            
            List<KrShaderVariantDefinition> platformVariants = new List<KrShaderVariantDefinition> ();

            using (var memoryStream = new MemoryStream (data)) {

                using (var sc = new SerialisationChannel 
                    <BinaryStreamSerialiser> (
                        memoryStream, 
                        ChannelMode.Read))
                {
                    // : Num Variants
                    Int32 numPlatformVariants = sc.Read <Int32> ();


                    for (Int32 i = 0; i < numPlatformVariants; ++i) 
                    {
                        var variant = new KrShaderVariantDefinition ();

                        // : Variant Name
                        variant.VariantName = sc.Read<String> ();

                        // : Num Variant Pass Definitions
                        Int32 numPassDefs = sc.Read <Int32> ();

                        variant.VariantPassDefinitions = new List<KrShaderVariantPassDefinition> ();

                        for (Int32 j = 0; j < numPassDefs; ++j) 
                        {
                            var pass = new KrShaderVariantPassDefinition ();

                            // : Pass Name
                            pass.PassName = sc.Read<String> ();

                            pass.PassDefinition = new KrShaderSourceDefinition ();

                            // : Vertex Shader Source
                            pass.PassDefinition.VertexShaderPath = sc.Read<String> ();

                            // : Pixel Shader Source
                            pass.PassDefinition.PixelShaderPath = sc.Read<String> ();

                            variant.VariantPassDefinitions.Add (pass);
                        }

                        platformVariants.Add (variant);
                    }
                }
            }

            var result = new ShaderHandle (
                shaderDefinition,
                platformVariants);

            return result;
        }

        public void DestroyShader (IShader shader)
        {
            var handle = (ShaderHandle) shader;
            handle.Dispose ();
        }

        public void SetBlendEquation (
            BlendFunction rgbBlendFunction,
            BlendFactor sourceRgb,
            BlendFactor destinationRgb,
            BlendFunction alphaBlendFunction,
            BlendFactor sourceAlpha,
            BlendFactor destinationAlpha
            )
        {
            GL.BlendEquationSeparate (
                KrEnumConverter.ToKhronos (rgbBlendFunction),
                KrEnumConverter.ToKhronos (alphaBlendFunction) );
            KrErrorHandler.Check ();

            GL.BlendFuncSeparate (
                KrEnumConverter.ToKhronosSrc (sourceRgb),
                KrEnumConverter.ToKhronosDest (destinationRgb),
                KrEnumConverter.ToKhronosSrc (sourceAlpha),
                KrEnumConverter.ToKhronosDest (destinationAlpha) );
            KrErrorHandler.Check ();
        }

        public void DrawPrimitives (
            PrimitiveType primitiveType,
            Int32 startVertex,
            Int32 primitiveCount)
        {
            throw new NotImplementedException ();
        }

        public void DrawIndexedPrimitives (
            PrimitiveType primitiveType,
            Int32 baseVertex,
            Int32 minVertexIndex,
            Int32 numVertices,
            Int32 startIndex,
            Int32 primitiveCount
            )
        {
            if (baseVertex != 0 || minVertexIndex != 0 || startIndex != 0 )
            {
                throw new NotImplementedException ();
            }

            var otkpType =  KrEnumConverter.ToKhronos (primitiveType);
            //Int32 numVertsInPrim = numVertices / primitiveCount;

            Int32 nVertsInPrim = PrimitiveHelper.NumVertsIn (primitiveType);
            Int32 count = primitiveCount * nVertsInPrim;

            var vertDecl = currentGeomBuffer.VertexBuffer.VertexDeclaration;

            this.EnableVertAttribs (vertDecl, (IntPtr) 0 );

            GL.DrawElements (
                otkpType,
                count,
                DrawElementsType.UnsignedShort,
                (System.IntPtr) 0 );

            KrErrorHandler.Check ();

            this.DisableVertAttribs (vertDecl);
        }

        public void DrawUserPrimitives <T> (
            PrimitiveType primitiveType,
            T[] vertexData,
            Int32 vertexOffset,
            Int32 primitiveCount,
            VertexDeclaration vertexDeclaration)
            where T : struct, IVertexType
        {
            // do i need to do this? todo: find out
            this.SetActiveGeometryBuffer (null);

            var vertDecl = vertexData[0].VertexDeclaration;

            //MSDN
            //
            //The GCHandle structure is used with the GCHandleType
            //enumeration to create a handle corresponding to any managed
            //object. This handle can be one of four types: Weak,
            //WeakTrackResurrection, Normal, or Pinned. When the handle has
            //been allocated, you can use it to prevent the managed object
            //from being collected by the garbage collector when an unmanaged
            //client holds the only reference. Without such a handle,
            //the object can be collected by the garbage collector before
            //completing its work on behalf of the unmanaged client.
            //
            //You can also use GCHandle to create a pinned object that
            //returns a memory address to prevent the garbage collector
            //from moving the object in memory.
            //
            //When the handle goes out of scope you must explicitly release
            //it by calling the Free method; otherwise, memory leaks may
            //occur. When you free a pinned handle, the associated object
            //will be unpinned and will become eligible for garbage
            //collection, if there are no other references to it.
            //
            GCHandle pinnedArray = GCHandle.Alloc (vertexData, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject ();

            if (vertexOffset != 0 )
            {
                pointer = Add (pointer, vertexOffset * vertDecl.VertexStride * sizeof (byte));
            }

            var glDrawMode = KrEnumConverter.ToKhronos (primitiveType);
            var glDrawModeAll = glDrawMode;

            var bindTarget = BufferTarget.ArrayBuffer;

            GL.BindBuffer (bindTarget, 0);
            KrErrorHandler.Check ();


            this.EnableVertAttribs (vertDecl, pointer);

            Int32 nVertsInPrim = PrimitiveHelper.NumVertsIn (primitiveType);
            Int32 count = primitiveCount * nVertsInPrim;

            GL.DrawArrays (
                glDrawModeAll, // specifies the primitive to render
                vertexOffset,  // specifies the starting vertex index in the enabled vertex arrays
                count); // specifies the number of indicies to be drawn

            KrErrorHandler.Check ();


            this.DisableVertAttribs (vertDecl);


            pinnedArray.Free ();
        }

        public void DrawUserIndexedPrimitives <T> (
            PrimitiveType primitiveType,
            T[] vertexData,
            Int32 vertexOffset,
            Int32 numVertices,
            Int32[] indexData,
            Int32 indexOffset,
            Int32 primitiveCount,
            VertexDeclaration vertexDeclaration)
            where T : struct, IVertexType
        {
            throw new NotImplementedException ();
        }

        #endregion
    }

    public sealed class IndexBuffer
        : IIndexBuffer
        , IDisposable
    {
        static Int32 resourceCounter;

        Int32 indexCount;
        BufferTarget type;
        UInt32 bufferHandle;
        GLBufferUsage bufferUsage;

        bool alreadyDisposed;

        public IndexBuffer (Int32 indexCount)
        {
            this.indexCount = indexCount;

            this.type = BufferTarget.ElementArrayBuffer;

            this.bufferUsage = GLBufferUsage.DynamicDraw;

            GL.GenBuffers (1, out this.bufferHandle);

            KrErrorHandler.Check ();

            if (this.bufferHandle == 0 )
            {
                throw new Exception ("Failed to generate vert buffer.");
            }

            this.Activate ();

            GL.BufferData (
                this.type,
                (System.IntPtr) (sizeof (UInt16) * this.indexCount),
                (System.IntPtr) null,
                this.bufferUsage);

            KrErrorHandler.Check ();

            resourceCounter++;

        }

        ~IndexBuffer ()
        {
            RunDispose (false);
        }

        void CleanUpManagedResources ()
        {

        }

        void CleanUpNativeResources ()
        {
            GL.DeleteBuffers (1, ref this.bufferHandle);
            KrErrorHandler.Check ();

            bufferHandle = 0;

            resourceCounter--;
        }

        public void Dispose ()
        {
            RunDispose (true);
            GC.SuppressFinalize (this);
        }

        public void RunDispose (bool isDisposing)
        {
            if (alreadyDisposed)
                return;

            if (isDisposing)
            {
                CleanUpNativeResources ();

            }

            // FREE UNMANAGED STUFF HERE
            CleanUpManagedResources ();

            alreadyDisposed = true;

        }

        void GetBufferData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount)
            where T : struct
        {
            throw new NotImplementedException ();/*
            GL.BindBuffer (BufferTarget.ArrayBuffer, ibo);
            GraphicsExtensions.CheckGLError ();
            var elementSizeInByte = Marshal.SizeOf (typeof (T));
            IntPtr ptr = GL.MapBuffer (BufferTarget.ArrayBuffer, BufferAccess.ReadOnly);
            // Pointer to the start of data to read in the index buffer
            ptr = new IntPtr (ptr.ToInt64() + offsetInBytes);
            if (data is byte[])
            {
                byte[] buffer = data as byte[];
                // If data is already a byte[] we can skip the temporary buffer
                // Copy from the index buffer to the destination array
                Marshal.Copy (ptr, buffer, 0, buffer.Length);
            }
            else
            {
                // Temporary buffer to store the copied section of data
                byte[] buffer = new byte[elementCount * elementSizeInByte];
                // Copy from the index buffer to the temporary buffer
                Marshal.Copy (ptr, buffer, 0, buffer.Length);
                // Copy from the temporary buffer to the destination array
                Buffer.BlockCopy (buffer, 0, data, startIndex * elementSizeInByte, elementCount * elementSizeInByte);
            }
            GL.UnmapBuffer (BufferTarget.ArrayBuffer);
            GraphicsExtensions.CheckGLError ();
            */
        }

        internal void Activate ()
        {
            GL.BindBuffer (this.type, this.bufferHandle);
            KrErrorHandler.Check ();
        }

        internal void Deactivate ()
        {
            GL.BindBuffer (this.type, 0);
            KrErrorHandler.Check ();
        }

        public int IndexCount
        {
            get
            {
                return indexCount;
            }
        }


        public void SetData (Int32[] data)
        {

            if (data.Length != indexCount)
            {
                throw new Exception ("?");
            }

            UInt16[] udata = new UInt16[data.Length];

            for (Int32 i = 0; i < data.Length; ++i)
            {
                udata[i] = (UInt16) data[i];
            }

            this.Activate ();

            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            GL.BufferSubData (
                this.type,
                (System.IntPtr) 0,
                (System.IntPtr) (sizeof (UInt16) * this.indexCount),
                udata);

            udata = null;

            KrErrorHandler.Check ();
        }

        public void GetData (Int32[] data)
        {
            throw new NotImplementedException ();
        }

        public void SetData (Int32[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException ();
        }

        public void GetData (Int32[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException ();
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents a handle to a Khronos GL shader on the GPU.
    /// </summary>
    public sealed class ShaderHandle
        : IShader
        , IDisposable
    {
        #region IShader

        /// <summary>
        /// Resets all the shader's variables to their default values.
        /// </summary>
        public void ResetVariables ()
        {
            // the shader definition defines the default values for the variables
            foreach (var variableDefinition in cachedShaderDefinition.VariableDefinitions)
            {
                string varName = variableDefinition.Name;
                object value = variableDefinition.DefaultValue;

                if (variableDefinition.Type == typeof (Matrix44) )
                {
                    this.SetVariable (varName, (Matrix44) value);
                }
                else if (variableDefinition.Type == typeof (Int32) )
                {
                    this.SetVariable (varName, (Int32) value);
                }
                else if (variableDefinition.Type == typeof (Single) )
                {
                    this.SetVariable (varName, (Single) value);
                }
                else if (variableDefinition.Type == typeof (Abacus.SinglePrecision.Vector2) )
                {
                    this.SetVariable (varName, (Abacus.SinglePrecision.Vector2) value);
                }
                else if (variableDefinition.Type == typeof (Abacus.SinglePrecision.Vector3) )
                {
                    this.SetVariable (varName, (Abacus.SinglePrecision.Vector3) value);
                }
                else if (variableDefinition.Type == typeof (Abacus.SinglePrecision.Vector4) )
                {
                    this.SetVariable (varName, (Abacus.SinglePrecision.Vector4) value);
                }
                else if (variableDefinition.Type == typeof (Rgba32) )
                {
                    this.SetVariable (varName, (Rgba32) value);
                }
                else
                {
                    throw new NotSupportedException ();
                }
            }
        }

        /// <summary>
        /// Resets all the shader's texture samplers point at texture slot 0.
        /// </summary>
        public void ResetSamplerTargets ()
        {
            foreach (var samplerDefinition in cachedShaderDefinition.SamplerDefinitions)
            {
                this.SetSamplerTarget (samplerDefinition.Name, 0);
            }
        }

        /// <summary>
        /// Sets the value of a specified shader variable.
        /// </summary>
        public void SetVariable<T>(string name, T value)
        {
            passes.ForEach (x => x.SetVariable (name, value));
        }

        /// <summary>
        /// Sets the texture slot that a texture sampler should sample from.
        /// </summary>
        public void SetSamplerTarget (string name, Int32 textureSlot)
        {
            foreach (var pass in passes)
            {
                pass.SetSamplerTarget (name, textureSlot);
            }
        }

        /// <summary>
        /// Provides access to the individual passes in this shader.
        /// the calling code can itterate though these and apply them
        ///to the graphics context before it makes a draw call.
        /// </summary>
        public IShaderPass[] Passes
        {
            get
            {
                return passes.ToArray ();
            }
        }

        /// <summary>
        /// Defines which vertex elements are required by this shader.
        /// </summary>
        public VertexElementUsage[] RequiredVertexElements
        {
            get
            {
                // todo: an array of vert elem usage
                // doesn't uniquely identify anything...
                return requiredVertexElements.ToArray ();
            }
        }

        /// <summary>
        /// Defines which vertex elements are optionally used by this
        /// shader if they happen to be present.
        /// </summary>
        public VertexElementUsage[] OptionalVertexElements
        {
            get
            {
                // todo: an array of vert elem usage
                // doesn't uniquely identify anything...
                return optionalVertexElements.ToArray ();
            }
        }

        public String Name { get; private set; }

        #endregion

        #region IDisposable

        /// <summary>
        /// Releases all resource used by the <see cref="Cor.MonoTouchRuntime.Shader"/> object.
        /// </summary>
        public void Dispose ()
        {
            foreach (var pass in passes)
            {
                pass.Dispose ();
            }
        }

        #endregion

        List<VertexElementUsage> requiredVertexElements = new List<VertexElementUsage>();
        List<VertexElementUsage> optionalVertexElements = new List<VertexElementUsage>();

        /// <summary>
        /// The <see cref="ShaderPass"/> objects that need to each, in turn,
        /// be individually activated and used to draw with to apply the effect
        /// of this containing <see cref="Shader"/> object.
        /// </summary>
        List<ShaderPassHandle> passes = new List<ShaderPassHandle>();

        /// <summary>
        /// Cached reference to the platform agnostic 
        /// <see cref="ShaderDefinition"/> object used
        /// to create this <see cref="Shader"/> object.
        /// </summary>
        readonly ShaderDefinition cachedShaderDefinition;

        public ShaderDefinition ShaderDefinition
        {
            get { return cachedShaderDefinition; }
        }

        /// <summary>
        /// Defines the variants.  Done for optimisation, instead of having one
        /// massive shader that supports all the the Inputs and attempts to
        /// process them accordingly, we load slight variants of effectively 
        /// the same shader, then we select the most optimal variant to run
        /// based upon the VertexDeclaration the calling code is about to draw.
        /// </summary>
        readonly List<KrShaderVariantDefinition> cachedVariantDefinitions;

        public List<KrShaderVariantDefinition> VariantDefinitions
        { 
            get { return cachedVariantDefinitions; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shader"/> class from a
        /// <see cref="ShaderDefinition"/> object.
        /// </summary>
        internal ShaderHandle (ShaderDefinition shaderDefinition, List<KrShaderVariantDefinition> platformVariants)
        {
            InternalUtils.Log.Info ("GFX", "\n");
            InternalUtils.Log.Info ("GFX", "\n");
            InternalUtils.Log.Info ("GFX", "=====================================================================");
            InternalUtils.Log.Info ("GFX", "Creating Shader: " + shaderDefinition.Name);
            this.cachedShaderDefinition = shaderDefinition;
            this.cachedVariantDefinitions = platformVariants;
            this.Name = shaderDefinition.Name;
            CalculateRequiredInputs (shaderDefinition);
            InitilisePasses (shaderDefinition);

            this.ResetVariables ();
        }

        /// <summary>
        /// Works out and caches a copy of which shader
        /// inputs are required/optional, needed as the
        /// <see cref="IShader"/> interface requires this information.
        /// </summary>
        void CalculateRequiredInputs (ShaderDefinition shaderDefinition)
        {
            foreach (var input in shaderDefinition.InputDefinitions)
            {
                if (input.Optional)
                {
                    optionalVertexElements.Add (input.Usage);
                }
                else
                {
                    requiredVertexElements.Add (input.Usage);
                }
            }
        }

        /// <summary>
        /// Triggers the creation of all of
        /// this <see cref="Shader"/> object's passes.
        /// </summary>
        void InitilisePasses (ShaderDefinition shaderDefinition)
        {
            // This function builds up an in memory object for each shader
            // pass in this shader.  The different shader varients are defined
            // outside of the scope of a conceptual shader pass, therefore this
            // function must traverse the shader definition and to create
            // shader pass objects that only contain the varient data
            // for that specific pass.

            // For each named shader pass.
            foreach (var definedPassName in shaderDefinition.PassNames)
            {
                InternalUtils.Log.Info ("GFX", 
                    " Preparing to initilising Shader Pass: " + definedPassName);

                // itterate over the defined pass names, ex: cel, outline...

                //shaderDefinition.VariantDefinitions
                //  .Select (x => x.PassDefinitions
                //  .Select (y => y.PassName == definedPassName))
                //  .ToList ();

                // Find all of the variants that are defined
                // in this shader object's definition
                // that support the current shaderpass.
                var passVariants___Name_AND_passVariantDefinition =
                    new List<Tuple<String, KrShaderVariantPassDefinition>>();

                // itterate over every shader variant in the definition
                foreach (var shaderVariantDefinition in this.VariantDefinitions)
                {
                    // each shader varient has a name
                    String shaderVariantName = shaderVariantDefinition.VariantName;

                    // find the pass in the shader variant definition
                    // that corresponds to the pass we are
                    // currently trying to initilise.
                    var variantPassDefinition =
                        shaderVariantDefinition.VariantPassDefinitions
                            .Find (x => x.PassName == definedPassName);

                    // now we have a Variant name, say:
                    //   - Unlit_PositionTextureColour
                    // and a pass definition, say :
                    //   - Main
                    //   - Shaders/Unlit_PositionTextureColour.vsh
                    //   - Shaders/Unlit_PositionTextureColour.fsh
                    //
                    passVariants___Name_AND_passVariantDefinition.Add (
                        new Tuple<String, KrShaderVariantPassDefinition>(
                            shaderVariantName, variantPassDefinition));
                }

                // Create one shader pass for each defined pass name.
                var shaderPass = new ShaderPassHandle (
                    definedPassName,
                    passVariants___Name_AND_passVariantDefinition);

                shaderPass.BindAttributes (
                    shaderDefinition
                        .InputDefinitions.Select (x => x.Name)
                        .ToList ());

                shaderPass.Link ();
                shaderPass.ValidateInputs (shaderDefinition.InputDefinitions);
                shaderPass.ValidateVariables (shaderDefinition.VariableDefinitions);
                shaderPass.ValidateSamplers (shaderDefinition.SamplerDefinitions);

                passes.Add (shaderPass);
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents in individual pass of a Cor.Xios high level Shader object.
    /// </summary>
    public sealed class ShaderPassHandle
        : IShaderPass
        , IDisposable
    {
        /// <summary>
        /// A collection of OpenGL shaders, all with slight variations in their
        /// input parameters, that are suitable for rendering this ShaderPass object.
        /// </summary>
        List<KrShader> Variants { get; set; }

        /// <summary>
        /// A nice name for the shader pass, for example: Main or Cel -> Outline.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Whenever this ShaderPass object gets asked to activate itself whilst a VertexDeclaration it has not seen
        /// before is active, the best matching shader pass variant is found and then stored in this map to fast
        /// access.
        /// </summary>
        Dictionary<VertexDeclaration, KrShader> BestVariantMap { get; set; }

        Dictionary<String, Object>  currentVariables = new Dictionary<String, Object>();
        Dictionary<String, Int32>   currentSamplerSlots = new Dictionary<String, Int32>();

        Dictionary<String, bool> logHistory = new Dictionary<String, bool>();

        internal void SetVariable<T>(string name, T value)
        {
            currentVariables[name] = value;
        }

        internal void SetSamplerTarget (string name, Int32 textureSlot)
        {
            currentSamplerSlots[name] = textureSlot;
        }

        public ShaderPassHandle (
            String passName,
            List<Tuple<String, KrShaderVariantPassDefinition>> passVariants___Name_AND_passVariantDefinition)
        {
            InternalUtils.Log.Info ("GFX", "Creating ShaderPass: " + passName);
            this.Name = passName;
            this.Variants =
                passVariants___Name_AND_passVariantDefinition
                    .Select (x => new KrShader (
                        x.Item1, passName, 
                        x.Item2.PassDefinition.VertexShaderPath,
                        x.Item2.PassDefinition.PixelShaderPath))
                    .ToList ();

            this.BestVariantMap = new Dictionary<VertexDeclaration, KrShader>();
        }


        internal void BindAttributes (IList<String> inputNames)
        {
            foreach (var variant in this.Variants)
            {
                variant.BindAttributes (inputNames);
            }
        }

        internal void Link ()
        {
            foreach (var variant in this.Variants)
            {
                variant.Link ();
            }
        }

        internal void ValidateInputs (List<ShaderInputDefinition> definitions)
        {
            foreach (var variant in this.Variants)
            {
                variant.ValidateInputs (definitions);
            }
        }

        internal void ValidateVariables (List<ShaderVariableDefinition> definitions)
        {
            foreach (var variant in this.Variants)
            {
                variant.ValidateVariables (definitions);
            }
        }

        internal void ValidateSamplers (List<ShaderSamplerDefinition> definitions)
        {
            foreach (var variant in this.Variants)
            {
                variant.ValidateSamplers (definitions);
            }
        }

        public void Activate (VertexDeclaration vertexDeclaration)
        {
            if (!BestVariantMap.ContainsKey (vertexDeclaration))
            {
                BestVariantMap[vertexDeclaration] = KrShaderHelper.WorkOutBestVariantFor (vertexDeclaration, Variants);
            }
            var bestVariant = BestVariantMap[vertexDeclaration];
            // select the correct shader pass variant and then activate it
            bestVariant.Activate ();

            foreach (var key1 in currentVariables.Keys)
            {
                var variable = bestVariant
                    .Variables
                    .Find (x => x.NiceName == key1 || x.Name == key1);

                if (variable == null)
                {
                    string warning = "WARNING: missing variable: " + key1;

                    if ( !logHistory.ContainsKey (warning) )
                    {
                        InternalUtils.Log.Info ("GFX", warning);

                        logHistory.Add (warning, true);
                    }
                }
                else
                {
                    var val = currentVariables[key1];

                    variable.Set (val);
                }
            }

            foreach (var key2 in currentSamplerSlots.Keys)
            {
                var sampler = bestVariant
                    .Samplers
                    .Find (x => x.NiceName == key2 || x.Name == key2);

                if (sampler == null)
                {
                    //InternalUtils.Log.Info ("GFX", "missing sampler: " + key2);
                }
                else
                {
                    var slot = currentSamplerSlots[key2];

                    sampler.SetSlot (slot);
                }
            }
        }

        public void Dispose ()
        {
            foreach (var oglesShader in Variants)
            {
                oglesShader.Dispose ();
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal sealed class TextureHandle
        : ITexture
        , IDisposable
    {
        internal TextureHandle (int textureid)
        {
            glTextureId = textureid;
        }

        internal int glTextureId { get; private set; }

        public void Dispose ()
        {

        }

        public SurfaceFormat SurfaceFormat
        {
            get
            {
                throw new NotImplementedException ();
            }
        }

        public Byte[] Primary
        {
            get
            {
                throw new NotImplementedException ();
            }
        }

		public Byte[][] Mipmaps
        {
            get
            {
                throw new NotImplementedException ();
            }
        }

        public int Width
        {
            get
            {
                throw new NotImplementedException ();
            }
        }

        public int Height
        {
            get
            {
                throw new NotImplementedException ();
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class VertexBuffer
        : IVertexBuffer
        , IDisposable
    {
        static Int32 resourceCounter;

        readonly VertexDeclaration vertDecl;

        readonly Int32 vertexCount;

        UInt32 bufferHandle;

        BufferTarget type;
        GLBufferUsage bufferUsage;

        bool alreadyDisposed;

        public VertexBuffer (VertexDeclaration vd, Int32 vertexCount)
        {
            this.vertDecl = vd;
            this.vertexCount = vertexCount;

            this.type = BufferTarget.ArrayBuffer;

            this.bufferUsage = GLBufferUsage.DynamicDraw;

            GL.GenBuffers (1, out this.bufferHandle);
            KrErrorHandler.Check ();


            if (this.bufferHandle == 0 )
            {
                throw new Exception ("Failed to generate vert buffer.");
            }


            this.Activate ();

            GL.BufferData (
                this.type,
                (System.IntPtr) (vertDecl.VertexStride * this.vertexCount),
                (System.IntPtr) null,
                this.bufferUsage);

            KrErrorHandler.Check ();

            resourceCounter++;

        }

        internal void Activate ()
        {
            GL.BindBuffer (this.type, this.bufferHandle);
            KrErrorHandler.Check ();
        }

        internal void Deactivate ()
        {
            GL.BindBuffer (this.type, 0);
            KrErrorHandler.Check ();
        }

        ~VertexBuffer ()
        {
            RunDispose (false);
        }

        void CleanUpManagedResources ()
        {

        }

        void CleanUpNativeResources ()
        {
            GL.DeleteBuffers (1, ref this.bufferHandle);
            KrErrorHandler.Check ();

            bufferHandle = 0;

            resourceCounter--;
        }

        public void Dispose ()
        {
            RunDispose (true);
            GC.SuppressFinalize (this);
        }

        public void RunDispose (bool isDisposing)
        {
            if (alreadyDisposed)
                return;

            if (isDisposing)
            {
                CleanUpNativeResources ();

            }

            // FREE UNMANAGED STUFF HERE
            CleanUpManagedResources ();

            alreadyDisposed = true;

        }

        public Int32 VertexCount
        {
            get
            {
                return this.vertexCount;
            }
        }

        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return this.vertDecl;
            }
        }

        void GetBufferData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride) where T : struct
        {
            throw new NotImplementedException ();/*
            GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
            GraphicsExtensions.CheckGLError ();
            var elementSizeInByte = Marshal.SizeOf (typeof (T));
            IntPtr ptr = GL.MapBuffer (BufferTarget.ArrayBuffer, BufferAccess.ReadOnly);
            GraphicsExtensions.CheckGLError ();

            // Pointer to the start of data to read in the index buffer
            ptr = new IntPtr (ptr.ToInt64 () + offsetInBytes);
            if (data is byte[])
            {
                byte[] buffer = data as byte[];
                // If data is already a byte[] we can skip the temporary buffer
                // Copy from the vertex buffer to the destination array
                Marshal.Copy (ptr, buffer, 0, buffer.Length);
            }
            else
            {
                // Temporary buffer to store the copied section of data
                byte[] buffer = new byte[elementCount * vertexStride - offsetInBytes];
                // Copy from the vertex buffer to the temporary buffer
                Marshal.Copy (ptr, buffer, 0, buffer.Length);

                var dataHandle = GCHandle.Alloc (data, GCHandleType.Pinned);
                var dataPtr = (IntPtr)(dataHandle.AddrOfPinnedObject ().ToInt64 () + startIndex * elementSizeInByte);

                // Copy from the temporary buffer to the destination array

                int dataSize = Marshal.SizeOf (typeof (T));
                if (dataSize == vertexStride)
                    Marshal.Copy (buffer, 0, dataPtr, buffer.Length);
                else
                {
                    // If the user is asking for a specific element within the vertex buffer, copy them one by one...
                    for (int i = 0; i < elementCount; i++)
                    {
                        Marshal.Copy (buffer, i * vertexStride, dataPtr, dataSize);
                        dataPtr = (IntPtr)(dataPtr.ToInt64() + dataSize);
                    }
                }

                dataHandle.Free ();

                //Buffer.BlockCopy (buffer, 0, data, startIndex * elementSizeInByte, elementCount * elementSizeInByte);
            }
            GL.UnmapBuffer (BufferTarget.ArrayBuffer);*/
        }

        public void SetData<T> (T[] data)
        where T
            : struct
            , IVertexType
        {
            this.SetData (data, 0, this.vertexCount);
        }

        public T[] GetData<T> ()
        where T
            : struct
            , IVertexType
        {
            return this.GetData<T> (0, this.vertexCount);
        }

        public void SetData<T> (T[] data, Int32 startIndex, Int32 elementCount)
        where T
            : struct
            , IVertexType
        {
            if (data.Length != vertexCount)
            {
                throw new Exception ("?");
            }

            this.Activate ();

            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            GL.BufferSubData (
                this.type,
                (System.IntPtr) (this.vertDecl.VertexStride * startIndex),
                (System.IntPtr) (this.vertDecl.VertexStride * elementCount),
                data);

            KrErrorHandler.Check ();
        }

        public T[] GetData<T> (Int32 startIndex, Int32 elementCount)
        where T
            : struct
            , IVertexType
        {
            throw new System.NotSupportedException ();
        }

        public void SetRawData (
            Byte[] data,
            Int32 startIndex,
            Int32 elementCount)
        {
            this.Activate ();

            GL.BufferSubData (
                this.type,
                (System.IntPtr) (this.vertDecl.VertexStride * startIndex),
                (System.IntPtr) (this.vertDecl.VertexStride * elementCount),
                data);

            KrErrorHandler.Check ();
        }

        public Byte[] GetRawData (
            Int32 startIndex,
            Int32 elementCount)
        {
            throw new System.NotSupportedException ();
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class KrShader
        : IDisposable
    {
        public List<KrShaderInput> Inputs { get; private set; }
        public List<KrShaderVariable> Variables { get; private set; }
        public List<KrShaderSampler> Samplers { get; private set; }

        internal string VariantName { get { return variantName; }}
        Int32 programHandle;
        Int32 fragShaderHandle;
        Int32 vertShaderHandle;

        // for debugging
        string variantName;
        string passName;

        internal KrShader (
            String variantName,
            String passName,
            String vertexShaderSource,
            String pixelShaderSource)
        {
            InternalUtils.Log.Info ("GFX", "  Creating Pass Variant: " + variantName);
            this.variantName = variantName;
            this.passName = passName;

            //Variables =
            programHandle = KrShaderUtils.CreateShaderProgram ();

            vertShaderHandle = KrShaderUtils.CreateVertexShader (vertexShaderSource);
            fragShaderHandle = KrShaderUtils.CreateFragmentShader (pixelShaderSource);

            KrShaderUtils.AttachShader (programHandle, vertShaderHandle);
            KrShaderUtils.AttachShader (programHandle, fragShaderHandle);

        }

        public override string ToString ()
        {
            //string a = Inputs.Select (x => x.Name).Join (", ");
            //string b = Variables.Select (x => x.Name).Join (", ");

            string a = string.Empty;

            for (int i = 0; i < Inputs.Count; ++i)
            {
                a += Inputs[i].Name; if (i + 1 < Inputs.Count) { a += ", "; }
            }

            string b = string.Empty;
            for (int i = 0; i < Variables.Count; ++i)
            {
                b += Variables[i].Name; if (i + 1 < Variables.Count) { b += ", "; }
            }

            return
                String.Format (
                    "[KrShader: Variant {0}, Pass {1}: Inputs: [{2}], Variables: [{3}]]",
                    variantName,
                    passName,
                    a,
                    b);
        }

        internal void ValidateInputs (List<ShaderInputDefinition> definitions)
        {
            InternalUtils.Log.Info ("GFX", 
                String.Format (
                    "Pass: {1} => ValidateInputs ({0})",
                    variantName,
                    passName));

            // Make sure that this shader implements all of the non-optional defined inputs.
            var nonOptionalDefinitions = definitions.Where (y => !y.Optional).ToList ();

            foreach (var definition in nonOptionalDefinitions)
            {
                var find = Inputs.Find (x => x.Name == definition.Name/* && x.Type == definition.Type */);

                if (find == null)
                {
                    throw new Exception ("problem");
                }
            }

            // Make sure that every implemented input is defined.
            foreach (var input in Inputs)
            {
                var find = definitions.Find (x => x.Name == input.Name
                    /*&& (x.Type == input.Type || (x.Type == typeof (Rgba32) && input.Type == typeof (Vector4)))*/
                    );

                if (find == null)
                {
                    throw new Exception ("problem");
                }
                else
                {
                    input.RegisterExtraInfo (find);
                }
            }
        }

        internal void ValidateVariables (List<ShaderVariableDefinition> definitions)
        {
            InternalUtils.Log.Info ("GFX", 
                String.Format (
                    "Pass: {1} => ValidateVariables ({0})",
                    variantName,
                    passName));


            // Make sure that every implemented input is defined.
            foreach (var variable in Variables)
            {
                var find = definitions.Find (
                    x =>
                    x.Name == variable.Name //&&
                    //(x.Type == variable.Type || (x.Type == typeof (Rgba32) && variable.Type == typeof (Vector4)))
                    );

                if (find == null)
                {
                    throw new Exception ("problem");
                }
                else
                {
                    variable.RegisterExtraInfo (find);
                }
            }
        }

        internal void ValidateSamplers (List<ShaderSamplerDefinition> definitions)
        {
            InternalUtils.Log.Info ("GFX", 
                String.Format (
                    "Pass: {1} => ValidateSamplers ({0})",
                    variantName,
                    passName));

            var nonOptionalSamplers =
                definitions
                    .Where (y => !y.Optional)
                    .ToList ();

            foreach (var definition in nonOptionalSamplers)
            {
                var find = this.Samplers.Find (x => x.Name == definition.Name);

                if (find == null)
                {
                    throw new Exception ("problem");
                }
            }

            // Make sure that every implemented input is defined.
            foreach (var sampler in this.Samplers)
            {
                var find = definitions.Find (x => x.Name == sampler.Name);

                if (find == null)
                {
                    throw new Exception ("problem");
                }
                else
                {
                    sampler.RegisterExtraInfo (find);
                }
            }
        }

        static string GetResourcePath (string path)
        {
            string ext = Path.GetExtension (path);

            string filename = path.Substring (0, path.Length - ext.Length);

            var resourcePathname =
#if COR_PLATFORM_XIOS
                MonoTouch.Foundation.NSBundle.MainBundle.PathForResource (
#else
                global::MonoMac.Foundation.NSBundle.MainBundle.PathForResource (
#endif
                    filename,
                    ext.Substring (1, ext.Length - 1)
                );

            if (resourcePathname == null)
            {
                throw new Exception ("Resource [" + path + "] not found");
            }

            return resourcePathname;
        }

        internal void BindAttributes (IList<String> orderedAttributes)
        {
            int index = 0;

            foreach (var attName in orderedAttributes)
            {
                GL.BindAttribLocation (programHandle, index, attName);
                KrErrorHandler.Check ();
                bool success = KrShaderUtils.LinkProgram (programHandle);
                if (success)
                {
                    index++;
                }

            }
        }

        internal void Link ()
        {
            // bind atts here
            //ShaderUtils.LinkProgram (programHandle);

            InternalUtils.Log.Info ("GFX", "  Finishing linking");

            InternalUtils.Log.Info ("GFX", "  Initilise Attributes");
            var attributes = KrShaderUtils.GetAttributes (programHandle);

            Inputs = attributes
                .Select (x => new KrShaderInput (programHandle, x))
                .OrderBy (y => y.AttributeLocation)
                .ToList ();

            String logInputs = "  Inputs : ";
            foreach (var input in Inputs) {
                logInputs += input.Name + ", ";
            }
            InternalUtils.Log.Info ("GFX", logInputs);

            InternalUtils.Log.Info ("GFX", "  Initilise Uniforms");
            var uniforms = KrShaderUtils.GetUniforms (programHandle);


            Variables = uniforms
                .Where (y =>
                       y.Type != ActiveUniformType.Sampler2D &&
                       y.Type != ActiveUniformType.SamplerCube)
                .Select (x => new KrShaderVariable (programHandle, x))
                .OrderBy (z => z.UniformLocation)
                .ToList ();
            String logVars = "  Variables : ";
            foreach (var variable in Variables) {
                logVars += variable.Name + ", ";
            }
            InternalUtils.Log.Info ("GFX", logVars);

            InternalUtils.Log.Info ("GFX", "  Initilise Samplers");
            Samplers = uniforms
                .Where (y =>
                       y.Type == ActiveUniformType.Sampler2D ||
                       y.Type == ActiveUniformType.SamplerCube)
                .Select (x => new KrShaderSampler (programHandle, x))
                .OrderBy (z => z.UniformLocation)
                .ToList ();

            #if DEBUG
            KrShaderUtils.ValidateProgram (programHandle);
            #endif

            KrShaderUtils.DetachShader (programHandle, fragShaderHandle);
            KrShaderUtils.DetachShader (programHandle, vertShaderHandle);

            KrShaderUtils.DeleteShader (programHandle, fragShaderHandle);
            KrShaderUtils.DeleteShader (programHandle, vertShaderHandle);
        }

        public void Activate ()
        {
            GL.UseProgram (programHandle);
            KrErrorHandler.Check ();
        }

        public void Dispose ()
        {
            KrShaderUtils.DestroyShaderProgram (programHandle);
            KrErrorHandler.Check ();
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents an Open GL ES shader input, all the data is read dynamically from
    /// the shader at runtime, not from the ShaderInputDefinition.  This way we can compare the
    /// two and check to see that we have what we are expecting.
    /// </summary>
    public sealed class KrShaderInput
    {
        int ProgramHandle { get; set; }
        internal int AttributeLocation { get; private set; }

        public String Name { get; private set; }
        public Type Type { get; private set; }
        public VertexElementUsage Usage { get; private set; }
        public Object DefaultValue { get; private set; }
        public Boolean Optional { get; private set; }

        public KrShaderInput (
            int programHandle, KrShaderUtils.KrShaderAttribute attribute)
        {
            int attLocation = GL.GetAttribLocation (programHandle, attribute.Name);

            KrErrorHandler.Check ();

            InternalUtils.Log.Info ("GFX", string.Format (
                "    Binding Shader Input: [Prog={0}, AttIndex={1}, AttLocation={4}, AttName={2}, AttType={3}]",
                programHandle, attribute.Index, attribute.Name, attribute.Type, attLocation));

            this.ProgramHandle = programHandle;
            this.AttributeLocation = attLocation;
            this.Name = attribute.Name;
            this.Type = KrEnumConverter.ToType (attribute.Type);


        }

        internal void RegisterExtraInfo (ShaderInputDefinition definition)
        {
            Usage = definition.Usage;
            DefaultValue = definition.DefaultValue;
            Optional = definition.Optional;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class KrShaderVariable
    {
        int ProgramHandle { get; set; }
        internal int UniformLocation { get; private set; }

        public String NiceName { get; private set; }
        public String Name { get; private set; }
        public Type Type { get; private set; }
        public Object DefaultValue { get; private set; }

        public KrShaderVariable (
            int programHandle, KrShaderUtils.KrShaderUniform uniform)
        {

            this.ProgramHandle = programHandle;

            int uniformLocation = GL.GetUniformLocation (programHandle, uniform.Name);

            KrErrorHandler.Check ();

            if (uniformLocation == -1 )
                throw new Exception ();

            this.UniformLocation = uniformLocation;
            this.Name = uniform.Name;
            this.Type = Cor.Lib.Khronos.KrEnumConverter.ToType (uniform.Type);

            InternalUtils.Log.Info ("GFX", 
                String.Format (
                "    Caching Reference to Shader Variable: [Prog={0}, UniIndex={1}, UniLocation={2}, UniName={3}, UniType={4}]",
                programHandle,
                uniform.Index,
                uniformLocation,
                uniform.Name,
                uniform.Type));

        }

        internal void RegisterExtraInfo (ShaderVariableDefinition definition)
        {
            NiceName = definition.NiceName;
            DefaultValue = definition.DefaultValue;
        }

        public void Set (object value)
        {
            //todo this should be using convert turn the data into proper opengl es types.
            Type t = value.GetType ();

            if (t == typeof (Matrix44) )
            {
                var castValue = (Matrix44) value;
                var otkValue = KrMatrix44Converter.ToKhronos (castValue);
                GL.UniformMatrix4(UniformLocation, false, ref otkValue);
            }
            else if (t == typeof (Int32) )
            {
                var castValue = (Int32) value;
                GL.Uniform1(UniformLocation, 1, ref castValue);
            }
            else if (t == typeof (Single) )
            {
                var castValue = (Single) value;
                GL.Uniform1(UniformLocation, 1, ref castValue);
            }
            else if (t == typeof (Abacus.SinglePrecision.Vector2) )
            {
                var castValue = (Abacus.SinglePrecision.Vector2) value;
                GL.Uniform2(UniformLocation, 1, ref castValue.X);
            }
            else if (t == typeof (Abacus.SinglePrecision.Vector3) )
            {
                var castValue = (Abacus.SinglePrecision.Vector3) value;
                GL.Uniform3(UniformLocation, 1, ref castValue.X);
            }
            else if (t == typeof (Abacus.SinglePrecision.Vector4) )
            {
                var castValue = (Abacus.SinglePrecision.Vector4) value;
                GL.Uniform4(UniformLocation, 1, ref castValue.X);
            }
            else if (t == typeof (Rgba32) )
            {
                var castValue = (Rgba32) value;

                Abacus.SinglePrecision.Vector4 vec4Value;
                castValue.UnpackTo (out vec4Value);

                // does this rgba value need to be packed in to a vector3 or a vector4
                if (this.Type == typeof (Abacus.SinglePrecision.Vector4) )
                    GL.Uniform4(UniformLocation, 1, ref vec4Value.X);
                else if (this.Type == typeof (Abacus.SinglePrecision.Vector3) )
                    GL.Uniform3(UniformLocation, 1, ref vec4Value.X);
                else
                    throw new Exception ("Not supported");
            }
            else
            {
                throw new Exception ("Not supported");
            }

            KrErrorHandler.Check ();

        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class KrShaderSampler
    {
        int ProgramHandle { get; set; }
        internal int UniformLocation { get; private set; }

        public String NiceName { get; set; }
        public String Name { get; set; }

        public KrShaderSampler (
            int programHandle, KrShaderUtils.KrShaderUniform uniform)
        {
            this.ProgramHandle = programHandle;

            int uniformLocation = GL.GetUniformLocation (programHandle, uniform.Name);

            KrErrorHandler.Check ();

            this.UniformLocation = uniformLocation;
            this.Name = uniform.Name;
        }

        internal void RegisterExtraInfo (ShaderSamplerDefinition definition)
        {
            NiceName = definition.NiceName;
        }

        public void SetSlot (Int32 slot)
        {
            // set the sampler texture unit to 0
            GL.Uniform1(this.UniformLocation, slot);
            KrErrorHandler.Check ();
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Shader variants could be useful outside of Khronos, PSM defo.
    /// It'd be good to make this system more generic and make it
    /// part of Cor.
    /// </summary>
    public static class KrShaderHelper
    {
        /// <summary>
        /// This function takes a VertexDeclaration and a collection of
        /// OpenGL shader passes and works out which
        /// pass is the best fit for the VertexDeclaration.
        /// </summary>
        public static KrShader WorkOutBestVariantFor (
            VertexDeclaration vertexDeclaration,
            IList<KrShader> variants)
        {
            InternalUtils.Log.Info ("GFX", "\n");
            InternalUtils.Log.Info ("GFX", "\n");
            InternalUtils.Log.Info ("GFX", "=====================================================================");
			InternalUtils.Log.Info ("GFX", "Working out the best shader variant for: " + vertexDeclaration);
            InternalUtils.Log.Info ("GFX", "Possible variants:");

            int best = 0;

            int bestNumMatchedVertElems = 0;
            int bestNumUnmatchedVertElems = 0;
            int bestNumMissingNonOptionalInputs = 0;

            // foreach variant
            for (int i = 0; i < variants.Count; ++i)
            {
                // work out how many vert inputs match


                var matchResult = CompareShaderInputs (vertexDeclaration, variants[i]);

                int numMatchedVertElems = matchResult.NumMatchedInputs;
                int numUnmatchedVertElems = matchResult.NumUnmatchedInputs;
                int numMissingNonOptionalInputs = matchResult.NumUnmatchedRequiredInputs;

                InternalUtils.Log.Info ("GFX", " - " + variants[i]);

                if (i == 0 )
                {
                    bestNumMatchedVertElems = numMatchedVertElems;
                    bestNumUnmatchedVertElems = numUnmatchedVertElems;
                    bestNumMissingNonOptionalInputs = numMissingNonOptionalInputs;
                }
                else
                {
                    if (
                        (
                            numMatchedVertElems > bestNumMatchedVertElems &&
                            bestNumMissingNonOptionalInputs == 0
                        )
                        ||
                        (
                            numMatchedVertElems == bestNumMatchedVertElems &&
                            bestNumMissingNonOptionalInputs == 0 &&
                            numUnmatchedVertElems < bestNumUnmatchedVertElems
                        )
                      )
                    {
                        bestNumMatchedVertElems = numMatchedVertElems;
                        bestNumUnmatchedVertElems = numUnmatchedVertElems;
                        bestNumMissingNonOptionalInputs = numMissingNonOptionalInputs;
                        best = i;
                    }

                }

            }

            //best = 2;
            InternalUtils.Log.Info ("GFX", "Chosen variant: " + variants[best].VariantName);

            return variants[best];
        }

        struct CompareShaderInputsResult
        {
            // the nume
            public int NumMatchedInputs;
            public int NumUnmatchedInputs;
            public int NumUnmatchedRequiredInputs;
        }

        static CompareShaderInputsResult CompareShaderInputs (
            VertexDeclaration vertexDeclaration,
            KrShader oglesShader
            )
        {
            var result = new CompareShaderInputsResult ();

            var oglesShaderInputsUsed = new List<KrShaderInput>();

            var vertElems = vertexDeclaration.GetVertexElements ();

            // itterate over each input defined in the vert decl
            foreach (var vertElem in vertElems)
            {
                var usage = vertElem.VertexElementUsage;

                var format = vertElem.VertexElementFormat;
                /*

                foreach (var input in oglesShader.Inputs)
                {
                    // the vertDecl knows what each input's intended use is,
                    // so lets match up
                    if (input.Usage == usage)
                    {
                        // intended use seems good
                    }
                }

                // find all inputs that could match
                var matchingInputs = oglesShader.Inputs.FindAll (
                    x =>

                        x.Usage == usage &&
                        (x.Type == VertexElementFormatHelper.FromEnum (format) ||
                        ( (x.Type.GetType () == typeof (Vector4)) && (format == VertexElementFormat.Colour) ))

                 );*/

                var matchingInputs = oglesShader.Inputs.FindAll (x => x.Usage == usage);

                // now make sure it's not been used already

                while (matchingInputs.Count > 0)
                {
                    var potentialInput = matchingInputs[0];

                    if (oglesShaderInputsUsed.Find (x => x == potentialInput) != null)
                    {
                        matchingInputs.RemoveAt (0);
                    }
                    else
                    {
                        oglesShaderInputsUsed.Add (potentialInput);
                    }
                }
            }

            result.NumMatchedInputs = oglesShaderInputsUsed.Count;

            result.NumUnmatchedInputs = vertElems.Length - result.NumMatchedInputs;

            result.NumUnmatchedRequiredInputs = 0;

            foreach (var input in oglesShader.Inputs)
            {
                if (!oglesShaderInputsUsed.Contains (input) )
                {
                    if ( !input.Optional)
                    {
                        result.NumUnmatchedRequiredInputs++;
                    }
                }

            }

            InternalUtils.Log.Info ("GFX", 
                String.Format (
                    "[{0}, {1}, {2}]",
                    result.NumMatchedInputs,
                    result.NumUnmatchedInputs,
                    result.NumUnmatchedRequiredInputs));

            return result;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Static class to help with horrible shader system.
    /// </summary>
    public static class KrShaderUtils
    {
        public class KrShaderUniform
        {
            public Int32 Index { get; set; }
            public String Name { get; set; }
            public ActiveUniformType Type { get; set; }
        }

        public class KrShaderAttribute
        {
            public Int32 Index { get; set; }
            public String Name { get; set; }
            public ActiveAttribType Type { get; set; }
        }

        public static Int32 CreateShaderProgram ()
        {
            // Create shader program.
            Int32 programHandle = GL.CreateProgram ();

            if (programHandle == 0 )
                throw new Exception ("Failed to create shader program");

            KrErrorHandler.Check ();

            return programHandle;
        }

        public static Int32 CreateVertexShader (String source)
        {
            Int32 vertShaderHandle;

            KrShaderUtils.CompileShader (
                GLShaderType.VertexShader,
                source,
                out vertShaderHandle);

            if (vertShaderHandle == 0 )
                throw new Exception ("Failed to compile vertex shader program");

            return vertShaderHandle;
        }

        public static Int32 CreateFragmentShader (String source)
        {
            Int32 fragShaderHandle;

            KrShaderUtils.CompileShader (
                GLShaderType.FragmentShader,
                source,
                out fragShaderHandle);

            if (fragShaderHandle == 0 )
                throw new Exception ("Failed to compile fragment shader program");


            return fragShaderHandle;
        }

        public static void AttachShader (
            Int32 programHandle,
            Int32 shaderHandle)
        {
            if (shaderHandle != 0)
            {
                // Attach vertex shader to program.
                GL.AttachShader (programHandle, shaderHandle);
                KrErrorHandler.Check ();
            }
        }

        public static void DetachShader (
            Int32 programHandle,
            Int32 shaderHandle)
        {
            if (shaderHandle != 0)
            {
                GL.DetachShader (programHandle, shaderHandle);
                KrErrorHandler.Check ();
            }
        }

        public static void DeleteShader (
            Int32 programHandle,
            Int32 shaderHandle)
        {
            if (shaderHandle != 0)
            {
                GL.DeleteShader (shaderHandle);
                shaderHandle = 0;
                KrErrorHandler.Check ();
            }
        }

        public static void DestroyShaderProgram (Int32 programHandle)
        {
            if (programHandle != 0)
            {
#if             COR_PLATFORM_XIOS
                GL.DeleteProgram (programHandle);
#elif           COR_PLATFORM_MONOMAC
                try
                {
                    GL.DeleteProgram (1, ref programHandle);
                }
                catch (Exception ex)
                {
                    InternalUtils.Log.Error ("FUCK! (It seems like OpenTK is broken): " + ex.Message);
                }
#endif

                programHandle = 0;
                KrErrorHandler.Check ();
            }
        }

        // This should happen offline.
        public static void CompileShader (
            GLShaderType type,
            String src,
            out Int32 shaderHandle)
        {
            // Create an empty vertex shader object
            shaderHandle = GL.CreateShader (type);

            KrErrorHandler.Check ();

            // Replace the source code in the vertex shader object
#if COR_PLATFORM_XIOS
            GL.ShaderSource (
                shaderHandle,
                1,
                new String[] { src },
                (Int32[]) null);
#elif COR_PLATFORM_MONOMAC
            GL.ShaderSource (
                shaderHandle,
                src);
#endif

            KrErrorHandler.Check ();

            GL.CompileShader (shaderHandle);

            KrErrorHandler.Check ();

#if DEBUG
            Int32 logLength = 0;
            GL.GetShader (
                shaderHandle,
                ShaderParameter.InfoLogLength,
                out logLength);

            KrErrorHandler.Check ();
            var infoLog = new System.Text.StringBuilder (logLength);

            if (logLength > 0)
            {
                int temp = 0;
                GL.GetShaderInfoLog (
                    shaderHandle,
                    logLength,
                    out temp,
                    infoLog);

                string log = infoLog.ToString ();

                InternalUtils.Log.Info ("GFX", src);
                InternalUtils.Log.Info ("GFX", log);
                InternalUtils.Log.Info ("GFX", type.ToString ());
            }
#endif
            Int32 status = 0;

            GL.GetShader (
                shaderHandle,
                ShaderParameter.CompileStatus,
                out status);

            KrErrorHandler.Check ();

            if (status == 0)
            {
                GL.DeleteShader (shaderHandle);
                throw new Exception ("Failed to compile " + type.ToString ());
            }
        }

        public static List<KrShaderUniform> GetUniforms (Int32 prog)
        {

            int numActiveUniforms = 0;

            var result = new List<KrShaderUniform>();

            GL.GetProgram (prog, ProgramParameter.ActiveUniforms, out numActiveUniforms);
            KrErrorHandler.Check ();

            for (int i = 0; i < numActiveUniforms; ++i)
            {
                var sb = new System.Text.StringBuilder ();

                int buffSize = 0;
                int length = 0;
                int size = 0;
                ActiveUniformType type;

                GL.GetActiveUniform (
                    prog,
                    i,
                    64,
                    out length,
                    out size,
                    out type,
                    sb);
                KrErrorHandler.Check ();

                result.Add (
                    new KrShaderUniform ()
                    {
                    Index = i,
                    Name = sb.ToString (),
                    Type = type
                    }
                );
            }

            return result;
        }

        public static List<KrShaderAttribute> GetAttributes (Int32 prog)
        {
            int numActiveAttributes = 0;

            var result = new List<KrShaderAttribute>();

            // gets the number of active vertex attributes
            GL.GetProgram (prog, ProgramParameter.ActiveAttributes, out numActiveAttributes);
            KrErrorHandler.Check ();

            for (int i = 0; i < numActiveAttributes; ++i)
            {
                var sb = new System.Text.StringBuilder ();

                int buffSize = 0;
                int length = 0;
                int size = 0;
                ActiveAttribType type;
                GL.GetActiveAttrib (
                    prog,
                    i,
                    64,
                    out length,
                    out size,
                    out type,
                    sb);
                KrErrorHandler.Check ();

                result.Add (
                    new KrShaderAttribute ()
                    {
                        Index = i,
                        Name = sb.ToString (),
                        Type = type
                    }
                );
            }

            return result;
        }


        public static bool LinkProgram (Int32 prog)
        {
            bool retVal = true;

            GL.LinkProgram (prog);

            KrErrorHandler.Check ();

#if DEBUG
            Int32 logLength = 0;

            GL.GetProgram (
                prog,
                ProgramParameter.InfoLogLength,
                out logLength);

            KrErrorHandler.Check ();

            if (logLength > 0)
            {
                retVal = false;

                /*
                var infoLog = new System.Text.StringBuilder ();

                GL.GetProgramInfoLog (
                    prog,
                    logLength,
                    out logLength,
                    infoLog);
                */
                var infoLog = string.Empty;
                GL.GetProgramInfoLog (prog, out infoLog);


                KrErrorHandler.Check ();

                InternalUtils.Log.Info ("GFX", string.Format ("[Cor.Resources] Program link log:\n{0}", infoLog));
            }
#endif
            Int32 status = 0;

            GL.GetProgram (
                prog,
                ProgramParameter.LinkStatus,
                out status);

            KrErrorHandler.Check ();

            if (status == 0)
            {
                throw new Exception (String.Format ("Failed to link program: {0:x}", prog));
            }

            return retVal;

        }

        public static void ValidateProgram (Int32 programHandle)
        {
            GL.ValidateProgram (programHandle);

            KrErrorHandler.Check ();

            Int32 logLength = 0;

            GL.GetProgram (
                programHandle,
                ProgramParameter.InfoLogLength,
                out logLength);

            KrErrorHandler.Check ();

            if (logLength > 0)
            {
                var infoLog = new System.Text.StringBuilder ();

                GL.GetProgramInfoLog (
                    programHandle,
                    logLength,
                    out logLength, infoLog);

                KrErrorHandler.Check ();

                InternalUtils.Log.Info ("GFX", string.Format ("[Cor.Resources] Program validate log:\n{0}", infoLog));
            }

            Int32 status = 0;

            GL.GetProgram (
                programHandle, ProgramParameter.LinkStatus,
                out status);

            KrErrorHandler.Check ();

            if (status == 0)
            {
                throw new Exception (String.Format ("Failed to validate program {0:x}", programHandle));
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class KrErrorHandler
    {
        [Conditional ("DEBUG")]
        public static void Check ()
        {
            var ec = GL.GetError ();

            if (ec != ErrorCode.NoError)
            {
                throw new Exception (ec.ToString ());
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal static class KrEnumConverter
    {
        internal static TextureUnit ToKhronosTextureSlot (Int32 slot)
        {
            switch (slot)
            {
                case 0: return TextureUnit.Texture0;
                case 1: return TextureUnit.Texture1;
                case 2: return TextureUnit.Texture2;
                case 3: return  TextureUnit.Texture3;
                case 4: return TextureUnit.Texture4;
                case 5: return TextureUnit.Texture5;
                case 6: return TextureUnit.Texture6;
                case 7: return TextureUnit.Texture7;
                case 8: return TextureUnit.Texture8;
                case 9: return TextureUnit.Texture9;
                case 10: return TextureUnit.Texture10;
                case 11: return TextureUnit.Texture11;
                case 12: return TextureUnit.Texture12;
                case 13: return TextureUnit.Texture13;
                case 14: return TextureUnit.Texture14;
                case 15: return TextureUnit.Texture15;
                case 16: return TextureUnit.Texture16;
                case 17: return TextureUnit.Texture17;
                case 18: return TextureUnit.Texture18;
                case 19: return TextureUnit.Texture19;
                case 20: return TextureUnit.Texture20;
                case 21: return TextureUnit.Texture21;
                case 22: return TextureUnit.Texture22;
                case 23: return TextureUnit.Texture23;
                case 24: return TextureUnit.Texture24;
                case 25: return TextureUnit.Texture25;
                case 26: return TextureUnit.Texture26;
                case 27: return TextureUnit.Texture27;
                case 28: return TextureUnit.Texture28;
                case 29: return TextureUnit.Texture29;
                case 30: return TextureUnit.Texture30;
            }

            throw new NotSupportedException ();
        }


        internal static Type ToType (ActiveAttribType ogl)
        {
            switch (ogl)
            {
            case ActiveAttribType.Float: return typeof (Single);
            case ActiveAttribType.FloatMat2: throw new NotSupportedException ();
            case ActiveAttribType.FloatMat3: throw new NotSupportedException ();
            case ActiveAttribType.FloatMat4: return typeof (Abacus.SinglePrecision.Matrix44);
            case ActiveAttribType.FloatVec2: return typeof (Abacus.SinglePrecision.Vector2);
            case ActiveAttribType.FloatVec3: return typeof (Abacus.SinglePrecision.Vector3);
            case ActiveAttribType.FloatVec4: return typeof (Abacus.SinglePrecision.Vector4);
            }

            throw new NotSupportedException ();
        }

        internal static Type ToType (ActiveUniformType ogl)
        {
            switch (ogl)
            {
            case ActiveUniformType.Bool: return typeof (Boolean);
            case ActiveUniformType.BoolVec2: throw new NotSupportedException ();
            case ActiveUniformType.BoolVec3: throw new NotSupportedException ();
            case ActiveUniformType.BoolVec4: throw new NotSupportedException ();
            case ActiveUniformType.Float: return typeof (Single);
            case ActiveUniformType.FloatMat2: throw new NotSupportedException ();
            case ActiveUniformType.FloatMat3: throw new NotSupportedException ();
            case ActiveUniformType.FloatMat4: return typeof (Abacus.SinglePrecision.Matrix44);
            case ActiveUniformType.FloatVec2: return typeof (Abacus.SinglePrecision.Vector2);
            case ActiveUniformType.FloatVec3: return typeof (Abacus.SinglePrecision.Vector3);
            case ActiveUniformType.FloatVec4: return typeof (Abacus.SinglePrecision.Vector4);
            case ActiveUniformType.Int: return typeof (Boolean);
            case ActiveUniformType.IntVec2: throw new NotSupportedException ();
            case ActiveUniformType.IntVec3: throw new NotSupportedException ();
            case ActiveUniformType.IntVec4: throw new NotSupportedException ();
            case ActiveUniformType.Sampler2D: throw new NotSupportedException ();
            case ActiveUniformType.SamplerCube: throw new NotSupportedException ();
            }

            throw new NotSupportedException ();
        }

        internal static void ToKhronos (
            VertexElementFormat blimey,
            out VertexAttribPointerType dataFormat,
            out bool normalized,
            out int size)
        {
            normalized = false;
            size = 0;
            dataFormat = VertexAttribPointerType.Float;

            switch (blimey)
            {
                case VertexElementFormat.Single:
                dataFormat = VertexAttribPointerType.Float;
                    size = 1;
                    break;
                case VertexElementFormat.Vector2:
                dataFormat = VertexAttribPointerType.Float;
                    size = 2;
                    break;
                case VertexElementFormat.Vector3:
                dataFormat = VertexAttribPointerType.Float;
                    size = 3;
                    break;
                case VertexElementFormat.Vector4:
                dataFormat = VertexAttribPointerType.Float;
                    size = 4;
                    break;
                case VertexElementFormat.Colour:
                dataFormat = VertexAttribPointerType.UnsignedByte;
                    normalized = true;
                    size = 4;
                    break;
                case VertexElementFormat.Byte4: throw new Exception ("?");
                case VertexElementFormat.Short2: throw new Exception ("?");
                case VertexElementFormat.Short4: throw new Exception ("?");
                case VertexElementFormat.NormalisedShort2: throw new Exception ("?");
                case VertexElementFormat.NormalisedShort4: throw new Exception ("?");
                case VertexElementFormat.HalfVector2: throw new Exception ("?");
                case VertexElementFormat.HalfVector4: throw new Exception ("?");
            }
        }

        internal static BlendingFactorSrc ToKhronosSrc (BlendFactor blimey)
        {
            switch (blimey)
            {
                case BlendFactor.Zero: return BlendingFactorSrc.Zero;
                case BlendFactor.One: return BlendingFactorSrc.One;

#if COR_PLATFORM_MONOMAC

                case BlendFactor.SourceColour: return BlendingFactorSrc.Src1Color; // todo: check this src1 stuff
                case BlendFactor.InverseSourceColour: return BlendingFactorSrc.OneMinusSrc1Color;

#elif COR_PLATFORM_XIOS

                case BlendFactor.SourceColour: return BlendingFactorSrc.SrcColor;
                case BlendFactor.InverseSourceColour: return BlendingFactorSrc.OneMinusSrcColor;

#endif

                case BlendFactor.SourceAlpha: return BlendingFactorSrc.SrcAlpha;
                case BlendFactor.InverseSourceAlpha: return BlendingFactorSrc.OneMinusSrcAlpha;
                case BlendFactor.DestinationAlpha: return BlendingFactorSrc.DstAlpha;
                case BlendFactor.InverseDestinationAlpha: return BlendingFactorSrc.OneMinusDstAlpha;
                case BlendFactor.DestinationColour: return BlendingFactorSrc.DstColor;
                case BlendFactor.InverseDestinationColour: return BlendingFactorSrc.OneMinusDstColor;
            }

            throw new Exception ();
        }

        internal static BlendingFactorDest ToKhronosDest (BlendFactor blimey)
        {
            switch (blimey)
            {
                case BlendFactor.Zero: return BlendingFactorDest.Zero;
                case BlendFactor.One: return BlendingFactorDest.One;
                case BlendFactor.SourceColour: return BlendingFactorDest.SrcColor;
                case BlendFactor.InverseSourceColour: return BlendingFactorDest.OneMinusSrcColor;
                case BlendFactor.SourceAlpha: return BlendingFactorDest.SrcAlpha;
                case BlendFactor.InverseSourceAlpha: return BlendingFactorDest.OneMinusSrcAlpha;
                case BlendFactor.DestinationAlpha: return BlendingFactorDest.DstAlpha;
                case BlendFactor.InverseDestinationAlpha: return BlendingFactorDest.OneMinusDstAlpha;
                case BlendFactor.DestinationColour: return BlendingFactorDest.SrcColor;
                case BlendFactor.InverseDestinationColour: return BlendingFactorDest.OneMinusSrcColor;
            }

            throw new Exception ();
        }

        internal static BlendFactor ToCorDestinationBlendFactor (All ogl)
        {
            switch (ogl)
            {
                case All.Zero: return BlendFactor.Zero;
                case All.One: return BlendFactor.One;
                case All.SrcColor: return BlendFactor.SourceColour;
                case All.OneMinusSrcColor: return BlendFactor.InverseSourceColour;
                case All.SrcAlpha: return BlendFactor.SourceAlpha;
                case All.OneMinusSrcAlpha: return BlendFactor.InverseSourceAlpha;
                case All.DstAlpha: return BlendFactor.DestinationAlpha;
                case All.OneMinusDstAlpha: return BlendFactor.InverseDestinationAlpha;
                case All.DstColor: return BlendFactor.DestinationColour;
                case All.OneMinusDstColor: return BlendFactor.InverseDestinationColour;
            }

            throw new Exception ();
        }

        internal static BlendEquationMode ToKhronos (BlendFunction blimey)
        {
            switch (blimey)
            {
                case BlendFunction.Add: return BlendEquationMode.FuncAdd;
                case BlendFunction.Max: throw new NotSupportedException ();
                case BlendFunction.Min: throw new NotSupportedException ();
                case BlendFunction.ReverseSubtract: return BlendEquationMode.FuncReverseSubtract;
                case BlendFunction.Subtract: return BlendEquationMode.FuncSubtract;
            }

            throw new Exception ();
        }

        internal static BlendFunction ToCorDestinationBlendFunction (All ogl)
        {
            switch (ogl)
            {
                case All.FuncAdd: return BlendFunction.Add;
                case All.MaxExt: return BlendFunction.Max;
                case All.MinExt: return BlendFunction.Min;
                case All.FuncReverseSubtract: return BlendFunction.ReverseSubtract;
                case All.FuncSubtract: return BlendFunction.Subtract;
            }

            throw new Exception ();
        }

        // PRIMITIVE TYPE
        internal static BeginMode ToKhronos (PrimitiveType blimey)
        {
            switch (blimey) {
            case PrimitiveType.LineList:
                return  BeginMode.Lines;
            case PrimitiveType.LineStrip:
                return  BeginMode.LineStrip;
            case PrimitiveType.TriangleList:
                return  BeginMode.Triangles;
            case PrimitiveType.TriangleStrip:
                return  BeginMode.TriangleStrip;

            default:
                throw new Exception ("problem");
            }
        }

        internal static PrimitiveType ToCorPrimitiveType (All ogl)
        {
            switch (ogl) {
            case All.Lines:
                return  PrimitiveType.LineList;
            case All.LineStrip:
                return  PrimitiveType.LineStrip;
            case All.Points:
                throw new Exception ("Not supported by Cor");
            case All.TriangleFan:
                throw new Exception ("Not supported by Cor");
            case All.Triangles:
                return  PrimitiveType.TriangleList;
            case All.TriangleStrip:
                return  PrimitiveType.TriangleStrip;

            default:
                throw new Exception ("problem");

            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class KrVector2Converter
    {
        // VECTOR 2
        public static KhronosVector2 ToKhronos (this Abacus.SinglePrecision.Vector2 vec)
        {
            return new KhronosVector2 (vec.X, vec.Y);
        }

        public static Abacus.SinglePrecision.Vector2 ToAbacus (this KhronosVector2 vec)
        {
            return new Abacus.SinglePrecision.Vector2 (vec.X, vec.Y);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class KrVector3Converter
    {
        // VECTOR 3
        public static KhronosVector3 ToKhronos (this Abacus.SinglePrecision.Vector3 vec)
        {
            return new KhronosVector3 (vec.X, vec.Y, vec.Z);
        }

        public static Abacus.SinglePrecision.Vector3 ToAbacus (this KhronosVector3 vec)
        {
            return new Abacus.SinglePrecision.Vector3 (vec.X, vec.Y, vec.Z);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class KrVector4Converter
    {
        // VECTOR 3
        public static KhronosVector4 ToKhronos (this Abacus.SinglePrecision.Vector4 vec)
        {
            return new KhronosVector4 (vec.X, vec.Y, vec.Z, vec.W);
        }

        public static Abacus.SinglePrecision.Vector4 ToAbacus (this KhronosVector4 vec)
        {
            return new Abacus.SinglePrecision.Vector4 (vec.X, vec.Y, vec.Z, vec.W);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class KrMatrix44Converter
    {
        public static KhronosMatrix4 ToKhronos (this Abacus.SinglePrecision.Matrix44 mat)
        {
            return new KhronosMatrix4(
                mat.R0C0, mat.R0C1, mat.R0C2, mat.R0C3,
                mat.R1C0, mat.R1C1, mat.R1C2, mat.R1C3,
                mat.R2C0, mat.R2C1, mat.R2C2, mat.R2C3,
                mat.R3C0, mat.R3C1, mat.R3C2, mat.R3C3);
        }

        public static Abacus.SinglePrecision.Matrix44 ToAbacus (this KhronosMatrix4 mat)
        {
            return new Abacus.SinglePrecision.Matrix44(
                mat.M11, mat.M12, mat.M13, mat.M14,
                mat.M21, mat.M22, mat.M23, mat.M24,
                mat.M31, mat.M32, mat.M33, mat.M34,
                mat.M41, mat.M42, mat.M43, mat.M44);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

#endif

}
