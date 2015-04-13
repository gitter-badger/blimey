namespace EngineDemo
{
    using System;
    using MonoMac.Foundation;
    using MonoMac.AppKit;
    using Blimey.Engine;
    using Blimey.Platform;

	class AppDelegate: NSApplicationDelegate, IDisposable
	{
        Platform platform;

		public override void FinishedLaunching (NSObject notification)
		{
            var appSettings = new AppSettings ("Blimey Engine Demo") {
                FullScreen = true,
                MouseGeneratesTouches = true
            };

            var entryPoint = new Demo ();
            var api = new Api ();

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

