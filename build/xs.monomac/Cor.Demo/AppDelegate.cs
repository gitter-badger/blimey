// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor.Demo MonoMac Startup Project                                       │ \\
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
// │ Copyright © 2014 A.J.Pook (http://ajpook.github.io)                    │ \\
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

