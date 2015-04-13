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
    /// Represents the state of the analogue thumbsticks on a gamepad.
    /// </summary>
    public sealed class GamepadThumbsticks
        : HumanInputDeviceComponent
    {
        readonly Gamepad gamepad;
        readonly PlayerIndex? playerIndex;

        internal GamepadThumbsticks (Gamepad gamepad, PlayerIndex? playerIndex = null)
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
                                    Left = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_0_Leftstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_0_Leftstick_Y)
                                    };
                                    Right = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_0_Rightstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_0_Rightstick_Y)
                                    };
                                }
                                break;

                            case PlayerIndex.Two:
                                {
                                    Left = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_1_Leftstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_1_Leftstick_Y)
                                    };
                                    Right = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_1_Rightstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_1_Rightstick_Y)
                                    };
                                }
                                break;

                            case PlayerIndex.Three:
                                {
                                    Left = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_2_Leftstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_2_Leftstick_Y)
                                    };
                                    Right = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_2_Rightstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_2_Rightstick_Y)
                                    };
                                }
                                break;

                            case PlayerIndex.Four:
                                {
                                    Left = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_3_Leftstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_3_Leftstick_Y)
                                    };
                                    Right = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_3_Rightstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_3_Rightstick_Y)
                                    };
                                }
                                break;
                        }
                    }
                    break;

                case Gamepad.PSM:
                    {
                        Left = new Vector2 {
                            X = GetAnalogState (inputFrame, AnalogControlIdentifier.PlayStationMobile_Leftstick_X),
                            Y = GetAnalogState (inputFrame, AnalogControlIdentifier.PlayStationMobile_Leftstick_Y)
                        };
                        Right = new Vector2 {
                            X = GetAnalogState (inputFrame, AnalogControlIdentifier.PlayStationMobile_Rightstick_X),
                            Y = GetAnalogState (inputFrame, AnalogControlIdentifier.PlayStationMobile_Rightstick_Y)
                        };
                    }
                    break;

                default: throw new NotSupportedException ();
            }
        }

        /// <summary>
        /// Represents the state of the left thumbstick, the X and Y values of the returned Vector2 are both in the
        /// range of -1.0 to 1.0 with 0.0 representing no movement.
        /// </summary>
        public Vector2 Left { get; private set; }

        /// <summary>
        /// Represents the state of the right thumbstick, the X and Y values of the returned Vector2 are both in the
        /// range of -1.0 to 1.0 with 0.0 representing no movement.
        /// </summary>
        public  Vector2 Right { get; private set; }
    }
}
