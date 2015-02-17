using System;
using Cor;
using Platform.Xios;

namespace Blimey.Demo
{
	[MonoTouch.Foundation.Register ("AppDelegate")]
	public partial class AppDelegate
		: MonoTouch.UIKit.UIApplicationDelegate
        , IDisposable
	{
		Engine engine;

		// This method is invoked when the application has
		// loaded its UI and is ready to run
		public override Boolean FinishedLaunching (
			MonoTouch.UIKit.UIApplication app,
			MonoTouch.Foundation.NSDictionary options)
		{
            var appSettings = new AppSettings ("Blimey Demo"){
                FullScreen = true,
                MouseGeneratesTouches = true
            };

            var entryPoint = new Demo ();

            engine = new Engine(
                new XiosPlatform (),
                appSettings,
                entryPoint);

            return true;
        }

        public new void Dispose ()
        {
            engine.Dispose ();
            base.Dispose ();
        }
	}
}
