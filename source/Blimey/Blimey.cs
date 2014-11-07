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
    using System.Collections.Generic;
    using System.Diagnostics;
    
    using Fudge;
    using Abacus.SinglePrecision;
    
    using System.Linq;
    using Cor;
    using Cor.Platform;
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
		public virtual void Render(Engine cor)
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
		public virtual void Stop (Engine cor)
		{
		}
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Blimey
    {
        internal Blimey (Engine engine)
        {
            this.Assets = new Assets (engine);
            this.InputEventSystem = new InputEventSystem (engine);
            this.DebugRenderer = new DebugRenderer (engine);
            this.PrimitiveRenderer = new PrimitiveRenderer (engine);

        }

        public Assets Assets { get; private set; }

        public InputEventSystem InputEventSystem { get; private set; }

        public DebugRenderer DebugRenderer { get; private set; }

        public PrimitiveRenderer PrimitiveRenderer { get; private set; }

        internal void PreUpdate (AppTime time)
        {
            this.DebugRenderer.Update(time);
            this.InputEventSystem.Update(time);
        }

        internal void PostUpdate(AppTime time)
        {
            this.PrimitiveRenderer.PostUpdate (time);
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
        public VertexBuffer VertexBuffer;

        /// <summary>
        /// todo
        /// </summary>
        public IndexBuffer IndexBuffer;
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
		
		Engine cor = null;
		Blimey blimey = null;
		readonly SceneConfiguration configuration = null;
		readonly SceneRuntimeConfiguration runtimeConfiguration = null;
		CameraManager cameraManager = null;
		SceneSceneGraph sceneGraph = null;
		Boolean isRunning = false;

		
		// ======================= //
		// Functions for consumers //
		// ======================= //
        public Engine Cor { get { return cor; } }
		public Blimey Blimey { get { return blimey; } }
		
		public Boolean Active { get { return isRunning;} }
		public SceneConfiguration Configuration { get { return configuration; } }
		public SceneRuntimeConfiguration RuntimeConfiguration { get { return runtimeConfiguration; } }
		public SceneSceneGraph SceneGraph { get { return sceneGraph; } }
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
		
        internal void Initialize (Engine cor, Blimey blimey)
        {
			this.cor = cor;
            this.blimey = blimey;
            //this.blimey.SetSceneConfig (this.configuration);
			this.sceneGraph = new SceneSceneGraph (this);
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

            this.blimey.PostUpdate (time);

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
        protected Engine Cor { get; set; }
        protected Blimey Blimey { get; set; }

        public Entity Parent { get; set; }

        public Boolean Active { get; set; }

        // INTERNAL METHODS
        // called after constructor and before awake
		internal void Initilise (Engine cor, Blimey blimeyServices, Entity parent)
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
        Engine cor;
        Blimey blimey;

        SceneRenderer renderManager;

        public event System.EventHandler SimulationStateChanged;

        public Scene ActiveState { get { return activeScene; } }

        public SceneManager (Engine cor, Blimey blimey, Scene startScene)
        {
            this.cor = cor;
            this.blimey = blimey;
            activeScene = startScene;
            activeScene.Initialize(cor, blimey);
            renderManager = new SceneRenderer(cor);

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

                activeScene.Initialize (cor, blimey);

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

	public class CameraManager
    {
		public Entity GetRenderPassCamera (String renderPass)
		{
			return GetActiveCamera(renderPass).Parent;
		}
		internal CameraTrait GetActiveCamera(String RenderPass)
        {
            return _activeCameras[RenderPass].GetTrait<CameraTrait> ();
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

                var cam = go.AddTrait<CameraTrait>();

                if (renderPass.Configuration.CameraProjectionType == CameraProjectionType.Perspective)
                {
                    go.Transform.Position = new Vector3(2, 1, 5);

                    var orbit = go.AddTrait<OrbitAroundSubjectTrait>();
                    orbit.CameraSubject = Transform.Origin;

                    var lookAtSub = go.AddTrait<LookAtSubjectTrait>();
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


    #region Vertex Types

    // Definitions for common vertex types.
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPosition
        : IVertexType
    {
        /// <summary>
        ///
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///
        /// </summary>
        public VertexPosition (Vector3 position)
        {
            this.Position = position;
        }

        /// <summary>
        ///
        /// </summary>
        static VertexPosition ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0)
            );

            _default = new VertexPosition (Vector3.Zero);
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPosition _default;

        /// <summary>
        ///
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        ///
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPositionColour
        : IVertexType
    {
        /// <summary>
        ///
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///
        /// </summary>
        public Rgba32 Colour;

        /// <summary>
        ///
        /// </summary>
        public VertexPositionColour (
            Vector3 position,
            Rgba32 color)
        {
            this.Position = position;
            this.Colour = color;
        }

        /// <summary>
        ///
        /// </summary>
        static VertexPositionColour ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Colour,
                    VertexElementUsage.Colour,
                    0)
            );

            _default = new VertexPositionColour (
                Vector3.Zero,
                Rgba32.Magenta);
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPositionColour _default;

        /// <summary>
        ///
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        ///
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPositionNormal
        : IVertexType
    {
        /// <summary>
        ///
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        ///
        /// </summary>
        public VertexPositionNormal (
            Vector3 position,
            Vector3 normal)
        {
            this.Position = position;
            this.Normal = normal;
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPositionNormal _default;

        /// <summary>
        ///
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        ///
        /// </summary>
        static VertexPositionNormal ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    sizeof (Single) * 3,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Normal,
                    0)
            );

            _default = new VertexPositionNormal (
                Vector3.Zero,
                Vector3.Zero);
        }

        /// <summary>
        ///
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPositionNormalColour
        : IVertexType
    {
        /// <summary>
        ///
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        ///
        /// </summary>
        public Rgba32 Colour;

        /// <summary>
        ///
        /// </summary>
        public VertexPositionNormalColour (
            Vector3 position,
            Vector3 normal,
            Rgba32 color)
        {
            this.Position = position;
            this.Normal = normal;
            this.Colour = color;
        }

        /// <summary>
        ///
        /// </summary>
        static VertexPositionNormalColour ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Normal,
                    0),
                new VertexElement (
                    24,
                    VertexElementFormat.Colour,
                    VertexElementUsage.Colour,
                    0)
            );

            _default = new VertexPositionNormalColour (
                Vector3.Zero,
                Vector3.Zero,
                Rgba32.White);
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPositionNormalColour _default;

        /// <summary>
        ///
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        ///
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPositionNormalTexture
        : IVertexType
    {
        /// <summary>
        ///
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        ///
        /// </summary>
        public Vector2 UV;

        /// <summary>
        ///
        /// </summary>
        public VertexPositionNormalTexture (
            Vector3 position,
            Vector3 normal,
            Vector2 uv)
        {
            this.Position = position;
            this.Normal = normal;
            this.UV = uv;
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPositionNormalTexture _default;

        /// <summary>
        ///
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        ///
        /// </summary>
        static VertexPositionNormalTexture ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Normal,
                    0),
                new VertexElement (
                    24,
                    VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate,
                    0)
            );

            _default = new VertexPositionNormalTexture (
                Vector3.Zero,
                Vector3.Zero,
                Vector2.Zero);
        }

        /// <summary>
        ///
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPositionNormalTextureColour
        : IVertexType
    {
        /// <summary>
        ///
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        ///
        /// </summary>
        public Vector2 UV;

        /// <summary>
        ///
        /// </summary>
        public Rgba32 Colour;

        /// <summary>
        ///
        /// </summary>
        public VertexPositionNormalTextureColour (
            Vector3 position,
            Vector3 normal,
            Vector2 uv,
            Rgba32 color)
        {
            this.Position = position;
            this.Normal = normal;
            this.UV = uv;
            this.Colour = color;
        }

        /// <summary>
        ///
        /// </summary>
        static VertexPositionNormalTextureColour ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Normal,
                    0),
                new VertexElement (
                    24,
                    VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate,
                    0),
                new VertexElement (
                    32,
                    VertexElementFormat.Colour,
                    VertexElementUsage.Colour,
                    0)
            );

            _default = new VertexPositionNormalTextureColour (
                Vector3.Zero,
                Vector3.Zero,
                Vector2.Zero,
                Rgba32.White);
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPositionNormalTextureColour _default;

        /// <summary>
        ///
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        ///
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPositionTexture
        : IVertexType
    {
        /// <summary>
        ///
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///
        /// </summary>
        public Vector2 UV;

        /// <summary>
        ///
        /// </summary>
        public VertexPositionTexture (
            Vector3 position,
            Vector2 uv)
        {
            this.Position = position;
            this.UV = uv;
        }

        /// <summary>
        ///
        /// </summary>
        static VertexPositionTexture ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate,
                    0)
            );

            _default = new VertexPositionTexture (
                Vector3.Zero,
                Vector2.Zero);
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPositionTexture _default;

        /// <summary>
        ///
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        ///
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPositionTextureColour
        : IVertexType
    {
        /// <summary>
        ///
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///
        /// </summary>
        public Vector2 UV;

        /// <summary>
        ///
        /// </summary>
        public Rgba32 Colour;

        /// <summary>
        ///
        /// </summary>
        public VertexPositionTextureColour (
            Vector3 position,
            Vector2 uv,
            Rgba32 color)
        {
            this.Position = position;
            this.UV = uv;
            this.Colour = color;
        }

        /// <summary>
        ///
        /// </summary>
        static VertexPositionTextureColour ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate,
                    0),
                new VertexElement (
                    20,
                    VertexElementFormat.Colour,
                    VertexElementUsage.Colour,
                    0)
            );

            _default = new VertexPositionTextureColour (
                Vector3.Zero,
                Vector2.Zero,
                Rgba32.White);
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPositionTextureColour _default;

        /// <summary>
        ///
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        ///
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }
    }

    #endregion

    #region Serialisers

    // Oats serialisers for types used in Cor's Asset Pipeline.
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //


    /// <summary>
    /// An explict Oats.Serialiser for the Abacus.SinglePrecision.Matrix44 type.
    /// </summary>
    public class Matrix44Serialiser
        : Serialiser <Matrix44>
    {
        /// <summary>
        /// Returns an Abacus.SinglePrecision.Matrix44 object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override Matrix44 Read (ISerialisationChannel ss)
        {
            Single m11 = ss.Read <Single> ();
            Single m12 = ss.Read <Single> ();
            Single m13 = ss.Read <Single> ();
            Single m14 = ss.Read <Single> ();

            Single m21 = ss.Read <Single> ();
            Single m22 = ss.Read <Single> ();
            Single m23 = ss.Read <Single> ();
            Single m24 = ss.Read <Single> ();

            Single m31 = ss.Read <Single> ();
            Single m32 = ss.Read <Single> ();
            Single m33 = ss.Read <Single> ();
            Single m34 = ss.Read <Single> ();

            Single m41 = ss.Read <Single> ();
            Single m42 = ss.Read <Single> ();
            Single m43 = ss.Read <Single> ();
            Single m44 = ss.Read <Single> ();

            return new Matrix44(
                m11, m12, m13, m14,
                m21, m22, m23, m24,
                m31, m32, m33, m34,
                m41, m42, m43, m44
            );
        }

        /// <summary>
        /// Writes an Abacus.SinglePrecision.Matrix44 object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, Matrix44 obj)
        {
            ss.Write <Single> (obj.R0C0);
            ss.Write <Single> (obj.R0C1);
            ss.Write <Single> (obj.R0C2);
            ss.Write <Single> (obj.R0C3);

            ss.Write <Single> (obj.R1C0);
            ss.Write <Single> (obj.R1C1);
            ss.Write <Single> (obj.R1C2);
            ss.Write <Single> (obj.R1C3);

            ss.Write <Single> (obj.R2C0);
            ss.Write <Single> (obj.R2C1);
            ss.Write <Single> (obj.R2C2);
            ss.Write <Single> (obj.R2C3);

            ss.Write <Single> (obj.R3C0);
            ss.Write <Single> (obj.R3C1);
            ss.Write <Single> (obj.R3C2);
            ss.Write <Single> (obj.R3C3);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Abacus.SinglePrecision.Quaternion type.
    /// </summary>
    public class QuaternionSerialiser
        : Serialiser<Quaternion>
    {
        /// <summary>
        /// Returns an Abacus.SinglePrecision.Quaternion object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override Quaternion Read (ISerialisationChannel ss)
        {
            Single i = ss.Read <Single> ();
            Single j = ss.Read <Single> ();
            Single k = ss.Read <Single> ();
            Single u = ss.Read <Single> ();

            return new Quaternion (i, j, k, u);
        }

        /// <summary>
        /// Writes an Abacus.SinglePrecision.Quaternion object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, Quaternion obj)
        {
            ss.Write <Single> (obj.I);
            ss.Write <Single> (obj.J);
            ss.Write <Single> (obj.K);
            ss.Write <Single> (obj.U);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Abacus.Packed.Rgba32 type.
    /// </summary>
    public class Rgba32Serialiser
        : Serialiser<Rgba32>
    {
        /// <summary>
        /// Returns an Abacus.Packed.Rgba32 object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override Rgba32 Read (ISerialisationChannel ss)
        {
            Byte r = ss.Read <Byte> ();
            Byte g = ss.Read <Byte> ();
            Byte b = ss.Read <Byte> ();
            Byte a = ss.Read <Byte> ();

            return new Rgba32(r, g, b, a);
        }

        /// <summary>
        /// Writes an Abacus.Packed.Rgba32 object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, Rgba32 obj)
        {
            ss.Write <Byte> (obj.R);
            ss.Write <Byte> (obj.G);
            ss.Write <Byte> (obj.B);
            ss.Write <Byte> (obj.A);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Abacus.SinglePrecision.Vector2 type.
    /// </summary>
    public class Vector2Serialiser
        : Serialiser<Vector2>
    {
        /// <summary>
        /// Returns an Abacus.SinglePrecision.Vector2 object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override Vector2 Read (ISerialisationChannel ss)
        {
            Single x = ss.Read <Single> ();
            Single y = ss.Read <Single> ();

            return new Vector2(x, y);
        }

        /// <summary>
        /// Writes an Abacus.SinglePrecision.Vector2 object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, Vector2 obj)
        {
            ss.Write <Single> (obj.X);
            ss.Write <Single> (obj.Y);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Abacus.SinglePrecision.Vector3 type.
    /// </summary>
    public class Vector3Serialiser
        : Serialiser<Vector3>
    {
        /// <summary>
        /// Returns an Abacus.SinglePrecision.Vector3 object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override Vector3 Read (ISerialisationChannel ss)
        {
            Single x = ss.Read <Single> ();
            Single y = ss.Read <Single> ();
            Single z = ss.Read <Single> ();

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Writes an Abacus.SinglePrecision.Vector3 object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, Vector3 obj)
        {
            ss.Write <Single> (obj.X);
            ss.Write <Single> (obj.Y);
            ss.Write <Single> (obj.Z);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Abacus.SinglePrecision.Vector4 type.
    /// </summary>
    public class Vector4Serialiser
        : Serialiser<Vector4>
    {
        /// <summary>
        /// Returns an Abacus.SinglePrecision.Vector4 object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override Vector4 Read (ISerialisationChannel ss)
        {
            Single x = ss.Read <Single> ();
            Single y = ss.Read <Single> ();
            Single z = ss.Read <Single> ();
            Single w = ss.Read <Single> ();

            return new Vector4(x, y, z, w);
        }

        /// <summary>
        /// Writes an Abacus.SinglePrecision.Vector4 object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, Vector4 obj)
        {
            ss.Write <Single> (obj.X);
            ss.Write <Single> (obj.Y);
            ss.Write <Single> (obj.Z);
            ss.Write <Single> (obj.W);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.ColourmapAsset type.
    /// </summary>
    public class ColourmapAssetSerialiser
        : Serialiser<ColourmapAsset>
    {
        /// <summary>
        /// Returns a Cor.ColourmapAsset object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ColourmapAsset Read (ISerialisationChannel ss)
        {
            var asset = new ColourmapAsset ();

            Int32 width = ss.Read <Int32> ();
            Int32 height = ss.Read <Int32> ();

            asset.Data = new Rgba32[width, height];

            for (Int32 i = 0; i < width; ++i)
            {
                for (Int32 j = 0; j < height; ++j)
                {
                    asset.Data[i, j] =
                        ss.Read <Rgba32> ();
                }
            }

            return asset;
        }

        /// <summary>
        /// Writes a Cor.ColourmapAsset object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ColourmapAsset obj)
        {
            ss.Write <Int32> (obj.Width);
            ss.Write <Int32> (obj.Height);

            for (Int32 i = 0; i < obj.Width; ++i)
            {
                for (Int32 j = 0; j < obj.Height; ++j)
                {
                    ss.Write <Rgba32> (obj.Data[i, j]);
                }
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.ShaderAsset type.
    /// </summary>
    public class ShaderAssetSerialiser
        : Serialiser<ShaderAsset>
    {
        /// <summary>
        /// Returns a Cor.ShaderAsset object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderAsset Read (ISerialisationChannel ss)
        {
            var asset = new ShaderAsset ();

            asset.Declaration = ss.Read <ShaderDeclaration> ();
            asset.Format = ss.Read <ShaderFormat> ();
            asset.Source = ss.Read <Byte[]> ();

            return asset;
        }

        /// <summary>
        /// Writes a Cor.ShaderAsset object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderAsset obj)
        {
            ss.Write <ShaderDeclaration> (obj.Declaration);
            ss.Write <ShaderFormat> (obj.Format);
            ss.Write <Byte[]> (obj.Source);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.TextAsset type.
    /// </summary>
    public class TextAssetSerialiser
        : Serialiser<TextAsset>
    {
        /// <summary>
        /// Returns a Cor.TextAsset object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override TextAsset Read (ISerialisationChannel ss)
        {
            var asset = new TextAsset ();

            asset.Text = ss.Read <String> ();

            return asset;
        }

        /// <summary>
        /// Writes a Cor.TextAsset object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, TextAsset obj)
        {
            ss.Write <String> (obj.Text);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.TextureAsset type.
    /// </summary>
    public class TextureAssetSerialiser
        : Serialiser<TextureAsset>
    {
        /// <summary>
        /// Returns a Cor.TextureAsset object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override TextureAsset Read (ISerialisationChannel ss)
        {
            var asset = new TextureAsset ();

            asset.TextureFormat = ss.Read <TextureFormat> ();
            asset.Width = ss.Read <Int32> ();
            asset.Height = ss.Read <Int32> ();
            Int32 byteCount = ss.Read <Int32> ();

            asset.Data = new Byte[byteCount];

            for (Int32 i = 0; i < byteCount; ++i)
            {
                asset.Data[i] = ss.Read <Byte> ();
            }

            return asset;
        }

        /// <summary>
        /// Writes a Cor.TextureAsset object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, TextureAsset obj)
        {
            ss.Write <TextureFormat> (obj.TextureFormat);
            ss.Write <Int32> (obj.Width);
            ss.Write <Int32> (obj.Height);
            ss.Write <Int32> (obj.Data.Length);

            for (Int32 i = 0; i < obj.Data.Length; ++i)
            {
                ss.Write <Byte> (obj.Data[i]);
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.ShaderDefinition type.
    /// </summary>
    public class ShaderDeclarationSerialiser
        : Serialiser<ShaderDeclaration>
    {
        /// <summary>
        /// Returns a Cor.ShaderDefinition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderDeclaration Read (ISerialisationChannel ss)
        {
            var sd = new ShaderDeclaration ();

            sd.Name =                   ss.Read <String> ();
            sd.InputDeclarations =      new List <ShaderInputDeclaration> ();
            sd.SamplerDeclarations =    new List <ShaderSamplerDeclaration> ();
            sd.VariableDeclarations =   new List <ShaderVariableDeclaration> ();

            Int32 numInputDefintions = (Int32) ss.Read <Byte> ();
            Int32 numSamplerDefinitions = (Int32) ss.Read <Byte> ();
            Int32 numVariableDefinitions = (Int32) ss.Read <Byte> ();

            for (Int32 i = 0; i < numInputDefintions; ++i)
            {
                var inputDef = ss.Read <ShaderInputDeclaration> ();
                sd.InputDeclarations.Add (inputDef);
            }

            for (Int32 i = 0; i < numSamplerDefinitions; ++i)
            {
                var samplerDef = ss.Read <ShaderSamplerDeclaration> ();
                sd.SamplerDeclarations.Add (samplerDef);
            }

            for (Int32 i = 0; i < numVariableDefinitions; ++i)
            {
                var variableDef = ss.Read <ShaderVariableDeclaration> ();
                sd.VariableDeclarations.Add (variableDef);
            }

            return sd;
        }

        /// <summary>
        /// Writes a Cor.ShaderDefinition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderDeclaration sd)
        {
            if (sd.InputDeclarations.Count > Byte.MaxValue ||
                sd.SamplerDeclarations.Count > Byte.MaxValue ||
                sd.VariableDeclarations.Count > Byte.MaxValue)
            {
                throw new SerialisationException ("Too much!");
            }

            ss.Write <String> (sd.Name);

            ss.Write <Byte> ((Byte) sd.InputDeclarations.Count);
            ss.Write <Byte> ((Byte) sd.SamplerDeclarations.Count);
            ss.Write <Byte> ((Byte) sd.VariableDeclarations.Count);

            foreach (var inputDef in sd.InputDeclarations)
            {
                ss.Write <ShaderInputDeclaration> (inputDef);
            }

            foreach (var samplerDef in sd.SamplerDeclarations)
            {
                ss.Write <ShaderSamplerDeclaration> (samplerDef);
            }

            foreach (var variableDef in sd.VariableDeclarations)
            {
                ss.Write <ShaderVariableDeclaration> (variableDef);
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.ShaderInputDefinition type.
    /// </summary>
    public class ShaderInputDeclarationSerialiser
        : Serialiser<ShaderInputDeclaration>
    {
        /// <summary>
        /// Returns a Cor.ShaderInputDefinition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderInputDeclaration Read (ISerialisationChannel ss)
        {
            var sid = new ShaderInputDeclaration ();

            // Name
            sid.Name = ss.Read <String> ();

            // Nice Name
            sid.NiceName = ss.Read <String> ();

            // Optional
            sid.Optional = ss.Read <Boolean> ();

            // Usage
            sid.Usage = ss.Read <VertexElementUsage> ();

            // Null
            if (ss.Read <Boolean> ())
            {
                // Default Value
                Byte typeIndex = ss.Read <Byte> ();
                Type defaultValueType = ShaderInputDeclaration.SupportedTypes [typeIndex];
                sid.DefaultValue = ss.ReadReflective (defaultValueType);
            }

            return sid;
        }

        /// <summary>
        /// Writes a Cor.ShaderInputDefinition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderInputDeclaration sid)
        {
            // Name
            ss.Write <String> (sid.Name);

            // Nice Name
            ss.Write <String> (sid.NiceName);

            // Optional
            ss.Write <Boolean> (sid.Optional);

            // Usage
            ss.Write <VertexElementUsage> (sid.Usage);

            // Null
            ss.Write <Boolean> (sid.DefaultValue != null);

            // Default Value
            if (sid.DefaultValue != null)
            {
                Type defaultValueType = sid.DefaultValue.GetType ();
                Byte typeIndex = (Byte)
                    ShaderInputDeclaration.SupportedTypes
                    .ToList ()
                    .IndexOf (defaultValueType);

                ss.Write<Byte> (typeIndex);
                ss.WriteReflective (defaultValueType, sid.DefaultValue);
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.ShaderSamplerDefinition type.
    /// </summary>
    public class ShaderSamplerDeclarationSerialiser
        : Serialiser<ShaderSamplerDeclaration>
    {
        /// <summary>
        /// Returns a Cor.ShaderSamplerDefinition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderSamplerDeclaration Read (ISerialisationChannel ss)
        {
            var ssd = new ShaderSamplerDeclaration ();

            ssd.Name =           ss.Read <String> ();
            ssd.NiceName =       ss.Read <String> ();
            ssd.Optional =       ss.Read <Boolean> ();

            return ssd;
        }

        /// <summary>
        /// Writes a Cor.ShaderSamplerDefinition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderSamplerDeclaration ssd)
        {
            ss.Write <String> (ssd.Name);
            ss.Write <String> (ssd.NiceName);
            ss.Write <Boolean> (ssd.Optional);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.ShaderVariableDefinition type.
    /// </summary>
    public class ShaderVariableDefinitionSerialiser
        : Serialiser<ShaderVariableDeclaration>
    {
        /// <summary>
        /// Returns a Cor.ShaderVariableDefinition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderVariableDeclaration Read (ISerialisationChannel ss)
        {
            var svd = new ShaderVariableDeclaration ();

            // Name
            svd.Name = ss.Read <String> ();

            // Nice Name
            svd.NiceName = ss.Read <String> ();

            // Null
            if (ss.Read <Boolean> ())
            {
                Byte typeIndex = ss.Read <Byte> ();
                Type defaultValueType = ShaderVariableDeclaration.SupportedTypes [typeIndex];
                svd.DefaultValue = ss.ReadReflective (defaultValueType);
            }

            return svd;
        }

        /// <summary>
        /// Writes a Cor.ShaderVariableDefinition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderVariableDeclaration svd)
        {
            // Name
            ss.Write <String> (svd.Name);

            // Nice Name
            ss.Write <String> (svd.NiceName);

            // Null
            ss.Write <Boolean> (svd.DefaultValue != null);

            // Default Value
            if (svd.DefaultValue != null)
            {
                Type defaultValueType = svd.DefaultValue.GetType ();
                Byte typeIndex = (Byte)
                    ShaderVariableDeclaration.SupportedTypes
                    .ToList ()
                    .IndexOf (defaultValueType);

                ss.Write<Byte> (typeIndex);
                ss.WriteReflective (defaultValueType, svd.DefaultValue);
            }
        }
    }

    #endregion
}
