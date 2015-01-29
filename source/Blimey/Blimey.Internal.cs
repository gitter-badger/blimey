// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Blimey - Fast, efficient, high level engine built upon Cor & Abacus                                            │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │                     Brought to you by:                                                                         │ \\
// │                              _________                    .__               __                                 │ \\
// │                             /   _____/__ __  ____    ____ |__|____    _____/  |_                               │ \\
// │                             \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\                              │ \\
// │                             /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |                                │ \\
// │                            /_______  /____/|___|  /\___  /|__(____  /___|  /__|                                │ \\
// │                                    \/           \//_____/         \/     \/                                    │ \\
// │                                                                                                                │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2008-2014 A.J.Pook (http://ajpook.github.io)                                                       │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors: A.J.Pook                                                                                              │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated   │ \\
// │ documentation files (the "Software"), to deal in the Software without restriction, including without           │ \\
// │ limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sellcopies of the   │ \\
// │ Software, and to permit persons to whom the Software is furnished to do so, subject to the following           │ \\
// │ conditions:                                                                                                    │ \\
// │                                                                                                                │ \\
// │ The above copyright notice and this permission notice shall be included in all copies or substantial portions  │ \\
// │ of the Software.                                                                                               │ \\
// │                                                                                                                │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED  │ \\
// │ TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL │ \\
// │ THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF  │ \\
// │ CONTRACT, TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER        │ \\
// │ DEALINGS IN THE SOFTWARE.                                                                                      │ \\
// └────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘ \\

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    
    using Fudge;
    using Abacus.SinglePrecision;
    
    using System.Linq;
    using Cor;


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal static class FrameStats
    {
        static readonly Dictionary <String, Double> timers = new Dictionary <String, Double> ();

        static FrameStats ()
        {
            Reset ();
        }

        public static void Add (String key, Double delta)
        {
            if (!timers.ContainsKey (key))
                timers [key] = 0;
            timers [key] += delta;
        }

        public static void Reset ()
        {
            timers.Clear ();
        }

        static Int32 counter = 0;
        public static void SlowLog ()
        {
            if (counter++ % 30 != 0)
                return;
            // Right now we are targeting 30 FPS
            // and have allocated 10ms to update
            // and 10ms to render per frame which
            // gives us plenty of headroom.

            foreach (var key in timers.Keys)
            {
                if (key == "DrawUserPrimitivesCount" || key == "DrawIndexedPrimitivesCount")
                {
                    if (timers [key] > 50)
                        Console.WriteLine (String.Format ("{0} -> {1}", key, timers [key]));
                }
                else if (key == "RenderTime" || key == "UpdateTime")
                {
                    if (timers [key] > 10)
                        Console.WriteLine (String.Format ("{0} -> {1:0.##}ms", key, timers [key]));
                }
                else
                {
                    if (timers [key] > 0.5)
                        Console.WriteLine (String.Format ("{0} -> {1:0.##}ms", key, timers [key]));
                }
            }
        }
    }
}
