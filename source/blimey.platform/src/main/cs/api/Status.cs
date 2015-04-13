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
    /// Provides frame by frame information about the external state of the App.
    /// </summary>
    public sealed class Status
    {
        readonly IApi platform;

        internal Status (IApi platform)
        {
            this.platform = platform;
        }

        /// <summary>
        /// Is the device running in fullscreen mode?  For things that don't support fullscreen mode this will be null.
        /// </summary>
        public Boolean? Fullscreen { get { return this.platform.app_IsFullscreen (); } }

        /// <summary>
        /// Returns the current width in pixels of the window the App is running in.  On most devices this will be the
        /// same as the however on desktops the app could be running in windowed mode and not take up all of the screen.
        /// This does not represent the size of the frame buffer or any other render targets.  With default settings the
        /// frame buffer for most platforms is instantiated with this width.  This value is from the context of the
        /// current orientation, for example of their is a 640x480 window on a desktop monitor that is orientated
        /// at 90deg this width will be 640.
        /// </summary>
        public Int32 Width { get { return this.platform.app_GetWidth(); } }

        /// <summary>
        /// Returns the current height in pixels of the window the App is running in.  On most devices this will be the
        /// same as the however on desktops the app could be running in windowed mode and not take up all of the screen.
        /// This does not represent the size of the frame buffer or any other render targets.  With default settings the
        /// frame buffer for most platforms is instantiated with this height.  This value is from the context of the
        /// current orientation, for example of their is a 640x480 window on a desktop monitor that is orientated
        /// at 90deg this height will be 480.
        /// </summary>
        public Int32 Height { get { return this.platform.app_GetHeight(); } }
    }
}
