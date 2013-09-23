using System;

namespace Sungiant.Blimey.PsmRuntime
{
	public class SystemManager
		: ISystemManager
	{
		TouchScreen touchScreen;
		
		public SystemManager(
			IEngine engine, 
			Sce.Pss.Core.Graphics.GraphicsContext gfxContext,
			TouchScreen touchScreen)
		{
			this.touchScreen = touchScreen;
		}

		public string OS
		{ 
			get
			{
				return System.Environment.OSVersion.Platform.ToString();
			}
		}

		internal void SetDeviceOrientation(Blimey.DeviceOrientation orientation)
		{
			_orientation = orientation;
		}

		DeviceOrientation _orientation = Blimey.DeviceOrientation.Default;
		public DeviceOrientation DeviceOrientation
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
				return touchScreen;
			}
		}

		public IPanelSpecification PanelSpecification
		{
			get
			{
				return touchScreen;
			}
		}
	}
}

