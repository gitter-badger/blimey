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

namespace Sungiant.Cor.MonoTouchRuntime
{
	public class SystemManager
        : BaseRuntime.SystemManager
	{
		TouchScreen screen;

		public SystemManager(TouchScreen screen)
		{
			this.screen = screen;

			MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
				MonoTouch.UIKit.UIApplication.DidEnterBackgroundNotification, this.DidEnterBackground );

			MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
				MonoTouch.UIKit.UIApplication.DidBecomeActiveNotification, this.DidBecomeActive );
			
			MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
				MonoTouch.UIKit.UIApplication.DidReceiveMemoryWarningNotification, this.DidReceiveMemoryWarning );

			MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
				MonoTouch.UIKit.UIApplication.DidFinishLaunchingNotification, this.DidFinishLaunching );

			MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
				MonoTouch.UIKit.UIDevice.OrientationDidChangeNotification, this.OrientationDidChange );

		}

		public override String OperatingSystem
		{
			get
			{
				return System.Environment.OSVersion.VersionString;
			}
		}

		public void DidReceiveMemoryWarning(MonoTouch.Foundation.NSNotification ntf)
		{
			Console.WriteLine("[Cor.System] DidReceiveMemoryWarning");
		}

		public void DidBecomeActive(MonoTouch.Foundation.NSNotification ntf)
		{
			Console.WriteLine("[Cor.System] DidBecomeActive");
		}

		public void DidEnterBackground(MonoTouch.Foundation.NSNotification ntf)
		{
			Console.WriteLine("[Cor.System] DidEnterBackground");
		}
		
		public void DidFinishLaunching(MonoTouch.Foundation.NSNotification ntf)
		{
			Console.WriteLine("[Cor.System] DidFinishLaunching");
		}

		public void OrientationDidChange(MonoTouch.Foundation.NSNotification ntf)
		{
			Console.WriteLine("[Cor.System] OrientationDidChange, CurrentOrientation: " + CurrentOrientation.ToString() 
			                  + ", CurrentDisplaySize: " + CurrentDisplaySize.ToString());

		}

		public override String DeviceName
		{
			get
			{
				return MonoTouch.UIKit.UIDevice.CurrentDevice.Name;
			}
		}

		public override String DeviceModel
		{
			get
			{
				return MonoTouch.UIKit.UIDevice.CurrentDevice.Model;
			}
		}

		public override String SystemName
		{
			get
			{
				return MonoTouch.UIKit.UIDevice.CurrentDevice.SystemName;
			}
		}

		public override String SystemVersion
		{
			get
			{
				return MonoTouch.UIKit.UIDevice.CurrentDevice.SystemVersion;
			}
		}

		public override DeviceOrientation CurrentOrientation
		{
			get
			{
				var monoTouchOrientation = MonoTouch.UIKit.UIDevice.CurrentDevice.Orientation;

				return EnumConverter.ToCor(monoTouchOrientation);
			}
		}


		public override IScreenSpecification ScreenSpecification
		{
			get
			{
				return this.screen;
			}
		}

		public override IPanelSpecification PanelSpecification
		{
			get
			{
				return this.screen;
			}
		}
	}
}

