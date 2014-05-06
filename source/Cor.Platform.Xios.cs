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
}
