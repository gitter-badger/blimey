// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! Mono Mac Platform Implementation                                  │ \\
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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

using Abacus;
using Abacus.Packed;
using Abacus.SinglePrecision;
using Abacus.Int32Precision;

using Cor.Lib.Khronos;
using Cor.Platform.Stub;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreVideo;
using MonoMac.CoreGraphics;
using MonoMac.CoreImage;
using MonoMac.ImageIO;
using MonoMac.ImageKit;

namespace Cor.Platform.MonoMac
{
    public sealed class Engine
        : ICor
    {
        readonly AudioManager audio;
        readonly GraphicsManager graphics;
        readonly InputManager input;
        readonly SystemManager system;
        readonly AppSettings settings;
        readonly DisplayStatus displayStatus;
        readonly IApp app;
        readonly LogManager log;
        readonly AssetManager assets;

        public Engine(
            AppSettings settings,
            IApp app,
            Int32 width,
            Int32 height)
        {
            InternalUtils.Log.Info(
                "Engine -> ()");

            this.audio = new AudioManager();
            this.graphics = new GraphicsManager();
            this.input = new InputManager();
            this.system = new SystemManager();
            this.displayStatus = new DisplayStatus (width, height);
            this.settings = settings;
            
            this.log = new LogManager(this.settings.LogSettings);
            this.assets = new AssetManager(this.graphics, this.system);

            this.app = app;
            this.app.Initilise(this);
        }

        internal AudioManager AudioImplementation { get { return this.audio; } }

        internal GraphicsManager GraphicsImplementation { get { return this.graphics; } }

        internal InputManager InputImplementation { get { return this.input; } }

        internal SystemManager SystemImplementation { get { return this.system; } }

        internal DisplayStatus DisplayStatusImplementation { get { return this.displayStatus; } }

        #region ICor

        public IAudioManager Audio { get { return this.audio; } }

        public IGraphicsManager Graphics { get { return this.graphics; } }

        public IDisplayStatus DisplayStatus { get { return this.displayStatus; } }

        public IInputManager Input { get { return this.input; } }

        public ISystemManager System { get { return this.system; } }

        public LogManager Log { get { return this.log; } }

        public AssetManager Assets { get { return this.assets; } }

        public AppSettings Settings { get { return this.settings; } }

        #endregion


        internal Boolean Update(AppTime time)
        {
            return app.Update(time);
        }

        internal void Render()
        {
            app.Render();
        }
    }

    public sealed class AudioManager
        : IAudioManager
    {
        public Single volume = 1f;

        public Single Volume
        {
            get { return this.volume; }
            set
            {
                this.volume = value;

                InternalUtils.Log.Info(
                    "AudioManager -> Setting Volume:" + value);
            }
        }

        #region IAudioManager

        public AudioManager()
        {
            InternalUtils.Log.Info(
                "AudioManager -> ()");

            this.volume = 1f;
        }

        #endregion
    }

    public sealed class InputManager
        : IInputManager
    {
        readonly Keyboard keyboard;
        readonly Mouse mouse;

        readonly IXbox360Gamepad xbox360Gamepad = new StubXbox360Gamepad();
        readonly IPsmGamepad psmGamepad = new StubPsmGamepad();
        readonly IMultiTouchController multiTouchController = new StubMultiTouchController();
        readonly IGenericGamepad genericGamepad = new StubGenericGamepad();

        public InputManager()
        {
            InternalUtils.Log.Info(
                "InputManager -> ()");

            keyboard = new Keyboard();
            mouse = new Mouse();
        }

        internal Keyboard KeyboardImplemenatation { get { return keyboard; } }
        internal Mouse MouseImplemenatation { get { return mouse; } }

        #region IInputManager

        public IXbox360Gamepad Xbox360Gamepad
        {
            get { return xbox360Gamepad; }
        }

        public IPsmGamepad PsmGamepad
        {
            get { return psmGamepad; }
        }

        public IMultiTouchController MultiTouchController
        {
            get { return multiTouchController; }
        }

        public IGenericGamepad GenericGamepad
        {
            get { return genericGamepad; }
        }

        public IMouse Mouse
        {
            get
            {
                return mouse;
            }
        }

        public IKeyboard Keyboard
        {
            get
            {
                return keyboard;
            }
        }

        #endregion
    }

    public sealed class PanelSpecification
        : IPanelSpecification
    {
        public PanelSpecification()
        {
            InternalUtils.Log.Info(
                "PanelSpecification -> ()");
        }

        #region IPanelSpecification

        public Vector2 PanelPhysicalSize
        {
            get { return new Vector2(0.20f, 0.15f ); }
        }

        public Single PanelPhysicalAspectRatio
        {
            get { return PanelPhysicalSize.X / PanelPhysicalSize.Y; }
        }

        public PanelType PanelType { get { return PanelType.TouchScreen; } }

        #endregion
    }

