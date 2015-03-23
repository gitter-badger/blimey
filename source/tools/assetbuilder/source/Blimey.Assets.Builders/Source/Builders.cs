using System;
#if OSX
using MonoMac.AppKit;
#endif

namespace Blimey.Assets.Builders
{
    public static class Builders
    {
        public static void Init ()
        {
            #if OSX
            NSApplication.Init ();
            #endif
        }
    }
}

