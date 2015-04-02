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
    /// All supported functional keyboard keys (not including character keys).
    /// </summary>
    [Flags]
    public enum FunctionalKey
    {
        /// <summary>
        /// BACKSPACE key.
        /// </summary>
        Backspace,

        /// <summary>
        /// TAB key.
        /// </summary>
        Tab,

        /// <summary>
        /// ENTER key.
        /// </summary>
        Enter,

        /// <summary>
        /// CAPS LOCK key.
        /// </summary>
        CapsLock,

        /// <summary>
        /// ESC key.
        /// </summary>
        Escape,

        // SPACEBAR
        Spacebar,

        /// <summary>
        /// PAGE UP key.
        /// </summary>
        PageUp,

        /// <summary>
        /// PAGE DOWN key.
        /// </summary>
        PageDown,

        /// <summary>
        /// END key.
        /// </summary>
        End,

        /// <summary>
        /// HOME key.
        /// </summary>
        Home,

        /// <summary>
        /// LEFT ARROW key.
        /// </summary>
        Left,

        /// <summary>
        /// UP ARROW key.
        /// </summary>
        Up,

        /// <summary>
        /// RIGHT ARROW key.
        /// </summary>
        Right,

        /// <summary>
        /// DOWN ARROW key.
        /// </summary>
        Down,

        /// <summary>
        /// SELECT key.
        /// </summary>
        Select,

        /// <summary>
        /// PRINT key.
        /// </summary>
        Print,

        /// <summary>
        /// EXECUTE key.
        /// </summary>
        Execute,

        /// <summary>
        /// PRINT SCREEN key.
        /// </summary>
        PrintScreen,

        /// <summary>
        /// INS key.
        /// </summary>
        Insert,

        /// <summary>
        /// DEL key.
        /// </summary>
        Delete,

        /// <summary>
        /// HELP key.
        /// </summary>
        Help,

        /// <summary>
        /// Left Windows key.
        /// </summary>
        LeftWindows,

        /// <summary>
        /// Right Windows key.
        /// </summary>
        RightWindows,

        /// <summary>
        /// Left Windows key.
        /// </summary>
        LeftFlower,

        /// <summary>
        /// Right Windows key.
        /// </summary>
        RightFlower,

        /// <summary>
        /// Applications key.
        /// </summary>
        Apps,

        /// <summary>
        /// Computer Sleep key.
        /// </summary>
        Sleep,

        /// <summary>
        /// Numeric pad 0 key.
        /// </summary>
        NumPad0,

        /// <summary>
        /// Numeric pad 1 key.
        /// </summary>
        NumPad1,

        /// <summary>
        /// Numeric pad 2 key.
        /// </summary>
        NumPad2,

        /// <summary>
        /// Numeric key
        /// pad 3 key.
        /// </summary>
        NumPad3,

        /// <summary>
        /// Numeric key
        /// pad 4 key.
        /// </summary>
        NumPad4,

        /// <summary>
        /// Numeric pad 5 key.
        /// </summary>
        NumPad5,

        /// <summary>
        /// Numeric pad 6 key.
        /// </summary>
        NumPad6,

        /// <summary>
        /// Numeric pad 7 key.
        /// </summary>
        NumPad7,

        /// <summary>
        /// Numeric pad 8 key.
        /// </summary>
        NumPad8,

        /// <summary>
        /// Numeric pad 9 key.
        /// </summary>
        NumPad9,

        /// <summary>
        /// Multiply key.
        /// </summary>
        Multiply,

        /// <summary>
        /// Add key.
        /// </summary>
        Add,

        /// <summary>
        /// Separator key.
        /// </summary>
        Separator,

        /// <summary>
        /// Subtract key.
        /// </summary>
        Subtract,

        /// <summary>
        /// Decimal key.
        /// </summary>
        Decimal,

        /// <summary>
        /// Divide key.
        /// </summary>
        Divide,

        /// <summary>
        /// F1 key.
        /// </summary>
        F1,

        /// <summary>
        /// F2 key.
        /// </summary>
        F2,

        /// <summary>
        /// F3 key.
        /// </summary>
        F3,

        /// <summary>
        /// F4 key.
        /// </summary>
        F4,

        /// <summary>
        /// F5 key.
        /// </summary>
        F5,

        /// <summary>
        /// F6 key.
        /// </summary>
        F6,

        /// <summary>
        /// F7 key.
        /// </summary>
        F7,

        /// <summary>
        /// F8 key.
        /// </summary>
        F8,

        /// <summary>
        /// F9 key.
        /// </summary>
        F9,

        /// <summary>
        /// F10 key.
        /// </summary>
        F10,

        /// <summary>
        /// F11 key.
        /// </summary>
        F11,

        /// <summary>
        /// F12 key.
        /// </summary>
        F12,

        /// <summary>
        /// F13 key.
        /// </summary>
        F13,

        /// <summary>
        /// F14 key.
        /// </summary>
        F14,

        /// <summary>
        /// F15 key.
        /// </summary>
        F15,

        /// <summary>
        /// F16 key.
        /// </summary>
        F16,

        /// <summary>
        /// F17 key.
        /// </summary>
        F17,

        /// <summary>
        /// F18 key.
        /// </summary>
        F18,

        /// <summary>
        /// F19 key.
        /// </summary>
        F19,

        /// <summary>
        /// F20 key.
        /// </summary>
        F20,

        /// <summary>
        /// F21 key.
        /// </summary>
        F21,

        /// <summary>
        /// F22 key.
        /// </summary>
        F22,

        /// <summary>
        /// F23 key.
        /// </summary>
        F23,

        /// <summary>
        /// F24 key.
        /// </summary>
        F24,

        /// <summary>
        /// NUM LOCK key.
        /// </summary>
        NumLock,

        /// <summary>
        /// SCROLL LOCK key.
        /// </summary>
        ScrollLock,

        /// <summary>
        /// Left SHIFT key.
        /// </summary>
        LeftShift,

        /// <summary>
        /// Right SHIFT key.
        /// </summary>
        RightShift,

        /// <summary>
        /// Left CONTROL key.
        /// </summary>
        LeftControl,

        /// <summary>
        /// Right CONTROL key.
        /// </summary>
        RightControl,

        /// <summary>
        /// Left ALT key.
        /// </summary>
        LeftAlt,

        /// <summary>
        /// Right ALT key.
        /// </summary>
        RightAlt,
    }
}