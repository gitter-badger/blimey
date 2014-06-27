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
        static FrameStats ()
        {
            Reset ();
        }

        public static void Reset ()
        {
            UpdateTime = 0.0;
            RenderTime = 0.0;

            SetCullModeTime = 0.0;
            ActivateGeomBufferTime = 0.0;
            MaterialTime = 0.0;
            ActivateShaderTime = 0.0;
            DrawTime = 0.0;

            DrawUserPrimitivesCount = 0;
            DrawIndexedPrimitivesCount = 0;
        }

        public static Double UpdateTime { get; set; }
        public static Double RenderTime { get; set; }

        public static Double SetCullModeTime { get; set; }
        public static Double ActivateGeomBufferTime { get; set; }
        public static Double MaterialTime { get; set; }
        public static Double ActivateShaderTime { get; set; }
        public static Double DrawTime { get; set; }

        public static Int32 DrawUserPrimitivesCount { get; set; }
        public static Int32 DrawIndexedPrimitivesCount { get; set; }

        public static Int32 DrawCallCount
        {
            get
            {
                return
                    DrawUserPrimitivesCount +
                    DrawIndexedPrimitivesCount;
            }
        }

        public static void SlowLog ()
        {
            if (UpdateTime > 5.0)
            {
                Console.WriteLine(
                    string.Format(
                        "UpdateTime -> {0:0.##}ms",
                        UpdateTime ));
            }

            if (RenderTime > 10.0)
            {
                Console.WriteLine(
                    string.Format(
                        "RenderTime -> {0:0.##}ms",
                        RenderTime ));


                Console.WriteLine(
                    string.Format(
                        "\tMeshRenderer -> SetCullModeTime -> {0:0.##}ms",
                        SetCullModeTime ));

                Console.WriteLine(
                    string.Format(
                        "\tActivateGeomBufferTime -> DrawTime -> {0:0.##}ms",
                        ActivateGeomBufferTime ));

                Console.WriteLine(
                    string.Format(
                        "\tMeshRenderer -> MaterialTime -> {0:0.##}ms",
                        MaterialTime ));

                Console.WriteLine(
                    string.Format(
                        "\tMeshRenderer -> ActivateShaderTime -> {0:0.##}ms",
                        ActivateShaderTime ));

                Console.WriteLine(
                    string.Format(
                        "\tMeshRenderer -> DrawTime -> {0:0.##}ms",
                        DrawTime ));
            }

            if (DrawCallCount > 25)
            {
                Console.WriteLine(
                    string.Format(
                        "Draw Call Count -> {0}",
                        DrawCallCount ));
            }
        }
    }
}
