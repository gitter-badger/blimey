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
// │ Copyright © 2013 A.J.Pook (http://sungiant.github.com)                 │ \\
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
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Diagnostics;

using Sungiant.Abacus;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Abacus.Int32Precision;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreVideo;
using MonoMac.CoreGraphics;

namespace Sungiant.Cor.Platform.Managed.MonoMac
{
    public class Engine
        : ICor
    {
        IAudioManager audio;
        IGraphicsManager graphics;
        IResourceManager resources;
        IInputManager input;
        ISystemManager system;
        AppSettings settings;

        public Engine(AppSettings settings)
        {
            Console.WriteLine(
                "Engine -> ()");
            
            this.audio = new AudioManager();
            this.graphics = new GraphicsManager();
            this.resources = new ResourceManager();
            this.input = new InputManager();
            this.system = new SystemManager();
            this.settings = settings;
        }

        #region ICor

        public IAudioManager Audio { get { return this.audio; } }

        public IGraphicsManager Graphics { get { return this.graphics; } }

        public IResourceManager Resources { get { return this.resources; } }

        public IInputManager Input { get { return this.input; } }

        public ISystemManager System { get { return this.system; } }

        public AppSettings Settings { get { return this.settings; } }

        #endregion
    }

    public class AudioManager
        : IAudioManager
    {
        public Single volume = 1f;

        public Single Volume
        { 
            get { return this.volume; }
            set
            {
                this.volume = value;

                Console.WriteLine(
                    "AudioManager -> Setting Volume:" + value);
            } 
        }

        #region IAudioManager

        public AudioManager()
        {
            Console.WriteLine(
                "AudioManager -> ()");

            this.volume = 1f;
        }

        #endregion
    }

    public class GraphicsManager
        : IGraphicsManager
    {
        IDisplayStatus displayStatus;

        public GraphicsManager()
        {
            Console.WriteLine(
                "GraphicsManager -> ()");

            this.displayStatus = new DisplayStatus();
        }

        #region IGraphicsManager

        public IDisplayStatus DisplayStatus { get { return this.displayStatus; } }

        public IGpuUtils GpuUtils { get { return null; } }

        public void Reset()
        {
            this.ClearDepthBuffer();
            this.ClearColourBuffer();
            this.SetActiveGeometryBuffer(null);

            // todo, here we need to set all the texture slots to point to null
            this.SetActiveTexture(0, null);
        }

        public void ClearColourBuffer(Rgba32 col = new Rgba32())
        {
            Vector4 c;

            col.UnpackTo(out c);

            global::MonoMac.OpenGL.GL.ClearColor (c.X, c.Y, c.Z, c.W);

            var mask = global::MonoMac.OpenGL.ClearBufferMask.ColorBufferBit;

            global::MonoMac.OpenGL.GL.Clear ( mask );

            OpenGLHelper.CheckError();
        }

        public void ClearDepthBuffer(Single val = 1)
        {
            global::MonoMac.OpenGL.GL.ClearDepth(val);

            var mask = global::MonoMac.OpenGL.ClearBufferMask.DepthBufferBit;

            global::MonoMac.OpenGL.GL.Clear ( mask );

            OpenGLHelper.CheckError();
        }

        public void SetCullMode(CullMode cullMode)
        {
            
        }

        public IGeometryBuffer CreateGeometryBuffer (
            VertexDeclaration vertexDeclaration,
            Int32 vertexCount,
            Int32 indexCount )
        {
            return new GeometryBuffer(vertexDeclaration, vertexCount, indexCount);
        }

        public void SetActiveGeometryBuffer(IGeometryBuffer buffer)
        {
            
        }

        public void SetActiveTexture(Int32 slot, Texture2D tex)
        {
            
        }

        public void SetBlendEquation(
            BlendFunction rgbBlendFunction, 
            BlendFactor sourceRgb, 
            BlendFactor destinationRgb,
            BlendFunction alphaBlendFunction, 
            BlendFactor sourceAlpha, 
            BlendFactor destinationAlpha
            )
        {
            global::MonoMac.OpenGL.GL.BlendEquationSeparate(
                EnumConverter.ToOpenGL(rgbBlendFunction),
                EnumConverter.ToOpenGL(alphaBlendFunction) );
            OpenGLHelper.CheckError();

            global::MonoMac.OpenGL.GL.BlendFuncSeparate(
                EnumConverter.ToOpenGLSrc(sourceRgb),
                EnumConverter.ToOpenGLDest(destinationRgb),
                EnumConverter.ToOpenGLSrc(sourceAlpha),
                EnumConverter.ToOpenGLDest(destinationAlpha) );
            OpenGLHelper.CheckError();
        }

        public void DrawPrimitives(
            PrimitiveType primitiveType,            
            Int32 startVertex,                      
            Int32 primitiveCount )
        {
            
        }              

        public void DrawIndexedPrimitives (
            PrimitiveType primitiveType,
            Int32 baseVertex,
            Int32 minVertexIndex,
            Int32 numVertices,
            Int32 startIndex,
            Int32 primitiveCount
            )
        {
            
        }

        public void DrawUserPrimitives <T> (
            PrimitiveType primitiveType,
            T[] vertexData,
            Int32 vertexOffset,
            Int32 primitiveCount,
            VertexDeclaration vertexDeclaration )
            where T : struct, IVertexType
        {
            
        }

        public void DrawUserIndexedPrimitives <T> (
            PrimitiveType primitiveType,
            T[] vertexData,
            Int32 vertexOffset,
            Int32 numVertices,
            Int32[] indexData,
            Int32 indexOffset,
            Int32 primitiveCount,
            VertexDeclaration vertexDeclaration ) 
            where T : struct, IVertexType
        {

        }

        #endregion
    }

