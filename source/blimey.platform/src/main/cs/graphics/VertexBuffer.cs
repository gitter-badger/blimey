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
    using System.Reflection;
    using Abacus.SinglePrecision;
    using Fudge;
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides the means to interact with a vertex buffer in GRAM.
    /// </summary>
    public sealed class VertexBuffer
        : IDisposable
        , ICorResource
        , IEquatable <VertexBuffer>
    {
        readonly IApi platform;
        readonly Handle handle;

        public Handle Handle { get { return handle; } }

        Boolean disposed;

        static Int32 vertexBufferCount = 0;
        static System.Collections.Concurrent.ConcurrentQueue <Handle> vertexBuffersToClean =
            new System.Collections.Concurrent.ConcurrentQueue<Handle> ();

        internal static void CollectGpuGarbage (IApi platform)
        {
            Handle handle = null;
            while (vertexBuffersToClean.TryDequeue (out handle))
            {
                platform.gfx_DestroyVertexBuffer (handle);
                --vertexBufferCount;
                InternalUtils.Log.Info ("GFX", "Vertex buffer destroyed: " + handle.Identifier);
            }
        }

        internal VertexBuffer (IApi platform, VertexDeclaration vertexDeclaration, Int32 vertexCount)
        {
            this.platform = platform;
            this.handle = platform.gfx_CreateVertexBuffer (vertexDeclaration, vertexCount);
            ++vertexBufferCount;
            InternalUtils.Log.Info ("GFX", "Vertex buffer created: " + handle.Identifier);
        }

        public override int GetHashCode ()
        {
            return Handle.GetHashCode ();
        }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;
            if (obj is VertexBuffer) flag = this.Equals ((VertexBuffer) obj);
            return flag;
        }

        public Boolean Equals (VertexBuffer other)
        {
            if (this.Handle != other.Handle)
                return false;

            return true;
        }

        public static Boolean operator == (VertexBuffer a, VertexBuffer b) { return Equals (a, b); }
        public static Boolean operator != (VertexBuffer a, VertexBuffer b) { return !Equals (a, b); }

        // This finalizer will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~VertexBuffer ()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose (false);
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose ()
        {
            Dispose (true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize (this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        /* protected virtual*/ void Dispose (bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.

                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.

                InternalUtils.Log.Info ("GFX", "Enqueuing vertex buffer for destruction: " + handle.Identifier);
                vertexBuffersToClean.Enqueue (handle);
                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Int32 VertexCount { get { return platform.gfx_vbff_GetVertexCount (handle); } }

        /// <summary>
        ///
        /// </summary>
        public VertexDeclaration VertexDeclaration { get { return platform.gfx_vbff_GetVertexDeclaration (handle); } }

        public void SetDataEx (IVertexType[] data)
        {
            MethodInfo mi = typeof(VertexBuffer).GetMethod ("SetDataR");

            var vertType = data [0].GetType ();
            var gmi = mi.MakeGenericMethod (vertType);

            try
            {
                gmi.Invoke (this, new [] { data });
            }
            catch (Exception ex)
            {
                throw new Exception (
                    "Failed to invoke SetDataR for type [" + vertType + "]" +
                    "\n" + ex.Message +
                    "\n" + ex.InnerException.Message);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void SetDataR<T> (IVertexType[] data) where T: struct, IVertexType
        {
            var cast = data.Cast <T> ().ToArray ();
            SetData (cast);
        }

        public void SetData<T> (T[] data)
        where T
            : struct
            , IVertexType
        {
            platform.gfx_vbff_SetData (handle, data, 0, data.Length);
        }

        /// <summary>
        ///
        /// </summary>
        public T[] GetData<T> ()
        where T
            : struct
            , IVertexType
        {
            return platform.gfx_vbff_GetData <T> (handle, 0, VertexCount);
        }

        /// <summary>
        ///
        /// </summary>
        public void SetData<T> (T[] data, Int32 startIndex, Int32 elementCount)
        where T
            : struct
            , IVertexType
        {
            platform.gfx_vbff_SetData (handle, data, startIndex, elementCount);
        }

        /// <summary>
        ///
        /// </summary>
        public T[] GetData<T> (Int32 startIndex, Int32 elementCount)
        where T
            : struct
            , IVertexType
        {
            return platform.gfx_vbff_GetData <T> (handle, startIndex, elementCount);
        }
    }
}