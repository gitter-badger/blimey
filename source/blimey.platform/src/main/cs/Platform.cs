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
    /// The Cor App Framework provides a user's app access to Cor features via this interface.
    /// </summary>
    public sealed class Platform
        : IDisposable
    {
        readonly Audio audio;
        readonly Graphics graphics;
        readonly Resources resources;
        readonly Status status;
        readonly Input input;
        readonly Host host;

        readonly IApi api;
        IApp userApp;
        AppSettings userAppSettings;

        readonly Stopwatch timer = new Stopwatch ();

        Boolean running = false;
        Single elapsedTime;
        Int64 frameCounter = -1;
        TimeSpan previousTimeSpan;
        Boolean firstUpdate = true;
        Boolean firstRender = true;

        public Platform (IApi api)
        {
            this.api = api;

            this.audio = new Audio (api);
            this.graphics = new Graphics (api);
            this.resources = new Resources (api);
            this.status = new Status (api);
            this.input = new Input (api);
            this.host = new Host (api);

        }

        public void Start (AppSettings userAppSettings, IApp userApp)
        {
            if (running) throw new Exception ("Already running!");

            this.userAppSettings = userAppSettings;
            this.userApp = userApp;

            // apply settings
            input.UpdateSettings (userAppSettings.MouseGeneratesTouches);

            // start the app
            api.app_Start (Update, Render);
            this.running = true;
        }

        public void Stop ()
        {
            if (!running) throw new Exception ("Already stopped!");

            api.app_Stop ();

            this.userAppSettings = null;
            this.userApp = null;
            this.running = false;
        }

        /// <summary>
        /// Provides access to Cor's audio manager.
        /// </summary>
        public Audio Audio { get { return audio; } }

        /// <summary>
        /// Provides access to Cor's graphics manager, which  provides an interface to working with the GPU.
        /// </summary>
        public Graphics Graphics { get { return graphics; } }

        public Resources Resources { get { return resources; } }

        /// <summary>
        /// Provides information about the current state of the App.
        /// </summary>
        public Status Status { get { return status; } }

        /// <summary>
        /// Provides access to Cor's input manager.
        /// </summary>
        public Input Input { get { return input; } }

        /// <summary>
        /// Provides information about the hardware and environment.
        /// </summary>
        public Host Host { get { return host; } }

        /// <summary>
        /// Provides access to Cor's logging system.
        /// </summary>
        public LogManager Log { get { throw new NotImplementedException (); } }

        /// <summary>
        /// Gets the settings used to initilise the app.
        /// </summary>
        public AppSettings Settings { get { return userAppSettings; } }

        public Boolean IsRunning { get { return running; } }

        void Update ()
        {
            if (firstUpdate)
            {
                firstUpdate = false;

                this.timer.Start ();

                this.userApp.Start (this);
            }

            var dt = (Single)(timer.Elapsed.TotalSeconds - previousTimeSpan.TotalSeconds);
            previousTimeSpan = timer.Elapsed;

            if (dt > 0.5f)
            {
                dt = 0.0f;
            }

            elapsedTime += dt;

            var appTime = new AppTime (dt, elapsedTime, ++frameCounter);

            this.input.Update (appTime);

            Boolean userAppToDie = this.userApp.Update (this, appTime);

            if (userAppToDie)
            {
                timer.Stop ();
                userApp.Stop (this);
                api.app_Stop ();
            }

            VertexBuffer.CollectGpuGarbage (api);
            IndexBuffer.CollectGpuGarbage (api);
            Texture.CollectGpuGarbage (api);
            Shader.CollectGpuGarbage (api);
        }

        void Render ()
        {
            if (firstRender)
            {
                firstRender = false;

                this.Graphics.Reset ();
            }

            this.userApp.Render (this);
        }

        public void Dispose ()
        {
            // todo
        }
    }
}
