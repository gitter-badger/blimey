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

namespace Blimey.Engine
{
    using System;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Fudge;
    using global::Blimey.Platform;
    using global::Blimey.Asset;
    using Abacus.SinglePrecision;
    using Oats;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    public abstract partial class Stage
    {
        // ======================== //
        // Consumers must implement //
        // ======================== //

        public abstract void Start ();


        // If the this returns a function for creating a new state then we need to shut
        // it down and start the returned state.

        //  If a game state returns null then we need to
        // shut the platform down.

        public abstract Func<Stage> Update (AppTime time);

        public abstract void Shutdown ();


        // ========== //
        // Stage Data //
        // ========== //

        Platform platform = null;
        Engine engine = null;

        Boolean isRunning = false;
        Boolean firstUpdate = true;

        readonly Configuration configuration = null;
        readonly RuntimeConfiguration runtimeConfiguration = null;

        // ======================= //
        // Functions for consumers //
        // ======================= //
        public Platform Platform { get { return platform; } }
        public Engine Engine { get { return engine; } }

        public Boolean Active { get { return isRunning;} }
        public Configuration Config { get { return configuration; } }
        public RuntimeConfiguration RuntimeConfig { get { return runtimeConfiguration; } }

        protected Stage (Configuration configuration = null)
        {
            if (configuration == null)
                this.configuration = Configuration.CreateDefault ();
            else
                this.configuration = configuration;

            this.runtimeConfiguration = new RuntimeConfiguration (this);
        }

        // ===================== //
        // Blimey internal calls //
        // ===================== //

        internal void Initialize (Platform platform, Engine engine)
        {
            this.platform = platform;
            this.engine = engine;
            this.Start();
        }

        internal Func<Stage> RunUpdate (AppTime time)
        {
            if (this.firstUpdate)
            {
                this.firstUpdate = false;
                this.isRunning = true;
            }

            var ret = this.Update(time);
            return ret;
        }

        internal void RunShutdown ()
        {
            this.Shutdown ();
            this.isRunning = false;
        }
    }
}
