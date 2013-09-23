using System;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Cor;

namespace Sungiant.Cor.Xna4Runtime
{
	public class ScreenImplementation
		: IPanelSpecification
		, IScreenSpecification
	{
		Microsoft.Xna.Framework.Graphics.GraphicsDevice gfxDevice;
		Vector2 realWorldSize = Vector2.Zero;

		ICor engine;

		public ScreenImplementation(ICor engine, Microsoft.Xna.Framework.Graphics.GraphicsDevice gfxDevice)
		{
			this.engine = engine;
			this.gfxDevice = gfxDevice;

			this.EstimatePhysicalSize();
		}

		void EstimatePhysicalSize()
		{
#if WINDOWS
			realWorldSize = new Vector2(
				this.ScreenResolutionWidth, 
				this.ScreenResolutionHeight
				) / 5000f;
#endif

#if WP7
			// do lookup here into all device types
			realWorldSize = new Vector2(0.048f, 0.08f);
#endif


#if XBOX
			//guess 
			realWorldSize = new Vector2(0.8f, 0.45f);
#endif
		}

		public float ScreenResolutionAspectRatio
		{
			get
			{
				return this.gfxDevice.Adapter.CurrentDisplayMode.AspectRatio;
			}
		}

		public Int32 ScreenResolutionHeight
		{
			get
			{
				return this.gfxDevice.Adapter.CurrentDisplayMode.Height;
			}
		}

		public Int32 ScreenResolutionWidth
		{
			get
			{
				return this.gfxDevice.Adapter.CurrentDisplayMode.Width;
			}
		}

		public Vector2 PanelPhysicalSize
		{
			get
			{
				return realWorldSize;
			}
		}

		public float PanelPhysicalAspectRatio
		{
			get
			{
				return realWorldSize.X / realWorldSize.Y;
			}
		}

		public PanelType PanelType
		{ 
			get 
			{

#if TARGET_XBOX
				return PanelType.Screen; 
#endif

#if TARGET_WINDOWS_PHONE
				return PanelType.TouchScreen;
#endif

#if TARGET_WINDOWS

				if (engine.Settings.MouseGeneratesTouches)
				{
					return PanelType.TouchScreen;
				}
				else
				{
					return PanelType.Screen; 
				}
#endif
			} 

		}
	}
}
