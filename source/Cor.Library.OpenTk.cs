﻿// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor.Lib.OpenTK                                                                                                 │ \\
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

namespace Cor.Library.OTK
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

    
    using Fudge;
    using Abacus.SinglePrecision;
    using Cor.Platform;
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
    
    
    
    // Cross platform wrapper around the Open TK libary, sitting at a slightly higher level.
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
    public class OTKShaderHandle
        : Handle
    {
        public Int32 VertexShaderHandle { get; private set; }
        public Int32 FragmentShaderHandle { get; private set; }
        public Int32 ProgramHandle { get; private set; }

        internal OTKShaderHandle (Int32 vertexShaderHandle, Int32 fragmentShaderHandle, Int32 programHandle)
        {
            this.VertexShaderHandle = vertexShaderHandle;
            this.FragmentShaderHandle = fragmentShaderHandle;
            this.ProgramHandle = programHandle;
        }

        protected override void CleanUpNativeResources ()
        {
            OTKWrapper.DestroyShader (this);

            VertexShaderHandle = 0;

            base.CleanUpNativeResources ();
        }
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
    public class OTKTextureHandle
        : Handle
    {
        public Int32 TextureId { get; private set; }
        
        internal OTKTextureHandle (int textureId)
        {
            this.TextureId = TextureId;
        }

        protected override void CleanUpNativeResources ()
        {
            OTKWrapper.DestroyTexture (this);

            TextureId = 0;

            base.CleanUpNativeResources ();
        }
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
    public class OTKIndexBufferHandle
        : Handle
    {
        static Int32 resourceCounter;
        
        public UInt32 GLHandle { get; private set; }
        
        internal OTKIndexBufferHandle (UInt32 glHandle, Int32 indexCount)
        {
            GLHandle = glHandle;
            resourceCounter++;
        }

        protected override void CleanUpNativeResources ()
        {
            OTKWrapper.DestroyIndexBuffer (this);

            GLHandle = 0;
            resourceCounter--;
            
            base.CleanUpNativeResources ();
        }
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
    public class OTKVertexBufferHandle
        : Handle
    {
        static Int32 resourceCounter;
        
        public UInt32 GLHandle { get; private set; }
        
        internal OTKVertexBufferHandle (UInt32 glHandle)
        {
            GLHandle = glHandle;
            resourceCounter++;
        }

        protected override void CleanUpNativeResources ()
        {
            OTKWrapper.DestroyVertexBuffer (this);

            GLHandle = 0;
            resourceCounter--;
            
            base.CleanUpNativeResources ();
        }
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class OTKWrapper
    {
        //#region Global Fucking State
        
        //static GeometryBuffer currentGeomBuffer;
        //static CullMode? currentCullMode;
    
        //#endregion
        
        #region Initilisation
        
        static OTKWrapper ()
        {
            GL.Enable (EnableCap.Blend);
            ThrowErrors ();

            SetBlendEquation (
                BlendFunction.Add, BlendFactor.SourceAlpha, BlendFactor.InverseSourceAlpha,
                BlendFunction.Add, BlendFactor.One, BlendFactor.InverseSourceAlpha);

            GL.Enable (EnableCap.DepthTest);
            ThrowErrors ();

            GL.DepthMask (true);
            ThrowErrors ();

            GL.DepthRange (0f, 1f);
            ThrowErrors ();

            GL.DepthFunc (DepthFunction.Lequal);
            ThrowErrors ();

            SetCullMode (CullMode.CW);
        }
        
        #endregion
        
        #region Public API
        
        #region Render State
        
        public static void ClearColourBuffer (Rgba32 colour)
        {
            Single r, g, b, a;

            colour.UnpackTo (out r, out g, out b, out a);

            GL.ClearColor (r, g, b, a);

            var mask = ClearBufferMask.ColorBufferBit;

            GL.Clear (mask);

            ThrowErrors ();
        }
        
        public static void ClearDepthBuffer (Single depth)
        {
            GL.ClearDepth (depth);

            var mask = ClearBufferMask.DepthBufferBit;

            GL.Clear (mask);

            ThrowErrors ();
        }
        
        public static void SetCullMode (CullMode cullMode)
        {
            if (cullMode == CullMode.None)
            {
                GL.Disable (EnableCap.CullFace);
                ThrowErrors ();
            }
            else
            {
                GL.Enable (EnableCap.CullFace);
                ThrowErrors ();

                GL.FrontFace (FrontFaceDirection.Cw);
                ThrowErrors ();

                if (cullMode == CullMode.CW)
                {
                    GL.CullFace (CullFaceMode.Back);
                    ThrowErrors ();
                }
                else if (cullMode == CullMode.CCW)
                {
                    GL.CullFace (CullFaceMode.Front);
                    ThrowErrors ();
                }
                else
                {
                    throw new NotSupportedException ();
                }
            }
        }
        
        public static void SetBlendEquation (
            BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb, 
            BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha)
        {
            GL.BlendEquationSeparate (
                ConvertToOTKBlendEquationModeEnum (rgbBlendFunction),
                ConvertToOTKBlendEquationModeEnum (alphaBlendFunction) );
            ThrowErrors ();

            GL.BlendFuncSeparate (
                ConvertToOTKBlendingFactorSrcEnum (sourceRgb),
                ConvertToOTKBlendingFactorDestEnum (destinationRgb),
                ConvertToOTKBlendingFactorSrcEnum (sourceAlpha),
                ConvertToOTKBlendingFactorDestEnum (destinationAlpha) );
            ThrowErrors ();
        }
        
        #endregion
        
        #region Vertex Buffers
        
        public static OTKVertexBufferHandle CreateVertexBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount)
        {
            const BufferTarget type = BufferTarget.ArrayBuffer;
            const GLBufferUsage bufferUsage = GLBufferUsage.DynamicDraw;
            
            UInt32 bufferHandle = 0;
            GL.GenBuffers (1, out bufferHandle);
            ThrowErrors ();
            
            var handle = new OTKVertexBufferHandle (bufferHandle);

            if (bufferHandle == 0 )
            {
                throw new Exception ("Failed to generate vert buffer.");
            }

            ActivateVertexBuffer (handle);

            GL.BufferData (
                type,
                (System.IntPtr) (vertexDeclaration.VertexStride * vertexCount),
                (System.IntPtr) null,
                bufferUsage);

            ThrowErrors ();
            
            return handle;
        }
        
        public static void ActivateVertexBuffer (OTKVertexBufferHandle handle)
        {
            const BufferTarget type = BufferTarget.ArrayBuffer;
            GL.BindBuffer (type, handle.GLHandle);
            ThrowErrors ();
        }

        public static void DeactivateVertexBuffer (OTKVertexBufferHandle handle)
        {
            const BufferTarget type = BufferTarget.ArrayBuffer;
            GL.BindBuffer (type, 0);
            ThrowErrors ();
        }
        
        public static void DestroyVertexBuffer (OTKVertexBufferHandle handle)
        {  
            uint h = handle.GLHandle;
            GL.DeleteBuffers (1, ref h);
            ThrowErrors ();
        }
        
        public static void SetVertexBufferData<T> (OTKVertexBufferHandle handle, T[] data, Int32 startIndex, Int32 elementCount)
        where T
            : struct
            , IVertexType
        {
            if (data.Length != handle.VertexCount)
            {
                throw new Exception ("?");
            }

            ActivateVertexBuffer (handle);
            
            const BufferTarget type = BufferTarget.ArrayBuffer;
            
            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            GL.BufferSubData (
                type,
                (System.IntPtr) (handle.VertDecl.VertexStride * startIndex),
                (System.IntPtr) (handle.VertDecl.VertexStride * elementCount),
                data);

            ThrowErrors ();
        }

        public static void SetVertexBufferRawData (OTKVertexBufferHandle handle, Byte[] data, Int32 startIndex, Int32 elementCount)
        {
            if (data.Length != handle.VertexCount)
            {
                throw new Exception ("?");
            }
            
            ActivateVertexBuffer (handle);
            
            const BufferTarget type = BufferTarget.ArrayBuffer;

            GL.BufferSubData (
                type,
                (System.IntPtr) (handle.VertDecl.VertexStride * startIndex),
                (System.IntPtr) (handle.VertDecl.VertexStride * elementCount),
                data);

            ThrowErrors ();
        }
        
        #endregion
        
        #region Index Buffers
        
        public static OTKIndexBufferHandle CreateIndexBuffer (Int32 indexCount)
        {
            UInt32 glHandle;
            const BufferTarget type = BufferTarget.ElementArrayBuffer;
            const GLBufferUsage bufferUsage = GLBufferUsage.DynamicDraw;
            
            GL.GenBuffers (1, out glHandle);
            var handle = new OTKIndexBufferHandle (glHandle, indexCount);

            ThrowErrors ();

            if (glHandle == 0 )
            {
                throw new Exception ("Failed to generate index buffer.");
            }
            
            ActivateIndexBuffer (handle);

            GL.BufferData (
                type,
                (System.IntPtr) (sizeof (UInt16) * indexCount),
                (System.IntPtr) null,
                bufferUsage);

            ThrowErrors ();
            
            return handle;
        }
        
        public static void ActivateIndexBuffer (OTKIndexBufferHandle handle)
        {
            const BufferTarget type = BufferTarget.ElementArrayBuffer;
            GL.BindBuffer (type, handle.GLHandle);
            ThrowErrors ();
        }

        public static void DeactivateIndexBuffer (OTKIndexBufferHandle handle)
        {
            const BufferTarget type = BufferTarget.ElementArrayBuffer;
            GL.BindBuffer (type, 0);
            ThrowErrors ();
        }
        
        public static void DestroyIndexBuffer (OTKIndexBufferHandle handle)
        {
            uint h = handle.GLHandle;
            GL.DeleteBuffers (1, ref h);
            ThrowErrors ();
        }
        
        public static void SetIndexBufferData (OTKIndexBufferHandle handle, Int32[] data, Int32 startIndex, Int32 elementCount)
        {
            if (data.Length != handle.IndexCount)
            {
                throw new Exception ("?");
            }

            UInt16[] udata = new UInt16[data.Length];

            for (Int32 i = 0; i < data.Length; ++i)
            {
                udata[i] = (UInt16) data[i];
            }

            ActivateIndexBuffer (handle);
            
            const BufferTarget type = BufferTarget.ElementArrayBuffer;

            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            GL.BufferSubData (
                type,
                (System.IntPtr) 0,
                (System.IntPtr) (sizeof (UInt16) * handle.IndexCount),
                udata);

            udata = null;

            ThrowErrors ();
        }
        
        #endregion
        
        #region Textures
        
        public static OTKTextureHandle CreateTexture (TextureFormat textureFormat, Int32 width, Int32 height, Byte[] source)
        {
            if (textureFormat != TextureFormat.Rgba32)
                throw new NotImplementedException ();

            IntPtr pixelDataRgba32 = Marshal.AllocHGlobal (source.Length);
            Marshal.Copy (source, 0, pixelDataRgba32, source.Length);

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

            ThrowErrors ();

            // the first sept in the application of texture is to create the
            // texture object.  this is a container object that holds the
            // texture data.  this function returns a handle to a texture
            // object.
            GL.GenTextures (1, out textureId);
            ThrowErrors ();

            const TextureTarget textureTarget = TextureTarget.Texture2D;

            // we need to bind the texture object so that we can opperate on it.
            GL.BindTexture (textureTarget, textureId);
            ThrowErrors ();

            // the incoming texture format
            // (the format that [pixelDataRgba32] is in)
            const PixelFormat format = PixelFormat.Rgba;

            const PixelInternalFormat internalFormat = PixelInternalFormat.Rgba;

            const PixelType textureDataFormat = PixelType.UnsignedByte;

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

            ThrowErrors ();

            // sets the minification and maginfication filtering modes.  required
            // because we have not loaded a complete mipmap chain for the texture
            // so we must select a non mipmapped minification filter.
            GL.TexParameter (textureTarget, TextureParameterName.TextureMinFilter, (int) All.Nearest);

            ThrowErrors ();

            GL.TexParameter (textureTarget, TextureParameterName.TextureMagFilter, (int) All.Nearest);

            ThrowErrors ();

            return new OTKTextureHandle (textureId);
        }

        public static void DestroyTexture (Handle textureHandle)
        {
            int glTextureId = (textureHandle as OTKTextureHandle).TextureId;

            GL.DeleteTextures (1, ref glTextureId);
        }
        
        public static void ActivateTexture (Int32 slot, Handle tex)
        {
            TextureUnit oglTexSlot = ConvertToOTKTextureSlotEnum (slot);
            GL.ActiveTexture (oglTexSlot);

            var oglt0 = tex as OTKTextureHandle;

            if (oglt0 != null)
            {
                const TextureTarget textureTarget = TextureTarget.Texture2D;

                // we need to bind the texture object so that we can opperate on it.
                GL.BindTexture (textureTarget, oglt0.TextureId);
                ThrowErrors ();
            }
        }
        
        #endregion
        
        #region Shaders
        
        public static OTKShaderHandle CreateShader (ShaderFormat shaderFormat, String vertexShaderSource, )
        {
            Int32 vertexShaderHandle = -1;
            Int32 fragmentShaderHandle = -1;

            OTKWrapper.CompileShader (global::MonoMac.OpenGL.ShaderType.VertexShader, vertexShaderSource, out vertexShaderHandle);
            OTKWrapper.CompileShader (global::MonoMac.OpenGL.ShaderType.FragmentShader, fragmentShaderSource, out fragmentShaderHandle);

            OTKWrapper.AttachShader

            var result = new OTKShaderHandle (
                shaderDefinition,
                platformVariants);

            return result;
        }

        public static void DestroyShader (Handle shaderHandle)
        {
            var handle = shaderHandle as OTKShaderHandle;

            DestroyShaderProgram (handle.ProgramHandle);
        }

        #endregion
        
        #region Drawing
        
        public static void DrawIndexedPrimitives (
            PrimitiveType primitiveType,
            Int32 baseVertex,
            Int32 minVertexIndex,
            Int32 numVertices,
            Int32 startIndex,
            Int32 primitiveCount,
            VertexDeclaration vertDecl)
        {
            if (baseVertex != 0 || minVertexIndex != 0 || startIndex != 0 )
            {
                throw new NotImplementedException ();
            }

            var otkpType = ConvertToOTKBeginModeEnum (primitiveType);
            //Int32 numVertsInPrim = numVertices / primitiveCount;

            Int32 nVertsInPrim = PrimitiveHelper.NumVertsIn (primitiveType);
            Int32 count = primitiveCount * nVertsInPrim;

            EnableVertAttribs (vertDecl, (IntPtr) 0 );

            GL.DrawElements (
                otkpType,
                count,
                DrawElementsType.UnsignedShort,
                (System.IntPtr) 0 );

            ThrowErrors ();

            DisableVertAttribs (vertDecl);
        }

        public static void DrawUserPrimitives <T> (
            PrimitiveType primitiveType,
            T[] vertexData,
            Int32 vertexOffset,
            Int32 primitiveCount,
            VertexDeclaration vertexDeclaration)
        where T 
            : IVertexType
        {
            // do i need to do this? todo: find out
            ActivateVertexBuffer (null);
            ActivateIndexBuffer (null);

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

            var glDrawMode = ConvertToOTKBeginModeEnum (primitiveType);
            var glDrawModeAll = glDrawMode;

            var bindTarget = BufferTarget.ArrayBuffer;

            GL.BindBuffer (bindTarget, 0);
            ThrowErrors ();


            EnableVertAttribs (vertDecl, pointer);

            Int32 nVertsInPrim = PrimitiveHelper.NumVertsIn (primitiveType);
            Int32 count = primitiveCount * nVertsInPrim;

            GL.DrawArrays (
                glDrawModeAll, // specifies the primitive to render
                vertexOffset,  // specifies the starting vertex index in the enabled vertex arrays
                count); // specifies the number of indicies to be drawn

            ThrowErrors ();


            DisableVertAttribs (vertDecl);


            pinnedArray.Free ();
        }
        
        #endregion
        
        #endregion

        #region Private / Internal
        
        [Conditional ("DEBUG")]
        static void ThrowErrors ()
        {
            var ec = GL.GetError ();

            if (ec != ErrorCode.NoError)
            {
                throw new Exception (ec.ToString ());
            }
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
        
        static void EnableVertAttribs (VertexDeclaration vertDecl, IntPtr pointer)
        {
            var vertElems = vertDecl.GetVertexElements ();

            IntPtr ptr = pointer;

            int counter = 0;
            foreach (var elem in vertElems)
            {
                GL.EnableVertexAttribArray (counter);
                ThrowErrors ();

                //var vertElemUsage = elem.VertexElementUsage;
                var vertElemFormat = elem.VertexElementFormat;
                var vertElemOffset = elem.Offset;

                Int32 numComponentsInVertElem = 0;
                Boolean vertElemNormalized = false;
                VertexAttribPointerType glVertElemFormat;

                Convert (vertElemFormat, out glVertElemFormat, out vertElemNormalized, out numComponentsInVertElem);

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

                ThrowErrors ();

                counter++;

            }
        }

        static void DisableVertAttribs (VertexDeclaration vertDecl)
        {
            var vertElems = vertDecl.GetVertexElements ();

            for (int i = 0; i < vertElems.Length; ++i)
            {
                GL.DisableVertexAttribArray (i);
                ThrowErrors ();
            }
        }
        
        #region Enum Converters

        static TextureUnit ConvertToOTKTextureSlotEnum (Int32 slot)
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

        static Type ConvertToType (ActiveAttribType ogl)
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

        static Type ConvertToType (ActiveUniformType ogl)
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

        static void Convert (
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

        static BlendingFactorSrc ConvertToOTKBlendingFactorSrcEnum (BlendFactor blimey)
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

        static BlendingFactorDest ConvertToOTKBlendingFactorDestEnum (BlendFactor blimey)
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

        static BlendFactor ConvertToCorBlendFactorEnum (All ogl)
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

        static BlendEquationMode ConvertToOTKBlendEquationModeEnum (BlendFunction blimey)
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

        static BlendFunction ConvertToCorBlendFunctionEnum (All ogl)
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
        static BeginMode ConvertToOTKBeginModeEnum (PrimitiveType blimey)
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

        static PrimitiveType ConvertToCorPrimitiveTypeEnum (All ogl)
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
        
        #endregion
        
        #region Extensions
        
        static KhronosVector2 ToOTK (this Abacus.SinglePrecision.Vector2 vec)
        {
            return new KhronosVector2 (vec.X, vec.Y);
        }

        static Abacus.SinglePrecision.Vector2 ToAbacus (this KhronosVector2 vec)
        {
            return new Abacus.SinglePrecision.Vector2 (vec.X, vec.Y);
        }
        
        static KhronosVector3 ToOTK (this Abacus.SinglePrecision.Vector3 vec)
        {
            return new KhronosVector3 (vec.X, vec.Y, vec.Z);
        }

        static Abacus.SinglePrecision.Vector3 ToAbacus (this KhronosVector3 vec)
        {
            return new Abacus.SinglePrecision.Vector3 (vec.X, vec.Y, vec.Z);
        }
        
        static KhronosVector4 ToOTK (this Abacus.SinglePrecision.Vector4 vec)
        {
            return new KhronosVector4 (vec.X, vec.Y, vec.Z, vec.W);
        }

        static Abacus.SinglePrecision.Vector4 ToAbacus (this KhronosVector4 vec)
        {
            return new Abacus.SinglePrecision.Vector4 (vec.X, vec.Y, vec.Z, vec.W);
        }
        
        static KhronosMatrix4 ToOTK (this Abacus.SinglePrecision.Matrix44 mat)
        {
            return new KhronosMatrix4(
                mat.R0C0, mat.R0C1, mat.R0C2, mat.R0C3,
                mat.R1C0, mat.R1C1, mat.R1C2, mat.R1C3,
                mat.R2C0, mat.R2C1, mat.R2C2, mat.R2C3,
                mat.R3C0, mat.R3C1, mat.R3C2, mat.R3C3);
        }

        static Abacus.SinglePrecision.Matrix44 ToAbacus (this KhronosMatrix4 mat)
        {
            return new Abacus.SinglePrecision.Matrix44(
                mat.M11, mat.M12, mat.M13, mat.M14,
                mat.M21, mat.M22, mat.M23, mat.M24,
                mat.M31, mat.M32, mat.M33, mat.M34,
                mat.M41, mat.M42, mat.M43, mat.M44);
        }
        
        #endregion
        
        #endregion
    
        #region Shader Helper Functions

        internal static Int32 CreateShaderProgram ()
        {
            // Create shader program.
            Int32 programHandle = GL.CreateProgram ();

            if (programHandle == 0 )
                throw new Exception ("Failed to create shader program");

            ThrowErrors ();

            return programHandle;
        }

        internal static Int32 CreateVertexShader (String source)
        {
            Int32 vertShaderHandle;

            CompileShader (
                GLShaderType.VertexShader,
                source,
                out vertShaderHandle);

            if (vertShaderHandle == 0 )
                throw new Exception ("Failed to compile vertex shader program");

            return vertShaderHandle;
        }

        internal static Int32 CreateFragmentShader (String source)
        {
            Int32 fragShaderHandle;

            CompileShader (
                GLShaderType.FragmentShader,
                source,
                out fragShaderHandle);

            if (fragShaderHandle == 0 )
                throw new Exception ("Failed to compile fragment shader program");


            return fragShaderHandle;
        }

        internal static void AttachShader (
            Int32 programHandle,
            Int32 shaderHandle)
        {
            if (shaderHandle != 0)
            {
                // Attach vertex shader to program.
                GL.AttachShader (programHandle, shaderHandle);
                ThrowErrors ();
            }
        }

        internal static void DetachShader (
            Int32 programHandle,
            Int32 shaderHandle)
        {
            if (shaderHandle != 0)
            {
                GL.DetachShader (programHandle, shaderHandle);
                ThrowErrors ();
            }
        }

        internal static void DeleteShader (
            Int32 programHandle,
            Int32 shaderHandle)
        {
            if (shaderHandle != 0)
            {
                GL.DeleteShader (shaderHandle);
                shaderHandle = 0;
                ThrowErrors ();
            }
        }

        static void DestroyShaderProgram (Int32 programHandle)
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
                    //InternalUtils.Log.Error ("FUCK! (It seems like OpenTK is broken): " + ex.Message);
                }
#endif

                programHandle = 0;
                ThrowErrors ();
            }
        }

        // This should happen offline.
        internal static void CompileShader (
            GLShaderType type,
            String src,
            out Int32 shaderHandle)
        {
            // Create an empty vertex shader object
            shaderHandle = GL.CreateShader (type);

            ThrowErrors ();

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

            ThrowErrors ();

            GL.CompileShader (shaderHandle);

            ThrowErrors ();

#if DEBUG
            Int32 logLength = 0;
            GL.GetShader (
                shaderHandle,
                ShaderParameter.InfoLogLength,
                out logLength);

            ThrowErrors ();
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

                //InternalUtils.Log.Info ("GFX", src);
                //InternalUtils.Log.Info ("GFX", log);
                //InternalUtils.Log.Info ("GFX", type.ToString ());
            }
#endif
            Int32 status = 0;

            GL.GetShader (
                shaderHandle,
                ShaderParameter.CompileStatus,
                out status);

            ThrowErrors ();

            if (status == 0)
            {
                GL.DeleteShader (shaderHandle);
                throw new Exception ("Failed to compile " + type.ToString ());
            }
        }

        internal static List<OTKShaderUniform> GetUniforms (Int32 prog)
        {

            int numActiveUniforms = 0;

            var result = new List<OTKShaderUniform>();

            GL.GetProgram (prog, ProgramParameter.ActiveUniforms, out numActiveUniforms);
            ThrowErrors ();

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
                ThrowErrors ();

                result.Add (
                    new OTKShaderUniform ()
                    {
                    Index = i,
                    Name = sb.ToString (),
                    Type = type
                    }
                );
            }

            return result;
        }

        internal static List<OTKShaderAttribute> GetAttributes (Int32 prog)
        {
            int numActiveAttributes = 0;

            var result = new List<OTKShaderAttribute>();

            // gets the number of active vertex attributes
            GL.GetProgram (prog, ProgramParameter.ActiveAttributes, out numActiveAttributes);
            ThrowErrors ();

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
                ThrowErrors ();

                result.Add (
                    new OTKShaderAttribute ()
                    {
                        Index = i,
                        Name = sb.ToString (),
                        Type = type
                    }
                );
            }

            return result;
        }

        internal static bool LinkProgram (Int32 prog)
        {
            bool retVal = true;

            GL.LinkProgram (prog);

            ThrowErrors ();

#if DEBUG
            Int32 logLength = 0;

            GL.GetProgram (
                prog,
                ProgramParameter.InfoLogLength,
                out logLength);

            ThrowErrors ();

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


                ThrowErrors ();

                //InternalUtils.Log.Info ("GFX", string.Format ("[Cor.Resources] Program link log:\n{0}", infoLog));
            }
