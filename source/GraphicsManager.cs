using System;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Cor;

namespace Sungiant.Cor.Xna4Runtime
{
	internal class DisplayStatus
		: IDisplayStatus
	{
		Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager;

		internal DisplayStatus(Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager)
		{
			this.gfxManager = gfxManager;
		}

		public Boolean Fullscreen { get { return this.gfxManager.IsFullScreen; } }

		// What is the size of the frame buffer?
		// On most devices this will be the same as the screen size.
		// However on a PC or Mac the app could be running in windowed mode
		// and not take up the whole screen.
		public Int32 CurrentWidth { get { return this.gfxManager.GraphicsDevice.PresentationParameters.BackBufferWidth; } }
		public Int32 CurrentHeight { get { return this.gfxManager.GraphicsDevice.PresentationParameters.BackBufferHeight; } }
	}

	public class GraphicsManager
		: IGraphicsManager
	{
		Microsoft.Xna.Framework.GraphicsDeviceManager _xnaGfxDeviceManager;
		GpuUtils _gpuUtils;
		DisplayStatus displayStatus;

		public GraphicsManager(ICor engine, Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager)
		{
			_xnaGfxDeviceManager = gfxManager;

			_xnaGfxDeviceManager.GraphicsDevice.RasterizerState = Microsoft.Xna.Framework.Graphics.RasterizerState.CullNone;
			_xnaGfxDeviceManager.GraphicsDevice.BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Opaque;
			_xnaGfxDeviceManager.GraphicsDevice.DepthStencilState = Microsoft.Xna.Framework.Graphics.DepthStencilState.Default;

			_gpuUtils = new GpuUtils();

			displayStatus = new DisplayStatus(_xnaGfxDeviceManager);
		}

		public IDisplayStatus DisplayStatus
		{
			get
			{
				return displayStatus;
			}
		}
		public void ClearColourBuffer(Rgba32 col)
		{

			var xnaCol = col.ToXNA();

			_xnaGfxDeviceManager.GraphicsDevice.Clear(xnaCol);
		}

		public void ResetBetweenScenes()
		{

		}

		public void ClearDepthBuffer(Single z)
		{
			
			_xnaGfxDeviceManager.GraphicsDevice.Clear(
				Microsoft.Xna.Framework.Graphics.ClearOptions.DepthBuffer,
				Microsoft.Xna.Framework.Vector4.Zero, 
				z, 
				0 );
		}

		public IGpuUtils GpuUtils
		{
			get
			{
				return _gpuUtils;
			}
		}
		public IGeometryBuffer CreateGeometryBuffer(VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount)
		{
			return new GeometryBuffer(_xnaGfxDeviceManager.GraphicsDevice, vertexDeclaration, vertexCount, indexCount);
		}

		public void SetGeometryBuffer(IGeometryBuffer buffer)
		{
			var vbuf = buffer.VertexBuffer as VertexBufferWrapper;

			_xnaGfxDeviceManager.GraphicsDevice.SetVertexBuffer(vbuf.XNAVertexBuffer);

			var ibuf = buffer.IndexBuffer as IndexBufferWrapper;

			_xnaGfxDeviceManager.GraphicsDevice.Indices = ibuf.XNAIndexBuffer;
		}


		public void DrawIndexedPrimitives(
			PrimitiveType primitiveType,
			int numVertices,
			int primitiveCount)
		{
			var xnaPrimType = EnumConverter.ToXNA(primitiveType);
			_xnaGfxDeviceManager.GraphicsDevice.DrawIndexedPrimitives(xnaPrimType, 0, 0, numVertices, 0, primitiveCount);
		}

		public void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct, IVertexType
		{
			var xnaPrimType = EnumConverter.ToXNA(primitiveType);
			var xnaVertDecl = vertexDeclaration.ToXNA();

			_xnaGfxDeviceManager.GraphicsDevice.DrawUserPrimitives(
				xnaPrimType, vertexData, vertexOffset, primitiveCount, xnaVertDecl);
		}
	}
}