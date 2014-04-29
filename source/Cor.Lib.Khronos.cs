// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor.Lib.Khronos                                                │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Brought to you by:                                                     │ \\
// │          _________                    .__               __             │ \\
// │         /   _____/__ __  ____    ____ |__|____    _____/  |_           │ \\
// │         \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\          │ \\
// │         /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |            │ \\
// │        /_______  /____/|___|  /\___  /|__(____  /___|  /__|            │ \\
// │                \/           \//_____/         \/     \/                │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2014 A.J.Pook (http://ajpook.github.io)                    │ \\
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

using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

using Abacus;
using Abacus.Packed;
using Abacus.SinglePrecision;
using Abacus.Int32Precision;

#if COR_PLATFORM_MANAGED_XIOS
using OpenTK.Graphics.ES20;
using GLShaderType = OpenTK.Graphics.ES20.ShaderType;
using GLBufferUsage = OpenTK.Graphics.ES20.BufferUsage;
#elif COR_PLATFORM_MANAGED_MONOMAC
using MonoMac.OpenGL;
using GLShaderType = MonoMac.OpenGL.ShaderType;
using GLBufferUsage = MonoMac.OpenGL.BufferUsageHint;
#else
    Platform not supported.
#endif

using Boolean = System.Boolean;

namespace Cor.Lib.Khronos
{
    public static class ErrorHandler
    {
        [Conditional("DEBUG")]
        public static void Check()
        {
            var ec = GL.GetError();

            if (ec != ErrorCode.NoError)
            {
                throw new Exception( ec.ToString());
            }
        }
    }

    /// <summary>
    /// Static class to help with horrible shader system.
    /// </summary>
    public static class ShaderUtils
    {
        public class ShaderUniform
        {
            public Int32 Index { get; set; }
            public String Name { get; set; }
            public ActiveUniformType Type { get; set; }
        }

        public class ShaderAttribute
        {
            public Int32 Index { get; set; }
            public String Name { get; set; }
            public ActiveAttribType Type { get; set; }
        }

        public static Int32 CreateShaderProgram()
        {
            // Create shader program.
            Int32 programHandle = GL.CreateProgram ();

            if( programHandle == 0 )
                throw new Exception("Failed to create shader program");

            ErrorHandler.Check();

            return programHandle;
        }

        public static Int32 CreateVertexShader(string path)
        {
            Int32 vertShaderHandle;

            if( Path.GetExtension(path) != ".vsh" )
            {
                throw new Exception("Vertex shader [" + path + "] should end with .vsh");
            }

            if( !File.Exists(path))
            {
                throw new Exception("Vertex shader at [" + path + "] does not exist.");
            }

            ShaderUtils.CompileShader (
                GLShaderType.VertexShader,
                path,
                out vertShaderHandle );

            if( vertShaderHandle == 0 )
                throw new Exception("Failed to compile vertex shader program");

            return vertShaderHandle;
        }

        public static Int32 CreateFragmentShader(string path)
        {
            Int32 fragShaderHandle;

            if( Path.GetExtension(path) != ".fsh" )
            {
                throw new Exception("Fragement shader [" + path + "] should end with .fsh");
            }

            if( !File.Exists(path))
            {
                throw new Exception("Fragement shader at [" + path + "] does not exist.");
            }

            ShaderUtils.CompileShader (
                GLShaderType.FragmentShader,
                path,
                out fragShaderHandle );

            if( fragShaderHandle == 0 )
                throw new Exception("Failed to compile fragment shader program");


            return fragShaderHandle;
        }

        public static void AttachShader(
            Int32 programHandle,
            Int32 shaderHandle)
        {
            if (shaderHandle != 0)
            {
                // Attach vertex shader to program.
                GL.AttachShader (programHandle, shaderHandle);
                ErrorHandler.Check();
            }
        }

        public static void DetachShader(
            Int32 programHandle,
            Int32 shaderHandle )
        {
            if (shaderHandle != 0)
            {
                GL.DetachShader (programHandle, shaderHandle);
                ErrorHandler.Check();
            }
        }

        public static void DeleteShader(
            Int32 programHandle,
            Int32 shaderHandle )
        {
            if (shaderHandle != 0)
            {
                GL.DeleteShader (shaderHandle);
                shaderHandle = 0;
                ErrorHandler.Check();
            }
        }

        public static void DestroyShaderProgram (Int32 programHandle)
        {
            if (programHandle != 0)
            {
#if COR_PLATFORM_MANAGED_XIOS
                GL.DeleteProgram (programHandle);
#elif COR_PLATFORM_MANAGED_MONOMAC
                GL.DeleteProgram (1, new int[]{ programHandle } );
#endif

                programHandle = 0;
                ErrorHandler.Check();
            }
        }

