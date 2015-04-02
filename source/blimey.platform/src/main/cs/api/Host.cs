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
    /// Provides information about the hardware and environment that the Cor App Framework is running against.
    /// </summary>
    public sealed class Host
    {
        readonly IApi platform;
        readonly ScreenSpecification screenSpecification;
        readonly PanelSpecification panelSpecification;

        internal Host (IApi platform)
        {
            this.platform = platform;
            this.screenSpecification = new ScreenSpecification (platform);
            this.panelSpecification = new PanelSpecification (platform);
        }

        /// <summary>
        /// Identifies the Machine that Cor's host Virtual Machine is running on.
        /// Ex: PC, Macintosh, iPad2, Samsung Galaxy S4
        /// </summary>
        public String Machine { get { return platform.sys_GetMachineIdentifier (); } }

        /// <summary>
        /// Identifies the Operating System that Cor's host Virtual Machine is running on.
        /// Ex: Ubuntu, Windows NT, OSX, iOS 7.0, Android Jelly Bean
        /// </summary>
        public String OperatingSystem { get { return platform.sys_GetOperatingSystemIdentifier (); } }

        /// <summary>
        /// Identifies the Virtual Machine that Cor is running in.
        /// Ex: .NET 4.0, MONO 2.10
        /// </summary>
        public String VirtualMachine { get { return platform.sys_GetVirtualMachineIdentifier (); } }

        /// <summary>
        /// The current orientation of the machine.
        /// </summary>
        public DeviceOrientation? CurrentOrientation { get { return platform.hid_GetCurrentOrientation (); } }

        /// <summary>
        /// The screen specification of the machine.
        /// </summary>
        public ScreenSpecification ScreenSpecification { get { return screenSpecification; } }

        /// <summary>
        ///
        /// </summary>
        public PanelSpecification PanelSpecification { get { return panelSpecification; } }
    }
}