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

    public partial class Stage
    {
        public class Configuration
        {
            readonly Dictionary<String, RenderPass> renderPasses = new Dictionary<String, RenderPass> ();

            public Rgba32? BackgroundColour { get; set; }

            public List<RenderPass> RenderPasses { get { return renderPasses.Values.ToList (); } }

            internal Configuration() {}

            public void AddRenderPass (String passName, RenderPass.Configuration renderPassConfig)
            {
                if (renderPasses.ContainsKey (passName))
                {
                    throw new Exception("Can't have render passes with the same name");
                }

                var renderPass = new RenderPass ()
                {
                    Name = passName,
                    Config = renderPassConfig
                };

                renderPasses.Add(passName, renderPass);
            }

            public void RemoveRenderPass (String passName)
            {
                renderPasses.Remove (passName);
            }

            public RenderPass GetRenderPass(String passName)
            {
                if (!renderPasses.ContainsKey (passName))
                {
                    return null;
                }

                return renderPasses[passName];
            }

            public static Configuration CreateVanilla ()
            {
                var ss = new Configuration ();
                return ss;
            }

            public static Configuration CreateDefault ()
            {
                var ss = new Configuration ();

                ss.BackgroundColour = Rgba32.Crimson;

                var debugPassSettings = new RenderPass.Configuration ();
                debugPassSettings.ClearDepthBuffer = true;
                ss.AddRenderPass ("Debug", debugPassSettings);

                var defaultPassSettings = new RenderPass.Configuration ();
                defaultPassSettings.EnableDefaultLighting = true;
                defaultPassSettings.FogEnabled = true;
                ss.AddRenderPass ("Default", defaultPassSettings);

                var guiPassSettings = new RenderPass.Configuration ();
                guiPassSettings.ClearDepthBuffer = true;
                //guiPassSettings.Camera.ProjectionType = CameraProjectionType.Orthographic;
                ss.AddRenderPass ("Gui", guiPassSettings);

                return ss;
            }
        }
    }
}