    public class DisplayStatus
        : IDisplayStatus
    {
        public DisplayStatus()
        {
            Console.WriteLine(
                "DisplayStatus -> ()");
        }

        #region IDisplayStatus

        public Boolean Fullscreen { get { return true; } }

        public Int32 CurrentWidth { get { return 800; } }

        public Int32 CurrentHeight { get { return 600; } }

        #endregion
    }    public class IndexBuffer
        : IIndexBuffer
    {
        UInt16[] data;

        public IndexBuffer()
        {
            Console.WriteLine(
                "IndexBuffer -> ()");
        }

        static internal UInt16[] ConvertToUnsigned (Int32[] indexBuffer)
        {   
            UInt16[] udata = new UInt16[indexBuffer.Length];

            for(Int32 i = 0; i < indexBuffer.Length; ++i)
            {
                udata[i] = (UInt16) indexBuffer[i];
            }
            
            return udata;
        }

        #region IIndexBuffer

        public Int32 IndexCount { get { return this.data.Length; } }

        public void SetData(Int32[] data)
        {
            this.data = ConvertToUnsigned(data);
        }

        #endregion
    }
    public class InputManager
        : IInputManager
    {
        public InputManager()
        {
            Console.WriteLine(
                "InputManager -> ()");
        }

        #region IInputManager

        public Xbox360Gamepad GetXbox360Gamepad(PlayerIndex player)
        {
            return null;
        }

        public PsmGamepad GetPsmGamepad()
        {
            return null;
        }

        public MultiTouchController GetMultiTouchController()
        {
            return null;
        }

        public GenericGamepad GetGenericGamepad()
        {
            return null;
        }

        #endregion
    }

    public class ResourceManager
        : IResourceManager
    {
        public ResourceManager()
        {
            Console.WriteLine(
                "ResourceManager -> ()");
        }

        #region IResourceManager

        public T Load<T>(String path) where T : IResource
        {
            return default(T);
        }

        public IShader LoadShader(ShaderType shaderType)
        {
            return null;
        }

        #endregion
    }

