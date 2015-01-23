// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! WFP Platform Implementation                                                                               │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │                     Brought to you by:                                                                         │ \\
// │                              _________                    .__               __                                 │ \\
// │                             /   _____/__ __  ____    ____ |__|____    _____/  |_                               │ \\
// │                             \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\                              │ \\
// │                             /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |                                │ \\
// │                            /_______  /____/|___|  /\___  /|__(____  /___|  /__|                                │ \\
// │                                    \/           \//_____/         \/     \/                                    │ \\
// │                                                                                                                │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2008-2014 A.J.Pook (http://ajpook.github.io)                                                       │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors: A.J.Pook                                                                                              │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated   │ \\
// │ documentation files (the "Software"), to deal in the Software without restriction, including without           │ \\
// │ limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sellcopies of the   │ \\
// │ Software, and to permit persons to whom the Software is furnished to do so, subject to the following           │ \\
// │ conditions:                                                                                                    │ \\
// │                                                                                                                │ \\
// │ The above copyright notice and this permission notice shall be included in all copies or substantial portions  │ \\
// │ of the Software.                                                                                               │ \\
// │                                                                                                                │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED  │ \\
// │ TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL │ \\
// │ THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF  │ \\
// │ CONTRACT, TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER        │ \\
// │ DEALINGS IN THE SOFTWARE.                                                                                      │ \\
// └────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘ \\



namespace Cor.Platform.Wpf
{
    using System;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Runtime.ConstrainedExecution;
    using System.Threading;

    using Cor;
    using Platform;
    using Fudge;
    using Abacus.SinglePrecision;
    using System.Windows.Forms.Integration;
    using global::OpenTK;
    using global::OpenTK.Graphics;
    using global::OpenTK.Graphics.OpenGL;
    using System.Windows.Forms;
    using System.Windows.Interop;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    public class Foo { public Int32 bar; }

    public class CorWpfPlatform
        : IPlatform
    {

        public CorWpfPlatform(WindowsFormsHost winformsHost)
        {
            var program = new WpfProgram();
            var api = new Xna4Api();

            api.InitialiseDependencies(program);
            program.InitialiseDependencies(api);

            Api = api;
            Program = program;

            winformsHost.Child = program.glControl;
        }

        public IProgram Program { get; private set; }
        public IApi Api { get; private set; }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class WpfProgram : IProgram
    {
        public GLControl glControl { get; private set; }

        Xna4Api Api { get; set; }

        internal void InitialiseDependencies(Xna4Api api) { Api = api; }
        Action update;
        Action render;
        bool loaded;
        public void Start(IApi platformImplementation, Action update, Action render)
        {
            this.update = update;
            this.render = render;
            ComponentDispatcher.ThreadIdle += (sender, e) => System.Windows.Forms.Application.RaiseIdle(e);
            glControl = new GLControl(new GraphicsMode(32, 24), 2, 0, GraphicsContextFlags.Default);
            glControl.Paint += (object sender, PaintEventArgs e) => {
                if (!loaded)
                    return;
                render();
                glControl.SwapBuffers();
            };
            glControl.Dock = DockStyle.Fill;
            glControl.Load += (object sender, EventArgs e) => { this.loaded = true; };
            Application.Idle += (object sender, EventArgs e) => {
                double milliseconds = ComputeTimeSlice();
                Accumulate(milliseconds);
                update();
                Animate(milliseconds);
            };
            glControl.MakeCurrent();
        }

        public void Stop()
        {
        }

        float rotation = 0;
        private void Animate(double milliseconds)
        {
            float deltaRotation = (float)milliseconds / 20.0f;
            rotation += deltaRotation;
            glControl.Invalidate();
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

    public partial class Xna4Api
        : IApi
    {
        WpfProgram Program { get; set; }

        internal void InitialiseDependencies(WpfProgram program)
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
        public Stream res_GetFileStream(String fileName)
        {
            throw new NotImplementedException();
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
            return false;
        }

        public Int32 app_GetWidth()
        {
            return 800;
        }

        public Int32 app_GetHeight()
        {
            return 600;
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
            throw new NotImplementedException();
        }

        public Dictionary<AnalogControlIdentifier, Single> hid_GetAnalogControlStates()
        {
            throw new NotImplementedException();
        }

        public HashSet<BinaryControlIdentifier> hid_GetBinaryControlStates()
        {
            throw new NotImplementedException();
        }

        public HashSet<Char> hid_GetPressedCharacters()
        {
            throw new NotImplementedException();
        }

        public HashSet<RawTouch> hid_GetActiveTouches()
        {
            throw new NotImplementedException();
        }
    }
}