        public static void CompileShader (
            GLShaderType type,
            String file,
            out Int32 shaderHandle )
        {
            String src = string.Empty;

            try
            {
                // Get the data from the text file
                src = System.IO.File.ReadAllText (file);
            }
            catch(Exception e)
            {
                InternalUtils.Log.Info(e.Message);
                shaderHandle = 0;
                return;
            }

            // Create an empty vertex shader object
            shaderHandle = GL.CreateShader (type);

            ErrorHandler.Check();

            // Replace the source code in the vertex shader object
#if COR_PLATFORM_MANAGED_XIOS
            GL.ShaderSource (
                shaderHandle,
                1,
                new String[] { src },
                (Int32[]) null );
#elif COR_PLATFORM_MANAGED_MONOMAC
            GL.ShaderSource (
                shaderHandle,
                src);
#endif

            ErrorHandler.Check();

            GL.CompileShader (shaderHandle);

            ErrorHandler.Check();

#if DEBUG
            Int32 logLength = 0;
            GL.GetShader (
                shaderHandle,
                ShaderParameter.InfoLogLength,
                out logLength);

            ErrorHandler.Check();
            var infoLog = new System.Text.StringBuilder(logLength);

            if (logLength > 0)
            {
                int temp = 0;
                GL.GetShaderInfoLog (
                    shaderHandle,
                    logLength,
                    out temp,
                    infoLog );

                string log = infoLog.ToString();

                InternalUtils.Log.Info(file);
                InternalUtils.Log.Info (log);
                InternalUtils.Log.Info(type.ToString());
            }
#endif
            Int32 status = 0;

            GL.GetShader (
                shaderHandle,
                ShaderParameter.CompileStatus,
                out status );

            ErrorHandler.Check();

            if (status == 0)
            {
                GL.DeleteShader (shaderHandle);
                throw new Exception ("Failed to compile " + type.ToString());
            }
        }

        public static List<ShaderUniform> GetUniforms (Int32 prog)
        {

            int numActiveUniforms = 0;

            var result = new List<ShaderUniform>();

            GL.GetProgram(prog, ProgramParameter.ActiveUniforms, out numActiveUniforms);
            ErrorHandler.Check();

            for(int i = 0; i < numActiveUniforms; ++i)
            {
                var sb = new System.Text.StringBuilder ();

                int buffSize = 0;
                int length = 0;
                int size = 0;
                ActiveUniformType type;

                GL.GetActiveUniform(
                    prog,
                    i,
                    64,
                    out length,
                    out size,
                    out type,
                    sb);
                ErrorHandler.Check();

                result.Add(
                    new ShaderUniform()
                    {
                    Index = i,
                    Name = sb.ToString(),
                    Type = type
                    }
                );
            }

            return result;
        }

