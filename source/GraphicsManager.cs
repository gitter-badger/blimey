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
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

namespace Sungiant.Cor.MonoTouchRuntime
{
	public class GraphicsManager
#if aot
		: IGraphicsManager
#else
        : BaseRuntime.GraphicsManager
#endif
	{
		GpuUtils gpuUtils;

		DisplayStatus displayStatus;

		GeometryBuffer currentGeomBuffer;

		public GraphicsManager(OpenTK.Graphics.IGraphicsContext gfxContext)
		{
			gpuUtils = new GpuUtils();
			displayStatus = new DisplayStatus();

			OpenTK.Graphics.ES20.GL.Enable((OpenTK.Graphics.ES20.All)OpenTK.Graphics.ES20.EnableCap.Blend);
			OpenTKHelper.CheckError();

            // default this
            // todo: all the interfaces need an base abstract implementation
            // where common stuff gets set.
            this.SetBlendEquation(
                BlendFunction.Add, BlendFactor.SourceAlpha, BlendFactor.InverseSourceAlpha,
                BlendFunction.Add, BlendFactor.One, BlendFactor.InverseSourceAlpha);

            /* subtract blend mode

            this.SetBlendEquation(
                BlendFunction.ReverseSubtract, BlendFactor.SourceAlpha, BlendFactor.One,
                BlendFunction.ReverseSubtract, BlendFactor.SourceAlpha, BlendFactor.One)
            */

			OpenTK.Graphics.ES20.GL.Enable((OpenTK.Graphics.ES20.All)OpenTK.Graphics.ES20.EnableCap.CullFace);
			OpenTKHelper.CheckError();

			OpenTK.Graphics.ES20.GL.Enable((OpenTK.Graphics.ES20.All)OpenTK.Graphics.ES20.EnableCap.DepthTest);
			OpenTKHelper.CheckError();

			OpenTK.Graphics.ES20.GL.DepthMask(true);
			OpenTKHelper.CheckError();

			OpenTK.Graphics.ES20.GL.DepthRange(0f, 1f);
			OpenTKHelper.CheckError();

			OpenTK.Graphics.ES20.GL.DepthFunc(OpenTK.Graphics.ES20.All.Lequal);
			OpenTKHelper.CheckError();

			OpenTK.Graphics.ES20.GL.FrontFace(OpenTK.Graphics.ES20.All.Cw);
			OpenTKHelper.CheckError();

			OpenTK.Graphics.ES20.GL.CullFace(OpenTK.Graphics.ES20.All.Back);
			OpenTKHelper.CheckError();



		}

#if aot
		public void Reset()
#else
		public override void Reset()
#endif
		{
		}

		/*
		// Clear all buffers to default values.
		public void Clear()
		{
			var mask =
				OpenTK.Graphics.ES20.ClearBufferMask.ColorBufferBit &
				OpenTK.Graphics.ES20.ClearBufferMask.DepthBufferBit &
				OpenTK.Graphics.ES20.ClearBufferMask.StencilBufferBit;

			OpenTK.Graphics.ES20.GL.Clear ( (Int32) mask );

			OpenTKHelper.CheckError();

		}
*/

#if aot
        public void SetActiveTexture(Int32 slot, ITexture tex)
#else
		public override void SetActiveTexture(Int32 slot, Texture2D tex)
#endif
        {
            OpenTK.Graphics.ES20.All oglTexSlot = EnumConverter.ToOpenTKTextureSlot(slot); 
            OpenTK.Graphics.ES20.GL.ActiveTexture(oglTexSlot);

            var oglt0 = tex as OpenGLTextureWrapper;
            
            if( oglt0 != null )
            {
                var textureTarget = OpenTK.Graphics.ES20.All.Texture2D;
                
                // we need to bind the texture object so that we can opperate on it.
                OpenTK.Graphics.ES20.GL.BindTexture(textureTarget, oglt0.glTextureId);
                OpenTKHelper.CheckError();
            }

        }
        
#if aot
		public void ClearColourBuffer(Rgba32 col)
#else
		public override void ClearColourBuffer(Rgba32 col)
#endif       
		{
			Vector4 c;

            col.UnpackTo(out c);

			OpenTK.Graphics.ES20.GL.ClearColor (c.X, c.Y, c.Z, c.W);

			var mask = OpenTK.Graphics.ES20.ClearBufferMask.ColorBufferBit;

			OpenTK.Graphics.ES20.GL.Clear ( (Int32) mask );

			OpenTKHelper.CheckError();
		}

#if aot
        public void ClearDepthBuffer(Single val)
#else
		public override void ClearDepthBuffer(Single val)
#endif
		{
			OpenTK.Graphics.ES20.GL.ClearDepth(val);

			var mask = OpenTK.Graphics.ES20.ClearBufferMask.DepthBufferBit;

			OpenTK.Graphics.ES20.GL.Clear ( (Int32) mask );

			OpenTKHelper.CheckError();
		}

#if aot
		public IGpuUtils GpuUtils
#else
        public override IGpuUtils GpuUtils
#endif
		{
			get
			{
				return gpuUtils;
			}
		}

#if aot
        public IGeometryBuffer CreateGeometryBuffer(
#else
		public override IGeometryBuffer CreateGeometryBuffer(
#endif
			VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount)
		{
			return new GeometryBuffer(vertexDeclaration, vertexCount, indexCount);
		}

#if aot
        public void SetActiveGeometryBuffer(IGeometryBuffer buffer)
#else
		public override void SetActiveGeometryBuffer(IGeometryBuffer buffer)
#endif
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

#if aot
        public IDisplayStatus DisplayStatus
#else
		public override IDisplayStatus DisplayStatus
#endif
		{
			get
			{
				return displayStatus;
			}
		}

#if aot
        public void DrawIndexedPrimitives(
#else
		public override void DrawIndexedPrimitives(
#endif
			PrimitiveType primitiveType,            // Describes the type of primitive to render. PrimitiveType.PointList is not supported with this method.
            Int32 baseVertex,                       // Offset to add to each vertex index in the index buffer.
            Int32 minVertexIndex,                   // Minimum vertex index for vertices used during the call. The minVertexIndex parameter and all of the indices in the index stream are relative to the baseVertex parameter.
            Int32 numVertices,                      // Number of vertices used during the call. The first vertex is located at index: baseVertex + minVertexIndex.
            Int32 startIndex,                       // Location in the index array at which to start reading vertices.
            Int32 primitiveCount                    // Number of primitives to render. The number of vertices used is a function of primitiveCount and primitiveType.
            )
		{

            if( baseVertex != 0 || minVertexIndex != 0 || startIndex != 0 )
            {
                throw new NotImplementedException();
            }

			var otkptype =  EnumConverter.ToOpenTK(primitiveType);

			var otkpAllType = (OpenTK.Graphics.ES20.All) otkptype;

			//Int32 numVertsInPrim = numVertices / primitiveCount;

			Int32 nVertsInPrim = PrimitiveHelper.NumVertsIn(primitiveType);
			Int32 count = primitiveCount * nVertsInPrim;

			var vertDecl = currentGeomBuffer.VertexBuffer.VertexDeclaration;

			this.EnableVertAttribs( vertDecl, (IntPtr) 0 );

			OpenTK.Graphics.ES20.GL.DrawElements (
				otkpAllType,
				count,
				OpenTK.Graphics.ES20.All.UnsignedShort,
				(System.IntPtr) 0 );

			OpenTKHelper.CheckError();

			this.DisableVertAttribs(vertDecl);

		}

#if aot
		public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPosition[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }
		public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionColour[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }
		public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionNormal[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }
		public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionNormalColour[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }
		public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionNormalTexture[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }
		public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionNormalTextureColour[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }
		public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionTexture[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }
		public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionTextureColour[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }

		void DrawUserPrimitivesHelper<T>(
#else
        public override void DrawUserPrimitives<T>(
#endif
			PrimitiveType primitiveType,
			T[] vertexData,
			int vertexOffset,
			int primitiveCount,
			VertexDeclaration vertexDeclaration)
#if aot
			where T : struct, IVertexType
#endif
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

			var glDrawMode = EnumConverter.ToOpenTK(primitiveType);
			var glDrawModeAll = (OpenTK.Graphics.ES20.All) glDrawMode;


			var bindTarget = (OpenTK.Graphics.ES20.All) OpenTK.Graphics.ES20.BufferObjects.ArrayBuffer;

			OpenTK.Graphics.ES20.GL.BindBuffer(bindTarget, 0);
			OpenTKHelper.CheckError();


			this.EnableVertAttribs( vertDecl, pointer );

			Int32 nVertsInPrim = PrimitiveHelper.NumVertsIn(primitiveType);
			Int32 count = primitiveCount * nVertsInPrim;

			OpenTK.Graphics.ES20.GL.DrawArrays(
				glDrawModeAll, // specifies the primitive to render
				vertexOffset,  // specifies the starting vertex index in the enabled vertex arrays
				count ); // specifies the number of indicies to be drawn

			OpenTKHelper.CheckError();


			this.DisableVertAttribs(vertDecl);


			pinnedArray.Free();

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
				OpenTK.Graphics.ES20.GL.EnableVertexAttribArray(counter);
				OpenTKHelper.CheckError();

				//var vertElemUsage = elem.VertexElementUsage;
				var vertElemFormat = elem.VertexElementFormat;
				var vertElemOffset = elem.Offset;

				Int32 numComponentsInVertElem = 0;
				Boolean vertElemNormalized = false;
				OpenTK.Graphics.ES20.DataType glVertElemFormat;

				EnumConverter.ToOpenTK(vertElemFormat, out glVertElemFormat, out vertElemNormalized, out numComponentsInVertElem);

				var type = (OpenTK.Graphics.ES20.All) glVertElemFormat;

				if( counter != 0)
				{
					ptr = Add(ptr, vertElemOffset);
				}

				OpenTK.Graphics.ES20.GL.VertexAttribPointer(
					counter,				// index - specifies the generic vertex attribute index.  This value is 0 to
											//         max vertex attributes supported - 1.
					numComponentsInVertElem,// size - number of components specified in the vertex array for the
											//        vertex attribute referenced by index.  Valid values are 1 - 4.
					type,					// type - Data format, valid values are GL_BYTE, GL_UNSIGNED_BYTE, GL_SHORT, GL_UNSIGNED_SHORT,
											//        GL_FLOAT, GL_FIXED, GL_HALF_FLOAT_OES*(Optional feature of es2)
					vertElemNormalized,		// normalised - used to indicate whether the non-floating data format type should be normalised
											//              or not when converted to floating point.
					vertDecl.VertexStride,	// stride - the components of vertex attribute specified by size are stored sequentially for each
											//          vertex.  stride specifies the delta between data for vertex index 1 and vertex (1 + 1).
											//          If stride is 0, attribute data for all vertices are stored sequentially.
											//          If stride is > 0, then we use the stride valude tas the pitch to get vertex data
											//          for the next index.
					ptr
					
					);

				OpenTKHelper.CheckError();

				counter++;

			}
		}

		void DisableVertAttribs(VertexDeclaration vertDecl)
		{
			var vertElems = vertDecl.GetVertexElements();

			for(int i = 0; i < vertElems.Length; ++i)
			{
				OpenTK.Graphics.ES20.GL.DisableVertexAttribArray(i);
				OpenTKHelper.CheckError();
			}
		}

#if aot
		public void SetBlendEquation(
#else
        public override void SetBlendEquation(
#endif
            BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb,
            BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha
            )
        {
            OpenTK.Graphics.ES20.GL.BlendEquationSeparate(
                EnumConverter.ToOpenTK(rgbBlendFunction),
                EnumConverter.ToOpenTK(alphaBlendFunction) );
            OpenTKHelper.CheckError();

            OpenTK.Graphics.ES20.GL.BlendFuncSeparate(
                EnumConverter.ToOpenTK(sourceRgb),
                EnumConverter.ToOpenTK(destinationRgb),
                EnumConverter.ToOpenTK(sourceAlpha),
                EnumConverter.ToOpenTK(destinationAlpha) );
            OpenTKHelper.CheckError();

        }

#if aot
		public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPosition[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }
		public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionColour[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }
		public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionNormal[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }
		public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionNormalColour[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }
		public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionNormalTexture[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }
		public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionNormalTextureColour[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }
		public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionTexture[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }
		public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionTextureColour[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }

		void DrawUserIndexedPrimitivesHelper<T>(
#else
        public override void DrawUserIndexedPrimitives<T>(
#endif
            PrimitiveType primitiveType,            // Describes the type of primitive to render.
            T[] vertexData,                         // The vertex data.
            Int32 vertexOffset,                     // Offset (in vertices) from the beginning of the vertex buffer to the first vertex to draw.
            Int32 numVertices,                      // Number of vertices to draw.
            Int32[] indexData,                      // The index data.
            Int32 indexOffset,                      // Offset (in indices) from the beginning of the index buffer to the first index to use.
            Int32 primitiveCount,                   // Number of primitives to render.
            VertexDeclaration vertexDeclaration)
#if aot
			where T : struct, IVertexType
#endif
        {
            throw new NotImplementedException();
        }

#if aot
        public void DrawPrimitives(
#else
		public override void DrawPrimitives(
#endif
            PrimitiveType primitiveType,            // Describes the type of primitive to render.
            Int32 startVertex,                      // Index of the first vertex to load. Beginning at startVertex, the correct number of vertices is read out of the vertex buffer.
            Int32 primitiveCount)                   // Number of primitives to render. The primitiveCount is the number of primitives as determined by the primitive type. If it is a line list, each primitive has two vertices. If it is a triangle list, each primitive has three vertices.
		{
            throw new NotImplementedException();
        }


	}


}