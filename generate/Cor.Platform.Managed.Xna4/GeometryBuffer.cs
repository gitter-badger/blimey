using System;
using Sungiant.Cor;

namespace Sungiant.Cor.Platform.Managed.Xna4
{
	public class GeometryBuffer
		: IGeometryBuffer
	{
		VertexBufferWrapper _vertexBufWrap;
		IndexBufferWrapper _indexBufWrap;
		

		public GeometryBuffer(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount)
		{
			_vertexBufWrap = new VertexBufferWrapper(graphicsDevice, vertexDeclaration, vertexCount);
			_indexBufWrap = new IndexBufferWrapper(graphicsDevice, indexCount);
		}

		public IVertexBuffer VertexBuffer { get { return _vertexBufWrap; } }
		public IIndexBuffer IndexBuffer { get { return _indexBufWrap; } }
	}
}