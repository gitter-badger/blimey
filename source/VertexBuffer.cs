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
using Sungiant.Abacus;
using System.Collections.Generic;

namespace Sungiant.Cor.MonoTouchRuntime
{

	//
	// After the buffer object data store has been initialized or updated using
	// the glBufferData or the glBufferSubData FN, the client data store is no longer
	// needed and can be realeased.  For static geometry, applications can free
	// the client data store and reduce the overall system memory consumed by the application.
	//
	public sealed class VertexBuffer
#if aot
        : IVertexBuffer
#else
		: BaseRuntime.VertexBuffer
#endif
		, IDisposable
	{
		Int32 resourceCounter;
		VertexDeclaration vertDecl;

		Int32 vertexCount;

		UInt32 bufferHandle;

		OpenTK.Graphics.ES20.All type;
		OpenTK.Graphics.ES20.All bufferUsage;

		public VertexBuffer (VertexDeclaration vd, Int32 vertexCount)
		{
			this.vertDecl = vd;
			this.vertexCount = vertexCount;

			this.type = (OpenTK.Graphics.ES20.All) OpenTK.Graphics.ES20.BufferObjects.ArrayBuffer;

			this.bufferUsage = (OpenTK.Graphics.ES20.All) OpenTK.Graphics.ES20.BufferObjects.DynamicDraw;

			OpenTK.Graphics.ES20.GL.GenBuffers(1, ref this.bufferHandle);
			OpenTKHelper.CheckError();


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
				(System.IntPtr) (vertDecl.VertexStride * this.vertexCount),
				(System.IntPtr) null,
				this.bufferUsage);

			OpenTKHelper.CheckError();

			resourceCounter++;

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

		~VertexBuffer()
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

#if aot
		public void SetData (VertexPosition[] data) { SetDataHelper (data); }
		public void SetData (VertexPositionColour[] data) { SetDataHelper (data); }
		public void SetData (VertexPositionNormal[] data) { SetDataHelper (data); }
		public void SetData (VertexPositionNormalColour[] data) { SetDataHelper (data); }
		public void SetData (VertexPositionNormalTexture[] data) { SetDataHelper (data); }
		public void SetData (VertexPositionNormalTextureColour[] data) { SetDataHelper (data); }
		public void SetData (VertexPositionTexture[] data) { SetDataHelper (data); }
		public void SetData (VertexPositionTextureColour[] data) { SetDataHelper (data); }

		void SetDataHelper<T> (T[] data) where T: struct, IVertexType
#else
		public override void SetData<T> (T[] data)
#endif
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
			OpenTK.Graphics.ES20.GL.BufferSubData(
				this.type,
				(System.IntPtr) 0,
				(System.IntPtr) (vertDecl.VertexStride * this.vertexCount),
				data);

			OpenTKHelper.CheckError();
		}


#if aot
		public Int32 VertexCount
#else
		public override Int32 VertexCount
#endif
		{
			get
			{
				return this.vertexCount;
			}
		}

#if aot
		public VertexDeclaration VertexDeclaration 
#else
		public override VertexDeclaration VertexDeclaration
#endif
		{
			get
			{
				return this.vertDecl;
			}
		} 


#if aot
		public void GetData<T> (T[] data) where T: struct, IVertexType { throw new System.NotSupportedException(); }
		
		public void GetData<T> (T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType { throw new System.NotSupportedException(); }
		
		public void GetData<T> (Int32 offsetInBytes, T[] data, Int32 startIndex, Int32 elementCount, Int32 vertexStride) where T: struct, IVertexType { throw new System.NotSupportedException(); }
		
		public void SetData<T> (T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType { throw new System.NotSupportedException(); }
		
		public void SetData<T> (Int32 offsetInBytes, T[] data, Int32 startIndex, Int32 elementCount, Int32 vertexStride) where T: struct, IVertexType { throw new System.NotSupportedException(); }
#endif

	}
}
