// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
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
// │ Copyright © 2008-2014 Sungiant ~ http://www.blimey3d.com ~ Authors: A.J.Pook                                   │ \\
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

#if COR_PLATFORM_XIOS
namespace Cor.Platform.Xios
#elif COR_PLATFORM_MONOMAC
namespace Cor.Platform.MonoMac
#else
namespace Cor.Library.OpenTK
#endif
{
    using System;
    using global::System.Text;
    using global::System.Globalization;
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

    using global::MonoMac.OpenGL;
    using GLShaderType = global::MonoMac.OpenGL.ShaderType;
    using GLBufferUsage = global::MonoMac.OpenGL.BufferUsageHint;
    using ActiveUniformType = global::MonoMac.OpenGL.ActiveUniformType;
    using KhronosVector2 = global::MonoMac.OpenGL.Vector2;
    using KhronosVector3 = global::MonoMac.OpenGL.Vector3;
    using KhronosVector4 = global::MonoMac.OpenGL.Vector4;
    using KhronosMatrix4 = global::MonoMac.OpenGL.Matrix4;

#endif

    using Boolean = System.Boolean;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
#if COR_PLATFORM_XIOS
    public partial class XiosApi
#elif COR_PLATFORM_MONOMAC
    public partial class MonoMacApi
#endif
    {
        // Keeping global state around like is rather hacky and should refactored out.
        VertexDeclaration currentActiveVertexBufferVertexDeclaration;
        ShaderHandle currentActiveShaderHandle;
        Int32? currentActiveShaderVariantIndex;

        #region gfx

        public void gfx_ClearColourBuffer (Rgba32 colour)
        {
            Single r, g, b, a;

            colour.UnpackTo (out r, out g, out b, out a);

            GL.ClearColor (r, g, b, a);

            var mask = ClearBufferMask.ColorBufferBit;

            GL.Clear (mask);
            OpenTKHelper.ThrowErrors ();
        }

        public void gfx_ClearDepthBuffer (Single depth)
        {
            GL.ClearDepth (depth);

            var mask = ClearBufferMask.DepthBufferBit;

            GL.Clear (mask);

            OpenTKHelper.ThrowErrors ();
        }

        public void gfx_SetCullMode (CullMode cullMode)
        {
            if (cullMode == CullMode.None)
            {
                GL.Disable (EnableCap.CullFace);
                OpenTKHelper.ThrowErrors ();
            }
            else
            {
                GL.Enable (EnableCap.CullFace);
                OpenTKHelper.ThrowErrors ();

                GL.FrontFace (FrontFaceDirection.Cw);
                OpenTKHelper.ThrowErrors ();

                if (cullMode == CullMode.CW)
                {
                    GL.CullFace (CullFaceMode.Back);
                    OpenTKHelper.ThrowErrors ();
                }
                else if (cullMode == CullMode.CCW)
                {
                    GL.CullFace (CullFaceMode.Front);
                    OpenTKHelper.ThrowErrors ();
                }
                else
                {
                    throw new NotSupportedException ();
                }
            }
        }

        public void gfx_SetBlendEquation (
            BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb, 
            BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha)
        {
            var gl_rgbBlendFunction = OpenTKHelper.ConvertToOpenTKBlendEquationModeEnum (rgbBlendFunction);
            var gl_alphaBlendFunction = OpenTKHelper.ConvertToOpenTKBlendEquationModeEnum (alphaBlendFunction); 

            GL.BlendEquationSeparate (gl_rgbBlendFunction, gl_alphaBlendFunction);
            OpenTKHelper.ThrowErrors ();

            var gl_sourceRgb = OpenTKHelper.ConvertToOpenTKBlendingFactorSrcEnum (sourceRgb);
            var gl_destinationRgb = OpenTKHelper.ConvertToOpenTKBlendingFactorDestEnum (destinationRgb);
            var gl_sourceAlpha = OpenTKHelper.ConvertToOpenTKBlendingFactorSrcEnum (sourceAlpha);
            var gl_destinationAlpha = OpenTKHelper.ConvertToOpenTKBlendingFactorDestEnum (destinationAlpha);

            GL.BlendFuncSeparate (gl_sourceRgb, gl_destinationRgb, gl_sourceAlpha, gl_destinationAlpha);
            OpenTKHelper.ThrowErrors ();
        }


        public Handle gfx_CreateVertexBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount)
        {
            const BufferTarget type = BufferTarget.ArrayBuffer;
            const GLBufferUsage bufferUsage = GLBufferUsage.DynamicDraw;

            UInt32 bufferHandle = 0;
            GL.GenBuffers (1, out bufferHandle);

            OpenTKHelper.ThrowErrors ();

            var handle = new VertexBufferHandle (bufferHandle);

            OpenTkCache.Set<VertexDeclaration> (handle, "VertexDeclaration", vertexDeclaration);
            OpenTkCache.Set<Int32> (handle, "VertexCount", vertexCount);

            if (bufferHandle == 0 )
            {
                throw new Exception ("Failed to generate vert buffer.");
            }

            gfx_vbff_Activate (handle);

            GL.BufferData (
                type,
                (IntPtr) (vertexDeclaration.VertexStride * vertexCount),
                (IntPtr) null,
                bufferUsage);

            OpenTKHelper.ThrowErrors ();

            return handle;
        }

        public Handle gfx_CreateIndexBuffer (Int32 indexCount)
        {
            UInt32 glHandle;
            const BufferTarget type = BufferTarget.ElementArrayBuffer;
            const GLBufferUsage bufferUsage = GLBufferUsage.DynamicDraw;

            GL.GenBuffers (1, out glHandle);

            var handle = new IndexBufferHandle (glHandle);

            OpenTkCache.Set<Int32> (handle, "IndexCount", indexCount);

            OpenTKHelper.ThrowErrors ();

            if (glHandle == 0 )
            {
                throw new Exception ("Failed to generate index buffer.");
            }

            gfx_ibff_Activate (handle);

            GL.BufferData (
                type,
                (IntPtr) (sizeof (UInt16) * indexCount),
                (IntPtr) null,
                bufferUsage);

            OpenTKHelper.ThrowErrors ();

            return handle;
        }

        public Handle gfx_CreateTexture (TextureFormat textureFormat, Int32 width, Int32 height, Byte[] source)
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

            OpenTKHelper.ThrowErrors ();

            // the first sept in the application of texture is to create the
            // texture object.  this is a container object that holds the
            // texture data.  this function returns a handle to a texture
            // object.
            GL.GenTextures (1, out textureId);
            OpenTKHelper.ThrowErrors ();

            const TextureTarget textureTarget = TextureTarget.Texture2D;

            // we need to bind the texture object so that we can opperate on it.
            GL.BindTexture (textureTarget, textureId);
            OpenTKHelper.ThrowErrors ();

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

            OpenTKHelper.ThrowErrors ();

            // sets the minification and maginfication filtering modes.  required
            // because we have not loaded a complete mipmap chain for the texture
            // so we must select a non mipmapped minification filter.
            GL.TexParameter (textureTarget, TextureParameterName.TextureMinFilter, (int) All.Nearest);

            OpenTKHelper.ThrowErrors ();

            GL.TexParameter (textureTarget, TextureParameterName.TextureMagFilter, (int) All.Nearest);

            OpenTKHelper.ThrowErrors ();

            var handle = new TextureHandle (textureId);

            OpenTkCache.Set <Int32> (handle, "Width", width);
            OpenTkCache.Set <Int32> (handle, "Height", height);
            OpenTkCache.Set <TextureFormat> (handle, "TextureFormat", textureFormat);

            return handle;
        }

        public Handle gfx_CreateShader (ShaderDeclaration shaderDeclaration, ShaderFormat shaderFormat, Byte[] source)
        {
            if (shaderFormat != ShaderFormat.GLSL && shaderFormat != ShaderFormat.GLSL_ES)
                throw new NotSupportedException ();

            var variantHandles = new List<ShaderVariantHandle> ();
            var variantIdentifiers = new List<String> ();

            using (var memStream = new MemoryStream (source))
            {
                using (var binReader = new BinaryReader (memStream))
                {
                    Byte variantCount = binReader.ReadByte ();

                    for (Int32 i = 0; i < (Int32)variantCount; ++i)
                    {
                        Int32 variantByteCount = binReader.ReadInt32 ();
                        Byte[] variantEncoded = binReader.ReadBytes (variantByteCount);

                        String corShaderSource = Encoding.ASCII.GetString (variantEncoded);
                        // Noddy parser for the custom GLSL shader format that combines both
                        // the vertex and fragment shaders.
                        String identifier = "";
                        String vertexShaderSource = "";
                        String fragmentShaderSource = "";
                        char state = (char)0;
                        foreach (var line in corShaderSource.Split ('\n'))
                        {
                            if (line == "=VSH=")
                            {
                                state = (char)1;
                                continue;
                            }
                            if (line == "=FSH=")
                            {
                                state = (char)2;
                                continue;
                            }
                            if (state == 0)
                                identifier = line;
                            if (state == 1)
                                vertexShaderSource += line + "\n";
                            if (state == 2)
                                fragmentShaderSource += line + "\n";
                        }

                        var variantHandle = OpenTKHelper.CreateInternalShaderVariant (identifier, vertexShaderSource, fragmentShaderSource);
                        variantHandles.Add (variantHandle);
                        variantIdentifiers.Add (identifier);
                    }
                }
            }

            // Link
            for (int i = 0; i < variantHandles.Count; ++i)
            {
                var variantHandle = variantHandles [i];

                InternalUtils.Log.Info ("gfx_CreateShader", variantIdentifiers [i] + ": Linking");

                int index = 0;
                shaderDeclaration
                    .InputDeclarations
                    .Select (x => x.Name)
                    .ToList () // ordered attributes as per declaration
                    .ForEach (attName => {
                        // https://www.khronos.org/opengles/sdk/docs/man/xhtml/glBindAttribLocation.xml
                        GL.BindAttribLocation (variantHandle.ProgramHandle, index, attName);
                        OpenTKHelper.ThrowErrors ();
                        // incremental linkage
                        Boolean success = OpenTKHelper.LinkProgram (variantHandle.ProgramHandle);
                        // Only increment the index if we managed to link.  The shader may not implement
                        // all of the inputs in the shader declaration.  Cor requires the subset of inputs that the
                        // shader implements have sequential indices, starting at zero, ordered in the same way as the
                        // shader declaration.
                        if (success)
                            index++;
                    });


                InternalUtils.Log.Info ("gfx_CreateShader", variantIdentifiers [i] + ": Validating shader program");
#if DEBUG
                OpenTKHelper.ValidateProgram (variantHandle.ProgramHandle);
#endif

                InternalUtils.Log.Info ("gfx_CreateShader", variantIdentifiers [i] + ": Detaching frag & vert sources");
                OpenTKHelper.DetachShader (variantHandle.ProgramHandle, variantHandle.FragmentShaderHandle);
                OpenTKHelper.DetachShader (variantHandle.ProgramHandle, variantHandle.VertexShaderHandle);

                InternalUtils.Log.Info ("gfx_CreateShader", variantIdentifiers [i] + ": Deleting frag & vert sources");
                OpenTKHelper.DeleteShader (variantHandle.ProgramHandle,  variantHandle.FragmentShaderHandle);
                OpenTKHelper.DeleteShader (variantHandle.ProgramHandle, variantHandle.VertexShaderHandle);
            }

            var handle = new ShaderHandle (variantHandles);

            for (Int32 i = 0; i < variantIdentifiers.Count; ++i)
            {
                OpenTkCache.Set<String> (handle, "VariantIdentifier" + i, variantIdentifiers [i]);
            }

            return handle;
        }

        public void gfx_DestroyVertexBuffer (Handle handle)
        {
            OpenTkCache.Clear (handle);
            uint h = (handle as VertexBufferHandle).GLHandle;
            GL.DeleteBuffers (1, ref h);
            OpenTKHelper.ThrowErrors ();
        }

        public void gfx_DestroyIndexBuffer (Handle handle)
        {
            OpenTkCache.Clear (handle);
            uint h = (handle as IndexBufferHandle).GLHandle;
            GL.DeleteBuffers (1, ref h);
            OpenTKHelper.ThrowErrors ();
        }

        public void gfx_DestroyTexture (Handle handle)
        {
            OpenTKHelper.DestroyTexture (handle);
        }

        public void gfx_DestroyShader (Handle handle)
        {
            Console.WriteLine ("");
            foreach (var variantHandle in (handle as ShaderHandle).VariantHandles)
            {
                OpenTKHelper.DestroyShaderProgram (variantHandle.ProgramHandle);
            }
            Console.WriteLine ("");
        }

        public void gfx_vbff_Activate (Handle handle)
        {
            const BufferTarget type = BufferTarget.ArrayBuffer;

            currentActiveVertexBufferVertexDeclaration = null;
            if (handle == null)
            {
                GL.BindBuffer (type, 0);
                OpenTKHelper.ThrowErrors ();
                return;
            }
            var vd = OpenTkCache.Get <VertexDeclaration> (handle, "VertexDeclaration");

            // Keep track of this for later draw calls that do not provide it.
            currentActiveVertexBufferVertexDeclaration = vd;

            GL.BindBuffer (type, (handle as VertexBufferHandle).GLHandle);
            OpenTKHelper.ThrowErrors ();
        }

        public void gfx_ibff_Activate (Handle handle)
        {
            const BufferTarget type = BufferTarget.ElementArrayBuffer;
            if (handle == null)
            {
                GL.BindBuffer (type, 0);
                OpenTKHelper.ThrowErrors ();
                return;
            }

            GL.BindBuffer (type, (handle as IndexBufferHandle).GLHandle);
            OpenTKHelper.ThrowErrors ();
        }

        public void gfx_DrawPrimitives (
            PrimitiveType primitiveType,
            Int32 startVertex,
            Int32 primitiveCount)
        {
            throw new NotImplementedException ();
        }

        public void gfx_DrawIndexedPrimitives (
            PrimitiveType primitiveType, 
            Int32 baseVertex, 
            Int32 minVertexIndex,
            Int32 numVertices, 
            Int32 startIndex, 
            Int32 primitiveCount)
        {
            if (baseVertex != 0 || minVertexIndex != 0 || startIndex != 0)
            {
                throw new NotImplementedException ();
            }

            var otkpType = OpenTKHelper.ConvertToOpenTKBeginModeEnum (primitiveType);
            //Int32 numVertsInPrim = numVertices / primitiveCount;

            Int32 nVertsInPrim = PrimitiveHelper.NumVertsIn (primitiveType);
            Int32 count = primitiveCount * nVertsInPrim;

            GL.DrawElements (
                otkpType,
                count,
                DrawElementsType.UnsignedShort,
                (IntPtr) 0 );

            OpenTKHelper.ThrowErrors ();
        }

        public void gfx_DrawUserPrimitives <T> (
            PrimitiveType primitiveType, 
            T[] vertexData, 
            Int32 vertexOffset,
            Int32 primitiveCount) 
        where T
            : struct
            , IVertexType
        {
            // do i need to do this? todo: find out
            //gfx_vbff_Activate (null);
            //gfx_ibff_Activate (null);

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
                pointer = OpenTKHelper.Add (pointer, vertexOffset * vertDecl.VertexStride * sizeof (byte));
            }

            __bindVertices (vertDecl, null, pointer);

            var glDrawMode = OpenTKHelper.ConvertToOpenTKBeginModeEnum (primitiveType);
            var glDrawModeAll = glDrawMode;

            var bindTarget = BufferTarget.ArrayBuffer;

            GL.BindBuffer (bindTarget, 0);
            OpenTKHelper.ThrowErrors ();


            Int32 nVertsInPrim = PrimitiveHelper.NumVertsIn (primitiveType);
            Int32 count = primitiveCount * nVertsInPrim;

            GL.DrawArrays (
                glDrawModeAll, // specifies the primitive to render
                vertexOffset,  // specifies the starting vertex index in the enabled vertex arrays
                count); // specifies the number of indicies to be drawn

            OpenTKHelper.ThrowErrors ();

            pinnedArray.Free ();
        }

        public void gfx_DrawUserIndexedPrimitives <T> (
            PrimitiveType primitiveType, 
            T[] vertexData, 
            Int32 vertexOffset, 
            Int32 numVertices, 
            Int32[] indexData, 
            Int32 indexOffset, 
            Int32 primitiveCount) 
        where T
            : struct
            , IVertexType
        {
            throw new NotImplementedException ();
        }

        public Byte[] gfx_CompileShader (String source)
        {
            throw new NotImplementedException ();
        }

        public Int32 gfx_dbg_BeginEvent (Rgba32 colour, String eventName)
        {
            //throw new NotImplementedException ();
            return 0;
        }

        public Int32 gfx_dbg_EndEvent ()
        {
            //throw new NotImplementedException ();
            return 0;
        }

        public void gfx_dbg_SetMarker (Rgba32 colour, String marker)
        {
            //throw new NotImplementedException ();
        }

        public void gfx_dbg_SetRegion (Rgba32 colour, String region)
        {
            //throw new NotImplementedException ();
        }

        public Int32 gfx_vbff_GetVertexCount (Handle h)
        {
            return OpenTkCache.Get <Int32> (h, "VertexCount");
        }

        public VertexDeclaration gfx_vbff_GetVertexDeclaration (Handle h)
        {
            return OpenTkCache.Get <VertexDeclaration> (h, "VertexDeclaration");
        }

        public void gfx_vbff_SetData<T> (Handle h, T[] data, Int32 startIndex, Int32 elementCount) 
            where T
            : struct
            , IVertexType
        {
            Int32 vertexCount = OpenTkCache.Get <Int32> (h, "VertexCount");
            if (data.Length != vertexCount)
            {
                throw new Exception ("?");
            }

            gfx_vbff_Activate (h);

            const BufferTarget type = BufferTarget.ArrayBuffer;
            var vd = OpenTkCache.Get <VertexDeclaration> (h, "VertexDeclaration");

            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            GL.BufferSubData (
                type,
                (IntPtr) (vd.VertexStride * startIndex),
                (IntPtr) (vd.VertexStride * elementCount),
                data);

            OpenTKHelper.ThrowErrors ();
        }

        [Obsolete]
        public void gfx_vbff_SetData (Handle handle, Byte[] data, Int32 startIndex, Int32 elementCount)
        {
            Int32 vertexCount = OpenTkCache.Get <Int32> (handle, "VertexCount");
            if (data.Length != vertexCount)
            {
                throw new Exception ("?");
            }

            gfx_vbff_Activate (handle);

            const BufferTarget type = BufferTarget.ArrayBuffer;
            var vd = OpenTkCache.Get <VertexDeclaration> (handle, "VertexDeclaration");

            GL.BufferSubData (
                type,
                (IntPtr) (vd.VertexStride * startIndex),
                (IntPtr) (vd.VertexStride * elementCount),
                data);

            OpenTKHelper.ThrowErrors ();
        }

        public T[] gfx_vbff_GetData<T> (Handle h, Int32 startIndex, Int32 elementCount)
            where T
            : struct
            , IVertexType
        {
            throw new NotImplementedException ();
        }

        //elementIndicesToActivate
        // ~ The order of this array maps to the order of the variables in the shader
        // ~ The elements themselves are Vertex Element indices.
        public void gfx_vbff_Bind (VertexDeclaration vd, Int32[] elementIndicesToActivate)
        {
            __bindVertices (vd, elementIndicesToActivate, (IntPtr) 0);
        }

        internal void __bindVertices (VertexDeclaration vd, Int32[] elementIndicesToActivate, IntPtr ptr)
        {
            // Clear the custom vertex array
            // this is rather hacky...
            const Int32 max = 8;
            if (elementIndicesToActivate != null && elementIndicesToActivate.Length > max)
            {
                throw new NotImplementedException ();
            }
            for(Int32 i = 0; i < max; ++i)
            {
                GL.DisableVertexAttribArray (i);
                OpenTKHelper.ThrowErrors ();
            }
            // TODO: Find a better way to do this.
            if (vd == null)
                return;

            // https://www.khronos.org/opengles/sdk/docs/man/xhtml/glVertexAttribPointer.xmlbindd
            var vertElems = vd.GetVertexElements ();

            if (elementIndicesToActivate == null)
                elementIndicesToActivate = vertElems.Select ((x, i) => i).ToArray ();

            for(Int32 i = 0; i < elementIndicesToActivate.Length; ++i)
            {
                Int32 vertIndex = elementIndicesToActivate [i];
                Int32 attribIndex = i;

                // https://www.khronos.org/opengles/sdk/docs/man/xhtml/glEnableVertexAttribArray.xml
                GL.EnableVertexAttribArray (attribIndex);
                OpenTKHelper.ThrowErrors ();

                var vertElemUsage = vertElems [vertIndex].VertexElementUsage;
                var vertElemFormat = vertElems [vertIndex].VertexElementFormat;
                var vertElemOffset = vertElems [vertIndex].Offset;

                Int32 numComponentsInVertElem = 0;
                Boolean vertElemNormalized = false;
                VertexAttribPointerType glVertElemFormat;

                OpenTKHelper.Convert (vertElemFormat, out glVertElemFormat, out vertElemNormalized, out numComponentsInVertElem);

                IntPtr ptr2 = OpenTKHelper.Add (ptr, vertElemOffset);

                // https://www.khronos.org/opengles/sdk/docs/man/xhtml/glVertexAttribPointer.xml
                GL.VertexAttribPointer (
                    // index
                    // specifies the generic vertex attribute index.  This value is 0 to
                    // max vertex attributes supported - 1.
                    attribIndex,
                    // size
                    // number of components specified in the vertex array for the
                    // vertex attribute referenced by index.  Valid values are 1 - 4.
                    numComponentsInVertElem,
                    // type
                    // Data format, valid values are GL_BYTE, GL_UNSIGNED_BYTE, GL_SHORT, GL_UNSIGNED_SHORT,
                    // GL_FLOAT, GL_FIXED, GL_HALF_FLOAT_OES*(Optional feature of es2)
                    glVertElemFormat,
                    // normalised
                    // used to indicate whether the non-floating data format type should be normalised
                    // or not when converted to floating point.
                    vertElemNormalized,
                    // stride
                    // the components of vertex attribute specified by size are stored sequentially for each
                    // vertex.  stride specifies the delta between data for vertex index i and vertex (i + 1).
                    // If stride is 0, attribute data for all vertices are stored sequentially.
                    // If stride is > 0, then we use the stride valude tas the pitch to get vertex data
                    // for the next index.
                    vd.VertexStride,
                    // offset into the vert data
                    ptr2
                );

                OpenTKHelper.ThrowErrors ();
            }
        }

        public Int32 gfx_ibff_GetIndexCount (Handle h)
        {
            return OpenTkCache.Get <Int32> (h, "IndexCount");
        }

        public void gfx_ibff_SetData (Handle h, Int32[] data, Int32 startIndex, Int32 elementCount)
        {
            Int32 indexCount = OpenTkCache.Get <Int32> (h, "IndexCount");

            if (data.Length != indexCount)
            {
                throw new Exception ("?");
            }

            UInt16[] udata = new UInt16[data.Length];

            for (Int32 i = 0; i < data.Length; ++i)
            {
                udata[i] = (UInt16) data[i];
            }

            gfx_ibff_Activate (h);

            const BufferTarget type = BufferTarget.ElementArrayBuffer;

            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            GL.BufferSubData (
                type,
                (IntPtr) 0,
                (IntPtr) (sizeof (UInt16) * indexCount),
                udata);

            udata = null;

            OpenTKHelper.ThrowErrors ();
        }

        public void gfx_ibff_GetData (Handle h, Int32[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException ();
        }

        public Int32 gfx_tex_GetWidth (Handle h)
        {
            return OpenTkCache.Get <Int32> (h, "Width");
        }

        public Int32 gfx_tex_GetHeight (Handle h)
        {
            return OpenTkCache.Get <Int32> (h, "Height");
        }

        public Byte[] gfx_tex_GetData (Handle h)
        {
            throw new NotImplementedException ();
        }

        public TextureFormat gfx_tex_GetTextureFormat (Handle h)
        {
            return OpenTkCache.Get <TextureFormat> (h, "TextureFormat");
        }

        public void gfx_shdr_SetVariable<T> (Handle h, Int32 variantIndex, String name, T value)
        {
            // todo: find a better way, this will be slow
            var uniforms = OpenTKHelper.GetUniforms ((h as ShaderHandle).VariantHandles[variantIndex].ProgramHandle);
            var uniform = uniforms.ToList ().Find (x => x.Name == name);

            //todo this should be using convert turn the data into proper opengl es types.
            Type t = value.GetType();
            Object tx = value;

            if( t == typeof(Matrix44) )
            {
                var castValue = (Matrix44) tx;
                var otkValue = castValue.ToOpenTK();
                GL.UniformMatrix4( uniform.Location, false, ref otkValue );
            }
            else if( t == typeof(Int32) )
            {
                var castValue = (Int32) tx;
                GL.Uniform1( uniform.Location, 1, ref castValue );
            }
            else if( t == typeof(Single) )
            {
                var castValue = (Single) tx;
                GL.Uniform1( uniform.Location, 1, ref castValue );
            }
            else if( t == typeof(Abacus.SinglePrecision.Vector2) )
            {
                var castValue = (Abacus.SinglePrecision.Vector2) tx;
                GL.Uniform2( uniform.Location, 1, ref castValue.X );
            }
            else if( t == typeof(Abacus.SinglePrecision.Vector3) )
            {
                var castValue = (Abacus.SinglePrecision.Vector3) tx;
                GL.Uniform3( uniform.Location, 1, ref castValue.X );
            }
            else if( t == typeof(Abacus.SinglePrecision.Vector4) )
            {
                var castValue = (Abacus.SinglePrecision.Vector4) tx;
                GL.Uniform4( uniform.Location, 1, ref castValue.X );
            }
            else if( t == typeof(Rgba32) )
            {
                var castValue = (Rgba32) tx;

                Abacus.SinglePrecision.Vector4 vec4Value;
                castValue.UnpackTo(out vec4Value.X, out vec4Value.Y, out vec4Value.Z, out vec4Value.W);

                // does this rgba value need to be packed in to a vector3 or a vector4
                if( OpenTKHelper.ConvertToType(uniform.Type) == typeof(Abacus.SinglePrecision.Vector4) )
                    GL.Uniform4( uniform.Location, 1, ref vec4Value.X );
                else if( OpenTKHelper.ConvertToType(uniform.Type) == typeof(Abacus.SinglePrecision.Vector3) )
                    GL.Uniform3( uniform.Location, 1, ref vec4Value.X );
                else
                    throw new Exception("Not supported");
            }
            else
            {
                throw new Exception("Not supported");
            }

            OpenTKHelper.ThrowErrors();
        }

        public void gfx_shdr_SetSampler (Handle h, Int32 variantIndex, String name, Int32 slot)
        {
            var uniforms = OpenTKHelper.GetUniforms ((h as ShaderHandle).VariantHandles[variantIndex].ProgramHandle);
            var uniform = uniforms.ToList ().Find (x => x.Name == name);

            // set the sampler texture unit to 0
            GL.Uniform1( uniform.Location, slot );
            OpenTKHelper.ThrowErrors();
        }

        public void gfx_shdr_Activate (Handle h, Int32 variantIndex)
        {
            currentActiveShaderHandle = (h as ShaderHandle);
            currentActiveShaderVariantIndex = variantIndex;
            GL.UseProgram (currentActiveShaderHandle.VariantHandles [variantIndex].ProgramHandle);
            OpenTKHelper.ThrowErrors ();
        }

        public Int32 gfx_shdr_GetVariantCount (Handle h)
        {
            return (h as ShaderHandle).VariantHandles.Count;
        }

        public String gfx_shdr_GetIdentifier (Handle h, Int32 variantIndex)
        {
            return OpenTkCache.Get<String> (h, "VariantIdentifier" + variantIndex);
        }

        public ShaderInputInfo[] gfx_shdr_GetInputs (Handle h, Int32 variantIndex)
        {
            string id = gfx_shdr_GetIdentifier (h, variantIndex);
            InternalUtils.Log.Info ("gfx_shdr_GetInputs", id + ": Initilise Attributes");
            var attributes = OpenTKHelper.GetAttributes ((h as ShaderHandle).VariantHandles[variantIndex].ProgramHandle);

            var inputs = attributes
                .OrderBy (y => y.Location)
                .Select ((x, i) => new ShaderInputInfo {
                    Index = i,
                    Name = x.Name,
                    Type = OpenTKHelper.ConvertToType (x.Type) })
                .ToArray ();

            String logInputs = id + ": Inputs ~ ";
            foreach (var input in inputs)
            {
                logInputs += input.Name + ", ";
            }
            InternalUtils.Log.Info ("gfx_shdr_GetInputs", logInputs);

            return inputs;
        }

        public ShaderVariableInfo[] gfx_shdr_GetVariables (Handle h, Int32 variantIndex)
        {
            string id = gfx_shdr_GetIdentifier (h, variantIndex);
            InternalUtils.Log.Info ("gfx_shdr_GetVariables", id + ": Initilise Uniforms");

            var uniforms = OpenTKHelper.GetUniforms ((h as ShaderHandle).VariantHandles[variantIndex].ProgramHandle);

            var variables = uniforms
                .Where (y =>
                    y.Type != ActiveUniformType.Sampler2D &&
                    y.Type != ActiveUniformType.SamplerCube)
                .OrderBy (z => z.Location)
                .Select ((x, i) => new ShaderVariableInfo {
                    Index = i,
                    Name = x.Name,
                    Type = OpenTKHelper.ConvertToType (x.Type) })
                .ToArray ();

            String logVars = id + ": Variables ~ ";
            foreach(var variable in variables)
            {
                logVars += variable.Name + ", ";
            }
            InternalUtils.Log.Info ("gfx_shdr_GetVariables", logVars);

            return variables;
        }

        public ShaderSamplerInfo[] gfx_shdr_GetSamplers (Handle h, Int32 variantIndex)
        {
            string id = gfx_shdr_GetIdentifier (h, variantIndex);
            InternalUtils.Log.Info ("gfx_shdr_GetSamplers", id + ": Initilise Samplers");

            var uniforms = OpenTKHelper.GetUniforms ((h as ShaderHandle).VariantHandles[variantIndex].ProgramHandle);

            var samplers = uniforms
                .Where (y =>
                    y.Type == ActiveUniformType.Sampler2D ||
                    y.Type == ActiveUniformType.SamplerCube)
                .OrderBy (z => z.Location)
                .Select ((x, i) => new ShaderSamplerInfo { Index = i, Name = x.Name })
                .ToArray ();

            String logVars = id + ": Samplers : ";
            foreach(var sampler in samplers)
            {
                logVars += sampler.Name + ", ";
            }
            InternalUtils.Log.Info ("gfx_shdr_GetSamplers", logVars);

            return samplers;
        }

        public void gfx_tex_Activate (Handle h, int slot)
        {
            const TextureTarget textureTarget = TextureTarget.Texture2D;

            if (h == null)
            {
                GL.BindTexture (textureTarget, 0);
                OpenTKHelper.ThrowErrors ();
            }
            else
            {
                TextureUnit oglTexSlot = OpenTKHelper.ConvertToOpenTKTextureSlotEnum (slot);

                GL.ActiveTexture (oglTexSlot);
                OpenTKHelper.ThrowErrors ();
                var oglt0 = h as TextureHandle;

                // we need to bind the texture object so that we can opperate on it.
                GL.BindTexture (textureTarget, oglt0.TextureId );
                OpenTKHelper.ThrowErrors ();
            }
        }

        public int[] gfx_ibff_GetData (Handle indexBufferHandle, int startIndex, int elementCount)
        {
            throw new NotImplementedException ();
        }

        #endregion

    }

    internal static class OpenTKHelper
    {
        internal static void InitilizeRenderSettings ()
        {
            GL.Enable (EnableCap.Blend);
            OpenTKHelper.ThrowErrors ();

            GL.Enable (EnableCap.DepthTest);
            OpenTKHelper.ThrowErrors ();

            GL.DepthMask (true);
            OpenTKHelper.ThrowErrors ();

            GL.DepthRange (0f, 1f);
            OpenTKHelper.ThrowErrors ();

            GL.DepthFunc (DepthFunction.Lequal);
            OpenTKHelper.ThrowErrors ();
           
            #if COR_PLATFORM_XIOS

            #elif COR_PLATFORM_MONOMAC

            GL.Enable (EnableCap.Texture2D);
            OpenTKHelper.ThrowErrors ();

            // Enables Smooth Shading
            GL.ShadeModel (ShadingModel.Smooth);
            OpenTKHelper.ThrowErrors ();

            // Setup Depth Testing
            // Really Nice Perspective Calculations
            GL.Hint (HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            OpenTKHelper.ThrowErrors ();
            #endif
        }

        internal static void InitilizeRenderTargets (Int32 width, Int32 height)
        {
            Int32 renderbuffer;
            GL.GenRenderbuffers (1, out renderbuffer);
            OpenTKHelper.ThrowErrors ();

            GL.BindRenderbuffer (RenderbufferTarget.Renderbuffer, renderbuffer);
            OpenTKHelper.ThrowErrors ();

#if COR_PLATFORM_XIOS

            GL.RenderbufferStorage (
            RenderbufferTarget.Renderbuffer,
            RenderbufferInternalFormat.DepthComponent16,
            width,
            height);
            OpenTKHelper.ThrowErrors ();

            GL.FramebufferRenderbuffer (
            FramebufferTarget.Framebuffer,
            FramebufferSlot.DepthAttachment,
            RenderbufferTarget.Renderbuffer,
            renderbuffer);
            OpenTKHelper.ThrowErrors ();

#elif COR_PLATFORM_MONOMAC

            GL.RenderbufferStorage (
                RenderbufferTarget.Renderbuffer,
                RenderbufferStorage.DepthComponent32,
                width,
                height);
            OpenTKHelper.ThrowErrors ();

            GL.FramebufferRenderbuffer (
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer,
                renderbuffer);
            OpenTKHelper.ThrowErrors ();
#endif
        }


        [Conditional ("DEBUG")]
        public static void ThrowErrors ()
        {
#if COR_PLATFORM_MONOMAC
            var ec = GL.GetError ();
#elif COR_PLATFORM_XIOS
            var ec = GL.GetErrorCode ();
#endif

            if (ec != ErrorCode.NoError)
            {
                throw new Exception (ec.ToString ());
            }
        }

        [ReliabilityContract (Consistency.MayCorruptInstance, Cer.MayFail)]
        public static IntPtr Add (IntPtr pointer, int offset)
        {
            unsafe
            {
                return (IntPtr) (unchecked (((byte *) pointer) + offset));
            }
        }

        [ReliabilityContract (Consistency.MayCorruptInstance, Cer.MayFail)]
        public static IntPtr Subtract (IntPtr pointer, int offset)
        {
            unsafe
            {
                return (IntPtr) (unchecked (((byte *) pointer) - offset));
            }
        }

        public static TextureUnit ConvertToOpenTKTextureSlotEnum (Int32 slot)
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

        public static Type ConvertToType (ActiveAttribType ogl)
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

        public static Type ConvertToType (ActiveUniformType ogl)
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

        public static void Convert (VertexElementFormat blimey, out VertexAttribPointerType dataFormat, out bool normalized, out int size)
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

        public static BlendingFactorSrc ConvertToOpenTKBlendingFactorSrcEnum (BlendFactor blimey)
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

        public static BlendingFactorDest ConvertToOpenTKBlendingFactorDestEnum (BlendFactor blimey)
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

        public static BlendFactor ConvertToCorBlendFactorEnum (All ogl)
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

        public static BlendEquationMode ConvertToOpenTKBlendEquationModeEnum (BlendFunction blimey)
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

        public static BlendFunction ConvertToCorBlendFunctionEnum (All ogl)
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

        public static BeginMode ConvertToOpenTKBeginModeEnum (PrimitiveType blimey)
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

        public static PrimitiveType ConvertToCorPrimitiveTypeEnum (All ogl)
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

        public static Int32 CreateShaderProgram ()
        {
            // Create shader program.
            Int32 programHandle = GL.CreateProgram ();

            if (programHandle == 0 )
                throw new Exception ("Failed to create shader program");

            OpenTKHelper.ThrowErrors ();

            return programHandle;
        }

        public static Int32 CreateVertexShader (String source)
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

        public static Int32 CreateFragmentShader (String source)
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

        public static void AttachShader (Int32 programHandle, Int32 shaderHandle)
        {
            if (shaderHandle != 0)
            {
                // Attach vertex shader to program.
                GL.AttachShader (programHandle, shaderHandle);
                OpenTKHelper.ThrowErrors ();
            }
        }

        public static void DetachShader (Int32 programHandle, Int32 shaderHandle)
        {
            if (shaderHandle != 0)
            {
                GL.DetachShader (programHandle, shaderHandle);
                OpenTKHelper.ThrowErrors ();
            }
        }

        public static void DeleteShader (Int32 programHandle, Int32 shaderHandle)
        {
            if (shaderHandle != 0)
            {
                GL.DeleteShader (shaderHandle);
                shaderHandle = 0;
                OpenTKHelper.ThrowErrors ();
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
                    // TODO: ...
                    InternalUtils.Log.Error ("FUCK! (It seems like OpenTK is broken): " + ex.Message);
                }
#endif

                programHandle = 0;
                OpenTKHelper.ThrowErrors ();
            }
        }

        public static void CompileShader (GLShaderType type, String src, out Int32 shaderHandle)
        {
            // Create an empty vertex shader object
            shaderHandle = GL.CreateShader (type);

            OpenTKHelper.ThrowErrors ();

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

            OpenTKHelper.ThrowErrors ();

            GL.CompileShader (shaderHandle);

            OpenTKHelper.ThrowErrors ();

#if DEBUG
            Int32 logLength = 0;
            GL.GetShader (
                shaderHandle,
                ShaderParameter.InfoLogLength,
                out logLength);

            OpenTKHelper.ThrowErrors ();
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
                InternalUtils.Log.Info ("GFX", log);
                //InternalUtils.Log.Info ("GFX", type.ToString ());
            }
#endif
            Int32 status = 0;

            GL.GetShader (
                shaderHandle,
                ShaderParameter.CompileStatus,
                out status);

            OpenTKHelper.ThrowErrors ();

            if (status == 0)
            {
                GL.DeleteShader (shaderHandle);
                throw new Exception ("Failed to compile " + type.ToString ());
            }
        }

        public static List<OTKShaderUniform> GetUniforms (Int32 prog)
        {
            int numActiveUniforms = 0;

            var result = new List<OTKShaderUniform>();

            GL.GetProgram (prog, ProgramParameter.ActiveUniforms, out numActiveUniforms);
            OpenTKHelper.ThrowErrors ();

            for (int i = 0; i < numActiveUniforms; ++i)
            {
                var sb = new StringBuilder ();

                //int buffSize = 0;
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
                OpenTKHelper.ThrowErrors ();

                string name = sb.ToString ();

                int uniformLocation = GL.GetUniformLocation (prog, name);

                OpenTKHelper.ThrowErrors ();

                if (uniformLocation == -1 )
                    throw new Exception ();

                result.Add (
                    new OTKShaderUniform {
                        Index = i,
                        Name = name,
                        Type = type,
                        Location = uniformLocation });



                /*
            InternalUtils.Log.Info ("GFX",
                String.Format (
                "    Caching Reference to Shader Variable: [Prog={0}, UniIndex={1}, UniLocation={2}, UniName={3}, UniType={4}]",
                programHandle,
                uniform.Index,
                uniformLocation,
                uniform.Name,
                uniform.Type));
                */

            }

            return result;
        }

        public static List<OTKShaderAttribute> GetAttributes (Int32 prog)
        {
            int numActiveAttributes = 0;

            var result = new List<OTKShaderAttribute>();

            // gets the number of active vertex attributes
            GL.GetProgram (prog, ProgramParameter.ActiveAttributes, out numActiveAttributes);
            OpenTKHelper.ThrowErrors ();

            for (int i = 0; i < numActiveAttributes; ++i)
            {
                var sb = new StringBuilder ();

                //int buffSize = 0;
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
                OpenTKHelper.ThrowErrors ();

                string name = sb.ToString ();

                int attLocation = GL.GetAttribLocation (prog, name);

                OpenTKHelper.ThrowErrors ();

                result.Add (
                    new OTKShaderAttribute
                    {
                        Index = i,
                        Name = name,
                        Type = type,
                        Location = attLocation
                    }
                );
            }

            return result;
        }

        public static bool LinkProgram (Int32 prog)
        {
            bool retVal = true;

            GL.LinkProgram (prog);
            OpenTKHelper.ThrowErrors ();

#if DEBUG
            Int32 logLength = 0;

            GL.GetProgram ( prog, ProgramParameter.InfoLogLength, out logLength);
            OpenTKHelper.ThrowErrors ();

            if (logLength > 0)
            {
                retVal = false;
                var infoLog = string.Empty;
                GL.GetProgramInfoLog (prog, out infoLog);
                OpenTKHelper.ThrowErrors ();
                InternalUtils.Log.Info ("GFX", string.Format ("Program link log:\n{0}", infoLog));
            }
#endif
            Int32 status = 0;

            GL.GetProgram (prog, ProgramParameter.LinkStatus, out status);
            OpenTKHelper.ThrowErrors ();

            if (status == 0)
            {
                throw new Exception (String.Format ("Failed to link program: {0:x}", prog));
            }

            return retVal;

        }

        public static void ValidateProgram (Int32 programHandle)
        {
            GL.ValidateProgram (programHandle);

            OpenTKHelper.ThrowErrors ();

            Int32 logLength = 0;

            GL.GetProgram (
                programHandle,
                ProgramParameter.InfoLogLength,
                out logLength);

            OpenTKHelper.ThrowErrors ();

            if (logLength > 0)
            {
                var infoLog = new StringBuilder ();

                GL.GetProgramInfoLog (
                    programHandle,
                    logLength,
                    out logLength, infoLog);

                OpenTKHelper.ThrowErrors ();

                //InternalUtils.Log.Info ("GFX", string.Format ("[Cor.Resources] Program validate log:\n{0}", infoLog));
            }

            Int32 status = 0;

            GL.GetProgram (
                programHandle, ProgramParameter.LinkStatus,
                out status);

            OpenTKHelper.ThrowErrors ();

            if (status == 0)
            {
                throw new Exception (String.Format ("Failed to validate program {0:x}", programHandle));
            }
        }

        public static void DeactivateVertexBuffer (VertexBufferHandle handle)
        {
            const BufferTarget type = BufferTarget.ArrayBuffer;
            GL.BindBuffer (type, 0);
            OpenTKHelper.ThrowErrors ();
        }

        public static void DeactivateIndexBuffer (IndexBufferHandle handle)
        {
            const BufferTarget type = BufferTarget.ElementArrayBuffer;
            GL.BindBuffer (type, 0);
            OpenTKHelper.ThrowErrors ();
        }

        public static void DestroyTexture (Handle textureHandle)
        {
            int glTextureId = (textureHandle as TextureHandle).TextureId;
            GL.DeleteTextures (1, ref glTextureId);
            OpenTKHelper.ThrowErrors ();
        }

        public static ShaderVariantHandle CreateInternalShaderVariant (String identifier, String vertexShaderSource, String fragmentShaderSource)
        {
            Int32 vertexShaderHandle = -1;
            Int32 fragmentShaderHandle = -1;

            CompileShader (ShaderType.VertexShader, vertexShaderSource, out vertexShaderHandle);
            CompileShader (ShaderType.FragmentShader, fragmentShaderSource, out fragmentShaderHandle);

            Console.WriteLine("Creating Pass Variant: " + identifier);

            var programHandle = CreateShaderProgram ();

            var vertShaderHandle = CreateVertexShader (vertexShaderSource);
            var fragShaderHandle = CreateFragmentShader (fragmentShaderSource);

            AttachShader (programHandle, vertShaderHandle);
            AttachShader (programHandle, fragShaderHandle);

            var result = new ShaderVariantHandle (vertShaderHandle, fragShaderHandle, programHandle);

            return result;
        }
    }

    internal static class TypeConversionExtensions
    {
        public static KhronosVector2 ToOpenTK (this Abacus.SinglePrecision.Vector2 vec)
        {
            return new KhronosVector2 (vec.X, vec.Y);
        }

        public static Abacus.SinglePrecision.Vector2 ToAbacus (this KhronosVector2 vec)
        {
            return new Abacus.SinglePrecision.Vector2 (vec.X, vec.Y);
        }

        public static KhronosVector3 ToOpenTK (this Abacus.SinglePrecision.Vector3 vec)
        {
            return new KhronosVector3 (vec.X, vec.Y, vec.Z);
        }

        public static Abacus.SinglePrecision.Vector3 ToAbacus (this KhronosVector3 vec)
        {
            return new Abacus.SinglePrecision.Vector3 (vec.X, vec.Y, vec.Z);
        }

        public static KhronosVector4 ToOpenTK (this Abacus.SinglePrecision.Vector4 vec)
        {
            return new KhronosVector4 (vec.X, vec.Y, vec.Z, vec.W);
        }

        public static Abacus.SinglePrecision.Vector4 ToAbacus (this KhronosVector4 vec)
        {
            return new Abacus.SinglePrecision.Vector4 (vec.X, vec.Y, vec.Z, vec.W);
        }

        public static KhronosMatrix4 ToOpenTK (this Abacus.SinglePrecision.Matrix44 mat)
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

    
    // Cross platform wrapper around the Open TK libary, sitting at a slightly higher level.
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal class ShaderVariantHandle
    {
        public Int32 VertexShaderHandle { get; private set; }
        public Int32 FragmentShaderHandle { get; private set; }
        public Int32 ProgramHandle { get; private set; }

        internal ShaderVariantHandle (Int32 vertexShaderHandle, Int32 fragmentShaderHandle, Int32 programHandle)
        {
            this.VertexShaderHandle = vertexShaderHandle;
            this.FragmentShaderHandle = fragmentShaderHandle;
            this.ProgramHandle = programHandle;
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class ShaderHandle
        : Handle
    {
        internal List<ShaderVariantHandle> VariantHandles { get; private set; }

        internal ShaderHandle (List<ShaderVariantHandle> variantHandles)
        {
            this.VariantHandles = variantHandles;
        }

        protected override void CleanUpNativeResources ()
        {
            // TODO: Make destroy call here?

            VariantHandles.Clear ();

            base.CleanUpNativeResources ();
        }
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
    public class TextureHandle
        : Handle
    {
        public Int32 TextureId { get; private set; }
        
        internal TextureHandle (int textureId)
        {
            this.TextureId = textureId;
        }

        protected override void CleanUpNativeResources ()
        {
            // TODO: Make destroy call here?

            TextureId = 0;

            base.CleanUpNativeResources ();
        }
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
    public class IndexBufferHandle
        : Handle
    {
        static Int32 resourceCounter;
        
        public UInt32 GLHandle { get; private set; }
        
        internal IndexBufferHandle (UInt32 glHandle)
        {
            GLHandle = glHandle;
            resourceCounter++;
        }

        protected override void CleanUpNativeResources ()
        {
            // TODO: Make destroy call here?

            GLHandle = 0;
            resourceCounter--;
            
            base.CleanUpNativeResources ();
        }
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
    public class VertexBufferHandle
        : Handle
    {
        static Int32 resourceCounter;
        
        public UInt32 GLHandle { get; private set; }
        
        internal VertexBufferHandle (UInt32 glHandle)
        {
            GLHandle = glHandle;
            resourceCounter++;
        }

        protected override void CleanUpNativeResources ()
        {
            // TODO: Make destroy call here?

            GLHandle = 0;
            resourceCounter--;
            
            base.CleanUpNativeResources ();
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    // This is a dirty hack for the time being.
    public static class OpenTkCache
    {
        static readonly Dictionary <String, Dictionary<String, Object>> data = 
            new Dictionary<String, Dictionary<String, Object>> ();

        public static void Set<T> (Handle h, String identifier, T value)
        {
            if (!data.ContainsKey (h.Identifier))
                data.Add (h.Identifier, new Dictionary<String, Object> ());

            data[h.Identifier].Add (identifier, value);
        }

        public static T Get <T> (Handle h, String identifier)
        {
            return (T) data[h.Identifier][identifier];
        }

        public static void Clear (Handle h)
        {
            data.Remove (h.Identifier);
        }
    }

    sealed class OTKShaderUniform
    {
        public Int32 Index { get; set; }
        public String Name { get; set; }
        public ActiveUniformType Type { get; set; }
        public Int32 Location { get; set; }
    }

    sealed class OTKShaderAttribute
    {
        public Int32 Index { get; set; }
        public String Name { get; set; }
        public ActiveAttribType Type { get; set; }
        public Int32 Location { get; set; }
    }
}
