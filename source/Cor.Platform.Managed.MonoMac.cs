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
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

using Sungiant.Abacus;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Abacus.Int32Precision;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreVideo;
using MonoMac.CoreGraphics;
using MonoMac.CoreImage;
using MonoMac.ImageIO;
using MonoMac.ImageKit;

namespace Sungiant.Cor.Platform.Managed.MonoMac
{
    public class Engine
        : ICor
    {
        readonly IAudioManager audio;
        readonly IGraphicsManager graphics;
        readonly IResourceManager resources;
        readonly IInputManager input;
        readonly ISystemManager system;
        readonly AppSettings settings;
        readonly IApp app;

        public Engine(
            AppSettings settings,
            IApp app)
        {
            Console.WriteLine(
                "Engine -> ()");
            
            this.audio = new AudioManager();
            this.graphics = new GraphicsManager();
            this.resources = new ResourceManager();
            this.input = new InputManager();
            this.system = new SystemManager();
            this.settings = settings;
            this.app = app;
            this.app.Initilise(this);
        }

        #region ICor

        public IAudioManager Audio { get { return this.audio; } }

        public IGraphicsManager Graphics { get { return this.graphics; } }

        public IResourceManager Resources { get { return this.resources; } }

        public IInputManager Input { get { return this.input; } }

        public ISystemManager System { get { return this.system; } }

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
        readonly IDisplayStatus displayStatus;
        readonly IGpuUtils gpuUtils;

        GeometryBuffer currentGeomBuffer;
        CullMode? currentCullMode;

        public GraphicsManager()
        {
            Console.WriteLine(
                "GraphicsManager -> ()");

            this.displayStatus = new DisplayStatus();
            this.gpuUtils = new MonoMacGpuUtils();

            global::MonoMac.OpenGL.GL.Enable(global::MonoMac.OpenGL.EnableCap.Blend);
            OpenGLHelper.CheckError();

            this.SetBlendEquation(
                BlendFunction.Add, BlendFactor.SourceAlpha, BlendFactor.InverseSourceAlpha,
                BlendFunction.Add, BlendFactor.One, BlendFactor.InverseSourceAlpha);

            global::MonoMac.OpenGL.GL.Enable(global::MonoMac.OpenGL.EnableCap.DepthTest);
            OpenGLHelper.CheckError();

            global::MonoMac.OpenGL.GL.DepthMask(true);
            OpenGLHelper.CheckError();

            global::MonoMac.OpenGL.GL.DepthRange(0f, 1f);
            OpenGLHelper.CheckError();

            global::MonoMac.OpenGL.GL.DepthFunc(global::MonoMac.OpenGL.DepthFunction.Lequal);
            OpenGLHelper.CheckError();

            SetCullMode (CullMode.CW);
        }

        [ReliabilityContract (Consistency.MayCorruptInstance, Cer.MayFail)]
        static IntPtr Add (IntPtr pointer, int offset)
        {
            unsafe
            {
                return (IntPtr) (unchecked (((byte *) pointer) + offset));
            }
        }

        [ReliabilityContract (Consistency.MayCorruptInstance, Cer.MayFail)]
        static IntPtr Subtract (IntPtr pointer, int offset)
        {
            unsafe
            {
                return (IntPtr) (unchecked (((byte *) pointer) - offset));
            }
        }

        void DisableVertAttribs(VertexDeclaration vertDecl)
        {
            var vertElems = vertDecl.GetVertexElements();

            for(int i = 0; i < vertElems.Length; ++i)
            {
                global::MonoMac.OpenGL.GL.DisableVertexAttribArray(i);
                OpenGLHelper.CheckError();
            }
        }


        #region IGraphicsManager

        public IDisplayStatus DisplayStatus { get { return this.displayStatus; } }

        public IGpuUtils GpuUtils { get { return this.gpuUtils; } }

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
            if (!currentCullMode.HasValue || currentCullMode.Value != cullMode)
            {
                if (cullMode == CullMode.None)
                {
                    global::MonoMac.OpenGL.GL.Disable (global::MonoMac.OpenGL.EnableCap.CullFace);
                    OpenGLHelper.CheckError ();

                }
                else
                {
                    global::MonoMac.OpenGL.GL.Enable(global::MonoMac.OpenGL.EnableCap.CullFace);
                    OpenGLHelper.CheckError();

                    global::MonoMac.OpenGL.GL.FrontFace(global::MonoMac.OpenGL.FrontFaceDirection.Cw);
                    OpenGLHelper.CheckError();

                    if (cullMode == CullMode.CW)
                    {
                        global::MonoMac.OpenGL.GL.CullFace (global::MonoMac.OpenGL.CullFaceMode.Back);
                        OpenGLHelper.CheckError ();
                    }
                    else if (cullMode == CullMode.CCW)
                    {
                        global::MonoMac.OpenGL.GL.CullFace (global::MonoMac.OpenGL.CullFaceMode.Front);
                        OpenGLHelper.CheckError ();
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }

                currentCullMode = cullMode;
            }
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
            var temp = buffer as GeometryBuffer;

            if( temp != this.currentGeomBuffer )
            {
                if( this.currentGeomBuffer != null )
                {
                    this.currentGeomBuffer.Deactivate();

                    this.currentGeomBuffer = null;
                }

                if( temp != null )
                {
                    temp.Activate();
                }
                
                this.currentGeomBuffer = temp;
            }
        }

        public void SetActiveTexture(Int32 slot, Texture2D tex)
        {
            global::MonoMac.OpenGL.TextureUnit oglTexSlot = EnumConverter.ToOpenGLTextureSlot(slot); 
            global::MonoMac.OpenGL.GL.ActiveTexture(oglTexSlot);

            var oglt0 = tex as OpenGLTexture;
            
            if( oglt0 != null )
            {
                var textureTarget = global::MonoMac.OpenGL.TextureTarget.Texture2D;
                
                // we need to bind the texture object so that we can opperate on it.
                global::MonoMac.OpenGL.GL.BindTexture(textureTarget, oglt0.glTextureId);
                OpenGLHelper.CheckError();
            }

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
    }    public sealed class IndexBuffer
        : IIndexBuffer
        , IDisposable
    {
        static Int32 resourceCounter;

        Int32 indexCount;
        global::MonoMac.OpenGL.BufferTarget type;
        UInt32 bufferHandle;
        global::MonoMac.OpenGL.BufferUsageHint bufferUsage;

        public IndexBuffer (Int32 indexCount)
        {
            this.indexCount = indexCount;

            this.type = global::MonoMac.OpenGL.BufferTarget.ElementArrayBuffer;

            this.bufferUsage = global::MonoMac.OpenGL.BufferUsageHint.DynamicDraw;

            global::MonoMac.OpenGL.GL.GenBuffers(1, out this.bufferHandle);
            
            OpenGLHelper.CheckError();

            if( this.bufferHandle == 0 )
            {
                throw new Exception("Failed to generate vert buffer.");
            }

            this.Activate();

            global::MonoMac.OpenGL.GL.BufferData(
                this.type,
                (System.IntPtr) (sizeof(UInt16) * this.indexCount),
                (System.IntPtr) null,
                this.bufferUsage);

            OpenGLHelper.CheckError();

            resourceCounter++;

        }

        ~IndexBuffer()
        {
            CleanUpNativeResources();
        }

        void CleanUpManagedResources()
        {

        }

        void CleanUpNativeResources()
        {
            global::MonoMac.OpenGL.GL.DeleteBuffers(1, ref this.bufferHandle);
            OpenGLHelper.CheckError();

            bufferHandle = 0;

            resourceCounter--;
        }

        public void Dispose()
        {
            CleanUpManagedResources();
            CleanUpNativeResources();
            GC.SuppressFinalize(this);
        }

        internal void Activate()
        {
            global::MonoMac.OpenGL.GL.BindBuffer(this.type, this.bufferHandle);
            OpenGLHelper.CheckError();
        }

        internal void Deactivate()
        {
            global::MonoMac.OpenGL.GL.BindBuffer(this.type, 0);
            OpenGLHelper.CheckError();
        }


        public void SetData (Int32[] data)
        {

            if( data.Length != indexCount )
            {
                throw new Exception("?");
            }

            UInt16[] udata = new UInt16[data.Length];

            for(Int32 i = 0; i < data.Length; ++i)
            {
                udata[i] = (UInt16) data[i];
            }
            
            this.Activate();

            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            global::MonoMac.OpenGL.GL.BufferSubData(
                this.type,
                (System.IntPtr) 0,
                (System.IntPtr) (sizeof(UInt16) * this.indexCount),
                udata);

            udata = null;

            OpenGLHelper.CheckError();
        }

        public int IndexCount
        {
            get
            {
                return indexCount;
            }
        }

        public void GetData(Int32[] data)
        {
            throw new NotImplementedException();    
        }

        public void GetData(Int16[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();    
        }

        public void GetData(Int32 offsetInBytes, Int16[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();    
        }

        public void SetData(Int16[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();    
        }

        public void SetData(Int32 offsetInBytes, Int16[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();    
        }

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
        readonly IShader tempStubShader = new StubShader();
        
        public ResourceManager()
        {
            Console.WriteLine(
                "ResourceManager -> ()");
        }

        #region IResourceManager

        public T Load<T>(String path) where T : IResource
        {
            if(!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            if(typeof(T) == typeof(Texture2D))
            {
                var tex = OpenGLTexture.CreateFromFile(path);
                
                return (T)(IResource) tex;
            }
            
            throw new NotImplementedException();
        }

        public IShader LoadShader(ShaderType shaderType)
        {
            return tempStubShader;
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
        IndexBuffer _iBuf;
        VertexBuffer _vBuf;
        
        public GeometryBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount)
        {

            if(vertexCount == 0)
            {
                throw new Exception("A geometry buffer must have verts");
            }

            if( indexCount != 0 )
            {
                _iBuf = new IndexBuffer(indexCount);
            }

            _vBuf = new VertexBuffer(vertexDeclaration, vertexCount);

        }

        internal void Activate()
        {
            _vBuf.Activate();

            if( _iBuf != null )
                _iBuf.Activate();
        }

        internal void Deactivate()
        {
            _vBuf.Deactivate();

            if( _iBuf != null )
                _iBuf.Deactivate();
        }


        
        public IVertexBuffer VertexBuffer { get { return _vBuf; } }
        public IIndexBuffer IndexBuffer { get { return _iBuf; } }

        internal VertexBuffer OpenTKVertexBuffer { get { return _vBuf; } }
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

    public sealed class VertexBuffer
        : IVertexBuffer
        , IDisposable
    {
        Int32 resourceCounter;
        VertexDeclaration vertDecl;

        Int32 vertexCount;

        UInt32 bufferHandle;

        global::MonoMac.OpenGL.BufferTarget type;
        global::MonoMac.OpenGL.BufferUsageHint bufferUsage;

        public VertexBuffer (VertexDeclaration vd, Int32 vertexCount)
        {
            this.vertDecl = vd;
            this.vertexCount = vertexCount;

            this.type = global::MonoMac.OpenGL.BufferTarget.ArrayBuffer;

            this.bufferUsage = global::MonoMac.OpenGL.BufferUsageHint.DynamicDraw;

            global::MonoMac.OpenGL.GL.GenBuffers(1, out this.bufferHandle);
            OpenGLHelper.CheckError();


            if( this.bufferHandle == 0 )
            {
                throw new Exception("Failed to generate vert buffer.");
            }
            

            this.Activate();

            global::MonoMac.OpenGL.GL.BufferData(
                this.type,
                (System.IntPtr) (vertDecl.VertexStride * this.vertexCount),
                (System.IntPtr) null,
                this.bufferUsage);

            OpenGLHelper.CheckError();

            resourceCounter++;

        }

        internal void Activate()
        {
            global::MonoMac.OpenGL.GL.BindBuffer(this.type, this.bufferHandle);
            OpenGLHelper.CheckError();
        }

        internal void Deactivate()
        {
            global::MonoMac.OpenGL.GL.BindBuffer(this.type, 0);
            OpenGLHelper.CheckError();
        }

        ~VertexBuffer()
        {
            CleanUpNativeResources();
        }

        void CleanUpManagedResources()
        {

        }

        void CleanUpNativeResources()
        {
            global::MonoMac.OpenGL.GL.DeleteBuffers(1, ref this.bufferHandle);
            OpenGLHelper.CheckError();

            bufferHandle = 0;

            resourceCounter--;
        }

        public void Dispose()
        {
            CleanUpManagedResources();
            CleanUpNativeResources();
            GC.SuppressFinalize(this);
        }

        public void SetData<T> (T[] data)
            where T: struct, IVertexType
        {
            if( data.Length != vertexCount )
            {
                throw new Exception("?");
            }
            
            this.Activate();

            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            global::MonoMac.OpenGL.GL.BufferSubData(
                this.type,
                (System.IntPtr) 0,
                (System.IntPtr) (vertDecl.VertexStride * this.vertexCount),
                data);

            OpenGLHelper.CheckError();
        }


        public Int32 VertexCount
        {
            get
            {
                return this.vertexCount;
            }
        }

        public VertexDeclaration VertexDeclaration 
        {
            get
            {
                return this.vertDecl;
            }
        } 

        public void GetData<T> (T[] data) where T: struct, IVertexType { throw new System.NotSupportedException(); }
        
        public void GetData<T> (T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType { throw new System.NotSupportedException(); }
        
        public void GetData<T> (Int32 offsetInBytes, T[] data, Int32 startIndex, Int32 elementCount, Int32 vertexStride) where T: struct, IVertexType { throw new System.NotSupportedException(); }
        
        public void SetData<T> (T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType { throw new System.NotSupportedException(); }
        
        public void SetData<T> (Int32 offsetInBytes, T[] data, Int32 startIndex, Int32 elementCount, Int32 vertexStride) where T: struct, IVertexType { throw new System.NotSupportedException(); }

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
        //Scene scene;

        Engine gameEngine;
        Stopwatch timer = new Stopwatch();
        Single elapsedTime;
        Int64 frameCounter = -1;
        TimeSpan previousTimeSpan;
        Int32 frameInterval;

        uint _depthRenderbuffer;

        readonly AppSettings settings;
        readonly IApp entryPoint;

        
        public OpenGLView(AppSettings settings, IApp entryPoint, RectangleF frame) 
            : base (frame)
        {
            this.settings = settings;
            this.entryPoint = entryPoint;

            this.AutoresizingMask = 
                global::MonoMac.AppKit.NSViewResizingMask.HeightSizable |
                global::MonoMac.AppKit.NSViewResizingMask.MaxXMargin |
                global::MonoMac.AppKit.NSViewResizingMask.MinYMargin |
                global::MonoMac.AppKit.NSViewResizingMask.WidthSizable;

            RectangleF rect = NSScreen.MainScreen.Frame;
            clientBounds = new Rectangle (0,0,(int)rect.Width,(int)rect.Height);


            
            Resize += delegate {
                //scene.ResizeGLScene(Bounds);    
            };
            
            Load += OnLoad;
            
            UpdateFrame += delegate(object src, global::MonoMac.OpenGL.FrameEventArgs fea) {

                Single dt = (Single)(timer.Elapsed.TotalSeconds - previousTimeSpan.TotalSeconds);
                previousTimeSpan = timer.Elapsed;
                
                if (dt > 0.5f)
                {
                    dt = 0.0f;
                }

                elapsedTime += dt;

                var appTime = new AppTime(dt, elapsedTime, ++frameCounter);

                gameEngine.Update(appTime);   
            };
            
            RenderFrame += delegate(object src, global::MonoMac.OpenGL.FrameEventArgs fea) {

                gameEngine.Render();
            };
            
        }
        
        void OnLoad (object src, EventArgs fea)
        {
            //CreateFrameBuffer();
            //scene = new Scene();

            gameEngine = new Engine(
                this.settings,
                this.entryPoint
                //this, 
                //this.GraphicsContext, 
                //this.touchState
                );
            timer.Start();

            //Console.WriteLine("load ");   
            //InitGL();
            //UpdateView();
            
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

        void CreateFrameBuffer()
        {
            //
            // Enable the depth buffer
            //
            global::MonoMac.OpenGL.GL.GenRenderbuffers(1, out _depthRenderbuffer);
            OpenGLHelper.CheckError();

            global::MonoMac.OpenGL.GL.BindRenderbuffer(
                global::MonoMac.OpenGL.RenderbufferTarget.Renderbuffer, 
                _depthRenderbuffer);
            OpenGLHelper.CheckError();

            global::MonoMac.OpenGL.GL.RenderbufferStorage(
                global::MonoMac.OpenGL.RenderbufferTarget.Renderbuffer, 
                global::MonoMac.OpenGL.RenderbufferStorage.DepthComponent16, 
                Size.Width, Size.Height);
            OpenGLHelper.CheckError();

            global::MonoMac.OpenGL.GL.FramebufferRenderbuffer(
                global::MonoMac.OpenGL.FramebufferTarget.Framebuffer,
                global::MonoMac.OpenGL.FramebufferAttachment.DepthAttachment,
                global::MonoMac.OpenGL.RenderbufferTarget.Renderbuffer,
                _depthRenderbuffer);
            OpenGLHelper.CheckError();

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

    public class MonoMacGpuUtils
        : IGpuUtils
    {
        public MonoMacGpuUtils()
        {
        }

        #region IGpuUtils

        public Int32 BeginEvent(Rgba32 colour, String eventName)
        {
            return 0;
        }

        public Int32 EndEvent()
        {
            return 0;
        }

        public void SetMarker(Rgba32 colour, String eventName)
        {

        }

        public void SetRegion(Rgba32 colour, String eventName)
        {

        }

        #endregion
    }
    public class StubShader
        : IShader
    {
        IShaderPass[] passes = new IShaderPass[0];
        VertexElementUsage[] requiredVertexElements = new VertexElementUsage[0];
        VertexElementUsage[] optionalVertexElements = new VertexElementUsage[0];

        #region IShader

        public void ResetVariables()
        {
            
        }

        public void ResetSamplerTargets()
        {
            
        }

        public void SetSamplerTarget(String name, Int32 textureSlot)
        {

        }

        public IShaderPass[] Passes { get { return passes; } }

        public VertexElementUsage[] RequiredVertexElements { get { return requiredVertexElements; } }

        public VertexElementUsage[] OptionalVertexElements { get { return optionalVertexElements; } }

        public String Name { get { return "StubShader"; } }

        public void SetVariable<T>(String name, T value)
        {

        }

        #endregion
    }

    internal class OpenGLTexture
        : Texture2D
    {
        public int glTextureId {get; private set;}

        NSImage nsImage;

        int pixelsWide;
        int pixelsHigh;  

        internal static OpenGLTexture CreateFromFile(string path)
        {   
            using(var fStream = new FileStream(path, FileMode.Open))
            {
                var nsImage = NSImage.FromStream( fStream );
    
                var texture = new OpenGLTexture(nsImage);
    
                return texture;
            }
        }

        private OpenGLTexture(NSImage nsImage)
        {
            this.nsImage = nsImage;
            IntPtr dataPointer = RequestImagePixelData(nsImage);

            CreateTexture2D((int)nsImage.Size.Width, (int)nsImage.Size.Height, dataPointer);
        }


        //Store pixel data as an ARGB Bitmap
        IntPtr RequestImagePixelData (NSImage inImage)
        {
            var imageSize = inImage.Size;
            
            CGBitmapContext ctxt = CreateRgbaBitmapContext (inImage.CGImage);
            
            var rect = new RectangleF (0, 0, imageSize.Width, imageSize.Height);
            
            ctxt.DrawImage (rect, inImage.CGImage);
            var data = ctxt.Data;
            
            return data;
        }

        CGBitmapContext CreateRgbaBitmapContext (CGImage inImage)
        {
            pixelsWide = inImage.Width;
            pixelsHigh = inImage.Height;

            using (var colorSpace = CGColorSpace.CreateDeviceRGB())
            {
                var bitmapBytesPerRow = pixelsWide * 4;
                var bitmapByteCount = bitmapBytesPerRow * pixelsHigh;
                var bitmapData = Marshal.AllocHGlobal (bitmapByteCount);

                if (bitmapData == IntPtr.Zero)
                {
                    throw new Exception ("Memory not allocated.");
                }
                
                var context = new CGBitmapContext (
                    bitmapData, 
                    pixelsWide, 
                    pixelsHigh, 
                    8,
                    bitmapBytesPerRow, 
                    colorSpace, 
                    CGImageAlphaInfo.PremultipliedLast);

                if (context == null)
                {
                    throw new Exception ("Context not created");
                }

                return context;
            }
        }
        


        public override int Width
        {
            get
            {
                return pixelsWide;
            }
        }
        public override int Height
        {
            get
            {
                return pixelsHigh;
            }
        }

        
        void CreateTexture2D(int width, int height, IntPtr pixelDataRgba32)
        {
            int textureId = -1;
            
            
            // this sets the unpack alignment.  which is used when reading pixels
            // in the fragment shader.  when the textue data is uploaded via glTexImage2d,
            // the rows of pixels are assumed to be aligned to the value set for GL_UNPACK_ALIGNMENT.
            // By default, the value is 4, meaning that rows of pixels are assumed to begin
            // on 4-byte boundaries.  this is a global STATE.
            global::MonoMac.OpenGL.GL.PixelStore(global::MonoMac.OpenGL.PixelStoreParameter.UnpackAlignment, 4);
            OpenGLHelper.CheckError();

            // the first sept in the application of texture is to create the
            // texture object.  this is a container object that holds the 
            // texture data.  this function returns a handle to a texture
            // object.
            global::MonoMac.OpenGL.GL.GenTextures(1, out textureId);
            OpenGLHelper.CheckError();

            this.glTextureId = textureId;

            var textureTarget = global::MonoMac.OpenGL.TextureTarget.Texture2D;            
            
            // we need to bind the texture object so that we can opperate on it.
            global::MonoMac.OpenGL.GL.BindTexture(textureTarget, textureId);
            OpenGLHelper.CheckError();

            var internalFormat = global::MonoMac.OpenGL.PixelInternalFormat.Rgba;
            var format = global::MonoMac.OpenGL.PixelFormat.Rgba;
            
            var textureDataFormat = global::MonoMac.OpenGL.PixelType.UnsignedByte;
            
            // now use the bound texture object to load the image data.
            global::MonoMac.OpenGL.GL.TexImage2D(
                
                // specifies the texture target, either GL_TEXTURE_2D or one of the cubemap face targets.
                textureTarget,
                
                // specifies which mip level to load.  the base level is
                // specified by 0 following by an increasing level for each
                // successive mipmap.
                0,
                
                // internal format for the texture storage, can be:
                // - GL_RGBA
                // - GL_RGB
                // - GL_LUMINANCE_ALPHA
                // - GL_LUMINANCE
                // - GL_ALPHA
                internalFormat,
                
                // the width of the image in pixels
                width,
                
                // the height of the image in pixels
                height,
                
                // boarder - set to zero, only here for compatibility with OpenGL desktop
                0,
                
                // the format of the incoming texture data, in opengl es this 
                // has to be the same as the internal format
                format,
                
                // the type of the incoming pixel data, can be:
                // - unsigned byte
                // - unsigned short 4444
                // - unsigned short 5551
                // - unsigned short 565
                textureDataFormat, // this refers to each individual channel
                
                
                pixelDataRgba32
                
                );

            OpenGLHelper.CheckError();

            // sets the minification and maginfication filtering modes.  required
            // because we have not loaded a complete mipmap chain for the texture
            // so we must select a non mipmapped minification filter.
            global::MonoMac.OpenGL.GL.TexParameter(textureTarget, global::MonoMac.OpenGL.TextureParameterName.TextureMinFilter, (int) global::MonoMac.OpenGL.All.Nearest );

            OpenGLHelper.CheckError();

            global::MonoMac.OpenGL.GL.TexParameter(textureTarget, global::MonoMac.OpenGL.TextureParameterName.TextureMagFilter, (int) global::MonoMac.OpenGL.All.Nearest );

            OpenGLHelper.CheckError();
        }
        
        
        
        void DeleteTexture(Texture2D texture)
        {
            int textureId = (texture as OpenGLTexture).glTextureId;
            
            global::MonoMac.OpenGL.GL.DeleteTextures(1, ref textureId);
        }
    }

}
