// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__          __    _____                                     │ \\
// │ \______   \  | _____ _/  |__/ ____\___________  _____                  │ \\
// │  |     ___/  | \__  \\   __\   __\/  _ \_  __ \/     \                 │ \\
// │  |    |   |  |__/ __ \|  |  |  | (  <_> )  | \/  Y Y  \                │ \\
// │  |____|   |____(____  /__|  |__|  \____/|__|  |__|_|  /                │ \\
// │                     \/                              \/                 │ \\
// │                                                                        │ \\
// │ A partial implementation of the Blimey Plaform API targeting Xamarin's │ \\
// │ iOS framework.  This partial implementation does not implement any     │ \\
// │ of the Blimey Plaform API's `gfx` calls and is intended to be compiled │ \\
// │ alongside the OpenTK partial file with PLATFORM_XIOS defined.          │ \\
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

    public sealed class Platform
        : IPlatform
    {
        public Platform ()
        {
            var program = new Program ();
            var api = new Api ();

            api.InitialiseDependencies (program);
            program.InitialiseDependencies (api);

            Api = api;
            Program = program;
        }

        public IProgram Program { get; private set; }
        public IApi Api { get; private set; }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class Program
        : IProgram
    {
        OpenGLViewController viewController;
        UIWindow window;

        Api Api { get; set; }

        public OpenGLViewController OpenGLViewController { get { return viewController; } }

        internal void InitialiseDependencies (Api api) { Api = api; }

        public void Start (IApi platformImplementation, Action update, Action render)
        {
            RegisterLoggingListeners ();
            UIApplication.SharedApplication.StatusBarHidden = true;
            UIApplication.SharedApplication.SetStatusBarHidden (true, UIStatusBarAnimation.None);

            // create a new window instance based on the screen size
            window = new UIWindow (UIScreen.MainScreen.Bounds);

            viewController = new OpenGLViewController (Api, update, render);

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

    public partial class Api
        : IApi
    {
        Program Program { get; set; }

        internal void InitialiseDependencies (Program program)
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
         * Graphics ~ Implemented in CommonOpenTk.cs
         */


        /**
         * Resources
         */
        public Stream res_GetFileStream (String filename)
        {
            string ext = Path.GetExtension (filename);

            string path = filename.Substring (0, filename.Length - ext.Length);

            var resourcePathname = NSBundle.MainBundle.PathForResource (path, ext.Substring (1, ext.Length - 1));

            if (resourcePathname == null)
            {
                throw new Exception ("Resource [" + filename + "] not found");
            }

            var fStream = new FileStream (resourcePathname, FileMode.Open, FileAccess.Read);

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
            EAGLView view = Program.OpenGLViewController.View as EAGLView;
            return view.GetRawTouches ();
        }


    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class EnumConverter
    {
        public static DeviceOrientation ToCor (UIDeviceOrientation monoTouch)
        {
            switch (monoTouch)
            {
                case UIDeviceOrientation.FaceDown: return DeviceOrientation.Default;
                case UIDeviceOrientation.FaceUp: return DeviceOrientation.Default;
                case UIDeviceOrientation.LandscapeLeft: return DeviceOrientation.Leftside;
                case UIDeviceOrientation.LandscapeRight: return DeviceOrientation.Rightside;
                case UIDeviceOrientation.Portrait: return DeviceOrientation.Default;
                case UIDeviceOrientation.PortraitUpsideDown: return DeviceOrientation.Upsidedown;

                default:
                    Console.WriteLine ("WARNING: Unknown device orientaton: " + monoTouch);
                    return DeviceOrientation.Default;
            }
        }

        public static TouchPhase ToCorPrimitiveType (UITouchPhase phase)
        {
            switch (phase)
            {
                case UITouchPhase.Began: return TouchPhase.JustPressed;
                case UITouchPhase.Cancelled: return TouchPhase.JustReleased;
                case UITouchPhase.Ended: return TouchPhase.JustReleased;
                case UITouchPhase.Moved: return TouchPhase.Active;
                case UITouchPhase.Stationary: return TouchPhase.Active;
            }

            return TouchPhase.Invalid;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class Vector2Converter
    {
		public static PointF ToSystemDrawing (this Vector2 vec)
        {
			return new PointF (vec.X, vec.Y);
        }

		public static Vector2 ToAbacus (this PointF vec)
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

        internal Dictionary<Int32, iOSTouchState> iOSTouches
        {
            get { return touchState; }
        }

        readonly Dictionary<Int32, iOSTouchState> touchState = new Dictionary<int, iOSTouchState>();

        readonly IApi api;
        readonly Action update;
        readonly Action render;

        public EAGLView (IApi api, Action update, Action render, RectangleF frame)
            : base (frame)
        {
            this.api = api;
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

                if (ts.Phase == UITouchPhase.Cancelled ||
                    ts.Phase == UITouchPhase.Ended)
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

        public override void TouchesBegan (NSSet touches, UIEvent evt)
        {
            ProcessTouchChange (touches);

            base.TouchesBegan (touches, evt);
        }

        public override void TouchesMoved (NSSet touches, UIEvent evt)
        {
            ProcessTouchChange (touches);

            base.TouchesMoved (touches, evt);
        }

        public override void TouchesCancelled (NSSet touches, UIEvent evt)
        {
            ProcessTouchChange (touches);

            base.TouchesCancelled (touches, evt);
        }

        public override void TouchesEnded (NSSet touches, UIEvent evt)
        {
            ProcessTouchChange (touches);

            base.TouchesEnded (touches, evt);
        }

        Int32 changedCount = -1;
        Int32 lastUpdated = -1;

        void ProcessTouchChange (NSSet touches)
        {
            var touchesArray = touches.ToArray<UITouch> ();

            for (int i = 0; i < touchesArray.Length; ++i)
            {
                var touch = touchesArray [i];

                //Get position touch
                var location = touch.LocationInView (this);
                var id = touch.Handle.ToInt32 ();
                var phase = touch.Phase;

                // seems to be a problem with mono touch reporting a new touch with
                // the same id across multiple frames.
                if (touchState.Keys.Contains (id) && phase == UITouchPhase.Began)
                {
                    phase = UITouchPhase.Stationary;
                }

                var ts = new iOSTouchState ();
                ts.Handle = id;
                ts.LastUpdated = this.frameCounter;
                ts.Location = location;

                ts.Phase = phase;

                if (phase == UITouchPhase.Began)
                {
                    //Console.WriteLine ("add "+id);
                    touchState.Add (id, ts);
                }
                else
                {
                    if (touchState.ContainsKey (id) )
                    {
                        touchState[id] = ts;

                        if (ts.Phase == UITouchPhase.Began)
                        {
                            ts.Phase = UITouchPhase.Stationary;
                        }

                    }
                    else
                    {
                        throw new Exception ("eerrr???");
                    }
                }
            }

            UpdateRawTouches ();

            changedCount++;
        }


        readonly Dictionary <int, RawTouch> intermediateTouchMap = new Dictionary<int, RawTouch> ();
        readonly HashSet <RawTouch> rawTouchesHS = new HashSet<RawTouch> ();


        public HashSet <RawTouch> GetRawTouches ()
        {
            if (lastUpdated < changedCount)
            {
                rawTouchesHS.Clear ();
                foreach (var v in intermediateTouchMap.Values)
                {
                    rawTouchesHS.Add (v);
                }

                lastUpdated = changedCount;
            }

            return rawTouchesHS;
        }

        Vector2 GetTouchPos (PointF location)
        {
            var pos = new Vector2(location.X, location.Y);
            pos.X = pos.X / api.app_GetWidth ();
            pos.Y = pos.Y / api.app_GetHeight ();
            pos -= new Vector2(0.5f, 0.5f);
            pos.Y = -pos.Y;
            return pos;
        }

        void UpdateRawTouches ()
        {
            foreach (var iOSTouchHandle in iOSTouches.Keys)
            {
                var p = iOSTouches [iOSTouchHandle].Phase;
                var l = iOSTouches [iOSTouchHandle].Location;

                if (!intermediateTouchMap.ContainsKey (iOSTouchHandle))
                {
                    var r = new RawTouch ();
                    r.Id = new Guid ().ToString ();
                    r.Phase = EnumConverter.ToCorPrimitiveType(p);
                    r.Position = GetTouchPos(l);
                    intermediateTouchMap.Add (iOSTouchHandle, r);
                }
                else
                {
                    var rt = intermediateTouchMap [iOSTouchHandle];
                    rt.Phase = EnumConverter.ToCorPrimitiveType(p);
                    rt.Position = GetTouchPos(l);

                }
            }

            var toRemove = new List <Int32> ();
            foreach (var iOSTouchHandle in intermediateTouchMap.Keys)
            {
                if (!iOSTouches.ContainsKey (iOSTouchHandle))
                    toRemove.Add (iOSTouchHandle);
            }

            foreach (var key in toRemove)
                intermediateTouchMap.Remove (key);
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
        : UIViewController
    {
        readonly IApi api;
        readonly Action update;
        readonly Action render;

        public OpenGLViewController (IApi api, Action update, Action render)
        {
            this.api = api;
            this.update = update;
            this.render = render;
        }

        internal new EAGLView View
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
            var frame = UIScreen.MainScreen.Bounds;
            base.View = new EAGLView (api, update, render, frame);
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            NSNotificationCenter.DefaultCenter.AddObserver (
                UIApplication.WillResignActiveNotification, a => {
                if (IsViewLoaded && View.Window != null)
                    View.StopAnimating ();
                },
                this
            );

            NSNotificationCenter.DefaultCenter.AddObserver (
                UIApplication.DidBecomeActiveNotification, a => {
                if (IsViewLoaded && View.Window != null)
                    View.StartAnimating ();
                },
                this
            );

            NSNotificationCenter.DefaultCenter.AddObserver (
                UIApplication.WillTerminateNotification, a => {
                if (IsViewLoaded && View.Window != null)
                    View.StopAnimating ();
                },
                this
            );
        }

        protected override void Dispose (Boolean disposing)
        {
            base.Dispose (disposing);

            NSNotificationCenter.DefaultCenter.RemoveObserver (this);
        }

        public override void DidReceiveMemoryWarning ()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning ();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
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
}