    public class PanelSpecification
        : IPanelSpecification
    {
        public PanelSpecification()
        {
            Console.WriteLine(
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

    public class ScreenSpecification
        : IScreenSpecification
    {
        Int32 width = 800;
        Int32 height = 600;

        public ScreenSpecification()
        {
            Console.WriteLine(
                "ScreenSpecification -> ()");
        }

        #region IScreenSpecification

        public virtual Int32 ScreenResolutionWidth
        { 
            get { return width; }
        }
        
        public virtual Int32 ScreenResolutionHeight
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

    public class GeometryBuffer
        : IGeometryBuffer
    {
        IVertexBuffer vertexBuffer;
        IIndexBuffer indexBuffer;

        public GeometryBuffer(
            VertexDeclaration vertexDeclaration,
            Int32 vertexCount,
            Int32 indexCount )
        {
            Console.WriteLine(
                "GeometryBuffer -> ()");

            this.vertexBuffer = new VertexBuffer();
            this.indexBuffer = new IndexBuffer();
        }

        #region IGeometryBuffer

        public IVertexBuffer VertexBuffer
        {
            get { return this.vertexBuffer; }
        }

        public IIndexBuffer IndexBuffer
        {
            get { return this.indexBuffer; }
        }

        #endregion
    }
    public class SystemManager
        : ISystemManager
    {
        readonly IScreenSpecification screen;
        readonly IPanelSpecification panel;

        public SystemManager()
        {
            Console.WriteLine(
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

        #endregion
    }

    public class VertexBuffer
        : IVertexBuffer
    {
        public VertexBuffer()
        {
            Console.WriteLine(
                "VertexBuffer -> ()");
        }

        #region IVertexBuffer

        public Int32 VertexCount { get { return 0; } }

        public VertexDeclaration VertexDeclaration 
        { 
            get { return null; }
        }

        public void SetData<T> (T[] data) 
            where T: 
                struct, 
                IVertexType
        {

        }

        #endregion
    }

    public class MonoMacApp
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
                NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable,
                NSBackingStore.Buffered,
                true);

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
            // No need to dispose openGLView or mainWindow.  They will be released by the
            // nearest NSAutoreleasePool.
        }

        float GetTitleBarHeight()
        {
            RectangleF contentRect = NSWindow.ContentRectFor(
                mainWindow.Frame, mainWindow.StyleMask);

            return mainWindow.Frame.Height - contentRect.Height;
        }
    }

    public class OpenGLView 
        : global::MonoMac.OpenGL.MonoMacGameView
    {
        Rectangle clientBounds;
        NSTrackingArea trackingArea;
        bool _needsToResetElapsedTime = false;
        Scene scene;
        
        public OpenGLView(AppSettings settings, IApp entryPoint, RectangleF frame) 
            : base (frame)
        {
            Init();
        }

        [Export("initWithFrame:")]
        public OpenGLView () 
            : base (NSScreen.MainScreen.Frame)
        {
            Init();
        }


        void Init()
        {
            this.AutoresizingMask = 
                global::MonoMac.AppKit.NSViewResizingMask.HeightSizable |
                global::MonoMac.AppKit.NSViewResizingMask.MaxXMargin |
                global::MonoMac.AppKit.NSViewResizingMask.MinYMargin |
                global::MonoMac.AppKit.NSViewResizingMask.WidthSizable;

            RectangleF rect = NSScreen.MainScreen.Frame;
            clientBounds = new Rectangle (0,0,(int)rect.Width,(int)rect.Height);

            scene = new Scene();
            
            Resize += delegate {
                scene.ResizeGLScene(Bounds);    
            };
            
            Load += loader;
            
            UpdateFrame += delegate(object src, global::MonoMac.OpenGL.FrameEventArgs fea) {
                Console.WriteLine("update " + fea.Time);    
            };
            
            RenderFrame += delegate(object src, global::MonoMac.OpenGL.FrameEventArgs fea) {
                scene.DrawGLScene();
            };
            
        }
        
        void loader (object src, EventArgs fea)
        {
            //Console.WriteLine("load ");   
            InitGL();
            UpdateView();
            
        }
        
        // All Setup For OpenGL Goes Here
        bool InitGL ()
        {
            // Enables Smooth Shading  
            global::MonoMac.OpenGL.GL.ShadeModel (global::MonoMac.OpenGL.ShadingModel.Smooth);
            // Set background color to black     
            global::MonoMac.OpenGL.GL.ClearColor (Color.Black);

            // Setup Depth Testing

            // Depth Buffer setup
            global::MonoMac.OpenGL.GL.ClearDepth (1.0);
            // Enables Depth testing
            global::MonoMac.OpenGL.GL.Enable (global::MonoMac.OpenGL.EnableCap.DepthTest);
            // The type of depth testing to do
            global::MonoMac.OpenGL.GL.DepthFunc (global::MonoMac.OpenGL.DepthFunction.Lequal);

            // Really Nice Perspective Calculations
            global::MonoMac.OpenGL.GL.Hint (global::MonoMac.OpenGL.HintTarget.PerspectiveCorrectionHint, global::MonoMac.OpenGL.HintMode.Nicest);
            
            return true;
        }
        
        //[Export("toggleFullScreen:")]
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

        public void ResetElapsedTime ()
        {
            _needsToResetElapsedTime = true;
        }

        protected override void OnRenderFrame (global::MonoMac.OpenGL.FrameEventArgs e)
        {
            base.OnRenderFrame (e);
            // tick
        }

        public override bool AcceptsFirstResponder ()
        {
            // We want this view to be able to receive key events
            return true;
        }

        public override bool BecomeFirstResponder ()
        {
            return true;
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


    }

    public class MacGameNSWindow 
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

    class MainWindowDelegate 
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

    public static class OpenGLHelper
    {
        [Conditional("DEBUG")]
        public static void CheckError()
        {
            var ec = global::MonoMac.OpenGL.GL.GetError();

            if (ec != global::MonoMac.OpenGL.ErrorCode.NoError)
            {
                throw new Exception( ec.ToString());
            }
        }
    }

    public static class Vector2Converter
    {
        // VECTOR 2
        public static global::MonoMac.OpenGL.Vector2 ToOpenGL (this Vector2 vec)
        {
            return new global::MonoMac.OpenGL.Vector2 (vec.X, vec.Y);
        }

        public static Vector2 ToAbacus (this global::MonoMac.OpenGL.Vector2 vec)
        {
            return new Vector2 (vec.X, vec.Y);
        }

        
        public static System.Drawing.PointF ToSystemDrawing(this Vector2 vec)
        {
            return new System.Drawing.PointF (vec.X, vec.Y);
        }

        public static Vector2 ToAbacus (this System.Drawing.PointF vec)
        {
            return new Vector2 (vec.X, vec.Y);
        }
    }

    
    public static class Vector3Converter
    {
        // VECTOR 3
        public static global::MonoMac.OpenGL.Vector3 ToOpenGL (this Vector3 vec)
        {
            return new global::MonoMac.OpenGL.Vector3 (vec.X, vec.Y, vec.Z);
        }

        public static Vector3 ToAbacus (this global::MonoMac.OpenGL.Vector3 vec)
        {
            return new Vector3 (vec.X, vec.Y, vec.Z);
        }
    }
    
    public static class Vector4Converter
    {
        // VECTOR 3
        public static global::MonoMac.OpenGL.Vector4 ToOpenGL (this Vector4 vec)
        {
            return new global::MonoMac.OpenGL.Vector4 (vec.X, vec.Y, vec.Z, vec.W);
        }

        public static Vector4 ToAbacus (this global::MonoMac.OpenGL.Vector4 vec)
        {
            return new Vector4 (vec.X, vec.Y, vec.Z, vec.W);
        }
    }

    public static class MatrixConverter
    {
        static bool flip = false;

        // MATRIX
        public static global::MonoMac.OpenGL.Matrix4 ToOpenGL (this Matrix44 mat)
        {
            if( flip )
            {
                return new global::MonoMac.OpenGL.Matrix4(
                    mat.M11, mat.M21, mat.M31, mat.M41,
                    mat.M12, mat.M22, mat.M32, mat.M42,
                    mat.M13, mat.M23, mat.M33, mat.M43,
                    mat.M14, mat.M24, mat.M34, mat.M44
                    );
            }
            else
            {
                return new global::MonoMac.OpenGL.Matrix4(
                    mat.M11, mat.M12, mat.M13, mat.M14,
                    mat.M21, mat.M22, mat.M23, mat.M24,
                    mat.M31, mat.M32, mat.M33, mat.M34,
                    mat.M41, mat.M42, mat.M43, mat.M44
                    );
            }
        }

        public static Matrix44 ToAbacus (this global::MonoMac.OpenGL.Matrix4 mat)
        {

            if( flip )
            {
                return new Matrix44(
                    mat.M11, mat.M21, mat.M31, mat.M41,
                    mat.M12, mat.M22, mat.M32, mat.M42,
                    mat.M13, mat.M23, mat.M33, mat.M43,
                    mat.M14, mat.M24, mat.M34, mat.M44
                    );
            }
            else
            {
                return new Matrix44(
                    mat.M11, mat.M12, mat.M13, mat.M14,
                    mat.M21, mat.M22, mat.M23, mat.M24,
                    mat.M31, mat.M32, mat.M33, mat.M34,
                    mat.M41, mat.M42, mat.M43, mat.M44
                    );
            }
        }

    }


    public static class EnumConverter
    {
        public static global::MonoMac.OpenGL.TextureUnit ToOpenGLTextureSlot(Int32 slot)
        {
            switch(slot)
            {
                case 0: return global::MonoMac.OpenGL.TextureUnit.Texture0;
                case 1: return global::MonoMac.OpenGL.TextureUnit.Texture1;
                case 2: return global::MonoMac.OpenGL.TextureUnit.Texture2;
                case 3: return  global::MonoMac.OpenGL.TextureUnit.Texture3;
                case 4: return global::MonoMac.OpenGL.TextureUnit.Texture4;
                case 5: return global::MonoMac.OpenGL.TextureUnit.Texture5;
                case 6: return global::MonoMac.OpenGL.TextureUnit.Texture6;
                case 7: return global::MonoMac.OpenGL.TextureUnit.Texture7;
                case 8: return global::MonoMac.OpenGL.TextureUnit.Texture8;
                case 9: return global::MonoMac.OpenGL.TextureUnit.Texture9;
                case 10: return global::MonoMac.OpenGL.TextureUnit.Texture10;
                case 11: return global::MonoMac.OpenGL.TextureUnit.Texture11;
                case 12: return global::MonoMac.OpenGL.TextureUnit.Texture12;
                case 13: return global::MonoMac.OpenGL.TextureUnit.Texture13;
                case 14: return global::MonoMac.OpenGL.TextureUnit.Texture14;
                case 15: return global::MonoMac.OpenGL.TextureUnit.Texture15;
                case 16: return global::MonoMac.OpenGL.TextureUnit.Texture16;
                case 17: return global::MonoMac.OpenGL.TextureUnit.Texture17;
                case 18: return global::MonoMac.OpenGL.TextureUnit.Texture18;
                case 19: return global::MonoMac.OpenGL.TextureUnit.Texture19;
                case 20: return global::MonoMac.OpenGL.TextureUnit.Texture20;
                case 21: return global::MonoMac.OpenGL.TextureUnit.Texture21;
                case 22: return global::MonoMac.OpenGL.TextureUnit.Texture22;
                case 23: return global::MonoMac.OpenGL.TextureUnit.Texture23;
                case 24: return global::MonoMac.OpenGL.TextureUnit.Texture24;
                case 25: return global::MonoMac.OpenGL.TextureUnit.Texture25;
                case 26: return global::MonoMac.OpenGL.TextureUnit.Texture26;
                case 27: return global::MonoMac.OpenGL.TextureUnit.Texture27;
                case 28: return global::MonoMac.OpenGL.TextureUnit.Texture28;
                case 29: return global::MonoMac.OpenGL.TextureUnit.Texture29;
                case 30: return global::MonoMac.OpenGL.TextureUnit.Texture30;
            }

            throw new NotSupportedException();
        }


        public static Type ToType (global::MonoMac.OpenGL.ActiveAttribType ogl)
        {
            switch(ogl)
            {
            case global::MonoMac.OpenGL.ActiveAttribType.Float: return typeof(Single);
            case global::MonoMac.OpenGL.ActiveAttribType.FloatMat2: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveAttribType.FloatMat3: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveAttribType.FloatMat4: return typeof(Matrix44);
            case global::MonoMac.OpenGL.ActiveAttribType.FloatVec2: return typeof(Vector2);
            case global::MonoMac.OpenGL.ActiveAttribType.FloatVec3: return typeof(Vector3);
            case global::MonoMac.OpenGL.ActiveAttribType.FloatVec4: return typeof(Vector4);
            }

            throw new NotSupportedException();
        }

        public static Type ToType (global::MonoMac.OpenGL.ActiveUniformType ogl)
        {
            switch(ogl)
            {
            case global::MonoMac.OpenGL.ActiveUniformType.Bool: return typeof(Boolean);
            case global::MonoMac.OpenGL.ActiveUniformType.BoolVec2: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.BoolVec3: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.BoolVec4: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.Float: return typeof(Single);
            case global::MonoMac.OpenGL.ActiveUniformType.FloatMat2: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.FloatMat3: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.FloatMat4: return typeof(Matrix44);
            case global::MonoMac.OpenGL.ActiveUniformType.FloatVec2: return typeof(Vector2);
            case global::MonoMac.OpenGL.ActiveUniformType.FloatVec3: return typeof(Vector3);
            case global::MonoMac.OpenGL.ActiveUniformType.FloatVec4: return typeof(Vector4);
            case global::MonoMac.OpenGL.ActiveUniformType.Int: return typeof(Boolean);
            case global::MonoMac.OpenGL.ActiveUniformType.IntVec2: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.IntVec3: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.IntVec4: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.Sampler2D: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.SamplerCube: throw new NotSupportedException();
            }
            
            throw new NotSupportedException();
        }

        public static void ToOpenGL (
            VertexElementFormat blimey,
            out global::MonoMac.OpenGL.VertexAttribPointerType dataFormat,
            out bool normalized,
            out int size)
        {
            normalized = false;
            size = 0;
            dataFormat = global::MonoMac.OpenGL.VertexAttribPointerType.Float;

            switch(blimey)
            {
                case VertexElementFormat.Single: 
                dataFormat = global::MonoMac.OpenGL.VertexAttribPointerType.Float;
                    size = 1;
                    break;
                case VertexElementFormat.Vector2: 
                dataFormat = global::MonoMac.OpenGL.VertexAttribPointerType.Float; 
                    size = 2;
                    break;
                case VertexElementFormat.Vector3: 
                dataFormat = global::MonoMac.OpenGL.VertexAttribPointerType.Float; 
                    size = 3;
                    break;
                case VertexElementFormat.Vector4: 
                dataFormat = global::MonoMac.OpenGL.VertexAttribPointerType.Float; 
                    size = 4;
                    break;
                case VertexElementFormat.Colour: 
                dataFormat = global::MonoMac.OpenGL.VertexAttribPointerType.UnsignedByte; 
                    normalized = true;
                    size = 4;
                    break;
                case VertexElementFormat.Byte4: throw new Exception("?");
                case VertexElementFormat.Short2: throw new Exception("?");
                case VertexElementFormat.Short4: throw new Exception("?");
                case VertexElementFormat.NormalisedShort2: throw new Exception("?");
                case VertexElementFormat.NormalisedShort4: throw new Exception("?");
                case VertexElementFormat.HalfVector2: throw new Exception("?");
                case VertexElementFormat.HalfVector4: throw new Exception("?");
            }
        }

        public static global::MonoMac.OpenGL.BlendingFactorSrc ToOpenGLSrc(BlendFactor blimey)
        {
            switch(blimey)
            {
                case BlendFactor.Zero: return global::MonoMac.OpenGL.BlendingFactorSrc.Zero;
                case BlendFactor.One: return global::MonoMac.OpenGL.BlendingFactorSrc.One;
                case BlendFactor.SourceColour: return global::MonoMac.OpenGL.BlendingFactorSrc.Src1Color; // todo: check this src1 stuff
                case BlendFactor.InverseSourceColour: return global::MonoMac.OpenGL.BlendingFactorSrc.OneMinusSrc1Color;
                case BlendFactor.SourceAlpha: return global::MonoMac.OpenGL.BlendingFactorSrc.SrcAlpha;
                case BlendFactor.InverseSourceAlpha: return global::MonoMac.OpenGL.BlendingFactorSrc.OneMinusSrcAlpha;
                case BlendFactor.DestinationAlpha: return global::MonoMac.OpenGL.BlendingFactorSrc.DstAlpha;
                case BlendFactor.InverseDestinationAlpha: return global::MonoMac.OpenGL.BlendingFactorSrc.OneMinusDstAlpha;
                case BlendFactor.DestinationColour: return global::MonoMac.OpenGL.BlendingFactorSrc.DstColor;
                case BlendFactor.InverseDestinationColour: return global::MonoMac.OpenGL.BlendingFactorSrc.OneMinusDstColor;
            }

            throw new Exception();
        }

        public static global::MonoMac.OpenGL.BlendingFactorDest ToOpenGLDest(BlendFactor blimey)
        {
            switch(blimey)
            {
                case BlendFactor.Zero: return global::MonoMac.OpenGL.BlendingFactorDest.Zero;
                case BlendFactor.One: return global::MonoMac.OpenGL.BlendingFactorDest.One;
                case BlendFactor.SourceColour: return global::MonoMac.OpenGL.BlendingFactorDest.SrcColor;
                case BlendFactor.InverseSourceColour: return global::MonoMac.OpenGL.BlendingFactorDest.OneMinusSrcColor;
                case BlendFactor.SourceAlpha: return global::MonoMac.OpenGL.BlendingFactorDest.SrcAlpha;
                case BlendFactor.InverseSourceAlpha: return global::MonoMac.OpenGL.BlendingFactorDest.OneMinusSrcAlpha;
                case BlendFactor.DestinationAlpha: return global::MonoMac.OpenGL.BlendingFactorDest.DstAlpha;
                case BlendFactor.InverseDestinationAlpha: return global::MonoMac.OpenGL.BlendingFactorDest.OneMinusDstAlpha;
                case BlendFactor.DestinationColour: return global::MonoMac.OpenGL.BlendingFactorDest.SrcColor;
                case BlendFactor.InverseDestinationColour: return global::MonoMac.OpenGL.BlendingFactorDest.OneMinusSrcColor;
            }
            
            throw new Exception();
        }

        public static BlendFactor ToCorDestinationBlendFactor (global::MonoMac.OpenGL.All ogl)
        {
            switch(ogl)
            {
                case global::MonoMac.OpenGL.All.Zero: return BlendFactor.Zero;
                case global::MonoMac.OpenGL.All.One: return BlendFactor.One;
                case global::MonoMac.OpenGL.All.SrcColor: return BlendFactor.SourceColour;
                case global::MonoMac.OpenGL.All.OneMinusSrcColor: return BlendFactor.InverseSourceColour;
                case global::MonoMac.OpenGL.All.SrcAlpha: return BlendFactor.SourceAlpha;
                case global::MonoMac.OpenGL.All.OneMinusSrcAlpha: return BlendFactor.InverseSourceAlpha;
                case global::MonoMac.OpenGL.All.DstAlpha: return BlendFactor.DestinationAlpha;
                case global::MonoMac.OpenGL.All.OneMinusDstAlpha: return BlendFactor.InverseDestinationAlpha;
                case global::MonoMac.OpenGL.All.DstColor: return BlendFactor.DestinationColour;
                case global::MonoMac.OpenGL.All.OneMinusDstColor: return BlendFactor.InverseDestinationColour;
            }

            throw new Exception();
        }

        public static global::MonoMac.OpenGL.BlendEquationMode ToOpenGL(BlendFunction blimey)
        {
            switch(blimey)
            {
                case BlendFunction.Add: return global::MonoMac.OpenGL.BlendEquationMode.FuncAdd;
                case BlendFunction.Max: throw new NotSupportedException();
                case BlendFunction.Min: throw new NotSupportedException();
                case BlendFunction.ReverseSubtract: return global::MonoMac.OpenGL.BlendEquationMode.FuncReverseSubtract;
                case BlendFunction.Subtract: return global::MonoMac.OpenGL.BlendEquationMode.FuncSubtract;
            }
            
            throw new Exception();
        }

        public static BlendFunction ToCorDestinationBlendFunction (global::MonoMac.OpenGL.All ogl)
        {
            switch(ogl)
            {
                case global::MonoMac.OpenGL.All.FuncAdd: return BlendFunction.Add;
                case global::MonoMac.OpenGL.All.MaxExt: return BlendFunction.Max;
                case global::MonoMac.OpenGL.All.MinExt: return BlendFunction.Min;
                case global::MonoMac.OpenGL.All.FuncReverseSubtract: return BlendFunction.ReverseSubtract;
                case global::MonoMac.OpenGL.All.FuncSubtract: return BlendFunction.Subtract;
            }
            
            throw new Exception();
        }

        // PRIMITIVE TYPE
        public static global::MonoMac.OpenGL.BeginMode ToOpenGL (PrimitiveType blimey)
        {
            switch (blimey) {
            case PrimitiveType.LineList:
                return  global::MonoMac.OpenGL.BeginMode.Lines;
            case PrimitiveType.LineStrip:
                return  global::MonoMac.OpenGL.BeginMode.LineStrip;
            case PrimitiveType.TriangleList:
                return  global::MonoMac.OpenGL.BeginMode.Triangles;
            case PrimitiveType.TriangleStrip:
                return  global::MonoMac.OpenGL.BeginMode.TriangleStrip;
                    
            default:
                throw new Exception ("problem");
            }
        }

        public static PrimitiveType ToCorPrimitiveType (global::MonoMac.OpenGL.All ogl)
        {
            switch (ogl) {
            case global::MonoMac.OpenGL.All.Lines:
                return  PrimitiveType.LineList;
            case global::MonoMac.OpenGL.All.LineStrip:
                return  PrimitiveType.LineStrip;
            case global::MonoMac.OpenGL.All.Points:
                throw new Exception ("Not supported by Cor");
            case global::MonoMac.OpenGL.All.TriangleFan:
                throw new Exception ("Not supported by Cor");
            case global::MonoMac.OpenGL.All.Triangles:
                return  PrimitiveType.TriangleList;
            case global::MonoMac.OpenGL.All.TriangleStrip:
                return  PrimitiveType.TriangleStrip;
                
            default:
                throw new Exception ("problem");

            }
        }
    }

    public class Scene
    {

        float rtri; // Angle For The Triangle ( NEW )
        float rquad;    // Angle For The Quad     ( NEW )

        public Scene () : base()
        {
        }

        // Resize And Initialize The GL Window 
        //      - See also the method in the MyOpenGLView Constructor about the NSView.NSViewGlobalFrameDidChangeNotification
        public void ResizeGLScene (RectangleF bounds)
        {
            // Reset The Current Viewport
            global::MonoMac.OpenGL.GL.Viewport (0, 0, (int)bounds.Size.Width, (int)bounds.Size.Height);
            // Select The Projection Matrix
            global::MonoMac.OpenGL.GL.MatrixMode (global::MonoMac.OpenGL.MatrixMode.Projection);
            // Reset The Projection Matrix
            global::MonoMac.OpenGL.GL.LoadIdentity ();

            // Set perspective here - Calculate The Aspect Ratio Of The Window
            Perspective (45, bounds.Size.Width / bounds.Size.Height, 0.1, 100);

            // Select The Modelview Matrix
            global::MonoMac.OpenGL.GL.MatrixMode (global::MonoMac.OpenGL.MatrixMode.Modelview);
            // Reset The Modelview Matrix
            global::MonoMac.OpenGL.GL.LoadIdentity ();
        }

        // This creates a symmetric frustum.
        // It converts to 6 params (l, r, b, t, n, f) for glFrustum()
        // from given 4 params (fovy, aspect, near, far)
        public static void Perspective (double fovY, double aspectRatio, double front, double back)
        {
            const
            double DEG2RAD = Math.PI / 180 ; 

            // tangent of half fovY
            double tangent = Math.Tan (fovY / 2 * DEG2RAD);

            // half height of near plane
            double height = front * tangent;

            // half width of near plane
            double width = height * aspectRatio;

            // params: left, right, bottom, top, near, far
            global::MonoMac.OpenGL.GL.Frustum (-width, width, -height, height, front, back);
        }

        // This method renders our scene.
        // The main thing to note is that we've factored the drawing code out of the NSView subclass so that
        // the full-screen and non-fullscreen views share the same states for rendering 
        public bool DrawGLScene ()
        {
            // Clear The Screen And The Depth Buffer
            global::MonoMac.OpenGL.GL.Clear (global::MonoMac.OpenGL.ClearBufferMask.ColorBufferBit | global::MonoMac.OpenGL.ClearBufferMask.DepthBufferBit);
            // Reset The Current Modelview Matrix
            global::MonoMac.OpenGL.GL.LoadIdentity ();

            global::MonoMac.OpenGL.GL.Translate (-1.5f, 0.0f, -6.0f);
            // Move Left 1.5 Units And Into The Screen 6.0
            global::MonoMac.OpenGL.GL.Rotate (rtri, 0.0f, 1.0f, 0.0f);
            // Rotate The Triangle On The Y axis
            global::MonoMac.OpenGL.GL.Begin (global::MonoMac.OpenGL.BeginMode.Triangles);     // Start drawing the Pyramid

            global::MonoMac.OpenGL.GL.Color3 (1.0f, 0.0f, 0.0f);           // Red
            global::MonoMac.OpenGL.GL.Vertex3 (0.0f, 1.0f, 0.0f);          // Top Of Triangle (Front)
            global::MonoMac.OpenGL.GL.Color3 (0.0f, 1.0f, 0.0f);           // Green
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, -1.0f, 1.0f);            // Left Of Triangle (Front)
            global::MonoMac.OpenGL.GL.Color3 (0.0f, 0.0f, 1.0f);           // Blue
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, -1.0f, 1.0f);         // Right Of Triangle (Front)

            global::MonoMac.OpenGL.GL.Color3 (1.0f, 0.0f, 0.0f);           // Red
            global::MonoMac.OpenGL.GL.Vertex3 (0.0f, 1.0f, 0.0f);          // Top Of Triangle (Right)
            global::MonoMac.OpenGL.GL.Color3 (0.0f, 0.0f, 1.0f);           // Blue
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, -1.0f, 1.0f);         // Left Of Triangle (Right)
            global::MonoMac.OpenGL.GL.Color3 (0.0f, 1.0f, 0.0f);           // Green
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, -1.0f, -1.0f);            // Right Of Triangle (Right)

            global::MonoMac.OpenGL.GL.Color3 (1.0f, 0.0f, 0.0f);           // Red
            global::MonoMac.OpenGL.GL.Vertex3 (0.0f, 1.0f, 0.0f);          // Top Of Triangle (Back)
            global::MonoMac.OpenGL.GL.Color3 (0.0f, 1.0f, 0.0f);           // Green
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, -1.0f, -1.0f);            // Left Of Triangle (Back)
            global::MonoMac.OpenGL.GL.Color3 (0.0f, 0.0f, 1.0f);           // Blue
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, -1.0f, -1.0f);           // Right Of Triangle (Back)         

