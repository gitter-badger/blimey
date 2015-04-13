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
    /// Scenes are a engine feature that provide a simple scene graph and higher
    /// level abstraction of the renderering pipeline.
    /// </summary>
    public abstract class Scene
    {
        /// <summary>
        /// Game scene settings are used by the platform to detemine how
        /// to manage an associated scene.  For example the game scene
        /// settings are used to define the render pass for the scene.
        /// </summary>
        public class SceneConfiguration
        {
            readonly Dictionary<String, RenderPass> renderPasses = new Dictionary<String, RenderPass> ();

            public Rgba32? BackgroundColour { get; set; }

            public List<RenderPass> RenderPasses { get { return renderPasses.Values.ToList (); } }

            internal SceneConfiguration() {}

            public void AddRenderPass (String passName, RenderPass.RenderPassConfiguration renderPassConfig)
            {
                if (renderPasses.ContainsKey (passName))
                {
                    throw new Exception("Can't have render passes with the same name");
                }

                var renderPass = new RenderPass ()
                {
                    Name = passName,
                    Configuration = renderPassConfig
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

            public static SceneConfiguration CreateVanilla ()
            {
                var ss = new SceneConfiguration ();
                return ss;
            }

            public static SceneConfiguration CreateDefault ()
            {
                var ss = new SceneConfiguration ();

                ss.BackgroundColour = Rgba32.Crimson;

                var debugPassSettings = new RenderPass.RenderPassConfiguration ();
                debugPassSettings.ClearDepthBuffer = true;
                ss.AddRenderPass ("Debug", debugPassSettings);

                var defaultPassSettings = new RenderPass.RenderPassConfiguration ();
                defaultPassSettings.EnableDefaultLighting = true;
                defaultPassSettings.FogEnabled = true;
                ss.AddRenderPass ("Default", defaultPassSettings);

                var guiPassSettings = new RenderPass.RenderPassConfiguration ();
                guiPassSettings.ClearDepthBuffer = true;
                guiPassSettings.CameraProjectionType = CameraProjectionType.Orthographic;
                ss.AddRenderPass ("Gui", guiPassSettings);

                return ss;
            }
        }

        /// <summary>
        /// Provides a means to change some scene configuration settings at runtime,
        /// not settings can be changed at runtime.
        /// </summary>
        public class SceneRuntimeConfiguration
        {
            readonly Scene parent = null;

            internal SceneRuntimeConfiguration (Scene parent)
            {
                this.parent = parent;
            }

            public void ChangeBackgroundColour (Rgba32? colour)
            {
                parent.configuration.BackgroundColour = colour;
            }

            public void SetRenderPassCameraToDefault(string renderPass)
            {
                parent.cameraManager.SetDefaultCamera (renderPass);
            }

            public void SetRenderPassCameraTo (string renderPass, Entity go)
            {
                parent.cameraManager.SetMainCamera(renderPass, go);
            }

        }

        public class SceneSceneGraph
        {
            readonly Scene parent = null;

            public SceneSceneGraph (Scene parent)
            {
                this.parent = parent;
            }
            readonly List<Entity> sceneGraph = new List<Entity> ();

            public List<Entity> GetAllObjects ()
            {
                return sceneGraph;
            }

            public Entity CreateSceneObject (string zName)
            {
                var go = new Entity (parent, zName);
                sceneGraph.Add (go);
                return go;
            }

            public void DestroySceneObject (Entity zGo)
            {
                zGo.Shutdown ();
                foreach (Entity go in zGo.Children) {
                    this.DestroySceneObject (go);
                    sceneGraph.Remove (go);
                }
                sceneGraph.Remove (zGo);

                zGo = null;
            }
        }

        // ======================== //
        // Consumers must implement //
        // ======================== //

        public abstract void Start ();

        public abstract Scene Update (AppTime time);

        public abstract void Shutdown ();


        // ========== //
        // Scene Data //
        // ========== //

        Platform platform = null;
        Engine engine = null;
        readonly SceneConfiguration configuration = null;
        readonly SceneRuntimeConfiguration runtimeConfiguration = null;
        CameraManager cameraManager = null;
        SceneSceneGraph sceneGraph = null;
        Boolean isRunning = false;
        Boolean firstUpdate = true;


        // ======================= //
        // Functions for consumers //
        // ======================= //
        public Platform Platform { get { return platform; } }
        public Engine Engine { get { return engine; } }

        public Boolean Active { get { return isRunning;} }
        public SceneConfiguration Configuration { get { return configuration; } }
        public SceneRuntimeConfiguration RuntimeConfiguration { get { return runtimeConfiguration; } }
        public SceneSceneGraph SceneGraph { get { return sceneGraph; } }
        public CameraManager CameraManager { get { return cameraManager; } }

        protected Scene (SceneConfiguration configuration = null)
        {
            if (configuration == null)
                this.configuration = SceneConfiguration.CreateDefault ();
            else
                this.configuration = configuration;

            this.runtimeConfiguration = new SceneRuntimeConfiguration (this);
        }

        // ===================== //
        // Blimey internal calls //
        // ===================== //

        internal void Initialize (Platform platform, Engine engine)
        {
            this.platform = platform;
            this.engine = engine;
            this.sceneGraph = new SceneSceneGraph (this);
            this.cameraManager = new CameraManager(this);

            this.Start();
        }

        internal Scene RunUpdate(AppTime time)
        {
            if (firstUpdate)
            {
                firstUpdate = false;
                isRunning = true;
            }

            this.engine.PreUpdate (time);

            foreach (Entity go in sceneGraph.GetAllObjects())
            {
                go.Update(time);
            }

            this.engine.PostUpdate (time);

            var ret =  this.Update(time);
            return ret;
        }

        internal virtual void Uninitilise ()
        {
            this.Shutdown ();
            this.isRunning = false;

            List<Entity> onesToDestroy = new List<Entity> ();

            foreach (Entity go in sceneGraph.GetAllObjects ())
            {
                if (go.Transform.Parent == null)
                {
                    onesToDestroy.Add (go);
                }
            }

            foreach (Entity go in onesToDestroy)
            {
                sceneGraph.DestroySceneObject (go);
            }

            Debug.Assert(sceneGraph.GetAllObjects ().Count == 0);
        }
    }
}
