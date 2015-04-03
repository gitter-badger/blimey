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
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    using Fudge;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public enum BinaryControlIdentifier
    {
        Xbox360_0_DPad_Up,
        Xbox360_0_DPad_Down,
        Xbox360_0_DPad_Left,
        Xbox360_0_DPad_Right,
        Xbox360_0_A,
        Xbox360_0_B,
        Xbox360_0_X,
        Xbox360_0_Y,
        Xbox360_0_LeftSholder,
        Xbox360_0_RightSholder,
        Xbox360_0_LeftThumbstick,
        Xbox360_0_RightThumbstick,
        Xbox360_0_Start,
        Xbox360_0_Back,

        Xbox360_1_DPad_Up,
        Xbox360_1_DPad_Down,
        Xbox360_1_DPad_Left,
        Xbox360_1_DPad_Right,
        Xbox360_1_A,
        Xbox360_1_B,
        Xbox360_1_X,
        Xbox360_1_Y,
        Xbox360_1_LeftSholder,
        Xbox360_1_RightSholder,
        Xbox360_1_LeftThumbstick,
        Xbox360_1_RightThumbstick,
        Xbox360_1_Start,
        Xbox360_1_Back,

        Xbox360_2_DPad_Up,
        Xbox360_2_DPad_Down,
        Xbox360_2_DPad_Left,
        Xbox360_2_DPad_Right,
        Xbox360_2_A,
        Xbox360_2_B,
        Xbox360_2_X,
        Xbox360_2_Y,
        Xbox360_2_LeftSholder,
        Xbox360_2_RightSholder,
        Xbox360_2_LeftThumbstick,
        Xbox360_2_RightThumbstick,
        Xbox360_2_Start,
        Xbox360_2_Back,

        Xbox360_3_DPad_Up,
        Xbox360_3_DPad_Down,
        Xbox360_3_DPad_Left,
        Xbox360_3_DPad_Right,
        Xbox360_3_A,
        Xbox360_3_B,
        Xbox360_3_X,
        Xbox360_3_Y,
        Xbox360_3_LeftSholder,
        Xbox360_3_RightSholder,
        Xbox360_3_LeftThumbstick,
        Xbox360_3_RightThumbstick,
        Xbox360_3_Start,
        Xbox360_3_Back,

        PlayStationMobile_DPad_Up,
        PlayStationMobile_DPad_Down,
        PlayStationMobile_DPad_Left,
        PlayStationMobile_DPad_Right,
        PlayStationMobile_Cross,
        PlayStationMobile_Circle,
        PlayStationMobile_Triangle,
        PlayStationMobile_Square,
        PlayStationMobile_Start,
        PlayStationMobile_Select,
        PlayStationMobile_LeftSholder,
        PlayStationMobile_RightSholder,

        Mouse_Left,
        Mouse_Middle,
        Mouse_Right,

        Keyboard_Backspace,
        Keyboard_Tab,
        Keyboard_Enter,
        Keyboard_CapsLock,
        Keyboard_Escape,
        Keyboard_Spacebar,
        Keyboard_PageUp,
        Keyboard_PageDown,
        Keyboard_End,
        Keyboard_Home,
        Keyboard_Left,
        Keyboard_Up,
        Keyboard_Right,
        Keyboard_Down,
        Keyboard_Select,
        Keyboard_Print,
        Keyboard_Execute,
        Keyboard_PrintScreen,
        Keyboard_Insert,
        Keyboard_Delete,
        Keyboard_Help,
        Keyboard_LeftWindows,
        Keyboard_RightWindows,
        Keyboard_LeftFlower,
        Keyboard_RightFlower,
        Keyboard_Apps,
        Keyboard_Sleep,
        Keyboard_NumPad0,
        Keyboard_NumPad1,
        Keyboard_NumPad2,
        Keyboard_NumPad3,
        Keyboard_NumPad4,
        Keyboard_NumPad5,
        Keyboard_NumPad6,
        Keyboard_NumPad7,
        Keyboard_NumPad8,
        Keyboard_NumPad9,
        Keyboard_Multiply,
        Keyboard_Add,
        Keyboard_Separator,
        Keyboard_Subtract,
        Keyboard_Decimal,
        Keyboard_Divide,
        Keyboard_F1,
        Keyboard_F2,
        Keyboard_F3,
        Keyboard_F4,
        Keyboard_F5,
        Keyboard_F6,
        Keyboard_F7,
        Keyboard_F8,
        Keyboard_F9,
        Keyboard_F10,
        Keyboard_F11,
        Keyboard_F12,
        Keyboard_F13,
        Keyboard_F14,
        Keyboard_F15,
        Keyboard_F16,
        Keyboard_F17,
        Keyboard_F18,
        Keyboard_F19,
        Keyboard_F20,
        Keyboard_F21,
        Keyboard_F22,
        Keyboard_F23,
        Keyboard_F24,
        Keyboard_NumLock,
        Keyboard_ScrollLock,
        Keyboard_LeftShift,
        Keyboard_RightShift,
        Keyboard_LeftControl,
        Keyboard_RightControl,
        Keyboard_LeftAlt,
        Keyboard_RightAlt,


    }
}