            global::MonoMac.OpenGL.GL.Color3 (1.0f, 0.0f, 0.0f);           // Red
            global::MonoMac.OpenGL.GL.Vertex3 (0.0f, 1.0f, 0.0f);          // Top Of Triangle (Left)
            global::MonoMac.OpenGL.GL.Color3 (0.0f, 0.0f, 1.0f);           // Blue
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, -1.0f, -1.0f);           // Left Of Triangle (Left)
            global::MonoMac.OpenGL.GL.Color3 (0.0f, 1.0f, 0.0f);           // Green
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, -1.0f, 1.0f);            // Right Of Triangle (Left)

            global::MonoMac.OpenGL.GL.End ();                      // Finished Drawing The Pyramid

            // Reset The Current Modelview Matrix
            global::MonoMac.OpenGL.GL.LoadIdentity ();

            global::MonoMac.OpenGL.GL.Translate (1.5f, 0.0f, -7.0f);           // Move Right 1.5 Units And Into The Screen 7.0
            global::MonoMac.OpenGL.GL.Rotate (rquad, 1.0f, 0.0f, 0.0f);            // Rotate The Quad On The X axis ( NEW )   
            global::MonoMac.OpenGL.GL.Begin (global::MonoMac.OpenGL.BeginMode.Quads);             // Start Drawing Cube
            global::MonoMac.OpenGL.GL.Color3 (0.0f, 1.0f, 0.0f);           // Set The Color To Green
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, 1.0f, -1.0f);         // Top Right Of The Quad (Top)
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, 1.0f, -1.0f);        // Top Left Of The Quad (Top)
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, 1.0f, 1.0f);         // Bottom Left Of The Quad (Top)
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, 1.0f, 1.0f);          // Bottom Right Of The Quad (Top)                        

            global::MonoMac.OpenGL.GL.Color3 (1.0f, 0.5f, 0.0f);           // Set The Color To Orange
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, -1.0f, 1.0f);         // Top Right Of The Quad (Bottom)
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, -1.0f, 1.0f);        // Top Left Of The Quad (Bottom)
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, -1.0f, -1.0f);       // Bottom Left Of The Quad (Bottom)
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, -1.0f, -1.0f);        // Bottom Right Of The Quad (Bottom)

            global::MonoMac.OpenGL.GL.Color3 (1.0f, 0.0f, 0.0f);           // Set The Color To Red
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, 1.0f, 1.0f);          // Top Right Of The Quad (Front)
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, 1.0f, 1.0f);         // Top Left Of The Quad (Front)
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, -1.0f, 1.0f);            // Bottom Left Of The Quad (Front)
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, -1.0f, 1.0f);         // Bottom Right Of The Quad (Front) 

            global::MonoMac.OpenGL.GL.Color3 (1.0f, 1.0f, 0.0f);           // Set The Color To Yellow
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, -1.0f, -1.0f);        // Bottom Left Of The Quad (Back)
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, -1.0f, -1.0f);       // Bottom Right Of The Quad (Back)
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, 1.0f, -1.0f);        // Top Right Of The Quad (Back)
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, 1.0f, -1.0f);     // Top Left Of The Quad (Back)

            global::MonoMac.OpenGL.GL.Color3 (0.0f, 0.0f, 1.0f);           // Set The Color To Blue
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, 1.0f, 1.0f);         // Top Right Of The Quad (Left)
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, 1.0f, -1.0f);            // Top Left Of The Quad (Left)
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, -1.0f, -1.0f);           // Bottom Left Of The Quad (Left)
            global::MonoMac.OpenGL.GL.Vertex3 (-1.0f, -1.0f, 1.0f);            // Bottom Right Of The Quad (Left)

            global::MonoMac.OpenGL.GL.Color3 (1.0f, 0.0f, 1.0f);           // Set The Color To Violet
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, 1.0f, -1.0f);         // Top Right Of The Quad (Right)
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, 1.0f, 1.0f);          // Top Left Of The Quad (Right)
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, -1.0f, 1.0f);         // Bottom Left Of The Quad (Right)
            global::MonoMac.OpenGL.GL.Vertex3 (1.0f, -1.0f, -1.0f);            // Bottom Right Of The Quad (Right)         

            global::MonoMac.OpenGL.GL.End ();              // Done Drawing the Cube

            rtri += 0.2f;               // Increase The Rotation Variable For The Triangle
            rquad -= 0.15f;             // Decrease The Rotation Variable For The Quad 
            return true;
        }

    }}