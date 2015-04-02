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
    /// Represents the state of a pair of triggers on a gamepad.
    /// </summary>
    public sealed class GamepadTriggerPair
        : HumanInputDeviceComponent
    {
        readonly Gamepad gamepad;
        readonly PlayerIndex? playerIndex;

        internal GamepadTriggerPair (Gamepad gamepad, PlayerIndex? playerIndex = null)
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
                                    Left = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_0_LeftTrigger);
                                    Right = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_0_RightTrigger);
                                }
                                break;

                            case PlayerIndex.Two:
                                {
                                    Left = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_1_LeftTrigger);
                                    Right = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_1_RightTrigger);
                                }
                                break;

                            case PlayerIndex.Three:
                                {
                                    Left = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_2_LeftTrigger);
                                    Right = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_2_RightTrigger);
                                }
                                break;

                            case PlayerIndex.Four:
                                {
                                    Left = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_3_LeftTrigger);
                                    Right = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_3_RightTrigger);
                                }
                                break;
                        }
                    }
                    break;

                default: throw new NotSupportedException ();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Single Left { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public Single Right { get; private set; }
    }
}