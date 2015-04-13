// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey Engine (http://www.blimey.io)         │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors:                                                               │ \\
// │ ~ Ash Pook (http://www.ajpook.com)                                     │ \\
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

namespace Blimey.Platform
{
    using System;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    using Fudge;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public enum AnalogControlIdentifier
    {
        Xbox360_0_Leftstick_X,
        Xbox360_0_Leftstick_Y,
        Xbox360_0_Rightstick_X,
        Xbox360_0_Rightstick_Y,
        Xbox360_0_LeftTrigger,
        Xbox360_0_RightTrigger,

        Xbox360_1_Leftstick_X,
        Xbox360_1_Leftstick_Y,
        Xbox360_1_Rightstick_X,
        Xbox360_1_Rightstick_Y,
        Xbox360_1_LeftTrigger,
        Xbox360_1_RightTrigger,

        Xbox360_2_Leftstick_X,
        Xbox360_2_Leftstick_Y,
        Xbox360_2_Rightstick_X,
        Xbox360_2_Rightstick_Y,
        Xbox360_2_LeftTrigger,
        Xbox360_2_RightTrigger,

        Xbox360_3_Leftstick_X,
        Xbox360_3_Leftstick_Y,
        Xbox360_3_Rightstick_X,
        Xbox360_3_Rightstick_Y,
        Xbox360_3_LeftTrigger,
        Xbox360_3_RightTrigger,

        PlayStationMobile_Leftstick_X,
        PlayStationMobile_Leftstick_Y,
        PlayStationMobile_Rightstick_X,
        PlayStationMobile_Rightstick_Y,
    }
}
