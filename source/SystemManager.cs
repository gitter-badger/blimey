using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sungiant.Cor;
using Sungiant.Abacus.Int32Precision;

namespace Sungiant.Cor.Xna4Runtime
{

	public class SystemManager
		: ISystemManager
	{
		ScreenImplementation mainDisplayPanel;
		

		internal ScreenImplementation MainDisplayPanel
		{
			get
			{
				return mainDisplayPanel;
			}
		}

		public void GetEffectiveDisplaySize(ref Int32 frameBufferWidth, ref Int32 frameBufferHeight)
		{
			if (this.CurrentOrientation == DeviceOrientation.Default ||
				this.CurrentOrientation == DeviceOrientation.Upsidedown)
			{
				return;
			}
			else
			{
				Int32 temp = frameBufferWidth;
				frameBufferWidth = frameBufferHeight;
				frameBufferHeight = frameBufferWidth;
			}

		}

		public SystemManager(ICor engine, Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager)
		{
			
			mainDisplayPanel = new ScreenImplementation(engine, gfxManager.GraphicsDevice);
		}

        public Point2 CurrentDisplaySize
        {
            get
            {
                Int32 w = ScreenSpecification.ScreenResolutionWidth;
                Int32 h = ScreenSpecification.ScreenResolutionHeight;

                GetEffectiveDisplaySize(ref w, ref h);

                return new Point2(w, h);

            }
        }

		public String OperatingSystem { get { return System.Environment.OSVersion.Platform.ToString(); } }

		public String DeviceName { get { return string.Empty; } }

		public String DeviceModel { get { return string.Empty; } }

		public String SystemName { get { return string.Empty; } }

		public String SystemVersion { get { return string.Empty; } }


		internal void SetDeviceOrientation(DeviceOrientation orientation)
		{
			_orientation = orientation;
		}

		DeviceOrientation _orientation = DeviceOrientation.Default;
		public DeviceOrientation CurrentOrientation
		{
			get
			{
				return _orientation;
			}
			internal set
			{
				_orientation = value;
			}
		}

		public IScreenSpecification ScreenSpecification
		{
			get
			{
				return this.mainDisplayPanel;
			}
		}

		public IPanelSpecification PanelSpecification
		{
			get
			{
				return this.mainDisplayPanel;
			}
		}
	}
}
