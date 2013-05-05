// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! - Low Level 3D App Engine                                         │ \\
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


namespace Sungiant.Cor.MonoTouchRuntime
{
	public sealed class IndexBuffer
        : BaseRuntime.IndexBuffer
		, IDisposable
	{
		static Int32 resourceCounter;

		Int32 indexCount;
		OpenTK.Graphics.ES20.All type;
		UInt32 bufferHandle;
		OpenTK.Graphics.ES20.All bufferUsage;

		public IndexBuffer (Int32 indexCount)
		{
			this.indexCount = indexCount;

			this.type = (OpenTK.Graphics.ES20.All) OpenTK.Graphics.ES20.BufferObjects.ElementArrayBuffer;

			this.bufferUsage = (OpenTK.Graphics.ES20.All) OpenTK.Graphics.ES20.BufferObjects.DynamicDraw;

			OpenTK.Graphics.ES20.GL.GenBuffers(1, ref this.bufferHandle);
            //try
            //{
			OpenTKHelper.CheckError();
            //}
            //catch
            //{
                //todo: why is this firing?
            //}

			if( this.bufferHandle == 0 )
			{
				throw new Exception("Failed to generate vert buffer.");
			}

			this.Activate();

			// glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
			// be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
			// then content of data are copied to the allocated data store.  The contents of the buffer object data
			// store can be initialized or updated using the glBufferSubData FN
			OpenTK.Graphics.ES20.GL.BufferData(
				this.type,
				(System.IntPtr) (sizeof(UInt16) * this.indexCount),
				(System.IntPtr) null,
				this.bufferUsage);

			OpenTKHelper.CheckError();

			resourceCounter++;

		}

		~IndexBuffer()
		{
			CleanUpNativeResources();
		}

		void CleanUpManagedResources()
		{

		}

		void CleanUpNativeResources()
		{
			OpenTK.Graphics.ES20.GL.DeleteBuffers(1, ref this.bufferHandle);
			OpenTKHelper.CheckError();

			bufferHandle = 0;

			resourceCounter--;
		}

		public void Dispose()
		{
			CleanUpManagedResources();
			CleanUpNativeResources();
			GC.SuppressFinalize(this);
		}

		internal void Activate()
		{
			OpenTK.Graphics.ES20.GL.BindBuffer(this.type, this.bufferHandle);
			OpenTKHelper.CheckError();
		}

		internal void Deactivate()
		{
			OpenTK.Graphics.ES20.GL.BindBuffer(this.type, 0);
			OpenTKHelper.CheckError();
		}


		public override void SetData (Int32[] data)
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
			OpenTK.Graphics.ES20.GL.BufferSubData(
				this.type,
				(System.IntPtr) 0,
				(System.IntPtr) (sizeof(UInt16) * this.indexCount),
				udata);

			udata = null;

			OpenTKHelper.CheckError();
		}

		public override int IndexCount
		{
			get
			{
				return indexCount;
			}
		}

	}
}
