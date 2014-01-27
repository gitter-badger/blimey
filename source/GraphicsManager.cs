﻿using System;
using Sungiant.Abacus;
using System.Collections.Generic;

namespace Sungiant.Blimey.PsmRuntime
{
	public class GraphicsManager
		: IGraphicsManager
	{
		Sce.Pss.Core.Graphics.GraphicsContext _graphicsContext;
		
		IGeometryBuffer _currentGeomBuffer;
		GpuUtils _gpuUtils;
		
		
		Int32 _drawCallCount = 0;
		
		public Int32 DrawCallCount { get { return _drawCallCount; } }
		
		public void MarkNewFrame(){ _drawCallCount = 0; }
		
		public GraphicsManager(Sce.Pss.Core.Graphics.GraphicsContext graphicsContext)
		{

			_graphicsContext = graphicsContext;
			_gpuUtils = new GpuUtils();
			
			_graphicsContext.Enable( Sce.Pss.Core.Graphics.EnableMode.CullFace, false );
			_graphicsContext.Enable( Sce.Pss.Core.Graphics.EnableMode.Blend, true );
			_graphicsContext.Enable( Sce.Pss.Core.Graphics.EnableMode.DepthTest, true );
		
		}

		public void Clear(Colour col)
		{
			_graphicsContext.SetClearColor (col.R, col.G, col.B, col.A);
			_graphicsContext.Clear ();
		}
		
		public void ClearDepthBuffer()
		{
			_graphicsContext.SetClearDepth(1f);
			_graphicsContext.Clear (Sce.Pss.Core.Graphics.ClearMask.Depth);
		}

		public IGeometryBuffer CreateGeometryBuffer(
			VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount)
		{
			return new GeometryBuffer(vertexDeclaration, vertexCount, indexCount);
		}
		
		public IGpuUtils GpuUtils
		{
			get
			{
				return _gpuUtils;
			}
		}
		
		public void SetGeometryBuffer(IGeometryBuffer buffer)
		{
			var pssGeomBuff = buffer as GeometryBuffer;
			
			_graphicsContext.SetVertexBuffer(0, pssGeomBuff.PSSVertexBuffer);
			
			_currentGeomBuffer = buffer;
		}

		public void DrawIndexedPrimitives(
			PrimitiveType primitiveType, 
			int baseVertex, 
			int minVertexIndex, 
			int numVertices, 
			int startIndex, 
			int primitiveCount)
		{
			var pssPrimType = EnumConverter.ToPSS(primitiveType);
			
			int numVertsInPrim = numVertices / primitiveCount;
			
			_graphicsContext.DrawArrays(pssPrimType, 0, primitiveCount * PrimitiveHelper.NumVertsIn(primitiveType));
		}

		public void DrawUserPrimitives<T>(
			PrimitiveType primitiveType, 
			T[] vertexData, 
			int vertexOffset, 
			int primitiveCount, 
			VertexDeclaration vertexDeclaration) 
			where T : struct, IVertexType
		{
						

			var pssDrawMode = EnumConverter.ToPSS(primitiveType);
			
			var geomBuff = this.CreateGeometryBuffer(vertexDeclaration, vertexData.Length, 0) as GeometryBuffer;
			
			geomBuff.VertexBuffer.SetData(vertexData);
			
			this.SetGeometryBuffer(geomBuff);
			
			_graphicsContext.DrawArrays(pssDrawMode, 0, primitiveCount * PrimitiveHelper.NumVertsIn(primitiveType));
			
			geomBuff.PSSVertexBuffer.Dispose();
			
		}

	}


}