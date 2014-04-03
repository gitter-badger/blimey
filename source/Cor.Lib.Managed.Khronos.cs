// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor.Lib.Managed.Khronos                                                │ \\
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
// │ Copyright © 2013 A.J.Pook (http://sungiant.github.com)                 │ \\
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
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

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

namespace Cor.Lib.Managed.Khronos
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

}
