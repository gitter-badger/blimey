using System;

namespace Sungiant.Blimey.PsmRuntime
{
	public class GeometryBuffer
		: IGeometryBuffer
	{
		Sce.Pss.Core.Graphics.VertexBuffer _pssVertexBuf;
		
		IndexBuffer _iBuf;
		VertexBuffer _vBuf;
		
		public GeometryBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount)
		{
			var pssDecl = vertexDeclaration.ToPSS();
			_pssVertexBuf = new Sce.Pss.Core.Graphics.VertexBuffer(vertexCount, indexCount, pssDecl);
			
			_iBuf = new IndexBuffer(_pssVertexBuf);
			_vBuf = new VertexBuffer(_pssVertexBuf, vertexDeclaration);
		}
		
		public IVertexBuffer VertexBuffer { get { return _vBuf; } }
		public IIndexBuffer IndexBuffer { get { return _iBuf; } }
		
		public Sce.Pss.Core.Graphics.VertexBuffer PSSVertexBuffer { get { return _pssVertexBuf; } }
		
	}
}

