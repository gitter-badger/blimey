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
    using global::System.Text;
	using global::System.Globalization;
	using global::System.Collections;
	using global::System.Collections.Generic;
	using global::System.Linq;
	using global::System.IO;
	using global::System.Diagnostics;
	using global::System.Runtime.InteropServices;
	using global::System.Runtime.ConstrainedExecution;

    using global::MonoMac.Foundation;
    using global::MonoMac.AppKit;
    using global::MonoMac.CoreVideo;
    using global::MonoMac.CoreGraphics;
    using global::MonoMac.CoreImage;
    using global::MonoMac.ImageIO;
    using global::MonoMac.ImageKit;

    using Fudge;
    using Abacus.SinglePrecision;


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class MonoMacPlatform
        : IPlatform
    {
        public MonoMacPlatform ()
        {
            var program = new MonoMacProgram ();
            var api = new MonoMacApi ();

            api.InitialiseDependencies (program);
            program.InitialiseDependencies (api);

            Api = api;
            Program = program;
        }

        public IProgram Program { get; private set; }
        public IApi Api { get; private set; }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class MonoMacProgram
        : IProgram
    {
        MonoMacApi Api { get; set; }

        internal void InitialiseDependencies (MonoMacApi api) { Api = api; }

        MacGameNSWindow mainWindow;
        OpenGLView openGLView;

        internal OpenGLView OpenGLView { get { return openGLView; } }

        Single GetTitleBarHeight ()
        {
            System.Drawing.RectangleF contentRect = NSWindow.ContentRectFor (mainWindow.Frame, mainWindow.StyleMask);
            return mainWindow.Frame.Height - contentRect.Height;
        }

        internal MonoMacProgram () {}

        public void Start (IApi platformImplementation, Action update, Action render)
        {
            var initialAppSize = new System.Drawing.RectangleF (0, 0, 800, 600);

            mainWindow = new MacGameNSWindow (
                initialAppSize,
                NSWindowStyle.Titled |
                NSWindowStyle.Closable |
                NSWindowStyle.Miniaturizable |
                NSWindowStyle.Resizable,
                NSBackingStore.Buffered,
                true);

            mainWindow.Title = "Cor Blimey!";

            mainWindow.WindowController = new NSWindowController (mainWindow);
            mainWindow.Delegate = new MainWindowDelegate (this);

            mainWindow.IsOpaque = true;
            mainWindow.EnableCursorRects ();
            mainWindow.AcceptsMouseMovedEvents = false;
            mainWindow.Center ();


            openGLView = new OpenGLView (update, render, initialAppSize);


            mainWindow.ContentView.AddSubview (openGLView);

            mainWindow.MakeKeyAndOrderFront (mainWindow);

            openGLView.StartRunLoop (60f);
        }

        public void Stop ()
        {
            openGLView.Stop ();
            openGLView.Close ();
            openGLView.Dispose ();
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

	public partial class MonoMacApi
		: IApi
    {
        MonoMacProgram Program { get; set; }

        internal void InitialiseDependencies (MonoMacProgram program)
        {
            Program = program;
        }

		Single volume = 1f;

		internal MonoMacApi ()
        {
            this.volume = 1f;
        }

        #region IPlatform
        
        #region sfx

        public Single sfx_GetVolume () { return this.volume; }

		public void sfx_SetVolume (Single value)
		{
	        this.volume = value;
        }
        
        #endregion

        #region gfx

        // Partial implementation in Cor.Library.OpenTK.cs

        #endregion
        
        #region res
        
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
        
        #region sys
        
        public String sys_GetMachineIdentifier ()
        {
            return "Machintosh";
        }
        
        public String sys_GetOperatingSystemIdentifier ()
        {
            return "OSX" + Environment.OSVersion.VersionString;
        }
        
        public String sys_GetVirtualMachineIdentifier ()
        {
            return "Mono v?";
        }

        public Int32 sys_GetPrimaryScreenResolutionWidth ()
        {
            return (Int32) NSScreen.MainScreen.Frame.Width;
        }

        public Int32 sys_GetPrimaryScreenResolutionHeight ()
        {
            // TODO: this should return the number of pixels the primary screen has vertically.
            return (Int32) NSScreen.MainScreen.Frame.Height;
        }

        public Vector2? sys_GetPrimaryPanelPhysicalSize ()
        {
            return null;
        }

        public PanelType sys_GetPrimaryPanelType ()
        {
            // Mono Mac is just monitor support atm, phew!
            return PanelType.Screen;
        }
        
        #endregion
        
        #region app

        public Boolean? app_IsFullscreen ()
        {
            return false;
        }
        
        public Int32 app_GetWidth ()
        {
            return (Int32) Program.OpenGLView.Window.Frame.Width;
        }
        
        public Int32 app_GetHeight ()
        {
            return (Int32) Program.OpenGLView.Window.Frame.Height;
        }
        
        #endregion

        #region hid
        
        public DeviceOrientation? hid_GetCurrentOrientation ()
        {
            throw new NotImplementedException ();
        }

        readonly Dictionary<DigitalControlIdentifier, int> digitalControlStates = 
            new Dictionary<DigitalControlIdentifier, int> ();

        readonly Dictionary<AnalogControlIdentifier, float> analogControlStates = 
            new Dictionary<AnalogControlIdentifier, float> ();

        readonly HashSet<RawTouch> touches = new HashSet<RawTouch>();

        public Dictionary<DigitalControlIdentifier, int> hid_GetDigitalControlStates ()
        {
            return digitalControlStates;
        }

        public Dictionary<AnalogControlIdentifier, float> hid_GetAnalogControlStates ()
        {
            return analogControlStates;
        }

        public HashSet<BinaryControlIdentifier> hid_GetBinaryControlStates ()
        {
            return Program.OpenGLView.FunctionalKeysThatAreDown;
        }

        public HashSet<Char> hid_GetPressedCharacters ()
        {
            return Program.OpenGLView.CharacterKeysThatAreDown;
        }

        public HashSet<RawTouch> hid_GetActiveTouches ()
        {
            return touches;
        }
        
        #endregion

        #endregion
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    static class NSKeyboardHelper
    {
        internal static BinaryControlIdentifier? GetFunctionalKey (UInt16 hardwareIndependantKeyCode)
        {
            if (hardwareIndependantKeyCode == 0x24) return BinaryControlIdentifier.Keyboard_Enter;
            if (hardwareIndependantKeyCode == 0x7C) return BinaryControlIdentifier.Keyboard_Right;
            if (hardwareIndependantKeyCode == 0x7B) return BinaryControlIdentifier.Keyboard_Left;
            if (hardwareIndependantKeyCode == 0x7E) return BinaryControlIdentifier.Keyboard_Up;
            if (hardwareIndependantKeyCode == 0x7D) return BinaryControlIdentifier.Keyboard_Down;
            if (hardwareIndependantKeyCode == 0x31) return BinaryControlIdentifier.Keyboard_Spacebar;
            if (hardwareIndependantKeyCode == 0x35) return BinaryControlIdentifier.Keyboard_Escape;
            if (hardwareIndependantKeyCode == 0x30) return BinaryControlIdentifier.Keyboard_Tab;
            if (hardwareIndependantKeyCode == 0x33) return BinaryControlIdentifier.Keyboard_Backspace;
            if (hardwareIndependantKeyCode == 0x74) return BinaryControlIdentifier.Keyboard_PageUp;
            if (hardwareIndependantKeyCode == 0x79) return BinaryControlIdentifier.Keyboard_PageDown;
            if (hardwareIndependantKeyCode == 0x73) return BinaryControlIdentifier.Keyboard_Home;
            if (hardwareIndependantKeyCode == 0x3C) return BinaryControlIdentifier.Keyboard_RightShift;
            if (hardwareIndependantKeyCode == 0x38) return BinaryControlIdentifier.Keyboard_LeftShift;

            if (hardwareIndependantKeyCode == 0x7A) return BinaryControlIdentifier.Keyboard_F1;
            if (hardwareIndependantKeyCode == 0x78) return BinaryControlIdentifier.Keyboard_F2;
            if (hardwareIndependantKeyCode == 0x63) return BinaryControlIdentifier.Keyboard_F3;
            if (hardwareIndependantKeyCode == 0x76) return BinaryControlIdentifier.Keyboard_F4;
            if (hardwareIndependantKeyCode == 0x60) return BinaryControlIdentifier.Keyboard_F5;
            if (hardwareIndependantKeyCode == 0x61) return BinaryControlIdentifier.Keyboard_F6;
            if (hardwareIndependantKeyCode == 0x62) return BinaryControlIdentifier.Keyboard_F7;
            if (hardwareIndependantKeyCode == 0x64) return BinaryControlIdentifier.Keyboard_F8;
            if (hardwareIndependantKeyCode == 0x65) return BinaryControlIdentifier.Keyboard_F9;
            if (hardwareIndependantKeyCode == 0x6D) return BinaryControlIdentifier.Keyboard_F10;
            if (hardwareIndependantKeyCode == 0x67) return BinaryControlIdentifier.Keyboard_F11;
            if (hardwareIndependantKeyCode == 0x6F) return BinaryControlIdentifier.Keyboard_F12;
            if (hardwareIndependantKeyCode == 0x69) return BinaryControlIdentifier.Keyboard_F13;
            if (hardwareIndependantKeyCode == 0x6B) return BinaryControlIdentifier.Keyboard_F14;
            if (hardwareIndependantKeyCode == 0x71) return BinaryControlIdentifier.Keyboard_F15;
            if (hardwareIndependantKeyCode == 0x6A) return BinaryControlIdentifier.Keyboard_F16;
            if (hardwareIndependantKeyCode == 0x40) return BinaryControlIdentifier.Keyboard_F17;
            if (hardwareIndependantKeyCode == 0x4F) return BinaryControlIdentifier.Keyboard_F18;
            if (hardwareIndependantKeyCode == 0x50) return BinaryControlIdentifier.Keyboard_F19;
            if (hardwareIndependantKeyCode == 0x5A) return BinaryControlIdentifier.Keyboard_F20;


            //kVK_Command                   = 0x37,
            //kVK_CapsLock                  = 0x39,
            //kVK_Option                    = 0x3A,
            //kVK_Control                   = 0x3B,
            //kVK_RightOption               = 0x3D,
            //kVK_RightControl              = 0x3E,
            //kVK_Function                  = 0x3F,
            //kVK_VolumeUp                  = 0x48,
            //kVK_VolumeDown                = 0x49,
            //kVK_Mute                      = 0x4A,
            //kVK_Help                      = 0x72,
            //kVK_ForwardDelete             = 0x75,
            //kVK_End                       = 0x77,


            return null;
        }

        internal static Boolean IsFunctionalKey (Char c)
        {
            if (c == '\r') return true;
            if (c == '\n') return true;
            if (c == '\t') return true;

            return false;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    [CLSCompliant (false)]
    public sealed class OpenGLView
        : global::MonoMac.OpenGL.MonoMacGameView
    {
        NSTrackingArea trackingArea;

        readonly Action update;
        readonly Action render;

        public HashSet<Char> CharacterKeysThatAreDown { get { return characterKeysThatAreDown; } }
        public HashSet<BinaryControlIdentifier> FunctionalKeysThatAreDown { get { return functionalKeysThatAreDown; } }

        readonly HashSet<Char> characterKeysThatAreDown = new HashSet<Char>();
        readonly HashSet<BinaryControlIdentifier> functionalKeysThatAreDown = new HashSet<BinaryControlIdentifier>();

        //------------------------------------------------------------------------------------------------------------//
        // Init
        //------------------------------------------------------------------------------------------------------------//

        public OpenGLView (Action update, Action render, System.Drawing.RectangleF frame)
            : base (frame)
        {
            this.update = update;
            this.render = render;

            // Make the suface size automatically update when the window
            // size changes.
            this.WantsBestResolutionOpenGLSurface = true;

            this.AutoresizingMask =
                global::MonoMac.AppKit.NSViewResizingMask.HeightSizable |
                global::MonoMac.AppKit.NSViewResizingMask.MaxXMargin |
                global::MonoMac.AppKit.NSViewResizingMask.MinYMargin |
                global::MonoMac.AppKit.NSViewResizingMask.WidthSizable;

            MakeCurrent ();
        }

        public override void MakeCurrent ()
        {
            base.MakeCurrent ();
            OpenTKHelper.InitilizeRenderSettings ();
            // todo: I'm not sure how to properly set up render buffers with the Mono Mac game view thing
            //       perhaps it is worth taking this approach:
            //       https://github.com/xamarin/mac-samples/blob/master/OpenGL-NeHe/NeHeLesson17/MyOpenGLView.cs
            //OpenTKHelper.InitilizeRenderTargets (Size.Width, Size.Height);
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
            Console.WriteLine ("MonoMacGameView.OnClosed");
            base.OnClosed (e);
        }

        protected override void OnDisposed (EventArgs e)
        {
            Console.WriteLine ("MonoMacGameView.OnDisposed");
            base.OnDisposed (e);
        }

        protected override void OnLoad (EventArgs e)
        {
            Console.WriteLine ("MonoMacGameView.OnLoad");
            base.OnLoad (e);
        }

        protected override void OnRenderFrame (global::MonoMac.OpenGL.FrameEventArgs e)
        {
            try
            {
                render ();
            }
            catch (Exception ex)
            {
                Console.WriteLine ("Failed to render frame: " + ex.GetType() + " ~ " + ex.Message + "\n" + ex.StackTrace);
            }

            base.OnRenderFrame (e);
        }

        protected override void OnResize (EventArgs e)
        {
            // Occurs whenever GameWindow is resized.
            // Update the OpenGL Viewport and Projection Matrix here.
            Console.WriteLine ("MonoMacGameView.OnResize -> Bounds:" + Bounds + ", Frame:" + Frame);
            base.OnResize (e);
        }

        protected override void OnTitleChanged (EventArgs e)
        {
            Console.WriteLine ("MonoMacGameView.OnTitleChanged");
            base.OnTitleChanged (e);
        }

        protected override void OnUnload (EventArgs e)
        {
            Console.WriteLine ("MonoMacGameView.OnUnload");
            base.OnUnload (e);
        }

        protected override void OnUpdateFrame (global::MonoMac.OpenGL.FrameEventArgs fea)
        {
            update ();

            base.OnUpdateFrame (fea);
        }

        protected override void OnVisibleChanged (EventArgs e)
        {
            Console.WriteLine ("MonoMacGameView.OnVisibleChanged");
            base.OnVisibleChanged (e);
        }

        protected override void OnWindowStateChanged (EventArgs e)
        {
            Console.WriteLine ("MonoMacGameView.OnWindowStateChanged");
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
            theEvent.Characters
                .ToCharArray ()
                .Where (x => !NSKeyboardHelper.IsFunctionalKey (x))
                .ToList ()
                .ForEach (x => characterKeysThatAreDown.Add (x));

            var fKey = NSKeyboardHelper.GetFunctionalKey (theEvent.KeyCode);
            if (fKey.HasValue) functionalKeysThatAreDown.Add (fKey.Value);
        }

        public override void KeyUp (NSEvent theEvent)
        {
            theEvent.Characters
                .ToCharArray ()
                .Where (x => !NSKeyboardHelper.IsFunctionalKey (x))
                .ToList ()
                .ForEach (x => characterKeysThatAreDown.Remove (x));

            var fKey = NSKeyboardHelper.GetFunctionalKey (theEvent.KeyCode);
            if (fKey.HasValue) functionalKeysThatAreDown.Remove (fKey.Value);
        }

        public override void FlagsChanged (NSEvent theEvent)
        {
            base.FlagsChanged (theEvent);
        }


        // Mouse //---------------------------------------------------------------------------------------------------//
        public override void MouseDown (NSEvent theEvent)
        {
            //this.gameEngine.InputImplementation.MouseImplemenatation.LeftMouseDown (theEvent);
        }

        public override void MouseUp (NSEvent theEvent)
        {
            //this.gameEngine.InputImplementation.MouseImplemenatation.LeftMouseUp (theEvent);
        }

        public override void MouseDragged (NSEvent theEvent)
        {
            base.MouseDragged (theEvent);
        }

        public override void RightMouseDown (NSEvent theEvent)
        {
            //this.gameEngine.InputImplementation.MouseImplemenatation.RightMouseDown (theEvent);
        }

        public override void RightMouseUp (NSEvent theEvent)
        {
            //this.gameEngine.InputImplementation.MouseImplemenatation.RightMouseUp (theEvent);
        }

        public override void RightMouseDragged (NSEvent theEvent)
        {
            base.RightMouseDragged (theEvent);
        }

        public override void OtherMouseDown (NSEvent theEvent)
        {
            //this.gameEngine.InputImplementation.MouseImplemenatation.MiddleMouseDown (theEvent);
        }


        public override void OtherMouseUp (NSEvent theEvent)
        {
            //this.gameEngine.InputImplementation.MouseImplemenatation.MiddletMouseUp (theEvent);
        }

        public override void OtherMouseDragged (NSEvent theEvent)
        {
            base.OtherMouseDragged (theEvent);
        }

        public override void ScrollWheel (NSEvent theEvent)
        {
            //this.gameEngine.InputImplementation.MouseImplemenatation.ScrollWheel (theEvent);
        }

        public override void MouseMoved (NSEvent theEvent)
        {
            //this.gameEngine.InputImplementation.MouseImplemenatation.MouseMoved (theEvent);
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

    sealed class MainWindowDelegate
        : NSWindowDelegate
    {
        readonly MonoMacProgram owner;

        public MainWindowDelegate (MonoMacProgram owner)
        {
            if (owner == null) throw new ArgumentNullException ("owner");
            this.owner = owner;
        }

        public override Boolean ShouldZoom (NSWindow window, System.Drawing.RectangleF newFrame)
        {
            return true;
        }

        public override void WillClose (NSNotification notification)
        {
            owner.Stop ();
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    static class Vector2Converter
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
}
