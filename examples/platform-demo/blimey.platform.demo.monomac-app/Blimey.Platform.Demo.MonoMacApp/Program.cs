using System;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Cor.Demo
{
	class Program
	{
		static void Main (string [] args)
		{
			NSApplication.Init ();

			using (var p = new NSAutoreleasePool ()) {
				NSApplication.SharedApplication.Delegate = new AppDelegate();
				NSApplication.Main(args);
			}
		}
	}
}