#endif
            Int32 status = 0;

            GL.GetProgram (
                prog,
                ProgramParameter.LinkStatus,
                out status);

            ThrowErrors ();

            if (status == 0)
            {
                throw new Exception (String.Format ("Failed to link program: {0:x}", prog));
            }

            return retVal;

        }

        internal static void ValidateProgram (Int32 programHandle)
        {
            GL.ValidateProgram (programHandle);

            ThrowErrors ();

            Int32 logLength = 0;

            GL.GetProgram (
                programHandle,
                ProgramParameter.InfoLogLength,
                out logLength);

            ThrowErrors ();

            if (logLength > 0)
            {
                var infoLog = new System.Text.StringBuilder ();

                GL.GetProgramInfoLog (
                    programHandle,
                    logLength,
                    out logLength, infoLog);

                ThrowErrors ();

                //InternalUtils.Log.Info ("GFX", string.Format ("[Cor.Resources] Program validate log:\n{0}", infoLog));
            }

            Int32 status = 0;

            GL.GetProgram (
                programHandle, ProgramParameter.LinkStatus,
                out status);

            ThrowErrors ();

            if (status == 0)
            {
                throw new Exception (String.Format ("Failed to validate program {0:x}", programHandle));
            }
        }
        
        #endregion
    }

    internal class OTKShaderUniform
    {
        public Int32 Index { get; set; }
        public String Name { get; set; }
        public ActiveUniformType Type { get; set; }
    }

    internal class OTKShaderAttribute
    {
        public Int32 Index { get; set; }
        public String Name { get; set; }
        public ActiveAttribType Type { get; set; }
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal sealed class OTKShaderDefinition
    {
        internal sealed class OTKShaderSourceDefinition
        {
            public string VertexShaderPath { get; set; }
            public string PixelShaderPath { get; set; }
        }
        
        internal sealed class OTKShaderVariantDefinition
        {
            public String VariantName { get; set; }
            public List<OTKShaderVariantPassDefinition> VariantPassDefinitions { get; set; }
        }
    
        internal sealed class OTKShaderVariantPassDefinition
        {
            public String PassName { get; set; }
            public OTKShaderSourceDefinition PassDefinition { get; set; }
        }
        
        public List<OTKShaderVariantDefinition> VariantDefinitions { get; set; }
    }
}
