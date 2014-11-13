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
// │ Copyright © 2008-2014 Sungiant ~ http://www.blimey3d.com ~ Authors: A.J.Pook                                   │ \\
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

    using MonoTouch.UIKit;
    using MonoTouch.Foundation;
    using MonoTouch.CoreText;
    using MonoTouch.CoreGraphics;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class XiosPlatform
        : IPlatform
    {
        public XiosPlatform ()
        {
            var program = new XiosProgram ();
            var api = new XiosApi ();

            api.InitialiseDependencies (program);
            program.InitialiseDependencies (api);

            Api = api;
            Program = program;
        }

        public IProgram Program { get; private set; }
        public IApi Api { get; private set; }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class XiosProgram
        : IProgram
    {
        OpenGLViewController viewController;
        MonoTouch.UIKit.UIWindow window;

        XiosApi Api { get; set; }

        internal void InitialiseDependencies (XiosApi api) { Api = api; }

        public void Start (IApi platformImplementation, Action update, Action render)
        {
            RegisterLoggingListeners ();
            UIApplication.SharedApplication.StatusBarHidden = true;
            UIApplication.SharedApplication.SetStatusBarHidden (true, UIStatusBarAnimation.None);

            // create a new window instance based on the screen size
            window = new UIWindow (UIScreen.MainScreen.Bounds);

            viewController = new OpenGLViewController (update, render);

            window.RootViewController = viewController;

            // make the window visible
            window.MakeKeyAndVisible ();
        }

        public void Stop ()
        {
            throw new NotImplementedException ();
        }

        void RegisterLoggingListeners ()
        {
            NSNotificationCenter.DefaultCenter.AddObserver (
                UIApplication.DidEnterBackgroundNotification, this.DidEnterBackground);

            NSNotificationCenter.DefaultCenter.AddObserver (
                UIApplication.DidBecomeActiveNotification, this.DidBecomeActive);

            NSNotificationCenter.DefaultCenter.AddObserver (
                UIApplication.DidReceiveMemoryWarningNotification, this.DidReceiveMemoryWarning);

            NSNotificationCenter.DefaultCenter.AddObserver (
                UIApplication.DidFinishLaunchingNotification, this.DidFinishLaunching);

            NSNotificationCenter.DefaultCenter.AddObserver (
                UIDevice.OrientationDidChangeNotification, this.OrientationDidChange);

        }

        void DidReceiveMemoryWarning (NSNotification ntf)
        {
            Console.WriteLine ("[Cor.System] DidReceiveMemoryWarning");
        }

        void DidBecomeActive (NSNotification ntf)
        {
            Console.WriteLine ("[Cor.System] DidBecomeActive");
        }

        void DidEnterBackground (NSNotification ntf)
        {
            Console.WriteLine ("[Cor.System] DidEnterBackground");
        }

        void DidFinishLaunching (NSNotification ntf)
        {
            Console.WriteLine ("[Cor.System] DidFinishLaunching");
        }

        void OrientationDidChange (NSNotification ntf)
        {
            Console.WriteLine ("[Cor.System] OrientationDidChange");
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public partial class XiosApi
        : IApi
    {
        XiosProgram Program { get; set; }

        internal void InitialiseDependencies (XiosProgram program)
        {
            Program = program;
        }

        /**
         * Audio
         */
        public Single sfx_GetVolume ()
        {
            throw new NotImplementedException ();
        }

        public void sfx_SetVolume (Single volume)
        {
            throw new NotImplementedException ();
        }


        /**
         * Graphics ~ Implemented in Cor.Platform.OpenTk.cs
         */


        /**
         * Resources
         */
        public Stream res_GetFileStream (String filename)
        {
            string os = sys_GetOperatingSystemIdentifier ();


            string ext = Path.GetExtension (filename);

            string filenameNoExt = filename.Substring (0, filename.Length - ext.Length);

            string path = Path.Combine ("assets/xios", filenameNoExt);

            var resourcePathname = NSBundle.MainBundle.PathForResource (
                path, ext.Substring (1, ext.Length - 1));

            if (os == "iPhone OS : 8.1")
            {
                //resourcePathname = NSFileManager.DefaultManager.GetUrls (
                //                    NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0].AbsoluteString;

                resourcePathname = Path.Combine ("assets/xios", filename);
            }

            if (resourcePathname == null)
            {
                throw new Exception ("Resource [" + filename + "] not found");
            }

            var fStream = new FileStream (resourcePathname, FileMode.Open);

            return fStream;

        }


        /**
         * System
         */
        public String sys_GetMachineIdentifier ()
        {
            return UIDevice.CurrentDevice.Model + " : " + UIDevice.CurrentDevice.Name;
        }

        public String sys_GetOperatingSystemIdentifier ()
        {
            return UIDevice.CurrentDevice.SystemName + " : " + UIDevice.CurrentDevice.SystemVersion;
        }

        public String sys_GetVirtualMachineIdentifier ()
        {
            return "Mono v?";
        }

        public Int32 sys_GetPrimaryScreenResolutionWidth ()
        {
            return (Int32) (UIScreen.MainScreen.Bounds.Width * UIScreen.MainScreen.Scale);
        }

        public Int32 sys_GetPrimaryScreenResolutionHeight ()
        {
            return (Int32) (UIScreen.MainScreen.Bounds.Height * UIScreen.MainScreen.Scale);
        }

        public Vector2? sys_GetPrimaryPanelPhysicalSize ()
        {
            // todo: lookup here into all device types
            return new Vector2(0.0768f, 0.1024f);
        }

        public PanelType sys_GetPrimaryPanelType ()
        {
            return PanelType.TouchScreen;
        }


        /**
         * Application
         */
        public Boolean? app_IsFullscreen ()
        {
            return true;
        }

        public Int32 app_GetWidth ()
        {
            return (Int32) UIScreen.MainScreen.CurrentMode.Size.Width;
        }

        public Int32 app_GetHeight ()
        {
            return (Int32) UIScreen.MainScreen.CurrentMode.Size.Height;
        }


        /**
         * Input
         */
        public DeviceOrientation? hid_GetCurrentOrientation ()
        {
            var monoTouchOrientation = UIDevice.CurrentDevice.Orientation;
            return EnumConverter.ToCor (monoTouchOrientation);
        }

        public Dictionary <DigitalControlIdentifier, Int32> hid_GetDigitalControlStates ()
        {
            return new Dictionary<DigitalControlIdentifier, Int32> ();
        }

        public Dictionary <AnalogControlIdentifier, Single> hid_GetAnalogControlStates ()
        {
            return new Dictionary<AnalogControlIdentifier, float> ();
        }

        public HashSet <BinaryControlIdentifier> hid_GetBinaryControlStates ()
        {
            return new HashSet<BinaryControlIdentifier> ();
        }

        public HashSet <Char> hid_GetPressedCharacters ()
        {
            return new HashSet<char> ();
        }

        public HashSet <RawTouch> hid_GetActiveTouches ()
        {
            return new HashSet<RawTouch> ();
        }


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

    [MonoTouch.Foundation.Register ("EAGLView")]
    public sealed class EAGLView
        : OpenTK.Platform.iPhoneOS.iPhoneOSGameView
    {
        Int64 frameCounter = -1;
        Int32 frameInterval;

        MonoTouch.CoreAnimation.CADisplayLink displayLink;

        readonly Dictionary<Int32, iOSTouchState> touchState = new Dictionary<int, iOSTouchState>();

        readonly Action update;
        readonly Action render;

        public EAGLView (Action update, Action render, RectangleF frame)
            : base (frame)
        {
            this.update = update;
            this.render = render;

            LayerRetainsBacking = true;
            LayerColorFormat = MonoTouch.OpenGLES.EAGLColorFormat.RGBA8;
            ContextRenderingApi = MonoTouch.OpenGLES.EAGLRenderingAPI.OpenGLES2;


        }

        public Boolean IsAnimating
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

        [MonoTouch.Foundation.Export ("layerClass")]
        public static new MonoTouch.ObjCRuntime.Class GetLayerClass ()
        {
            return OpenTK.Platform.iPhoneOS.iPhoneOSGameView.GetLayerClass ();
        }


        protected override void ConfigureLayer (MonoTouch.CoreAnimation.CAEAGLLayer eaglLayer)
        {
            eaglLayer.Opaque = true;
        }

        public void StartAnimating ()
        {
            if (IsAnimating)
                return;

            OpenTKHelper.InitilizeRenderTargets (Size.Width, Size.Height);
            CreateFrameBuffer ();
            OpenTKHelper.InitilizeRenderSettings ();

            displayLink = UIScreen.MainScreen.CreateDisplayLink (this, new MonoTouch.ObjCRuntime.Selector ("drawFrame"));
            displayLink.FrameInterval = frameInterval;
            displayLink.AddToRunLoop (NSRunLoop.Current, NSRunLoop.NSDefaultRunLoopMode);

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

            frameCounter++;

            this.ClearOldTouches ();

            update ();
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
            }
        }

        protected override void OnRenderFrame (OpenTK.FrameEventArgs e)
        {
            base.OnRenderFrame (e);

            base.MakeCurrent ();

            render ();

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
        readonly Action update;
        readonly Action render;

        public OpenGLViewController (Action update, Action render)
        {
            this.update = update;
            this.render = render;
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
            base.View = new EAGLView (update, render, frame);
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

            //View.SetEngineDetails (_settings, _game);
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

    /*

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

    */
}
