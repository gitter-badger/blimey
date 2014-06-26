// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Blimey - Fast, efficient, high level engine built upon Cor & Abacus                                            │ \\
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

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Abacus;
    using Fudge;
    using Abacus.SinglePrecision;
    using System.Linq;
    using Cor;

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
            Single colourChangeTime = 5.0f;
            Single colourChangeTimer = 0.0f;

            IGraphicsManager gfx;

            public FrameBufferHelper(IGraphicsManager gfx)
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
		public virtual void Start(ICor cor)
        {
            fps = new FpsHelper();
            frameBuffer = new FrameBufferHelper(cor.Graphics);
            this.sceneManager = new SceneManager(cor, startScene);
        }

		/// <summary>
		/// Blimey's root update loop.
		/// </summary>
		public virtual Boolean Update(ICor cor, AppTime time)
        {
			//FrameStats.SlowLog ();
			FrameStats.Reset ();

            using (new ProfilingTimer(t => FrameStats.UpdateTime += t))
            {
                fps.Update(time);
                frameBuffer.Update(time);

                return this.sceneManager.Update(time);
            }
        }

		/// <summary>
		/// Blimey's render loop.
		/// </summary>
		public virtual void Render(ICor cor)
        {
            using (new ProfilingTimer(t => FrameStats.RenderTime += t))
            {
                fps.LogRender();
                frameBuffer.Clear();
                this.sceneManager.Render();
            }
        }

		/// <summary>
		/// Blimey's termination routine.
		/// </summary>
		public virtual void Stop (ICor cor)
		{
		}
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Blimey
    {
        ICor cor;

        internal Blimey (ICor cor, Scene.SceneConfiguration settings)
        {
            this.cor = cor;

            this.InputEventSystem = new InputEventSystem(this.cor);
            this.DebugShapeRenderer = new DebugShapeRenderer(
                this.cor,
                settings.RenderPasses);

        }

        public InputEventSystem InputEventSystem { get; set; }
        public DebugShapeRenderer DebugShapeRenderer { get; set; }

        internal void PreUpdate (AppTime time)
        {
            this.DebugShapeRenderer.Update(time);
            this.InputEventSystem.Update(time);
        }

        internal void PostUpdate(AppTime time)
        {

        }

        internal void PreRender()
        {

        }

        internal void PostRender()
        {

        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public abstract class Mesh
    {
        /// <summary>
        /// todo
        /// </summary>
        public Int32 TriangleCount;

        /// <summary>
        /// todo
        /// </summary>
        public Int32 VertexCount;

        /// <summary>
        /// todo
        /// </summary>
        public abstract VertexDeclaration VertDecl { get; }

        /// <summary>
        /// todo
        /// </summary>
        public IGeometryBuffer GeomBuffer;
    }


 	// ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
	public class RenderPass
	{
		public struct RenderPassConfiguration
	    {
			public static RenderPassConfiguration Default
			{
				get
				{
					var rpc = new RenderPassConfiguration ();
					rpc.ClearDepthBuffer = false;
					rpc.FogEnabled = false;
					rpc.FogColour = Rgba32.CornflowerBlue;
					rpc.FogStart = 300.0f;
					rpc.FogEnd = 550.0f;
					rpc.EnableDefaultLighting = true;
					rpc.CameraProjectionType = CameraProjectionType.Perspective;
					return rpc;
				}
			}
			
	        public Boolean ClearDepthBuffer;
	        public Boolean FogEnabled;
	        public Rgba32 FogColour;
	        public Single FogStart;
	        public Single FogEnd;
	        public Boolean EnableDefaultLighting;
	        public CameraProjectionType CameraProjectionType;
	    }
		
		internal RenderPass () {}
		public String Name { get; set; }
		public RenderPassConfiguration Configuration { get; set; }
	}

	
 	// ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

	/// <summary>
    /// Scenes are a blimey feature that provide a simple scene graph and higher
	/// level abstraction of the renderering pipeline.
	/// </summary>
    public abstract class Scene
    {
		/// <summary>
	    /// Game scene settings are used by the engine to detemine how
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
		
		public class ObjectGraph
		{
			readonly Scene parent = null;
			
			public ObjectGraph (Scene parent)
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
		
		ICor cor = null;
		Blimey blimey = null;
		readonly SceneConfiguration configuration = null;
		readonly SceneRuntimeConfiguration runtimeConfiguration = null;
		CameraManager cameraManager = null;
		ObjectGraph sceneGraph = null;
		Boolean isRunning = false;

		
		// ======================= //
		// Functions for consumers //
		// ======================= //
        public ICor Cor { get { return cor; } }
		public Blimey Blimey { get { return blimey; } }
		
		public Boolean Active { get { return isRunning;} }
		public SceneConfiguration Configuration { get { return configuration; } }
		public SceneRuntimeConfiguration RuntimeConfiguration { get { return runtimeConfiguration; } }
		public ObjectGraph SceneGraph { get { return sceneGraph; } }
		public CameraManager CameraManager { get { return cameraManager; } }

		public Scene (SceneConfiguration configuration = null)
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
		
        internal void Initialize (ICor cor)
        {
			this.cor = cor;
			this.blimey = new Blimey (cor, this.configuration);
			this.sceneGraph = new ObjectGraph (this);
            this.cameraManager = new CameraManager(this);
            this.Start();
            this.isRunning = true;
        }

        internal Scene RunUpdate(AppTime time)
        {
			this.blimey.PreUpdate (time);
			
			foreach (Entity go in sceneGraph.GetAllObjects())
			{
                go.Update(time);
            }

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


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class Entity
    {
        List<Trait> behaviours = new List<Trait> ();
        Transform location = new Transform ();
        String name = "SceneObject";
        readonly Scene containedBy;
        Boolean enabled = false;

        public Scene Owner { get { return containedBy; } }

        // Used to define where in the game engine's hierarchy this
        // game object exists.
        public Transform Transform { get { return location; } }

        // The name of the this game object, defaults to "SceneObject"
        // can be set upon creation or changed at anytime.  Only real
        // use is to doing lazy searches of the hierachy by name
        // and also making the hierachy look neat.
        public string Name { get { return name; } set { name = value; } }

        // Defines whether or not the SceneObject's behaviours should be updated
        // and rendered.
        public Boolean Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if( enabled == value )
                    return;

                Boolean changeFlag = false;

                changeFlag = true;
                enabled = value;

                if( changeFlag )
                {
                    foreach (var behaviour in behaviours)
                    {
                        if(enabled)
                            behaviour.OnEnable();
                        else
                            behaviour.OnDisable();
                    }
                }
            }
        }


        internal List<Entity> Children {
            get {
                List<Entity> kids = new List<Entity> ();
				foreach (Entity go in containedBy.SceneGraph.GetAllObjects ()) {
                    if (go.Transform.Parent == this.Transform)
                        kids.Add (go);
                }
                return kids;
            }
        }


        public T AddTrait<T> ()
            where T : Trait, new()
        {
            if( this.GetTrait<T>() != null )
                throw new Exception("This Trait already exists on the gameobject");

            T behaviour = new T ();
            behaviours.Add (behaviour);
			behaviour.Initilise(containedBy.Cor, containedBy.Blimey, this);

            behaviour.OnAwake();

            if( this.Enabled )
                behaviour.OnEnable();
            else
                behaviour.OnDisable();

            return behaviour;

        }

        public void RemoveTrait<T> ()
            where T : Trait
        {
            Trait trait = behaviours.Find(x => x is T );
            trait.OnDestroy();
            behaviours.Remove(trait);
        }

        public T GetTrait<T> ()
            where T : Trait
        {
            foreach (Trait b in behaviours) {
                if (b as T != null)
                    return b as T;
            }

            return null;
        }

        public T GetTraitInChildren<T> ()
            where T : Trait
        {
            foreach (var go in Children) {
                foreach (var b in go.behaviours) {
                    if (b as T != null)
                        return b as T;
                }
            }

            return null;
        }

        internal Entity (Scene containedBy, string name)
        {
            this.Name = name;

            this.containedBy = containedBy;

            // directly set _enabled to false, don't want any callbacks yet
            this.enabled = true;

        }

        internal void Update(AppTime time)
        {
            if (!Enabled)
                return;

            foreach (Trait behaviour in behaviours) {
                if (behaviour.Active)
                {
                    behaviour.OnUpdate(time);
                }
            }
        }

        internal void Shutdown ()
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.OnDestroy ();
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    //
    // BEHAVIOUR
    //
    // A game object can exhibit a number of behaviours.  The behaviours are defined as children of
    // this abstract class.
    //
    public abstract class Trait
    {
        protected ICor Cor { get; set; }
        protected Blimey Blimey { get; set; }

        public Entity Parent { get; set; }

        public Boolean Active { get; set; }

        // INTERNAL METHODS
        // called after constructor and before awake
		internal void Initilise (ICor cor, Blimey blimeyServices, Entity parent)
        {
            Cor = cor;
            Blimey = blimeyServices;
            Parent = parent;

            Active = true;
        }

        // PUBLIC CALLBACKS
        public virtual void OnAwake () {}

        public virtual void OnUpdate (AppTime time) {}

        // Called when the Enabled state of the parent gameobject changes
        public virtual void OnEnable() {}
        public virtual void OnDisable() {}
        
        public virtual void OnDestroy () {}
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

	// todo: this should be owned by the scene
    public static class LightingManager
    {
        public static Rgba32 ambientLightColour;
        public static Rgba32 emissiveColour;
        public static Rgba32 specularColour;
        public static Single specularPower;


        public static Boolean fogEnabled;
        public static Single fogStart;
        public static Single fogEnd;
        public static Rgba32 fogColour;


        public static Vector3 dirLight0Direction;
        public static Rgba32 dirLight0DiffuseColour;
        public static Rgba32 dirLight0SpecularColour;

        public static Vector3 dirLight1Direction;
        public static Rgba32 dirLight1DiffuseColour;
        public static Rgba32 dirLight1SpecularColour;

        public static Vector3 dirLight2Direction;
        public static Rgba32 dirLight2DiffuseColour;
        public static Rgba32 dirLight2SpecularColour;

        static LightingManager()
        {
            ambientLightColour = Rgba32.Black;
            emissiveColour = Rgba32.DarkSlateGrey;
            specularColour = Rgba32.DarkGrey;
            specularPower = 2f;

            fogEnabled = true;
            fogStart = 100f;
            fogEnd = 1000f;
            fogColour = Rgba32.BlueViolet;


            dirLight0Direction = new Vector3(-0.3f, -0.9f, +0.3f);
            Vector3.Normalise(ref dirLight0Direction, out dirLight0Direction);
            dirLight0DiffuseColour = Rgba32.DimGrey;
            dirLight0SpecularColour = Rgba32.DarkGreen;

            dirLight1Direction = new Vector3(0.3f, 0.1f, -0.3f);
            Vector3.Normalise(ref dirLight1Direction, out dirLight1Direction);
            dirLight1DiffuseColour = Rgba32.DimGrey;
            dirLight1SpecularColour = Rgba32.DarkRed;

            dirLight2Direction = new Vector3( -0.7f, -0.3f, +0.1f);
            Vector3.Normalise(ref dirLight2Direction, out dirLight2Direction);
            dirLight2DiffuseColour = Rgba32.DimGrey;
            dirLight2SpecularColour = Rgba32.DarkBlue;

        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal class SceneManager
    {
        Scene activeScene;
        ICor cor;

        SceneRenderManager renderManager;

        public event System.EventHandler SimulationStateChanged;

        public Scene ActiveState { get { return activeScene; } }

        public SceneManager (ICor cor, Scene startScene)
        {
            this.cor = cor;
            activeScene = startScene;
            activeScene.Initialize(cor);
            renderManager = new SceneRenderManager(cor);

        }

        public Boolean Update(AppTime time)
        {
            Scene a = activeScene.RunUpdate (time);

            // If the active state returns a game state other than itself then we need to shut
            // it down and start the returned state.  If a game state returns null then we need to
            // shut the engine down.

            //quitting the game
            if (a == null)
            {
                activeScene.Uninitilise ();
                return true;
            }
            else if (a != activeScene)
            {
                activeScene.Uninitilise ();

                activeScene = a;

                this.cor.Graphics.Reset();

                GC.Collect();

                activeScene.Initialize (cor);

                if (SimulationStateChanged != null)
                {
                    SimulationStateChanged(this, System.EventArgs.Empty);
                }

                this.Update(time);

            }

            return false;

        }

        public void Render()
        {
            if (activeScene != null && activeScene.Active)
            {
				
                renderManager.Render(activeScene);
            }
            else
            {
                Console.WriteLine("Beep");
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal class SceneRenderManager
    {
        ICor Cor { get; set; }

        internal SceneRenderManager(ICor cor)
        {
            this.Cor = cor;
        }

        internal void Render(Scene scene)
        {
            // Clear the background colour if the scene settings want us to.
			if (scene.Configuration.BackgroundColour.HasValue)
            {
				this.Cor.Graphics.ClearColourBuffer(scene.Configuration.BackgroundColour.Value);
            }
			
            foreach (var renderPass in scene.Configuration.RenderPasses)
            {
                this.RenderPass(scene, renderPass);
            }
        }

        List<MeshRenderer> list = new List<MeshRenderer>();
        List<MeshRenderer> GetMeshRenderersWithMaterials(Scene scene, string pass)
        {
            list.Clear ();
			foreach (var go in scene.SceneGraph.GetAllObjects())
            {
                if (!go.Enabled) continue;
                
                var mr = go.GetTrait<MeshRenderer>();

                if (mr == null)
                {
                    continue;
                }

                if (mr.Material == null)
                {
                    continue;
                }

                // if the material is for this pass
                if (mr.Material.RenderPass == pass)
                {
                    list.Add(mr);
                }
            }

            return list;
        }

		void RenderPass(Scene scene, RenderPass pass)
        {
            // init pass

            var gfxManager = this.Cor.Graphics;

			if (pass.Configuration.ClearDepthBuffer)
            {
                gfxManager.ClearDepthBuffer ();
            }

            var cam = scene.CameraManager.GetActiveCamera(pass.Name);

            var meshRenderers = this.GetMeshRenderersWithMaterials (scene, pass.Name);
            
            // filter this to get only renderers in the camera's frustum
            
            // first group by material
            var materialIDs = meshRenderers.Select (x => x.Material.ID).Distinct ().ToList ();
            
            // next build two groups, those supporting alpha and those that are opaque
            
            // for all the opaque objects groups render them from front to back to maximise gpu z-clipping
            
            // for all transparent object render them from back to front
            
            foreach (var materialID in materialIDs)
            {
                foreach (var mr in meshRenderers)
                {
                    if (mr.Material.ID == materialID)
                        _renderMeshRenderer (gfxManager, pass.Name, cam.ViewMatrix44, cam.ProjectionMatrix44, mr);
                }
            }

            scene.Blimey.DebugShapeRenderer.Render(gfxManager, pass.Name, cam.ViewMatrix44, cam.ProjectionMatrix44);
        }
        
        static void _renderMeshRenderer (IGraphicsManager zGfx, string renderPass, Matrix44 zView, Matrix44 zProjection, MeshRenderer mr)
        {
            if (!mr.Active)
                return;
            
            if (mr.Material.RenderPass != renderPass )
                return;
            
            zGfx.GpuUtils.BeginEvent(Rgba32.Red, "MeshRenderer.Render");

            using (new ProfilingTimer(t => FrameStats.SetCullModeTime += t))
            {
                zGfx.SetCullMode(mr.CullMode);
            }

            using (new ProfilingTimer(t => FrameStats.ActivateGeomBufferTime += t))
            {
                // Set our vertex declaration, vertex buffer, and index buffer.
                zGfx.SetActiveGeometryBuffer(mr.Mesh.GeomBuffer);
            }

            using (new ProfilingTimer(t => FrameStats.MaterialTime += t))
            {
                mr.Material.UpdateGpuSettings (zGfx);

                // The lighing manager right now just grabs the shader and tries to set
                // all variables to do with lighting, without even knowing if the shader
                // supports lighting.
                mr.Material.SetColour( "AmbientLightColour", LightingManager.ambientLightColour );
                mr.Material.SetColour( "EmissiveColour", LightingManager.emissiveColour );
                mr.Material.SetColour( "SpecularColour", LightingManager.specularColour );
                mr.Material.SetFloat( "SpecularPower", LightingManager.specularPower );

                mr.Material.SetFloat( "FogEnabled", LightingManager.fogEnabled ? 1f : 0f );
                mr.Material.SetFloat( "FogStart", LightingManager.fogStart );
                mr.Material.SetFloat( "FogEnd", LightingManager.fogEnd );
                mr.Material.SetColour( "FogColour", LightingManager.fogColour );

                mr.Material.SetVector3( "DirectionalLight0Direction", LightingManager.dirLight0Direction );
                mr.Material.SetColour( "DirectionalLight0DiffuseColour", LightingManager.dirLight0DiffuseColour );
                mr.Material.SetColour( "DirectionalLight0SpecularColour", LightingManager.dirLight0SpecularColour );

                mr.Material.SetVector3( "DirectionalLight1Direction", LightingManager.dirLight1Direction );
                mr.Material.SetColour( "DirectionalLight1DiffuseColour", LightingManager.dirLight1DiffuseColour );
                mr.Material.SetColour( "DirectionalLight1SpecularColour", LightingManager.dirLight1SpecularColour );

                mr.Material.SetVector3( "DirectionalLight2Direction", LightingManager.dirLight2Direction );
                mr.Material.SetColour( "DirectionalLight2DiffuseColour", LightingManager.dirLight2DiffuseColour );
                mr.Material.SetColour( "DirectionalLight2SpecularColour", LightingManager.dirLight2SpecularColour );

                mr.Material.SetVector3( "EyePosition", zView.Translation );

                // Get the material's shader and apply all of the settings
                // it needs.
                mr.Material.UpdateShaderVariables (
                    mr.Parent.Transform.Location,
                    zView,
                    zProjection
                    );
            }

            var shader = mr.Material.GetShader ();

            if( shader != null)
            {
                foreach (var effectPass in shader.Passes)
                {
                    using (new ProfilingTimer(t => FrameStats.ActivateShaderTime += t))
                    {
                        effectPass.Activate (mr.Mesh.GeomBuffer.VertexBuffer.VertexDeclaration);
                    }
                    using (new ProfilingTimer(t => FrameStats.DrawTime += t))
                    {
                        FrameStats.DrawIndexedPrimitivesCount ++;
                        zGfx.DrawIndexedPrimitives (
                            PrimitiveType.TriangleList, 0, 0,
                            mr.Mesh.VertexCount, 0, mr.Mesh.TriangleCount);
                    }
                }
            }

            zGfx.GpuUtils.EndEvent();
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

	public class CameraManager
    {
		public Entity GetRenderPassCamera (String renderPass)
		{
			return GetActiveCamera(renderPass).Parent;
		}
		internal Camera GetActiveCamera(String RenderPass)
        {
            return _activeCameras[RenderPass].GetTrait<Camera> ();
        }

        Dictionary<String, Entity> _defaultCameras = new Dictionary<String,Entity>();
        Dictionary<String, Entity> _activeCameras = new Dictionary<String,Entity>();

        internal void SetDefaultCamera(String RenderPass)
        {
            _activeCameras[RenderPass] = _defaultCameras[RenderPass];
        }

        internal void SetMainCamera (String RenderPass, Entity go)
        {
            _activeCameras[RenderPass] = go;
        }

        internal CameraManager (Scene scene)
        {
			var settings = scene.Configuration;

			foreach (var renderPass in settings.RenderPasses)
            {
				var go = scene.SceneGraph.CreateSceneObject("RenderPass(" + renderPass + ") Provided Camera");

                var cam = go.AddTrait<Camera>();

                if (renderPass.Configuration.CameraProjectionType == CameraProjectionType.Perspective)
                {
                    go.Transform.Position = new Vector3(2, 1, 5);

                    var orbit = go.AddTrait<OrbitAroundSubject>();
                    orbit.CameraSubject = Transform.Origin;

                    var lookAtSub = go.AddTrait<LookAtSubject>();
                    lookAtSub.Subject = Transform.Origin;
                }
                else
                {
                    cam.Projection = CameraProjectionType.Orthographic;

                    go.Transform.Position = new Vector3(0, 0, 0.5f);
                    go.Transform.LookAt(Vector3.Zero);
                }


				_defaultCameras.Add(renderPass.Name, go);
                _activeCameras.Add(renderPass.Name, go);
            }
        }
    }
}
