// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! Xamarin iOS Platform Implementation                                                                       │ \\
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

namespace Cor.Platform.Xios
{
    using global::System;
    using global::System.Globalization;
    using global::System.Collections;
    using global::System.Collections.ObjectModel;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Drawing;
    using global::System.Diagnostics;
    using global::System.Runtime.ConstrainedExecution;
    using global::System.Runtime.InteropServices;
    using global::System.Linq;

    using Fudge;
    using Abacus.SinglePrecision;

    using Cor.Lib.Khronos;
    using Cor.Platform.Stub;

    using MonoTouch.UIKit;
    using MonoTouch.Foundation;
    using MonoTouch.CoreText;
    using MonoTouch.CoreGraphics;


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class AudioManager
        : AudioBase
    {
        public override Single Volume { get; set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class EnumConverter
    {
        public static DeviceOrientation ToCor (MonoTouch.UIKit.UIDeviceOrientation monoTouch)
        {
            switch (monoTouch)
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

        public static TouchPhase ToCorPrimitiveType (MonoTouch.UIKit.UITouchPhase phase)
        {
            switch (phase)
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


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class Vector2Converter
    {
		public static global::System.Drawing.PointF ToSystemDrawing (this Vector2 vec)
        {
			return new global::System.Drawing.PointF (vec.X, vec.Y);
        }

		public static Vector2 ToAbacus (this global::System.Drawing.PointF vec)
        {
            return new Vector2 (vec.X, vec.Y);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class Status
		: StatusBase
    {
		public override Boolean? Fullscreen
        {
            get
            {
                // always fullscreen on iOS
                return true;
            }
        }

		public override Int32 Width
        {
            get
            {
                return (Int32) MonoTouch.UIKit.UIScreen.MainScreen.CurrentMode.Size.Width;
            }
        }

		public override Int32 Height
        {
            get
            {
                return (Int32) MonoTouch.UIKit.UIScreen.MainScreen.CurrentMode.Size.Height;
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    [MonoTouch.Foundation.Register ("EAGLView")]
    public sealed class EAGLView 
        : OpenTK.Platform.iPhoneOS.iPhoneOSGameView
    {
        AppSettings settings;
        IApp game;

        Engine gameEngine;
        Stopwatch timer = new Stopwatch ();
        Single elapsedTime;
        Int64 frameCounter = -1;
        TimeSpan previousTimeSpan;
        Int32 frameInterval;

        MonoTouch.CoreAnimation.CADisplayLink displayLink;

        uint _depthRenderbuffer;

        Dictionary<Int32, iOSTouchState> touchState = new Dictionary<int, iOSTouchState>();

		public global::System.Boolean IsAnimating 
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

		public EAGLView (global::System.Drawing.RectangleF frame)
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

        protected override void CreateFrameBuffer ()
        {
            base.CreateFrameBuffer ();

            //
            // Enable the depth buffer
            //
            OpenTK.Graphics.ES20.GL.GenRenderbuffers (1, out _depthRenderbuffer);
            KrErrorHandler.Check ();

            OpenTK.Graphics.ES20.GL.BindRenderbuffer (
                OpenTK.Graphics.ES20.RenderbufferTarget.Renderbuffer,
                _depthRenderbuffer);
            KrErrorHandler.Check ();

            OpenTK.Graphics.ES20.GL.RenderbufferStorage (
                OpenTK.Graphics.ES20.RenderbufferTarget.Renderbuffer, 
                OpenTK.Graphics.ES20.RenderbufferInternalFormat.DepthComponent16, 
                Size.Width, 
                Size.Height);
            KrErrorHandler.Check ();

            OpenTK.Graphics.ES20.GL.FramebufferRenderbuffer (
                OpenTK.Graphics.ES20.FramebufferTarget.Framebuffer,
                OpenTK.Graphics.ES20.FramebufferSlot.DepthAttachment,
                OpenTK.Graphics.ES20.RenderbufferTarget.Renderbuffer,
                _depthRenderbuffer);
            KrErrorHandler.Check ();
        }
        
        public void SetEngineDetails (AppSettings settings, IApp game)
        {
            this.settings = settings;
            this.game = game;
        }

        void CreateEngine ()
        {
            gameEngine = new Engine (
                this.settings,
                this.game,
                this, 
                this.GraphicsContext, 
                this.touchState);
            timer.Start ();
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

            CreateEngine ();

            displayLink = 
                MonoTouch.UIKit.UIScreen.MainScreen.CreateDisplayLink (
                    this, 
                    new MonoTouch.ObjCRuntime.Selector ("drawFrame")
                    );

            displayLink.FrameInterval = frameInterval;
            displayLink.AddToRunLoop (
                MonoTouch.Foundation.NSRunLoop.Current, 
                MonoTouch.Foundation.NSRunLoop.NSDefaultRunLoopMode);
            
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
            OnUpdateFrame (e);
            OnRenderFrame (e);
        }

        protected override void OnUpdateFrame (OpenTK.FrameEventArgs e)
        {
            base.OnUpdateFrame (e);

            this.ClearOldTouches ();

            Single dt = (Single)(timer.Elapsed.TotalSeconds - previousTimeSpan.TotalSeconds);
            previousTimeSpan = timer.Elapsed;
            
            if (dt > 0.5f)
            {
                dt = 0.0f;
            }

            elapsedTime += dt;

            var appTime = new AppTime (dt, elapsedTime, ++frameCounter);

            gameEngine.Update (appTime);
        }

        void ClearOldTouches ()
        {
            var keysToDitch = new List<Int32>();

            //remove stuff
            var keys = touchState.Keys;

            foreach (var key in keys)
            {
                var ts = touchState[key];

                if (ts.Phase == MonoTouch.UIKit.UITouchPhase.Cancelled ||
                    ts.Phase == MonoTouch.UIKit.UITouchPhase.Ended)
                {
                    if (ts.LastUpdated < this.frameCounter)
                    {
                        keysToDitch.Add (key);
                    }
                }
            }

            foreach (var key in keysToDitch)
            {
                touchState.Remove (key);
                
                //Console.WriteLine ("remove "+key);
            }
        }

        protected override void OnRenderFrame (OpenTK.FrameEventArgs e)
        {
            base.OnRenderFrame (e);

            base.MakeCurrent ();
            
            gameEngine.Render ();

            this.SwapBuffers ();
        }

        public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
        {
            ProcessTouchChange (touches);

            base.TouchesBegan (touches, evt);
        }

        public override void TouchesMoved (MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
        {
            ProcessTouchChange (touches);

            base.TouchesMoved (touches, evt);
        }

        public override void TouchesCancelled (MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
        {
            ProcessTouchChange (touches);

            base.TouchesCancelled (touches, evt);
        }

        public override void TouchesEnded (MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
        {
            ProcessTouchChange (touches);

            base.TouchesEnded (touches, evt);
        }

        void ProcessTouchChange (MonoTouch.Foundation.NSSet touches)
        {
            var touchesArray = touches.ToArray<MonoTouch.UIKit.UITouch> ();

            for (int i = 0; i < touchesArray.Length; ++i) 
            {
                var touch = touchesArray [i];

                //Get position touch
                var location = touch.LocationInView (this);
                var id = touch.Handle.ToInt32 ();
                var phase = touch.Phase;

                var ts = new iOSTouchState ();
                ts.Handle = id;
                ts.LastUpdated = this.frameCounter;
                ts.Location = location;
                ts.Phase = phase;

                if (phase == MonoTouch.UIKit.UITouchPhase.Began)
                {
                    //Console.WriteLine ("add "+id);
                    touchState.Add (id, ts);
                }
                else
                {
                    if (touchState.ContainsKey (id) )
                    {
                        touchState[id] = ts;

                        if (ts.Phase == MonoTouch.UIKit.UITouchPhase.Began)
                        {
                            ts.Phase = MonoTouch.UIKit.UITouchPhase.Stationary;
                        }

                    }
                    else
                    {
                        throw new Exception ("eerrr???");
                    }
                }
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class Engine
        : EngineBase
    {
        readonly TouchScreen touchScreen;
        readonly AppSettings settings;
        readonly IApp app;
        readonly Graphics graphics;
        readonly InputManager input;
		readonly Host host;
        readonly AudioManager audio;
        readonly Status appStatus;
        readonly LogManager log;
        readonly Assets assets;

        internal Engine (
            AppSettings settings,
            IApp app,
            OpenTK.Platform.iPhoneOS.iPhoneOSGameView view,
            OpenTK.Graphics.IGraphicsContext gfxContext,
            Dictionary<Int32, iOSTouchState> touches)
        {
            this.settings = settings;

            this.app = app;

            this.graphics = new Graphics ();

            this.touchScreen = new TouchScreen (this, view, touches);

            this.host = new Host (touchScreen);

            this.input = new InputManager (this, this.touchScreen);

            this.appStatus = new Status ();

            this.log = new LogManager (this.settings.LogSettings);

			this.assets = new Assets (this.graphics);

			this.app.Start (this);

        }

        internal TouchScreen TouchScreenImplementation
        {
            get
            {
                return touchScreen;
            }
        }

        #region EngineBase

        public override AudioBase Audio { get { return this.audio; } }
        public override GraphicsBase Graphics { get { return this.graphics; } }
        public override InputBase Input { get { return this.input; } }
		public override HostBase Host { get { return this.host; } }
		public override StatusBase Status { get { return this.appStatus; } }
        public override LogManager Log { get { return this.log; } }
        public override AssetsBase Assets { get { return this.assets; } }
        public override AppSettings Settings { get { return this.settings; } }

        #endregion

        internal Boolean Update (AppTime time)
        {
            input.Update (time);
			return app.Update (this, time);
        }

        internal void Render ()
        {
			app.Render (this);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class InputManager
        : InputBase
    {
        readonly TouchScreen touchScreen;

        readonly IXbox360Gamepad stubXbox360Gamepad = new StubXbox360Gamepad ();
        readonly IPsmGamepad stubPsmGamepad = new StubPsmGamepad ();
        readonly IGenericGamepad stubGenericGamepad = new StubGenericGamepad ();
        readonly IKeyboard stubKeyboard = new StubKeyboard ();
        readonly IMouse stubMouse = new StubMouse ();

        internal void Update (AppTime time)
        {
            this.touchScreen.Update (time);
        }

        #region IInputManager

        public InputManager (EngineBase engine, TouchScreen touchScreen)
        {
            this.touchScreen = touchScreen;
        }

        public override IMultiTouchController MultiTouchController
        {
            get { return this.touchScreen; }
        }

        public override IXbox360Gamepad Xbox360Gamepad
        {
            get { return stubXbox360Gamepad; }
        }

        public override IPsmGamepad PsmGamepad
        {
            get { return stubPsmGamepad; }
        }

        public override IGenericGamepad GenericGamepad
        {
            get { return stubGenericGamepad; }
        }

        public override IMouse Mouse
        {
            get { return stubMouse; }
        }

        public override IKeyboard Keyboard
        {
            get { return stubKeyboard; }
        }

        #endregion
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal struct iOSTouchState
    {
        public Int32 Handle;
		public global::System.Drawing.PointF Location;
        public MonoTouch.UIKit.UITouchPhase Phase;
        public Int64 LastUpdated;
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

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

        public override void LoadView ()
        {
            //var size = MonoTouch.UIKit.UIScreen.MainScreen.CurrentMode.Size;
            //var frame = new System.Drawing.RectangleF (0, 0, size.Width, size.Height);
            var frame = MonoTouch.UIKit.UIScreen.MainScreen.Bounds;
            base.View = new EAGLView (frame);
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
            
            View.SetEngineDetails (_settings, _game);
        }
        
        protected override void Dispose (Boolean disposing)
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

        public override void DidRotate (MonoTouch.UIKit.UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate (fromInterfaceOrientation);
        }

        
        public override void ViewWillAppear (Boolean animated)
        {
            base.ViewWillAppear (animated);
            View.StartAnimating ();
        }
        
        public override void ViewWillDisappear (Boolean animated)
        {
            base.ViewWillDisappear (animated);
            View.StopAnimating ();
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class Host
		: HostBase
    {
        TouchScreen screen;

        public Host (TouchScreen screen)
        {
            this.screen = screen;

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.DidEnterBackgroundNotification, this.DidEnterBackground);

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.DidBecomeActiveNotification, this.DidBecomeActive);
            
            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.DidReceiveMemoryWarningNotification, this.DidReceiveMemoryWarning);

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.DidFinishLaunchingNotification, this.DidFinishLaunching);

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIDevice.OrientationDidChangeNotification, this.OrientationDidChange);

        }

        public void DidReceiveMemoryWarning (MonoTouch.Foundation.NSNotification ntf)
        {
            Console.WriteLine ("[Cor.System] DidReceiveMemoryWarning");
        }

        public void DidBecomeActive (MonoTouch.Foundation.NSNotification ntf)
        {
            Console.WriteLine ("[Cor.System] DidBecomeActive");
        }

        public void DidEnterBackground (MonoTouch.Foundation.NSNotification ntf)
        {
            Console.WriteLine ("[Cor.System] DidEnterBackground");
        }
        
        public void DidFinishLaunching (MonoTouch.Foundation.NSNotification ntf)
        {
            Console.WriteLine ("[Cor.System] DidFinishLaunching");
        }

        public void OrientationDidChange (MonoTouch.Foundation.NSNotification ntf)
        {
            Console.WriteLine (
                "[Cor.System] OrientationDidChange, CurrentOrientation: " + 
                CurrentOrientation.ToString () + ", CurrentDisplaySize: " + CurrentDisplaySize.ToString ());
        }

        public Point2 CurrentDisplaySize
        {
            get
            {
                Int32 w = ScreenSpecification.ScreenResolutionWidth;
                Int32 h = ScreenSpecification.ScreenResolutionHeight;

                GetEffectiveDisplaySize (ref w, ref h);

                return new Point2(w, h);
            }
        }

        void GetEffectiveDisplaySize (ref Int32 screenSpecWidth, ref Int32 screenSpecHeight)
        {
            if (this.CurrentOrientation == DeviceOrientation.Default ||
                this.CurrentOrientation == DeviceOrientation.Upsidedown)
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

		public override String Machine
		{
			get
			{
				return MonoTouch.UIKit.UIDevice.CurrentDevice.Model + 
					" : " + MonoTouch.UIKit.UIDevice.CurrentDevice.Name;
			}
		}

		public override String OperatingSystem
		{ 
			get 
			{ 
				return MonoTouch.UIKit.UIDevice.CurrentDevice.SystemName + 
					" : " + MonoTouch.UIKit.UIDevice.CurrentDevice.SystemVersion;
			}
		}

		public override String VirtualMachine { get { return "Mono ?"; } }

        public override DeviceOrientation CurrentOrientation
        {
            get
            {
                var monoTouchOrientation = MonoTouch.UIKit.UIDevice.CurrentDevice.Orientation;

                return EnumConverter.ToCor (monoTouchOrientation);
            }
        }

        public override IScreenSpecification ScreenSpecification
        {
            get
            {
                return this.screen;
            }
        }

        public override IPanelSpecification PanelSpecification
        {
            get
            {
                return this.screen;
            }
        }
	}


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

	public sealed class Assets
		: AssetsBase
	{
		public Assets (GraphicsBase gfx)
			: base (gfx)
		{
		}
		
        static string GetResourcePath (string path)
        {
            string ext = Path.GetExtension (path);

            string filename = path.Substring (0, path.Length - ext.Length);

            var resourcePathname =
                MonoTouch.Foundation.NSBundle.MainBundle.PathForResource (
                    filename,
                    ext.Substring (1, ext.Length - 1)
                );

            if (resourcePathname == null)
            {
                throw new Exception ("Resource [" + path + "] not found");
            }

            return resourcePathname;
        }
		
		#region AssetsBase

        public override Stream GetAssetStream (String assetId)
        {
            string path = GetResourcePath (Path.Combine ("assets/xios", assetId));

            var fStream = new FileStream (path, FileMode.Open);

            return fStream;
        }
		
		#endregion
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class TouchScreen
        : IMultiTouchController
        , IPanelSpecification
        , IScreenSpecification
    {
        readonly Dictionary<Int32, iOSTouchState> touchData;
        readonly MonoTouch.UIKit.UIView view;
        readonly TouchCollection collection = new TouchCollection ();
        readonly EngineBase engine;

        internal TouchScreen (
            EngineBase engine,
            MonoTouch.UIKit.UIView view,
            Dictionary<Int32, iOSTouchState> touches)
        {
            this.view = view;
            this.engine = engine;
            this.touchData = touches;

            Console.WriteLine (
                string.Format (
                    "Screen Specification - Width: {0}, Height: {1}", 
                    ScreenResolutionWidth, 
                    ScreenResolutionHeight));
        }

        public IPanelSpecification PanelSpecification
        {
            get { return this; }
        }

        internal void Update (AppTime time)
        {
            // seems to be a problem with mono touch reporting a new touch with
            // the same id across multiple frames.
            List<Int32> touchIDsLastFrame = new List<int>();

            foreach (var touch in this.collection)
            {
                touchIDsLastFrame.Add (touch.ID);
            }

            this.collection.ClearBuffer ();


            foreach (var key in touchData.Keys)
            {
                var uiKitTouch = touchData[key];
				global::System.Drawing.PointF location = uiKitTouch.Location;

                Int32 id = uiKitTouch.Handle;

                Vector2 pos = new Vector2(location.X, location.Y);

                //Console.WriteLine (string.Format ("UIKitTouch - id: {0}, pos: {1}", id, pos));

                // todo: this needs to be current display res, not just the screen specs

				pos.X = pos.X / engine.Status.Width;
				pos.Y = pos.Y / engine.Status.Height;

                pos -= new Vector2(0.5f, 0.5f);

                pos.Y = -pos.Y;

                var state = EnumConverter.ToCorPrimitiveType (uiKitTouch.Phase);

                if (touchIDsLastFrame.Contains (id) )
                {
                    if (state == TouchPhase.JustPressed)
                    {
                        //Core.Teletype.WriteLine ("ignoring " + id);

                        state = TouchPhase.Active;
                    }
                }

                if (state == TouchPhase.JustPressed)
                {
                    Console.WriteLine (string.Format ("Touch - id: {0}, pos: {1}", id, pos));
                }

                this.collection.RegisterTouch (id, pos, state, time.FrameNumber, time.Elapsed);
            }
        }

		public Vector2? PanelPhysicalSize
        {
            get
            {
                // do lookup here into all device types
                return new Vector2(0.0768f, 0.1024f); 
            }
        }

		public float? PanelPhysicalAspectRatio
        {
            get { return PanelPhysicalSize.Value.X / PanelPhysicalSize.Value.Y; }
        }
        public PanelType PanelType
        {
            get { return PanelType.TouchScreen; }
        }


        public float ScreenResolutionAspectRatio
        {
            get { return this.ScreenResolutionWidth / this.ScreenResolutionHeight; }
        }

        // need to think about
        public Single PixelDensity
        {
            get { return 1f; }
            set { ; }
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


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

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

        public void Run ()
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
}
