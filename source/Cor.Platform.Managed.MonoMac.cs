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
            
        }

        public void ClearColourBuffer(Rgba32 color = new Rgba32())
        {
            
        }

        public void ClearDepthBuffer(Single depth = 1f)
        {
            
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