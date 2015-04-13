namespace PlatformDemo
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
            var appSettings = new AppSettings ("Blimey Platform Demo") {
                MouseGeneratesTouches = true
            };

            var entryPoint = new BasicApp();

            IApi xamarinIOSApiImplementation = new Api ();
            platform = new Platform (xamarinIOSApiImplementation);
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
