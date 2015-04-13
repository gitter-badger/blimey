namespace PlatformDemo
{
    using System;
    using MonoMac.Foundation;
    using MonoMac.AppKit;
    using Blimey.Platform;

	class AppDelegate: NSApplicationDelegate, IDisposable
	{
        Platform platform;

		public override void FinishedLaunching (NSObject notification)
		{
            var appSettings = new AppSettings ("Blimey Platform Demo") {
                FullScreen = true,
                MouseGeneratesTouches = true
            };

            var entryPoint = new BasicApp ();

            IApi api = new Api ();

            platform = new Platform (api);
            platform.Start (appSettings, entryPoint);
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
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

