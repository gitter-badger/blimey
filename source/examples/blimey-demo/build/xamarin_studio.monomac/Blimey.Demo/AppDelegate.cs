//#define LOGGED_PROXY

using System;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Platform.MonoMac;
using Cor;
using System.IO;

namespace Blimey.Demo
{
    class AppDelegate
        : NSApplicationDelegate
    {
        Engine engine;

#if LOGGED_PROXY
        FileStream memory;
        StreamWriter writer;
#endif

        public override void FinishedLaunching (NSObject notification)
        {
            var appSettings = new AppSettings ("Cor Demo"){
                FullScreen = true,
                MouseGeneratesTouches = true
            };

            var entryPoint = new Demo ();
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

