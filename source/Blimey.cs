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
    using Abacus.Packed;
    using Abacus.SinglePrecision;
    using Abacus.Int32Precision;
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
    public sealed class Blimey
        : IApp
    {
        Scene startScene;
        SceneManager sceneManager;

        FpsHelper fps;
        FrameBufferHelper frameBuffer;

		/// <summary>
		/// Initializes a new instance of the <see cref="Blimey.Blimey"/> class.
		/// </summary>
        public Blimey(Scene startScene)
        {
            this.startScene = startScene;
        }

		/// <summary>
		/// Blimey's initilisation rountine.
		/// </summary>
		public void Start(ICor cor)
        {
            fps = new FpsHelper();
            frameBuffer = new FrameBufferHelper(cor.Graphics);
            this.sceneManager = new SceneManager(cor, startScene);
        }

		/// <summary>
		/// Blimey's root update loop.
		/// </summary>
		public Boolean Update(ICor cor, AppTime time)
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
		public void Render(ICor cor)
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
		public void Stop (ICor cor)
		{
		}
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Services
    {
        ICor cor;

        internal Services (ICor cor, SceneSettings settings)
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

    public enum CameraProjectionType
    {
        Perspective,
        Orthographic,
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

    public class Material
    {
        IShader shader;
        string renderPass;

        public BlendMode BlendMode { get; set; }
        public string RenderPass { get { return renderPass; } }

        public Material(string renderPass, IShader shader)
        {
            this.BlendMode = BlendMode.Default;

            this.renderPass = renderPass;
            this.shader = shader;
        }

        internal void UpdateShaderVariables(Matrix44 world, Matrix44 view, Matrix44 proj)
        {
            if(shader == null)
                return;

            // Right now we need to make sure that the shader variables are all set with this
            // settings this material has defined.

            // We don't know if the shader being used is exclusive to this material, or if it
            // is shared between many.

            // Therefore to be 100% sure we could reset every variable on the shader to the defaults,
            // then set the ones that this material knows about, thus avoiding running with shader settings
            // that this material doesn't know about that are being changed by something else that
            // shares the shader.  This would be bad, as it will likely involve setting the same variable multiple times

            // So instead, as an optimisation, iterate over all settings that this material knows about,
            // and ask the shader to change them, this compare those changes against a full list of
            // all of the shader's variables, if any were missed by the material, then set them to
            // their default values.

            // Right now, just use the easy option and optimise later ;-D

            shader.ResetVariables();

            shader.SetVariable ("World", world);
            shader.SetVariable ("View", view);
            shader.SetVariable ("Projection", proj);

            foreach(var propertyName in colourSettings.Keys)
            {
                shader.SetVariable (propertyName, colourSettings[propertyName]);
            }

            foreach(var propertyName in floatSettings.Keys)
            {
                shader.SetVariable (propertyName, floatSettings[propertyName]);
            }

            foreach(var propertyName in matrixSettings.Keys)
            {
                shader.SetVariable (propertyName, matrixSettings[propertyName]);
            }

            foreach(var propertyName in vector3Settings.Keys)
            {
                shader.SetVariable (propertyName, vector3Settings[propertyName]);
            }

            foreach(var propertyName in vector4Settings.Keys)
            {
                shader.SetVariable (propertyName, vector4Settings[propertyName]);
            }

            foreach(var propertyName in scaleSettings.Keys)
            {
                shader.SetVariable (propertyName, scaleSettings[propertyName]);
            }

            foreach(var propertyName in textureOffsetSettings.Keys)
            {
                shader.SetVariable (propertyName, textureOffsetSettings[propertyName]);
            }

            shader.ResetSamplerTargets();

            int i = 0;
            foreach(var key in textureSamplerSettings.Keys)
            {
                shader.SetSamplerTarget (key, i);
                i++;
            }
        }

        internal IShader GetShader()
        {
            return shader;
        }

		public Vector2 Tiling
		{ 
			get { throw new NotImplementedException (); }
			set { throw new NotImplementedException (); }
		}
        
		public Vector2 Offset
		{ 
			get { throw new NotImplementedException (); }
			set { throw new NotImplementedException (); }
		}

        internal void UpdateGpuSettings(IGraphicsManager graphics)
        {
            // Update the render states on the gpu
            BlendMode.Apply (graphics, this.BlendMode);

            graphics.SetActiveTexture (0, null);

            // Set the active textures on the gpu
            int i = 0;
            foreach(var key in textureSamplerSettings.Keys)
            {
                graphics.SetActiveTexture (i, textureSamplerSettings[key]);
                i++;
            }
        }

        Dictionary<string, Rgba32> colourSettings = new Dictionary<string, Rgba32>();
        Dictionary<string, Single> floatSettings = new Dictionary<string, Single>();
        Dictionary<string, Matrix44> matrixSettings = new Dictionary<string, Matrix44>();
        Dictionary<string, Vector3> vector3Settings = new Dictionary<string, Vector3>();
        Dictionary<string, Vector4> vector4Settings = new Dictionary<string, Vector4>();
        Dictionary<string, Vector2> scaleSettings = new Dictionary<string, Vector2>();
        Dictionary<string, Vector2> textureOffsetSettings = new Dictionary<string, Vector2>();
        Dictionary<string, ITexture> textureSamplerSettings = new Dictionary<string, ITexture>();

        public void SetColour(string propertyName, Rgba32 colour)
        {
            colourSettings[propertyName] = colour;
        }

        public void SetFloat(string propertyName, Single value)
        {
            floatSettings[propertyName] = value;
        }

        public void SetMatrix(string propertyName, Matrix44 matrix)
        {
            matrixSettings[propertyName] = matrix;
        }

        public void SetVector4(string propertyName, Vector4 vector)
        {
            vector4Settings[propertyName] = vector;
        }

        public void SetVector3(string propertyName, Vector3 vector)
        {
            vector3Settings[propertyName] = vector;
        }

        public void SetTextureOffset(string propertyName, Vector2 offset)
        {
            textureOffsetSettings[propertyName] = offset;
        }

        public void SetTextureScale(string propertyName, Vector2 scale)
        {
            scaleSettings[propertyName] = scale;
        }


        public void SetTexture(string propertyName, ITexture texture)
        {
            textureSamplerSettings[propertyName] = texture;
        }
         }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Game scene settings are used by the engine to detemine how
    /// to manage an associated scene.  For example the game scene
    /// settings are used to define the render pass for the scene.
	/// </summary>
    public class SceneSettings
    {
        Dictionary<String, RenderPassSettings> renderPassSettings;
        List<String> renderPassOrder;
        Boolean startByClearingBackBuffer;
        Rgba32 clearBackBufferColour;

        readonly static SceneSettings defaultSettings = new SceneSettings();

        internal static SceneSettings Default
        {
            get
            {
                return defaultSettings;
            }
        }

        static SceneSettings()
        {
            defaultSettings.InitDefault();
        }

        public SceneSettings()
        {
            renderPassSettings = new Dictionary<String, RenderPassSettings>();
            renderPassOrder = new List<String>();
            startByClearingBackBuffer = false;
            clearBackBufferColour = Rgba32.CornflowerBlue;
        }

        void InitDefault()
        {
            /// BIG TODO, ADD AWAY TO MAKE RENDER PASSES SHARE THE SAME CAMERA!!!!!
			/// SO DEBUG AND DEFAULT CAN USER THE SAME
			var debugPassSettings = RenderPassSettings.Default;
			debugPassSettings.ClearDepthBuffer = true;
			AddRenderPass("Debug", debugPassSettings);

            var defaultPassSettings = RenderPassSettings.Default;
            defaultPassSettings.EnableDefaultLighting = true;
            defaultPassSettings.FogEnabled = true;
            AddRenderPass("Default", defaultPassSettings);

            var guiPassSettings = RenderPassSettings.Default;
            guiPassSettings.ClearDepthBuffer = true;
            guiPassSettings.CameraProjectionType = CameraProjectionType.Orthographic;
            AddRenderPass("Gui", guiPassSettings);
        }

        public void AddRenderPass(String passID, RenderPassSettings settings)
        {
            if (renderPassOrder.Contains(passID))
            {
                throw new Exception("Can't have render passes with the same name");
            }

            renderPassOrder.Add(passID);
            renderPassSettings.Add(passID, settings);
        }

        public RenderPassSettings GetRenderPassSettings(String passName)
        {
            return renderPassSettings[passName];
        }

        public List<String> RenderPasses
        {
            get
            {
                return renderPassOrder;
            }
        }

        // Debug Background Rgba
        public bool StartByClearingBackBuffer
        {
            get
            {
                return startByClearingBackBuffer;
            }
            set
            {
                startByClearingBackBuffer = value;
            }
        }

        public Rgba32 BackgroundColour
        {
            get
            {
                return clearBackBufferColour;
            }
            set
            {
                startByClearingBackBuffer = true;
                clearBackBufferColour = value;
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public enum Space
    {
        World,
        Self
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    // high level wrapper for blending stuff
    public struct BlendMode
        : IEquatable<BlendMode>
    {
        BlendFunction rgbBlendFunction;
        BlendFactor sourceRgb;
        BlendFactor destinationRgb;

        BlendFunction alphaBlendFunction;
        BlendFactor sourceAlpha;
        BlendFactor destinationAlpha;

        public override String ToString ()
        {
            return string.Format (
                "{{rgbBlendFunction:{0} sourceRgb:{1} destinationRgb:{2} alphaBlendFunction:{3} sourceAlpha:{4} destinationAlpha:{5}}}"
                , new Object[]
                    {
                        rgbBlendFunction.ToString (), sourceRgb.ToString (), destinationRgb.ToString (),
                        alphaBlendFunction.ToString (), sourceAlpha.ToString (), destinationAlpha.ToString ()
                    }
            );
        }

        public Boolean Equals (BlendMode other)
        {
            return this == other;
        }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;
            if (obj is BlendMode) {
                flag = this.Equals ((BlendMode)obj);
            }
            return flag;
        }

        public override Int32 GetHashCode ()
        {
            int a = (int) rgbBlendFunction.GetHashCode();
            int b = (int) sourceRgb.GetHashCode();
            int c = (int) destinationRgb.GetHashCode();

            int d = (int) alphaBlendFunction.GetHashCode();
            int e = (int) sourceAlpha.GetHashCode();
            int f = (int) destinationAlpha.GetHashCode();


            return a + b + c + d + e + f;
        }

        public static Boolean operator != (BlendMode value1, BlendMode value2)
        {
            return !(value1 == value2);
        }

        public static Boolean operator == (BlendMode value1, BlendMode value2)
        {
            if (value1.rgbBlendFunction != value2.rgbBlendFunction) return false;
            if (value1.sourceRgb != value2.sourceRgb) return false;
            if (value1.destinationRgb != value2.destinationRgb) return false;
            if (value1.alphaBlendFunction != value2.alphaBlendFunction) return false;
            if (value1.sourceAlpha != value2.sourceAlpha) return false;
            if (value1.destinationAlpha != value2.destinationAlpha) return false;

            return true;
        }

        public static BlendMode Default
        {
            get
            {
                var blendMode = new BlendMode();

                blendMode.rgbBlendFunction =    BlendFunction.Add;
                blendMode.sourceRgb =           BlendFactor.SourceAlpha;
                blendMode.destinationRgb =      BlendFactor.InverseSourceAlpha;

                blendMode.alphaBlendFunction =  BlendFunction.Add;
                blendMode.sourceAlpha =         BlendFactor.One;
                blendMode.destinationAlpha =    BlendFactor.InverseSourceAlpha;

                return blendMode;
            }
        }

        public static BlendMode Opaque
        {
            get
            {
                var blendMode = new BlendMode();

                blendMode.rgbBlendFunction =    BlendFunction.Add;
                blendMode.sourceRgb =           BlendFactor.One;
                blendMode.destinationRgb =      BlendFactor.Zero;

                blendMode.alphaBlendFunction =  BlendFunction.Add;
                blendMode.sourceAlpha =         BlendFactor.One;
                blendMode.destinationAlpha =    BlendFactor.Zero;

                return blendMode;
            }
        }

        public static BlendMode Subtract
        {
            get
            {
                var blendMode = new BlendMode();

                blendMode.rgbBlendFunction =    BlendFunction.ReverseSubtract;
                blendMode.sourceRgb =           BlendFactor.SourceAlpha;
                blendMode.destinationRgb =      BlendFactor.One;

                blendMode.alphaBlendFunction =  BlendFunction.ReverseSubtract;
                blendMode.sourceAlpha =         BlendFactor.SourceAlpha;
                blendMode.destinationAlpha =    BlendFactor.One;

                return blendMode;
            }
        }

        public static BlendMode Additive
        {
            get
            {
                var blendMode = new BlendMode();

                blendMode.rgbBlendFunction =    BlendFunction.Add;
                blendMode.sourceRgb =           BlendFactor.SourceAlpha;
                blendMode.destinationRgb =      BlendFactor.One;

                blendMode.alphaBlendFunction =  BlendFunction.Add;
                blendMode.sourceAlpha =         BlendFactor.SourceAlpha;
                blendMode.destinationAlpha =    BlendFactor.One;

                return blendMode;
            }
        }

        static BlendMode lastSet = BlendMode.Default;
        static Boolean neverSet = true;

        public static void Apply (IGraphicsManager graphics, BlendMode blendMode)
        {
            if (neverSet || lastSet != blendMode)
            {
                graphics.SetBlendEquation (
                    blendMode.rgbBlendFunction, blendMode.sourceRgb, blendMode.destinationRgb,
                    blendMode.alphaBlendFunction, blendMode.sourceAlpha, blendMode.destinationAlpha
                    );

                lastSet = blendMode;
            }
        }
    }


	// ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public struct RenderPassSettings
    {
        public static readonly RenderPassSettings Default;

        static RenderPassSettings()
        {
            Default.ClearDepthBuffer = false;
            Default.FogEnabled = false;
            Default.FogColour = Rgba32.CornflowerBlue;
            Default.FogStart = 300.0f;
            Default.FogEnd = 550.0f;
            Default.EnableDefaultLighting = true;
            Default.CameraProjectionType = CameraProjectionType.Perspective;
        }

        public Boolean ClearDepthBuffer;
        public Boolean FogEnabled;
        public Rgba32 FogColour;
        public Single FogStart;
        public Single FogEnd;
        public Boolean EnableDefaultLighting;
        public CameraProjectionType CameraProjectionType;
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    // THe user creates game states and uses them to interact with the engine
    public abstract class Scene
    {
        public virtual SceneSettings Settings
        {
            get
            {
                return SceneSettings.Default;
            }
        }

        internal void RegisterDrawCall() { drawCalls++; }
        int drawCalls = 0;

        internal void RegisterTriangles(int count) { triCount += count; }
        int triCount = 0;


		Services services;

        public ICor Cor { get; set;}

        public Services Blimey { get { return services; } }

        public Boolean Active { get { return _active; } }

        Boolean _active;
        List<SceneObject> _gameObjects = new List<SceneObject> ();

        public List<SceneObject> SceneObjects { get { return _gameObjects; } }

        CameraManager cameraManager;

        public SceneObject CreateSceneObject (string zName)
        {
            var go = new SceneObject (this, zName);
            _gameObjects.Add (go);
            return go;
        }

        public void DestroySceneObject (SceneObject zGo)
        {
            zGo.Shutdown ();
            foreach (SceneObject go in zGo.Children) {
                this.DestroySceneObject (go);
                _gameObjects.Remove (go);
            }
            _gameObjects.Remove (zGo);

            zGo = null;
        }

        public abstract void Start ();

        public abstract Scene Update(AppTime time);

        public abstract void Shutdown ();

        public void Initialize(ICor cor)
        {
            this.Cor = cor;
			this.services = new Services(this.Cor, this.Settings); ;

            cameraManager = new CameraManager(this);
            _active = true;

            this.Start();
        }

        internal Scene RunUpdate(AppTime time)
        {
            drawCalls = 0;
            triCount = 0;

            this.services.PreUpdate(time);

            foreach (SceneObject go in _gameObjects) {
                go.Update(time);
            }

            var ret =  this.Update(time);

            this.services.PostUpdate(time);

            return ret;
        }

        internal CameraManager CameraManager { get { return cameraManager; } }

        public void SetRenderPassCameraToDefault(string renderPass)
        {
            cameraManager.SetDefaultCamera (renderPass);
        }

        public void SetRenderPassCameraTo (string renderPass, SceneObject go)
        {
            cameraManager.SetMainCamera(renderPass, go);
        }

        public SceneObject GetRenderPassCamera(string renderPass)
        {
            return cameraManager.GetActiveCamera(renderPass).Parent;
        }

        public virtual void Uninitilise ()
        {
            this.Shutdown ();

            _active = false;

            List<SceneObject> onesToDestroy = new List<SceneObject> ();

            foreach (SceneObject go in _gameObjects) {
                if (go.Transform.Parent == null)
                    onesToDestroy.Add (go);
            }

            foreach (SceneObject go in onesToDestroy) {
                DestroySceneObject (go);
            }

            System.Diagnostics.Debug.Assert(_gameObjects.Count == 0);

            this.services = null;
            this.cameraManager = null;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    // Game Object
    // -----------
    //
    public sealed class SceneObject
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


        internal List<SceneObject> Children {
            get {
                List<SceneObject> kids = new List<SceneObject> ();
                foreach (SceneObject go in containedBy.SceneObjects) {
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

        internal SceneObject (Scene containedBy, string name)
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

        internal void Render(IGraphicsManager zGfx, Matrix44 zView, Matrix44 zProjection){
            if (!Enabled)
                return;

            foreach (Trait behaviour in behaviours) {
                if (behaviour.Active)
                    behaviour.Render(zGfx, zView, zProjection);
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
        protected Services Blimey { get; set; }

        public SceneObject Parent { get; set; }

        public Boolean Active { get; set; }

        // INTERNAL METHODS
        // called after constructor and before awake
		internal void Initilise (ICor cor, Services blimeyServices, SceneObject parent)
        {
            Cor = cor;
            Blimey = blimeyServices;
            Parent = parent;

            Active = true;
        }

        // INTERNAL CALLBACKS
        internal virtual void Render(IGraphicsManager zGfx, Matrix44 zView, Matrix44 zProjection) {}

        //TODO MAKE THESE ABSTRACT

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
}