        public static List<ShaderAttribute> GetAttributes (Int32 prog)
        {
            int numActiveAttributes = 0;

            var result = new List<ShaderAttribute>();

            // gets the number of active vertex attributes
            GL.GetProgram(prog, ProgramParameter.ActiveAttributes, out numActiveAttributes);
            ErrorHandler.Check();

            for(int i = 0; i < numActiveAttributes; ++i)
            {
                var sb = new System.Text.StringBuilder ();

                int buffSize = 0;
                int length = 0;
                int size = 0;
                ActiveAttribType type;
                GL.GetActiveAttrib(
                    prog,
                    i,
                    64,
                    out length,
                    out size,
                    out type,
                    sb);
                ErrorHandler.Check();

                result.Add(
                    new ShaderAttribute()
                    {
                        Index = i,
                        Name = sb.ToString(),
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

            ErrorHandler.Check();

#if DEBUG
            Int32 logLength = 0;

            GL.GetProgram (
                prog,
                ProgramParameter.InfoLogLength,
                out logLength );

            ErrorHandler.Check();

            if (logLength > 0)
            {
                retVal = false;

                /*
                var infoLog = new System.Text.StringBuilder ();

                GL.GetProgramInfoLog (
                    prog,
                    logLength,
                    out logLength,
                    infoLog );
                */
                var infoLog = string.Empty;
                GL.GetProgramInfoLog(prog, out infoLog);


                ErrorHandler.Check();

                InternalUtils.Log.Info (string.Format("[Cor.Resources] Program link log:\n{0}", infoLog));
            }
#endif
            Int32 status = 0;

            GL.GetProgram (
                prog,
                ProgramParameter.LinkStatus,
                out status );

            ErrorHandler.Check();

            if (status == 0)
            {
                throw new Exception(String.Format("Failed to link program: {0:x}", prog));
            }

            return retVal;

        }

        public static void ValidateProgram (Int32 programHandle)
        {
            GL.ValidateProgram (programHandle);

            ErrorHandler.Check();

            Int32 logLength = 0;

            GL.GetProgram (
                programHandle,
                ProgramParameter.InfoLogLength,
                out logLength );

            ErrorHandler.Check();

            if (logLength > 0)
            {
                var infoLog = new System.Text.StringBuilder ();

                GL.GetProgramInfoLog (
                    programHandle,
                    logLength,
                    out logLength, infoLog );

                ErrorHandler.Check();

                InternalUtils.Log.Info (string.Format("[Cor.Resources] Program validate log:\n{0}", infoLog));
            }

            Int32 status = 0;

            GL.GetProgram (
                programHandle, ProgramParameter.LinkStatus,
                out status );

            ErrorHandler.Check();

            if (status == 0)
            {
                throw new Exception (String.Format("Failed to validate program {0:x}", programHandle));
            }
        }
    }

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

            GL.GenBuffers(1, out this.bufferHandle);
            ErrorHandler.Check();


            if( this.bufferHandle == 0 )
            {
                throw new Exception("Failed to generate vert buffer.");
            }


            this.Activate();

            GL.BufferData(
                this.type,
                (System.IntPtr) (vertDecl.VertexStride * this.vertexCount),
                (System.IntPtr) null,
                this.bufferUsage);

            ErrorHandler.Check();

            resourceCounter++;

        }

        internal void Activate()
        {
            GL.BindBuffer(this.type, this.bufferHandle);
            ErrorHandler.Check();
        }

        internal void Deactivate()
        {
            GL.BindBuffer(this.type, 0);
            ErrorHandler.Check();
        }

        ~VertexBuffer()
        {
            RunDispose(false);
        }

        void CleanUpManagedResources()
        {

        }

        void CleanUpNativeResources()
        {
            GL.DeleteBuffers(1, ref this.bufferHandle);
            ErrorHandler.Check();

            bufferHandle = 0;

            resourceCounter--;
        }

        public void Dispose()
        {
            RunDispose(true);
            GC.SuppressFinalize(this);
        }

        public void RunDispose(bool isDisposing)
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
            throw new NotImplementedException();/*
            GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
            GraphicsExtensions.CheckGLError();
            var elementSizeInByte = Marshal.SizeOf(typeof(T));
            IntPtr ptr = GL.MapBuffer (BufferTarget.ArrayBuffer, BufferAccess.ReadOnly);
            GraphicsExtensions.CheckGLError();

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
                Marshal.Copy(ptr, buffer, 0, buffer.Length);

                var dataHandle = GCHandle.Alloc (data, GCHandleType.Pinned);
                var dataPtr = (IntPtr)(dataHandle.AddrOfPinnedObject ().ToInt64 () + startIndex * elementSizeInByte);

                // Copy from the temporary buffer to the destination array

                int dataSize = Marshal.SizeOf(typeof(T));
                if (dataSize == vertexStride)
                    Marshal.Copy(buffer, 0, dataPtr, buffer.Length);
                else
                {
                    // If the user is asking for a specific element within the vertex buffer, copy them one by one...
                    for (int i = 0; i < elementCount; i++)
                    {
                        Marshal.Copy(buffer, i * vertexStride, dataPtr, dataSize);
                        dataPtr = (IntPtr)(dataPtr.ToInt64() + dataSize);
                    }
                }

                dataHandle.Free ();

                //Buffer.BlockCopy(buffer, 0, data, startIndex * elementSizeInByte, elementCount * elementSizeInByte);
            }
            GL.UnmapBuffer(BufferTarget.ArrayBuffer);*/
        }

        public void SetData<T> (T[] data)
        where T
            : struct
            , IVertexType
        {
            this.SetData(data, 0, this.vertexCount);
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
            if( data.Length != vertexCount )
            {
                throw new Exception("?");
            }

            this.Activate();

            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            GL.BufferSubData(
                this.type,
                (System.IntPtr) (this.vertDecl.VertexStride * startIndex),
                (System.IntPtr) (this.vertDecl.VertexStride * elementCount),
                data);

            ErrorHandler.Check();
        }

        public T[] GetData<T> (Int32 startIndex, Int32 elementCount)
        where T
            : struct
            , IVertexType
        {
            throw new System.NotSupportedException();
        }

        public void SetRawData (
            Byte[] data,
            Int32 startIndex,
            Int32 elementCount)
        {
            this.Activate();

            GL.BufferSubData(
                this.type,
                (System.IntPtr) (this.vertDecl.VertexStride * startIndex),
                (System.IntPtr) (this.vertDecl.VertexStride * elementCount),
                data);

            ErrorHandler.Check();
        }

        public Byte[] GetRawData (
            Int32 startIndex,
            Int32 elementCount)
        {
            throw new System.NotSupportedException();
        }
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

            GL.GenBuffers(1, out this.bufferHandle);

            ErrorHandler.Check();

            if( this.bufferHandle == 0 )
            {
                throw new Exception("Failed to generate vert buffer.");
            }

            this.Activate();

            GL.BufferData(
                this.type,
                (System.IntPtr) (sizeof(UInt16) * this.indexCount),
                (System.IntPtr) null,
                this.bufferUsage);

            ErrorHandler.Check();

            resourceCounter++;

        }

        ~IndexBuffer()
        {
            RunDispose(false);
        }

        void CleanUpManagedResources()
        {

        }

