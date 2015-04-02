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

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.IO;

    using Abacus.SinglePrecision;
    using Fudge;
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// A screen specification provides data about the attributes of a specific screen.  Every screen is associated with
    /// a panel that supports a screen.  The screen specification provides information about the OS configuration of the
    ///  hardware, not the specfic context or settings that the Cor App Framework is using, for example: if you had a
    /// desktop monitor with that supported a resolution of 1680x1050, but in the OS you chose to run it at
    /// a resolution of 1024x768 and the Cor App Framework is running a window with size 640x360 the screen
    /// specification would always return 1024x768.
    /// </summary>
    public sealed class ScreenSpecification
    {
        readonly IApi platform;

        internal ScreenSpecification (IApi platform)
        {
            this.platform = platform;
        }

        /// <summary>
        /// Defines the total width of the screen in question in pixels when the device is in it's default
        /// orientation.
        /// </summary>
        public Int32 ScreenResolutionWidth { get { return this.platform.sys_GetPrimaryScreenResolutionWidth (); } }

        /// <summary>
        /// Defines the total height of the screen in question in pixels when the device is in it's default
        /// orientation.
        /// </summary>
        public Int32 ScreenResolutionHeight { get { return this.platform.sys_GetPrimaryScreenResolutionHeight (); } }

        /// <summary>
        /// This is just the ratio of the ScreenResolutionWidth to ScreenResolutionHeight (w/h) (in it's default
        /// orientation).
        /// </summary>
        public Single ScreenResolutionAspectRatio { get { return (Single) ScreenResolutionWidth / (Single) ScreenResolutionHeight; } }
    }

}