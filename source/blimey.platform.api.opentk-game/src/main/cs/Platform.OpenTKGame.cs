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
// │ ~ Ash Pook (http://www.ajpook.com)										│ \\
// │ ~ Ryan Sullivan (http://ryanpsullivan.github.io)                       │ \\
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
    using System.Collections.Generic;
    using System.IO;
    using System.Diagnostics;
    using global::OpenTK;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public partial class Api
        : IApi
    {
        public void Run () { gameWindow.Run (24, 32); }

        GameWindow gameWindow = null;

        double accumulator = 0;
        int idleCounter = 0;
        Stopwatch sw = new Stopwatch();
        float rotation = 0;
        bool loaded = false;
        bool firstUpdate = false;

        Action update;
        Action render;

        void Animate (double milliseconds)
        {
            float deltaRotation = (float)milliseconds / 20.0f;
            rotation += deltaRotation;
        }

        void Accumulate (double milliseconds)
        {
            idleCounter++;
            accumulator += milliseconds;
            if (accumulator > 1000)
            {
                accumulator -= 1000;
                idleCounter = 0;
            }
        }

        double ComputeTimeSlice()
        {
            sw.Stop();
            double timeslice = sw.Elapsed.TotalMilliseconds;
            sw.Reset();
            sw.Start();
            return timeslice;
        }

        void OnLoad (object sender, EventArgs e){ this.loaded = true; }

        void OnUpdate (object sender, FrameEventArgs e)
        {
            if( !firstUpdate )
            {
                firstUpdate = true;
            }

            double milliseconds = ComputeTimeSlice();
            Accumulate(milliseconds);
            update();
            Animate(milliseconds);
        }

        void OnRender (object sender, FrameEventArgs e)
        {
            //don't render until we have loaded and run the first update
            if (!loaded || !firstUpdate)
            {
                return;
            }

            render ();

            gameWindow.SwapBuffers();
        }


        /**
         * Audio
         */
        public void sfx_SetVolume(Single volume)
        {
            throw new NotImplementedException();
        }

        public Single sfx_GetVolume()
        {
            throw new NotImplementedException();
        }

        /**
         * Resources
         */
        public Stream res_GetFileStream (String fileName)
        {
            if (!File.Exists (fileName))
            {
                throw new FileNotFoundException (fileName);
            }

            return new FileStream (fileName, FileMode.Open);
        }

        /**
         * System
         */
        public String sys_GetMachineIdentifier()
        {
            throw new NotImplementedException();
        }

        public String sys_GetOperatingSystemIdentifier()
        {
            throw new NotImplementedException();
        }

        public String sys_GetVirtualMachineIdentifier()
        {
            throw new NotImplementedException();
        }

        public Int32 sys_GetPrimaryScreenResolutionWidth()
        {
            throw new NotImplementedException();
        }

        public Int32 sys_GetPrimaryScreenResolutionHeight()
        {
            throw new NotImplementedException();
        }

        public Abacus.SinglePrecision.Vector2? sys_GetPrimaryPanelPhysicalSize()
        {
            return null;
        }

        public PanelType sys_GetPrimaryPanelType()
        {
            return PanelType.Screen;
        }


        /**
         * Application
         */
        public void app_Start (Action update, Action render)
        {
            this.update = update;
            this.render = render;

            gameWindow = new GameWindow ();

            gameWindow.Load += OnLoad;
            gameWindow.UpdateFrame += OnUpdate;
            gameWindow.RenderFrame += OnRender;
        }

        public void app_Stop ()
        {
            gameWindow.Load -= OnLoad;
            gameWindow.UpdateFrame -= OnUpdate;
            gameWindow.RenderFrame -= OnRender;

            gameWindow = null;

            this.update = null;
            this.render = null;

            this.loaded = false;
            this.firstUpdate = false;
        }

        public System.Boolean? app_IsFullscreen()
        {
			return gameWindow.WindowState == WindowState.Fullscreen;
        }

        public Int32 app_GetWidth()
        {
			return gameWindow.Width;
        }

        public Int32 app_GetHeight()
        {
			return gameWindow.Height;
        }



        /**
         * Input
         */
        public DeviceOrientation? hid_GetCurrentOrientation()
        {
            return DeviceOrientation.Default;
        }


        public Dictionary<DigitalControlIdentifier, Int32> hid_GetDigitalControlStates()
        {
			return new Dictionary<DigitalControlIdentifier, int> ();
        }

        public Dictionary<AnalogControlIdentifier, Single> hid_GetAnalogControlStates()
        {
			return new Dictionary<AnalogControlIdentifier, Single> ();
        }

        public HashSet<BinaryControlIdentifier> hid_GetBinaryControlStates()
        {
			return new HashSet<BinaryControlIdentifier> ();
        }

        public HashSet<Char> hid_GetPressedCharacters()
        {
			return new HashSet<char> ();
        }

        public HashSet<RawTouch> hid_GetActiveTouches()
        {
			return new HashSet<RawTouch> ();
        }
    }
}
