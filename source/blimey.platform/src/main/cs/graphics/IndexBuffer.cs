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
    /// Provides the means to interact with an index buffer in GRAM.
    /// Unlike many rendering apis where the data is unsigned, Cor uses signed data so that it can maintain CLS
    /// compliance.  I'm not sure if this will cause problems in the future with very large models, however, until
    /// it causes a problem it can stay like this.
    /// </summary>
    public sealed class IndexBuffer
        : IDisposable
        , ICorResource
        , IEquatable <IndexBuffer>
    {
        readonly IApi platform;
        readonly Handle handle;

        public Handle Handle { get { return handle; } }

        Boolean disposed;

        static Int32 indexBufferCount = 0;
        static System.Collections.Concurrent.ConcurrentQueue <Handle> indexBuffersToClean =
            new System.Collections.Concurrent.ConcurrentQueue<Handle> ();

        internal static void CollectGpuGarbage (IApi platform)
        {
            Handle handle = null;
            while (indexBuffersToClean.TryDequeue (out handle))
            {
                platform.gfx_DestroyIndexBuffer (handle);
                --indexBufferCount;
                InternalUtils.Log.Info ("GFX", "Index buffer destroyed: " + handle.Identifier);
            }
        }

        internal IndexBuffer (IApi platform, Int32 indexCount)
        {
            this.platform = platform;
            this.handle = platform.gfx_CreateIndexBuffer (indexCount);
            ++indexBufferCount;
            InternalUtils.Log.Info ("GFX", "Index buffer created: " + handle.Identifier);
        }

        public override int GetHashCode ()
        {
            return Handle.GetHashCode ();
        }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;
            if (obj is IndexBuffer) flag = this.Equals ((IndexBuffer) obj);
            return flag;
        }

        public Boolean Equals (IndexBuffer other)
        {
            if (this.Handle != other.Handle)
                return false;

            return true;
        }

        public static Boolean operator == (IndexBuffer a, IndexBuffer b) { return Equals (a, b); }
        public static Boolean operator != (IndexBuffer a, IndexBuffer b) { return !Equals (a, b); }

        // This finalizer will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~IndexBuffer ()
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

                InternalUtils.Log.Info ("GFX", "Enqueuing index buffer for destruction: " + handle.Identifier);
                indexBuffersToClean.Enqueue (handle);

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// The cardinality of the index buffer,
        /// </summary>
        public Int32 IndexCount { get { return platform.gfx_ibff_GetIndexCount (handle); } }

        /// <summary>
        /// Sets all of the indicies in the buffer.
        /// </summary>
        public void SetData (Int32[] data) { platform.gfx_ibff_SetData (handle, data, 0, data.Length); }

        /// <summary>
        /// Gets all of the indices in the buffer.
        /// </summary>
        public Int32[] GetData () { return platform.gfx_ibff_GetData (handle, 0, IndexCount); }

        /// <summary>
        /// Sets indices in the buffer within the given range.
        /// </summary>
        public void SetData (Int32[] data, Int32 startIndex, Int32 elementCount)
        {
            platform.gfx_ibff_SetData (handle, data, startIndex, elementCount);
        }

        /// <summary>
        /// Gets indices in the buffer within the given range.
        /// </summary>
        public Int32[] GetData (Int32 startIndex, Int32 elementCount)
        {
            return platform.gfx_ibff_GetData (handle, startIndex, elementCount);
        }
    }
}