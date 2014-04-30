// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! Xamarin iOS Platform Implementation                               │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Brought to you by:                                                     │ \\
// │          _________                    .__               __             │ \\
// │         /   _____/__ __  ____    ____ |__|____    _____/  |_           │ \\
// │         \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\          │ \\
// │         /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |            │ \\
// │        /_______  /____/|___|  /\___  /|__(____  /___|  /__|            │ \\
// │                \/           \//_____/         \/     \/                │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2014 A.J.Pook (http://ajpook.github.io)                    │ \\
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

using System;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Linq;

using Abacus;
using Abacus.Packed;
using Abacus.SinglePrecision;
using Abacus.Int32Precision;

using Cor.Lib.Khronos;
using Cor.Platform.Stub;

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreText;
using MonoTouch.CoreGraphics;

namespace Cor.Platform.Xios
{
    public sealed class AudioManager
        : IAudioManager
    {
        public Single Volume { get; set; }
    }

    public static class EnumConverter
    {
        public static DeviceOrientation ToCor(MonoTouch.UIKit.UIDeviceOrientation monoTouch)
        {
            switch(monoTouch)
            {
                case MonoTouch.UIKit.UIDeviceOrientation.FaceDown: return DeviceOrientation.Default;
                case MonoTouch.UIKit.UIDeviceOrientation.FaceUp: return DeviceOrientation.Default;
                case MonoTouch.UIKit.UIDeviceOrientation.LandscapeLeft: return DeviceOrientation.Leftside;
                case MonoTouch.UIKit.UIDeviceOrientation.LandscapeRight: return DeviceOrientation.Rightside;
                case MonoTouch.UIKit.UIDeviceOrientation.Portrait: return DeviceOrientation.Default;
                case MonoTouch.UIKit.UIDeviceOrientation.PortraitUpsideDown: return DeviceOrientation.Upsidedown;
            
                default:
                    Console.WriteLine ("WARNING: Unknown device orientaton: " + monoTouch);
                    return DeviceOrientation.Default;
            }
        }

        public static TouchPhase ToCorPrimitiveType(MonoTouch.UIKit.UITouchPhase phase)
        {
            switch(phase)
            {
                case MonoTouch.UIKit.UITouchPhase.Began: return TouchPhase.JustPressed;
                case MonoTouch.UIKit.UITouchPhase.Cancelled: return TouchPhase.JustReleased;
                case MonoTouch.UIKit.UITouchPhase.Ended: return TouchPhase.JustReleased;
                case MonoTouch.UIKit.UITouchPhase.Moved: return TouchPhase.Active;
                case MonoTouch.UIKit.UITouchPhase.Stationary: return TouchPhase.Active;
            }

            return TouchPhase.Invalid;
        }
    }

    public static class Vector2Converter
    {
        public static System.Drawing.PointF ToSystemDrawing(this Vector2 vec)
        {
            return new System.Drawing.PointF (vec.X, vec.Y);
        }

        public static Vector2 ToAbacus (this System.Drawing.PointF vec)
        {
            return new Vector2 (vec.X, vec.Y);
        }
    }

    public sealed class DisplayStatus
        : IDisplayStatus
    {
        public Boolean Fullscreen
        {
            get
            {
                // always fullscreen on iOS
                return true;
            }
        }

        public Int32 CurrentWidth
        {
            get
            {
                return (Int32) MonoTouch.UIKit.UIScreen.MainScreen.CurrentMode.Size.Width;
            }
        }

        public Int32 CurrentHeight
        {
            get
            {
                return (Int32) MonoTouch.UIKit.UIScreen.MainScreen.CurrentMode.Size.Height;
            }
        }
    }

    [MonoTouch.Foundation.Register ("EAGLView")]
    public sealed class EAGLView 
        : OpenTK.Platform.iPhoneOS.iPhoneOSGameView
    {

        AppSettings settings;
        IApp game;

        Engine gameEngine;
        Stopwatch timer = new Stopwatch();
        Single elapsedTime;
        Int64 frameCounter = -1;
        TimeSpan previousTimeSpan;
        Int32 frameInterval;

        MonoTouch.CoreAnimation.CADisplayLink displayLink;

        uint _depthRenderbuffer;

        Dictionary<Int32, iOSTouchState> touchState = new Dictionary<int, iOSTouchState>();

        public System.Boolean IsAnimating 
        { 
            get; 
            private set; 
        }
        
        // How many display frames must pass between each time the display link fires.
        public Int32 FrameInterval
        {
            get
            {
                return frameInterval;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException ();
                }

                frameInterval = value;

                if (IsAnimating)
                {
                    StopAnimating ();
                    StartAnimating ();
                }
            }
        }


        /*
        [MonoTouch.Foundation.Export("initWithCoder:")]
        public EAGLView (MonoTouch.Foundation.NSCoder coder) 
            : base (coder)
        {
            LayerRetainsBacking = true;
            LayerColorFormat = MonoTouch.OpenGLES.EAGLColorFormat.RGBA8;


        }

*/
        public EAGLView (System.Drawing.RectangleF frame)
            : base (frame)
        {
            LayerRetainsBacking = true;
            LayerColorFormat = MonoTouch.OpenGLES.EAGLColorFormat.RGBA8;
            ContextRenderingApi = MonoTouch.OpenGLES.EAGLRenderingAPI.OpenGLES2;

        }

        [MonoTouch.Foundation.Export ("layerClass")]
        public static new MonoTouch.ObjCRuntime.Class GetLayerClass ()
        {
            return OpenTK.Platform.iPhoneOS.iPhoneOSGameView.GetLayerClass ();
        }

        
        protected override void ConfigureLayer (MonoTouch.CoreAnimation.CAEAGLLayer eaglLayer)
        {
            eaglLayer.Opaque = true;
        }

        protected override void CreateFrameBuffer()
        {
            base.CreateFrameBuffer();

            //
            // Enable the depth buffer
            //
            OpenTK.Graphics.ES20.GL.GenRenderbuffers(1, out _depthRenderbuffer);
            ErrorHandler.Check();

            OpenTK.Graphics.ES20.GL.BindRenderbuffer(OpenTK.Graphics.ES20.RenderbufferTarget.Renderbuffer, _depthRenderbuffer);
            ErrorHandler.Check();

            OpenTK.Graphics.ES20.GL.RenderbufferStorage(OpenTK.Graphics.ES20.RenderbufferTarget.Renderbuffer, OpenTK.Graphics.ES20.RenderbufferInternalFormat.DepthComponent16, Size.Width, Size.Height);
            ErrorHandler.Check();

            OpenTK.Graphics.ES20.GL.FramebufferRenderbuffer(
                OpenTK.Graphics.ES20.FramebufferTarget.Framebuffer,
                OpenTK.Graphics.ES20.FramebufferSlot.DepthAttachment,
                OpenTK.Graphics.ES20.RenderbufferTarget.Renderbuffer,
                _depthRenderbuffer);
            ErrorHandler.Check();

        }
        
        public void SetEngineDetails(AppSettings settings, IApp game)
        {
            this.settings = settings;
            this.game = game;
        }

        void CreateEngine()
        {
            gameEngine = new Engine(
                this.settings,
                this.game,
                this, 
                this.GraphicsContext, 
                this.touchState);
            timer.Start();
        }
        
        protected override void DestroyFrameBuffer ()
        {
            base.DestroyFrameBuffer ();
        }

        public void StartAnimating ()
        {
            if (IsAnimating)
                return;
            
            CreateFrameBuffer ();

            CreateEngine();

            displayLink = 
                MonoTouch.UIKit.UIScreen.MainScreen.CreateDisplayLink (
                    this, 
                    new MonoTouch.ObjCRuntime.Selector ("drawFrame")
                    );

            displayLink.FrameInterval = frameInterval;
            displayLink.AddToRunLoop (MonoTouch.Foundation.NSRunLoop.Current, MonoTouch.Foundation.NSRunLoop.NSDefaultRunLoopMode);
            
            IsAnimating = true;
        }
        
        public void StopAnimating ()
        {
            if (!IsAnimating)
                return;

            displayLink.Invalidate ();
            displayLink = null;

            DestroyFrameBuffer ();

            IsAnimating = false;
        }
        
        

        [MonoTouch.Foundation.Export ("drawFrame")]
        void DrawFrame ()
        {
            var e = new OpenTK.FrameEventArgs ();
            OnUpdateFrame(e);
            OnRenderFrame(e);

        }



        protected override void OnUpdateFrame(OpenTK.FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            this.ClearOldTouches();

            Single dt = (Single)(timer.Elapsed.TotalSeconds - previousTimeSpan.TotalSeconds);
            previousTimeSpan = timer.Elapsed;
            
            if (dt > 0.5f)
            {
                dt = 0.0f;
            }

            elapsedTime += dt;

            var appTime = new AppTime(dt, elapsedTime, ++frameCounter);


            gameEngine.Update(appTime);
        
        }

