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

namespace Blimey.Platform
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
    /// Provides a means to interact with a 2D texture living in GRAM.
    /// </summary>
    public sealed class Texture
        : IDisposable
        , IResource
        , IEquatable <Texture>
    {
        readonly IApi platform;
        readonly Handle textureHandle;

        public Handle Handle { get { return textureHandle; } }

        Boolean disposed;

        static Int32 textureCount = 0;
        static System.Collections.Concurrent.ConcurrentQueue <Handle> texturesToClean =
            new System.Collections.Concurrent.ConcurrentQueue<Handle> ();

        internal static void CollectGpuGarbage (IApi platform)
        {
            Handle handle = null;
            while (texturesToClean.TryDequeue (out handle))
            {
                platform.gfx_DestroyTexture (handle);
                --textureCount;
                InternalUtils.Log.Info ("GFX", "Texture destroyed: " + handle.Identifier);
            }
        }

        public Texture (IApi platform, TextureFormat textureFormat, Int32 width, Int32 height, Byte[] source)
        {
            this.platform = platform;
            this.textureHandle = platform.gfx_CreateTexture (textureFormat, width, height, source);
            ++textureCount;

            InternalUtils.Log.Info ("GFX", "Texture created: " + textureHandle.Identifier);
        }

        public override int GetHashCode ()
        {
            return Handle.GetHashCode ();
        }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;
            if (obj is Texture) flag = this.Equals ((Texture) obj);
            return flag;
        }

        public Boolean Equals (Texture other)
        {
            if (this.Handle != other.Handle)
                return false;

            return true;
        }

        public static Boolean operator == (Texture a, Texture b) { return Equals (a, b); }
        public static Boolean operator != (Texture a, Texture b) { return !Equals (a, b); }

        // This finalizer will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~Texture ()
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
        /*protected virtual*/ void Dispose (bool disposing)
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

                InternalUtils.Log.Info ("GFX", "Enqueuing texture for destruction: " + textureHandle.Identifier);
                texturesToClean.Enqueue (textureHandle);

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// The width of the texture in pixels.
        /// </summary>
        public Int32 Width
        {
            get
            {
                if (disposed) throw new Exception ("This texture has been unloaded from the GPU");
                return platform.gfx_tex_GetWidth (textureHandle);
            }
        }

        /// <summary>
        /// THe height of the texture in pixels.
        /// </summary>
        public Int32 Height
        {
            get
            {
                if (disposed) throw new Exception ("This texture has been unloaded from the GPU");
                return platform.gfx_tex_GetHeight (textureHandle);
            }
        }

        /// <summary>
        /// Defines the format in which the texture data is reperesented in GRAM.
        /// </summary>
        public TextureFormat SurfaceFormat
        {
            get
            {
                if (disposed) throw new Exception ("This texture has been unloaded from the GPU");
                return platform.gfx_tex_GetTextureFormat (textureHandle);
            }
        }

        /// <summary>
        /// The texture data in Cor.ITexture.SurfaceFormat.
        /// </summary>
        public Byte[] Primary
        {
            get
            {
                if (disposed) throw new Exception ("This texture has been unloaded from the GPU");
                return platform.gfx_tex_GetData (textureHandle);
            }
        }

        /// <summary>
        /// Contains mipmaps for the texture, if they exist, index -> byte[].
        /// </summary>
        public Byte[][] Mipmaps { get { throw new NotImplementedException (); } }
    }
}