        void CleanUpNativeResources()
        {
            GL.DeleteBuffers(1, ref this.bufferHandle);
            ErrorHandler.Check();

            bufferHandle = 0;

            resourceCounter--;
        }

        public void Dispose()
        {
            RunDispose(true);
            GC.SuppressFinalize(this);
        }

        public void RunDispose(bool isDisposing)
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
            throw new NotImplementedException();/*
            GL.BindBuffer(BufferTarget.ArrayBuffer, ibo);
            GraphicsExtensions.CheckGLError();
            var elementSizeInByte = Marshal.SizeOf(typeof(T));
            IntPtr ptr = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.ReadOnly);
            // Pointer to the start of data to read in the index buffer
            ptr = new IntPtr(ptr.ToInt64() + offsetInBytes);
            if (data is byte[])
            {
                byte[] buffer = data as byte[];
                // If data is already a byte[] we can skip the temporary buffer
                // Copy from the index buffer to the destination array
                Marshal.Copy(ptr, buffer, 0, buffer.Length);
            }
            else
            {
                // Temporary buffer to store the copied section of data
                byte[] buffer = new byte[elementCount * elementSizeInByte];
                // Copy from the index buffer to the temporary buffer
                Marshal.Copy(ptr, buffer, 0, buffer.Length);
                // Copy from the temporary buffer to the destination array
                Buffer.BlockCopy(buffer, 0, data, startIndex * elementSizeInByte, elementCount * elementSizeInByte);
            }
            GL.UnmapBuffer(BufferTarget.ArrayBuffer);
            GraphicsExtensions.CheckGLError();
            */
        }

        internal void Activate()
        {
            GL.BindBuffer(this.type, this.bufferHandle);
            ErrorHandler.Check();
        }

        internal void Deactivate()
        {
            GL.BindBuffer(this.type, 0);
            ErrorHandler.Check();
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

            if( data.Length != indexCount )
            {
                throw new Exception("?");
            }

            UInt16[] udata = new UInt16[data.Length];

            for(Int32 i = 0; i < data.Length; ++i)
            {
                udata[i] = (UInt16) data[i];
            }

            this.Activate();

            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            GL.BufferSubData(
                this.type,
                (System.IntPtr) 0,
                (System.IntPtr) (sizeof(UInt16) * this.indexCount),
                udata);

            udata = null;

            ErrorHandler.Check();
        }

        public void GetData(Int32[] data)
        {
            throw new NotImplementedException();
        }

        public void SetData(Int32[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();
        }

        public void GetData(Int32[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();
        }

        public void SetRawData(Byte[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();
        }

        public Byte[] GetRawData(Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class GeometryBuffer
        : IGeometryBuffer
    {
        IndexBuffer _iBuf;
        VertexBuffer _vBuf;
        
        public GeometryBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount)
        {

            if(vertexCount == 0)
            {
                throw new Exception("A geometry buffer must have verts");
            }

            if( indexCount != 0 )
            {
                _iBuf = new IndexBuffer(indexCount);
            }

            _vBuf = new VertexBuffer(vertexDeclaration, vertexCount);

        }

        internal void Activate()
        {
            _vBuf.Activate();

            if( _iBuf != null )
                _iBuf.Activate();
        }

        internal void Deactivate()
        {
            _vBuf.Deactivate();

            if( _iBuf != null )
                _iBuf.Deactivate();
        }

        public IVertexBuffer VertexBuffer { get { return _vBuf; } }
        public IIndexBuffer IndexBuffer { get { return _iBuf; } }

        internal VertexBuffer OpenTKVertexBuffer { get { return _vBuf; } }
    }

    public sealed class GraphicsManager
        : IGraphicsManager
    {
        readonly GpuUtils gpuUtils;

        GeometryBuffer currentGeomBuffer;
        CullMode? currentCullMode;

        public GraphicsManager()
        {
            InternalUtils.Log.Info(
                "Khronos Graphics Manager -> ()");

            this.gpuUtils = new GpuUtils();

            global::MonoMac.OpenGL.GL.Enable(global::MonoMac.OpenGL.EnableCap.Blend);
            ErrorHandler.Check();

            this.SetBlendEquation(
                BlendFunction.Add, BlendFactor.SourceAlpha, BlendFactor.InverseSourceAlpha,
                BlendFunction.Add, BlendFactor.One, BlendFactor.InverseSourceAlpha);

            global::MonoMac.OpenGL.GL.Enable(global::MonoMac.OpenGL.EnableCap.DepthTest);
            ErrorHandler.Check();

            global::MonoMac.OpenGL.GL.DepthMask(true);
            ErrorHandler.Check();

            global::MonoMac.OpenGL.GL.DepthRange(0f, 1f);
            ErrorHandler.Check();

            global::MonoMac.OpenGL.GL.DepthFunc(global::MonoMac.OpenGL.DepthFunction.Lequal);
            ErrorHandler.Check();

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

        void EnableVertAttribs(VertexDeclaration vertDecl, IntPtr pointer)
        {
            var vertElems = vertDecl.GetVertexElements();

            IntPtr ptr = pointer;

            int counter = 0;
            foreach(var elem in vertElems)
            {
                global::MonoMac.OpenGL.GL.EnableVertexAttribArray(counter);
                ErrorHandler.Check();

                //var vertElemUsage = elem.VertexElementUsage;
                var vertElemFormat = elem.VertexElementFormat;
                var vertElemOffset = elem.Offset;

                Int32 numComponentsInVertElem = 0;
                Boolean vertElemNormalized = false;
                global::MonoMac.OpenGL.VertexAttribPointerType glVertElemFormat;

                EnumConverter.ToOpenGL(vertElemFormat, out glVertElemFormat, out vertElemNormalized, out numComponentsInVertElem);

                if( counter != 0)
                {
                    ptr = Add(ptr, vertElemOffset);
                }

                global::MonoMac.OpenGL.GL.VertexAttribPointer(
                    counter,                // index - specifies the generic vertex attribute index.  This value is 0 to
                                            //         max vertex attributes supported - 1.
                    numComponentsInVertElem,// size - number of components specified in the vertex array for the
                                            //        vertex attribute referenced by index.  Valid values are 1 - 4.
                    glVertElemFormat,       // type - Data format, valid values are GL_BYTE, GL_UNSIGNED_BYTE, GL_SHORT, GL_UNSIGNED_SHORT,
                                            //        GL_FLOAT, GL_FIXED, GL_HALF_FLOAT_OES*(Optional feature of es2)
                    vertElemNormalized,     // normalised - used to indicate whether the non-floating data format type should be normalised
                                            //              or not when converted to floating point.
                    vertDecl.VertexStride,  // stride - the components of vertex attribute specified by size are stored sequentially for each
                                            //          vertex.  stride specifies the delta between data for vertex index 1 and vertex (1 + 1).
                                            //          If stride is 0, attribute data for all vertices are stored sequentially.
                                            //          If stride is > 0, then we use the stride valude tas the pitch to get vertex data
                                            //          for the next index.
                    ptr

                    );

                ErrorHandler.Check();

                counter++;

            }
        }

        void DisableVertAttribs(VertexDeclaration vertDecl)
        {
            var vertElems = vertDecl.GetVertexElements();

            for(int i = 0; i < vertElems.Length; ++i)
            {
                global::MonoMac.OpenGL.GL.DisableVertexAttribArray(i);
                ErrorHandler.Check();
            }
        }


        #region IGraphicsManager

        public IGpuUtils GpuUtils { get { return this.gpuUtils; } }

        public void Reset()
        {
            this.ClearDepthBuffer();
            this.ClearColourBuffer();
            this.SetActiveGeometryBuffer(null);

            // todo, here we need to set all the texture slots to point to null
            this.SetActiveTexture(0, null);
        }

        public void ClearColourBuffer(Rgba32 col = new Rgba32())
        {
            Abacus.SinglePrecision.Vector4 c;

            col.UnpackTo(out c);

            global::MonoMac.OpenGL.GL.ClearColor (c.X, c.Y, c.Z, c.W);

            var mask = global::MonoMac.OpenGL.ClearBufferMask.ColorBufferBit;

            global::MonoMac.OpenGL.GL.Clear ( mask );

            ErrorHandler.Check();
        }

        public void ClearDepthBuffer(Single val = 1)
        {
            global::MonoMac.OpenGL.GL.ClearDepth(val);

            var mask = global::MonoMac.OpenGL.ClearBufferMask.DepthBufferBit;

            global::MonoMac.OpenGL.GL.Clear ( mask );

            ErrorHandler.Check();
        }

        public void SetCullMode(CullMode cullMode)
        {
            if (!currentCullMode.HasValue || currentCullMode.Value != cullMode)
            {
                if (cullMode == CullMode.None)
                {
                    global::MonoMac.OpenGL.GL.Disable (global::MonoMac.OpenGL.EnableCap.CullFace);
                    ErrorHandler.Check ();

                }
                else
                {
                    global::MonoMac.OpenGL.GL.Enable(global::MonoMac.OpenGL.EnableCap.CullFace);
                    ErrorHandler.Check();

                    global::MonoMac.OpenGL.GL.FrontFace(global::MonoMac.OpenGL.FrontFaceDirection.Cw);
                    ErrorHandler.Check();

                    if (cullMode == CullMode.CW)
                    {
                        global::MonoMac.OpenGL.GL.CullFace (global::MonoMac.OpenGL.CullFaceMode.Back);
                        ErrorHandler.Check ();
                    }
                    else if (cullMode == CullMode.CCW)
                    {
                        global::MonoMac.OpenGL.GL.CullFace (global::MonoMac.OpenGL.CullFaceMode.Front);
                        ErrorHandler.Check ();
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }

                currentCullMode = cullMode;
            }
        }

        public IGeometryBuffer CreateGeometryBuffer (
            VertexDeclaration vertexDeclaration,
            Int32 vertexCount,
            Int32 indexCount )
        {
            return new GeometryBuffer(vertexDeclaration, vertexCount, indexCount);
        }

        public void SetActiveGeometryBuffer(IGeometryBuffer buffer)
        {
            var temp = buffer as GeometryBuffer;

            if( temp != this.currentGeomBuffer )
            {
                if( this.currentGeomBuffer != null )
                {
                    this.currentGeomBuffer.Deactivate();

                    this.currentGeomBuffer = null;
                }

                if( temp != null )
                {
                    temp.Activate();
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

            IntPtr pixelDataRgba32 = Marshal.AllocHGlobal(tex.Data.Length);
            Marshal.Copy(tex.Data, 0, pixelDataRgba32, tex.Data.Length);
            
            // Call unmanaged code
            Marshal.FreeHGlobal(pixelDataRgba32);

            int textureId = -1;

            // this sets the unpack alignment.  which is used when reading pixels
            // in the fragment shader.  when the textue data is uploaded via glTexImage2d,
            // the rows of pixels are assumed to be aligned to the value set for GL_UNPACK_ALIGNMENT.
            // By default, the value is 4, meaning that rows of pixels are assumed to begin
            // on 4-byte boundaries.  this is a global STATE.
            global::MonoMac.OpenGL.GL.PixelStore(
                global::MonoMac.OpenGL.PixelStoreParameter.UnpackAlignment, 4);

            ErrorHandler.Check();

            // the first sept in the application of texture is to create the
            // texture object.  this is a container object that holds the
            // texture data.  this function returns a handle to a texture
            // object.
            global::MonoMac.OpenGL.GL.GenTextures(1, out textureId);
            ErrorHandler.Check();

            var textureHandle = new TextureHandle (textureId);

            var textureTarget = global::MonoMac.OpenGL.TextureTarget.Texture2D;

            // we need to bind the texture object so that we can opperate on it.
            global::MonoMac.OpenGL.GL.BindTexture(textureTarget, textureId);
            ErrorHandler.Check();

            // the incoming texture format
            // (the format that [pixelDataRgba32] is in)
            var format = global::MonoMac.OpenGL.PixelFormat.Rgba;

            var internalFormat = global::MonoMac.OpenGL.PixelInternalFormat.Rgba;

            var textureDataFormat = global::MonoMac.OpenGL.PixelType.UnsignedByte;

            // now use the bound texture object to load the image data.
            global::MonoMac.OpenGL.GL.TexImage2D(

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

            ErrorHandler.Check();

            // sets the minification and maginfication filtering modes.  required
            // because we have not loaded a complete mipmap chain for the texture
            // so we must select a non mipmapped minification filter.
            global::MonoMac.OpenGL.GL.TexParameter(textureTarget, global::MonoMac.OpenGL.TextureParameterName.TextureMinFilter, (int) global::MonoMac.OpenGL.All.Nearest );

            ErrorHandler.Check();

            global::MonoMac.OpenGL.GL.TexParameter(textureTarget, global::MonoMac.OpenGL.TextureParameterName.TextureMagFilter, (int) global::MonoMac.OpenGL.All.Nearest );

            ErrorHandler.Check();

            return textureHandle;
        }

        public void UnloadTexture (ITexture texture)
        {
            int textureId = (texture as TextureHandle).glTextureId;

            global::MonoMac.OpenGL.GL.DeleteTextures(1, ref textureId);
        }

        public void SetActiveTexture (Int32 slot, ITexture tex)
        {
            global::MonoMac.OpenGL.TextureUnit oglTexSlot = EnumConverter.ToOpenGLTextureSlot(slot);
            global::MonoMac.OpenGL.GL.ActiveTexture(oglTexSlot);

            var oglt0 = tex as TextureHandle;

            if( oglt0 != null )
            {
                var textureTarget = global::MonoMac.OpenGL.TextureTarget.Texture2D;

                // we need to bind the texture object so that we can opperate on it.
                global::MonoMac.OpenGL.GL.BindTexture(textureTarget, oglt0.glTextureId);
                ErrorHandler.Check();
            }
        }

        public IShader CreateShader (ShaderAsset asset)
        {
            throw new NotImplementedException ();
        }

        public void DestroyShader (IShader shader)
        {
            throw new NotImplementedException ();
        }

        public void SetBlendEquation(
            BlendFunction rgbBlendFunction,
            BlendFactor sourceRgb,
            BlendFactor destinationRgb,
            BlendFunction alphaBlendFunction,
            BlendFactor sourceAlpha,
            BlendFactor destinationAlpha
            )
        {
            global::MonoMac.OpenGL.GL.BlendEquationSeparate(
                EnumConverter.ToOpenGL(rgbBlendFunction),
                EnumConverter.ToOpenGL(alphaBlendFunction) );
            ErrorHandler.Check();

            global::MonoMac.OpenGL.GL.BlendFuncSeparate(
                EnumConverter.ToOpenGLSrc(sourceRgb),
                EnumConverter.ToOpenGLDest(destinationRgb),
                EnumConverter.ToOpenGLSrc(sourceAlpha),
                EnumConverter.ToOpenGLDest(destinationAlpha) );
            ErrorHandler.Check();
        }

        public void DrawPrimitives(
            PrimitiveType primitiveType,
            Int32 startVertex,
            Int32 primitiveCount )
        {
            throw new NotImplementedException();
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
            if( baseVertex != 0 || minVertexIndex != 0 || startIndex != 0 )
            {
                throw new NotImplementedException();
            }

            var otkpType =  EnumConverter.ToOpenGL(primitiveType);
            //Int32 numVertsInPrim = numVertices / primitiveCount;

            Int32 nVertsInPrim = PrimitiveHelper.NumVertsIn(primitiveType);
            Int32 count = primitiveCount * nVertsInPrim;

            var vertDecl = currentGeomBuffer.VertexBuffer.VertexDeclaration;

            this.EnableVertAttribs( vertDecl, (IntPtr) 0 );

            global::MonoMac.OpenGL.GL.DrawElements (
                otkpType,
                count,
                global::MonoMac.OpenGL.DrawElementsType.UnsignedShort,
                (System.IntPtr) 0 );

            ErrorHandler.Check();

            this.DisableVertAttribs(vertDecl);
        }

        public void DrawUserPrimitives <T> (
            PrimitiveType primitiveType,
            T[] vertexData,
            Int32 vertexOffset,
            Int32 primitiveCount,
            VertexDeclaration vertexDeclaration )
            where T : struct, IVertexType
        {
            // do i need to do this? todo: find out
            this.SetActiveGeometryBuffer(null);

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
            GCHandle pinnedArray = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();

            if( vertexOffset != 0 )
            {
                pointer = Add(pointer, vertexOffset * vertDecl.VertexStride * sizeof(byte));
            }

            var glDrawMode = EnumConverter.ToOpenGL(primitiveType);
            var glDrawModeAll = glDrawMode;

            var bindTarget = global::MonoMac.OpenGL.BufferTarget.ArrayBuffer;

            global::MonoMac.OpenGL.GL.BindBuffer(bindTarget, 0);
            ErrorHandler.Check();


            this.EnableVertAttribs( vertDecl, pointer );

            Int32 nVertsInPrim = PrimitiveHelper.NumVertsIn(primitiveType);
            Int32 count = primitiveCount * nVertsInPrim;

            global::MonoMac.OpenGL.GL.DrawArrays(
                glDrawModeAll, // specifies the primitive to render
                vertexOffset,  // specifies the starting vertex index in the enabled vertex arrays
                count ); // specifies the number of indicies to be drawn

            ErrorHandler.Check();


            this.DisableVertAttribs(vertDecl);


            pinnedArray.Free();
        }

        public void DrawUserIndexedPrimitives <T> (
            PrimitiveType primitiveType,
            T[] vertexData,
            Int32 vertexOffset,
            Int32 numVertices,
            Int32[] indexData,
            Int32 indexOffset,
            Int32 primitiveCount,
            VertexDeclaration vertexDeclaration )
            where T : struct, IVertexType
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal sealed class TextureHandle
        : ITexture
    {
        internal TextureHandle (int textureid)
        {
            glTextureId = textureid;
        }

        internal int glTextureId { get; private set; }

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

        public Byte[,] Mipmaps
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

    internal static class EnumConverter
    {
        internal static TextureUnit ToOpenGLTextureSlot(Int32 slot)
        {
            switch(slot)
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

            throw new NotSupportedException();
        }


        internal static Type ToType (ActiveAttribType ogl)
        {
            switch(ogl)
            {
            case ActiveAttribType.Float: return typeof(Single);
            case ActiveAttribType.FloatMat2: throw new NotSupportedException();
            case ActiveAttribType.FloatMat3: throw new NotSupportedException();
            case ActiveAttribType.FloatMat4: return typeof(Abacus.SinglePrecision.Matrix44);
            case ActiveAttribType.FloatVec2: return typeof(Abacus.SinglePrecision.Vector2);
            case ActiveAttribType.FloatVec3: return typeof(Abacus.SinglePrecision.Vector3);
            case ActiveAttribType.FloatVec4: return typeof(Abacus.SinglePrecision.Vector4);
            }

            throw new NotSupportedException();
        }

        internal static Type ToType (ActiveUniformType ogl)
        {
            switch(ogl)
            {
            case ActiveUniformType.Bool: return typeof(Boolean);
            case ActiveUniformType.BoolVec2: throw new NotSupportedException();
            case ActiveUniformType.BoolVec3: throw new NotSupportedException();
            case ActiveUniformType.BoolVec4: throw new NotSupportedException();
            case ActiveUniformType.Float: return typeof(Single);
            case ActiveUniformType.FloatMat2: throw new NotSupportedException();
            case ActiveUniformType.FloatMat3: throw new NotSupportedException();
            case ActiveUniformType.FloatMat4: return typeof(Abacus.SinglePrecision.Matrix44);
            case ActiveUniformType.FloatVec2: return typeof(Abacus.SinglePrecision.Vector2);
            case ActiveUniformType.FloatVec3: return typeof(Abacus.SinglePrecision.Vector3);
            case ActiveUniformType.FloatVec4: return typeof(Abacus.SinglePrecision.Vector4);
            case ActiveUniformType.Int: return typeof(Boolean);
            case ActiveUniformType.IntVec2: throw new NotSupportedException();
            case ActiveUniformType.IntVec3: throw new NotSupportedException();
            case ActiveUniformType.IntVec4: throw new NotSupportedException();
            case ActiveUniformType.Sampler2D: throw new NotSupportedException();
            case ActiveUniformType.SamplerCube: throw new NotSupportedException();
            }
            
            throw new NotSupportedException();
        }

        internal static void ToOpenGL (
            VertexElementFormat blimey,
            out VertexAttribPointerType dataFormat,
            out bool normalized,
            out int size)
        {
            normalized = false;
            size = 0;
            dataFormat = VertexAttribPointerType.Float;

            switch(blimey)
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
                case VertexElementFormat.Byte4: throw new Exception("?");
                case VertexElementFormat.Short2: throw new Exception("?");
                case VertexElementFormat.Short4: throw new Exception("?");
                case VertexElementFormat.NormalisedShort2: throw new Exception("?");
                case VertexElementFormat.NormalisedShort4: throw new Exception("?");
                case VertexElementFormat.HalfVector2: throw new Exception("?");
                case VertexElementFormat.HalfVector4: throw new Exception("?");
            }
        }

        internal static BlendingFactorSrc ToOpenGLSrc(BlendFactor blimey)
        {
            switch(blimey)
            {
                case BlendFactor.Zero: return BlendingFactorSrc.Zero;
                case BlendFactor.One: return BlendingFactorSrc.One;
                case BlendFactor.SourceColour: return BlendingFactorSrc.Src1Color; // todo: check this src1 stuff
                case BlendFactor.InverseSourceColour: return BlendingFactorSrc.OneMinusSrc1Color;
                case BlendFactor.SourceAlpha: return BlendingFactorSrc.SrcAlpha;
                case BlendFactor.InverseSourceAlpha: return BlendingFactorSrc.OneMinusSrcAlpha;
                case BlendFactor.DestinationAlpha: return BlendingFactorSrc.DstAlpha;
                case BlendFactor.InverseDestinationAlpha: return BlendingFactorSrc.OneMinusDstAlpha;
                case BlendFactor.DestinationColour: return BlendingFactorSrc.DstColor;
                case BlendFactor.InverseDestinationColour: return BlendingFactorSrc.OneMinusDstColor;
            }

            throw new Exception();
        }

        internal static BlendingFactorDest ToOpenGLDest(BlendFactor blimey)
        {
            switch(blimey)
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
            
            throw new Exception();
        }

        internal static BlendFactor ToCorDestinationBlendFactor (All ogl)
        {
            switch(ogl)
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

            throw new Exception();
        }

        internal static BlendEquationMode ToOpenGL(BlendFunction blimey)
        {
            switch(blimey)
            {
                case BlendFunction.Add: return BlendEquationMode.FuncAdd;
                case BlendFunction.Max: throw new NotSupportedException();
                case BlendFunction.Min: throw new NotSupportedException();
                case BlendFunction.ReverseSubtract: return BlendEquationMode.FuncReverseSubtract;
                case BlendFunction.Subtract: return BlendEquationMode.FuncSubtract;
            }
            
            throw new Exception();
        }

        internal static BlendFunction ToCorDestinationBlendFunction (All ogl)
        {
            switch(ogl)
            {
                case All.FuncAdd: return BlendFunction.Add;
                case All.MaxExt: return BlendFunction.Max;
                case All.MinExt: return BlendFunction.Min;
                case All.FuncReverseSubtract: return BlendFunction.ReverseSubtract;
                case All.FuncSubtract: return BlendFunction.Subtract;
            }
            
            throw new Exception();
        }

        // PRIMITIVE TYPE
        internal static BeginMode ToOpenGL (PrimitiveType blimey)
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

    public sealed class GpuUtils
        : IGpuUtils
    {
        public GpuUtils()
        {
        }

        #region IGpuUtils

        public Int32 BeginEvent(Rgba32 colour, String eventName)
        {
            return 0;
        }

        public Int32 EndEvent()
        {
            return 0;
        }

        public void SetMarker(Rgba32 colour, String eventName)
        {

        }

        public void SetRegion(Rgba32 colour, String eventName)
        {

        }

        #endregion
    }
}
