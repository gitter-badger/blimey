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
    /// Depending on the implementation you are running against various input devices will be avaiable.  Those that are
    /// not will be returned as NULL.  It is down to your app to deal with only some of input devices being available.
    /// For example, if you are running on iPad, the GetXbox360Gamepad method will return NULL.  The way to make your
    /// app deal with multiple platforms is to poll the input devices at bootup and then query only those that are
    /// avaible in your update loop.
    /// </summary>
    public sealed class Input
    {
        readonly IApi platform;

        Boolean mouseGeneratesTouches;
        String currentMouseTouchId = null;

        readonly InputFrame inputFrame = new InputFrame ();

        readonly List<HumanInputDevice> humanInputDevices = new List<HumanInputDevice> ();

        public sealed class InputFrame
        {
            public readonly Dictionary <DigitalControlIdentifier, Int32> DigitalControlStates;
            public readonly Dictionary <AnalogControlIdentifier, Single> AnalogControlStates;
            public readonly HashSet <BinaryControlIdentifier> BinaryControlStates;
            public readonly HashSet <Char> PressedCharacters;
            public readonly HashSet <RawTouch> ActiveTouches;

            internal InputFrame ()
            {
                DigitalControlStates =  new Dictionary <DigitalControlIdentifier, Int32> ();
                AnalogControlStates =   new Dictionary <AnalogControlIdentifier, Single> ();
                BinaryControlStates =   new HashSet<BinaryControlIdentifier> ();
                PressedCharacters =     new HashSet <Char> ();
                ActiveTouches =         new HashSet <RawTouch> ();
            }
        }

        internal Input (IApi platform)
        {
            this.platform = platform;

            this.Xbox360Gamepad = humanInputDevices.AddEx (new Xbox360Gamepad (PlayerIndex.One));
            this.PsmGamepad = humanInputDevices.AddEx (new PsmGamepad ());
            this.MultiTouchController = humanInputDevices.AddEx (new MultiTouchController ());
            this.Mouse = humanInputDevices.AddEx (new Mouse ());
            this.Keyboard = humanInputDevices.AddEx (new Keyboard ());
            this.GenericGamepad = new GenericGamepad ();
        }

        public void UpdateSettings (Boolean mouseGeneratesTouches)
        {

            this.mouseGeneratesTouches = mouseGeneratesTouches;
        }

        public override int GetHashCode ()
        {
            throw new NotImplementedException ();
        }

        internal void Update (AppTime appTime)
        {
            UpdateCurrentInputFrame ();

            foreach (var hid in this.humanInputDevices)
            {
                hid.Update (appTime, inputFrame);
            }
        }

        void UpdateCurrentInputFrame ()
        {

            var digitalControlStates = this.platform.hid_GetDigitalControlStates ();
            var analogControlStates = this.platform.hid_GetAnalogControlStates ();
            var binaryControlStates = this.platform.hid_GetBinaryControlStates ();
            var pressedCharacters = this.platform.hid_GetPressedCharacters ();
            var activeTouches = this.platform.hid_GetActiveTouches ();

            RawTouch mouseTouch = null;

            if (mouseGeneratesTouches)
            {
                Boolean mouseDownLastFrame = inputFrame.BinaryControlStates.Contains (BinaryControlIdentifier.Mouse_Left);
                Boolean mouseDownThisFrame = binaryControlStates.Contains (BinaryControlIdentifier.Mouse_Left);

                if (digitalControlStates.ContainsKey (DigitalControlIdentifier.Mouse_X) &&
                    digitalControlStates.ContainsKey (DigitalControlIdentifier.Mouse_Y))
                {
                    Single mouseX = digitalControlStates [DigitalControlIdentifier.Mouse_X];
                    Single mouseY = digitalControlStates [DigitalControlIdentifier.Mouse_Y];

                    if (!mouseDownLastFrame && mouseDownThisFrame)
                    {
                        currentMouseTouchId = "mouse" + Guid.NewGuid ().ToString ();
                        mouseTouch = new RawTouch {
                            Id = currentMouseTouchId,
                            Position = new Vector2 (mouseX, mouseY),
                            Phase = TouchPhase.JustPressed
                        };
                    }
                    else if (mouseDownLastFrame && mouseDownThisFrame)
                    {
                        mouseTouch = new RawTouch {
                            Id = currentMouseTouchId,
                            Position = new Vector2 (mouseX, mouseY),
                            Phase = TouchPhase.Active
                        };
                    }
                    else if (mouseDownLastFrame && !mouseDownThisFrame)
                    {
                        mouseTouch = new RawTouch {
                            Id = currentMouseTouchId,
                            Position = new Vector2 (mouseX, mouseY),
                            Phase = TouchPhase.JustReleased
                        };
                    }
                }
            }

            inputFrame.DigitalControlStates.Clear ();
            inputFrame.AnalogControlStates.Clear ();
            inputFrame.PressedCharacters.Clear ();
            inputFrame.ActiveTouches.Clear ();
            inputFrame.BinaryControlStates.Clear ();

            if (mouseTouch != null)
                inputFrame.ActiveTouches.Add (mouseTouch);

            digitalControlStates.Keys
                .ToList ()
                .ForEach (k => inputFrame.DigitalControlStates.Add (k, digitalControlStates [k]));

            analogControlStates.Keys
                .ToList ()
                .ForEach (k => inputFrame.AnalogControlStates.Add (k, analogControlStates [k]));

            binaryControlStates
                .ToList ()
                .ForEach (k => inputFrame.BinaryControlStates.Add (k));

            pressedCharacters
                .ToList ()
                .ForEach (k => inputFrame.PressedCharacters.Add (k));

            activeTouches
                .ToList ()
                .ForEach (k => inputFrame.ActiveTouches.Add (k));
        }

        /// <summary>
        /// Provides access to an Xbox 360 gamepad.
        /// </summary>
        public Xbox360Gamepad Xbox360Gamepad { get; private set; }

        /// <summary>
        /// Provides access to the virtual gamepad used by PlayStation Mobile systems, if you are running on Vita
        /// this will be the Vita itself.
        /// </summary>
        public PsmGamepad PsmGamepad { get; private set; }

        /// <summary>
        /// Provides access to a generalised multitouch pad, which may or may not have a screen.
        /// </summary>
        public MultiTouchController MultiTouchController { get; private set; }

        /// <summary>
        /// Provides access to a very basic gamepad, supported by most implementations.
        /// </summary>
        public GenericGamepad GenericGamepad { get; private set; }

        /// <summary>
        /// Provides access to a desktop mouse.
        /// </summary>
        public Mouse Mouse { get; private set; }

        /// <summary>
        /// Provides access to a desktop keyboard.
        /// </summary>
        public Keyboard Keyboard { get; private set; }
    }
}
