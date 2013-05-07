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
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

using Sungiant.Abacus;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Abacus.Int32Precision;

namespace Sungiant.Cor.BaseRuntime
{
	#region BaseRuntime

	public abstract class AudioManager
		: IAudioManager
	{
	}
#if !AOT
    public abstract class GraphicsManager
        : IGraphicsManager
    {
        public virtual IDisplayStatus DisplayStatus
		{ 
			get			
			{
				throw new NotImplementedException();	
			}
		}
        
        public virtual IGpuUtils GpuUtils
		{ 
			get	
			{
				throw new NotImplementedException();	
			}
		}
        
        public virtual void Reset()
        {
            this.ClearDepthBuffer();
            this.ClearColourBuffer();

            this.SetActiveGeometryBuffer(null);
            this.SetActiveTexture(0, null);
        }

        public virtual void ClearColourBuffer(Rgba32 colour = new Rgba32())
		{
			throw new NotImplementedException();	
		}

        public virtual void ClearDepthBuffer(Single depth = 1f)
		{
			throw new NotImplementedException();	
		}

        public virtual IGeometryBuffer CreateGeometryBuffer(
            VertexDeclaration vertexDeclaration,
            Int32 vertexCount,
            Int32 indexCount)
			
		{
			throw new NotImplementedException();	
		}

        public virtual void SetActiveGeometryBuffer(IGeometryBuffer buffer)		
		{
			throw new NotImplementedException();	
		}

        public virtual void SetActiveTexture(Int32 slot, Texture2D tex)
		{
			throw new NotImplementedException();	
		}

        public virtual void SetBlendEquation(
            BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb,
            BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha
            )
		{
			throw new NotImplementedException();	
		}

        public virtual void DrawPrimitives(
            PrimitiveType primitiveType,
            Int32 startVertex,
            Int32 primitiveCount)
		{
			throw new NotImplementedException();	
		}

        public virtual void DrawIndexedPrimitives (
            PrimitiveType primitiveType,
            Int32 baseVertex,
            Int32 minVertexIndex,
            Int32 numVertices,
            Int32 startIndex,
            Int32 primitiveCount
            )
		{
			throw new NotImplementedException();	
		}
        
        public virtual void DrawUserPrimitives<T>(
            PrimitiveType primitiveType, 
            T[] vertexData,
            Int32 vertexOffset,
            Int32 primitiveCount,
            VertexDeclaration vertexDeclaration)
            where T : struct, IVertexType
		{
			throw new NotImplementedException();	
		}
        
        public virtual void DrawUserIndexedPrimitives<T>(
            PrimitiveType primitiveType,
            T[] vertexData,
            Int32 vertexOffset,
            Int32 numVertices,
            Int32[] indexData,
            Int32 indexOffset,
            Int32 primitiveCount,
            VertexDeclaration vertexDeclaration) 
            where T : struct, IVertexType
		{
			throw new NotImplementedException();	
		}
    }

#endif

	public abstract class IndexBuffer
		: IIndexBuffer
	{
		static internal UInt16[] ConvertToUnsigned (Int32[] indexBuffer)
		{	
			UInt16[] udata = new UInt16[indexBuffer.Length];

			for(Int32 i = 0; i < indexBuffer.Length; ++i)
			{
				udata[i] = (UInt16) indexBuffer[i];
			}
			
			return udata;
		}
		
		public virtual void GetData(Int32[] data)
		{
			throw new NotImplementedException();	
		}

		public virtual void GetData(Int16[] data, Int32 startIndex, Int32 elementCount)
		{
			throw new NotImplementedException();	
		}

		public virtual void GetData(Int32 offsetInBytes, Int16[] data, Int32 startIndex, Int32 elementCount)
		{
			throw new NotImplementedException();	
		}

		public virtual void SetData(Int32[] data)
		{
			throw new NotImplementedException();	
		}

		public virtual void SetData(Int16[] data, Int32 startIndex, Int32 elementCount)
		{
			throw new NotImplementedException();	
		}

		public virtual void SetData(Int32 offsetInBytes, Int16[] data, Int32 startIndex, Int32 elementCount)
		{
			throw new NotImplementedException();	
		}

		public virtual Int32 IndexCount
		{ 
			get	
			{
				throw new NotImplementedException();	
			}
		}
	}
    public abstract class InputManager
        : IInputManager
    {
		public abstract void Update(AppTime time);
		
        public virtual Xbox360Gamepad GetXbox360Gamepad(PlayerIndex player)
        {
            return null;
        }
        
        public virtual MultiTouchController GetMultiTouchController()
        {
            return null;
        }
        
        public virtual PsmGamepad GetPsmGamepad()
        {
            return null;
        }
        
        public virtual GenericGamepad GetGenericGamepad()
        {
            return null;
        }
    }

#if !AOT

