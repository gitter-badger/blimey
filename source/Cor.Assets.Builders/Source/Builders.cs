using System;
using MonoMac.AppKit;

namespace Cor.Assets.Builders
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

