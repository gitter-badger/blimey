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
    /// The Cor App Framework provides a user's app access to Cor features via this interface.
    /// </summary>
    public sealed class Engine
        : IDisposable
    {
        readonly Audio audio;
        readonly Graphics graphics;
        readonly Resources resources;
        readonly Status status;
        readonly Input input;
        readonly Host host;

        readonly IPlatform platform;

        Single elapsedTime;
        Int64 frameCounter = -1;
        TimeSpan previousTimeSpan;

        readonly IApp userApp;

        readonly Stopwatch timer = new Stopwatch ();
        Boolean firstUpdate = true;

        public Engine (IPlatform platform, AppSettings appSettings, IApp userApp)
        {
            this.platform = platform;
            this.userApp = userApp;

            this.platform.Program.Start (this.platform.Api, this.Update, this.Render);

            this.audio = new Audio (this.platform.Api);
            this.graphics = new Graphics (this.platform.Api);
            this.resources = new Resources (this.platform.Api);
            this.status = new Status (this.platform.Api);
            this.input = new Input (this.platform.Api, appSettings.MouseGeneratesTouches);
            this.host = new Host (this.platform.Api);

            this.Settings = appSettings;
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
        public LogManager Log { get; private set; }

        /// <summary>
        /// Gets the settings used to initilise the app.
        /// </summary>
        public AppSettings Settings { get; private set; }

        void Update ()
        {
            if (firstUpdate)
            {
                firstUpdate = false;

                this.Graphics.Reset ();

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
                this.userApp.Stop (this);
                this.platform.Program.Stop ();
            }

            VertexBuffer.CollectGpuGarbage (this.platform.Api);
            IndexBuffer.CollectGpuGarbage (this.platform.Api);
            Texture.CollectGpuGarbage (this.platform.Api);
            Shader.CollectGpuGarbage (this.platform.Api);
        }

        void Render ()
        {
            this.userApp.Render (this);
        }

        public void Dispose ()
        {
        }
    }
}