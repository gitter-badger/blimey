using System;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Cor;
using Platform.MonoMac;

namespace Cor.Demo
{
	class AppDelegate: NSApplicationDelegate, IDisposable
	{
        Engine engine;

		public override void FinishedLaunching (NSObject notification)
		{
            var appSettings = new AppSettings ("Cor") {
                FullScreen = true,
                MouseGeneratesTouches = true
            };

            var entryPoint = new BasicApp();
         
            var platform = new MonoMacPlatform ();

            engine = new Engine (platform, appSettings, entryPoint);
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
        
        public new void Dispose ()
        {
            engine.Dispose ();
            base.Dispose ();
        }
	}
}

