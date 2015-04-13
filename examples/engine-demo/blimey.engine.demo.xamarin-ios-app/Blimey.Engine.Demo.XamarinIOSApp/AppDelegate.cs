namespace EngineDemo
{
    using System;
    using Blimey.Platform;

	[MonoTouch.Foundation.Register ("AppDelegate")]
	public partial class AppDelegate
		: MonoTouch.UIKit.UIApplicationDelegate
        , IDisposable
    {
        Platform platform;

		// This method is invoked when the application has
		// loaded its UI and is ready to run
		public override Boolean FinishedLaunching (
			MonoTouch.UIKit.UIApplication app,
			MonoTouch.Foundation.NSDictionary options)
		{
            var appSettings = new AppSettings ("Engine Demo") {
                FullScreen = true,
                MouseGeneratesTouches = true
            };

            var entryPoint = new Demo ();
            var api = new Api ();

            platform = new Platform (api);
            platform.Start (appSettings, entryPoint);

			return true;
		}

        public new void Dispose ()
        {
            platform.Stop ();
            platform.Dispose ();
            base.Dispose ();
        }
	}
}
