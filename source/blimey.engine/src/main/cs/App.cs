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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Fudge;
    using Abacus.SinglePrecision;
    using Oats;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Blimey provides its own implementation of <see cref="Cor.IApp"/> which means you don't need to do so yourself,
    /// instead instantiate <see cref="Blimey.Blimey"/> by providing it with the <see cref="Blimey.Scene"/> you wish
    /// to run and use it as you would a custom <see cref="Cor.IApp"/> implementation.
    /// Blimey takes care of the Update and Render loop for you, all you need to do is author <see cref="Blimey.Scene"/>
    /// objects for Blimey to handle.
    /// </summary>
    public class App
        : IApp
    {
        class FpsHelper
        {
            Single fps = 0;
            TimeSpan sampleSpan;
            Stopwatch stopwatch;
            Int32 sampleFrames;

            Single Fps { get { return fps; } }

            public FpsHelper()
            {
                sampleSpan = TimeSpan.FromSeconds(1);
                fps = 0;
                sampleFrames = 0;
                stopwatch = Stopwatch.StartNew();
            }

            public void Update(AppTime time)
            {
                if (stopwatch.Elapsed > sampleSpan)
                {
                    // Update FPS value and start next sampling period.
                    fps = (Single)sampleFrames / (Single)stopwatch.Elapsed.TotalSeconds;

                    stopwatch.Reset();
                    stopwatch.Start();
                    sampleFrames = 0;
                }
            }

            public void LogRender()
            {
                sampleFrames++;
            }
        }

        class FrameBufferHelper
        {
            Rgba32 randomColour = Rgba32.CornflowerBlue;
            const Single colourChangeTime = 5.0f;
            Single colourChangeTimer = 0.0f;

            Graphics gfx;

            public FrameBufferHelper(Graphics gfx)
            {
                this.gfx = gfx;
            }

            public void Update(AppTime time)
            {
                colourChangeTimer += time.Delta;

                if (colourChangeTimer > colourChangeTime)
                {
                    colourChangeTimer = 0.0f;
                    randomColour = RandomGenerator.Default.GetRandomColour();
                }
            }

            public void Clear()
            {
                gfx.ClearColourBuffer(randomColour);
            }
        }

        Scene startScene;
        SceneManager sceneManager;
        protected Blimey blimey;

        FpsHelper fps;
        FrameBufferHelper frameBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Blimey.Blimey"/> class.
        /// </summary>
        public App (Scene startScene)
        {
            this.startScene = startScene;
        }

        /// <summary>
        /// Blimey's initilisation rountine.
        /// </summary>
        public virtual void Start(Engine cor)
        {
            fps = new FpsHelper();
            frameBuffer = new FrameBufferHelper(cor.Graphics);
            blimey = new Blimey (cor);
            sceneManager = new SceneManager(cor, blimey, startScene);
        }

        /// <summary>
        /// Blimey's root update loop.
        /// </summary>
        public virtual Boolean Update(Engine cor, AppTime time)
        {
            FrameStats.SlowLog ();
            FrameStats.Reset ();

            using (new ProfilingTimer(t => FrameStats.Add ("UpdateTime", t)))
            {
                fps.Update(time);
                frameBuffer.Update(time);

                return this.sceneManager.Update(time);
            }
        }

        /// <summary>
        /// Blimey's render loop.
        /// </summary>
        public virtual void Render(Engine cor)
        {
            using (new ProfilingTimer(t => FrameStats.Add ("RenderTime", t)))
            {
                fps.LogRender();
                frameBuffer.Clear();
                this.sceneManager.Render();
            }
        }

        /// <summary>
        /// Blimey's termination routine.
        /// </summary>
        public virtual void Stop (Engine cor)
        {
        }
    }
}