        void ClearOldTouches()
        {
            var keysToDitch = new List<Int32>();

            //remove stuff
            var keys = touchState.Keys;

            foreach(var key in keys)
            {
                var ts = touchState[key];

                if( ts.Phase == MonoTouch.UIKit.UITouchPhase.Cancelled ||
                    ts.Phase == MonoTouch.UIKit.UITouchPhase.Ended )
                {
                    if( ts.LastUpdated < this.frameCounter )
                    {
                        keysToDitch.Add(key);
                    }
                }
            }

            foreach(var key in keysToDitch)
            {
                touchState.Remove(key);
                
                //Console.WriteLine("remove "+key);
            }
        }

        protected override void OnRenderFrame (OpenTK.FrameEventArgs e)
        {
            base.OnRenderFrame (e);

            base.MakeCurrent();
            
            gameEngine.Render();

            this.SwapBuffers ();
        }

        /*
        public override void Draw(RectangleF rect)
        {
            var gctx = UIGraphics.GetCurrentContext ();
            
            gctx.TranslateCTM (10, 0.5f * Bounds.Height);
            gctx.ScaleCTM (1, -1);
            gctx.RotateCTM ((float)Math.PI * 315 / 180);
            
            gctx.SetFillColor (UIColor.Green.CGColor);
            
            string someText = "ä½ å¥½ä¸ç";

            var attributedString = new NSAttributedString (someText,
                                                           new CTStringAttributes{
                ForegroundColorFromContext =  true,
                Font = new CTFont ("Arial", 24)
            });

            using (var textLine = new CTLine (attributedString)) {
                textLine.Draw (gctx);
            }
            
            base.Draw(rect);

        }*/

        public override void TouchesBegan(MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
        {
            ProcessTouchChange(touches);

            base.TouchesBegan(touches, evt);
        }

        public override void TouchesMoved(MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
        {
            ProcessTouchChange(touches);

            base.TouchesMoved(touches, evt);
        }

        public override void TouchesCancelled(MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
        {
            ProcessTouchChange(touches);

            base.TouchesCancelled(touches, evt);
        }

        public override void TouchesEnded(MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
        {
            ProcessTouchChange(touches);

            base.TouchesEnded(touches, evt);
        }


        void ProcessTouchChange(MonoTouch.Foundation.NSSet touches)
        {
            var touchesArray = touches.ToArray<MonoTouch.UIKit.UITouch> ();

            for (int i = 0; i < touchesArray.Length; ++i) 
            {
                var touch = touchesArray [i];

                //Get position touch
                var location = touch.LocationInView (this);
                var id = touch.Handle.ToInt32 ();
                var phase = touch.Phase;

                var ts = new iOSTouchState();
                ts.Handle = id;
                ts.LastUpdated = this.frameCounter;
                ts.Location = location;
                ts.Phase = phase;

                if( phase == MonoTouch.UIKit.UITouchPhase.Began )
                {
                    //Console.WriteLine("add "+id);
                    touchState.Add(id, ts);
                }
                else
                {
                    if( touchState.ContainsKey(id) )
                    {
                        touchState[id] = ts;

                        if(ts.Phase == MonoTouch.UIKit.UITouchPhase.Began)
                        {
                            ts.Phase = MonoTouch.UIKit.UITouchPhase.Stationary;
                        }

                    }
                    else
                    {
                        throw new Exception("eerrr???");
                    }
                }
            }
        }
    }

    public sealed class Engine
        : ICor
    {
        readonly TouchScreen touchScreen;
        readonly AppSettings settings;
        readonly IApp app;
        readonly GraphicsManager graphics;
        readonly InputManager input;
        readonly SystemManager system;
        readonly AudioManager audio;
        readonly DisplayStatus displayStatus;
        readonly LogManager log;
        readonly AssetManager assets;

        internal Engine(
            AppSettings settings,
            IApp app,
            OpenTK.Platform.iPhoneOS.iPhoneOSGameView view,
            OpenTK.Graphics.IGraphicsContext gfxContext,
            Dictionary<Int32, iOSTouchState> touches)
        {
            this.settings = settings;

            this.app = app;

            this.graphics = new GraphicsManager();

            this.touchScreen = new TouchScreen(this, view, touches);

            this.system = new SystemManager(touchScreen);

            this.input = new InputManager(this, this.touchScreen);

            this.displayStatus = new DisplayStatus ();

            this.log = new LogManager(this.settings.LogSettings);

            this.assets = new AssetManager(this.graphics, this.system);

            this.app.Initilise(this);

        }

        internal TouchScreen TouchScreenImplementation
        {
            get
            {
                return touchScreen;
            }
        }

        #region ICor

        public IAudioManager Audio { get { return this.audio; } }

        public IGraphicsManager Graphics { get { return this.graphics; } }

        public IInputManager Input { get { return this.input; } }

        public ISystemManager System { get { return this.system; } }

        public IDisplayStatus DisplayStatus { get { return this.displayStatus; } }

        public LogManager Log { get { return this.log; } }

        public AssetManager Assets { get { return this.assets; } }

        public AppSettings Settings { get { return this.settings; } }

        #endregion

        internal Boolean Update(AppTime time)
        {
            input.Update(time);
            return app.Update(time);
        }

        internal void Render()
        {
            app.Render();
        }

    }

    public sealed class InputManager
        : IInputManager
    {
        readonly TouchScreen touchScreen;

        readonly IXbox360Gamepad stubXbox360Gamepad = new StubXbox360Gamepad();
        readonly IPsmGamepad stubPsmGamepad = new StubPsmGamepad();
        readonly IGenericGamepad stubGenericGamepad = new StubGenericGamepad();
        readonly IKeyboard stubKeyboard = new StubKeyboard();
        readonly IMouse stubMouse = new StubMouse();

        internal void Update(AppTime time)
        {
            this.touchScreen.Update(time);
        }

        #region IInputManager

        public InputManager(ICor engine, TouchScreen touchScreen)
        {
            this.touchScreen = touchScreen;
        }

        public IMultiTouchController MultiTouchController
        {
            get { return this.touchScreen; }
        }

        public IXbox360Gamepad Xbox360Gamepad
        {
            get { return stubXbox360Gamepad; }
        }

        public IPsmGamepad PsmGamepad
        {
            get { return stubPsmGamepad; }
        }

        public IGenericGamepad GenericGamepad
        {
            get { return stubGenericGamepad; }
        }

        public IMouse Mouse
        {
            get { return stubMouse; }
        }

        public IKeyboard Keyboard
        {
            get { return stubKeyboard; }
        }

        #endregion
    }

    internal struct iOSTouchState
    {
        public Int32 Handle;
        public System.Drawing.PointF Location;
        public MonoTouch.UIKit.UITouchPhase Phase;
        public Int64 LastUpdated;
    }

    [MonoTouch.Foundation.Register ("OpenGLViewController")]
    public sealed class OpenGLViewController 
        : MonoTouch.UIKit.UIViewController
    {
        AppSettings _settings;
        IApp _game;
            
        public OpenGLViewController (
            AppSettings settings,
            IApp game)
            : base ()
        {
            MonoTouch.UIKit.UIApplication.SharedApplication.SetStatusBarHidden (true, MonoTouch.UIKit.UIStatusBarAnimation.None);
            _settings = settings;
            _game = game;
        }
        
        new EAGLView View
        {
            get
            {
                return (EAGLView) base.View;
            }
        }
        /*
        // stuff to expose specifically to the monotouch implementation
        public override MonoTouch.UIKit.UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
        {
            return new MonoTouch.UIKit.UIInterfaceOrientationMask();
        }

        public override MonoTouch.UIKit.UIInterfaceOrientation PreferredInterfaceOrientationForPresentation ()
        {
            return base.PreferredInterfaceOrientationForPresentation ();
        }


        public override bool ShouldAutorotate ()
        {
            return base.ShouldAutorotate ();
        }
        */

        public override void LoadView()
        {
            //var size = MonoTouch.UIKit.UIScreen.MainScreen.CurrentMode.Size;
            //var frame = new System.Drawing.RectangleF(0, 0, size.Width, size.Height);
            var frame = MonoTouch.UIKit.UIScreen.MainScreen.Bounds;
            base.View = new EAGLView(frame);
        }
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.WillResignActiveNotification, a => {
                if (IsViewLoaded && View.Window != null)
                    View.StopAnimating ();
                },
                this
            );

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.DidBecomeActiveNotification, a => {
                if (IsViewLoaded && View.Window != null)
                    View.StartAnimating ();
                },
                this
            );

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.WillTerminateNotification, a => {
                if (IsViewLoaded && View.Window != null)
                    View.StopAnimating ();
                },
                this
            );
            
            View.SetEngineDetails(_settings, _game);
        }
        
        protected override void Dispose (System.Boolean disposing)
        {
            base.Dispose (disposing);
            
            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.RemoveObserver (this);
        }
        
