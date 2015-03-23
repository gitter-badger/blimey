using System;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Cor;
using Platform;
using Platform.MonoMac;
using System.IO;

namespace Cor.Demo
{
	class AppDelegate
		: NSApplicationDelegate
        , IDisposable
	{
        Engine engine;

#if LOGGED_PROXY
        readonly FileStream memory;
        readonly StreamWriter writer;
#endif

		public override void FinishedLaunching (NSObject notification)
		{
            var appSettings = new AppSettings ("Cor") {
                FullScreen = true,
                MouseGeneratesTouches = true
            };

            var entryPoint = new BasicApp();

            var platform = new MonoMacPlatform ();

#if LOGGED_PROXY
            memory = new FileStream (Path.Combine (Environment.GetEnvironmentVariable ("HOME"), "log.txt"), FileMode.Create);
            writer = new StreamWriter (memory);
            engine = new Engine(new LoggedProxyPlatform (platform, writer), appSettings, entryPoint);
#else
            engine = new Engine(
                platform,
                appSettings,
                entryPoint);
#endif
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
        
        public new void Dispose ()
        {
            engine.Dispose ();
#if LOGGED_PROXY
            writer.Dispose ();
            memory.Dispose ();
#endif
            base.Dispose ();
        }
	}
}