    public sealed class ScreenSpecification
        : IScreenSpecification
    {
        Int32 width = 800;
        Int32 height = 600;

        public ScreenSpecification()
        {
            InternalUtils.Log.Info(
                "ScreenSpecification -> ()");
        }

        #region IScreenSpecification

        public Int32 ScreenResolutionWidth
        {
            get { return width; }
        }

        public Int32 ScreenResolutionHeight
        {
            get { return height; }
        }

        public Single ScreenResolutionAspectRatio
        {
            get
            {
                return
                    (Single)this.ScreenResolutionWidth /
                    (Single)this.ScreenResolutionHeight;
            }
        }

        #endregion
    }

    public sealed class SystemManager
        : ISystemManager
    {
        readonly IScreenSpecification screen;
        readonly IPanelSpecification panel;

        public SystemManager()
        {
            InternalUtils.Log.Info(
                "SystemManager -> ()");

            screen = new ScreenSpecification();
            panel = new PanelSpecification();
        }

        void GetEffectiveDisplaySize(
            ref Int32 screenSpecWidth,
            ref Int32 screenSpecHeight)
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

        static string GetBundlePath(string path)
        {
            string rtype = Path.GetExtension(path);
            string rname = Path.Combine(
                Path.GetDirectoryName(path),
                Path.GetFileNameWithoutExtension(path));

            var correctPath =
                global::MonoMac.Foundation.NSBundle.MainBundle.PathForResource(rname, rtype);

            if(!File.Exists(correctPath))
            {
                throw new FileNotFoundException(correctPath);
            }

            return correctPath;
        }

        #region ISystemManager

        public String OperatingSystem { get { return " OS 2013"; } }

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

        public String DeviceName { get { return "The New  Pad"; } }

        public String DeviceModel { get { return "xf4bs2013"; } }

        public String SystemName { get { return "Sungiant's System"; } }

        public String SystemVersion { get { return "1314.0.1.29"; } }

        public DeviceOrientation CurrentOrientation
        {
            get { return DeviceOrientation.Default; }
        }

        public IScreenSpecification ScreenSpecification
        {
            get { return this.screen; }
        }

        public IPanelSpecification PanelSpecification
        {
            get { return this.panel; }
        }

        public Stream GetAssetStream (String assetId)
        {
            string path = GetBundlePath(Path.Combine("assets/monomac", assetId));

            var fStream = new FileStream(path, FileMode.Open);

            return fStream;
        }

        #endregion
    }

    public sealed class MonoMacApp
        : IDisposable
    {
        MacGameNSWindow mainWindow;
        OpenGLView openGLView;
        readonly AppSettings settings;
        readonly IApp entryPoint;

        public MonoMacApp(AppSettings settings, IApp entryPoint)
        {
            this.settings = settings;
            this.entryPoint = entryPoint;
        }

        void InitializeMainWindow()
        {
            RectangleF frame = new RectangleF(
                0, 0,
                800,
                600);

            mainWindow = new MacGameNSWindow(
                frame,
                NSWindowStyle.Titled |
                NSWindowStyle.Closable |
                NSWindowStyle.Miniaturizable |
                NSWindowStyle.Resizable,
                NSBackingStore.Buffered,
                true);

            mainWindow.Title = this.settings.AppName;

            mainWindow.WindowController = new NSWindowController(mainWindow);
            mainWindow.Delegate = new MainWindowDelegate(this);

            mainWindow.IsOpaque = true;
            mainWindow.EnableCursorRects();
            mainWindow.AcceptsMouseMovedEvents = false;
            mainWindow.Center();

            openGLView = new OpenGLView(this.settings, this.entryPoint, frame);

            mainWindow.ContentView.AddSubview(openGLView);

            mainWindow.MakeKeyAndOrderFront(mainWindow);

            openGLView.StartRunLoop(60f);
        }

        public void Run()
        {
            InitializeMainWindow();
        }

        public void Dispose()
        {
            mainWindow.Dispose ();
            openGLView.Dispose ();
        }

        float GetTitleBarHeight()
        {
            RectangleF contentRect = NSWindow.ContentRectFor(
                mainWindow.Frame, mainWindow.StyleMask);

            return mainWindow.Frame.Height - contentRect.Height;
        }
    }