        public override void DidReceiveMemoryWarning ()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning ();
            
            // Release any cached data, images, etc that aren't in use.
        }

        public override void DidRotate(MonoTouch.UIKit.UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            //String previous = fromInterfaceOrientation


        }

        
        public override void ViewWillAppear (System.Boolean animated)
        {
            base.ViewWillAppear (animated);
            View.StartAnimating ();
        }
        
        public override void ViewWillDisappear (System.Boolean animated)
        {
            base.ViewWillDisappear (animated);
            View.StopAnimating ();
        }
    }

    /// <summary>
    /// The Cor.Xios implementation of Cor's IShader interface.
    /// </summary>
    public sealed class Shader
        : IShader
        , IDisposable
    {
        //static Dictionary<string, parp>


        #region IShader

        /// <summary>
        /// Resets all the shader's variables to their default values.
        /// </summary>
        public void ResetVariables()
        {
            // the shader definition defines the default values for the variables
            foreach (var variableDefinition in cachedShaderDefinition.VariableDefinitions)
            {
                string varName = variableDefinition.Name;
                object value = variableDefinition.DefaultValue;

                if( variableDefinition.Type == typeof(Matrix44) )
                {
                    this.SetVariable(varName, (Matrix44) value);
                }
                else if( variableDefinition.Type == typeof(Int32) )
                {
                    this.SetVariable(varName, (Int32) value);
                }
                else if( variableDefinition.Type == typeof(Single) )
                {
                    this.SetVariable(varName, (Single) value);
                }
                else if( variableDefinition.Type == typeof(Vector2) )
                {
                    this.SetVariable(varName, (Vector2) value);
                }
                else if( variableDefinition.Type == typeof(Vector3) )
                {
                    this.SetVariable(varName, (Vector3) value);
                }
                else if( variableDefinition.Type == typeof(Vector4) )
                {
                    this.SetVariable(varName, (Vector4) value);
                }
                else if( variableDefinition.Type == typeof(Rgba32) )
                {
                    this.SetVariable(varName, (Rgba32) value);
                }
                else
                {
                    throw new NotSupportedException();
                }

            }
        }

        /// <summary>
        /// Resets all the shader's texture samplers point at texture slot 0.
        /// </summary>
        public void ResetSamplerTargets()
        {
            foreach (var samplerDefinition in cachedShaderDefinition.SamplerDefinitions)
            {
                this.SetSamplerTarget(samplerDefinition.Name, 0);
            }
        }

        /// <summary>
        /// Sets the value of a specified shader variable.
        /// </summary>
        public void SetVariable<T>(string name, T value)
        {
            passes.ForEach( x => x.SetVariable(name, value));
        }

        /// <summary>
        /// Sets the texture slot that a texture sampler should sample from.
        /// </summary>
        public void SetSamplerTarget(string name, Int32 textureSlot)
        {
            foreach (var pass in passes)
            {
                pass.SetSamplerTarget(name, textureSlot);
            }
        }


        /// <summary>
        /// Provides access to the individual passes in this shader.
        /// the calling code can itterate though these and apply them
        ///to the graphics context before it makes a draw call.
        /// </summary>
        public IShaderPass[] Passes
        {
            get
            {
                return passes.ToArray();
            }
        }

        /// <summary>
        /// Defines which vertex elements are required by this shader.
        /// </summary>
        public VertexElementUsage[] RequiredVertexElements
        {
            get
            {
                // todo: an array of vert elem usage doesn't uniquely identify anything...
                return requiredVertexElements.ToArray();
            }
        }

        /// <summary>
        /// Defines which vertex elements are optionally used by this
        /// shader if they happen to be present.
        /// </summary>
        public VertexElementUsage[] OptionalVertexElements
        {
            get
            {
                // todo: an array of vert elem usage doesn't uniquely identify anything...
                return optionalVertexElements.ToArray();
            }
        }

        public String Name { get; private set; }

        #endregion

        #region IDisposable

        /// <summary>
        /// Releases all resource used by the <see cref="Cor.MonoTouchRuntime.Shader"/> object.
        /// </summary>
        public void Dispose()
        {
            foreach (var pass in passes)
            {
                pass.Dispose();
            }
        }

        #endregion


        List<VertexElementUsage> requiredVertexElements = new List<VertexElementUsage>();
        List<VertexElementUsage> optionalVertexElements = new List<VertexElementUsage>();


        /// <summary>
        /// The <see cref="ShaderPass"/> objects that need to each, in turn,  be individually activated and used to
        /// draw with to apply the effect of this containing <see cref="Shader"/> object.
        /// </summary>
        List<ShaderPass> passes = new List<ShaderPass>();

        /// <summary>
        /// Cached reference to the <see cref="ShaderDefinition"/> object used
        /// to create this <see cref="Shader"/> object.
        /// </summary>
        readonly ShaderDefinition cachedShaderDefinition;

        public ShaderDefinition ShaderDefinition { get { return cachedShaderDefinition; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shader"/> class from a
        /// <see cref="ShaderDefinition"/> object.
        /// </summary>
        internal Shader (ShaderDefinition shaderDefinition)
        {
            Console.WriteLine("\n");
            Console.WriteLine("\n");
            Console.WriteLine("=====================================================================");
            Console.WriteLine("Creating Shader: " + shaderDefinition.Name);
            this.cachedShaderDefinition = shaderDefinition;
            this.Name = shaderDefinition.Name;
            CalculateRequiredInputs(shaderDefinition);
            InitilisePasses (shaderDefinition);

            this.ResetVariables();
        }

        /// <summary>
        /// Works out and caches a copy of which shader inputs are required/optional, needed as the
        /// <see cref="IShader"/> interface requires this information.
        /// </summary>
        void CalculateRequiredInputs(ShaderDefinition shaderDefinition)
        {
            foreach (var input in shaderDefinition.InputDefinitions)
            {
                if( input.Optional )
                {
                    optionalVertexElements.Add(input.Usage);
                }
                else
                {
                    requiredVertexElements.Add(input.Usage);
                }
            }
        }

        /// <summary>
        /// Triggers the creation of all of this <see cref="Shader"/> object's passes.
        /// </summary>
        void InitilisePasses(ShaderDefinition shaderDefinition)
        {
            // This function builds up an in memory object for each shader pass in this shader.
            // The different shader varients are defined outside of the scope of a conceptual shader pass,
            // therefore this function must traverse the shader definition and to create shader pass objects
            // that only contain the varient data for that specific pass.


            // For each named shader pass.
            foreach (var definedPassName in shaderDefinition.PassNames)
            {

                Console.WriteLine(" Preparing to initilising Shader Pass: " + definedPassName);
                //

                // itterate over the defined pass names, ex: cel, outline...



                //shaderDefinition.VariantDefinitions
                //  .Select(x => x.PassDefinitions.Select(y => y.PassName == definedPassName))
                //  .ToList();

                // Find all of the variants that are defined in this shader object's definition
                // that support the current shaderpass.
                var passVariants___Name_AND_passVariantDefinition = new List<Tuple<string, ShaderVarientPassDefinition>>();

                // itterate over every shader variant in the definition
                foreach (var shaderVariantDefinition in shaderDefinition.VariantDefinitions)
                {
                    // each shader varient has a name
                    string shaderVariantName = shaderVariantDefinition.VariantName;

                    // find the pass in the shader variant definition that corresponds to the pass we are
                    // currently trying to initilise.
                    var variantPassDefinition =
                        shaderVariantDefinition.VariantPassDefinitions
                            .Find(x => x.PassName == definedPassName);


                    // now we have a Variant name, say:
                    //   - Unlit_PositionTextureColour
                    // and a pass definition, say :
                    //   - Main
                    //   - Shaders/Unlit_PositionTextureColour.vsh
                    //   - Shaders/Unlit_PositionTextureColour.fsh
                    //

                    passVariants___Name_AND_passVariantDefinition.Add(
                        new Tuple<string, ShaderVarientPassDefinition>(shaderVariantName, variantPassDefinition));

                }

                // Create one shader pass for each defined pass name.
                var shaderPass = new ShaderPass( definedPassName, passVariants___Name_AND_passVariantDefinition );

                shaderPass.BindAttributes (shaderDefinition.InputDefinitions.Select(x => x.Name).ToList());
                shaderPass.Link ();
                shaderPass.ValidateInputs(shaderDefinition.InputDefinitions);
                shaderPass.ValidateVariables(shaderDefinition.VariableDefinitions);
                shaderPass.ValidateSamplers(shaderDefinition.SamplerDefinitions);

                passes.Add(shaderPass);
            }
        }
    }

    /// <summary>
    /// Defines how to create Cor.Xios's implementation
    /// of IShader.
    /// </summary>
    public sealed class ShaderDefinition
    {
        /// <summary>
        /// Defines a global name for this shader
        /// </summary>
        public string Name { get; set; }
        
        /// Defines which passes this shader is made from 
        /// (ex: a toon shader is made for a cel-shading pass 
        /// followed by an edge detection pass)
        /// </summary>
        public List<String> PassNames { get; set; }
        
        /// <summary>
        /// Lists all of the supported inputs into this shader and
        /// defines whether or not they are optional to an implementation.
        /// </summary>
        public List<ShaderInputDefinition> InputDefinitions { get; set; }
        
        /// <summary>
        /// Defines all of the variables supported by this shader.  Every
        /// variant must support all of the variables.
        /// </summary>
        public List<ShaderVariableDefinition> VariableDefinitions { get; set; }

        
        public List<ShaderSamplerDefinition> SamplerDefinitions { get; set; }
        
        /// <summary>
        /// Defines the variants.  Done for optimisation, instead of having one
        /// massive shader that supports all the the Inputs and attempts to
        /// process them accordingly, we load slight variants of effectively 
        /// the same shader, then we select the most optimal variant to run
        /// based upon the VertexDeclaration the calling code is about to draw.
        /// </summary>
        public List<ShaderVariantDefinition> VariantDefinitions { get; set; }
    }

    public static class ShaderHelper
    {
        /// <summary>
        /// This function takes a VertexDeclaration and a collection of OpenGL shader passes and works out which
        /// pass is the best fit for the VertexDeclaration.
        /// </summary>
        public static OglesShader WorkOutBestVariantFor(VertexDeclaration vertexDeclaration, IList<OglesShader> variants)
        {
            Console.WriteLine("\n");
            Console.WriteLine("\n");
            Console.WriteLine("=====================================================================");
            Console.WriteLine("Working out the best shader variant for: " + vertexDeclaration);
            Console.WriteLine("Possible variants:");

            int best = 0;

            int bestNumMatchedVertElems = 0;
            int bestNumUnmatchedVertElems = 0;
            int bestNumMissingNonOptionalInputs = 0;

            // foreach variant
            for (int i = 0; i < variants.Count; ++i)
            {
                // work out how many vert inputs match

                
                var matchResult = CompareShaderInputs(vertexDeclaration, variants[i]);

                int numMatchedVertElems = matchResult.NumMatchedInputs;
                int numUnmatchedVertElems = matchResult.NumUnmatchedInputs;
                int numMissingNonOptionalInputs = matchResult.NumUnmatchedRequiredInputs;

                Console.WriteLine(" - " + variants[i]);

                if( i == 0 )
                {
                    bestNumMatchedVertElems = numMatchedVertElems;
                    bestNumUnmatchedVertElems = numUnmatchedVertElems;
                    bestNumMissingNonOptionalInputs = numMissingNonOptionalInputs;
                }
                else
                {
                    if( 
                        (
                            numMatchedVertElems > bestNumMatchedVertElems && 
                            bestNumMissingNonOptionalInputs == 0
                        )
                        || 
                        (
                            numMatchedVertElems == bestNumMatchedVertElems && 
                            bestNumMissingNonOptionalInputs == 0 &&
                            numUnmatchedVertElems < bestNumUnmatchedVertElems 
                        )
                      )
                    {
                        bestNumMatchedVertElems = numMatchedVertElems;
                        bestNumUnmatchedVertElems = numUnmatchedVertElems;
                        bestNumMissingNonOptionalInputs = numMissingNonOptionalInputs;
                        best = i;
                    }
                    
                }
                
            }

            //best = 2;
            Console.WriteLine("Chosen variant: " + variants[best].VariantName);

            return variants[best];
        }

        struct CompareShaderInputsResult
        {
            // the nume
            public int NumMatchedInputs;
            public int NumUnmatchedInputs;
            public int NumUnmatchedRequiredInputs;
        }

        static CompareShaderInputsResult CompareShaderInputs (
            VertexDeclaration vertexDeclaration, 
            OglesShader oglesShader
            )
        {
            var result = new CompareShaderInputsResult();
            
            var oglesShaderInputsUsed = new List<OglesShaderInput>();
            
            var vertElems = vertexDeclaration.GetVertexElements();

            // itterate over each input defined in the vert decl
            foreach(var vertElem in vertElems)
            {
                var usage = vertElem.VertexElementUsage;

                var format = vertElem.VertexElementFormat;
                /*

                foreach( var input in oglesShader.Inputs )
                {
                    // the vertDecl knows what each input's intended use is,
                    // so lets match up 
                    if( input.Usage == usage )
                    {
                        // intended use seems good
                    }
                }

                // find all inputs that could match
                var matchingInputs = oglesShader.Inputs.FindAll(
                    x => 

                        x.Usage == usage &&
                        (x.Type == VertexElementFormatHelper.FromEnum(format) || 
                        ( (x.Type.GetType() == typeof(Vector4)) && (format == VertexElementFormat.Colour) ))

                 );*/

                var matchingInputs = oglesShader.Inputs.FindAll(x => x.Usage == usage);
                
                // now make sure it's not been used already
                
                while(matchingInputs.Count > 0)
                {
                    var potentialInput = matchingInputs[0];
                    
                    if( oglesShaderInputsUsed.Find(x => x == potentialInput) != null)
                    {
                        matchingInputs.RemoveAt(0);
                    }
                    else
                    {
                        oglesShaderInputsUsed.Add(potentialInput);
                    }
                }
            }
            
            result.NumMatchedInputs = oglesShaderInputsUsed.Count;
            
            result.NumUnmatchedInputs = vertElems.Length - result.NumMatchedInputs;
            
            result.NumUnmatchedRequiredInputs = 0;
            
            foreach (var input in oglesShader.Inputs)
            {
                if(!oglesShaderInputsUsed.Contains(input) )
                {
                    if( !input.Optional )
                    {
                        result.NumUnmatchedRequiredInputs++;
                    }
                }
                
            }

            Console.WriteLine(string.Format("[{0}, {1}, {2}]", result.NumMatchedInputs, result.NumUnmatchedInputs, result.NumUnmatchedRequiredInputs));
            return result;
        }

    }

    /// <summary>
    /// Represents in individual pass of a Cor.Xios high level Shader object.
    /// </summary>
    public sealed class ShaderPass
        : IShaderPass
        , IDisposable
    {
        /// <summary>
        /// A collection of OpenGL shaders, all with slight variations in their
        /// input parameters, that are suitable for rendering this ShaderPass object.
        /// </summary>
        List<OglesShader> Variants { get; set; }
        
        /// <summary>
        /// A nice name for the shader pass, for example: Main or Cel -> Outline.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Whenever this ShaderPass object gets asked to activate itself whilst a VertexDeclaration it has not seen
        /// before is active, the best matching shader pass variant is found and then stored in this map to fast
        /// access.
        /// </summary>
        Dictionary<VertexDeclaration, OglesShader> BestVariantMap { get; set; }

        Dictionary<String, Object>  currentVariables = new Dictionary<String, Object>();
        Dictionary<String, Int32>   currentSamplerSlots = new Dictionary<String, Int32>();

        Dictionary<String, bool> logHistory = new Dictionary<String, bool>();

        internal void SetVariable<T>(string name, T value)
        {
            currentVariables[name] = value; 
        }

        internal void SetSamplerTarget(string name, Int32 textureSlot)
        {
            currentSamplerSlots[name] = textureSlot;
        }
        
        public ShaderPass(string passName, List<Tuple<string, ShaderVarientPassDefinition>> passVariants___Name_AND_passVariantDefinition)
        {
            Console.WriteLine("Creating ShaderPass: " + passName);
            this.Name = passName;
            this.Variants = 
                passVariants___Name_AND_passVariantDefinition
                    .Select (x => new OglesShader (x.Item1, passName, x.Item2.PassDefinition))
                    .ToList();

            this.BestVariantMap = new Dictionary<VertexDeclaration, OglesShader>();
        }

        
        internal void BindAttributes(IList<String> inputNames)
        {
            foreach (var variant in this.Variants)
            {
                variant.BindAttributes(inputNames);
            }
        }

        internal void Link()
        {
            foreach (var variant in this.Variants)
            {
                variant.Link();
            }
        }
        
        internal void ValidateInputs(List<ShaderInputDefinition> definitions)
        {
            foreach(var variant in this.Variants)
            {
                variant.ValidateInputs(definitions);
            }
        }
        
        internal void ValidateVariables(List<ShaderVariableDefinition> definitions)
        {
            foreach(var variant in this.Variants)
            {
                variant.ValidateVariables(definitions);
            }
        }

        internal void ValidateSamplers(List<ShaderSamplerDefinition> definitions)
        {
            foreach(var variant in this.Variants)
            {
                variant.ValidateSamplers(definitions);
            }
        }
        
        
        public void Activate(VertexDeclaration vertexDeclaration)
        {
            if (!BestVariantMap.ContainsKey (vertexDeclaration))
            {
                BestVariantMap[vertexDeclaration] = ShaderHelper.WorkOutBestVariantFor(vertexDeclaration, Variants);
            }
            var bestVariant = BestVariantMap[vertexDeclaration];
            // select the correct shader pass variant and then activate it
            bestVariant.Activate ();
            
            foreach (var key1 in currentVariables.Keys)
            {
                var variable = bestVariant
                    .Variables
                    .Find(x => x.NiceName == key1 || x.Name == key1);
                
                if( variable == null )
                {
                    string warning = "WARNING: missing variable: " + key1;

                    if( !logHistory.ContainsKey(warning) )
                    {
                        Console.WriteLine(warning);

                        logHistory.Add(warning, true);
                    }
                }
                else
                {
                    var val = currentVariables[key1];
                    
                    variable.Set(val);
                }
            }

            foreach (var key2 in currentSamplerSlots.Keys)
            {
                var sampler = bestVariant
                    .Samplers
                    .Find(x => x.NiceName == key2 || x.Name == key2);

                if( sampler == null )
                {
                    //Console.WriteLine("missing sampler: " + key2);
                }
                else
                {
                    var slot = currentSamplerSlots[key2];

                    sampler.SetSlot(slot);
                }
            }
            
        }
        
        public void Dispose()
        {
            foreach (var oglesShader in Variants)
            {
                oglesShader.Dispose ();
            }
        }
    }
    
    public sealed class SystemManager
        : ISystemManager
    {
        TouchScreen screen;

        public SystemManager(TouchScreen screen)
        {
            this.screen = screen;

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.DidEnterBackgroundNotification, this.DidEnterBackground );

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.DidBecomeActiveNotification, this.DidBecomeActive );
            
            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.DidReceiveMemoryWarningNotification, this.DidReceiveMemoryWarning );

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.DidFinishLaunchingNotification, this.DidFinishLaunching );

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIDevice.OrientationDidChangeNotification, this.OrientationDidChange );

        }

        public String OperatingSystem
        {
            get
            {
                return System.Environment.OSVersion.VersionString;
            }
        }

        public void DidReceiveMemoryWarning(MonoTouch.Foundation.NSNotification ntf)
        {
            Console.WriteLine("[Cor.System] DidReceiveMemoryWarning");
        }

        public void DidBecomeActive(MonoTouch.Foundation.NSNotification ntf)
        {
            Console.WriteLine("[Cor.System] DidBecomeActive");
        }

        public void DidEnterBackground(MonoTouch.Foundation.NSNotification ntf)
        {
            Console.WriteLine("[Cor.System] DidEnterBackground");
        }
        
        public void DidFinishLaunching(MonoTouch.Foundation.NSNotification ntf)
        {
            Console.WriteLine("[Cor.System] DidFinishLaunching");
        }

        public void OrientationDidChange(MonoTouch.Foundation.NSNotification ntf)
        {
            Console.WriteLine("[Cor.System] OrientationDidChange, CurrentOrientation: " + CurrentOrientation.ToString() 
                              + ", CurrentDisplaySize: " + CurrentDisplaySize.ToString());

        }

        public Point2 CurrentDisplaySize
        {
            get
            {
                Int32 w = ScreenSpecification.ScreenResolutionWidth;
                Int32 h = ScreenSpecification.ScreenResolutionHeight;

                GetEffectiveDisplaySize(ref w, ref h);

                return new Point2(w, h);

            }
        }

        void GetEffectiveDisplaySize(ref Int32 screenSpecWidth, ref Int32 screenSpecHeight)
        {
            if (this.CurrentOrientation == DeviceOrientation.Default ||
                this.CurrentOrientation == DeviceOrientation.Upsidedown )
            {
                return;
            }
            else
            {
                Int32 temp = screenSpecWidth;
                screenSpecWidth = screenSpecHeight;
                screenSpecHeight = temp;
            }
        }


        public String DeviceName
        {
            get
            {
                return MonoTouch.UIKit.UIDevice.CurrentDevice.Name;
            }
        }

        public String DeviceModel
        {
            get
            {
                return MonoTouch.UIKit.UIDevice.CurrentDevice.Model;
            }
        }

        public String SystemName
        {
            get
            {
                return MonoTouch.UIKit.UIDevice.CurrentDevice.SystemName;
            }
        }

        public String SystemVersion
        {
            get
            {
                return MonoTouch.UIKit.UIDevice.CurrentDevice.SystemVersion;
            }
        }

        public DeviceOrientation CurrentOrientation
        {
            get
            {
                var monoTouchOrientation = MonoTouch.UIKit.UIDevice.CurrentDevice.Orientation;

                return EnumConverter.ToCor(monoTouchOrientation);
            }
        }


        public IScreenSpecification ScreenSpecification
        {
            get
            {
                return this.screen;
            }
        }

        public IPanelSpecification PanelSpecification
        {
            get
            {
                return this.screen;
            }
        }

        public Stream GetAssetStream (String assetId)
        {
            throw new NotImplementedException ();
        }

    }

    public sealed class TouchScreen
        : IMultiTouchController
        , IPanelSpecification
        , IScreenSpecification
    {
        readonly Dictionary<Int32, iOSTouchState> touchData;
        readonly MonoTouch.UIKit.UIView view;
        readonly TouchCollection collection = new TouchCollection();
        readonly ICor engine;

        internal TouchScreen(
            ICor engine,
            MonoTouch.UIKit.UIView view,
            Dictionary<Int32, iOSTouchState> touches)
        {
            this.view = view;
            this.engine = engine;
            this.touchData = touches;

            Console.WriteLine(string.Format("Screen Specification - Width: {0}, Height: {1}", ScreenResolutionWidth, ScreenResolutionHeight));
        }

        public IPanelSpecification PanelSpecification
        {
            get
            {
                return this;
            }
        }

        internal void Update(AppTime time)
        {
            //Console.WriteLine(string.Format("MonoTouch.UIKit.UIScreen.MainScreen.Bounds - h: {0}, w: {1}", ScreenResolutionWidth, ScreenResolutionHeight));

            // seems to be a problem with mono touch reporting a new touch with
            // the same id across multiple frames.
            List<Int32> touchIDsLastFrame = new List<int>();

            foreach(var touch in this.collection)
            {
                touchIDsLastFrame.Add(touch.ID);
            }

            this.collection.ClearBuffer();


            foreach (var key in touchData.Keys)
            {
                var uiKitTouch = touchData[key];
                System.Drawing.PointF location = uiKitTouch.Location;

                Int32 id = uiKitTouch.Handle;

                Vector2 pos = new Vector2(location.X, location.Y);

                //Console.WriteLine(string.Format("UIKitTouch - id: {0}, pos: {1}", id, pos));

                // todo: this needs to be current display res, not just the screen specs


                pos.X = pos.X / engine.System.CurrentDisplaySize.X;
                pos.Y = pos.Y / engine.System.CurrentDisplaySize.Y;

                pos -= new Vector2(0.5f, 0.5f);

                pos.Y = -pos.Y;

                var state = EnumConverter.ToCorPrimitiveType(uiKitTouch.Phase);

                if( touchIDsLastFrame.Contains(id) )
                {
                    if( state == TouchPhase.JustPressed )
                    {
                        //Core.Teletype.WriteLine("ignoring " + id);

                        state = TouchPhase.Active;
                    }
                }

                if( state == TouchPhase.JustPressed )
                {
                    Console.WriteLine(string.Format("Touch - id: {0}, pos: {1}", id, pos));
                }

                this.collection.RegisterTouch(id, pos, state, time.FrameNumber, time.Elapsed);
            }
        }



        public Vector2 PanelPhysicalSize
        {
            get
            {
                // do lookup here into all device types
                //MonoTouch.ObjCRuntime.
                return new Vector2(0.0768f, 0.1024f);
            }
        }

        public float PanelPhysicalAspectRatio
        {
            get
            {
                return PanelPhysicalSize.X / PanelPhysicalSize.Y;
            }
        }
        public PanelType PanelType
        {
            get
            {
                return PanelType.TouchScreen;
            }
        }


        public float ScreenResolutionAspectRatio
        {
            get
            {
                return this.ScreenResolutionWidth / this.ScreenResolutionHeight;
            }
        }

        // need to think about
        public Single PixelDensity
        {
            get
            {
                return 1f;
            }
            set
            {
                ;
            }
        }

        public Int32 ScreenResolutionHeight
        {
            get
            {
                return (Int32) (
                    MonoTouch.UIKit.UIScreen.MainScreen.Bounds.Height *
                    MonoTouch.UIKit.UIScreen.MainScreen.Scale);
            }
        }

        public Int32 ScreenResolutionWidth
        {
            get
            {
                return (Int32) (
                    MonoTouch.UIKit.UIScreen.MainScreen.Bounds.Width *
                    MonoTouch.UIKit.UIScreen.MainScreen.Scale);
            }
        }

        public TouchCollection TouchCollection
        {
            get { return this.collection; }
        }
    }

    public sealed class CorAppEngine
    {
        OpenGLViewController viewController;
        MonoTouch.UIKit.UIWindow window;

        readonly AppSettings settings;
        readonly IApp game;

        public CorAppEngine (AppSettings settings, IApp game)
        {
            this.settings = settings;
            this.game = game;
        }

        public void Run()
        {  
            UIApplication.SharedApplication.StatusBarHidden = true;

            // create a new window instance based on the screen size
            window = new MonoTouch.UIKit.UIWindow (
                MonoTouch.UIKit.UIScreen.MainScreen.Bounds);

            viewController = new OpenGLViewController (this.settings, this.game);

            window.RootViewController = viewController;

            // make the window visible
            window.MakeKeyAndVisible ();
        }
    }

    #region OpenGL ES Shaders

    public class OglesShader
        : IDisposable
    {
        public List<OglesShaderInput> Inputs { get; private set; }
        public List<OglesShaderVariable> Variables { get; private set; }
        public List<OglesShaderSampler> Samplers { get; private set; }

        internal string VariantName { get { return variantName; }}
        Int32 programHandle;
        Int32 fragShaderHandle;
        Int32 vertShaderHandle;

        // for debugging
        string variantName;
        string passName;

        string pixelShaderPath;
        string vertexShaderPath;

        public override string ToString ()
        {
            //string a = Inputs.Select(x => x.Name).Join(", ");
            //string b = Variables.Select(x => x.Name).Join(", ");

            string a = string.Empty;

            for(int i = 0; i < Inputs.Count; ++i)
            { 
                a += Inputs[i].Name; if( i + 1 < Inputs.Count ) { a += ", "; } 
            }

            string b = string.Empty;
            for(int i = 0; i < Variables.Count; ++i)
            { 
                b += Variables[i].Name; if( i + 1 < Variables.Count ) { b += ", "; } 
            }

            return string.Format (
                "[OglesShader: Variant {0}, Pass {1}: Inputs: [{2}], Variables: [{3}]]", 
                variantName, 
                passName, 
                a, 
                b);
        }

        internal void ValidateInputs(List<ShaderInputDefinition> definitions)
        {
            Console.WriteLine(string.Format ("Pass: {1} => ValidateInputs({0})", variantName, passName ));

            // Make sure that this shader implements all of the non-optional defined inputs.
            var nonOptionalDefinitions = definitions.Where(y => !y.Optional).ToList();

            foreach(var definition in nonOptionalDefinitions)
            {
                var find = Inputs.Find(x => x.Name == definition.Name/* && x.Type == definition.Type */);

                if( find == null )
                {
                    throw new Exception("problem");
                }
            }

            // Make sure that every implemented input is defined.
            foreach(var input in Inputs)
            {
                var find = definitions.Find(x => x.Name == input.Name 
                    /*&& (x.Type == input.Type || (x.Type == typeof(Rgba32) && input.Type == typeof(Vector4)))*/
                    );

                if( find == null )
                {
                    throw new Exception("problem");
                }
                else
                {
                    input.RegisterExtraInfo(find);
                }
            }
        }

        internal void ValidateVariables(List<ShaderVariableDefinition> definitions)
        {
            Console.WriteLine(string.Format ("Pass: {1} => ValidateVariables({0})", variantName, passName ));


            // Make sure that every implemented input is defined.
            foreach(var variable in Variables)
            {
                var find = definitions.Find(
                    x => 
                    x.Name == variable.Name //&& 
                    //(x.Type == variable.Type || (x.Type == typeof(Rgba32) && variable.Type == typeof(Vector4)))
                    );
                
                if( find == null )
                {
                    throw new Exception("problem");
                }
                else
                {
                    variable.RegisterExtraInfo(find);
                }
            }
        }

        internal void ValidateSamplers(List<ShaderSamplerDefinition> definitions)
        {
            Console.WriteLine(string.Format ("Pass: {1} => ValidateSamplers({0})", variantName, passName ));

            var nonOptionalSamplers = definitions.Where(y => !y.Optional).ToList();

            foreach(var definition in nonOptionalSamplers)
            {
                var find = this.Samplers.Find(x => x.Name == definition.Name);

                if( find == null )
                {
                    throw new Exception("problem");
                }
            }

            // Make sure that every implemented input is defined.
            foreach(var sampler in this.Samplers)
            {
                var find = definitions.Find(x => x.Name == sampler.Name);

                if( find == null )
                {
                    throw new Exception("problem");
                }
                else
                {
                    sampler.RegisterExtraInfo(find);
                }
            }
        }

        /*
        static void CheckVariableCompatibility(List<OglesShaderVariable> definedVariables )
        {
            throw new NotImplementedException();
        }
        
        static void CheckInputCompatibility(List<OglesShaderInput> definedInputs, Dictionary<string, OpenTK.Graphics.ES20.ActiveAttribType> actualAttributes )
        {
            // make sure that the shader we just loaded will work with this shader definition   
            if( actualAttributes.Count != definedInputs.Count )
            {
                throw new Exception("shader doesn't implement definition");
            }
        
            foreach( var key in actualAttributes.Keys )
            {
                var item = definedInputs.Find(x => x.Name == key);
                
                if( item == null )
                {
                    throw new Exception("shader doesn't implement definition - missing variable");
                }
                
                if( item.Type != EnumConverter.ToType( actualAttributes[key] ) )
                {
                    throw new Exception("shader doesn't implement definition - variable is of the wrong type");
                }
            }
        }
        */

        static string GetResourcePath(string path)
        {
            string ext = Path.GetExtension(path);

            string filename = path.Substring(0, path.Length - ext.Length);

            var resourcePathname =
                MonoTouch.Foundation.NSBundle.MainBundle.PathForResource (
                    filename,
                    ext.Substring(1, ext.Length - 1)
                );

            if( resourcePathname == null )
            {
                throw new Exception("Resource [" + path + "] not found");
            }

            return resourcePathname;
        }

        internal OglesShader(String variantName, String passName, OglesShaderDefinition definition)
        {
            Console.WriteLine ("  Creating Pass Variant: " + variantName);
            this.variantName = variantName;
            this.passName = passName;
            this.vertexShaderPath = definition.VertexShaderPath;
            this.pixelShaderPath = definition.PixelShaderPath;
            
            //Variables = 
            programHandle = ShaderUtils.CreateShaderProgram ();
            vertShaderHandle = ShaderUtils.CreateVertexShader (GetResourcePath(this.vertexShaderPath));
            fragShaderHandle = ShaderUtils.CreateFragmentShader (GetResourcePath(this.pixelShaderPath));
            
            ShaderUtils.AttachShader (programHandle, vertShaderHandle);
            ShaderUtils.AttachShader (programHandle, fragShaderHandle);

        }

        internal void BindAttributes(IList<String> orderedAttributes)
        {
            int index = 0;

            foreach(var attName in orderedAttributes)
            {
                OpenTK.Graphics.ES20.GL.BindAttribLocation(programHandle, index, attName);
                ErrorHandler.Check();
                bool success = ShaderUtils.LinkProgram (programHandle);
                if (success)
                {
                    index++;
                }

            }
        }

        internal void Link()
        {
            // bind atts here
            //ShaderUtils.LinkProgram (programHandle);

            Console.WriteLine("  Finishing linking");

            Console.WriteLine("  Initilise Attributes");
            var attributes = ShaderUtils.GetAttributes(programHandle);

            Inputs = attributes
                .Select(x => new OglesShaderInput(programHandle, x))
                .OrderBy(y => y.AttributeLocation)
                .ToList();
            Console.Write("  Inputs : ");
            foreach (var input in Inputs) {
                Console.Write (input.Name + ", ");
            }
            Console.Write (Environment.NewLine);

            Console.WriteLine("  Initilise Uniforms");
            var uniforms = ShaderUtils.GetUniforms(programHandle);


            Variables = uniforms
                .Where(y => 
                       y.Type != OpenTK.Graphics.ES20.ActiveUniformType.Sampler2D && 
                       y.Type != OpenTK.Graphics.ES20.ActiveUniformType.SamplerCube)
                .Select(x => new OglesShaderVariable(programHandle, x))
                .OrderBy(z => z.UniformLocation)
                .ToList();
            Console.Write("  Variables : ");
            foreach (var variable in Variables) {
                Console.Write (variable.Name + ", ");
            }
            Console.Write (Environment.NewLine);

            Console.WriteLine("  Initilise Samplers");
            Samplers = uniforms
                .Where(y => 
                       y.Type == OpenTK.Graphics.ES20.ActiveUniformType.Sampler2D || 
                       y.Type == OpenTK.Graphics.ES20.ActiveUniformType.SamplerCube)
                .Select(x => new OglesShaderSampler(programHandle, x))
                .OrderBy(z => z.UniformLocation)
                .ToList();

            #if DEBUG
            ShaderUtils.ValidateProgram (programHandle);
            #endif
            
            ShaderUtils.DetachShader(programHandle, fragShaderHandle);
            ShaderUtils.DetachShader(programHandle, vertShaderHandle);
            
            ShaderUtils.DeleteShader(programHandle, fragShaderHandle);
            ShaderUtils.DeleteShader(programHandle, vertShaderHandle);
        }
        
        public void Activate ()
        {
            OpenTK.Graphics.ES20.GL.UseProgram (programHandle);
            ErrorHandler.Check ();
        }
        
        public void Dispose()
        {
            ShaderUtils.DestroyShaderProgram(programHandle);
            ErrorHandler.Check();
        }
    }
    
    public class OglesShaderDefinition
    {
        public string VertexShaderPath { get; set; }
        public string PixelShaderPath { get; set; }
    }

    /// <summary>
    /// Represents an Open GL ES shader input, all the data is read dynamically from
    /// the shader at runtime, not from the ShaderInputDefinition.  This way we can compare the
    /// two and check to see that we have what we are expecting.
    /// </summary>
    public class OglesShaderInput
    {
        int ProgramHandle { get; set; }
        internal int AttributeLocation { get; private set; }
        
        public String Name { get; private set; }
        public Type Type { get; private set; }
        public VertexElementUsage Usage { get; private set; }
        public Object DefaultValue { get; private set; }
        public Boolean Optional { get; private set; }
        
        public OglesShaderInput(
            int programHandle, ShaderUtils.ShaderAttribute attribute)
        {
            int attLocation = OpenTK.Graphics.ES20.GL.GetAttribLocation(programHandle, attribute.Name);

            ErrorHandler.Check();

            Console.WriteLine(string.Format(
                "    Binding Shader Input: [Prog={0}, AttIndex={1}, AttLocation={4}, AttName={2}, AttType={3}]",
                programHandle, attribute.Index, attribute.Name, attribute.Type, attLocation));

            this.ProgramHandle = programHandle;
            this.AttributeLocation = attLocation;
            this.Name = attribute.Name;
            this.Type = Cor.Lib.Khronos.EnumConverter.ToType(attribute.Type);
            

        }
        
        internal void RegisterExtraInfo(ShaderInputDefinition definition)
        {
            Usage = definition.Usage;
            DefaultValue = definition.DefaultValue;
            Optional = definition.Optional;
        }   
    }

    public class OglesShaderSampler
    {
        int ProgramHandle { get; set; }
        internal int UniformLocation { get; private set; }

        public String NiceName { get; set; }
        public String Name { get; set; }

        public OglesShaderSampler(
            int programHandle, ShaderUtils.ShaderUniform uniform )
        {
            this.ProgramHandle = programHandle;

            int uniformLocation = OpenTK.Graphics.ES20.GL.GetUniformLocation(programHandle, uniform.Name);

            ErrorHandler.Check();


            this.UniformLocation = uniformLocation;
            this.Name = uniform.Name;
        }

        internal void RegisterExtraInfo(ShaderSamplerDefinition definition)
        {
            NiceName = definition.NiceName;
        }

        public void SetSlot(Int32 slot)
        {
            // set the sampler texture unit to 0
            OpenTK.Graphics.ES20.GL.Uniform1( this.UniformLocation, slot );
            ErrorHandler.Check();
        }

    }

    public class OglesShaderVariable
    {
        int ProgramHandle { get; set; }
        internal int UniformLocation { get; private set; }
        
        public String NiceName { get; private set; }
        public String Name { get; private set; }
        public Type Type { get; private set; }
        public Object DefaultValue { get; private set; }
        
        public OglesShaderVariable(
            int programHandle, ShaderUtils.ShaderUniform uniform)
        {

            this.ProgramHandle = programHandle;

            int uniformLocation = OpenTK.Graphics.ES20.GL.GetUniformLocation(programHandle, uniform.Name);

            ErrorHandler.Check();

            if( uniformLocation == -1 )
                throw new Exception();
                
            this.UniformLocation = uniformLocation;
            this.Name = uniform.Name;
            this.Type = Cor.Lib.Khronos.EnumConverter.ToType(uniform.Type);

            Console.WriteLine(string.Format(
                "    Caching Reference to Shader Variable: [Prog={0}, UniIndex={1}, UniLocation={2}, UniName={3}, UniType={4}]",
                programHandle, uniform.Index, uniformLocation, uniform.Name, uniform.Type));

        }
        
        internal void RegisterExtraInfo(ShaderVariableDefinition definition)
        {
            NiceName = definition.NiceName;
            DefaultValue = definition.DefaultValue;
        }
        
        public void Set(object value)
        {
            //todo this should be using convert turn the data into proper opengl es types.
            Type t = value.GetType();
            
            if( t == typeof(Matrix44) )
            {
                var castValue = (Matrix44) value;
                var otkValue = Matrix44Converter.ToKhronos(castValue);
                OpenTK.Graphics.ES20.GL.UniformMatrix4( UniformLocation, false, ref otkValue );
            }
            else if( t == typeof(Int32) )
            {
                var castValue = (Int32) value;
                OpenTK.Graphics.ES20.GL.Uniform1( UniformLocation, 1, ref castValue );
            }
            else if( t == typeof(Single) )
            {
                var castValue = (Single) value;
                OpenTK.Graphics.ES20.GL.Uniform1( UniformLocation, 1, ref castValue );
            }
            else if( t == typeof(Vector2) )
            {
                var castValue = (Vector2) value;
                OpenTK.Graphics.ES20.GL.Uniform2( UniformLocation, 1, ref castValue.X );
            }
            else if( t == typeof(Vector3) )
            {
                var castValue = (Vector3) value;
                OpenTK.Graphics.ES20.GL.Uniform3( UniformLocation, 1, ref castValue.X );
            } 
            else if( t == typeof(Vector4) )
            {
                var castValue = (Vector4) value;
                OpenTK.Graphics.ES20.GL.Uniform4( UniformLocation, 1, ref castValue.X );
            }
            else if( t == typeof(Rgba32) )
            {
                var castValue = (Rgba32) value;
                
                Vector4 vec4Value;
                castValue.UnpackTo(out vec4Value);
                
                // does this rgba value need to be packed in to a vector3 or a vector4
                if( this.Type == typeof(Vector4) )
                    OpenTK.Graphics.ES20.GL.Uniform4( UniformLocation, 1, ref vec4Value.X );
                else if( this.Type == typeof(Vector3) )
                    OpenTK.Graphics.ES20.GL.Uniform3( UniformLocation, 1, ref vec4Value.X );
                else
                    throw new Exception("Not supported");
            }
            else
            {
                throw new Exception("Not supported");
            }
            
            ErrorHandler.Check();

        }
    }


    #endregion

    #region Shader Definitions

    public class ShaderInputDefinition
    {
        public String Name { get; set; }
        public Type Type { get; set; }
        public VertexElementUsage Usage { get; set; }
        public Object DefaultValue { get; set; }
        public Boolean Optional { get; set; }
    }

    public class ShaderSamplerDefinition
    {
        public String NiceName { get; set; }
        public String Name { get; set; }
        public Boolean Optional { get; set; }
    }

    public class ShaderVariableDefinition
    {
        public String NiceName { get; set; }

        String name;
        public String Name
        { 
            get { return name; }
            set { 
                if (value.Length > 16)
                    name = value.Substring (0, 16);
                else
                    name = value;
            }
        }
        public Type Type { get; set; }
        public Object DefaultValue { get; set; }
    }

    public class ShaderVariantDefinition
    {
        public string VariantName { get; set; }
        public List<ShaderVarientPassDefinition> VariantPassDefinitions { get; set; }
    }

    public class ShaderVarientPassDefinition
    {
        public string PassName { get; set; }
        public OglesShaderDefinition PassDefinition { get; set; }
    }


    #endregion

    #region Shader Definitions

    public static partial class CorShaders
    {   
        public static IShader CreatePhongPixelLit()
        {
            var parameter = new ShaderDefinition()
            {
                Name = "PixelLit",
                PassNames = new List<string>() { "Main" },
                InputDefinitions = new List<ShaderInputDefinition>()
                {
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertPos",
                        Type = typeof(Vector3),
                        Usage = VertexElementUsage.Position,
                        DefaultValue = Vector3.Zero,
                        Optional = false,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertNormal",
                        Type = typeof(Vector3),
                        Usage = VertexElementUsage.Normal,
                        DefaultValue = Vector3.Zero,
                        Optional = false,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertTexcoord",
                        Type = typeof(Vector2),
                        Usage = VertexElementUsage.TextureCoordinate,
                        DefaultValue = Vector2.Zero,
                        Optional = true,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertColour",
                        Type = typeof(Rgba32),
                        Usage = VertexElementUsage.Colour,
                        DefaultValue = Rgba32.White,
                        Optional = true,
                    },
                },
                SamplerDefinitions = new List<ShaderSamplerDefinition>()
                {
                    new ShaderSamplerDefinition()
                    {
                        NiceName = "TextureSampler",
                        Name = "s_tex0",
                        Optional = true,
                    }
                },
                VariableDefinitions = new List<ShaderVariableDefinition>()
                {
                    new ShaderVariableDefinition()
                    {
                        NiceName = "World",
                        Name = "u_world",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "View",
                        Name = "u_view",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "Projection",
                        Name = "u_proj",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "MaterialColour",
                        Name = "u_colour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.White,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "AmbientLightColour",
                        Name = "u_liAmbient",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Grey,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "EmissiveColour",
                        Name = "u_emissiveColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Black,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "SpecularColour",
                        Name = "u_specularColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.White,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "SpecularPower",
                        Name = "u_specularPower",
                        Type = typeof(Single),
                        DefaultValue = 0.7f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "EyePosition",
                        Name = "u_eyePosition",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Zero,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogEnabled",
                        Name = "u_fogEnabled",
                        Type = typeof(Single),
                        DefaultValue = 1f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogStart",
                        Name = "u_fogStart",
                        Type = typeof(Single),
                        DefaultValue = 100f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogEnd",
                        Name = "u_fogEnd",
                        Type = typeof(Single),
                        DefaultValue = 1000f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogColour",
                        Name = "u_fogColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Blue,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0Direction",
                        Name = "u_li0Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0DiffuseColour",
                        Name = "u_li0Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0SpecularColour",
                        Name = "u_li0Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1Direction",
                        Name = "u_li1Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1DiffuseColour",
                        Name = "u_li1Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1SpecularColour",
                        Name = "u_li1Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2Direction",
                        Name = "u_li2Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2DiffuseColour",
                        Name = "u_li2Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2SpecularColour",
                        Name = "u_li2Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                },
                VariantDefinitions = new List<ShaderVariantDefinition>()
                {
                    new ShaderVariantDefinition()
                    {
                        VariantName = "PixelLit_PositionNormal",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/PixelLit_PositionNormal.vsh",
                                    PixelShaderPath = "shaders/PixelLit_PositionNormal.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "PixelLit_PositionNormalTexture",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/PixelLit_PositionNormalTexture.vsh",
                                    PixelShaderPath = "shaders/PixelLit_PositionNormalTexture.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "PixelLit_PositionNormalColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/PixelLit_PositionNormalColour.vsh",
                                    PixelShaderPath = "shaders/PixelLit_PositionNormalColour.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "PixelLit_PositionNormalTextureColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/PixelLit_PositionNormalTextureColour.vsh",
                                    PixelShaderPath = "shaders/PixelLit_PositionNormalTextureColour.fsh",
                                },
                            },
                        },
                    },
                },
            };

            var s = new Shader (parameter);

            Console.WriteLine(s);

            return s;
        }
    }

    public static partial class CorShaders
    {   
        public static IShader CreatePhongVertexLit()
        {
            var parameter = new ShaderDefinition()
            {
                Name = "VertexLit",
                PassNames = new List<string>() { "Main" },
                InputDefinitions = new List<ShaderInputDefinition>()
                {
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertPos",
                        Type = typeof(Vector3),
                        Usage = VertexElementUsage.Position,
                        DefaultValue = Vector3.Zero,
                        Optional = false,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertNormal",
                        Type = typeof(Vector3),
                        Usage = VertexElementUsage.Normal,
                        DefaultValue = Vector3.Zero,
                        Optional = false,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertTexcoord",
                        Type = typeof(Vector2),
                        Usage = VertexElementUsage.TextureCoordinate,
                        DefaultValue = Vector2.Zero,
                        Optional = true,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertColour",
                        Type = typeof(Rgba32),
                        Usage = VertexElementUsage.Colour,
                        DefaultValue = Rgba32.White,
                        Optional = true,
                    },
                },
                SamplerDefinitions = new List<ShaderSamplerDefinition>()
                {
                    new ShaderSamplerDefinition()
                    {
                        NiceName = "TextureSampler",
                        Name = "s_tex0",
                        Optional = true,
                    }
                },
                VariableDefinitions = new List<ShaderVariableDefinition>()
                {
                    new ShaderVariableDefinition()
                    {
                        NiceName = "World",
                        Name = "u_world",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "View",
                        Name = "u_view",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "Projection",
                        Name = "u_proj",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "MaterialColour",
                        Name = "u_colour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.White,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "AmbientLightColour",
                        Name = "u_liAmbient",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Grey,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "EmissiveColour",
                        Name = "u_emissiveColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Black,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "SpecularColour",
                        Name = "u_specularColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.White,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "SpecularPower",
                        Name = "u_specularPower",
                        Type = typeof(Single),
                        DefaultValue = 0.7f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "EyePosition",
                        Name = "u_eyePosition",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Zero,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogEnabled",
                        Name = "u_fogEnabled",
                        Type = typeof(Single),
                        DefaultValue = 1f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogStart",
                        Name = "u_fogStart",
                        Type = typeof(Single),
                        DefaultValue = 100f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogEnd",
                        Name = "u_fogEnd",
                        Type = typeof(Single),
                        DefaultValue = 1000f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogColour",
                        Name = "u_fogColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Blue,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0Direction",
                        Name = "u_li0Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0DiffuseColour",
                        Name = "u_li0Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0SpecularColour",
                        Name = "u_li0Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1Direction",
                        Name = "u_li1Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1DiffuseColour",
                        Name = "u_li1Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1SpecularColour",
                        Name = "u_li1Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2Direction",
                        Name = "u_li2Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2DiffuseColour",
                        Name = "u_li2Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2SpecularColour",
                        Name = "u_li2Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                },
                VariantDefinitions = new List<ShaderVariantDefinition>()
                {
                    new ShaderVariantDefinition()
                    {
                        VariantName = "VertexLit_PositionNormal",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/VertexLit_PositionNormal.vsh",
                                    PixelShaderPath = "shaders/VertexLit_PositionNormal.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "VertexLit_PositionNormalTexture",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/VertexLit_PositionNormalTexture.vsh",
                                    PixelShaderPath = "shaders/VertexLit_PositionNormalTexture.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "VertexLit_PositionNormalColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/VertexLit_PositionNormalColour.vsh",
                                    PixelShaderPath = "shaders/VertexLit_PositionNormalColour.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "VertexLit_PositionNormalTextureColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/VertexLit_PositionNormalTextureColour.vsh",
                                    PixelShaderPath = "shaders/VertexLit_PositionNormalTextureColour.fsh",
                                },
                            },
                        },
                    },
                },
            };


            var s = new Shader (parameter);

            Console.WriteLine(s);

            return s;
        }
    }

    public static partial class CorShaders
    {   
        public static IShader CreateUnlit()
        {
            var parameter = new ShaderDefinition()
            {
                Name = "Unlit",
                PassNames = new List<string>() { "Main" },
                InputDefinitions = new List<ShaderInputDefinition>()
                {
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertPos",
                        Type = typeof(Vector3),
                        Usage = VertexElementUsage.Position,
                        DefaultValue = Vector3.Zero,
                        Optional = false,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertTexcoord",
                        Type = typeof(Vector2),
                        Usage = VertexElementUsage.TextureCoordinate,
                        DefaultValue = Vector2.Zero,
                        Optional = true,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertColour",
                        Type = typeof(Rgba32),
                        Usage = VertexElementUsage.Colour,
                        DefaultValue = Rgba32.White,
                        Optional = true,
                    },
                },
                SamplerDefinitions = new List<ShaderSamplerDefinition>()
                {
                    new ShaderSamplerDefinition()
                    {
                        NiceName = "TextureSampler",
                        Name = "s_tex0",
                        Optional = true,
                    }
                },
                VariableDefinitions = new List<ShaderVariableDefinition>()
                {
                    new ShaderVariableDefinition()
                    {
                        NiceName = "MaterialColour",
                        Name = "u_colour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.White,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "World",
                        Name = "u_world",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "View",
                        Name = "u_view",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "Projection",
                        Name = "u_proj",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                },
                VariantDefinitions = new List<ShaderVariantDefinition>()
                {
                    new ShaderVariantDefinition()
                    {
                        VariantName = "Unlit_Position",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/Unlit_Position.vsh",
                                    PixelShaderPath = "shaders/Unlit_Position.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "Unlit_PositionTexture",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/Unlit_PositionTexture.vsh",
                                    PixelShaderPath = "shaders/Unlit_PositionTexture.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "Unlit_PositionColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/Unlit_PositionColour.vsh",
                                    PixelShaderPath = "shaders/Unlit_PositionColour.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "Unlit_PositionTextureColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/Unlit_PositionTextureColour.vsh",
                                    PixelShaderPath = "shaders/Unlit_PositionTextureColour.fsh",
                                },
                            },
                        },
                    },
                },
            };


            var s = new Shader (parameter);

            Console.WriteLine(s);

            return s;
        }
    }


    #endregion
}
