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
    using Cor.Library.OTK;


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

        void InitializeMainWindow (Action update, Action render)
        {
            var frame = new System.Drawing.RectangleF (0, 0, 800, 600);

            mainWindow = new MacGameNSWindow (
                frame,
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

            openGLView = new OpenGLView (update, render, frame);

            mainWindow.ContentView.AddSubview (openGLView);

            mainWindow.MakeKeyAndOrderFront (mainWindow);

            openGLView.StartRunLoop (60f);
        }

        Single GetTitleBarHeight ()
        {
            System.Drawing.RectangleF contentRect = NSWindow.ContentRectFor (mainWindow.Frame, mainWindow.StyleMask);
            return mainWindow.Frame.Height - contentRect.Height;
        }

        internal MonoMacProgram () {}

        public void Start (IApi platformImplementation, Action update, Action render)
        {
            InitializeMainWindow (update, render);
        }

        public void Stop ()
        {
            openGLView.Stop ();
            openGLView.Close ();
            openGLView.Dispose ();

            mainWindow.Close ();
            mainWindow.Dispose ();
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

	public class MonoMacApi
		: IApi
    {
        MonoMacProgram Program { get; set; }

        internal void InitialiseDependencies (MonoMacProgram program) { Program = program; }

		Single volume = 1f;

        static VertexDeclaration currentVertexDeclaration;

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
        
        public void gfx_ClearColourBuffer (Rgba32 colour)
        {
            OpenTkWrapper.ClearColourBuffer (colour);
        }
        
        public void gfx_ClearDepthBuffer (Single depth)
        {
            OpenTkWrapper.ClearDepthBuffer (depth);
        }
        
        public void gfx_SetCullMode (CullMode cullMode)
        {
            OpenTkWrapper.SetCullMode (cullMode);
        }
        
        public void gfx_SetBlendEquation (
            BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb, 
            BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha)
        {
            OpenTkWrapper.SetBlendEquation (
                rgbBlendFunction, sourceRgb, destinationRgb, alphaBlendFunction, sourceAlpha, destinationAlpha);
        }
        

        public Handle gfx_CreateVertexBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount)
        {
            return OpenTkWrapper.CreateVertexBuffer (vertexDeclaration, vertexCount) as Handle;
        }
        
        public Handle gfx_CreateIndexBuffer (Int32 indexCount)
        {
            return OpenTkWrapper.CreateIndexBuffer (indexCount) as Handle;
        }
        
        public Handle gfx_CreateTexture (TextureFormat textureFormat, Int32 width, Int32 height, Byte[] source)
        {
            throw new NotImplementedException ();
        }
        
        public Handle gfx_CreateShader (ShaderDeclaration shaderDeclaration, ShaderFormat shaderFormat, Byte[][] sources)
        {
            if (shaderFormat != ShaderFormat.GLSL)
                throw new NotSupportedException ();

            return OpenTkWrapper.CreateShader (shaderDeclaration, sources);
        }

        public void gfx_DestroyVertexBuffer (Handle handle)
        {
            OpenTkWrapper.DestroyVertexBuffer (handle as VertexBufferHandle);
        }
        
        public void gfx_DestroyIndexBuffer (Handle handle)
        {
            OpenTkWrapper.DestroyIndexBuffer (handle as IndexBufferHandle);
        }
        
        public void gfx_DestroyTexture (Handle handle)
        {
            throw new NotImplementedException ();
        }
        
        public void gfx_DestroyShader (Handle handle)
        {
            throw new NotImplementedException ();
        }

        public void gfx_vbff_Activate (Handle handle)
        {
            var vd = OpenTkCache.Get <VertexDeclaration> (handle, "VertexDeclaration");

            // Keep track of this for later draw calls that do not provide it.
            currentVertexDeclaration = vd;

            OpenTkWrapper.ActivateVertexBuffer (handle as VertexBufferHandle);
        }
        
        public void gfx_ibff_Activate (Handle handle)
        {
            OpenTkWrapper.ActivateIndexBuffer (handle as IndexBufferHandle);
        }

        public void gfx_DrawPrimitives (
            PrimitiveType primitiveType,
            Int32 startVertex,
            Int32 primitiveCount)
        {
            throw new NotImplementedException ();
        }
        
        public void gfx_DrawIndexedPrimitives (
            PrimitiveType primitiveType, 
            Int32 baseVertex, 
            Int32 minVertexIndex,
            Int32 numVertices, 
            Int32 startIndex, 
            Int32 primitiveCount)
        {
            OpenTkWrapper.DrawIndexedPrimitives (
                primitiveType, baseVertex, minVertexIndex, numVertices, 
                startIndex, primitiveCount, currentVertexDeclaration);
        }
        
        public void gfx_DrawUserPrimitives <T> (
            PrimitiveType primitiveType, 
            T[] vertexData, 
            Int32 vertexOffset,
            Int32 primitiveCount) 
        where T
            : struct
            , IVertexType
        {
            OpenTkWrapper.DrawUserPrimitives (
                primitiveType, vertexData, vertexOffset, 
                primitiveCount, currentVertexDeclaration);
        }
        
        public void gfx_DrawUserIndexedPrimitives <T> (
            PrimitiveType primitiveType, 
            T[] vertexData, 
            Int32 vertexOffset, 
            Int32 numVertices, 
            Int32[] indexData, 
            Int32 indexOffset, 
            Int32 primitiveCount) 
        where T
            : struct
        , IVertexType
        {
            throw new NotImplementedException ();
        }

        public Byte[] gfx_CompileShader (String source)
        {
            throw new NotImplementedException ();
        }

        public Int32 gfx_dbg_BeginEvent (Rgba32 colour, String eventName)
        {
            throw new NotImplementedException ();
        }
        
        public Int32 gfx_dbg_EndEvent ()
        {
            throw new NotImplementedException ();
        }
        
        public void gfx_dbg_SetMarker (Rgba32 colour, String marker)
        {
            throw new NotImplementedException ();
        }
        
        public void gfx_dbg_SetRegion (Rgba32 colour, String region)
        {
            throw new NotImplementedException ();
        }

        public Int32 gfx_vbff_GetVertexCount (Handle h)
        {
            return OpenTkCache.Get <Int32> (h, "VertexCount");
        }
        
        public VertexDeclaration gfx_vbff_GetVertexDeclaration (Handle h)
        {
            return OpenTkCache.Get <VertexDeclaration> (h, "VertexDeclaration");
        }
        
        public void gfx_vbff_SetData<T> (Handle h, T[] data, Int32 startIndex, Int32 elementCount) 
        where T
            : struct
            , IVertexType
        {
            OpenTkWrapper.SetVertexBufferData (h as VertexBufferHandle, data, startIndex, elementCount);
        }
        
        public T[] gfx_vbff_GetData<T> (Handle h, Int32 startIndex, Int32 elementCount)
        where T
            : struct
            , IVertexType
        {
            throw new NotImplementedException ();
        }

        public Int32 gfx_ibff_GetIndexCount (Handle h)
        {
            return OpenTkCache.Get <Int32> (h, "IndexCount");
        }
        
        public void gfx_ibff_SetData (Handle h, Int32[] data, Int32 startIndex, Int32 elementCount)
        {
            OpenTkWrapper.SetIndexBufferData (h as IndexBufferHandle, data, startIndex, elementCount);
        }
        
        public void gfx_ibff_GetData (Handle h, Int32[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException ();
        }

        public Int32 gfx_tex_GetWidth (Handle h)
        {
            return OpenTkCache.Get <Int32> (h, "Width");
        }
        
        public Int32 gfx_tex_GetHeight (Handle h)
        {
            return OpenTkCache.Get <Int32> (h, "Height");
        }

        public Byte[] gfx_tex_GetData (Handle h)
        {
            throw new NotImplementedException ();
        }

        public TextureFormat gfx_tex_GetTextureFormat (Handle h)
        {
            return OpenTkCache.Get <TextureFormat> (h, "TextureFormat");
        }
        
        public void gfx_shdr_SetVariable<T> (Handle h, Int32 variantIndex, String name, T value)
        {
            OpenTkWrapper.SetVariable (h as ShaderHandle, variantIndex, name, value);
        }
        
        public void gfx_shdr_SetSampler (Handle h, Int32 variantIndex, String name, Handle textureHandle)
        {
            OpenTkWrapper.SetSampler (h as ShaderHandle, variantIndex, name, textureHandle);
        }

        public void gfx_shdr_Activate (Handle h, Int32 variantIndex)
        {
            OpenTkWrapper.Activate (h as ShaderHandle, variantIndex);
        }

        public Int32 gfx_shdr_GetVariantCount (Handle h)
        {
            return OpenTkWrapper.GetVariantCount (h as ShaderHandle);
        }

        public String gfx_shdr_GetIdentifier (Handle h, Int32 variantIndex)
        {
            return OpenTkWrapper.GetIdentifier (h as ShaderHandle, variantIndex);
        }

        public ShaderInputInfo[] gfx_shdr_GetInputs (Handle h, Int32 variantIndex)
        {
            return OpenTkWrapper.GetInputs (h as ShaderHandle, variantIndex);
        }

        public ShaderVariableInfo[] gfx_shdr_GetVariables (Handle h, Int32 variantIndex)
        {
            return OpenTkWrapper.GetVariables (h as ShaderHandle, variantIndex);
        }

        public ShaderSamplerInfo[] gfx_shdr_GetSamplers (Handle h, Int32 variantIndex)
        {
            return OpenTkWrapper.GetSamplers (h as ShaderHandle, variantIndex);
        }

        public void gfx_Reset ()
        {
            throw new NotImplementedException ();
        }

        public void gfx_tex_Activate (int slot, Handle textureHandle)
        {
            throw new NotImplementedException ();
        }

        public int[] gfx_ibff_GetData (Handle indexBufferHandle, int startIndex, int elementCount)
        {
            throw new NotImplementedException ();
        }
   
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

        public int sys_GetPrimaryScreenResolutionWidth ()
        {
            // TODO: this should return the number of pixels the primary screen has horizontally.
            return 800;
        }

        public int sys_GetPrimaryScreenResolutionHeight ()
        {
            // TODO: this should return the number of pixels the primary screen has vertically.
            return 600;
        }

        public Vector2? sys_GetPrimaryPanelPhysicalSize ()
        {
            // TODO: this is total guess atm
            return new Vector2 (0.32f, 0.18f);
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
            throw new NotImplementedException ();
        }
        
        public Int32 app_GetWidth ()
        {
            throw new NotImplementedException ();
        }
        
        public Int32 app_GetHeight ()
        {
            throw new NotImplementedException ();
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
            //gameEngine = new Engine (
            //    this.settings, this.entryPoint, (Int32) this.Frame.Width, (Int32) this.Frame.Height);

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

            //gameEngine.DisplayStatusImplementation.UpdateSize ((Int32)Frame.Width, (Int32)Frame.Height);

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