    [CLSCompliant(false)]
    public sealed class OpenGLView
        : global::MonoMac.OpenGL.MonoMacGameView
    {
        NSTrackingArea trackingArea;

        Engine gameEngine;
        Single elapsedTime;
        Int64 frameCounter = -1;
        TimeSpan previousTimeSpan;

        uint _depthRenderbuffer;

        readonly AppSettings settings;
        readonly IApp entryPoint;
        readonly Stopwatch timer = new Stopwatch();

        //------------------------------------------------------------------------------------------------------------//
        // Init
        //------------------------------------------------------------------------------------------------------------//

        public OpenGLView(AppSettings settings, IApp entryPoint, RectangleF frame)
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


        public void StartRunLoop(double updateRate)
        {
            Run(updateRate);
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
            gameEngine = new Engine(
                this.settings,
                this.entryPoint,
                (Int32) this.Frame.Width,
                (Int32) this.Frame.Height
                );

            timer.Start();

            InternalUtils.Log.Info ("MonoMacGameView.OnLoad");
            base.OnLoad (e);
        }

        protected override void OnRenderFrame (global::MonoMac.OpenGL.FrameEventArgs e)
        {
            try
            {
                gameEngine.Render();
            }
            catch(Exception ex)
            {
                InternalUtils.Log.Error("Failed to render frame:" + ex.Message);
            }

            base.OnRenderFrame (e);
        }

        protected override void OnResize (EventArgs e)
        {
            // Occurs whenever GameWindow is resized.
            // Update the OpenGL Viewport and Projection Matrix here. 
            InternalUtils.Log.Info (
                "MonoMacGameView.OnResize -> Bounds:" + Bounds + 
                ", Frame:" + Frame);

            gameEngine.DisplayStatusImplementation.UpdateSize (
                (Int32)Frame.Width, (Int32)Frame.Height);

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

            var appTime = new AppTime(dt, elapsedTime, ++frameCounter);

            gameEngine.Update(appTime);

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

        public override bool AcceptsFirstResponder ()
        {
            // We want this view to be able to receive key events
            return true;
        }

        public override bool BecomeFirstResponder ()
        {
            return true;
        }

        public override bool EnterFullscreenModeWithOptions (NSScreen screen, NSDictionary options)
        {
            return base.EnterFullscreenModeWithOptions (screen, options);
        }

        public override void ExitFullscreenModeWithOptions (NSDictionary options)
        {
            base.ExitFullscreenModeWithOptions (options);
        }

        public override void ViewWillMoveToWindow (NSWindow newWindow)
        {
            if (trackingArea != null)
                RemoveTrackingArea(trackingArea);

            trackingArea = new NSTrackingArea(
                Frame,
                NSTrackingAreaOptions.MouseMoved |
                NSTrackingAreaOptions.MouseEnteredAndExited |
                NSTrackingAreaOptions.EnabledDuringMouseDrag |
                NSTrackingAreaOptions.ActiveWhenFirstResponder |
                NSTrackingAreaOptions.InVisibleRect |
                NSTrackingAreaOptions.CursorUpdate,
                this,
                new NSDictionary()
            );

            AddTrackingArea(trackingArea);

        }

        // Keyboard //--------------------------------------------------------//
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


        // Mouse //-----------------------------------------------------------//
        public override void MouseDown (NSEvent theEvent)
        {
            base.MouseDown (theEvent);
        }

        public override void MouseUp (NSEvent theEvent)
        {
            base.MouseUp (theEvent);
        }

        public override void MouseDragged (NSEvent theEvent)
        {
            base.MouseDragged (theEvent);
        }

        public override void RightMouseDown (NSEvent theEvent)
        {
            base.RightMouseDown (theEvent);
        }

        public override void RightMouseUp (NSEvent theEvent)
        {
            base.RightMouseUp (theEvent);
        }

        public override void RightMouseDragged (NSEvent theEvent)
        {
            base.RightMouseDragged (theEvent);
        }

        public override void OtherMouseDown (NSEvent theEvent)
        {
            base.OtherMouseDown (theEvent);
        }


        public override void OtherMouseUp (NSEvent theEvent)
        {
            base.OtherMouseUp (theEvent);
        }

        public override void OtherMouseDragged (NSEvent theEvent)
        {
            base.OtherMouseDragged (theEvent);
        }

        public override void ScrollWheel (NSEvent theEvent)
        {
            base.ScrollWheel (theEvent);
        }

        public override void MouseMoved (NSEvent theEvent)
        {
            base.MouseMoved (theEvent);
        }
    }
    public sealed class MacGameNSWindow 
        : NSWindow
    {
        [Export ("initWithContentRect:styleMask:backing:defer:")]
        public MacGameNSWindow (
            RectangleF rect, 
            NSWindowStyle style, 
            NSBackingStore backing, 
            Boolean defer)
            : base (rect, style, backing, defer)
        {

        }

        public override Boolean CanBecomeKeyWindow
        {
            get
            {
                return true;
            }
        }
    }

    internal sealed class MainWindowDelegate 
        : NSWindowDelegate
    {
        private readonly MonoMacApp owner;

        public MainWindowDelegate(MonoMacApp owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");
            this.owner = owner;
        }

        public override bool ShouldZoom (NSWindow window, RectangleF newFrame)
        {
            return true;
        }
    }

    internal static class Vector2Converter
    {
        internal static System.Drawing.PointF ToSystemDrawing(this Vector2 vec)
        {
            return new System.Drawing.PointF (vec.X, vec.Y);
        }

        internal static Vector2 ToAbacus (this System.Drawing.PointF vec)
        {
            return new Vector2 (vec.X, vec.Y);
        }
    }
    public sealed class Keyboard
        : IKeyboard
    {
        readonly HashSet<Char> characterKeysThatAreDown = new HashSet<Char>();
        readonly HashSet<FunctionalKey> functionalKeysThatAreDown = new HashSet<FunctionalKey>();

        internal void KeyDown (NSEvent theEvent)
        {
            theEvent.Characters
                .ToCharArray ()
                .Where(x => !IsFunctionalKey(x))
                .ToList ()
                .ForEach (x => characterKeysThatAreDown.Add(x));

            var fKey = GetFunctionalKey(theEvent.KeyCode);
            if( fKey.HasValue ) functionalKeysThatAreDown.Add(fKey.Value);
        }

        internal void KeyUp (NSEvent theEvent)
        {
            theEvent.Characters
                .ToCharArray ()
                .Where(x => !IsFunctionalKey(x))
                .ToList ()
                .ForEach (x => characterKeysThatAreDown.Remove(x));

            var fKey = GetFunctionalKey(theEvent.KeyCode);
            if( fKey.HasValue ) functionalKeysThatAreDown.Remove(fKey.Value);
        }

        static FunctionalKey? GetFunctionalKey (UInt16 hardwareIndependantKeyCode)
        {
            if (hardwareIndependantKeyCode == 0x24) return FunctionalKey.Enter;
            if (hardwareIndependantKeyCode == 0x7c) return FunctionalKey.Right;
            if (hardwareIndependantKeyCode == 0x7b) return FunctionalKey.Left;
            if (hardwareIndependantKeyCode == 0x7e) return FunctionalKey.Up;
            if (hardwareIndependantKeyCode == 0x7d) return FunctionalKey.Down;
            if (hardwareIndependantKeyCode == 0x31) return FunctionalKey.Spacebar;
            if (hardwareIndependantKeyCode == 0x35) return FunctionalKey.Escape;

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
            return functionalKeysThatAreDown.ToArray();
        }

        public Boolean IsFunctionalKeyDown (FunctionalKey key)
        {
            return functionalKeysThatAreDown.Contains(key);
        }

        public Boolean IsFunctionalKeyUp (FunctionalKey key)
        {
            return !functionalKeysThatAreDown.Contains(key);
        }

        public KeyState this [FunctionalKey key]
        {
            get
            {
                return functionalKeysThatAreDown.Contains(key)
                    ? KeyState.Down
                    : KeyState.Up;
            }
        }

        public Char[] GetPressedCharacterKeys()
        {
            return characterKeysThatAreDown.ToArray();
        }

        public Boolean IsCharacterKeyDown (Char key)
        {
            return characterKeysThatAreDown.Contains(key);
        }

        public Boolean IsCharacterKeyUp (Char key)
        {
            return !characterKeysThatAreDown.Contains(key);
        }

        public KeyState this [Char key]
        {
            get
            {
                return characterKeysThatAreDown.Contains(key)
                    ? KeyState.Down
                    : KeyState.Up;
            }
        }

        #endregion
    }

    public sealed class Mouse
        : IMouse
    {
        public ButtonState Left
        {
            get
            {
                return ButtonState.Released;
            }
        }

        public ButtonState Middle
        {
            get
            {
                return ButtonState.Released;
            }
        }

        public ButtonState Right
        {
            get
            {
                return ButtonState.Released;
            }
        }

        public Int32 ScrollWheelValue
        {
            get
            {
                return 0;
            }
        }

        public Int32 X
        {
            get
            {
                return 0;
            }
        }

        public Int32 Y
        {
            get
            {
                return 0;
            }
        }
    }

    public sealed class DisplayStatus
        : IDisplayStatus
    {
        Int32 width = 0;
        Int32 height = 0;

        public DisplayStatus(Int32 width, Int32 height)
        {
            InternalUtils.Log.Info(
                "DisplayStatus -> ()");

            this.width = width;
            this.height = height;
        }

        internal void UpdateSize (Int32 width, Int32 height)
        {
            this.width = width;
            this.height = height;
        }

        #region IDisplayStatus

        public Boolean Fullscreen { get { return true; } }

        public Int32 CurrentWidth { get { return width; } }

        public Int32 CurrentHeight { get { return height; } }

        #endregion
    }
}
