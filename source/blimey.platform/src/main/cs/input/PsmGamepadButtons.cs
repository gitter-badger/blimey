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
    /// Represents the state of the buttons on a PlayStation Mobile Gamepad.
    /// </summary>
    public sealed class PsmGamepadButtons
        : HumanInputDeviceComponent
    {
        internal PsmGamepadButtons () {}

        internal override void Poll (AppTime appTime, Input.InputFrame inputFrame)
        {
            Triangle = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_Triangle);
            Square = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_Square);
            Circle = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_Circle);
            Cross = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_Cross);
            Start = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_Start);
            Select = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_Select);
            LeftShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_LeftSholder);
            RightShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_RightSholder);
        }

        /// <summary>
        /// Represents the state of the triangle button.
        /// </summary>
        public ButtonState Triangle { get; private set; }

        /// <summary>
        /// Represents the state of the square button.
        /// </summary>
        public ButtonState Square { get; private set; }

        /// <summary>
        /// Represents the state of the circle button.
        /// </summary>
        public ButtonState Circle { get; private set; }

        /// <summary>
        /// Represents the state of the cross button.
        /// </summary>
        public ButtonState Cross { get; private set; }

        /// <summary>
        /// Represents the state of the start button.
        /// </summary>
        public ButtonState Start { get; private set; }

        /// <summary>
        /// Represents the state of the select button.
        /// </summary>
        public ButtonState Select { get; private set; }

        /// <summary>
        /// Represents the state of the left shoulder button.
        /// </summary>
        public ButtonState LeftShoulder { get; private set; }

        /// <summary>
        /// Represents the state of the right shoulder button.
        /// </summary>
        public ButtonState RightShoulder { get; private set; }
    }
}