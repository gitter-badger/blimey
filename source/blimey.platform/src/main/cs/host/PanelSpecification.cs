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
    /// Specifies the attributes of a panel, a panel could be a screen, a touch device, or both.  A system / machine
    /// may have a number of panels, a desktop could have two monitors and a PlayStation Vita has a touchscreen on the
    /// front and a touch panel on the back.
    /// </summary>
    public sealed class PanelSpecification
    {
        readonly Vector2? panelPhysicalSize;
        readonly Single? panelPhysicalAspectRatio;
        readonly PanelType panelType;

        internal PanelSpecification (IApi platform)
        {
            panelPhysicalSize = platform.sys_GetPrimaryPanelPhysicalSize ();
            panelPhysicalAspectRatio =
                panelPhysicalSize.HasValue
                    ? (Single) panelPhysicalSize.Value.X / (Single) panelPhysicalSize.Value.Y
                    : (Single?) null;
            panelType = platform.sys_GetPrimaryPanelType ();
        }

        /// <summary>
        /// Provides data about the physical size of the panel measured in meters with the panel (in its default
        /// orientation).  This information is not alway known / available, which is why this property is nullable.
        /// </summary>
        public Vector2? PanelPhysicalSize { get { return panelPhysicalSize; } }

        /// <summary>
        /// Provides the physical aspect ratio of the panel (in it's default orientation).  This information is not
        /// alway known / available, which is why this property is nullable.
        /// </summary>
        public Single? PanelPhysicalAspectRatio { get { return panelPhysicalAspectRatio; } }

        /// <summary>
        /// Provides information about the capabilities of the panel.
        /// </summary>
        public PanelType PanelType { get { return panelType; } }
    }
}