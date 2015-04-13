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
    /// Represents the state of the buttons on an Xbox 360 gamepad.
    /// </summary>
    public sealed class Xbox360GamepadButtons
        : HumanInputDeviceComponent
    {
        readonly PlayerIndex playerIndex;
        internal Xbox360GamepadButtons (PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;

            A = ButtonState.Released;
            B = ButtonState.Released;
            Back = ButtonState.Released;
            LeftShoulder = ButtonState.Released;
            LeftStick = ButtonState.Released;
            RightShoulder = ButtonState.Released;
            RightStick = ButtonState.Released;
            Start = ButtonState.Released;
            X = ButtonState.Released;
            Y = ButtonState.Released;
        }

        internal override void Poll (AppTime appTime, Input.InputFrame inputFrame)
        {
            switch (playerIndex)
            {
                case PlayerIndex.Two:
                    {
                        A = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_A);
                        B = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_B);
                        Back = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_Back);
                        LeftShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_LeftSholder);
                        LeftStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_LeftThumbstick);
                        RightShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_RightSholder);
                        RightStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_RightThumbstick);
                        Start = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_Start);
                        X = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_X);
                        Y = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_Y);
                    }
                    break;

                case PlayerIndex.Three:
                    {
                        A = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_A);
                        B = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_B);
                        Back = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_Back);
                        LeftShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_LeftSholder);
                        LeftStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_LeftThumbstick);
                        RightShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_RightSholder);
                        RightStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_RightThumbstick);
                        Start = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_Start);
                        X = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_X);
                        Y = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_Y);
                    }
                    break;

                case PlayerIndex.Four:
                    {
                        A = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_A);
                        B = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_B);
                        Back = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_Back);
                        LeftShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_LeftSholder);
                        LeftStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_LeftThumbstick);
                        RightShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_RightSholder);
                        RightStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_RightThumbstick);
                        Start = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_Start);
                        X = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_X);
                        Y = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_Y);
                    }
                    break;

                case PlayerIndex.One:
                    {
                        A = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_A);
                        B = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_B);
                        Back = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_Back);
                        LeftShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_LeftSholder);
                        LeftStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_LeftThumbstick);
                        RightShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_RightSholder);
                        RightStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_RightThumbstick);
                        Start = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_Start);
                        X = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_X);
                        Y = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_Y);
                    }
                    break;

                default: throw new NotSupportedException ();
            }
        }



        /// <summary>
        /// Represents the state of the A button.
        /// </summary>
        public ButtonState A { get; private set; }

        /// <summary>
        /// Represents the state of the B button.
        /// </summary>
        public ButtonState B { get; private set; }

        /// <summary>
        /// Represents the state of the back button.
        /// </summary>
        public ButtonState Back { get; private set; }

        /// <summary>
        /// Represents the state of the left shoulder button.
        /// </summary>
        public ButtonState LeftShoulder { get; private set; }

        /// <summary>
        /// Represents the state of the left analogue stick's click button thing.
        /// </summary>
        public ButtonState LeftStick { get; private set; }

        /// <summary>
        /// Represents the state of the right shoulder button.
        /// </summary>
        public ButtonState RightShoulder { get; private set; }

        /// <summary>
        /// Represents the state of the right analogue stick's click button thing.
        /// </summary>
        public ButtonState RightStick { get; private set; }

        /// <summary>
        /// Represents the state of the start button.
        /// </summary>
        public ButtonState Start { get; private set; }

        /// <summary>
        /// Represents the state of the X button.
        /// </summary>
        public ButtonState X { get; private set; }

        /// <summary>
        /// Represents the state of the Y button.
        /// </summary>
        public ButtonState Y { get; private set; }
    }
}
