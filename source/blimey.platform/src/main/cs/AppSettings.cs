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
    /// Defines the settings for a Cor App Framework program.
    /// </summary>
    public class AppSettings
    {
        readonly String appName;
        readonly LogManagerSettings logManagerSettings;

        /// <summary>
        /// Constructs a new Cor.AppSettings object.  The Cor.AppSettings object is intented to be instantiated by the
        /// user and provided along with their IApp object to a platform's Cor.ICor implementation in order to
        /// trigger the entry point into a Cor App Framework program.
        /// </summary>
        public AppSettings (String appName)
        {
            this.appName = appName;
            this.logManagerSettings = new LogManagerSettings (this.appName);

            // Default configuration
            this.MouseGeneratesTouches = true;
            this.FullScreen = true;
        }

        /// <summary>
        /// Provides access to the App's name.  This is displayed at the top of windows and in the taskbar for desktop
        /// platforms and as an App Name on mobile platforms.
        /// </summary>
        public String AppName { get { return appName; } }

        /// <summary>
        /// Encapsulates settings pertaining to logging.
        /// </summary>
        public LogManagerSettings LogSettings { get { return logManagerSettings; } }

        /// <summary>
        /// Gets or sets a value indicating whether the mouse input device (if it exists on the platform) should
        /// generates touch events to simulate a touch controller inside the Cor App Framework.
        /// </summary>
        public Boolean MouseGeneratesTouches { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the app should be run in fullscreen mode inside the Cor App
        /// Framework.  On platforms where running in a windowed mode is not possible this variable is ignored.
        /// </summary>
        public Boolean FullScreen { get; set; }
    }
}