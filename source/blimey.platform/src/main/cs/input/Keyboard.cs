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
    ///
    /// </summary>
    public sealed class Keyboard
        : HumanInputDevice
    {
        internal Keyboard () {}

        readonly HashSet<Char> PressedCharacterKeys = new HashSet<Char> ();
        readonly HashSet<FunctionalKey> PressedFunctionalKeys = new HashSet<FunctionalKey> ();

        static readonly Dictionary <BinaryControlIdentifier, FunctionalKey> mapping;

        static Keyboard ()
        {
            mapping = new Dictionary<BinaryControlIdentifier, FunctionalKey>()
            {
                { BinaryControlIdentifier.Keyboard_Backspace,       FunctionalKey.Backspace },
                { BinaryControlIdentifier.Keyboard_Tab,             FunctionalKey.Tab },
                { BinaryControlIdentifier.Keyboard_Enter,           FunctionalKey.Enter },
                { BinaryControlIdentifier.Keyboard_CapsLock,        FunctionalKey.CapsLock },
                { BinaryControlIdentifier.Keyboard_Escape,          FunctionalKey.Escape },
                { BinaryControlIdentifier.Keyboard_Spacebar,        FunctionalKey.Spacebar },
                { BinaryControlIdentifier.Keyboard_PageUp,          FunctionalKey.PageUp },
                { BinaryControlIdentifier.Keyboard_PageDown,        FunctionalKey.PageDown },
                { BinaryControlIdentifier.Keyboard_End,             FunctionalKey.End },
                { BinaryControlIdentifier.Keyboard_Home,            FunctionalKey.Home },
                { BinaryControlIdentifier.Keyboard_Left,            FunctionalKey.Left },
                { BinaryControlIdentifier.Keyboard_Up,              FunctionalKey.Up },
                { BinaryControlIdentifier.Keyboard_Right,           FunctionalKey.Right },
                { BinaryControlIdentifier.Keyboard_Down,            FunctionalKey.Down },
                { BinaryControlIdentifier.Keyboard_Select,          FunctionalKey.Select },
                { BinaryControlIdentifier.Keyboard_Print,           FunctionalKey.Print },
                { BinaryControlIdentifier.Keyboard_Execute,         FunctionalKey.Execute },
                { BinaryControlIdentifier.Keyboard_PrintScreen,     FunctionalKey.PrintScreen },
                { BinaryControlIdentifier.Keyboard_Insert,          FunctionalKey.Insert },
                { BinaryControlIdentifier.Keyboard_Delete,          FunctionalKey.Delete },
                { BinaryControlIdentifier.Keyboard_Help,            FunctionalKey.Help },
                { BinaryControlIdentifier.Keyboard_LeftWindows,     FunctionalKey.LeftWindows },
                { BinaryControlIdentifier.Keyboard_RightWindows,    FunctionalKey.RightWindows },
                { BinaryControlIdentifier.Keyboard_LeftFlower,      FunctionalKey.LeftFlower },
                { BinaryControlIdentifier.Keyboard_RightFlower,     FunctionalKey.RightFlower },
                { BinaryControlIdentifier.Keyboard_Apps,            FunctionalKey.Apps },
                { BinaryControlIdentifier.Keyboard_Sleep,           FunctionalKey.Sleep },
                { BinaryControlIdentifier.Keyboard_NumPad0,         FunctionalKey.NumPad0 },
                { BinaryControlIdentifier.Keyboard_NumPad1,         FunctionalKey.NumPad1 },
                { BinaryControlIdentifier.Keyboard_NumPad2,         FunctionalKey.NumPad2 },
                { BinaryControlIdentifier.Keyboard_NumPad3,         FunctionalKey.NumPad3 },
                { BinaryControlIdentifier.Keyboard_NumPad4,         FunctionalKey.NumPad4 },
                { BinaryControlIdentifier.Keyboard_NumPad5,         FunctionalKey.NumPad5 },
                { BinaryControlIdentifier.Keyboard_NumPad6,         FunctionalKey.NumPad6 },
                { BinaryControlIdentifier.Keyboard_NumPad7,         FunctionalKey.NumPad7 },
                { BinaryControlIdentifier.Keyboard_NumPad8,         FunctionalKey.NumPad8 },
                { BinaryControlIdentifier.Keyboard_NumPad9,         FunctionalKey.NumPad9 },
                { BinaryControlIdentifier.Keyboard_Multiply,        FunctionalKey.Multiply },
                { BinaryControlIdentifier.Keyboard_Add,             FunctionalKey.Add },
                { BinaryControlIdentifier.Keyboard_Separator,       FunctionalKey.Separator },
                { BinaryControlIdentifier.Keyboard_Subtract,        FunctionalKey.Subtract },
                { BinaryControlIdentifier.Keyboard_Decimal,         FunctionalKey.Decimal },
                { BinaryControlIdentifier.Keyboard_Divide,          FunctionalKey.Divide },
                { BinaryControlIdentifier.Keyboard_F1,              FunctionalKey.F1 },
                { BinaryControlIdentifier.Keyboard_F2,              FunctionalKey.F2 },
                { BinaryControlIdentifier.Keyboard_F3,              FunctionalKey.F3 },
                { BinaryControlIdentifier.Keyboard_F4,              FunctionalKey.F4 },
                { BinaryControlIdentifier.Keyboard_F5,              FunctionalKey.F5 },
                { BinaryControlIdentifier.Keyboard_F6,              FunctionalKey.F6 },
                { BinaryControlIdentifier.Keyboard_F7,              FunctionalKey.F7 },
                { BinaryControlIdentifier.Keyboard_F8,              FunctionalKey.F8 },
                { BinaryControlIdentifier.Keyboard_F9,              FunctionalKey.F9 },
                { BinaryControlIdentifier.Keyboard_F10,             FunctionalKey.F10 },
                { BinaryControlIdentifier.Keyboard_F11,             FunctionalKey.F11 },
                { BinaryControlIdentifier.Keyboard_F12,             FunctionalKey.F12 },
                { BinaryControlIdentifier.Keyboard_F13,             FunctionalKey.F13 },
                { BinaryControlIdentifier.Keyboard_F14,             FunctionalKey.F14 },
                { BinaryControlIdentifier.Keyboard_F15,             FunctionalKey.F15 },
                { BinaryControlIdentifier.Keyboard_F16,             FunctionalKey.F16 },
                { BinaryControlIdentifier.Keyboard_F17,             FunctionalKey.F17 },
                { BinaryControlIdentifier.Keyboard_F18,             FunctionalKey.F18 },
                { BinaryControlIdentifier.Keyboard_F19,             FunctionalKey.F19 },
                { BinaryControlIdentifier.Keyboard_F20,             FunctionalKey.F20 },
                { BinaryControlIdentifier.Keyboard_F21,             FunctionalKey.F21 },
                { BinaryControlIdentifier.Keyboard_F22,             FunctionalKey.F22 },
                { BinaryControlIdentifier.Keyboard_F23,             FunctionalKey.F23 },
                { BinaryControlIdentifier.Keyboard_F24,             FunctionalKey.F24 },
                { BinaryControlIdentifier.Keyboard_NumLock,         FunctionalKey.NumLock },
                { BinaryControlIdentifier.Keyboard_ScrollLock,      FunctionalKey.ScrollLock },
                { BinaryControlIdentifier.Keyboard_LeftShift,       FunctionalKey.LeftShift },
                { BinaryControlIdentifier.Keyboard_RightShift,      FunctionalKey.RightShift },
                { BinaryControlIdentifier.Keyboard_LeftControl,     FunctionalKey.LeftControl },
                { BinaryControlIdentifier.Keyboard_RightControl,    FunctionalKey.RightControl },
                { BinaryControlIdentifier.Keyboard_LeftAlt,         FunctionalKey.LeftAlt },
                { BinaryControlIdentifier.Keyboard_RightAlt,        FunctionalKey.RightAlt },
            };
        }

        internal override void Poll (AppTime time, Input.InputFrame inputFrame)
        {
            PressedCharacterKeys.Clear ();
            PressedFunctionalKeys.Clear ();

            inputFrame.PressedCharacters.ToList ().ForEach (x => PressedCharacterKeys.Add (x));

            foreach (var key in mapping.Keys)
            {
                if (inputFrame.BinaryControlStates.Contains (key))
                    PressedFunctionalKeys.Add (mapping [key]);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public FunctionalKey[] GetPressedFunctionalKeys ()
        {
            return PressedFunctionalKeys.ToArray ();
        }

        /// <summary>
        ///
        /// </summary>
        public Boolean IsFunctionalKeyDown (FunctionalKey key)
        {
            return PressedFunctionalKeys.Contains (key);
        }

        /// <summary>
        ///
        /// </summary>
        public Boolean IsFunctionalKeyUp (FunctionalKey key)
        {
            return !PressedFunctionalKeys.Contains (key);
        }

        /// <summary>
        ///
        /// </summary>
        public KeyState this [FunctionalKey key]
        {
            get {return PressedFunctionalKeys.Contains (key) ? KeyState.Down : KeyState.Up; }
        }

        /// <summary>
        ///
        /// </summary>
        public Char[] GetPressedCharacterKeys ()
        {
            return PressedCharacterKeys.ToArray ();
        }

        /// <summary>
        ///
        /// </summary>
        public Boolean IsCharacterKeyDown (Char key)
        {
            return PressedCharacterKeys.Contains (key);
        }

        /// <summary>
        ///
        /// </summary>
        public Boolean IsCharacterKeyUp (Char key)
        {
            return !PressedCharacterKeys.Contains (key);
        }

        /// <summary>
        ///
        /// </summary>
        public KeyState this [Char key]
        {
            get {return PressedCharacterKeys.Contains (key) ? KeyState.Down : KeyState.Up; }
        }
    }
}