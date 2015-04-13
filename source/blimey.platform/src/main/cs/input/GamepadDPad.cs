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
    /// Represents the state of the D-Pad on a gamepad.
    /// </summary>
    public sealed class GamepadDPad
        : HumanInputDeviceComponent
    {
        readonly Gamepad gamepad;
        readonly PlayerIndex? playerIndex;

        internal GamepadDPad (Gamepad gamepad, PlayerIndex? playerIndex = null)
        {
            this.gamepad = gamepad;
            this.playerIndex = playerIndex;
        }

        internal override void Poll (AppTime appTime, Input.InputFrame inputFrame)
        {
            switch (gamepad)
            {
                case Gamepad.Xbox360:
                    {
                        switch (playerIndex.Value)
                        {
                            case PlayerIndex.One:
                                {
                                    Down = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_DPad_Down);
                                    Left = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_DPad_Left);
                                    Right = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_DPad_Right);
                                    Up = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_DPad_Up);
                                }
                                break;

                            case PlayerIndex.Two:
                                {
                                    Down = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_DPad_Down);
                                    Left = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_DPad_Left);
                                    Right = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_DPad_Right);
                                    Up = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_DPad_Up);
                                }
                                break;

                            case PlayerIndex.Three:
                                {
                                    Down = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_DPad_Down);
                                    Left = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_DPad_Left);
                                    Right = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_DPad_Right);
                                    Up = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_DPad_Up);
                                }
                                break;

                            case PlayerIndex.Four:
                                {
                                    Down = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_DPad_Down);
                                    Left = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_DPad_Left);
                                    Right = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_DPad_Right);
                                    Up = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_DPad_Up);
                                }
                                break;
                        }
                    }
                    break;

                case Gamepad.PSM:
                    {
                        Down = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_DPad_Down);
                        Left = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_DPad_Left);
                        Right = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_DPad_Right);
                        Up = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_DPad_Up);
                    }
                    break;

                default: throw new NotSupportedException ();
            }
        }

        /// <summary>
        /// Represents the state of the down button.
        /// </summary>
        public ButtonState Down { get; private set; }

        /// <summary>
        /// Represents the state of the left button.
        /// </summary>
        public ButtonState Left { get; private set; }

        /// <summary>
        /// Represents the state of the right button.
        /// </summary>
        public ButtonState Right { get; private set; }

        /// <summary>
        /// Represents the state of the up button.
        /// </summary>
        public ButtonState Up { get; private set; }
    }
}
