// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// │ A partial implementation of the Blimey Plaform API targeting           │ \\
// │ Microsoft's WPF framework.  This partial implementation does not       │ \\
// │ implement any of the Blimey Plaform API's `gfx` calls and is intended  │ \\
// │ to be compiled alongside either:                                       │ \\
// │ - the OpenTK partial file with PLATFORM_WPF defined.                   │ \\
// │ - the Xna4 partial file with PLATFORM_XNA4_X86 defined.                │ \\
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

namespace Blimey
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Diagnostics;
    using global::OpenTK;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    public class Foo { public Int32 bar; }

    public class Platform
		: IPlatform
    {

        public Platform()
        {
            var program = new Program();
            var api = new Api();

            api.InitialiseDependencies(program);
            program.InitialiseDependencies(api);

            Api = api;
			_linuxProgram = program;

        }

		public void Run()
		{
			_linuxProgram.Run (24, 32);
		}

		private readonly Program _linuxProgram;
		public IProgram Program { get { return _linuxProgram; } }
        public IApi Api { get; private set; }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class Program : GameWindow, IProgram
    {
        Api Api { get; set; }

		internal void InitialiseDependencies(Api api) { Api = api; }
        Action update;
        Action render;

		bool loaded = false;
		bool firstUpdate = false;

        public void Start(IApi platformImplementation, Action update, Action render)
        {
            this.update = update;
            this.render = render;

			Load += (object sender, EventArgs e) => { this.loaded = true; };

			this.UpdateFrame += (object sender, FrameEventArgs e) =>
			{
				if( !firstUpdate )
				{
					firstUpdate = true;
				}

				double milliseconds = ComputeTimeSlice();
				Accumulate(milliseconds);
				update();
				Animate(milliseconds);
			};

			this.RenderFrame += (object sender, FrameEventArgs e) =>
			{
				//don't render until we have loaded and run the first update
				if (!loaded || !firstUpdate)
					return;

				render();
				SwapBuffers();
			};
        }

        public void Stop()
        {
        }

        float rotation = 0;
        private void Animate(double milliseconds)
        {
            float deltaRotation = (float)milliseconds / 20.0f;
            rotation += deltaRotation;

            //glControl.Invalidate();
        }

        double accumulator = 0;
        int idleCounter = 0;
        private void Accumulate(double milliseconds)
        {
            idleCounter++;
            accumulator += milliseconds;
            if (accumulator > 1000)
            {
                accumulator -= 1000;
                idleCounter = 0;
            }
        }

        Stopwatch sw = new Stopwatch();
        private double ComputeTimeSlice()
        {
            sw.Stop();
            double timeslice = sw.Elapsed.TotalMilliseconds;
            sw.Reset();
            sw.Start();
            return timeslice;
        }

    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public partial class Api
        : IApi
    {
        Program Program { get; set; }

        internal void InitialiseDependencies(Program program)
        {
            Program = program;
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
        public System.Boolean? app_IsFullscreen()
        {
			return Program.WindowState == WindowState.Fullscreen;
        }

        public Int32 app_GetWidth()
        {
			return Program.Width;
        }

        public Int32 app_GetHeight()
        {
			return Program.Height;
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
