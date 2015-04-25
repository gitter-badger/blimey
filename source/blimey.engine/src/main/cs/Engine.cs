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

    public sealed partial class Engine
    {
        // STAGE MANAGEMENT
        Stage activeStage = null;
        Func<Stage> createNextStageFn = null;

        // ENGINE COMPONENTS

/*
        FrameGauge frameGauge;
        ColourWiper colourWiper;
        AssetSystem assets;
        InputEventSystem inputEventSystem;
        DebugBatcher debugBatcher;
        PrimitiveBatcher primitiveBatcher;
        SceneGraph sceneGraph;

        public FrameGauge FrameGauge { get { return frameGauge; } }
        public ColourWiper ColourWiper { get { return colourWiper; } }
        public AssetSystem Assets { get { return assets; } }
        public InputEventSystem Touch { get { return inputEventSystem; } }
        public DebugBatcher DebugBatcher { get { return debugBatcher; } }
        public PrimitiveBatcher PrimitiveBatcher { get { return primitiveBatcher; } }
        public SceneGraph SceneGraph { get { return sceneGraph; } }
*/
        readonly Func<Stage> createStartStageFn = null;
        readonly List<Component> components = new List <Component> ();

        public Engine (Func<Stage> createStartStageFn)
        {
            this.createStartStageFn = createStartStageFn;
        }

        internal void Start (Platform platform)
        {
            activeStage = null;
            createNextStageFn = createStartStageFn;
/*
            frameGauge =           components.AddEx (new FrameGauge ());
            colourWiper =          components.AddEx (new ColourWiper ());
            assets =               components.AddEx (new AssetSystem (platform));
            inputEventSystem =     components.AddEx (new InputEventSystem (platform));
            debugBatcher =         components.AddEx (new DebugBatcher (platform));
            primitiveBatcher =     components.AddEx (new PrimitiveBatcher (platform));
            sceneGraph =           components.AddEx (new SceneGraph (platform));
*/
        }

        internal void Stop (Platform platform)
        {
            createNextStageFn = null;

            components.Clear ();
/*
            frameGauge = null;
            colourWiper = null;
            assets = null;
            inputEventSystem = null;
            debugBatcher = null;
            primitiveBatcher = null;
            sceneGraph = null;
*/
        }

        internal Boolean Update (Platform platform, AppTime time)
        {
            /*
            // If quitting the game
            if (createNextStageFn == null)
            {
                activeStage.Uninitilise ();
                activeStage = null;
                return true;
            }

            // Shutdown active stage, we'll init a new one
            // next update.
            if (nextStage != activeStage && activeStage != null)
            {
                activeStage.Uninitilise ();
                activeStage = null;
                GC.Collect ();
                return false;
            }

            if (nextStage != activeStage)
            {
                activeStage = nextStage;
                activeStage.Initialize (platform, engine);
            }

            if (activeStage == null)
            {
                return;
            }

            nextStage = activeStage.RunUpdate (time);



            frameStats.SlowLog ();
            frameStats.Reset ();

            using (new ProfilingTimer (t => frameStats.Add ("UpdateTime", t)))
            {
                foreach (var component in components)
                {
                    using (new ProfilingTimer (t => frameStats.Add (component.Name + " UpdateTime", t)))
                    {
                        component.Update (platform, time);
                    }
                }

                fps.Update (time);
                frameBuffer.Update (time);

                debugBatcher.Update (time);
                inputEventSystem.Update (time);

                val x = sceneGraph.Update (time);

                primitiveBatcher.PostUpdate (time);

                return x;
            }
            */
            return false;
        }

        internal void Render (Platform platform)
        {
            /*
            if (activeStage == null) return;

            using (new ProfilingTimer (t => FrameStats.Add ("RenderTime", t)))
            {
                fps.LogRender ();
                frameBuffer.Clear ();

                // Clear the background colour if the scene settings want us to.
                if (scene.Configuration.BackgroundColour.HasValue)
                {
                    platform.Graphics.ClearColourBuffer (scene.Configuration.BackgroundColour.Value);
                }

                platform.Graphics.Reset ();

                if (activeStage != null && activeStage.Active)
                {
                    sceneRenderer.Render (activeStage);
                }

                foreach (var renderPass in scene.Configuration.RenderPasses)
                {
                    // #0: Apply this pass' clear settings.
                    if (pass.Configuration.ClearDepthBuffer)
                    {
                        this.platform.Graphics.ClearDepthBuffer ();
                    }

                    // #0: Render the scene graph.
                    using (new ProfilingTimer (t => FrameStats.Add ("StageGraphRenderTime", t)))
                    {
                        StageManager.Render (this.platform.Graphics, pass.Name);
                    }

                    using (new ProfilingTimer (t => FrameStats.Add ("PrimitiveRenderTime", t)))
                    {
                        // #2: Render all primitives that are associated with this pass.
                        PrimitivBatcher.Render (this.platform.Graphics, pass.Name, cam.ViewMatrix44, cam.ProjectionMatrix44);
                    }

                    using (new ProfilingTimer (t => FrameStats.Add ("DebugRenderTime", t)))
                    {
                        // #3: Render all debug primitives that are associated with this pass.
                        DebugBatcher.Render (this.platform.Graphics, pass.Name, cam.ViewMatrix44, cam.ProjectionMatrix44);
                    }
                }
            }
            */
        }
    }
}