	public abstract class ResourceManager
		: IResourceManager
	{
		public virtual T Load<T>(String path) 
            where T : IResource	
		{
			throw new NotImplementedException();	
		}

		public virtual IShader LoadShader(ShaderType shaderType)
		{
			throw new NotImplementedException();	
		}
	}
	
#endif
    public abstract class ScreenSpecification
        : IScreenSpecification
    {
        public virtual Int32 ScreenResolutionWidth
		{ 
			get	
			{
				throw new NotImplementedException();	
			}
		}
        
        public virtual Int32 ScreenResolutionHeight
		{ 
			get	
			{
				throw new NotImplementedException();	
			}
		}
        
        public Single ScreenResolutionAspectRatio
        {
            get 
            { 
                return 
                    (Single)this.ScreenResolutionWidth / 
                    (Single)this.ScreenResolutionHeight;
            }
        }
    }


	public abstract class SystemManager
		: ISystemManager
	{
		public virtual Point2 CurrentDisplaySize
		{
			get
			{
				Int32 w = ScreenSpecification.ScreenResolutionWidth;
				Int32 h = ScreenSpecification.ScreenResolutionHeight;

				GetEffectiveDisplaySize(ref w, ref h);

				return new Point2(w, h);

			}
		}

		void GetEffectiveDisplaySize(ref Int32 screenSpecWidth, ref Int32 screenSpecHeight)
		{
			if (this.CurrentOrientation == DeviceOrientation.Default ||
			    this.CurrentOrientation == DeviceOrientation.Upsidedown )
			{
				return;
			}
			else
			{
				Int32 temp = screenSpecWidth;
				screenSpecWidth = screenSpecHeight;
				screenSpecHeight = temp;
			}
		}

		public virtual String OperatingSystem
		{ 
			get	
			{
				throw new NotImplementedException();	
			}
		}

		public virtual String DeviceName
		{ 
			get	
			{
				throw new NotImplementedException();	
			}
		}

		public virtual String DeviceModel
		{ 
			get	
			{
				throw new NotImplementedException();	
			}
		}

		public virtual String SystemName
		{ 
			get	
			{
				throw new NotImplementedException();	
			}
		}

		public virtual String SystemVersion
		{ 
			get	
			{
				throw new NotImplementedException();	
			}
		}

		public virtual DeviceOrientation CurrentOrientation
		{ 
			get	
			{
				throw new NotImplementedException();	
			}
		}

		public virtual IScreenSpecification ScreenSpecification
		{ 
			get	
			{
				throw new NotImplementedException();	
			}
		}

		public virtual IPanelSpecification PanelSpecification
		{ 
			get	
			{
				throw new NotImplementedException();	
			}
		}
	}

#if !AOT

	public abstract class VertexBuffer
		: IVertexBuffer
	{
		public virtual Int32 VertexCount
		{ 
			get	
			{
				throw new NotImplementedException();	
			}
		}
		
		public virtual void SetData<T> (T[] data) 
			where T
				: struct
				, IVertexType	
		{
			throw new NotImplementedException();	
		}

		public virtual VertexDeclaration VertexDeclaration
		{ 
			get	
			{
				throw new NotImplementedException();	
			}
		}


		// not yet implemented
		public virtual void GetData<T> (T[] data) 
			where T
				: struct
				, IVertexType
		{
			throw new NotImplementedException();	
		}

		public virtual void GetData<T> (
			T[] data, 
			Int32 startIndex, 
			Int32 elementCount) 
			where T
				: struct
				, IVertexType
		{
			throw new NotImplementedException();	
		}

		public virtual void GetData<T> (
			Int32 offsetInBytes, 
			T[] data, 
			Int32 startIndex, 
			Int32 elementCount, 
			Int32 vertexStride) 
			where T
				: struct
				, IVertexType
		{
			throw new NotImplementedException();	
		}

		public virtual void SetData<T> (
			T[] data, 
			Int32 startIndex, 
			Int32 elementCount) 
			where T
				: struct
				, IVertexType
		{
			throw new NotImplementedException();	
		}

		public virtual void SetData<T> (
			Int32 offsetInBytes, 
			T[] data, 
			Int32 startIndex, 
			Int32 elementCount, 
			Int32 vertexStride) 
			where T
				: struct
				, IVertexType
		{
			throw new NotImplementedException();	
		}

	}

#endif

	#endregion
}