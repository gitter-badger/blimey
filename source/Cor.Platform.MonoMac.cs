// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! Mono Mac Platform Implementation                                                                          │ \\
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

namespace Cor.Platform.MonoMac
{
	using global::System;
	using global::System.Globalization;
	using global::System.Collections;
	using global::System.Collections.Generic;
	using global::System.Linq;
	using global::System.IO;
	using global::System.Diagnostics;
	using global::System.Runtime.InteropServices;
	using global::System.Runtime.ConstrainedExecution;

    
    using Fudge;
    using Abacus.SinglePrecision;
    

    using Cor.Lib.OTK;
    using Cor.Platform.Stub;

	using global::MonoMac.Foundation;
	using global::MonoMac.AppKit;
	using global::MonoMac.CoreVideo;
	using global::MonoMac.CoreGraphics;
	using global::MonoMac.CoreImage;
	using global::MonoMac.ImageIO;
	using global::MonoMac.ImageKit;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class Engine
        : EngineBase
    {
        readonly Graphics graphics;
        readonly Input input;
		readonly Host host;
        readonly AppSettings settings;
        readonly Status status;
        readonly IApp app;
        readonly LogManager log;
        
        public Engine (AppSettings settings, IApp app, Int32 width, Int32 height)
			: base (new MonoMacPlatform ())
        {
            InternalUtils.Log.Info ("Engine -> ()");

            this.settings = settings;
            this.graphics = new Graphics ();
			this.host = new Host ();
            this.status = new Status (width, height);
			this.input = new Input (this.host, this.status, settings.MouseGeneratesTouches);
            this.log = new LogManager (this.settings.LogSettings);
            this.app = app;
			this.app.Start (this);
        }

        internal Graphics GraphicsImplementation { get { return this.graphics; } }

        internal Input InputImplementation { get { return this.input; } }

		internal Host HostImplementation { get { return this.host; } }

        internal Status DisplayStatusImplementation { get { return this.status; } }

        #region EngineBase

        public override GraphicsBase Graphics { get { return this.graphics; } }

		public override StatusBase Status { get { return this.status; } }

		public override InputBase Input { get { return this.input; } }

		public override HostBase Host { get { return this.host; } }

        public override LogManager Log { get { return this.log; } }

        public override AppSettings Settings { get { return this.settings; } }

        #endregion

        internal Boolean Update (AppTime time)
        {
            InputImplementation.Update (time);
			return app.Update (this, time);
        }

        internal void Render ()
        {
			app.Render (this);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

	public class MonoMacPlatform
		: IPlatform
    {
		Single volume = 1f;

		public MonoMacPlatform ()
        {
            this.volume = 1f;
        }

        #region IPlatform

        public Single sfx_GetVolume () { return this.volume; }

		public void sfx_SetVolume (Single value)
		{
	        this.volume = value;
        }

        public Stream res_GetFileStream (String filePath)
        {
            String platformPath = Path.Combine ("assets/monomac", filePath);
            String rtype = Path.GetExtension (platformPath);
            String rname = Path.Combine (
                Path.GetDirectoryName (platformPath),
                Path.GetFileNameWithoutExtension (platformPath));
            var correctPath = global::MonoMac.Foundation.NSBundle.MainBundle.PathForResource (rname, rtype);

            if (!File.Exists (correctPath))
            {
                throw new FileNotFoundException (correctPath);
            }

            return new FileStream (correctPath, FileMode.Open);
        }

        #endregion
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class Input
        : InputBase
    {
        readonly Keyboard keyboard;
        readonly Mouse mouse;

        readonly IXbox360Gamepad xbox360Gamepad = new StubXbox360Gamepad ();
        readonly IPsmGamepad psmGamepad = new StubPsmGamepad ();
        readonly IMultiTouchController multiTouchController = new StubMultiTouchController ();
        readonly IGenericGamepad genericGamepad = new StubGenericGamepad ();

        TouchScreenImplementation touchScreen;

		public Input (HostBase host, StatusBase status, Boolean mouseGeneratesTouches)
        {
            InternalUtils.Log.Info ("InputManager -> ()");

            keyboard = new Keyboard ();
            mouse = new Mouse ();

            if (mouseGeneratesTouches)
            {
				touchScreen = new TouchScreenImplementation (host, this, status);
            }
        }

        internal Keyboard KeyboardImplemenatation { get { return keyboard; } }
        internal Mouse MouseImplemenatation { get { return mouse; } }

        #region IInputManager

        public override IXbox360Gamepad Xbox360Gamepad { get { return xbox360Gamepad; } }
        public override IPsmGamepad PsmGamepad { get { return psmGamepad; } }
        public override IMultiTouchController MultiTouchController { get { return multiTouchController; } }
        public override IGenericGamepad GenericGamepad { get { return genericGamepad; } }
        public override IMouse Mouse { get { return mouse; } }
        public override IKeyboard Keyboard { get { return keyboard; } }

        #endregion

        public void Update (AppTime time)
        {
            if (touchScreen != null)
            {
                touchScreen.Update (time);
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class PanelSpecification
        : IPanelSpecification
    {
		public PanelSpecification () { InternalUtils.Log.Info ("PanelSpecification -> ()"); }

        #region IPanelSpecification

		public Vector2? PanelPhysicalSize { get { return null; } }
		public Single? PanelPhysicalAspectRatio { get { return null; } }
        public PanelType PanelType { get { return PanelType.TouchScreen; } }

        #endregion
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class ScreenSpecification
        : IScreenSpecification
    {
        Int32 width = 800;
        Int32 height = 600;

		public ScreenSpecification () { InternalUtils.Log.Info ("ScreenSpecification -> ()"); }

        #region IScreenSpecification

		public Int32 ScreenResolutionWidth { get { return width; } }
		public Int32 ScreenResolutionHeight { get { return height; } }

        public Single ScreenResolutionAspectRatio
        {
			get { return (Single) this.ScreenResolutionWidth / (Single) this.ScreenResolutionHeight; }
        }

        #endregion
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

	public sealed class Host
		: HostBase
    {
        readonly IScreenSpecification screen;
        readonly IPanelSpecification panel;

		public Host ()
        {
			InternalUtils.Log.Info ("Host -> ()");

            screen = new ScreenSpecification ();
            panel = new PanelSpecification ();
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

        #region ISystemManager

		public override String Machine { get { return "Machintosh"; } }
		public override String OperatingSystem { get { return "OSX" + Environment.OSVersion.VersionString;; } }
		public override String VirtualMachine { get { return "Mono ?"; } }

        public override DeviceOrientation CurrentOrientation { get { return DeviceOrientation.Default; } }
        public override IScreenSpecification ScreenSpecification { get { return this.screen; } }
        public override IPanelSpecification PanelSpecification { get { return this.panel; } }
        
        #endregion
	}


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class MonoMacApp
        : IDisposable
    {
        MacGameNSWindow mainWindow;
        OpenGLView openGLView;
        readonly AppSettings settings;
        readonly IApp entryPoint;

        public MonoMacApp (AppSettings settings, IApp entryPoint)
        {
            this.settings = settings;
            this.entryPoint = entryPoint;
        }

        void InitializeMainWindow ()
        {
            System.Drawing.RectangleF frame = new System.Drawing.RectangleF (
                0, 0,
                800,
                600);

            mainWindow = new MacGameNSWindow (
                frame,
                NSWindowStyle.Titled |
                NSWindowStyle.Closable |
                NSWindowStyle.Miniaturizable |
                NSWindowStyle.Resizable,
                NSBackingStore.Buffered,
                true);

            mainWindow.Title = this.settings.AppName;

            mainWindow.WindowController = new NSWindowController (mainWindow);
            mainWindow.Delegate = new MainWindowDelegate (this);

            mainWindow.IsOpaque = true;
            mainWindow.EnableCursorRects ();
            mainWindow.AcceptsMouseMovedEvents = false;
            mainWindow.Center ();

            openGLView = new OpenGLView (this.settings, this.entryPoint, frame);

            mainWindow.ContentView.AddSubview (openGLView);

            mainWindow.MakeKeyAndOrderFront (mainWindow);

            openGLView.StartRunLoop (60f);
        }

        public void Run ()
        {
            InitializeMainWindow ();
        }

        public void Dispose ()
        {
            mainWindow.Dispose ();
            openGLView.Dispose ();
        }

        Single GetTitleBarHeight ()
        {
            System.Drawing.RectangleF contentRect = NSWindow.ContentRectFor (mainWindow.Frame, mainWindow.StyleMask);
            return mainWindow.Frame.Height - contentRect.Height;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    [CLSCompliant (false)]
    public sealed class OpenGLView
        : global::MonoMac.OpenGL.MonoMacGameView
    {
        NSTrackingArea trackingArea;

        Engine gameEngine;
        Single elapsedTime;
        Int64 frameCounter = -1;
        TimeSpan previousTimeSpan;

        readonly AppSettings settings;
        readonly IApp entryPoint;
        readonly Stopwatch timer = new Stopwatch ();

        //------------------------------------------------------------------------------------------------------------//
        // Init
        //------------------------------------------------------------------------------------------------------------//

        public OpenGLView (AppSettings settings, IApp entryPoint, System.Drawing.RectangleF frame)
            : base (frame)
        {
            this.settings = settings;
            this.entryPoint = entryPoint;

            // Make the suface size automatically update when the window
            // size changes.
            this.WantsBestResolutionOpenGLSurface = true;

            this.AutoresizingMask =
                global::MonoMac.AppKit.NSViewResizingMask.HeightSizable |
                global::MonoMac.AppKit.NSViewResizingMask.MaxXMargin |
                global::MonoMac.AppKit.NSViewResizingMask.MinYMargin |
                global::MonoMac.AppKit.NSViewResizingMask.WidthSizable;
        }

        void toggleFullScreen (NSObject sender)
        {
            if (WindowState == global::MonoMac.OpenGL.WindowState.Fullscreen)
                WindowState = global::MonoMac.OpenGL.WindowState.Normal;
            else
                WindowState = global::MonoMac.OpenGL.WindowState.Fullscreen;
        }


        public void StartRunLoop (double updateRate)
        {
            Run (updateRate);
        }

        //------------------------------------------------------------------------------------------------------------//
        // MonoMacGameView Callbacks
        //------------------------------------------------------------------------------------------------------------//

        protected override void OnClosed (EventArgs e)
        {
            InternalUtils.Log.Info ("MonoMacGameView.OnClosed");
            base.OnClosed (e);
        }

        protected override void OnDisposed (EventArgs e)
        {
            InternalUtils.Log.Info ("MonoMacGameView.OnDisposed");
            base.OnDisposed (e);
        }

        protected override void OnLoad (EventArgs e)
        {
            gameEngine = new Engine (
                this.settings, this.entryPoint, (Int32) this.Frame.Width, (Int32) this.Frame.Height);

            timer.Start ();

            InternalUtils.Log.Info ("MonoMacGameView.OnLoad");
            base.OnLoad (e);
        }

        protected override void OnRenderFrame (global::MonoMac.OpenGL.FrameEventArgs e)
        {
            try
            {
                gameEngine.Render ();
            }
            catch (Exception ex)
            {
                InternalUtils.Log.Error ("Failed to render frame:" + ex.Message);
            }

            base.OnRenderFrame (e);
        }

        protected override void OnResize (EventArgs e)
        {
            // Occurs whenever GameWindow is resized.
            // Update the OpenGL Viewport and Projection Matrix here.
            InternalUtils.Log.Info ("MonoMacGameView.OnResize -> Bounds:" + Bounds + ", Frame:" + Frame);

            gameEngine.DisplayStatusImplementation.UpdateSize ((Int32)Frame.Width, (Int32)Frame.Height);

            base.OnResize (e);
        }

        protected override void OnTitleChanged (EventArgs e)
        {
            InternalUtils.Log.Info ("MonoMacGameView.OnTitleChanged");
            base.OnTitleChanged (e);
        }

        protected override void OnUnload (EventArgs e)
        {
            InternalUtils.Log.Info ("MonoMacGameView.OnUnload");
            base.OnUnload (e);
        }

        protected override void OnUpdateFrame (global::MonoMac.OpenGL.FrameEventArgs fea)
        {
            Single dt = (Single)(timer.Elapsed.TotalSeconds - previousTimeSpan.TotalSeconds);
            previousTimeSpan = timer.Elapsed;

            if (dt > 0.5f)
            {
                dt = 0.0f;
            }

            elapsedTime += dt;

            var appTime = new AppTime (dt, elapsedTime, ++frameCounter);

            gameEngine.Update (appTime);

            base.OnUpdateFrame (fea);
        }

        protected override void OnVisibleChanged (EventArgs e)
        {
            InternalUtils.Log.Info ("MonoMacGameView.OnVisibleChanged");
            base.OnVisibleChanged (e);
        }

        protected override void OnWindowStateChanged (EventArgs e)
        {
            InternalUtils.Log.Info ("MonoMacGameView.OnWindowStateChanged");
            base.OnWindowStateChanged (e);
        }

        //------------------------------------------------------------------------------------------------------------//
        // NSResponder Callbacks
        //------------------------------------------------------------------------------------------------------------//

        public override Boolean AcceptsFirstResponder ()
        {
            return true; // We want this view to be able to receive key events
        }

        public override Boolean BecomeFirstResponder ()
        {
            return true;
        }

        public override Boolean EnterFullscreenModeWithOptions (NSScreen screen, NSDictionary options)
        {
            return base.EnterFullscreenModeWithOptions (screen, options);
        }

        public override void ExitFullscreenModeWithOptions (NSDictionary options)
        {
            base.ExitFullscreenModeWithOptions (options);
        }

        public override void ViewWillMoveToWindow (NSWindow newWindow)
        {
            if (trackingArea != null) RemoveTrackingArea (trackingArea);

            trackingArea = new NSTrackingArea (
                Frame,
                NSTrackingAreaOptions.MouseMoved |
                NSTrackingAreaOptions.MouseEnteredAndExited |
                NSTrackingAreaOptions.EnabledDuringMouseDrag |
                NSTrackingAreaOptions.ActiveWhenFirstResponder |
                NSTrackingAreaOptions.InVisibleRect |
                NSTrackingAreaOptions.CursorUpdate,
                this,
                new NSDictionary ()
            );

            AddTrackingArea (trackingArea);
        }

        // Keyboard //------------------------------------------------------------------------------------------------//
        public override void KeyDown (NSEvent theEvent)
        {
            this.gameEngine.InputImplementation.KeyboardImplemenatation.KeyDown (theEvent);
        }

        public override void KeyUp (NSEvent theEvent)
        {
            this.gameEngine.InputImplementation.KeyboardImplemenatation.KeyUp (theEvent);
        }

        public override void FlagsChanged (NSEvent theEvent)
        {
            base.FlagsChanged (theEvent);
        }


        // Mouse //---------------------------------------------------------------------------------------------------//
        public override void MouseDown (NSEvent theEvent)
        {
            this.gameEngine.InputImplementation.MouseImplemenatation.LeftMouseDown (theEvent);
        }

        public override void MouseUp (NSEvent theEvent)
        {
            this.gameEngine.InputImplementation.MouseImplemenatation.LeftMouseUp (theEvent);
        }

        public override void MouseDragged (NSEvent theEvent)
        {
            base.MouseDragged (theEvent);
        }

        public override void RightMouseDown (NSEvent theEvent)
        {
            this.gameEngine.InputImplementation.MouseImplemenatation.RightMouseDown (theEvent);
        }

        public override void RightMouseUp (NSEvent theEvent)
        {
            this.gameEngine.InputImplementation.MouseImplemenatation.RightMouseUp (theEvent);
        }

        public override void RightMouseDragged (NSEvent theEvent)
        {
            base.RightMouseDragged (theEvent);
        }

        public override void OtherMouseDown (NSEvent theEvent)
        {
            this.gameEngine.InputImplementation.MouseImplemenatation.MiddleMouseDown (theEvent);
        }


        public override void OtherMouseUp (NSEvent theEvent)
        {
            this.gameEngine.InputImplementation.MouseImplemenatation.MiddletMouseUp (theEvent);
        }

        public override void OtherMouseDragged (NSEvent theEvent)
        {
            base.OtherMouseDragged (theEvent);
        }

        public override void ScrollWheel (NSEvent theEvent)
        {
            this.gameEngine.InputImplementation.MouseImplemenatation.ScrollWheel (theEvent);
        }

        public override void MouseMoved (NSEvent theEvent)
        {
            this.gameEngine.InputImplementation.MouseImplemenatation.MouseMoved (theEvent);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class MacGameNSWindow
        : NSWindow
    {
        [Export ("initWithContentRect:styleMask:backing:defer:")]
        public MacGameNSWindow (System.Drawing.RectangleF rect, NSWindowStyle style, NSBackingStore backing, Boolean defer)
            : base (rect, style, backing, defer)
        {
        }

        public override Boolean CanBecomeKeyWindow { get { return true; } }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal sealed class MainWindowDelegate
        : NSWindowDelegate
    {
        private readonly MonoMacApp owner;

        public MainWindowDelegate (MonoMacApp owner)
        {
            if (owner == null) throw new ArgumentNullException ("owner");
            this.owner = owner;
        }

        public override Boolean ShouldZoom (NSWindow window, System.Drawing.RectangleF newFrame)
        {
            return true;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal static class Vector2Converter
    {
		internal static global::System.Drawing.PointF ToSystemDrawing (this Vector2 vec)
        {
			return new global::System.Drawing.PointF (vec.X, vec.Y);
        }

		internal static Vector2 ToAbacus (this global::System.Drawing.PointF vec)
        {
            return new Vector2 (vec.X, vec.Y);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class Keyboard
        : IKeyboard
    {
        readonly HashSet<Char> characterKeysThatAreDown = new HashSet<Char>();
        readonly HashSet<FunctionalKey> functionalKeysThatAreDown = new HashSet<FunctionalKey>();

        internal void KeyDown (NSEvent theEvent)
        {
            theEvent.Characters
                .ToCharArray ()
                .Where (x => !IsFunctionalKey (x))
                .ToList ()
                .ForEach (x => characterKeysThatAreDown.Add (x));

            var fKey = GetFunctionalKey (theEvent.KeyCode);
            if (fKey.HasValue) functionalKeysThatAreDown.Add (fKey.Value);
        }

        internal void KeyUp (NSEvent theEvent)
        {
            theEvent.Characters
                .ToCharArray ()
                .Where (x => !IsFunctionalKey (x))
                .ToList ()
                .ForEach (x => characterKeysThatAreDown.Remove (x));

            var fKey = GetFunctionalKey (theEvent.KeyCode);
            if (fKey.HasValue) functionalKeysThatAreDown.Remove (fKey.Value);
        }

        static FunctionalKey? GetFunctionalKey (UInt16 hardwareIndependantKeyCode)
        {
            if (hardwareIndependantKeyCode == 0x24) return FunctionalKey.Enter;
            if (hardwareIndependantKeyCode == 0x7C) return FunctionalKey.Right;
            if (hardwareIndependantKeyCode == 0x7B) return FunctionalKey.Left;
            if (hardwareIndependantKeyCode == 0x7E) return FunctionalKey.Up;
            if (hardwareIndependantKeyCode == 0x7D) return FunctionalKey.Down;
            if (hardwareIndependantKeyCode == 0x31) return FunctionalKey.Spacebar;
            if (hardwareIndependantKeyCode == 0x35) return FunctionalKey.Escape;
            if (hardwareIndependantKeyCode == 0x30) return FunctionalKey.Tab;
            if (hardwareIndependantKeyCode == 0x33) return FunctionalKey.Backspace;
            if (hardwareIndependantKeyCode == 0x74) return FunctionalKey.PageUp;
            if (hardwareIndependantKeyCode == 0x79) return FunctionalKey.PageDown;
            if (hardwareIndependantKeyCode == 0x73) return FunctionalKey.Home;
            if (hardwareIndependantKeyCode == 0x3C) return FunctionalKey.RightShift;
            if (hardwareIndependantKeyCode == 0x38) return FunctionalKey.LeftShift;

            if (hardwareIndependantKeyCode == 0x7A) return FunctionalKey.F1;
            if (hardwareIndependantKeyCode == 0x78) return FunctionalKey.F2;
            if (hardwareIndependantKeyCode == 0x63) return FunctionalKey.F3;
            if (hardwareIndependantKeyCode == 0x76) return FunctionalKey.F4;
            if (hardwareIndependantKeyCode == 0x60) return FunctionalKey.F5;
            if (hardwareIndependantKeyCode == 0x61) return FunctionalKey.F6;
            if (hardwareIndependantKeyCode == 0x62) return FunctionalKey.F7;
            if (hardwareIndependantKeyCode == 0x64) return FunctionalKey.F8;
            if (hardwareIndependantKeyCode == 0x65) return FunctionalKey.F9;
            if (hardwareIndependantKeyCode == 0x6D) return FunctionalKey.F10;
            if (hardwareIndependantKeyCode == 0x67) return FunctionalKey.F11;
            if (hardwareIndependantKeyCode == 0x6F) return FunctionalKey.F12;
            if (hardwareIndependantKeyCode == 0x69) return FunctionalKey.F13;
            if (hardwareIndependantKeyCode == 0x6B) return FunctionalKey.F14;
            if (hardwareIndependantKeyCode == 0x71) return FunctionalKey.F15;
            if (hardwareIndependantKeyCode == 0x6A) return FunctionalKey.F16;
            if (hardwareIndependantKeyCode == 0x40) return FunctionalKey.F17;
            if (hardwareIndependantKeyCode == 0x4F) return FunctionalKey.F18;
            if (hardwareIndependantKeyCode == 0x50) return FunctionalKey.F19;
            if (hardwareIndependantKeyCode == 0x5A) return FunctionalKey.F20;

            /*
            kVK_Command                   = 0x37,
            kVK_CapsLock                  = 0x39,
            kVK_Option                    = 0x3A,
            kVK_Control                   = 0x3B,
            kVK_RightOption               = 0x3D,
            kVK_RightControl              = 0x3E,
            kVK_Function                  = 0x3F,
            kVK_VolumeUp                  = 0x48,
            kVK_VolumeDown                = 0x49,
            kVK_Mute                      = 0x4A,
            kVK_Help                      = 0x72,
            kVK_ForwardDelete             = 0x75,
            kVK_End                       = 0x77,
            */

            return null;
        }

        static Boolean IsFunctionalKey (Char c)
        {
            if (c == '\r') return true;
            if (c == '\n') return true;
            if (c == '\t') return true;

            return false;
        }

        #region IKeyboard

        public FunctionalKey[] GetPressedFunctionalKey ()
        {
            return functionalKeysThatAreDown.ToArray ();
        }

        public Boolean IsFunctionalKeyDown (FunctionalKey key)
        {
            return functionalKeysThatAreDown.Contains (key);
        }

        public Boolean IsFunctionalKeyUp (FunctionalKey key)
        {
            return !functionalKeysThatAreDown.Contains (key);
        }

        public KeyState this [FunctionalKey key]
        {
            get { return functionalKeysThatAreDown.Contains (key) ? KeyState.Down : KeyState.Up; }
        }

        public Char[] GetPressedCharacterKeys ()
        {
            return characterKeysThatAreDown.ToArray ();
        }

        public Boolean IsCharacterKeyDown (Char key)
        {
            return characterKeysThatAreDown.Contains (key);
        }

        public Boolean IsCharacterKeyUp (Char key)
        {
            return !characterKeysThatAreDown.Contains (key);
        }

        public KeyState this [Char key]
        {
            get { return characterKeysThatAreDown.Contains (key) ? KeyState.Down : KeyState.Up; }
        }

        #endregion
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal sealed class Mouse
        : IMouse
    {
        ButtonState left = ButtonState.Released;
        ButtonState middle = ButtonState.Released;
        ButtonState right = ButtonState.Released;
        Int32 scrollWheelValue = 0;
        Int32 x = 0;
        Int32 y = 0;

        internal void MouseMoved (NSEvent theEvent)
        {
            x = theEvent.AbsoluteX;
            y = theEvent.AbsoluteY;
        }

        internal void LeftMouseUp (NSEvent theEvent) { left = ButtonState.Released; }
        internal void LeftMouseDown (NSEvent theEvent) { left = ButtonState.Pressed; }
        internal void MiddletMouseUp (NSEvent theEvent) { middle = ButtonState.Released; }
        internal void MiddleMouseDown (NSEvent theEvent) { middle = ButtonState.Pressed; }
        internal void RightMouseUp (NSEvent theEvent) { right = ButtonState.Released; }
        internal void RightMouseDown (NSEvent theEvent) { right = ButtonState.Pressed; }
        internal void ScrollWheel (NSEvent theEvent) { ; }

        #region IMouse

        public ButtonState Left { get { return left; } }
        public ButtonState Middle { get { return middle; } }
        public ButtonState Right { get { return right; } }
        public Int32 ScrollWheelValue { get { return scrollWheelValue; } }
        public Int32 X { get { return x; } }
        public Int32 Y { get { return y; } }

        #endregion
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class Status
		: StatusBase
    {
        Int32 width = 0;
        Int32 height = 0;

        public Status (Int32 width, Int32 height)
        {
            InternalUtils.Log.Info (
                "DisplayStatus -> ()");

            this.width = width;
            this.height = height;
        }

        internal void UpdateSize (Int32 width, Int32 height)
        {
            this.width = width;
            this.height = height;
        }

        #region StatusBase

		public override Boolean? Fullscreen { get { return true; } }

        public override Int32 Width { get { return width; } }

        public override Int32 Height { get { return height; } }

        #endregion
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class TouchScreenImplementation
        : IMultiTouchController
    {
        Boolean doneFirstUpdateFlag = false;

		readonly HostBase host;
		readonly InputBase input;
		readonly StatusBase status;
        ButtonState previousMouseLeftState;
        readonly TouchCollection collection = new TouchCollection ();

        internal TouchScreenImplementation (HostBase host, InputBase input, StatusBase status)
        {
            this.host = host;
            this.input = input;
            this.status = status;
        }

        public TouchCollection TouchCollection
        {
            get { return this.collection; }
        }

		public IPanelSpecification PanelSpecification
        {
			get { return (host as Host).PanelSpecification; }
        }

        internal void Update (AppTime time)
        {
            this.collection.ClearBuffer ();

            if (doneFirstUpdateFlag)
            {
                Boolean pressedThisFrame = (this.input.Mouse.Left == ButtonState.Pressed);
                Boolean pressedLastFrame = (previousMouseLeftState == ButtonState.Pressed);

                Int32 id = -42;
                Vector2 pos = new Vector2(this.input.Mouse.X, this.input.Mouse.Y);

				Int32 w = this.status.Width;
				Int32 h = this.status.Height;

                pos.X = pos.X / (Single)w;
                pos.Y = pos.Y / (Single)h;

                pos -= new Vector2(0.5f, 0.5f);

                pos.Y = -pos.Y;

                var state = TouchPhase.Invalid;

                if (pressedThisFrame && !pressedLastFrame)
                {
                    // new press
                    state = TouchPhase.JustPressed;
                }
                else if (pressedLastFrame && pressedThisFrame)
                {
                    // press in progress
                    state = TouchPhase.Active;
                }
                else if (pressedLastFrame && !pressedThisFrame)
                {
                    // released
                    state = TouchPhase.JustReleased;
                }

                if (state != TouchPhase.Invalid)
                {
                    this.collection.RegisterTouch (id, pos, state, time.FrameNumber, time.Elapsed);
                }
            }
            else
            {
                doneFirstUpdateFlag = true;
            }

            previousMouseLeftState = this.input.Mouse.Left;
        }
    }
}
